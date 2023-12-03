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
using Remotion.Logging;
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
    private readonly Dictionary<Type, ServiceConfigurationEntry> _registrations = new();

    public BootstrapServiceConfiguration ()
    {
      // Logging
      RegisterAsSingleton<ILogManager, Log4NetLogManager>(() => new Log4NetLogManager());

      // ReSharper disable once LocalFunctionHidesMethod
      void RegisterAsSingleton<TService, TImplementation> (Func<TImplementation> factory)
          where TService : class
          where TImplementation : class, TService
      {
        lock (_lock)
        {
          var singleton = new Lazy<TImplementation>(factory);
          _registrations.Add(
              typeof(TService),
              new ServiceConfigurationEntry(typeof(TService), ServiceImplementationInfo.CreateSingle(() => singleton.Value, LifetimeKind.Singleton)));
        }
      }
    }

    public IReadOnlyCollection<ServiceConfigurationEntry> GetRegistrations ()
    {
      lock (_lock)
      {
        return _registrations.Values;
      }
    }

    public void Register (ServiceConfigurationEntry entry)
    {
      ArgumentUtility.CheckNotNull("entry", entry);

      lock (_lock)
      {
        _registrations[entry.ServiceType] = entry;
      }
    }
  }
}
