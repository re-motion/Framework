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
using System.Web.UI;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.Clients.Web.UI.AccessControl
{
  public partial class EditStatelessAccessControlListControl : EditAccessControlListControlBase<StatelessAccessControlList>
  {
    protected override void OnPreRender (EventArgs e)
    {
      var resourceManager = GetResourceManager(typeof(ResourceIdentifier));
      DeleteAccessControlListButton.Text = resourceManager.GetString(ResourceIdentifier.DeleteAccessControlListButtonText);
      NewAccessControlEntryButton.Text = resourceManager.GetString(ResourceIdentifier.NewAccessControlEntryButtonText);

      base.OnPreRender (e);
    }

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override ControlCollection GetAccessControlEntryControls ()
    {
      return AccessControlEntryControls.Controls;
    }
  }
}
