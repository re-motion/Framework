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
using Remotion.Utilities;
using Remotion.Web.UI.Controls;


namespace OBWTest
{

public class PersonDetailsForm : SingleBocTestWxeBasePage

{
  protected HtmlTable FormGrid;
  protected Button SaveButton;
  protected FormGridManager FormGridManager;
  protected BocTextValue FirstNameField;
  protected BocEnumValue GenderField;
  protected BocReferenceValue PartnerField;
  protected BocDateTimeValue BirthdayField;
  protected BocTextValue LastNameField;
  protected BindableObjectDataSourceControl CurrentObject;
  protected HtmlHeadContents HtmlHeadContents;
  protected BocBooleanValue DeceasedField;
  protected BocMultilineTextValue CVField;
  protected BocList JobList;
  protected BusinessObjectReferenceDataSourceControl PartnerDataSource;
  protected BocTextValue ParterFirstNameField;
  protected Button PostBackButton;

	private void Page_Load(object sender, EventArgs e)
	{
	  var  personID = new Guid (((ViewPersonDetailsWxeFunction)CurrentFunction).ID);

    Person person = Person.GetObject (personID);
#pragma warning disable 219 // unused variable
    Person partner;
    if (person != null)
      partner = person.Partner;
#pragma warning restore 219

    CurrentObject.BusinessObject = (IBusinessObjectWithIdentity) person;
    CurrentObject.LoadValues (IsPostBack);

    if (! IsPostBack)
    {
      IBusinessObjectWithIdentity[] objects = (IBusinessObjectWithIdentity[]) ArrayUtility.Convert (
          XmlReflectionBusinessObjectStorageProvider.Current.GetObjects (typeof (Person)), typeof (IBusinessObjectWithIdentity));
      PartnerField.SetBusinessObjectList (objects);
    }
	}

	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);

    if (!IsPostBack)
      XmlReflectionBusinessObjectStorageProvider.Current.Reset();

    InitalizePartnerFieldMenuItems();
    InitalizeJobListMenuItems();
	}

  private void InitalizePartnerFieldMenuItems()
  {
    BocMenuItem menuItem = null;

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Open";
    menuItem.Text = "Open";
    menuItem.Category = "Object";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.Parameters = "objects";
    menuItem.Command.WxeFunctionCommand.TypeName = "OBWTest.ViewPersonsWxeFunction,OBWTest";
    PartnerField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Text = "Copy";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    PartnerField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Cut";
    menuItem.Text = "Cut";
    menuItem.Category = "Edit";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    PartnerField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = "Paste";
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    PartnerField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = "Delete";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    PartnerField.OptionsMenuItems.Add (menuItem);
  }

  private void InitalizeJobListMenuItems()
  {
    BocMenuItem menuItem = null;

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = "Paste";
    menuItem.Category = "Edit";
    menuItem.IsDisabled = true;
    menuItem.Command.Type = CommandType.Event;
    JobList.ListMenuItems.Add (menuItem);
  
    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = "Delete";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    JobList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.ExactlyOne;
    menuItem.Command.Type = CommandType.Event;
    JobList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Open";
    menuItem.Text = "Open";
    menuItem.Category = "Object";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.Parameters = "objects";
    menuItem.Command.WxeFunctionCommand.TypeName = "OBWTest.ViewPersonsWxeFunction,OBWTest";
    JobList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Text = "Copy";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    JobList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Cut";
    menuItem.Text = "Cut";
    menuItem.Category = "Edit";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    JobList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = "Paste";
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    JobList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Duplicate";
    menuItem.Text = "Duplicate";
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    JobList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = "Delete";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    JobList.OptionsMenuItems.Add (menuItem);
  }

	#region Web Form Designer generated code
	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.PartnerField.SelectionChanged += new System.EventHandler(this.PartnerField_SelectionChanged);
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void SaveButton_Click(object sender, EventArgs e)
  {
    bool isValid = FormGridManager.Validate();
    if (isValid)
    {
      CurrentObject.SaveValues (false);
      Person person = (Person) CurrentObject.BusinessObject;
      person.SaveObject();
      if (person.Partner != null)
        person.Partner.SaveObject();
    }
    XmlReflectionBusinessObjectStorageProvider.Current.Reset();
  }

  private void PartnerField_SelectionChanged(object sender, EventArgs e)
  {
    PartnerField.SaveValue (false);
    PartnerDataSource.LoadValue (false);
  }
}

}
