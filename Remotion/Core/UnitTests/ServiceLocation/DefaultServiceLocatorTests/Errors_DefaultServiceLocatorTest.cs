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
using Remotion.ServiceLocation;
using Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain;

namespace Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests
{
  [TestFixture]
  public class Errors_DefaultServiceLocatorTest : TestBase
  {
    [Test]
    public void Register_SingleAndCompoundImplementations_ThrowsInvalidOperationException ()
    {
      var single = new ServiceImplementationInfo(typeof(TestImplementation1), LifetimeKind.InstancePerDependency, RegistrationType.Single);
      var compound = new ServiceImplementationInfo(typeof(TestCompound), LifetimeKind.InstancePerDependency, RegistrationType.Compound);
      var serviceConfigurationEntry = new ServiceConfigurationEntry(typeof(ITestType), single, compound);

      var serviceLocator = CreateServiceLocator();

      Assert.That(
          () => serviceLocator.Register(serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Service type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType': "
              + "Registrations of type 'Single' and 'Compound' are mutually exclusive."));
    }

    [Test]
    public void Register_SingleAndMultipleImplementations_ThrowsInvalidOperationException ()
    {
      var single = new ServiceImplementationInfo(typeof(TestImplementation1), LifetimeKind.InstancePerDependency, RegistrationType.Single);
      var multiple = new ServiceImplementationInfo(typeof(TestImplementation2), LifetimeKind.InstancePerDependency, RegistrationType.Multiple);
      var serviceConfigurationEntry = new ServiceConfigurationEntry(typeof(ITestType), single, multiple);

      var serviceLocator = CreateServiceLocator();

      Assert.That(
          () => serviceLocator.Register(serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Service type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType': "
              + "Registrations of type 'Single' and 'Multiple' are mutually exclusive."));
    }

    [Test]
    public void Register_TypeWithTooManyPublicCtors_ThrowsInvalidOperationException ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(
          typeof(ITestTypeWithErrors),
          typeof(TestTypeWithTooManyPublicConstructors));

      var serviceLocator = CreateServiceLocator();

      Assert.That(
          () => serviceLocator.Register(serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestTypeWithTooManyPublicConstructors' cannot be instantiated. "
              + "The type must have exactly one public constructor."));
    }

    [Test]
    public void Register_TypeWithOnlyNonPublicCtor_ThrowsInvalidOperationException ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(
          typeof(ITestTypeWithErrors),
          typeof(TestTypeWithOnlyNonPublicConstructor));

      var serviceLocator = CreateServiceLocator();

      Assert.That(
          () => serviceLocator.Register(serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestTypeWithOnlyNonPublicConstructor' cannot be instantiated. "
              + "The type must have exactly one public constructor."));
    }

    [Test]
    public void Register_Twice_ExceptionIsThrown ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(
          typeof(ITestType),
          typeof(TestImplementation1));

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      Assert.That(
          () => serviceLocator.Register(serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Register cannot be called twice or after GetInstance for service type: 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType'."));
    }

    [Test]
    public void Register_AfterGetInstance_ExceptionIsThrown ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(
          typeof(ITestType),
          typeof(TestImplementation1));

      var serviceConfigurationDiscoveryServiceMock = new Mock<IServiceConfigurationDiscoveryService>(MockBehavior.Strict);
      serviceConfigurationDiscoveryServiceMock.Setup(_ => _.GetDefaultConfiguration(typeof(ITestType))).Returns(serviceConfigurationEntry);

      var serviceLocator = CreateServiceLocator(serviceConfigurationDiscoveryServiceMock.Object);
      serviceLocator.Register(serviceConfigurationEntry);

      Assert.That(
          () => serviceLocator.Register(serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Register cannot be called twice or after GetInstance for service type: 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType'."));
    }

    [Test]
    public void GetInstance_IndirectActivationExceptionDuringDependencyResolution_ThrowsActivationException_CausesFullMessageToBeBuilt ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(
          typeof(ITestType),
          typeof(TestImplementationWithMultipleConstructorParameters));

      var expectedException = new Exception("Expected Exception Message");
      var serviceConfigurationDiscoveryServiceStub = new Mock<IServiceConfigurationDiscoveryService>(MockBehavior.Strict);
      serviceConfigurationDiscoveryServiceStub.Setup(_ => _.GetDefaultConfiguration(typeof(InstanceService))).Throws(expectedException);

      var serviceLocator = CreateServiceLocator(serviceConfigurationDiscoveryServiceStub.Object);
      serviceLocator.Register(serviceConfigurationEntry);
      serviceLocator.Register(CreateMultipleService());
      serviceLocator.Register(CreateSingletonService());

      Assert.That(
          () => serviceLocator.GetInstance(typeof(ITestType)),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo(
              "Could not resolve type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType': "
              + "Error resolving indirect dependency of constructor parameter 'instanceService1' "
              + "of type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestImplementationWithMultipleConstructorParameters': "
              + "Error resolving service Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.InstanceService': "
              + "Expected Exception Message"));
    }

    [Test]
    public void GetInstance_IndirectActivationExceptionDuringConstructor_ThrowsActivationException_CausesFullMessageToBeBuilt ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(
          typeof(ITestType),
          typeof(TestTypeWithConstructorThrowingSingleDependency));

      var serviceConfigurationEntryForError = CreateSingleServiceConfigurationEntry(
          typeof(ITestTypeWithErrors),
          typeof(TestTypeWithConstructorThrowingException));

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);
      serviceLocator.Register(serviceConfigurationEntryForError);

      Assert.That(
          () => serviceLocator.GetInstance(typeof(ITestType)),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo(
              "Could not resolve type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType': "
              + "Error resolving indirect dependency of constructor parameter 'param' "
              + "of type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestTypeWithConstructorThrowingSingleDependency': "
              + "ApplicationException: This exception comes from the ctor."));
    }

    [Test]
    public void GetInstance_IndirectActivationExceptionDuringDependencyResolution_ForCollectionParameter_ThrowsActivationException_CausesFullMessageToBeBuilt ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(
          typeof(ITestType),
          typeof(TestImplementationWithMultipleConstructorParameters));

      var expectedException = new Exception("Expected Exception Message");
      var serviceConfigurationDiscoveryServiceStub = new Mock<IServiceConfigurationDiscoveryService>(MockBehavior.Strict);
      serviceConfigurationDiscoveryServiceStub.Setup(_ => _.GetDefaultConfiguration(typeof(MultipleService))).Throws(expectedException);

      var serviceLocator = CreateServiceLocator(serviceConfigurationDiscoveryServiceStub.Object);
      serviceLocator.Register(serviceConfigurationEntry);
      serviceLocator.Register(CreateInstanceService());
      serviceLocator.Register(CreateSingletonService());

      Assert.That(
          () => serviceLocator.GetInstance(typeof(ITestType)),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo(
              "Could not resolve type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType': "
              + "Error resolving indirect collection dependency of constructor parameter 'multipleService' "
              + "of type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestImplementationWithMultipleConstructorParameters': "
              + "Error resolving service Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.MultipleService': "
              + "Expected Exception Message"));
    }

    [Test]
    public void GetInstance_IndirectActivationExceptionDuringConstructor_ForCollectionParameter_ThrowsActivationException_CausesFullMessageToBeBuilt ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry(
          typeof(ITestType),
          typeof(TestTypeWithConstructorThrowingMultipleDependency));

      var serviceConfigurationEntryForError = CreateMultipleServiceConfigurationEntry(
          typeof(ITestTypeWithErrors),
          new[] { typeof(TestTypeWithConstructorThrowingException) });

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);
      serviceLocator.Register(serviceConfigurationEntryForError);

      Assert.That(
          () => serviceLocator.GetInstance(typeof(ITestType)),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo(
              "Could not resolve type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType': "
              + "Error resolving indirect collection dependency of constructor parameter 'param' "
              + "of type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestTypeWithConstructorThrowingMultipleDependency': "
              + "ApplicationException: This exception comes from the ctor."));
    }

    [Test]
    public void GetInstance_WithFactoryReturningWrongType_ThrowsActivationException ()
    {
      Func<ITestType> factory = () => new TestImplementation1();
      Func<object> factoryAsObject = factory;
      var implementation = ServiceImplementationInfo.CreateSingle<ITestTypeWithErrors>(() => null);
      PrivateInvoke.SetNonPublicField(implementation, "_factory", factoryAsObject);
      var serviceConfigurationEntry = new ServiceConfigurationEntry(typeof(ITestTypeWithErrors), implementation);

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register(serviceConfigurationEntry);

      Assert.That(
          () => serviceLocator.GetInstance(typeof(ITestTypeWithErrors)),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo(
              "The instance returned by the registered factory does not implement the requested type "
              + "'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestTypeWithErrors'. "
              + "(Instance type: 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestImplementation1'.)"));
    }
  }
}
