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
using System.Linq;
using NUnit.Framework;
using Remotion.Logging;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class BootstrapServiceConfigurationTest
  {
    private BootstrapServiceConfiguration _configuration;

    [SetUp]
    public void SetUp ()
    {
      _configuration = new BootstrapServiceConfiguration();
    }

    [Test]
    public void Register_ServiceConfigurationEntry_AddsEntryToRegistrations ()
    {
      var entry = new ServiceConfigurationEntry(typeof(IService), new ServiceImplementationInfo(typeof(Service), LifetimeKind.Singleton));
      _configuration.Register(entry);

      Assert.That(_configuration.GetRegistrations(), Has.Member(entry));
    }

    [Test]
    public void Register_ServiceConfigurationEntry_OverridesExistingEntry ()
    {
      var entry1 = new ServiceConfigurationEntry(typeof(IService), new ServiceImplementationInfo(typeof(Service), LifetimeKind.Singleton));
      _configuration.Register(entry1);

      var entry2 = new ServiceConfigurationEntry(typeof(IService), new ServiceImplementationInfo(typeof(Service), LifetimeKind.Singleton));
      _configuration.Register(entry2);

      Assert.That(_configuration.GetRegistrations(), Has.Member(entry2).And.Not.Member(entry1));
    }

    [Test]
    public void GetRegistrations_HasDefaultRegistrations ()
    {
      var serviceConfigurationEntries = _configuration.GetRegistrations();
      Assert.That(
          serviceConfigurationEntries.Select(e => e.ServiceType),
          Is.EquivalentTo(new[] { typeof(ILogManager) }));
      Assert.That(
          serviceConfigurationEntries.SelectMany(e => e.ImplementationInfos.Select(i => i.ImplementationType)),
          Is.EquivalentTo(new[] { typeof(Log4NetLogManager) }));
      Assert.That(serviceConfigurationEntries.SelectMany(e => e.ImplementationInfos.Select(i => i.Lifetime)), Has.All.EqualTo(LifetimeKind.Singleton));
    }

    [Test]
    public void GetRegistrations_AllowsOverridingDefaultRegistrations ()
    {
      var entry = new ServiceConfigurationEntry(typeof(ILogManager), new ServiceImplementationInfo(typeof(FakeLogManager), LifetimeKind.Singleton));
      _configuration.Register(entry);

      var serviceConfigurationEntries = _configuration.GetRegistrations();
      Assert.That(serviceConfigurationEntries.SingleOrDefault(e => e.ServiceType == typeof(ILogManager)), Is.EqualTo(entry));
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

    public class FakeLogManager : ILogManager
    {
      ILog ILogManager.GetLogger (string name) => throw new NotImplementedException();

      ILog ILogManager.GetLogger (Type type) => throw new NotImplementedException();

      void ILogManager.Initialize () => throw new NotImplementedException();

      void ILogManager.InitializeConsole () => throw new NotImplementedException();

      void ILogManager.InitializeConsole (LogLevel defaultThreshold, params LogThreshold[] logThresholds) => throw new NotImplementedException();
    }
  }
}
