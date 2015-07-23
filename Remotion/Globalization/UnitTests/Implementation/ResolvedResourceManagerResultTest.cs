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
  public class ResolvedResourceManagerResultTest
  {
    [Test]
    public void Empty ()
    {
      Assert.That (ResolvedResourceManagerResult.Null.IsNull, Is.True);
      Assert.That (ResolvedResourceManagerResult.Null.ResourceManager.IsNull, Is.True);
      Assert.That (ResolvedResourceManagerResult.Null.DefinedResourceManager.IsNull, Is.True);
      Assert.That (ResolvedResourceManagerResult.Null.InheritedResourceManager.IsNull, Is.True);
    }

    [Test]
    public void Create_WithBothDefinedAndInheritedResourceManagerIsNotNull_CombinesResourceManagers ()
    {
      var definedResourceManager = MockRepository.GenerateStub<IResourceManager>();
      var inheritedResourceManager = MockRepository.GenerateStub<IResourceManager>();

      var result = ResolvedResourceManagerResult.Create (definedResourceManager, inheritedResourceManager);

      Assert.That (result.IsNull, Is.False);
      Assert.That (result.ResourceManager, Is.InstanceOf<ResourceManagerSet>());
      Assert.That (
          ((ResourceManagerSet) result.ResourceManager).ResourceManagers,
          Is.EqualTo (new[] { definedResourceManager, inheritedResourceManager }));
      Assert.That (result.DefinedResourceManager, Is.SameAs (definedResourceManager));
      Assert.That (result.InheritedResourceManager, Is.SameAs (inheritedResourceManager));
    }

    [Test]
    public void Create_WithDefinedResourceManagerIsNull_UsesInheritedResourceManager ()
    {
      var definedResourceManager = NullResourceManager.Instance;
      var inheritedResourceManager = MockRepository.GenerateStub<IResourceManager>();

      var result = ResolvedResourceManagerResult.Create (definedResourceManager, inheritedResourceManager);

      Assert.That (result.IsNull, Is.False);
      Assert.That (result.ResourceManager, Is.SameAs (inheritedResourceManager));
      Assert.That (result.DefinedResourceManager, Is.SameAs (definedResourceManager));
      Assert.That (result.InheritedResourceManager, Is.SameAs (inheritedResourceManager));
    }

    [Test]
    public void Create_WithInheritedResourceManagerIsNotNull_UsesDefinedResourceManager ()
    {
      var definedResourceManager = MockRepository.GenerateStub<IResourceManager>();
      var inheritedResourceManager = NullResourceManager.Instance;

      var result = ResolvedResourceManagerResult.Create (definedResourceManager, inheritedResourceManager);

      Assert.That (result.IsNull, Is.False);
      Assert.That (result.ResourceManager, Is.SameAs (definedResourceManager));
      Assert.That (result.DefinedResourceManager, Is.SameAs (definedResourceManager));
      Assert.That (result.InheritedResourceManager, Is.SameAs (inheritedResourceManager));
    }

    [Test]
    public void Create_WithBothDefinedAndInheritedResourceManagerIsNull_UsesNullValue ()
    {
      var result = ResolvedResourceManagerResult.Create (NullResourceManager.Instance, NullResourceManager.Instance);

      Assert.That (result, Is.SameAs (ResolvedResourceManagerResult.Null));
    }
  }
}