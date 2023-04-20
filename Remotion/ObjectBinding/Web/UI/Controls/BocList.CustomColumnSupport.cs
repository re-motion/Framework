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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public partial class BocList
  {
    private bool _customColumnsInitialized;

    private readonly Dictionary<BocCustomColumnDefinition, BocListCustomColumnTuple[]> _customColumnControls =
        new Dictionary<BocCustomColumnDefinition, BocListCustomColumnTuple[]>();

    private readonly PlaceHolder _customColumnsPlaceHolder = new PlaceHolder();

    private void CreateChildControlsForCustomColumns ()
    {
      Controls.Add(_customColumnsPlaceHolder);
    }

    private void ResetCustomColumns ()
    {
      _customColumnControls.Clear();
      _customColumnsPlaceHolder.Controls.Clear();
      _customColumnsInitialized = false;
    }

    private void EnsureCustomColumnsInitialized (IReadOnlyList<BocColumnDefinition> columns)
    {
      if (_customColumnsInitialized)
        return;

      _customColumnsInitialized = true;

      if (!HasValue)
        return;

      CreateCustomColumnControls(columns);
      InitCustomColumns();
      LoadCustomColumns();
    }

    /// <summary> Creates the controls for the custom columns in the <paramref name="columns"/> array. </summary>
    private void CreateCustomColumnControls (IReadOnlyList<BocColumnDefinition> columns)
    {
      EnsureChildControls();

      Assertion.IsTrue(_customColumnControls.Count == 0);
      Assertion.IsTrue(_customColumnsPlaceHolder.Controls.Count == 0);

      var controlEnabledCustomColumns = columns
          .Select((column, index) => new { Column = column as BocCustomColumnDefinition, Index = index })
          .Where(d => d.Column != null)
          .Where(
              d => d.Column!.Mode == BocCustomColumnDefinitionMode.ControlsInAllRows
                   || d.Column.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow)
          .ToArray();

      foreach (var customColumnData in controlEnabledCustomColumns)
      {
        var customColumn = customColumnData.Column!;
        var placeHolder = new PlaceHolder();

        var customColumnTuples = new List<BocListCustomColumnTuple>();
        foreach (var row in EnsureBocListRowsForCurrentPageGot())
        {
          bool isEditedRow = _editModeController.IsRowEditModeActive && _editModeController.GetEditableRow(row.ValueRow.Index) != null;
          if (customColumn.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow && !isEditedRow)
            continue;

          var args = new BocCustomCellArguments(this, customColumn);
          Control control = customColumn.CustomCell.CreateControlInternal(args);
          control.ID = ID + "_CustomColumnControl_" + customColumnData.Index + "_" + RowIDProvider.GetControlRowID(row.ValueRow);
          placeHolder.Controls.Add(control);
          customColumnTuples.Add(new BocListCustomColumnTuple(row.ValueRow.BusinessObject, row.ValueRow.Index, control));
        }
        _customColumnsPlaceHolder.Controls.Add(placeHolder);
        _customColumnControls[customColumn] = customColumnTuples.ToArray();
      }
    }

    /// <summary> Invokes the <see cref="BocCustomColumnDefinitionCell.Init"/> method for each custom column. </summary>
    private void InitCustomColumns ()
    {
      foreach (var keyValuePair in _customColumnControls.Where(p => p.Value.Any()))
      {
        var customColumn = keyValuePair.Key;
        var args = new BocCustomCellArguments(this, customColumn);
        customColumn.CustomCell.Init(args);
      }
    }

    /// <summary>
    ///   Invokes the <see cref="BocCustomColumnDefinitionCell.Load"/> method for each cell with a control in the custom columns. 
    /// </summary>
    private void LoadCustomColumns ()
    {
      foreach (var keyValuePair in _customColumnControls)
      {
        var customColumn = keyValuePair.Key;
        var customColumnTuples = keyValuePair.Value;
        foreach (var customColumnTuple in customColumnTuples)
        {
          int originalRowIndex = customColumnTuple.Item2;
          IBusinessObject businessObject = customColumnTuple.Item1;
          Control control = customColumnTuple.Item3;

          var args = new BocCustomCellLoadArguments(this, businessObject, customColumn, originalRowIndex, control);
          customColumn.CustomCell.Load(args);
        }
      }
    }

    private bool ValidateCustomColumns ()
    {
      bool isValid = true;

      foreach (var keyValuePair in _customColumnControls.Where(p => p.Key.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow))
      {
        var customColumn = keyValuePair.Key;
        var customColumnTuples = keyValuePair.Value;
        foreach (var customColumnTuple in customColumnTuples)
        {
          IBusinessObject businessObject = customColumnTuple.Item1;
          Control control = customColumnTuple.Item3;
          var args = new BocCustomCellValidationArguments(this, businessObject, customColumn, control);
          customColumn.CustomCell.Validate(args);
          isValid &= args.IsValid;
        }
      }
      return isValid;
    }

    /// <summary> 
    /// Invokes the <see cref="BocCustomColumnDefinitionCell.PreRender"/> method for each custom column. 
    /// Used by postback-links which must be registered for synchronous postbacks.
    /// </summary>
    private void PreRenderCustomColumns ()
    {
      var columns = EnsureColumnsGot();
      for (int index = 0; index < columns.Count; index++)
      {
        var column = columns[index];
        var customColumn = column as BocCustomColumnDefinition;
        if (customColumn == null)
          continue;
        var args = new BocCustomCellPreRenderArguments(this, customColumn, index);
        customColumn.CustomCell.PreRender(args);
      }
    }
  }
}
