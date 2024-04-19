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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// Creates <see cref="IStorageProviderCommand{TExecutionContext}"/> instances for use with <see cref="RdbmsProvider"/>.
  /// </summary>
  public class RdbmsProviderCommandFactory : IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>
  {
    private readonly RdbmsProviderDefinition _storageProviderDefinition;
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
    private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
    private readonly IObjectReaderFactory _objectReaderFactory;
    private readonly ITableDefinitionFinder _tableDefinitionFinder;
    private readonly IDataStoragePropertyDefinitionFactory _dataStoragePropertyDefinitionFactory;

    private readonly LookupCommandFactory _lookupCommandFactory;
    private readonly RelationLookupCommandFactory _relationLookupCommandFactory;
    private readonly SaveCommandFactory _saveCommandFactory;
    private readonly QueryCommandFactory _queryCommandFactory;
    private readonly IDataParameterDefinitionFactory _dataParameterDefinitionFactory;

    public RdbmsProviderCommandFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IDbCommandBuilderFactory dbCommandBuilderFactory,
        IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider,
        IObjectReaderFactory objectReaderFactory,
        ITableDefinitionFinder tableDefinitionFinder,
        IDataStoragePropertyDefinitionFactory dataStoragePropertyDefinitionFactory,
        IDataParameterDefinitionFactory dataParameterDefinitionFactory)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("dbCommandBuilderFactory", dbCommandBuilderFactory);
      ArgumentUtility.CheckNotNull("rdbmsPersistenceModelProvider", rdbmsPersistenceModelProvider);
      ArgumentUtility.CheckNotNull("objectReaderFactory", objectReaderFactory);
      ArgumentUtility.CheckNotNull("tableDefinitionFinder", tableDefinitionFinder);
      ArgumentUtility.CheckNotNull("dataStoragePropertyDefinitionFactory", dataStoragePropertyDefinitionFactory);
      ArgumentUtility.CheckNotNull("dataParameterDefinitionFactory", dataParameterDefinitionFactory);

      _storageProviderDefinition = storageProviderDefinition;
      _dbCommandBuilderFactory = dbCommandBuilderFactory;
      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
      _objectReaderFactory = objectReaderFactory;
      _tableDefinitionFinder = tableDefinitionFinder;
      _dataStoragePropertyDefinitionFactory = dataStoragePropertyDefinitionFactory;
      _dataParameterDefinitionFactory = dataParameterDefinitionFactory;

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

    public SaveCommandFactory SaveCommandFactory
    {
      get { return _saveCommandFactory; }
    }

    public QueryCommandFactory QueryCommandFactory
    {
      get { return _queryCommandFactory; }
    }

    public IStorageProviderCommand<ObjectLookupResult<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForSingleIDLookup (
        ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      return _lookupCommandFactory.CreateForSingleIDLookup(objectID);
    }

    public IStorageProviderCommand<IEnumerable<ObjectLookupResult<DataContainer>>, IRdbmsProviderCommandExecutionContext> CreateForSortedMultiIDLookup (
        IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

      return _lookupCommandFactory.CreateForSortedMultiIDLookup(objectIDs);
    }

    public IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForRelationLookup (
        RelationEndPointDefinition foreignKeyEndPoint, ObjectID foreignKeyValue, SortExpressionDefinition? sortExpressionDefinition)
    {
      ArgumentUtility.CheckNotNull("foreignKeyEndPoint", foreignKeyEndPoint);
      ArgumentUtility.CheckNotNull("foreignKeyValue", foreignKeyValue);

      return _relationLookupCommandFactory.CreateForRelationLookup(foreignKeyEndPoint, foreignKeyValue, sortExpressionDefinition);
    }

    public IStorageProviderCommand<IEnumerable<DataContainer?>, IRdbmsProviderCommandExecutionContext> CreateForDataContainerQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);
      return _queryCommandFactory.CreateForDataContainerQuery(query);
    }

    public IStorageProviderCommand<IEnumerable<IQueryResultRow>, IRdbmsProviderCommandExecutionContext> CreateForCustomQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);
      return _queryCommandFactory.CreateForCustomQuery(query);
    }

    public IStorageProviderCommand<object?, IRdbmsProviderCommandExecutionContext> CreateForScalarQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);
      return _queryCommandFactory.CreateForScalarQuery(query);
    }

    public IStorageProviderCommand<IEnumerable<ObjectLookupResult<object>>, IRdbmsProviderCommandExecutionContext> CreateForMultiTimestampLookup (
        IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

      return _lookupCommandFactory.CreateForMultiTimestampLookup(objectIDs);
    }

    public IStorageProviderCommand<IRdbmsProviderCommandExecutionContext> CreateForSave (IEnumerable<DataContainer> dataContainers)
    {
      ArgumentUtility.CheckNotNull("dataContainers", dataContainers);

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

    protected virtual SaveCommandFactory CreateSaveCommandFactory ()
    {
      return new SaveCommandFactory(_dbCommandBuilderFactory, _rdbmsPersistenceModelProvider, _tableDefinitionFinder);
    }

    protected virtual QueryCommandFactory CreateQueryCommandFactory ()
    {
      return new QueryCommandFactory(_objectReaderFactory, _dbCommandBuilderFactory, _dataStoragePropertyDefinitionFactory, _dataParameterDefinitionFactory);
    }
  }
}
