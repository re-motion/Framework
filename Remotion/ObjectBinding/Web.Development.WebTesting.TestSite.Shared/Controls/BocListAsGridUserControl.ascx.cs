﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Results;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.Controls
{
  public partial class BocListAsGridUserControl : DataEditUserControl
  {
    private IBusinessObjectValidationResult _validationResult = BusinessObjectValidationResult.Create(new ValidationResult());

    protected string SampleIconUrl;

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      var dummyPersonWithNoJobs = Person.CreateObject();
      dummyPersonWithNoJobs.Jobs = new Job[0];
      EmptyObject.BusinessObject = (IBusinessObject)dummyPersonWithNoJobs;
      EmptyObject.LoadValues(false);

      var view1 = new BocListView { ItemID = "ViewCmd1", Title = "View 1" };
      var view2 = new BocListView { ItemID = "ViewCmd2", Title = "View 2" };
      JobList_Normal.AvailableViews.AddRange(view1, view2);
      JobList_Normal.SelectedView = view2;

      JobList_Normal.MenuItemClick += MenuItemClickHandler;
      JobList_Normal.SortingOrderChanged += SortingOrderChangedHandler;
      JobList_Normal.EditableRowChangesSaved += EditableRowChangedSavedHandler;
      JobList_Normal.EditableRowChangesCanceled += EditableRowChangesCanceledHandler;
      JobList_Normal.ListItemCommandClick += ListItemCommandClickHandler;
      JobList_Normal.CustomCellClick += CustomCellClickHandler;

      AlwaysInvalidValidator.ServerValidate += (source, args) => args.IsValid = false;
      JobList_AlwaysInvalid.RegisterValidator(AlwaysInvalidValidator);

      SampleIconUrl = SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>()
          .CreateResourceUrl(typeof(BocListAsGridUserControl), ResourceType.Image, "SampleIcon.gif")
          .GetUrl();
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad(e);

      if (!IsPostBack)
      {
        JobList_Normal.SwitchListIntoEditMode();
        JobList_Empty.SwitchListIntoEditMode();
        JobList_Validation.SwitchListIntoEditMode();
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender(e);
      SetTestOutput();

      if (ValidationTestCaseStartDate.Checked)
      {
        var jobs = ((Person)CurrentObject.BusinessObject!).Jobs;
        var startDateProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Job _) => _.StartDate));
        AddValidationFailures(
            ValidationFailure.CreatePropertyValidationFailure(
                jobs[0],
                startDateProperty,
                jobs[0].StartDate,
                "Start date has a failure",
                "Localized start date has a failure"));
      }

      // We do the validation on every request since it is easier that covering it in multiple event handlers.
      // While we use domain validation, it should only affect the last control that is intended for testing validation.
      ((SmartPage)Page)!.PrepareValidation();
      CurrentObjectValidationResultDispatchingValidator.DispatchValidationFailures(_validationResult);
      CurrentObjectValidationResultDispatchingValidator.Validate();
    }

    private void MenuItemClickHandler (object sender, WebMenuItemClickEventArgs e)
    {
      var bocList = (BocList)sender;
      var command = e.Item.ItemID + "|" + e.Item.Text.ToString();
      TestOutput.SetActionPerformed(bocList.ID, -1, "ListMenuOrOptionsClick", command);
    }

    private void SortingOrderChangedHandler (object sender, BocListSortingOrderChangeEventArgs bocListSortingOrderChangeEventArgs)
    {
      var bocList = (BocList)sender;
      TestOutput.SetActionPerformed(
          bocList.ID,
          -1,
          "SortingOrderChanged",
          string.Join(", ", bocListSortingOrderChangeEventArgs.NewSortingOrder.Select(nso => nso.Column.ItemID + "-" + nso.Direction.ToString()))
      );
    }

    private void EditableRowChangedSavedHandler (object sender, BocListItemEventArgs bocListItemEventArgs)
    {
      var bocList = (BocList)sender;
      TestOutput.SetActionPerformed(bocList.ID, bocListItemEventArgs.ListIndex, "InLineEdit", "Saved");
    }

    private void EditableRowChangesCanceledHandler (object sender, BocListItemEventArgs bocListItemEventArgs)
    {
      var bocList = (BocList)sender;
      TestOutput.SetActionPerformed(bocList.ID, bocListItemEventArgs.ListIndex, "InLineEdit", "Canceled");
    }

    private void ListItemCommandClickHandler (object sender, BocListItemCommandClickEventArgs bocListItemCommandClickEventArgs)
    {
      var bocList = (BocList)sender;
      var cell = bocListItemCommandClickEventArgs.Column.ItemID;
      TestOutput.SetActionPerformed(
          bocList.ID,
          bocListItemCommandClickEventArgs.ListIndex,
          "CellCommandClick",
          cell);
    }

    private void CustomCellClickHandler (object sender, BocCustomCellClickEventArgs bocCustomCellClickEventArgs)
    {
      var bocList = (BocList)sender;
      var cell = bocCustomCellClickEventArgs.Column.ItemID + "|" + bocCustomCellClickEventArgs.Column.ColumnTitleDisplayValue.ToString();
      TestOutput.SetActionPerformed(bocList.ID, -1, "CustomCellClick", cell);
    }

    private void SetTestOutput ()
    {
      TestOutput.SetInfoForNormalBocList(JobList_Normal);
    }

    private BocListAsGridUserControlTestOutput TestOutput
    {
      get { return (BocListAsGridUserControlTestOutput)((Layout)Page.Master).GetTestOutputControl(); }
    }

    protected void ValidationTestCaseRow (object sender, EventArgs e)
    {
      var jobs = ((Person)CurrentObject.BusinessObject!).Jobs;
      AddValidationFailures(
          ValidationFailure.CreateObjectValidationFailure(
              jobs[0],
              "Row validation failure message",
              "Localized row validation failure message"));
    }

    protected void ValidationTestCaseCell (object sender, EventArgs e)
    {
      var jobs = ((Person)CurrentObject.BusinessObject!).Jobs;
      var displayNameProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Job _) => _.DisplayName));

      AddValidationFailures(
          ValidationFailure.CreatePropertyValidationFailure(
              jobs[0],
              displayNameProperty,
              jobs[0].DisplayName,
              "Cell validation failure message",
              "Localized cell validation failure message"));
    }

    private void AddValidationFailures (params ValidationFailure[] validationFailures)
    {
      _validationResult = BusinessObjectValidationResult.Create(new ValidationResult(validationFailures));
    }
  }
}
