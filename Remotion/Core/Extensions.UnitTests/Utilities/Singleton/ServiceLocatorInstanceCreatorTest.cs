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
using Remotion.Development.UnitTesting;
using Remotion.Extensions.UnitTests.Utilities.Singleton.TestDomain;
using Remotion.ServiceLocation;
using Remotion.Utilities.Singleton;

namespace Remotion.Extensions.UnitTests.Utilities.Singleton
{
  [TestFixture]
  public class ServiceLocatorInstanceCreatorTest
  {
    private ServiceLocatorInstanceCreator<IInterfaceWithConcreteImplementation> _creator;

    [SetUp]
    public void SetUp ()
    {
      _creator = new ServiceLocatorInstanceCreator<IInterfaceWithConcreteImplementation>();
    }

    [Test]
    public void CreateInstance_WithRegisteredServiceLocator ()
    {
      var serviceLocatorStub = new Mock<IServiceLocator>();
      var fakeInstance = new SecondaryImplementationOfInterface();
      serviceLocatorStub.Setup(stub => stub.GetInstance<IInterfaceWithConcreteImplementation>()).Returns(fakeInstance);

      using (new ServiceLocatorScope(serviceLocatorStub.Object))
      {
        var instance = _creator.CreateInstance();
        Assert.That(instance, Is.SameAs(fakeInstance));
      }
    }

    [Test]
    public void CreateInstance_WithoutRegisteredServiceLocator_WithAttribute ()
    {
      var result = _creator.CreateInstance();
      Assert.That(result, Is.TypeOf<ConcreteImplementationOfInterface>());
    }
  }
}
