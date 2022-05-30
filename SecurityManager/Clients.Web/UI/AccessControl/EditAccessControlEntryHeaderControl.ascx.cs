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
using Remotion.Utilities;
using Remotion.Web;

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
      get
      {
        Assertion.IsNotNull(CurrentObject.BusinessObject, "CurrentClassDefinition has not been set.");
        return (SecurableClassDefinition)CurrentObject.BusinessObject;
      }
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues(interim);

      var type = typeof(AccessControlEntry);
      var provider = BindableObjectProvider.GetProviderForBindableObjectType(type);
      var aceClass = provider.GetBindableObjectClass(type);

      var cssHorizontal = "titleCellHorizontal";
      var cssVertical = "titleCellVertical";
      HeaderCells.Controls.Add(CreateTableCell(WebString.Empty, cssHorizontal)); //ExpandButton
      HeaderCells.Controls.Add(CreateTableCell(WebString.Empty, cssHorizontal)); //DeleteButton
      HeaderCells.Controls.Add(CreateTableCell(GetDisplayNameForProperty(aceClass, "TenantCondition"), cssHorizontal));
      HeaderCells.Controls.Add(CreateTableCell(GetDisplayNameForProperty(aceClass, "GroupCondition"), cssHorizontal));
      HeaderCells.Controls.Add(CreateTableCell(GetDisplayNameForProperty(aceClass, "UserCondition"), cssHorizontal));
      HeaderCells.Controls.Add(CreateTableCell(GetDisplayNameForProperty(aceClass, "SpecificAbstractRole"), cssHorizontal));
      HeaderCells.Controls.Add(CreateTableCell(WebString.Empty, cssHorizontal)); //Toggle Permissions
      foreach (var accessType in CurrentClassDefinition.AccessTypes)
        HeaderCells.Controls.Add(CreateTableCell(WebString.CreateFromText(accessType.DisplayName), cssVertical));

      WebString GetDisplayNameForProperty (BindableObjectClass bindableObjectClass, string propertyIdentifier)
      {
        return WebString.CreateFromText(
            Assertion.IsNotNull(
                bindableObjectClass.GetPropertyDefinition(propertyIdentifier),
                "Business object class '{0}' does not contain property '{1}'",
                bindableObjectClass.Identifier,
                propertyIdentifier).DisplayName);
      }
    }

    private HtmlGenericControl CreateTableCell (WebString title, string cssClass)
    {
      var th = new HtmlGenericControl("th");
      th.Attributes.Add("class", cssClass);

      var div = new HtmlGenericControl("div");
      div.InnerHtml = title.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks);
      th.Controls.Add(div);

      return th;
    }
  }
}
