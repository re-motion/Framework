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
  public class ContextStoreMemberLookupUtility<TMemberDefinition>
      where TMemberDefinition : MemberDefinitionBase
  {
    private Func<object, object> _contextStoreValueFactory;

    public IEnumerable<TMemberDefinition> GetCachedMembersByName (IDictionary<object, object> contextStore, TargetClassDefinition targetClass, string name)
    {
      Tuple<string, TargetClassDefinition> cacheKey =
          Tuple.Create (typeof (ContextStoreMemberLookupUtility<TMemberDefinition>).FullName + ".GetCachedMembersByName", targetClass);

      // Optimized for memory allocations
      if (_contextStoreValueFactory == null)
        _contextStoreValueFactory = key => GetUncachedMethodDefinitions (((Tuple<string, TargetClassDefinition>) key).Item2);

      var methodDefinitions = (MultiDictionary<string, TMemberDefinition>) contextStore.GetOrCreateValue (cacheKey, _contextStoreValueFactory);
      return methodDefinitions[name];
    }

    private MultiDictionary<string, TMemberDefinition> GetUncachedMethodDefinitions (TargetClassDefinition targetClass)
    {
      var memberDefinitions = new MultiDictionary<string, TMemberDefinition> ();
      foreach (MemberDefinitionBase memberDefinition in targetClass.GetAllMembers ())
      {
        var castMemberDefinition = memberDefinition as TMemberDefinition;
        if (castMemberDefinition != null)
          memberDefinitions.Add (memberDefinition.Name, castMemberDefinition);
      }
      return memberDefinitions;
    }
  }
}
