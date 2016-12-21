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
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport
{
  [ToolboxItem (false)]
  public class EditableRow : PlaceHolder, INamingContainer, IEditableRow
  {
    // types

    // static members and constants

    // member fields

    private readonly IEditModeHost _editModeHost;

    private EditableRowDataSourceFactory _dataSourceFactory;
    private EditableRowControlFactory _controlFactory;
  
    private IBusinessObjectReferenceDataSource _dataSource;

    private PlaceHolder _editControls;
    private PlaceHolder _validatorControls;

    private bool _isRowEditModeValidatorsRestored;
    private IBusinessObjectBoundEditableWebControl[] _rowEditModeControls;

    // construction and disposing

    public EditableRow (IEditModeHost editModeHost)
    {
      ArgumentUtility.CheckNotNull ("editModeHost", editModeHost);

      _editModeHost = editModeHost;
    }

    // methods and properties

    public EditableRowDataSourceFactory DataSourceFactory
    {
      get 
      {
        return _dataSourceFactory; 
      }
      set 
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _dataSourceFactory = value; 
      }
    }

    public EditableRowControlFactory ControlFactory
    {
      get 
      {
        return _controlFactory; 
      }
      set 
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _controlFactory = value; 
      }
    }

    public virtual void CreateControls (IBusinessObject value, BocColumnDefinition[] columns)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

      if (_dataSourceFactory == null)
      {
        throw new InvalidOperationException (
            string.Format ("BocList '{0}': DataSourceFactory has not been set prior to invoking CreateControls().", _editModeHost.ID));
      }

      if (_controlFactory == null)
      {
        throw new InvalidOperationException (
            string.Format ("BocList '{0}': ControlFactory has not been set prior to invoking CreateControls().", _editModeHost.ID));
      }

      CreatePlaceHolders (columns);

      _dataSource = _dataSourceFactory.Create (value);

      _rowEditModeControls = new IBusinessObjectBoundEditableWebControl[columns.Length];

      for (int idxColumns = 0; idxColumns < columns.Length; idxColumns++)
      {
        BocSimpleColumnDefinition simpleColumn = columns[idxColumns] as BocSimpleColumnDefinition;

        if (IsColumnEditable (simpleColumn))
        {
          IBusinessObjectBoundEditableWebControl control = _controlFactory.Create (simpleColumn, idxColumns);

          if (control != null)
          {
            control.ID = idxColumns.ToString();
            control.DataSource = _dataSource;
            IBusinessObjectPropertyPath propertyPath = simpleColumn.GetPropertyPath ();
            control.Property = propertyPath.Properties[0];
            SetEditControl (idxColumns, control);

            _rowEditModeControls[idxColumns] = control;
          }
        }
      }  
      _isRowEditModeValidatorsRestored = false;
    }

    protected void CreatePlaceHolders (BocColumnDefinition[] columns)
    {
      RemoveControls();

      _editControls = new PlaceHolder();
      Controls.Add (_editControls);

      _validatorControls = new PlaceHolder();
      Controls.Add (_validatorControls);

      for (int idxColumns = 0; idxColumns < columns.Length; idxColumns++)
      {
        _editControls.Controls.Add (new PlaceHolder());
        _validatorControls.Controls.Add (new PlaceHolder());
      }
    }

    protected bool IsColumnEditable (BocSimpleColumnDefinition column)
    {
      if (column == null)
        return false;

      if (column.IsReadOnly)
        return false;

      var propertyPath = column.GetPropertyPath();

      if (propertyPath.IsDynamic)
        return false;

      if (propertyPath.Properties.Count > 1)
        return false;

      return true;
    }

    public void RemoveControls()
    {
      ClearChildState();
      Controls.Clear();
      _editControls = null;
      _validatorControls = null;
    }

    public IBusinessObjectReferenceDataSource GetDataSource()
    {
      return _dataSource;
    }

    protected void SetEditControl (int index, IBusinessObjectBoundEditableWebControl control)
    {
      Control webControl = ArgumentUtility.CheckNotNullAndType<Control> ("control", control);

      ControlCollection cellControls = GetEditControls (index);
      cellControls.Clear();
      cellControls.Add (webControl);
    }

    private ControlCollection GetEditControls (int columnIndex)
    {
      if (columnIndex < 0 || columnIndex >= _editControls.Controls.Count) throw new ArgumentOutOfRangeException ("columnIndex");

      return _editControls.Controls[columnIndex].Controls;
    }

    public IBusinessObjectBoundEditableWebControl GetEditControl (int columnIndex)
    {
      if (HasEditControl (columnIndex))
      {
        ControlCollection controls = GetEditControls (columnIndex);
        return (IBusinessObjectBoundEditableWebControl) controls[0];
      }
      else
      {
        return null;
      }
    }

    public bool HasEditControls ()
    {
      return _editControls != null;
    }

    public bool HasEditControl (int columnIndex)
    {
      if (HasEditControls())
        return GetEditControls (columnIndex).Count > 0;
      else
        return false;
    }

    protected void AddToValidators (int columnIndex, IEnumerable<BaseValidator> validators)
    {
      ArgumentUtility.CheckNotNull ("validators", validators);

      ControlCollection cellValidators = GetValidators (columnIndex);
      foreach (var validator in validators)
        cellValidators.Add (validator);
    }

    public ControlCollection GetValidators (int columnIndex)
    {
      if (columnIndex < 0 || columnIndex >= _validatorControls.Controls.Count) throw new ArgumentOutOfRangeException ("columnIndex");
    
      if (HasEditControl (columnIndex))
        return _validatorControls.Controls[columnIndex].Controls;
      else
        return null;
    }

    public bool HasValidators ()
    {
      return _validatorControls != null;
    }

    public bool HasValidators (int columnIndex)
    {
      if (HasValidators())
        return HasEditControl (columnIndex);
      else
        return false;
    }

    /// <remarks>
    ///   Validators must be added to the controls collection after LoadPostData is complete.
    ///   If not, invalid validators will know that they are invalid without first calling validate.
    /// </remarks>
    public void EnsureValidatorsRestored()
    {
      if (_isRowEditModeValidatorsRestored)
        return;
      _isRowEditModeValidatorsRestored = true;

      if (! HasEditControls())
        return;

      for (int i = 0; i < _editControls.Controls.Count; i++)
        CreateValidators (i);
    }

    protected void CreateValidators (int columnIndex)
    {
      if (HasEditControl (columnIndex))
      {
        IBusinessObjectBoundEditableWebControl editControl = GetEditControl (columnIndex);
        var validators = editControl.CreateValidators();
        AddToValidators (columnIndex, validators);
      }
    }

    public void PrepareValidation ()
    {
      if (! HasEditControls())
        return;

      for (int i = 0; i < _editControls.Controls.Count; i++)
        PrepareValidation (i);
    }

    protected void PrepareValidation (int columnIndex)
    {
      if (HasEditControl (columnIndex))
      {
        IBusinessObjectBoundEditableWebControl editControl = GetEditControl (columnIndex);
        editControl.PrepareValidation ();
      }
    }

    public bool Validate ()
    {
      bool isValid = true;

      if (HasValidators())
      {
        for (int i = 0; i < _editControls.Controls.Count; i++)
          isValid &= Validate (i);
      }

      return isValid;
    }

    protected bool Validate (int columnIndex)
    {
      bool isValid = true;

      if (HasValidators (columnIndex))
      {
        ControlCollection cellValidators = GetValidators (columnIndex);
        for (int i = 0; i < cellValidators.Count; i++)
        {
          BaseValidator validator = (BaseValidator) cellValidators[i];
          validator.Validate();
          isValid &= validator.IsValid;
        }
      }

      return isValid;
    }

    public string[] GetTrackedClientIDs()
    {
      StringCollection trackedIDs = new StringCollection();

      if (HasEditControls())
      {
        for (int i = 0; i < _editControls.Controls.Count; i++)
          trackedIDs.AddRange (GetTrackedClientIDs (i));
      }

      string[] trackedIDsArray = new string[trackedIDs.Count];
      trackedIDs.CopyTo (trackedIDsArray, 0);
      return trackedIDsArray;
    }

    protected string[] GetTrackedClientIDs (int columnIndex)
    {
      if (HasEditControl (columnIndex))
        return GetEditControl (columnIndex).GetTrackedClientIDs();
      else
        return new string[0];
    }

    public bool IsDirty()
    {
      if (HasEditControls())
      {
        for (int i = 0; i < _editControls.Controls.Count; i++)
        {
          if (IsDirty (i))
            return true;
        }
      }
      return false;
    }
  
    protected bool IsDirty (int columnIndex)
    {
      if (HasEditControl (columnIndex))
        return GetEditControl (columnIndex).IsDirty;
      else
        return false;
    }

    public bool IsRequired (int columnIndex)
    {
      if (HasEditControl (columnIndex))
        return GetEditControl (columnIndex).IsRequired;
      else
        return false;
    }

    public IBusinessObjectBoundEditableWebControl[] GetEditControlsAsArray()
    {
      return (IBusinessObjectBoundEditableWebControl[]) _rowEditModeControls.Clone ();
    }

    public void RenderSimpleColumnCellEditModeControl (
        HtmlTextWriter writer, 
        BocSimpleColumnDefinition column,
        IBusinessObject businessObject,
        int columnIndex)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("column", column);
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      if (! HasEditControl (columnIndex))
        return;
  
      ControlCollection validators = GetValidators (columnIndex);

      IBusinessObjectBoundEditableWebControl editModeControl = _rowEditModeControls[columnIndex];

      bool enforceWidth = column.EnforceWidth 
                          && ! column.Width.IsEmpty
                          && column.Width.Type != UnitType.Percentage;

      if (enforceWidth)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, column.Width.ToString (CultureInfo.InvariantCulture));

      writer.AddAttribute (HtmlTextWriterAttribute.Class, "bocListEditableCell");
      writer.RenderBeginTag (HtmlTextWriterTag.Span); // Span Container

      if (_editModeHost.ShowEditModeValidationMarkers)
      {
        bool isCellValid = true;
        Image validationErrorMarker = _editModeHost.GetValidationErrorMarker();

        for (int i = 0; i < validators.Count; i++)
        {
          BaseValidator validator = (BaseValidator) validators[i];
          isCellValid &= validator.IsValid;
          if (! validator.IsValid)
          {
            if (string.IsNullOrEmpty (validationErrorMarker.ToolTip))
            {
              validationErrorMarker.ToolTip = validator.ErrorMessage;
            }
            else
            {
              validationErrorMarker.ToolTip += "\r\n";
              validationErrorMarker.ToolTip += validator.ErrorMessage;
            }
          }
        }
        if (! isCellValid)
          validationErrorMarker.RenderControl (writer);
      }

      writer.AddAttribute (HtmlTextWriterAttribute.Class, "control");
      writer.RenderBeginTag (HtmlTextWriterTag.Span); // Span Control

      editModeControl.RenderControl (writer);

      writer.RenderEndTag(); // Span Control

      foreach (BaseValidator validator in validators)
      {
        writer.RenderBeginTag (HtmlTextWriterTag.Div);
        validator.RenderControl (writer);
        writer.RenderEndTag();

        if (   ! validator.IsValid 
               && validator.Display == ValidatorDisplay.None
               && ! _editModeHost.DisableEditModeValidationMessages)
        {
          if (! string.IsNullOrEmpty (validator.CssClass))
            writer.AddAttribute (HtmlTextWriterAttribute.Class, validator.CssClass);
          else
            writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassEditModeValidationMessage);
          writer.RenderBeginTag (HtmlTextWriterTag.Div);
          writer.Write (validator.ErrorMessage); // Do not HTML encode.
          writer.RenderEndTag();
        }
      }

      writer.RenderEndTag(); // Span Container
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (Controls.Count > 0)
        PreRenderValidators();
    }

    private void PreRenderValidators ()
    {
      var editModeValidator = _editModeHost.GetEditModeValidator();
      var disableEditModeValidationMessages = _editModeHost.DisableEditModeValidationMessages;

      foreach (var validator in _validatorControls.Controls.Cast<Control>().SelectMany (placeHolder => placeHolder.Controls.Cast<BaseValidator>()))
      {
        if (editModeValidator == null || disableEditModeValidationMessages)
        {
          validator.Display = ValidatorDisplay.None;
          validator.EnableClientScript = false;
        }
        else
        {
          validator.Display = editModeValidator.Display;
          validator.EnableClientScript = editModeValidator.EnableClientScript;
        }
      }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s edit mode validation messages. </summary>
    /// <remarks>
    ///   <para> Class: <c>bocListEditModeValidationMessage</c> </para>
    ///   <para> Only applied if the <see cref="EditModeValidator"/> has no CSS-class of its own. </para>
    ///   </remarks>
    protected virtual string CssClassEditModeValidationMessage
    { get { return "bocListEditModeValidationMessage"; } }
  
  }
}