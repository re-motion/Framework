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
  [ImplementationFor (typeof(ISecurityTokenBuilder), Lifetime = LifetimeKind.Singleton)]
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
        Tenant principalTenant = user.Tenant;
        User principalUser;
        IEnumerable<Role> principalRoles;

        if (principal.SubstitutedUser != null)
        {
          var substitutionsWithMatchingUser = user.GetActiveSubstitutions()
              .Where(s => s.SubstitutedUser.UserName == principal.SubstitutedUser)
              .Where(s => s.SubstitutedUser.Tenant == user.Tenant)
              .ToArray();

          var substitutionForUser = substitutionsWithMatchingUser.FirstOrDefault(s => s.SubstitutedRole == null);

          var substitutionForRoles = FilterSubstitutionsForMatchingRoles(substitutionsWithMatchingUser, principal.SubstitutedRoles);

          if (principal.SubstitutedRoles == null && substitutionForUser != null)
          {
            principalUser = substitutionForUser.SubstitutedUser;
            principalRoles = substitutionForUser.SubstitutedUser.Roles;
          }
          else if (principal.SubstitutedRoles != null && substitutionForRoles != null)
          {
            principalUser = substitutionForRoles.SubstitutedUser;
            principalRoles = EnumerableUtility.Singleton(substitutionForRoles.SubstitutedRole);
          }
          else if (principal.SubstitutedRoles != null && substitutionForRoles == null && substitutionForUser != null)
          {
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
              .Where(s => s.SubstitutedUser.Tenant == user.Tenant)
              .ToArray();

          var substitutionForRoles = FilterSubstitutionsForMatchingRoles(substitutionsWithMatchingTenant, principal.SubstitutedRoles);

          principalUser = null;
          if (substitutionForRoles != null)
            principalRoles = EnumerableUtility.Singleton(substitutionForRoles.SubstitutedRole);
          else
            principalRoles = new Role[0];
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
            principalRoles.Select(r => new PrincipalRole(r.Position.GetHandle(), r.Group.GetHandle())));
      }
    }

    private Substitution FilterSubstitutionsForMatchingRoles (
        IEnumerable<Substitution> availableSubstitutions,
        IReadOnlyList<ISecurityPrincipalRole> principalSubstitutedRoles)
    {
      // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
      return availableSubstitutions
          .Where(s => s.SubstitutedRole != null)
          .Where(s => principalSubstitutedRoles != null && principalSubstitutedRoles.Any())
          // ReSharper disable once AssignNullToNotNullAttribute
          .Where(s => principalSubstitutedRoles.All(r => IsRoleMatchingPrincipalRole(s.SubstitutedRole, r)))
          .FirstOrDefault();
    }

    private bool IsRoleContainedInPrincipalRoles ([CanBeNull]Role role, [NotNull] IEnumerable<ISecurityPrincipalRole> principalRoles)
    {
      // ReSharper disable once LoopCanBeConvertedToQuery
      foreach (var principalRole in principalRoles)
      {
        if (IsRoleMatchingPrincipalRole(role, principalRole))
          return true;
      }
      return false;
    }

    private bool IsRoleMatchingPrincipalRole ([CanBeNull]Role role, [NotNull] ISecurityPrincipalRole principalRole)
    {
      if (role == null)
        return false;

      var principalPositionHandle = _securityContextRepository.GetPosition(principalRole.Position);
      if (!principalPositionHandle.Equals(role.Position.GetHandle()))
        return false;

      var principalRoleGroupHandle = _securityContextRepository.GetGroup(principalRole.Group);
      if (!principalRoleGroupHandle.Equals(role.Group.GetHandle()))
        return false;

      return true;
    }

    private IDomainObjectHandle<Tenant> GetTenant (string uniqueIdentifier)
    {
      if (string.IsNullOrEmpty(uniqueIdentifier))
        return null;

      return _securityContextRepository.GetTenant(uniqueIdentifier);
    }

    private IDomainObjectHandle<User> GetUser (string userName)
    {
      if (string.IsNullOrEmpty(userName))
        return null;

      return _securityContextRepository.GetUser(userName);
    }

    private IDomainObjectHandle<Group> GetGroup (string uniqueIdentifier)
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
