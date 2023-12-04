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
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.ExtensibleEnums.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.ExtensibleEnums.UnitTests.Infrastructure
{
  [TestFixture]
  public class ExtensibleEnumDefinitionCacheTest
  {
    private ExtensibleEnumDefinitionCache _cache;
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      _cache = new ExtensibleEnumDefinitionCache(new ExtensibleEnumValueDiscoveryService());
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_cache.ValueDiscoveryService, Is.InstanceOf(typeof(ExtensibleEnumValueDiscoveryService)));
      Assert.That(
          ((ExtensibleEnumValueDiscoveryService)_cache.ValueDiscoveryService).TypeDiscoveryService,
          Is.SameAs(ContextAwareTypeUtility.GetTypeDiscoveryService()));
    }

    [Test]
    public void GetDefinition ()
    {
      IExtensibleEnumDefinition instance = _cache.GetDefinition(typeof(Color));

      Assert.That(instance, Is.InstanceOf(typeof(ExtensibleEnumDefinition<Color>)));
    }

    [Test]
    public void GetDefinition_SetsDiscoveryService ()
    {
      IExtensibleEnumDefinition instance = _cache.GetDefinition(typeof(Color));

      Assert.That(PrivateInvoke.GetNonPublicField(instance, "_valueDiscoveryService"), Is.SameAs(_cache.ValueDiscoveryService));
    }

    [Test]
    public void GetDefinition_Cached ()
    {
      IExtensibleEnumDefinition instance1 = _cache.GetDefinition(typeof(Color));
      IExtensibleEnumDefinition instance2 = _cache.GetDefinition(typeof(Color));

      Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void GetDefinition_ThrowsOnInvalidType ()
    {
      Assert.That(
          () => _cache.GetDefinition(typeof(object)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Type 'System.Object' is not an extensible enum type "
                  + "derived from ExtensibleEnum<T>.", "extensibleEnumType"));
    }

    [Test]
    public void GetDefinition_ThrowsOnDerivedEnum ()
    {
      Assert.That(
          () => _cache.GetDefinition(typeof(MetallicColor)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Type 'Remotion.ExtensibleEnums.UnitTests.TestDomain.MetallicColor' is not an extensible enum type "
                  + "derived from ExtensibleEnum<T>.", "extensibleEnumType"));
    }

    [Test]
    public void GetInstance_Once ()
    {
      var factory = _serviceLocator.GetInstance<ExtensibleEnumDefinitionCache>();

      Assert.That(factory, Is.TypeOf(typeof(ExtensibleEnumDefinitionCache)));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var factory1 = _serviceLocator.GetInstance<ExtensibleEnumDefinitionCache>();
      var factory2 = _serviceLocator.GetInstance<ExtensibleEnumDefinitionCache>();

      Assert.That(factory1, Is.SameAs(factory2));
    }
  }
}
