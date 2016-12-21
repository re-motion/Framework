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
using Remotion.Globalization.UnitTests.TestDomain;

namespace Remotion.Globalization.UnitTests.Obsolete
{
  [Obsolete]
  [TestFixture]
  public class MultiLingualResourcesTest
  {
    [Test]
    public void GetResourceManager_TypeWithResources_ReturnsResources ()
    {
      var resourceManager = MultiLingualResources.GetResourceManager (typeof (ClassWithMultiLingualResourcesAttributes), true);

      Assert.That (resourceManager.IsNull, Is.False);
      Assert.That (resourceManager, Is.InstanceOf<ResourceManagerSet>());

      var resourceManagerSet = (ResourceManagerSet) resourceManager;

      Assert.That (
          resourceManagerSet.ResourceManagers.Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.One, NamedResources.Two, NamedResources.Three }));
    }

    [Test]
    public void GetResourceManager_TypeWithoutResources_ThrowsResourceException ()
    {
      Assert.That (
          () => MultiLingualResources.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), true),
          Throws.TypeOf<ResourceException>());
    }

    [Test]
    public void GetResourceManager_WithTypeDefiningAndInheritingMultipleResources_ReturnsResourceManagersInOrderOfDefinition ()
    {
      var resourceManager = MultiLingualResources.GetResourceManager (typeof (DerivedClassWithMultiLingualResourcesAttributes), true);

      Assert.That (resourceManager, Is.InstanceOf<ResourceManagerSet>());
      var resourceManagerSet = (ResourceManagerSet) resourceManager;
      Assert.That (
          resourceManagerSet.ResourceManagers.Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.Four, NamedResources.Five, NamedResources.One, NamedResources.Two, NamedResources.Three }));
      Assert.That (
          resourceManagerSet.ResourceManagers.Take (2).Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.Four, NamedResources.Five }));
      Assert.That (
          resourceManagerSet.ResourceManagers.Skip (2).Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.One, NamedResources.Two, NamedResources.Three }));
    }

    [Test]
    public void GetResourceManagerWithoutInheritance_WithTypeDefiningAndInheritingMultipleResources_ReturnsResourceManagersInOrderOfDefinition ()
    {
      var resourceManager = MultiLingualResources.GetResourceManager (typeof (DerivedClassWithMultiLingualResourcesAttributes));

      Assert.That (resourceManager, Is.InstanceOf<ResourceManagerSet>());
      var resourceManagerSet = (ResourceManagerSet) resourceManager;
      Assert.That (
          resourceManagerSet.ResourceManagers.Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.Four, NamedResources.Five }));
    }

    [Test]
    public void GetResourceManager_WithTypeDefiningAndInheritingMultipleResources_AndDoNotGetInheritedResources_ReturnsResourceManagersInOrderOfDefinition ()
    {
      var resourceManager = MultiLingualResources.GetResourceManager (typeof (DerivedClassWithMultiLingualResourcesAttributes), false);

      Assert.That (resourceManager, Is.InstanceOf<ResourceManagerSet>());
      var resourceManagerSet = (ResourceManagerSet) resourceManager;
      Assert.That (
          resourceManagerSet.ResourceManagers.Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.Four, NamedResources.Five }));
    }

    [Test]
    public void GetResourceManager_TypeWithOnlyInheritedResources_AndDoNotGetInheritedResources_DoesNotThrowResourceExceptionBecauseOfBug ()
    {
      var resourceManager = MultiLingualResources.GetResourceManager (typeof (DerivedClassWithoutMultiLingualResourcesAttributes), false);

      Assert.That (resourceManager.IsNull, Is.False);
      Assert.That (resourceManager, Is.InstanceOf<ResourceManagerSet>());

      var resourceManagerSet = (ResourceManagerSet) resourceManager;

      Assert.That (
          resourceManagerSet.ResourceManagers.Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.Four, NamedResources.Five }));
    }
  }
}