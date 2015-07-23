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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.Collections;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace OBWTest
{
public class TestTabbedPersonDetailsUserControl : 
    DataEditUserControl, IControl, IFormGridRowProvider
{
  protected BocTextValue FirstNameField;
  protected BocTextValue LastNameField;
  protected FormGridManager FormGridManager;
  protected BindableObjectDataSourceControl CurrentObject;
  protected BocDateTimeValue DateOfBirthField;
  protected BocReferenceValue PartnerField;
  protected BocBooleanValue DeceasedField;
  protected BocDateTimeValue DateOfDeathField;
  protected BocEnumValue MarriageStatusField;
  protected HtmlTable FormGrid;
  protected PlaceHolder ExtraFormGridPlaceHolder;
  private AutoInitDictionary<HtmlTable,FormGridRowInfoCollection> _listOfFormGridRowInfos = new AutoInitDictionary<HtmlTable,FormGridRowInfoCollection>();
  private AutoInitDictionary<HtmlTable,StringCollection> _listOfHiddenRows = new AutoInitDictionary<HtmlTable,StringCollection>();

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
    StringCollection hiddenRows = (StringCollection)_listOfHiddenRows[FormGrid];
    FormGridRowInfoCollection newRows = (FormGridRowInfoCollection)_listOfFormGridRowInfos[FormGrid];

		base.OnInit(e);
    InitalizePartnerFieldMenuItems();
	  CreateExtraFormGrid("Init");
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

  protected void ShowExtraFormGridButton_Click (object sender, EventArgs e)
  {
    var textField = (BocTextValue) ((HtmlTable) ExtraFormGridPlaceHolder.Controls[0]).Rows[1].Cells[2].Controls[0];
    Assertion.IsTrue (DataSource.GetAllBoundControls().Contains (textField));

    CreateExtraFormGrid ("Button");

    Assertion.IsFalse (DataSource.GetAllBoundControls().Contains (textField));
  }

  private void CreateExtraFormGrid (string contextInformation)
  {
    var formGrid = new HtmlTable();
    formGrid.ID = "ExtraFormGrid";

    var titleRow = new HtmlTableRow();
    titleRow.Cells.Add (new HtmlTableCell { ColSpan = 2, InnerText = "Extra FormGrid from " + contextInformation });
    formGrid.Rows.Add (titleRow);

    var dataRow = new HtmlTableRow();
    dataRow.Cells.Add (new HtmlTableCell());
    var controlCell = new HtmlTableCell();
    controlCell.Controls.Add (new BocTextValue { ID = "TextField", Required = true, DataSource = DataSource });
    dataRow.Cells.Add (controlCell);
    formGrid.Rows.Add (dataRow);

    ExtraFormGridPlaceHolder.Controls.Clear();
    ExtraFormGridPlaceHolder.Controls.Add (formGrid);
    if (FormGridManager.IsFormGridRegistered (formGrid))
      FormGridManager.UnregisterFormGrid (formGrid);
    FormGridManager.RegisterFormGrid (formGrid);


  }
}
}
