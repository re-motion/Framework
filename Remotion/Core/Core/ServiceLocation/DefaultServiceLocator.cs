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
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Provides a default implementation of the <see cref="IServiceLocator"/> interface based on the <see cref="ImplementationForAttribute"/>.
  /// The <see cref="SafeServiceLocator"/> uses (and registers) an instance of this class unless an application registers its own service locator via 
  /// <see cref="ServiceLocator.SetLocatorProvider"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This implementation of <see cref="IServiceLocator"/> uses the <see cref="ImplementationForAttribute"/> to resolve implementations of
  /// "service types" (usually interfaces or abstract classes). When the <see cref="DefaultServiceLocator"/> is asked to get an instance of a specific 
  /// service type for the first time, that type is checked for a <see cref="ImplementationForAttribute"/>, which is then inspected to determine 
  /// the actual concrete type to be instantiated, its lifetime, and similar properties. An instance is then returned that fulfills the properties 
  /// defined by the <see cref="ImplementationForAttribute"/>. After the first resolution of a service type, the instance (or a factory, 
  /// depending on the <see cref="LifetimeKind"/> associated with the type) is cached, so subsequent lookups for the same type are very fast.
  /// </para>
  /// <para>
  /// The <see cref="DefaultServiceLocator"/> also provides a set of <see cref="DefaultServiceLocator.Register(ServiceConfigurationEntry)"/> methods 
  /// that allow to registration of custom 
  /// implementations or factories for service types even if those types do not have the <see cref="ImplementationForAttribute"/> applied. 
  /// Applications can use this to override the configuration defined by the <see cref="ImplementationForAttribute"/> and to register 
  /// implementations of service types that do not have the <see cref="ImplementationForAttribute"/> applied. Custom implementations or factories
  /// must be registered before an instance of the respective service type is retrieved for the first time.
  /// </para>
  /// <para>
  /// In order to be instantiable by the <see cref="DefaultServiceLocator"/>, a concrete type indicated by the 
  /// <see cref="ImplementationForAttribute"/> must have exactly one public constructor. The constructor may have parameters, in which case
  /// the <see cref="DefaultServiceLocator"/> will try to get an instance for each of the parameters using the same <see cref="IServiceLocator"/>
  /// methods. If a parameter cannot be resolved (because the parameter type has no <see cref="ImplementationForAttribute"/> applied and no
  /// custom implementation or factory was manually registered), an exception is thrown. Dependency cycles are not detected and will lead to a 
  /// <see cref="StackOverflowException"/> or infinite loop. Use the <see cref="Register(ServiceConfigurationEntry)"/> method to manually 
  /// register a factory for types that do not apply to these constructor rules.
  /// </para>
  /// <para>
  /// In order to have a custom service locator use the same defaults as the <see cref="DefaultServiceLocator"/>, the 
  /// <see cref="DefaultServiceConfigurationDiscoveryService"/> can be used to extract those defaults from a set of types.
  /// </para>
  /// </remarks>
  /// <threadsafety static="true" instance="true" />
  public sealed partial class DefaultServiceLocator : IServiceLocator
  {
    private readonly IServiceConfigurationDiscoveryService _serviceConfigurationDiscoveryService;

    public static DefaultServiceLocator Create ()
    {
      return new DefaultServiceLocator(DefaultServiceConfigurationDiscoveryService.Create());
    }

    public DefaultServiceLocator (IServiceConfigurationDiscoveryService serviceConfigurationDiscoveryService)
    {
      ArgumentUtility.CheckNotNull("serviceConfigurationDiscoveryService", serviceConfigurationDiscoveryService);

      _serviceConfigurationDiscoveryService = serviceConfigurationDiscoveryService;

      // Optimized for memory allocations
      _createRegistrationFromTypeFunc = CreateRegistrationFromType;

      Register(new ServiceConfigurationEntry(typeof(ILogManager), new ServiceImplementationInfo(typeof(Log4NetLogManager), LifetimeKind.Singleton)));
    }

    /// <summary>
    /// Get an instance of the given <paramref name="serviceType"/>. The type must either have a <see cref="ImplementationForAttribute"/>, or
    /// a concrete implementation or factory must have been registered using one of the <see cref="Register(ServiceConfigurationEntry)"/> methods.
    /// </summary>
    /// <param name="serviceType">The type of object requested.</param>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="ActivationException">There was an error resolving the service instance: The
    /// <see cref="ImplementationForAttribute"/> could not be found on the <paramref name="serviceType"/>, or the concrete implementation could
    /// not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public object GetInstance (Type serviceType)
    {
      ArgumentUtility.CheckNotNull("serviceType", serviceType);

      var instance = GetInstanceOrNull(serviceType);
      if (instance == null)
        throw new ActivationException(string.Format("No implementation is registered for service type '{0}'.", serviceType));

      return instance;
    }

    /// <summary>
    /// Get an instance of the given <paramref name="serviceType"/>. The type must either have a <see cref="ImplementationForAttribute"/>, or
    /// a concrete implementation or factory must have been registered using one of the <see cref="Register(ServiceConfigurationEntry)"/> methods.
    /// </summary>
    /// <param name="serviceType">The type of object requested.</param>
    /// <param name="key">The name the object was registered with. This parameter is ignored by this implementation of <see cref="IServiceLocator"/>.</param>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="ActivationException">There was an error resolving the service instance: The
    /// <see cref="ImplementationForAttribute"/> could not be found on the <paramref name="serviceType"/>, or the concrete implementation could
    /// not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public object GetInstance (Type serviceType, string key)
    {
      ArgumentUtility.CheckNotNull("serviceType", serviceType);

      return GetInstance(serviceType);
    }

    /// <summary>
    /// Get all instance of the given <paramref name="serviceType"/>, or an empty sequence if no instance could be found.
    /// </summary>
    /// <param name="serviceType">The type of object requested.</param>
    /// <returns>
    /// A sequence of instances of the requested <paramref name="serviceType"/>. The <paramref name="serviceType"/> must either have a 
    /// <see cref="ImplementationForAttribute"/>, or a concrete implementation or factory must have been registered using one of the 
    /// <see cref="Register(ServiceConfigurationEntry)"/> methods. Otherwise, the sequence is empty.
    /// </returns>
    /// <exception cref="ActivationException">There was an error resolving the service instances: The concrete
    /// implementation could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public IEnumerable<object> GetAllInstances (Type serviceType)
    {
      ArgumentUtility.CheckNotNull("serviceType", serviceType);

      return GetAllInstances(serviceType, false);
    }

    /// <summary>
    /// Get an instance of the given <typeparamref name="TService"/> type. The type must either have a <see cref="ImplementationForAttribute"/>, 
    /// or a concrete implementation or factory must have been registered using one of the <see cref="Register(ServiceConfigurationEntry)"/> methods.
    /// </summary>
    ///<typeparam name="TService">The type of object requested.</typeparam>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="ActivationException">There was an error resolving the service instance: The
    /// <see cref="ImplementationForAttribute"/> could not be found on the <typeparamref name="TService"/>, type or the concrete implementation 
    /// could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public TService GetInstance<TService> ()
    {
      return (TService)GetInstance(typeof(TService));
    }

    /// <summary>
    /// Get an instance of the given <typeparamref name="TService"/> type. The type must either have a <see cref="ImplementationForAttribute"/>,
    /// or a concrete implementation or factory must have been registered using one of the <see cref="Register(ServiceConfigurationEntry)"/> methods.
    /// </summary>
    /// <typeparam name="TService">The type of object requested.</typeparam>
    /// <param name="key">The name the object was registered with. This parameter is ignored by this implementation of <see cref="IServiceLocator"/>.</param>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="ActivationException">There was an error resolving the service instance: The
    /// <see cref="ImplementationForAttribute"/> could not be found on the <typeparamref name="TService"/>, type or the concrete implementation
    /// could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public TService GetInstance<TService> (string key)
    {
      return (TService)GetInstance(typeof(TService), key);
    }

    /// <summary>
    /// Get all instance of the given <typeparamref name="TService"/> type, or an empty sequence if no instance could be found.
    /// </summary>
    /// <typeparam name="TService">The type of object requested.</typeparam>
    /// <returns>
    /// A sequence of instances of the requested <typeparamref name="TService"/> type. The <typeparamref name="TService"/> type must either have a 
    /// <see cref="ImplementationForAttribute"/>, or a concrete implementation or factory must have been registered using one of the 
    /// <see cref="Register(ServiceConfigurationEntry)"/> methods. Otherwise, the sequence is empty.
    /// </returns>
    /// <exception cref="ActivationException">There was an error resolving the service instances: The concrete
    /// implementation could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public IEnumerable<TService> GetAllInstances<TService> ()
    {
      return GetAllInstances(typeof(TService)).Cast<TService>();
    }

    /// <summary>
    /// Get an instance of the given <paramref name="serviceType"/>. The type must either have a <see cref="ImplementationForAttribute"/>, or
    /// a concrete implementation or factory must have been registered using one of the <see cref="Register(ServiceConfigurationEntry)"/> methods. Otherwise, 
    /// the method returns <see langword="null"/>.
    /// </summary>
    /// <param name="serviceType">The type of object requested.</param>
    /// <returns>The requested service instance, or <see langword="null" /> if no instance could be found.</returns>
    /// <exception cref="ActivationException">There was an error resolving the service instance: The concrete
    /// implementation could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    object? IServiceProvider.GetService (Type serviceType)
    {
      ArgumentUtility.CheckNotNull("serviceType", serviceType);

      return GetInstanceOrNull(serviceType);
    }

    private object? GetInstanceOrNull (Type serviceType)
    {
      var registration = GetOrCreateRegistrationWithActivationException(serviceType);

      if (registration.CompoundFactory != null)
        return InvokeInstanceFactoryWithActivationException(registration.CompoundFactory, serviceType);

      if (registration.SingleFactory != null)
        return InvokeInstanceFactoryWithActivationException(registration.SingleFactory, serviceType);

      if (registration.SingleFactory == null && registration.MultipleFactories.Count > 0)
      {
        throw new ActivationException(
            string.Format(
                "Multiple implementations are configured for service type '{0}'. Use GetAllInstances() to retrieve the implementations.",
                serviceType));
      }

      return null;
    }

    private IEnumerable<object> GetAllInstances (Type serviceType, bool isCompoundResolution)
    {
      var registration = GetOrCreateRegistrationWithActivationException(serviceType);

      if (registration.SingleFactory != null)
      {
        throw new ActivationException(
            string.Format(
                "A single implementation is configured for service type '{0}'. Use GetInstance() to retrieve the implementation.",
                serviceType));
      }

      if (!isCompoundResolution && registration.CompoundFactory != null)
      {
        throw new ActivationException(
            string.Format(
                "A compound implementation is configured for service type '{0}'. Use GetInstance() to retrieve the implementation.",
                serviceType));
      }

      return registration.MultipleFactories.Select(factory => InvokeInstanceFactoryWithActivationException(factory, serviceType));
    }

    private object InvokeInstanceFactoryWithActivationException (Func<object> factory, Type serviceType)
    {
      object instance;
      try
      {
        instance = factory();
      }
      catch (ActivationException ex)
      {
        var message = string.Format("Could not resolve type '{0}': {1}", serviceType, ex.Message);
        throw new ActivationException(message, ex);
      }
      catch (Exception ex)
      {
        var message = string.Format("{0}: {1}", ex.GetType().Name, ex.Message);
        throw new ActivationException(message, ex);
      }

      if (instance == null)
      {
        var message = string.Format(
            "The registered factory returned null instead of an instance implementing the requested service type '{0}'.",
            serviceType);
        throw new ActivationException(message);
      }

      if (!serviceType.IsInstanceOfType(instance))
      {
        var message = string.Format(
            "The instance returned by the registered factory does not implement the requested type '{0}'. (Instance type: '{1}'.)",
            serviceType,
            instance.GetType());
        throw new ActivationException(message);
      }

      return instance;
    }
  }
}
