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
using Remotion.Globalization.Implementation;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class ResourceManagerResolverTest
  {
    private class BaseClass
    {
    }

    private class Class : BaseClass
    {
    }

    private class DerivedClass : Class
    {
    }

    private ResourceManagerResolver _resolver;
    private Mock<IResourceManagerFactory> _factoryStub;

    [SetUp]
    public void SetUp ()
    {
      _factoryStub = new Mock<IResourceManagerFactory>();
      _factoryStub.Setup(_ => _.CreateResourceManager(typeof (object))).Returns(NullResourceManager.Instance);

      _resolver = new ResourceManagerResolver(_factoryStub.Object);
    }

    [Test]
    public void Resolve_WithResourceManagerOnlyDefinedOnCurrentType_ReturnsNullResourceManagerForInheritedResourceManager ()
    {
      var resourceManagerStub = new Mock<IResourceManager>();
      _factoryStub.Setup(_ => _.CreateResourceManager(typeof (Class))).Returns(resourceManagerStub.Object);
      _factoryStub.Setup(_ => _.CreateResourceManager(typeof (BaseClass))).Returns(NullResourceManager.Instance);

      var result = _resolver.Resolve(typeof (Class));

      Assert.That(result.ResourceManager, Is.SameAs(resourceManagerStub.Object));
      Assert.That(result.DefinedResourceManager, Is.SameAs(result.ResourceManager));
      Assert.That(result.InheritedResourceManager.IsNull, Is.True);
    }

    [Test]
    public void Resolve_WithResourceManagerOnlyDefinedOnBaseTypeReturnsNullResourceManagerForDefinedResourceManager ()
    {
      var resourceManagerStub = new Mock<IResourceManager>();
      _factoryStub.Setup(_ => _.CreateResourceManager(typeof (Class))).Returns(NullResourceManager.Instance);
      _factoryStub.Setup(_ => _.CreateResourceManager(typeof (BaseClass))).Returns(resourceManagerStub.Object);

      var result = _resolver.Resolve(typeof (Class));

      Assert.That(result.ResourceManager, Is.SameAs(resourceManagerStub.Object));
      Assert.That(result.DefinedResourceManager.IsNull, Is.True);
      Assert.That(result.InheritedResourceManager, Is.SameAs(result.ResourceManager));
    }

    [Test]
    public void Resolve_WithMultipleResourceManagersDefinedOnTypeHierarchy_ReturnsResourceManagersInOrderOfDefinition ()
    {
      var resourceManagerOnBaseClassStub = new Mock<IResourceManager>();
      resourceManagerOnBaseClassStub.Setup(_ => _.Name).Returns("Base");

      var resourceManagerOnClassStub = new Mock<IResourceManager>();
      resourceManagerOnClassStub.Setup(_ => _.Name).Returns("Class");

      var resourceManagerOnDerivedClassStub = new Mock<IResourceManager>();
      resourceManagerOnDerivedClassStub.Setup(_ => _.Name).Returns("Derived");

      _factoryStub.Setup(_ => _.CreateResourceManager(typeof (BaseClass))).Returns(resourceManagerOnBaseClassStub.Object);
      _factoryStub.Setup(_ => _.CreateResourceManager(typeof (Class))).Returns(resourceManagerOnClassStub.Object);
      _factoryStub.Setup(_ => _.CreateResourceManager(typeof (DerivedClass))).Returns(resourceManagerOnDerivedClassStub.Object);

      var result = _resolver.Resolve(typeof (DerivedClass));

      Assert.That(result.ResourceManager, Is.InstanceOf<ResourceManagerSet>());
      Assert.That(result.DefinedResourceManager, Is.SameAs(resourceManagerOnDerivedClassStub.Object));
      Assert.That(result.InheritedResourceManager, Is.InstanceOf<ResourceManagerSet>());
      var inheritedResourceManagerSet = (ResourceManagerSet) result.InheritedResourceManager;
      Assert.That(inheritedResourceManagerSet.ResourceManagers, Is.EqualTo(new[] { resourceManagerOnClassStub.Object, resourceManagerOnBaseClassStub.Object }));
    }

    [Test]
    public void Resolve_WithoutResourcesDefinedOnTypeHierarchy_ReturnsNullResult ()
    {
      _factoryStub.Setup(_ => _.CreateResourceManager(typeof (Class))).Returns(NullResourceManager.Instance);
      _factoryStub.Setup(_ => _.CreateResourceManager(typeof (BaseClass))).Returns(NullResourceManager.Instance);

      var result = _resolver.Resolve(typeof (Class));

      Assert.That(result.IsNull, Is.True);
    }
  }
}