// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Logging;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.Logging;

[TestFixture]
public class LazyLoggerFactoryTest
{
  [Test]
  public void CreateLogger_WithTypeParameter ()
  {
    var serviceLocatorStub = new Mock<IServiceLocator>();
    var lazyLogger = LazyLoggerFactory.CreateLogger(typeof(SampleType));
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
  public void CreateLogger_WithGeneric ()
  {
    var serviceLocatorStub = new Mock<IServiceLocator>();
    var lazyLogger = LazyLoggerFactory.CreateLogger<SampleType>();
    var fakeLoggerCollector = new FakeLogCollector();

    using (new ServiceLocatorScope(serviceLocatorStub.Object))
    {
      serviceLocatorStub.Setup(_ => _.GetInstance<ILoggerFactory>()).Returns(new LoggerFactory([new FakeLoggerProvider(fakeLoggerCollector)]));

      lazyLogger.Log(LogLevel.Debug, new EventId(21), "Test Message Generic");

      Assert.That(fakeLoggerCollector.LatestRecord.Level, Is.EqualTo(LogLevel.Debug));
      Assert.That(fakeLoggerCollector.LatestRecord.Message, Is.EqualTo("Test Message Generic"));
    }
  }

  [Test]
  public void CreateLogger_WithStringParameter ()
  {
    var serviceLocatorStub = new Mock<IServiceLocator>();
    var lazyLogger = LazyLoggerFactory.CreateLogger("My.Logged.Type");
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
