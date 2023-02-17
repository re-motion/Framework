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
  /// Defines an API for registering services before the actual container or <see cref="IServiceLocator"/> has been built.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public interface IBootstrapServiceConfiguration
  {
    /// <summary>
    /// Returns the <see cref="ServiceConfigurationEntry"/> instances registered so far.
    /// </summary>
    ServiceConfigurationEntry[] Registrations { get; }

    /// <summary>
    /// Registers the given <see cref="ServiceConfigurationEntry"/> with this <see cref="IBootstrapServiceConfiguration"/>.
    /// </summary>
    /// <param name="entry">The <see cref="ServiceConfigurationEntry"/> to be registered.</param>
    void Register (ServiceConfigurationEntry entry);

    /// <summary>
    /// Registers an entry with the given <see cref="Type"/> instances and <see cref="LifetimeKind"/>.
    /// </summary>
    /// <param name="serviceType">The service type. This is a type for which instances are requested from a service locator.</param>
    /// <param name="implementationType">The concrete implementation of the service type.</param>
    /// <param name="lifetime">The lifetime of the instances of <paramref name="implementationType"/>.</param>
    void Register (Type serviceType, Type implementationType, LifetimeKind lifetime);

    /// <summary>
    /// Removes all registered <see cref="ServiceConfigurationEntry"/> data.
    /// </summary>
    void Reset ();
  }
}
