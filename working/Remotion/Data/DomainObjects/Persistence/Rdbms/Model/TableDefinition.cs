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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// <see cref="TableDefinition"/> defines a table in a relational database.
  /// </summary>
  public class TableDefinition : RdbmsStorageEntityDefinitionBase
  {
    private readonly EntityNameDefinition _tableName;
    private readonly ReadOnlyCollection<ITableConstraintDefinition> _constraints;

    public TableDefinition (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition tableName,
        EntityNameDefinition viewName,
        ObjectIDStoragePropertyDefinition objectIDProperty,
        IRdbmsStoragePropertyDefinition timestampProperty,
        IEnumerable<IRdbmsStoragePropertyDefinition> dataProperties,
        IEnumerable<ITableConstraintDefinition> constraints,
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
      ArgumentUtility.CheckNotNull ("tableName", tableName);
      ArgumentUtility.CheckNotNull ("constraints", constraints);

      _tableName = tableName;
      _constraints = constraints.ToList().AsReadOnly();
    }

    public EntityNameDefinition TableName
    {
      get { return _tableName; }
    }

    public ReadOnlyCollection<ITableConstraintDefinition> Constraints
    {
      get { return _constraints; }
    }

    public override void Accept (IRdbmsStorageEntityDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitTableDefinition (this);
    }

    public ColumnDefinition[] CalculateAdjustedColumnList (IEnumerable<ColumnDefinition> fullColumnList)
    {
      ArgumentUtility.CheckNotNull ("fullColumnList", fullColumnList);

      var availableColumnsAsDictionary = GetAllColumns().ToDictionary (c => c);

      return fullColumnList.Select (columnDefinition => availableColumnsAsDictionary.GetValueOrDefault (columnDefinition)).ToArray();
    }
  }
}