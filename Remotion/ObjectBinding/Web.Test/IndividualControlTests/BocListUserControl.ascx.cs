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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Results;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace OBWTest.IndividualControlTests
{

[WebMultiLingualResources("OBWTest.Globalization.IndividualControlTests.BocListUserControl")]
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
  protected Button ChildrenListAddRowButton;
  protected Button ChildrenListRemoveRowsButton;
  protected CheckBox EnableValidationErrorsCheckbox;
  protected CheckBox ChildrenListEventCheckBox;
  protected Label ChildrenListEventArgsLabel;
  protected Label UnhandledValidationErrorsLabel;
  protected FormGridManager FormGridManager;
  protected BindableObjectDataSourceControl EmptyDataSourceControl;
  protected HtmlTable FormGrid;
  protected TestBocListValidator EmptyListValidator;
  protected TestBocList Testboclist1;
  protected BocList AllColumnsList;
  protected HtmlGenericControl NonVisualControls;
  protected BindableObjectDataSourceControl CurrentObject;
  protected BindableObjectDataSourceControlValidationResultDispatchingValidator CurrentObjectValidationResultDispatchingValidator;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    ChildrenListAddAndEditButton.Click += new EventHandler(AddAndEditButton_Click);
    ChildrenListEndEditModeButton.Click += new EventHandler(ChildrenListEndEditModeButton_Click);
    ChildrenListSetPageButton.Click += ChildrenListSetPageButton_Click;
    ChildrenListAddRowButton.Click += ChildrenListAddRowButton_Click;
    ChildrenListRemoveRowsButton.Click += ChildrenListRemoveRowsButton_Click;

    ChildrenList.ListItemCommandClick += new BocListItemCommandClickEventHandler(ChildrenList_ListItemCommandClick);
    ChildrenList.MenuItemClick += new WebMenuItemClickEventHandler(ChildrenList_MenuItemClick);
    ChildrenList.RowMenuItemClick += ChildrenList_RowMenuItemClick;
    ChildrenList.DataRowRender += new BocListDataRowRenderEventHandler(ChildrenList_DataRowRender);

    ChildrenList.EditableRowChangesCanceling += new BocListEditableRowChangesEventHandler(ChildrenList_EditableRowChangesCanceling);
    ChildrenList.EditableRowChangesCanceled += new BocListItemEventHandler(ChildrenList_EditableRowChangesCanceled);
    ChildrenList.EditableRowChangesSaving += new BocListEditableRowChangesEventHandler(ChildrenList_EditableRowChangesSaving);
    ChildrenList.EditableRowChangesSaved += new BocListItemEventHandler(ChildrenList_EditableRowChangesSaved);

    ChildrenList.SortingOrderChanging += new BocListSortingOrderChangeEventHandler(ChildrenList_SortingOrderChanging);
    ChildrenList.SortingOrderChanged += new BocListSortingOrderChangeEventHandler(ChildrenList_SortingOrderChanged);
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

  public override BindableObjectDataSourceControlValidationResultDispatchingValidator DataSourceValidationResultDispatchingValidator
  {
    get { return CurrentObjectValidationResultDispatchingValidator; }
  }

  override protected void OnInit (EventArgs e)
  {
    base.OnInit(e);
    InitializeMenuItems();
  }

  private void InitializeMenuItems ()
  {
    BocMenuItem menuItem = null;

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Event";
    menuItem.Text = WebString.CreateFromText("Event");
    menuItem.Category = "PostBacks";
    menuItem.Command.Type = CommandType.Event;
    JobList.ListMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Enum.Href";
    menuItem.Text = WebString.CreateFromText("Href");
    menuItem.Category = "Links";
    menuItem.Style = WebMenuItemStyle.Text;
    menuItem.Command.Type = CommandType.Href;
    menuItem.Command.HrefCommand.Href = "link.htm";
    JobList.ListMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = WebString.CreateFromHtml("<b>Wxe</b>");
    menuItem.Category = "PostBacks";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.TypeName = "MyType, MyAssembly";
    JobList.ListMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = WebString.CreateFromHtml("<b>Wxe</b>");
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.TypeName = "MyType, MyAssembly";
    menuItem.Command.WxeFunctionCommand.Parameters = "Test'Test";
    JobList.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = WebString.CreateFromText("Event");
    menuItem.Command.Type = CommandType.Event;
    JobList.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = WebString.CreateFromText("Href");
    menuItem.Command.Type = CommandType.Href;
    menuItem.Command.HrefCommand.Href = "link.htm";
    JobList.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = WebString.CreateFromText("Invisible Item");
    menuItem.IsVisible = false;
    JobList.ListMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = WebString.CreateFromText("Invisible Item");
    menuItem.IsVisible = false;
    JobList.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = WebString.CreateFromText("Paste");
    menuItem.Category = "Edit";
    menuItem.IsDisabled = true;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.ListMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = c_deleteItemID;
    menuItem.Text = WebString.CreateFromText("Delete");
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "~/Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "~/Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.ListMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "~/Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.ExactlyOne;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.ListMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Open";
    menuItem.Text = WebString.CreateFromText("Open");
    menuItem.Category = "Object";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.Parameters = "objects";
    menuItem.Command.WxeFunctionCommand.TypeName = "OBWTest.ViewPersonsWxeFunction,OBWTest";
    ChildrenList.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Text = WebString.CreateFromText("Copy");
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "~/Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Cut";
    menuItem.Text = WebString.CreateFromText("Cut");
    menuItem.Category = "Edit";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = WebString.CreateFromText("Paste");
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Duplicate";
    menuItem.Text = WebString.CreateFromText("Duplicate");
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = WebString.CreateFromText("Delete");
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "~/Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "~/Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = WebString.CreateFromText("Invisible Item");
    menuItem.IsVisible = false;
    ChildrenList.ListMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.Text = WebString.CreateFromText("Invisible Item");
    menuItem.IsVisible = false;
    ChildrenList.OptionsMenuItems.Add(menuItem);

    ChildrenList.OptionsMenuItems.Add(WebMenuItem.GetSeparator());

    menuItem = new BocMenuItem();
    menuItem.ItemID = "FilterByService";
    menuItem.Text = WebString.CreateFromText("Should be filtered");
    menuItem.IsVisible = true;
    ChildrenList.OptionsMenuItems.Add(menuItem);

    ChildrenList.OptionsMenuItems.Add(WebMenuItem.GetSeparator());

    menuItem = new BocMenuItem();
    menuItem.ItemID = "DisabledByService";
    menuItem.Text = WebString.CreateFromText("Should be disabled");
    menuItem.IsDisabled = false;
    ChildrenList.OptionsMenuItems.Add(menuItem);
  }

  override protected void OnLoad (EventArgs e)
  {
    base.OnLoad(e);

    IBusinessObjectProperty dateOfBirth = CurrentObject.BusinessObjectClass.GetPropertyDefinition("DateOfBirth");
    IBusinessObjectProperty dateOfDeath = CurrentObject.BusinessObjectClass.GetPropertyDefinition("DateOfDeath");
    IBusinessObjectProperty height = CurrentObject.BusinessObjectClass.GetPropertyDefinition("Height");
    IBusinessObjectProperty gender = CurrentObject.BusinessObjectClass.GetPropertyDefinition("Gender");
    IBusinessObjectProperty cv = CurrentObject.BusinessObjectClass.GetPropertyDefinition("CV");
    IBusinessObjectProperty income = CurrentObject.BusinessObjectClass.GetPropertyDefinition("Income");


    //  Additional columns, in-code generated

    BocSimpleColumnDefinition birthdayColumnDefinition = new BocSimpleColumnDefinition();
    birthdayColumnDefinition.ColumnTitle = WebString.CreateFromText("Birthday");
    birthdayColumnDefinition.SetPropertyPath(BusinessObjectPropertyPath.CreateStatic(new []{dateOfBirth}));
    birthdayColumnDefinition.Width = Unit.Parse("17em");
    birthdayColumnDefinition.EnforceWidth = true;

    BocSimpleColumnDefinition dayofDeathColumnDefinition = new BocSimpleColumnDefinition();
    dayofDeathColumnDefinition.ColumnTitle = WebString.CreateFromText("Day of Death");
    dayofDeathColumnDefinition.SetPropertyPath(BusinessObjectPropertyPath.CreateStatic(new []{dateOfDeath}));
    dayofDeathColumnDefinition.Width = Unit.Parse("7em");
    dayofDeathColumnDefinition.EnforceWidth = true;

    BocSimpleColumnDefinition heightColumnDefinition = new BocSimpleColumnDefinition();
    heightColumnDefinition.SetPropertyPath(BusinessObjectPropertyPath.CreateStatic(new []{height}));

    BocSimpleColumnDefinition genderColumnDefinition = new BocSimpleColumnDefinition();
    genderColumnDefinition.SetPropertyPath(BusinessObjectPropertyPath.CreateStatic(new []{gender}));

    BocSimpleColumnDefinition cvColumnDefinition = new BocSimpleColumnDefinition();
    cvColumnDefinition.SetPropertyPath(BusinessObjectPropertyPath.CreateStatic(new []{cv}));
    cvColumnDefinition.FormatString = "lines=3";

    BocSimpleColumnDefinition incomeColumnDefinition = new BocSimpleColumnDefinition();
    incomeColumnDefinition.SetPropertyPath(BusinessObjectPropertyPath.CreateStatic(new []{income}));

    BocListView datesView = new BocListView();
    datesView.Title = "Dates";
    datesView.ColumnDefinitions.AddRange(new BocColumnDefinition[] {birthdayColumnDefinition, dayofDeathColumnDefinition});

    BocListView statsView = new BocListView();
    statsView.Title = "Stats";
    statsView.ColumnDefinitions.AddRange(new BocColumnDefinition[] {heightColumnDefinition, genderColumnDefinition});

    BocListView cvView = new BocListView();
    cvView.Title = "CV";
    cvView.ColumnDefinitions.AddRange(new BocColumnDefinition[] {cvColumnDefinition});

    BocListView incomeView = new BocListView();
    incomeView.Title = "Income";
    incomeView.ColumnDefinitions.AddRange(new BocColumnDefinition[] {incomeColumnDefinition});

    ChildrenList.AvailableViews.AddRange(new BocListView[] {
      datesView,
      statsView,
      cvView,
      incomeView});

    if (! IsPostBack)
      ChildrenList.SelectedView = datesView;

    if (!IsPostBack)
    {
      ChildrenList.SetSortingOrder(
          new BocListSortingOrderEntry[] {
              new BocListSortingOrderEntry((IBocSortableColumnDefinition)ChildrenList.FixedColumns[7], SortingDirection.Ascending) });
    }
    if (IsPostBack)
    {
//      BocListSortingOrderEntry[] sortingOrder = ChildrenList.GetSortingOrder();
    }

    if (!IsPostBack)
    {
      JobList.SetSelectedRows(new[] { 1 });
      ChildrenList.SetSelectedBusinessObjects(new[] { ChildrenList.Value[1] });
    }
  }

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender(e);

    if (EnableValidationErrorsCheckbox.Checked)
      CreateValidationErrors();
  }

  public override void LoadValues (bool interim)
  {
    base.LoadValues(interim);

    if (CurrentObject.BusinessObject is Person)
    {
      Person person = (Person)CurrentObject.BusinessObject;
      //AllColumnsList.LoadUnboundValue (person.Children, IsPostBack);
    }
  }

  private void AddAndEditButton_Click (object sender, EventArgs e)
  {
    Person person = Person.CreateObject(Guid.NewGuid());
    ChildrenList.AddAndEditRow((IBusinessObject)person);
  }

  private void ChildrenListEndEditModeButton_Click (object sender, EventArgs e)
  {
    ChildrenList.EndRowEditMode(true);
  }

  private void ChildrenListSetPageButton_Click (object sender, EventArgs eventArgs)
  {
    ChildrenList.SetPageIndex(0);
  }

  private void ChildrenListAddRowButton_Click (object sender, EventArgs e)
  {
    Person person = Person.CreateObject(Guid.NewGuid());
    person.LastName = "X";

    // Exercise IList<T> in BocList.Value
    ((IList<Person>)ChildrenList.Value).Add(person);
    ChildrenList.SynchronizeRows();
  }

  private void ChildrenListRemoveRowsButton_Click (object sender, EventArgs e)
  {
    IBusinessObject[] selectedBusinessObjects = ChildrenList.GetSelectedBusinessObjects();
    foreach (var obj in selectedBusinessObjects)
      ((IList<Person>)ChildrenList.Value).Remove((Person)obj);
    ChildrenList.SynchronizeRows();
  }


  private void CreateValidationErrors ()
  {
    // Ensure that the last row of the jobs list is not displayed to test that failure from rows
    // that are currently not visible are also displayed
    JobList.PageSize = 1;

    var person = (Person)CurrentObject.BusinessObject;

    var firstJob = person.Jobs.First();
    var lastJob = person.Jobs.Last();
    var jobsProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.Jobs));
    var jobTitleProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Job _) => _.Title));
    var jobStartDateProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Job _) => _.StartDate));
    var jobEndDateProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Job _) => _.EndDate));

    var firstChild = person.Children.OrderBy(e => e.LastName).First();
    var lastChild = person.Children.OrderBy(e => e.LastName).Last();
    var childrenProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.Children));
    var personPartnerProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.Partner));
    var personFirstNameProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.FirstName));
    var personLastNameProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.LastName));
    var personFullNameProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.DisplayName));

    var firstNameProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.FirstName));

    var validationResult = BusinessObjectValidationResult.Create(
        new ValidationResult(
            new ValidationFailure[]
            {
                // Jobs property failures
                ValidationFailure.CreatePropertyValidationFailure(
                    person,
                    jobsProperty,
                    person.Jobs,
                    "Bad jobs",
                    "Localized bad jobs"),
                // First job failure (visible)
                ValidationFailure.CreateObjectValidationFailure(
                    firstJob,
                    "Bad first job",
                    "Localized bad first job"),
                ValidationFailure.CreatePropertyValidationFailure(
                    firstJob,
                    jobTitleProperty,
                    firstJob.Title,
                    "Bad first job.Title",
                    "Localized bad first job.Title"),
                ValidationFailure.CreateObjectValidationFailure(
                    firstJob,
                    new[]
                    {
                      new ValidatedProperty(firstJob, jobStartDateProperty),
                    },
                    "Bad first job date",
                    "Localized bad first job dates"),
                ValidationFailure.CreateObjectValidationFailure(
                    firstJob,
                    new[]
                    {
                        new ValidatedProperty(firstJob, jobStartDateProperty),
                        new ValidatedProperty(firstJob, jobEndDateProperty)
                    },
                    "Bad first job dates",
                    "Localized bad first job dates"),
                // Last job failure (invisible)
                ValidationFailure.CreateObjectValidationFailure(
                    lastJob,
                    "Bad last job",
                    "Localized bad last job"),
                ValidationFailure.CreatePropertyValidationFailure(
                    lastJob,
                    jobTitleProperty,
                    lastJob.Title,
                    "Bad last job.Title",
                    "Localized bad last job.Title"),
                ValidationFailure.CreateObjectValidationFailure(
                    lastJob,
                    new[]
                    {
                        new ValidatedProperty(lastJob, jobStartDateProperty),
                    },
                    "Bad last job date",
                    "Localized bad last job dates"),
                ValidationFailure.CreateObjectValidationFailure(
                    lastJob,
                    new[]
                    {
                        new ValidatedProperty(lastJob, jobStartDateProperty),
                        new ValidatedProperty(lastJob, jobEndDateProperty)
                    },
                    "Bad last job dates",
                    "Localized bad last job dates"),
                // Children property failures
                ValidationFailure.CreatePropertyValidationFailure(
                    person,
                    childrenProperty,
                    person.Children,
                    "Bad children",
                    "Localized bad children"),
                // First child failure (visible)
                ValidationFailure.CreateObjectValidationFailure(
                    firstChild,
                    "Bad first child",
                    "Localized bad first child"),
                ValidationFailure.CreatePropertyValidationFailure(
                    firstChild,
                    personPartnerProperty,
                    firstChild.Partner,
                    "Bad first child.Partner",
                    "Localized bad first child.Partner"),
                ValidationFailure.CreateObjectValidationFailure(
                    firstChild,
                    new[]
                    {
                        new ValidatedProperty(firstChild, personLastNameProperty),
                    },
                    "Bad first child name",
                    "Localized bad first child name"),
                ValidationFailure.CreateObjectValidationFailure(
                    firstChild,
                    new[]
                    {
                        new ValidatedProperty(firstChild, personLastNameProperty),
                        new ValidatedProperty(firstChild, personFullNameProperty)
                    },
                    "Bad first child names",
                    "Localized bad first child names"),
                // Last child failure (invisible)
                ValidationFailure.CreateObjectValidationFailure(
                    lastChild,
                    "Bad last child",
                    "Localized bad last child"),
                ValidationFailure.CreatePropertyValidationFailure(
                    lastChild,
                    personPartnerProperty,
                    lastChild.Partner,
                    "Bad last child.Partner",
                    "Localized bad last child.Partner"),
                ValidationFailure.CreateObjectValidationFailure(
                    lastChild,
                    new[]
                    {
                        new ValidatedProperty(lastChild, personLastNameProperty),
                    },
                    "Bad last child name",
                    "Localized bad last child name"),
                ValidationFailure.CreateObjectValidationFailure(
                    lastChild,
                    new[]
                    {
                        new ValidatedProperty(lastChild, personLastNameProperty),
                        new ValidatedProperty(lastChild, personFullNameProperty)
                    },
                    "Bad last child names",
                    "Localized bad last child names is a very long message so yeah about that"),
            }));

    ((SmartPage)Page).PrepareValidation();

    DataSourceValidationResultDispatchingValidator.DispatchValidationFailures(validationResult);
    DataSourceValidationResultDispatchingValidator.Validate();

    if (validationResult.GetUnhandledValidationFailures().Any())
    {
      UnhandledValidationErrorsLabel.Text =
          $"This should be the only red message shown in this block. If there are other validation failures shown here, then you probably have a bug somewhere.\n"
          + $"{string.Join(Environment.NewLine, validationResult.GetUnhandledValidationFailures().Select(FormatValidationFailure))}";
    }

    static string FormatValidationFailure (ValidationFailure failure)
    {
      return $"Unhandled validation failure '{failure.ErrorMessage}' for "
             + $"properties {string.Join(", ", failure.ValidatedProperties.Select(vp => $"'{vp}'"))} on object '{failure.ValidatedObject}'.";
    }
  }

  private void ChildrenList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
  {
    ChildrenListEventCheckBox.Checked = true;
    ChildrenListEventArgsLabel.Text += string.Format("ColumnID: {0}<br />", e.Column.ItemID);
    if (e.BusinessObject is IBusinessObjectWithIdentity)
      ChildrenListEventArgsLabel.Text += string.Format("BusinessObjectID: {0}<br />", ((IBusinessObjectWithIdentity)e.BusinessObject).UniqueIdentifier);
    ChildrenListEventArgsLabel.Text += string.Format("ListIndex: {0}<br />", e.ListIndex);

    if (e.Column.ItemID == "Edit")
      ChildrenList.SwitchRowIntoEditMode(e.ListIndex);
  }

  private void ChildrenList_MenuItemClick (object sender, WebMenuItemClickEventArgs e)
  {
    ChildrenListEventArgsLabel.Text = e.Item.ItemID;
    if (e.Item.ItemID == c_deleteItemID)
      ChildrenList.RemoveRows(ChildrenList.GetSelectedBusinessObjects());
  }

  private void ChildrenList_RowMenuItemClick (object sender, BocListItemEventArgs e)
  {
    ChildrenListEventArgsLabel.Text += string.Format("MenuItemID: {0}<br />", ((WebMenuItem)sender).ItemID);
    if (e.BusinessObject is IBusinessObjectWithIdentity)
      ChildrenListEventArgsLabel.Text += string.Format("BusinessObjectID: {0}<br />", ((IBusinessObjectWithIdentity)e.BusinessObject).UniqueIdentifier);
    ChildrenListEventArgsLabel.Text += string.Format("ListIndex: {0}<br />", e.ListIndex);
  }


  private void ChildrenList_DataRowRender (object sender, BocListDataRowRenderEventArgs e)
  {
    if (e.ListIndex == 3)
    {
      e.SetRowReadOnly();
      e.SetAdditionalCssClassForDataRow("test");
    }
  }

  private void ChildrenList_EditableRowChangesCanceling (object sender, BocListEditableRowChangesEventArgs e)
  {
  }

  private void ChildrenList_EditableRowChangesCanceled (object sender, BocListItemEventArgs e)
  {
  }

  private void ChildrenList_EditableRowChangesSaving (object sender, BocListEditableRowChangesEventArgs e)
  {
  }

  private void ChildrenList_EditableRowChangesSaved (object sender, BocListItemEventArgs e)
  {
  }

  private void ChildrenList_SortingOrderChanging (object sender, BocListSortingOrderChangeEventArgs e)
  {

  }

  private void ChildrenList_SortingOrderChanged (object sender, BocListSortingOrderChangeEventArgs e)
  {

  }
}

}
