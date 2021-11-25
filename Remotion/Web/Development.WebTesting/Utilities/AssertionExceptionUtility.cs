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
using System.Runtime.CompilerServices;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Provides method for common exceptions
  /// </summary>
  public static class AssertionExceptionUtility
  {
    [NotNull]
    [MustUseReturnValue]
    public static WebTestException CreateControlDisabledException ([NotNull] IDriver driver, [CallerMemberName] string operationName = "")
    {
      ArgumentUtility.CheckNotNull("driver", driver);
      ArgumentUtility.CheckNotNullOrEmpty("operationName", operationName);

      return CreateException(driver, string.Format("The control is currently in a disabled state. Therefore, the '{0}' operation is not possible.", operationName));
    }

    [NotNull]
    [MustUseReturnValue]
    public static WebTestException CreateCommandDisabledException ([NotNull] IDriver driver, [CallerMemberName] string operationName = "")
    {
      ArgumentUtility.CheckNotNull("driver", driver);
      ArgumentUtility.CheckNotNullOrEmpty("operationName", operationName);

      return CreateException(driver, string.Format("The command is currently in a disabled state. Therefore, the '{0}' operation is not possible.", operationName));
    }

    [NotNull]
    [MustUseReturnValue]
    public static WebTestException CreateControlReadOnlyException ([NotNull] IDriver driver)
    {
      ArgumentUtility.CheckNotNull("driver", driver);

      return CreateException(driver, "The control is currently in a read-only state. Therefore, the operation is not possible.");
    }

    [NotNull]
    [MustUseReturnValue]
    public static WebTestException CreateControlNotReadOnlyException ([NotNull] IDriver driver)
    {
      ArgumentUtility.CheckNotNull("driver", driver);

      return CreateException(driver, "The control is currently not in a read-only state. Therefore, the operation is not possible.");
    }

    [NotNull]
    [MustUseReturnValue]
    public static WebTestException CreateControlMissingException ([NotNull] IDriver driver, [NotNull] string exceptionDetails)
    {
      ArgumentUtility.CheckNotNull("driver", driver);
      ArgumentUtility.CheckNotNullOrEmpty("exceptionDetails", exceptionDetails);

      return CreateException(driver, $"The element cannot be found: {exceptionDetails}");
    }

    [NotNull]
    [MustUseReturnValue]
    public static WebTestException CreateControlAmbiguousException ([NotNull] IDriver driver, [NotNull] string exceptionDetails)
    {
      ArgumentUtility.CheckNotNull("driver", driver);
      ArgumentUtility.CheckNotNullOrEmpty("exceptionDetails", exceptionDetails);

      return CreateException(driver, $"Multiple elements were found: {exceptionDetails}");
    }

    [NotNull]
    [MustUseReturnValue]
    [StringFormatMethod ("message")]
    public static WebTestException CreateExpectationException ([NotNull] IDriver driver, [NotNull] string message, params object[] args)
    {
      ArgumentUtility.CheckNotNull("driver", driver);
      ArgumentUtility.CheckNotNullOrEmpty("message", message);

      return CreateException(driver, string.Format(message, args));
    }

    private static WebTestException CreateException ([NotNull] IDriver driver, string message)
    {
      ArgumentUtility.CheckNotNull("driver", driver);
      return new WebTestException(
          $"{message}\r\n(Browser: {driver.GetBrowserName()}, version {driver.GetBrowserVersion()})\r\n(Webdriver version: {driver.GetWebDriverVersion()})");
    }
  }
}