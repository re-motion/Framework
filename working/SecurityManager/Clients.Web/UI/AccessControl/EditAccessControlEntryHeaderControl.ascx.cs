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
using System.Web.UI.HtmlControls;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.Clients.Web.UI.AccessControl
{
  public partial class EditAccessControlEntryHeaderControl : BaseControl
  {
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected SecurableClassDefinition CurrentClassDefinition
    {
      get { return (SecurableClassDefinition) CurrentObject.BusinessObject; }
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      var type = typeof (AccessControlEntry);
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (type);
      var aceClass = provider.GetBindableObjectClass (type);

      var cssHorizontal = "titleCellHorizontal";
      var cssVertical = "titleCellVertical";
      HeaderCells.Controls.Add (CreateTableCell (string.Empty, cssHorizontal)); //ExpandButton
      HeaderCells.Controls.Add (CreateTableCell (string.Empty, cssHorizontal)); //DeleteButton
      HeaderCells.Controls.Add (CreateTableCell (aceClass.GetPropertyDefinition ("TenantCondition").DisplayName, cssHorizontal));
      HeaderCells.Controls.Add (CreateTableCell (aceClass.GetPropertyDefinition ("GroupCondition").DisplayName, cssHorizontal));
      HeaderCells.Controls.Add (CreateTableCell (aceClass.GetPropertyDefinition ("UserCondition").DisplayName, cssHorizontal));
      HeaderCells.Controls.Add (CreateTableCell (aceClass.GetPropertyDefinition ("SpecificAbstractRole").DisplayName, cssHorizontal));
      HeaderCells.Controls.Add (CreateTableCell (string.Empty, cssHorizontal)); //Toggle Permissions
      foreach (var accessType in CurrentClassDefinition.AccessTypes)
        HeaderCells.Controls.Add (CreateTableCell (accessType.DisplayName, cssVertical));
    }

    private HtmlGenericControl CreateTableCell (string title, string cssClass)
    {
      var th = new HtmlGenericControl ("th");
      th.Attributes.Add ("class", cssClass);

      var div = new HtmlGenericControl ("div");
      div.InnerText = title;
      th.Controls.Add (div);

      return th;
    }
  }
}
