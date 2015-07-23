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
  public class DefaultMixinRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinCannotBeInterface));
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinMustBePublic));
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinWithOverriddenMembersMustHavePublicOrProtectedDefaultCtor));
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinCannotMixItself));
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinCannotMixItsBase));
      visitor.MixinRules.Add (new DelegateValidationRule<MixinDefinition> (MixinNeedingDerivedTypeMustBeDerivedFromMixinBase));
    }

    [DelegateRuleDescription (Message = "An interface is configured as a mixin, but mixins must be classes or value types.")]
    private void MixinCannotBeInterface (DelegateValidationRule<MixinDefinition>.Args args)
    {
      SingleMust (!args.Definition.Type.IsInterface, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "A mixin type does not have public visibility.")]
    private void MixinMustBePublic (DelegateValidationRule<MixinDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsVisible, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "A mixin whose members are overridden by the target class must have a public or protected default constructor.")]
    private void MixinWithOverriddenMembersMustHavePublicOrProtectedDefaultCtor (DelegateValidationRule<MixinDefinition>.Args args)
    {
      ConstructorInfo defaultCtor = args.Definition.Type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null, Type.EmptyTypes, null);
      SingleMust (!args.Definition.HasOverriddenMembers() || (defaultCtor != null && ReflectionUtility.IsPublicOrProtected (defaultCtor)),
          args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "A mixin is applied to itself.")]
    private void MixinCannotMixItself (DelegateValidationRule<MixinDefinition>.Args args)
    {
      SingleMust (args.Definition.Type != args.Definition.TargetClass.Type, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "A mixin is applied to one of its base types.")]
    private void MixinCannotMixItsBase (DelegateValidationRule<MixinDefinition>.Args args)
    {
      SingleMust (!args.Definition.TargetClass.Type.IsAssignableFrom (args.Definition.Type), args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "A mixin for which a concrete subtype must be generated is not derived from one of the generic Mixin classes.")]
    private void MixinNeedingDerivedTypeMustBeDerivedFromMixinBase (DelegateValidationRule<MixinDefinition>.Args args)
    {
      SingleMust (!args.Definition.NeedsDerivedMixinType() || MixinReflector.GetMixinBaseType (args.Definition.Type) != null, args.Log, args.Self);
    }
  }
}
