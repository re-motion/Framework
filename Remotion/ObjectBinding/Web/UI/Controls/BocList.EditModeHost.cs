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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public partial class BocList
  {
    private class EditModeHost : IEditModeHost
    {
      private readonly BocList _bocList;

      public EditModeHost (BocList bocList)
      {
        _bocList = bocList;
      }

      public IReadOnlyList<IBusinessObject>? Value
      {
        get { return _bocList.Value; }
      }

      public string? ID
      {
        get { return _bocList.ID; }
      }

      public bool IsReadOnly
      {
        get { return _bocList.IsReadOnly; }
      }

      public bool IsDirty
      {
        get { return _bocList.IsDirty; }
        set { _bocList.IsDirty = value; }
      }


      public bool? EnableOptionalValidators
      {
        get { return _bocList.EnableOptionalValidators; }
      }

      public bool IsInlineValidationDisplayEnabled
      {
        get { return ((IBocList)_bocList).IsInlineValidationDisplayEnabled; }
      }

      public EditableRowDataSourceFactory EditModeDataSourceFactory
      {
        get { return _bocList.EditModeDataSourceFactory; }
      }

      public EditableRowControlFactory EditModeControlFactory
      {
        get { return _bocList.EditModeControlFactory; }
      }

      public PlainTextString ErrorMessage
      {
        get { return _bocList.ErrorMessage; }
      }

      public bool DisableEditModeValidationMessages
      {
        get { return _bocList.DisableEditModeValidationMessages; }
      }

      public bool ShowEditModeValidationMarkers
      {
        get { return _bocList.ShowEditModeValidationMarkers; }
      }

      public bool ShowEditModeRequiredMarkers
      {
        get { return _bocList.ShowEditModeRequiredMarkers; }
      }

      public bool EnableEditModeValidator
      {
        get { return _bocList.EnableEditModeValidator; }
      }

      public bool IsAutoFocusOnSwitchToEditModeEnabled
      {
        get { return _bocList.EnableAutoFocusOnSwitchToEditMode; }
      }

      public IRowIDProvider RowIDProvider
      {
        get { return _bocList.RowIDProvider; }
      }

      public Image GetRequiredMarker ()
      {
        return _bocList.GetRequiredMarker();
      }

      public Control GetValidationErrorMarker ()
      {
        return _bocList.GetValidationErrorMarker();
      }

      public EditModeValidator? GetEditModeValidator ()
      {
        return _bocList.GetEditModeValidator();
      }

      public BocListRow[] AddRows (IBusinessObject[] businessObjects)
      {
        return _bocList.AddRowsImplementation(businessObjects);
      }

      public BocListRow[] RemoveRows (IBusinessObject[] businessObjects)
      {
        return _bocList.RemoveRowsImplementation(businessObjects);
      }

      public void EndRowEditModeCleanUp (int modifiedRowIndex)
      {
        _bocList.EndRowEditModeCleanUp(modifiedRowIndex);
      }

      public void EndListEditModeCleanUp ()
      {
        _bocList.EndListEditModeCleanUp();
      }

      public bool ValidateEditableRows ()
      {
        return _bocList.ValidateCustomColumns();
      }

      public void OnEditableRowChangesSaving (
          int index,
          IBusinessObject businessObject,
          IBusinessObjectDataSource dataSource,
          IBusinessObjectBoundEditableWebControl[] controls)
      {
        _bocList.OnEditableRowChangesSaving(index, businessObject, dataSource, controls);
      }

      public void OnEditableRowChangesSaved (int index, IBusinessObject businessObject)
      {
        _bocList.OnEditableRowChangesSaved(index, businessObject);
      }

      public void OnEditableRowChangesCanceling (
          int index,
          IBusinessObject businessObject,
          IBusinessObjectDataSource dataSource,
          IBusinessObjectBoundEditableWebControl[] controls)
      {
        _bocList.OnEditableRowChangesCanceling(index, businessObject, dataSource, controls);
      }

      public void OnEditableRowChangesCanceled (int index, IBusinessObject businessObject)
      {
        _bocList.OnEditableRowChangesCanceled(index, businessObject);
      }

      public void SetFocus (IFocusableControl control)
      {
        _bocList.SetFocusImplementation(control);
      }
    }
  }
}
