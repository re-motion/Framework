// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;

/// <summary>
/// The <see cref="BatchedUpdateDbCommandBuilder"/> builds a command that updates a set of records.
/// </summary>
public class BatchedUpdateDbCommandBuilder : DbCommandBuilder
{
  private static readonly ConcurrentDictionary<TableDefinition, string> s_statementCache = new();
  private readonly IReadOnlyList<IBatchedCommandSpecification> _commandSpecifications;

  public BatchedUpdateDbCommandBuilder (ISqlDialect sqlDialect, IReadOnlyList<IBatchedCommandSpecification> commandSpecifications)
      : base(sqlDialect)
  {
    ArgumentNullException.ThrowIfNull(sqlDialect);
    ArgumentUtility.CheckNotNullOrEmptyOrItemsNull(nameof(commandSpecifications), commandSpecifications);

    _commandSpecifications = commandSpecifications;
  }

  public override DbCommand Create (IDbCommandFactory dbCommandFactory)
  {
    ArgumentNullException.ThrowIfNull(dbCommandFactory);

    var command = dbCommandFactory.CreateDbCommand();

    var statement = new StringBuilder();

    var isFirstCommandSpecification = true;
    foreach (var specification in _commandSpecifications)
    {
      if (!isFirstCommandSpecification)
        statement.AppendLine();

      isFirstCommandSpecification = false;

      statement.Append(GetOrCreateStatement(specification));
      command.Parameters.Add(specification.CreateDbParameter(command, GetParameterName(specification)));
    }

    command.CommandText = statement.ToString();

    return command;
  }

  private string GetParameterName (IBatchedCommandSpecification specification)
  {
    return SqlDialect.GetParameterName("TVP_Update_" + specification.TableDefinition.TableName.EntityName);
  }

  private string GetOrCreateStatement (IBatchedCommandSpecification specification)
  {
    return s_statementCache.GetOrAdd(specification.TableDefinition, _ => CreateUpdateStatement(specification));
  }

  private string CreateUpdateStatement (IBatchedCommandSpecification specification)
  {
    var statement = new StringBuilder();

    var tableAlias = SqlDialect.DelimitIdentifier("T");
    var parameterAlias = SqlDialect.DelimitIdentifier("P");
    var parameterName = GetParameterName(specification);
    var schemaName = GetSchemaName(specification.TableDefinition);
    var tableName = SqlDialect.DelimitIdentifier(specification.TableDefinition.TableName.EntityName);

    var optionalColumns = specification.Columns.Where(c => c.EndsWith(TableManipulationRecordDefinitionProvider.IsSetColumnPostFix))
        .Select(c => c.Substring(0, c.Length - TableManipulationRecordDefinitionProvider.IsSetColumnPostFix.Length))
        .ToArray();
    var nonOptionalColumns = specification.Columns.Where(c => !optionalColumns.Contains(c) && !c.EndsWith(TableManipulationRecordDefinitionProvider.IsSetColumnPostFix))
        .Select(c => SqlDialect.DelimitIdentifier(c)).ToArray();

    AppendUpdateStatement(statement, schemaName, tableName, tableAlias, parameterName, parameterAlias, nonOptionalColumns, optionalColumns);
    return statement.ToString();
  }

  private void AppendUpdateStatement (
      StringBuilder stringBuilder,
      string schemaName,
      string tableName,
      string tableAlias,
      string parameterName,
      string parameterAlias,
      string[] nonOptionalColumns,
      string[] optionalColumns
      )
  {
    var nonOptionalSets = nonOptionalColumns.Select(c => $"{tableAlias}.{c} = {parameterAlias}.{c}");
    var optionalSets = optionalColumns.Select(c => CreateOptionalSet(c, parameterAlias, tableAlias));

    var combinedSets = string.Join($",{Environment.NewLine}", nonOptionalSets.Concat(optionalSets));
    stringBuilder.Append(
        $"""
         UPDATE {tableAlias}
         SET
         {combinedSets}
         FROM {schemaName}{tableName} {tableAlias}
         INNER JOIN {parameterName} {parameterAlias} ON {parameterAlias}.[ID] = {tableAlias}.[ID]{SqlDialect.StatementDelimiter}
         """);
  }

  private string CreateOptionalSet (string column, string parameterAlias, string tableAlias)
  {
    var delimitedIsSetColumn = SqlDialect.DelimitIdentifier(column + TableManipulationRecordDefinitionProvider.IsSetColumnPostFix);
    var delimitedColumn = SqlDialect.DelimitIdentifier(column);
    return $"{tableAlias}.{delimitedColumn} = CASE WHEN {parameterAlias}.{delimitedIsSetColumn} = 1 THEN {parameterAlias}.{delimitedColumn} ELSE {tableAlias}.{delimitedColumn} END";
  }

  private string GetSchemaName (TableDefinition tableDefinition)
  {
    if (tableDefinition.TableName.SchemaName == null)
      return string.Empty;

    return SqlDialect.DelimitIdentifier(tableDefinition.TableName.SchemaName) + ".";
  }
}
