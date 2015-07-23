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
using Remotion.Globalization.Mixins.UnitTests.TestDomain;
using Remotion.Mixins;
using Remotion.Mixins.Globalization;

namespace Remotion.Globalization.Mixins.UnitTests.Obsolete.MixedMultiLingualResourcesTests
{
  [Obsolete]
  [TestFixture]
  public class GetResourceManagerTest
  {
    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage =
        "Type Remotion.Globalization.Mixins.UnitTests.TestDomain.ClassWithoutMultiLingualResourcesAttributes "
        + "and its base classes do not define a resource attribute.")]
    public void NoAttributes_NoInheritance ()
    {
      MixedMultiLingualResources.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), false);
    }

    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage =
        "Type Remotion.Globalization.Mixins.UnitTests.TestDomain.ClassWithoutMultiLingualResourcesAttributes "
        + "and its base classes do not define a resource attribute.")]
    public void NoAttributes_Inheritance ()
    {
      MixedMultiLingualResources.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), true);
    }

    [Test]
    public void AttributesOnClass ()
    {
      ResourceManagerSet resourceManager =
          (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (ClassWithMultiLingualResourcesAttributes), false);

      Assert.That (resourceManager.ResourceManagers.Select (rm => rm.Name), Is.EqualTo (new[] { NamedResources.OnTarget }));
    }

    [Test]
    public void AttributesOnBase_Inheritance ()
    {
      ResourceManagerSet resourceManager =
          (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), true);

      Assert.That (resourceManager.ResourceManagers.Select (rm => rm.Name), Is.EqualTo (new[] { NamedResources.OnInherited, NamedResources.OnTarget }));
    }

    [Test]
    public void AttributesOnBase_DoesNotThrowResourceExceptionBecauseOfBug ()
    {
      ResourceManagerSet resourceManager =
          (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false);

      Assert.That (resourceManager.ResourceManagers.Select (rm => rm.Name), Is.EqualTo (new[] { NamedResources.OnInherited }));
    }

    [Test]
    public void AttributesOnBaseAndClass_InheritedDefault ()
    {
      ResourceManagerSet resourceManager =
          (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes));

      Assert.That (resourceManager.ResourceManagers.Select (rm => rm.Name), Is.EqualTo (new[] { NamedResources.OnInherited }));
    }

    [Test]
    public void AttributesOnBaseAndClass_InheritedFalse ()
    {
      ResourceManagerSet resourceManager =
          (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), false);

      Assert.That (resourceManager.ResourceManagers.Select (rm => rm.Name), Is.EqualTo (new[] { NamedResources.OnInherited }));
    }

    [Test]
    public void AttributesOnBaseAndClass_InheritedTrue ()
    {
      ResourceManagerSet resourceManager =
          (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);

      Assert.That (resourceManager.ResourceManagers.Select (rm => rm.Name), Is.EqualTo (new[] { NamedResources.OnInherited, NamedResources.OnTarget }));
    }

    [Test]
    public void AttributesFromMixin_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .EnterScope())
      {
        ResourceManagerSet resourceManager =
            (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), false);

        Assert.That (resourceManager.ResourceManagers.Select (rm => rm.Name), Is.EqualTo (new[] { NamedResources.OnMixin1 }));
      }
    }

    [Test]
    public void AttributesFromMixin_InheritedTrue ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .EnterScope())
      {
        ResourceManagerSet resourceManager =
            (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), true);

        Assert.That (resourceManager.ResourceManagers.Select (rm => rm.Name), Is.EqualTo (new[] { NamedResources.OnMixin1 }));
      }
    }

    [Test]
    public void AttributesFromMultipleMixins_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2>()
          .EnterScope())
      {
        ResourceManagerSet resourceManager =
            (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), false);

        Assert.That (resourceManager.ResourceManagers.Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnMixin1, NamedResources.OnMixin2a, NamedResources.OnMixin2b }));
        Assert.That (resourceManager.ResourceManagers.Take (1).Select (rm => rm.Name), Is.EqualTo (new[] { NamedResources.OnMixin1 }));
        Assert.That (resourceManager.ResourceManagers.Skip (1).Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnMixin2a, NamedResources.OnMixin2b }));
      }
    }

    [Test]
    public void AttributesFromMixinOfMixin_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2>()
          .ForClass<MixinAddingMultiLingualResourcesAttributes1>()
          .AddMixin<MixinOfMixinWithResources>()
          .EnterScope())
      {
        ResourceManagerSet resourceManager =
            (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), false);

        //Note: MixinOfMixin was previously sorted after the introducing mixin, but there is no know client code that actually used MixinOfMixin with resources.
        Assert.That (
            resourceManager.ResourceManagers.Select (rm => rm.Name),
            Is.EquivalentTo (new[] { NamedResources.MixinOfMixinWithResources, NamedResources.OnMixin1, NamedResources.OnMixin2a, NamedResources.OnMixin2b }));
        Assert.That (resourceManager.ResourceManagers.Take (2).Select (rm => rm.Name), Is.EqualTo (new[] { NamedResources.MixinOfMixinWithResources, NamedResources.OnMixin1 }));
        Assert.That (resourceManager.ResourceManagers.Skip (2).Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnMixin2a, NamedResources.OnMixin2b }));
      }
    }

    [Test]
    public void AttributesFromMixinsAndBaseAndClass_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .ForClass<InheritedClassWithMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2>()
          .EnterScope())
      {
        ResourceManagerSet resourceManager =
            (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), false);

        Assert.That (resourceManager.ResourceManagers.Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnInherited, NamedResources.OnMixin2a, NamedResources.OnMixin2b }));
        Assert.That (resourceManager.ResourceManagers.Take (1).Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnInherited }));
        Assert.That (resourceManager.ResourceManagers.Skip (1).Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnMixin2a, NamedResources.OnMixin2b }));
      }
    }

    [Test]
    public void AttributesFromMixinsAndBaseAndClass_InheritedTrue ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass<InheritedClassWithMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2>()
          .EnterScope())
      {
        ResourceManagerSet resourceManager =
            (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);

        Assert.That (
            resourceManager.ResourceManagers.Select (rm => rm.Name),
            Is.EquivalentTo (new[] { NamedResources.OnInherited, NamedResources.OnMixin1, NamedResources.OnMixin2a, NamedResources.OnMixin2b, NamedResources.OnTarget }));
        Assert.That (resourceManager.ResourceManagers.Take (2).Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnInherited, NamedResources.OnMixin1 }));
        Assert.That (resourceManager.ResourceManagers.Skip (2).Take (2).Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnMixin2a, NamedResources.OnMixin2b }));
        Assert.That (resourceManager.ResourceManagers.Skip (4).Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnTarget }));
      }
    }

    [Test]
    public void AttributesFromMixinsAndBase_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass<ClassWithMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .ForClass<InheritedClassWithMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2>()
          .EnterScope())
      {
        ResourceManagerSet resourceManager =
            (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false);

        Assert.That (resourceManager.ResourceManagers.Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnMixin1, NamedResources.OnMixin2a, NamedResources.OnMixin2b }));
        Assert.That (resourceManager.ResourceManagers.Take (1).Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnMixin1 }));
        Assert.That (resourceManager.ResourceManagers.Skip (1).Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnMixin2a, NamedResources.OnMixin2b }));
      }
    }

    [Test]
    public void AttributesFromMixinsAndBase2_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass<ClassWithMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2>()
          .ForClass<InheritedClassWithMultiLingualResourcesAttributes>()
          .EnterScope())
      {
        ResourceManagerSet resourceManager =
            (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false);

        Assert.That (resourceManager.ResourceManagers.Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnMixin1, NamedResources.OnMixin2a, NamedResources.OnMixin2b }));
        Assert.That (resourceManager.ResourceManagers.Take (1).Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnMixin1 }));
        Assert.That (resourceManager.ResourceManagers.Skip (1).Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnMixin2a, NamedResources.OnMixin2b }));
      }
    }

    [Test]
    public void AttributesFromMixinsOnBaseAndClass_InheritedTrue ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass<ClassWithMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .ForClass<InheritedClassWithMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2>()
          .EnterScope())
      {
        ResourceManagerSet resourceManager =
            (ResourceManagerSet) MixedMultiLingualResources.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);

        Assert.That (
            resourceManager.ResourceManagers.Select (rm => rm.Name),
            Is.EquivalentTo (new[] { NamedResources.OnInherited, NamedResources.OnMixin1, NamedResources.OnMixin2a, NamedResources.OnMixin2b, NamedResources.OnTarget }));
        Assert.That (resourceManager.ResourceManagers.Take (2).Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnInherited, NamedResources.OnMixin1 }));
        Assert.That (resourceManager.ResourceManagers.Skip (2).Take (2).Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnMixin2a, NamedResources.OnMixin2b }));
        Assert.That (resourceManager.ResourceManagers.Skip (4).Select (rm => rm.Name), Is.EquivalentTo (new[] { NamedResources.OnTarget }));
      }
    }
  }
}