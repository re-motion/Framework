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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Holds the parameters used by <see cref="DefaultServiceLocator"/> for instantiating instances of service types. Use 
  /// <see cref="DefaultServiceConfigurationDiscoveryService"/> to retrieve the <see cref="ServiceConfigurationEntry"/> data for a specific type.
  /// </summary>
  public sealed class ServiceConfigurationEntry
  {
    /// <summary>
    /// Creates a <see cref="ServiceConfigurationEntry"/> from a <see cref="ImplementationForAttribute"/>.
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <param name="attributes">Tuples holding information about the concrete type implementing <paramref name="serviceType"/> as well as the attribute instance.</param>
    /// <returns>A <see cref="ServiceConfigurationEntry"/> containing the data from the <paramref name="attributes"/>.</returns>
    public static ServiceConfigurationEntry CreateFromAttributes (Type serviceType, IEnumerable<Tuple<Type, ImplementationForAttribute>> attributes)
    {
      ArgumentUtility.CheckNotNull("serviceType", serviceType);
      ArgumentUtility.CheckNotNull("attributes", attributes);

      var attributesAndResolvedTypes =
          (from attribute in attributes
           orderby attribute.Item2.Position
           select new { Attribute = attribute.Item2, ResolvedType = attribute.Item1}).ConvertToCollection();

      var serviceImplementationInfos =
          attributesAndResolvedTypes
              .ApplySideEffect(tuple => CheckImplementationType(serviceType, tuple.ResolvedType, s => new InvalidOperationException(s)))
              .Select(tuple => new ServiceImplementationInfo(tuple.ResolvedType, tuple.Attribute.Lifetime, tuple.Attribute.RegistrationType));

      return new ServiceConfigurationEntry(serviceType, serviceImplementationInfos);
    }

    private static void CheckImplementationType (Type serviceType, Type implementationType, Func<string, Exception> exceptionFactory)
    {
      if (!serviceType.IsAssignableFrom(implementationType))
      {
        var message = string.Format("The implementation type '{0}' does not implement the service type.", implementationType);
        throw exceptionFactory(message);
      }
    }

    private readonly Type _serviceType;
    private readonly ReadOnlyCollection<ServiceImplementationInfo> _implementationInfos;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceConfigurationEntry"/> class.
    /// </summary>
    /// <param name="serviceType">The service type. This is a type for which instances are requested from a service locator.</param>
    /// <param name="implementationInfos">The <see cref="ServiceImplementationInfo"/> for the <paramref name="serviceType" />.</param>
    public ServiceConfigurationEntry (Type serviceType, params ServiceImplementationInfo[] implementationInfos)
        : this(serviceType, (IEnumerable<ServiceImplementationInfo>)implementationInfos)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceConfigurationEntry"/> class.
    /// </summary>
    /// <param name="serviceType">The service type. This is a type for which instances are requested from a service locator.</param>
    /// <param name="implementationInfos">The service implementation information.</param>
    public ServiceConfigurationEntry (Type serviceType, IEnumerable<ServiceImplementationInfo> implementationInfos)
    {
      ArgumentUtility.CheckNotNull("serviceType", serviceType);
      ArgumentUtility.CheckNotNull("implementationInfos", implementationInfos);

      _serviceType = serviceType;
      var checkedImplementationInfos =
          implementationInfos.ApplySideEffect(
              info => CheckImplementationType(serviceType, info.ImplementationType, message => new ArgumentException(message, "implementationInfos")));
      _implementationInfos = Array.AsReadOnly(checkedImplementationInfos.ToArray());
    }

    /// <summary>
    /// Gets the service type. This is a type for which instances are requested from a service locator.
    /// </summary>
    /// <value>The service type.</value>
    public Type ServiceType
    {
      get { return _serviceType; }
    }

    /// <summary>
    /// Gets information about all service implementations.
    /// </summary>
    /// <value>A collection of <see cref="ServiceImplementationInfo"/> instances.</value>
    /// <remarks>
    /// When this information is used to configure an implementation of <see cref="IServiceLocator"/>, <see cref="IServiceLocator.GetAllInstances"/>
    /// must return the implementing instances in exactly the same order as defined by <see cref="ImplementationInfos"/>.
    /// </remarks>
    public ReadOnlyCollection<ServiceImplementationInfo> ImplementationInfos
    {
      get { return _implementationInfos; }
    }

    /// <inheritdoc />
    public override string ToString ()
    {
      var implementationInfos = _implementationInfos.Select(i => i.ToString()).ToArray();
      var joinedImplementationInfos = string.Join(", ", implementationInfos);
      return string.Format("{0} implementations: [{1}]", _serviceType, joinedImplementationInfos);
    }
  }
}
