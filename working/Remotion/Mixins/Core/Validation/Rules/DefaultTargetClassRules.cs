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
using System.Reflection;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultTargetClassRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.TargetClassRules.Add (new DelegateValidationRule<TargetClassDefinition> (TargetClassMustNotBeSealed));
      visitor.TargetClassRules.Add (new DelegateValidationRule<TargetClassDefinition> (TargetClassMustNotBeAnInterface));
			visitor.TargetClassRules.Add (new DelegateValidationRule<TargetClassDefinition> (TargetClassMustHavePublicOrProtectedCtor));
    	visitor.TargetClassRules.Add (new DelegateValidationRule<TargetClassDefinition> (TargetClassMustBePublic));
    }

  	[DelegateRuleDescription (Message = "A target class for mixins is declared sealed (or it is a value type).")]
    private void TargetClassMustNotBeSealed (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
      SingleMust(!args.Definition.Type.IsSealed, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "An interface is used as a target class for mixins.")]
    private void TargetClassMustNotBeAnInterface (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
      SingleMust (!args.Definition.Type.IsInterface, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "A target class for mixins does not have a public or protected constructor.")]
    private void TargetClassMustHavePublicOrProtectedCtor (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
      ConstructorInfo[] ctors = args.Definition.Type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      ConstructorInfo[] publicOrProtectedCtors = Array.FindAll (ctors,
          delegate (ConstructorInfo ctor) { return ReflectionUtility.IsPublicOrProtected (ctor); });
      SingleMust (publicOrProtectedCtors.Length > 0, args.Log, args.Self);
    }

		[DelegateRuleDescription (Message = "A target class for mixins is not publicly visible.")]
		private void TargetClassMustBePublic (DelegateValidationRule<TargetClassDefinition>.Args args)
		{
			SingleMust (args.Definition.Type.IsVisible, args.Log, args.Self);
		
		}
  }
}
