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
  public class DefaultTargetCallDependencyRulesTest : ValidationTestBase
  {
    [Test]
    public void SucceedsIfEmptyTargetCallDependencyNotFulfilled ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (MixinWithUnsatisfiedEmptyTargetCallDependency));
      var log = Validator.Validate (
          definition.Mixins[typeof (MixinWithUnsatisfiedEmptyTargetCallDependency)].TargetCallDependencies[typeof (IEmptyInterface)]);

      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfCircularTargetCallDependency ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (MixinWithCircularTargetCallDependency1), typeof (MixinWithCircularTargetCallDependency2));
      var log = Validator.Validate (
          definition.Mixins[typeof (MixinWithCircularTargetCallDependency1)]);

      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfDuckTargetCallDependency ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (ClassFulfillingAllMemberRequirementsDuck),
          typeof (MixinRequiringAllMembersTargetCall));
      var log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void SucceedsIfAggregateTargetCallDependencyIsFullyImplemented ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (Bt3Mixin7TargetCall));
      var log = Validator.Validate (definition);

      AssertSuccess (log);
    }


    [Test]
    public void SucceedsIfEmptyAggregateTargetCallDependencyIsNotAvailable ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (NullTarget), typeof (MixinWithUnsatisfiedEmptyAggregateTargetCallDependency));
      var log = Validator.Validate (definition);

      AssertSuccess (log);
    }


  }
}
