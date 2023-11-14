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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.EditableRowSupport
{
  public class FakeEditModeHost : IEditModeHost
  {
    public FakeEditModeHost ()
    {
    }

    public Action<int, IBusinessObject> NotifyOnEditableRowChangesCanceled { private get; set; }
    public Action<int, IBusinessObject> NotifyOnEditableRowChangesCanceling { private get; set; }
    public Action<int, IBusinessObject> NotifyOnEditableRowChangesSaved { private get; set; }
    public Action<int, IBusinessObject> NotifyOnEditableRowChangesSaving { private get; set; }
    public Func<IBusinessObject[], BocListRow[]> NotifyAddRows { private  get; set; }
    public Func<IBusinessObject[], BocListRow[]> NotifyRemoveRows { private get; set; }
    public Action<int> NotifyEndRowEditModeCleanUp { private get; set; }
    public Action NotifyEndListEditModeCleanUp { private get; set; }
    public Action NotifyValidateEditableRows { private get; set; }
    public bool AreCustomCellsValid { private get; set; }
    public IFocusableControl FocusedControl { get; set; }

    public IReadOnlyList<IBusinessObject> Value { get; set; }
    public string ID { get; set; }
    public bool IsReadOnly { get; set; }
    public bool IsDirty { get; set; }
    public bool? EnableOptionalValidators { get; set; }
    public bool IsInlineValidationDisplayEnabled { get; set; }
    public EditableRowDataSourceFactory EditModeDataSourceFactory { get; set; }
    public EditableRowControlFactory EditModeControlFactory { get; set; }
    public PlainTextString ErrorMessage { get; set; }
    public bool DisableEditModeValidationMessages { get; set; }
    public bool ShowEditModeValidationMarkers { get; set; }
    public bool ShowEditModeRequiredMarkers { get; set; }
    public bool EnableEditModeValidator { get; set; }
    public bool IsAutoFocusOnSwitchToEditModeEnabled { get; set; }
    public IRowIDProvider RowIDProvider { get; set; }

    public BocListRow[] AddRows (IBusinessObject[] businessObjects)
    {
      if (NotifyAddRows != null)
      {
        var addedRows = NotifyAddRows(businessObjects);
        foreach (var addedRow in addedRows.OrderBy(r=>r.Index))
          RowIDProvider.AddRow(addedRow);
        return addedRows;
      }
      return new BocListRow[0];
    }

    public BocListRow[] RemoveRows (IBusinessObject[] bocListRows)
    {
      if (NotifyRemoveRows != null)
      {
        var removedRows = NotifyRemoveRows(bocListRows);
        foreach (var removedRow in removedRows.OrderByDescending(r=>r.Index))
          RowIDProvider.RemoveRow(removedRow);
        return removedRows;
      }
      return new BocListRow[0];
    }

    public void EndRowEditModeCleanUp (int value)
    {
      if (NotifyEndRowEditModeCleanUp != null)
        NotifyEndRowEditModeCleanUp(value);
    }

    public void EndListEditModeCleanUp ()
    {
      if (NotifyEndListEditModeCleanUp != null)
        NotifyEndListEditModeCleanUp();
    }

    public bool ValidateEditableRows ()
    {
      if (NotifyValidateEditableRows != null)
        NotifyValidateEditableRows();

      return AreCustomCellsValid;
    }

    public void OnEditableRowChangesSaving (int index, IBusinessObject businessObject, IBusinessObjectDataSource dataSource, IBusinessObjectBoundEditableWebControl[] controls)
    {
      if (NotifyOnEditableRowChangesSaving != null)
        NotifyOnEditableRowChangesSaving(index, businessObject);
    }

    public void OnEditableRowChangesSaved (int index, IBusinessObject businessObject)
    {
      if (NotifyOnEditableRowChangesSaved != null)
        NotifyOnEditableRowChangesSaved(index, businessObject);
    }

    public void OnEditableRowChangesCanceling (int index, IBusinessObject businessObject, IBusinessObjectDataSource dataSource, IBusinessObjectBoundEditableWebControl[] controls)
    {
      if (NotifyOnEditableRowChangesCanceling != null)
        NotifyOnEditableRowChangesCanceling(index, businessObject);
    }

    public void OnEditableRowChangesCanceled (int index, IBusinessObject businessObject)
    {
      if (NotifyOnEditableRowChangesCanceled != null)
        NotifyOnEditableRowChangesCanceled(index, businessObject);
    }

    public Image GetRequiredMarker ()
    {
      throw new System.NotImplementedException();
    }

    public Control GetValidationErrorMarker ()
    {
      throw new System.NotImplementedException();
    }

    public EditModeValidator GetEditModeValidator ()
    {
      throw new System.NotImplementedException();
    }

    public void SetFocus (IFocusableControl control)
    {
      FocusedControl = control;
    }
  }
}
