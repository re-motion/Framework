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
  /// The <see cref="SqlSynonymScriptElementFactory"/> is responsible to create script-elements for synonyms in a sql-server database.
  /// </summary>
  public class SqlSynonymScriptElementFactory
      :
          SqlElementFactoryBase,
          ISynonymScriptElementFactory<TableDefinition>,
          ISynonymScriptElementFactory<UnionViewDefinition>,
          ISynonymScriptElementFactory<FilterViewDefinition>,
          ISynonymScriptElementFactory<EmptyViewDefinition>
  {
    public IScriptElement GetCreateElement (TableDefinition tableDefinition, EntityNameDefinition synonymName)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull ("synonymName", synonymName);

      return GetSynonymCreateScriptStatement (tableDefinition.TableName, synonymName);
    }

    public IScriptElement GetDropElement (TableDefinition tableDefinition, EntityNameDefinition synonymName)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull ("synonymName", synonymName);

      return GetSynonymDropScriptStatement (synonymName);
    }

    public IScriptElement GetCreateElement (UnionViewDefinition unionViewDefinition, EntityNameDefinition synonymName)
    {
      ArgumentUtility.CheckNotNull ("unionViewDefinition", unionViewDefinition);
      ArgumentUtility.CheckNotNull ("synonymName", synonymName);

      return GetSynonymCreateScriptStatement (unionViewDefinition.ViewName, synonymName);
    }

    public IScriptElement GetDropElement (UnionViewDefinition unionViewDefinition, EntityNameDefinition synonymName)
    {
      ArgumentUtility.CheckNotNull ("unionViewDefinition", unionViewDefinition);
      ArgumentUtility.CheckNotNull ("synonymName", synonymName);

      return GetSynonymDropScriptStatement (synonymName);
    }

    public IScriptElement GetCreateElement (FilterViewDefinition filterViewDefinition, EntityNameDefinition synonymName)
    {
      ArgumentUtility.CheckNotNull ("filterViewDefinition", filterViewDefinition);
      ArgumentUtility.CheckNotNull ("synonymName", synonymName);

      return GetSynonymCreateScriptStatement (filterViewDefinition.ViewName, synonymName);
    }

    public IScriptElement GetDropElement (FilterViewDefinition filterViewDefinition, EntityNameDefinition synonymName)
    {
      ArgumentUtility.CheckNotNull ("filterViewDefinition", filterViewDefinition);
      ArgumentUtility.CheckNotNull ("synonymName", synonymName);

      return GetSynonymDropScriptStatement (synonymName);
    }

    public IScriptElement GetCreateElement (EmptyViewDefinition emptyViewDefinition, EntityNameDefinition synonymName)
    {
      ArgumentUtility.CheckNotNull ("emptyViewDefinition", emptyViewDefinition);
      ArgumentUtility.CheckNotNull ("synonymName", synonymName);

      return GetSynonymCreateScriptStatement (emptyViewDefinition.ViewName, synonymName);
    }

    public IScriptElement GetDropElement (EmptyViewDefinition emptyViewDefinition, EntityNameDefinition synonymName)
    {
      ArgumentUtility.CheckNotNull ("emptyViewDefinition", emptyViewDefinition);
      ArgumentUtility.CheckNotNull ("synonymName", synonymName);

      return GetSynonymDropScriptStatement (synonymName);
    }

    private ScriptStatement GetSynonymCreateScriptStatement (EntityNameDefinition referencedEntityName, EntityNameDefinition synonymName)
    {
      return new ScriptStatement (
          string.Format (
              "CREATE SYNONYM [{0}].[{1}] FOR [{2}].[{3}]",
              synonymName.SchemaName ?? DefaultSchema,
              synonymName.EntityName,
              referencedEntityName.SchemaName ?? DefaultSchema,
              referencedEntityName.EntityName));
    }

    private ScriptStatement GetSynonymDropScriptStatement (EntityNameDefinition synonymName)
    {
      return new ScriptStatement (
          string.Format (
              "IF EXISTS (SELECT * FROM sys.synonyms WHERE name = '{0}' AND SCHEMA_NAME(schema_id) = '{1}')\r\n"
              + "  DROP SYNONYM [{0}].[{1}]",
              synonymName.SchemaName ?? DefaultSchema,
              synonymName.EntityName));
    }
  }
}