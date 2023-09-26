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
using Remotion.Utilities;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Allows users to register services before an actual container or <see cref="IServiceLocator"/> has been built.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class BootstrapServiceConfiguration : IBootstrapServiceConfiguration
  {
    private readonly object _lock = new object();
    private readonly List<ServiceConfigurationEntry> _registrations = new List<ServiceConfigurationEntry>();

    private DefaultServiceLocator _bootstrapServiceLocator = DefaultServiceLocator.Create();

    public BootstrapServiceConfiguration ()
    {

    }

    public IServiceLocator BootstrapServiceLocator
    {
      get { return _bootstrapServiceLocator; }
    }

    public ServiceConfigurationEntry[] Registrations
    {
      get
      {
        lock (_lock)
        {
          return _registrations.ToArray();
        }
      }
    }

    public void Register (ServiceConfigurationEntry entry)
    {
      ArgumentUtility.CheckNotNull("entry", entry);

      lock (_lock)
      {
        _bootstrapServiceLocator.Register(entry);
        _registrations.Add(entry);
      }
    }

    public void Register (Type serviceType, Type implementationType, LifetimeKind lifetime)
    {
      ArgumentUtility.CheckNotNull("serviceType", serviceType);
      ArgumentUtility.CheckNotNull("implementationType", implementationType);

      var entry = new ServiceConfigurationEntry(serviceType, new ServiceImplementationInfo(implementationType, lifetime));
      Register(entry);
    }

    public void Reset ()
    {
      lock (_lock)
      {
        _bootstrapServiceLocator = DefaultServiceLocator.Create();
        _registrations.Clear();
      }
    }
  }
}
