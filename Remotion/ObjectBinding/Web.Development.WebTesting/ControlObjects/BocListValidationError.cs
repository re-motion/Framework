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
using Coypu;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Represents information about validation error shown in the BocList.
  /// </summary>
  public class BocListValidationError
  {
    public static BocListValidationError Parse (ElementScope elementScope)
    {
      var errorMessage = elementScope.FindCss("span, a").InnerHTML ?? string.Empty; // .InnerHTML is used instead of .Text as it would return an empty string instead
      var rowItemID = elementScope.GetAttribute(DiagnosticMetadataAttributesForObjectBinding.BocListValidationFailureSourceRow);
      var columnItemID = elementScope.GetAttribute(DiagnosticMetadataAttributesForObjectBinding.BocListValidationFailureSourceColumn);
      var businessObjectUniqueIdentifier = elementScope.GetAttribute(DiagnosticMetadataAttributesForObjectBinding.ValidationFailureSourceBusinessObject);
      var propertyIdentifier = elementScope.GetAttribute(DiagnosticMetadataAttributesForObjectBinding.ValidationFailureSourceProperty);

      return new BocListValidationError(
          errorMessage,
          string.IsNullOrEmpty(rowItemID) ? null : rowItemID,
          string.IsNullOrEmpty(columnItemID) ? null : columnItemID,
          string.IsNullOrEmpty(businessObjectUniqueIdentifier) ? null : businessObjectUniqueIdentifier,
          string.IsNullOrEmpty(propertyIdentifier) ? null : propertyIdentifier);
    }

    /// <summary>
    /// Gets the errors message associated with the validation error.
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    /// Gets the ItemID of the row in which the validation error occured,
    /// or <see langword="null"/> if the validation error is not assigned to a row.
    /// </summary>
    public string? RowItemID { get; }

    /// <summary>
    /// Gets the ItemID of the column in which the validation error occured,
    /// or <see langword="null"/> if the validation error is not assigned to a column.
    /// </summary>
    public string? ColumnItemID { get; }

    /// <summary>
    /// Gets the unique identifier of the validated domain object,
    /// or <see langword="null"/> if the validation error has no associated business object.
    /// </summary>
    public string? ValidatedDomainObject { get; }

    /// <summary>
    /// Gets the identifier of the validated domain property,
    /// or <see langword="null"/> if the validation error has no associated property.
    /// </summary>
    public string? ValidatedDomainProperty { get; }

    public BocListValidationError (string errorMessage, string? rowItemID, string? columnItemID, string? validatedDomainObject, string? validatedDomainProperty)
    {
      ArgumentUtility.CheckNotNull(nameof(errorMessage), errorMessage);

      ErrorMessage = errorMessage;
      RowItemID = rowItemID;
      ColumnItemID = columnItemID;
      ValidatedDomainObject = validatedDomainObject;
      ValidatedDomainProperty = validatedDomainProperty;
    }

    /// <summary>
    /// Returns whether the validation error has an associated <see cref="RowItemID"/>.
    /// </summary>
    public bool HasRowItemID => RowItemID != null;

    /// <summary>
    /// Returns whether the validation error has an associated <see cref="ColumnItemID"/>.
    /// </summary>
    public bool HasColumnItemID => ColumnItemID != null;

    protected bool Equals (BocListValidationError other)
    {
      return ErrorMessage == other.ErrorMessage
             && RowItemID == other.RowItemID
             && ColumnItemID == other.ColumnItemID
             && ValidatedDomainObject == other.ValidatedDomainObject
             && ValidatedDomainProperty == other.ValidatedDomainProperty;
    }

    public override bool Equals (object? obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != GetType())
        return false;
      return Equals((BocListValidationError)obj);
    }

    public override int GetHashCode ()
    {
      unchecked
      {
        var hashCode = ErrorMessage.GetHashCode();
        hashCode = (hashCode * 397) ^ (RowItemID != null ? RowItemID.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (ColumnItemID != null ? ColumnItemID.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (ValidatedDomainObject != null ? ValidatedDomainObject.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (ValidatedDomainProperty != null ? ValidatedDomainProperty.GetHashCode() : 0);
        return hashCode;
      }
    }

    public static bool operator == (BocListValidationError? left, BocListValidationError? right) => Equals(left, right);

    public static bool operator != (BocListValidationError? left, BocListValidationError? right) => !Equals(left, right);

    public override string ToString ()
    {
      return $"{nameof(ErrorMessage)}: {ErrorMessage}, {nameof(RowItemID)}: {RowItemID}, {nameof(ColumnItemID)}: {ColumnItemID},"
             + $" {nameof(ValidatedDomainObject)}: {ValidatedDomainObject}, {nameof(ValidatedDomainProperty)}: {ValidatedDomainProperty}";
    }
  }
}
