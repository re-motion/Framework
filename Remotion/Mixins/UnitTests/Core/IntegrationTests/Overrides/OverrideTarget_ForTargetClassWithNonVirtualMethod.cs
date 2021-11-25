using System;
using NUnit.Framework;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.UnitTests.Core.IntegrationTests.Overrides
{
  [TestFixture]
  public class OverrideTarget_ForTargetClassWithNonVirtualMethod
  {
    [Test]
    [TestCase (typeof (MixinWithImplicitTargetSpecification))]
    [TestCase (typeof (MixinWithoutTargetSpecification))]
    [TestCase (typeof (MixinWithExplicitTargetSpecification), IgnoreReason = "RM-2745")]
    public void InstantiateTargetTypeWithNonVirtualMethod_ShouldThrowValidationException (Type mixinType)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        Assert.That(
            () => ObjectFactory.Create<C>(),
            Throws.TypeOf<ValidationException>().With.Message.EqualTo(
                "Some parts of the mixin configuration could not be validated.\r\n"
                + "MethodDefinition 'Remotion.Mixins.UnitTests.Core.IntegrationTests.Overrides.OverrideTarget_ForTargetClassWithNonVirtualMethod+C.M', 6 rules executed\r\n"
                + "Context: Remotion.Mixins.UnitTests.Core.IntegrationTests.Overrides.OverrideTarget_ForTargetClassWithNonVirtualMethod+C\r\n"
                + "  failures - 1\r\n"
                + "    An overridden method is not declared virtual. (Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual)\r\n"));
      }
    }

    [Test]
    [TestCase (typeof (MixinWithImplicitTargetSpecification))]
    [TestCase (typeof (MixinWithoutTargetSpecification))]
    [TestCase (typeof (MixinWithExplicitTargetSpecification), IgnoreReason = "RM-2745")]
    public void InstantiateDerivedType_ShouldThrowValidationException (Type mixinType)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        Assert.That(
            () => ObjectFactory.Create<D>(),
            Throws.TypeOf<ValidationException>().With.Message.EqualTo(
                "Some parts of the mixin configuration could not be validated.\r\n"
                + "MethodDefinition 'Remotion.Mixins.UnitTests.Core.IntegrationTests.Overrides.OverrideTarget_ForTargetClassWithNonVirtualMethod+C.M', 6 rules executed\r\n"
                + "Context: Remotion.Mixins.UnitTests.Core.IntegrationTests.Overrides.OverrideTarget_ForTargetClassWithNonVirtualMethod+D\r\n"
                + "  failures - 1\r\n"
                + "    An overridden method is not declared virtual. (Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual)\r\n"));
      }
    }

    [Test]
    [Ignore ("RM-2745")]
    [TestCase (typeof (MixinWithImplicitTargetSpecification))]
    [TestCase (typeof (MixinWithoutTargetSpecification))]
    [TestCase (typeof (MixinWithExplicitTargetSpecification))]
    public void InstantiateShadowOfTargetTypeWithNonVirtualMethod_ShouldThrowValidationException (Type mixinType)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        Assert.That(
            () => ObjectFactory.Create<C_Shadow>(),
            Throws.TypeOf<ValidationException>().With.Message.EqualTo(
                "Some parts of the mixin configuration could not be validated.\r\n"
                + "MethodDefinition 'Remotion.Mixins.UnitTests.Core.IntegrationTests.Overrides.OverrideTarget_ForTargetClassWithNonVirtualMethod+C.M', 6 rules executed\r\n"
                + "Context: Remotion.Mixins.UnitTests.Core.IntegrationTests.Overrides.OverrideTarget_ForTargetClassWithNonVirtualMethod+C\r\n"
                + "  failures - 1\r\n"
                + "    An overridden method is not declared virtual. (Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual)\r\n"));
      }
    }

    [Test]
    [Ignore ("RM-2745")]
    [TestCase (typeof (MixinWithImplicitTargetSpecification))]
    [TestCase (typeof (MixinWithoutTargetSpecification))]
    [TestCase (typeof (MixinWithExplicitTargetSpecification))]
    public void InstantiateShadowOfDerivedType_ShouldThrowValidationException (Type mixinType)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        Assert.That(
            () => ObjectFactory.Create<D_Shadow>(),
            Throws.TypeOf<ValidationException>().With.Message.EqualTo(
                "Some parts of the mixin configuration could not be validated.\r\n"
                + "MethodDefinition 'Remotion.Mixins.UnitTests.Core.IntegrationTests.Overrides.OverrideTarget_ForTargetClassWithNonVirtualMethod+C.M', 6 rules executed\r\n"
                + "Context: Remotion.Mixins.UnitTests.Core.IntegrationTests.Overrides.OverrideTarget_ForTargetClassWithNonVirtualMethod+D\r\n"
                + "  failures - 1\r\n"
                + "    An overridden method is not declared virtual. (Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual)\r\n"));
      }
    }


    public class C
    {
      public string M ()
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

    public class D : C
    {
      // No Override
    }

    public class D_Shadow : D
    {
      public new virtual string M ()
      {
        return "D_Shadow.M";
      }
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