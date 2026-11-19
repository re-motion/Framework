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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// Creates <see cref="IRdbmsProviderCommand"/> instances for use with <see cref="RdbmsProvider"/>.
  /// </summary>
  public class RdbmsProviderCommandFactory : IRdbmsProviderCommandFactory
  {
    private readonly RdbmsProviderDefinition _storageProviderDefinition;
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
    private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
    private readonly IObjectReaderFactory _objectReaderFactory;
    private readonly ITableDefinitionFinder _tableDefinitionFinder;
    private readonly IDataStoragePropertyDefinitionFactory _dataStoragePropertyDefinitionFactory;

    private readonly LookupCommandFactory _lookupCommandFactory;
    private readonly RelationLookupCommandFactory _relationLookupCommandFactory;
    private readonly ISaveCommandFactory _saveCommandFactory;
    private readonly QueryCommandFactory _queryCommandFactory;
    private readonly IDataParameterDefinitionFactory _dataParameterDefinitionFactory;

    public RdbmsProviderCommandFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IDbCommandBuilderFactory dbCommandBuilderFactory,
        IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider,
        IObjectReaderFactory objectReaderFactory,
        ITableDefinitionFinder tableDefinitionFinder,
        IDataStoragePropertyDefinitionFactory dataStoragePropertyDefinitionFactory,
        IDataParameterDefinitionFactory dataParameterDefinitionFactory,
        ITableManipulationRecordDefinitionProvider tableManipulationRecordDefinitionProvider)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(dbCommandBuilderFactory);
      ArgumentNullException.ThrowIfNull(rdbmsPersistenceModelProvider);
      ArgumentNullException.ThrowIfNull(objectReaderFactory);
      ArgumentNullException.ThrowIfNull(tableDefinitionFinder);
      ArgumentNullException.ThrowIfNull(dataStoragePropertyDefinitionFactory);
      ArgumentNullException.ThrowIfNull(dataParameterDefinitionFactory);
      ArgumentNullException.ThrowIfNull(tableManipulationRecordDefinitionProvider);

      _storageProviderDefinition = storageProviderDefinition;
      _dbCommandBuilderFactory = dbCommandBuilderFactory;
      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
      _objectReaderFactory = objectReaderFactory;
      _tableDefinitionFinder = tableDefinitionFinder;
      _dataStoragePropertyDefinitionFactory = dataStoragePropertyDefinitionFactory;
      _dataParameterDefinitionFactory = dataParameterDefinitionFactory;
      TableManipulationRecordDefinitionProvider = tableManipulationRecordDefinitionProvider;

      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      _lookupCommandFactory = CreateLookupCommandFactory();
      _relationLookupCommandFactory = CreateRelationLookupCommandFactory();
      _saveCommandFactory = CreateSaveCommandFactory();
      _queryCommandFactory = CreateQueryCommandFactory();
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
    }

    public RdbmsProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    public IDbCommandBuilderFactory DbCommandBuilderFactory
    {
      get { return _dbCommandBuilderFactory; }
    }

    public ITableManipulationRecordDefinitionProvider TableManipulationRecordDefinitionProvider { get; }

    public IRdbmsPersistenceModelProvider RdbmsPersistenceModelProvider
    {
      get { return _rdbmsPersistenceModelProvider; }
    }

    public IObjectReaderFactory ObjectReaderFactory
    {
      get { return _objectReaderFactory; }
    }

    public ITableDefinitionFinder TableDefinitionFinder
    {
      get { return _tableDefinitionFinder; }
    }

    public IDataStoragePropertyDefinitionFactory DataStoragePropertyDefinitionFactory
    {
      get { return _dataStoragePropertyDefinitionFactory; }
    }

    public LookupCommandFactory LookupCommandFactory
    {
      get { return _lookupCommandFactory; }
    }

    public RelationLookupCommandFactory RelationLookupCommandFactory
    {
      get { return _relationLookupCommandFactory; }
    }

    public ISaveCommandFactory SaveCommandFactory
    {
      get { return _saveCommandFactory; }
    }

    public QueryCommandFactory QueryCommandFactory
    {
      get { return _queryCommandFactory; }
    }

    public IRdbmsProviderCommandWithReadOnlySupport<ObjectLookupResult<DataContainer>> CreateForSingleIDLookup (ObjectID objectID)
    {
      ArgumentNullException.ThrowIfNull(objectID);

      return _lookupCommandFactory.CreateForSingleIDLookup(objectID);
    }

    public IRdbmsProviderCommandWithReadOnlySupport<IEnumerable<ObjectLookupResult<DataContainer>>> CreateForSortedMultiIDLookup (IEnumerable<ObjectID> objectIDs)
    {
      ArgumentNullException.ThrowIfNull(objectIDs);

      return _lookupCommandFactory.CreateForSortedMultiIDLookup(objectIDs);
    }

    public IRdbmsProviderCommandWithReadOnlySupport<IEnumerable<DataContainer>> CreateForRelationLookup (
        RelationEndPointDefinition foreignKeyEndPoint,
        ObjectID foreignKeyValue,
        SortExpressionDefinition? sortExpressionDefinition)
    {
      ArgumentNullException.ThrowIfNull(foreignKeyEndPoint);
      ArgumentNullException.ThrowIfNull(foreignKeyValue);

      return _relationLookupCommandFactory.CreateForRelationLookup(foreignKeyEndPoint, foreignKeyValue, sortExpressionDefinition);
    }

    public IRdbmsProviderCommandWithReadOnlySupport<IEnumerable<DataContainer?>> CreateForDataContainerQuery (IQuery query)
    {
      ArgumentNullException.ThrowIfNull(query);
      return _queryCommandFactory.CreateForDataContainerQuery(query);
    }

    public IRdbmsProviderCommandWithReadOnlySupport<IEnumerable<IQueryResultRow>> CreateForCustomQuery (IQuery query)
    {
      ArgumentNullException.ThrowIfNull(query);
      return _queryCommandFactory.CreateForCustomQuery(query);
    }

    public IRdbmsProviderCommandWithReadOnlySupport<object?> CreateForScalarQuery (IQuery query)
    {
      ArgumentNullException.ThrowIfNull(query);
      return _queryCommandFactory.CreateForScalarQuery(query);
    }

    public IRdbmsProviderCommandWithReadOnlySupport<IEnumerable<ObjectLookupResult<object>>> CreateForMultiTimestampLookup (IEnumerable<ObjectID> objectIDs)
    {
      ArgumentNullException.ThrowIfNull(objectIDs);

      return _lookupCommandFactory.CreateForMultiTimestampLookup(objectIDs);
    }

    public IRdbmsProviderCommand CreateForSave (IEnumerable<DataContainer> dataContainers)
    {
      ArgumentNullException.ThrowIfNull(dataContainers);

      return _saveCommandFactory.CreateForSave(dataContainers);
    }

    protected virtual LookupCommandFactory CreateLookupCommandFactory ()
    {
      return new LookupCommandFactory(_storageProviderDefinition, _dbCommandBuilderFactory, _objectReaderFactory, _tableDefinitionFinder);
    }

    protected virtual RelationLookupCommandFactory CreateRelationLookupCommandFactory ()
    {
      return new RelationLookupCommandFactory(this, _dbCommandBuilderFactory, _rdbmsPersistenceModelProvider, _objectReaderFactory);
    }

    protected virtual ISaveCommandFactory CreateSaveCommandFactory ()
    {
      return new BatchedSaveCommandFactory(_dbCommandBuilderFactory, _rdbmsPersistenceModelProvider, _tableDefinitionFinder, TableManipulationRecordDefinitionProvider);
    }

    protected virtual QueryCommandFactory CreateQueryCommandFactory ()
    {
      return new QueryCommandFactory(_objectReaderFactory, _dbCommandBuilderFactory, _dataStoragePropertyDefinitionFactory, _dataParameterDefinitionFactory);
    }
  }
}
