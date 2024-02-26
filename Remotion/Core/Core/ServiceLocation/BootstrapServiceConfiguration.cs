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
using System.ComponentModel.Design;
using Remotion.Logging;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Reflection.TypeResolution;
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
      RegisterInstanceAsSingleton<ILogManager, Log4NetLogManager>(() => new Log4NetLogManager());

      // Type resolution
      RegisterImplementationAsSingleton<ITypeResolutionService, DefaultTypeResolutionService>();

      // Type discovery
      RegisterInstanceAsSingleton<IAssemblyLoaderFilter, ApplicationAssemblyLoaderFilter>(() => ApplicationAssemblyLoaderFilter.Instance);
      RegisterImplementationAsSingleton<IAssemblyLoader, FilteringAssemblyLoader>();
      RegisterImplementationAsSingleton<IAppContextProvider, AppContextProvider>();
#if NETFRAMEWORK
      RegisterImplementationAsSingleton<IRootAssemblyFinder, CurrentAppDomainBasedRootAssemblyFinder>();
#else
      RegisterImplementationAsSingleton<IRootAssemblyFinder, AppContextBasedRootAssemblyFinder>();
#endif
      RegisterDecoratedImplementationAsSingleton<IAssemblyFinder, AssemblyFinder, CachingAssemblyFinderDecorator>();
      RegisterImplementationAsSingleton<ITypeDiscoveryService, AssemblyFinderTypeDiscoveryService>();

      // Service Location
      RegisterImplementationAsSingleton<IServiceLocatorProvider, DefaultServiceLocatorProvider>();
      RegisterImplementationAsSingleton<IServiceConfigurationDiscoveryService, DefaultServiceConfigurationDiscoveryService>();
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

    private void RegisterImplementationAsSingleton<TService, TImplementation> ()
        where TService : class
        where TImplementation : class, TService
    {
      lock (_lock)
      {
        _registrations.Add(
            typeof(TService),
            new ServiceConfigurationEntry(typeof(TService), new ServiceImplementationInfo(typeof(TImplementation), LifetimeKind.Singleton, RegistrationType.Single)));
      }
    }

    private void RegisterDecoratedImplementationAsSingleton<TService, TImplementation, TDecorator> ()
        where TService : class
        where TImplementation : class, TService
        where TDecorator : class, TService
    {
      lock (_lock)
      {
        _registrations.Add(
            typeof(TService),
            new ServiceConfigurationEntry(
                typeof(TService),
                new ServiceImplementationInfo(typeof(TImplementation), LifetimeKind.Singleton, RegistrationType.Single),
                new ServiceImplementationInfo(typeof(TDecorator), LifetimeKind.InstancePerDependency, RegistrationType.Decorator)));
      }
    }

    private void RegisterInstanceAsSingleton<TService, TImplementation> (Func<TImplementation> factory)
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
}
