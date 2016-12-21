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
using Remotion.Configuration.TypeResolution;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Represents the central entry point into re-motion's type discovery mechanism. All components requiring type discovery should use this class and
  /// its <see cref="GetTypeDiscoveryService"/> method.
  /// </summary>
  /// <threadsafety static="true" instance="false"/>
  public static class ContextAwareTypeUtility
  {
    private static readonly Lazy<ITypeDiscoveryService> s_defaultTypeDiscoveryService =
        new Lazy<ITypeDiscoveryService> (TypeDiscoveryConfiguration.Current.CreateTypeDiscoveryService, 
          LazyThreadSafetyMode.ExecutionAndPublication);

    private static readonly Lazy<ITypeResolutionService> s_defaultTypeResolutionService =
        new Lazy<ITypeResolutionService> (TypeResolutionConfiguration.Current.CreateTypeResolutionService, 
          LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Gets the current context-specific <see cref="ITypeDiscoveryService"/>. If <see cref="DesignerUtility.IsDesignMode"/> is set to 
    /// <see langword="true" />, the designer's <see cref="ITypeDiscoveryService"/> is returned. Otherwise, the <see cref="T:Remotion.Configuration.TypeDiscovery.TypeDiscoveryConfiguration"/> 
    /// is used to create a new  <see cref="ITypeDiscoveryService"/> when the property is first retrieved. That instance is stored for later uses.
    /// </summary>
    /// <returns>The current context-specific <see cref="ITypeDiscoveryService"/>.</returns>
    public static ITypeDiscoveryService GetTypeDiscoveryService ()
    {
      if (DesignerUtility.IsDesignMode)
        return (ITypeDiscoveryService) DesignerUtility.DesignModeHelper.DesignerHost.GetService (typeof (ITypeDiscoveryService));

      return s_defaultTypeDiscoveryService.Value;
    }

    /// <summary>
    /// Gets the current context-specific <see cref="ITypeResolutionService"/>. If <see cref="DesignerUtility.IsDesignMode"/> is set to 
    /// <see langword="true" />, the designer's <see cref="ITypeResolutionService"/> is returned. Otherwise, the <see cref="T:Remotion.Configuration.TypeDiscovery.TypeDiscoveryConfiguration"/> 
    /// is used to create a new  <see cref="ITypeResolutionService"/> when the property is first retrieved. That instance is stored for later uses.
    /// </summary>
    /// <returns>The current context-specific <see cref="ITypeResolutionService"/>.</returns>
    public static ITypeResolutionService GetTypeResolutionService ()
    {
      if (DesignerUtility.IsDesignMode)
        return (ITypeResolutionService) DesignerUtility.DesignModeHelper.DesignerHost.GetService (typeof (ITypeResolutionService));

      return s_defaultTypeResolutionService.Value;
    }
  }
}
