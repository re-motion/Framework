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

public class BocTextValueUserControl : BaseUserControl
{
  protected HtmlGenericControl NonVisualControls;
  protected FormGridManager FormGridManager;
  protected BocTextValue FirstNameField;
  protected Label FirstNameFieldValueLabel;
  protected BocTextValue ReadOnlyFirstNameField;
  protected Label ReadOnlyFirstNameFieldValueLabel;
  protected BocTextValue UnboundFirstNameField;
  protected Label UnboundFirstNameFieldValueLabel;
  protected BocTextValue UnboundReadOnlyFirstNameField;
  protected Label UnboundReadOnlyFirstNameFieldValueLabel;
  protected BocTextValue IncomeField;
  protected Label Label1;
  protected BocTextValue HeightField;
  protected Label Label4;
  protected BocTextValue DateOfBirthField;
  protected Label Label2;
  protected BocTextValue DateOfDeathField;
  protected Label Label3;
  protected BocTextValue DisabledFirstNameField;
  protected Label DisabledFirstNameFieldValueLabel;
  protected BocTextValue DisabledReadOnlyFirstNameField;
  protected Label DisabledReadOnlyFirstNameFieldValueLabel;
  protected BocTextValue DisabledUnboundFirstNameField;
  protected Label DisabledUnboundFirstNameFieldValueLabel;
  protected BocTextValue DisabledUnboundReadOnlyFirstNameField;
  protected Label DisabledUnboundReadOnlyFirstNameFieldValueLabel;
  protected BocTextValue BocTextValue1;
  protected BocTextValue BocTextValue2;
  protected WebButton FirstNameTestSetNullButton;
  protected WebButton FirstNameTestSetNewValueButton;
  protected Label FirstNameFieldTextChangedLabel;
  protected WebButton ReadOnlyFirstNameTestSetNullButton;
  protected WebButton ReadOnlyFirstNameTestSetNewValueButton;
  protected HtmlTable FormGrid;
  protected BindableObjectDataSourceControl CurrentObject;
  protected BindableObjectDataSourceControlValidationResultDispatchingValidator CurrentObjectValidationResultDispatchingValidator;
  protected BocTextValue PasswordRenderMasked;
  protected BocTextValue PasswordNoRender;
  protected BocTextValue PasswordRenderMaskedReadOnly;
  protected BocTextValue PasswordNoRenderReadOnly;
  protected BocTextValue Multiline;
  protected BocTextValue MultilineReadOnly;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    this.FirstNameField.TextChanged += new EventHandler(this.FirstNameField_TextChanged);
    this.FirstNameTestSetNullButton.Click += new EventHandler(this.FirstNameTestSetNullButton_Click);
    this.FirstNameTestSetNewValueButton.Click += new EventHandler(this.FirstNameTestSetNewValueButton_Click);
    this.ReadOnlyFirstNameTestSetNullButton.Click += new EventHandler(this.ReadOnlyFirstNameTestSetNullButton_Click);
    this.ReadOnlyFirstNameTestSetNewValueButton.Click += new EventHandler(this.ReadOnlyFirstNameTestSetNewValueButton_Click);
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

  public override BindableObjectDataSourceControlValidationResultDispatchingValidator DataSourceValidationResultDispatchingValidator
  {
    get { return CurrentObjectValidationResultDispatchingValidator; }
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

  override protected void OnLoad (EventArgs e)
  {
    base.OnLoad(e);

    Person person = (Person)CurrentObject.BusinessObject;

    //UnboundFirstNameField.LoadUnboundValue (person.FirstName, IsPostBack);
    UnboundReadOnlyFirstNameField.LoadUnboundValue(person.FirstName, IsPostBack);
    DisabledUnboundFirstNameField.LoadUnboundValue(person.FirstName, IsPostBack);
    DisabledUnboundReadOnlyFirstNameField.LoadUnboundValue(person.FirstName, IsPostBack);
    PasswordNoRender.LoadUnboundValue("Password", IsPostBack);
    PasswordRenderMasked.LoadUnboundValue("Password", IsPostBack);
    PasswordNoRenderReadOnly.LoadUnboundValue("Password", IsPostBack);
    PasswordRenderMaskedReadOnly.LoadUnboundValue("Password", IsPostBack);
    MultilineReadOnly.LoadUnboundValue(@"line 1
line 2
line 3", IsPostBack);

    if (!IsPostBack)
    {
      if (Page is ISmartNavigablePage)
        ((ISmartNavigablePage)Page).SetFocus(FirstNameField);
    }
  }

  override protected void OnPreRender (EventArgs e)
  {
    base.OnPreRender(e);

    SetDebugLabel(FirstNameField, FirstNameFieldValueLabel);
    SetDebugLabel(ReadOnlyFirstNameField, ReadOnlyFirstNameFieldValueLabel);
    SetDebugLabel(UnboundFirstNameField, UnboundFirstNameFieldValueLabel);
    SetDebugLabel(UnboundReadOnlyFirstNameField, UnboundReadOnlyFirstNameFieldValueLabel);
    SetDebugLabel(DisabledFirstNameField, DisabledFirstNameFieldValueLabel);
    SetDebugLabel(DisabledReadOnlyFirstNameField, DisabledReadOnlyFirstNameFieldValueLabel);
    SetDebugLabel(DisabledUnboundFirstNameField, DisabledUnboundFirstNameFieldValueLabel);
    SetDebugLabel(DisabledUnboundReadOnlyFirstNameField, DisabledUnboundReadOnlyFirstNameFieldValueLabel);
  }

  private void SetDebugLabel (IBusinessObjectBoundWebControl control, Label label)
  {
   if (control.Value != null)
      label.Text = control.Value.ToString();
    else
      label.Text = "not set";
  }

  private void FirstNameTestSetNullButton_Click (object sender, EventArgs e)
  {
    FirstNameField.Value = null;
  }

  private void FirstNameTestSetNewValueButton_Click (object sender, EventArgs e)
  {
    FirstNameField.Value = "Foo Bar";
  }

  private void ReadOnlyFirstNameTestSetNullButton_Click (object sender, EventArgs e)
  {
    ReadOnlyFirstNameField.Value = null;
  }

  private void ReadOnlyFirstNameTestSetNewValueButton_Click (object sender, EventArgs e)
  {
    ReadOnlyFirstNameField.Value = "Foo Bar";
  }

  private void FirstNameField_TextChanged (object sender, EventArgs e)
  {
    if (FirstNameField.Value != null)
      FirstNameFieldTextChangedLabel.Text = FirstNameField.Value.ToString();
    else
      FirstNameFieldTextChangedLabel.Text = "not set";
  }
}

}
