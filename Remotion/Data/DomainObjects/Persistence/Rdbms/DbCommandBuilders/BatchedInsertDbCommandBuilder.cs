// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;

/// <summary>
/// The <see cref="BatchedInsertDbCommandBuilder"/> builds a command that inserts a set of records.
/// </summary>
public class BatchedInsertDbCommandBuilder : DbCommandBuilder
{
  private readonly IReadOnlyList<IBatchedCommandSpecification> _commandSpecifications;

  public BatchedInsertDbCommandBuilder (ISqlDialect sqlDialect, IReadOnlyList<IBatchedCommandSpecification> commandSpecifications)
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

      var parameterName = SqlDialect.GetParameterName("TVP_Insert_" + specification.TableDefinition.TableName.EntityName);
      var schemaName = GetSchemaName(specification.TableDefinition);
      var tableName = SqlDialect.DelimitIdentifier(specification.TableDefinition.TableName.EntityName);

      var delimitedColumns = specification.Columns.Select(c => SqlDialect.DelimitIdentifier(c)).ToArray();

      AppendInsertStatement(statement, schemaName, tableName, parameterName, delimitedColumns);
      command.Parameters.Add(specification.CreateDbParameter(command, parameterName));
    }

    command.CommandText = statement.ToString();

    return command;
  }

  private void AppendInsertStatement (
      StringBuilder stringBuilder,
      string schemaName,
      string tableName,
      string parameterName,
      string[] columns)
  {
    var joinedColumns = string.Join(", ", columns);
    stringBuilder.Append(
        $"""
         INSERT INTO {schemaName}{tableName} ({joinedColumns})
         SELECT {joinedColumns} FROM {parameterName}{SqlDialect.StatementDelimiter}
         """);
  }

  private string GetSchemaName (TableDefinition tableDefinition)
  {
    if (tableDefinition.TableName.SchemaName == null)
      return string.Empty;

    return SqlDialect.DelimitIdentifier(tableDefinition.TableName.SchemaName) + ".";
  }
}
