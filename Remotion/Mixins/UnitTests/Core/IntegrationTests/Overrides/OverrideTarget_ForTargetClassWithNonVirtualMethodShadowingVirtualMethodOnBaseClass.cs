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
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.UnitTests.Core.IntegrationTests.Overrides
{
  [TestFixture]
  [Ignore ("RM-2745")]
  public class OverrideTarget_ForTargetClassWithNonVirtualMethodShadowingVirtualMethodOnBaseClass
  {
    [Test]
    [TestCase (typeof (MixinWithImplicitTargetSpecification), "TheMixin.M -> C.M")]
    [TestCase (typeof (MixinWithoutTargetSpecification), "TheMixin.M")]
    [TestCase (typeof (MixinWithExplicitTargetSpecification), "TheMixin.M -> C.M")]
    public void InstantiateTargetType_ShouldThrowValidationException (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin (mixinType).EnterScope())
      {
        Assert.That (
            () => ObjectFactory.Create<C>(),
            Throws.TypeOf<ValidationException>().With.Message.EqualTo (
                "Some parts of the mixin configuration could not be validated.\r\n"
                + "MethodDefinition 'Remotion.Mixins.UnitTests.Core.IntegrationTests.Overrides.OverrideTarget_ForTargetClassWithNonVirtualMethodShadowingVirtualMethodOnBaseClass+C.M', 6 rules executed\r\n"
                + "Context: Remotion.Mixins.UnitTests.Core.IntegrationTests.Overrides.OverrideTarget_ForTargetClassWithNonVirtualMethodShadowingVirtualMethodOnBaseClass+D\r\n"
                + "  failures - 1\r\n"
                + "    An overridden method is not declared virtual. (Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual)\r\n"));
      }
    }

    [Test]
    [TestCase (typeof (MixinWithImplicitTargetSpecification), "TheMixin.M -> C.M")]
    [TestCase (typeof (MixinWithoutTargetSpecification), "TheMixin.M")]
    [TestCase (typeof (MixinWithExplicitTargetSpecification), "TheMixin.M -> C.M")]
    public void InstantiateDerivedTypeWithoutOverride_ShouldThrowValidationException (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin (mixinType).EnterScope())
      {
        Assert.That (
          () => ObjectFactory.Create<D>(),
          Throws.TypeOf<ValidationException>().With.Message.EqualTo (
              "Some parts of the mixin configuration could not be validated.\r\n"
              + "MethodDefinition 'Remotion.Mixins.UnitTests.Core.IntegrationTests.Overrides.OverrideTarget_ForTargetClassWithNonVirtualMethodShadowingVirtualMethodOnBaseClass+C.M', 6 rules executed\r\n"
              + "Context: Remotion.Mixins.UnitTests.Core.IntegrationTests.Overrides.OverrideTarget_ForTargetClassWithNonVirtualMethodShadowingVirtualMethodOnBaseClass+D\r\n"
              + "  failures - 1\r\n"
              + "    An overridden method is not declared virtual. (Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual)\r\n"));
      }
    }


    public class Shadowed_C
    {
      public virtual string M ()
      {
        return "Shadowed_C.M";
      }
    }

    public class C : Shadowed_C
    {
      public new string M ()
      {
        return "C.M";
      }
    }

    public class D : C
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
        return "TheMixin.M -> " + Next.M ();
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
        return "TheMixin.M -> " + Next.M ();
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