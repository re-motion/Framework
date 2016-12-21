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
  public class DefaultPropertyIntroductionRulesTest : ValidationTestBase
  {
    [Test]
    public void SucceedsIfPrivateIntroducedProperty_HasSameNameAsTargetClassProperty ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      PropertyIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithDefaultVisibility")];

      var log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void FailsIfPublicIntroducedProperty_HasSameNameButDifferentSignatureFromTargetClassProperty ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesDifferentSignaturesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      PropertyIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithPublicVisibility")];

      var log = Validator.Validate (definition);
      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultPropertyIntroductionRules.PublicPropertyNameMustBeUniqueInTargetClass", log), Is.True);
    }

    [Test]
    public void FailsIfPublicIntroducedProperty_HasSameNameAndSignatureAsTargetClassProperty ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (TargetClassWithSameNamesAsIntroducedMembers),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      PropertyIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithPublicVisibility")];

      var log = Validator.Validate (definition);
      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultPropertyIntroductionRules.PublicPropertyNameMustBeUniqueInTargetClass", log), Is.True);
    }

    [Test]
    public void SucceedsIfPrivateIntroducedProperty_HasSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithDifferentVisibilities));
      PropertyIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithDefaultVisibility")];

      var log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedProperty_DoesNotHaveSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities));
      PropertyIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithPublicVisibility")];

      var log = Validator.Validate (definition);
      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfPublicIntroducedProperty_HasSameNameAsPrivateOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (MixinIntroducingMembersWithPrivateVisibilities));
      PropertyIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithPublicVisibility")];

      var log = Validator.Validate (definition);
      AssertSuccess (log);
    }


    [Test]
    public void FailsIfPublicIntroducedProperty_HasSameNameButDifferentSignatureAsOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithPublicVisibilityDifferentSignatures));
      PropertyIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithPublicVisibility")];

      var log = Validator.Validate (definition);
      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultPropertyIntroductionRules.PublicPropertyNameMustBeUniqueInOtherMixins", log), Is.True);
    }

    [Test]
    public void FailsIfPublicIntroducedProperty_HasSameNameAsOther ()
    {
      TargetClassDefinition classDefinition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (NullTarget),
              typeof (MixinIntroducingMembersWithDifferentVisibilities),
              typeof (OtherMixinIntroducingMembersWithDifferentVisibilities));
      PropertyIntroductionDefinition definition = classDefinition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)].IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithPublicVisibility")];

      var log = Validator.Validate (definition);
      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultPropertyIntroductionRules.PublicPropertyNameMustBeUniqueInOtherMixins", log), Is.True);
    }
  }
}
