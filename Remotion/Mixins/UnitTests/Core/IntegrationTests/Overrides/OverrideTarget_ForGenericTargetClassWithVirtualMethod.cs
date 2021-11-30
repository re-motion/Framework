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

namespace Remotion.Mixins.UnitTests.Core.IntegrationTests.Overrides
{
  [TestFixture]
  public class OverrideTarget_ForGenericTargetClassWithVirtualMethod
  {
    [Test]
    [TestCase(typeof(MixinWithImplicitTargetSpecification<>), "TheMixin.M -> C.M")]
    [TestCase(typeof(MixinWithoutTargetSpecification<>), "TheMixin.M")]
    [TestCase(typeof(MixinWithExplicitTargetSpecification<>), "TheMixin.M -> C.M", IgnoreReason = "RM-2745")]
    public void InstantiateTargetType_ShouldOverrideTargetMethodFromTargetType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass(typeof(C<>)).AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<C<int>>();
        Assert.That(instance.M(1), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [TestCase(typeof(MixinWithImplicitTargetSpecification<>), "TheMixin.M -> C.M")]
    [TestCase(typeof(MixinWithoutTargetSpecification<>), "TheMixin.M")]
    [TestCase(typeof(MixinWithExplicitTargetSpecification<>), "TheMixin.M -> C.M", IgnoreReason = "RM-2745")]
    public void InstantiateDerivedTypeWithoutOverride_ShouldOverrideTargetMethodFromTargetType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass(typeof(C<>)).AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<D1<int>>();
        Assert.That(instance.M(1), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [TestCase(typeof(MixinWithImplicitTargetSpecification<>), "TheMixin.M -> D2.M")]
    [TestCase(typeof(MixinWithoutTargetSpecification<>), "TheMixin.M")]
    [TestCase(typeof(MixinWithExplicitTargetSpecification<>), "TheMixin.M -> D2.M", IgnoreReason = "RM-2745")]
    public void InstantiateDerivedType_ShouldOverrideTargetMethodFromDerivedType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass(typeof(C<>)).AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<D2<int>>();
        Assert.That(instance.M(1), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [TestCase(typeof(MixinWithImplicitTargetSpecification<int>), "TheMixin.M -> D2.M")]
    [TestCase(typeof(MixinWithoutTargetSpecification<int>), "TheMixin.M")]
    [TestCase(typeof(MixinWithExplicitTargetSpecification<int>), "TheMixin.M -> D2.M", IgnoreReason = "RM-2745")]
    public void InstantiateDerivedDerivedTypeWithoutOverride_ShouldOverrideTargetMethodFromDerivedType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass(typeof(C<>)).AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<E<int>>();
        Assert.That(instance.M(1), Is.EqualTo(expectedMethodOutput));
      }
    }


    [Test]
    [Ignore("RM-2745")]
    [TestCase(typeof(MixinWithImplicitTargetSpecification<>), "TheMixin.M -> C.M")]
    [TestCase(typeof(MixinWithoutTargetSpecification<>), "TheMixin.M")]
    [TestCase(typeof(MixinWithExplicitTargetSpecification<>), "TheMixin.M -> C.M")]
    public void InstantiateShadowOfTargetType_ShouldOverrideTargetMethodFromTargetType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass(typeof(C<>)).AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<C_Shadow<int>>();
        Assert.That(instance.M(1), Is.EqualTo("C_Shadow.M"));
        Assert.That(((C<int>)instance).M(1), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [Ignore("RM-2745")]
    [TestCase(typeof(MixinWithImplicitTargetSpecification<>), "TheMixin.M -> C.M")]
    [TestCase(typeof(MixinWithoutTargetSpecification<>), "TheMixin.M")]
    [TestCase(typeof(MixinWithExplicitTargetSpecification<>), "TheMixin.M -> C.M")]
    public void InstantiateShadowOfDerivedTypeWithoutOverride_ShouldOverrideTargetMethodFromTargetType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass(typeof(C<>)).AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<D1_Shadow<int>>();
        Assert.That(instance.M(1), Is.EqualTo("D1_Shadow.M"));
        Assert.That(((C<int>)instance).M(1), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [Ignore("RM-2745")]
    [TestCase(typeof(MixinWithImplicitTargetSpecification<>), "TheMixin.M -> D2.M")]
    [TestCase(typeof(MixinWithoutTargetSpecification<>), "TheMixin.M")]
    [TestCase(typeof(MixinWithExplicitTargetSpecification<>), "TheMixin.M -> D2.M")]
    public void InstantiateShadowOfDerivedTypeWithOverride_ShouldOverrideTargetMethodFromDerivedType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass(typeof(C<>)).AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<D2_Shadow<int>>();
        Assert.That(instance.M(1), Is.EqualTo("D2_Shadow.M"));
        Assert.That(((C<int>)instance).M(1), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [Ignore("RM-2745")]
    [TestCase(typeof(MixinWithImplicitTargetSpecification<>), "TheMixin.M -> D2.M")]
    [TestCase(typeof(MixinWithoutTargetSpecification<>), "TheMixin.M")]
    [TestCase(typeof(MixinWithExplicitTargetSpecification<>), "TheMixin.M -> D2.M", IgnoreReason = "RM-2745")]
    public void InstantiateShadowOfDerivedTypeWithoutOverrideOfDerivedTypeWithOverride_ShouldOverrideTargetMethodFromDerivedType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass(typeof(C<>)).AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<E_Shadow<int>>();
        Assert.That(instance.M(1), Is.EqualTo("E_Shadow.M"));
        Assert.That(((C<int>)instance).M(1), Is.EqualTo(expectedMethodOutput));
      }
    }


    public class C<T>
    {
      public virtual string M (T p1)
      {
        return "C.M";
      }
    }

    public class C_Shadow<T> : C<T>
    {
      public new virtual string M (T p1)
      {
        return "C_Shadow.M";
      }
    }

    public class D1<T> : C<T>
    {
      // No Override
    }

    public class D1_Shadow<T> : D1<T>
    {
      public new virtual string M (T p1)
      {
        return "D1_Shadow.M";
      }
    }

    public class D2<T> : C<T>
    {
      public override string M (T p1)
      {
        return "D2.M";
      }
    }

    public class D2_Shadow<T> : D2<T>
    {
      public new virtual string M (T p1)
      {
        return "D2_Shadow.M";
      }
    }

    public class E<T> : D2<T>
    {
      // No Override
    }

    public class E_Shadow<T> : E<T>
    {
      public new virtual string M (T p1)
      {
        return "E_Shadow.M";
      }
    }

    public class MixinWithImplicitTargetSpecification<[BindToGenericTargetParameter] T> : Mixin<C<T>, MixinWithImplicitTargetSpecification<T>.I>
    {
      public interface I
      {
        string M (T p1);
      }

      [OverrideTarget]
      public string M (T p1)
      {
        return "TheMixin.M -> " + Next.M(p1);
      }
    }

    public class MixinWithExplicitTargetSpecification<[BindToGenericTargetParameter] T> : Mixin<object, MixinWithExplicitTargetSpecification<T>.I>
    {
      public interface I
      {
        string M (T p1);
      }

      // TODO 2745: Add specification here.
      [OverrideTarget]
      public string M (T p1)
      {
        return "TheMixin.M -> " + Next.M(p1);
      }
    }

    public class MixinWithoutTargetSpecification<[BindToGenericTargetParameter] T> : object
    {
      [OverrideTarget]
      public string M (T p1)
      {
        return "TheMixin.M";
      }
    }
  }
}
