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
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Common functionality of all control objects representing rows within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>. Specific
  /// classes (<see cref="BocListRowControlObject"/>, <see cref="BocListEditableRowControlObject"/> and
  /// <see cref="BocListAsGridRowControlObject"/>) serve only as different interfaces.
  /// </summary>
  internal class BocListRowFunctionality : WebFormsControlObjectWithDiagnosticMetadata
  {
    private readonly IBocListRowControlObjectHostAccessor _accessor;
    private readonly int _rowIndexOnPage;

    public BocListRowFunctionality (IBocListRowControlObjectHostAccessor accessor, [NotNull] ControlObjectContext context)
        : base (context)
    {
      _accessor = accessor;
      _rowIndexOnPage = int.Parse (Scope[DiagnosticMetadataAttributesForObjectBinding.BocListRowIndex]);
    }

    /// <summary>
    /// Selects the row.
    /// </summary>
    public void Select ()
    {
      var scope = GetRowSelectorCheckboxScope();
      new CheckAction (this, scope).Execute (Opt.ContinueImmediately());
    }

    /// <summary>
    /// Deselects the row.
    /// </summary>
    public void Deselect ()
    {
      var scope = GetRowSelectorCheckboxScope();
      new UncheckAction (this, scope).Execute (Opt.ContinueImmediately());
    }

    /// <summary>
    /// Returns whether the row is currently selected.
    /// </summary>
    public bool IsSelected
    {
      get
      {
        var rowSelectorCheckboxScope = GetRowSelectorCheckboxScope();
        return rowSelectorCheckboxScope.IsSelected();
      }
    }

    private ElementScope GetRowSelectorCheckboxScope ()
    {
      var zeroBasedAbsoluteRowIndexOfFirstRow = _accessor.GetZeroBasedAbsoluteRowIndexOfFirstRow();

      var zeroBasedAbsoluteRowIndex = zeroBasedAbsoluteRowIndexOfFirstRow + _rowIndexOnPage - 1;
      return _accessor.ParentScope.FindChild (string.Format ("RowSelector_{0}", zeroBasedAbsoluteRowIndex));
    }

    /// <inheritdoc/>
    public TCellControlObject GetCellWithColumnItemID<TCellControlObject> ([NotNull] string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);

      var index = _accessor.GetColumnIndexForItemID (columnItemID);
      return GetCellWithColumnIndex<TCellControlObject> (index);
    }

    /// <inheritdoc/>
    public TCellControlObject GetCellWithColumnIndex<TCellControlObject> (int index)
    {
      var cellScope = Scope.FindTagWithAttribute ("td", DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, index.ToString());
      return (TCellControlObject) Activator.CreateInstance (typeof (TCellControlObject), new object[] { Context.CloneForControl (cellScope) });
    }

    /// <inheritdoc/>
    public TCellControlObject GetCellWithColumnTitle<TCellControlObject> ([NotNull] string columnTitle)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnTitle", columnTitle);

      var index = _accessor.GetColumnIndexForTitle (columnTitle);
      return GetCellWithColumnIndex<TCellControlObject> (index);
    }

    /// <inheritdoc/>
    public TCellControlObject GetCellWithColumnTitleContains<TCellControlObject> ([NotNull] string columnTitleContains)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnTitleContains", columnTitleContains);

      var index = _accessor.GetColumnIndexForTitleContains (columnTitleContains);
      return GetCellWithColumnIndex<TCellControlObject> (index);
    }

    /// <inheritdoc/>
    public DropDownMenuControlObject GetDropDownMenu ()
    {
      var cellScope = Scope.FindTagWithAttribute ("td", DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownRowDropDownMenuCell, "true");
      var rowDropDownMenuScope = cellScope.FindCss ("span.DropDownMenuContainer");
      return new DropDownMenuControlObject (Context.CloneForControl (rowDropDownMenuScope));
    }

    /// <summary>
    /// Enters edit-mode for the row.
    /// </summary>
    public BocListEditableRowControlObject Edit ()
    {
      var editCell = GetWellKnownEditCell();

      var edit = editCell.GetControl (new SingleControlSelectionCommand<CommandControlObject> (new CommandSelector()));
      edit.Click();

      return new BocListEditableRowControlObject (_accessor, Context);
    }

    /// <summary>
    /// Saves the edited row and returns to the non-edit mode.
    /// </summary>
    public BocListRowControlObject Save ()
    {
      var editCell = GetWellKnownEditCell();

      var save = editCell.GetControl (new IndexControlSelectionCommand<CommandControlObject> (new CommandSelector(), 1));
      save.Click();

      return new BocListRowControlObject (_accessor, Context);
    }

    /// <summary>
    /// Cancels the editing of the row and returns to the non-edit mode.
    /// </summary>
    public BocListRowControlObject Cancel ()
    {
      var editCell = GetWellKnownEditCell();

      var cancel = editCell.GetControl (new IndexControlSelectionCommand<CommandControlObject> (new CommandSelector(), 2));
      cancel.Click();

      return new BocListRowControlObject (_accessor, Context);
    }

    private BocListEditableCellControlObject GetWellKnownEditCell ()
    {
      var editCellScope = Scope.FindTagWithAttribute ("td", DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownEditCell, "true");
      return new BocListEditableCellControlObject (Context.CloneForControl (editCellScope));
    }
  }
}