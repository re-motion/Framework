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

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Defines the kind of a given <see cref="RelationDefinition"/>.
  /// </summary>
  public enum RelationKindType
  {
    /// <summary>
    /// There is a one-to-one relationship between referenced objects.
    /// </summary>
    OneToOne,
    /// <summary>
    /// There is a one-to-many (or many-to-one) relationship between referenced objects.
    /// </summary>
    OneToMany,
    /// <summary>
    /// There is a one-to-many relationship between referenced objects, but only the "many" side has a reference to its one related object; there
    /// is no back-reference to the many objects from the "one" side.
    /// </summary>
    Unidirectional
  }
}
