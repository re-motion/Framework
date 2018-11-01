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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// The <see cref="SqlTableViewScriptElementFactory"/> is responsible to create script-elements for empty-views in a sql-server database.
  /// </summary>
  public class SqlEmptyViewScriptElementFactory : SqlViewScriptElementFactoryBase<EmptyViewDefinition>
  {
    protected override string GetSelectStatements (EmptyViewDefinition emptyViewDefinition)
    {
      ArgumentUtility.CheckNotNull ("emptyViewDefinition", emptyViewDefinition);

      return string.Format (
            "  SELECT {0}\r\n"
          + "    WHERE 1 = 0",
            GetNullColumnList (emptyViewDefinition.GetAllColumns()));
    }

    protected override bool UseCheckOption (EmptyViewDefinition emptyViewDefinition)
    {
      ArgumentUtility.CheckNotNull ("emptyViewDefinition", emptyViewDefinition);
      return false;
    }

    protected override bool UseSchemaBinding (EmptyViewDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);
      return false;
    }

    private string GetNullColumnList (IEnumerable<ColumnDefinition> columns)
    {
      ArgumentUtility.CheckNotNull ("columns", columns);
      return String.Join ((string) ", ", (IEnumerable<string>) columns.Select (cd => "CONVERT(" + cd.StorageTypeInfo.StorageTypeName + ",NULL) AS ["+ cd.Name + "]"));
    }
  }
}