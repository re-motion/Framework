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

namespace Remotion.ObjectBinding.Validation
{
  /// <remarks>
  /// TBD if this should be a struct for memory optimization purposes or if we keep it as class for pointer-copy and non-null safety of ErrorMessage.
  /// </remarks>
  public class BusinessObjectValidationFailure
  {
    public static BusinessObjectValidationFailure CreateForBusinessObject (
        [NotNull] IBusinessObject validatedObject,
        [NotNull] string errorMessage)
    {
      ArgumentUtility.CheckNotNull ("validatedObject", validatedObject);
      ArgumentUtility.CheckNotNullOrEmpty ("errorMessage", errorMessage);

      return new BusinessObjectValidationFailure (errorMessage, validatedObject, null);
    }

    public static BusinessObjectValidationFailure CreateForBusinessObjectProperty (
        [NotNull] string errorMessage,
        [NotNull] IBusinessObject validatedObject,
        [NotNull] IBusinessObjectProperty validatedProperty)
    {
      ArgumentUtility.CheckNotNull ("validatedObject", validatedObject);
      ArgumentUtility.CheckNotNull ("validatedProperty", validatedProperty);
      ArgumentUtility.CheckNotNullOrEmpty ("errorMessage", errorMessage);

      return new BusinessObjectValidationFailure (errorMessage, validatedObject, validatedProperty);
    }

    public static BusinessObjectValidationFailure Create ([NotNull] string errorMessage)
    {
      return new BusinessObjectValidationFailure (errorMessage, null, null);
    }

    [NotNull]
    public string ErrorMessage { get; }

    [CanBeNull]
    public IBusinessObject? ValidatedObject { get; }

    [CanBeNull]
    public IBusinessObjectProperty? ValidatedProperty { get; }

    private BusinessObjectValidationFailure (
        [NotNull] string errorMessage,
        [CanBeNull] IBusinessObject? validatedObject,
        [CanBeNull] IBusinessObjectProperty? validatedProperty)
    {
      // Additional properties are possible, e.g. indicating if the message is needed or obvious (required field)

      ErrorMessage = errorMessage;
      ValidatedObject = validatedObject;
      ValidatedProperty = validatedProperty;
    }
  }
}