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
  public class DefaultEventIntroductionRules: RuleSetBase
  {
    private readonly ContextStoreMemberLookupUtility<EventDefinition> _memberLookupUtility = new ContextStoreMemberLookupUtility<EventDefinition> ();
    private readonly ContextStoreMemberIntroductionLookupUtility<EventIntroductionDefinition> _introductionLookupUtility =
        new ContextStoreMemberIntroductionLookupUtility<EventIntroductionDefinition> ();

    public override void Install (ValidatingVisitor visitor)
    {
      visitor.EventIntroductionRules.Add (new DelegateValidationRule<EventIntroductionDefinition> (PublicEventNameMustBeUniqueInTargetClass));
      visitor.EventIntroductionRules.Add (new DelegateValidationRule<EventIntroductionDefinition> (PublicEventNameMustBeUniqueInOtherMixins));
    }

    [DelegateRuleDescription (Message = "An event introduced by a mixin cannot be public if the target class already has an event of the same name.")]
    private void PublicEventNameMustBeUniqueInTargetClass (DelegateValidationRule<EventIntroductionDefinition>.Args args)
    {
      if (args.Definition.Visibility == MemberVisibility.Public)
      {
        EventInfo introducedMember = args.Definition.InterfaceMember;
        if (_memberLookupUtility.GetCachedMembersByName (
                args.Log.ContextStore, args.Definition.DeclaringInterface.TargetClass, introducedMember.Name).FirstOrDefault () != null)
        {
          args.Log.Fail (args.Self);
          return;
        }
      }
      args.Log.Succeed (args.Self);
    }

    [DelegateRuleDescription (Message = "An event introduced by a mixin cannot be public if another mixin also introduces a public event of the same name.")]
    private void PublicEventNameMustBeUniqueInOtherMixins (DelegateValidationRule<EventIntroductionDefinition>.Args args)
    {
      if (args.Definition.Visibility == MemberVisibility.Public)
      {
        EventInfo introducedEvent = args.Definition.InterfaceMember;
        IEnumerable<EventIntroductionDefinition> otherIntroductionsWithSameName =
            _introductionLookupUtility.GetCachedPublicIntroductionsByName (
            args.Log.ContextStore, args.Definition.DeclaringInterface.TargetClass, introducedEvent.Name);

        foreach (EventIntroductionDefinition eventIntroductionDefinition in otherIntroductionsWithSameName)
        {
          if (eventIntroductionDefinition != args.Definition)
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
