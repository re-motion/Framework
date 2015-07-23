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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport
{
  [ToolboxItem (false)]
  public class EditModeController : PlaceHolder, IEditModeController
  {
    // types

    private enum EditMode
    {
      None,
      RowEditMode,
      ListEditMode,
    }

    // static members and constants

    private const string c_whiteSpace = "&nbsp;";

    // member fields

    private readonly IEditModeHost _editModeHost;

    private EditMode _editMode = EditMode.None;
    private List<string> _editedRowIDs;
    private List<BocListRow> _newRows;
    private bool _isEditNewRow;

    private bool _isEditModeRestored;

    private readonly List<EditableRow> _rows = new List<EditableRow>();
    private EditModeValidator _editModeValidator;

    // construction and disposing

    public EditModeController (IEditModeHost editModeHost)
    {
      ArgumentUtility.CheckNotNull ("editModeHost", editModeHost);

      _editModeHost = editModeHost;
    }

    // methods and properties

    public IEditableRow GetEditableRow (int index)
    {
      if (_editModeHost.Value == null || index >= _editModeHost.Value.Count)
        throw new ArgumentOutOfRangeException ("index", "The index must not point to an object past the elements in the Value collection");

      if (_editMode == EditMode.None)
        return null;

      var editedRowID = _editModeHost.RowIDProvider.GetItemRowID (new BocListRow (index, (IBusinessObject) _editModeHost.Value[index]));
      var editedIndex = _editedRowIDs.IndexOf (editedRowID);
      if (editedIndex == -1)
      {
        for (int i = 0; i < _editedRowIDs.Count; i++)
        {
          var oldID = _editedRowIDs[i];
          var bocListRow = _editModeHost.RowIDProvider.GetRowFromItemRowID (_editModeHost.Value, oldID);
          if (bocListRow != null)
          {
            var newID = _editModeHost.RowIDProvider.GetItemRowID (bocListRow);
            _editedRowIDs[i] = newID;
            if (newID == editedRowID)
              editedIndex = i;
          }
        }

        if (editedIndex == -1)
          return null;
      }

      return _rows[editedIndex];
    }

    public void SwitchRowIntoEditMode (int index, BocColumnDefinition[] columns)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

      if (_editModeHost.Value == null)
      {
        throw new InvalidOperationException (
            string.Format ("Cannot initialize row edit mode: The BocList '{0}' does not have a Value.", _editModeHost.ID));
      }

      if (index < 0)
        throw new ArgumentOutOfRangeException ("index");
      if (index >= _editModeHost.Value.Count)
        throw new ArgumentOutOfRangeException ("index");

      RestoreAndEndEditMode (columns);

      if (_editModeHost.IsReadOnly || IsListEditModeActive || IsRowEditModeActive)
        return;

      _editedRowIDs =
          new List<string> { _editModeHost.RowIDProvider.GetItemRowID (new BocListRow (index, (IBusinessObject) _editModeHost.Value[index])) };
      _editMode = EditMode.RowEditMode;
      CreateEditModeControls (columns);
      SetFocus (_rows.First());
      LoadValues (false);
    }

    public void SwitchListIntoEditMode (BocColumnDefinition[] columns)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

      if (_editModeHost.Value == null)
      {
        throw new InvalidOperationException (
            string.Format ("Cannot initialize list edit mode: The BocList '{0}' does not have a Value.", _editModeHost.ID));
      }

      RestoreAndEndEditMode (columns);

      if (_editModeHost.IsReadOnly || IsRowEditModeActive || IsListEditModeActive)
        return;

      _editedRowIDs =
          _editModeHost.Value.Cast<IBusinessObject>().Select ((o, i) => _editModeHost.RowIDProvider.GetItemRowID (new BocListRow (i, o))).ToList();
      _editMode = EditMode.ListEditMode;
      CreateEditModeControls (columns);
      if (_rows.Any())
        SetFocus (_rows.First());
      LoadValues (false);
    }

    public bool AddAndEditRow (IBusinessObject businessObject, BocColumnDefinition[] columns)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

      RestoreAndEndEditMode (columns);

      if (_editModeHost.IsReadOnly || IsListEditModeActive || IsRowEditModeActive)
        return false;

      int index = AddRow (businessObject, columns);
      if (index < 0)
        return false;

      SwitchRowIntoEditMode (index, columns);

      if (! IsRowEditModeActive)
      {
        throw new InvalidOperationException (
            string.Format ("BocList '{0}': Could not switch newly added row into edit mode.", _editModeHost.ID));
      }
      _isEditNewRow = true;
      return true;
    }

    private void RestoreAndEndEditMode (BocColumnDefinition[] columns)
    {
      EnsureEditModeRestored (columns);

      if (IsRowEditModeActive)
        EndRowEditMode (true, columns);
      else if (IsListEditModeActive)
        EndListEditMode (true, columns);
    }

    public void EndRowEditMode (bool saveChanges, BocColumnDefinition[] columns)
    {
      ArgumentUtility.CheckNotNull ("columns", columns);

      if (! IsRowEditModeActive)
        return;

      EnsureEditModeRestored (columns);

      if (! _editModeHost.IsReadOnly)
      {
        var editedRow = GetEditedRow();

        if (saveChanges)
        {
          OnEditableRowChangesSaving (editedRow.Index, editedRow.BusinessObject, _rows[0].GetDataSource(), _rows[0].GetEditControlsAsArray());

          bool isValid = Validate();
          if (! isValid)
            return;

          _editModeHost.IsDirty = IsDirty();

          _rows[0].GetDataSource().SaveValues (false);
          OnEditableRowChangesSaved (editedRow.Index, editedRow.BusinessObject);
        }
        else
        {
          OnEditableRowChangesCanceling (editedRow.Index, editedRow.BusinessObject, _rows[0].GetDataSource(), _rows[0].GetEditControlsAsArray());

          if (_isEditNewRow)
          {
            _editModeHost.RemoveRows (new[] { editedRow.BusinessObject });
            OnEditableRowChangesCanceled (-1, editedRow.BusinessObject);
          }
          else
            OnEditableRowChangesCanceled (editedRow.Index, editedRow.BusinessObject);
        }

        _editModeHost.EndRowEditModeCleanUp (editedRow.Index);
      }

      RemoveEditModeControls();
      _editMode = EditMode.None;
      _editedRowIDs = null;
      _isEditNewRow = false;
    }

    public void EndListEditMode (bool saveChanges, BocColumnDefinition[] columns)
    {
      ArgumentUtility.CheckNotNull ("columns", columns);
      
      if (! IsListEditModeActive)
        return;

      EnsureEditModeRestored (columns);

      if (! _editModeHost.IsReadOnly)
      {
        Assertion.IsNotNull (_editModeHost.Value, "BocList does not have a value.");

        var editedRows = _editedRowIDs.Select (
            (editedRowID, index) =>
                Maybe.ForValue (_editModeHost.RowIDProvider.GetRowFromItemRowID (_editModeHost.Value, editedRowID))
                    .Select (row => Tuple.Create (row.Index, row.BusinessObject))
                    .ValueOrDefault (Tuple.Create (-1, _rows[index].GetDataSource().BusinessObject))).ToArray();

        if (saveChanges)
        {
          for (int i = 0; i < _rows.Count; i++)
            OnEditableRowChangesSaving (editedRows[i].Item1, editedRows[i].Item2, _rows[i].GetDataSource(), _rows[i].GetEditControlsAsArray());

          bool isValid = Validate();
          if (! isValid)
            return;

          _editModeHost.IsDirty = IsDirty();

          for (int i = 0; i < _rows.Count; i++)
            _rows[i].GetDataSource().SaveValues (false);

          for (int i = 0; i < _rows.Count; i++)
            OnEditableRowChangesSaved (editedRows[i].Item1, editedRows[i].Item2);
        }
        else
        {
          for (int i = 0; i < _rows.Count; i++)
            OnEditableRowChangesCanceling (editedRows[i].Item1, editedRows[i].Item2, _rows[i].GetDataSource(), _rows[i].GetEditControlsAsArray());

          //if (_isEditNewRow)
          //{
          //  IBusinessObject editedBusinessObject = values[_editableRowIndex.Value];
          //  RemoveRow (_editableRowIndex.Value);
          //  OnRowEditModeCanceled (-1, editedBusinessObject);
          //}
          //else
          //{
          for (int i = 0; i < _rows.Count; i++)
            OnEditableRowChangesCanceled (editedRows[i].Item1, editedRows[i].Item2);
          //}
        }

        _editModeHost.EndListEditModeCleanUp();
      }

      RemoveEditModeControls();
      _editMode = EditMode.None;
      _editedRowIDs = null;
    }


    private void CreateEditModeControls (BocColumnDefinition[] columns)
    {
      EnsureChildControls();

      Assertion.IsTrue (_rows.Count == 0, "Populating the editable rows only happens after the last edit mode was ended.");
      Assertion.IsTrue (Controls.Count == 0, "Populating the editable rows only happens after the last edit mode was ended.");

      var missingRowIDs = new List<string>();

      foreach (var editedRowID in _editedRowIDs)
      {
        var bocListRow = _editModeHost.RowIDProvider.GetRowFromItemRowID (_editModeHost.Value, editedRowID);
        if (bocListRow == null)
        {
          if (IsListEditModeActive)
          {
            // Remove ViewState for missing row
            var missingRow = new EditableRow (_editModeHost);
            Controls.Add (missingRow);
            Controls.Remove (missingRow);

            missingRowIDs.Add (editedRowID);
          }
          else
          {
            throw new InvalidOperationException (
                string.Format (
                    "Cannot create edit mode controls for the row with ID '{1}'. The BocList '{0}' does not contain the row in its Value collection.",
                    _editModeHost.ID,
                    editedRowID));
          }
        }
        else
        {
          AddRowToDataStructure (bocListRow, columns);
        }
      }

      _editedRowIDs.RemoveAll (missingRowIDs.Contains);
      
      _newRows = new List<BocListRow>();
      if (IsListEditModeActive && _editedRowIDs.Count < _editModeHost.Value.Count)
      {
        var availableRows = _editModeHost.Value.Cast<IBusinessObject>().Select ((o, i) => new BocListRow (i, o));
        var editedRows = _editedRowIDs.Select (rowID => _editModeHost.RowIDProvider.GetRowFromItemRowID (_editModeHost.Value, rowID));
        var newRows = availableRows.Except (editedRows).ToList();

        foreach (var row in newRows)
        {
          _editedRowIDs.Add (_editModeHost.RowIDProvider.GetItemRowID (row));
          AddRowToDataStructure (row, columns);
          _newRows.Add (row);
        }
      }
    }

    private EditableRow CreateEditableRow (BocListRow bocListRow, BocColumnDefinition[] columns)
    {
      EditableRow row = new EditableRow (_editModeHost);
      row.ID = ID + "_Row_" + _editModeHost.RowIDProvider.GetControlRowID (bocListRow);

      row.DataSourceFactory = _editModeHost.EditModeDataSourceFactory;
      row.ControlFactory = _editModeHost.EditModeControlFactory;

      row.CreateControls (bocListRow.BusinessObject, columns);

      return row;
    }

    private void SetFocus (EditableRow row)
    {
      var firstControl = row.GetEditControlsAsArray().OfType<IFocusableControl>().FirstOrDefault (c => !string.IsNullOrEmpty (c.FocusID));
      if (firstControl == null)
        return;

      _editModeHost.SetFocus (firstControl);
    }

    private void LoadValues (bool interim)
    {
      Assertion.IsNotNull (_editModeHost.Value, "BocList does not have a value.");
      if (IsListEditModeActive)
        Assertion.IsTrue (_editModeHost.Value.Count == _rows.Count, "Number of rows in BocList differs from rows in ListEditMode.");

      var newRows = _newRows.ToDictionary (r => r.BusinessObject);
      foreach (var dataSource in _rows.Select (r => r.GetDataSource()))
        dataSource.LoadValues (interim && !newRows.ContainsKey (dataSource.BusinessObject));
      _newRows.Clear();
    }

    public void EnsureEditModeRestored (BocColumnDefinition[] columns)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

      if (_isEditModeRestored)
        return;
      _isEditModeRestored = true;

      if (IsRowEditModeActive || IsListEditModeActive)
      {
        if (_editModeHost.Value == null)
        {
          throw new InvalidOperationException (
              string.Format ("Cannot restore edit mode: The BocList '{0}' does not have a Value.", _editModeHost.ID));
        }
        CreateEditModeControls (columns);
        LoadValues (true);
      }
    }

    private void RemoveEditModeControls ()
    {
      for (int i = _rows.Count - 1; i >= 0; i--)
        RemoveRowFromDataStructure (i);
    }

    public BocListRow[] AddRows (IBusinessObject[] businessObjects, BocColumnDefinition[] columns)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("businessObjects", businessObjects);
      ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

      var bocListRows = _editModeHost.AddRows (businessObjects);

      if (_editModeHost.Value != null)
      {
        EnsureEditModeRestored (columns);
        if (IsListEditModeActive)
        {
          var newRows = new List<EditableRow>();
          foreach (var bocListRow in bocListRows.OrderBy (r=>r.Index))
          {
            var newRow = AddRowToDataStructure (bocListRow, columns);
            _editedRowIDs.Add (_editModeHost.RowIDProvider.GetItemRowID (bocListRow));
            newRow.GetDataSource().LoadValues (false);
            newRows.Add (newRow);
          }
          if (newRows.Any())
            SetFocus (newRows.First());
        }
      }

      return bocListRows;
    }

    public int AddRow (IBusinessObject businessObject, BocColumnDefinition[] columns)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

      var bocListRows = AddRows (new[] { businessObject }, columns);

      if (bocListRows.Length == 0)
        return -1;
      return bocListRows.Single().Index;
    }

    private EditableRow AddRowToDataStructure (BocListRow bocListRow, BocColumnDefinition[] columns)
    {
      EditableRow row = CreateEditableRow (bocListRow, columns);
      Controls.Add (row);
      _rows.Add (row);

      return row;
    }

    public void RemoveRows (IBusinessObject[] businessObjects)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("businessObjects", businessObjects);

      var bocListRows = _editModeHost.RemoveRows (businessObjects);

      if (_editModeHost.Value != null)
      {
        if (IsRowEditModeActive)
        {
          throw new InvalidOperationException (
              string.Format (
                  "Cannot remove rows while the BocList '{0}' is in row edit mode. Call EndEditMode() before removing the rows.",
                  _editModeHost.ID));
        }
        else if (IsListEditModeActive)
        {
          foreach (var row in bocListRows.OrderByDescending (r => r.Index))
          {
            RemoveRowFromDataStructure (row.Index);
            _editedRowIDs.RemoveAt (row.Index);
          }
        }
      }
    }

    public void RemoveRow (IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      RemoveRows (new[] { businessObject });
    }

    private void RemoveRowFromDataStructure (int index)
    {
      EditableRow row = _rows[index];
      row.RemoveControls();
      Controls.Remove (row);
      _rows.RemoveAt (index);
    }

    public bool IsRowEditModeActive
    {
      get { return _editMode == EditMode.RowEditMode; }
    }

    public bool IsListEditModeActive
    {
      get { return _editMode == EditMode.ListEditMode; }
    }

    public BocListRow GetEditedRow()
    {
      if (!IsRowEditModeActive)
      {
        throw new InvalidOperationException (
            string.Format ("Cannot retrieve edited row: The BocList '{0}' is not in row edit mode.", _editModeHost.ID));
      }

      if (_editModeHost.Value == null)
      {
        throw new InvalidOperationException (string.Format ("Cannot retrieve edited row: The BocList '{0}' does not have a Value.", _editModeHost.ID));
      }

      var editedRow = _editModeHost.RowIDProvider.GetRowFromItemRowID (_editModeHost.Value, _editedRowIDs.Single());
      if (editedRow == null)
      {
        throw new InvalidOperationException (
            string.Format (
                "Cannot retrieve edited row: The BocList '{0}' no longer contains the edited row in its Value collection.", _editModeHost.ID));
      }

      return editedRow;
    }

    public IEnumerable<BaseValidator> CreateValidators (bool isReadOnly, IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

      _editModeValidator = null;

      if (isReadOnly)
        yield break;

      if ((IsListEditModeActive || IsRowEditModeActive) && _editModeHost.EnableEditModeValidator)
      {
        _editModeValidator = CreateEditModeValidator (resourceManager);
        yield return _editModeValidator;
      }
    }

    private EditModeValidator CreateEditModeValidator (IResourceManager resourceManager)
    {
      EditModeValidator editModeValidator = new EditModeValidator (this);
      editModeValidator.ID = ID + "_ValidatorEditMode";
      editModeValidator.ControlToValidate = _editModeHost.ID;
      if (string.IsNullOrEmpty (_editModeHost.ErrorMessage))
      {
        if (IsRowEditModeActive)
          editModeValidator.ErrorMessage = resourceManager.GetString (UI.Controls.BocList.ResourceIdentifier.RowEditModeErrorMessage);
        else if (IsListEditModeActive)
          editModeValidator.ErrorMessage = resourceManager.GetString (UI.Controls.BocList.ResourceIdentifier.ListEditModeErrorMessage);
      }
      else
        editModeValidator.ErrorMessage = _editModeHost.ErrorMessage;
      return editModeValidator;
    }

    /// <remarks>
    ///   Validators must be added to the controls collection after LoadPostData is complete.
    ///   If not, invalid validators will know that they are invalid without first calling validate.
    /// </remarks>
    public void EnsureValidatorsRestored ()
    {
      if (IsRowEditModeActive || IsListEditModeActive)
      {
        for (int i = 0; i < _rows.Count; i++)
          _rows[i].EnsureValidatorsRestored();
      }
    }

    public void PrepareValidation ()
    {
      if (IsRowEditModeActive || IsListEditModeActive)
      {
        for (int i = 0; i < _rows.Count; i++)
          _rows[i].PrepareValidation();
      }
    }

    public bool Validate ()
    {
      EnsureValidatorsRestored();

      bool isValid = true;

      if (IsRowEditModeActive || IsListEditModeActive)
      {
        for (int i = 0; i < _rows.Count; i++)
          isValid &= _rows[i].Validate();

        isValid &= _editModeHost.ValidateEditableRows();
      }

      return isValid;
    }

    public void UpdateValidationErrorMessage (string value)
    {
      if (_editModeValidator != null)
        _editModeValidator.ErrorMessage = value;
    }

    public EditModeValidator GetEditModeValidator ()
    {
      return _editModeValidator;
    }

    public void RenderTitleCellMarkers (HtmlTextWriter writer, BocColumnDefinition column, int columnIndex)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("column", column);

      if (_editModeHost.ShowEditModeRequiredMarkers && IsRequired (columnIndex))
      {
        Image requriedFieldMarker = _editModeHost.GetRequiredMarker();
        requriedFieldMarker.RenderControl (writer);
        writer.Write (c_whiteSpace);
      }
    }

    public bool IsRequired (int columnIndex)
    {
      if (IsRowEditModeActive || IsListEditModeActive)
      {
        for (int i = 0; i < _rows.Count; i++)
        {
          if (_rows[i].IsRequired (columnIndex))
            return true;
        }
      }

      return false;
    }


    public bool IsDirty ()
    {
      if (IsRowEditModeActive || IsListEditModeActive)
      {
        for (int i = 0; i < _rows.Count; i++)
        {
          if (_rows[i].IsDirty())
            return true;
        }
      }

      return false;
    }

    public string[] GetTrackedClientIDs ()
    {
      if (IsRowEditModeActive || IsListEditModeActive)
      {
        StringCollection trackedIDs = new StringCollection();
        for (int i = 0; i < _rows.Count; i++)
          trackedIDs.AddRange (_rows[i].GetTrackedClientIDs());

        string[] trackedIDsArray = new string[trackedIDs.Count];
        trackedIDs.CopyTo (trackedIDsArray, 0);
        return trackedIDsArray;
      }
      else
        return new string[0];
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      if (!ControlHelper.IsDesignMode (this))
        Page.RegisterRequiresControlState (this);
    }

    protected override void LoadControlState (object savedState)
    {
      if (savedState == null)
        base.LoadControlState (null);
      else
      {
        object[] values = (object[]) savedState;

        base.LoadControlState (values[0]);
        _editMode = (EditMode) values[1];
        _editedRowIDs = (List<string>) values[2];
        _isEditNewRow = (bool) values[3];
      }
    }

    protected override object SaveControlState ()
    {
      object[] values = new object[4];

      values[0] = base.SaveControlState();
      values[1] = _editMode;
      values[2] = _editedRowIDs;
      values[3] = _isEditNewRow;

      return values;
    }


    protected virtual void OnEditableRowChangesSaving (
        int index,
        IBusinessObject businessObject,
        IBusinessObjectDataSource dataSource,
        IBusinessObjectBoundEditableWebControl[] controls)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      ArgumentUtility.CheckNotNull ("dataSource", dataSource);
      ArgumentUtility.CheckNotNull ("controls", controls);

      _editModeHost.OnEditableRowChangesSaving (index, businessObject, dataSource, controls);
    }

    protected virtual void OnEditableRowChangesSaved (int index, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      _editModeHost.OnEditableRowChangesSaved (index, businessObject);
    }

    protected virtual void OnEditableRowChangesCanceling (
        int index,
        IBusinessObject businessObject,
        IBusinessObjectDataSource dataSource,
        IBusinessObjectBoundEditableWebControl[] controls)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      ArgumentUtility.CheckNotNull ("dataSource", dataSource);
      ArgumentUtility.CheckNotNull ("controls", controls);

      _editModeHost.OnEditableRowChangesCanceling (index, businessObject, dataSource, controls);
    }

    protected virtual void OnEditableRowChangesCanceled (int index, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      _editModeHost.OnEditableRowChangesCanceled (index, businessObject);
    }

    IPage IControl.Page
    {
      get { return PageWrapper.CastOrCreate (base.Page); }
    }
  }
}