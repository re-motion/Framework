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
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection.MemberSignatures;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  public class OverridesAnalyzer<TMember>
      where TMember : MemberDefinitionBase
  {
    private static readonly MemberSignatureEqualityComparer s_signatureComparer = new MemberSignatureEqualityComparer ();

    private readonly Type _attributeType;
    private readonly IEnumerable<TMember> _baseMembers;

    private MultiDictionary<string, TMember> _baseMembersByNameCache = null;

    public OverridesAnalyzer (Type attributeType, IEnumerable<TMember> baseMembers)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("attributeType", attributeType, typeof (IOverrideAttribute));
      ArgumentUtility.CheckNotNull ("baseMembers", baseMembers);

      _attributeType = attributeType;
      _baseMembers = baseMembers;
    }

    public IEnumerable<MemberOverridePair<TMember>> Analyze (IEnumerable<TMember> overriderMembers)
    {
      ArgumentUtility.CheckNotNull ("overriderMembers", overriderMembers);

      foreach (TMember member in overriderMembers)
      {
        var overrideAttribute = (IOverrideAttribute) AttributeUtility.GetCustomAttribute (member.MemberInfo, _attributeType, true);
        if (overrideAttribute != null)
        {
          TMember baseMember = FindOverriddenMember (overrideAttribute, member);

          if (baseMember == null)
          {
            string message = string.Format (
                "The member overridden by '{0}' declared by type '{1}' could not be found. Candidates: {2}.",
                member.MemberInfo,
                member.DeclaringClass.FullName,
                BuildCandidateStringForExceptionMessage (BaseMembersByName[member.Name]));
            throw new ConfigurationException (message);
          }
          yield return new MemberOverridePair<TMember> (baseMember, member);
        }
      }
    }

    private MultiDictionary<string, TMember> BaseMembersByName
    {
      get
      {
        EnsureMembersCached();
        return _baseMembersByNameCache;
      }
    }

    private void EnsureMembersCached ()
    {
      if (_baseMembersByNameCache == null)
      {
        _baseMembersByNameCache = new MultiDictionary<string, TMember> ();
        foreach (TMember member in _baseMembers)
          _baseMembersByNameCache.Add (member.Name, member);
      }
    }

    private TMember FindOverriddenMember (IOverrideAttribute attribute, TMember overrider)
    {
      var candidates = from candidate in BaseMembersByName[overrider.Name]
                       let candidateType = candidate.DeclaringClass.Type
                       where OverriddenMemberTypeMatches (candidateType, attribute.OverriddenType)
                       where s_signatureComparer.Equals (candidate.MemberInfo, overrider.MemberInfo)
                       select candidate;

      try
      {
        return candidates.SingleOrDefault ();
      }
      catch (InvalidOperationException)
      {
        string message = string.Format (
              "Ambiguous override: Member '{0}' declared by type '{1}' could override any of the following: {2}.",
              overrider.MemberInfo,
              overrider.DeclaringClass.FullName,
              BuildCandidateStringForExceptionMessage (candidates));
        throw new ConfigurationException (message);
      }
    }

    private string BuildCandidateStringForExceptionMessage (IEnumerable<TMember> candidates)
    {
      var candidatesByType = candidates.ToLookup (md => md.DeclaringClass);
      return string.Join ("; ", candidatesByType.Select (group => string.Join (", ", @group.Select (md => "'" + md.MemberInfo.ToString () + "'")) + " (on '" + @group.Key.FullName + "')"));
    }

    private bool OverriddenMemberTypeMatches (Type overriddenMemberType, Type requiredType)
    {
      if (requiredType == null) // no type required
        return true;

      if (requiredType.IsAssignableFrom (overriddenMemberType)) // same type or base type required
        return true;

      if (requiredType.IsGenericTypeDefinition 
          && overriddenMemberType.IsGenericType 
          && overriddenMemberType.GetGenericTypeDefinition() == requiredType)
      {
        return true;
      }

      return false;
    }
  }
}
