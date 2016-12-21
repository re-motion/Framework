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
  public class DefaultNextCallDependencyRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfEmptyNextCallDependencyNotFulfilled ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (MixinWithUnsatisfiedEmptyNextCallDependency));
      var log = Validator.Validate (
          definition.Mixins[typeof (MixinWithUnsatisfiedEmptyNextCallDependency)].NextCallDependencies[typeof (IEmptyInterface)]);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultNextCallDependencyRules.DependencyMustBeSatisfied", log), Is.True);
    }

    [Test]
    public void SucceedsIfDuckNextCallDependency ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (ClassFulfillingAllMemberRequirementsDuck),
          typeof (MixinRequiringAllMembersNextCall));
      var log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfAggregateNextCallDependencyIsFullyImplemented ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7Base));
      var log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsIfEmptyAggregateNextCallDependencyIsNotAvailable ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (NullTarget), typeof (MixinWithUnsatisfiedEmptyAggregateNextCallDependency));
      var log = Validator.Validate (definition);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultNextCallDependencyRules.DependencyMustBeSatisfied", log), Is.True);
    }
  }
}
