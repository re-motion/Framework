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
using Remotion.Globalization.Implementation;
using Rhino.Mocks;

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
    private IResourceManagerFactory _factoryStub;

    [SetUp]
    public void SetUp ()
    {
      _factoryStub = MockRepository.GenerateStub<IResourceManagerFactory>();
      _factoryStub.Stub (_ => _.CreateResourceManager (typeof (object))).Return (NullResourceManager.Instance);

      _resolver = new ResourceManagerResolver (_factoryStub);
    }

    [Test]
    public void Resolve_WithResourceManagerOnlyDefinedOnCurrentType_ReturnsNullResourceManagerForInheritedResourceManager ()
    {
      var resourceManagerStub = MockRepository.GenerateStub<IResourceManager>();
      _factoryStub.Stub (_ => _.CreateResourceManager (typeof (Class))).Return (resourceManagerStub);
      _factoryStub.Stub (_ => _.CreateResourceManager (typeof (BaseClass))).Return (NullResourceManager.Instance);

      var result = _resolver.Resolve (typeof (Class));

      Assert.That (result.ResourceManager, Is.SameAs (resourceManagerStub));
      Assert.That (result.DefinedResourceManager, Is.SameAs (result.ResourceManager));
      Assert.That (result.InheritedResourceManager.IsNull, Is.True);
    }

    [Test]
    public void Resolve_WithResourceManagerOnlyDefinedOnBaseTypeReturnsNullResourceManagerForDefinedResourceManager ()
    {
      var resourceManagerStub = MockRepository.GenerateStub<IResourceManager>();
      _factoryStub.Stub (_ => _.CreateResourceManager (typeof (Class))).Return (NullResourceManager.Instance);
      _factoryStub.Stub (_ => _.CreateResourceManager (typeof (BaseClass))).Return (resourceManagerStub);

      var result = _resolver.Resolve (typeof (Class));

      Assert.That (result.ResourceManager, Is.SameAs (resourceManagerStub));
      Assert.That (result.DefinedResourceManager.IsNull, Is.True);
      Assert.That (result.InheritedResourceManager, Is.SameAs (result.ResourceManager));
    }

    [Test]
    public void Resolve_WithMultipleResourceManagersDefinedOnTypeHierarchy_ReturnsResourceManagersInOrderOfDefinition ()
    {
      var resourceManagerOnBaseClassStub = MockRepository.GenerateStub<IResourceManager>();
      resourceManagerOnBaseClassStub.Stub (_ => _.Name).Return ("Base");

      var resourceManagerOnClassStub = MockRepository.GenerateStub<IResourceManager>();
      resourceManagerOnClassStub.Stub (_ => _.Name).Return ("Class");

      var resourceManagerOnDerivedClassStub = MockRepository.GenerateStub<IResourceManager>();
      resourceManagerOnDerivedClassStub.Stub (_ => _.Name).Return ("Derived");

      _factoryStub.Stub (_ => _.CreateResourceManager (typeof (BaseClass))).Return (resourceManagerOnBaseClassStub);
      _factoryStub.Stub (_ => _.CreateResourceManager (typeof (Class))).Return (resourceManagerOnClassStub);
      _factoryStub.Stub (_ => _.CreateResourceManager (typeof (DerivedClass))).Return (resourceManagerOnDerivedClassStub);

      var result = _resolver.Resolve (typeof (DerivedClass));

      Assert.That (result.ResourceManager, Is.InstanceOf<ResourceManagerSet>());
      Assert.That (result.DefinedResourceManager, Is.SameAs (resourceManagerOnDerivedClassStub));
      Assert.That (result.InheritedResourceManager, Is.InstanceOf<ResourceManagerSet>());
      var inheritedResourceManagerSet = (ResourceManagerSet) result.InheritedResourceManager;
      Assert.That (inheritedResourceManagerSet.ResourceManagers, Is.EqualTo (new[] { resourceManagerOnClassStub, resourceManagerOnBaseClassStub }));
    }

    [Test]
    public void Resolve_WithoutResourcesDefinedOnTypeHierarchy_ReturnsNullResult ()
    {
      _factoryStub.Stub (_ => _.CreateResourceManager (typeof (Class))).Return (NullResourceManager.Instance);
      _factoryStub.Stub (_ => _.CreateResourceManager (typeof (BaseClass))).Return (NullResourceManager.Instance);

      var result = _resolver.Resolve (typeof (Class));

      Assert.That (result.IsNull, Is.True);
    }
  }
}