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

namespace Remotion.Utilities
{
  public static partial class ArgumentUtility
  {
    /// <summary>Checks whether <paramref name="enumValue"/> is defined within its enumeration type.</summary>
    /// <exception cref="ArgumentNullException"> If <paramref name="enumValue"/> is a null reference. </exception>
    /// <exception cref="ArgumentOutOfRangeException"> If <paramref name="enumValue"/> has a numeric value that is not completely defined within its 
    /// enumeration type. For flag types, every bit must correspond to at least one enumeration value. </exception>
    public static Enum CheckValidEnumValue (
        [InvokerParameterName] string argumentName, 
        [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] Enum enumValue)
    {
      if (enumValue == null)
        throw new ArgumentNullException (argumentName);

      if (! EnumUtility.IsValidEnumValue (enumValue))
        throw CreateEnumArgumentOutOfRangeException (argumentName, enumValue);

      return enumValue;
    }

    /// <summary>Checks whether <paramref name="enumValue"/> is of the enumeration type <typeparamref name="TEnum"/> and defined within this type.</summary>
    /// <remarks>
    /// When successful, the value is returned as a <c>Nullable</c> of the specified type for direct assignment. 
    /// </remarks>
    /// <exception cref="ArgumentException"> If <paramref name="enumValue"/> is not of the specified type. </exception>
    /// <exception cref="ArgumentOutOfRangeException"> If <paramref name="enumValue"/> has a numeric value that is not completely defined within its 
    /// enumeration type. For flag types, every bit must correspond to at least one enumeration value. </exception>
    public static TEnum? CheckValidEnumValueAndType<TEnum> ([InvokerParameterName] string argumentName, object enumValue)
        where TEnum: struct
    {
      if (enumValue == null)
        return default (TEnum?);

      if (! (enumValue is TEnum))
        throw CreateArgumentTypeException (argumentName, enumValue.GetType(), typeof (TEnum));

      if (! EnumUtility.IsValidEnumValue (enumValue))
        throw CreateEnumArgumentOutOfRangeException (argumentName, enumValue);

      return (TEnum?) enumValue;
    }

    /// <summary>Checks whether <paramref name="enumValue"/> is of the enumeration type <typeparamref name="TEnum"/>, is defined within this 
    /// type, and is not a null reference.</summary>
    /// <remarks>
    /// When successful, the value is returned as the specified type for direct assignment. 
    /// </remarks>
    /// <exception cref="ArgumentNullException"> If <paramref name="enumValue"/> is a null reference. </exception>
    /// <exception cref="ArgumentException"> If <paramref name="enumValue"/> is not of the specified type. </exception>
    /// <exception cref="ArgumentOutOfRangeException"> If <paramref name="enumValue"/> has a numeric value that is not completely defined within its 
    /// enumeration type. For flag types, every bit must correspond to at least one enumeration value. </exception>
    public static TEnum CheckValidEnumValueAndTypeAndNotNull<TEnum> (
        [InvokerParameterName] string argumentName, 
        [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] object enumValue)
        where TEnum: struct
    {
      if (enumValue == null)
        throw new ArgumentNullException (argumentName);

      if (! (enumValue is TEnum))
        throw CreateArgumentTypeException (argumentName, enumValue.GetType(), typeof (TEnum));

      if (!EnumUtility.IsValidEnumValue (enumValue))
        throw CreateEnumArgumentOutOfRangeException (argumentName, enumValue);

      return (TEnum) enumValue;
    }
 
    public static ArgumentOutOfRangeException CreateEnumArgumentOutOfRangeException ([InvokerParameterName] string argumentName, object actualValue)
    {
      string message = string.Format (
          "The value of parameter '{0}' is not a valid value of the type '{1}'. Actual value was '{2}'.",
          argumentName,
          actualValue.GetType(),
          actualValue);
      return new ArgumentOutOfRangeException (argumentName, actualValue, message);
    }
  }
}