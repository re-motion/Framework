// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using NUnit.Framework;
using Remotion.Logging;

namespace Remotion.Extensions.UnitTests.Logging.LoggerExtensionsTests;

[TestFixture]
public class StringFormatTest
{
  [Test]
  public void LogFormat ()
  {
    var fakeLogger = new FakeLogger();
    fakeLogger.ControlLevel(LogLevel.Trace, true);

    fakeLogger.LogFormat(LogLevel.Critical, "message: {2}, {1}", "p0", "p1", "p2");

    var logs = fakeLogger.Collector.GetSnapshot();
    Assert.That(logs.Count, Is.EqualTo(1));
    Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Critical));
    Assert.That(logs[0].Message, Is.EqualTo("message: p2, p1"));
    Assert.That(logs[0].StructuredState, Is.EqualTo(new[] { new KeyValuePair<string, string>("{OriginalFormat}", "message: p2, p1") }));
  }

  [Test]
  public void LogFormat_WithException ()
  {
    var fakeLogger = new FakeLogger();
    fakeLogger.ControlLevel(LogLevel.Trace, true);

    var exception = new ApplicationException("Test exception");

    fakeLogger.LogFormat(LogLevel.Error, exception, "message: {2}, {0}", "p0", "p1", "p2");

    var logs = fakeLogger.Collector.GetSnapshot();
    Assert.That(logs.Count, Is.EqualTo(1));
    Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Error));
    Assert.That(logs[0].Message, Is.EqualTo("message: p2, p0"));
    Assert.That(logs[0].StructuredState, Is.EqualTo(new[] { new KeyValuePair<string, string>("{OriginalFormat}", "message: p2, p0") }));
    Assert.That(logs[0].Exception, Is.SameAs(exception));
  }

  [Test]
  public void LogTraceFormat ()
  {
    var fakeLogger = new FakeLogger();
    fakeLogger.ControlLevel(LogLevel.Trace, true);

    fakeLogger.LogTraceFormat("message: {2}, {1}", "p0", "p1", "p2");

    var logs = fakeLogger.Collector.GetSnapshot();
    Assert.That(logs.Count, Is.EqualTo(1));
    Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Trace));
    Assert.That(logs[0].Message, Is.EqualTo("message: p2, p1"));
    Assert.That(logs[0].StructuredState, Is.EqualTo(new[] { new KeyValuePair<string, string>("{OriginalFormat}", "message: p2, p1") }));
  }

  [Test]
  public void LogTraceFormat_WithException ()
  {
    var fakeLogger = new FakeLogger();
    fakeLogger.ControlLevel(LogLevel.Trace, true);

    var exception = new ApplicationException("Test exception");

    fakeLogger.LogTraceFormat(exception, "message: {2}, {0}", "p0", "p1", "p2");

    var logs = fakeLogger.Collector.GetSnapshot();
    Assert.That(logs.Count, Is.EqualTo(1));
    Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Trace));
    Assert.That(logs[0].Message, Is.EqualTo("message: p2, p0"));
    Assert.That(logs[0].StructuredState, Is.EqualTo(new[] { new KeyValuePair<string, string>("{OriginalFormat}", "message: p2, p0") }));
    Assert.That(logs[0].Exception, Is.SameAs(exception));
  }

  [Test]
  public void LogDebugFormat ()
  {
    var fakeLogger = new FakeLogger();
    fakeLogger.ControlLevel(LogLevel.Debug, true);

    fakeLogger.LogDebugFormat("message: {2}, {1}", "p0", "p1", "p2");

    var logs = fakeLogger.Collector.GetSnapshot();
    Assert.That(logs.Count, Is.EqualTo(1));
    Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Debug));
    Assert.That(logs[0].Message, Is.EqualTo("message: p2, p1"));
    Assert.That(logs[0].StructuredState, Is.EqualTo(new[] { new KeyValuePair<string, string>("{OriginalFormat}", "message: p2, p1") }));
  }

  [Test]
  public void LogDebugFormat_WithException ()
  {
    var fakeLogger = new FakeLogger();
    fakeLogger.ControlLevel(LogLevel.Debug, true);

    var exception = new ApplicationException("Test exception");

    fakeLogger.LogDebugFormat(exception, "message: {2}, {0}", "p0", "p1", "p2");

    var logs = fakeLogger.Collector.GetSnapshot();
    Assert.That(logs.Count, Is.EqualTo(1));
    Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Debug));
    Assert.That(logs[0].Message, Is.EqualTo("message: p2, p0"));
    Assert.That(logs[0].StructuredState, Is.EqualTo(new[] { new KeyValuePair<string, string>("{OriginalFormat}", "message: p2, p0") }));
    Assert.That(logs[0].Exception, Is.SameAs(exception));
  }

  [Test]
  public void LogInformationFormat ()
  {
    var fakeLogger = new FakeLogger();
    fakeLogger.ControlLevel(LogLevel.Information, true);

    fakeLogger.LogInformationFormat("message: {2}, {1}", "p0", "p1", "p2");

    var logs = fakeLogger.Collector.GetSnapshot();
    Assert.That(logs.Count, Is.EqualTo(1));
    Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Information));
    Assert.That(logs[0].Message, Is.EqualTo("message: p2, p1"));
    Assert.That(logs[0].StructuredState, Is.EqualTo(new[] { new KeyValuePair<string, string>("{OriginalFormat}", "message: p2, p1") }));
  }

  [Test]
  public void LogInformationFormat_WithException ()
  {
    var fakeLogger = new FakeLogger();
    fakeLogger.ControlLevel(LogLevel.Information, true);

    var exception = new ApplicationException("Test exception");

    fakeLogger.LogInformationFormat(exception, "message: {2}, {0}", "p0", "p1", "p2");

    var logs = fakeLogger.Collector.GetSnapshot();
    Assert.That(logs.Count, Is.EqualTo(1));
    Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Information));
    Assert.That(logs[0].Message, Is.EqualTo("message: p2, p0"));
    Assert.That(logs[0].StructuredState, Is.EqualTo(new[] { new KeyValuePair<string, string>("{OriginalFormat}", "message: p2, p0") }));
    Assert.That(logs[0].Exception, Is.SameAs(exception));
  }

  [Test]
  public void LogWarningFormat ()
  {
    var fakeLogger = new FakeLogger();
    fakeLogger.ControlLevel(LogLevel.Warning, true);

    fakeLogger.LogWarningFormat("message: {2}, {1}", "p0", "p1", "p2");

    var logs = fakeLogger.Collector.GetSnapshot();
    Assert.That(logs.Count, Is.EqualTo(1));
    Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Warning));
    Assert.That(logs[0].Message, Is.EqualTo("message: p2, p1"));
    Assert.That(logs[0].StructuredState, Is.EqualTo(new[] { new KeyValuePair<string, string>("{OriginalFormat}", "message: p2, p1") }));
  }

  [Test]
  public void LogWarningFormat_WithException ()
  {
    var fakeLogger = new FakeLogger();
    fakeLogger.ControlLevel(LogLevel.Warning, true);

    var exception = new ApplicationException("Test exception");

    fakeLogger.LogWarningFormat(exception, "message: {2}, {0}", "p0", "p1", "p2");

    var logs = fakeLogger.Collector.GetSnapshot();
    Assert.That(logs.Count, Is.EqualTo(1));
    Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Warning));
    Assert.That(logs[0].Message, Is.EqualTo("message: p2, p0"));
    Assert.That(logs[0].StructuredState, Is.EqualTo(new[] { new KeyValuePair<string, string>("{OriginalFormat}", "message: p2, p0") }));
    Assert.That(logs[0].Exception, Is.SameAs(exception));
  }

  [Test]
  public void LogErrorFormat ()
  {
    var fakeLogger = new FakeLogger();
    fakeLogger.ControlLevel(LogLevel.Error, true);

    fakeLogger.LogErrorFormat("message: {2}, {1}", "p0", "p1", "p2");

    var logs = fakeLogger.Collector.GetSnapshot();
    Assert.That(logs.Count, Is.EqualTo(1));
    Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Error));
    Assert.That(logs[0].Message, Is.EqualTo("message: p2, p1"));
    Assert.That(logs[0].StructuredState, Is.EqualTo(new[] { new KeyValuePair<string, string>("{OriginalFormat}", "message: p2, p1") }));
  }

  [Test]
  public void LogErrorFormat_WithException ()
  {
    var fakeLogger = new FakeLogger();
    fakeLogger.ControlLevel(LogLevel.Error, true);

    var exception = new ApplicationException("Test exception");

    fakeLogger.LogErrorFormat(exception, "message: {2}, {0}", "p0", "p1", "p2");

    var logs = fakeLogger.Collector.GetSnapshot();
    Assert.That(logs.Count, Is.EqualTo(1));
    Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Error));
    Assert.That(logs[0].Message, Is.EqualTo("message: p2, p0"));
    Assert.That(logs[0].StructuredState, Is.EqualTo(new[] { new KeyValuePair<string, string>("{OriginalFormat}", "message: p2, p0") }));
    Assert.That(logs[0].Exception, Is.SameAs(exception));
  }

  [Test]
  public void LogCriticalFormat ()
  {
    var fakeLogger = new FakeLogger();
    fakeLogger.ControlLevel(LogLevel.Critical, true);

    fakeLogger.LogCriticalFormat("message: {2}, {0}", "p0", "p1", "p2");

    var logs = fakeLogger.Collector.GetSnapshot();
    Assert.That(logs.Count, Is.EqualTo(1));
    Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Critical));
    Assert.That(logs[0].Message, Is.EqualTo("message: p2, p0"));
    Assert.That(logs[0].StructuredState, Is.EqualTo(new[] { new KeyValuePair<string, string>("{OriginalFormat}", "message: p2, p0") }));
  }

  [Test]
  public void LogCriticalFormat_WithException ()
  {
    var fakeLogger = new FakeLogger();
    fakeLogger.ControlLevel(LogLevel.Critical, true);

    var exception = new ApplicationException("Test exception");

    fakeLogger.LogCriticalFormat(exception, "message: {1}, {0}", "p0", "p1", "p2");

    var logs = fakeLogger.Collector.GetSnapshot();
    Assert.That(logs.Count, Is.EqualTo(1));
    Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Critical));
    Assert.That(logs[0].Message, Is.EqualTo("message: p1, p0"));
    Assert.That(logs[0].StructuredState, Is.EqualTo(new[] { new KeyValuePair<string, string>("{OriginalFormat}", "message: p1, p0") }));
    Assert.That(logs[0].Exception, Is.SameAs(exception));
  }
}
