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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// The <see cref="SqlSecondaryXmlIndexDefinitionScriptElementFactory"/> is responsible to create script-elements for secondary-xml indexes in a 
  /// sql-server database.
  /// </summary>
  public class SqlSecondaryXmlIndexDefinitionScriptElementFactory : SqlIndexScriptElementFactoryBase<SqlSecondaryXmlIndexDefinition>
  {
    public override IScriptElement GetCreateElement (SqlSecondaryXmlIndexDefinition indexDefinition, EntityNameDefinition ownerName)
    {
      ArgumentUtility.CheckNotNull ("indexDefinition", indexDefinition);

      return new ScriptStatement(
        string.Format (
          "CREATE XML INDEX [{0}]\r\n"
          + "  ON [{1}].[{2}] ([{3}])\r\n"
          + "  USING XML INDEX [{4}]\r\n"
          + "  FOR {5}{6}",
          indexDefinition.IndexName,
          ownerName.SchemaName ?? DefaultSchema,
          ownerName.EntityName,
          indexDefinition.XmlColumn.Name,
          indexDefinition.PrimaryIndexName,
          indexDefinition.Kind,
          GetCreateIndexOptions (GetCreateIndexOptionItems (indexDefinition))));
    }
  }
}