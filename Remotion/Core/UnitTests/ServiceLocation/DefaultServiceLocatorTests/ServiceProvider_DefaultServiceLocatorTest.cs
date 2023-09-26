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
using Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain;

namespace Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests
{
  [TestFixture]
  public class ServiceProvider_DefaultServiceLocatorTest : TestBase
  {
    [Test]
    public void GetService_TypeWithConcreteImplementationAttribute ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(typeof(ITestType), typeof(TestImplementation1));

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      var result = ((IServiceLocator)serviceLocator).GetService(typeof(ITestType));

      Assert.That(result, Is.TypeOf<TestImplementation1>());
    }

    [Test]
    public void GetService_TypeWithoutConcreteImplementatioAttribute ()
    {
      //TODO RM-5506: Integration Test
      var serviceConfigurationEntry = CreateMultipleServiceConfigurationEntry(typeof(ITestType), new Type[0]);

      var serviceConfigurationDiscoveryServiceMock = new Mock<IServiceConfigurationDiscoveryService>(MockBehavior.Strict);
      serviceConfigurationDiscoveryServiceMock.Setup(_ => _.GetDefaultConfiguration(typeof(ITestType))).Returns(serviceConfigurationEntry);
      var serviceLocator = CreateServiceLocator(serviceConfigurationDiscoveryServiceMock.Object);

      var result = ((IServiceLocator)serviceLocator).GetService(typeof(ITestType));

      Assert.That(result, Is.Null);
    }
  }
}
