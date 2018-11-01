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
using System.Linq;
using System.Reflection;
using Remotion.Mixins.Definitions;
using Remotion.TypePipe.MutableReflection.MemberSignatures;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultMethodIntroductionRules : RuleSetBase
  {
    private readonly ContextStoreMemberLookupUtility<MethodDefinition> _memberLookupUtility = new ContextStoreMemberLookupUtility<MethodDefinition> ();
    private readonly ContextStoreMemberIntroductionLookupUtility<MethodIntroductionDefinition> _introductionLookupUtility =
        new ContextStoreMemberIntroductionLookupUtility<MethodIntroductionDefinition> ();

    private readonly MemberSignatureEqualityComparer _signatureComparer = new MemberSignatureEqualityComparer ();

    public override void Install (ValidatingVisitor visitor)
    {
      visitor.MethodIntroductionRules.Add (new DelegateValidationRule<MethodIntroductionDefinition> (PublicMethodNameMustBeUniqueInTargetClass));
      visitor.MethodIntroductionRules.Add (new DelegateValidationRule<MethodIntroductionDefinition> (PublicMethodNameMustBeUniqueInOtherMixins));
    }

    [DelegateRuleDescription (Message = "A method introduced by a mixin cannot be public if the target class already has a method of the same name.")]
    private void PublicMethodNameMustBeUniqueInTargetClass (DelegateValidationRule<MethodIntroductionDefinition>.Args args)
    {
      if (args.Definition.Visibility == MemberVisibility.Public)
      {
        MethodInfo introducedMethod = args.Definition.InterfaceMember;
        
        var targetMethodsWithSameNameAndSignature = from candidate in _memberLookupUtility.GetCachedMembersByName (
                                                        args.Log.ContextStore, 
                                                        args.Definition.DeclaringInterface.TargetClass, 
                                                        introducedMethod.Name)
                                                    where _signatureComparer.Equals (candidate.MethodInfo, introducedMethod)
                                                    select candidate;
        if (targetMethodsWithSameNameAndSignature.Any())
        {
          args.Log.Fail (args.Self);
          return;
        }
      }

      args.Log.Succeed (args.Self);
    }

    [DelegateRuleDescription (Message = "A method introduced by a mixin cannot be public if another mixin also introduces a public method of the same name.")]
    private void PublicMethodNameMustBeUniqueInOtherMixins (DelegateValidationRule<MethodIntroductionDefinition>.Args args)
    {
      if (args.Definition.Visibility == MemberVisibility.Public)
      {
        MethodInfo introducedMethod = args.Definition.InterfaceMember;
        var otherIntroductionsWithSameNameAndSignature = from candidate in _introductionLookupUtility.GetCachedPublicIntroductionsByName (
                                                            args.Log.ContextStore,
                                                            args.Definition.DeclaringInterface.TargetClass,
                                                            introducedMethod.Name)
                                                         where candidate != args.Definition
                                                            && _signatureComparer.Equals (candidate.InterfaceMember, introducedMethod)
                                                         select candidate;

        if (otherIntroductionsWithSameNameAndSignature.Any ())
        {
          args.Log.Fail (args.Self);
          return;
        }
      }

      args.Log.Succeed (args.Self);
    }
  }
}
