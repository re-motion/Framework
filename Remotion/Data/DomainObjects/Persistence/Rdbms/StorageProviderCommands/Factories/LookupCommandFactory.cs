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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.StorageProviderCommands;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories
{
  /// <summary>
  /// The <see cref="LookupCommandFactory"/> is responsible for creating lookup commands for a relational database.
  /// </summary>
  public class LookupCommandFactory
  {
    private readonly StorageProviderDefinition _storageProviderDefinition;
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
    private readonly IObjectReaderFactory _objectReaderFactory;
    private readonly ITableDefinitionFinder _tableDefinitionFinder;

    public LookupCommandFactory (
        StorageProviderDefinition storageProviderDefinition,
        IDbCommandBuilderFactory dbCommandBuilderFactory,
        IObjectReaderFactory objectReaderFactory,
        ITableDefinitionFinder tableDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("dbCommandBuilderFactory", dbCommandBuilderFactory);
      ArgumentUtility.CheckNotNull("objectReaderFactory", objectReaderFactory);
      ArgumentUtility.CheckNotNull("tableDefinitionFinder", tableDefinitionFinder);

      _storageProviderDefinition = storageProviderDefinition;
      _dbCommandBuilderFactory = dbCommandBuilderFactory;
      _objectReaderFactory = objectReaderFactory;
      _tableDefinitionFinder = tableDefinitionFinder;
    }

    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    public IDbCommandBuilderFactory DbCommandBuilderFactory
    {
      get { return _dbCommandBuilderFactory; }
    }

    public IObjectReaderFactory ObjectReaderFactory
    {
      get { return _objectReaderFactory; }
    }

    public ITableDefinitionFinder TableDefinitionFinder
    {
      get { return _tableDefinitionFinder; }
    }

    public virtual IRdbmsProviderCommand<ObjectLookupResult<DataContainer>> CreateForSingleIDLookup (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      var tableDefinition = _tableDefinitionFinder.GetTableDefinition(objectID);
      var selectedColumns = tableDefinition.GetAllColumns().ToArray();
      var dataContainerReader = _objectReaderFactory.CreateDataContainerReader(tableDefinition, selectedColumns);
      var comparedColumns = tableDefinition.ObjectIDProperty.SplitValueForComparison(objectID);
      var dbCommandBuilder = _dbCommandBuilderFactory.CreateForSelect(tableDefinition, selectedColumns, comparedColumns, Array.Empty<OrderedColumn>());

      var loadCommand = new SingleObjectLoadCommand<DataContainer?>(dbCommandBuilder, dataContainerReader);
      return new SingleDataContainerAssociateWithIDCommand(objectID, loadCommand);
    }

    public virtual IRdbmsProviderCommand<IEnumerable<ObjectLookupResult<DataContainer>>> CreateForSortedMultiIDLookup (
        IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

      var objectIDList = objectIDs as IReadOnlyCollection<ObjectID> ?? objectIDs.ToList();
      var dbCommandBuildersAndReaders =
          from id in objectIDList
          let tableDefinition = _tableDefinitionFinder.GetTableDefinition(id)
          group id by tableDefinition
          into idsByTable
          let selectedColumns = idsByTable.Key.GetAllColumns().ToArray()
          let dataContainerReader = _objectReaderFactory.CreateDataContainerReader(idsByTable.Key, selectedColumns)
          let dbCommandBuilder = CreateIDLookupDbCommandBuilder(idsByTable.Key, selectedColumns, idsByTable)
          select Tuple.Create(dbCommandBuilder, dataContainerReader);

      var loadCommand = new MultiObjectLoadCommand<DataContainer?>(dbCommandBuildersAndReaders);
      return new MultiDataContainerAssociateWithIDsCommand(objectIDList, loadCommand);
    }

    public virtual IRdbmsProviderCommand<IEnumerable<ObjectLookupResult<object>>> CreateForMultiTimestampLookup (
        IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

      var dbCommandBuildersAndReaders =
          from id in objectIDs
          let tableDefinition = _tableDefinitionFinder.GetTableDefinition(id)
          group id by tableDefinition
          into idsByTable
          let selectedColumns = idsByTable.Key.ObjectIDProperty.GetColumns().Concat(idsByTable.Key.TimestampProperty.GetColumns()).ToArray()
          let timestampReader = _objectReaderFactory.CreateTimestampReader(idsByTable.Key, selectedColumns)
          let dbCommandBuilder = CreateIDLookupDbCommandBuilder(idsByTable.Key, selectedColumns, idsByTable)
          select Tuple.Create(dbCommandBuilder, timestampReader);

      var loadCommand = new MultiObjectLoadCommand<Tuple<ObjectID, object>?>(dbCommandBuildersAndReaders);
      return DelegateBasedCommand.Create(
          loadCommand,
          lookupResults => lookupResults.Select(
              result =>
              {
                Assertion.IsNotNull<Tuple<ObjectID, object>?>(
                    result,
                    "Because no OUTER JOIN query is involved in retrieving the result, the DataContainer can never be null.");
                Assertion.IsNotNull<ObjectID>(
                    result.Item1,
                    "Because we included IDColumn into the projection and used it for the lookup, every row in the result set certainly has an ID.");
                return new ObjectLookupResult<object>(result.Item1, result.Item2);
              }));
    }

    protected virtual IDbCommandBuilder CreateIDLookupDbCommandBuilder (
        TableDefinition tableDefinition,
        IEnumerable<ColumnDefinition> selectedColumns,
        IEnumerable<ObjectID> objectIDs)
    {
      var checkedCastObjectIDs = objectIDs.Select(id =>
      {
        if (id.StorageProviderDefinition != _storageProviderDefinition)
          throw new NotSupportedException("Multi-ID lookups can only be performed for ObjectIDs from this storage provider.");
        return (object)id;
      }).ToList();

      if (checkedCastObjectIDs.Count == 1)
      {
        var columnValues = tableDefinition.ObjectIDProperty.SplitValueForComparison(checkedCastObjectIDs[0]);
        return _dbCommandBuilderFactory.CreateForSelect(tableDefinition, selectedColumns, columnValues, Enumerable.Empty<OrderedColumn>());
      }

      var comparedColumnValueTable = tableDefinition.ObjectIDProperty.SplitValuesForComparison(checkedCastObjectIDs);
      return _dbCommandBuilderFactory.CreateForSelect(tableDefinition, selectedColumns, comparedColumnValueTable, Enumerable.Empty<OrderedColumn>());
    }
  }
}
