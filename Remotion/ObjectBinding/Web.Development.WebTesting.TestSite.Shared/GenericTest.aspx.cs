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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.GenericPages;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared
{
  public partial class GenericTest : ObjectBindingGenericTestPage<GenericTestOptions>
  {
    protected override Dictionary<string, GenericTestPageParameter> Parameters { get; } = new Dictionary<string, GenericTestPageParameter>();

    public GenericTest ()
    {
      Register("autoCompleteReferenceValue", new BocAutoCompleteReferenceValueGenericTestPage());
      Register("booleanValue", new BocBooleanValueGenericTestPage());
      Register("checkBox", new BocCheckBoxValueGenericTestPage());
      Register("dateTimeValue", new BocDateTimeValueGenericTestPage());
      Register("dropDownList", new BocEnumValueGenericTestPage(ListControlType.DropDownList));
      Register("list", new BocListGenericTestPage());
      Register("listAsGrid", new BocListAsGridGenericTestPage());
      Register("listBox", new BocEnumValueGenericTestPage(ListControlType.ListBox));
      Register("multilineText", new BocMultilineTextValueGenericTestPage());
      Register("radioButtonList", new BocEnumValueGenericTestPage(ListControlType.RadioButtonList));
      Register("referenceValue", new BocReferenceValueGenericTestPage());
      Register("textValue", new BocTextValueGenericTestPage());
      Register("treeView", new BocTreeViewGenericTestPage());
    }

    protected override GenericTestOptions AmbiguousControlOptions => OptionsFactory.CreateAmbiguous();
    protected override GenericTestOptions FormGridControlOptions => OptionsFactory.CreateFormGrid();
    protected override GenericTestOptions FormGridWithReadonlyControlOptions => OptionsFactory.CreateFormGridWithReadOnly();
    protected override GenericTestOptions OneControlOverMultipleRowsFormGridControlOptions => OptionsFactory.CreateOneControlOverMultipleRows();
    protected override GenericTestOptions ShiftedColumnsFormGridControlOptions => OptionsFactory.CreateShiftedColumnsFormGrid();
    protected override GenericTestOptions FormGridMultiControlOptions1 => OptionsFactory.CreateFormGridMulti1();
    protected override GenericTestOptions FormGridMultiControlOptions2 => OptionsFactory.CreateFormGridMulti2();
    protected override GenericTestOptions DisabledControlOptions => OptionsFactory.CreateDisabled();
    protected override GenericTestOptions ReadOnlyControlOptions => OptionsFactory.CreateReadOnly();
    protected override GenericTestOptions HiddenControlOptions => OptionsFactory.CreateHidden();
    protected override GenericTestOptions VisibleControlOptions => OptionsFactory.CreateVisible();
    protected override GenericTestOptions CustomValidatedControlOptions => OptionsFactory.CreateCustomValidated();
    protected override GenericTestOptions CustomValidatedReadOnlyControlOptions => OptionsFactory.CreateCustomValidatedReadOnly();
    protected override GenericTestOptions MultipleValidatedControlOptions => OptionsFactory.CreateMultipleValidated();

    protected override Control AmbiguousControlPanel => PanelAmbiguousControl;
    protected override HtmlTable FormGridControlTable => FormGrid;
    protected override HtmlTable FormGridWithReadonlyControlTable => ReadonlyControlFormGrid;
    protected override HtmlTable OneControlOverMultipleRowsFormGridTable => OneControlOverMultipleRowsFormGrid;
    protected override PlaceHolder ShiftedColumnsFormGrid => ShiftedColumnsFormGridPlaceHolder;
    protected override PlaceHolder MultipleControlsFormGrid => MultipleControlsFormGridPlaceHolder;
    protected override Control DisabledControlPanel => PanelDisabledControl;
    protected override Control ReadOnlyControlPanel => PanelReadOnlyControl;
    protected override Control HiddenControlPanel => PanelHiddenControl;
    protected override Control VisibleControlPanel => PanelVisibleControl;
    protected override HtmlTable FormGridValidationTable => FormGridValidation;

    private GenericTestOptionsFactory OptionsFactory
    {
      get
      {
        if (DataSource == null)
          throw new InvalidOperationException("DataSource has not been initialized yet.");
        return new GenericTestOptionsFactory(DataSource.ID);
      }
    }

    private ObjectBindingGenericTestPageParameterFactory ParameterFactory => new ObjectBindingGenericTestPageParameterFactory();

    protected override void OnInit (EventArgs e)
    {
      // Parameters which will be passed to the client
      Parameters.Add(ParameterFactory.CreateHtmlIDSelector());
      Parameters.Add(ParameterFactory.CreateIndexSelector());
      Parameters.Add(ParameterFactory.CreateLocalIdSelector());
      Parameters.Add(ParameterFactory.CreateFirstSelector());
      Parameters.Add(ParameterFactory.CreateSingleSelector());
      Parameters.Add(ParameterFactory.CreateDisabledTests());
      Parameters.Add(ParameterFactory.CreateReadOnlyTests());
      Parameters.Add(ParameterFactory.CreateLabelTests());
      Parameters.Add(ParameterFactory.CreateValidationErrorTests());

      ValidateButton.Click += ValidateButton_Click;

      base.OnInit(e);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad(e);

      DataSource.BusinessObject = (IBusinessObject) ((GenericTestFunction) CurrentFunction).Person;
      DataSource.LoadValues(IsReturningPostBack);
    }

    protected override void SetTestInformation (string information)
    {
      var master = Master as Layout;
      if (master == null)
        throw new InvalidOperationException("The master page of the generic test page is not set.");
      master.SetTestInformation(information);
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