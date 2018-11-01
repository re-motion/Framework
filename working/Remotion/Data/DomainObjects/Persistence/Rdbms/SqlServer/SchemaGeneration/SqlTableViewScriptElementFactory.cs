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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// The <see cref="SqlTableViewScriptElementFactory"/> is responsible to create script-elements for table-views in a sql-server database.
  /// </summary>
  public class SqlTableViewScriptElementFactory : SqlViewScriptElementFactoryBase<TableDefinition>
  {
    protected override string GetSelectStatements (TableDefinition tableDefinition)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);

      return string.Format (
          "  SELECT {0}\r\n    FROM [{1}].[{2}]",
          GetColumnList (tableDefinition.GetAllColumns()),
          tableDefinition.TableName.SchemaName ?? DefaultSchema,
          tableDefinition.TableName.EntityName);
    }

    protected override bool UseCheckOption (TableDefinition tableDefinition)
    {
      return true;
    }
  }
}