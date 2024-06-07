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
