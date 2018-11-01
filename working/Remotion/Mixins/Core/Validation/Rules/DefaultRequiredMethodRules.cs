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
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultRequiredMethodRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.RequiredMethodRules.Add (new DelegateValidationRule<RequiredMethodDefinition> (RequiredNextCallMethodMustBePublicOrProtected));
    }

    [DelegateRuleDescription (Message = "One of the methods specified via the TNext type parameter of a mixin is not implemented as a public "
        + "or protected method.")]
    private void RequiredNextCallMethodMustBePublicOrProtected (DelegateValidationRule<RequiredMethodDefinition>.Args args)
    {
      SingleMust (!(args.Definition.DeclaringRequirement is RequiredNextCallTypeDefinition)
          || ReflectionUtility.IsPublicOrProtected (args.Definition.ImplementingMethod.MethodInfo), args.Log, args.Self);
    }
  }
}
