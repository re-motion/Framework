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
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Remotion.Collections;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Test.Shared
{
public partial class CompleteBocUserControl :
    UserControl,
    IFormGridRowProvider //  Provides new rows and rows to hide to the FormGridManager
{

  private AutoInitDictionary<HtmlTable,FormGridRowInfoCollection> _listOfFormGridRowInfos = new AutoInitDictionary<HtmlTable,FormGridRowInfoCollection>();
  private AutoInitDictionary<HtmlTable,StringCollection> _listOfHiddenRows = new AutoInitDictionary<HtmlTable,StringCollection>();

  private void Page_Load (object sender, EventArgs e)
  {
    Guid personID = new Guid(0,0,0,0,0,0,0,0,0,0,1);
    Person person = Person.GetObject(personID);
    Person partner;
    if (person == null)
    {
      person = Person.CreateObject(personID);
      person.FirstName = "Hugo";
      person.LastName = "Meier";
      person.DateOfBirth = new DateTime(1959, 4, 15);
      person.Height = 179;
      person.Income = 2000;

      partner = person.Partner = Person.CreateObject();
      partner.FirstName = "Sepp";
      partner.LastName = "Forcher";
    }
    else
    {
      partner = person.Partner;
    }

    CurrentObject.BusinessObject = (IBusinessObject)person;
    CurrentObject.LoadValues(IsPostBack);

    if (! IsPostBack)
    {
      IBusinessObjectWithIdentity[] objects = (IBusinessObjectWithIdentity[])ArrayUtility.Convert(
          XmlReflectionBusinessObjectStorageProvider.Current.GetObjects(typeof(Person)), typeof(IBusinessObjectWithIdentity));
      ReferenceField.SetBusinessObjectList(objects);
    }

  }

  override protected void OnInit (EventArgs e)
  {
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);
    base.OnInit(e);

    if (!IsPostBack)
      XmlReflectionBusinessObjectStorageProvider.Current.Reset();

    FormGridRowInfoCollection newRows = (FormGridRowInfoCollection)_listOfFormGridRowInfos[FormGrid];

    BocTextValue incomeField = new BocTextValue();
    incomeField.ID = "IncomeField";
    incomeField.DataSourceControl = CurrentObject.ID;
    incomeField.PropertyIdentifier = "Income";
    incomeField.Visible = false;
    //  A new row
    newRows.Add(new FormGridRowInfo(
        incomeField,
        FormGridRowInfo.RowType.ControlInRowWithLabel,
        BooleanField.ID,
        FormGridRowInfo.RowPosition.AfterRowWithID));

    InitalizeReferenceFieldMenuItems();
  }

  private void InitalizeReferenceFieldMenuItems ()
  {
    BocMenuItem menuItem = null;

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Open";
    menuItem.Text = WebString.CreateFromText("Open");
    menuItem.Category = "Object";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.Parameters = "objects";
    menuItem.Command.WxeFunctionCommand.TypeName = "OBWTest.ViewPersonsWxeFunction,OBWTest";
    ReferenceField.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Text = WebString.CreateFromText("Copy");
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    ReferenceField.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Cut";
    menuItem.Text = WebString.CreateFromText("Cut");
    menuItem.Category = "Edit";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    ReferenceField.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = WebString.CreateFromText("Paste");
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    ReferenceField.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = WebString.CreateFromText("Delete");
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    ReferenceField.OptionsMenuItems.Add(menuItem);
  }


  private void SaveButton_Click (object sender, EventArgs e)
  {
    bool isValid = FormGridManager.Validate();
    if (isValid)
    {
      CurrentObject.SaveValues(false);
      Person person = (Person)CurrentObject.BusinessObject;
      person.SaveObject();
    }
  }

  public virtual StringCollection GetHiddenRows (HtmlTable table)
  {
    return (StringCollection)_listOfHiddenRows[table];
  }

  public virtual FormGridRowInfoCollection GetAdditionalRows (HtmlTable table)
  {
    return (FormGridRowInfoCollection)_listOfFormGridRowInfos[table];
  }
}
}
