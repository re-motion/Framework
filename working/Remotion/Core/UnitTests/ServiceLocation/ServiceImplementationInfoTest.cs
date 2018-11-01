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
  public class ServiceImplementationInfoTest
  {
    [Test]
    public void CreateSingle_RetrievesImplementationType ()
    {
      Func<ServiceImplementationInfoTest> factory = () => new ServiceImplementationInfoTest();
      var implementationInfo = ServiceImplementationInfo.CreateSingle (factory);

      Assert.That (implementationInfo.Factory, Is.Not.Null);
      Assert.That (implementationInfo.ImplementationType, Is.SameAs (typeof (ServiceImplementationInfoTest)));
      Assert.That (implementationInfo.Lifetime, Is.EqualTo (LifetimeKind.InstancePerDependency));
    }

    [Test]
    public void CreateSingle_WithLifetime ()
    {
      Func<ServiceImplementationInfoTest> factory = () => new ServiceImplementationInfoTest();
      var implementationInfo = ServiceImplementationInfo.CreateSingle (factory, LifetimeKind.Singleton);

      Assert.That (implementationInfo.Factory, Is.Not.Null);
      Assert.That (implementationInfo.ImplementationType, Is.SameAs (typeof (ServiceImplementationInfoTest)));
      Assert.That (implementationInfo.Lifetime, Is.EqualTo (LifetimeKind.Singleton));
    }

    [Test]
    public void CreateMultiple_RetrievesImplementationType ()
    {
      Func<ServiceImplementationInfoTest> factory = () => new ServiceImplementationInfoTest();
      var implementationInfo = ServiceImplementationInfo.CreateSingle (factory);

      Assert.That (implementationInfo.Factory, Is.Not.Null);
      Assert.That (implementationInfo.ImplementationType, Is.SameAs (typeof (ServiceImplementationInfoTest)));
      Assert.That (implementationInfo.Lifetime, Is.EqualTo (LifetimeKind.InstancePerDependency));
    }

    [Test]
    public void CreateMultiple_WithLifetime ()
    {
      Func<ServiceImplementationInfoTest> factory = () => new ServiceImplementationInfoTest();
      var implementationInfo = ServiceImplementationInfo.CreateSingle (factory, LifetimeKind.Singleton);

      Assert.That (implementationInfo.Factory, Is.Not.Null);
      Assert.That (implementationInfo.ImplementationType, Is.SameAs (typeof (ServiceImplementationInfoTest)));
      Assert.That (implementationInfo.Lifetime, Is.EqualTo (LifetimeKind.Singleton));
    }

    [Test]
    public void InitializeDecorator_WithLifetimeSingleton_ThrowsArgumentException ()
    {
      Assert.That (
          () => new ServiceImplementationInfo (typeof (ServiceImplementationInfoTest), LifetimeKind.Singleton, RegistrationType.Decorator),
          Throws.ArgumentException.And.Message.EqualTo (
              "For implementations of type 'Decorator', the lifetime can only be specified as 'InstancePerDependency'.\r\nParameter name: lifetime"));
    }

    [Test]
    public void ToString_DebugInfo ()
    {
      var implementation0 = new ServiceImplementationInfo (typeof (object), LifetimeKind.Singleton, RegistrationType.Compound);
      var implementation1 = new ServiceImplementationInfo (typeof (string), LifetimeKind.InstancePerDependency, RegistrationType.Multiple);

      Assert.That (implementation0.ToString (), Is.EqualTo ("{System.Object, Singleton, Compound}"));
      Assert.That (implementation1.ToString (), Is.EqualTo ("{System.String, InstancePerDependency, Multiple}"));
    }
  }
}