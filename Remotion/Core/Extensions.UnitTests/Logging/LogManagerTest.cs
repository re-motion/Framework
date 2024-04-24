// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Logging;
using Remotion.ServiceLocation;

#pragma warning disable REMOTION0003

namespace Remotion.Extensions.UnitTests.Logging;

[TestFixture]
public class LogManagerTest
{
  [Test]
  public void GetLogger_WithTypeParameter ()
  {
    var serviceLocatorStub = new Mock<IServiceLocator>();
    var lazyLogger = LogManager.GetLogger(typeof(LogManagerTest));
    var fakeLoggerCollector = new FakeLogCollector();

    using (new ServiceLocatorScope(serviceLocatorStub.Object))
    {
      serviceLocatorStub.Setup(_ => _.GetInstance<ILoggerFactory>()).Returns(new LoggerFactory([new FakeLoggerProvider(fakeLoggerCollector)]));

      lazyLogger.Log(LogLevel.Trace, new EventId(42), "Test Message");

      Assert.That(fakeLoggerCollector.LatestRecord.Level, Is.EqualTo(LogLevel.Trace));
      Assert.That(fakeLoggerCollector.LatestRecord.Message, Is.EqualTo("Test Message"));
    }
  }

  [Test]
  public void GetLogger_WithStringParameter ()
  {
    var serviceLocatorStub = new Mock<IServiceLocator>();
    var lazyLogger = LogManager.GetLogger("My.Logged.Type");
    var fakeLoggerCollector = new FakeLogCollector();

    using (new ServiceLocatorScope(serviceLocatorStub.Object))
    {
      serviceLocatorStub.Setup(_ => _.GetInstance<ILoggerFactory>()).Returns(new LoggerFactory([new FakeLoggerProvider(fakeLoggerCollector)]));

      lazyLogger.Log(LogLevel.Trace, new EventId(42), "Test Message");

      Assert.That(fakeLoggerCollector.LatestRecord.Level, Is.EqualTo(LogLevel.Trace));
      Assert.That(fakeLoggerCollector.LatestRecord.Message, Is.EqualTo("Test Message"));
    }
  }
}
