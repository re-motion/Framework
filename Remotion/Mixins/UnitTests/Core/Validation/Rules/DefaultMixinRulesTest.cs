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
  public class DefaultMixinRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfMixinAppliedToItself ()
    {
      TargetClassDefinition bc = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (object), typeof (object));
      var log = Validator.Validate (bc);
      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinCannotMixItself", log), Is.True);
    }

    [Test]
    public void FailsIfMixinAppliedToItsBase ()
    {
      TargetClassDefinition bc = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (object), typeof (NullMixin));
      var log = Validator.Validate (bc);
      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinCannotMixItsBase", log), Is.True);
    }

    [Test]
    public void FailsIfMixinIsInterface ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (IBT1Mixin1));
      var log = Validator.Validate (definition.Mixins[typeof (IBT1Mixin1)]);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinCannotBeInterface", log), Is.True);
    }

    [Test]
    public void FailsIfMixinNonPublic ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType5), typeof (BT5Mixin2));
      var log = Validator.Validate (definition.Mixins[typeof (BT5Mixin2)]);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log), Is.True);
    }

    [Test]
    public void SucceedsIfNestedPublicMixin ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (PublicNester.PublicNested));
      var log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsIfNestedPublicMixinInNonPublic ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (InternalNester.PublicNested));
      var log = Validator.Validate (definition);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log), Is.True);
    }

    [Test]
    public void FailsIfNestedPrivateMixin ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (PublicNester.InternalNested));
      var log = Validator.Validate (definition);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log), Is.True);
    }

    [Test]
    public void FailsIfNestedPrivateMixinInNonPublic ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (InternalNester.InternalNested));
      var log = Validator.Validate (definition);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinMustBePublic", log), Is.True);
    }

    [Test]
    public void FailsIfNoPublicOrProtectedDefaultCtorInMixinClassWithOverriddenMembers ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod),
          typeof (MixinWithPrivateCtorAndVirtualMethod));
      var log = Validator.Validate (definition);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinWithOverriddenMembersMustHavePublicOrProtectedDefaultCtor",
                               log), Is.True);
    }

    [Test]
    public void FailsIfNoMixinBaseWith ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (ClassWithVirtualMethod),
          typeof (MixinWithProtectedOverriderWithoutMixinBase));
      var log = Validator.Validate (definition);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinRules.MixinNeedingDerivedTypeMustBeDerivedFromMixinBase",
                               log), Is.True);
    }

    [Test]
    public void SucceedsIfNoPublicOrProtectedDefaultCtorInMixinClassWithoutOverriddenMembers ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (NullTarget),
          typeof (MixinWithPrivateCtorAndVirtualMethod));
      var log = Validator.Validate (definition);

      AssertSuccess (log);
    }

  }
}
