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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

/// <summary>
/// Represents a unique constraint in a relational database management system.
/// </summary>
/// <remarks>
/// In contrast to <see cref="PrimaryKeyConstraintDefinition"/>, a <see cref="UniqueConstraintDefinition"/> can include <see cref="DBNull"/> values.
/// </remarks>
public class UniqueConstraintDefinition : ITableConstraintDefinition
{
  public string ConstraintName { get; }

  public bool IsClustered { get; }

  public IReadOnlyCollection<ColumnDefinition> Columns { get; }

  public UniqueConstraintDefinition (string constraintName, bool isClustered, IEnumerable<ColumnDefinition> columns)
  {
    ArgumentUtility.CheckNotNullOrEmpty(nameof(constraintName), constraintName);
    ArgumentUtility.CheckNotNull(nameof(columns), columns);

    var columnsList = columns.ToList().AsReadOnly();
    ArgumentUtility.CheckNotNullOrEmptyOrItemsNull(nameof(columns), columnsList);

    ConstraintName = constraintName;
    IsClustered = isClustered;
    Columns = columnsList;
  }

  public void Accept (ITableConstraintDefinitionVisitor visitor)
  {
    ArgumentUtility.CheckNotNull(nameof(visitor), visitor);

    visitor.VisitUniqueConstraintDefinition(this);
  }
}
