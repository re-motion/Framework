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
  /// Associated an (implementation) type with the service type (usually an interface or abstract class) 
  /// as well as its <see cref="LifetimeKind"/> and <see cref="RegistrationType"/>.
  /// This attribute is used by the <see cref="DefaultServiceLocator"/> to determine how to instantiate a service type. 
  /// Mutiple <see cref="ImplementationForAttribute"/> instances can be applied to a single implementation type. They are not inherited.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public sealed class ImplementationForAttribute : Attribute
  {
    private readonly Type _serviceType;
    private RegistrationType _registrationType;

    /// <summary>
    /// Defines a concrete implementation for a service type.
    /// </summary>
    /// <param name="serviceType">The type representing the concrete implementation for the service type.</param>
    public ImplementationForAttribute (Type serviceType)
    {
      ArgumentUtility.CheckNotNull("serviceType", serviceType);

      _serviceType = serviceType;
      Lifetime = LifetimeKind.InstancePerDependency;
      _registrationType = RegistrationType.Single;
    }

    /// <summary>
    /// Gets the type of the implemented service interface.
    /// </summary>
    /// <remarks>
    /// The attribute's implementation type must be assignable to the respective <see cref="ServiceType"/>. Incompatible types will result in an error.
    /// </remarks>
    public Type ServiceType
    {
      get { return _serviceType; }
    }

    /// <summary>
    /// Gets or sets the lifetime of instances of the concrete implementation type.
    /// </summary>
    /// <remarks>
    /// The lifetime is used by service locators to control when to reuse instances of the concrete implementation type and when to create new ones. 
    /// The default value is <see cref="LifetimeKind.InstancePerDependency"/>.
    /// In case of a decorator, specifying a lifetime other than <see cref="LifetimeKind.InstancePerDependency"/> will result in an error.
    /// </remarks>
    /// <value>The lifetime of instances of the concrete implementation type.</value>
    public LifetimeKind Lifetime { get; set; }

    /// <summary>
    /// Gets the position of the concrete implementation in the list of all concrete implementations for the respective service type. </summary>
    /// <remarks>
    /// The position does not denote the exact index; instead, it only influences the relative ordering of this implementation with respect to the other
    /// implementations. The lowest position indicates the first implementation. In case of a decorator, it's the decorator closest to the implementation.
    /// </remarks>
    /// <value>The position of the concrete implementation in the list of all concrete implementations.</value>
    public int Position  { get; set; }

    /// <summary>
    /// Gets or sets the registration type of the concrete implementation type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When using the <see cref="ServiceLocator"/>, <see cref="ServiceLocation.RegistrationType.Single"/> and <see cref="ServiceLocation.RegistrationType.Compound"/>
    /// registrations must be resolved via <see cref="IServiceLocator.GetInstance(System.Type)"/> and <see cref="ServiceLocation.RegistrationType.Multiple"/> 
    /// registrations must be resolved via <see cref="IServiceLocator.GetAllInstances(System.Type)"/>. Otherwise, an <see cref="ActivationException"/> is thrown.
    /// </para>
    /// <para>
    /// <see cref="ServiceLocation.RegistrationType.Compound"/> types will contain all <see cref="ServiceLocation.RegistrationType.Multiple"/> 
    /// registrations via a constructor parameter of type <see cref="IEnumerable{T}"/>. Note that the <see cref="Lifetime"/> of the compound type will
    /// override the lifetime of the injected types, same as all other service compositions.
    /// </para>
    /// <para>
    /// <see cref="ServiceLocation.RegistrationType.Decorator"/> types are applied to the other registration types.
    /// Note that for <see cref="ServiceLocation.RegistrationType.Compound"/> types, only the <see cref="ServiceLocation.RegistrationType.Compound"/> type will be decorated,
    /// not the contained <see cref="ServiceLocation.RegistrationType.Multiple"/> 
    /// types.
    /// </para>
    /// </remarks>
    /// <value>The registration type of the concrete implementation type.</value>
    public RegistrationType RegistrationType
    {
      get { return _registrationType; }
      set { _registrationType = value; }
    }
  }
}
