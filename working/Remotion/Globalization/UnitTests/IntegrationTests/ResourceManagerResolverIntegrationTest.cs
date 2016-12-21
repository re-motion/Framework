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
using Remotion.Globalization.Implementation;
using Remotion.Globalization.UnitTests.TestDomain;
using Remotion.ServiceLocation;

namespace Remotion.Globalization.UnitTests.IntegrationTests
{
  [TestFixture]
  public class ResourceManagerResolverIntegrationTest
  {
    private IResourceManagerResolver _resolver;

    [SetUp]
    public void SetUp ()
    {
      _resolver = SafeServiceLocator.Current.GetInstance<IResourceManagerResolver>();
    }

    [Test]
    public void Resolve_WithTypeDefiningMultipleResources_ReturnsResourceManagersInOrderOfDefinition ()
    {
      var result = _resolver.Resolve (typeof (ClassWithMultiLingualResourcesAttributes));

      Assert.That (result.ResourceManager.IsNull, Is.False);
      Assert.That (result.ResourceManager, Is.InstanceOf<ResourceManagerSet>());
      Assert.That (result.DefinedResourceManager, Is.SameAs (result.ResourceManager));
      Assert.That (result.InheritedResourceManager.IsNull, Is.True);

      var resourceManagerSet = (ResourceManagerSet) result.ResourceManager;

      Assert.That (
          resourceManagerSet.ResourceManagers.Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.One, NamedResources.Two, NamedResources.Three }));
    }

    [Test]
    public void Resolve_WithTypeDefiningAndInheritingMultipleResources_ReturnsResourceManagersInOrderOfDefinition ()
    {
      var result = _resolver.Resolve (typeof (DerivedClassWithMultiLingualResourcesAttributes));

      Assert.That (result.ResourceManager, Is.InstanceOf<ResourceManagerSet>());
      var resourceManagerSet = (ResourceManagerSet) result.ResourceManager;
      Assert.That (
          resourceManagerSet.ResourceManagers.Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.Four, NamedResources.Five, NamedResources.One, NamedResources.Two, NamedResources.Three }));
      Assert.That (
          resourceManagerSet.ResourceManagers.Take (2).Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.Four, NamedResources.Five }));
      Assert.That (
          resourceManagerSet.ResourceManagers.Skip (2).Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.One, NamedResources.Two, NamedResources.Three }));

      Assert.That (result.DefinedResourceManager, Is.InstanceOf<ResourceManagerSet>());
      var definedResourceManagerSet = (ResourceManagerSet) result.DefinedResourceManager;
      Assert.That (
          definedResourceManagerSet.ResourceManagers.Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.Four, NamedResources.Five }));

      Assert.That (result.InheritedResourceManager, Is.InstanceOf<ResourceManagerSet>());
      var inheritedResourceManagerSet = (ResourceManagerSet) result.InheritedResourceManager;
      Assert.That (
          inheritedResourceManagerSet.ResourceManagers.Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.One, NamedResources.Two, NamedResources.Three }));
    }

    [Test]
    public void Resolve_WithTypeOnlyInheritingMultipleResources_ReturnsNullResourceManagerForDefinedResourceManager ()
    {
      var result = _resolver.Resolve (typeof (DerivedClassWithoutMultiLingualResourcesAttributes));

      Assert.That (result.ResourceManager, Is.InstanceOf<ResourceManagerSet>());
      var resourceManagerSet = (ResourceManagerSet) result.ResourceManager;
      Assert.That (
          resourceManagerSet.ResourceManagers.Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.Four, NamedResources.Five, NamedResources.One, NamedResources.Two, NamedResources.Three }));
      Assert.That (
          resourceManagerSet.ResourceManagers.Take (2).Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.Four, NamedResources.Five }));
      Assert.That (
          resourceManagerSet.ResourceManagers.Skip (2).Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.One, NamedResources.Two, NamedResources.Three }));

      Assert.That (result.DefinedResourceManager.IsNull, Is.True);

      Assert.That (result.InheritedResourceManager, Is.SameAs (result.ResourceManager));
    }
  }
}