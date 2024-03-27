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
using System.IO;
using System.Linq;
using System.Reflection;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using NUnit.Framework;
using Remotion.Logging.Log4Net;
using Remotion.Reflection;
using MicrosoftLogging = Microsoft.Extensions.Logging;

namespace Remotion.Extensions.UnitTests.Logging.Log4NetTests;

public class Log4NetLoggerTest
{
  private TextWriter _originalConsoleOut;
  private StringWriter _fakeConsoleOut;

  [SetUp]
  public void SetUp ()
  {
    _originalConsoleOut = Console.Out;
    _fakeConsoleOut = new StringWriter();
    Console.SetOut(_fakeConsoleOut);
  }

  [TearDown]
  public void TearDown ()
  {
    log4net.LogManager.ResetConfiguration();
    Console.SetOut(_originalConsoleOut);
  }

  [Test]
  public void Convert_WithValidLogLevel_ReturnsConvertedLogLevel ()
  {
    var result1 = Log4NetLogger.Convert(MicrosoftLogging.LogLevel.Debug);
    Assert.That(result1, Is.EqualTo(Level.Debug));

    var resul2 = Log4NetLogger.Convert(MicrosoftLogging.LogLevel.Information);
    Assert.That(resul2, Is.EqualTo(Level.Info));

    var result3 = Log4NetLogger.Convert(MicrosoftLogging.LogLevel.Warning);
    Assert.That(result3, Is.EqualTo(Level.Warn));

    var result4 = Log4NetLogger.Convert(MicrosoftLogging.LogLevel.Error);
    Assert.That(result4, Is.EqualTo(Level.Error));

    var result5 = Log4NetLogger.Convert(MicrosoftLogging.LogLevel.Critical);
    Assert.That(result5, Is.EqualTo(Level.Critical));

    var result6 = Log4NetLogger.Convert(MicrosoftLogging.LogLevel.None);
    Assert.That(result6, Is.EqualTo(Level.Off));
  }

  [Test]
  [TestCase("TRACE", MicrosoftLogging.LogLevel.Trace)]
  [TestCase("DEBUG", MicrosoftLogging.LogLevel.Debug)]
  [TestCase("INFO", MicrosoftLogging.LogLevel.Information)]
  [TestCase("WARN", MicrosoftLogging.LogLevel.Warning)]
  [TestCase("ERROR", MicrosoftLogging.LogLevel.Error)]
  [TestCase("CRITICAL", MicrosoftLogging.LogLevel.Critical)]
  [TestCase("OFF", MicrosoftLogging.LogLevel.None)]
  public void Log_WritesCorrectlyFormattedMessage (string level, MicrosoftLogging.LogLevel logLevel)
  {
    var logger = new Log4NetLogger("categoryName");
    InitializeConsole(MicrosoftLogging.LogLevel.Trace);

    logger.Log(logLevel, new MicrosoftLogging.EventId(1, "event"), "testState", new Exception("TestMessage"), (s, ex) => $"{s} - {ex.Message}");

    CheckConsoleOutput($"{level}: testState - TestMessage\r\nSystem.Exception: TestMessage");
  }

  [Test]
  public void IsEnabled_WithLowerLogLevel_ReturnsFalse ()
  {

    var logger = new Log4NetLogger("categoryNameTest");
    InitializeConsole(MicrosoftLogging.LogLevel.Error);
    var result = logger.IsEnabled(MicrosoftLogging.LogLevel.Information);

    Assert.That(result, Is.False);
  }

  [Test]
  public void IsEnabled_WithHigherLogLevel_ReturnsTrue ()
  {

    var logger = new Log4NetLogger("categoryNameTest");
    InitializeConsole(MicrosoftLogging.LogLevel.Information);
    var result = logger.IsEnabled(MicrosoftLogging.LogLevel.Warning);

    Assert.That(result, Is.True);
  }

  [Test]
  public void IsEnabled_WithSameLogLevel_ReturnsTrue ()
  {

    var logger = new Log4NetLogger("categoryNameTest");
    InitializeConsole(MicrosoftLogging.LogLevel.Information);
    var result = logger.IsEnabled(MicrosoftLogging.LogLevel.Information);

    Assert.That(result, Is.True);
  }

  [Test]
  public void BeginScope_ReturnsNull ()
  {
    var logger = new Log4NetLogger("categoryNameTest");
    var result = logger.BeginScope<string>("Test");

    Assert.That(result, Is.Null);
  }

  private void InitializeConsole (MicrosoftLogging.LogLevel defaultThreshold)
  {
    var appender = CreateConsoleAppender();

    var repositoryAssembly = Assembly.GetCallingAssembly();
    var loggerRepository = log4net.LogManager.GetRepository(repositoryAssembly);
    var hierarchy = loggerRepository as Hierarchy;
    if (hierarchy == null)
    {
      var message = string.Format(
          "Cannot set a default threshold for the logger repository of type '{1}' configured for assembly '{0}'. The repository does not derive "
          + "from the '{2}' class.",
          repositoryAssembly.GetName().GetNameSafe(),
          loggerRepository.GetType(),
          typeof(Hierarchy));
      throw new InvalidOperationException(message);
    }

    hierarchy.Root.Level = Log4NetLogger.Convert(defaultThreshold);
    hierarchy.Root.AddAppender(appender);
    hierarchy.Configured = true;
  }

  private ConsoleAppender CreateConsoleAppender ()
  {
    var appender = new ConsoleAppender();
    appender.Layout = new PatternLayout("%level: %message%newline");
    return appender;
  }

  private void CheckConsoleOutput (params string[] expectedLines)
  {
    var fullString = string.Concat(expectedLines.Select(line => line + Environment.NewLine).ToArray());
    Assert.That(_fakeConsoleOut.ToString(), Is.EqualTo(fullString));
  }
}
