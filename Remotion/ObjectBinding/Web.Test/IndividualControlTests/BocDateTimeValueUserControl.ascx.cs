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
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace OBWTest.IndividualControlTests
{

public class BocDateTimeValueUserControl : BaseUserControl
{
  protected HtmlGenericControl NonVisualControls;
  protected FormGridManager FormGridManager;
  protected BocTextValue FirstNameField;
  protected BocTextValue LastNameField;
  protected BocDateTimeValue BirthdayField;
  protected Label BirthdayFieldValueLabel;
  protected BocDateTimeValue ReadOnlyBirthdayField;
  protected Label ReadOnlyBirthdayFieldValueLabel;
  protected BocDateTimeValue UnboundBirthdayField;
  protected Label UnboundBirthdayFieldValueLabel;
  protected BocDateTimeValue UnboundRequiredBirthdayField;
  protected Label UnboundRequiredBirthdayFieldValueLabel;
  protected BocDateTimeValue UnboundReadOnlyBirthdayField;
  protected Label UnboundReadOnlyBirthdayFieldValueLabel;
  protected BocDateTimeValue DateOfDeathField;
  protected Label DateOfDeathFieldValueLabel;
  protected BocDateTimeValue ReadOnlyDateOfDeathField;
  protected Label ReadOnlyDateOfDeathFieldValueLabel;
  protected BocDateTimeValue UnboundDateOfDeathField;
  protected Label UnboundDateOfDeathFieldValueLabel;
  protected BocDateTimeValue UnboundReadOnlyDateOfDeathField;
  protected Label UnboundReadOnlyDateOfDeathFieldValueLabel;
  protected BocDateTimeValue DirectlySetBocDateTimeValueField;
  protected Label DirectlySetBocDateTimeValueFieldValueLabel;
  protected BocDateTimeValue ReadOnlyDirectlySetBocDateTimeValueField;
  protected Label ReadOnlyDirectlySetBocDateTimeValueFieldValueLabel;
  protected BocDateTimeValue DisabledBirthdayField;
  protected Label DisabledBirthdayFieldValueLabel;
  protected BocDateTimeValue DisabledReadOnlyBirthdayField;
  protected Label DisabledReadOnlyCitizenshipFieldValueLabel;
  protected BocDateTimeValue DisabledReadOnlyCitizenshipField;
  protected Label ReadOnlyCitizenshipFieldValueLabel;
  protected BocDateTimeValue ReadOnlyCitizenshipField;
  protected Label CitizenshipFieldValueLabel;
  protected BocDateTimeValue CitizenshipField;
  protected Label UnboundCitizenshipFieldValueLabel;
  protected BocDateTimeValue UnboundCitizenshipField;
  protected Label DisabledReadOnlyBirthdayFieldValueLabel;
  protected BocDateTimeValue DisabledUnboundBirthdayField;
  protected Label DisabledUnboundBirthdayFieldValueLabel;
  protected BocDateTimeValue DisabledUnboundReadOnlyBirthdayField;
  protected Label DisabledUnboundReadOnlyBirthdayFieldValueLabel;
  protected WebButton BirthdayTestSetNullButton;
  protected WebButton BirthdayTestSetNewValueButton;
  protected Label BirthdayFieldDateTimeChangedLabel;
  protected WebButton ReadOnlyBirthdayTestSetNullButton;
  protected WebButton ReadOnlyBirthdayTestSetNewValueButton;
  protected HtmlTable FormGrid;
  protected BindableObjectDataSourceControl CurrentObject;
  protected BindableObjectDataSourceControlValidationResultDispatchingValidator CurrentObjectValidationResultDispatchingValidator;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    this.BirthdayField.DateTimeChanged += new EventHandler(this.BirthdayField_DateTimeChanged);
    this.BirthdayTestSetNullButton.Click += new EventHandler(this.BirthdayTestSetNullButton_Click);
    this.BirthdayTestSetNewValueButton.Click += new EventHandler(this.BirthdayTestSetNewValueButton_Click);
    this.ReadOnlyBirthdayTestSetNullButton.Click += new EventHandler(this.ReadOnlyBirthdayTestSetNullButton_Click);
    this.ReadOnlyBirthdayTestSetNewValueButton.Click += new EventHandler(this.ReadOnlyBirthdayTestSetNewValueButton_Click);
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

  public override BindableObjectDataSourceControlValidationResultDispatchingValidator DataSourceValidationResultDispatchingValidator
  {
    get { return CurrentObjectValidationResultDispatchingValidator; }
  }

  override protected void OnLoad (EventArgs e)
  {
    base.OnLoad(e);

    Person person = (Person)CurrentObject.BusinessObject;

    //UnboundBirthdayField.LoadUnboundValue (person.DateOFBirth, IsPostBack);
    UnboundReadOnlyBirthdayField.LoadUnboundValue(person.DateOfBirth, IsPostBack);

    UnboundDateOfDeathField.LoadUnboundValue(person.DateOfDeath, IsPostBack);
    UnboundReadOnlyDateOfDeathField.LoadUnboundValue(person.DateOfDeath, IsPostBack);

    DisabledUnboundBirthdayField.LoadUnboundValue(person.DateOfBirth, IsPostBack);
    DisabledUnboundReadOnlyBirthdayField.LoadUnboundValue(person.DateOfBirth, IsPostBack);

    DirectlySetBocDateTimeValueField.LoadUnboundValue(DateTime.Now, IsPostBack);
    ReadOnlyDirectlySetBocDateTimeValueField.LoadUnboundValue(DateTime.Now, IsPostBack);

#if NETFRAMEWORK
    UnboundCitizenshipField.LoadUnboundValue(DateTime.Today, IsPostBack);
#else
    UnboundCitizenshipField.LoadUnboundValue(DateOnly.FromDateTime(DateTime.Today), IsPostBack);
#endif

    if (!IsPostBack)
    {
      if (Page is ISmartNavigablePage)
        ((ISmartNavigablePage)Page).SetFocus(BirthdayField);
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

    SetDebugLabel(BirthdayField, BirthdayFieldValueLabel);
    SetDebugLabel(ReadOnlyBirthdayField, ReadOnlyBirthdayFieldValueLabel);
    SetDebugLabel(UnboundBirthdayField, UnboundBirthdayFieldValueLabel);
    SetDebugLabel(UnboundReadOnlyBirthdayField, UnboundReadOnlyBirthdayFieldValueLabel);
    SetDebugLabel(DateOfDeathField, DateOfDeathFieldValueLabel);
    SetDebugLabel(ReadOnlyDateOfDeathField, ReadOnlyDateOfDeathFieldValueLabel);
    SetDebugLabel(UnboundDateOfDeathField, UnboundDateOfDeathFieldValueLabel);
    SetDebugLabel(UnboundReadOnlyDateOfDeathField, UnboundReadOnlyDateOfDeathFieldValueLabel);
    SetDebugLabel(DirectlySetBocDateTimeValueField, DirectlySetBocDateTimeValueFieldValueLabel);
    SetDebugLabel(ReadOnlyDirectlySetBocDateTimeValueField, ReadOnlyDirectlySetBocDateTimeValueFieldValueLabel);
    SetDebugLabel(DisabledBirthdayField, DisabledBirthdayFieldValueLabel);
    SetDebugLabel(DisabledReadOnlyBirthdayField, DisabledReadOnlyBirthdayFieldValueLabel);
    SetDebugLabel(DisabledUnboundBirthdayField, DisabledUnboundBirthdayFieldValueLabel);
    SetDebugLabel(DisabledUnboundReadOnlyBirthdayField, DisabledUnboundReadOnlyBirthdayFieldValueLabel);
    SetDebugLabel(CitizenshipField, CitizenshipFieldValueLabel);
    SetDebugLabel(ReadOnlyCitizenshipField, ReadOnlyCitizenshipFieldValueLabel);
    SetDebugLabel(DisabledReadOnlyCitizenshipField, DisabledReadOnlyCitizenshipFieldValueLabel);
    SetDebugLabel(UnboundCitizenshipField, UnboundCitizenshipFieldValueLabel);
  }

  private void SetDebugLabel (IBusinessObjectBoundWebControl control, Label label)
  {
   if (control.Value != null)
      label.Text = control.Value.ToString();
    else
      label.Text = "not set";
  }

  private void BirthdayTestSetNullButton_Click (object sender, EventArgs e)
  {
    BirthdayField.Value = null;
  }

  private void BirthdayTestSetNewValueButton_Click (object sender, EventArgs e)
  {
    BirthdayField.Value = new DateTime(1950, 1, 1);
  }

  private void ReadOnlyBirthdayTestSetNullButton_Click (object sender, EventArgs e)
  {
    ReadOnlyBirthdayField.Value = null;
  }

  private void ReadOnlyBirthdayTestSetNewValueButton_Click (object sender, EventArgs e)
  {
    ReadOnlyBirthdayField.Value = new DateTime(1950, 1, 1);;
  }

  private void BirthdayField_DateTimeChanged (object sender, EventArgs e)
  {
    if (BirthdayField.Value != null)
      BirthdayFieldDateTimeChangedLabel.Text = BirthdayField.Value.ToString();
    else
      BirthdayFieldDateTimeChangedLabel.Text = "not set";
  }

	#region Web Form Designer generated code
	override protected void OnInit (EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);
	}

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
