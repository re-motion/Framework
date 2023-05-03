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
    /// <summary>
    /// Adds <see cref="BusinessObjectValidationFailure"/>s that belong to the <see cref="BocList"/> as a whole.
    /// </summary>
    public void AddValidationFailuresForBocList (IEnumerable<BusinessObjectValidationFailure> validationFailures);

    /// <summary>
    /// Adds <see cref="BusinessObjectValidationFailure"/>s that belong to an <see cref="IBusinessObject"/> row inside the <see cref="BocList"/>.
    /// </summary>
    public void AddValidationFailuresForDataRow (IBusinessObject rowObject, IEnumerable<BusinessObjectValidationFailure> validationFailures);

    /// <summary>
    /// Adds <see cref="BusinessObjectValidationFailure"/>s that belong to an <see cref="IBocColumnDefinitionWithValidationSupport"/> cell
    /// inside an <see cref="IBusinessObject"/> row inside the <see cref="BocList"/>.
    /// </summary>
    public void AddValidationFailuresForDataCell (
        IBusinessObject rowObject,
        IBocColumnDefinitionWithValidationSupport cellDefinition,
        IEnumerable<BusinessObjectValidationFailure> validationFailures);

    /// <summary>
    /// Returns a collection of all unhandled <see cref="BusinessObjectValidationFailure"/>s associated directly with the <see cref="BocList"/>.
    /// </summary>
    /// <remarks>
    /// Validation failures associated with the <see cref="IBusinessObject"/>s of individual rows and cells are not included in this result.
    /// </remarks>
    /// <param name="markAsHandled">If set to true, validation failures will not be returned again by any of the <c>GetUnhandled*</c> methods of this repository.</param>
    public IReadOnlyCollection<BusinessObjectValidationFailure> GetUnhandledValidationFailuresForBocList (bool markAsHandled);

    /// <summary>
    /// Returns a collection of all unhandled <see cref="BusinessObjectValidationFailure"/>s associated directly with the <see cref="BocList"/>
    /// itself and any of the (nested) property values associated with the <see cref="BusinessObject"/>s of individual rows and cells.
    /// </summary>
    /// <param name="markAsHandled">If set to true, validation failures will not be returned again by any of the <c>GetUnhandled*</c> methods of this repository.</param>
    public IReadOnlyCollection<BusinessObjectValidationFailure> GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells (bool markAsHandled);

    /// <summary>
    /// Returns a collection of all unhandled <see cref="BusinessObjectValidationFailure"/>s that exist for the <paramref name="rowObject"/>.
    /// </summary>
    /// <param name="rowObject">The row object the failures belong to.</param>
    /// <param name="markAsHandled">If set to true, validation failures will not be returned again by any of the <c>GetUnhandled*</c> methods of this repository.</param>
    public IReadOnlyCollection<BusinessObjectValidationFailure> GetUnhandledValidationFailuresForDataRow (IBusinessObject rowObject, bool markAsHandled);

    /// <summary>
    /// Returns a collection of all unhandled <see cref="BusinessObjectValidationFailure"/>s that exist for the <paramref name="rowObject"/> itself
    /// and any of the (nested) property values associated with this <see cref="BocList"/> row.
    /// </summary>
    /// <param name="rowObject">The row object the failures belong to.</param>
    /// <param name="markAsHandled">If set to true, validation failures will not be returned again by any of the <c>GetUnhandled*</c> methods of this repository.</param>
    public IReadOnlyCollection<BusinessObjectValidationFailure> GetUnhandledValidationFailuresForDataRowAndContainingDataCells (IBusinessObject rowObject, bool markAsHandled);

    /// <summary>
    /// Returns a collection of all unhandled <see cref="BusinessObjectValidationFailure"/>s
    /// that exist for the <paramref name="rowObject"/>'s <paramref name="columnDefinition" />.
    /// </summary>
    /// <param name="rowObject">The row object the failures belong to.</param>
    /// <param name="columnDefinition">The column definition the failures belong to.</param>
    /// <param name="markAsHandled">If set to true, validation failures will not be returned again by any of the <c>GetUnhandled*</c> methods of this repository.</param>
    public IReadOnlyCollection<BusinessObjectValidationFailure> GetUnhandledValidationFailuresForDataCell (
        IBusinessObject rowObject,
        IBocColumnDefinitionWithValidationSupport columnDefinition,
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
