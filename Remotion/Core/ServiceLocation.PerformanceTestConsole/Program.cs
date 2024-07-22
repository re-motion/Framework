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
using System.Linq;
using log4net.Config;
using Remotion.Utilities;

namespace Remotion.ServiceLocation.PerformanceTestConsole
{
  internal static class Program
  {
    private static void Main (string[] args)
    {
      Console.WriteLine("{0} - Application started", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss,fff"));
      XmlConfigurator.Configure();

      var bootstrapServiceLocatorEntries = SafeServiceLocator.BootstrapConfiguration.GetRegistrations();
      var provider = new DefaultServiceLocatorProvider(new BootstrapServiceConfigurationDiscoveryService());
      var bootstrapServiceLocator = provider.GetServiceLocator(bootstrapServiceLocatorEntries);
      var typeDiscoveryService = bootstrapServiceLocator.GetInstance<ITypeDiscoveryService>();

      var domainObjectType = Type.GetType("Remotion.Data.DomainObjects.DomainObject, Remotion.Data.DomainObjects", true, false);
      typeDiscoveryService.GetTypes(domainObjectType, false);
      typeDiscoveryService.GetTypes(domainObjectType, false);

      using (StopwatchScope.CreateScope(Console.Out, "Reading IoC Configuration: {elapsed}"))
      {
        var p = new DefaultServiceConfigurationDiscoveryService(typeDiscoveryService);
        p.GetDefaultConfiguration().ToList();
      }
    }
  }
}
