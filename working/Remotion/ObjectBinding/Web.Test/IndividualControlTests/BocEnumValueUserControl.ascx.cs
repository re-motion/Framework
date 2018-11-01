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
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace OBWTest.IndividualControlTests
{

public class BocEnumValueUserControl : BaseUserControl
{
  protected HtmlGenericControl NonVisualControls;
  protected FormGridManager FormGridManager;
  protected BindableObjectDataSourceControl CurrentObject;
  protected BindableObjectDataSourceControl EnumObject;
  protected BocTextValue FirstNameField;
  protected BocTextValue LastNameField;
  protected BocEnumValue GenderField;
  protected Label GenderFieldValueLabel;
  protected BocEnumValue ReadOnlyGenderField;
  protected Label ReadOnlyGenderFieldValueLabel;
  protected BocEnumValue MarriageStatusField;
  protected Label MarriageStatusFieldValueLabel;
  protected BocEnumValue UnboundMarriageStatusField;
  protected Label UnboundMarriageStatusFieldValueLabel;
  protected BocEnumValue UnboundReadOnlyMarriageStatusField;
  protected Label UnboundReadOnlyMarriageStatusFieldValueLabel;
  protected BocEnumValue DeceasedAsEnumField;
  protected Label DeceasedAsEnumFieldValueLabel;
  protected BocEnumValue DisabledGenderField;
  protected Label DisabledGenderFieldValueLabel;
  protected BocEnumValue DisabledReadOnlyGenderField;
  protected Label DisabledReadOnlyGenderFieldValueLabel;
  protected BocEnumValue DisabledMarriageStatusField;
  protected Label DisabledMarriageStatusFieldValueLabel;
  protected BocEnumValue DisabledUnboundMarriageStatusField;
  protected Label DisabledUnboundMarriageStatusFieldValueLabel;
  protected BocEnumValue DisabledUnboundReadOnlyMarriageStatusField;
  protected Label DisabledUnboundReadOnlyMarriageStatusFieldValueLabel;
  protected BocEnumValue InstanceEnumField;
  protected Label InstanceEnumFieldValueLabel;
  protected Label GenderFieldSelectionChangedLabel;
  protected WebButton GenderTestSetNullButton;
  protected WebButton GenderTestSetDisabledGenderButton;
  protected WebButton GenderTestSetMarriedButton;
  protected WebButton ReadOnlyGenderTestSetNullButton;
  protected WebButton ReadOnlyGenderTestSetNewItemButton;
  protected HtmlTable FormGrid;
  
  private string _instanceEnum;

  public string InstanceEnum
  {
    get { return _instanceEnum; }
    set { _instanceEnum = value; }
  }

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    this.GenderField.SelectionChanged += new EventHandler(this.GenderField_SelectionChanged);
    this.GenderTestSetNullButton.Click += new EventHandler(this.GenderTestSetNullButton_Click);
    this.GenderTestSetDisabledGenderButton.Click += new EventHandler(this.GenderTestSetDisabledGenderButton_Click);
    this.GenderTestSetMarriedButton.Click += new EventHandler(this.GenderTestSetMarriedButton_Click);
    this.ReadOnlyGenderTestSetNullButton.Click += new EventHandler(this.ReadOnlyGenderTestSetNullButton_Click);
    this.ReadOnlyGenderTestSetNewItemButton.Click += new EventHandler(this.ReadOnlyGenderTestSetFemaleButton_Click);
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

  override protected void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    Person person = (Person) CurrentObject.BusinessObject;

    GenderField.LoadUnboundValue ((Gender?)null, IsPostBack);

    UnboundMarriageStatusField.Property = (IBusinessObjectEnumerationProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("MarriageStatus");
    //UnboundMarriageStatusField.LoadUnboundValue (person.MarriageStatus, IsPostBack);
    UnboundReadOnlyMarriageStatusField.Property = (IBusinessObjectEnumerationProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("MarriageStatus");
    UnboundReadOnlyMarriageStatusField.LoadUnboundValue (person.MarriageStatus, IsPostBack);
    DisabledUnboundMarriageStatusField.Property = (IBusinessObjectEnumerationProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("MarriageStatus");
    DisabledUnboundMarriageStatusField.LoadUnboundValue (person.MarriageStatus, IsPostBack);
    DisabledUnboundReadOnlyMarriageStatusField.Property = (IBusinessObjectEnumerationProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("MarriageStatus");
    DisabledUnboundReadOnlyMarriageStatusField.LoadUnboundValue (person.MarriageStatus, IsPostBack);

    if (!IsPostBack)
    {
      if (Page is ISmartNavigablePage)
        ((ISmartNavigablePage) Page).SetFocus (MarriageStatusField);
    }

    EnumObject.BusinessObject = (IBusinessObject) ClassWithEnums.CreateObject();
  }
  public override bool Validate ()
  {
    bool isValid = base.Validate();
    isValid &= EnumObject.Validate();
    return isValid;
  }

  override protected void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    SetDebugLabel (GenderField, GenderFieldValueLabel);
    SetDebugLabel (ReadOnlyGenderField, ReadOnlyGenderFieldValueLabel);
    SetDebugLabel (MarriageStatusField, MarriageStatusFieldValueLabel);
    SetDebugLabel (UnboundMarriageStatusField, UnboundMarriageStatusFieldValueLabel);
    SetDebugLabel (UnboundReadOnlyMarriageStatusField, UnboundReadOnlyMarriageStatusFieldValueLabel);
    SetDebugLabel (DeceasedAsEnumField, DeceasedAsEnumFieldValueLabel);
    SetDebugLabel (DisabledGenderField, DisabledGenderFieldValueLabel);
    SetDebugLabel (DisabledReadOnlyGenderField, DisabledReadOnlyGenderFieldValueLabel);
    SetDebugLabel (DisabledMarriageStatusField, DisabledMarriageStatusFieldValueLabel);
    SetDebugLabel (DisabledUnboundMarriageStatusField, DisabledUnboundMarriageStatusFieldValueLabel);
    SetDebugLabel (DisabledUnboundReadOnlyMarriageStatusField, DisabledUnboundReadOnlyMarriageStatusFieldValueLabel);
    SetDebugLabel (InstanceEnumField, InstanceEnumFieldValueLabel);
  }

  private void SetDebugLabel (IBusinessObjectBoundWebControl control, Label label)
  {
   if (control.Value != null)
      label.Text = control.Value.ToString();
    else
      label.Text = "not set";
  }

  private void GenderTestSetNullButton_Click(object sender, EventArgs e)
  {
    GenderField.Value = null;
  }

  private void GenderTestSetDisabledGenderButton_Click(object sender, EventArgs e)
  {
    GenderField.Value = Gender.UnknownGender;
  }

  private void GenderTestSetMarriedButton_Click(object sender, EventArgs e)
  {
    GenderField.Value = MarriageStatus.Married;
  }

  private void ReadOnlyGenderTestSetNullButton_Click(object sender, EventArgs e)
  {
    ReadOnlyGenderField.Value = null;
  }

  private void ReadOnlyGenderTestSetFemaleButton_Click(object sender, EventArgs e)
  {
    ReadOnlyGenderField.Value = Gender.Female;
  }

  private void GenderField_SelectionChanged(object sender, EventArgs e)
  {
    if (GenderField.Value != null)
      GenderFieldSelectionChangedLabel.Text = GenderField.Value.ToString();
    else
      GenderFieldSelectionChangedLabel.Text = "not set";
  }

	#region Web Form Designer generated code
	override protected void OnInit(EventArgs e)
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
	private void InitializeComponent()
	{

  }
  #endregion
}

}
