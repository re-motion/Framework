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
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.UnitTests.Core.Validation.Rules
{
  [TestFixture]
  public class DefaultMixinDependencyRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfClassMixinDependencyNotFulfilled ()
    {
      ClassContext context = new ClassContextBuilder (typeof (TargetClassWithAdditionalDependencies)).AddMixin<MixinWithAdditionalClassDependency> ().WithDependency<MixinWithNoAdditionalDependency> ().BuildClassContext ();

      TargetClassDefinition definition = TargetClassDefinitionFactory.CreateWithoutValidation (context);
      var log = Validator.Validate (definition.Mixins[typeof (MixinWithAdditionalClassDependency)]);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinDependencyRules.DependencyMustBeSatisfiedByAnotherMixin", log), Is.True);
    }

    [Test]
    public void FailsIfInterfaceMixinDependencyNotFulfilled ()
    {
      ClassContext context = new ClassContextBuilder (typeof (TargetClassWithAdditionalDependencies)).AddMixin<MixinWithAdditionalInterfaceDependency> ().WithDependency<IMixinWithAdditionalClassDependency> ().BuildClassContext ();

      TargetClassDefinition definition = TargetClassDefinitionFactory.CreateWithoutValidation (context);
      var log = Validator.Validate (definition.Mixins[typeof (MixinWithAdditionalInterfaceDependency)]);

      Assert.That (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMixinDependencyRules.DependencyMustBeSatisfiedByAnotherMixin", log), Is.True);
    }

  }
}
