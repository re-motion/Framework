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
  public class DefaultTargetClassRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfSealedTargetClass ()
    {
      TargetClassDefinition bc = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (DateTime));
      var log = Validator.Validate (bc);
      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustNotBeSealed", log), Is.True);
      Assert.That (log.GetNumberOfWarnings (), Is.EqualTo (0));
    }

    [Test]
    public void SucceedsIfAbstractTargetClass ()
    {
      TargetClassDefinition bc = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (MixinWithAbstractMembers));
      var log = Validator.Validate (bc);
      AssertSuccess (log);
    }

    [Test]
    public void FailsIfTargetClassDefinitionIsInterface ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (IBaseType2));
      var log = Validator.Validate (definition);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustNotBeAnInterface", log), Is.True);
    }

    [Test]
    public void FailsIfNoPublicOrProtectedCtorInTargetClass ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (ClassWithPrivateCtor),
          typeof (NullMixin));
      var log = Validator.Validate (definition);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustHavePublicOrProtectedCtor", log), Is.True);
    }

    [Test]
    public void FailsIfTargetClassIsNotPublic ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (InternalClass),
          typeof (NullMixin));
      var log = Validator.Validate (definition);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustBePublic", log), Is.True);
    }

    [Test]
    public void FailsIfNestedTargetClassIsNotPublic ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (PublicNester.InternalNested),
          typeof (NullMixin));
      var log = Validator.Validate (definition);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultTargetClassRules.TargetClassMustBePublic", log), Is.True);
    }

    [Test]
    public void SucceedsIfNestedTargetClassIsPublic ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (PublicNester.PublicNested),
          typeof (NullMixin));
      var log = Validator.Validate (definition);

      AssertSuccess (log);
    }
  }
}
