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
using NUnit.Framework;
using Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain;

namespace Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests
{
  [TestFixture]
  public class GenericService_DefaultServiceLocatorTest : TestBase
  {
    [Test]
    public void GetInstance_TypeWithGenericServiceInterface_AndImplementationClosedDuringDefinition ()
    {
      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register (
          CreateSingleServiceConfigurationEntry (typeof (ITestGeneric<int>), typeof (TestGenericImplementationClosedWithInt)));
      serviceLocator.Register (
          CreateSingleServiceConfigurationEntry (typeof (ITestGeneric<string>), typeof (TestGenericImplementationClosedWithString)));

      Assert.That (serviceLocator.GetInstance (typeof (ITestGeneric<int>)), Is.TypeOf<TestGenericImplementationClosedWithInt>());
      Assert.That (serviceLocator.GetInstance (typeof (ITestGeneric<string>)), Is.TypeOf<TestGenericImplementationClosedWithString>());
    }

    [Test]
    public void GetInstance_TypeWithGenericServiceInterface_AndImplementationClosedDuringRegistration ()
    {
      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register (
          CreateSingleServiceConfigurationEntry (typeof (ITestGeneric<int>), typeof (TestGenericImplementation<int>)));
      serviceLocator.Register (
          CreateSingleServiceConfigurationEntry (typeof (ITestGeneric<string>), typeof (TestGenericImplementation<string>)));

      Assert.That (serviceLocator.GetInstance (typeof (ITestGeneric<int>)), Is.TypeOf<TestGenericImplementation<int>>());
      Assert.That (serviceLocator.GetInstance (typeof (ITestGeneric<string>)), Is.TypeOf<TestGenericImplementation<string>>());
    }
  }
}