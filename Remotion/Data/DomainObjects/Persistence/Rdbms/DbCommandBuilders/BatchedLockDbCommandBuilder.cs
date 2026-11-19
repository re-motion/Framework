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
/// The <see cref="BatchedLockDbCommandBuilder"/> builds a command that locks a set of records.
/// </summary>
public class BatchedLockDbCommandBuilder : DbCommandBuilder
{
  private record StatementCacheEntry (string ForReadCommit, string ForNonReadCommit);

  private static readonly ConcurrentDictionary<TableDefinition, StatementCacheEntry> s_statementCache = new();
  private readonly IReadOnlyList<IBatchedCommandSpecification> _commandSpecifications;

  public BatchedLockDbCommandBuilder (ISqlDialect sqlDialect, IReadOnlyList<IBatchedCommandSpecification> commandSpecifications)
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

    var forReadCommitBuilder = new StringBuilder();
    var forNonReadCommitBuilder = new StringBuilder();

    var isFirstCommandSpecification = true;
    foreach (var specification in _commandSpecifications)
    {
      if (!isFirstCommandSpecification)
      {
        forReadCommitBuilder.AppendLine().AppendLine("UNION ALL");
        forNonReadCommitBuilder.AppendLine().AppendLine("UNION ALL");
      }

      isFirstCommandSpecification = false;

      var parameterName = GetParameterName(specification);

      var statements = GetOrCreateLockStatements(specification);
      forReadCommitBuilder.Append(statements.ForReadCommit);
      forNonReadCommitBuilder.Append(statements.ForNonReadCommit);

      command.Parameters.Add(specification.CreateDbParameter(command, parameterName));
    }

    command.CommandText = CreateStatement(forReadCommitBuilder, forNonReadCommitBuilder);

    return command;
  }

  private string CreateStatement (StringBuilder forReadCommit, StringBuilder forNonReadCommit)
  {
    return $"""
           DECLARE @TransactionIsolationLevel int;
           DECLARE @IsReadCommittedSnapshotOn bit;
           SET @TransactionIsolationLevel = (SELECT [transaction_isolation_level] FROM [sys].[dm_exec_sessions] WHERE [session_id] = @@SPID);
           SET @IsReadCommittedSnapshotOn = (SELECT [is_read_committed_snapshot_on] FROM [sys].[databases] WHERE [database_id] = DB_ID());
           IF (@TransactionIsolationLevel = 2 AND @IsReadCommittedSnapshotOn = 1)
           BEGIN
           {forReadCommit}{SqlDialect.StatementDelimiter}
           END
           ELSE
           BEGIN
           {forNonReadCommit}{SqlDialect.StatementDelimiter}
           END
           """;
  }

  private string GetParameterName (IBatchedCommandSpecification specification)
  {
    return SqlDialect.GetParameterName("TVP_Lock_" + specification.TableDefinition.TableName.EntityName);
  }

  private StatementCacheEntry GetOrCreateLockStatements (IBatchedCommandSpecification specification)
  {
    return s_statementCache.GetOrAdd(specification.TableDefinition, _ => CreateLockStatements(specification));
  }

  private StatementCacheEntry CreateLockStatements (IBatchedCommandSpecification specification)
  {
    var tableAlias = SqlDialect.DelimitIdentifier("T");
    var parameterAlias = SqlDialect.DelimitIdentifier("P");

    var parameterName = GetParameterName(specification);
    var schemaName = GetSchemaName(specification.TableDefinition);
    var tableName = SqlDialect.DelimitIdentifier(specification.TableDefinition.TableName.EntityName);

    var delimitedColumns = specification.Columns.Select(c => SqlDialect.DelimitIdentifier(c)).ToArray();

    var forReadCommit = CreateLockStatement(true, schemaName, tableName, tableAlias, parameterName, parameterAlias, delimitedColumns);
    var forNonReadCommit = CreateLockStatement(false, schemaName, tableName, tableAlias, parameterName, parameterAlias, delimitedColumns);

    return new StatementCacheEntry(forReadCommit, forNonReadCommit);
  }

  private string CreateLockStatement (
      bool forReadCommittedIsolation,
      string schemaName,
      string tableName,
      string tableAlias,
      string parameterName,
      string parameterAlias,
      string[] columns)
  {
    var selectColumns = string.Join(", ", columns.Select(c => $"{parameterAlias}.{c}"));
    var joinCondition = string.Join(" AND ", columns.Select(c => $"{parameterAlias}.{c} = {tableAlias}.{c}"));
    // FORCESEEK guarantees that the optimizer seeks into the target table's index instead of scanning it, which would otherwise
    // XLOCK every row of the table (not just the rows being locked) on small tables and cause spurious ConcurrencyViolations
    // for concurrent batches locking disjoint rows (RM-9724). The target table's ID column always has a clustered index (its
    // primary key), so a seek plan always exists.
    var tableHints = forReadCommittedIsolation ? "ROWLOCK, XLOCK, READPAST, FORCESEEK" : "ROWLOCK, XLOCK, FORCESEEK";

    return $"""
            SELECT {selectColumns} FROM {schemaName}{tableName} {tableAlias} WITH({tableHints})
            RIGHT JOIN {parameterName} {parameterAlias} ON {joinCondition}
            WHERE {tableAlias}.[ID] IS NULL
            """;
  }

  private string GetSchemaName (TableDefinition tableDefinition)
  {
    if (tableDefinition.TableName.SchemaName == null)
      return string.Empty;

    return SqlDialect.DelimitIdentifier(tableDefinition.TableName.SchemaName) + ".";
  }
}
