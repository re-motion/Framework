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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a row in edit-mode within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListEditableRowControlObject
      : WebFormsControlObjectWithDiagnosticMetadata,
          IBocListRowControlObject<BocListEditableCellControlObject>,
          IFluentBocListRowControlObject<BocListEditableCellControlObject>
  {
    private readonly BocListRowFunctionality _impl;

    public BocListEditableRowControlObject (IBocListRowControlObjectHostAccessor accessor, [NotNull] ControlObjectContext context)
        : base(context)
    {
      _impl = new BocListRowFunctionality(accessor, context);
    }

    /// <summary>
    /// Saves the edited row and returns to the non-edit mode.
    /// </summary>
    public BocListRowControlObject Save ()
    {
      return _impl.Save();
    }

    /// <summary>
    /// Cancels the editing of the row and returns to the non-edit mode.
    /// </summary>
    public BocListRowControlObject Cancel ()
    {
      return _impl.Cancel();
    }

    /// <inheritdoc/>
    public IFluentBocListRowControlObject<BocListEditableCellControlObject> GetCell ()
    {
      return this;
    }

    /// <inheritdoc/>
    public BocListEditableCellControlObject GetCell (string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("columnItemID", columnItemID);

      return GetCell().WithColumnItemID(columnItemID);
    }

    /// <inheritdoc/>
    public BocListEditableCellControlObject GetCell (int oneBasedIndex)
    {
      return GetCell().WithIndex(oneBasedIndex);
    }

    /// <summary>
    /// Gets any validation errors assigned to the row.
    /// </summary>
    public BocListValidationError[] GetValidationErrors ()
    {
      return Scope.FindAllCss(".bocListDataCellValidationFailureIndicator ul li")
          .Select(scope => BocListValidationError.Parse(scope, Logger))
          .ToArray();
    }

    /// <inheritdoc/>
    BocListEditableCellControlObject IFluentControlObjectWithCells<BocListEditableCellControlObject>.WithColumnItemID (string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("columnItemID", columnItemID);

      return _impl.GetCellWithColumnItemID<BocListEditableCellControlObject>(columnItemID);
    }

    /// <inheritdoc/>
    BocListEditableCellControlObject IFluentControlObjectWithCells<BocListEditableCellControlObject>.WithIndex (int oneBasedIndex)
    {
      return _impl.GetCellWithColumnIndex<BocListEditableCellControlObject>(oneBasedIndex);
    }

    /// <inheritdoc/>
    BocListEditableCellControlObject IFluentControlObjectWithCells<BocListEditableCellControlObject>.WithColumnTitle (string columnTitle)
    {
      ArgumentUtility.CheckNotNullOrEmpty("columnTitle", columnTitle);

      return _impl.GetCellWithColumnTitle<BocListEditableCellControlObject>(columnTitle);
    }

    /// <inheritdoc/>
    BocListEditableCellControlObject IFluentControlObjectWithCells<BocListEditableCellControlObject>.WithColumnTitleContains (string columnTitleContains)
    {
      ArgumentUtility.CheckNotNullOrEmpty("columnTitleContains", columnTitleContains);

      return _impl.GetCellWithColumnTitleContains<BocListEditableCellControlObject>(columnTitleContains);
    }

    /// <inheritdoc />
    BocListEditableCellControlObject IFluentBocListRowControlObject<BocListEditableCellControlObject>.WithDomainPropertyPaths (string[] domainPropertyPaths)
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("domainPropertyPaths", domainPropertyPaths);

      return _impl.GetCellWithColumnDomainPropertyPaths<BocListEditableCellControlObject>(domainPropertyPaths);
    }
  }
}
