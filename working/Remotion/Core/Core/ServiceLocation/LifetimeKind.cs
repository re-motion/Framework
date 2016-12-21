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

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Defines the lifetime of instances of a type managed by a service locator.
  /// </summary>
  public enum LifetimeKind
  {
    /// <summary>
    /// One instance is created and then reused every time an instance of the type is requested. This is the most efficient 
    /// <see cref="LifetimeKind"/>, and it should be used whenever possible. However, in cases where the service locator is used from multiple threads,
    /// the <see cref="Singleton"/> lifetime kind requires the instantiated type to be safe for multi-threading. Use <see cref="InstancePerDependency"/> when an
    /// implementation is not thread-safe.
    /// </summary>
    Singleton,
    /// <summary>
    /// A new instance is created every time an instance of the type is requested. This is the simplest <see cref="LifetimeKind"/> and works
    /// well even in multi-threaded environments, but it might not be the most efficient one. Use <see cref="Singleton"/> for more efficiency.
    /// </summary>
    InstancePerDependency
  }
}