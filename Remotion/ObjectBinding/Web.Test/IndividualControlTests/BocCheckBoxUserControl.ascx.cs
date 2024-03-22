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

public class BocCheckBoxUserControl : BaseUserControl
{
  protected HtmlGenericControl NonVisualControls;
  protected FormGridManager FormGridManager;
  protected BocTextValue FirstNameField;
  protected BocTextValue LastNameField;
  protected BocCheckBox DeceasedField;
  protected Label DeceasedFieldValueLabel;
  protected BocCheckBox ReadOnlyDeceasedField;
  protected Label ReadOnlyDeceasedFieldValueLabel;
  protected BocCheckBox UnboundDeceasedField;
  protected Label UnboundDeceasedFieldValueLabel;
  protected BocCheckBox UnboundReadOnlyDeceasedField;
  protected Label UnboundReadOnlyDeceasedFieldValueLabel;
  protected BocCheckBox DisabledDeceasedField;
  protected Label DisabledDeceasedFieldValueLabel;
  protected BocCheckBox DisabledReadOnlyDeceasedField;
  protected Label DisabledReadOnlyDeceasedFieldValueLabel;
  protected BocCheckBox DisabledUnboundDeceasedField;
  protected Label DisabledUnboundDeceasedFieldValueLabel;
  protected BocCheckBox DisabledUnboundReadOnlyDeceasedField;
  protected Label DisabledUnboundReadOnlyDeceasedFieldValueLabel;
  protected Label DeceasedFieldCheckedChangedLabel;
  protected Label UnboundDeceasedFieldCheckedChangedLabel;
  protected WebButton DeceasedTestSetNullButton;
  protected WebButton DeceasedTestToggleValueButton;
  protected WebButton ReadOnlyDeceasedTestSetNullButton;
  protected WebButton ReadOnlyDeceasedTestToggleValueButton;
  protected HtmlTable FormGrid;
  protected BindableObjectDataSourceControl CurrentObject;
  protected BindableObjectDataSourceControlValidationResultDispatchingValidator CurrentObjectValidationResultDispatchingValidator;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    this.DeceasedField.CheckedChanged += new EventHandler(this.DeceasedField_CheckedChanged);
    this.UnboundDeceasedField.CheckedChanged += UnboundDeceasedField_CheckedChanged;
    this.DeceasedTestSetNullButton.Click += new EventHandler(this.DeceasedTestSetNullButton_Click);
    this.DeceasedTestToggleValueButton.Click += new EventHandler(this.DeceasedTestToggleValueButton_Click);
    this.ReadOnlyDeceasedTestSetNullButton.Click += new EventHandler(this.ReadOnlyDeceasedTestSetNullButton_Click);
    this.ReadOnlyDeceasedTestToggleValueButton.Click += new EventHandler(this.ReadOnlyDeceasedTestToggleValueButton_Click);
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

    //UnboundDeceasedField.LoadUnboundValue (person.Deceased, IsPostBack);
    UnboundReadOnlyDeceasedField.LoadUnboundValue(person.Deceased, IsPostBack);
    DisabledUnboundDeceasedField.LoadUnboundValue(person.Deceased, IsPostBack);
    DisabledUnboundReadOnlyDeceasedField.LoadUnboundValue(person.Deceased, IsPostBack);

    if (! IsPostBack)
    {
      if (Page is ISmartNavigablePage)
        ((ISmartNavigablePage)Page).SetFocus(DeceasedField);
    }
  }

  override protected void OnPreRender (EventArgs e)
  {
    base.OnPreRender(e);

    SetDebugLabel(DeceasedField, DeceasedFieldValueLabel);
    SetDebugLabel(ReadOnlyDeceasedField, ReadOnlyDeceasedFieldValueLabel);
    SetDebugLabel(UnboundDeceasedField, UnboundDeceasedFieldValueLabel);
    SetDebugLabel(UnboundReadOnlyDeceasedField, UnboundReadOnlyDeceasedFieldValueLabel);
    SetDebugLabel(DisabledDeceasedField, DisabledDeceasedFieldValueLabel);
    SetDebugLabel(DisabledReadOnlyDeceasedField, DisabledReadOnlyDeceasedFieldValueLabel);
    SetDebugLabel(DisabledUnboundDeceasedField, DisabledUnboundDeceasedFieldValueLabel);
    SetDebugLabel(DisabledUnboundReadOnlyDeceasedField, DisabledUnboundReadOnlyDeceasedFieldValueLabel);
  }

  private void SetDebugLabel (IBusinessObjectBoundWebControl control, Label label)
  {
   if (control.Value != null)
      label.Text = control.Value.ToString();
    else
      label.Text = "not set";
  }

  private void DeceasedTestSetNullButton_Click (object sender, EventArgs e)
  {
    DeceasedField.Value = null;
  }

  private void DeceasedTestToggleValueButton_Click (object sender, EventArgs e)
  {
    if (DeceasedField.Value != null)
      DeceasedField.Value = ! (bool)DeceasedField.Value;
    else
      DeceasedField.Value = false;
  }

  private void ReadOnlyDeceasedTestSetNullButton_Click (object sender, EventArgs e)
  {
    ReadOnlyDeceasedField.Value = null;
  }

  private void ReadOnlyDeceasedTestToggleValueButton_Click (object sender, EventArgs e)
  {
    if (ReadOnlyDeceasedField.Value != null)
      ReadOnlyDeceasedField.Value = ! (bool)ReadOnlyDeceasedField.Value;
    else
      ReadOnlyDeceasedField.Value = false;
  }

  private void DeceasedField_CheckedChanged (object sender, EventArgs e)
  {
    if (DeceasedField.Value != null)
      DeceasedFieldCheckedChangedLabel.Text = DeceasedField.Value.ToString();
    else
      DeceasedFieldCheckedChangedLabel.Text = "not set";
  }

  private void UnboundDeceasedField_CheckedChanged (object sender, EventArgs e)
  {
    if (UnboundDeceasedField.Value != null)
      UnboundDeceasedFieldCheckedChangedLabel.Text = UnboundDeceasedField.Value.ToString();
    else
      UnboundDeceasedFieldCheckedChangedLabel.Text = "not set";
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
