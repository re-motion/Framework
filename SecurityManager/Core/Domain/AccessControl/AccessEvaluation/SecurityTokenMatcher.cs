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
using Remotion.Data.DomainObjects;
using Remotion.FunctionalProgramming;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  public class SecurityTokenMatcher
  {
    private readonly AccessControlEntry _ace;
    private readonly ClientTransaction _clientTransaction;

    public SecurityTokenMatcher (AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull("ace", ace);

      _ace = ace;
      _clientTransaction = ace.RootTransaction;
    }

    public bool MatchesToken (SecurityToken token)
    {
      ArgumentUtility.CheckNotNull("token", token);

      if (token.Principal.IsNull)
        return false;

      if (!MatchesTenantCondition(token))
        return false;

      if (!MatchesAbstractRole(token))
        return false;

      if (!MatchesUserCondition(token))
        return false;

      if (!MatchesGroupCondition(token))
        return false;

      return true;
    }

    private bool MatchesTenantCondition (SecurityToken token)
    {
      switch (_ace.TenantCondition)
      {
        case TenantCondition.None:
          return true;

        case TenantCondition.OwningTenant:
          return MatchPrincipalAgainstTenant(token.Principal, GetOwningTenant(token));

        case TenantCondition.SpecificTenant:
          return MatchPrincipalAgainstTenant(token.Principal, _ace.SpecificTenant);

        default:
          throw CreateInvalidOperationException("The value '{0}' is not a valid value for 'TenantCondition'.", _ace.TenantCondition);
      }
    }

    private bool MatchPrincipalAgainstTenant (Principal principal, Tenant? referenceTenant)
    {
      if (referenceTenant == null)
        return false;

      switch (_ace.TenantHierarchyCondition)
      {
        case TenantHierarchyCondition.Undefined:
          throw CreateInvalidOperationException("The value 'Undefined' is not a valid value for matching the 'TenantHierarchyCondition'.");

        case TenantHierarchyCondition.This:
          return referenceTenant.Equals(GetPrincipalTenant(principal));

        case TenantHierarchyCondition.Parent:
          throw CreateInvalidOperationException("The value 'Parent' is not a valid value for matching the 'TenantHierarchyCondition'.");

        case TenantHierarchyCondition.ThisAndParent:
          return GetTenantAndParents(referenceTenant).Contains(GetPrincipalTenant(principal));

        default:
          throw CreateInvalidOperationException(
              "The value '{0}' is not a valid value for 'TenantHierarchyCondition'.", _ace.TenantHierarchyCondition);
      }
    }

    private bool MatchesAbstractRole (SecurityToken token)
    {
      if (_ace.SpecificAbstractRole == null)
        return true;

      return GetAbstractRoles(token).Contains(_ace.SpecificAbstractRole);
    }

    private bool MatchesUserCondition (SecurityToken token)
    {
      switch (_ace.UserCondition)
      {
        case UserCondition.None:
          return true;

        case UserCondition.Owner:
          return MatchPrincipalAgainstUser(token.Principal, token.OwningUser);

        case UserCondition.SpecificUser:
        {
          Assertion.IsNotNull(
              _ace.SpecificUser,
              "AccessControlEntry{{{0}}}.SpecificUser != null when AccessControlEntry.UserCondition == UserCondition.SpecificUser",
              _ace.ID);
          return MatchPrincipalAgainstUser(token.Principal, _ace.SpecificUser.GetHandle());
        }

        case UserCondition.SpecificPosition:
          return MatchPrincipalAgainstPosition(token.Principal);

        default:
          throw CreateInvalidOperationException("The value '{0}' is not a valid value for 'UserCondition'.", _ace.UserCondition);
      }
    }

    private bool MatchPrincipalAgainstUser (Principal principal, IDomainObjectHandle<User>? referenceUser)
    {
      // User is only compared using the DomainObjectHandle to prevent spamming the ClientTransaction with unnecessary User-objects

      if (referenceUser == null)
        return false;

      var principalUser = principal.User;
      if (principalUser == null)
        return false;

      return referenceUser.ObjectID.Equals(principalUser.ObjectID);
    }

    private bool MatchPrincipalAgainstPosition (Principal principal)
    {
      return GetMatchingPrincipalRoles(principal).Any();
    }

    private bool MatchesGroupCondition (SecurityToken token)
    {
      switch (_ace.GroupCondition)
      {
        case GroupCondition.None:
          return true;

        case GroupCondition.OwningGroup:
          return MatchPrincipalAgainstGroup(token.Principal, GetOwningGroup(token), _ace.GroupHierarchyCondition);

        case GroupCondition.SpecificGroup:
          return MatchPrincipalAgainstGroup(token.Principal, _ace.SpecificGroup, _ace.GroupHierarchyCondition);

          // BranchOfOwningGroup matches the ACE if: The (security token) owning group or any of its parent-groups has the 
          // group type stored in the ACE - and - the principal is a member of any of the groups which come below the first
          // group in the group hierarchy including and above the owning group whose group type is equal to the ACE-group-type.
          // Algorithmically the matcher first walks upward the group hierarchy of the owning group (including the owning group itself),
          // returning the first group whose group type is equal to the group type stored in the ACE. 
          // Starting with this group, it then traverses the group hierarchy downward and checks if any of the groups is a member
          // of the principal's groups; if yes, the ACE matches, otherwise it does not.
        case GroupCondition.BranchOfOwningGroup:
          return MatchPrincipalAgainstGroup(token.Principal, FindBranchRoot(GetOwningGroup(token)), GroupHierarchyCondition.ThisAndChildren);

        case GroupCondition.AnyGroupWithSpecificGroupType:
          return MatchPrincipalAgainstPosition(token.Principal);

        default:
          throw CreateInvalidOperationException("The value '{0}' is not a valid value for 'GroupCondition'.", _ace.GroupCondition);
      }
    }

    // Starting at the passed group, returns the first group in the parent hierarchy of the passed group whose
    // GroupType is equal to the ACE specific GroupType.
    private Group? FindBranchRoot (Group? referenceGroup)
    {
      Assertion.IsTrue(_ace.GroupCondition == GroupCondition.BranchOfOwningGroup);

      if (referenceGroup == null)
        return null;

      Assertion.IsNotNull(
          _ace.SpecificGroupType,
          "AccessControlEntry{{{0}}}.SpecificGroupType != null when AccessControlEntry.GroupCondition == GroupCondition.BranchOfOwningGroup",
          _ace.ID);

      return GetGroupAndParents(referenceGroup).FirstOrDefault(g => _ace.SpecificGroupType.Equals(g.GroupType));
    }

    private bool MatchPrincipalAgainstGroup (Principal principal, Group? referenceGroup, GroupHierarchyCondition groupHierarchyCondition)
    {
      if (referenceGroup == null)
        return false;

      var principalRoles = GetMatchingPrincipalRoles(principal);
      var principalGroups = principalRoles.Select(r => r.Group.GetObjectReference());

      Func<bool> isPrincipalMatchingReferenceGroupOrParents = () => principalGroups.Intersect(GetGroupAndParents(referenceGroup)).Any();

      Func<bool> isPrincipalMatchingReferenceGroupOrChildren = () => principalGroups.SelectMany(GetGroupAndParents).Contains(referenceGroup);

      switch (groupHierarchyCondition)
      {
        case GroupHierarchyCondition.Undefined:
          throw CreateInvalidOperationException("The value 'Undefined' is not a valid value for matching the 'GroupHierarchyCondition'.");

        case GroupHierarchyCondition.This:
          return principalGroups.Contains(referenceGroup);

        case GroupHierarchyCondition.Parent:
          throw CreateInvalidOperationException("The value 'Parent' is not a valid value for matching the 'GroupHierarchyCondition'.");

        case GroupHierarchyCondition.Children:
          throw CreateInvalidOperationException("The value 'Children' is not a valid value for matching the 'GroupHierarchyCondition'.");

        case GroupHierarchyCondition.ThisAndParent:
          return isPrincipalMatchingReferenceGroupOrParents();

        case GroupHierarchyCondition.ThisAndChildren:
          return isPrincipalMatchingReferenceGroupOrChildren();

        case GroupHierarchyCondition.ThisAndParentAndChildren:
          return isPrincipalMatchingReferenceGroupOrParents() || isPrincipalMatchingReferenceGroupOrChildren();

        default:
          throw CreateInvalidOperationException("The value '{0}' is not a valid value for 'GroupHierarchyCondition'.", groupHierarchyCondition);
      }
    }

    private IEnumerable<PrincipalRole> GetMatchingPrincipalRoles (Principal principal)
    {
      var roles = (IEnumerable<PrincipalRole>)principal.Roles;
      if (_ace.UserCondition == UserCondition.SpecificPosition)
      {
        Assertion.IsNotNull(
            _ace.SpecificPosition,
            "AccessControlEntry{{{0}}}.SpecificPosition != null when AccessControlEntry.UserCondition == UserCondition.SpecificPosition",
            _ace.ID);

        roles = roles.Where(r => _ace.SpecificPosition.Equals(r.Position.GetObjectReference(_clientTransaction)));
      }

      bool hasSpecificPositionAndGroupType;
      if (_ace.UserCondition == UserCondition.SpecificPosition && _ace.GroupCondition == GroupCondition.BranchOfOwningGroup)
      {
        Assertion.IsNotNull(
            _ace.SpecificGroupType,
            "AccessControlEntry{{{0}}}.SpecificGroupType != null when AccessControlEntry.GroupCondition == GroupCondition.BranchOfOwningGroup",
            _ace.ID);

        hasSpecificPositionAndGroupType = true;
      }
      else if (_ace.GroupCondition == GroupCondition.AnyGroupWithSpecificGroupType)
      {
        Assertion.IsNotNull(
            _ace.SpecificGroupType,
            "AccessControlEntry{{{0}}}.SpecificGroupType != null when AccessControlEntry.GroupCondition == GroupCondition.AnyGroupWithSpecificGroupType",
            _ace.ID);

        hasSpecificPositionAndGroupType = true;
      }
      else
      {
        hasSpecificPositionAndGroupType = false;
      }

      if (hasSpecificPositionAndGroupType)
      {
        Assertion.DebugIsNotNull(_ace.SpecificGroupType, "_ace.SpecificGroupType != null");
        roles = roles.Where(r => _ace.SpecificGroupType.Equals(r.Group.GetObject(_clientTransaction).GroupType));
      }

      return roles;
    }

    private Tenant? GetPrincipalTenant (Principal principal)
    {
      if (principal.Tenant == null)
        return null;

      return principal.Tenant.GetObjectReference(_clientTransaction);
    }

    private Tenant? GetOwningTenant (SecurityToken token)
    {
      if (token.OwningTenant == null)
        return null;

      return token.OwningTenant.GetObjectReference(_clientTransaction);
    }

    private Group? GetOwningGroup (SecurityToken token)
    {
      if (token.OwningGroup == null)
        return null;

      return token.OwningGroup.GetObjectReference(_clientTransaction);
    }

    private IEnumerable<AbstractRoleDefinition> GetAbstractRoles (SecurityToken token)
    {
      return token.AbstractRoles.Select(abstractRole => abstractRole.GetObjectReference(_clientTransaction));
    }

    private IEnumerable<Tenant> GetTenantAndParents (Tenant tenant)
    {
      return EnumerableUtility.Singleton(tenant).Concat(tenant.GetParents());
    }

    private IEnumerable<Group> GetGroupAndParents (Group @group)
    {
      return EnumerableUtility.Singleton(@group).Concat(@group.GetParents());
    }

    private InvalidOperationException CreateInvalidOperationException (string message, params object[] args)
    {
      return new InvalidOperationException(string.Format(message, args));
    }
  }
}
