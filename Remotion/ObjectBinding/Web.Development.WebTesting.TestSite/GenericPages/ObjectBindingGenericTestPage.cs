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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.TestSite.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.GenericPages
{
  public abstract class ObjectBindingGenericTestPage<TOptions> : GenericTestPageBase<TOptions>
  {
    /// <summary>
    /// <see cref="CustomValidator"/> returns false on <see cref="CustomValidator"/>.<see cref="CustomValidator.ControlPropertiesValid"/> for the <see cref="BocList"/>.
    /// Therefore we use this, to override the property to always return true.
    /// </summary>
    private class ControlPropertiesAlwaysTrueValidator : CustomValidator
    {
      protected override bool ControlPropertiesValid ()
      {
        return true;
      }
    }

    [NotNull]
    protected abstract TOptions FormGridControlOptions { get; }

    [NotNull]
    protected abstract TOptions FormGridWithReadonlyControlOptions { get; }

    [NotNull]
    protected abstract HtmlTable FormGridControlTable { get; }

    [NotNull]
    protected abstract HtmlTable FormGridWithReadonlyControlTable { get; }

    [NotNull]
    protected abstract TOptions OneControlOverMultipleRowsFormGridControlOptions { get; }

    [NotNull]
    protected abstract HtmlTable OneControlOverMultipleRowsFormGridTable { get; }

    [NotNull]
    protected abstract TOptions ShiftedColumnsFormGridControlOptions { get; }

    [NotNull]
    protected abstract PlaceHolder ShiftedColumnsFormGrid { get; }

    [NotNull]
    protected abstract TOptions FormGridMultiControlOptions1 { get; }

    [NotNull]
    protected abstract TOptions FormGridMultiControlOptions2 { get; }

    [NotNull]
    protected abstract PlaceHolder MultipleControlsFormGrid { get; }

    [NotNull]
    protected abstract TOptions ReadOnlyControlOptions { get; }

    [NotNull]
    protected abstract Control ReadOnlyControlPanel { get; }

    [NotNull]
    protected abstract HtmlTable FormGridValidationTable { get; }

    [NotNull]
    protected abstract TOptions CustomValidatedControlOptions { get; }

    [NotNull]
    protected abstract TOptions CustomValidatedReadOnlyControlOptions { get; }

    [NotNull]
    protected abstract TOptions MultipleValidatedControlOptions { get; }


    protected override void AddControlsOnInit (GenericTestPageType pageType, IGenericTestPage<TOptions> testPage)
    {
      base.AddControlsOnInit (pageType, testPage);

      if (pageType.HasFlag (GenericTestPageType.ReadOnlyElements))
        ReadOnlyControlPanel.Controls.Add (testPage.CreateControl (ReadOnlyControlOptions));

      if (pageType.HasFlag (GenericTestPageType.EnabledFormGrid))
      {
        var shiftedColumnsFormGridTable = new HtmlTable();

        var shiftedColumnsTableRow = new HtmlTableRow();
        shiftedColumnsTableRow.Cells.Add (new HtmlTableCell());
        shiftedColumnsTableRow.Cells.Add (new HtmlTableCell());
        shiftedColumnsTableRow.Cells.Add (new HtmlTableCell());
        shiftedColumnsTableRow.Cells.Add (new HtmlTableCell());
        shiftedColumnsFormGridTable.Rows.Add (shiftedColumnsTableRow);
        shiftedColumnsFormGridTable.ID = "FormGridShifted";

        ShiftedColumnsFormGrid.Controls.Add (shiftedColumnsFormGridTable);

        var formGridManager1 = new FormGridManager();
        ShiftedColumnsFormGrid.Controls.Add (formGridManager1);
        formGridManager1.ID = "ShiftedFormGridManager";
        formGridManager1.FormGridSuffix = "Shifted";
        formGridManager1.ControlsColumn = 3;
        formGridManager1.LabelsColumn = 2;

        var formGridManager2 = new FormGridManager();
        MultipleControlsFormGrid.Controls.Add (formGridManager2);
        formGridManager2.ID = "MultiFormGridManager1";
        formGridManager2.FormGridSuffix = "Multi";

        var formGridManager3 = new FormGridManager();
        MultipleControlsFormGrid.Controls.Add (formGridManager3);
        formGridManager3.ID = "MultiFormGridManager2";
        formGridManager3.FormGridSuffix = "Multi";
        formGridManager3.ControlsColumn = 4;
        formGridManager3.LabelsColumn = 3;

        var formGridMultiTable = new HtmlTable();
        var formGridMultiTableRow = new HtmlTableRow();
        formGridMultiTableRow.Cells.Add (new HtmlTableCell());
        formGridMultiTableRow.Cells.Add (new HtmlTableCell());
        formGridMultiTableRow.Cells.Add (new HtmlTableCell());
        formGridMultiTableRow.Cells.Add (new HtmlTableCell());
        formGridMultiTableRow.Cells.Add (new HtmlTableCell());
        formGridMultiTable.Rows.Add (formGridMultiTableRow);
        formGridMultiTable.ID = "FormGridMulti";

        MultipleControlsFormGrid.Controls.Add (formGridMultiTable);

        FormGridControlTable.Rows[0].Cells[1].Controls.Add (testPage.CreateControl (FormGridControlOptions));
        FormGridWithReadonlyControlTable.Rows[0].Cells[1].Controls.Add (testPage.CreateControl (FormGridWithReadonlyControlOptions));
        OneControlOverMultipleRowsFormGridTable.Rows[1].Cells[0].Controls.Add (testPage.CreateControl (OneControlOverMultipleRowsFormGridControlOptions));
        shiftedColumnsFormGridTable.Rows[0].Cells[3].Controls.Add (testPage.CreateControl (ShiftedColumnsFormGridControlOptions));
        formGridMultiTable.Rows[0].Cells[1].Controls.Add (testPage.CreateControl (FormGridMultiControlOptions1));

        var secondControlInMultiTable = testPage.CreateControl (FormGridMultiControlOptions2);
        formGridMultiTable.Rows[0].Cells[4].Controls.Add (secondControlInMultiTable);

        // Label is not created automatically
        var labelsControl = new SmartLabel();
        labelsControl.ForControl = secondControlInMultiTable.ID;
        labelsControl.ID = secondControlInMultiTable.ID + "_Label";
        labelsControl.EnableViewState = true;

        formGridMultiTable.Rows[0].Cells[3].Controls.Add (labelsControl);
      }

      if (pageType.HasFlag (GenericTestPageType.EnabledValidation))
      {
        FormGridValidationTable.Rows[1].Cells[1].Controls.Add (testPage.CreateControl (FormGridControlOptions));

        var customValidatedControl = testPage.CreateControl (CustomValidatedControlOptions);
        FormGridValidationTable.Rows[3].Cells[1].Controls.Add (customValidatedControl);

        var customValidatorForCustomValidatedControl = CreateCustomValidator (customValidatedControl, "Always false.");
        FormGridValidationTable.Rows[3].Cells[1].Controls.Add (customValidatorForCustomValidatedControl);


        var multipleValidatedControl = testPage.CreateControl (MultipleValidatedControlOptions);
        FormGridValidationTable.Rows[5].Cells[1].Controls.Add (multipleValidatedControl);

        var customValidatorForMultipleValidatedControl1 = CreateCustomValidator (multipleValidatedControl, "Always false.");
        FormGridValidationTable.Rows[5].Cells[1].Controls.Add (customValidatorForMultipleValidatedControl1);

        var customValidatorForMultipleValidatedControl2 = CreateCustomValidator (multipleValidatedControl, "Always false. The second.");
        FormGridValidationTable.Rows[5].Cells[1].Controls.Add (customValidatorForMultipleValidatedControl2);

        var customValidatedReadOnlyControl = testPage.CreateControl (CustomValidatedReadOnlyControlOptions);
        FormGridValidationTable.Rows[7].Cells[1].Controls.Add (customValidatedReadOnlyControl);

        var customValidatorForCustomValidatedReadOnlyControl = CreateCustomValidator (customValidatedReadOnlyControl, "Always false.");
        FormGridValidationTable.Rows[7].Cells[1].Controls.Add (customValidatorForCustomValidatedReadOnlyControl);
      }
    }

    private ControlPropertiesAlwaysTrueValidator CreateCustomValidator (Control customValidatedControl, string errorMessage)
    {
      var customValidator =
          new ControlPropertiesAlwaysTrueValidator { ControlToValidate = customValidatedControl.ID, ValidateEmptyText = true, ErrorMessage = errorMessage };

      customValidator.ServerValidate += (source, args) => args.IsValid = false;
      return customValidator;
    }
  }
}