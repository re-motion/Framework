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
using System.ComponentModel;
using System.Web.UI.WebControls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport
{
  [ToolboxItem (false)]
  public class EditModeValidator : CustomValidator
  {
    // types

    // static members and constants

    // member fields
    private readonly IEditModeController _editModeController;

    // construction and disposing

    public EditModeValidator (IEditModeController editModeController)
    {
      _editModeController = editModeController;
    }

    // methods and properties

    protected override bool EvaluateIsValid()
    {
      return _editModeController.Validate();
    }

    protected override bool ControlPropertiesValid()
    {
      string controlToValidate = ControlToValidate;
      if (string.IsNullOrEmpty (controlToValidate))
        return base.ControlPropertiesValid();
      else
        return NamingContainer.FindControl (controlToValidate) != null;
    }
  }
}