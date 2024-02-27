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
using System.ComponentModel.Design;
using NUnit.Framework;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation;

[TestFixture]
public class BootstrapServiceConfigurationExtensionsTest
{
  [Test]
  [Ignore("RM-5193")]
  public void Deserialization_SpecificRootAssemblies_ByName ()
  {
    /*
     <specificRootAssemblies>
        <byName>
          <include name="Moq"/>
        </byName>
      </specificRootAssemblies>
     */

    IBootstrapServiceConfiguration bootstrapServiceConfiguration = new BootstrapServiceConfiguration();
    BootstrapServiceConfigurationExtensions.RegisterSpecificRootAssemblies(bootstrapServiceConfiguration);

    var de = new DefaultServiceLocatorProvider(new BootstrapServiceConfigurationDiscoveryService());
    var serviceLocator = de.GetServiceLocator(bootstrapServiceConfiguration.GetRegistrations());
    var typeDiscoveryServices = serviceLocator.GetInstance<ITypeDiscoveryService>();

    var types = typeDiscoveryServices.GetTypes(null, false);

    Assert.That(types, Has.Member(typeof(Moq.Capture)));
  }

  [Test]
  [Ignore("RM-5193")]
  public void Deserialization_SpecificRootAssemblies_ByFilePattern ()
  {
    /*
      <specificRootAssemblies>
        <byFile>
          <include filePattern=""" + GetTestAssemblyName() + @".*""/>
        </byFile>
      </specificRootAssemblies>
     */

    ApplicationAssemblyLoaderFilter.Instance.AddIgnoredAssembly(GetTestAssemblyName());
    try
    {
      IBootstrapServiceConfiguration bootstrapServiceConfiguration = new BootstrapServiceConfiguration();
      BootstrapServiceConfigurationExtensions.RegisterSpecificRootAssemblies(bootstrapServiceConfiguration);

      var de = new DefaultServiceLocatorProvider(new BootstrapServiceConfigurationDiscoveryService());
      var serviceLocator = de.GetServiceLocator(bootstrapServiceConfiguration.GetRegistrations());
      var typeDiscoveryServices = serviceLocator.GetInstance<ITypeDiscoveryService>();

      var types = typeDiscoveryServices.GetTypes(null, false);

      Assert.That(types, Has.Member(typeof(BootstrapServiceConfigurationExtensionsTest)));
    }
    finally
    {
      ApplicationAssemblyLoaderFilter.Instance.Reset();
    }
  }

  private static string GetTestAssemblyName ()
  {
    return typeof(BootstrapServiceConfigurationExtensionsTest).Assembly.GetName().Name;
  }
}
