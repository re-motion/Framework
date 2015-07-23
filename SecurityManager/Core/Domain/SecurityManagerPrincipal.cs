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
using System.Linq;
using JetBrains.Annotations;
using Remotion.Context;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Security;
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
        new SafeContextSingleton<ISecurityManagerPrincipal> (SafeContextKeys.SecurityManagerPrincipalCurrent, () => Null);

    [NotNull]
    public static ISecurityManagerPrincipal Current
    {
      get { return s_principal.Current; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        s_principal.SetCurrent (value);
      }
    }

    private readonly IDomainObjectHandle<Tenant> _tenantHandle;
    private readonly IDomainObjectHandle<User> _userHandle;
    private readonly IDomainObjectHandle<Substitution> _substitutionHandle;
    private readonly GuidRevisionValue _revision;
    private readonly TenantProxy _tenantProxy;
    private readonly UserProxy _userProxy;
    private readonly SubstitutionProxy _substitutionProxy;
    private readonly ISecurityPrincipal _securityPrincipal;

    public SecurityManagerPrincipal (
        IDomainObjectHandle<Tenant> tenantHandle, IDomainObjectHandle<User> userHandle, IDomainObjectHandle<Substitution> substitutionHandle)
    {
      ArgumentUtility.CheckNotNull ("tenantHandle", tenantHandle);
      ArgumentUtility.CheckNotNull ("userHandle", userHandle);

      _tenantHandle = tenantHandle;
      _userHandle = userHandle;
      _substitutionHandle = substitutionHandle;

      _revision = GetRevision();
      var transaction = CreateClientTransaction();

      _tenantProxy = CreateTenantProxy (GetTenant (transaction));
      _userProxy = CreateUserProxy (GetUser (transaction));
      var substitution = GetSubstitution (transaction);
      _substitutionProxy = substitution != null ? CreateSubstitutionProxy (substitution) : null;
      _securityPrincipal = CreateSecurityPrincipal (transaction);
    }

    public TenantProxy Tenant
    {
      get { return _tenantProxy; }
    }

    public UserProxy User
    {
      get { return _userProxy; }
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
      var currentRevision = GetRevision();
      if (_revision.IsCurrent (currentRevision))
        return this;
      return new SecurityManagerPrincipal (_tenantHandle, _userHandle, _substitutionHandle);
    }

    public TenantProxy[] GetTenants (bool includeAbstractTenants)
    {
      Tenant tenant;
      using (SecurityFreeSection.Activate())
      {
        tenant = GetUser (CreateClientTransaction()).Tenant;
      }

      return tenant.GetHierachy()
                   .Where (t => includeAbstractTenants || !t.IsAbstract)
                   .Select (CreateTenantProxy)
                   .ToArray();
    }

    public SubstitutionProxy[] GetActiveSubstitutions ()
    {
      return GetUser (CreateClientTransaction()).GetActiveSubstitutions()
                                                .Select (CreateSubstitutionProxy)
                                                .ToArray();
    }

    private SecurityPrincipal CreateSecurityPrincipal (ClientTransaction transaction)
    {
      using (SecurityFreeSection.Activate())
      {
        string user = GetUser (transaction).UserName;
        ISecurityPrincipalRole role = null;

        string substitutedUser = null;
        ISecurityPrincipalRole substitutedRole = null;

        Substitution substitution = GetSubstitution (transaction);
        if (substitution != null)
        {
          substitutedUser = substitution.SubstitutedUser.UserName;
          if (substitution.SubstitutedRole != null)
          {
            substitutedRole = new SecurityPrincipalRole (
                substitution.SubstitutedRole.Group.UniqueIdentifier,
                substitution.SubstitutedRole.Position.UniqueIdentifier);
          }
        }

        return new SecurityPrincipal (user, role, substitutedUser, substitutedRole);
      }
    }

    private TenantProxy CreateTenantProxy (Tenant tenant)
    {
      using (SecurityFreeSection.Activate())
      {
        return TenantProxy.Create (tenant);
      }
    }

    private UserProxy CreateUserProxy (User user)
    {
      using (SecurityFreeSection.Activate())
      {
        return UserProxy.Create (user);
      }
    }

    private SubstitutionProxy CreateSubstitutionProxy (Substitution substitution)
    {
      using (SecurityFreeSection.Activate())
      {
        return SubstitutionProxy.Create (substitution);
      }
    }

    private Tenant GetTenant (ClientTransaction transaction)
    {
      return _tenantHandle.GetObject (transaction);
    }

    private User GetUser (ClientTransaction transaction)
    {
      return _userHandle.GetObject (transaction);
    }

    private Substitution GetSubstitution (ClientTransaction transaction)
    {
      if (_substitutionHandle == null)
        return null;

      return (Substitution) LifetimeService.GetObject (transaction, _substitutionHandle.ObjectID, false);
    }

    private ClientTransaction CreateClientTransaction ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();

      if (!SecurityConfiguration.Current.DisableAccessChecks)
        transaction.Extensions.Add (new SecurityClientTransactionExtension());

      return transaction;
    }

    private GuidRevisionValue GetRevision ()
    {
      return SafeServiceLocator.Current.GetInstance<IDomainRevisionProvider>().GetRevision(new RevisionKey());
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}