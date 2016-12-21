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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// The <see cref="SqlForeignKeyConstraintScriptElementFactory"/> is responsible to create script-elements for foreign-key constraints in a 
  /// sql-server database.
  /// </summary>
  public class SqlForeignKeyConstraintScriptElementFactory : SqlElementFactoryBase, IForeignKeyConstraintScriptElementFactory
  {
    public IScriptElement GetCreateElement (ForeignKeyConstraintDefinition constraintDefinition, EntityNameDefinition tableName)
    {
      ArgumentUtility.CheckNotNull ("constraintDefinition", constraintDefinition);
      ArgumentUtility.CheckNotNull ("tableName", tableName);

      return new ScriptStatement(
        string.Format (
            "ALTER TABLE [{0}].[{1}] ADD\r\n{2}",
            tableName.SchemaName ?? DefaultSchema,
            tableName.EntityName,
            GetConstraintDeclaration(constraintDefinition)));
    }

    public IScriptElement GetDropElement (ForeignKeyConstraintDefinition constraintDefinition, EntityNameDefinition tableName)
    {
      ArgumentUtility.CheckNotNull ("constraintDefinition", constraintDefinition);
      ArgumentUtility.CheckNotNull ("tableName", tableName);

      return new ScriptStatement (
          string.Format (
              "IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND "
              + "fk.name = '{2}' AND schema_name (t.schema_id) = '{0}' AND t.name = '{1}')\r\n"
              + "  ALTER TABLE [{0}].[{1}] DROP CONSTRAINT {2}",
              tableName.SchemaName ?? DefaultSchema,
              tableName.EntityName,
              constraintDefinition.ConstraintName));
    }

    private string GetConstraintDeclaration (ForeignKeyConstraintDefinition foreignKeyConstraintDefinition)
    {
      var referencedColumnNameList = GetColumnNameList (foreignKeyConstraintDefinition.ReferencedColumns);
      var referencingColumnNameList = GetColumnNameList (foreignKeyConstraintDefinition.ReferencingColumns);

      return string.Format (
          "  CONSTRAINT [{0}] FOREIGN KEY ({1}) REFERENCES [{2}].[{3}] ({4})",
          foreignKeyConstraintDefinition.ConstraintName,
          referencingColumnNameList,
          foreignKeyConstraintDefinition.ReferencedTableName.SchemaName ?? DefaultSchema,
          foreignKeyConstraintDefinition.ReferencedTableName.EntityName,
          referencedColumnNameList);
    }

    private string GetColumnNameList (IEnumerable<ColumnDefinition> columns)
    {
      return String.Join ((string) ", ", (IEnumerable<string>) columns.Select (c => "[" + c.Name + "]"));
    }
  }
}