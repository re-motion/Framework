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
using Remotion.Configuration.ServiceLocation;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// <see cref="SafeServiceLocator"/> is intended as a wrapper for <see cref="ServiceLocator"/>, specifically the 
  /// <see cref="ServiceLocator.Current"/> property. In contrast to <see cref="ServiceLocator"/>, <see cref="SafeServiceLocator"/> will never throw
  /// a <see cref="NullReferenceException"/> but instead register an default <see cref="IServiceLocator"/> instance if no custom service locator was
  /// registered.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Accessing <see cref="ServiceLocator"/> will always lead to a <see cref="NullReferenceException"/> if no service locator is 
  /// configured. Using <see cref="SafeServiceLocator"/> instead will catch the exception and register a default <see cref="IServiceLocator"/> instance.
  /// A provider for the default instance can be defined in the application configuration file (handled by 
  /// <see cref="ServiceLocationConfiguration"/>). The provider needs to implement <see cref="IServiceLocatorProvider"/> and must have a default 
  /// constructor.
  /// <code>
  /// &lt;?xml version="1.0" encoding="utf-8" ?&gt;
  /// &lt;configuration&gt;
  ///   &lt;configSections&gt;
  ///     &lt;section name="remotion.serviceLocation" type="Remotion.Configuration.ServiceLocation.ServiceLocationConfiguration,Remotion" /&gt;
  ///   &lt;/configSections&gt;
  /// 
  ///   &lt;remotion.serviceLocation xmlns="http://www.re-motion.org/serviceLocation/configuration"&gt;
  ///     &lt;serviceLocatorProvider type="MyAssembly::MyServiceLocatorProvider"/&gt;
  ///   &lt;/remotion.serviceLocation&gt;
  /// &lt;/configuration&gt;
  /// </code>
  /// </para>
  /// <para>
  /// If no provider is configured, a <see cref="DefaultServiceLocator"/> instance is used as the default instance.
  /// </para>
  /// </remarks>
  public static class SafeServiceLocator
  {
    /// <summary>Workaround to allow reflection to reset the fields since setting a static readonly field is not supported in .NET 3.0 and later.</summary>
    private class Fields
    {
      public readonly BootstrapServiceConfiguration BootstrapConfiguration = new();
      public readonly DoubleCheckedLockingContainer<IServiceLocator> DefaultServiceLocator = new(GetDefaultServiceLocator);
    }

    private static readonly Fields s_fields = new();

    /// <summary>
    /// Gets the currently configured <see cref="IServiceLocator"/>. 
    /// If no service locator is configured or <see cref="ServiceLocator.Current"/> returns <see langword="null" />, 
    /// a default <see cref="IServiceLocator"/> will be returned as defined by the <see cref="ServiceLocationConfiguration"/>.
    /// </summary>
    /// <remarks>
    /// When this property is accessed while the default <see cref="IServiceLocator"/> instance is built (triggered through an outer access to this
    /// property), it will return a bootstrapping service locator that behaves just like <see cref="DefaultServiceLocator"/>. This means that the
    /// code building the default <see cref="IServiceLocator"/> can indeed access the <see cref="Current"/> property without triggering any
    /// exceptions. To configure the bootstrapping <see cref="IServiceLocator"/>, use the <see cref="BootstrapConfiguration"/> property.
    /// </remarks>
    public static IServiceLocator Current
    {
      get
      {
        if (ServiceLocator.IsLocationProviderSet)
          return ServiceLocator.Current ?? s_fields.DefaultServiceLocator.Value;

        ServiceLocator.SetLocatorProvider(() => s_fields.DefaultServiceLocator.Value);
        return s_fields.DefaultServiceLocator.Value;
      }
    }

    /// <summary>
    /// Allows clients to register services that are available while the <see cref="IServiceLocator"/> is built.
    /// These service registrations are also passed on to the <see cref="IServiceLocatorProvider"/> so that they can be incorporated in the
    /// final <see cref="IServiceLocator"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When the <see cref="Current"/> property is accessed for the first time, and no provider has been set  via 
    /// <see cref="ServiceLocator.SetLocatorProvider"/>, a default <see cref="IServiceLocator"/> is built as defined by the 
    /// <see cref="ServiceLocationConfiguration"/>. Building the default provider, which may include construction of an IoC container, can be a
    /// complex operation. When the code building the default locator accesses the <see cref="Current"/> property, it will get a reference to a
    /// bootstrapping <see cref="IServiceLocator"/>, which behaves just like the <see cref="DefaultServiceLocator"/> (i.e., it evaluates
    /// <see cref="ImplementationForAttribute"/> declarations and such).
    /// The <see cref="BootstrapConfiguration"/> allows clients to register additional services with the bootstrapping <see cref="IServiceLocator"/>.
    /// </para>
    /// </remarks>
    public static IBootstrapServiceConfiguration BootstrapConfiguration
    {
      get { return s_fields.BootstrapConfiguration; }
    }

    private static IServiceLocator GetDefaultServiceLocator ()
    {
      var bootstrapServiceLocatorEntries = s_fields.BootstrapConfiguration.GetRegistrations();

      // Temporarily set the bootstrapper to allow for reentrancy to SafeServiceLocator.Current.
      // Since we're called from s_defaultServiceLocator.Value's getter, we can be sure that our return value will overwrite the bootstrapper.
      var bootstrapServiceLocatorProvider = new DefaultServiceLocatorProvider();
      var bootstrapServiceLocator = bootstrapServiceLocatorProvider.GetServiceLocator(bootstrapServiceLocatorEntries);
      s_fields.DefaultServiceLocator.Value = bootstrapServiceLocator;

      var serviceLocatorProvider = ServiceLocationConfiguration.Current.CreateServiceLocatorProvider();
      return serviceLocatorProvider.GetServiceLocator(bootstrapServiceLocatorEntries);
    }
  }
}
