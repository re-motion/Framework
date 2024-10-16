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
using log4net.Core;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Logging.Log4Net.UnitTests.Log4NetLoggerTests
{
  [TestFixture]
  public class ConvertTest
  {
    private static readonly object[] s_loglevels =
    [
        new object[] { LogLevel.Trace, Level.Trace },
        new object[] { LogLevel.Debug, Level.Debug },
        new object[] { LogLevel.Information, Level.Info },
        new object[] { LogLevel.Warning, Level.Warn },
        new object[] { LogLevel.Error, Level.Error },
        new object[] { LogLevel.Critical, Level.Critical },
        new object[] { LogLevel.None, Level.Off }
    ];

    [Test]
    [TestCaseSource(nameof(s_loglevels))]
    public void Convert_WithValidLogLevel_ReturnsConvertedLogLevel (LogLevel logLevel, Level level)
    {
      var result = Log4NetLogger.Convert(logLevel);
      Assert.That(result, Is.EqualTo(level));
    }

    [Test]
    public void Test_InvalidLevel ()
    {
      Assert.That(
          () => Log4NetLogger.Convert((LogLevel)10),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "LogLevel does not support value 10.", "logLevel"));
    }
  }
}
