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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders
{
  /// <summary>
  /// The <see cref="SqlDbCommandBuilderFactory"/> creates SQL Server-specific <see cref="IDbCommandBuilder"/> instances.
  /// </summary>
  public class SqlDbCommandBuilderFactory : IDbCommandBuilderFactory
  {
    private readonly ISingleScalarStructuredTypeDefinitionProvider _tableTypeDefinitionProvider;
    private readonly ISqlDialect _sqlDialect;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDbCommandBuilderFactory"/> class.
    /// </summary>
    public SqlDbCommandBuilderFactory (ISingleScalarStructuredTypeDefinitionProvider tableTypeDefinitionProvider, ISqlDialect sqlDialect)
    {
      ArgumentNullException.ThrowIfNull(tableTypeDefinitionProvider);
      ArgumentNullException.ThrowIfNull(sqlDialect);
      _tableTypeDefinitionProvider = tableTypeDefinitionProvider;
      _sqlDialect = sqlDialect;
    }

    public IDbCommandBuilder CreateForSelect (
        TableDefinition table,
        IEnumerable<ColumnDefinition> selectedColumns,
        IEnumerable<ColumnValue> comparedColumnValues,
        IEnumerable<OrderedColumn> orderedColumns)
    {
      ArgumentNullException.ThrowIfNull(table);
      ArgumentNullException.ThrowIfNull(selectedColumns);
      ArgumentNullException.ThrowIfNull(comparedColumnValues);
      ArgumentNullException.ThrowIfNull(orderedColumns);

      return new SelectDbCommandBuilder(
          table,
          new SelectedColumnsSpecification(selectedColumns),
          new ComparedColumnsSpecification(comparedColumnValues),
          new OrderedColumnsSpecification(orderedColumns),
          _sqlDialect);
    }

    public IDbCommandBuilder CreateForSelect (
        TableDefinition table,
        IEnumerable<ColumnDefinition> selectedColumns,
        ColumnValueTable comparedColumnValueTable,
        IEnumerable<OrderedColumn> orderedColumns)
    {
      ArgumentNullException.ThrowIfNull(table);
      ArgumentNullException.ThrowIfNull(selectedColumns);
      ArgumentNullException.ThrowIfNull(orderedColumns);

      var (columnDefinition, comparedValues) = GetValuesForSingleColumnDefinition(comparedColumnValueTable);
      var tableType = (TableTypeDefinition)_tableTypeDefinitionProvider.GetStructuredTypeDefinition(columnDefinition.StorageTypeInfo.DotNetType, false);
      Assertion.IsTrue(
          tableType.Properties.Count == 1 && tableType.Properties.Single().GetColumns().Count() == 1,
          "Table type returned by ISingleScalarStructuredTypeDefinitionProvider should only contain a single property with a single column.");

      var recordDefinition = new RecordDefinition(
          columnDefinition.Name,
          tableType,
          [RecordPropertyDefinition.ScalarAsValue(tableType.Properties.Single())]);

      var dataParameterDefinition = new SqlTableValuedDataParameterDefinition(recordDefinition);

      return new SelectDbCommandBuilder(
          table,
          new SelectedColumnsSpecification(selectedColumns),
          new SqlTableValuedParameterComparedColumnSpecification(columnDefinition, comparedValues, dataParameterDefinition),
          new OrderedColumnsSpecification(orderedColumns),
          _sqlDialect);
    }

    public IDbCommandBuilder CreateForSelect (
        UnionViewDefinition view,
        IEnumerable<ColumnDefinition> selectedColumns,
        IEnumerable<ColumnValue> comparedColumnValues,
        IEnumerable<OrderedColumn> orderedColumns)
    {
      ArgumentNullException.ThrowIfNull(view);
      ArgumentNullException.ThrowIfNull(selectedColumns);
      ArgumentNullException.ThrowIfNull(comparedColumnValues);
      ArgumentNullException.ThrowIfNull(orderedColumns);

      return new UnionSelectDbCommandBuilder(
          view,
          new SelectedColumnsSpecification(selectedColumns),
          new ComparedColumnsSpecification(comparedColumnValues),
          new OrderedColumnsSpecification(orderedColumns),
          _sqlDialect);
    }

    public IDbCommandBuilder CreateForQuery (QueryStatementType statementType, string statement, IEnumerable<QueryParameterWithDataParameterDefinition> parametersWithType)
    {
      ArgumentNullException.ThrowIfNull(statement);
      ArgumentNullException.ThrowIfNull(parametersWithType);

      return statementType switch
      {
          QueryStatementType.Text
              => new QueryDbCommandBuilder(statement, parametersWithType.ToArray(), _sqlDialect),
          QueryStatementType.StoredProcedure
              => new StoredProcedureDbCommandBuilder(statement, parametersWithType, _sqlDialect),
          _
              => throw new NotSupportedException($"{nameof(QueryStatementType)} '{statementType}' is not supported.)")
      };
    }

    public IDbCommandBuilder CreateForInsert (TableDefinition tableDefinition, IEnumerable<ColumnValue> insertedColumns)
    {
      ArgumentNullException.ThrowIfNull(tableDefinition);
      ArgumentNullException.ThrowIfNull(insertedColumns);

      return new InsertDbCommandBuilder(tableDefinition, new InsertedColumnsSpecification(insertedColumns), _sqlDialect);
    }

    public IDbCommandBuilder CreateForUpdate (
        TableDefinition tableDefinition,
        IEnumerable<ColumnValue> updatedColumns,
        IEnumerable<ColumnValue> comparedColumnValues)
    {
      ArgumentNullException.ThrowIfNull(tableDefinition);
      ArgumentNullException.ThrowIfNull(updatedColumns);
      ArgumentNullException.ThrowIfNull(comparedColumnValues);

      return new UpdateDbCommandBuilder(
          tableDefinition,
          new UpdatedColumnsSpecification(updatedColumns),
          new ComparedColumnsSpecification(comparedColumnValues),
          _sqlDialect);
    }

    public IDbCommandBuilder CreateForDelete (TableDefinition tableDefinition, IEnumerable<ColumnValue> comparedColumnValues)
    {
      ArgumentNullException.ThrowIfNull(tableDefinition);
      ArgumentNullException.ThrowIfNull(comparedColumnValues);

      return new DeleteDbCommandBuilder(tableDefinition, new ComparedColumnsSpecification(comparedColumnValues), _sqlDialect);
    }

    private Tuple<ColumnDefinition, IEnumerable<object?>> GetValuesForSingleColumnDefinition (ColumnValueTable comparedColumnValueTable)
    {
      ColumnDefinition singleColumn;
      try
      {
        singleColumn = comparedColumnValueTable.Columns.Single();
      }
      catch (InvalidOperationException ex)
      {
        throw new NotSupportedException("The SQL provider can only handle multi-value comparisons with a single ColumnDefinition.", ex);
      }

      // We assume that the Single below can never throw when the Single above didn't throw. (Otherwise, the ColumnValueTable wouldn't be correct.)
      var values = comparedColumnValueTable.Rows.Select(r => r.Values.Single());

      return Tuple.Create(singleColumn, values);
    }
  }
}
