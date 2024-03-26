// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories
{
  /// <summary>
  /// The <see cref="SaveCommandFactory"/> is responsible to reate save commands for a relational database.
  /// </summary>
  public class SaveCommandFactory
  {
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
    private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
    private readonly ITableDefinitionFinder _tableDefinitionFinder;

    public SaveCommandFactory (
        IDbCommandBuilderFactory dbCommandBuilderFactory,
        IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider,
        ITableDefinitionFinder tableDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull("dbCommandBuilderFactory", dbCommandBuilderFactory);
      ArgumentUtility.CheckNotNull("rdbmsPersistenceModelProvider", rdbmsPersistenceModelProvider);
      ArgumentUtility.CheckNotNull("tableDefinitionFinder", tableDefinitionFinder);

      _dbCommandBuilderFactory = dbCommandBuilderFactory;
      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
      _tableDefinitionFinder = tableDefinitionFinder;
    }

    public IDbCommandBuilderFactory DbCommandBuilderFactory
    {
      get { return _dbCommandBuilderFactory; }
    }

    public IRdbmsPersistenceModelProvider RdbmsPersistenceModelProvider
    {
      get { return _rdbmsPersistenceModelProvider; }
    }

    public ITableDefinitionFinder TableDefinitionFinder
    {
      get { return _tableDefinitionFinder; }
    }

    public virtual IStorageProviderCommand CreateForSave (IEnumerable<DataContainer> dataContainers)
    {
      ArgumentUtility.CheckNotNull("dataContainers", dataContainers);

      return new MultiDataContainerSaveCommand(CreateDbCommandsForSave(dataContainers));
    }

    private IEnumerable<Tuple<ObjectID, IDbCommandBuilder>> CreateDbCommandsForSave (IEnumerable<DataContainer> dataContainers)
    {
      var insertCommands = new List<Tuple<ObjectID, IDbCommandBuilder>>();
      var updateCommands = new List<Tuple<ObjectID, IDbCommandBuilder>>();
      var deleteCommands = new List<Tuple<ObjectID, IDbCommandBuilder>>();

      foreach (var dataContainer in dataContainers)
      {
        var tableDefinition = _tableDefinitionFinder.GetTableDefinition(dataContainer.ID);

        if (ShouldCreateInsertCommand(dataContainer))
          insertCommands.Add(Tuple.Create(dataContainer.ID, CreateDbCommandForInsert(dataContainer, tableDefinition)));
        if (ShouldCreateDeleteCommand(dataContainer))
          deleteCommands.Add(Tuple.Create(dataContainer.ID, CreateDbCommandForDelete(dataContainer, tableDefinition)));

        var updatedColumnValues = GetUpdatedColumnValues(dataContainer, tableDefinition).ToArray();
        if (updatedColumnValues.Any())
        {
          var dbCommandForUpdate = CreateDbCommandForUpdate(dataContainer, tableDefinition, updatedColumnValues);
          updateCommands.Add(Tuple.Create(dataContainer.ID, dbCommandForUpdate));
        }
      }

      return insertCommands.Concat(updateCommands).Concat(deleteCommands);
    }

    protected virtual bool ShouldCreateInsertCommand (DataContainer dataContainer)
    {
      return dataContainer.State.IsNew;
    }

    protected virtual bool ShouldCreateDeleteCommand (DataContainer dataContainer)
    {
      return dataContainer.State.IsDeleted;
    }

    protected virtual IEnumerable<ColumnValue> GetComparedColumnValuesForUpdate (DataContainer dataContainer, TableDefinition tableDefinition)
    {
      var objectIDColumnValues = tableDefinition.ObjectIDProperty.SplitValueForComparison(dataContainer.ID);
      if (dataContainer.State.IsNew)
        return objectIDColumnValues;
      else
        return objectIDColumnValues.Concat(tableDefinition.TimestampProperty.SplitValueForComparison(dataContainer.Timestamp));
    }

    protected virtual IEnumerable<ColumnValue> GetComparedColumnValuesForDelete (DataContainer dataContainer, TableDefinition tableDefinition)
    {
      var objectIDColumnValues = tableDefinition.ObjectIDProperty.SplitValueForComparison(dataContainer.ID);
      // If a DataContainer contains a relation property, an Update previous to the Delete will already have checked the timestamp.
      // Otherwise (no relation properties), the Delete must check the timestamp.
      var mustAddTimestamp = dataContainer.ClassDefinition.GetPropertyDefinitions().All(pd => !pd.IsObjectID);
      if (mustAddTimestamp)
        return objectIDColumnValues.Concat(tableDefinition.TimestampProperty.SplitValueForComparison(dataContainer.Timestamp));
      else
        return objectIDColumnValues;
    }

    protected virtual IEnumerable<ColumnValue> GetUpdatedColumnValues (DataContainer dataContainer, TableDefinition tableDefinition)
    {
      var propertyFilter = GetUpdatedPropertyFilter(dataContainer);

      var dataStorageColumnValues = dataContainer.ClassDefinition.GetPropertyDefinitions()
          .Where(pd => pd.StorageClass == StorageClass.Persistent && propertyFilter(pd))
          .SelectMany(pd => GetColumnValuesForPropertyValue(dataContainer, pd))
          .ToArray();

      if (!dataStorageColumnValues.Any() && dataContainer.HasBeenMarkedChanged)
      {
        // If the data container has no changed properties, but must still be saved (to update its timestamp), update the ClassID
        return tableDefinition.ObjectIDProperty.ClassIDProperty.SplitValue(dataContainer.ID.ClassID);
      }

      return dataStorageColumnValues;
    }

    protected virtual IEnumerable<ColumnValue> GetInsertedColumnValues (DataContainer dataContainer, TableDefinition tableDefinition)
    {
      var objectIDStoragePropertyDefinition = (IRdbmsStoragePropertyDefinition)((IRdbmsStorageEntityDefinition)tableDefinition).ObjectIDProperty;
      var columnValuesForID = objectIDStoragePropertyDefinition.SplitValue(dataContainer.ID);

      var columnValuesForDataProperties = dataContainer.ClassDefinition.GetPropertyDefinitions()
          .Where(pd => pd.StorageClass == StorageClass.Persistent && !pd.IsObjectID)
          .SelectMany(pd => GetColumnValuesForPropertyValue(dataContainer, pd));
      return columnValuesForID.Concat(columnValuesForDataProperties);
    }

    protected virtual Func<PropertyDefinition, bool> GetUpdatedPropertyFilter (DataContainer dataContainer)
    {
      if (dataContainer.State.IsNew || dataContainer.State.IsDeleted)
        return pd => pd.IsObjectID;
      else if (dataContainer.State.IsChanged)
        return dataContainer.HasValueChanged;
      else
        return pd => false;
    }

    protected virtual IEnumerable<ColumnValue> GetColumnValuesForPropertyValue (DataContainer dataContainer, PropertyDefinition propertyDefinition)
    {
      var storageProperty = _rdbmsPersistenceModelProvider.GetStoragePropertyDefinition(propertyDefinition);
      var columnValues = storageProperty.SplitValue(dataContainer.GetValueWithoutEvents(propertyDefinition, ValueAccess.Current));
      return columnValues;
    }

    private IDbCommandBuilder CreateDbCommandForInsert (DataContainer dataContainer, TableDefinition tableDefinition)
    {
      var columnValues = GetInsertedColumnValues(dataContainer, tableDefinition);

      return _dbCommandBuilderFactory.CreateForInsert(tableDefinition, columnValues);
    }

    private IDbCommandBuilder CreateDbCommandForDelete (DataContainer dataContainer, TableDefinition tableDefinition)
    {
      var columnValues = GetComparedColumnValuesForDelete(dataContainer, tableDefinition);

      return _dbCommandBuilderFactory.CreateForDelete(tableDefinition, columnValues);
    }

    private IDbCommandBuilder CreateDbCommandForUpdate (
    DataContainer dataContainer,
    TableDefinition tableDefinition,
    IReadOnlyCollection<ColumnValue> updatedColumnValues)
    {
      var comparedColumnValues = GetComparedColumnValuesForUpdate(dataContainer, tableDefinition);

      return _dbCommandBuilderFactory.CreateForUpdate(tableDefinition, updatedColumnValues, comparedColumnValues);
    }
  }
}
