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
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace OBWTest.IndividualControlTests
{

[WebMultiLingualResources ("OBWTest.Globalization.IndividualControlTests.BocListUserControl")]
public class BocListUserControl : BaseUserControl
{
  private const string c_deleteItemID = "Delete";
  protected HtmlTable Table3;
  protected BocTextValue FirstNameField;
  protected BocTextValue LastNameField;
  protected BocList JobList;
  protected TestBocList ChildrenList;
  protected TestBocList EmptyList;
  protected Button ChildrenListEndEditModeButton;
  protected Button ChildrenListAddAndEditButton;
  protected Button ChildrenListSetPageButton;
  protected CheckBox ChildrenListEventCheckBox;
  protected Label ChildrenListEventArgsLabel;
  protected FormGridManager FormGridManager;
  protected BindableObjectDataSourceControl EmptyDataSourceControl;
  protected HtmlTable FormGrid;
  protected TestBocListValidator EmptyListValidator;
  protected TestBocList Testboclist1;
  protected BocList AllColumnsList;
  protected HtmlGenericControl NonVisualControls;
  protected BindableObjectDataSourceControl CurrentObject;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    ChildrenListAddAndEditButton.Click += new EventHandler(AddAndEditButton_Click);
    ChildrenListEndEditModeButton.Click += new EventHandler(ChildrenListEndEditModeButton_Click);
    ChildrenListSetPageButton.Click += ChildrenListSetPageButton_Click;

    ChildrenList.ListItemCommandClick += new BocListItemCommandClickEventHandler (ChildrenList_ListItemCommandClick);
    ChildrenList.MenuItemClick += new WebMenuItemClickEventHandler (ChildrenList_MenuItemClick);
ChildrenList.RowMenuItemClick += ChildrenList_RowMenuItemClick;
    ChildrenList.DataRowRender += new BocListDataRowRenderEventHandler(ChildrenList_DataRowRender);
    
    ChildrenList.EditableRowChangesCanceling += new BocListEditableRowChangesEventHandler (ChildrenList_EditableRowChangesCanceling);
    ChildrenList.EditableRowChangesCanceled += new BocListItemEventHandler (ChildrenList_EditableRowChangesCanceled);
    ChildrenList.EditableRowChangesSaving += new BocListEditableRowChangesEventHandler (ChildrenList_EditableRowChangesSaving);
    ChildrenList.EditableRowChangesSaved += new BocListItemEventHandler (ChildrenList_EditableRowChangesSaved);

    ChildrenList.SortingOrderChanging += new BocListSortingOrderChangeEventHandler (ChildrenList_SortingOrderChanging);
    ChildrenList.SortingOrderChanged += new BocListSortingOrderChangeEventHandler (ChildrenList_SortingOrderChanged);
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

  override protected void OnInit(EventArgs e)
  {
    base.OnInit (e);
    InitializeMenuItems();
  }

  private void InitializeMenuItems()
  {
    BocMenuItem menuItem = null;

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Event";
    menuItem.Text = "Event";
    menuItem.Category = "PostBacks";
    menuItem.Command.Type = CommandType.Event;
    JobList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Enum.Href";
    menuItem.Text = "Href";
    menuItem.Category = "Links";
    menuItem.Style = WebMenuItemStyle.Text;
    menuItem.Command.Type = CommandType.Href;
    menuItem.Command.HrefCommand.Href = "link.htm";
    JobList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = "<b>Wxe</b>";
    menuItem.Category = "PostBacks";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.TypeName = "MyType, MyAssembly";
    JobList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = "<b>Wxe</b>";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.TypeName = "MyType, MyAssembly";
    menuItem.Command.WxeFunctionCommand.Parameters = "Test'Test";
    JobList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = "Event";
    menuItem.Command.Type = CommandType.Event;
    JobList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = "Href";
    menuItem.Command.Type = CommandType.Href;
    menuItem.Command.HrefCommand.Href = "link.htm";
    JobList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = "Invisible Item";
    menuItem.IsVisible = false;
    ChildrenList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = "Invisible Item";
    menuItem.IsVisible = false;
    ChildrenList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = "Paste";
    menuItem.Category = "Edit";
    menuItem.IsDisabled = true;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.ListMenuItems.Add (menuItem);
  
    menuItem = new BocMenuItem();
    menuItem.ItemID = c_deleteItemID;
    menuItem.Text = "Delete";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "~/Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "~/Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "~/Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.ExactlyOne;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Open";
    menuItem.Text = "Open";
    menuItem.Category = "Object";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.Parameters = "objects";
    menuItem.Command.WxeFunctionCommand.TypeName = "OBWTest.ViewPersonsWxeFunction,OBWTest";
    ChildrenList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Text = "Copy";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "~/Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Cut";
    menuItem.Text = "Cut";
    menuItem.Category = "Edit";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = "Paste";
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Duplicate";
    menuItem.Text = "Duplicate";
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = "Delete";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "~/Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "~/Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = "Invisible Item";
    menuItem.IsVisible = false;
    ChildrenList.ListMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = "Invisible Item";
    menuItem.IsVisible = false;
    ChildrenList.OptionsMenuItems.Add (menuItem);
  }

  override protected void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    IBusinessObjectProperty dateOfBirth = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("DateOfBirth");
    IBusinessObjectProperty dateOfDeath = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("DateOfDeath");
    IBusinessObjectProperty height = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Height");
    IBusinessObjectProperty gender = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Gender");
    IBusinessObjectProperty cv = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("CV");
    IBusinessObjectProperty income = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Income");


    //  Additional columns, in-code generated

    BocSimpleColumnDefinition birthdayColumnDefinition = new BocSimpleColumnDefinition();
    birthdayColumnDefinition.ColumnTitle = "Birthday";
    birthdayColumnDefinition.SetPropertyPath (BusinessObjectPropertyPath.CreateStatic (new []{dateOfBirth}));
    birthdayColumnDefinition.Width = Unit.Parse ("14em");
    birthdayColumnDefinition.EnforceWidth = true;

    BocSimpleColumnDefinition dayofDeathColumnDefinition = new BocSimpleColumnDefinition();
    dayofDeathColumnDefinition.ColumnTitle = "Day of Death";
    dayofDeathColumnDefinition.SetPropertyPath (BusinessObjectPropertyPath.CreateStatic (new []{dateOfDeath}));
    dayofDeathColumnDefinition.Width = Unit.Parse ("7em");
    dayofDeathColumnDefinition.EnforceWidth = true;

    BocSimpleColumnDefinition heightColumnDefinition = new BocSimpleColumnDefinition();
    heightColumnDefinition.SetPropertyPath (BusinessObjectPropertyPath.CreateStatic (new []{height}));

    BocSimpleColumnDefinition genderColumnDefinition = new BocSimpleColumnDefinition();
    genderColumnDefinition.SetPropertyPath (BusinessObjectPropertyPath.CreateStatic (new []{gender}));

    BocSimpleColumnDefinition cvColumnDefinition = new BocSimpleColumnDefinition();
    cvColumnDefinition.SetPropertyPath (BusinessObjectPropertyPath.CreateStatic (new []{cv}));
    cvColumnDefinition.FormatString = "lines=3";

    BocSimpleColumnDefinition incomeColumnDefinition = new BocSimpleColumnDefinition();
    incomeColumnDefinition.SetPropertyPath (BusinessObjectPropertyPath.CreateStatic (new []{income}));

    BocListView datesView = new BocListView();
    datesView.Title = "Dates";
    datesView.ColumnDefinitions.AddRange (new BocColumnDefinition[] {birthdayColumnDefinition, dayofDeathColumnDefinition});

    BocListView statsView = new BocListView();
    statsView.Title = "Stats";
    statsView.ColumnDefinitions.AddRange (new BocColumnDefinition[] {heightColumnDefinition, genderColumnDefinition});

    BocListView cvView = new BocListView();
    cvView.Title = "CV";
    cvView.ColumnDefinitions.AddRange (new BocColumnDefinition[] {cvColumnDefinition});

    BocListView incomeView = new BocListView();
    incomeView.Title = "Income";
    incomeView.ColumnDefinitions.AddRange (new BocColumnDefinition[] {incomeColumnDefinition});

    ChildrenList.AvailableViews.AddRange (new BocListView[] {
      datesView,
      statsView,
      cvView,
      incomeView});

    if (! IsPostBack)
      ChildrenList.SelectedView = datesView;

    if (!IsPostBack)
    {
      ChildrenList.SetSortingOrder (
          new BocListSortingOrderEntry[] {
              new BocListSortingOrderEntry ((IBocSortableColumnDefinition) ChildrenList.FixedColumns[7], SortingDirection.Ascending) });
    }
    if (IsPostBack)
    {
//      BocListSortingOrderEntry[] sortingOrder = ChildrenList.GetSortingOrder();
    }

    if (!IsPostBack)
    {
      JobList.SetSelectedRows (new[] { 1 });
      ChildrenList.SetSelectedBusinessObjects (new[] { ChildrenList.Value[1] });
    }
  }

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);
  }

  public override void LoadValues(bool interim)
  {
    base.LoadValues (interim);

    if (CurrentObject.BusinessObject is Person)
    {
      Person person = (Person) CurrentObject.BusinessObject;
      //AllColumnsList.LoadUnboundValue (person.Children, IsPostBack);
    }
  }

  private void AddAndEditButton_Click(object sender, EventArgs e)
  {
    Person person = Person.CreateObject (Guid.NewGuid());
    ChildrenList.AddAndEditRow ((IBusinessObject) person);
  }

  private void ChildrenListEndEditModeButton_Click(object sender, EventArgs e)
  {
    ChildrenList.EndRowEditMode (true);
  }

  private void ChildrenListSetPageButton_Click (object sender, EventArgs eventArgs)
  {
    ChildrenList.SetPageIndex (0);
  }

  private void ChildrenList_ListItemCommandClick(object sender, BocListItemCommandClickEventArgs e)
  {
    ChildrenListEventCheckBox.Checked = true;
    ChildrenListEventArgsLabel.Text += string.Format ("ColumnID: {0}<br />", e.Column.ItemID);
    if (e.BusinessObject is IBusinessObjectWithIdentity)
      ChildrenListEventArgsLabel.Text += string.Format ("BusinessObjectID: {0}<br />", ((IBusinessObjectWithIdentity) e.BusinessObject).UniqueIdentifier);
    ChildrenListEventArgsLabel.Text += string.Format ("ListIndex: {0}<br />", e.ListIndex);

    if (e.Column.ItemID == "Edit")
      ChildrenList.SwitchRowIntoEditMode (e.ListIndex);
  }

  private void ChildrenList_MenuItemClick(object sender, WebMenuItemClickEventArgs e)
  {
    ChildrenListEventArgsLabel.Text = e.Item.ItemID;
    if (e.Item.ItemID == c_deleteItemID)
      ChildrenList.RemoveRows (ChildrenList.GetSelectedBusinessObjects());
  }

  private void ChildrenList_RowMenuItemClick (object sender, BocListItemEventArgs e)
  {
    ChildrenListEventArgsLabel.Text += string.Format ("MenuItemID: {0}<br />", ((WebMenuItem) sender).ItemID);
    if (e.BusinessObject is IBusinessObjectWithIdentity)
      ChildrenListEventArgsLabel.Text += string.Format ("BusinessObjectID: {0}<br />", ((IBusinessObjectWithIdentity) e.BusinessObject).UniqueIdentifier);
    ChildrenListEventArgsLabel.Text += string.Format ("ListIndex: {0}<br />", e.ListIndex);
  }


  private void ChildrenList_DataRowRender(object sender, BocListDataRowRenderEventArgs e)
  {
    if (e.ListIndex == 3)
    {
      e.SetRowReadOnly();
      e.SetAdditionalCssClassForDataRow ("test");
    }
  }

  private void ChildrenList_EditableRowChangesCanceling(object sender, BocListEditableRowChangesEventArgs e)
  {
  }

  private void ChildrenList_EditableRowChangesCanceled(object sender, BocListItemEventArgs e)
  {
  }

  private void ChildrenList_EditableRowChangesSaving(object sender, BocListEditableRowChangesEventArgs e)
  {
  }

  private void ChildrenList_EditableRowChangesSaved(object sender, BocListItemEventArgs e)
  {
  }

  private void ChildrenList_SortingOrderChanging(object sender, BocListSortingOrderChangeEventArgs e)
  {
  
  }

  private void ChildrenList_SortingOrderChanged(object sender, BocListSortingOrderChangeEventArgs e)
  {
  
  }
}

}
