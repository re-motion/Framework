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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// <see cref="UnionViewDefinition"/> defines a union view in a relational database.
  /// </summary>
  public class UnionViewDefinition : RdbmsStorageEntityDefinitionBase
  {
    private readonly ReadOnlyCollection<IRdbmsStorageEntityDefinition> _unionedEntities;

    public UnionViewDefinition (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition viewName,
        IEnumerable<IRdbmsStorageEntityDefinition> unionedEntities,
        ObjectIDStoragePropertyDefinition objectIDProperty,
        IRdbmsStoragePropertyDefinition timestampProperty,
        IEnumerable<IRdbmsStoragePropertyDefinition> dataProperties,
        IEnumerable<IIndexDefinition> indexes,
        IEnumerable<EntityNameDefinition> synonyms)
        : base (
            storageProviderDefinition,
            viewName,
            objectIDProperty,
            timestampProperty,
            dataProperties,
            indexes,
            synonyms)
    {
      ArgumentUtility.CheckNotNull ("unionedEntities", unionedEntities);

      var unionedEntitiesList = unionedEntities.ToList().AsReadOnly();
      ArgumentUtility.CheckNotEmpty ("unionedEntities", unionedEntitiesList);

      for (int i = 0; i < unionedEntitiesList.Count; ++i)
      {
        var unionedEntity = unionedEntitiesList[i];
        if (!(unionedEntity is TableDefinition || unionedEntity is UnionViewDefinition))
        {
          throw new ArgumentException (
              string.Format (
                  "Item {0} is of type '{1}', but the unioned entities must either be a TableDefinitions or UnionViewDefinitions.",
                  i,
                  unionedEntity.GetType()),
              "unionedEntities");
        }
      }

      _unionedEntities = unionedEntitiesList;
    }

    public ReadOnlyCollection<IRdbmsStorageEntityDefinition> UnionedEntities
    {
      get { return _unionedEntities; }
    }

    public ColumnDefinition[] CalculateFullColumnList (IEnumerable<ColumnDefinition> availableColumns)
    {
      ArgumentUtility.CheckNotNull ("availableColumns", availableColumns);

      // Since validation hasn't run yet, we can't be sure that all column names are unique. Therefore, choose the first column with matching name.
      var availableColumnsAsDictionary = availableColumns.ToLookup (c => c.Name);
      return GetAllColumns().Select (columnDefinition => availableColumnsAsDictionary[columnDefinition.Name].FirstOrDefault()).ToArray();
    }

    // Always returns at least one table
    public IEnumerable<TableDefinition> GetAllTables ()
    {
      foreach (var entityDefinition in _unionedEntities)
      {
        if (entityDefinition is TableDefinition)
          yield return (TableDefinition) entityDefinition;
        else
        {
          foreach (var derivedTable in ((UnionViewDefinition) entityDefinition).GetAllTables())
            yield return derivedTable;
        }
      }
    }

    public override void Accept (IRdbmsStorageEntityDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitUnionViewDefinition (this);
    }
  }
}