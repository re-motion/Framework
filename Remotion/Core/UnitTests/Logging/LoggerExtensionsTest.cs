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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Remotion.Logging;
using Microsoft.Extensions.Logging.Testing;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Remotion.UnitTests.Logging
{
  [TestFixture]
  public class LoggerExtensionsTest
  {
    [Test]
    public void LogAndReturnValue_ReturnsValue ()
    {
      var fakeLogger = new FakeLogger();
      fakeLogger.ControlLevel(LogLevel.Debug, false);

      var result = "test".LogAndReturnValue(fakeLogger, LogLevel.Debug, value => string.Format("x{0}y", value));
      Assert.That(result, Is.EqualTo("test"));
    }

    [Test]
    public void LogAndReturnValue_DoesNotLog_IfNotConfigured ()
    {
      var fakeLogger = new FakeLogger();
      fakeLogger.ControlLevel(LogLevel.Debug, false);

      "test".LogAndReturnValue(fakeLogger, LogLevel.Debug, value => { throw new Exception("Should not be called"); });
      Assert.That(fakeLogger.Collector.Count, Is.EqualTo(0));
    }

    [Test]
    public void LogAndReturnValue_Logs_IfConfigured ()
    {
      var fakeLogger = new FakeLogger();
      fakeLogger.ControlLevel(LogLevel.Debug, true);

      "test".LogAndReturnValue(fakeLogger, LogLevel.Debug, value => string.Format("x{0}y", value));

      var logs = fakeLogger.Collector.GetSnapshot();
      Assert.That(logs.Count, Is.EqualTo(1));
      Assert.That(logs[0].Level, Is.EqualTo(LogLevel.Debug));
      Assert.That(logs[0].Message, Is.EqualTo("xtesty"));
    }

    [Test]
    public void LogAndReturnItems_ReturnsValue ()
    {
      var fakeLogger = new FakeLogger();
      fakeLogger.ControlLevel(LogLevel.Debug, true);

      var input = new[] { "A", "B", "C" };
      var result = input.LogAndReturnItems(fakeLogger, LogLevel.Debug, count => string.Format("x{0}y", count));
      Assert.That(result, Is.EqualTo(new[] { "A", "B", "C" }));
      Assert.That(result, Is.Not.SameAs(input));
    }

    [Test]
    public void LogAndReturnItems_DoesNotIterate_AndDoesNotLog_IfNotConfigured ()
    {
      var fakeLogger = new FakeLogger();
      fakeLogger.ControlLevel(LogLevel.Debug, false);
      var sequenceMock = new Mock<IEnumerable<int>>(MockBehavior.Strict);

      var result = sequenceMock.Object.LogAndReturnItems(fakeLogger, LogLevel.Debug, value => { throw new Exception("Should not be called"); });
      Assert.That(result, Is.SameAs(sequenceMock.Object));
      Assert.That(fakeLogger.Collector.Count, Is.EqualTo(0));
    }

    [Test]
    public void LogAndReturnItems_LogsAfterIterationIsComplete_IfConfigured ()
    {
      var fakeLogger = new FakeLogger();
      fakeLogger.ControlLevel(LogLevel.Debug, true);
      var result = new[] { "A", "B", "C" }.LogAndReturnItems(fakeLogger, LogLevel.Debug, count => string.Format("x{0}y", count));
      var enumerator = result.GetEnumerator();
      enumerator.MoveNext();
      enumerator.MoveNext();

      enumerator.MoveNext();
      Assert.That(enumerator.Current, Is.EqualTo("C"));
      var logs1 = fakeLogger.Collector.GetSnapshot();
      Assert.That(logs1.Count, Is.EqualTo(0));

      enumerator.MoveNext();
      var logs2 = fakeLogger.Collector.GetSnapshot();
      Assert.That(logs2.Count, Is.EqualTo(1));
      Assert.That(logs2[0].Level, Is.EqualTo(LogLevel.Debug));
      Assert.That(logs2[0].Message, Is.EqualTo("x3y"));
    }
  }
}
