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
using System.ComponentModel.Design;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Logging;
using Remotion.Reflection.TypeResolution;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class SafeServiceLocatorTest
  {
    private ServiceLocatorProvider _serviceLocatorProviderBackup;

    [OneTimeSetUp]
    public void OneTimeSetUp ()
    {
      _serviceLocatorProviderBackup = (ServiceLocatorProvider)PrivateInvoke.GetNonPublicStaticField(typeof(ServiceLocator), "_currentProvider");
      PrivateInvoke.SetNonPublicStaticField(typeof(ServiceLocator), "_currentProvider", null);
    }

    [OneTimeTearDown]
    public void TestFixtureTearDown ()
    {
      PrivateInvoke.SetNonPublicStaticField(typeof(ServiceLocator), "_currentProvider", _serviceLocatorProviderBackup);
    }

    [SetUp]
    public void SetUp ()
    {
      ResetSafeServiceLocator();
    }

    [TearDown]
    public void TearDown ()
    {
      ResetSafeServiceLocator();
    }

    [Test]
    public void GetCurrent_WithLocatorProvider ()
    {
      var serviceLocatorStub = new Mock<IServiceLocator>();
      ServiceLocator.SetLocatorProvider(() => serviceLocatorStub.Object);

      Assert.That(SafeServiceLocator.Current, Is.SameAs(serviceLocatorStub.Object));
    }

    [Test]
    public void GetCurrent_WithLocatorProvider_IgnoresServiceConfiguration ()
    {
      var serviceLocatorStub = new Mock<IServiceLocator>();
      ServiceLocator.SetLocatorProvider(() => serviceLocatorStub.Object);

      IServiceLocatorProvider serviceLocatorProvider = new Mock<IServiceLocatorProvider>(MockBehavior.Strict).Object;
      ResetSafeServiceLocator();
      SafeServiceLocator.BootstrapConfiguration.Register(serviceLocatorProvider);

      Assert.That(SafeServiceLocator.Current, Is.SameAs(serviceLocatorStub.Object));
    }

    [Test]
    public void GetCurrent_WithoutLocatorProvider_ReturnsDefaultServiceLocator ()
    {
      ServiceLocator.SetLocatorProvider(null);

      Assert.That(SafeServiceLocator.Current, Is.TypeOf(typeof(DefaultServiceLocator)));
    }

    [Test]
    public void GetCurrent_WithoutLocatorProvider_ReturnsConfiguredServiceLocator ()
    {
      ServiceLocator.SetLocatorProvider(null);

      var serviceLocatorProviderStub = new Mock<IServiceLocatorProvider>();
      var fakeServiceLocator = new Mock<IServiceLocator>();
      serviceLocatorProviderStub
          .Setup(stub => stub.GetServiceLocator(It.IsAny<IReadOnlyCollection<ServiceConfigurationEntry>>()))
          .Returns(fakeServiceLocator.Object);

      ResetSafeServiceLocator();
      SafeServiceLocator.BootstrapConfiguration.Register(serviceLocatorProviderStub.Object);

      Assert.That(SafeServiceLocator.Current, Is.SameAs(fakeServiceLocator.Object));
    }

    [Test]
    public void GetCurrent_WithoutLocatorProvider_ReturnsConfiguredServiceLocator_WithBootstrapConfigurationEntries ()
    {
      ServiceLocator.SetLocatorProvider(null);

      var entry1 = new ServiceConfigurationEntry(
          typeof(IService1),
          new ServiceImplementationInfo(typeof(Service1), LifetimeKind.InstancePerDependency));
      var entry2 = new ServiceConfigurationEntry(
          typeof(IService2),
          new ServiceImplementationInfo(typeof(Service2), LifetimeKind.InstancePerDependency));

      var serviceLocatorProviderMock = new Mock<IServiceLocatorProvider>(MockBehavior.Loose);
      var fakeServiceLocator = new Mock<IServiceLocator>();
      IReadOnlyCollection<ServiceConfigurationEntry> bootstrapConfigurationParameter = null;
      serviceLocatorProviderMock
          .Setup(mock => mock.GetServiceLocator(It.IsNotNull<IReadOnlyCollection<ServiceConfigurationEntry>>()))
          .Returns(fakeServiceLocator.Object)
          .Callback((IReadOnlyCollection<ServiceConfigurationEntry> bootstrapConfiguration) => bootstrapConfigurationParameter = bootstrapConfiguration)
          .Verifiable();

      ResetSafeServiceLocator();
      SafeServiceLocator.BootstrapConfiguration.Register(serviceLocatorProviderMock.Object);

      SafeServiceLocator.BootstrapConfiguration.Register(entry1);
      SafeServiceLocator.BootstrapConfiguration.Register(entry2);

      var result = SafeServiceLocator.Current;

      serviceLocatorProviderMock.Verify();
      Assert.That(result, Is.SameAs(fakeServiceLocator.Object));
      Assert.That(bootstrapConfigurationParameter, Is.Not.Null);
      Assert.That(bootstrapConfigurationParameter, Has.Member(entry1).And.Member(entry2));
    }

    [Test]
    public void GetCurrent_WithoutLocatorProvider_SetsServiceLocatorCurrent ()
    {
      ServiceLocator.SetLocatorProvider(null);

      var safeCurrent = SafeServiceLocator.Current;
      Assert.That(ServiceLocator.Current, Is.Not.Null.And.SameAs(safeCurrent));
    }

    [Test]
    public void GetCurrent_WithLocatorProviderReturningNull_ReturnsDefaultServiceLocator ()
    {
      ServiceLocator.SetLocatorProvider(() => null);

      Assert.That(SafeServiceLocator.Current, Is.TypeOf(typeof(DefaultServiceLocator)));
    }

    [Test]
    public void GetCurrent_WithLocatorProviderReturningNull_ReturnsConfiguredServiceLocator ()
    {
      ServiceLocator.SetLocatorProvider(() => null);

      var serviceLocatorProviderStub = new Mock<IServiceLocatorProvider>();
      var fakeServiceLocator = new Mock<IServiceLocator>();
      serviceLocatorProviderStub
          .Setup(stub => stub.GetServiceLocator(It.IsAny<IReadOnlyCollection<ServiceConfigurationEntry>>()))
          .Returns(fakeServiceLocator.Object);

      ResetSafeServiceLocator();
      SafeServiceLocator.BootstrapConfiguration.Register(serviceLocatorProviderStub.Object);

      Assert.That(SafeServiceLocator.Current, Is.SameAs(fakeServiceLocator.Object));
    }

    [Test]
    public void GetCurrent_WithLocatorProviderReturningNull_DoesNotSetServiceLocatorCurrent ()
    {
      ServiceLocator.SetLocatorProvider(() => null);

      Dev.Null = SafeServiceLocator.Current;
      Assert.That(ServiceLocator.Current, Is.Null);
    }

    [Test]
    public void GetCurrent_WithInvalidServiceLocationConfiguration ()
    {
      ServiceLocator.SetLocatorProvider(() => null);

      var exception = new Exception();
      var serviceLocatorProvider = new Mock<IServiceLocatorProvider>();
      serviceLocatorProvider
          .Setup(mock => mock.GetServiceLocator(It.IsAny<IReadOnlyCollection<ServiceConfigurationEntry>>()))
          .Throws(exception)
          .Verifiable();

      ResetSafeServiceLocator();
      SafeServiceLocator.BootstrapConfiguration.Register(serviceLocatorProvider.Object);

      Assert.That(() => SafeServiceLocator.Current, Throws.Exception.SameAs(exception));
    }

    [Test]
    public void GetCurrent_ProvidesAccessToBootstrapLocator_WhileConfiguredLocatorIsConstructed ()
    {
      var serviceLocatorProvider = new Mock<IServiceLocatorProvider>();
      var fakeServiceLocator = new Mock<IServiceLocator>();
      serviceLocatorProvider
          .Setup(stub => stub.GetServiceLocator(It.IsAny<IReadOnlyCollection<ServiceConfigurationEntry>>()))
          .Returns(fakeServiceLocator.Object)
          .Callback((IReadOnlyCollection<ServiceConfigurationEntry> bootstrapConfiguration) => Assert.That(SafeServiceLocator.Current, Is.Not.Null));

      ResetSafeServiceLocator();
      SafeServiceLocator.BootstrapConfiguration.Register(serviceLocatorProvider.Object);

      var result = SafeServiceLocator.Current;

      Assert.That(result, Is.SameAs(fakeServiceLocator.Object));
    }

    [Test]
    public void DefaultConfiguration_IntegrationTest_CreatesDefaultServiceLocator ()
    {
      Assert.That(SafeServiceLocator.Current, Is.TypeOf<DefaultServiceLocator>());
    }

    [Test]
    public void DefaultConfiguration_IntegrationTest_WithManualRegistration ()
    {
      SafeServiceLocator.BootstrapConfiguration.Register(typeof(IService1), typeof(Service1));

      Assert.That(SafeServiceLocator.Current.GetInstance<IService1>(), Is.Not.Null.And.TypeOf<Service1>());
    }

    [Test]
    public void DefaultConfiguration_IntegrationTest_WithAttributeBasedRegistration ()
    {
      Assert.That(SafeServiceLocator.Current.GetInstance<IServiceWithAttribute>(), Is.Not.Null.And.TypeOf<ServiceWithAttribute>());
    }

    [Test]
    public void DefaultConfiguration_IntegrationTest_WithoutRegistrationRegistration ()
    {
      Assert.That(() => SafeServiceLocator.Current.GetInstance<IService2>(), Throws.TypeOf<ActivationException>());
    }

    [Test]
    public void DefaultConfiguration_IntegrationTest_WithDependency ()
    {
      SafeServiceLocator.BootstrapConfiguration.Register(typeof(IServiceWithDependency), typeof(ServiceWithDependency));

      var serviceWithDependency = SafeServiceLocator.Current.GetInstance<IServiceWithDependency>();
      var logManager = SafeServiceLocator.Current.GetInstance<ILogManager>();
      Assert.That(serviceWithDependency, Is.Not.Null.And.TypeOf<ServiceWithDependency>());
      Assert.That(((ServiceWithDependency)serviceWithDependency).LogManager, Is.SameAs(logManager));
    }

    [Test]
    public void DefaultConfiguration_IntegrationTest_PreservesBootstrappedSingletons ()
    {
      var serviceLocator1 = SafeServiceLocator.Current;
      var logManagerInServiceLocator1 = serviceLocator1.GetInstance<ILogManager>();
      Assert.That(serviceLocator1, Is.TypeOf<DefaultServiceLocator>());

      ServiceLocator.SetLocatorProvider(null);
      ResetSafeServiceLocator(resetBootstrapConfiguration: false, resetDefaultServiceLocator: true);

      var serviceLocator2 = SafeServiceLocator.Current;
      var logManagerInServiceLocator2 = serviceLocator2.GetInstance<ILogManager>();
      Assert.That(serviceLocator2, Is.Not.SameAs(serviceLocator1));
      Assert.That(logManagerInServiceLocator2, Is.SameAs(logManagerInServiceLocator1));
    }

    [Test]
    public void DefaultConfiguration_IntegrationTest_ILogManager ()
    {
      Assert.That(SafeServiceLocator.Current.GetInstance<ILogManager>(), Is.InstanceOf<Log4NetLogManager>());
    }

    [Test]
    public void DefaultConfiguration_IntegrationTest_IServiceLocatorProvider ()
    {
      Assert.That(SafeServiceLocator.Current.GetInstance<IServiceLocatorProvider>(), Is.InstanceOf<DefaultServiceLocatorProvider>());
    }

    [Test]
    public void DefaultConfiguration_IntegrationTest_ITypeResolutionService ()
    {
      Assert.That(SafeServiceLocator.Current.GetInstance<ITypeResolutionService>(), Is.InstanceOf<DefaultTypeResolutionService>());
    }

    private void ResetSafeServiceLocator (bool resetBootstrapConfiguration = true, bool resetDefaultServiceLocator = true)
    {
      var fields = PrivateInvoke.GetNonPublicStaticField(typeof(SafeServiceLocator), "s_fields");
      Assertion.IsNotNull(fields);

      if (resetBootstrapConfiguration)
      {
        PrivateInvoke.SetPublicField(fields, "BootstrapConfiguration", new BootstrapServiceConfiguration());
      }

      if (resetDefaultServiceLocator)
      {
        var defaultServiceLocatorContainer = (DoubleCheckedLockingContainer<IServiceLocator>)PrivateInvoke.GetPublicField(fields, "DefaultServiceLocator");
        Assertion.IsNotNull(defaultServiceLocatorContainer);
        defaultServiceLocatorContainer.Value = null;
      }
    }

    interface IService1 { }
    class Service1 : IService1 { }

    interface IService2 { }
    class Service2 : IService2 { }

    interface IServiceWithAttribute { }
    [ImplementationFor(typeof(IServiceWithAttribute))]
    class ServiceWithAttribute : IServiceWithAttribute { }

    interface IServiceWithDependency { }

    class ServiceWithDependency : IServiceWithDependency
    {
      public ILogManager LogManager { get; }

      public ServiceWithDependency (ILogManager logManager)
      {
        LogManager = logManager;
      }
    }
  }
}
