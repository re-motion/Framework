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
  public class OverrideTarget_ForTargetClassWithOverriddenMethodShadowingVirtualMethodOnBaseClass
  {
    [Test]
    [TestCase (typeof(MixinWithImplicitTargetSpecification), "TheMixin.M -> C.M")]
    [TestCase (typeof(MixinWithoutTargetSpecification), "TheMixin.M")]
    [TestCase (typeof(MixinWithExplicitTargetSpecification), "TheMixin.M -> C.M")]
    public void InstantiateTargetType_ShouldOverrideTargetMethodFromTargetType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<C>();
        Assert.That(((Shadowed_B) instance).M(), Is.EqualTo("Shadowed_B.M"));
        Assert.That(instance.M(), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [TestCase (typeof(MixinWithImplicitTargetSpecification), "TheMixin.M -> C.M")]
    [TestCase (typeof(MixinWithoutTargetSpecification), "TheMixin.M")]
    [TestCase (typeof(MixinWithExplicitTargetSpecification), "TheMixin.M -> C.M")]
    public void InstantiateDerivedTypeWithoutOverride_ShouldOverrideTargetMethodFromTargetType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<D1>();
        Assert.That(((Shadowed_B) instance).M(), Is.EqualTo("Shadowed_B.M"));
        Assert.That(instance.M(), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [TestCase (typeof(MixinWithImplicitTargetSpecification), "TheMixin.M -> D2.M")]
    [TestCase (typeof(MixinWithoutTargetSpecification), "TheMixin.M")]
    [TestCase (typeof(MixinWithExplicitTargetSpecification), "TheMixin.M -> D2.M")]
    public void InstantiateDerivedTypeWithOverride_ShouldOverrideTargetMethodFromDerivedType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<D2>();
        Assert.That(((Shadowed_B) instance).M(), Is.EqualTo("Shadowed_B.M"));
        Assert.That(instance.M(), Is.EqualTo(expectedMethodOutput));
      }
    }

    [Test]
    [TestCase (typeof(MixinWithImplicitTargetSpecification), "TheMixin.M -> D2.M")]
    [TestCase (typeof(MixinWithoutTargetSpecification), "TheMixin.M")]
    [TestCase (typeof(MixinWithExplicitTargetSpecification), "TheMixin.M -> D2.M")]
    public void InstantiateDerivedTypeWithoutOverrideOfDerivedTypeWithOverride_ShouldOverrideTargetMethodFromDerivedType (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        var instance = ObjectFactory.Create<E>();
        Assert.That(((Shadowed_B) instance).M(), Is.EqualTo("Shadowed_B.M"));
        Assert.That(instance.M(), Is.EqualTo(expectedMethodOutput));
      }
    }

    public class Shadowed_B
    {
      public virtual string M ()
      {
        return "Shadowed_B.M";
      }
    }

    public class B : Shadowed_B
    {
      public new virtual string M ()
      {
        return "B.M";
      }
    }

    public class C : B
    {
      public override string M ()
      {
        return "C.M";
      }
    }

    public class D1 : C
    {
      // No Override
    }

    public class D2 : C
    {
      public override string M ()
      {
        return "D2.M";
      }
    }

    public class E : D2
    {
      // No Override
    }

    public class MixinWithImplicitTargetSpecification : Mixin<C, MixinWithImplicitTargetSpecification.I>
    {
      public interface I
      {
        string M ();
      }

      [OverrideTarget]
      public string M ()
      {
        return "TheMixin.M -> " + Next.M();
      }
    }

    public class MixinWithExplicitTargetSpecification : Mixin<object, MixinWithExplicitTargetSpecification.I>
    {
      public interface I
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

    public class MixinWithoutTargetSpecification : object
    {
      [OverrideTarget]
      public string M ()
      {
        return "TheMixin.M";
      }
    }
  }
}
