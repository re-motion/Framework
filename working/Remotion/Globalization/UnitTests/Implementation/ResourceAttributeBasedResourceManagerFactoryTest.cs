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
using System.Resources;
using NUnit.Framework;
using Remotion.Globalization.Implementation;
using Remotion.Globalization.UnitTests.TestDomain;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class ResourceAttributeBasedResourceManagerFactoryTest
  {
    private IResourceManagerFactory _factory;

    [SetUp]
    public void SetUp ()
    {
      _factory = new ResourceAttributeBasedResourceManagerFactory();
    }

    [Test]
    public void CreateResourceManager_WithTypeDefiningSingleResource_ReturnsResourceManager ()
    {
      var result = _factory.CreateResourceManager (typeof (ClassWithResources));

      Assert.That (result.IsNull, Is.False);
      Assert.That (result.Name, Is.EqualTo ("Remotion.Globalization.UnitTests.TestDomain.Resources.ClassWithResources"));
    }

    [Test]
    public void CreateResourceManager_WithTypeDefiningMultipleResources_ReturnsResourceManagersInOrderOfDefinition ()
    {
      var result =  _factory.CreateResourceManager (typeof (ClassWithMultiLingualResourcesAttributes));

      Assert.That (result.IsNull, Is.False);
      Assert.That (result, Is.InstanceOf<ResourceManagerSet>());

      var resourceManagerSet = (ResourceManagerSet) result;

      Assert.That (
          resourceManagerSet.ResourceManagers.Select (rm => rm.Name),
          Is.EquivalentTo (new[] { NamedResources.One, NamedResources.Two, NamedResources.Three }));
    }

    [Test]
    public void CreateResourceManager_TypeWithoutResources_ReturnsNullResult ()
    {
      var result =  _factory.CreateResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes));

      Assert.That (result.IsNull, Is.True);
    }

    [Test]
    public void CreateResourceManager_UsesCache ()
    {
      var resourceManagers1 = _factory.CreateResourceManager (typeof (ClassWithResources));
      var resourceManagers2 = _factory.CreateResourceManager (typeof (ClassWithResources));

      Assert.That (resourceManagers2, Is.Not.SameAs (resourceManagers1));
      Assert.That (resourceManagers1, Is.InstanceOf<ResourceManagerSet>());
      Assert.That (resourceManagers2, Is.InstanceOf<ResourceManagerSet>());
      Assert.That (
          ((ResourceManagerWrapper)((ResourceManagerSet) resourceManagers2).ResourceManagers[0]).ResourceManager,
          Is.SameAs (((ResourceManagerWrapper)((ResourceManagerSet) resourceManagers1).ResourceManagers[0]).ResourceManager));
    }

    [Test]
    public void CreateResourceManager_MissingResourceFile_ThrowsMissingManifestResourceException ()
    {
      Assert.That (
          () => _factory.CreateResourceManager (typeof (ClassWithMissingResources)),
          Throws.TypeOf<MissingManifestResourceException>()
              .With.Message.EqualTo (
                  "Could not find any resources appropriate for the neutral culture. "
                  + "Make sure 'MissingResources.resources' was correctly embedded into assembly 'Remotion.Globalization.UnitTests' at compile time."));
    }
  }
}