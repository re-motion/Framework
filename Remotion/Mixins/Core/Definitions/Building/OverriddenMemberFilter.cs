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
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;
using ReflectionUtility = Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.Mixins.Definitions.Building
{
  /// <summary>
  /// Takes a list of <see cref="MemberInfo"/> objects and removed those members that are overridden within that list, so that only the most derived
  /// members are returned. For members with more than one accessor (e.g. properties with a get and a set accessor), both overriding and overridden
  /// member are returned unless all accessors are overridden. This also applies in a chain of overrides - all members are returned that contain one
  /// accessor that is not overridden.
  /// </summary>
  public class OverriddenMemberFilter
  {
    public T[] RemoveOverriddenMembers<T> (IEnumerable<T> members) where T : MemberInfo
    {
      ArgumentUtility.CheckNotNull ("members", members);

      // maps the associated methods' base definitions to the most derived member in the list; we adjust this dictionary as we walk the members
      var baseDefinitionsToMostDerivedMembers = new Dictionary<MethodInfo, T> ();

      foreach (var member in members)
      {
        var associatedMethodsForMember = ReflectionUtility.GetAssociatedMethods (member);
        foreach (var associatedMethodForMember in associatedMethodsForMember)
        {
          // check whether we already have a member for that base definition; if yes, check which one is more derived
          var baseDefinition = MethodBaseDefinitionCache.GetBaseDefinition (associatedMethodForMember);
          T existingMember;
          if (!baseDefinitionsToMostDerivedMembers.TryGetValue (baseDefinition, out existingMember) // we have no member for the base definition...
              || existingMember.DeclaringType.IsAssignableFrom (member.DeclaringType)) // the current one is more derived...
          {
            baseDefinitionsToMostDerivedMembers[baseDefinition] = member; // ...so store the current member
          }
        }
      }

      return baseDefinitionsToMostDerivedMembers.Values.Distinct ().ToArray (); // Distinct required for members that have more than one accessor
    }
  }
}
