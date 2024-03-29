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
using System.Web.UI;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Security;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Domain;
using Remotion.Utilities;
using SecurityManagerUser = Remotion.SecurityManager.Domain.OrganizationalStructure.User;

namespace Remotion.SecurityManager.Clients.Web
{
  public partial class DefaultPage : Page
  {
    private ClientTransaction? _clientTransaction;

    protected SecurityManagerHttpApplication ApplicationInstance
    {
      get { return (SecurityManagerHttpApplication)Context.ApplicationInstance; }
    }

    protected override void OnLoad (EventArgs e)
    {
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

    protected void UsersField_SelectionChanged (object sender, EventArgs e)
    {
      var user = (SecurityManagerUser?)UsersField.Value;
      if (user == null)
      {
        ApplicationInstance.SetCurrentPrincipal(SecurityManagerPrincipal.Null);
      }
      else
      {
        Assertion.IsNotNull(user.Tenant, "User{{{0}}}.Tenant != null", user.ID);

        var securityManagerPrincipal =
            ApplicationInstance.SecurityManagerPrincipalFactory.Create(user.Tenant.GetHandle(), user.GetHandle(), null);
        ApplicationInstance.SetCurrentPrincipal(securityManagerPrincipal);
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      Assertion.IsNotNull(_clientTransaction, "_clientTransaction != null");
      _clientTransaction.EnterDiscardingScope();
      base.OnPreRender(e);
    }

    protected override void OnUnload (EventArgs e)
    {
      base.OnUnload(e);
      ClientTransactionScope.ResetActiveScope();
    }
  }
}
