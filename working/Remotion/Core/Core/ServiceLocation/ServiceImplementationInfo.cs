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
using Remotion.Utilities;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Encapsulates a service implementation type and <see cref="LifetimeKind"/>.
  /// </summary>
  public sealed class ServiceImplementationInfo
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceImplementationInfo"/> class 
    /// with <see cref="RegistrationType"/> set to <see cref="ServiceLocation.RegistrationType.Single"/>.
    /// </summary>
    /// <param name="factory">The factory delegate that creates an instance of the service implementation.</param>
    /// <param name="lifetime">The <see cref="LifetimeKind"/> of the instances created by the <paramref name="factory"/>. Defaults to <see cref="LifetimeKind.InstancePerDependency"/>.</param>
    public static ServiceImplementationInfo CreateSingle<T> (Func<T> factory, LifetimeKind lifetime = LifetimeKind.InstancePerDependency)
        where T : class
    {
      ArgumentUtility.CheckNotNull ("factory", factory);
      return new ServiceImplementationInfo (typeof (T), lifetime, RegistrationType.Single, factory);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceImplementationInfo"/> class
    /// with <see cref="RegistrationType"/> set to <see cref="ServiceLocation.RegistrationType.Multiple"/>.
    /// </summary>
    /// <param name="factory">The factory delegate that creates an instance of the service implementation.</param>
    /// <param name="lifetime">The <see cref="LifetimeKind"/> of the instances created by the <paramref name="factory"/>. Defaults to <see cref="LifetimeKind.InstancePerDependency"/>.</param>
    public static ServiceImplementationInfo CreateMultiple<T> (Func<T> factory, LifetimeKind lifetime = LifetimeKind.InstancePerDependency)
        where T : class
    {
      ArgumentUtility.CheckNotNull ("factory", factory);
      return new ServiceImplementationInfo (typeof(T), lifetime, RegistrationType.Multiple, factory);
    }

    private readonly Func<object> _factory;
    private readonly Type _implementationType;
    private readonly LifetimeKind _lifetime;
    private readonly RegistrationType _registrationType;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceImplementationInfo"/> class.
    /// </summary>
    /// <param name="implementationType">The concrete implementation of the service type.</param>
    /// <param name="lifetime">The <see cref="LifetimeKind"/> of the instances of <paramref name="implementationType"/>. Defaults to <see cref="LifetimeKind.InstancePerDependency"/>.</param>
    /// <param name="registrationType">The <see cref="RegistrationType"/> of the <paramref name="implementationType"/>. Defaults to <see cref="T:RegistrationType.Single"/>.</param>
    public ServiceImplementationInfo (Type implementationType, LifetimeKind lifetime, RegistrationType registrationType = RegistrationType.Single)
        : this (ArgumentUtility.CheckNotNull ("implementationType", implementationType), lifetime, registrationType, null)
    {
    }

    private ServiceImplementationInfo (Type implementationType, LifetimeKind lifetime, RegistrationType registrationType, Func<object> factory)
    {
      if (registrationType == RegistrationType.Decorator && lifetime != LifetimeKind.InstancePerDependency)
        throw new ArgumentException ("For implementations of type 'Decorator', the lifetime can only be specified as 'InstancePerDependency'.", "lifetime");

      _implementationType = implementationType;
      _lifetime = lifetime;
      _registrationType = registrationType;
      _factory = factory;
    }

    /// <summary>
    /// Gets the concrete implementation type of the service.
    /// </summary>
    /// <value>The concrete implementation.</value>
    public Type ImplementationType
    {
      get { return _implementationType; }
    }

    /// <summary>
    /// Gets the lifetime of the instances of <see cref="ImplementationType"/>.
    /// </summary>
    /// <value>The lifetime of the instances.</value>
    public LifetimeKind Lifetime
    {
      get { return _lifetime; }
    }

    /// <summary>
    /// The factory delegate that creates an instance of the service implementation.
    /// </summary>
    public Func<object> Factory
    {
      get { return _factory; }
    }

    public RegistrationType RegistrationType
    {
      get { return _registrationType; }
    }

    /// <inheritdoc />
    public override string ToString ()
    {
      return string.Format ("{{{0}, {1}, {2}}}", _implementationType, _lifetime, _registrationType);
    }
  }
}