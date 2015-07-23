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
  /// <see cref="SqlIndexScriptElementFactoryBase{T}"/> represents the base-class for all factory classes that are responsible to create new script 
  /// elements for creating indexes in a relational database.
  /// </summary>
  public abstract class SqlIndexScriptElementFactoryBase<T> : SqlElementFactoryBase, ISqlIndexDefinitionScriptElementFactory<T>
      where T: SqlIndexDefinitionBase
  {
    public abstract IScriptElement GetCreateElement (T indexDefinition, EntityNameDefinition ownerName);

    public virtual IScriptElement GetDropElement (T indexDefinition, EntityNameDefinition ownerName)
    {
      ArgumentUtility.CheckNotNull ("indexDefinition", indexDefinition);
      ArgumentUtility.CheckNotNull ("ownerName", ownerName);

      return new ScriptStatement (
          string.Format (
              "IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] "
              + "WHERE so.[name] = '{0}' AND schema_name (so.schema_id)='{1}' AND si.[name] = '{2}')\r\n"
              + "  DROP INDEX [{2}] ON [{1}].[{0}]",
              ownerName.EntityName,
              ownerName.SchemaName ?? DefaultSchema,
              indexDefinition.IndexName));
    }

    protected string GetIndexedColumnNames (IEnumerable<SqlIndexedColumnDefinition> indexedColumnDefinitions)
    {
      return string.Join (", ", indexedColumnDefinitions.Select (
          cd => "[" + cd.Columnn.Name + "]" + (cd.IndexOrder.HasValue ? " " + cd.IndexOrder.ToString().ToUpper() : string.Empty)));
    }

    protected virtual string GetCreateIndexOptions (IEnumerable<string> optionItems)
    {
      ArgumentUtility.CheckNotNull ("optionItems", optionItems);

      var filteredItems = optionItems.Except (new[] { string.Empty, null }).ToList ();
      if (filteredItems.Any())
        return "\r\n  WITH (" + string.Join (", ", filteredItems) + ")";
      else
        return string.Empty;
    }

    protected virtual IEnumerable<string> GetCreateIndexOptionItems (T indexDefinition)
    {
      ArgumentUtility.CheckNotNull ("indexDefinition", indexDefinition);

      yield return GetIndexOption("PAD_INDEX", indexDefinition.PadIndex);
      yield return GetIndexOption ("FILLFACTOR", indexDefinition.FillFactor);
      yield return GetIndexOption ("SORT_IN_TEMPDB", indexDefinition.SortInDb);
      yield return GetIndexOption ("STATISTICS_NORECOMPUTE", indexDefinition.StatisticsNoReCompute);
      yield return GetIndexOption ("DROP_EXISTING", indexDefinition.DropExisiting);
      yield return GetIndexOption ("ALLOW_ROW_LOCKS", indexDefinition.AllowRowLocks);
      yield return GetIndexOption ("ALLOW_PAGE_LOCKS", indexDefinition.AllowPageLocks);
      yield return GetIndexOption ("MAXDOP", indexDefinition.MaxDop);
    }

    protected string GetIndexOption (string optionName , bool? optionValue)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("optionName", optionName);

      if (optionValue.HasValue)
        return string.Format ("{0} = {1}", optionName, optionValue.Value ? "ON" : "OFF");
      else
        return string.Empty;
    }

    protected string GetIndexOption (string optionName , int? optionValue)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("optionName", optionName);

      if (optionValue.HasValue)
        return string.Format ("{0} = {1}", optionName, optionValue.Value);
      else
        return string.Empty;
    }
  }
}