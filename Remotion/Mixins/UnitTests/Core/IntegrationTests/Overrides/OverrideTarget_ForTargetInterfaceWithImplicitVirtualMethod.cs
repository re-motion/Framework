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
  public class OverrideTarget_ForTargetInterfaceWithImplicitVirtualMethod
  {
    [Test]
    [TestCase (typeof (MixinWithImplicitTargetSpecification), "TheMixin.M -> C.M")]
    [TestCase (typeof (MixinWithExplicitTargetSpecification), "TheMixin.M -> C.M", IgnoreReason = "RM-2745")]
    public void InstantiateTargetType_ShouldOverrideTargetMethodFromTargetType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<C>();
        Assert.That(instance.M(), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [TestCase (typeof (MixinWithImplicitTargetSpecification), "TheMixin.M -> C.M")]
    [TestCase (typeof (MixinWithExplicitTargetSpecification), "TheMixin.M -> C.M", IgnoreReason = "RM-2745")]
    public void InstantiateDerivedTypeWithoutOverride_ShouldOverrideTargetMethodFromTargetType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<D1>();
        Assert.That(instance.M(), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [TestCase (typeof (MixinWithImplicitTargetSpecification), "TheMixin.M -> D2.M")]
    [TestCase (typeof (MixinWithExplicitTargetSpecification), "TheMixin.M -> D2.M", IgnoreReason = "RM-2745")]
    public void InstantiateDerivedType_ShouldOverrideTargetMethodFromDerivedType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<D2>();
        Assert.That(instance.M(), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [TestCase (typeof (MixinWithImplicitTargetSpecification), "TheMixin.M -> D2.M")]
    [TestCase (typeof (MixinWithExplicitTargetSpecification), "TheMixin.M -> D2.M", IgnoreReason = "RM-2745")]
    public void InstantiateDerivedDerivedTypeWithoutOverride_ShouldOverrideTargetMethodFromDerivedType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<E>();
        Assert.That(instance.M(), Is.EqualTo(expectedMethodOutput));
      }
    }


    [Test]
    [Ignore ("RM-2745")]
    [TestCase (typeof (MixinWithImplicitTargetSpecification), "TheMixin.M -> C.M")]
    [TestCase (typeof (MixinWithExplicitTargetSpecification), "TheMixin.M -> C.M")]
    public void InstantiateShadowOfTargetType_ShouldOverrideTargetMethodFromTargetType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<C_Shadow>();
        Assert.That(instance.M(), Is.EqualTo("C_Shadow.M"));
        Assert.That(((C) instance).M(), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [Ignore ("RM-2745")]
    [TestCase (typeof (MixinWithImplicitTargetSpecification), "TheMixin.M -> C.M")]
    [TestCase (typeof (MixinWithExplicitTargetSpecification), "TheMixin.M -> C.M")]
    public void InstantiateShadowOfDerivedTypeWithoutOverride_ShouldOverrideTargetMethodFromTargetType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<D1_Shadow>();
        Assert.That(instance.M(), Is.EqualTo("D1_Shadow.M"));
        Assert.That(((C) instance).M(), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [Ignore ("RM-2745")]
    [TestCase (typeof (MixinWithImplicitTargetSpecification), "TheMixin.M -> D2.M")]
    [TestCase (typeof (MixinWithExplicitTargetSpecification), "TheMixin.M -> D2.M")]
    public void InstantiateShadowOfDerivedTypeWithOverride_ShouldOverrideTargetMethodFromDerivedType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<D2_Shadow>();
        Assert.That(instance.M(), Is.EqualTo("D2_Shadow.M"));
        Assert.That(((C) instance).M(), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [Ignore ("RM-2745")]
    [TestCase (typeof (MixinWithImplicitTargetSpecification), "TheMixin.M -> D2.M")]
    [TestCase (typeof (MixinWithExplicitTargetSpecification), "TheMixin.M -> D2.M", IgnoreReason = "RM-2745")]
    public void InstantiateShadowOfDerivedTypeWithoutOverrideOfDerivedTypeWithOverride_ShouldOverrideTargetMethodFromDerivedType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<E_Shadow>();
        Assert.That(instance.M(), Is.EqualTo("E_Shadow.M"));
        Assert.That(((C) instance).M(), Is.EqualTo(expectedMethodOutput));
      }
    }

    public interface IC
    {
      string M ();
    }

    public class C : IC
    {
      public virtual string M ()
      {
        return "C.M";
      }
    }

    public class C_Shadow : C
    {
      public new virtual string M ()
      {
        return "C_Shadow.M";
      }
    }

    public class D1 : C
    {
      // No Override
    }

    public class D1_Shadow : D1
    {
      public new virtual string M ()
      {
        return "D1_Shadow.M";
      }
    }

    public class D2 : C
    {
      public override string M ()
      {
        return "D2.M";
      }
    }

    public class D2_Shadow : D2
    {
      public new virtual string M ()
      {
        return "D2_Shadow.M";
      }
    }

    public class E : D2
    {
      // No Override
    }

    public class E_Shadow : E
    {
      public new virtual string M ()
      {
        return "E_Shadow.M";
      }
    }

    public class MixinWithImplicitTargetSpecification : Mixin<IC, MixinWithImplicitTargetSpecification.INext>
    {
      public interface INext
      {
        string M ();
      }

      [OverrideTarget]
      public string M ()
      {
        return "TheMixin.M -> " + Next.M();
      }
    }

    public class MixinWithExplicitTargetSpecification : Mixin<object, MixinWithExplicitTargetSpecification.INext>
    {
      public interface INext
      {
        string M ();
      }

      // TODO 2745: Add specification here.
      [OverrideTarget]
      public string M ()
      {
        return "TheMixin.M -> " + Next.M();
      }
    }
  }
}