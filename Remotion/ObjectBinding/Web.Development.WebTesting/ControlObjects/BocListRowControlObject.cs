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

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a row within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListRowControlObject
      : WebFormsControlObjectWithDiagnosticMetadata,
          IDropDownMenuHost,
          IBocListRowControlObject<BocListCellControlObject>,
          IFluentBocListRowControlObject<BocListCellControlObject>,
          IStyledControlObject
  {
    private readonly BocListRowFunctionality _impl;

    public BocListRowControlObject (IBocListRowControlObjectHostAccessor accessor, [NotNull] ControlObjectContext context)
        : base(context)
    {
      _impl = new BocListRowFunctionality(accessor, context);
      ((IControlObjectNotifier)_impl).ActionExecute += OnActionExecute;
    }

    /// <inheritdoc/>
    public IStyleInformation StyleInfo
    {
      get { return new DefaultStyleInformation(this, Scope); }
    }

    /// <summary>
    /// Selects all rows by checking the table's select all checkbox.
    /// </summary>
    public void Select ()
    {
      _impl.Select();
    }

    /// <summary>
    /// Unselect all rows by checking the table's select all checkbox.
    /// </summary>
    /// <exception cref="WebTestException">Thrown if the row-selection is based on radio buttons instead of checkboxes.</exception>
    public void Deselect ()
    {
      _impl.Deselect();
    }

    /// <summary>
    /// Returns whether the row is currently selected.
    /// </summary>
    public bool IsSelected
    {
      get { return _impl.IsSelected; }
    }

    /// <summary>
    /// Enters edit-mode for the row.
    /// </summary>
    public BocListEditableRowControlObject Edit ()
    {
      return _impl.Edit();
    }

    /// <inheritdoc/>
    public IFluentBocListRowControlObject<BocListCellControlObject> GetCell ()
    {
      return this;
    }

    /// <inheritdoc/>
    public BocListCellControlObject GetCell (string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("columnItemID", columnItemID);

      return GetCell().WithColumnItemID(columnItemID);
    }

    /// <inheritdoc/>
    public BocListCellControlObject GetCell (int oneBasedIndex)
    {
      return GetCell().WithIndex(oneBasedIndex);
    }

    /// <summary>
    /// Gets any validation errors assigned to the row.
    /// </summary>
    public BocListValidationError[] GetValidationErrors ()
    {
      return _impl.GetValidationErrors();
    }

    /// <inheritdoc/>
    BocListCellControlObject IFluentControlObjectWithCells<BocListCellControlObject>.WithColumnItemID (string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("columnItemID", columnItemID);

      return _impl.GetCellWithColumnItemID<BocListCellControlObject>(columnItemID);
    }

    /// <inheritdoc/>
    BocListCellControlObject IFluentControlObjectWithCells<BocListCellControlObject>.WithIndex (int oneBasedIndex)
    {
      return _impl.GetCellWithColumnIndex<BocListCellControlObject>(oneBasedIndex);
    }

    /// <inheritdoc/>
    BocListCellControlObject IFluentControlObjectWithCells<BocListCellControlObject>.WithColumnTitle (string columnTitle)
    {
      ArgumentUtility.CheckNotNullOrEmpty("columnTitle", columnTitle);

      return _impl.GetCellWithColumnTitle<BocListCellControlObject>(columnTitle);
    }

    /// <inheritdoc/>
    BocListCellControlObject IFluentControlObjectWithCells<BocListCellControlObject>.WithColumnTitleContains (string columnTitleContains)
    {
      ArgumentUtility.CheckNotNullOrEmpty("columnTitleContains", columnTitleContains);

      return _impl.GetCellWithColumnTitleContains<BocListCellControlObject>(columnTitleContains);
    }

    /// <inheritdoc />
    BocListCellControlObject IFluentBocListRowControlObject<BocListCellControlObject>.WithDomainPropertyPaths (string[] domainPropertyPaths)
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("domainPropertyPaths", domainPropertyPaths);

      return _impl.GetCellWithColumnDomainPropertyPaths<BocListCellControlObject>(domainPropertyPaths);
    }

    /// <inheritdoc/>
    public DropDownMenuControlObject GetDropDownMenu ()
    {
      return _impl.GetDropDownMenu();
    }
  }
}
