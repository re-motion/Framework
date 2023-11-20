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
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using Remotion.Configuration.ServiceLocation;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class SafeServiceLocatorTest
  {
    private ServiceLocatorProvider _serviceLocatorProviderBackup;
    private IServiceLocationConfiguration _previousConfiguration;

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
      _previousConfiguration = ServiceLocationConfiguration.Current;
      ServiceLocationConfiguration.SetCurrent(null);
      ResetDefaultServiceLocator();
      SafeServiceLocator.BootstrapConfiguration.Reset();
    }

    [TearDown]
    public void TearDown ()
    {
      ServiceLocationConfiguration.SetCurrent(_previousConfiguration);
      ResetDefaultServiceLocator();
      SafeServiceLocator.BootstrapConfiguration.Reset();
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

      ConfigureServiceLocatorProvider(new Mock<IServiceLocatorProvider>(MockBehavior.Strict).Object);

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
          .Setup(stub => stub.GetServiceLocator(It.IsAny<ReadOnlyCollection<ServiceConfigurationEntry>>()))
          .Returns(fakeServiceLocator.Object);

      ConfigureServiceLocatorProvider(serviceLocatorProviderStub.Object);

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

      SafeServiceLocator.BootstrapConfiguration.Register(entry1);
      SafeServiceLocator.BootstrapConfiguration.Register(entry2);

      var serviceLocatorProviderMock = new Mock<IServiceLocatorProvider>(MockBehavior.Strict);
      var fakeServiceLocator = new Mock<IServiceLocator>();
      serviceLocatorProviderMock
          .Setup(mock => mock.GetServiceLocator(new ReadOnlyCollection<ServiceConfigurationEntry>(new[] { entry1, entry2 })))
          .Returns(fakeServiceLocator.Object)
          .Verifiable();

      ConfigureServiceLocatorProvider(serviceLocatorProviderMock.Object);

      var result = SafeServiceLocator.Current;

      serviceLocatorProviderMock.Verify();
      Assert.That(result, Is.SameAs(fakeServiceLocator.Object));
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
          .Setup(stub => stub.GetServiceLocator(It.IsAny<ReadOnlyCollection<ServiceConfigurationEntry>>()))
          .Returns(fakeServiceLocator.Object);

      ConfigureServiceLocatorProvider(serviceLocatorProviderStub.Object);

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
      serviceLocatorProvider.Setup(mock => mock.GetServiceLocator(It.IsAny<ReadOnlyCollection<ServiceConfigurationEntry>>())).Throws(exception).Verifiable();

      ConfigureServiceLocatorProvider(serviceLocatorProvider.Object);

      Assert.That(() => SafeServiceLocator.Current, Throws.Exception.SameAs(exception));
    }

    [Test]
    public void GetCurrent_ProvidesAccessToBootstrapLocator_WhileConfiguredLocatorIsConstructed ()
    {
      var serviceLocatorProvider = new Mock<IServiceLocatorProvider>();
      var fakeServiceLocator = new Mock<IServiceLocator>();
      serviceLocatorProvider
          .Setup(stub => stub.GetServiceLocator(It.IsAny<ReadOnlyCollection<ServiceConfigurationEntry>>()))
          .Returns(fakeServiceLocator.Object)
          .Callback(
              (ReadOnlyCollection<ServiceConfigurationEntry> bootstrapConfiguration) =>
              {
                Assert.That(
                    SafeServiceLocator.Current,
                    Is.Not.Null.And.SameAs(((BootstrapServiceConfiguration)SafeServiceLocator.BootstrapConfiguration).BootstrapServiceLocator));
              });

      ConfigureServiceLocatorProvider(serviceLocatorProvider.Object);

      var result = SafeServiceLocator.Current;

      Assert.That(result, Is.SameAs(fakeServiceLocator.Object));
    }

    [Test]
    public void DefaultConfiguration_IntegrationTest ()
    {
      SafeServiceLocator.BootstrapConfiguration.Register(typeof(IService1), typeof(Service1), LifetimeKind.InstancePerDependency);

      Assert.That(SafeServiceLocator.Current, Is.TypeOf<DefaultServiceLocator>());
      Assert.That(SafeServiceLocator.Current.GetInstance<IServiceWithAttribute>(), Is.Not.Null.And.TypeOf<ServiceWithAttribute>());
      Assert.That(SafeServiceLocator.Current.GetInstance<IService1>(), Is.Not.Null.And.TypeOf<Service1>());
      Assert.That(() => SafeServiceLocator.Current.GetInstance<IService2>(), Throws.TypeOf<ActivationException>());
    }

    private void ConfigureServiceLocatorProvider (IServiceLocatorProvider serviceLocatorProvider)
    {
      var serviceLocationConfiguration = new Mock<IServiceLocationConfiguration>();
      serviceLocationConfiguration.Setup(stub => stub.CreateServiceLocatorProvider()).Returns(serviceLocatorProvider);
      ServiceLocationConfiguration.SetCurrent(serviceLocationConfiguration.Object);
      ResetDefaultServiceLocator();
    }

    private void ResetDefaultServiceLocator ()
    {
      var defaultServiceLocatorContainer =
          (DoubleCheckedLockingContainer<IServiceLocator>)PrivateInvoke.GetNonPublicStaticField(typeof(SafeServiceLocator), "s_defaultServiceLocator");
      defaultServiceLocatorContainer.Value = null;
    }

    interface IService1 { }
    class Service1 : IService1 { }

    interface IService2 { }
    class Service2 : IService2 { }

    interface IServiceWithAttribute { }
    [ImplementationFor(typeof(IServiceWithAttribute))]
    class ServiceWithAttribute : IServiceWithAttribute { }
  }
}
