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
using System.Linq;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// The <see cref="SqlUnionViewScriptElementFactory"/> is responsible to create script-elements for union-views in a sql-server database.
  /// </summary>
  public class SqlUnionViewScriptElementFactory : SqlViewScriptElementFactoryBase<UnionViewDefinition>
  {
    protected override string GetSelectStatements (UnionViewDefinition unionViewDefinition)
    {
      ArgumentUtility.CheckNotNull ("unionViewDefinition", unionViewDefinition);

      var createSelectStringBuilder = new StringBuilder ();
      
      foreach (var tableDefinition in unionViewDefinition.GetAllTables ())
      {
        if (createSelectStringBuilder.Length > 0)
          createSelectStringBuilder.AppendFormat ("\r\n  UNION ALL\r\n");

        var availableTableColumns = tableDefinition.GetAllColumns();
        var unionedColumns = unionViewDefinition.CalculateFullColumnList (availableTableColumns);
        createSelectStringBuilder.AppendFormat (
            "  SELECT {0}\r\n"
            + "    FROM [{1}].[{2}]",
            GetColumnList (unionedColumns),
            tableDefinition.TableName.SchemaName ?? DefaultSchema,
            tableDefinition.TableName.EntityName);
      }
      return createSelectStringBuilder.ToString ();
    }

    protected override bool UseCheckOption (UnionViewDefinition unionViewDefinition)
    {
      ArgumentUtility.CheckNotNull ("unionViewDefinition", unionViewDefinition);

      return unionViewDefinition.GetAllTables().Count() == 1;
    }
  }
}