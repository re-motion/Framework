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
using CommonServiceLocator;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class ServiceLocatorScopeTest
  {
    private IServiceLocator _locator1;
    private IServiceLocator _locator2;

    [SetUp]
    public void SetUp ()
    {
      _locator1 = MockRepository.GenerateStub<IServiceLocator>();
      _locator2 = MockRepository.GenerateStub<IServiceLocator>();

      ServiceLocator.SetLocatorProvider(null);
    }

    [TearDown]
    public void TearDown ()
    {
      ServiceLocator.SetLocatorProvider(null);
    }

    [Test]
    public void Initialization_AndDispose_ServiceLocator_Set ()
    {
      ServiceLocator.SetLocatorProvider(() => _locator1);
      Assert.That(ServiceLocator.Current, Is.SameAs(_locator1));

      using (new ServiceLocatorScope(_locator2))
      {
        Assert.That(ServiceLocator.Current, Is.SameAs(_locator2));
      }

      Assert.That(ServiceLocator.Current, Is.SameAs(_locator1));
    }

    [Test]
    public void Initialization_AndDispose_ServiceLocator_InitialLocatorNull ()
    {
      ServiceLocator.SetLocatorProvider(() => null);
      Assert.That(ServiceLocator.Current, Is.Null);

      using (new ServiceLocatorScope(_locator2))
      {
        Assert.That(ServiceLocator.Current, Is.SameAs(_locator2));
      }

      Assert.That(ServiceLocator.Current, Is.Null);
    }

    [Test]
    public void Initialization_AndDispose_ServiceLocator_InitialProviderNull ()
    {
      Assert.That(ServiceLocator.IsLocationProviderSet, Is.False);

      using (new ServiceLocatorScope(_locator2))
      {
        Assert.That(ServiceLocator.Current, Is.SameAs(_locator2));
      }

      Assert.That(ServiceLocator.IsLocationProviderSet, Is.False);
    }

    [Test]
    public void Initialization_AndDispose_ServiceLocator_SetNull ()
    {
      ServiceLocator.SetLocatorProvider(() => _locator1);
      Assert.That(ServiceLocator.Current, Is.SameAs(_locator1));

      using (new ServiceLocatorScope((IServiceLocator) null))
      {
        Assert.That(ServiceLocator.Current, Is.Null);
      }

      Assert.That(ServiceLocator.Current, Is.SameAs(_locator1));
    }

    [Test]
    public void Initialization_AndDispose_ServiceLocator_ServiceConfigurationEntries ()
    {
      ServiceLocator.SetLocatorProvider(() => _locator1);
      Assert.That(ServiceLocator.Current, Is.SameAs(_locator1));

      var entry1 = new ServiceConfigurationEntry(typeof(object), new ServiceImplementationInfo(typeof(DomainType1), LifetimeKind.InstancePerDependency));
      var entry2 = new ServiceConfigurationEntry(typeof(IFormattable), new ServiceImplementationInfo(typeof(DomainType2), LifetimeKind.Singleton));

      using (new ServiceLocatorScope(entry1, entry2))
      {
        Assert.That(ServiceLocator.Current, Is.Not.SameAs(_locator1));
        Assert.That(ServiceLocator.Current, Is.TypeOf<DefaultServiceLocator>());
        Assert.That(ServiceLocator.Current.GetInstance(typeof(object)), Is.TypeOf<DomainType1>());
        Assert.That(ServiceLocator.Current.GetInstance(typeof(object)), Is.Not.SameAs(ServiceLocator.Current.GetInstance(typeof(object))));
        Assert.That(ServiceLocator.Current.GetInstance(typeof(IFormattable)), Is.TypeOf<DomainType2>());
        Assert.That(ServiceLocator.Current.GetInstance(typeof(IFormattable)), Is.SameAs(ServiceLocator.Current.GetInstance(typeof(IFormattable))));
      }

      Assert.That(ServiceLocator.Current, Is.SameAs(_locator1));
    }

    class DomainType1 { }
    class DomainType2 : IFormattable {
      public string ToString (string format, IFormatProvider formatProvider)
      {
        throw new NotImplementedException();
      }
    }
  }
}
