// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  [ImplementationFor (typeof (IAccessControlListFinder), Lifetime = LifetimeKind.Singleton)]
  public class AccessControlListFinder : IAccessControlListFinder
  {
    private readonly ISecurityContextRepository _securityContextRepository;

    public AccessControlListFinder (ISecurityContextRepository securityContextRepository)
    {
      ArgumentUtility.CheckNotNull ("securityContextRepository", securityContextRepository);
      
      _securityContextRepository = securityContextRepository;
    }

    /// <exception cref="AccessControlException">
    ///   The <see cref="SecurableClassDefinition"/> is not found.<br/>- or -<br/>
    ///   A matching <see cref="AccessControlList"/> is not found.<br/>- or -<br/>
    ///   <paramref name="context"/> is not state-less and a <see cref="StatePropertyDefinition"/> is missing.<br/>- or -<br/>
    ///   <paramref name="context"/> is not state-less and contains an invalid state for a <see cref="StatePropertyDefinition"/>.
    /// </exception>
    public IDomainObjectHandle<AccessControlList> Find (ISecurityContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      // Status quo:
      // Don't match ACL if Context contains more properties then the Class
      // Throw if Context misses a property for the specified Class
      // Match ACL if all States of the StateCombination match with the Context
      // if current Class does not contain a matching ACL, go to parent
      // -> once the BaseClass contains less properties then the Context a match is no longer possible
      // -> Inheritance happens on a by-state base

      for (var @class = GetClass (context.Class); @class != null; @class = GetClass (@class.BaseClass))
      {
        var foundAccessControlList = FindAccessControlList (@class, context);
        if (foundAccessControlList != null)
          return foundAccessControlList;

        var isInheritanceEnabled = @class.StatelessAccessControlList == null && !@class.StatefulAccessControlLists.Any();
        if (!isInheritanceEnabled)
          break;
      }

      return null;
    }

    private SecurableClassDefinitionData GetClass (string className)
    {
      if (className == null)
        return null;
      return _securityContextRepository.GetClass (className);
    }

    private IDomainObjectHandle<AccessControlList> FindAccessControlList (SecurableClassDefinitionData classData, ISecurityContext context)
    {
      if (context.IsStateless)
        return classData.StatelessAccessControlList;
      else
        return classData.StatefulAccessControlLists.Where (acl => MatchesStates (context, acl.States)).Select (acl => acl.Handle).FirstOrDefault();
    }

    private bool MatchesStates (ISecurityContext context, ICollection<State> states)
    {
      if (context.GetNumberOfStates() > states.Count)
        return false;

      return states.All (s => MatchesState (context, s));
    }

    private bool MatchesState (ISecurityContext context, State state)
    {
      if (!context.ContainsState (state.PropertyName))
        throw CreateAccessControlException ("The state '{0}' is missing in the security context.", state.PropertyName);

      var enumWrapper = context.GetState (state.PropertyName);

      var validStates = _securityContextRepository.GetStatePropertyValues (state.PropertyHandle);
      if (!validStates.Contains (enumWrapper.Name))
      {
        throw CreateAccessControlException (
            "The state '{0}' is not defined for the property '{1}' of the securable class '{2}' or its base classes.",
            enumWrapper.Name,
            state.PropertyName,
            context.Class);
      }

      return enumWrapper.Name.Equals (state.Value);
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException (string.Format (message, args));
    }
  }
}
