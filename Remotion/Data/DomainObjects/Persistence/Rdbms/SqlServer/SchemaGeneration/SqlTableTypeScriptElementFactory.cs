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
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// The <see cref="SqlTableTypeScriptElementFactory"/> is responsible for creating script-elements for table types in an SQL Server database.
  /// </summary>
  public class SqlTableTypeScriptElementFactory : IStructuredTypeScriptElementFactory
  {
    private readonly string _defaultSchema = "dbo";

    public IScriptElement GetCreateElement (SqlTableTypeDefinition tableTypeDefinition)
    {
      ArgumentUtility.CheckNotNull(nameof(tableTypeDefinition), tableTypeDefinition);

      var columnDeclarationList = tableTypeDefinition.GetAllColumns().Select(GetColumnDeclaration).ToList();
      var tableConstraintList = GetTableConstraintDeclarations(tableTypeDefinition);
      var schemaName = tableTypeDefinition.TypeName.SchemaName ?? _defaultSchema;
      var typeName = tableTypeDefinition.TypeName.EntityName;

      var script = new StringBuilder();
      script.AppendLine($"IF TYPE_ID('[{schemaName}].[{typeName}]') IS NULL CREATE TYPE [{schemaName}].[{typeName}] AS TABLE");
      script.AppendLine("(");

      for (var i = 0; i < columnDeclarationList.Count; i++)
      {
        if (i > 0)
          script.AppendLine(",");

        script.Append($"  {columnDeclarationList[i]}");
      }

      script.AppendLine();

      for (var i = 0; i < tableConstraintList.Count; i++)
      {
        if (i > 0)
          script.AppendLine(",");

        script.Append($"  {tableConstraintList[i]}");
      }

      if (tableConstraintList.Any())
        script.AppendLine();

      script.Append(')');
      return new ScriptStatement(script.ToString());
    }

    public IScriptElement GetDropElement (SqlTableTypeDefinition tableTypeDefinition)
    {
      return new ScriptStatement($"DROP TYPE IF EXISTS [{tableTypeDefinition.TypeName.SchemaName ?? _defaultSchema}].[{tableTypeDefinition.TypeName.EntityName}]");
    }

    private string GetColumnDeclaration (ColumnDefinition column)
    {
      return $"[{column.Name}] {column.StorageTypeInfo.StorageTypeName} {(column.StorageTypeInfo.IsStorageTypeNullable ? "NULL" : "NOT NULL")}";
    }

    private string GetPrimaryKeyDeclaration (SqlTableTypeDefinition tableTypeDefinition)
    {
      var primaryKeyConstraint = tableTypeDefinition.Constraints.OfType<PrimaryKeyConstraintDefinition>().SingleOrDefault();
      if (primaryKeyConstraint == null)
        return string.Empty;

      return $"PRIMARY KEY {(primaryKeyConstraint.IsClustered ? "CLUSTERED" : "NONCLUSTERED")} " +
             $"({GetColumnList(primaryKeyConstraint.Columns)})";
    }

    private IReadOnlyList<string> GetTableConstraintDeclarations (SqlTableTypeDefinition tableTypeDefinition)
    {
      var constraints = new List<string>();

      var primaryKeyConstraint = tableTypeDefinition.Constraints.OfType<PrimaryKeyConstraintDefinition>().SingleOrDefault();
      if (primaryKeyConstraint != null)
      {
        constraints.Add(
            $"PRIMARY KEY {(primaryKeyConstraint.IsClustered ? "CLUSTERED" : "NONCLUSTERED")} " +
            $"({GetColumnList(primaryKeyConstraint.Columns)})");
      }

      foreach (var uniqueConstraint in tableTypeDefinition.Constraints.OfType<UniqueConstraintDefinition>())
      {
        constraints.Add(
            $"UNIQUE {(uniqueConstraint.IsClustered ? "CLUSTERED" : "NONCLUSTERED")} " +
            $"({GetColumnList(uniqueConstraint.Columns)})");
      }

      return constraints;
    }

    protected string GetColumnList (IEnumerable<ColumnDefinition> columns)
    {
      ArgumentUtility.CheckNotNull(nameof(columns), columns);
      return string.Join((string)", ", columns.Select(cd => $"[{cd.Name}]"));
    }
  }
}
