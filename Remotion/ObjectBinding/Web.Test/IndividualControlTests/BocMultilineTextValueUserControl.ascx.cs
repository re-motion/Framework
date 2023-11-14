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
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace OBWTest.IndividualControlTests
{

public class BocMultilineTextValueUserControl : BaseUserControl
{
  protected HtmlGenericControl NonVisualControls;
  protected FormGridManager FormGridManager;
  protected BocTextValue FirstNameField;
  protected BocTextValue LastNameField;
  protected BocMultilineTextValue CVField;
  protected Label CVFieldValueLabel;
  protected BocMultilineTextValue ReadOnlyCVField;
  protected Label ReadOnlyCVFieldValueLabel;
  protected BocMultilineTextValue UnboundCVField;
  protected Label UnboundCVFieldValueLabel;
  protected BocMultilineTextValue UnboundReadOnlyCVField;
  protected Label UnboundReadOnlyCVFieldValueLabel;
  protected BocMultilineTextValue DisabledCVField;
  protected Label DisabledCVFieldValueLabel;
  protected BocMultilineTextValue DisabledReadOnlyCVField;
  protected Label DisabledReadOnlyCVFieldValueLabel;
  protected BocMultilineTextValue DisabledUnboundCVField;
  protected Label DisabledUnboundCVFieldValueLabel;
  protected BocMultilineTextValue DisabledUnboundReadOnlyCVField;
  protected Label DisabledUnboundReadOnlyCVFieldValueLabel;
  protected Label CVFieldTextChangedLabel;
  protected WebButton CVTestSetNullButton;
  protected WebButton CVTestSetNewValueButton;
  protected WebButton ReadOnlyCVTestSetNullButton;
  protected WebButton ReadOnlyCVTestSetNewValueButton;
  protected HtmlTable FormGrid;
  protected BindableObjectDataSourceControl CurrentObject;
  protected BindableObjectDataSourceControlValidationResultDispatchingValidator CurrentObjectValidationResultDispatchingValidator;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    this.CVField.TextChanged += new EventHandler(this.CVField_TextChanged);
    this.CVTestSetNullButton.Click += new EventHandler(this.CVTestSetNullButton_Click);
    this.CVTestSetNewValueButton.Click += new EventHandler(this.CVTestSetNewValueButton_Click);
    this.ReadOnlyCVTestSetNullButton.Click += new EventHandler(this.ReadOnlyCVTestSetNullButton_Click);
    this.ReadOnlyCVTestSetNewValueButton.Click += new EventHandler(this.ReadOnlyCVTestSetNewValueButton_Click);
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

    //UnboundCVField.LoadUnboundValue (person.CV, IsPostBack);
    UnboundReadOnlyCVField.LoadUnboundValue(person.CV, IsPostBack);
    DisabledUnboundCVField.LoadUnboundValue(person.CV, IsPostBack);
    DisabledUnboundReadOnlyCVField.LoadUnboundValue(person.CV, IsPostBack);

    if (!IsPostBack)
    {
      if (Page is ISmartNavigablePage)
        ((ISmartNavigablePage)Page).SetFocus(CVField);
    }
  }

  override protected void OnPreRender (EventArgs e)
  {
    base.OnPreRender(e);

    SetDebugLabel(CVField, CVFieldValueLabel);
    SetDebugLabel(ReadOnlyCVField, ReadOnlyCVFieldValueLabel);
    SetDebugLabel(UnboundCVField, UnboundCVFieldValueLabel);
    SetDebugLabel(UnboundReadOnlyCVField, UnboundReadOnlyCVFieldValueLabel);
    SetDebugLabel(DisabledCVField, DisabledCVFieldValueLabel);
    SetDebugLabel(DisabledReadOnlyCVField, DisabledReadOnlyCVFieldValueLabel);
    SetDebugLabel(DisabledUnboundCVField, DisabledUnboundCVFieldValueLabel);
    SetDebugLabel(DisabledUnboundReadOnlyCVField, DisabledUnboundReadOnlyCVFieldValueLabel);
  }

  private void SetDebugLabel (BocMultilineTextValue control, Label label)
  {
   if (control.Value != null)
   {
     label.Text = string.Join("<br />", control.Value.Select(HttpUtility.HtmlEncode));
   }
   else
      label.Text = "not set";
  }

  private void CVTestSetNullButton_Click (object sender, EventArgs e)
  {
    CVField.Value = null;
  }

  private void CVTestSetNewValueButton_Click (object sender, EventArgs e)
  {
    CVField.Value = new string[] {"Foo", "Bar"};
  }

  private void ReadOnlyCVTestSetNullButton_Click (object sender, EventArgs e)
  {
    ReadOnlyCVField.Value = null;
  }

  private void ReadOnlyCVTestSetNewValueButton_Click (object sender, EventArgs e)
  {
    ReadOnlyCVField.Value = new string[] {"Foo", "Bar"};
  }

  private void CVField_TextChanged (object sender, EventArgs e)
  {
    if (CVField.Value != null)
      CVFieldTextChangedLabel.Text = string.Join("<br />", CVField.Value);
    else
      CVFieldTextChangedLabel.Text = "not set";
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
