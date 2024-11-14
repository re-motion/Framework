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
