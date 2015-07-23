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
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.UnitTests.Core.Validation.ValidationTestDomain;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.UnitTests.Core.Validation.Rules
{
  [TestFixture]
  public class DefaultMethodRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfOverriddenMethodNotVirtual ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      var log = Validator.Validate (definition.Methods[typeof (BaseType4).GetMethod ("NonVirtualMethod")]);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log), Is.True);
    }

    [Test]
    public void FailsIfOverriddenBaseMethodAbstract ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (AbstractBaseType), typeof (BT1Mixin1));
      var log = Validator.Validate (definition);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.AbstractTargetClassMethodMustNotBeOverridden", log), Is.True);
    }

    [Test]
    public void FailsIfOverriddenMethodFinal ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (ClassWithFinalMethod), typeof (MixinForFinalMethod));
      var log = Validator.Validate (definition);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustNotBeFinal", log), Is.True);
    }

    [Test]
    public void FailsIfOverriddenPropertyMethodNotVirtual ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      var log = Validator.Validate (definition.Properties[typeof (BaseType4).GetProperty ("NonVirtualProperty")]);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log), Is.True);
    }

    [Test]
    public void FailsIfOverriddenEventMethodNotVirtual ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      var log = Validator.Validate (definition.Events[typeof (BaseType4).GetEvent ("NonVirtualEvent")]);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log), Is.True);
    }

    [Test]
    public void FailsIfOverriddenMixinMethodNotVirtual ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod),
          typeof (MixinWithNonVirtualMethodToBeOverridden));
      var log = Validator.Validate (definition);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log), Is.True);
    }

    [Test]
    public void FailsIfAbstractMixinMethodHasNoOverride ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithAbstractMembers));
      var log = Validator.Validate (definition);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.AbstractMixinMethodMustBeOverridden", log), Is.True);
    }

    [Test]
    public void FailsIfCrossOverridesOnSameMethods ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod),
          typeof (MixinOverridingSameClassMethod));
      var log = Validator.Validate (definition);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.NoCircularOverrides", log), Is.True);
    }

    [Test]
    public void SucceedsIfCrossOverridesNotOnSameMethods ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod),
          typeof (MixinOverridingClassMethod));
      var log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsIfMixinMethodIsOverriddenWhichHasNoThisProperty ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod), typeof (AbstractMixinWithoutBase));
      var log = Validator.Validate (definition);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverridingMixinMethodsOnlyPossibleWhenMixinDerivedFromMixinBase", log), Is.True);
    }

    [Test]
    public void SucceedsIfOverridingMembersAreProtected ()
    {
      TargetClassDefinition definition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithProtectedOverrider));
      Assert.That (definition.Mixins[0].HasProtectedOverriders (), Is.True);
      var log = Validator.Validate (definition);

      AssertSuccess (log);
    }
  }
}
