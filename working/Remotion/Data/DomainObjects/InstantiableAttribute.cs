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

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// When the <see cref="InstantiableAttribute"/> is defined on a type, it signals that this type can be instantiated by the
  /// <see cref="DomainObject"/> infrastructure even though it declared as <see langword="abstract"/>.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class InstantiableAttribute: Attribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="InstantiableAttribute"/> class.
    /// </summary>
    public InstantiableAttribute()
    {
    }
  }
}
