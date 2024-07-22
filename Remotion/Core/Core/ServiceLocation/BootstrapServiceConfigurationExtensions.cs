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
using Remotion.Configuration.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Extension methods for <see cref="IBootstrapServiceConfiguration"/>.
  /// </summary>
  public static class BootstrapServiceConfigurationExtensions
  {
    /// <summary>
    /// Registers an entry with the given <see cref="Type"/> instances and a <see cref="LifetimeKind"/>.<see cref="LifetimeKind.Singleton"/>.
    /// </summary>
    /// <param name="bootstrapServiceConfiguration">The <see cref="IBootstrapServiceConfiguration"/> for which the registration is performed.</param>
    /// <param name="serviceType">The service type. This is a type for which instances are requested from a service locator.</param>
    /// <param name="implementationType">The concrete implementation of the service type.</param>
    public static void Register (this IBootstrapServiceConfiguration bootstrapServiceConfiguration, Type serviceType, Type implementationType)
    {
      ArgumentUtility.CheckNotNull("bootstrapServiceConfiguration", bootstrapServiceConfiguration);
      ArgumentUtility.CheckNotNull("serviceType", serviceType);
      ArgumentUtility.CheckNotNull("implementationType", implementationType);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("implementationType", implementationType, serviceType);

      bootstrapServiceConfiguration.Register(
          new ServiceConfigurationEntry(
              serviceType,
              new ServiceImplementationInfo(
                  implementationType,
                  LifetimeKind.Singleton,
                  RegistrationType.Single)));
    }

    /// <summary>
    /// Registers an entry with the given <see cref="Type"/> instances and a <see cref="LifetimeKind"/>.<see cref="LifetimeKind.Singleton"/>.
    /// </summary>
    /// <typeparam name="TService">The service type to register the factories for.</typeparam>
    /// <param name="bootstrapServiceConfiguration">The <see cref="IBootstrapServiceConfiguration"/> for which the registration is performed.</param>
    /// <param name="instance">The instance to return when resolving for the <typeparamref name="TService"/>.</param>
    public static void Register<TService> (this IBootstrapServiceConfiguration bootstrapServiceConfiguration, TService instance)
        where TService : class
    {
      ArgumentUtility.CheckNotNull("bootstrapServiceConfiguration", bootstrapServiceConfiguration);
      ArgumentUtility.CheckNotNull("instance", instance);

      bootstrapServiceConfiguration.Register(
          new ServiceConfigurationEntry(
              typeof(TService),
              ServiceImplementationInfo.CreateSingle(() => instance, LifetimeKind.Singleton)));
    }

    /// <summary>
    /// The given root assemblies are employed for type discovery.
    /// </summary>
    /// <param name="bootstrapServiceConfiguration"></param>
    public static void RegisterSpecificRootAssemblies (this IBootstrapServiceConfiguration bootstrapServiceConfiguration)
    {
      ArgumentUtility.CheckNotNull("bootstrapServiceConfiguration", bootstrapServiceConfiguration);

      var assemblyLoader = new FilteringAssemblyLoader(new LoadAllAssemblyLoaderFilter());
      var specificRootAssemblies = TypeDiscoveryConfiguration.Current.SpecificRootAssemblies;
      var namedFinder = specificRootAssemblies.ByName.CreateRootAssemblyFinder(assemblyLoader);
      var filePatternFinder = specificRootAssemblies.ByFile.CreateRootAssemblyFinder(assemblyLoader);
      var rootAssemblyFinder = new CompositeRootAssemblyFinder(new IRootAssemblyFinder[] { namedFinder, filePatternFinder });

      bootstrapServiceConfiguration.Register<IRootAssemblyFinder>(rootAssemblyFinder);
    }
  }
}
