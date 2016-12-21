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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultPropertyIntroductionRules: RuleSetBase
  {
    private readonly ContextStoreMemberLookupUtility<PropertyDefinition> _memberLookupUtility = new ContextStoreMemberLookupUtility<PropertyDefinition> ();
    private readonly ContextStoreMemberIntroductionLookupUtility<PropertyIntroductionDefinition> _introductionLookupUtility =
        new ContextStoreMemberIntroductionLookupUtility<PropertyIntroductionDefinition> ();

    public override void Install (ValidatingVisitor visitor)
    {
      visitor.PropertyIntroductionRules.Add (new DelegateValidationRule<PropertyIntroductionDefinition> (PublicPropertyNameMustBeUniqueInTargetClass));
      visitor.PropertyIntroductionRules.Add (new DelegateValidationRule<PropertyIntroductionDefinition> (PublicPropertyNameMustBeUniqueInOtherMixins));
    }

    [DelegateRuleDescription (Message = "A property introduced by a mixin cannot be public if the target class already has a property of the same name.")]
    private void PublicPropertyNameMustBeUniqueInTargetClass (DelegateValidationRule<PropertyIntroductionDefinition>.Args args)
    {
      if (args.Definition.Visibility == MemberVisibility.Public)
      {
        PropertyInfo introducedMember = args.Definition.InterfaceMember;
        if (_memberLookupUtility.GetCachedMembersByName (
                args.Log.ContextStore, args.Definition.DeclaringInterface.TargetClass, introducedMember.Name).FirstOrDefault () != null)
        {
          args.Log.Fail (args.Self);
          return;
        }
      }
      args.Log.Succeed (args.Self);
    }

    [DelegateRuleDescription (Message = "A property introduced by a mixin cannot be public if another mixin also introduces a public property of the same name.")]
    private void PublicPropertyNameMustBeUniqueInOtherMixins (DelegateValidationRule<PropertyIntroductionDefinition>.Args args)
    {
      if (args.Definition.Visibility == MemberVisibility.Public)
      {
        PropertyInfo introducedProperty = args.Definition.InterfaceMember;
        IEnumerable<PropertyIntroductionDefinition> otherIntroductionsWithSameName =
            _introductionLookupUtility.GetCachedPublicIntroductionsByName (
            args.Log.ContextStore, args.Definition.DeclaringInterface.TargetClass, introducedProperty.Name);

        foreach (PropertyIntroductionDefinition property in otherIntroductionsWithSameName)
        {
          if (property != args.Definition)
          {
            args.Log.Fail (args.Self);
            return;
          }
        }
        args.Log.Succeed (args.Self);
      }
    }
  }
}
