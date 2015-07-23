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
using Remotion.Data.DomainObjects.DomainImplementation;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Marker interface allowing <see cref="DomainObjectHandleExtensions.GetObject{T}"/> and similar methods to be called for a given <see cref="DomainObject"/>
  /// type.
  /// </summary>
  /// <remarks>
  /// Implement this interface on <see cref="DomainObject"/> classes that should provide <see cref="DomainObjectHandleExtensions.GetObject{T}"/> methods to
  /// domain users. When this interface is not implemented, the <see cref="LifetimeService"/> can be used to get instances of the 
  /// <see cref="DomainObject"/> type.
  /// </remarks>
  public interface ISupportsGetObject
  {
  }
}