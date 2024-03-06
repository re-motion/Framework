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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Provides services for scanning a range of types for default service configuration settings, as they would be applied by 
  /// <see cref="DefaultServiceLocator"/>. Use this class in order to configure a specific service locator with <see cref="DefaultServiceLocator"/>'s
  /// defaults.
  /// </summary>
  /// <remarks>
  /// <para>
  /// <see cref="DefaultServiceConfigurationDiscoveryService"/> uses the same logic as <see cref="DefaultServiceLocator"/> in order to find the
  /// default concrete implementation of service types configured via the <see cref="ImplementationForAttribute"/>. See 
  /// <see cref="DefaultServiceLocator"/> for more information about this.
  /// </para>
  /// <para>
  /// Concrete implementations registered with a specific <see cref="DefaultServiceLocator"/> using its <see cref="DefaultServiceLocator.Register(ServiceConfigurationEntry)"/>
  /// methods are not returned by this class.
  /// </para>
  /// </remarks>
  public class DefaultServiceConfigurationDiscoveryService : IServiceConfigurationDiscoveryService
  {
    private readonly ITypeDiscoveryService _typeDiscoveryService;

    private readonly ConcurrentDictionary<Type, IReadOnlyCollection<ImplementationForAttribute>> _implementationForAttributesCache =
        new ConcurrentDictionary<Type, IReadOnlyCollection<ImplementationForAttribute>>();

    private readonly bool _excludeGlobalTypesForDefaultConfiguration = !AssemblyTypeCache.IsGacAssembly(typeof(ImplementationForAttribute).Assembly);

    [Obsolete("Use the constructor instead, and pass ContextAwareTypeUtility.GetTypeDiscoveryService() as parameter. (Version 6.0.0)", true)]
    public static DefaultServiceConfigurationDiscoveryService Create ()
    {
      throw new NotSupportedException("Use the constructor instead, and pass ContextAwareTypeUtility.GetTypeDiscoveryService() as parameter. (Version 6.0.0)");
    }

    public DefaultServiceConfigurationDiscoveryService (ITypeDiscoveryService typeDiscoveryService)
    {
      ArgumentUtility.CheckNotNull("typeDiscoveryService", typeDiscoveryService);

      _typeDiscoveryService = typeDiscoveryService;
    }

    /// <summary>
    /// Gets the default service configuration for the types returned by the given <see cref="ITypeDiscoveryService"/>.
    /// </summary>
    /// <returns>A <see cref="ServiceConfigurationEntry"/> for each serviceType that has implementations with a <see cref="ImplementationForAttribute"/> applied. 
    /// Types without the attribute are ignored.</returns>
    public IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration ()
    {
#if !FEATURE_GAC
      if (!_excludeGlobalTypesForDefaultConfiguration)
        throw new PlatformNotSupportedException("The default service configuration cannot be part of the GAC on this platform.");
#endif

      return GetDefaultConfiguration(_typeDiscoveryService.GetTypes(null, excludeGlobalTypes: _excludeGlobalTypesForDefaultConfiguration).Cast<Type>());
    }

    /// <summary>
    /// Gets the default service configuration for the given types.
    /// </summary>
    /// <param name="serviceTypes">The types to get the default service configuration for.</param>
    /// <returns>A <see cref="ServiceConfigurationEntry"/> for each serviceType that has implementations with a <see cref="ImplementationForAttribute"/> applied. 
    /// Types without the attribute are ignored.</returns>
    public IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration (IEnumerable<Type> serviceTypes)
    {
      ArgumentUtility.CheckNotNull("serviceTypes", serviceTypes);

      return serviceTypes.Select(GetDefaultConfiguration).Where(configuration => configuration.ImplementationInfos.Any());
    }

    /// <summary>
    /// Gets the default service configuration for the given types.
    /// </summary>
    /// <param name="serviceType">The type to get the default service configuration for.</param>
    /// <returns>A <see cref="ServiceConfigurationEntry"/> for each serviceType that has implementations with a <see cref="ImplementationForAttribute"/> applied. 
    /// Types without the attribute are ignored.</returns>
    public ServiceConfigurationEntry GetDefaultConfiguration (Type serviceType)
    {
      ArgumentUtility.CheckNotNull("serviceType", serviceType);

      try
      {
        var implementingTypes = GetImplementingTypes(serviceType);

        var attributes = implementingTypes
            .SelectMany(
                implementingType => GetImplementationForAttributesFromCache(implementingType)
                    .Where(attribute => attribute.ServiceType == serviceType)
                    .Select(CloneAttribute)
                    .Select(attribute => Tuple.Create(implementingType, attribute)))
            .ToLookup(a => a.Item2.RegistrationType);

        if (attributes.Contains(RegistrationType.Compound) && attributes.Contains(RegistrationType.Single))
          throw new InvalidOperationException("Registration types 'Compound' and 'Single' cannot be used together.");

        if (attributes.Contains(RegistrationType.Single) && attributes.Contains(RegistrationType.Multiple))
          throw new InvalidOperationException("Registration types 'Single' and 'Multiple' cannot be used together.");

        return ServiceConfigurationEntry.CreateFromAttributes(serviceType, FilterAttributes(attributes));
      }
      catch (Exception ex)
      {
        var message = string.Format("Invalid configuration of service type '{0}'. {1}", serviceType, ex.Message);
        throw new InvalidOperationException(message, ex);
      }
    }

    /// <summary>
    /// Gets the default service configuration for the types in the given assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies for whose types to get the default service configuration.</param>
    /// <returns>A <see cref="ServiceConfigurationEntry"/> for each type that has the <see cref="ImplementationForAttribute"/> applied. 
    /// Types without the attribute are ignored.</returns>
    public IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration (IEnumerable<Assembly> assemblies)
    {
      ArgumentUtility.CheckNotNull("assemblies", assemblies);

      return assemblies.SelectMany(a => GetDefaultConfiguration(AssemblyTypeCache.GetTypes(a)));
    }

    private IEnumerable<Type> GetImplementingTypes (Type serviceType)
    {
      Type normalizedServiceType;
      if (serviceType.IsConstructedGenericType)
        normalizedServiceType = serviceType.GetGenericTypeDefinition();
      else
        normalizedServiceType = serviceType;

      var derivedTypes = _typeDiscoveryService.GetTypes(normalizedServiceType, excludeGlobalTypes: false);
      Assertion.IsNotNull(derivedTypes, "TypeDiscoveryService evaluated for serviceType '{0}' and returned null.", serviceType);

      return derivedTypes.Cast<Type>().Where(serviceType.IsAssignableFrom);
    }

    private IReadOnlyCollection<ImplementationForAttribute> GetImplementationForAttributesFromCache (Type type)
    {
      // C# compiler 7.2 already provides caching for anonymous method.
      return _implementationForAttributesCache.GetOrAdd(
          type,
          key => AttributeUtility.GetCustomAttributes<ImplementationForAttribute>(key, false).ToArray());
    }

    private List<Tuple<Type, ImplementationForAttribute>> FilterAttributes (ILookup<RegistrationType, Tuple<Type, ImplementationForAttribute>> attributes)
    {
      var filteredAttributes = new List<Tuple<Type, ImplementationForAttribute>>();

      foreach (var registrationTypeGroup in attributes)
      {
        EnsureUniqueProperty("Implementation type", registrationTypeGroup.Select(r => r.Item1));
        EnsureUniqueProperty(
            string.Format("Position for registration type '{0}'", registrationTypeGroup.Key),
            registrationTypeGroup.Select(r => r.Item2.Position));
      }

      filteredAttributes.AddRange(attributes[RegistrationType.Single].OrderBy(a => a.Item2.Position).Take(1));
      filteredAttributes.AddRange(attributes[RegistrationType.Multiple].OrderBy(a => a.Item2.Position));
      filteredAttributes.AddRange(attributes[RegistrationType.Compound].OrderBy(a => a.Item2.Position).Take(1));
      filteredAttributes.AddRange(attributes[RegistrationType.Decorator].OrderBy(a => a.Item2.Position));
      return filteredAttributes;
    }

    private void EnsureUniqueProperty<T> (string propertyDescription, IEnumerable<T> propertyValues)
    {
      var visitedValues = new HashSet<T>();
      foreach (var value in propertyValues)
      {
        if (visitedValues.Contains(value))
        {
          var message = string.Format("Ambiguous {0}: {1} must be unique.", typeof(ImplementationForAttribute).Name, propertyDescription);
          throw new InvalidOperationException(message);
        }
        visitedValues.Add(value);
      }
    }

    private ImplementationForAttribute CloneAttribute (ImplementationForAttribute attribute)
    {
      return new ImplementationForAttribute(attribute.ServiceType)
             {
                 Lifetime = attribute.Lifetime,
                 RegistrationType = attribute.RegistrationType,
                 Position = attribute.Position
             };
    }
  }
}
