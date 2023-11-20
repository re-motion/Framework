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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace OBWTest.IndividualControlTests
{

[WebMultiLingualResources("OBWTest.Globalization.IndividualControlTests.BocAutoCompleteReferenceValueUserControl")]
public class BocAutoCompleteReferenceValueUserControl : BaseUserControl
{
  protected HtmlGenericControl NonVisualControls;
  protected FormGridManager FormGridManager;
  protected BocTextValue FirstNameField;
  protected BocTextValue LastNameField;
  protected BocAutoCompleteReferenceValue PartnerField;
  protected Label PartnerFieldValueLabel;
  protected BocAutoCompleteReferenceValue ReadOnlyPartnerField;
  protected Label ReadOnlyPartnerFieldValueLabel;
  protected BocAutoCompleteReferenceValue UnboundPartnerField;
  protected Label UnboundPartnerFieldValueLabel;
  protected BocAutoCompleteReferenceValue UnboundReadOnlyPartnerField;
  protected Label UnboundReadOnlyPartnerFieldValueLabel;
  protected BocAutoCompleteReferenceValue DisabledPartnerField;
  protected Label DisabledPartnerFieldValueLabel;
  protected BocAutoCompleteReferenceValue DisabledReadOnlyPartnerField;
  protected Label DisabledReadOnlyPartnerFieldValueLabel;
  protected BocAutoCompleteReferenceValue DisabledUnboundPartnerField;
  protected Label DisabledUnboundPartnerFieldValueLabel;
  protected BocAutoCompleteReferenceValue DisabledUnboundReadOnlyPartnerField;
  protected Label DisabledUnboundReadOnlyPartnerFieldValueLabel;
  protected BocAutoCompleteReferenceValue FatherField;
  protected Label FatherFieldValueLabel;
  protected Label PartnerFieldSelectionChangedLabel;
  protected Label PartnerFieldMenuClickEventArgsLabel;
  protected WebButton PartnerTestSetNullButton;
  protected WebButton PartnerTestSetNewItemButton;
  protected WebButton ReadOnlyPartnerTestSetNullButton;
  protected WebButton ReadOnlyPartnerTestSetNewItemButton;
  protected HtmlTable FormGrid;
  protected Label PartnerCommandClickLabel;
  protected BindableObjectDataSourceControl CurrentObject;
  protected BindableObjectDataSourceControlValidationResultDispatchingValidator CurrentObjectValidationResultDispatchingValidator;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    PartnerField.MenuItemClick += PartnerField_MenuItemClick;
    PartnerField.SelectionChanged += PartnerField_SelectionChanged;
    PartnerTestSetNullButton.Click += PartnerTestSetNullButton_Click;
    PartnerTestSetNewItemButton.Click += PartnerTestSetNewItemButton_Click;
    ReadOnlyPartnerTestSetNullButton.Click += ReadOnlyPartnerTestSetNullButton_Click;
    ReadOnlyPartnerTestSetNewItemButton.Click += ReadOnlyPartnerTestSetNewItemButton_Click;
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

  public override BindableObjectDataSourceControlValidationResultDispatchingValidator DataSourceValidationResultDispatchingValidator
  {
    get { return CurrentObjectValidationResultDispatchingValidator; }
  }

  override protected void OnInit (EventArgs e)
  {
    InitializeComponent();

    base.OnInit(e);

    WebMenuItem menuItem = new WebMenuItem();
    menuItem.ItemID = "webmenuitem";
    menuItem.Text = WebString.CreateFromText("webmenuitem");
    PartnerField.OptionsMenuItems.Add(menuItem);

    InitalizeReferenceValueMenuItems(PartnerField);
    InitalizeReferenceValueMenuItems(ReadOnlyPartnerField);
    InitalizeReferenceValueMenuItems(UnboundPartnerField);
    InitalizeReferenceValueMenuItems(UnboundReadOnlyPartnerField);
    InitalizeReferenceValueMenuItems(DisabledPartnerField);
    InitalizeReferenceValueMenuItems(DisabledReadOnlyPartnerField);
    InitalizeReferenceValueMenuItems(DisabledUnboundPartnerField);
  }

  private void InitalizeReferenceValueMenuItems (BocAutoCompleteReferenceValue referenceValue)
  {
    BocMenuItem menuItem;

    menuItem = new BocMenuItem();
    menuItem.Text = WebString.CreateFromText("Invisible Item");
    menuItem.IsVisible = false;
    referenceValue.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Open";
    menuItem.Text = WebString.CreateFromText("Open");
    menuItem.Category = "Object";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.Parameters = "objects";
    menuItem.Command.WxeFunctionCommand.MappingID = "ViewPersons";
    referenceValue.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Text = WebString.CreateFromText("Copy");
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "~/Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    referenceValue.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Cut";
    menuItem.Text = WebString.CreateFromText("Cut");
    menuItem.Category = "Edit";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    referenceValue.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = WebString.CreateFromText("Paste");
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    referenceValue.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = WebString.CreateFromText("Delete");
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "~/Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "~/Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    referenceValue.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = WebString.CreateFromText("Invisible Item");
    menuItem.IsVisible = false;
    referenceValue.OptionsMenuItems.Add(menuItem);

    if (!string.IsNullOrEmpty(referenceValue.ControlServicePath))
    {
      referenceValue.OptionsMenuItems.Add(WebMenuItem.GetSeparator());

      menuItem = new BocMenuItem();
      menuItem.ItemID = "FilterByService";
      menuItem.Text = WebString.CreateFromText("Should be filtered");
      menuItem.IsVisible = true;
      referenceValue.OptionsMenuItems.Add(menuItem);

      referenceValue.OptionsMenuItems.Add(WebMenuItem.GetSeparator());

      menuItem = new BocMenuItem();
      menuItem.ItemID = "DisabledByService";
      menuItem.Text = WebString.CreateFromText("Should be disabled");
      menuItem.IsDisabled = false;
      referenceValue.OptionsMenuItems.Add(menuItem);
    }
  }

  override protected void OnLoad (EventArgs e)
  {
    base.OnLoad(e);

    Person person = (Person)CurrentObject.BusinessObject;

    //UnboundPartnerField.LoadUnboundValue (person.Partner, IsPostBack);
    UnboundReadOnlyPartnerField.LoadUnboundValue((IBusinessObjectWithIdentity)person.Partner, IsPostBack);
    DisabledUnboundPartnerField.LoadUnboundValue((IBusinessObjectWithIdentity)person.Partner, IsPostBack);
    DisabledUnboundReadOnlyPartnerField.LoadUnboundValue((IBusinessObjectWithIdentity)person.Partner, IsPostBack);

    if (!IsPostBack)
    {
      if (Page is ISmartNavigablePage)
        ((ISmartNavigablePage)Page).SetFocus(PartnerField);
    }
  }

  public override bool Validate ()
  {
    bool isValid = base.Validate();
    isValid &= FormGridManager.Validate();
    return isValid;
  }

  override protected void OnPreRender (EventArgs e)
  {
    base.OnPreRender(e);

    var delay = ((TestFunction)((IWxePage)Page).CurrentFunction).Delay;
    if (delay.HasValue)
    {
      var args = (delay.Value / 2).ToString();
      PartnerField.ControlServiceArguments = args;
      UnboundPartnerField.ControlServiceArguments = args;
    }

    SetDebugLabel(PartnerField, PartnerFieldValueLabel);
    SetDebugLabel(ReadOnlyPartnerField, ReadOnlyPartnerFieldValueLabel);
    SetDebugLabel(UnboundPartnerField, UnboundPartnerFieldValueLabel);
    SetDebugLabel(UnboundReadOnlyPartnerField, UnboundReadOnlyPartnerFieldValueLabel);
    SetDebugLabel(DisabledPartnerField, DisabledPartnerFieldValueLabel);
    SetDebugLabel(DisabledReadOnlyPartnerField, DisabledReadOnlyPartnerFieldValueLabel);
    SetDebugLabel(DisabledUnboundPartnerField, DisabledUnboundPartnerFieldValueLabel);
    SetDebugLabel(DisabledUnboundReadOnlyPartnerField, DisabledUnboundReadOnlyPartnerFieldValueLabel);
    SetDebugLabel(FatherField, FatherFieldValueLabel);
  }

  private void SetDebugLabel (IBusinessObjectBoundWebControl control, Label label)
  {
   if (control.Value != null)
      label.Text = control.Value.ToString();
    else
      label.Text = "not set";
  }

  protected void Control_SelectionChanged (object sender, EventArgs e)
  {
    ((SmartPage)Page).PrepareValidation();
    if (!((IValidatableControl)sender).Validate())
      ((ISmartNavigablePage)Page).SetFocus(((IFocusableControl)sender));
  }

  private void PartnerTestSetNullButton_Click (object sender, EventArgs e)
  {
    PartnerField.Value = null;
  }

  private void PartnerTestSetNewItemButton_Click (object sender, EventArgs e)
  {
    Person person = Person.CreateObject(Guid.NewGuid());
    person.LastName = person.ID.ToByteArray()[15].ToString();
    person.FirstName = "--";

    PartnerField.Value = (IBusinessObjectWithIdentity)person;
  }

  private void ReadOnlyPartnerTestSetNullButton_Click (object sender, EventArgs e)
  {
    ReadOnlyPartnerField.Value = null;
  }

  private void ReadOnlyPartnerTestSetNewItemButton_Click (object sender, EventArgs e)
  {
    Person person = Person.CreateObject(Guid.NewGuid());
    person.LastName = person.ID.ToByteArray()[15].ToString();
    person.FirstName = "--";

    ReadOnlyPartnerField.Value = (IBusinessObjectWithIdentity)person;
  }

  private void PartnerField_SelectionChanged (object sender, EventArgs e)
  {
    if (PartnerField.Value != null)
      PartnerFieldSelectionChangedLabel.Text = PartnerField.Value.ToString();
    else
      PartnerFieldSelectionChangedLabel.Text = "not set";
  }

  private void PartnerField_MenuItemClick (object sender, WebMenuItemClickEventArgs e)
  {
    PartnerFieldMenuClickEventArgsLabel.Text = e.Item.Text.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks);
  }

  #region Web Form Designer generated code
  /// <summary>
  ///		Required method for Designer support - do not modify
  ///		the contents of this method with the code editor.
  /// </summary>
  private void InitializeComponent ()
  {

  }
  #endregion
}

}
