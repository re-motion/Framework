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
using System.Security.Principal;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects;
using Remotion.FunctionalProgramming;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  /// <summary>
  /// The <see cref="SecurityTokenBuilder"/> is responsible for creating a <see cref="SecurityToken"/> from an <see cref="ISecurityContext"/> and an
  /// <see cref="IPrincipal"/>.
  /// </summary>
  [ImplementationFor(typeof(ISecurityTokenBuilder), Lifetime = LifetimeKind.Singleton)]
  public class SecurityTokenBuilder : ISecurityTokenBuilder
  {
    private readonly ISecurityPrincipalRepository _securityPrincipalRepository;
    private readonly ISecurityContextRepository _securityContextRepository;

    public SecurityTokenBuilder (ISecurityPrincipalRepository securityPrincipalRepository, ISecurityContextRepository securityContextRepository)
    {
      ArgumentUtility.CheckNotNull("securityPrincipalRepository", securityPrincipalRepository);
      ArgumentUtility.CheckNotNull("securityContextRepository", securityContextRepository);

      _securityPrincipalRepository = securityPrincipalRepository;
      _securityContextRepository = securityContextRepository;
    }

    /// <exception cref="AccessControlException">
    ///   A matching <see cref="User"/> is not found for the <paramref name="principal"/>.<br/>- or -<br/>
    ///   A matching <see cref="Group"/> is not found for the <paramref name="context"/>'s <see cref="ISecurityContext.OwnerGroup"/>.<br/>- or -<br/>
    ///   A matching <see cref="AbstractRoleDefinition"/> is not found for all entries in the <paramref name="context"/>'s <see cref="SecurityContext.AbstractRoles"/> collection.
    /// </exception>
    public SecurityToken CreateToken (ISecurityPrincipal principal, ISecurityContext context)
    {
      ArgumentUtility.CheckNotNull("principal", principal);
      ArgumentUtility.CheckNotNull("context", context);

        var principalUser = CreatePrincipal(principal);
        var owningTenant = GetTenant(context.OwnerTenant);
        var owningGroup = GetGroup(context.OwnerGroup);
        var owningUser = GetUser(context.Owner);
        var abstractRoles = GetAbstractRoles(context.AbstractRoles);

        return new SecurityToken(principalUser, owningTenant, owningGroup, owningUser, abstractRoles);
    }

    private Principal CreatePrincipal (ISecurityPrincipal principal)
    {
      if (principal.IsNull)
        return Principal.Null;

      if (string.IsNullOrEmpty(principal.User))
        throw CreateAccessControlException("No principal was provided.");

      User user = _securityPrincipalRepository.GetUser(principal.User);
      lock (user.RootTransaction)
      {
        Tenant principalTenant = Assertion.IsNotNull(user.Tenant, "User{{{0}}}.Tenant != null", user.ID);
        User? principalUser;
        IEnumerable<Role> principalRoles;

        if (principal.SubstitutedUser != null)
        {
          var substitutionsWithMatchingUser = user.GetActiveSubstitutions()
              .Where(s => Assertion.IsNotNull(s.SubstitutedUser, "Substitution{{{0}}}.SubstitutedUser != null", s.ID).UserName == principal.SubstitutedUser)
              .Where(s => Assertion.IsNotNull(s.SubstitutedUser, "Substitution{{{0}}}.SubstitutedUser != null", s.ID).Tenant == user.Tenant)
              .ToArray();

          var substitutionForUser = substitutionsWithMatchingUser.FirstOrDefault(s => s.SubstitutedRole == null);

          var substitutionForRoles = FilterSubstitutionsForMatchingRoles(substitutionsWithMatchingUser, principal.SubstitutedRoles);

          if (principal.SubstitutedRoles == null && substitutionForUser != null)
          {
            Assertion.DebugIsNotNull(substitutionForUser.SubstitutedUser, "substitutionForUser.SubstitutedUser != null");
            principalUser = substitutionForUser.SubstitutedUser;
            principalRoles = substitutionForUser.SubstitutedUser.Roles;
          }
          else if (principal.SubstitutedRoles != null && substitutionForRoles != null)
          {
            Assertion.DebugIsNotNull(substitutionForRoles.SubstitutedUser, "substitutionForRoles.SubstitutedUser != null");
            Assertion.DebugIsNotNull(substitutionForRoles.SubstitutedRole, "substitutionForRoles.SubstitutedRole != null");
            principalUser = substitutionForRoles.SubstitutedUser;
            principalRoles = EnumerableUtility.Singleton(substitutionForRoles.SubstitutedRole);
          }
          else if (principal.SubstitutedRoles != null && substitutionForRoles == null && substitutionForUser != null)
          {
            Assertion.DebugIsNotNull(substitutionForUser.SubstitutedUser, "substitutionForUser.SubstitutedUser != null");
            principalUser = substitutionForUser.SubstitutedUser;
            principalRoles = substitutionForUser.SubstitutedUser.Roles.Where(r => IsRoleContainedInPrincipalRoles(r, principal.SubstitutedRoles));
          }
          else
          {
            principalUser = null;
            principalRoles = new Role[0];
          }
        }
        else if (principal.SubstitutedRoles != null)
        {
          Assertion.IsNull(principal.SubstitutedUser);
          var substitutionsWithMatchingTenant = user.GetActiveSubstitutions()
              .Where(s => Assertion.IsNotNull(s.SubstitutedUser, "Substitution{{{0}}}.SubstitutedUser != null", s.ID).Tenant == user.Tenant)
              .ToArray();

          var substitutionForRoles = FilterSubstitutionsForMatchingRoles(substitutionsWithMatchingTenant, principal.SubstitutedRoles);

          principalUser = null;
          if (substitutionForRoles != null)
          {
            Assertion.DebugIsNotNull(substitutionForRoles.SubstitutedRole, "substitutionForRoles.SubstitutedRole != null");
            principalRoles = EnumerableUtility.Singleton(substitutionForRoles.SubstitutedRole);
          }
          else
          {
            principalRoles = new Role[0];
          }
        }
        else
        {
          principalUser = user;
          principalRoles = user.Roles;

          if (principal.Roles != null)
            principalRoles = principalRoles.Where(r => IsRoleContainedInPrincipalRoles(r, principal.Roles));
        }

        return new Principal(
            principalTenant.GetHandle(),
            principalUser.GetSafeHandle(),
            principalRoles.Select(
                r => new PrincipalRole(
                    Assertion.IsNotNull(r.Position, "Role{{{0}}}.Position != null", r.ID).GetHandle(),
                    Assertion.IsNotNull(r.Group, "Role{{{0}}}.Group != null", r.ID).GetHandle())));
      }
    }

    private Substitution? FilterSubstitutionsForMatchingRoles (
        IEnumerable<Substitution> availableSubstitutions,
        IReadOnlyList<ISecurityPrincipalRole>? principalSubstitutedRoles)
    {
      if (principalSubstitutedRoles == null)
        return null;

      if (principalSubstitutedRoles.Count == 0)
        return null;

      // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
      return availableSubstitutions
          .Where(s => s.SubstitutedRole != null)
          // ReSharper disable once AssignNullToNotNullAttribute
          .Where(s => principalSubstitutedRoles.All(r => IsRoleMatchingPrincipalRole(s.SubstitutedRole, r)))
          .FirstOrDefault();
    }

    private bool IsRoleContainedInPrincipalRoles ([CanBeNull]Role? role, [NotNull] IEnumerable<ISecurityPrincipalRole> principalRoles)
    {
      // ReSharper disable once LoopCanBeConvertedToQuery
      foreach (var principalRole in principalRoles)
      {
        if (IsRoleMatchingPrincipalRole(role, principalRole))
          return true;
      }
      return false;
    }

    private bool IsRoleMatchingPrincipalRole ([CanBeNull]Role? role, [NotNull] ISecurityPrincipalRole principalRole)
    {
      if (role == null)
        return false;

      Assertion.DebugIsNotNull(role.Position, "role.Position != null");
      var principalPositionHandle = _securityContextRepository.GetPosition(principalRole.Position);
      if (!principalPositionHandle.Equals(role.Position.GetHandle()))
        return false;

      Assertion.DebugIsNotNull(role.Group, "role.Group != null");
      var principalRoleGroupHandle = _securityContextRepository.GetGroup(principalRole.Group);
      if (!principalRoleGroupHandle.Equals(role.Group.GetHandle()))
        return false;

      return true;
    }

    private IDomainObjectHandle<Tenant>? GetTenant (string? uniqueIdentifier)
    {
      if (string.IsNullOrEmpty(uniqueIdentifier))
        return null;

      return _securityContextRepository.GetTenant(uniqueIdentifier);
    }

    private IDomainObjectHandle<User>? GetUser (string? userName)
    {
      if (string.IsNullOrEmpty(userName))
        return null;

      return _securityContextRepository.GetUser(userName);
    }

    private IDomainObjectHandle<Group>? GetGroup (string? uniqueIdentifier)
    {
      if (string.IsNullOrEmpty(uniqueIdentifier))
        return null;

      return _securityContextRepository.GetGroup(uniqueIdentifier);
    }

    private IEnumerable<IDomainObjectHandle<AbstractRoleDefinition>> GetAbstractRoles (IEnumerable<EnumWrapper> abstractRoleNames)
    {
      return abstractRoleNames.Select(name => _securityContextRepository.GetAbstractRole(name));
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException(string.Format(message, args));
    }
  }
}
