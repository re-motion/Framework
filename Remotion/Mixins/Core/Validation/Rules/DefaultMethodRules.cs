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
using Remotion.Utilities;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultMethodRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (OverriddenMethodMustBeVirtual));
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (OverriddenMethodMustNotBeFinal));
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (AbstractMixinMethodMustBeOverridden));
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (AbstractTargetClassMethodMustNotBeOverridden));
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (NoCircularOverrides));
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (OverridingMixinMethodsOnlyPossibleWhenMixinDerivedFromMixinBase));
      visitor.MethodRules.Add (new DelegateValidationRule<MethodDefinition> (OverridingMethodMustBePublicOrProtected));
    }

    [DelegateRuleDescription (Message = "An overridden method is not declared virtual.")]
    private void OverriddenMethodMustBeVirtual (DelegateValidationRule<MethodDefinition>.Args args)
    {
      SingleMust (args.Definition.Overrides.Count > 0 ? args.Definition.MethodInfo.IsVirtual : true, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "An overridden method is declared final or sealed.")]
    private void OverriddenMethodMustNotBeFinal (DelegateValidationRule<MethodDefinition>.Args args)
    {
      SingleMust (args.Definition.Overrides.Count > 0 ? !args.Definition.MethodInfo.IsFinal : true, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "An abstract target class method is overridden.")]
    private void AbstractTargetClassMethodMustNotBeOverridden (DelegateValidationRule<MethodDefinition>.Args args)
    {
      SingleMust (!(args.Definition.DeclaringClass is TargetClassDefinition) || !args.Definition.MethodInfo.IsAbstract
          || args.Definition.Overrides.Count == 0, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "An abstract mixin method is not overridden (implemented) by the target class.")]
    private void AbstractMixinMethodMustBeOverridden (DelegateValidationRule<MethodDefinition>.Args args)
    {
      SingleMust (!(args.Definition.DeclaringClass is MixinDefinition) || !args.Definition.MethodInfo.IsAbstract
          || args.Definition.Overrides.Count > 0, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "There is a cycle in overrides: method A overrides B which overrides A.")]
    private void NoCircularOverrides (DelegateValidationRule<MethodDefinition>.Args args)
    {
      MethodDefinition originalMethod = args.Definition;
      MethodDefinition method = args.Definition.Base;
      while (method != null && method != originalMethod)
        method = method.Base;
      SingleMust (method != originalMethod, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "A target class overrides a method from one of its mixins, but the mixin is not derived from one of the "
        + "Mixin<...> base classes.")]
    private void OverridingMixinMethodsOnlyPossibleWhenMixinDerivedFromMixinBase (DelegateValidationRule<MethodDefinition>.Args args)
    {
      SingleMust (!(args.Definition.DeclaringClass is MixinDefinition) || args.Definition.Overrides.Count == 0
          || MixinReflector.GetMixinBaseType (args.Definition.DeclaringClass.Type) != null, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "A method overriding a target class or mixin method is neither public nor protected.")]
    private void OverridingMethodMustBePublicOrProtected (DelegateValidationRule<MethodDefinition>.Args args)
    {
      Assertion.IsTrue (args.Definition.Base == null || args.Definition.MethodInfo.IsPublic || args.Definition.MethodInfo.IsFamily 
          || args.Definition.MethodInfo.IsFamilyOrAssembly, "Private and internal methods are ignored by the mixin engine");
    }
  }
}
