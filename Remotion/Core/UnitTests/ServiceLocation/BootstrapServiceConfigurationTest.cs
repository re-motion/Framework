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
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class BootstrapServiceConfigurationTest
  {
    private BootstrapServiceConfiguration _configuration;
    private ILoggerFactory _backupLoggerFactory;

    [SetUp]
    public void SetUp ()
    {
      _configuration = new BootstrapServiceConfiguration();

      _backupLoggerFactory = GetLoggerFactoryOnBootstrapServiceConfiguration();
    }

    [TearDown]
    public void TearDown ()
    {
      SetLoggerFactoryOnBootstrapServiceConfiguration(_backupLoggerFactory);
    }

    [Test]
    public void Register_ServiceConfigurationEntry_AddsEntryToRegistrations_AndToBootstrapLocator ()
    {
      var entry = new ServiceConfigurationEntry(typeof(IService), new ServiceImplementationInfo(typeof(Service), LifetimeKind.Singleton));
      _configuration.Register(entry);

      Assert.That(_configuration.Registrations, Is.EqualTo(new[] { entry }));
      Assert.That(_configuration.BootstrapServiceLocator.GetInstance<IService>(), Is.Not.Null.And.TypeOf<Service>());
    }

    [Test]
    public void Register_ServiceConfigurationEntry_IfLocatorThrows_NoRegistrationIsAdded ()
    {
      // This causes the association between IServiceWithAttribute and ServiceWithAttribute1 to be stored in the BootstrapServiceLocator, so it will
      // later throw on Register.
      _configuration.BootstrapServiceLocator.GetInstance<IServiceWithAttribute>();

      var entry = new ServiceConfigurationEntry(
          typeof(IServiceWithAttribute),
          new ServiceImplementationInfo(typeof(ServiceWithAttribute2), LifetimeKind.Singleton));
      Assert.That(() => _configuration.Register(entry), Throws.InvalidOperationException);

      Assert.That(_configuration.Registrations, Is.Empty);
      Assert.That(_configuration.BootstrapServiceLocator.GetInstance<IServiceWithAttribute>(), Is.Not.Null.And.TypeOf<ServiceWithAttribute1>());
    }

    [Test]
    public void Register_Types_AddsEntry ()
    {
      _configuration.Register(typeof(IService), typeof(Service), LifetimeKind.InstancePerDependency);

      Assert.That(_configuration.Registrations, Has.Length.EqualTo(1));
      Assert.That(_configuration.Registrations[0].ServiceType, Is.SameAs(typeof(IService)));

      Assert.That(_configuration.Registrations[0].ImplementationInfos.Count, Is.EqualTo(1));
      Assert.That(_configuration.Registrations[0].ImplementationInfos[0].ImplementationType, Is.EqualTo(typeof(Service)));
      Assert.That(_configuration.Registrations[0].ImplementationInfos[0].Lifetime, Is.EqualTo(LifetimeKind.InstancePerDependency));

      Assert.That(_configuration.BootstrapServiceLocator.GetInstance<IService>(), Is.Not.Null.And.TypeOf<Service>());
    }

    [Test]
    public void Reset ()
    {
      _configuration.Register(typeof(IService), typeof(Service), LifetimeKind.InstancePerDependency);

      Assert.That(_configuration.Registrations, Is.Not.Empty);
      Assert.That(_configuration.BootstrapServiceLocator.GetInstance<IService>(), Is.Not.Null.And.TypeOf<Service>());

      _configuration.Reset();

      Assert.That(_configuration.Registrations, Is.Empty);
      Assert.That(() => _configuration.BootstrapServiceLocator.GetInstance<IService>(), Throws.TypeOf<ActivationException>());
    }

    [Test]
    public void GetLoggerFactory_AfterSetLoggerFactory_ReturnsLoggerFactory ()
    {
      SetLoggerFactoryOnBootstrapServiceConfiguration(null);

      var loggerFactoryStub = Mock.Of<ILoggerFactory>();
      BootstrapServiceConfiguration.SetLoggerFactory(loggerFactoryStub);

      Assert.That(BootstrapServiceConfiguration.GetLoggerFactory(), Is.SameAs(loggerFactoryStub));
    }

    [Test]
    public void GetLoggerFactory_WithoutSetLoggerFactory_ThrowsInvalidOperationException ()
    {
      SetLoggerFactoryOnBootstrapServiceConfiguration(null);

      Assert.That(
          () => BootstrapServiceConfiguration.GetLoggerFactory(),
          Throws.InvalidOperationException
              .With.Message.StartsWith("The BootstrapServiceConfiguration.SetLoggerFactory(...) method must be called before accessing the service configuration."));
    }

    private static ILoggerFactory GetLoggerFactoryOnBootstrapServiceConfiguration ()
    {
      return (ILoggerFactory)PrivateInvoke.GetNonPublicStaticField(typeof(BootstrapServiceConfiguration), "s_loggerFactory");
    }

    private static void SetLoggerFactoryOnBootstrapServiceConfiguration (ILoggerFactory value)
    {
      PrivateInvoke.SetNonPublicStaticField(typeof(BootstrapServiceConfiguration), "s_loggerFactory", value);
    }

    public interface IService { }
    public class Service : IService { }

    public interface IServiceWithAttribute
    {
    }

    [ImplementationFor(typeof(IServiceWithAttribute))]
    public class ServiceWithAttribute1 : IServiceWithAttribute
    {
    }

    public class ServiceWithAttribute2 : IServiceWithAttribute
    {
    }
  }
}
