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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// <see cref="SqlViewScriptElementFactoryBase{T}"/> represents the base-class for all factory classes that are responsible to create new script 
  /// elements for creating views in a relational database.
  /// </summary>
  public abstract class SqlViewScriptElementFactoryBase<T> : SqlElementFactoryBase, IViewScriptElementFactory<T> where T : IRdbmsStorageEntityDefinition
  {
    public IScriptElement GetCreateElement (T entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);

      var statements = new ScriptElementCollection ();
      statements.AddElement (CreateBatchDelimiterStatement ());
      statements.AddElement (
          new ScriptStatement (
              string.Format (
                  "CREATE VIEW [{0}].[{1}] ({2})\r\n"
                  + "  {3}AS\r\n{4}{5}",
                  entityDefinition.ViewName.SchemaName ?? DefaultSchema,
                  entityDefinition.ViewName.EntityName,
                  GetColumnList (entityDefinition.GetAllColumns()),
                  UseSchemaBinding (entityDefinition) ? "WITH SCHEMABINDING " : string.Empty,
                  GetSelectStatements(entityDefinition),
                  UseCheckOption(entityDefinition) ? "\r\n  WITH CHECK OPTION" : string.Empty)));
      statements.AddElement (CreateBatchDelimiterStatement ());
      return statements;
    }

    public virtual IScriptElement GetDropElement (T entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);

      return new ScriptStatement (
        string.Format (
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = '{1}' AND TABLE_SCHEMA = '{0}')\r\n"
          + "  DROP VIEW [{0}].[{1}]",
          entityDefinition.ViewName.SchemaName ?? DefaultSchema,
          entityDefinition.ViewName.EntityName
          ));
    }

    protected abstract string GetSelectStatements (T entityDefinition);
    protected abstract bool UseCheckOption (T entityDefinition);
    
    protected virtual bool UseSchemaBinding (T entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);
      return true;
    }
  }
}