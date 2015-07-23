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
  public class DefaultMethodIntroductionRulesTest : ValidationTestBase
  {
    [Test]
    public void SucceedsIfPrivateIntroducedMethod_HasSameNameAsTargetClassMethod ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithDefaultVisibility")];

      var log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedMethod_HasSameNameButDifferentSignatureFromTargetClassMethod ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesDifferentSignaturesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      var log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void FailsIfPublicIntroducedMethod_HasSameNameAndSignatureAsTargetClassMethod ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      var log = Validator.Validate (definition);
      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodIntroductionRules.PublicMethodNameMustBeUniqueInTargetClass", log), Is.True);
    }

    [Test]
    public void SucceedsIfPrivateIntroducedMethod_HasSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithDefaultVisibility")];

      var log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedMethod_DoesNotHaveSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      var log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedMethod_HasSameNameAsPrivateOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (MixinIntroducingMembersWithPrivateVisibilities));
      MethodIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      var log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedMethod_HasSameNameButDifferentSignatureAsOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithPublicVisibilityDifferentSignatures));
      MethodIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      var log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void FailsIfPublicIntroducedMethod_HasSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithDifferentVisibilities));
      MethodIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")];

      var log = Validator.Validate (definition);
      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodIntroductionRules.PublicMethodNameMustBeUniqueInOtherMixins", log), Is.True);
    }
  }
}
