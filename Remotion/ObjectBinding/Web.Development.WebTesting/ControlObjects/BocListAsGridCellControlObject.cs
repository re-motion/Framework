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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a cell within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/> in grid mode.
  /// </summary>
  public class BocListAsGridCellControlObject : WebFormsControlObjectWithDiagnosticMetadata, IControlHost, ICommandHost
  {
    private readonly BocListCellFunctionality _impl;

    public BocListAsGridCellControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
      _impl = new BocListCellFunctionality(context);
    }

    /// <summary>
    /// Gets any validation errors assigned to the cell.
    /// </summary>
    public BocListValidationError[] GetValidationErrors ()
    {
      return _impl.GetValidationErrors();
    }

    /// <summary>
    /// Returns the text content of the cell.
    /// </summary>
    public string GetText ()
    {
      return _impl.GetText();
    }

    /// <inheritdoc/>
    public CommandControlObject GetCommand ()
    {
      return _impl.GetCommand();
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject ExecuteCommand (IWebTestActionOptions? actionOptions = null)
    {
      return _impl.ExecuteCommand(actionOptions);
    }

    /// <inheritdoc/>
    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull("controlSelectionCommand", controlSelectionCommand);

      return _impl.GetControl(controlSelectionCommand);
    }

    /// <inheritdoc/>
    public TControlObject? GetControlOrNull<TControlObject> (IControlOptionalSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull("controlSelectionCommand", controlSelectionCommand);

      return _impl.GetControlOrNull(controlSelectionCommand);
    }

    /// <inheritdoc/>
    public bool HasControl (IControlExistsCommand controlSelectionCommand)
    {
      ArgumentUtility.CheckNotNull("controlSelectionCommand", controlSelectionCommand);

      return _impl.HasControl(controlSelectionCommand);
    }
  }
}
