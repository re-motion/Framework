using System;
using System.Collections.Generic;
using System.Reflection;

namespace Remotion.ServiceLocation
{
  public interface IServiceConfigurationDiscoveryService
  {
    IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration ();
    IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration (IEnumerable<Type> serviceTypes);
    IEnumerable<ServiceConfigurationEntry> GetDefaultConfiguration (IEnumerable<Assembly> assemblies);
    ServiceConfigurationEntry GetDefaultConfiguration (Type serviceType);
  }
}