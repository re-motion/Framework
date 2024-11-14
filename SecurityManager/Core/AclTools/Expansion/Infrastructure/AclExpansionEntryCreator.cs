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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
{
  public class AclExpansionEntryCreator
  {
    public virtual AclExpansionEntry? CreateAclExpansionEntry (UserRoleAclAceCombination userRoleAclAce)
    {
      var accessTypesResult = GetAccessTypes(userRoleAclAce);

      AclExpansionEntry? aclExpansionEntry = null;

      // Create an AclExpansionEntry, if the current probe ACE contributed to the result and returned allowed access types.
      if (accessTypesResult.AccessTypeStatistics.IsInAccessTypesContributingAces(userRoleAclAce.Ace) && accessTypesResult.AccessInformation.AllowedAccessTypes.Length > 0)
      {
        aclExpansionEntry = new AclExpansionEntry(userRoleAclAce.User, userRoleAclAce.Role, userRoleAclAce.Acl, accessTypesResult.AclProbe.AccessConditions,
                                                   accessTypesResult.AccessInformation.AllowedAccessTypes, accessTypesResult.AccessInformation.DeniedAccessTypes);
      }

      return aclExpansionEntry;
    }


    public virtual AclExpansionEntryCreator_GetAccessTypesResult GetAccessTypes (UserRoleAclAceCombination userRoleAclAce)
    {
      if (ClientTransaction.Current == null)
        throw new InvalidOperationException("No ClientTransaction has been associated with the current thread.");

      var aclProbe = AclProbe.CreateAclProbe(userRoleAclAce.User, userRoleAclAce.Role, userRoleAclAce.Ace);

      // Note: The aclProbe created above will NOT always match the ACE it was designed to probe; the reason for this
      // is that its SecurityToken created by the AclProbe is only designed to match the non-decideable access conditions
      // (e.g. abstract role, owning tenant, owning group, etc) of the ACE. If this were not the case, then the AclProbe would need
      // to reproduce code from the SecurityManager, to be able to decide beforehand, whether decideable access condtions
      // (e.g. specific tenant, specific user) will match or not. 
      // 
      // The "non-decideable" here refers to the information context of the AclExpander, which is lacking some information
      // available during normal SecurityManager access rights querying.
      // For decideable access conditons (e.g. specific tenant or specific group), the created SecurityToken
      // is not guaranteed to match, therefore the AccessTypeStatistics returned by Acl.GetAccessTypes are used to filter out these cases.
      //
      // One could also try to remove these entries by removing all AclExpansionEntry|s which are identical to another AclExpansionEntry,
      // apart from having more restrictive AccessConditions; note however that such "double" entries can also come from ACEs which are
      // being shadowed by a 2nd, less restrictive ACE.
      //
      // Note also that it does not suffice to get the access types for the current ACE only, since these rights might be denied
      // by another matching ACE in the current ACL (deny rights always win). 
      var accessTypeStatistics = new AccessTypeStatistics();

      var roles = aclProbe.SecurityToken.Principal.Roles;
      Assertion.IsTrue(roles.Count == 1);
      Assertion.IsTrue(object.ReferenceEquals(roles[0].Position.GetObjectReference(), userRoleAclAce.Role.Position));
      Assertion.IsTrue(object.ReferenceEquals(roles[0].Group.GetObjectReference(), userRoleAclAce.Role.Group));

      AccessInformation accessInformation = userRoleAclAce.Acl.GetAccessTypes(aclProbe.SecurityToken, accessTypeStatistics);

      return new AclExpansionEntryCreator_GetAccessTypesResult(accessInformation, aclProbe, accessTypeStatistics);
    }
  }
}
