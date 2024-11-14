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
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;
using Remotion.Data.DomainObjects;
using Remotion.Reflection;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using SecurityManagerUser = Remotion.SecurityManager.Domain.OrganizationalStructure.User;

namespace Remotion.SecurityManager.Clients.Web.Classes
{
  public class SecurityManagerHttpApplication : HttpApplication
  {
    private static readonly string s_principalKey = typeof(SecurityManagerHttpApplication).GetAssemblyQualifiedNameChecked() + "_Principal";
    private ISecurityManagerPrincipalFactory? _securityManagerPrincipalFactory;

    public SecurityManagerHttpApplication ()
    {
    }

    public ISecurityManagerPrincipalFactory SecurityManagerPrincipalFactory
    {
      get
      {
        Assertion.IsNotNull(_securityManagerPrincipalFactory, "_securityManagerPrincipalFactory != null after HttpApplication.Init()");
        return _securityManagerPrincipalFactory;
      }
      set { _securityManagerPrincipalFactory = ArgumentUtility.CheckNotNull("value", value); }
    }

    public void SetCurrentPrincipal (ISecurityManagerPrincipal securityManagerPrincipal)
    {
      ArgumentUtility.CheckNotNull("securityManagerPrincipal", securityManagerPrincipal);

      SecurityManagerPrincipal.Current = securityManagerPrincipal;
      SavePrincipalToSession(securityManagerPrincipal);
    }

    protected ISecurityManagerPrincipal LoadPrincipalFromSession ()
    {
      return (ISecurityManagerPrincipal?)Session[s_principalKey] ?? SecurityManagerPrincipal.Null;
    }

    protected void SavePrincipalToSession (ISecurityManagerPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("principal", principal);

      Session[s_principalKey] = principal;
    }

    protected bool HasSessionState
    {
      get { return Context.Handler is IRequiresSessionState; }
    }

    [MemberNotNull(nameof(_securityManagerPrincipalFactory))]
    public override void Init ()
    {
      base.Init();

      if (_securityManagerPrincipalFactory == null)
        _securityManagerPrincipalFactory = SafeServiceLocator.Current.GetInstance<ISecurityManagerPrincipalFactory>();

      PostAcquireRequestState += SecurityManagerHttpApplication_PostAcquireRequestState;
    }

    private void SecurityManagerHttpApplication_PostAcquireRequestState (object? sender, EventArgs e)
    {
      if (HasSessionState)
      {
        ISecurityManagerPrincipal principal;
        if (Session.IsNewSession)
          principal = GetSecurityManagerPrincipalByUserName(Context.User);
        else
          principal = LoadPrincipalFromSession();

        SetCurrentPrincipal(principal);
      }
    }

    private ISecurityManagerPrincipal GetSecurityManagerPrincipalByUserName (IPrincipal principal)
    {
      if (principal.Identity == null)
        return SecurityManagerPrincipal.Null;

      if (!principal.Identity.IsAuthenticated)
        return SecurityManagerPrincipal.Null;

      Assertion.IsNotNull(principal.Identity.Name, "IPrincipal.Identity.Name != null when IPrincipal.Identity.IsAuthenticated == true");

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        using (SecurityFreeSection.Activate())
        {
          var user = SecurityManagerUser.FindByUserName(principal.Identity.Name);
          if (user == null)
          {
            return SecurityManagerPrincipal.Null;
          }
          else
          {
            Assertion.IsNotNull(user.Tenant, "User{{{0}}}.Tenant != null", user.ID);
            return SecurityManagerPrincipalFactory.Create(user.Tenant.GetHandle(), user.GetHandle(), null);
          }
        }
      }
    }
  }
}
