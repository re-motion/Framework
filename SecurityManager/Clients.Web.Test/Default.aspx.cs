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
using System.Web.UI;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Security;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Test.Domain;
using Remotion.SecurityManager.Domain;
using Remotion.ServiceLocation;
using SecurityManagerUser = Remotion.SecurityManager.Domain.OrganizationalStructure.User;

namespace Remotion.SecurityManager.Clients.Web.Test
{
  public partial class _Default : Page
  {
    private ClientTransaction _clientTransaction;

    protected SecurityManagerHttpApplication ApplicationInstance
    {
      get { return (SecurityManagerHttpApplication)Context.ApplicationInstance; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad(e);

      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _clientTransaction.EnterDiscardingScope();
      if (!IsPostBack)
      {
        using (SecurityFreeSection.Activate())
        {
          var users = (from u in QueryFactory.CreateLinqQuery<SecurityManagerUser>() orderby u.UserName select u).ToArray();
          var user = SecurityManagerPrincipal.Current.User;

          UsersField.SetBusinessObjectList(users);
          UsersField.LoadUnboundValue(user, false);
        }
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      _clientTransaction.EnterDiscardingScope();
      base.OnPreRender(e);
    }

    protected override void OnUnload (EventArgs e)
    {
      base.OnUnload(e);
      ClientTransactionScope.ResetActiveScope();
    }

    protected void EvaluateSecurity_Click (object sender, EventArgs e)
    {
      ISecurityProvider provider = SafeServiceLocator.Current.GetInstance<ISecurityProvider>();
      SecurityContext context =
          SecurityContext.Create(
              typeof(File),
              "1A",
              "{00000004-1000-0000-0000-000000000007}",
              string.Empty,
              new Dictionary<string, Enum>(),
              new Enum[] { DomainAbstractRoles.Creator });
      ISecurityPrincipal user = new SecurityPrincipal("1A", null, null, null);
      AccessType[] accessTypes = provider.GetAccess(context, user);
    }

    protected void UsersField_SelectionChanged (object sender, EventArgs e)
    {
      var user = (SecurityManagerUser)UsersField.Value;
      if (user == null)
      {
        ApplicationInstance.SetCurrentPrincipal(SecurityManagerPrincipal.Null);
      }
      else
      {
        var securityManagerPrincipal =
            ApplicationInstance.SecurityManagerPrincipalFactory.Create(user.Tenant.GetHandle(), user.GetHandle(), null);
        ApplicationInstance.SetCurrentPrincipal(securityManagerPrincipal);
      }
    }
  }
}
