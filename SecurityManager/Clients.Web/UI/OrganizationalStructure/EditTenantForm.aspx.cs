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
using Remotion.Globalization;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  public partial class EditTenantForm : BaseEditPage<Tenant>
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure.OrganizationalStructureResources")]
    public enum ResourceIdentifier
    {
      Title,
    }

    protected override IFocusableControl InitialFocusControl
    {
      get { return EditTenantControl.InitialFocusControl; }
    }

    protected override void OnLoad (EventArgs e)
    {
      RegisterDataEditUserControl(EditTenantControl);

      base.OnLoad(e);
    }

    protected override void OnPreRender (EventArgs e)
    {
      Title = GlobalizationService.GetResourceManager(typeof(ResourceIdentifier)).GetString(ResourceIdentifier.Title);

      SaveButton.Text = GlobalResourcesHelper.GetText(GlobalResources.Save);
      CancelButton.Text = GlobalResourcesHelper.GetText(GlobalResources.Cancel);

      base.OnPreRender(e);
    }

    protected override void ShowErrors ()
    {
      ErrorMessageControl.ShowError();
    }

    protected void CancelButton_Click (object sender, EventArgs e)
    {
      throw new WxeUserCancelException();
    }
  }
}
