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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.ServiceLocation;
using Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain;

namespace Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests
{
  [TestFixture]
  public class Multiple_DefaultServiceLocatorTest : TestBase
  {
    [Test]
    public void GetAllInstances_LookUpViaServiceConfigurationDiscoveryService_InstantiatesImplementations ()
    {
      var serviceConfigurationEntry = CreateMultipleServiceConfigurationEntry(
          typeof(ITestType),
          new[] { typeof(TestImplementation1), typeof(TestImplementation2) });

      var serviceConfigurationDiscoveryServiceStub = new Mock<IServiceConfigurationDiscoveryService>(MockBehavior.Strict);
      serviceConfigurationDiscoveryServiceStub.Setup(_=>_.GetDefaultConfiguration(typeof(ITestType))).Returns(serviceConfigurationEntry);

      var serviceLocator = CreateServiceLocator(serviceConfigurationDiscoveryServiceStub.Object);

      var instances = serviceLocator.GetAllInstances(typeof(ITestType));

      Assert.That(
          instances.Select(c => c.GetType()),
          Is.EqualTo(new[] { typeof(TestImplementation1), typeof(TestImplementation2) }));
    }

    [Test]
    public void GetAllInstances_InstantiatesImplementationsInOrderOfRegistration ()
    {
      var serviceConfigurationEntry = CreateMultipleServiceConfigurationEntry(
          typeof(ITestType),
          new[] { typeof(TestImplementation1), typeof(TestImplementation2) });

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      IEnumerable<object> instances = serviceLocator.GetAllInstances(typeof(ITestType));

      Assert.That(
          instances.Select(c => c.GetType()),
          Is.EqualTo(new[] { typeof(TestImplementation1), typeof(TestImplementation2) }));
    }

    [Test]
    public void GetAllInstances_NoImplementations_ReturnsEmptySequence ()
    {
      var serviceConfigurationEntry = CreateMultipleServiceConfigurationEntry(typeof(ITestType), new Type[0]);

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      var instances = serviceLocator.GetAllInstances(typeof(ITestType));

      Assert.That(instances, Is.Empty);
    }

    [Test]
    public void GetAllInstances_GenericOverload_InstantiatesImplementationsInOrderOfRegistration ()
    {
      var serviceConfigurationEntry = CreateMultipleServiceConfigurationEntry(
          typeof(ITestType),
          new[] { typeof(TestImplementation1), typeof(TestImplementation2) });

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      IEnumerable<ITestType> instances = serviceLocator.GetAllInstances<ITestType>();

      Assert.That(
          instances.Select(c => c.GetType()),
          Is.EqualTo(new[] { typeof(TestImplementation1), typeof(TestImplementation2) }));
    }

    [Test]
    public void GetAllInstances_WithMixedLifetimeKind_ReturnsSingletonAsSingletonAndInstanceAsInstance ()
    {
      var implementation1 = new ServiceImplementationInfo(typeof(TestImplementation1), LifetimeKind.Singleton, RegistrationType.Multiple);
      var implementation2 = new ServiceImplementationInfo(typeof(TestImplementation2), LifetimeKind.InstancePerDependency, RegistrationType.Multiple);
      var serviceConfigurationEntry = new ServiceConfigurationEntry(typeof(ITestType), implementation1, implementation2);

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      var instances1 = serviceLocator.GetAllInstances(typeof(ITestType)).ToArray();
      var instances2 = serviceLocator.GetAllInstances(typeof(ITestType)).ToArray();

      Assert.That(instances1, Is.Not.SameAs(instances2));
      Assert.That(instances1[0], Is.SameAs(instances2[0]));
      Assert.That(instances1[1], Is.Not.SameAs(instances2[1]));
    }

    [Test]
    public void GetAllInstances_WithSingletonLifetimeKind_AndSingletonIsLazyInitialized ()
    {
      var serviceConfigurationEntry = CreateMultipleServiceConfigurationEntry(
          typeof(ITestType),
          new[] { typeof(TestImplementationWithOneConstructorParameter) },
          LifetimeKind.Singleton);

      var serviceLocator = CreateServiceLocator();
      Assert.That(() => serviceLocator.Register(serviceConfigurationEntry), Throws.Nothing);
    }

    [Test]
    public void GetAllInstances_ImplementationIsRegisteredAsFactoryWithInstanceLifetime ()
    {
      TestImplementation1 expectedInstance = null;
      var serviceImplementation = ServiceImplementationInfo.CreateMultiple(
          () => expectedInstance = new TestImplementation1(),
          LifetimeKind.InstancePerDependency);
      var serviceConfigurationEntry = new ServiceConfigurationEntry(typeof(ITestType), serviceImplementation);

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      var instance1 = serviceLocator.GetAllInstances(typeof(ITestType)).SingleOrDefault();
      Assert.That(expectedInstance, Is.Not.Null);

      var instance2 = serviceLocator.GetAllInstances(typeof(ITestType)).SingleOrDefault();
      Assert.That(instance1, Is.Not.SameAs(instance2));
    }

    [Test]
    public void GetAllInstances_ImplementationIsRegisteredAsFactoryWithSingletonLifetime_AndSingletonIsLazyInitialized ()
    {
      TestImplementation1 expectedInstance = null;
      var serviceImplementation = ServiceImplementationInfo.CreateMultiple(
          () => expectedInstance = new TestImplementation1(),
          LifetimeKind.Singleton);
      var serviceConfigurationEntry = new ServiceConfigurationEntry(typeof(ITestType), serviceImplementation);

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      Assert.That(expectedInstance, Is.Null);
      var instance1 = serviceLocator.GetAllInstances(typeof(ITestType)).SingleOrDefault();
      Assert.That(expectedInstance, Is.Not.Null);

      var instance2 = serviceLocator.GetAllInstances(typeof(ITestType)).SingleOrDefault();
      Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void GetInstance_ThrowsActivationException ()
    {
      var serviceConfigurationEntry = CreateMultipleServiceConfigurationEntry(
          typeof(ITestType),
          new[] { typeof(TestImplementation1) });

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      Assert.That(
          () => serviceLocator.GetInstance(typeof(ITestType)),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo(
              "Multiple implementations are configured for service type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType'. "
              + "Use GetAllInstances() to retrieve the implementations."));
    }

    [Test]
    public void GetAllInstances_ConstructorThrowingException_ExceptionIsWrappedInActivationException ()
    {
      var serviceConfigurationEntry = CreateMultipleServiceConfigurationEntry(
          typeof(ITestTypeWithErrors),
          new[] { typeof(TestTypeWithConstructorThrowingException) });

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      var instances = serviceLocator.GetAllInstances(typeof(ITestTypeWithErrors));
      var exception = Assert.Throws<ActivationException>(() => instances.ForceEnumeration());
      Assert.That(exception.Message, Is.EqualTo("ApplicationException: This exception comes from the ctor."));
      Assert.That(exception.InnerException, Is.TypeOf<ApplicationException>());
      Assert.That(exception.InnerException.Message, Is.EqualTo("This exception comes from the ctor."));
    }

    [Test]
    public void GetAllInstances_ServiceTypeWithNullImplementation_ThrowsActivationException ()
    {
      var implementation = ServiceImplementationInfo.CreateMultiple<ITestTypeWithErrors>(() => null);
      var serviceConfigurationEntry = new ServiceConfigurationEntry(typeof(ITestTypeWithErrors), implementation);

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      Assert.That(
          () => serviceLocator.GetAllInstances(typeof(ITestTypeWithErrors)).ToArray(),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo(
              "The registered factory returned null instead of an instance implementing the requested service type "
              + "'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestTypeWithErrors'."));
    }

    [Test]
    public void GetAllInstances_ServiceTypeWithoutImplementations_DefaultsToMultipleRegistration ()
    {
      //TODO RM-5506: Integration Test
      var serviceConfigurationEntry = CreateMultipleServiceConfigurationEntry(typeof(ITestType), new Type[0]);
      var serviceConfigurationDiscoveryServiceStub = new Mock<IServiceConfigurationDiscoveryService>(MockBehavior.Strict);
      serviceConfigurationDiscoveryServiceStub.Setup(_=>_.GetDefaultConfiguration(typeof(ITestType))).Returns(serviceConfigurationEntry);

      var serviceLocator = CreateServiceLocator(serviceConfigurationDiscoveryServiceStub.Object);

      var result = serviceLocator.GetAllInstances(typeof(ITestType));

      Assert.That(result, Is.Empty);
    }
  }
}
