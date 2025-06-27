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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// The <see cref="SqlIndexDefinitionScriptElementFactory"/> is responsible to create script-elements for standard indexes in a sql-server database.
  /// </summary>
  public class SqlIndexDefinitionScriptElementFactory : SqlIndexScriptElementFactoryBase<SqlIndexDefinition>
  {
    public override IScriptElement GetCreateElement (SqlIndexDefinition indexDefinition, EntityNameDefinition ownerName)
    {
      ArgumentNullException.ThrowIfNull(indexDefinition);
      ArgumentNullException.ThrowIfNull(ownerName);

      return new ScriptStatement(
      string.Format(
         "CREATE {0}{1} INDEX [{2}]{8}  ON [{3}].[{4}] ({5}){6}{7}",
         indexDefinition.IsUnique.HasValue && indexDefinition.IsUnique.Value ? "UNIQUE " : string.Empty,
         indexDefinition.IsClustered.HasValue && indexDefinition.IsClustered.Value ? "CLUSTERED" : "NONCLUSTERED",
         indexDefinition.IndexName,
         ownerName.SchemaName ?? DefaultSchema,
         ownerName.EntityName,
         GetIndexedColumnNames(indexDefinition.Columns),
         indexDefinition.IncludedColumns != null
             ? $"{Environment.NewLine}  INCLUDE ({string.Join(", ", indexDefinition.IncludedColumns.Select(c => $"[{c.Name}]"))})"
             : string.Empty,
         GetCreateIndexOptions(GetCreateIndexOptionItems(indexDefinition)),
         Environment.NewLine));
    }

    protected override IEnumerable<string> GetCreateIndexOptionItems (SqlIndexDefinition indexDefinition)
    {
      var options = new List<string>();
      options.Add(GetIndexOption("IGNORE_DUP_KEY", indexDefinition.IgnoreDupKey));
      options.Add(GetIndexOption("ONLINE", indexDefinition.Online));
      options.AddRange(base.GetCreateIndexOptionItems(indexDefinition));
      return options;
    }
  }
}
