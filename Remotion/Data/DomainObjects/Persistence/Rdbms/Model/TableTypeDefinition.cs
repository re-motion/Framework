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
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// Represents an SQL Server table type.
  /// </summary>
  public class TableTypeDefinition : IRdbmsStructuredTypeDefinition
  {
    /// <inheritdoc/>
    public EntityNameDefinition TypeName { get; }

    /// <inheritdoc/>
    public IReadOnlyCollection<IRdbmsStoragePropertyDefinition> Properties { get; }

    /// <summary>
    /// The constraints that are defined on the table type. 
    /// </summary>
    public IReadOnlyCollection<ITableConstraintDefinition> Constraints { get; }

    public TableTypeDefinition (
        EntityNameDefinition typeName,
        IReadOnlyCollection<IRdbmsStoragePropertyDefinition> properties,
        IReadOnlyCollection<ITableConstraintDefinition> constraints
    )
    {
      ArgumentUtility.CheckNotNull(nameof(typeName), typeName);
      ArgumentUtility.CheckNotNullOrEmpty(nameof(properties), properties);
      ArgumentUtility.CheckNotNull(nameof(constraints), constraints);

      TypeName = typeName;
      Properties = properties.AsReadOnly();
      Constraints = constraints.AsReadOnly();
    }

    /// <summary>
    /// Gets a <see cref="ColumnDefinition"/> for each column that is defined on the table type.
    /// </summary>
    public IEnumerable<ColumnDefinition> GetAllColumns ()
    {
      return Properties.SelectMany(p => p.GetColumns());
    }
  }
}
