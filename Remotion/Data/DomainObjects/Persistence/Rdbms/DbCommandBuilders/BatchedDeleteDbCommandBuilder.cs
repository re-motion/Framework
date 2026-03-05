// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;

/// <summary>
/// The <see cref="BatchedDeleteDbCommandBuilder"/> builds a command that deletes a set of records.
/// </summary>
public class BatchedDeleteDbCommandBuilder : DbCommandBuilder
{
  private static readonly ConcurrentDictionary<TableDefinition, string> s_statementCache = new();
  private readonly IReadOnlyList<IBatchedCommandSpecification> _commandSpecifications;

  public BatchedDeleteDbCommandBuilder (ISqlDialect sqlDialect, IReadOnlyList<IBatchedCommandSpecification> commandSpecifications)
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
    return SqlDialect.GetParameterName("TVP_Delete_" + specification.TableDefinition.TableName.EntityName);
  }

  private string GetOrCreateStatement (IBatchedCommandSpecification specification)
  {
    return s_statementCache.GetOrAdd(specification.TableDefinition, _ => CreateDeleteStatement(specification));
  }

  private string CreateDeleteStatement (IBatchedCommandSpecification specification)
  {
    var tableAlias = SqlDialect.DelimitIdentifier("T");
    var parameterAlias = SqlDialect.DelimitIdentifier("P");
    var parameterName = GetParameterName(specification);
    var schemaName = GetSchemaName(specification.TableDefinition);
    var tableName = SqlDialect.DelimitIdentifier(specification.TableDefinition.TableName.EntityName);

    var statement = $"""
                     DELETE {tableAlias}
                     FROM {schemaName}{tableName} {tableAlias}
                     INNER JOIN {parameterName} {parameterAlias} ON {parameterAlias}.[ID] = {tableAlias}.[ID]{SqlDialect.StatementDelimiter}
                     """;
    return statement;
  }

  private string GetSchemaName (TableDefinition tableDefinition)
  {
    if (tableDefinition.TableName.SchemaName == null)
      return string.Empty;

    return SqlDialect.DelimitIdentifier(tableDefinition.TableName.SchemaName) + ".";
  }
}
