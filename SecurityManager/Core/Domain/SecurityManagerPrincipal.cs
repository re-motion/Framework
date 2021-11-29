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
using JetBrains.Annotations;
using Remotion.Context;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Security;
using Remotion.FunctionalProgramming;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  /// <summary>
  /// The <see cref="SecurityManagerPrincipal"/> type represents the current <see cref="Tenant"/>, <see cref="User"/>, 
  /// and optional <see cref="Substitution"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The <see cref="Current"/> <see cref="SecurityManagerPrincipal"/> is hosted by the <see cref="SafeContext"/>, i.e. it is thread-local in ordinary 
  /// applications and request-local (HttpContext) in applications using Remotion.Web.
  /// </para>
  /// <para>
  /// The domain objects held by a <see cref="SecurityManagerPrincipal"/> instance are stored in a dedicated <see cref="ClientTransaction"/>.
  /// Changes made to those objects are only saved when that transaction is committed, eg. via 
  /// <code>SecurityManagerPrincipal.Current.User.RootTransaction.Commit()</code>.
  /// </para>
  /// </remarks>
  /// <threadsafety static="true" instance="true"/>
  [Serializable]
  public sealed class SecurityManagerPrincipal : ISecurityManagerPrincipal
  {
    public static readonly ISecurityManagerPrincipal Null = new NullSecurityManagerPrincipal();

    private static readonly SafeContextSingleton<ISecurityManagerPrincipal> s_principal =
        new SafeContextSingleton<ISecurityManagerPrincipal>(SafeContextKeys.SecurityManagerPrincipalCurrent, () => Null);

    [NotNull]
    public static ISecurityManagerPrincipal Current
    {
      get { return s_principal.Current; }
      set
      {
        ArgumentUtility.CheckNotNull("value", value);
        s_principal.SetCurrent(value);
      }
    }

    private readonly IDomainObjectHandle<Tenant> _tenantHandle;
    private readonly IDomainObjectHandle<User> _userHandle;
    private readonly IReadOnlyList<IDomainObjectHandle<Role>> _roleHandles;
    private readonly IDomainObjectHandle<Substitution> _substitutionHandle;
    [CanBeNull]
    private readonly IDomainObjectHandle<User> _substitutedUserHandle;

    [CanBeNull]
    private readonly IReadOnlyList<IDomainObjectHandle<Role>> _substitutedRoleHandles;

    /// <summary>Required in case the domain change is an update to the tenant, group, or position identifiers.</summary>
    private readonly GuidRevisionValue _domainRevision;

    private readonly GuidRevisionValue _userRevision;

    private readonly TenantProxy _tenantProxy;
    private readonly UserProxy _userProxy;
    private readonly IReadOnlyList<RoleProxy> _roleProxies;
    private readonly SubstitutionProxy _substitutionProxy;
    private readonly ISecurityPrincipal _securityPrincipal;

    public SecurityManagerPrincipal (
        [NotNull] IDomainObjectHandle<Tenant> tenantHandle,
        [NotNull] IDomainObjectHandle<User> userHandle,
        [CanBeNull] IReadOnlyList<IDomainObjectHandle<Role>> roleHandles,
        [CanBeNull] IDomainObjectHandle<Substitution> substitutionHandle,
        [CanBeNull] IDomainObjectHandle<User> substitutedUserHandle,
        [CanBeNull] IReadOnlyList<IDomainObjectHandle<Role>> substitutedRoleHandles)
    {
      ArgumentUtility.CheckNotNull("tenantHandle", tenantHandle);
      ArgumentUtility.CheckNotNull("userHandle", userHandle);
      if (substitutionHandle == null && (substitutedUserHandle != null || substitutedRoleHandles != null))
      {
        throw new ArgumentException(
            "When the 'substitutedUserHandle' or the 'substitutedRoleHandles' are set, the 'substitutionHandle' must also be specified.",
            "substitutionHandle");
      }

      _tenantHandle = tenantHandle;
      _userHandle = userHandle;
      _roleHandles = roleHandles;
      _substitutionHandle = substitutionHandle;
      _substitutedUserHandle = substitutedUserHandle;
      _substitutedRoleHandles = substitutedRoleHandles;

      var transaction = CreateClientTransaction();

      _tenantProxy = CreateTenantProxy(GetTenant(transaction));
      _userProxy = CreateUserProxy(GetUser(transaction));
      _roleProxies = CreateRoleProxies(GetRoles(transaction));
      var substitution = GetSubstitution(transaction);
      _substitutionProxy = substitution != null ? CreateSubstitutionProxy(substitution) : null;
      _securityPrincipal = CreateSecurityPrincipal(transaction);

      _domainRevision = GetDomainRevision();
      _userRevision = GetUserRevision(_securityPrincipal.User);
    }

    public TenantProxy Tenant
    {
      get { return _tenantProxy; }
    }

    public UserProxy User
    {
      get { return _userProxy; }
    }

    public IReadOnlyList<RoleProxy> Roles
    {
      get { return _roleProxies; }
    }

    public SubstitutionProxy Substitution
    {
      get { return _substitutionProxy; }
    }

    public ISecurityPrincipal GetSecurityPrincipal ()
    {
      return _securityPrincipal;
    }

    public ISecurityManagerPrincipal GetRefreshedInstance ()
    {
      if (_domainRevision.IsCurrent(GetDomainRevision()) && _userRevision.IsCurrent(GetUserRevision(_securityPrincipal.User)))
        return this;
      return new SecurityManagerPrincipal(
          _tenantHandle,
          _userHandle,
          _roleHandles,
          _substitutionHandle,
          _substitutedUserHandle,
          _substitutedRoleHandles);
    }

    public TenantProxy[] GetTenants (bool includeAbstractTenants)
    {
      Tenant tenant;
      using (SecurityFreeSection.Activate())
      {
        tenant = GetUser(CreateClientTransaction()).Tenant;
      }

      return tenant.GetHierachy()
                   .Where(t => includeAbstractTenants || !t.IsAbstract)
                   .Select(CreateTenantProxy)
                   .ToArray();
    }

    public SubstitutionProxy[] GetActiveSubstitutions ()
    {
      return GetUser(CreateClientTransaction()).GetActiveSubstitutions()
                                                .Select(CreateSubstitutionProxy)
                                                .ToArray();
    }

    private SecurityPrincipal CreateSecurityPrincipal (ClientTransaction transaction)
    {
      using (SecurityFreeSection.Activate())
      {
        string user = GetUser(transaction).UserName;

        ISecurityPrincipalRole[] roles = null;
        var rolesOrNull = GetRoles(transaction);
        if (rolesOrNull != null)
          roles = rolesOrNull.Select(CreateSecurityPrincipalRole).ToArray();

        var substitutedUserObject = GetSubstitutedUser(transaction);
        var substitutedRoleObjects = GetSubstitutedRoles(transaction);
        if (substitutedUserObject == null && substitutedRoleObjects == null)
        {
          Substitution substitution = GetSubstitution(transaction);
          if (substitution != null)
          {
            substitutedUserObject = substitution.SubstitutedUser;
            if (substitution.SubstitutedRole != null)
              substitutedRoleObjects = EnumerableUtility.Singleton(substitution.SubstitutedRole);
          }
        }

        string substitutedUser = null;
        if (substitutedUserObject != null)
          substitutedUser = substitutedUserObject.UserName;

        ISecurityPrincipalRole[] substitutedRoles = null;
        if (substitutedRoleObjects != null)
          substitutedRoles = substitutedRoleObjects.Select(CreateSecurityPrincipalRole).ToArray();

        return new SecurityPrincipal(user, roles, substitutedUser, substitutedRoles);
      }
    }

    private static ISecurityPrincipalRole CreateSecurityPrincipalRole (Role role)
    {
      return new SecurityPrincipalRole(role.Group.UniqueIdentifier, role.Position.UniqueIdentifier);
    }

    private TenantProxy CreateTenantProxy (Tenant tenant)
    {
      using (SecurityFreeSection.Activate())
      {
        return TenantProxy.Create(tenant);
      }
    }

    private UserProxy CreateUserProxy (User user)
    {
      using (SecurityFreeSection.Activate())
      {
        return UserProxy.Create(user);
      }
    }

    [CanBeNull]
    private IReadOnlyList<RoleProxy> CreateRoleProxies (IEnumerable<Role> roles)
    {
      if (roles == null)
        return null;

      using (SecurityFreeSection.Activate())
      {
        return roles.Select(RoleProxy.Create).ToArray();
      }
    }

    private SubstitutionProxy CreateSubstitutionProxy (Substitution substitution)
    {
      using (SecurityFreeSection.Activate())
      {
        return SubstitutionProxy.Create(substitution);
      }
    }

    private Tenant GetTenant (ClientTransaction transaction)
    {
      return _tenantHandle.GetObject(transaction);
    }

    private User GetUser (ClientTransaction transaction)
    {
      return _userHandle.GetObject(transaction);
    }

    [CanBeNull]
    private IEnumerable<Role> GetRoles (ClientTransaction transaction)
    {
      if (_roleHandles == null)
        return null;

      return LifetimeService.TryGetObjects<Role>(transaction, _roleHandles.Select(r => r.ObjectID)).Where(r => r != null);
    }

    [CanBeNull]
    private Substitution GetSubstitution (ClientTransaction transaction)
    {
      if (_substitutionHandle == null)
        return null;

      return (Substitution) LifetimeService.GetObject(transaction, _substitutionHandle.ObjectID, false);
    }

    private User GetSubstitutedUser (ClientTransaction transaction)
    {
      if (_substitutedUserHandle == null)
        return null;

      return _substitutedUserHandle.GetObject(transaction);
    }

    [CanBeNull]
    private IEnumerable<Role> GetSubstitutedRoles (ClientTransaction transaction)
    {
      if (_substitutedRoleHandles == null)
        return null;

      return LifetimeService.TryGetObjects<Role>(transaction, _substitutedRoleHandles.Select(r=>r.ObjectID)).Where(r=>r != null);
    }

    private ClientTransaction CreateClientTransaction ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();

      if (!SecurityConfiguration.Current.DisableAccessChecks)
        transaction.Extensions.Add(new SecurityClientTransactionExtension());

      return transaction;
    }

    private GuidRevisionValue GetDomainRevision ()
    {
      return SafeServiceLocator.Current.GetInstance<IDomainRevisionProvider>().GetRevision(new RevisionKey());
    }

    private GuidRevisionValue GetUserRevision (string userName)
    {
      return SafeServiceLocator.Current.GetInstance<IUserRevisionProvider>().GetRevision(new UserRevisionKey(userName));
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
