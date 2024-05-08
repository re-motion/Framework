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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  public class SqlTableTypeScriptElementFactory : IStructuredTypeScriptElementFactory
  {
    private readonly string _defaultSchema = "dbo";

    public IScriptElement GetCreateElement (TableTypeDefinition tableTypeDefinition)
    {
      ArgumentUtility.CheckNotNull(nameof(tableTypeDefinition), tableTypeDefinition);

      var columnDeclarationList = string.Join(",\r\n", tableTypeDefinition.GetAllColumns().Select(GetColumnDeclaration));
      var primaryKeyConstraintString = GetPrimaryKeyDeclaration(tableTypeDefinition);
      return
          new ScriptStatement(
              $"CREATE TYPE [{tableTypeDefinition.TypeName.SchemaName ?? _defaultSchema}].[{tableTypeDefinition.TypeName.TypeName}] AS TABLE\r\n(\r\n{columnDeclarationList}{primaryKeyConstraintString}\r\n)");
    }

    public IScriptElement GetDropElement (TableTypeDefinition tableTypeDefinition)
    {
      return new ScriptStatement($"DROP TYPE IF EXISTS [{tableTypeDefinition.TypeName.SchemaName ?? _defaultSchema}].[{tableTypeDefinition.TypeName.TypeName}]");
    }

    private string GetColumnDeclaration (ColumnDefinition column)
    {
      return $"  [{column.Name}] {column.StorageTypeInfo.StorageTypeName} {(column.StorageTypeInfo.IsStorageTypeNullable ? "NULL" : "NOT NULL")}";
    }

    private string GetPrimaryKeyDeclaration (TableTypeDefinition tableTypeDefinition)
    {
      var primaryKeyConstraint = tableTypeDefinition.Constraints.OfType<PrimaryKeyConstraintDefinition>().SingleOrDefault();
      if (primaryKeyConstraint == null)
        return string.Empty;

      return
          $",\r\n  CONSTRAINT [{primaryKeyConstraint.ConstraintName}] PRIMARY KEY {(primaryKeyConstraint.IsClustered ? "CLUSTERED" : "NONCLUSTERED")} ({GetColumnList(primaryKeyConstraint.Columns)})";
    }

    protected string GetColumnList (IEnumerable<ColumnDefinition> columns)
    {
      ArgumentUtility.CheckNotNull(nameof(columns), columns);
      return string.Join((string)", ", columns.Select(cd => $"[{cd.Name}]"));
    }
  }
}
