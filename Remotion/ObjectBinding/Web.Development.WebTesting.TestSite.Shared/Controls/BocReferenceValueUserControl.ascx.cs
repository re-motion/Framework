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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.Controls
{
  public partial class BocReferenceValueUserControl : DataEditUserControl, IDataControlWithValidationDispatcher
  {
    public BindableObjectDataSourceControlValidationResultDispatchingValidator DataSourceDispatchingValidator => CurrentObjectValidationResultDispatchingValidator;

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    public override bool Validate ()
    {
      var noObjectIsValid = NoObject.Validate();
      var baseObjectIsValid = base.Validate();

      return noObjectIsValid && baseObjectIsValid;
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      PartnerField_Normal.MenuItemClick += MenuItemClickHandler;
      PartnerField_ReadOnly.MenuItemClick += MenuItemClickHandler;
      PartnerField_Disabled.MenuItemClick += MenuItemClickHandler;
      PartnerField_NoAutoPostBack.MenuItemClick += MenuItemClickHandler;
      PartnerField_NoCommandNoMenu.MenuItemClick += MenuItemClickHandler;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender(e);
      SetTestOutput();
    }

    private void MenuItemClickHandler (object sender, WebMenuItemClickEventArgs e)
    {
      TestOutput.SetActionPerformed("MenuItemClick", e.Item.ItemID + "|" + e.Item.Text.ToString(), e.Command.OwnerControl.ID);
    }

    private void SetTestOutput ()
    {
      TestOutput.SetBOUINormal(PartnerField_Normal.BusinessObjectUniqueIdentifier);
      TestOutput.SetBOUINoAutoPostBack(PartnerField_NoAutoPostBack.BusinessObjectUniqueIdentifier);
    }

    private BocReferenceValueUserControlTestOutput TestOutput
    {
      get { return (BocReferenceValueUserControlTestOutput)((Layout)Page.Master).GetTestOutputControl(); }
    }

    protected void PostbackButton_OnClick (object sender, EventArgs e)
    {
      // NOP
    }

    protected void ResetBusinessObjectListButton_OnClick (object sender, EventArgs e)
    {
      PartnerField_Normal.ResetBusinessObjectList();
    }

    protected void SetEmptyBusinessObjectListButton_OnClick (object sender, EventArgs e)
    {
      PartnerField_Normal.SetBusinessObjectList(Array.Empty<IBusinessObjectWithIdentity>());
    }
  }
}
