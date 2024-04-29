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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Microsoft.Extensions.Logging;
using Remotion.Globalization;
using Remotion.Logging;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls.FormGridManagerImplementation;
using Remotion.Web.UI.Controls.Hotkey;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{

  /// <summary> Transforms one or more tables into form grids. </summary>
  /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/Class/*' />
  [NonVisualControl]
  [ToolboxItemFilter("System.Web.UI")]
  public class FormGridManager : Control, IControl, IResourceDispatchTarget, ISupportsPostLoadControl
  {
    // types

    /// <summary> A list of possible images/icons displayed in the Form Grid. </summary>
    /// <remarks> The symbol names map directly to the image's file names. </remarks>
    protected enum FormGridImage
    {
      /// <summary> Used for field's with a mandatory input. </summary>
      RequiredField,

      /// <summary> Used to indicate a help link. </summary>
      Help,

      /// <summary> Used if an entered value does not validate. </summary>
      ValidationError
    }

    /// <summary> A list of form grid manager wide resources. </summary>
    /// <remarks> Resources will be accessed using IResourceManager.GetString (Enum). </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.Web.Globalization.FormGridManager")]
    public enum ResourceIdentifier
    {
      /// <summary>The tool tip text for the required icon.</summary>
      RequiredFieldTitle,
      /// <summary>The tool tip text for the help icon.</summary>
      HelpTitle,
      /// <summary>The tool tip text for the validation icon.</summary>
      ValidationErrorInfoTitle,
    }

    protected enum TransformationStep
    {
      TransformationNotStarted = 0,
      PreLoadViewStateTransformationCompleted = 1,
      PostLoadTransformationCompleted = 2,
      PostValidationTransformationCompleted = 3,
      RenderTransformationCompleted = 4
    }

    /// <summary>
    ///   Wrapper class for a single HtmlTable plus the additional information
    ///   added through the <see cref="FormGridManager"/>.
    /// </summary>
    protected class FormGrid
    {
      /// <summary> The <see cref="HtmlTable"/> used as a base for the form grid. </summary>
      private HtmlTable _table;

      /// <summary> The <see cref="FormGridRow"/> collection for this <see cref="FormGrid"/>. </summary>
      private FormGridRowCollection _rows;

      /// <summary> The column normally containing the labels. </summary>
      private int _defaultLabelsColumn;

      /// <summary> The column normally containing the controls. </summary>
      private int _defaultControlsColumn;

      /// <summary> 
      ///   Initializes a new isntance of the <see cref="FormGrid"/> class with the 
      ///   <see cref="HtmlTable"/> used as a form grid, the <see cref="FormGridRow"/> array 
      ///   and the indeces of the columns normally containing the labels and the controls.
      /// </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGrid/Constructor/*' />
      public FormGrid (
          HtmlTable table,
          FormGridRow[] rows,
          int defaultLabelsColumn,
          int defaultControlsColumn)
      {
        ArgumentUtility.CheckNotNull("table", table);
        ArgumentUtility.CheckNotNull("rows", rows);

        _table = table;
        _defaultLabelsColumn = defaultLabelsColumn;
        _defaultControlsColumn = defaultControlsColumn;
        _rows = new FormGridRowCollection(this, rows);
      }

      /// <summary>
      ///   Returns all <see cref="ValidationError"/> objects defined in the 
      ///   <see cref="FormGridRow"/> objects collection.
      /// </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGrid/GetValidationErrors/*' />
      public virtual ValidationError[] GetValidationErrors ()
      {
        ArrayList validationErrorList = new ArrayList();

        for (int i = 0; i < _rows.Count; i++)
        {
          FormGridRow row = (FormGridRow)_rows[i];
          if (row.Type == FormGridRowType.DataRow)
            validationErrorList.AddRange(row.ValidationErrors);
        }

        return (ValidationError[])validationErrorList.ToArray(typeof(ValidationError));
      }

      /// <summary>
      ///   Searches through the <see cref="FormGridRow"/> objects collection for a validation error.
      /// </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGrid/HasValidationErrors/*' />
      public virtual bool HasValidationErrors ()
      {
        for (int i = 0; i < _rows.Count; i++)
        {
          FormGridRow row = (FormGridRow)_rows[i];
          if (    row.Type == FormGridRowType.DataRow
              &&  row.ValidationErrors.Length > 0)
          {
            return true;
          }
        }

        return false;
      }

      /// <summary>
      ///   Searches through the <see cref="FormGridRow"/> objects collection for a validation markers.
      /// </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGrid/HasValidationMarkers/*' />
      public virtual bool HasValidationMarkers ()
      {
        for (int i = 0; i < _rows.Count; i++)
        {
          FormGridRow row = (FormGridRow)_rows[i];
          if (    row.Type == FormGridRowType.DataRow
              &&  row.ValidationMarker != null)
          {
            return true;
          }
        }

        return false;
      }

      /// <summary>
      ///   Searches through the <see cref="FormGridRow"/> objects collection for a required markers.
      /// </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGrid/HasRequiredMarkers/*' />
      public virtual bool HasRequiredMarkers ()
      {
        for (int i = 0; i < _rows.Count; i++)
        {
          FormGridRow row = (FormGridRow)_rows[i];
          if (    row.Type == FormGridRowType.DataRow
              &&  row.RequiredMarker != null)
          {
              return true;
          }
        }

        return false;
      }

      /// <summary> Searches through the <see cref="FormGridRow"/> objects collection for a help providers. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGrid/HasHelpProviders/*' />
      public virtual bool HasHelpProviders ()
      {
        for (int i = 0; i < _rows.Count; i++)
        {
          FormGridRow row = (FormGridRow)_rows[i];
          if (    row.Type == FormGridRowType.DataRow
              &&  row.HelpProvider != null)
          {
              return true;
          }
        }

        return false;
      }

      /// <summary> Build the ID collection for this form grid. </summary>
      public virtual void BuildIDCollection ()
      {
        for (int i = 0; i < _rows.Count; i++)
        {
          FormGridRow row = (FormGridRow)_rows[i];
          row.BuildIDCollection();
        }
      }

      /// <summary>
      ///   Searches for a <see cref="FormGridRow"/> containing the specified <paramref name="id"/>.
      /// </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGrid/FormGridRow/*' />
      public FormGridRow? GetRowForID (string? id)
      {
        if (id == null || id == string.Empty)
          return null;

        for (int i = 0; i < _rows.Count; i++)
        {
          FormGridRow row = (FormGridRow)_rows[i];
          if (row.ContainsControlWithID(id))
            return row;
        }

        return null;
      }

      /// <summary>
      ///   Inserts a <see cref="FormGridRow"/> at the position specified by 
      ///   <paramref name="positionInFormGrid"/> and <paramref name="relatedRowID"/>.
      /// </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGrid/InsertNewFormGridRow/*' />
      public void InsertNewFormGridRow (
          FormGridRow newFormGridRow,
          string relatedRowID,
          FormGridRowInfo.RowPosition positionInFormGrid)
      {
        ArgumentUtility.CheckNotNull("newFormGridRow", newFormGridRow);

        FormGridRow? relatedRow = GetRowForID(relatedRowID);

        //  Not found, append to form grid instead of inserting at position of related form grid row
        if (relatedRow == null)
        {
          s_logger.LogWarning(
              $"Could not find control '{relatedRowID}' inside FormGrid (HtmlTable) '{_table.ID}' "
              + $"in naming container '{_table.NamingContainer.GetType().GetFullNameSafe()}' on page '{_table.Page!}'.");

          //  append html table rows
          for (int i = 0; i < newFormGridRow.HtmlTableRows.Count; i++)
          {
            HtmlTableRow newHtmlTableRow = (HtmlTableRow)newFormGridRow.HtmlTableRows[i];
            _table.Rows.Add(newHtmlTableRow);
          }
          //  append form grid row
          Rows.Add(newFormGridRow);
        }
          //  Insert after the related form grid row
        else if (positionInFormGrid == FormGridRowInfo.RowPosition.AfterRowWithID)
        {
          //  Find insertion postion for the html table rows

          int idxHtmlTableRow = 0;

          HtmlTableRow lastReleatedTableRow =
            relatedRow.HtmlTableRows[relatedRow.HtmlTableRows.Count - 1];

          //  Find position in html table
          for (; idxHtmlTableRow < _table.Rows.Count; idxHtmlTableRow++)
          {
            if (_table.Rows[idxHtmlTableRow] == lastReleatedTableRow)
            {
              //  We want to insert after the current position
              idxHtmlTableRow++;
              break;
            }
          }

          //  Insert the new html table rows
          for (int i = 0; i < newFormGridRow.HtmlTableRows.Count; i++)
          {
            HtmlTableRow newHtmlTableRow = (HtmlTableRow)newFormGridRow.HtmlTableRows[i];
            _table.Rows.Insert(idxHtmlTableRow, newHtmlTableRow);
            idxHtmlTableRow++;
          }


          //  Insert row into Form Grid

          int idxFormGridRow = Rows.IndexOf(relatedRow);
          //  After the index of the related row
          idxFormGridRow++;
          Rows.Insert(idxFormGridRow, newFormGridRow);
        }
          //  Insert before the related form grid row
        else if (positionInFormGrid == FormGridRowInfo.RowPosition.BeforeRowWithID)
        {
          //  Find insertion postion for the html table rows

          int idxHtmlTableRow = 0;
          HtmlTableRow firstReleatedTableRow = relatedRow.HtmlTableRows[0];

          //  Find position in html table
          for (; idxHtmlTableRow < _table.Rows.Count; idxHtmlTableRow++)
          {
            if (_table.Rows[idxHtmlTableRow] == firstReleatedTableRow)
              break;
          }

          //  Insert the new html table rows
          for (int i = 0; i < newFormGridRow.HtmlTableRows.Count; i++)
          {
            HtmlTableRow newHtmlTableRow = (HtmlTableRow)newFormGridRow.HtmlTableRows[i];
            _table.Rows.Insert(idxHtmlTableRow, newHtmlTableRow);
            idxHtmlTableRow++;
          }


          //  Insert row into Form Grid
          int idxFormGridRow = Rows.IndexOf(relatedRow);
          //  Before the related row
          Rows.Insert(idxFormGridRow, newFormGridRow);
        }

        newFormGridRow.BuildIDCollection();
      }

      /// <summary> The <see cref="HtmlTable"/> used as base for the form grid. </summary>
      public HtmlTable Table
      {
        get { return _table; }
      }

      /// <summary> The <see cref="FormGridRowCollection"/> for this <c>FormGrid</c>. </summary>
      public FormGridRowCollection Rows
      {
        get { return _rows; }
      }

      /// <summary> Gets or sets the index of the column used for labels. </summary>
      public int DefaultLabelsColumn
      {
        get { return _defaultLabelsColumn; }
        set { _defaultLabelsColumn = value; }
      }

      /// <summary> Gets or sets the index of the column used for controls. </summary>
      /// <remarks>
      ///   Note that controls using a seperate row may exist in the column <see cref="DefaultLabelsColumn"/>.
      /// </remarks>
      public int DefaultControlsColumn
      {
        get { return _defaultControlsColumn; }
        set { _defaultControlsColumn = value; }
      }
    }

    /// <summary> A collection of <see cref="FormGridRow"/> objects. </summary>
    protected sealed class FormGridRowCollection: CollectionBase, IList
    {
      /// <summary> The <see cref="FormGrid"/> to which this collection belongs to. </summary>
      private FormGrid _ownerFormGrid;

      /// <summary> Simple constructor. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRowCollection/Constructor/*' />
      public FormGridRowCollection (FormGrid ownerFormGrid, FormGridRow[] formGridRows)
      {
        ArgumentUtility.CheckNotNull("formGridRows", formGridRows);
        ArgumentUtility.CheckNotNull("ownerFormGrid", ownerFormGrid);

        _ownerFormGrid = ownerFormGrid;

        for (int i = 0; i < formGridRows.Length; i++)
        {
          if (formGridRows[i] == null)
            throw new ArgumentNullException("formGridRows[" + i + "]");
          formGridRows[i]._formGrid = _ownerFormGrid;
        }

        InnerList.AddRange(formGridRows);
      }

      /// <summary> A read only indexer for the <see cref="FormGridRow"/> objects. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRowCollection/Indexer/*' />
      public FormGridRow this [int index]
      {
        get
        {
          if (index < 0 || index >= InnerList.Count)
            throw new ArgumentOutOfRangeException("index");
          return (FormGridRow)InnerList[index]!;
        }
      }

      /// <summary> Allows only the insertion of objects of type of <see cref="FormGridRow"/>. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRowCollection/OnInsert/*' />
      protected override void OnInsert (int index, object? value)
      {
        ArgumentUtility.CheckNotNull("value", value!);
        FormGridRow formGridRow = ArgumentUtility.CheckType<FormGridRow>("value", value);

        if (formGridRow.HtmlTableRows[0].Parent != _ownerFormGrid.Table)
        {
          // TODO RM-8118: not null assertion
          throw new InvalidOperationException(
              $"The FormGridRow that attempted to be inserted at position {index} contains HtmlTableRows belonging to the table '{formGridRow.HtmlTableRows[0].Parent!.ID}',"
              + $" but the FormGrid encapsulates the table '{_ownerFormGrid.Table.ID}'.");
        }

        formGridRow._formGrid = _ownerFormGrid;
        base.OnInsert(index, value);
      }

      public int IndexOf (object? value)
      {
        return InnerList.IndexOf(value);
      }

      public void Insert (int index, object? value)
      {
        OnInsert(index, value);
        InnerList.Insert(index, value);
        OnInsertComplete(index, value);
      }

      public void Add (object value)
      {
        OnInsert(InnerList.Count, value);
        int index = InnerList.Add(value);
        OnInsertComplete(index, value);
     }
    }

    /// <summary>
    ///   Wrapper class for one or more <see cref="HtmlTableRows"/> forming a logical row in the 
    ///   <see cref="FormGrid"/> object's base <see cref="HtmlTable"/>.
    /// </summary>
    protected class FormGridRow
    {
      /// <summary> The <see cref="FormGrid"/> instance this <c>FormGridRow</c> is a part of. </summary>
      internal FormGrid _formGrid = null!;

      /// <summary> The <see cref="HtmlTableRow"/> collection for this <c>FormGridRow</c>. </summary>
      private ReadOnlyHtmlTableRowCollection _htmlTableRows;

      /// <summary> The type of this <c>FormGridRow</c>. </summary>
      private FormGridRowType _type;

      /// <summary> <see langword="true"/> if the row sould be rendered. </summary>
      private bool _visible;

      /// <summary> The <c>ValidationError</c> objects for this <c>FormGridRow</c>. </summary>
      private ValidationError[] _validationErrors;

      /// <summary> The validation marker for this <c>FormGridRow</c>. </summary>
      private Control? _validationMarker;

      /// <summary>The required marker for this <c>FormGridRow</c>. </summary>
      private Control? _requiredMarker;

      /// <summary> The help provider for this <c>FormGridRow</c>. </summary>
      private Control? _helpProvider;

      /// <summary> The index of the row containing the labels cell. </summary>
      private int _labelsRowIndex;

      /// <summary> The index of the row containing the controls cell. </summary>
      private int _controlsRowIndex;

      /// <summary> The index of the column normally containing the labels cell. </summary>
      private int _labelsColumn;

      /// <summary> The index of the column normally containing the controls cell. </summary>
      private int _controlsColumn;

      /// <summary> The cell containing the labels. </summary>
      private HtmlTableCell? _labelsCell;

      /// <summary> The cell containing the controls. </summary>
      private HtmlTableCell? _controlsCell;

      private bool _isGenerated;

      /// <summary>
      ///   The cell used as a place holder if the controls cell is not at the standard position.
      /// </summary>
      private HtmlTableCell? _controlsCellDummy;

      /// <summary> The cell containing the markers. </summary>
      private HtmlTableCell? _markersCell;

      /// <summary> The cell containing the validation messages. </summary>
      private HtmlTableCell? _validationMessagesCell;

      /// <summary>
      ///   The cell used as a place holder if the validation message cell is not at the standard
      ///   position.
      /// </summary>
      private HtmlTableCell? _validationMessagesCellDummy;

      /// <summary> The Web.UI.Controls in this <see cref="FormGridRow"/>, using the ID as key. </summary>
      private Hashtable _controls;

      /// <summary> Simple contructor. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/Constructor/*' />
      public FormGridRow (
        HtmlTableRow[] htmlTableRows,
        FormGridRowType type,
        int labelsColumn,
        int controlsColumn,
        bool isGenerated)
      {
        ArgumentUtility.CheckNotNullOrEmpty("htmlTableRows", htmlTableRows);

        _htmlTableRows = new ReadOnlyHtmlTableRowCollection(htmlTableRows);
        _type = type;
        _validationErrors = new ValidationError[]{};
        _labelsColumn = labelsColumn;
        _controlsColumn = controlsColumn;
        _visible = true;
        _controls = new Hashtable(0);
        _isGenerated = isGenerated;

        for (int index = 0; index < htmlTableRows.Length; index++)
        {
          if (htmlTableRows[index] == null)
            throw new ArgumentNullException("htmlTableRows[" + index + "]");
        }
      }

      /// <summary> Set the labels cell for this <see cref="FormGridRow"/>. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/SetLabelsCell/*' />
      public virtual HtmlTableCell SetLabelsCell (int rowIndex, int cellIndex)
      {
        CheckCellRange(rowIndex, cellIndex);
        _labelsRowIndex = rowIndex;
        _labelsCell = _htmlTableRows[rowIndex].Cells[cellIndex];
        return _labelsCell;
      }

      /// <summary> Set the controls cell for this <see cref="FormGridRow"/>. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/SetControlsCell/*' />
      public virtual HtmlTableCell SetControlsCell (int rowIndex, int cellIndex)
      {
        CheckCellRange(rowIndex, cellIndex);
        _controlsRowIndex = rowIndex;
        _controlsCell = _htmlTableRows[rowIndex].Cells[cellIndex];
        return _controlsCell;
      }

      /// <summary> Set the controls cell dummy for this <see cref="FormGridRow"/>. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/SetControlsCellDummy/*' />
      public virtual HtmlTableCell SetControlsCellDummy (int rowIndex, int cellIndex)
      {
        CheckCellRange(rowIndex, cellIndex);
        _controlsCellDummy = _htmlTableRows[rowIndex].Cells[cellIndex];
        return _controlsCellDummy;
      }

      /// <summary> Set the markers cell for this <see cref="FormGridRow"/>. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/SetMarkersCell/*' />
      public virtual HtmlTableCell SetMarkersCell (int rowIndex, int cellIndex)
      {
        CheckCellRange(rowIndex, cellIndex);
        _markersCell = _htmlTableRows[rowIndex].Cells[cellIndex];
        return _markersCell;
      }

      /// <summary> Set the validation messages cell for this <see cref="FormGridRow"/>. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/SetValidationMessagesCell/*' />
      public virtual HtmlTableCell SetValidationMessagesCell (int rowIndex, int cellIndex)
      {
        CheckCellRange(rowIndex, cellIndex);
        _validationMessagesCell = _htmlTableRows[rowIndex].Cells[cellIndex];
        return _validationMessagesCell;
      }

      /// <summary>Set the labels validation messages cell dummy for this <see cref="FormGridRow"/>.</summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/SetValidationMessagesCellDummy/*' />
      public virtual HtmlTableCell SetValidationMessagesCellDummy (int rowIndex, int cellIndex)
      {
        CheckCellRange(rowIndex, cellIndex);
        _validationMessagesCellDummy = _htmlTableRows[rowIndex].Cells[cellIndex];
        return _validationMessagesCellDummy;
      }

      /// <summary> Checks if the indices are inside the bounds. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/CheckCellRange/*' />
      private void CheckCellRange (int rowIndex, int cellIndex)
      {
        if (   rowIndex >= _htmlTableRows.Count
            && rowIndex < 0)
        {
          string? tableID = _formGrid.Table.ID;
          throw new ArgumentOutOfRangeException(
              "rowIndex",
              rowIndex,
              string.Format(
                  "Error while formatting HtmlTable '{0}': The rowIndex exceeds the number of rows in the row-group being formatted. Rows in the row-group: {1}",
                  tableID,
                  _htmlTableRows.Count));
        }

        if (   cellIndex >= _htmlTableRows[rowIndex].Cells.Count
            || cellIndex < 0)
        {
          string? tableID = _formGrid.Table.ID;
          int htmlRowIndex = _formGrid.Table.Controls.IndexOf(_htmlTableRows[rowIndex]);
          throw new ArgumentOutOfRangeException(
              "cellIndex",
              cellIndex,
              string.Format("Error while formatting HtmlTable '{0}', row {1}: The row has no cell at index {2}.", tableID, htmlRowIndex, cellIndex));
        }
      }

      /// <summary>
      ///   Fills a <see cref="Hashtable"/> with the controls contained in this 
      ///   <see cref="FormGridRow"/>, using their ID as a key.
      /// </summary>
      /// <remarks> Considers only controls where <see cref="Control.ID"/> is set.</remarks>
      public virtual void BuildIDCollection ()
      {
        //  Assume an average of 2 controls per cell
        _controls = new Hashtable(2 * _htmlTableRows.Count * _htmlTableRows[0].Cells.Count);

        for (int idxRows = 0; idxRows < _htmlTableRows.Count; idxRows++)
        {
          HtmlTableRow row = (HtmlTableRow)_htmlTableRows[idxRows];
          for (int idxCells = 0; idxCells < row.Cells.Count; idxCells++)
          {
            HtmlTableCell cell = (HtmlTableCell)row.Cells[idxCells];
            for (int idxControls = 0; idxControls < cell.Controls.Count; idxControls++)
            {
              Control control = (Control)cell.Controls[idxControls];
              if (control.ID != null && control.ID != string.Empty)
                _controls[control.ID] = control;
            }
          }
        }
      }

      /// <summary> Returns the control with the specified ID or <see langword="null"/>. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/GetControlForID/*' />
      public virtual Control? GetControlForID (string id)
      {
        string.IsNullOrEmpty(id);
        return (Control?)_controls[id];
      }

      /// <summary> 
      ///   Returns <see langword="true"/> if the control with the specified ID is contained 
      ///   in the <see cref="FormGridRow"/>.
      /// </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/ContainsControlWithID/*' />
      public virtual bool ContainsControlWithID (string id)
      {
        return GetControlForID(id) != null;
      }

      /// <summary>
      ///   Checks whether the row should be rendered.
      /// </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/CheckVisibility/*' />
      public virtual bool CheckVisibility ()
      {
        if (!_visible)
          return false;

        bool nonLabelControlFound = false;
        foreach (Control control in _controls.Values)
        {
          if (! (control is Label) && ! (control is SmartLabel) && ! (control is FormGridLabel))
          {
            if (control.Visible)
              return true;
            else
              nonLabelControlFound = true;
          }
        }

        return ! nonLabelControlFound;
      }

      /// <summary>
      ///   Sets the <see cref="FormGridRow"/> and its contained <see cref="HtmlTableRow"/> objects
      ///   invisible.
      /// </summary>
      public virtual void Hide ()
      {
        _visible = false;

        for (int i = 0; i < _htmlTableRows.Count; i++)
        {
          HtmlTableRow row = (HtmlTableRow)_htmlTableRows[i];
          row.Visible = false;
        }
      }

      /// <summary>
      ///   Sets the <see cref="FormGridRow"/> and its contained <see cref="HtmlTableRow"/> 
      ///   visible.
      /// </summary>
      public virtual void Show ()
      {
        _visible = true;

        for (int i = 0; i < _htmlTableRows.Count; i++)
        {
          HtmlTableRow row = (HtmlTableRow)_htmlTableRows[i];
          row.Visible = true;
        }
      }

      /// <summary>
      ///   The <see cref="FormGrid"/> instance of which this <c>FormGridRow</c> is a part of.
      /// </summary>
      public FormGrid FormGrid
      {
        get { return _formGrid; }
      }

      /// <summary> The <see cref="HtmlTableRow"/> collection for this <c>FormGridRow</c>. </summary>
      public ReadOnlyHtmlTableRowCollection HtmlTableRows
      {
        get { return _htmlTableRows; }
      }

      /// <summary> The type of this <c>FormGridRow</c>. </summary>
      public FormGridRowType Type
      {
        get { return _type; }
      }

      /// <summary>
      ///   Gets or sets a value indicating whether the contained <see cref="HtmlTableRows"/>
      ///   should be rendered on as Ui on the page.
      /// </summary>
      public bool Visible
      {
        get { return _visible; }
        set { _visible = value; }
      }

      /// <summary> Gets a value indicating whether this row has been generated from a <see cref="IFormGridRowProvider"/>. </summary>
      public bool IsGenerated
      {
        get { return _isGenerated; }
      }

      /// <summary> The <c>ValidationError</c> objects for this <c>FormGridRow</c>. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/ValidationErrors/remarks' />
      public ValidationError[] ValidationErrors
      {
        get
        {
          return _validationErrors;
        }
        set
        {
          ArgumentUtility.CheckNotNull("value", value);
          _validationErrors = value;
        }
      }

      /// <summary> The validation marker for this <c>FormGridRow</c>. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/whether/*' />
      public Control? ValidationMarker
      {
        get { return _validationMarker; }
        set { _validationMarker = value; }
      }

      /// <summary> The required marker for this <c>FormGridRow</c>. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/RequiredMarker/*' />
      public Control? RequiredMarker
      {
        get { return _requiredMarker; }
        set { _requiredMarker = value; }
      }

      /// <summary> The help provider for this <c>FormGridRow</c>. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/HelpProvider/*' />
      public Control? HelpProvider
      {
        get { return _helpProvider; }
        set { _helpProvider = value; }
     }

      /// <summary> The index of the row containing the labels cell. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/LabelsRowIndex/*' />
      public int LabelsRowIndex
      {
        get { return _labelsRowIndex; }
      }

      /// <summary> The index of the row containing the controls cell. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/ControlsRowIndex/*' />
      public int ControlsRowIndex
      {
        get { return _controlsRowIndex; }
      }

      /// <summary> The index of the column normally containing the labels cell. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/LabelsColumn/*' />
      public int LabelsColumn
      {
        get { return _labelsColumn; }
        set { _labelsColumn = value; }
      }

      /// <summary> The index of the column normally containing the controls cell. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/ControlsColumn/*' />
      public int ControlsColumn
      {
        get { return _controlsColumn; }
        set { _controlsColumn = value; }
      }

      /// <summary> The <see cref="HtmlTableRow"/> containing the labels cell. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/LabelsRow/*' />
      public HtmlTableRow LabelsRow
      {
        get { return _htmlTableRows[_labelsRowIndex]; }
      }

      /// <summary> The <see cref="HtmlTableRow"/> containing the controls cell. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/ControlsRow/*' />
      public HtmlTableRow ControlsRow
      {
        get { return _htmlTableRows[_controlsRowIndex]; }
      }

      /// <summary> The cell containing the labels. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/LabelsCell/*' />
      public HtmlTableCell? LabelsCell
      {
        get { return _labelsCell; }
      }

      /// <summary> The cell containing the controls. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/ControlsCell/*' />
      public HtmlTableCell? ControlsCell
      {
        get { return _controlsCell; }
      }

      /// <summary>
      ///   The cell used as a place holder if the controls cell is not at the standard position.
      /// </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/ControlsCellDummy/*' />
      public HtmlTableCell? ControlsCellDummy
      {
        get { return _controlsCellDummy; }
      }

      /// <summary> The cell containing the markers. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/MarkersCell/*' />
      public HtmlTableCell? MarkersCell
      {
        get { return _markersCell; }
      }

      /// <summary> The cell containing the validation messages. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/ValidationMessagesCell/*' />
      public HtmlTableCell? ValidationMessagesCell
      {
        get { return _validationMessagesCell; }
      }

      /// <summary>
      ///   The cell used as a place holder if the validation message cell is not at the standard
      ///   position.
      /// </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridRow/ValidationMessagesCellDummy/*' />
      public HtmlTableCell? ValidationMessagesCellDummy
      {
        get { return _validationMessagesCellDummy; }
      }
    }

    /// <summary> A read only collection of <see cref="HtmlTableRow"/> objects. </summary>
    protected sealed class ReadOnlyHtmlTableRowCollection : ReadOnlyCollectionBase
    {
      /// <summary> Simple constructor. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ReadOnlyHtmlTableRowCollection/Constructor/*' />
      public ReadOnlyHtmlTableRowCollection (HtmlTableRow[] htmlTableRows)
      {
        ArgumentUtility.CheckNotNull("htmlTableRows", htmlTableRows);

        for (int index = 0; index < htmlTableRows.Length; index++)
        {
          if (htmlTableRows[index] == null)
            throw new ArgumentNullException("htmlTableRows[" + index + "]");
        }

         InnerList.AddRange(htmlTableRows);
      }

      /// <summary> A read only indexer for the <see cref="HtmlTableRow"/> onbjects. </summary>
      /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ReadOnlyHtmlTableRowCollection/Indexer/*' />
      public HtmlTableRow this [int index]
      {
        get
        {
          if (index < 0 || index >= InnerList.Count)
            throw new ArgumentOutOfRangeException("index");
          return (HtmlTableRow)InnerList[index]!;
        }
      }
    }

    /// <summary> The row types possible for a <see cref="FormGridRow"/>. </summary>
    protected enum FormGridRowType
    {
      /// <summary> The row containing the form grid's title. </summary>
      TitleRow,
      /// <summary> The row containing the form grid's title. </summary>
      SubTitleRow,
      /// <summary> A row containing labels, controls and validators. </summary>
      DataRow,
      /// <summary> A row that can not be identified as one of the other types. </summary>
      UnknownRow
    }

    // constants

    /// <summary> Sufffix for identifying all tables to be used as form grids. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridSuffix/*' />
    private const string c_formGridSuffix = "FormGrid";

    private const string c_generatedLabelSuffix = "_Label";
                                                  #region private const string c_viewStateID...

  /// <summary> View State ID for the form grid view states. </summary>
  private const string c_viewStateIDFormGrids = "FormGrids";

  /// <summary> View State ID for _labelsColumn. </summary>
  private const string c_viewStateIDLabelsColumn = "_labelsColumn";

  /// <summary> View State ID for _controlsColumn. </summary>
  private const string c_viewStateIDControlsColumn = "_controlsColumn";

  /// <summary> View State ID for _showValidationMarkers. </summary>
  private const string c_viewStateIDShowValidationMarkers = "_showValidationMarkers";

  /// <summary> View State ID for _showRequiredMarkers. </summary>
  private const string c_viewStateIDShowRequiredMarkers = "_showRequiredMarkers";

  /// <summary> View State ID for _showHelpProviders. </summary>
  private const string c_viewStateIDHelpProviders = "_showHelpProviders";

  /// <summary> View State ID for _validatorVisibility. </summary>
  private const string c_viewStateIDValidatorVisibility = "_validatorVisibility";

  /// <summary> View State ID for _formGridSuffix. </summary>
  private const string c_viewStateIDFormGridSuffix = "_formGridSuffix";

  #endregion

    // static members

    private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger<FormGridManager>();
    private static NoneHotkeyFormatter s_noneHotkeyFormatter = new NoneHotkeyFormatter();

    // member fields

    /// <summary>
    ///   Hashtable&lt;string uniqueID, <see cref="FormGrid"/>&gt;
    /// </summary>
    private readonly Dictionary<string, FormGrid> _formGrids = new Dictionary<string, FormGrid>();

    /// <summary> Index of the column normally containing the labels. </summary>
    private int _labelsColumn;

    /// <summary>
    ///   Index of the column normally containing the controls. </summary>
    private int _controlsColumn;

    /// <summary>
    ///   Suffix identifying the <c>HtmlTables</c> to be managed by this <c>FormGridManager</c>.
    /// </summary>
    private string _formGridSuffix;

    /// <summary> The cell where the validation message should be written. </summary>
    private ValidatorVisibility _validatorVisibility;

    /// <summary> Enable/Disable the validation markers. </summary>
    private bool _showValidationMarkers;

    /// <summary> Enable/Disable the required markers. </summary>
    private bool _showRequiredMarkers;

    /// <summary> Enable/Disable the help providers. </summary>
    private bool _showHelpProviders;

    /// <summary> 
    ///   State variable for the two part transformation process. 
    ///   Hashtable&lt;FormGrid, TransformationStep completedStep&gt; 
    /// </summary>
    private Hashtable _completedTransformationStep = new Hashtable();

  //  /// <summary> State variable for automatic validators creation. </summary>
  //  private bool _hasValidatorsCreated;

    /// <summary> Caches the <see cref="IFormGridRowProvider"/> for this <see cref="FormGridManager"/>. </summary>
    private IFormGridRowProvider? _cachedFormGridRowProvider;

    /// <summary>
    ///   <see langword="true"/> if the control hierarchy doesn't implement <see cref="IFormGridRowProvider"/>.
    /// </summary>
    private bool _isFormGridRowProviderUndefined;

    /// <summary> Caches the <see cref="ResourceManagerSet"/> for this <see cref="FormGridManager"/>. </summary>
    private ResourceManagerSet? _cachedResourceManager;

    private bool _formGridListPopulated = false;
    private IInfrastructureResourceUrlFactory? _infrastructureResourceUrlFactory;
    private IResourceUrlFactory? _resourceUrlFactory;
    private IRenderingFeatures? _renderingFeatures;
    private IHotkeyFormatter? _hotkeyFormatter;

    // construction and disposing

    /// <summary> Simple constructor. </summary>
    public FormGridManager ()
    {
      _labelsColumn = 0;
      _controlsColumn = 1;

      _formGridSuffix = c_formGridSuffix;

      _validatorVisibility = ValidatorVisibility.ValidationMessageInControlsColumn;
      _showValidationMarkers = true;
      _showRequiredMarkers = true;
      _showHelpProviders = true;
    }

    // methods and properties

    public new IPage? Page
    {
      get { return PageWrapper.CastOrCreate(base.Page); }
    }

    /// <summary> Prepares all all <c>FormGrid</c> objects managed by this <c>FormGridManager</c> for validation. </summary>
    public void PrepareValidation ()
    {
      EnsureTransformationStep(TransformationStep.PostLoadTransformationCompleted);

      foreach (FormGrid formGrid in _formGrids.Values)
        PrepareValidationForFormGrid(formGrid);
    }

    /// <summary> Validates all <c>FormGrid</c> objects managed by this <c>FormGridManager</c>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/Validate/*' />
    public bool Validate ()
    {
      EnsureTransformationStep(TransformationStep.PostLoadTransformationCompleted);

      bool isValid = true;
      foreach (FormGrid formGrid in _formGrids.Values)
        isValid &= ValidateFormGrid(formGrid);
      return isValid;
    }

    /// <summary>
    ///   Assembles all <see cref="ValidationError"/> objects in the managed <c>FormGrids</c>.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/GetValidationErrors/*' />
    public ValidationError[] GetValidationErrors ()
    {
      ArrayList validationErrorList = new ArrayList();

      foreach (FormGrid formGrid in _formGrids.Values)
        validationErrorList.AddRange(formGrid.GetValidationErrors());

      return (ValidationError[])validationErrorList.ToArray(typeof(ValidationError));
    }
    /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
    /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
    void IResourceDispatchTarget.Dispatch (IDictionary<string, WebString> values)
    {
      ArgumentUtility.CheckNotNull("values", values);
      Dispatch(values);
    }

    /// <summary> Implementation of <see cref="IResourceDispatchTarget"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/Dispatch/*' />
    protected virtual void Dispatch (IDictionary<string, WebString> values)
    {
      EnsureTransformationStep(TransformationStep.PreLoadViewStateTransformationCompleted);

      var formGridControls = new Dictionary<string, IDictionary<string, IDictionary<string, WebString>>>();

      //  Parse the values

      foreach (var entry in values)
      {
        //  Compound key: "tableUniqueID:controlUniqueID:property"
        var key = entry.Key;

        int posColon = key.IndexOf(':');

        //  Split after table id
        string tableID = key.Substring(0, key.IndexOf(":"));
        string elementIDProperty = key.Substring(posColon + 1);

        if (_formGrids.ContainsKey(tableID))
        {
          //  Get the controls for the current FormGrid
          //  If no hashtable exists, create it and insert it into the formGridControls hashtable.
          if (!formGridControls.TryGetValue(tableID, out var controls))
          {
            controls = new Dictionary<string, IDictionary<string, WebString>>();
            formGridControls[tableID] = controls;
          }

          //  Test for a second colon in the key
          posColon = elementIDProperty.IndexOf(':');

          if (posColon >= 0)
          {
            //  If one is found, this is an elementID/property pair

            string controlID = elementIDProperty.Substring(0, posColon);
            string property = elementIDProperty.Substring(posColon + 1);

            //  Get the dictonary for the current element
            //  If no dictonary exists, create it and insert it into the elements hashtable.
            if (!controls.TryGetValue(controlID, out var controlValues))
            {
              controlValues = new Dictionary<string, WebString>();
              controls[controlID] = controlValues;
            }

            //  Insert the argument and resource's value into the dictonary for the specified element.
            controlValues.Add(property, entry.Value);
          }
          else
          {
            // TODO RM-8118: not null assertion
            //  Not supported format
            s_logger.LogWarning(
                $"FormGridManager '{UniqueID}' on page '{Page!}' received a resource with an invalid key '{key}'. Required format: 'tableUniqueID:controlUniqueID:property'.");
          }
        }
        else
        {
          // TODO RM-8118: not null assertion
          //  Invalid form grid
          s_logger.LogWarning($"FormGrid '{tableID}' is not managed by FormGridManager '{UniqueID}' on page '{Page!}'.");
        }
      }

      //  Assign the values

      foreach (var formGridEntry in formGridControls)
      {
        string tableID = formGridEntry.Key;
        var formGrid = _formGrids[tableID];

        var controls = formGridEntry.Value;

        foreach (var controlEntry in controls)
        {
          string controlID = controlEntry.Key;
          Control? control = formGrid.Table.FindControl(controlID);

          if (control != null)
          {
            var controlValues = controlEntry.Value;

            //  Pass the values to the control
            IResourceDispatchTarget? resourceDispatchTarget = control as IResourceDispatchTarget;

            if (resourceDispatchTarget != null) //  Control knows how to dispatch
              resourceDispatchTarget.Dispatch(controlValues);
            else
              ResourceDispatcher.DispatchGeneric(control, controlValues);

            //  Access key support for Labels
            Label? label = control as Label;
            if (label != null)
            {
#pragma warning disable 184
              Assertion.IsFalse(label is SmartLabel);
#pragma warning restore 184

              var labelTextAsWebString = WebString.CreateFromHtml(label.Text);

              //  Label has associated control
              if (!string.IsNullOrEmpty(label.AssociatedControlID))
              {
                var labelAccessKey = HotkeyFormatter.GetAccessKey(labelTextAsWebString);
                if (labelAccessKey.HasValue)
                  label.AccessKey = labelAccessKey.Value.ToString();
                label.Text = HotkeyFormatter.GetFormattedText(labelTextAsWebString).ToString(WebStringEncoding.HtmlWithTransformedLineBreaks);
              }
              else
              {
                label.AccessKey = null;
                label.Text = s_noneHotkeyFormatter.GetFormattedText(labelTextAsWebString).ToString(WebStringEncoding.HtmlWithTransformedLineBreaks);
              }
            }
          }
          else
          {
            // TODO RM-8118: not null assertion
            //  Invalid control
            s_logger.LogWarning(
                $"FormGrid '{tableID}' in naming container '{NamingContainer.GetType().GetFullNameSafe()}' on page '{Page!}' does not contain a control with UniqueID '{controlID}'.");
          }
        }
      }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      NamingContainer.Load += new EventHandler(NamingContainer_Load);

      HtmlHeadAppender.Current.RegisterCommonStyleSheet();

      string key = typeof(FormGridManager).GetFullNameChecked() + "_Style";
      if (!HtmlHeadAppender.Current.IsRegistered(key))
      {
        var url = InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Html, "FormGrid.css");
        HtmlHeadAppender.Current.RegisterStylesheetLink(key, url, HtmlHeadAppender.Priority.Library);
      }
    }

    private IResourceUrlFactory ResourceUrlFactory
    {
      get
      {
        if (_resourceUrlFactory == null)
          _resourceUrlFactory = SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>();
        return _resourceUrlFactory;
      }
    }

    private IInfrastructureResourceUrlFactory InfrastructureResourceUrlFactory
    {
      get
      {
        if (_infrastructureResourceUrlFactory == null)
          _infrastructureResourceUrlFactory = SafeServiceLocator.Current.GetInstance<IInfrastructureResourceUrlFactory>();
        return _infrastructureResourceUrlFactory;
      }
    }

    private IRenderingFeatures RenderingFeatures
    {
      get
      {
        if (_renderingFeatures == null)
          _renderingFeatures = SafeServiceLocator.Current.GetInstance<IRenderingFeatures>();
        return _renderingFeatures;
      }
    }

    private IHotkeyFormatter HotkeyFormatter
    {
      get
      {
        if (_hotkeyFormatter == null)
          _hotkeyFormatter = SafeServiceLocator.Current.GetInstance<IHotkeyFormatter>();
        return _hotkeyFormatter;
      }
    }

    private void NamingContainer_Load (object? sender, EventArgs e)
    {
      EnsureFormGridListPopulated();
    }

    /// <summary>
    ///   Calls <see cref="EnsureTransformationStep(FormGrid,TransformationStep)"/> with 
    ///   <see cref="TransformationStep.PreLoadViewStateTransformationCompleted"/>.
    /// </summary>
    private void Table_Load (object? sender, EventArgs e)
    {
      var formGrid = GetFormGrid((HtmlTable)sender!); // TODO RM-8118: not null assertion
      EnsureTransformationStep(formGrid, TransformationStep.PreLoadViewStateTransformationCompleted);
    }

    /// <summary>
    /// Optionally called after the <c>Load</c> event.
    /// </summary>
    public void OnPostLoad ()
    {
      EnsureTransformationStep(TransformationStep.PostLoadTransformationCompleted);
    }

    /// <summary>
    ///   Calls <see cref="EnsureTransformationStep(FormGrid,TransformationStep)"/> with 
    ///   <see cref="TransformationStep.PostValidationTransformationCompleted"/>.
    /// </summary>
    private void Table_PreRender (object? sender, EventArgs e)
    {
      var formGrid = GetFormGrid((HtmlTable)sender!); // TODO RM-8118: notnull assertion
      EnsureTransformationStep(formGrid, TransformationStep.PostValidationTransformationCompleted);
      ((HtmlTable)sender!).SetRenderMethodDelegate(Table_Render);
    }

    private void Table_Render (HtmlTextWriter writer, Control table)
    {
      var formGrid = GetFormGrid((HtmlTable)table);
      EnsureTransformationStep(formGrid, TransformationStep.RenderTransformationCompleted);

      foreach (Control row in table.Controls)
        row.RenderControl(writer);
    }

    /// <summary> This member overrides <see cref="Control.LoadViewState"/>. </summary>
    protected override void LoadViewState (object? savedState)
    {
      //  Get view state for the form grid manager

      if (savedState != null)
      {
        base.LoadViewState(savedState);

        object? labelsColumn = ViewState[c_viewStateIDLabelsColumn];
        if (labelsColumn != null)
          _labelsColumn = (int)labelsColumn;

        object? controlsColumn = ViewState[c_viewStateIDControlsColumn];
        if (controlsColumn != null)
          _controlsColumn = (int)controlsColumn;

        object? formGridSuffix = ViewState[c_viewStateIDFormGridSuffix];
        if (formGridSuffix != null)
          _formGridSuffix = (string)formGridSuffix;

        object? showValidationMarkers = ViewState[c_viewStateIDShowValidationMarkers];
        if (showValidationMarkers != null)
          _showValidationMarkers = (bool)showValidationMarkers;

        object? showRequiredMarkers = ViewState[c_viewStateIDShowRequiredMarkers];
        if (showRequiredMarkers != null)
          _showRequiredMarkers = (bool)showRequiredMarkers;

        object? showHelpProviders = ViewState[c_viewStateIDHelpProviders];
        if (showHelpProviders != null)
          _showHelpProviders = (bool)showHelpProviders;

        object? validatorVisibility = ViewState[c_viewStateIDValidatorVisibility];
        if (validatorVisibility != null)
          _validatorVisibility = (ValidatorVisibility)validatorVisibility;
      }


      //  Rebuild the HTML tables used as form grids
      EnsureTransformationStep(TransformationStep.PreLoadViewStateTransformationCompleted);


      //  Restore the view state to the form grids

      Hashtable? formGridViewStates = (Hashtable?)ViewState[c_viewStateIDFormGrids];

      if (formGridViewStates != null)
      {
        foreach (FormGrid formGrid in _formGrids.Values)
        {
          object? viewState = formGridViewStates[formGrid.Table.UniqueID];
          LoadFormGridViewState(formGrid, viewState);
        }
      }
    }

    /// <summary> This member overrides <see cref="Control.SaveViewState"/>. </summary>
    protected override object? SaveViewState ()
    {
      // Hashtable<string, object>
      Hashtable formGridViewStates = new Hashtable(_formGrids.Count);

      foreach (FormGrid formGrid in _formGrids.Values)
      {
        object formGridViewState = SaveFormGridViewState(formGrid);
        formGridViewStates.Add(formGrid.Table.UniqueID, formGridViewState);
      }

      ViewState[c_viewStateIDFormGrids] = formGridViewStates;

      ViewState[c_viewStateIDLabelsColumn] = _labelsColumn;
      ViewState[c_viewStateIDControlsColumn] = _controlsColumn;
      ViewState[c_viewStateIDShowValidationMarkers] = _showValidationMarkers;
      ViewState[c_viewStateIDShowRequiredMarkers] = _showRequiredMarkers;
      ViewState[c_viewStateIDHelpProviders] = _showHelpProviders;
      ViewState[c_viewStateIDValidatorVisibility] = _validatorVisibility;

      return base.SaveViewState();
    }

    /// <summary> Restore the view state to the form grids. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/LoadFormGridViewState/*' />
    private void LoadFormGridViewState (FormGrid formGrid, object? savedState)
    {
      ArgumentUtility.CheckNotNull("formGrid", formGrid);

      if (savedState == null)
        return;

      bool enableViewStateBackup = formGrid.Table.EnableViewState;
      formGrid.Table.EnableViewState = true;
      MemberCaller.LoadViewStateRecursive(formGrid.Table, savedState);
      formGrid.Table.EnableViewState = enableViewStateBackup;
    }

    /// <summary> Saves the view state of the form grids. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/SaveFormGridViewState/*' />
    private object SaveFormGridViewState (FormGrid formGrid)
    {
      ArgumentUtility.CheckNotNull("formGrid", formGrid);

      bool enableViewStateBackup = formGrid.Table.EnableViewState;
      formGrid.Table.EnableViewState = true;
      object viewState = MemberCaller.SaveViewStateRecursive(formGrid.Table);
      formGrid.Table.EnableViewState = enableViewStateBackup;

      return viewState;
    }

    /// <summary> This member overrides <see cref="Control.Render"/>. </summary>
    protected override void Render (HtmlTextWriter output)
    {
      //  nothing, required get a usefull designer output without much coding.
    }

    /// <summary> Analyzes the table layout and creates the appropriate <see cref="FormGridRow"/> isntances. </summary>
    private FormGridRow[] CreateFormGridRows (HtmlTable table, int labelsColumn, int controlsColumn)
    {
      ArgumentUtility.CheckNotNull("table", table);

      ArrayList formGridRows = new ArrayList(table.Rows.Count);

      HtmlTableRowCollection rows = table.Rows;

      //  Form Grid Title

      bool hasTitleRow = false;

      //  Table may only have a single cell in the first row
      if (rows[0].Cells.Count == 1)
        hasTitleRow = true;

      if (hasTitleRow)
      {
        HtmlTableRow[] tableRows = new HtmlTableRow[1];
        tableRows[0] = rows[0];

        FormGridRow formGridRow = new FormGridRow(
          tableRows,
          FormGridRowType.TitleRow,
          labelsColumn,
          controlsColumn,
          false);

        formGridRows.Add(formGridRow);
      }

      //  Form Grid Body

      for (int i = formGridRows.Count; i < rows.Count; i++)
      {
        bool isSubTitleRow = rows[i].Cells.Count == 1;
        bool isDataRow =    ! isSubTitleRow
                         && rows[i].Cells.Count > _controlsColumn;

        //  If ControlsColumn cell contains controls: single row constellation
        bool hasOneDataRow =   isDataRow
                            && HasContents(rows[i].Cells[_controlsColumn]);

        //  If it is not a single row constellation
        //  and the table still has another row left
        //  and the next row contains at the label's cell
        bool hasTwoDataRows =    isDataRow
                              && !hasOneDataRow
                              && i + 1 < rows.Count
                              && rows[i + 1].Cells.Count > _labelsColumn;

        if (isSubTitleRow)
        {
          HtmlTableRow[] tableRows = new HtmlTableRow[1];
          tableRows[0] = rows[i];

          FormGridRow formGridRow = new FormGridRow(
            tableRows,
            FormGridRowType.SubTitleRow,
            labelsColumn,
            controlsColumn,
            false);

          formGridRows.Add(formGridRow);
        }
        else if (hasOneDataRow)
        {
          //  One HtmlTableRow is one FormGrid DataRow
          HtmlTableRow[] tableRows = new HtmlTableRow[1];
          tableRows[0] = table.Rows[i];

          FormGridRow formGridRow = new FormGridRow(
            tableRows,
            FormGridRowType.DataRow,
            labelsColumn,
            controlsColumn,
            false);

          formGridRows.Add(formGridRow);
        }
        else if (hasTwoDataRows)
        {
          //  Two HtmlTableRows get combined into one FormGrid DataRow
          HtmlTableRow[] tableRows = new HtmlTableRow[2];
          tableRows[0] = rows[i];
          tableRows[1] = rows[i + 1];

          FormGridRow formGridRow = new FormGridRow(
            tableRows,
            FormGridRowType.DataRow,
            labelsColumn,
            controlsColumn,
            false);

          formGridRows.Add(formGridRow);

          i++;
        }
          //  Can't interpret layout of current HtmlTableRow
        else
        {
          HtmlTableRow[] tableRows = new HtmlTableRow[1];
          tableRows[0] = rows[i];

          FormGridRow formGridRow = new FormGridRow(
            tableRows,
            FormGridRowType.UnknownRow,
            labelsColumn,
            controlsColumn,
            false);

          formGridRows.Add(formGridRow);
        }
      }

      return (FormGridRow[])formGridRows.ToArray(typeof(FormGridRow));
    }

    private bool HasContents (HtmlTableCell? cell)
    {
      if (cell!.Controls.Count == 0) // TODO RM-8118: return false on null?
        return false;

      if (cell.Controls.Count > 1)
        return true;

      LiteralControl? literalControl = cell.Controls[0] as LiteralControl;
      if (literalControl == null)
        return true;

      if (literalControl.Text.Trim().Length > 0)
        return true;

      return false;
    }

    private void PrepareValidationForFormGrid (FormGrid formGrid)
    {
      for (int i = 0; i < formGrid.Rows.Count; i++)
      {
        FormGridRow formGridRow = (FormGridRow)formGrid.Rows[i];
        if (formGridRow.Type != FormGridRowType.DataRow)
          continue;

        PrepareValidationForDataRow(formGridRow);
      }
    }

    protected void PrepareValidationForDataRow (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      Assertion.IsNotNull(dataRow.ControlsCell, "dataRow.ControlsCell must not be null.");

      for (int i = 0; i < dataRow.ControlsCell.Controls.Count; i++)
      {
        IValidatableControl? control = dataRow.ControlsCell.Controls[i] as IValidatableControl;

        //  Only for IValidatableControl
        if (control == null)
          continue;

        //  Prepare Validate
        control.PrepareValidation();
      }
    }

    /// <summary> Validates all <see cref="BaseValidator"/> objects in the <see cref="FormGrid"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ValidateFormGrid/*' />
    private bool ValidateFormGrid (FormGrid formGrid)
    {
      bool isValid = true;
      for (int i = 0; i < formGrid.Rows.Count; i++)
      {
        FormGridRow formGridRow = (FormGridRow)formGrid.Rows[i];
        if (formGridRow.Type != FormGridRowType.DataRow)
          continue;

        isValid &= ValidateDataRow(formGridRow);
      }
      return isValid;
    }

    /// <summary> Validates the <see cref="BaseValidator"/> objects. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ValidateDataRow/*' />
    protected bool ValidateDataRow (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      Assertion.IsNotNull(dataRow.ControlsCell, "dataRow.ControlsCell must not be null.");

      dataRow.ValidationMarker = null;
      ArrayList validationErrorList = new ArrayList();

      if (!dataRow.Visible)
        return true;

      //  Check for validators and then their validation state
      //  Create a ValidationError object for each error
      //  Create the validationIcon

      bool isValid = true;
      for (int i = 0; i < dataRow.ControlsCell.Controls.Count; i++)
      {
        Control control = (Control)dataRow.ControlsCell.Controls[i];
        IValidator? validator = control as IValidator;

        //  Only for validators
        if (validator == null)
          continue;

        //  Validate
        validator.Validate();
        isValid &= validator.IsValid;
      }
      return isValid;
    }

    /// <summary>
    ///   Creates the appropriate <see cref="ValidationError"/> objects and adds them to the <paramref name="dataRow"/>.
    /// </summary>
    /// <param name="dataRow"> The <see cref="FormGridRow"/> for which the validation errors will analyzed and registered. </param>
    private void RegisterValidationErrors (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      Assertion.IsNotNull(dataRow.ControlsCell, "dataRow.ControlsCell must not be null.");

      var validationErrorList = new List<ValidationError>();

      for (int i = 0; i < dataRow.ControlsCell.Controls.Count; i++)
      {
        Control control = dataRow.ControlsCell.Controls[i];
        IValidator? validator = control as IValidator;
        if (validator == null || validator.IsValid)
          continue;

        //  Get control to validate
        Control? controlToValidate = null;

        BaseValidator? baseValidator = control as BaseValidator;
        IBaseValidator? iBaseValidator = control as IBaseValidator;

        if (baseValidator != null)
          controlToValidate = control.NamingContainer.FindControl(baseValidator.ControlToValidate);
        else if (iBaseValidator != null)
          controlToValidate = control.NamingContainer.FindControl(iBaseValidator.ControlToValidate!); // TODO RM-8118: not null assertion

        //  Only visible controls: Build ValidationError
        if (controlToValidate == null || controlToValidate.Visible)
        {
          Assertion.IsNotNull(dataRow.LabelsCell, "dataRow.LabelsCell must not be null.");
          validationErrorList.Add(new ValidationError(controlToValidate, validator, dataRow.LabelsCell.Controls));
        }
      }
      dataRow.ValidationErrors = validationErrorList.ToArray();
    }

    /// <summary>
    ///   Creates the appropriate validation marker for each <see cref="ValidationError"/> and adds them to the <paramref name="dataRow"/>.
    /// </summary>
    /// <param name="dataRow"> The <see cref="FormGridRow"/> for which the validation markers will created. </param>
    private void CreateValidationMarkers (FormGridRow dataRow)
    {
      dataRow.ValidationMarker = null;
      if (!dataRow.Visible)
        return;

      bool hasValidationErrors = dataRow.ValidationErrors.Length > 0;

      if (hasValidationErrors)
      {
        var toolTip = new StringBuilder(dataRow.ValidationErrors.Length * 50);
        for (int i = 0; i < dataRow.ValidationErrors.Length; i++)
        {
          ValidationError validationError = dataRow.ValidationErrors[i];
          //  Get validation message
          PlainTextString validationMessage = validationError.ValidationMessage;
          //  Get tool tip, tool tip is validation message
          if (!validationMessage.IsEmpty)
          {
            if (toolTip.Length > 0)
              toolTip.AppendLine();
            toolTip.Append(validationMessage.GetValue());
          }
        }
        dataRow.ValidationMarker = CreateValidationMarker(PlainTextString.CreateFromText(toolTip.ToString()));
      }
    }

    protected void EnsureTransformationStep (TransformationStep stepToBeCompleted)
    {
      EnsureFormGridListPopulated();

      foreach (FormGrid formGrid in _formGrids.Values)
        EnsureTransformationStep(formGrid, stepToBeCompleted);
    }

    private void EnsureTransformationStep (FormGrid formGrid, TransformationStep stepToBeCompleted)
    {
      object? boxedCompletedStep = _completedTransformationStep [formGrid];
      TransformationStep completedStep = TransformationStep.TransformationNotStarted;
      if (boxedCompletedStep != null)
        completedStep = (TransformationStep)boxedCompletedStep;

      if (   completedStep < TransformationStep.PreLoadViewStateTransformationCompleted
          && completedStep < stepToBeCompleted)
      {
        completedStep = TransformIntoFormGridPreLoadViewState(formGrid);
      }
      if (   completedStep < TransformationStep.PostLoadTransformationCompleted
          && completedStep < stepToBeCompleted)
      {
        completedStep = TransformIntoFormGridPostLoad(formGrid);
      }
      if (   completedStep < TransformationStep.PostValidationTransformationCompleted
          && completedStep < stepToBeCompleted)
      {
        completedStep = TransformIntoFormGridPostValidation(formGrid);
      }
      if (   completedStep < TransformationStep.RenderTransformationCompleted
          && completedStep < stepToBeCompleted)
      {
        completedStep = TransformIntoFormGridRender(formGrid);
      }
    }

    private TransformationStep TransformIntoFormGridPreLoadViewState (FormGrid formGrid)
    {
      formGrid.Table.EnableViewState = false;

      formGrid.BuildIDCollection();
      LoadNewFormGridRows(formGrid);
      ApplyExternalHiddenSettings(formGrid);
      ComposeFormGridContents(formGrid);
      ConfigureFormGrid(formGrid);
      TransformationStep completedStep = TransformationStep.PreLoadViewStateTransformationCompleted;
      _completedTransformationStep[formGrid] = completedStep;
      return completedStep;
    }

    private TransformationStep TransformIntoFormGridPostLoad (FormGrid formGrid)
    {
      for (int i = 0; i < formGrid.Rows.Count; i++)
      {
        FormGridRow formGridRow = (FormGridRow)formGrid.Rows[i];
        if (formGridRow.Type == FormGridRowType.DataRow)
        {
          CreateValidators(formGridRow);
          ApplyValidatorSettings(formGridRow);
        }
      }

      TransformationStep completedStep = TransformationStep.PostLoadTransformationCompleted;
      _completedTransformationStep[formGrid] = completedStep;
      return completedStep;
    }

    private TransformationStep TransformIntoFormGridPostValidation (FormGrid formGrid)
    {
      for (int i = 0; i < formGrid.Rows.Count; i++)
      {
        FormGridRow formGridRow = (FormGridRow)formGrid.Rows[i];
        if (formGridRow.IsGenerated)
          UpdateGeneratedRowsVisibility(formGridRow);

        if (formGridRow.Type == FormGridRowType.DataRow)
          RegisterValidationErrors(formGridRow);
      }

      FormatFormGrid(formGrid);
      AddAriaAnnotations(formGrid);
      if (RenderingFeatures.EnableDiagnosticMetadata)
        AddDiagnosticMetadataAttributes(formGrid);

      TransformationStep completedStep = TransformationStep.PostValidationTransformationCompleted;
      _completedTransformationStep[formGrid] = completedStep;
      return completedStep;
    }

    private void AddAriaAnnotations (FormGrid formGrid)
    {
      ArgumentUtility.CheckNotNull("formGrid", formGrid);

      if (formGrid.Table.Attributes[HtmlTextWriterAttribute2.Role] == null)
        formGrid.Table.Attributes[HtmlTextWriterAttribute2.Role] = HtmlRoleAttributeValue.None;
    }

    private void AddDiagnosticMetadataAttributes (FormGrid formGrid)
    {
      ArgumentUtility.CheckNotNull("formGrid", formGrid);

      if (formGrid.Table.Attributes[DiagnosticMetadataAttributes.ControlType] == null)
        formGrid.Table.Attributes[DiagnosticMetadataAttributes.ControlType] = "FormGrid";
    }

    private TransformationStep TransformIntoFormGridRender (FormGrid formGrid)
    {
      bool isTopDataRow = true;

      for (int i = 0; i < formGrid.Rows.Count; i++)
      {
        FormGridRow formGridRow = (FormGridRow)formGrid.Rows[i];
        switch (formGridRow.Type)
        {
          case FormGridRowType.TitleRow:
          {
            FormatTitleRow(formGridRow);
            break;
          }
          case FormGridRowType.SubTitleRow:
          {
            FormatSubTitleRow(formGridRow);
            break;
          }
          case FormGridRowType.DataRow:
          {
            FormatDataRow(formGridRow, isTopDataRow);
            isTopDataRow = false;
            break;
          }
          case FormGridRowType.UnknownRow:
          {
            FormatUnknownRow(formGridRow);
            break;
          }
          default:
          {
            break;
          }
        }
      }

      TransformationStep completedStep = TransformationStep.RenderTransformationCompleted;
      _completedTransformationStep[formGrid] = completedStep;
      return completedStep;
    }

    /// <summary>
    ///   Queries the parent hierarchy for an <see cref="IFormGridRowProvider"/> and inserts 
    ///   the provided new rows into the form grid.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/LoadNewFormGridRows/*' />
    private void LoadNewFormGridRows (FormGrid formGrid)
    {
      IFormGridRowProvider? rowProvider = GetFormGridRowProvider(this);

      if (rowProvider == null)
        return;

      FormGridRowInfoCollection formGridRowInfos = rowProvider.GetAdditionalRows(formGrid.Table);
      if (formGridRowInfos == null)
        return;

      foreach (FormGridRowInfo prototype in formGridRowInfos)
      {
        int rowCount = 1;
        if (prototype.NewRowType == FormGridRowInfo.RowType.ControlInRowAfterLabel)
          rowCount = 2;

        HtmlTableRow[] htmlTableRows = new HtmlTableRow [rowCount];

        //  Row with label
        htmlTableRows[0] = new HtmlTableRow();
        for (int idxCells = 0, columnCount = ControlsColumn + 1; idxCells <= columnCount; idxCells++)
          htmlTableRows[0].Cells.Add(new HtmlTableCell());

        //  Row with control
        if (prototype.NewRowType == FormGridRowInfo.RowType.ControlInRowAfterLabel)
        {
          htmlTableRows[1] = new HtmlTableRow();
          for (int idxCells = 0, columnCount = ControlsColumn + 1; idxCells <= columnCount; idxCells++)
            htmlTableRows[1].Cells.Add(new HtmlTableCell());
        }

        //  Control in labels row
        if (prototype.NewRowType == FormGridRowInfo.RowType.ControlInRowWithLabel)
        {
          htmlTableRows[0].Cells[ControlsColumn].Controls.Add(prototype.Control);
        }
          //  Control in row after labels row
        else if (prototype.NewRowType == FormGridRowInfo.RowType.ControlInRowAfterLabel)
        {
          htmlTableRows[1].Cells[LabelsColumn].Controls.Add(prototype.Control);
        }

        FormGridRow newFormGridRow = new FormGridRow(
          htmlTableRows,
          FormGridRowType.DataRow,
          LabelsColumn,
          ControlsColumn,
          true);

        formGrid.InsertNewFormGridRow(
          newFormGridRow,
          prototype.ReleatedRowID,
          prototype.PositionInFormGrid);
      }
    }

    /// <summary>
    ///   Queries the parent hierarchy for an <see cref="IFormGridRowProvider"/> and hides  
    ///   the rows identified as invisible rows.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ApplyExternalHiddenSettings/*' />
    private void ApplyExternalHiddenSettings (FormGrid formGrid)
    {
      IFormGridRowProvider? rowProvider = GetFormGridRowProvider(this);

      if (rowProvider == null)
        return;

      StringCollection strings = rowProvider.GetHiddenRows(formGrid.Table);
      if (strings == null)
        return;

      for (int i = 0; i < strings.Count; i++)
      {
        string? id = strings[i];
        FormGridRow? row = formGrid.GetRowForID(id);

        if (row != null)
          formGrid.GetRowForID(id)!.Visible = false;
      }
    }

    /// <summary>
    ///   Find the closest parent <see cref="Control"/> impementing
    ///   <see cref="IFormGridRowProvider"/>.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/GetFormGridRowProvider/*' />
    private IFormGridRowProvider? GetFormGridRowProvider (Control? control)
    {
      //  Control hierarchy doesn't implent this interface
      if (_isFormGridRowProviderUndefined)
        return null;

      //  Provider has already been identified.
      if (_cachedFormGridRowProvider != null)
          return _cachedFormGridRowProvider;

      //  No control, no provider
      if (control == null)
        return null;

      //  Try to get the provider

      _cachedFormGridRowProvider  = control as IFormGridRowProvider;

      if (_cachedFormGridRowProvider != null)
        return _cachedFormGridRowProvider;

      //  End of hierarchy and not found -> no IformGridRowProvider defined.
      if (control.Parent == null)
        _isFormGridRowProviderUndefined = true;

      //  Try the next level
      return GetFormGridRowProvider(control.Parent);
    }

    /// <summary> Find the <see cref="IResourceManager"/> for this <see cref="FormGridManager"/>. </summary>
    protected IResourceManager GetResourceManager ()
    {
      //  Provider has already been identified.
      if (_cachedResourceManager != null)
          return _cachedResourceManager;

      //  Get the resource managers

      IResourceManager localResourceManager = GlobalizationService.GetResourceManager(typeof(ResourceIdentifier));
      IResourceManager namingContainerResourceManager = ResourceManagerUtility.GetResourceManager(NamingContainer, true);
      _cachedResourceManager = ResourceManagerSet.Create(namingContainerResourceManager, localResourceManager);

      return _cachedResourceManager;
    }

    /// <summary>
    ///   Composes all information required to transform the <see cref="HtmlTable"/> 
    ///   into a form grid.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ComposeFormGridContents/*' />
    private void ComposeFormGridContents (FormGrid formGrid)
    {
      if (formGrid == null) throw new ArgumentNullException("formGrid");

      for (int i = 0; i < formGrid.Rows.Count; i++)
      {
        FormGridRow formGridRow = (FormGridRow)formGrid.Rows[i];
        if (formGridRow.Type != FormGridRowType.DataRow)
          continue;

        formGridRow.SetLabelsCell(0, formGridRow.LabelsColumn);

        if (formGridRow.HtmlTableRows.Count == 1)
          formGridRow.SetControlsCell(0, formGridRow.ControlsColumn);
        else
          formGridRow.SetControlsCell(1, formGridRow.LabelsColumn);

        HandleReadOnlyControls(formGridRow);

        CreateLabels(formGridRow);
      }
    }

    /// <summary>
    ///   Uses the information stored in <paramref name="formGrid"/> to configure the 
    ///   <see cref="HtmlTable"/> as a form grid.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ConfigureFormGrid/*' />
    private void ConfigureFormGrid (FormGrid formGrid)
    {
      ArgumentUtility.CheckNotNull("formGrid", formGrid);
      for (int i = 0; i < formGrid.Rows.Count; i++)
      {
        FormGridRow formGridRow = (FormGridRow)formGrid.Rows[i];
        switch (formGridRow.Type)
        {
          case FormGridRowType.TitleRow:
          {
            ConfigureTitleRow(formGridRow);
            break;
          }
          case FormGridRowType.SubTitleRow:
          {
            ConfigureSubTitleRow(formGridRow);
            break;
          }
          case FormGridRowType.DataRow:
          {
            ConfigureDataRow(formGridRow);
            break;
          }
          case FormGridRowType.UnknownRow:
          {
            ConfigureUnknownRow(formGridRow);
            break;
          }
          default:
          {
            break;
          }
        }
      }

      if (HasMarkersColumn)
        formGrid.DefaultControlsColumn++;
    }

    /// <summary> Configures the title row. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ConfigureTitleRow/*' />
    private void ConfigureTitleRow (FormGridRow titleRow)
    {
      ArgumentUtility.CheckNotNull("titleRow", titleRow);
      CheckFormGridRowType("titleRow", titleRow, FormGridRowType.TitleRow);

      //  Title cell: first row, first cell
      titleRow.SetLabelsCell(0, 0);

      //  Adapt ColSpan for added markers column
      if (HasMarkersColumn)
        titleRow.ControlsColumn++;
    }

    /// <summary> Configures a sub title row. </summary>
    private void ConfigureSubTitleRow (FormGridRow subTitleRow)
    {
      ArgumentUtility.CheckNotNull("subTitleRow", subTitleRow);
      CheckFormGridRowType("subTitleRow", subTitleRow, FormGridRowType.SubTitleRow);

      //  Sub title cell: first row, first cell
      subTitleRow.SetLabelsCell(0, 0);

      //  Adapt ColSpan for added markers column
      if (HasMarkersColumn)
        subTitleRow.ControlsColumn++;
    }

    /// <summary> Configures the unknown rows. </summary>
    private void ConfigureUnknownRow (FormGridRow row)
    {
      ArgumentUtility.CheckNotNull("row", row);
      CheckFormGridRowType("row", row, FormGridRowType.UnknownRow);

      //  Adapt ColSpan for added markers column
      if (HasMarkersColumn)
        row.ControlsColumn++;
    }

    /// <summary> Configures a data row. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ConfigureDataRow/*' />
    private void ConfigureDataRow (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      if (dataRow.LabelsRowIndex != dataRow.ControlsRowIndex)
        dataRow.SetControlsCellDummy(dataRow.LabelsRowIndex, dataRow.ControlsColumn);

      CreateMarkersCell(dataRow);

      SetOrCreateValidationMessagesCell(dataRow);
    }

    protected virtual void FormatFormGrid (FormGrid formGrid)
    {
      ArgumentUtility.CheckNotNull("formGrid", formGrid);

      //  Assign CSS-class to the table if none exists
      if (formGrid.Table.Attributes["class"] == null)
        formGrid.Table.Attributes["class"] = CssClassTable;
    }

    /// <summary> Formats the title row. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormatTitleRow/*' />
    protected virtual void FormatTitleRow (FormGridRow titleRow)
    {
      ArgumentUtility.CheckNotNull("titleRow", titleRow);
      CheckFormGridRowType("titleRow", titleRow, FormGridRowType.TitleRow);

      Assertion.IsNotNull(titleRow.LabelsCell, "titleRow.LabelsCell must not be null.");

      //  Adapt ColSpan for added markers column
      if (HasMarkersColumn)
        titleRow.LabelsCell.ColSpan++;

      //  Adapt ColSpan for added validation error message column
      if (HasValidationMessageColumn)
        titleRow.LabelsCell.ColSpan++;

      AssignCssClassToCell(titleRow.LabelsCell, CssClassTitleCell);

      if (!titleRow.CheckVisibility())
        titleRow.Hide();

      AddShowEmptyCellsHack(titleRow);
    }

    /// <summary> Formats a sub title row. </summary>
    protected virtual void FormatSubTitleRow (FormGridRow subTitleRow)
    {
      ArgumentUtility.CheckNotNull("subTitleRow", subTitleRow);
      CheckFormGridRowType("subTitleRow", subTitleRow, FormGridRowType.SubTitleRow);

      Assertion.IsNotNull(subTitleRow.LabelsCell, "subTitleRow.LabelsCell must not be null.");

      //  Adapt ColSpan for added markers column
      if (HasMarkersColumn)
        subTitleRow.LabelsCell.ColSpan++;

      //  Adapt ColSpan for added validation error message column
      if (HasValidationMessageColumn)
        subTitleRow.LabelsCell.ColSpan++;

      AssignCssClassToCell(subTitleRow.LabelsCell, CssClassSubTitleCell);

      if (!subTitleRow.CheckVisibility())
        subTitleRow.Hide();

      AddShowEmptyCellsHack(subTitleRow);
    }

    /// <summary> Formats the unknown rows. </summary>
    protected virtual void FormatUnknownRow (FormGridRow row)
    {
      ArgumentUtility.CheckNotNull("row", row);
      CheckFormGridRowType("row", row, FormGridRowType.UnknownRow);

      HtmlTableCell cell;
      if (row.HtmlTableRows[0].Cells.Count > row.LabelsColumn)
        cell = row.SetLabelsCell(0, row.LabelsColumn);
      else
        cell = row.HtmlTableRows[0].Cells[row.HtmlTableRows[0].Cells.Count - 1];

      //  Adapt ColSpan for added markers column
      if (HasMarkersColumn)
        cell.ColSpan++;

      //  Adapt ColSpan for added validation error message column
      if (HasValidationMessageColumn)
        cell.ColSpan++;

      // don't hide rows for unknown cells
      //    if (!row.CheckVisibility())
      //      row.Hide();

      AddShowEmptyCellsHack(row);
    }

    /// <summary> Formats a data row. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormatDataRow/*' />
    protected virtual void FormatDataRow (FormGridRow dataRow, bool isTopDataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      AssignCssClassesToCells(dataRow, isTopDataRow);
      AssignCssClassesToInputControls(dataRow);
      AssignCssClassesToValidators(dataRow);

      var isRowVisible = dataRow.CheckVisibility();
      if (isRowVisible)
      {
        CreateRequiredMarker(dataRow);
        CreateHelpProvider(dataRow);
        CreateValidationMarkers(dataRow);

        LoadMarkersIntoCell(dataRow);

        WrapDataRowCellControlsWrapper(dataRow);

        if (ValidatorVisibility == ValidatorVisibility.ValidationMessageInControlsColumn
            || ValidatorVisibility == ValidatorVisibility.ValidationMessageAfterControlsColumn)
        {
          LoadValidationMessagesIntoCell(dataRow);
        }
      }
      else
      {
        dataRow.Hide();
      }

      AddShowEmptyCellsHack(dataRow);

      //  Not implemented, since FrameWork 1.1 takes care of names
      //  Future version might loose this
      //  //  Put a name-tag with the control's ID in front of each control with an validation error
      //  foreach (ValidationError validationError in validationErrors)
      //  {
      //    if (validationError == null)
      //      continue;
      //
      //    //  Add name to controls
      //  }

      CreateDataRowCellWrappers(dataRow);
    }

    private static void WrapDataRowCellControlsWrapper (FormGridRow dataRow)
    {
      Assertion.IsNotNull(dataRow.ControlsCell, "dataRow.ControlsCell must not be null.");

      dataRow.ControlsCell.Controls.AddAt(0, new LiteralControl("<div>") { EnableViewState = false });
      dataRow.ControlsCell.Controls.Add(new LiteralControl("</div>") { EnableViewState = false });
    }

    /// <summary>
    ///   Calls <see cref="AddShowEmptyCellHack"/> for the cells identified by 
    ///   <see cref="FormGridRow"/>.
    /// </summary>
    /// <param name="formGridRow">The <see cref="FormGridRow"/>, must not be <see langword="null"/>.</param>
    protected virtual void AddShowEmptyCellsHack (FormGridRow formGridRow)
    {
      ArgumentUtility.CheckNotNull("formGridRow", formGridRow);

      AddShowEmptyCellHack(formGridRow.LabelsCell);
      AddShowEmptyCellHack(formGridRow.ControlsCell);
      AddShowEmptyCellHack(formGridRow.ControlsCellDummy);
      AddShowEmptyCellHack(formGridRow.MarkersCell);
      AddShowEmptyCellHack(formGridRow.ValidationMessagesCell);
      AddShowEmptyCellHack(formGridRow.ValidationMessagesCellDummy);
    }

    /// <summary> Creates the cell to be used for the markers. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CreateMarkersCell/*' />
    protected void CreateMarkersCell (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      if (!HasMarkersColumn)
        return;

      //  Markers cell is before controls cell
      dataRow.LabelsRow.Cells.Insert(dataRow.ControlsColumn, new HtmlTableCell());
      dataRow.SetMarkersCell(dataRow.LabelsRowIndex, dataRow.ControlsColumn);

      //  Controls cell now one cell to the right
      dataRow.ControlsColumn++;

      //  Control cell in second data row spans labels column to the end of the controls columns
      if (HasSeperateControlsRow(dataRow))
      {
        Assertion.IsNotNull(dataRow.ControlsCell, "dataRow.ControlsCell must not be null.");

        int colSpan = dataRow.ControlsColumn - dataRow.LabelsColumn + 1;
        dataRow.ControlsCell.ColSpan = colSpan;
      }

    }

    /// <summary>
    ///   Loads the markers or place holders into the <see cref="FormGridRow.MarkersCell"/> 
    ///   of the <paramref name="dataRow"/>.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/LoadMarkersIntoCell/*' />
    protected virtual void LoadMarkersIntoCell (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      if (!HasMarkersColumn)
        return;

      ArgumentUtility.CheckNotNull("dataRow.MarkersCell", dataRow.MarkersCell!);

      //  HelpProvider takes left-hand side in column

      var markersCellControlsWrapper = new HtmlGenericControl("div");
      dataRow.MarkersCell.Controls.Add(markersCellControlsWrapper);
      var markersCellControls = markersCellControlsWrapper.Controls;

      if (ShowHelpProviders)
      {
        if (dataRow.HelpProvider != null)
        {
          int formGridRowIndex = dataRow.FormGrid.Rows.IndexOf(dataRow);
          var namingContainer = new FormGridCellNamingContainer();
          namingContainer.ID = dataRow.FormGrid.Table.ID + "_HelpProvider_" + formGridRowIndex;
          namingContainer.Controls.Add(dataRow.HelpProvider);
          markersCellControls.Add(namingContainer);
        }
        else
          markersCellControls.Add(CreateBlankMarker());
      }

      //  ValidationMarker and RequiredMarker share right-hand position
      //  ValidationMarker takes precedence

      if (ShowValidationMarkers && dataRow.ValidationMarker != null)
      {
        markersCellControls.Add(dataRow.ValidationMarker);
      }
      else if (ShowRequiredMarkers && dataRow.RequiredMarker != null)
      {
        markersCellControls.Add(dataRow.RequiredMarker);
      }
      else if (ShowValidationMarkers || ShowRequiredMarkers)
      {
        markersCellControls.Add(CreateBlankMarker());
      }
    }

    private void CreateDataRowCellWrappers (FormGridRow dataRow)
    {
      Assertion.IsNotNull(dataRow.ControlsCell, "dataRow.ControlsCell must not be null.");
      Assertion.IsNotNull(dataRow.LabelsCell, "dataRow.LabelsCell must not be null.");

      dataRow.LabelsCell.Controls.AddAt(0, new LiteralControl("<div>") { EnableViewState = false });
      dataRow.LabelsCell.Controls.Add(new LiteralControl("</div>") { EnableViewState = false });

      dataRow.ControlsCell.Controls.AddAt(0, new LiteralControl("<div>") { EnableViewState = false });
      dataRow.ControlsCell.Controls.Add(new LiteralControl("</div>") { EnableViewState = false });
    }

    /// <summary>
    ///   Applies the <c>FormGridManager</c>'s validator settings to all objects of 
    ///   type <see cref="BaseValidator"/> inside the <paramref name="dataRow"/>.
    /// </summary>
    /// <param name="dataRow"> The <see cref="FormGridRow"/> containing the validators to be overridden. </param>
    protected virtual void ApplyValidatorSettings (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      Assertion.IsNotNull(dataRow.ControlsCell, "dataRow.ControlsCell must not be null.");

      for (int i = 0; i < dataRow.ControlsCell.Controls.Count; i++)
      {
        Control control = (Control)dataRow.ControlsCell.Controls[i];
        BaseValidator? baseValidator = control as BaseValidator;
        IBaseValidator? iBaseValidator = control as IBaseValidator;

        //  Only for validators
        if (baseValidator == null && iBaseValidator == null)
          continue;

        //  FormGrid override
        if (ValidatorVisibility != ValidatorVisibility.ShowValidators)
        {
          if (baseValidator != null)
          {
            baseValidator.Display = ValidatorDisplay.None;
            baseValidator.EnableClientScript = false;
          }
          else if (iBaseValidator != null)
          {
            iBaseValidator.Display = ValidatorDisplay.None;
            iBaseValidator.EnableClientScript = false;
          }
        }
      }
    }

    /// <summary>
    ///   Creates the labels from the controls inside <paramref name="dataRow"/>'s <see cref="FormGridRow.ControlsCell"/>
    ///   if they do not already exist.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CreateLabels/*' />
    protected virtual void CreateLabels (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      Assertion.IsNotNull(dataRow.ControlsCell, "dataRow.ControlsCell must not be null.");

      //  Already has labels
      if (HasContents(dataRow.LabelsCell))
        return;

      for (int i = 0; i < dataRow.ControlsCell.Controls.Count; i++)
      {
        Control control = (Control)dataRow.ControlsCell.Controls[i];
        if (! control.Visible)
          continue;

        //  Query the controls for the string to be used as the labeling Text

        Control? label = null;
        string newID = control.ID + c_generatedLabelSuffix;
        //  SmartLabel knows how the get the contents from ISmartControl
        if (control is ISmartControl)
        {
          SmartLabel smartLabel = CreateSmartLabel();
          smartLabel.ForControl = control.ID;
          label = smartLabel;
        }
          //  For these controls, the label's text will come from the resource dispatcher  
          //  auto:FormGridManagerUniqueID:TableUniqueID:ControlUniqueID_Label:Text
          //  auto:ControlUniqueID_Label:Text should also work
        else if (  control is TextBox
                || control is ListControl
                || control is Table)
          //  Label does not support HtmlControls, only WebControls.
          //    || control is HtmlInputControl
          //    || control is HtmlSelect
          //    || control is HtmlTextArea
          //    || control is HtmlTable)
        {
          Label primitiveLabel = CreateWebLabel();
          primitiveLabel.AssociatedControlID = control.ID;
          label = primitiveLabel;
        }
        else
        {
          //  The control found in this iteration does not get handled by this method.
          continue;
        }

        label.ID = newID;

        //  Add seperator if already a control in the cell

        Assertion.IsNotNull(dataRow.LabelsCell, "dataRow.LabelsCell must not be null.");

        if (HasContents(dataRow.LabelsCell))
        {
          LiteralControl seperator = new LiteralControl(", ");

          //  Not default, but ViewState is needed
          seperator.EnableViewState = true;

          dataRow.LabelsCell.Controls.Add(seperator);

          //  Set Visible after control is added so ViewState knows about it
          seperator.Visible = control.Visible;
        }

        //  Should be default, but better safe than sorry
        label.EnableViewState = true;

        dataRow.LabelsCell.Controls.Add(label);
      }
    }

    /// <summary>
    ///   Creates the <see cref="SmartLabel"/> used by <see cref="CreateLabels"/> to populate the <see cref="FormGridRow.LabelsCell"/>.
    /// </summary>
    protected virtual SmartLabel CreateSmartLabel ()
    {
      return new SmartLabel();
    }

    /// <summary>
    ///   Creates the <see cref="Label"/> used by <see cref="CreateLabels"/> to populate the <see cref="FormGridRow.LabelsCell"/>.
    /// </summary>
    protected virtual Label CreateWebLabel ()
    {
      return new Label();
    }

    /// <summary>
    ///   Creates the validators from the controls inside <paramref name="dataRow"/> if they do not already exist.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CreateValidators/*' />
    protected virtual void CreateValidators (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      Assertion.IsNotNull(dataRow.ControlsCell, "dataRow.ControlsCell must not be null.");

      ArrayList smartControls = new ArrayList();
      //ArrayList validators = new ArrayList();

      //  Split into smart controls and validators
      for (int i = 0; i < dataRow.ControlsCell.Controls.Count; i++)
      {
        Control control = (Control)dataRow.ControlsCell.Controls[i];
        if (control is ISmartControl)
          smartControls.Add(control);
        //else if (control is BaseValidator || control is IBaseValidator)
        //  validators.Add (control);
      }

      for (int i = 0; i < smartControls.Count; i++)
      {
        ISmartControl? smartControl = (ISmartControl)smartControls[i]!;
        if (!smartControl.Visible)
          continue;

        ////  Create Validators only if none are assigned for the SmartControl
        //foreach (IValidator validator in validators)
        //{
        //  BaseValidator baseValidator = validator as BaseValidator;
        //  IBaseValidator iBaseValidator = validator as IBaseValidator;
        //  if (   baseValidator != null
        //      && baseValidator.ControlToValidate == smartControl.ID)
        //  {
        //    return;
        //  }
        //  else if (   iBaseValidator != null
        //           && iBaseValidator.ControlToValidate == smartControl.ID)
        //  {
        //    return;
        //  }
        //}

        foreach (BaseValidator validator in  smartControl.CreateValidators())
          dataRow.ControlsCell.Controls.Add(validator);
      }
    }

    /// <summary>
    ///   Queries the controls in <paramref name="dataRow"/> for their mandatory setting 
    ///   and creates the required marker if necessary.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CreateRequiredMarker/*' />
    protected virtual void CreateRequiredMarker (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      Assertion.IsNotNull(dataRow.LabelsCell, "dataRow.LabelsCell must not be null.");
      Assertion.IsNotNull(dataRow.ControlsCell, "dataRow.ControlsCell must not be null.");

      for (int i = 0; i < dataRow.LabelsCell.Controls.Count; i++)
      {
        Control control = (Control)dataRow.LabelsCell.Controls[i];
        if (!control.Visible)
          continue;

        ISmartControl? smartControl = control as ISmartControl;
        if (smartControl == null)
          continue;

        if (smartControl.IsRequired)
        {
          dataRow.RequiredMarker = CreateRequiredMarker();

          //  We have a marker, rest would be redundant
          return;
        }
      }

      for (int i = 0; i < dataRow.ControlsCell.Controls.Count; i++)
      {
        Control control = (Control)dataRow.ControlsCell.Controls[i];
        if (!control.Visible)
          continue;

        ISmartControl? smartControl = control as ISmartControl;
        if (smartControl == null)
          continue;

        if (smartControl.IsRequired)
        {
          dataRow.RequiredMarker = CreateRequiredMarker();

          //  We have a marker, rest would be redundant
          return;
        }
      }
    }

    /// <summary>
    ///   Queries the controls in <paramref name="dataRow"/> if they provide help
    ///   and creates a help provider.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CreateHelpProvider/*' />
    protected void CreateHelpProvider (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      Assertion.IsNotNull(dataRow.ControlsCell, "dataRow.ControlsCell must not be null.");
      Assertion.IsNotNull(dataRow.LabelsCell, "dataRow.LabelsCell must not be null.");

      dataRow.HelpProvider = GetHelpProviderFromControlsCollection(dataRow.LabelsCell.Controls)
                             ?? GetHelpProviderFromControlsCollection(dataRow.ControlsCell.Controls);
    }

    private Control? GetHelpProviderFromControlsCollection (ControlCollection controls)
    {
      for (int i = 0; i < controls.Count; i++)
      {
        Control control = controls[i];
        if (!control.Visible)
          continue;

        ISmartControl? smartControl = control as ISmartControl;
        if (smartControl == null)
          continue;

        HelpInfo? helpInfo = smartControl.HelpInfo;
        if (helpInfo != null)
        {
          //  We have a help provider, first come, only one served
          return CreateHelpProvider(helpInfo);
        }
      }

      return null;
    }

    protected void UpdateGeneratedRowsVisibility (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      Assertion.IsNotNull(dataRow.ControlsCell, "dataRow.ControlsCell must not be null.");
      Assertion.IsNotNull(dataRow.LabelsCell, "dataRow.LabelsCell must not be null.");

      if (dataRow.LabelsCell.Controls.Count == 1 && dataRow.ControlsCell.Controls.Count == 1)
      {
        dataRow.LabelsCell.Controls[0].Visible = dataRow.ControlsCell.Controls[0].Visible;
      }
      else
      {
        bool isControlVisible = false;
        for (int i = 0; i < dataRow.ControlsCell.Controls.Count; i++)
        {
          Control control = (Control)dataRow.ControlsCell.Controls[i];
          if (control.Visible)
          {
            isControlVisible = true;
            break;
          }
        }
        if (! isControlVisible)
        {
          for (int i = 0; i < dataRow.LabelsCell.Controls.Count; i++)
          {
            Control label = (Control)dataRow.LabelsCell.Controls[i];
            label.Visible = false;
          }
        }
      }
    }

    /// <summary> Queries the control for its read-only setting and transforms it if necessary. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/HandleReadOnlyControls/*' />
    protected virtual void HandleReadOnlyControls (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      Assertion.IsNotNull(dataRow.ControlsCell, "dataRow.ControlsCell must not be null.");

      for (int idxControl = 0; idxControl < dataRow.ControlsCell.Controls.Count; idxControl++)
      {
       Control control = dataRow.ControlsCell.Controls[idxControl];

        if (!control.Visible)
        continue;

        // TODO: Support for non-TextBox, non-ISmartControl controls with read-only option

        TextBox? textBox = control as TextBox;

        if (textBox != null)
        {
          if (textBox.ReadOnly)
          {
            LiteralControl readOnlyValue = new LiteralControl();
            readOnlyValue.EnableViewState = false;
            readOnlyValue.Text = HttpUtility.HtmlEncode(textBox.Text);
            readOnlyValue.ID = textBox.ID;
            dataRow.ControlsCell.Controls.RemoveAt(idxControl);
            dataRow.ControlsCell.Controls.AddAt(idxControl, readOnlyValue);
          }
        }
        else
        {
          //  The control found in this iteration does not get handled by this method.
          continue;
        }
      }
    }

    /// <summary> 
    ///   Sets the cell to be used for the validation messages, creating a new cell if necessary.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/SetOrCreateValidationMessagesCell/*' />
    protected void SetOrCreateValidationMessagesCell (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      //  Validation message cell

      if (ValidatorVisibility == ValidatorVisibility.ValidationMessageInControlsColumn)
      {
        if (!HasSeperateControlsRow(dataRow))
          dataRow.SetValidationMessagesCell(dataRow.ControlsRowIndex, dataRow.ControlsColumn);
        else
          dataRow.SetValidationMessagesCell(dataRow.ControlsRowIndex, dataRow.LabelsColumn);
      }
          //  Validation message cell is after controls cell
      else if (HasValidationMessageColumn)
      {
        if (!HasSeperateControlsRow(dataRow))
        {
          dataRow.ControlsRow.Cells.Insert(dataRow.ControlsColumn + 1, new HtmlTableCell());
          dataRow.SetValidationMessagesCell(dataRow.ControlsRowIndex, dataRow.ControlsColumn + 1);
        }
        else
        {
          dataRow.ControlsRow.Cells.Insert(dataRow.LabelsColumn + 1, new HtmlTableCell());
          dataRow.SetValidationMessagesCell(dataRow.ControlsRowIndex, dataRow.LabelsColumn + 1);

          dataRow.LabelsRow.Cells.Insert(dataRow.ControlsColumn + 1, new HtmlTableCell());
          dataRow.SetValidationMessagesCellDummy(dataRow.LabelsRowIndex, dataRow.ControlsColumn + 1);
        }
      }
    }

    /// <summary> Outputs the validation messages into a <see cref="HtmlTableCell"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/LoadValidationMessagesIntoCell/*' />
    protected virtual void LoadValidationMessagesIntoCell (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);
      ArgumentUtility.CheckNotNull("dataRow.ValidationMessagesCell", dataRow.ValidationMessagesCell!);

      if (dataRow.ValidationErrors != null)
      {
        var validationMessages = new HtmlGenericControl("div");
        validationMessages.EnableViewState = false;

        //  Get validation messages
        for (int i = 0; i < dataRow.ValidationErrors.Length; i++)
        {
          ValidationError validationError = (ValidationError)dataRow.ValidationErrors[i];
          if (validationError == null)
            continue;

          validationMessages.Controls.Add(validationError.ToDiv(CssClassValidationMessage));
        }
        dataRow.ValidationMessagesCell.Controls.Add(validationMessages);
      }
    }

    /// <summary> Assign CSS classes for cells where none exist. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/AssignCssClassesToCells/*' />
    protected virtual void AssignCssClassesToCells (FormGridRow dataRow, bool isTopDataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      Assertion.IsNotNull(dataRow.ControlsCell, "dataRow.ControlsCell must not be null.");
      Assertion.IsNotNull(dataRow.LabelsCell, "dataRow.LabelsCell must not be null.");

      //  Label Cell
      if (dataRow.LabelsCell.Attributes["class"] == null)
      {
        string cssClass = CssClassLabelsCell;

        if (isTopDataRow)
          cssClass += " " + CssClassTopDataRow;

        AssignCssClassToCell(dataRow.LabelsCell, cssClass);
      }

      //  Marker Cell
      if (dataRow.MarkersCell != null)
      {
        string cssClass = CssClassMarkersCell;

        if (isTopDataRow)
          cssClass += " " + CssClassTopDataRow;

        AssignCssClassToCell(dataRow.MarkersCell, cssClass);
      }

      //  Control Cell
      if (dataRow.ControlsCell.Attributes["class"] == null)
      {
        string cssClass = CssClassInputControlsCell;

        if (isTopDataRow && dataRow.ControlsCellDummy == null)
          cssClass += " " + CssClassTopDataRow;

        AssignCssClassToCell(dataRow.ControlsCell, cssClass);
      }

      //  Control Cell Dummy
      if (dataRow.ControlsCellDummy != null)
      {
        string cssClass = CssClassInputControlsCell;

        if (isTopDataRow)
          cssClass += " " + CssClassTopDataRow;

        AssignCssClassToCell(dataRow.ControlsCellDummy, cssClass);
      }

      //  Validation Message Cell
      if (    dataRow.ValidationMessagesCell != null
          &&  ValidatorVisibility == ValidatorVisibility.ValidationMessageAfterControlsColumn)
      {
        string cssClass = CssClassValidationMessagesCell;

        if (isTopDataRow && dataRow.ValidationMessagesCellDummy == null)
          cssClass += " " + CssClassTopDataRow;

        AssignCssClassToCell(dataRow.ValidationMessagesCell, cssClass);
      }

      //  Validation Message Cell Dummy
      if (    dataRow.ValidationMessagesCellDummy != null
          &&  ValidatorVisibility == ValidatorVisibility.ValidationMessageAfterControlsColumn)
      {
        string cssClass = CssClassValidationMessagesCell;

        if (isTopDataRow)
          cssClass += " " + CssClassTopDataRow;

        AssignCssClassToCell(dataRow.ValidationMessagesCellDummy, cssClass);
      }
    }

    /// <summary> Assign CSS classes to input controls where none exist. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/AssignCssClassesToInputControls/*' />
    protected virtual void AssignCssClassesToInputControls (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      Assertion.IsNotNull(dataRow.ControlsCell, "dataRow.ControlsCell must not be null.");

      for (int idxControl = 0; idxControl < dataRow.ControlsCell.Controls.Count; idxControl++)
      {
        Control control = dataRow.ControlsCell.Controls[idxControl];

        // TODO: Query ISmartControl
        //  if ((Control as ISmartControl).UseInputControlStyle)

        TextBox? textBox = control as TextBox;
        DropDownList? dropDownList = control as DropDownList;

        if (textBox != null)
        {
          if (textBox.CssClass.Length == 0)
            textBox.CssClass = CssClassInputControl;
        }
        else if (dropDownList != null)
        {
          if (dropDownList.CssClass.Length == 0)
            dropDownList.CssClass = CssClassInputControl;
        }
        else
        {
          //  The control found in this iteration does not get handled by this method.
          continue;
        }
      }
    }

    /// <summary> Assign CSS classes to validators where none exist. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/AssignCssClassesToInputControls/*' />
    protected virtual void AssignCssClassesToValidators (FormGridRow dataRow)
    {
      ArgumentUtility.CheckNotNull("dataRow", dataRow);
      CheckFormGridRowType("dataRow", dataRow, FormGridRowType.DataRow);

      Assertion.IsNotNull(dataRow.ControlsCell, "dataRow.ControlsCell must not be null.");

      for (int i = 0; i < dataRow.ControlsCell.Controls.Count; i++)
      {
        Control control = (Control)dataRow.ControlsCell.Controls[i];
        BaseValidator? validator = control as BaseValidator;
        if (   validator != null
            && ! string.IsNullOrEmpty(validator.CssClass))
        {
          validator.CssClass = CssClassValidator;
        }
      }
    }

    /// <summary>
    ///   Tests for an empty <c>class</c> attribute and assigns the <paramref name="cssClass"/> 
    ///   if empty.
    /// </summary>
    /// <param name="cell"> The <see cref="HtmlTableCell"/> to be used. </param>
    /// <param name="cssClass"> The <c>CSS-class</c> to assign. </param> 
    protected void AssignCssClassToCell (HtmlTableCell cell, string cssClass) // TODO RM-8118: arg checks
    {
      if (cell.Attributes["class"] == null || cell.Attributes["class"] == string.Empty)
      {
        cell.Attributes["class"] = cssClass;
      }
    }

    /// <summary>
    ///   Adds a white space to the <paramref name="cell"/> to force show the cell in the browser.
    /// </summary>
    /// <param name="cell"> The <see cref="HtmlTableCell"/> to be made visible. </param>
    protected virtual void AddShowEmptyCellHack (HtmlTableCell? cell)
    {
      if (cell != null && cell.Controls.Count == 0)
      {
        cell.Controls.Add(new LiteralControl("&nbsp;") { EnableViewState = false });
      }
    }

    /// <summary> Returns the image URL for the images defined in the <c>FormGridManager</c>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/GetImageUrl/*' />
    protected string GetImageUrl (FormGridImage image)
    {
      string relativeUrl = "sprite.svg#" + image;

      return InfrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, relativeUrl).GetUrl();
    }

    /// <summary> Builds the input required marker. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CreateRequiredMarker1/*' />
    protected virtual Control CreateRequiredMarker ()
    {
      Image requiredIcon = new Image();
      requiredIcon.ImageUrl = GetImageUrl(FormGridImage.RequiredField);

      IResourceManager resourceManager = GetResourceManager();

      requiredIcon.AlternateText = "*";
      requiredIcon.ToolTip = resourceManager.GetString(ResourceIdentifier.RequiredFieldTitle);
      requiredIcon.Attributes.Add(HtmlTextWriterAttribute2.AriaHidden, HtmlAriaHiddenAttributeValue.True);

      return requiredIcon;
    }

    /// <summary> Builds the help provider. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CreateHelpProvider1/*' />
    protected virtual Control CreateHelpProvider (HelpInfo helpInfo)
    {
      ArgumentUtility.CheckNotNull("helpInfo", helpInfo);

      Image helpIcon = new Image();
      helpIcon.ImageUrl = GetImageUrl(FormGridImage.Help);

      IResourceManager resourceManager = GetResourceManager();

      helpIcon.AlternateText = "?";
      helpIcon.Attributes.Add(HtmlTextWriterAttribute2.AriaHidden, HtmlAriaHiddenAttributeValue.True);

      HtmlAnchor helpAnchor = new HtmlAnchor();
      helpAnchor.ID = "HelpLink";
      helpAnchor.Controls.Add(helpIcon);
      helpAnchor.HRef = ResolveClientUrl(helpInfo.NavigateUrl);
      helpAnchor.Target = helpInfo.Target;
      if (!string.IsNullOrEmpty(helpInfo.OnClick))
        helpAnchor.Attributes.Add("onclick", helpInfo.OnClick);

      if (helpInfo.ToolTip == null)
        helpAnchor.Title = resourceManager.GetString(ResourceIdentifier.HelpTitle);
      else
        helpAnchor.Attributes.Add(HtmlTextWriterAttribute2.AriaLabel, helpInfo.ToolTip);

      return helpAnchor;
    }

    /// <summary> Builds a new marker for validation errors. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CreateValidationMarker/*' />
    protected virtual Control CreateValidationMarker (PlainTextString toolTip)
    {
      Image validationErrorIcon = new Image();
      validationErrorIcon.ImageUrl = GetImageUrl(FormGridImage.ValidationError);

      IResourceManager resourceManager = GetResourceManager();

      validationErrorIcon.AlternateText = "!";
      validationErrorIcon.Attributes.Add(HtmlTextWriterAttribute2.AriaHidden, HtmlAriaHiddenAttributeValue.True);

      HtmlAnchor validationAnchor = new HtmlAnchor();
      validationAnchor.Controls.Add(validationErrorIcon);
      if (ValidatorVisibility == ValidatorVisibility.HideValidators)
      {
        validationAnchor.Attributes.Add(HtmlTextWriterAttribute2.AriaLabel, toolTip.GetValue());
        validationAnchor.Attributes["tabindex"] = "0";
      }
      else
      {
        validationAnchor.Title = resourceManager.GetString(ResourceIdentifier.ValidationErrorInfoTitle);
      }
      return validationAnchor;
    }

    /// <summary> Returns a spacer to be used instead of a marker. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CreateBlankMarker/*' />
    protected virtual Control CreateBlankMarker ()
    {
      Image spacer = new Image();
      spacer.ImageUrl = IconInfo.CreateSpacer(ResourceUrlFactory).Url;
      spacer.GenerateEmptyAlternateText = true;
      return spacer;
    }

    /// <summary>
    ///   Compares the <paramref name="formGridRow"/>'s <see cref="FormGridRowType"/> against the 
    ///   type passed in <paramref name="expectedFormGridRowType"/>.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CheckFormGridRowType/*' />
    protected void CheckFormGridRowType (
      string argumentName,
      FormGridRow formGridRow,
      FormGridRowType expectedFormGridRowType)
    {
      if (formGridRow == null || formGridRow.Type != expectedFormGridRowType)
        throw new ArgumentException("Specified FormGridRow is not set to type '" + expectedFormGridRowType.ToString() + "'.", argumentName);
    }

    /// <summary>
    ///   Tests the labels matches the controls row.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/HasSeperateControlsRow/*' />
    protected bool HasSeperateControlsRow (FormGridRow dataRow)
    {
      return dataRow.LabelsRowIndex != dataRow.ControlsRowIndex;
    }

    protected void EnsureFormGridListPopulated ()
    {
      if (! _formGridListPopulated)
      {
        PopulateFormGridList(NamingContainer);
        _formGridListPopulated = true;
      }
    }

    /// <summary> Registers all suffixed tables for this <c>FormGridManager</c>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/PopulateFormGridList/*' />
    private void PopulateFormGridList (Control control)
    {
      //  Add all table having the suffix
      for (int i = 0; i < control.Controls.Count; i++)
      {
        var childControl = control.Controls[i];
        var table = childControl as HtmlTable;
        if (table != null && table.ID != null && table.ID.EndsWith(_formGridSuffix) && !IsFormGridRegistered(table))
          RegisterFormGrid(table);

        if (! (childControl is TemplateControl))
          PopulateFormGridList(childControl);
      }
    }

    /// <summary>
    /// Tests whether the supplied <paramref name="table"/> is registered as a <see cref="FormGrid"/> with this <see cref="FormGridManager"/>.
    /// </summary>
    /// <param name="table">The <see cref="HtmlTable"/> to be used as the <see cref="FormGrid"/>. Must not be <see langword="null" />.</param>
    /// <returns><see langword="true" /> if the <paramref name="table"/> is registered. </returns>
    /// <exception cref="ArgumentException"> Thrown of the <paramref name="table"/> does not have a <see cref="Page"/>.</exception>
    public bool IsFormGridRegistered (HtmlTable table)
    {
      ArgumentUtility.CheckNotNull("table", table);
      if (Page != null && table.Page == null)
        throw new ArgumentException("The HtmlTable passed as FormGrid is not part of this page.", "table");

      return _formGrids.ContainsKey(table.UniqueID);
    }

    /// <summary>
    /// Registers the supplied <paramref name="table"/> as a <see cref="FormGrid"/> with this <see cref="FormGridManager"/>.
    /// </summary>
    /// <param name="table">The <see cref="HtmlTable"/> to be used as the <see cref="FormGrid"/>. Must not be <see langword="null" />.</param>
    /// <exception cref="ArgumentException">
    ///   Thrown of the <paramref name="table"/> does not have a <see cref="Page"/> or has already been registered with this <see cref="FormGridManager"/>.
    /// </exception>
    public void RegisterFormGrid (HtmlTable table)
    {
      ArgumentUtility.CheckNotNull("table", table);

      if (IsFormGridRegistered(table))
        throw new ArgumentException("The HtmlTable passed as FormGrid is already registered with this FormGridManager.", "table");

      if (IsParentControl(table))
      {
        throw new ArgumentException(
            string.Format("A FormGridManager must not be nested in an HtmlTable managed by it: FormGridManager '{0}', HtmlTable '{1}'", ID, table.ID));
      }

      FormGridRow[] rows = CreateFormGridRows(table, _labelsColumn, _controlsColumn);
      _formGrids[table.UniqueID] = new FormGrid(table, rows, _labelsColumn, _controlsColumn);
      table.Load += Table_Load;
      table.PreRender += Table_PreRender;
    }

    /// <summary>
    /// Removes the registration of the supplied <paramref name="table"/> as a <see cref="FormGrid"/> with this <see cref="FormGridManager"/>.
    /// </summary>
    /// <param name="table">The <see cref="HtmlTable"/> to be used as the <see cref="FormGrid"/>. Must not be <see langword="null" />.</param>
    /// <exception cref="ArgumentException">
    ///   Thrown of the <paramref name="table"/> does not have a <see cref="Page"/> or has not been registered with this <see cref="FormGridManager"/>.
    /// </exception>
    public void UnregisterFormGrid (HtmlTable table)
    {
      ArgumentUtility.CheckNotNull("table", table);

      if (!IsFormGridRegistered(table))
        throw new ArgumentException("The HtmlTable passed as FormGrid is not registered with this FormGridManager.", "table");

      _formGrids.Remove(table.UniqueID);
      table.Load -= Table_Load;
      table.PreRender -= Table_PreRender;
      table.SetRenderMethodDelegate(null);
    }

    /// <summary>
    /// Gets the <see cref="FormGrid"/> registered for the supplied <paramref name="table"/>.
    /// </summary>
    /// <param name="table">The <see cref="HtmlTable"/> for which to return the <see cref="FormGrid"/>. Must not be <see langword="null" />.</param>
    /// <exception cref="ArgumentException">
    ///   Thrown of the <paramref name="table"/> does not have a <see cref="Page"/> or has not been registered with this <see cref="FormGridManager"/>.
    /// </exception>
    protected FormGrid GetFormGrid (HtmlTable table)
    {
      ArgumentUtility.CheckNotNull("table", table);

      if (!IsFormGridRegistered(table))
        throw new ArgumentException("The HtmlTable passed as FormGrid is not registered with this FormGridManager.", "table");

      return _formGrids[table.UniqueID];
    }

    private bool IsParentControl (HtmlTable table)
    {
      for (Control? current = Parent; current != null; current = current.Parent)
      {
        if (current == table)
          return true;
      }
      return false;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IGlobalizationService GlobalizationService
    {
      get
      {
        return SafeServiceLocator.Current.GetInstance<IGlobalizationService>();
      }
    }

    [Browsable(false)]
    public HtmlTable[] Tables
    {
      get
      {
        ArrayList tables = new ArrayList();
        foreach (FormGrid grid in _formGrids.Values)
          tables.Add(grid.Table);
        return (HtmlTable[])tables.ToArray(typeof(HtmlTable));
      }
    }

    /// <summary> The suffix identifying all tables managed by this <c>FormGridManager</c>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/FormGridSuffix/*' />
    [Category("Behaviour")]
    [DefaultValue(c_formGridSuffix)]
    [Description("The suffix that must be appended to all tables to be used as a form grid.")]
    public string FormGridSuffix
    {
      get { return _formGridSuffix; }
      set { _formGridSuffix = value; }
    }

    /// <summary>
    ///   Specifies which column in the table or tables contains the labels.
    ///   Must be less than the value of <see cref="ControlsColumn"/>.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/LabelsColumn/*' />
    [Category("Appearance")]
    [DefaultValue(0)]
    [Description("The index of the label column in the form grid tables. Must be less than the ControlsColumn's index")]
    public int LabelsColumn
    {
      get
      {
        return _labelsColumn;
      }
      set
      {
        if (Page!.IsPostBack)  throw new InvalidOperationException("Setting 'LabelsColumn' is only allowed during the initial page load");
        if (value >= _controlsColumn) throw new ArgumentOutOfRangeException("'LabelsColumn' must be lower than 'ControlsColumn'");

        _labelsColumn = value;
      }
    }

    /// <summary>
    ///   Specifies which column in the table or tables contains the controls for single-line rows.
    ///   Must be higher than the value of <see cref="LabelsColumn"/>.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ControlsColumn/*' />
    [Category("Appearance")]
    [DefaultValue(1)]
    [Description("The index of the control column in the form grid tables. Must be higher than the LabelsColumn's index")]
    public int ControlsColumn
    {
      get
      {
        return _controlsColumn;
      }
      set
      {
        if (Page!.IsPostBack)  throw new InvalidOperationException("Setting 'ControlsColumn' is only allowed during the initial page load");
        if (value <= _labelsColumn) throw new ArgumentOutOfRangeException("'ControlsColumn' must be higher than 'LabelsColumn'");

        _controlsColumn = value;
      }
    }

    /// <summary> Defines how the validation messages are displayed. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ValidatorVisibility/*' />
    [Category("Behavior")]
    [DefaultValue(ValidatorVisibility.ValidationMessageInControlsColumn)]
    [Description("The position of the validation messages in the form grids.")]
    public ValidatorVisibility ValidatorVisibility
    {
      get {
        return _validatorVisibility; }
      set {
        _validatorVisibility = value; }
    }

    /// <summary> Enables/Disables the validation markers. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ShowValidationMarkers/*' />
    [Category("Behavior")]
    [DefaultValue(true)]
    [Description("Enables/Disables the validation markers.")]
    public bool ShowValidationMarkers
    {
      get { return _showValidationMarkers; }
      set { _showValidationMarkers = value; }
    }

    /// <summary> Enables/Disables the required markers. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ShowRequiredMarkers/*' />
    [Category("Behavior")]
    [DefaultValue(true)]
    [Description("Enables/Disables the required markers.")]
    public bool ShowRequiredMarkers
    {
      get { return _showRequiredMarkers; }
      set { _showRequiredMarkers = value; }
    }

    /// <summary> Enables/Disables the help providers. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ShowHelpProviders/*' />
    [Category("Behavior")]
    [DefaultValue(true)]
    [Description("Enables/Disables the help providers.")]
    public bool ShowHelpProviders
    {
      get { return _showHelpProviders; }
      set { _showHelpProviders = value; }
    }

    /// <summary> Returns <see langname="true"/> if the markers column is needed. </summary>
    protected virtual bool HasMarkersColumn
    {
      get
      {
        return _showValidationMarkers || _showRequiredMarkers || _showHelpProviders;
      }
    }

    /// <summary>
    ///   Returns <see langname="true"/> if the validation messages are shown in an extra column.
    /// </summary>
    protected virtual bool HasValidationMessageColumn
    {
      get
      {
        return ValidatorVisibility == ValidatorVisibility.ValidationMessageAfterControlsColumn;
      }
    }

    /// <summary> Extension of the images. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ImageExtension/*' />
    protected virtual string ImageExtension
    { get { return ".gif"; } }

    protected virtual IServiceLocator ServiceLocator
    {
      get { return SafeServiceLocator.Current; }
    }

    private IInternalControlMemberCaller MemberCaller
    {
      get { return ServiceLocator.GetInstance<IInternalControlMemberCaller>(); }
    }

    #region protected virtual string CssClass...

    /// <summary> CSS-Class applied to the form grid tables' <c>table</c> tag. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CssClassTable/*' />
    protected virtual string CssClassTable
    { get { return "formGridTable"; } }

    /// <summary> CSS-Class applied to the cell containing the header. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CssClassTitleCell/*' />
    protected virtual string CssClassTitleCell
    { get { return "formGridTitleCell"; } }

    /// <summary> CSS-Class applied to the cell containing a sub title. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CssClassSubTitleCell/*' />
    protected virtual string CssClassSubTitleCell
    { get { return "formGridSubTitleCell"; } }

    /// <summary> CSS-Class applied to the cells containing the labels. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CssClassLabelsCell/*' />
    protected virtual string CssClassLabelsCell
    { get { return "formGridLabelsCell"; } }

    /// <summary>
    ///   CSS-Class applied to the cells containing the marker controls
    ///   (required, validation error, help).
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CssClassMarkersCell/*' />
    protected virtual string CssClassMarkersCell
    { get { return "formGridMarkersCell"; } }

    /// <summary> CSS-Class applied to the cells containing the input controls. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CssClassInputControlsCell/*' />
    protected virtual string CssClassInputControlsCell
    { get { return "formGridControlsCell"; } }

    /// <summary> CSS-Class applied to the cells containing the validation messages. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CssClassValidationMessagesCell/*' />
    protected virtual string CssClassValidationMessagesCell
    { get { return "formGridValidationMessagesCell"; } }

    /// <summary> CSS-Class additionally applied to the first row after the header row. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CssClassTopDataRow/*' />
    protected virtual string CssClassTopDataRow
    { get { return "formGridTopDataRow"; } }

    /// <summary> CSS-Class applied to the input controls. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CssClassInputControl/*' />
    protected virtual string CssClassInputControl
    { get { return "formGridInputControl"; } }

    /// <summary> CSS-Class applied to the individual validation messages. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CssClassValidationMessage/*' />
    protected virtual string CssClassValidationMessage
    { get { return "formGridValidationMessage"; } }

    /// <summary> CSS-Class applied to the validators vreated by the <see cref="FormGridManager"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/CssClassValidator/*' />
    protected virtual string CssClassValidator
    { get { return "formGridValidator"; } }

    #endregion
  }

  /// <summary> Defiens how the validators are displayed in the FormGrid. </summary>
  public enum ValidatorVisibility
  {
    /// <summary> Don't display the validation messages. </summary>
    HideValidators,

    /// <summary> Leave displaying the validation messages to the individual validation controls. </summary>
    ShowValidators,

    /// <summary>
    ///   Display the validation message in the same cell as as the invalid control's.
    ///   Default implementation display each message inside it own <c>div</c>-tag.
    /// </summary>
    ValidationMessageInControlsColumn,

    /// <summary>
    ///   Display the validation message in a new cell inserted after the invalid control's cell.
    ///   Default implementation display each message inside it own <c>div</c>-tag.
    /// </summary>
    ValidationMessageAfterControlsColumn
  }

}
