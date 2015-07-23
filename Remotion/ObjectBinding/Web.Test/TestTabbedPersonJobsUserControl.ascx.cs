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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace OBWTest
{
public class TestTabbedPersonJobsUserControl : 
    DataEditUserControl, IControl, IFormGridRowProvider
{
  protected BocList ListField;
  protected FormGridManager FormGridManager;
  protected HtmlTable FormGrid;
  protected BocMultilineTextValue MultilineTextField;

  protected BindableObjectDataSourceControl CurrentObject;
  private AutoInitDictionary<HtmlTable,FormGridRowInfoCollection> _listOfFormGridRowInfos = new AutoInitDictionary<HtmlTable,FormGridRowInfoCollection>();
  private AutoInitDictionary<HtmlTable,StringCollection> _listOfHiddenRows = new AutoInitDictionary<HtmlTable,StringCollection>();
  private Control _incomeField;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad (e);
    _incomeField.Visible = false;
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

  public virtual StringCollection GetHiddenRows (HtmlTable table)
  {
    return (StringCollection) _listOfHiddenRows[table];
  }

  public virtual FormGridRowInfoCollection GetAdditionalRows (HtmlTable table)
  {
    return (FormGridRowInfoCollection) _listOfFormGridRowInfos[table];
  }

	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);

    PrepareAdditonalRows ();
    InitalizeListFieldMenuItems ();
    InitializeListFieldViews ();
  }
	
  private void InitalizeListFieldMenuItems ()
  {
    BocMenuItem menuItem = null;

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = "Paste";
    menuItem.Category = "Edit";
    menuItem.IsDisabled = true;
    menuItem.Command.Type = CommandType.Event;
    ListField.ListMenuItems.Add (menuItem);
  
    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = "Delete";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    ListField.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.ExactlyOne;
    menuItem.Command.Type = CommandType.Event;
    ListField.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Open";
    menuItem.Text = "Open";
    menuItem.Category = "Object";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.Parameters = "objects";
    menuItem.Command.WxeFunctionCommand.TypeName = "OBWTest.ViewPersonsWxeFunction,OBWTest";
    ListField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Text = "Copy";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    ListField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Cut";
    menuItem.Text = "Cut";
    menuItem.Category = "Edit";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    ListField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = "Paste";
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    ListField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Duplicate";
    menuItem.Text = "Duplicate";
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    ListField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = "Delete";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    ListField.OptionsMenuItems.Add (menuItem);
  }

  private void InitializeListFieldViews ()
  {
    IBusinessObjectProperty endDate = ListField.Property.ReferenceClass.GetPropertyDefinition ("EndDate");
    
    BocSimpleColumnDefinition endDateColumnDefinition = new BocSimpleColumnDefinition();
    endDateColumnDefinition.ColumnTitle = "EndDate";
    endDateColumnDefinition.SetPropertyPath (BusinessObjectPropertyPath.CreateStatic (new[] { endDate }));

    BocListView emptyView = new BocListView();
    emptyView.Title = "Empty";
    emptyView.ColumnDefinitions.AddRange (new BocColumnDefinition[] {});

    BocListView datesView = new BocListView();
    datesView.Title = "Dates";
    datesView.ColumnDefinitions.AddRange (new BocColumnDefinition[] {endDateColumnDefinition});

    ListField.AvailableViews.AddRange (new BocListView[] {emptyView, datesView});
  }

  private void PrepareAdditonalRows ()
  {
    FormGridRowInfoCollection newRows = (FormGridRowInfoCollection)_listOfFormGridRowInfos[FormGrid];

    BocTextValue incomeField = new BocTextValue();
    incomeField.ID = "IncomeField";
    incomeField.DataSource = CurrentObject;
    incomeField.PropertyIdentifier = "Income";
    _incomeField = incomeField;

    //  A new row
    newRows.Add (new FormGridRowInfo(
        incomeField, 
        FormGridRowInfo.RowType.ControlInRowWithLabel, 
        MultilineTextField.ID, 
        FormGridRowInfo.RowPosition.AfterRowWithID));

  }

	#region Web Form Designer generated code

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
