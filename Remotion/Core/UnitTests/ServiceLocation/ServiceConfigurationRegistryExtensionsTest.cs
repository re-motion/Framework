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
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class ServiceConfigurationRegistryExtensionsTest
  {
    #region Test Domain

    private interface ITestType
    {
    }

    private class TestImplementation1 : ITestType
    {
    }

    private class TestImplementation2 : ITestType
    {
    }

    private class TestDecorator1 : ITestType
    {
    }

    private class TestDecorator2 : ITestType
    {
    }

    private class TestCompound : ITestType
    {
    }

    #endregion

    private class FakeRegistry : IServiceConfigurationRegistry
    {
      public ServiceConfigurationEntry ServiceConfigurationEntry { get; private set; }

      public void Register (ServiceConfigurationEntry serviceConfigurationEntry)
      {
        ServiceConfigurationEntry = serviceConfigurationEntry;
      }
    }

    [Test]
    public void RegisterSingle_WithFactory ()
    {
      var registry = new FakeRegistry();

      Func<TestImplementation1> instanceFactory = () => new TestImplementation1();
      registry.RegisterSingle<ITestType> (instanceFactory);

      Assert.That (registry.ServiceConfigurationEntry.ServiceType, Is.EqualTo (typeof (ITestType)));
      Assert.That (registry.ServiceConfigurationEntry.ImplementationInfos.Count, Is.EqualTo (1));
      Assert.That (registry.ServiceConfigurationEntry.ImplementationInfos[0].ImplementationType, Is.EqualTo (typeof (ITestType)));
      Assert.That (registry.ServiceConfigurationEntry.ImplementationInfos[0].Factory, Is.SameAs (instanceFactory));
      Assert.That (registry.ServiceConfigurationEntry.ImplementationInfos[0].Lifetime, Is.EqualTo (LifetimeKind.InstancePerDependency));
      Assert.That (registry.ServiceConfigurationEntry.ImplementationInfos[0].RegistrationType, Is.EqualTo (RegistrationType.Single));
    }
  }
}