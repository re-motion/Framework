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
using Remotion.Collections;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.Validation.Rules
{
  public class ContextStoreMemberIntroductionLookupUtility<TMemberIntroductionDefinition>
      where TMemberIntroductionDefinition : class, IMemberIntroductionDefinition
  {

    public IEnumerable<TMemberIntroductionDefinition> GetCachedPublicIntroductionsByName (IDataStore<object, object> contextStore, TargetClassDefinition targetClass, string name)
    {
      Tuple<string, TargetClassDefinition> cacheKey = Tuple.Create (
          typeof (ContextStoreMemberIntroductionLookupUtility<TMemberIntroductionDefinition>).FullName + ".GetCachedPublicIntroductionsByName",
          targetClass);

      MultiDictionary<string, TMemberIntroductionDefinition> introductionDefinitions = (MultiDictionary<string, TMemberIntroductionDefinition>)
          contextStore.GetOrCreateValue (cacheKey, delegate { return GetUncachedIntroductions (targetClass); });
      return introductionDefinitions[name];
    }

    private MultiDictionary<string, TMemberIntroductionDefinition> GetUncachedIntroductions (TargetClassDefinition targetClass)
    {
      MultiDictionary<string, TMemberIntroductionDefinition> introductionDefinitions = new MultiDictionary<string, TMemberIntroductionDefinition> ();
      foreach (InterfaceIntroductionDefinition interfaceIntroduction in targetClass.ReceivedInterfaces)
      {
        foreach (IMemberIntroductionDefinition memberIntroduction in interfaceIntroduction.GetIntroducedMembers())
        {
          TMemberIntroductionDefinition castMemberIntroduction = memberIntroduction as TMemberIntroductionDefinition;
          if (castMemberIntroduction != null && castMemberIntroduction.Visibility == MemberVisibility.Public)
            introductionDefinitions.Add (castMemberIntroduction.Name, castMemberIntroduction);
        }
      }
      return introductionDefinitions;
    }
  }
}
