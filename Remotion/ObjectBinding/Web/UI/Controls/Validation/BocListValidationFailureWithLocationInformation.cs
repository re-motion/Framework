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
using Remotion.ObjectBinding.Validation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation
{
  /// <summary>
  /// Represents a validation error in the <see cref="BocList"/> and its relevant location information (row, column).
  /// </summary>
  public class BocListValidationFailureWithLocationInformation
  {
    public static BocListValidationFailureWithLocationInformation CreateFailure (BusinessObjectValidationFailure failure)
    {
      ArgumentUtility.CheckNotNull(nameof(failure), failure);

      return new BocListValidationFailureWithLocationInformation(failure);
    }

    public static BocListValidationFailureWithLocationInformation CreateFailureForRow (BusinessObjectValidationFailure failure, IBusinessObject rowObject)
    {
      ArgumentUtility.CheckNotNull(nameof(failure), failure);
      ArgumentUtility.CheckNotNull(nameof(rowObject), rowObject);

      return new BocListValidationFailureWithLocationInformation(failure, rowObject);
    }

    public static BocListValidationFailureWithLocationInformation CreateFailureForCell (BusinessObjectValidationFailure failure, IBusinessObject rowObject, BocColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull(nameof(failure), failure);
      ArgumentUtility.CheckNotNull(nameof(rowObject), rowObject);
      ArgumentUtility.CheckNotNull(nameof(columnDefinition), columnDefinition);

      return new BocListValidationFailureWithLocationInformation(failure, rowObject, columnDefinition);
    }

    public BusinessObjectValidationFailure Failure { get; }

    public IBusinessObject? RowObject { get; }

    public BocColumnDefinition? ColumnDefinition { get; }

    private BocListValidationFailureWithLocationInformation (BusinessObjectValidationFailure failure, IBusinessObject? rowObject = null, BocColumnDefinition? columnDefinition = null)
    {
      Failure = failure;
      RowObject = rowObject;
      ColumnDefinition = columnDefinition;
    }

    private bool Equals (BocListValidationFailureWithLocationInformation other)
    {
      return Failure.Equals(other.Failure) && Equals(RowObject, other.RowObject) && Equals(ColumnDefinition, other.ColumnDefinition);
    }

    public override bool Equals (object? obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != GetType())
        return false;
      return Equals((BocListValidationFailureWithLocationInformation)obj);
    }

    public override int GetHashCode ()
    {
      unchecked
      {
        var hashCode = Failure.GetHashCode();
        hashCode = (hashCode * 397) ^ (RowObject != null ? RowObject.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (ColumnDefinition != null ? ColumnDefinition.GetHashCode() : 0);
        return hashCode;
      }
    }
  }
}
