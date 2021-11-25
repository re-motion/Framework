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
  [Ignore ("RM-2745")]
  public class OverrideTarget_ForGenericTargetClassWithVirtualMethodShadowingVirtualMethodOnBaseClass
  {
    [Test]
    [TestCase (typeof (MixinWithImplicitTargetSpecification<>), "TheMixin.M -> C.M")]
    [TestCase (typeof (MixinWithoutTargetSpecification<>), "TheMixin.M")]
    [TestCase (typeof (MixinWithExplicitTargetSpecification<>), "TheMixin.M -> C.M")]
    public void InstantiateTargetType_ShouldOverrideTargetMethodFromTargetType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass(typeof (C<>)).AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<C<int>>();
        Assert.That(((Shadowed_C<int>) instance).M(0), Is.EqualTo("Shadowed_C.M"));
        Assert.That(instance.M(0), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [TestCase (typeof (MixinWithImplicitTargetSpecification<>), "TheMixin.M -> C.M")]
    [TestCase (typeof (MixinWithoutTargetSpecification<>), "TheMixin.M")]
    [TestCase (typeof (MixinWithExplicitTargetSpecification<>), "TheMixin.M -> C.M")]
    public void InstantiateDerivedTypeWithoutOverride_ShouldOverrideTargetMethodFromTargetType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass(typeof (C<>)).AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<D1<int>>();
        Assert.That(((Shadowed_C<int>) instance).M(0), Is.EqualTo("Shadowed_C.M"));
        Assert.That(instance.M(0), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [TestCase (typeof (MixinWithImplicitTargetSpecification<>), "TheMixin.M -> D2.M")]
    [TestCase (typeof (MixinWithoutTargetSpecification<>), "TheMixin.M")]
    [TestCase (typeof (MixinWithExplicitTargetSpecification<>), "TheMixin.M -> D2.M")]
    public void InstantiateDerivedTypeWithOverride_ShouldOverrideTargetMethodFromDerivedType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass(typeof (C<>)).AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<D2<int>>();
        Assert.That(((Shadowed_C<int>) instance).M(0), Is.EqualTo("Shadowed_C.M"));
        Assert.That(instance.M(0), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [TestCase (typeof (MixinWithImplicitTargetSpecification<>), "TheMixin.M -> D2.M")]
    [TestCase (typeof (MixinWithoutTargetSpecification<>), "TheMixin.M")]
    [TestCase (typeof (MixinWithExplicitTargetSpecification<>), "TheMixin.M -> D2.M")]
    public void InstantiateDerivedTypeWithoutOverrideOfDerivedTypeWithOverride_ShouldOverrideTargetMethodFromDerivedType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass(typeof (C<>)).AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<E<int>>();
        Assert.That(((Shadowed_C<int>) instance).M(0), Is.EqualTo("Shadowed_C.M"));
        Assert.That(instance.M(0), Is.EqualTo(expectedMethodOutput));
      }
    }

    public class Shadowed_C<T>
    {
      public virtual string M (T p1)
      {
        return "Shadowed_C.M";
      }
    }

    public class C<T> : Shadowed_C<T>
    {
      public new virtual string M (T p1)
      {
        return "C.M";
      }
    }

    public class D1<T> : C<T>
    {
      // No Override
    }

    public class D2<T> : C<T>
    {
      public override string M (T p1)
      {
        return "D2.M";
      }
    }

    public class E<T> : D2<T>
    {
      // No Override
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