// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
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
