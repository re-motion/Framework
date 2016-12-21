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
  [ImplementationFor (typeof (ISecurityTokenBuilder), Lifetime = LifetimeKind.Singleton)]
  public class SecurityTokenBuilder : ISecurityTokenBuilder
  {
    private readonly ISecurityPrincipalRepository _securityPrincipalRepository;
    private readonly ISecurityContextRepository _securityContextRepository;

    public SecurityTokenBuilder (ISecurityPrincipalRepository securityPrincipalRepository, ISecurityContextRepository securityContextRepository)
    {
      ArgumentUtility.CheckNotNull ("securityPrincipalRepository", securityPrincipalRepository);
      ArgumentUtility.CheckNotNull ("securityContextRepository", securityContextRepository);
      
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
      ArgumentUtility.CheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNull ("context", context);

        var principalUser = CreatePrincipal (principal);
        var owningTenant = GetTenant (context.OwnerTenant);
        var owningGroup = GetGroup (context.OwnerGroup);
        var owningUser = GetUser (context.Owner);
        var abstractRoles = GetAbstractRoles (context.AbstractRoles);

        return new SecurityToken (principalUser, owningTenant, owningGroup, owningUser, abstractRoles);
    }

    private Principal CreatePrincipal (ISecurityPrincipal principal)
    {
      if (principal.IsNull)
        return Principal.Null;

      if (string.IsNullOrEmpty (principal.User))
        throw CreateAccessControlException ("No principal was provided.");

      if (string.IsNullOrEmpty (principal.SubstitutedUser) && principal.SubstitutedRole != null)
        throw CreateAccessControlException ("A substituted role was specified without a substituted user.");

      User user = _securityPrincipalRepository.GetUser (principal.User);
      lock (user.RootTransaction)
      {
        Tenant principalTenant = user.Tenant;
        User principalUser;
        IEnumerable<Role> principalRoles;

        if (principal.SubstitutedUser != null)
        {
          Substitution substitution = GetSubstitution (principal, user);

          if (substitution == null)
          {
            principalUser = null;
            principalRoles = new Role[0];
          }
          else if (principal.SubstitutedRole != null)
          {
            principalUser = null;
            principalRoles = EnumerableUtility.Singleton (substitution.SubstitutedRole);
          }
          else
          {
            principalUser = substitution.SubstitutedUser;
            principalRoles = substitution.SubstitutedUser.Roles;
          }
        }
        else
        {
          principalUser = user;
          principalRoles = user.Roles;

          if (principal.Role != null)
            principalRoles = principalRoles.Where (r => IsRoleMatchingPrincipalRole (r, principal.Role));
        }

        return new Principal (
            principalTenant.GetHandle(),
            principalUser.GetSafeHandle(),
            principalRoles.Select (r => new PrincipalRole (r.Position.GetHandle(), r.Group.GetHandle())));
      }
    }

    private Substitution GetSubstitution (ISecurityPrincipal principal, User user)
    {
      IEnumerable<Substitution> substitutions = user.GetActiveSubstitutions ();
      
      substitutions = substitutions.Where (s => s.SubstitutedUser.UserName == principal.SubstitutedUser && s.SubstitutedUser.Tenant == user.Tenant);
      
      if (principal.SubstitutedRole != null)
        substitutions = substitutions.Where (s => IsRoleMatchingPrincipalRole (s.SubstitutedRole, principal.SubstitutedRole));
      else
        substitutions = substitutions.Where (s => s.SubstitutedRole == null);

      return substitutions.FirstOrDefault ();
    }

    private bool IsRoleMatchingPrincipalRole (Role role, ISecurityPrincipalRole principalRole)
    {
      if (role == null)
        return false;

      var principalPositionHandle = _securityContextRepository.GetPosition (principalRole.Position);
      if (!principalPositionHandle.Equals (role.Position.GetHandle()))
        return false;

      var principalRoleGroupHandle = _securityContextRepository.GetGroup (principalRole.Group);
      if (!principalRoleGroupHandle.Equals (role.Group.GetHandle()))
        return false;

      return true;
    }

    private IDomainObjectHandle<Tenant> GetTenant (string uniqueIdentifier)
    {
      if (string.IsNullOrEmpty (uniqueIdentifier))
        return null;

      return _securityContextRepository.GetTenant (uniqueIdentifier);
    }

    private IDomainObjectHandle<User> GetUser (string userName)
    {
      if (string.IsNullOrEmpty (userName))
        return null;

      return _securityContextRepository.GetUser (userName);
    }

    private IDomainObjectHandle<Group> GetGroup (string uniqueIdentifier)
    {
      if (string.IsNullOrEmpty (uniqueIdentifier))
        return null;

      return _securityContextRepository.GetGroup (uniqueIdentifier);
    }

    private IEnumerable<IDomainObjectHandle<AbstractRoleDefinition>> GetAbstractRoles (IEnumerable<EnumWrapper> abstractRoleNames)
    {
      return abstractRoleNames.Select (name => _securityContextRepository.GetAbstractRole (name));
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException (string.Format (message, args));
    }
  }
}
