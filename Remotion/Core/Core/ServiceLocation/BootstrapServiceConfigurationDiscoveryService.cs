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
using System.Reflection;
using Remotion.Reflection;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Implementation of the <see cref="IServiceConfigurationDiscoveryService"/> used only to satisfy a mandatory parameter requirement
  /// when instantiating the <see cref="DefaultServiceLocatorProvider"/> in a bootstrapping context.
  /// </summary>
  internal class BootstrapServiceConfigurationDiscoveryService : IServiceConfigurationDiscoveryService
  {
    public BootstrapServiceConfigurationDiscoveryService ()
    {
    }

    public IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration ()
    {
      throw new NotSupportedException($"The {nameof(BootstrapServiceConfigurationDiscoveryService)} does not support the automatic composition of the service configuration.");
    }

    public IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration (IEnumerable<Type> serviceTypes)
    {
      throw new NotSupportedException($"The {nameof(BootstrapServiceConfigurationDiscoveryService)} does not support the automatic composition of the service configuration.");
    }

    public IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration (IEnumerable<Assembly> assemblies)
    {
      throw new NotSupportedException($"The {nameof(BootstrapServiceConfigurationDiscoveryService)} does not support the automatic composition of the service configuration.");
    }

    public ServiceConfigurationEntry GetDefaultConfiguration (Type serviceType)
    {
      throw new NotSupportedException(
          $"The {nameof(BootstrapServiceConfigurationDiscoveryService)} does not support the automatic composition of the service configuration for a type. "
          + $"Register the requested type '{serviceType.GetFullNameSafe()}' via {nameof(SafeServiceLocator)}.{nameof(SafeServiceLocator.BootstrapConfiguration)} "
          + $"before resolving an instance of the type.");
    }
  }
}
