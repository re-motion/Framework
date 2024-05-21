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
using log4net.Core;
using Moq;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Logging;

namespace Remotion.UnitTests.Logging
{
  [TestFixture]
  public class Log4NetLogManagerTest
  {
    private TextWriter _originalConsoleOut;
    private StringWriter _fakeConsoleOut;
    private Log4NetLogManager _logManager;

    [SetUp]
    public void SetUp ()
    {
      _originalConsoleOut = Console.Out;
      _fakeConsoleOut = new StringWriter();
      Console.SetOut(_fakeConsoleOut);

      _logManager = new Log4NetLogManager();
    }

    [TearDown]
    public void TearDown ()
    {
      log4net.LogManager.ResetConfiguration();
      Console.SetOut(_originalConsoleOut);
    }

    [Test]
    public void GetLogger_WithNameAsString ()
    {
      var log = _logManager.GetLogger("The Name");

      Assert.That(log, Is.InstanceOf(typeof(Log4NetLog)));
      var log4NetLog = (Log4NetLog)log;
      Assert.That(log4NetLog.Logger.Name, Is.EqualTo("The Name"));
    }

    [Test]
    public void GetLogger_WithNameAsString_ReturnsSameLoggerTwice ()
    {
      var log = _logManager.GetLogger("The Name");

      Assert.That(_logManager.GetLogger("The Name"), Is.SameAs(log));
    }

    [Test]
    public void GetLogger_WithNameFromType ()
    {
      var log = _logManager.GetLogger(typeof(SampleType));

      Assert.That(log, Is.InstanceOf(typeof(Log4NetLog)));

      var log4NetLog = (Log4NetLog)log;
      Assert.That(log4NetLog.Logger.Name, Is.EqualTo("Remotion.UnitTests.Logging.SampleType"));
    }

    [Test]
    public void GetLogger_WithNameFromType_ReturnsSameLoggerTwice ()
    {
      var log = _logManager.GetLogger(typeof(SampleType));

      Assert.That(_logManager.GetLogger(typeof(SampleType)), Is.SameAs(log));
    }

    [Test]
    public void InitializeConsole_CausesLogsToBeWrittenToConsole ()
    {
      var log = _logManager.GetLogger(typeof(SampleType));

      log.Log(LogLevel.Debug, "Test before InitializeConsole");
      CheckConsoleOutput();

      _logManager.InitializeConsole();

      log.Log(LogLevel.Debug, "Test after InitializeConsole");
      CheckConsoleOutput("DEBUG: Test after InitializeConsole");
    }

    [Test]
    public void InitializeConsole_LogsAllLogLevels ()
    {
      _logManager.InitializeConsole();

      var log = _logManager.GetLogger(typeof(SampleType));
      log.Log(LogLevel.Debug, "Test Debug");
      log.Log(LogLevel.Info, "Test Info");
      log.Log(LogLevel.Warn, "Test Warn");
      log.Log(LogLevel.Error, "Test Error");
      log.Log(LogLevel.Fatal, "Test Fatal");

      CheckConsoleOutput(
          "DEBUG: Test Debug",
          "INFO : Test Info",
          "WARN : Test Warn",
          "ERROR: Test Error",
          "FATAL: Test Fatal");
    }

    [Test]
    public void InitializeConsole_WithThreshold_SetsDefaultThreshold ()
    {
      _logManager.InitializeConsole(LogLevel.Warn);

      var log = _logManager.GetLogger(typeof(SampleType));
      log.Log(LogLevel.Debug, "Test Debug");
      log.Log(LogLevel.Info, "Test Info");
      log.Log(LogLevel.Warn, "Test Warn");
      log.Log(LogLevel.Error, "Test Error");
      log.Log(LogLevel.Fatal, "Test Fatal");

      CheckConsoleOutput(
          "WARN : Test Warn",
          "ERROR: Test Error",
          "FATAL: Test Fatal");
    }

    [Test]
    public void InitializeConsole_WithSpecificThresholds_SetsDefaultThreshold_AndSpecificThresholds ()
    {
      var log1 = _logManager.GetLogger(typeof(SampleType));
      var log2 = _logManager.GetLogger("other");

      _logManager.InitializeConsole(LogLevel.Warn, new LogThreshold(log2, LogLevel.Info));

      log1.Log(LogLevel.Debug, "1: Test Debug");
      log1.Log(LogLevel.Info, "1: Test Info");
      log1.Log(LogLevel.Warn, "1: Test Warn");
      log1.Log(LogLevel.Error, "1: Test Error");
      log1.Log(LogLevel.Fatal, "1: Test Fatal");

      log2.Log(LogLevel.Debug, "2: Test Debug");
      log2.Log(LogLevel.Info, "2: Test Info");
      log2.Log(LogLevel.Warn, "2: Test Warn");
      log2.Log(LogLevel.Error, "2: Test Error");
      log2.Log(LogLevel.Fatal, "2: Test Fatal");

      CheckConsoleOutput(
          "WARN : 1: Test Warn",
          "ERROR: 1: Test Error",
          "FATAL: 1: Test Fatal",
          "INFO : 2: Test Info",
          "WARN : 2: Test Warn",
          "ERROR: 2: Test Error",
          "FATAL: 2: Test Fatal");
    }

    [Test]
    public void InitializeConsole_WithSpecificThresholds_InvalidLoggerType ()
    {
      var logger = new Mock<ILog>();
      Assert.That(
          () => _logManager.InitializeConsole(LogLevel.Debug, new LogThreshold(logger.Object, LogLevel.Error)),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "This LogManager only supports ILog implementations that also implement the log4net.Core.ILoggerWrapper interface.", "logThresholds"));
    }

    [Test]
    public void InitializeConsole_WithSpecificThresholds_InvalidLog4NetLoggerType ()
    {
      var logger = new Mock<ILogger>();
      logger.Setup(stub => stub.Repository).Returns(LoggerManager.GetRepository(GetType().Assembly));
      logger.Setup(stub => stub.Name).Returns("Foo");
      Assert.That(
          () => _logManager.InitializeConsole(LogLevel.Debug, new LogThreshold(new Log4NetLog(logger.Object), LogLevel.Error)),
          Throws.ArgumentException.With.Message.Matches(
              @"Log-specific thresholds can only be set for log4net loggers of type 'log4net\.Repository\.Hierarchy\.Logger'\. "
              + @"The specified logger 'Foo' is of type 'Castle\.Proxies\.ILoggerProxy.*'\."));

    }

    private void CheckConsoleOutput (params string[] expectedLines)
    {
      var fullString = string.Concat(expectedLines.Select(line => line + Environment.NewLine).ToArray());
      Assert.That(_fakeConsoleOut.ToString(), Is.EqualTo(fullString));
    }

  }
}
