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

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Indicates the kind of a <see cref="DomainObject">DomainObject's</see> property.
  /// </summary>
  public enum PropertyKind
  {
    /// <summary>
    /// The property is a simple value.
    /// </summary>
    PropertyValue,
    /// <summary>
    /// The property is a single related domain object.
    /// </summary>
    RelatedObject,
    /// <summary>
    /// The property is a collection of related domain objects.
    /// </summary>
    RelatedObjectCollection
  }
}
