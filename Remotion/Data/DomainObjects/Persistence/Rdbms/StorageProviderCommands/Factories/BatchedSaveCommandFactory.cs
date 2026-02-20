// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;

/// <summary>
/// The <see cref="BatchedSaveCommandFactory"/> is responsible to create batched save commands for a relational database.
/// </summary>
public class BatchedSaveCommandFactory : ISaveCommandFactory
{
  private class DataContainerGroup
  {
    public List<DataContainer> ForInsert { get; } = new();
    public List<(DataContainer DataContainer, ColumnValue[] UpdatedColumnValues)> ForUpdate { get; } = new();
    public List<DataContainer> ForDelete { get; } = new();
  }

  private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
  private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
  private readonly ITableDefinitionFinder _tableDefinitionFinder;
  private readonly ITableManipulationRecordDefinitionProvider _tableManipulationRecordDefinitionProvider;

  public BatchedSaveCommandFactory (
      IDbCommandBuilderFactory dbCommandBuilderFactory,
      IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider,
      ITableDefinitionFinder tableDefinitionFinder,
      ITableManipulationRecordDefinitionProvider tableManipulationRecordDefinitionProvider)
  {
    ArgumentNullException.ThrowIfNull(dbCommandBuilderFactory);
    ArgumentNullException.ThrowIfNull(rdbmsPersistenceModelProvider);
    ArgumentNullException.ThrowIfNull(tableDefinitionFinder);
    ArgumentNullException.ThrowIfNull(tableManipulationRecordDefinitionProvider);

    _dbCommandBuilderFactory = dbCommandBuilderFactory;
    _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
    _tableDefinitionFinder = tableDefinitionFinder;
    _tableManipulationRecordDefinitionProvider = tableManipulationRecordDefinitionProvider;
  }

  public IDbCommandBuilderFactory DbCommandBuilderFactory => _dbCommandBuilderFactory;

  public IRdbmsPersistenceModelProvider RdbmsPersistenceModelProvider => _rdbmsPersistenceModelProvider;

  public ITableDefinitionFinder TableDefinitionFinder => _tableDefinitionFinder;

  public virtual IRdbmsProviderCommand CreateForSave (IEnumerable<DataContainer> dataContainers)
  {
    ArgumentNullException.ThrowIfNull(dataContainers);

    return new BatchedDataContainerSaveCommand(CreateSaveCommandContextsForSave(dataContainers));
  }

  protected virtual bool ShouldCreateInsertCommand (DataContainer dataContainer)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);

    return dataContainer.State.IsNew;
  }

  protected virtual bool ShouldCreateDeleteCommand (DataContainer dataContainer)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);

    return dataContainer.State.IsDeleted;
  }

  protected virtual IEnumerable<ColumnValue> GetComparedColumnValuesForUpdate (DataContainer dataContainer, TableDefinition tableDefinition)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);
    ArgumentNullException.ThrowIfNull(tableDefinition);

    var objectIDColumnValues = tableDefinition.ObjectIDProperty.SplitValueForComparison(dataContainer.ID);
    if (dataContainer.State.IsNew)
      return objectIDColumnValues;

    return objectIDColumnValues.Concat(tableDefinition.TimestampProperty.SplitValueForComparison(dataContainer.Timestamp));
  }

  protected virtual IEnumerable<ColumnValue> GetComparedColumnValuesForDelete (DataContainer dataContainer, TableDefinition tableDefinition)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);
    ArgumentNullException.ThrowIfNull(tableDefinition);

    var objectIDColumnValues = tableDefinition.ObjectIDProperty.SplitValueForComparison(dataContainer.ID);
    // If a DataContainer contains a relation property, an Update previous to the Delete will already have checked the timestamp.
    // Otherwise (no relation properties), the Delete must check the timestamp.
    var mustAddTimestamp = dataContainer.ClassDefinition.GetPropertyDefinitions().All(pd => !pd.IsObjectID);
    if (mustAddTimestamp)
      return objectIDColumnValues.Concat(tableDefinition.TimestampProperty.SplitValueForComparison(dataContainer.Timestamp));

    return objectIDColumnValues;
  }

  protected virtual ColumnValue[] GetUpdatedColumnValues (DataContainer dataContainer, TableDefinition tableDefinition)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);
    ArgumentNullException.ThrowIfNull(tableDefinition);

    var propertyFilter = GetUpdatedPropertyFilter(dataContainer);

    var dataStorageColumnValues = dataContainer.ClassDefinition.GetPropertyDefinitions()
        .Where(pd => pd.StorageClass == StorageClass.Persistent && propertyFilter(pd))
        .SelectMany(pd => GetColumnValuesForPropertyValue(dataContainer, pd))
        .ToArray();

    if (dataStorageColumnValues.Length == 0 && dataContainer.HasBeenMarkedChanged)
    {
      // If the data container has no changed properties, but must still be saved (to update its timestamp), update the ClassID
      return tableDefinition.ObjectIDProperty.ClassIDProperty.SplitValue(dataContainer.ID.ClassID).ToArray();
    }

    return dataStorageColumnValues;
  }

  protected virtual IEnumerable<ColumnValue> GetInsertedColumnValues (DataContainer dataContainer, TableDefinition tableDefinition)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);
    ArgumentNullException.ThrowIfNull(tableDefinition);

    var objectIDStoragePropertyDefinition = ((IRdbmsStorageEntityDefinition)tableDefinition).ObjectIDProperty;
    var columnValuesForID = objectIDStoragePropertyDefinition.SplitValue(dataContainer.ID);

    var columnValuesForDataProperties = dataContainer.ClassDefinition.GetPropertyDefinitions()
        .Where(pd => pd.StorageClass == StorageClass.Persistent && !pd.IsObjectID)
        .SelectMany(pd => GetColumnValuesForPropertyValue(dataContainer, pd));
    return columnValuesForID.Concat(columnValuesForDataProperties);
  }

  protected virtual Func<PropertyDefinition, bool> GetUpdatedPropertyFilter (DataContainer dataContainer)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);

    if (dataContainer.State.IsNew || dataContainer.State.IsDeleted)
      return pd => pd.IsObjectID;
    if (dataContainer.State.IsChanged)
      return dataContainer.HasValueChanged;
    return _ => false;
  }

  protected virtual IEnumerable<ColumnValue> GetColumnValuesForPropertyValue (DataContainer dataContainer, PropertyDefinition propertyDefinition)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);
    ArgumentNullException.ThrowIfNull(propertyDefinition);

    var storageProperty = _rdbmsPersistenceModelProvider.GetStoragePropertyDefinition(propertyDefinition);
    var columnValues = storageProperty.SplitValue(dataContainer.GetValueWithoutEvents(propertyDefinition));
    return columnValues;
  }

  private IEnumerable<SingleObjectMultiDataContainerSaveCommandContext> CreateSaveCommandContextsForSave (IEnumerable<DataContainer> dataContainers)
  {
    var groupedDataContainers = GetGroupedDataContainer(dataContainers);

    var inserts = new List<SingleObjectMultiDataContainerSaveCommandContext>();
    var updates = new List<SingleObjectMultiDataContainerSaveCommandContext>();
    var deletes = new List<SingleObjectMultiDataContainerSaveCommandContext>();

    foreach (var kvp in groupedDataContainers)
    {
      var tableDefinition = kvp.Key;

      inserts.AddRange(CreateCommandContextsForInsert(tableDefinition, kvp.Value.ForInsert));
      updates.AddRange(CreateCommandContextsForUpdate(tableDefinition, kvp.Value.ForUpdate));
      deletes.AddRange(CreateCommandContextsForDelete(tableDefinition, kvp.Value.ForDelete));
    }

    return inserts.Concat(updates).Concat(deletes);
  }

  private IDictionary<TableDefinition, DataContainerGroup> GetGroupedDataContainer (IEnumerable<DataContainer> dataContainers)
  {
    var group = new Dictionary<TableDefinition, DataContainerGroup>();

    foreach (var dataContainer in dataContainers)
    {
      var tableDefinition = _tableDefinitionFinder.GetTableDefinition(dataContainer.ID);
      if (!group.TryGetValue(tableDefinition, out var currentGroup))
      {
        currentGroup = new DataContainerGroup();
        group.Add(tableDefinition, currentGroup);
      }

      if (ShouldCreateInsertCommand(dataContainer))
        currentGroup.ForInsert.Add(dataContainer);
      else if (ShouldCreateDeleteCommand(dataContainer))
        currentGroup.ForDelete.Add(dataContainer);

      var updatedColumnValues = GetUpdatedColumnValues(dataContainer, tableDefinition);
      if (updatedColumnValues.Length > 0)
        currentGroup.ForUpdate.Add(new(dataContainer, updatedColumnValues));
    }

    return group;
  }

  private IEnumerable<SingleObjectMultiDataContainerSaveCommandContext> CreateCommandContextsForInsert (TableDefinition tableDefinition, List<DataContainer> dataContainers)
  {
    foreach (var dataContainer in dataContainers)
    {
      var columnValues = GetInsertedColumnValues(dataContainer, tableDefinition);
      var commandBuilder = _dbCommandBuilderFactory.CreateForInsert(tableDefinition, columnValues);

      yield return new SingleObjectMultiDataContainerSaveCommandContext(dataContainer.ID, commandBuilder);
    }
  }

  private IEnumerable<SingleObjectMultiDataContainerSaveCommandContext> CreateCommandContextsForDelete (TableDefinition tableDefinition, List<DataContainer> dataContainers)
  {
    foreach (var dataContainer in dataContainers)
    {
      var columnValues = GetComparedColumnValuesForDelete(dataContainer, tableDefinition);
      var commandBuilder = _dbCommandBuilderFactory.CreateForDelete(tableDefinition, columnValues);

      yield return new SingleObjectMultiDataContainerSaveCommandContext(dataContainer.ID, commandBuilder);
    }
  }

  private IEnumerable<SingleObjectMultiDataContainerSaveCommandContext> CreateCommandContextsForUpdate (TableDefinition tableDefinition, List<(DataContainer DataContainer, ColumnValue[] UpdatedColumnValues)> updateInfos)
  {
    foreach (var info in updateInfos)
    {
      var comparedColumnValues = GetComparedColumnValuesForUpdate(info.DataContainer, tableDefinition);
      var commandBuilder = _dbCommandBuilderFactory.CreateForUpdate(tableDefinition, info.UpdatedColumnValues, comparedColumnValues);
      yield return new SingleObjectMultiDataContainerSaveCommandContext(info.DataContainer.ID, commandBuilder);
    }
  }
}
