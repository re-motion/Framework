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
using NUnit.Framework.Constraints;
using Remotion.Utilities;

namespace Remotion.Development.NUnit.UnitTesting
{
  public static class NUnitExtensions
  {
    public static EqualConstraint ArgumentExceptionMessageEqualTo (this ConstraintExpression constraintExpression, string message, string paramName)
    {
      AssertThatMessageContainsWhitespaces(message: message);
      AssertThatParameterDoesNotContainWhitespaces(paramName: paramName);
      return constraintExpression.With.Message.EqualTo(new ArgumentException(message: message, paramName: paramName).Message);
    }

    public static EqualConstraint ArgumentOutOfRangeExceptionMessageEqualTo (this ConstraintExpression constraintExpression, string message, string paramName, int actualValue)
    {
      AssertThatMessageContainsWhitespaces(message: message);
      AssertThatParameterDoesNotContainWhitespaces(paramName: paramName);
      return constraintExpression.With.Message.EqualTo(new ArgumentOutOfRangeException(message: message, paramName: paramName, actualValue: actualValue).Message);
    }

    public static EndsWithConstraint ArgumentExceptionMessageWithParameterNameEqualTo (this ConstraintExpression constraintExpression, string paramName)
    {
      AssertThatParameterDoesNotContainWhitespaces(paramName: paramName);
      return constraintExpression.With.Message.EndsWith(new ArgumentException(message: string.Empty, paramName: paramName).Message);
    }

    private static void AssertThatMessageContainsWhitespaces (string message)
    {
      Assertion.IsTrue(message.Contains(" "), "The exception message must contain at least one whitespace.\r\nmessage: {0}", message);
    }

    private static void AssertThatParameterDoesNotContainWhitespaces (string paramName)
    {
      Assertion.IsFalse(paramName.Contains(" "), "The parameter must not contain any whitespaces.\r\nparamName: {0}", paramName);
    }
  }
}
