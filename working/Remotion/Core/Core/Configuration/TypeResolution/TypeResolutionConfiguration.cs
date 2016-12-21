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
using System.Configuration;
using Remotion.Configuration.TypeDiscovery;
using Remotion.Reflection;
using Remotion.Reflection.TypeResolution;
using Remotion.Utilities;

namespace Remotion.Configuration.TypeResolution
{
  /// <summary>
  /// Configures the type discovery performed by <see cref="ContextAwareTypeUtility.GetTypeResolutionService"/>.
  /// </summary>
  /// <remarks>This configuration should be aligned with <see cref="TypeDiscoveryConfiguration"/>. See https://www.re-motion.org/jira/browse/RM-6413</remarks>
  public sealed class TypeResolutionConfiguration : ConfigurationSection
  {
    private static readonly DoubleCheckedLockingContainer<TypeResolutionConfiguration> s_current =
        new DoubleCheckedLockingContainer<TypeResolutionConfiguration> (GetTypeResolutionConfiguration);

    /// <summary>
    /// Gets the current <see cref="TypeResolutionConfiguration"/> instance. This is used by 
    /// <see cref="ContextAwareTypeUtility.GetTypeDiscoveryService"/> to retrieve a <see cref="ITypeDiscoveryService"/> instance if
    /// <see cref="DesignerUtility.IsDesignMode"/> is not set to <see langword="true" />.
    /// </summary>
    /// <value>The current <see cref="TypeResolutionConfiguration"/>.</value>
    public static TypeResolutionConfiguration Current
    {
      get { return s_current.Value; }
    }

    /// <summary>
    /// Sets the <see cref="Current"/> <see cref="TypeResolutionConfiguration"/> instance.
    /// </summary>
    /// <param name="configuration">The new configuration to set as the <see cref="Current"/> configuration.</param>
    public static void SetCurrent (TypeResolutionConfiguration configuration)
    {
      s_current.Value = configuration;
    }

    private static TypeResolutionConfiguration GetTypeResolutionConfiguration ()
    {
      // return (TypeResolutionConfiguration) ConfigurationWrapper.Current.GetSection ("remotion.typeResolution", false) ?? new TypeResolutionConfiguration();
      return new TypeResolutionConfiguration (new DefaultTypeResolutionService());
    }

    private readonly ITypeResolutionService _typeResolutionService;

    public TypeResolutionConfiguration (ITypeResolutionService typeResolutionService)
    {
      ArgumentUtility.CheckNotNull ("typeResolutionService", typeResolutionService);
      
      _typeResolutionService = typeResolutionService;
    }

    public ITypeResolutionService CreateTypeResolutionService ()
    {
      return _typeResolutionService;
    }
  }
}