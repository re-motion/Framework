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
using System.Web.UI.WebControls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport
{
  public interface IEditModeHost
  {
    IList Value { get; }
    string ID { get; }
    bool IsReadOnly { get; }
    bool IsDirty { get; set; }
    EditableRowDataSourceFactory EditModeDataSourceFactory { get; }
    EditableRowControlFactory EditModeControlFactory { get; }
    string ErrorMessage { get; }
    bool DisableEditModeValidationMessages { get; }
    bool ShowEditModeValidationMarkers { get; }
    bool ShowEditModeRequiredMarkers { get; }
    bool EnableEditModeValidator { get; }
    IRowIDProvider RowIDProvider { get; }
    BocListRow[] AddRows (IBusinessObject[] businessObjects);
    BocListRow[] RemoveRows (IBusinessObject[] bocListRows);
    void EndRowEditModeCleanUp (int value);
    void EndListEditModeCleanUp ();
    bool ValidateEditableRows ();
    void OnEditableRowChangesSaving (int index, IBusinessObject businessObject, IBusinessObjectDataSource dataSource, IBusinessObjectBoundEditableWebControl[] controls);
    void OnEditableRowChangesSaved (int index, IBusinessObject businessObject);
    void OnEditableRowChangesCanceling (int index, IBusinessObject businessObject, IBusinessObjectDataSource dataSource, IBusinessObjectBoundEditableWebControl[] controls);
    void OnEditableRowChangesCanceled (int index, IBusinessObject businessObject);
    Image GetRequiredMarker ();
    Image GetValidationErrorMarker ();
    EditModeValidator GetEditModeValidator ();
    void SetFocus (IFocusableControl control);
  }
}