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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Results;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace OBWTest.IndividualControlTests
{

public class BocListAsGridUserControl : BaseUserControl
{
  private class AllRequiredEditableRowControlFactory : EditableRowControlFactory
  {
    public override IBusinessObjectBoundEditableWebControl Create (BocSimpleColumnDefinition column, int columnIndex)
    {
      var control =(BusinessObjectBoundEditableWebControl)base.Create(column, columnIndex);
      control.Required = true;
      return control;
    }
  }

  protected HtmlTable Table3;
  protected BocTextValue FirstNameField;
  protected BocTextValue LastNameField;
  protected TestBocList ChildrenList;
  protected TestBocList EmptyList;
  protected CheckBox EnableValidationErrorsCheckBox;
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
  protected WebButton SwitchToEditModeButton;
  protected WebButton EndEditModeButton;
  protected WebButton AddItemButton;
  protected WebButton AddRowButton;
  protected WebButton AddRowsButton;
  protected BocTextValue NumberOfNewRowsField;
  protected WebButton RemoveRowsButton;
  protected WebButton RemoveItemsButton;
  protected WebButton CancelEditModeButton;
  protected BindableObjectDataSourceControl CurrentObject;
  protected BindableObjectDataSourceControlValidationResultDispatchingValidator CurrentObjectValidationResultDispatchingValidator;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    SwitchToEditModeButton.Click += new EventHandler(SwitchToEditModeButton_Click);
    EndEditModeButton.Click += new EventHandler(EndEditModeButton_Click);
    CancelEditModeButton.Click += new EventHandler(CancelEditModeButton_Click);

    AddItemButton.Click += new EventHandler(AddItemButton_Click);
    AddRowButton.Click += new EventHandler(AddRowButton_Click);
    AddRowsButton.Click += new EventHandler(AddRowsButton_Click);
    RemoveRowsButton.Click += new EventHandler(RemoveRowsButton_Click);
    RemoveItemsButton.Click += new EventHandler(RemoveItemsButton_Click);

    ChildrenList.ListItemCommandClick += new BocListItemCommandClickEventHandler(this.ChildrenList_ListItemCommandClick);
    ChildrenList.MenuItemClick += new WebMenuItemClickEventHandler(this.ChildrenList_MenuItemClick);

    ChildrenList.DataRowRender += new BocListDataRowRenderEventHandler(this.ChildrenList_DataRowRender);

    ChildrenList.EditableRowChangesCanceling += new BocListEditableRowChangesEventHandler(ChildrenList_EditableRowChangesCanceling);
    ChildrenList.EditableRowChangesCanceled += new BocListItemEventHandler(ChildrenList_EditableRowChangesCanceled);
    ChildrenList.EditableRowChangesSaving += new BocListEditableRowChangesEventHandler(ChildrenList_EditableRowChangesSaving);
    ChildrenList.EditableRowChangesSaved += new BocListItemEventHandler(ChildrenList_EditableRowChangesSaved);

    ChildrenList.EditModeControlFactory = new AllRequiredEditableRowControlFactory();
    ChildrenList.DisableEditModeValidationMessages = true;
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
    InitializeComponent();
    base.OnInit(e);
    InitializeMenuItems();
  }

  private void InitializeMenuItems ()
  {
    BocMenuItem menuItem = null;

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Category = "Edit";
    menuItem.Text = WebString.CreateFromText("Copy");
    menuItem.Icon.Url = "~/Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.ExactlyOne;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.ListMenuItems.Add(menuItem);
    ChildrenList.OptionsMenuItems.Add(menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Category = "Edit";
    menuItem.Text = WebString.CreateFromText("Paste");
    menuItem.IsDisabled = false;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.ListMenuItems.Add(menuItem);
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

    BocSimpleColumnDefinition dayofDeathColumnDefinition = new BocSimpleColumnDefinition();
    dayofDeathColumnDefinition.ColumnTitle = WebString.CreateFromText("Day of Death");
    dayofDeathColumnDefinition.SetPropertyPath(BusinessObjectPropertyPath.CreateStatic(new [] { dateOfDeath }));
    dayofDeathColumnDefinition.Width = Unit.Parse("9.1em", CultureInfo.InvariantCulture);
    dayofDeathColumnDefinition.EnforceWidth = true;

    BocSimpleColumnDefinition heightColumnDefinition = new BocSimpleColumnDefinition();
    heightColumnDefinition.SetPropertyPath(BusinessObjectPropertyPath.CreateStatic(new [] { height }));

    BocSimpleColumnDefinition genderColumnDefinition = new BocSimpleColumnDefinition();
    genderColumnDefinition.SetPropertyPath(BusinessObjectPropertyPath.CreateStatic(new [] { gender }));

    BocSimpleColumnDefinition cvColumnDefinition = new BocSimpleColumnDefinition();
    cvColumnDefinition.SetPropertyPath(BusinessObjectPropertyPath.CreateStatic(new [] { cv }));

    BocSimpleColumnDefinition incomeColumnDefinition = new BocSimpleColumnDefinition();
    incomeColumnDefinition.SetPropertyPath(BusinessObjectPropertyPath.CreateStatic(new [] { income }));

    BocListView datesView = new BocListView();
    datesView.Title = "Dates";
    datesView.ColumnDefinitions.AddRange(
          new BocColumnDefinition[] {birthdayColumnDefinition, dayofDeathColumnDefinition});

    BocListView statsView = new BocListView();
    statsView.Title = "Stats";
    statsView.ColumnDefinitions.AddRange(
        new BocColumnDefinition[] {heightColumnDefinition, genderColumnDefinition});

    BocListView cvView = new BocListView();
    cvView.Title = "CV";
    cvView.ColumnDefinitions.AddRange(
        new BocColumnDefinition[] {cvColumnDefinition});

    BocListView incomeView = new BocListView();
    incomeView.Title = "Income";
    incomeView.ColumnDefinitions.AddRange(
        new BocColumnDefinition[] {incomeColumnDefinition});

    ChildrenList.AvailableViews.AddRange(new BocListView[] {
      datesView,
      statsView,
      cvView,
      incomeView});

    if (! IsPostBack)
      ChildrenList.SelectedView = datesView;

    if (!IsPostBack)
    {
//      ChildrenList.SetSortingOrder (
//          new BocListSortingOrderEntry[] {
//              new BocListSortingOrderEntry ((BocColumnDefinition) ChildrenList.FixedColumns[7], SortingDirection.Ascending) });
    }

    if (! IsPostBack)
      ChildrenList.SwitchListIntoEditMode();

    if (! IsPostBack)
      NumberOfNewRowsField.Value = 1;
    NumberOfNewRowsField.IsDirty = false;
  }

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender(e);

    SwitchToEditModeButton.Enabled = ! ChildrenList.IsListEditModeActive;
    EndEditModeButton.Enabled = ChildrenList.IsListEditModeActive;
    EndEditModeButton.Enabled = ChildrenList.IsListEditModeActive;
    CancelEditModeButton.Enabled = ChildrenList.IsListEditModeActive;

    if (EnableValidationErrorsCheckBox.Checked)
      CreateValidationErrors();
  }

  private void SwitchToEditModeButton_Click (object sender, EventArgs e)
  {
    ChildrenList.SwitchListIntoEditMode();
  }

  private void EndEditModeButton_Click (object sender, EventArgs e)
  {
    ChildrenList.EndListEditMode(true);
  }

  private void CancelEditModeButton_Click (object sender, EventArgs e)
  {
    ChildrenList.EndListEditMode(false);
  }

  private void AddItemButton_Click (object sender, EventArgs e)
  {
    Person person = Person.CreateObject(Guid.NewGuid());
    person.LastName = "X";

    // Exercise IList in BocList.ValueAsList
    ChildrenList.ValueAsList.Add(person);
    ChildrenList.SynchronizeRows();
  }

  private void AddRowButton_Click (object sender, EventArgs e)
  {
    Person person = Person.CreateObject(Guid.NewGuid());
    person.LastName = "X";

    ChildrenList.AddRow((IBusinessObject)person);
  }

  private void AddRowsButton_Click (object sender, EventArgs e)
  {
    int count = 0;

    if (NumberOfNewRowsField.Validate())
      count = (int)NumberOfNewRowsField.Value;

    Person[] persons = new Person[count];
    for (int i = 0; i < count; i++)
      persons[i] = Person.CreateObject(Guid.NewGuid());

    ChildrenList.AddRows((IBusinessObjectWithIdentity[])ArrayUtility.Convert(persons, typeof(IBusinessObjectWithIdentity)));
  }

  private void RemoveRowsButton_Click (object sender, EventArgs e)
  {
    IBusinessObject[] selectedBusinessObjects = ChildrenList.GetSelectedBusinessObjects();
    ChildrenList.RemoveRows(selectedBusinessObjects);
  }

  private void RemoveItemsButton_Click (object sender, EventArgs e)
  {
    IBusinessObject[] selectedBusinessObjects = ChildrenList.GetSelectedBusinessObjects();
    foreach (var obj in selectedBusinessObjects)
      ChildrenList.ValueAsList.Remove((Person)obj);
    ChildrenList.SynchronizeRows();
  }

  private void CreateValidationErrors ()
  {
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
                    "Localized bad first child names"),
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
                    "Localized bad last child names"),
            }));

    PrepareValidation();
    FormGridManager.PrepareValidation();
    DataSourceValidationResultDispatchingValidator.DispatchValidationFailures(validationResult);
    DataSourceValidationResultDispatchingValidator.Validate();

    UnhandledValidationErrorsLabel.Text = string.Join(
        Environment.NewLine,
        validationResult.GetUnhandledValidationFailures().Select(FormatValidationFailure));

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
  }

  private void ChildrenList_MenuItemClick (object sender, WebMenuItemClickEventArgs e)
  {
    ChildrenListEventArgsLabel.Text = e.Item.ItemID;
  }

  private void ChildrenList_DataRowRender (object sender, BocListDataRowRenderEventArgs e)
  {
    if (e.ListIndex == 3)
      e.SetRowReadOnly();
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

  #region Web Form Designer generated code	
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
