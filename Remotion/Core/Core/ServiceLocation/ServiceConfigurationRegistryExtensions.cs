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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.ServiceLocation
{
  public static class ServiceConfigurationRegistryExtensions
  {
    //TODO RM-5506: register api: 
    //registerSingle(this IServiceConfigurationRegistry registry, Type serviceType, Type implementationType, Type[] decoratorTypes = null, LifetimeKind = Instance)
    //registerMultiple(this IServiceConfigurationRegistry registry, Type serviceType, Type[] implementationType, Type[] decoratorTypes = null, LifetimeKind = Instance)
    //registerCompound(this IServiceConfigurationRegistry registry, Type serviceType, Type compoundType, Type[] implementationTypes, Type[] decoratorTypes = null, LifetimeKind = Instance)
    //registerSingle(this IServiceConfigurationRegistry registry, Type serviceType, Func<object> instanceFactory, Type[] decoratorTypes = null, LifetimeKind = Instance
    //registerMultiple(this IServiceConfigurationRegistry registry, Type serviceType, Func<object>[] instanceFactories, Type[] decoratorTypes = null, LifetimeKind = Instance
    //registerCompound(this IServiceConfigurationRegistry registry, Type serviceType, Type compoundType, Func<object>[] instanceFactories, Type[] decoratorTypes = null, LifetimeKind = Instance

    /// <summary>
    /// Registers factories for the specified <typeparamref name="TService"/>. 
    /// The factories are subsequently invoked whenever instances for the <typeparamref name="TService"/> is requested.
    /// </summary>
    /// <typeparam name="TService">The service type to register the factories for.</typeparam>
    /// <param name="serviceConfigurationRegistry">The <see cref="IServiceConfigurationRegistry"/> with which to register the <typeparamref name="TService"/>.</param>
    /// <param name="instanceFactory">The instance factory to use when resolving instances for the <typeparamref name="TService"/>. This factory
    /// must return a non-null instance implementing the <typeparamref name="TService"/>, otherwise an <see cref="ActivationException"/> is thrown
    /// when an instance of <typeparamref name="TService"/> is requested.</param>
    /// <exception cref="InvalidOperationException">Factories have already been registered or an instance of the <typeparamref name="TService"/> has 
    /// already been retrieved. Registering factories or concrete implementations can only be done before any instances are retrieved.</exception>
    public static void RegisterSingle<TService> (this IServiceConfigurationRegistry serviceConfigurationRegistry, Func<TService> instanceFactory)
        where TService : class
    {
      ArgumentUtility.CheckNotNull("serviceConfigurationRegistry", serviceConfigurationRegistry);
      ArgumentUtility.CheckNotNull("instanceFactory", instanceFactory);

      var serviceConfigurationEntry = new ServiceConfigurationEntry(
          typeof(TService),
          ServiceImplementationInfo.CreateSingle(instanceFactory, LifetimeKind.InstancePerDependency));

      serviceConfigurationRegistry.Register(serviceConfigurationEntry);
    }

    /// <summary>
    /// Registers factories for the specified <typeparamref name="TService"/>. 
    /// The factories are subsequently invoked whenever instances for the <typeparamref name="TService"/> is requested.
    /// </summary>
    /// <typeparam name="TService">The service type to register the factories for.</typeparam>
    /// <param name="serviceConfigurationRegistry">The <see cref="IServiceConfigurationRegistry"/> with which to register the <typeparamref name="TService"/>.</param>
    /// <param name="instanceFactories">The instance factories to use when resolving instances for the <typeparamref name="TService"/>. These factories
    /// must return non-null instances implementing the <typeparamref name="TService"/>, otherwise an <see cref="ActivationException"/> is thrown
    /// when an instance of <typeparamref name="TService"/> is requested.</param>
    /// <exception cref="InvalidOperationException">Factories have already been registered or an instance of the <typeparamref name="TService"/> has 
    /// already been retrieved. Registering factories or concrete implementations can only be done before any instances are retrieved.</exception>
    public static void RegisterMultiple<TService> (this IServiceConfigurationRegistry serviceConfigurationRegistry, params Func<TService>[] instanceFactories)
        where TService : class
    {
      ArgumentUtility.CheckNotNull("serviceConfigurationRegistry", serviceConfigurationRegistry);
      ArgumentUtility.CheckNotNull("instanceFactories", instanceFactories);

      serviceConfigurationRegistry.RegisterMultiple((IEnumerable<Func<TService>>)instanceFactories);
    }

    //as extension for IServiceConfigurationRegistry, or drop if did not exist before feature branch or if no longer needed. Obsolete if old API was used in many projects
    /// <summary>
    /// Registers factories for the specified <typeparamref name="TService"/>. 
    /// The factories are subsequently invoked whenever instances for the <typeparamref name="TService"/> is requested.
    /// </summary>
    /// <typeparam name="TService">The service type to register the factories for.</typeparam>
    /// <param name="serviceConfigurationRegistry">The <see cref="IServiceConfigurationRegistry"/> with which to register the <typeparamref name="TService"/>.</param>
    /// <param name="instanceFactories">The instance factories to use when resolving instances for the <typeparamref name="TService"/>. These factories
    /// must return non-null instances implementing the <typeparamref name="TService"/>, otherwise an <see cref="ActivationException"/> is thrown
    /// when an instance of <typeparamref name="TService"/> is requested.</param>
    /// <exception cref="InvalidOperationException">Factories have already been registered or an instance of the <typeparamref name="TService"/> has 
    /// already been retrieved. Registering factories or concrete implementations can only be done before any instances are retrieved.</exception>
    public static void RegisterMultiple<TService> (this IServiceConfigurationRegistry serviceConfigurationRegistry, IEnumerable<Func<TService>> instanceFactories)
        where TService : class
    {
      ArgumentUtility.CheckNotNull("serviceConfigurationRegistry", serviceConfigurationRegistry);
      ArgumentUtility.CheckNotNull("instanceFactories", instanceFactories);

      var serviceConfigurationEntry = new ServiceConfigurationEntry(
          typeof(TService),
          instanceFactories.Select(f => ServiceImplementationInfo.CreateMultiple(f, LifetimeKind.InstancePerDependency)));

      serviceConfigurationRegistry.Register(serviceConfigurationEntry);
    }

    /// <summary>
    /// Registers a concrete implementation for the specified <paramref name="serviceType"/>.
    /// </summary>
    /// <param name="serviceConfigurationRegistry">The <see cref="IServiceConfigurationRegistry"/> with which to register the <paramref name="serviceType"/>.</param>
    /// <param name="serviceType">The service type to register a concrete implementation for.</param>
    /// <param name="concreteImplementationType">The type of the concrete implementation to be instantiated when an instance of the 
    /// <paramref name="serviceType"/> is retrieved.</param>
    /// <param name="lifetime">The lifetime of the instances.</param>
    /// <param name="registrationType">The registration type of the implementation.</param>
    /// <exception cref="InvalidOperationException">An instance of the <paramref name="serviceType"/> has already been retrieved. Registering factories
    /// or concrete implementations can only be done before any instances are retrieved.</exception>
    public static void Register (
        this IServiceConfigurationRegistry serviceConfigurationRegistry,
        Type serviceType,
        Type concreteImplementationType,
        LifetimeKind lifetime,
        RegistrationType registrationType = RegistrationType.Single)
    {
      ArgumentUtility.CheckNotNull("serviceConfigurationRegistry", serviceConfigurationRegistry);
      ArgumentUtility.CheckNotNull("serviceType", serviceType);
      ArgumentUtility.CheckNotNull("concreteImplementationType", concreteImplementationType);

      var serviceImplemetation = new ServiceImplementationInfo(concreteImplementationType, lifetime, registrationType);
      ServiceConfigurationEntry serviceConfigurationEntry;
      try
      {
        serviceConfigurationEntry = new ServiceConfigurationEntry(serviceType, serviceImplemetation);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException("Implementation type must implement service type.", "concreteImplementationType", ex);
      }

      serviceConfigurationRegistry.Register(serviceConfigurationEntry);
    }
  }
}
