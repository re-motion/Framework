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
using Remotion.ObjectBinding.Validation;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation
{
  /// <summary>
  /// Defines the API for a repository which contains all <see cref="BusinessObjectValidationFailure"/> belonging to a BocList.
  /// </summary>
  public interface IBocListValidationFailureRepository
  {
    /// <summary> Raised when <see cref="BusinessObjectValidationFailure"/>s are added to the repository. </summary>
    /// <remarks>
    ///   Register for this event to execute code that syncs its validation errors to this <see cref="IBocListValidationFailureRepository"/>.
    /// </remarks>
    public event EventHandler? ValidationFailureAdded;

    /// <summary>
    /// Gets the number of list failures currently contained in the repository.
    /// </summary>
    public int GetListFailureCount ();

    /// <summary>
    /// Gets the number of unhandled list failures currently contained in the repository.
    /// </summary>
    public int GetUnhandledListFailureCount ();

    /// <summary>
    /// Gets the number of row and cell failures currently contained in the repository.
    /// </summary>
    public int GetRowAndCellFailureCount ();

    /// <summary>
    /// Gets the number of unhandled row and cell failures currently contained in the repository.
    /// </summary>
    public int GetUnhandledRowAndCellFailureCount ();

    /// <summary>
    /// Adds <see cref="BusinessObjectValidationFailure"/>s that belong to the <see cref="BocList"/> as a whole.
    /// </summary>
    public void AddValidationFailuresForBocList (IEnumerable<BusinessObjectValidationFailure> validationFailures);

    /// <summary>
    /// Adds <see cref="BusinessObjectValidationFailure"/>s that belong to an <see cref="IBusinessObject"/> row inside the <see cref="BocList"/>.
    /// </summary>
    public void AddValidationFailuresForDataRow (IBusinessObject rowObject, IEnumerable<BusinessObjectValidationFailure> validationFailures);

    /// <summary>
    /// Adds <see cref="BusinessObjectValidationFailure"/>s that belong to an <see cref="BocColumnDefinition"/> cell
    /// inside an <see cref="IBusinessObject"/> row inside the <see cref="BocList"/>.
    /// </summary>
    public void AddValidationFailuresForDataCell (
        IBusinessObject rowObject,
        BocColumnDefinition cellDefinition,
        IEnumerable<BusinessObjectValidationFailure> validationFailures);

    /// <summary>
    /// Returns a collection of all unhandled <see cref="BusinessObjectValidationFailure"/>s associated directly with the <see cref="BocList"/>.
    /// </summary>
    /// <remarks>
    /// Validation failures associated with the <see cref="IBusinessObject"/>s of individual rows and cells are not included in this result.
    /// </remarks>
    /// <param name="markAsHandled">If set to true, validation failures will not be returned again by any of the <c>GetUnhandled*</c> methods of this repository.</param>
    /// <returns><see cref="BusinessObjectValidationFailure"/>s wrapped with positional information inside <see cref="BocListValidationFailureWithLocationInformation"/>s.</returns>
    public IReadOnlyCollection<BocListValidationFailureWithLocationInformation> GetUnhandledValidationFailuresForBocList (bool markAsHandled);

    /// <summary>
    /// Returns a collection of all unhandled <see cref="BusinessObjectValidationFailure"/>s associated directly with the <see cref="BocList"/>
    /// itself and any of the (nested) property values associated with the <see cref="BusinessObject"/>s of individual rows and cells.
    /// </summary>
    /// <param name="markAsHandled">If set to true, validation failures will not be returned again by any of the <c>GetUnhandled*</c> methods of this repository.</param>
    /// <returns><see cref="BusinessObjectValidationFailure"/>s wrapped with positional information inside <see cref="BocListValidationFailureWithLocationInformation"/>s.</returns>
    public IReadOnlyCollection<BocListValidationFailureWithLocationInformation> GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells (bool markAsHandled);

    /// <summary>
    /// Returns a collection of all unhandled <see cref="BusinessObjectValidationFailure"/>s that exist for the <paramref name="rowObject"/>.
    /// </summary>
    /// <param name="rowObject">The row object the failures belong to.</param>
    /// <param name="markAsHandled">If set to true, validation failures will not be returned again by any of the <c>GetUnhandled*</c> methods of this repository.</param>
    /// <returns><see cref="BusinessObjectValidationFailure"/>s wrapped with positional information inside <see cref="BocListValidationFailureWithLocationInformation"/>s.</returns>
    public IReadOnlyCollection<BocListValidationFailureWithLocationInformation> GetUnhandledValidationFailuresForDataRow (IBusinessObject rowObject, bool markAsHandled);

    /// <summary>
    /// Returns a collection of all unhandled <see cref="BusinessObjectValidationFailure"/>s that exist for all rows
    /// and any of the (nested) property values associated with this <see cref="BocList"/> row.
    /// </summary>
    /// <param name="markAsHandled">If set to true, validation failures will not be returned again by any of the <c>GetUnhandled*</c> methods of this repository.</param>
    /// <returns><see cref="BusinessObjectValidationFailure"/>s wrapped with positional information inside <see cref="BocListValidationFailureWithLocationInformation"/>s.</returns>
    public IReadOnlyCollection<BocListValidationFailureWithLocationInformation> GetUnhandledValidationFailuresForDataRowsAndContainingDataCells (bool markAsHandled);

    /// <summary>
    /// Returns a collection of all unhandled <see cref="BusinessObjectValidationFailure"/>s that exist for the <paramref name="rowObject"/> itself
    /// and any of the (nested) property values associated with this <see cref="BocList"/> row.
    /// </summary>
    /// <param name="rowObject">The row object the failures belong to.</param>
    /// <param name="markAsHandled">If set to true, validation failures will not be returned again by any of the <c>GetUnhandled*</c> methods of this repository.</param>
    /// <returns><see cref="BusinessObjectValidationFailure"/>s wrapped with positional information inside <see cref="BocListValidationFailureWithLocationInformation"/>s.</returns>
    public IReadOnlyCollection<BocListValidationFailureWithLocationInformation> GetUnhandledValidationFailuresForDataRowAndContainingDataCells (
        IBusinessObject rowObject,
        bool markAsHandled);

    /// <summary>
    /// Returns a collection of all unhandled <see cref="BusinessObjectValidationFailure"/>s
    /// that exist for the <paramref name="rowObject"/>'s <paramref name="columnDefinition" />.
    /// </summary>
    /// <param name="rowObject">The row object the failures belong to.</param>
    /// <param name="columnDefinition">The column definition the failures belong to.</param>
    /// <param name="markAsHandled">If set to true, validation failures will not be returned again by any of the <c>GetUnhandled*</c> methods of this repository.</param>
    /// <returns><see cref="BusinessObjectValidationFailure"/>s wrapped with positional information inside <see cref="BocListValidationFailureWithLocationInformation"/>s.</returns>
    public IReadOnlyCollection<BocListValidationFailureWithLocationInformation> GetUnhandledValidationFailuresForDataCell (
        IBusinessObject rowObject,
        BocColumnDefinition columnDefinition,
        bool markAsHandled);

    /// <summary>
    /// Determines if a <see cref="BusinessObjectValidationFailure"/> exists for the <paramref name="rowObject"/> itself
    /// or any of the (nested) property values associated with this <see cref="BocList"/> row.
    /// </summary>
    public bool HasValidationFailuresForDataRow (IBusinessObject rowObject);

    /// <summary>
    /// Resets the <see cref="IBocListValidationFailureRepository"/>.
    /// </summary>
    public void ClearAllValidationFailures ();
  }
}
