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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.GenericPages;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
{
  public partial class GenericTest : ObjectBindingGenericTestPage<GenericTestOptions>
  {
    private readonly GenericTestPageParameterCollection _parameters = new GenericTestPageParameterCollection();

    private GenericTestOptions _ambiguousControlOptions;
    private GenericTestOptions _disabledControlOptions;
    private GenericTestOptions _readOnlyControlOptions;
    private GenericTestOptions _hiddenControlOptions;
    private GenericTestOptions _visibleControlOptions;
    private GenericTestOptions _formGridControlOptions;
    private GenericTestOptions _formGridWithReadonlyControlOptions;
    private GenericTestOptions _oneControlOverMultipleRowsFormGridControlOptions;
    private GenericTestOptions _formGridMultiControlOptions1;
    private GenericTestOptions _formGridMultiControlOptions2;
    private GenericTestOptions _shiftedColumnsFormGridControlOptions;
    private GenericTestOptions _customValidatedControlOptions;
    private GenericTestOptions _multipleValidatedControlOptions;

    public GenericTest ()
    {
      Register ("autoCompleteReferenceValue", new BocAutoCompleteReferenceValueGenericTestPage());
      Register ("booleanValue", new BocBooleanValueGenericTestPage());
      Register ("checkBox", new BocCheckBoxValueGenericTestPage());
      Register ("dateTimeValue", new BocDateTimeValueGenericTestPage());
      Register ("dropDownList", new BocEnumValueGenericTestPage(ListControlType.DropDownList));
      Register ("list", new BocListGenericTestPage());
      Register ("listAsGrid", new BocListAsGridGenericTestPage());
      Register ("listBox", new BocEnumValueGenericTestPage(ListControlType.ListBox));
      Register ("multilineText", new BocMultilineTextValueGenericTestPage());
      Register ("radioButtonList", new BocEnumValueGenericTestPage(ListControlType.RadioButtonList));
      Register ("referenceValue", new BocReferenceValueGenericTestPage());
      Register ("textValue", new BocTextValueGenericTestPage());
      Register ("treeView", new BocTreeViewGenericTestPage());
    }

    /// <inheritdoc />
    protected override GenericTestOptions AmbiguousControlOptions
    {
      get { return _ambiguousControlOptions; }
    }

    /// <inheritdoc />
    protected override Control AmbiguousControlPanel
    {
      get { return PanelAmbiguousControl; }
    }

    /// <inheritdoc />
    protected override GenericTestOptions FormGridControlOptions
    {
      get { return _formGridControlOptions; }
    }

    /// <inheritdoc />
    protected override GenericTestOptions FormGridWithReadonlyControlOptions
    {
      get { return _formGridWithReadonlyControlOptions; }
    }

    /// <inheritdoc />
    protected override HtmlTable FormGridControlTable
    {
      get { return FormGrid; }
    }

    protected override HtmlTable FormGridWithReadonlyControlTable
    {
      get { return ReadonlyControlFormGrid; }
    }

    protected override GenericTestOptions OneControlOverMultipleRowsFormGridControlOptions
    {
      get { return _oneControlOverMultipleRowsFormGridControlOptions; }
    }

    protected override HtmlTable OneControlOverMultipleRowsFormGridTable
    {
      get { return OneControlOverMultipleRowsFormGrid; }
    }

    protected override GenericTestOptions ShiftedColumnsFormGridControlOptions
    {
      get { return _shiftedColumnsFormGridControlOptions; }
    }

    protected override PlaceHolder ShiftedColumnsFormGrid
    {
      get { return ShiftedColumnsFormGridPlaceHolder; }
    }

    protected override GenericTestOptions FormGridMultiControlOptions1
    {
      get { return _formGridMultiControlOptions1; }
    }

    protected override GenericTestOptions FormGridMultiControlOptions2
    {
      get { return _formGridMultiControlOptions2; }
    }

    protected override PlaceHolder MultipleControlsFormGrid
    {
      get { return MultipleControlsFormGridPlaceHolder; }
    }

    /// <inheritdoc />
    protected override GenericTestOptions DisabledControlOptions
    {
      get { return _disabledControlOptions; }
    }

    /// <inheritdoc />
    protected override Control DisabledControlPanel
    {
      get { return PanelDisabledControl; }
    }

    /// <inheritdoc />
    protected override GenericTestOptions ReadOnlyControlOptions
    {
      get { return _readOnlyControlOptions; }
    }

    /// <inheritdoc />
    protected override Control ReadOnlyControlPanel
    {
      get { return PanelReadOnlyControl; }
    }

    /// <inheritdoc />
    protected override GenericTestOptions HiddenControlOptions
    {
      get { return _hiddenControlOptions; }
    }

    /// <inheritdoc />
    protected override Control HiddenControlPanel
    {
      get { return PanelHiddenControl; }
    }

    /// <inheritdoc />
    protected override GenericTestPageParameterCollection Parameters
    {
      get { return _parameters; }
    }

    /// <inheritdoc />
    protected override GenericTestOptions VisibleControlOptions
    {
      get { return _visibleControlOptions; }
    }

    /// <inheritdoc />
    protected override Control VisibleControlPanel
    {
      get { return PanelVisibleControl; }
    }

    /// <inheritdoc />
    protected override HtmlTable FormGridValidationTable
    {
      get { return FormGridValidation; }
    }

    /// <inheritdoc />
    protected override GenericTestOptions CustomValidatedControlOptions
    {
      get { return _customValidatedControlOptions; }
    }

    /// <inheritdoc />
    protected override GenericTestOptions MultipleValidatedControlOptions
    {
      get { return _multipleValidatedControlOptions; }
    }

    /// <inheritdoc />
    protected override void OnInit (EventArgs e)
    {
      // Constants for all the controls on this generic page
      const string ambiguousID = "AmbiguousControl";
      const string disabledID = "DisabledControl";
      const string readonlyID = "ReadOnlyControl";
      const string hiddenID = "HiddenControl";
      const string visibleID = "VisibleControl";
      const string visibleIndex = "1", hiddenIndex = "133";

      const string controlInNormalFormGridID = "ControlInFormGrid";
      const string readonlyControlInNormalFormGridID = "ReadonlyControlInFormGrid";
      const string oneControlOverMultipleRowsFormGridID = "ControlInSecondRowFormGrid";
      const string shiftedColumnsControlFormGridID = "ColumnsShiftedControlFormGrid";
      const string controlInMultiFormGridID1 = "ControlInMultiFormGridID1";
      const string controlInMultiFormGridID2 = "ControlInMultiFormGridID2";
      const string correctDomainProperty = "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample";
      const string incorrectDomainProperty = "Remotion.ObjectBinding.Sample.Job, Remotion.ObjectBinding.Sample";

      const string customValidatedControlInFormGridID = "CustomValidatedControlInFormGrid";
      const string multipleValidatedControlInFormGridID = "MultipleValidatedControlInFormGrid";

      // "Real" HTML ids of the controls
      var ambiguousHtmlID = string.Concat ("body_", ambiguousID);
      var disabledHtmlID = string.Concat ("body_", disabledID);
      var readonlyHtmlID = string.Concat ("body_", readonlyID);
      var hiddenHtmlID = string.Concat ("body_", hiddenID);
      var visibleHtmlID = string.Concat ("body_", visibleID);

      var controlInFormGridHtmlID = string.Concat ("body_", controlInNormalFormGridID);
      var readonlyControlInFormGridHtmlID = string.Concat ("body_", readonlyControlInNormalFormGridID);
      var oneControlOverMultipleRowsFormGridHtmlID = string.Concat ("body_", oneControlOverMultipleRowsFormGridID);
      var shiftedColumnsControlFormGridHtmlID = string.Concat ("body_", shiftedColumnsControlFormGridID);
      var controlInMultiFormGridIDHtml1 = string.Concat ("body_", controlInMultiFormGridID1);
      var controlInMultiFormGridIDHtml2 = string.Concat ("body_", controlInMultiFormGridID2);

      var customValidatedControlInFormGridHtmlID = string.Concat ("body_", customValidatedControlInFormGridID);
      var multipleValidatedControlInFormGridHtmlID = string.Concat ("body_", multipleValidatedControlInFormGridID);


      // Options for creating the controls
      _ambiguousControlOptions = new GenericTestOptions (
          ambiguousID,
          ambiguousHtmlID,
          DataSource.ID,
          correctDomainProperty,
          incorrectDomainProperty);
      _disabledControlOptions = new GenericTestOptions (
          disabledID,
          disabledHtmlID,
          DataSource.ID,
          correctDomainProperty,
          incorrectDomainProperty,
          EnabledState.Disabled);
      _readOnlyControlOptions = new GenericTestOptions (
          readonlyID,
          readonlyHtmlID,
          DataSource.ID,
          correctDomainProperty,
          incorrectDomainProperty,
          EnabledState.Enabled,
          ReadOnlyState.ReadOnly);
      _hiddenControlOptions = new GenericTestOptions (
          hiddenID,
          hiddenHtmlID,
          DataSource.ID,
          correctDomainProperty,
          incorrectDomainProperty);
      _visibleControlOptions = new GenericTestOptions (
          visibleID,
          visibleHtmlID,
          DataSource.ID,
          correctDomainProperty,
          incorrectDomainProperty);

      _formGridControlOptions = new GenericTestOptions (
          controlInNormalFormGridID,
          controlInFormGridHtmlID,
          DataSource.ID,
          correctDomainProperty,
          incorrectDomainProperty);

      _formGridWithReadonlyControlOptions = new GenericTestOptions (
          readonlyControlInNormalFormGridID,
          readonlyControlInFormGridHtmlID,
          DataSource.ID,
          correctDomainProperty,
          incorrectDomainProperty,
          EnabledState.Enabled,
          ReadOnlyState.ReadOnly);

      _shiftedColumnsFormGridControlOptions = new GenericTestOptions (
          shiftedColumnsControlFormGridID,
          shiftedColumnsControlFormGridHtmlID,
          DataSource.ID,
          correctDomainProperty,
          incorrectDomainProperty);

      _oneControlOverMultipleRowsFormGridControlOptions = new GenericTestOptions (
          oneControlOverMultipleRowsFormGridID,
          oneControlOverMultipleRowsFormGridHtmlID,
          DataSource.ID,
          correctDomainProperty,
          incorrectDomainProperty);
      
      _formGridMultiControlOptions1 = new GenericTestOptions (
          controlInMultiFormGridID1,
          controlInMultiFormGridIDHtml1,
          DataSource.ID,
          correctDomainProperty,
          incorrectDomainProperty);

      _formGridMultiControlOptions2 = new GenericTestOptions (
          controlInMultiFormGridID2,
          controlInMultiFormGridIDHtml2,
          DataSource.ID,
          correctDomainProperty,
          incorrectDomainProperty);

      _customValidatedControlOptions = new GenericTestOptions (
          customValidatedControlInFormGridID,
          customValidatedControlInFormGridHtmlID,
          DataSource.ID,
          correctDomainProperty,
          incorrectDomainProperty);

      _multipleValidatedControlOptions = new GenericTestOptions (
          multipleValidatedControlInFormGridID,
          multipleValidatedControlInFormGridHtmlID,
          DataSource.ID,
          correctDomainProperty,
          incorrectDomainProperty);

      // Parameters which will be passed to the client
      _parameters.Add (TestConstants.HtmlIDSelectorID, visibleHtmlID, hiddenHtmlID);
      _parameters.Add (TestConstants.IndexSelectorID, visibleIndex, hiddenIndex, visibleHtmlID);
      _parameters.Add (TestConstants.LocalIDSelectorID, visibleID, hiddenID, visibleHtmlID);
      _parameters.Add (TestConstants.FirstSelectorID, visibleHtmlID);
      _parameters.Add (TestConstants.SingleSelectorID, visibleHtmlID);
      _parameters.Add (TestConstants.DisabledTestsID, visibleHtmlID, disabledHtmlID);
      _parameters.Add (TestConstants.ReadOnlyTestsID, visibleHtmlID, readonlyHtmlID);
      _parameters.Add (TestConstants.LabelTestsID, controlInFormGridHtmlID, readonlyControlInFormGridHtmlID, oneControlOverMultipleRowsFormGridHtmlID, shiftedColumnsControlFormGridHtmlID, controlInMultiFormGridIDHtml1, controlInMultiFormGridIDHtml2, visibleHtmlID);
      _parameters.Add (TestConstants.ValidationErrorTestsID, string.Concat ("body_", ValidateButton.ID), customValidatedControlInFormGridHtmlID, multipleValidatedControlInFormGridHtmlID, visibleHtmlID, controlInFormGridHtmlID, readonlyHtmlID);

      ValidateButton.Click += ValidateButton_Click;

      base.OnInit (e);
    }

    /// <inheritdoc />
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      DataSource.BusinessObject = (IBusinessObject) ((GenericTestFunction) CurrentFunction).Person;
      DataSource.LoadValues (IsReturningPostBack);
    }

    /// <inheritdoc />
    protected override void SetTestInformation (string information)
    {
      var master = Master as Layout;
      if (master == null)
        throw new InvalidOperationException ("The master page of the generic test page is not set.");
      master.SetTestInformation (information);
    }

    private void ValidateButton_Click (object sender, EventArgs e)
    {
      ValidateDataSources();
    }

    private void ValidateDataSources ()
    {
      PrepareValidation();

      FormGridManagerValidation.Validate();
    }
  }
}