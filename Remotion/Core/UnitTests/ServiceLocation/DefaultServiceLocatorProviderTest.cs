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
using Moq;
using NUnit.Framework;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class DefaultServiceLocatorProviderTest
  {
    [Test]
    public void GetServiceLocator ()
    {
      var serviceConfigurationDiscoveryServiceStub = Mock.Of<IServiceConfigurationDiscoveryService>(MockBehavior.Strict);
      var provider = new DefaultServiceLocatorProvider(serviceConfigurationDiscoveryServiceStub);
      var sloc = provider.GetServiceLocator(Array.AsReadOnly(new ServiceConfigurationEntry[0]));

      Assert.That(sloc, Is.TypeOf<DefaultServiceLocator>());
      Assert.That(((DefaultServiceLocator)sloc).ServiceConfigurationDiscoveryService, Is.SameAs(serviceConfigurationDiscoveryServiceStub));
    }

    [Test]
    public void GetServiceLocator_IncludesBootstrapConfiguration ()
    {
      var entry = new ServiceConfigurationEntry(typeof(IService), new ServiceImplementationInfo(typeof(Service), LifetimeKind.InstancePerDependency));

      var serviceConfigurationDiscoveryServiceStub = Mock.Of<IServiceConfigurationDiscoveryService>(MockBehavior.Strict);
      var provider = new DefaultServiceLocatorProvider(serviceConfigurationDiscoveryServiceStub);
      var sloc = provider.GetServiceLocator(Array.AsReadOnly(new[] { entry }));

      Assert.That(sloc, Is.TypeOf<DefaultServiceLocator>());
      Assert.That(sloc.GetInstance<IService>(), Is.Not.Null.And.TypeOf<Service>());
    }

    public interface IService { }
    public class Service : IService { }
  }
}
