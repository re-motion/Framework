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
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Common functionality of all control objects representing cells within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>. Specific
  /// classes (<see cref="BocListCellControlObject"/>, <see cref="BocListEditableCellControlObject"/> and
  /// <see cref="BocListAsGridCellControlObject"/>) serve only as different interfaces.
  /// </summary>
  internal class BocListCellFunctionality : WebFormsControlObjectWithDiagnosticMetadata, ICommandHost, IControlHost, IControlObjectWithText
  {
    public BocListCellFunctionality ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      return Scope.Text.Trim();
    }

    /// <summary>
    /// Gets any validation errors assigned to the cell.
    /// </summary>
    public BocListValidationError[] GetValidationErrors ()
    {
      return Scope.FindAllCss("div.screenReaderText:last-child ul li", options: Options.NoWait)
          .Select(BocListValidationError.Parse)
          .ToArray();
    }

    /// <inheritdoc/>
    public CommandControlObject GetCommand ()
    {
      var commandScope = Scope.FindLink();
      return new CommandControlObject(Context.CloneForControl(commandScope));
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject ExecuteCommand (IWebTestActionOptions? actionOptions)
    {
      return GetCommand().Click(actionOptions);
    }

    /// <inheritdoc/>
    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull("controlSelectionCommand", controlSelectionCommand);

      return Children.GetControl(controlSelectionCommand);
    }

    /// <inheritdoc/>
    public TControlObject? GetControlOrNull<TControlObject> (IControlOptionalSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull("controlSelectionCommand", controlSelectionCommand);

      return Children.GetControlOrNull(controlSelectionCommand);
    }

    /// <inheritdoc/>
    public bool HasControl (IControlExistsCommand controlSelectionCommand)
    {
      ArgumentUtility.CheckNotNull("controlSelectionCommand", controlSelectionCommand);

      return Children.HasControl(controlSelectionCommand);
    }
  }
}
