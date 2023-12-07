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
using System.ComponentModel.Design;
using System.Threading;
using Remotion.Configuration.TypeDiscovery;
using Remotion.ServiceLocation;

namespace Remotion.Reflection
{
  /// <summary>
  /// Represents the central entry point into re-motion's type discovery mechanism. All components requiring type discovery should use this class and
  /// its <see cref="GetTypeDiscoveryService"/> method.
  /// </summary>
  /// <threadsafety static="true" instance="false"/>
  public static class ContextAwareTypeUtility
  {
    /// <summary>Workaround to allow reflection to reset the fields since setting a static readonly field is not supported in .NET 3.0 and later.</summary>
    private class Fields
    {
      public readonly Lazy<ITypeDiscoveryService> DefaultTypeDiscoveryService =
          new Lazy<ITypeDiscoveryService>(
              () => TypeDiscoveryConfiguration.Current.CreateTypeDiscoveryService(),
              LazyThreadSafetyMode.ExecutionAndPublication);
    }

    private static readonly Fields s_fields = new Fields();

    /// <summary>
    /// Gets the current context-specific <see cref="ITypeDiscoveryService"/>. If an <see cref="T:System.ComponentModel.Design.IDesignerHost"/> is available,
    /// the designer's <see cref="ITypeDiscoveryService"/> is returned. Otherwise, the <see cref="T:Remotion.Configuration.TypeDiscovery.TypeDiscoveryConfiguration"/> 
    /// is used to create a new  <see cref="ITypeDiscoveryService"/> when the property is first retrieved. That instance is stored for later uses.
    /// </summary>
    /// <returns>The current context-specific <see cref="ITypeDiscoveryService"/>.</returns>
    public static ITypeDiscoveryService GetTypeDiscoveryService ()
    {
      // Here you could choose to get the ITypeDiscoveryService from IDesignerHost.GetService (typeof (ITypeDiscoveryService)) instead of the resolved one.

      return s_fields.DefaultTypeDiscoveryService.Value;
    }

    /// <summary>
    /// Gets the <see cref="ITypeResolutionService"/> from the <see cref="SafeServiceLocator"/>.
    /// </summary>
    [Obsolete("Retrieve via the application's IoC container, e.g. SafeServiceLocator.Current.GetInstance<ITypeResolutionService>(). (Version 6.0.0)")]
    public static ITypeResolutionService GetTypeResolutionService ()
    {
      // Here you could choose to get the ITypeResolutionService from IDesignerHost.GetService (typeof (ITypeResolutionService)) instead of the resolved one.

      return SafeServiceLocator.Current.GetInstance<ITypeResolutionService>();
    }
  }
}
