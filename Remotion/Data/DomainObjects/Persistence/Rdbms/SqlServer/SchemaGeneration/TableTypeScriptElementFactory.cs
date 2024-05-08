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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;

/// <summary>
/// The <see cref="TableTypeScriptElementFactory"/> is responsible for creating script-elements for table types in an SQL Server database.
/// </summary>
public class TableTypeScriptElementFactory : SqlElementFactoryBase, IStructuredTypeScriptElementFactory
{
  public TableTypeScriptElementFactory ()
  {
  }

  public IScriptElement GetCreateElement (TableTypeDefinition tableTypeDefinition)
  {
    ArgumentUtility.CheckNotNull(nameof(tableTypeDefinition), tableTypeDefinition);

    var columnDeclarations = tableTypeDefinition.GetAllColumns().Select(GetColumnDeclaration);
    var tableConstraints = GetTableConstraintDeclarations(tableTypeDefinition);
    var schemaName = tableTypeDefinition.TypeName.SchemaName ?? DefaultSchema;
    var typeName = tableTypeDefinition.TypeName.EntityName;

    var script = new StringBuilder();
    script.AppendLine($"CREATE TYPE [{schemaName}].[{typeName}] AS TABLE");
    script.AppendLine("(");

    var first = true;
    foreach (var columnDeclaration in columnDeclarations)
    {
      if (!first)
        script.AppendLine(",");

      script.Append($"  {columnDeclaration}");
      first = false;
    }

    script.AppendLine();

    first = true;
    foreach (var tableConstraint in tableConstraints)
    {
      if (!first)
        script.AppendLine(",");

      script.Append($"  {tableConstraint}");
      first = false;
    }

    if (tableConstraints.Any())
      script.AppendLine();

    script.Append(')');
    var scriptElementCollection = new ScriptElementCollection();
    scriptElementCollection.AddElement(new ScriptStatement(script.ToString()));
    scriptElementCollection.AddElement(CreateBatchDelimiterStatement());
    return scriptElementCollection;
  }

  public IScriptElement GetDropElement (TableTypeDefinition tableTypeDefinition)
  {
    return new ScriptStatement($"DROP TYPE IF EXISTS [{tableTypeDefinition.TypeName.SchemaName ?? DefaultSchema}].[{tableTypeDefinition.TypeName.EntityName}]");
  }

  private string GetColumnDeclaration (ColumnDefinition column)
  {
    return $"[{column.Name}] {column.StorageTypeInfo.StorageTypeName} {(column.StorageTypeInfo.IsStorageTypeNullable ? "NULL" : "NOT NULL")}";
  }

  private IReadOnlyList<string> GetTableConstraintDeclarations (TableTypeDefinition tableTypeDefinition)
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
}
