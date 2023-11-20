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
  public class Single_DefaultServiceLocatorTest : TestBase
  {
    [Test]
    public void GetInstance_LookUpViaServiceConfigurationDiscoveryService_InstantiatesImplementation ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(typeof(ITestType), typeof(TestImplementation1));
      var serviceConfigurationDiscoveryServiceStub = new Mock<IServiceConfigurationDiscoveryService>(MockBehavior.Strict);
      serviceConfigurationDiscoveryServiceStub.Setup(_=>_.GetDefaultConfiguration(typeof(ITestType))).Returns(serviceConfigurationEntry);

      var serviceLocator = CreateServiceLocator(serviceConfigurationDiscoveryServiceStub.Object);

      var instance = serviceLocator.GetInstance(typeof(ITestType));

      Assert.That(instance, Is.TypeOf<TestImplementation1>());
    }

    [Test]
    public void GetInstance_InstantiatesService ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(typeof(ITestType), typeof(TestImplementation1));

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      object instance = serviceLocator.GetInstance(typeof(ITestType));

      Assert.That(instance, Is.TypeOf<TestImplementation1>());
    }

    [Test]
    public void GetInstance_GenericOverload_InstantiatesImplementation ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(typeof(ITestType), typeof(TestImplementation1));

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      ITestType result = serviceLocator.GetInstance<ITestType>();

      Assert.That(result, Is.TypeOf(typeof(TestImplementation1)));
    }

    [Test]
    public void GetInstance_WithInstancePerDependencyLifetimeKind_ReturnsNotSameInstancesForAServiceType ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(
          typeof(ITestType),
          typeof(TestImplementation1),
          LifetimeKind.InstancePerDependency);

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      var instance1 = serviceLocator.GetInstance(typeof(ITestType));
      var instance2 = serviceLocator.GetInstance(typeof(ITestType));

      Assert.That(instance1, Is.Not.SameAs(instance2));
    }

    [Test]
    public void GetInstance_WithSingletonLifetimeKind_ReturnsSameInstancesForAServiceType ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(typeof(ITestType), typeof(TestImplementation1), LifetimeKind.Singleton);

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      var instance1 = serviceLocator.GetInstance(typeof(ITestType));
      var instance2 = serviceLocator.GetInstance(typeof(ITestType));

      Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void GetInstance_WithSingletonLifetimeKind_AndSingletonIsLazyInitialized ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(
          typeof(ITestType),
          typeof(TestImplementationWithOneConstructorParameter),
          LifetimeKind.Singleton);

      var serviceLocator = CreateServiceLocator();
      Assert.That(() => serviceLocator.Register(serviceConfigurationEntry), Throws.Nothing);
    }

    [Test]
    public void GetInstance_ImplementationIsRegisteredAsFactoryWithInstanceLifetime ()
    {
      TestImplementation1 expectedInstance = null;
      var serviceImplementation = ServiceImplementationInfo.CreateSingle(
          () => expectedInstance = new TestImplementation1(),
          LifetimeKind.InstancePerDependency);
      var serviceConfigurationEntry = new ServiceConfigurationEntry(typeof(ITestType), serviceImplementation);

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      var instance1 = serviceLocator.GetInstance(typeof(ITestType));
      Assert.That(expectedInstance, Is.Not.Null);

      var instance2 = serviceLocator.GetInstance(typeof(ITestType));
      Assert.That(instance1, Is.Not.SameAs(instance2));
    }

    [Test]
    public void GetInstance_ImplementationIsRegisteredAsFactoryWithSingletonLifetime_AndSingletonIsLazyInitialized ()
    {
      TestImplementation1 expectedInstance = null;
      var serviceImplementation = ServiceImplementationInfo.CreateSingle(() => expectedInstance = new TestImplementation1(), LifetimeKind.Singleton);
      var serviceConfigurationEntry = new ServiceConfigurationEntry(typeof(ITestType), serviceImplementation);

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      Assert.That(expectedInstance, Is.Null);
      var instance1 = serviceLocator.GetInstance(typeof(ITestType));
      Assert.That(expectedInstance, Is.Not.Null);

      var instance2 = serviceLocator.GetInstance(typeof(ITestType));
      Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void GetInstance_WithKeyParameter_KeyIsIgnored ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(typeof(ITestType), typeof(TestImplementation1));

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      var result = serviceLocator.GetInstance(typeof(ITestType), "Test");

      Assert.That(result, Is.TypeOf(typeof(TestImplementation1)));
    }

    [Test]
    public void GetInstance_Generic_WithKeyParameter_KeyIsIgnored ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(typeof(ITestType), typeof(TestImplementation1));

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      var result = serviceLocator.GetInstance<ITestType>("Test");

      Assert.That(result, Is.TypeOf(typeof(TestImplementation1)));
    }

    [Test]
    public void GetAllInstances_ThrowsActivationException ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(
          typeof(ITestType),
          typeof(TestImplementation1));

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      Assert.That(
          () => serviceLocator.GetAllInstances(typeof(ITestType)),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo(
              "A single implementation is configured for service type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType'. "
              + "Use GetInstance() to retrieve the implementation."));
    }

    [Test]
    public void GetInstance_ConstructorThrowingException_ExceptionIsWrappedInActivationException ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(
          typeof(ITestTypeWithErrors),
          typeof(TestTypeWithConstructorThrowingException));

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      var exception = Assert.Throws<ActivationException>(() => serviceLocator.GetInstance(typeof(ITestTypeWithErrors)));
      Assert.That(exception.Message, Is.EqualTo("ApplicationException: This exception comes from the ctor."));
      Assert.That(exception.InnerException, Is.TypeOf<ApplicationException>());
      Assert.That(exception.InnerException.Message, Is.EqualTo("This exception comes from the ctor."));
    }

    [Test]
    public void GetInstance_ServiceTypeWithFactoryReturningNull_ThrowsActivationException ()
    {
      var implementation = ServiceImplementationInfo.CreateSingle<ITestTypeWithErrors>(() => null);
      var serviceConfigurationEntry = new ServiceConfigurationEntry(typeof(ITestTypeWithErrors), implementation);

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      Assert.That(
          () => serviceLocator.GetInstance(typeof(ITestTypeWithErrors)),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo(
              "The registered factory returned null instead of an instance implementing the requested service type "
              + "'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestTypeWithErrors'."));
    }

    [Test]
    public void GetInstance_ServiceTypeWithoutImplementation_ThrowsActivationException ()
    {
      var serviceConfigurationEntry = CreateMultipleServiceConfigurationEntry(typeof(ITestType), new Type[0]);
      var serviceConfigurationDiscoveryServiceStub = new Mock<IServiceConfigurationDiscoveryService>(MockBehavior.Strict);
      serviceConfigurationDiscoveryServiceStub.Setup(_ => _.GetDefaultConfiguration(typeof(ITestType))).Returns(serviceConfigurationEntry);

      var serviceLocator = CreateServiceLocator(serviceConfigurationDiscoveryServiceStub.Object);

      Assert.That(
          () => serviceLocator.GetInstance(typeof(ITestType)),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo(
              "No implementation is registered for service type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType'."));
    }

    [Test]
    public void Register_SingleWithMultipleRegistrations_ThrowsInvalidOperationException ()
    {
      var implementation1 = new ServiceImplementationInfo(typeof(TestImplementation1), LifetimeKind.InstancePerDependency, RegistrationType.Single);
      var implementation2 = new ServiceImplementationInfo(typeof(TestImplementation2), LifetimeKind.InstancePerDependency, RegistrationType.Single);
      var serviceConfigurationEntry = new ServiceConfigurationEntry(typeof(ITestType), implementation1, implementation2);

      var serviceLocator = CreateServiceLocator();

      Assert.That(
          () => serviceLocator.Register(serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Cannot register multiple implementations with registration type 'Single' "
              + "for service type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType'."));
    }
  }
}
