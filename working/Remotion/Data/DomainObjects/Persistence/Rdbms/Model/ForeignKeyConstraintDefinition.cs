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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="ForeignKeyConstraintDefinition"/> represents a foreign key constraint in a relational database management system.
  /// </summary>
  public class ForeignKeyConstraintDefinition : ITableConstraintDefinition
  {
    private readonly string _constraintName;
    private readonly EntityNameDefinition _referencedTableName;
    private readonly ReadOnlyCollection<ColumnDefinition> _referencingColumns;
    private readonly ReadOnlyCollection<ColumnDefinition> _referencedColumns;

    public ForeignKeyConstraintDefinition (
        string constraintName,
        EntityNameDefinition referencedTableName,
        IEnumerable<ColumnDefinition> referencingColumns,
        IEnumerable<ColumnDefinition> referencedColumns)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("constraintName", constraintName);
      ArgumentUtility.CheckNotNull ("referencedTableName", referencedTableName);
      ArgumentUtility.CheckNotNull ("referencingColumns", referencingColumns);
      ArgumentUtility.CheckNotNull ("referencedColumns", referencedColumns);

      _constraintName = constraintName;
      _referencedTableName = referencedTableName;
      _referencingColumns = referencingColumns.ToList().AsReadOnly();
      _referencedColumns = referencedColumns.ToList().AsReadOnly();

      if (_referencingColumns.Count != _referencedColumns.Count)
        throw new ArgumentException ("The referencing and referenced column sets must have the same number of items.", "referencingColumns");
    }

    public string ConstraintName
    {
      get { return _constraintName; }
    }

    public EntityNameDefinition ReferencedTableName
    {
      get { return _referencedTableName; }
    }

    public ReadOnlyCollection<ColumnDefinition> ReferencingColumns
    {
      get { return _referencingColumns; }
    }

    public ReadOnlyCollection<ColumnDefinition> ReferencedColumns
    {
      get { return _referencedColumns; }
    }

    public void Accept (ITableConstraintDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitForeignKeyConstraintDefinition (this);
    }
  }
}