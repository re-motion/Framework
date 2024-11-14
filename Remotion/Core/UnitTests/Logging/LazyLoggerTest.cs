// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Remotion.Logging;

namespace Remotion.UnitTests.Logging;

[TestFixture]
public class LazyLoggerTest
{
  [Test]
  public void Log ()
  {
    var loggerStub = new Mock<ILogger>();
    var lazy = new Lazy<ILogger>(() => loggerStub.Object);
    var lazyLogger = new LazyLogger(lazy);

    var expectedLogLevel = LogLevel.Information;
    var expectedEventID = new EventId(42);
    var expectedState = "Test State";
    var exectedException = new Exception("Test Exception");
    var expectedFormatter = (string _, Exception _) => "test message";
    loggerStub.Setup(_ => _.Log(expectedLogLevel, expectedEventID, expectedState, exectedException, expectedFormatter));

    Assert.That(lazy.IsValueCreated, Is.False);
    lazyLogger.Log(expectedLogLevel, expectedEventID, expectedState, exectedException, expectedFormatter);
  }

  [Test]
  public void IsEnabled ()
  {
    var loggerStub = new Mock<ILogger>();
    var lazy = new Lazy<ILogger>(() => loggerStub.Object);
    var lazyLogger = new LazyLogger(lazy);

    var expectedLogLevel = LogLevel.Error;
    loggerStub.Setup(_ => _.IsEnabled(expectedLogLevel)).Returns(true);

    Assert.That(lazy.IsValueCreated, Is.False);
   Assert.That(lazyLogger.IsEnabled(expectedLogLevel), Is.True);
  }

  [Test]
  public void BeginScope ()
  {
    var loggerStub = new Mock<ILogger>();
    var lazy = new Lazy<ILogger>(() => loggerStub.Object);
    var lazyLogger = new LazyLogger(lazy);

    var expectedResult = Mock.Of<IDisposable>();
    var expectedState = "Test State";
    loggerStub.Setup(_ => _.BeginScope(expectedState)).Returns(expectedResult);

    Assert.That(lazy.IsValueCreated, Is.False);
    Assert.That(lazyLogger.BeginScope(expectedState), Is.SameAs(expectedResult));
  }
}
