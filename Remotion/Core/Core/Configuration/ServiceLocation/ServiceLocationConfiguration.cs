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
using System.Configuration;
using Remotion.ServiceLocation;

namespace Remotion.Configuration.ServiceLocation
{
  /// <summary>
  /// Configures the service location performed by <see cref="SafeServiceLocator"/>.
  /// </summary>
  public sealed class ServiceLocationConfiguration : ConfigurationSection, IServiceLocationConfiguration
  {
    private static readonly DoubleCheckedLockingContainer<IServiceLocationConfiguration> s_current =
        new DoubleCheckedLockingContainer<IServiceLocationConfiguration>(GetServiceLocationConfiguration);

    /// <summary>
    /// Gets the current <see cref="IServiceLocationConfiguration"/> instance. This is used by 
    /// <see cref="SafeServiceLocator.Current"/> to retrieve an <see cref="IServiceLocatorProvider"/> instance if no specific 
    /// <see cref="IServiceLocator"/> was configured via <see cref="ServiceLocator.SetLocatorProvider"/>.
    /// </summary>
    /// <value>The current <see cref="IServiceLocationConfiguration"/>.</value>
    public static IServiceLocationConfiguration Current
    {
      get { return s_current.Value; }
    }

    /// <summary>
    /// Sets the <see cref="Current"/> <see cref="IServiceLocationConfiguration"/> instance.
    /// </summary>
    /// <param name="configuration">The new configuration to set as the <see cref="Current"/> configuration.</param>
    public static void SetCurrent (IServiceLocationConfiguration? configuration)
    {
      s_current.Value = configuration!;
    }

    private static ServiceLocationConfiguration GetServiceLocationConfiguration ()
    {
      return (ServiceLocationConfiguration)(ConfigurationWrapper.Current.GetSection("remotion.serviceLocation", false) ?? new ServiceLocationConfiguration());
    }

    /// <summary>
    /// Initializes a new default instance of the <see cref="ServiceLocationConfiguration"/> class. To load the configuration from a config file,
    /// use <see cref="ConfigurationWrapper.GetSection(string)"/> instead.
    /// </summary>
    public ServiceLocationConfiguration ()
    {
      var xmlnsProperty = new ConfigurationProperty("xmlns", typeof(string), null, ConfigurationPropertyOptions.None);
      Properties.Add(xmlnsProperty);
    }

    /// <summary>
    /// Gets a <see cref="TypeElement{TBase}"/> describing the custom <see cref="IServiceLocatorProvider"/> to be used. This provider defines
    /// the <see cref="IServiceLocator"/> to be used by <see cref="SafeServiceLocator.Current"/> if no custom <see cref="IServiceLocator"/> was
    /// configured using <see cref="ServiceLocator.SetLocatorProvider"/>.
    /// </summary>
    /// <value>A <see cref="TypeElement{TBase}"/> describing the custom <see cref="IServiceLocatorProvider"/> type to be used.</value>
    [ConfigurationProperty("serviceLocatorProvider", IsRequired = false)]
    public TypeElement<IServiceLocatorProvider, DefaultServiceLocatorProvider> ServiceLocatorProvider
    {
      get { return (TypeElement<IServiceLocatorProvider, DefaultServiceLocatorProvider>)this["serviceLocatorProvider"]; }
    }

    /// <summary>
    /// Creates an <see cref="IServiceLocatorProvider"/> instance as indicated by <see cref="ServiceLocatorProvider"/>. If no 
    /// <see cref="ServiceLocatorProvider"/> is set, an instance of <see cref="DefaultServiceLocatorProvider"/> is returned.
    /// </summary>
    /// <returns>An new <see cref="IServiceLocatorProvider"/> instance.</returns>
    public IServiceLocatorProvider CreateServiceLocatorProvider ()
    {
      // TODO RM-7432: CreateInstance should not be nullable
      return ServiceLocatorProvider.CreateInstance()!;
    }
  }
}
