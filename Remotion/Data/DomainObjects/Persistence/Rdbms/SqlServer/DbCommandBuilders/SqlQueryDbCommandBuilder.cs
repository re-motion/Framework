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
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;

/// <summary>
/// Builds an <see cref="DbCommand"/> with MS SQL Server-specific features for a given <see cref="IQuery"/>.
/// </summary>
/// <remarks>
/// When using table-valued parameters, the command is rewritten to select the TVP records into a temporary table, with an index if possible.
/// This is because unlike a TVP, a TempTable has statistics; in some circumstances, a lack of statistics can lead to catastrophically slow query plans.
/// The index further improves performance, even when there are few records in the TVP.
/// </remarks>
public class SqlQueryDbCommandBuilder : QueryDbCommandBuilder
{
  private readonly IReadOnlyDictionary<string,TvpSpec> _tvpToTempTable;

  private struct TvpSpec
  {
    public TvpSpec (string tempTableName, string indexName, bool hasIndex)
    {
      TempTableName = tempTableName;
      IndexName = indexName;
      HasIndex = hasIndex;
    }

    public string IndexName { get; }
    public string TempTableName { get; }
    public bool HasIndex { get; }
  }

  public SqlQueryDbCommandBuilder (string statement, IReadOnlyCollection<QueryParameterWithDataParameterDefinition> parameters, ISqlDialect sqlDialect)
      : base(statement, parameters, sqlDialect)
  {
    ArgumentException.ThrowIfNullOrEmpty(statement);
    ArgumentNullException.ThrowIfNull(parameters);
    ArgumentNullException.ThrowIfNull(sqlDialect);

    var tvp = parameters.Where(p => p.DataParameterDefinition is SqlTableValuedDataParameterDefinition);
    _tvpToTempTable = tvp.ToDictionary(
        p => p.QueryParameter.Name,
        p => new TvpSpec(
            "#" + p.QueryParameter.Name,
            "IX_" + p.QueryParameter.Name,
            ShouldCreateIndex(((SqlTableValuedDataParameterDefinition)p.DataParameterDefinition).RecordDefinition)));
  }

  private static bool ShouldCreateIndex (RecordDefinition recordDefinition)
  {
    if (recordDefinition.PropertyDefinitions.Count > 1)
      return false;

    var storagePropertyDefinition = recordDefinition.PropertyDefinitions.Single().StoragePropertyDefinition;
    if (storagePropertyDefinition is SerializedObjectIDStoragePropertyDefinition)
      return true;

    if (storagePropertyDefinition is not SimpleStoragePropertyDefinition simpleStoragePropertyDefinition)
      return false;

    var storageTypeLength = simpleStoragePropertyDefinition.ColumnDefinition.StorageTypeInfo.StorageTypeLength;
    if (!storageTypeLength.HasValue)
      return true;

    if (storageTypeLength.Value < 0)
      return false;

    var storageDbType = simpleStoragePropertyDefinition.ColumnDefinition.StorageTypeInfo.StorageDbType;

    // Maximum index width on SQL server is 1700 bytes
    const int maxIndexWidth = 1700;
    var maxLength = storageDbType == DbType.String || storageDbType == DbType.StringFixedLength
        ? maxIndexWidth / 2 // nvarchar and nchar use 2 bytes per storageTypeLength
        : maxIndexWidth;

    return storageTypeLength <= maxLength;
  }

  public override DbCommand Create (IDbCommandFactory dbCommandFactory)
  {
    ArgumentNullException.ThrowIfNull(dbCommandFactory);

    var command = base.Create(dbCommandFactory);
    if (_tvpToTempTable.Count == 0)
      return command;

    var commandTextBuilder = new StringBuilder(command.CommandText.Length);
    var delimitedValue = SqlDialect.DelimitIdentifier("Value");
    foreach (var tvp in _tvpToTempTable)
    {
      var delimitedTable = SqlDialect.DelimitIdentifier(tvp.Value.TempTableName);
      commandTextBuilder.AppendLine($"SELECT {delimitedValue} INTO {delimitedTable} FROM {tvp.Key};");
      if (tvp.Value.HasIndex)
      {
        var delimitedIndex = SqlDialect.DelimitIdentifier(tvp.Value.IndexName);
        commandTextBuilder.AppendLine($"CREATE NONCLUSTERED INDEX {delimitedIndex} ON {delimitedTable} ({delimitedValue});");
      }
    }

    commandTextBuilder.AppendLine();
    commandTextBuilder.Append(command.CommandText);
    commandTextBuilder.AppendLine();
    commandTextBuilder.AppendLine();

    foreach (var tvp in _tvpToTempTable.Values)
    {
      var delimitedTable = SqlDialect.DelimitIdentifier(tvp.TempTableName);
      commandTextBuilder.AppendLine($"DROP TABLE {delimitedTable};");
    }

    command.CommandText = commandTextBuilder.ToString();
    return command;
  }

  protected override string GetParameterTextRepresentation (QueryParameterWithDataParameterDefinition parameterWithDefinition)
  {
    if (_tvpToTempTable.TryGetValue(parameterWithDefinition.QueryParameter.Name, out var spec))
    {
      return SqlDialect.DelimitIdentifier(spec.TempTableName);
    }
    return base.GetParameterTextRepresentation(parameterWithDefinition);
  }
}
