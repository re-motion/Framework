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
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories
{
  /// <summary>
  /// The <see cref="RelationLookupCommandFactory"/> is responsible for creating relation lookup commands for a relational database.
  /// </summary>
  public class RelationLookupCommandFactory
  {
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
    private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
    private readonly IObjectReaderFactory _objectReaderFactory;
    private readonly IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> _storageProviderCommandFactory;

    public RelationLookupCommandFactory (
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> storageProviderCommandFactory,
        IDbCommandBuilderFactory dbCommandBuilderFactory,
        IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider,
        IObjectReaderFactory objectReaderFactory)
    {
      ArgumentUtility.CheckNotNull("storageProviderCommandFactory", storageProviderCommandFactory);
      ArgumentUtility.CheckNotNull("dbCommandBuilderFactory", dbCommandBuilderFactory);
      ArgumentUtility.CheckNotNull("rdbmsPersistenceModelProvider", rdbmsPersistenceModelProvider);
      ArgumentUtility.CheckNotNull("objectReaderFactory", objectReaderFactory);

      _storageProviderCommandFactory = storageProviderCommandFactory;
      _dbCommandBuilderFactory = dbCommandBuilderFactory;
      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
      _objectReaderFactory = objectReaderFactory;
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

    public IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> StorageProviderCommandFactory
    {
      get { return _storageProviderCommandFactory; }
    }

    public virtual IRdbmsProviderCommand<IEnumerable<DataContainer>> CreateForRelationLookup (
        RelationEndPointDefinition foreignKeyEndPoint, ObjectID foreignKeyValue, SortExpressionDefinition? sortExpressionDefinition)
    {
      ArgumentUtility.CheckNotNull("foreignKeyEndPoint", foreignKeyEndPoint);
      ArgumentUtility.CheckNotNull("foreignKeyValue", foreignKeyValue);

      return InlineRdbmsStorageEntityDefinitionVisitor.Visit<IRdbmsProviderCommand<IEnumerable<DataContainer>>>(
          _rdbmsPersistenceModelProvider.GetEntityDefinition(foreignKeyEndPoint.ClassDefinition),
          (table, continuation) => CreateForDirectRelationLookup(table, foreignKeyEndPoint, foreignKeyValue, sortExpressionDefinition),
          (filterView, continuation) => continuation(filterView.BaseEntity),
          (unionView, continuation) => CreateForIndirectRelationLookup(unionView, foreignKeyEndPoint, foreignKeyValue, sortExpressionDefinition),
          (emptyView, continuation) => CreateForEmptyRelationLookup());
    }

    protected virtual IRdbmsProviderCommand<IEnumerable<DataContainer>> CreateForDirectRelationLookup (
        TableDefinition tableDefinition,
        RelationEndPointDefinition foreignKeyEndPoint,
        ObjectID foreignKeyValue,
        SortExpressionDefinition? sortExpression)
    {
      var selectedColumns = tableDefinition.GetAllColumns().ToArray();
      var dataContainerReader = _objectReaderFactory.CreateDataContainerReader(tableDefinition, selectedColumns);

      var dbCommandBuilder = _dbCommandBuilderFactory.CreateForSelect(
          tableDefinition,
          selectedColumns,
          GetComparedColumns(foreignKeyEndPoint, foreignKeyValue),
          GetOrderedColumns(sortExpression));
      return DelegateBasedCommand.Create(
          new MultiObjectLoadCommand<DataContainer?>(new[] { Tuple.Create(dbCommandBuilder, dataContainerReader) }),
          lookupResults => lookupResults.Select(
              result =>
              {
                return Assertion.IsNotNull<DataContainer?>(
                    result,
                    "Because no OUTER JOIN query is involved in retrieving the result, the DataContainer can never be null.");
              }));
    }

    protected virtual IRdbmsProviderCommand<IEnumerable<DataContainer>> CreateForIndirectRelationLookup (
        UnionViewDefinition unionViewDefinition,
        RelationEndPointDefinition foreignKeyEndPoint,
        ObjectID foreignKeyValue,
        SortExpressionDefinition? sortExpression)
    {
      var selectedColumns = unionViewDefinition.ObjectIDProperty.GetColumns().ToArray();
      var dbCommandBuilder = _dbCommandBuilderFactory.CreateForSelect(
          unionViewDefinition,
          selectedColumns,
          GetComparedColumns(foreignKeyEndPoint, foreignKeyValue),
          GetOrderedColumns(sortExpression));

      var objectIDReader = _objectReaderFactory.CreateObjectIDReader(unionViewDefinition, selectedColumns);

      var objectIDLoadCommand = DelegateBasedCommand.Create(
          new MultiObjectIDLoadCommand(new[] { dbCommandBuilder }, objectIDReader),
          lookupResults => lookupResults.Select(
              result =>
              {
                return Assertion.IsNotNull<ObjectID?>(
                    result,
                    "Because no OUTER JOIN query is involved in retrieving the result, the ObjectID can never be null.");
              }));

      var indirectDataContainerLoadCommand = new IndirectDataContainerLoadCommand(objectIDLoadCommand, _storageProviderCommandFactory);
      return DelegateBasedCommand.Create(
          indirectDataContainerLoadCommand,
          lookupResults => lookupResults.Select(
              result =>
              {
                return Assertion.IsNotNull<DataContainer?>(
                    result.LocatedObject,
                    "Because ID lookup and DataContainer lookup are executed within the same database transaction, the DataContainer can never be null.");
              }));
    }

    protected virtual FixedValueCommand<IEnumerable<DataContainer>> CreateForEmptyRelationLookup ()
    {
      return new FixedValueCommand<IEnumerable<DataContainer>>(Enumerable.Empty<DataContainer>());
    }

    protected virtual IEnumerable<ColumnValue> GetComparedColumns (RelationEndPointDefinition foreignKeyEndPoint, ObjectID foreignKeyValue)
    {
      var storagePropertyDefinition = _rdbmsPersistenceModelProvider.GetStoragePropertyDefinition(foreignKeyEndPoint.PropertyDefinition);
      return storagePropertyDefinition.SplitValueForComparison(foreignKeyValue);
    }

    protected virtual IEnumerable<OrderedColumn> GetOrderedColumns (SortExpressionDefinition? sortExpression)
    {
      if (sortExpression == null)
        return Array.Empty<OrderedColumn>();

      Assertion.IsTrue(sortExpression.SortedProperties.Count > 0, "The sort-expression must have at least one sorted property.");

      return from sortedProperty in sortExpression.SortedProperties
             let storagePropertyDefinition = _rdbmsPersistenceModelProvider.GetStoragePropertyDefinition(sortedProperty.PropertyDefinition)
             from column in storagePropertyDefinition.GetColumns()
             select new OrderedColumn(column, sortedProperty.Order);
    }

  }
}
