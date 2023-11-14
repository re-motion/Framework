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
using Moq;
using NUnit.Framework;
using Remotion.Logging;

namespace Remotion.UnitTests.Logging
{
  [TestFixture]
  public class LogExtensionsTest
  {
    [Test]
    public void LogAndReturnValue_ReturnsValue ()
    {
      var logMock = new Mock<ILog>();

      var result = "test".LogAndReturnValue(logMock.Object, LogLevel.Debug, value => string.Format("x{0}y", value));
      Assert.That(result, Is.EqualTo("test"));
    }

    [Test]
    public void LogAndReturnValue_DoesNotLog_IfNotConfigured ()
    {
      var logMock = new Mock<ILog>();

      "test".LogAndReturnValue(logMock.Object, LogLevel.Debug, value => { throw new Exception("Should not be called"); });
      logMock.Verify(mock => mock.Log(It.IsAny<LogLevel>(), It.IsAny<int?>(), It.IsAny<object>(), It.IsAny<Exception>()), Times.Never());
    }

    [Test]
    public void LogAndReturnValue_Logs_IfConfigured ()
    {
      var logMock = new Mock<ILog>();
      logMock.Setup(mock => mock.IsEnabled(LogLevel.Debug)).Returns(true).Verifiable();

      "test".LogAndReturnValue(logMock.Object, LogLevel.Debug, value => string.Format("x{0}y", value));
      logMock.Verify(mock => mock.Log(LogLevel.Debug, (int?)null, "xtesty", (Exception)null), Times.AtLeastOnce());
    }

    [Test]
    public void LogAndReturnItems_ReturnsValue ()
    {
      var logMock = new Mock<ILog>();
      logMock.Setup(mock => mock.IsEnabled(LogLevel.Debug)).Returns(true).Verifiable();

      var input = new[] { "A", "B", "C" };
      var result = input.LogAndReturnItems(logMock.Object, LogLevel.Debug, count => string.Format("x{0}y", count));
      Assert.That(result, Is.EqualTo(new[] { "A", "B", "C" }));
      Assert.That(result, Is.Not.SameAs(input));
    }

    [Test]
    public void LogAndReturnItems_DoesNotIterate_AndDoesNotLog_IfNotConfigured ()
    {
      var logMock = new Mock<ILog>();
      var sequenceMock = new Mock<IEnumerable<int>>(MockBehavior.Strict);

      var result = sequenceMock.Object.LogAndReturnItems(logMock.Object, LogLevel.Debug, value => { throw new Exception("Should not be called"); });
      Assert.That(result, Is.SameAs(sequenceMock.Object));
      logMock.Verify(mock => mock.Log(It.IsAny<LogLevel>(), It.IsAny<int?>(), It.IsAny<object>(), It.IsAny<Exception>()), Times.Never());
    }

    [Test]
    public void LogAndReturnItems_LogsAfterIterationIsComplete_IfConfigured ()
    {
      var logMock = new Mock<ILog>();
      logMock.Setup(mock => mock.IsEnabled(LogLevel.Debug)).Returns(true).Verifiable();

      var result = new[] { "A", "B", "C" }.LogAndReturnItems(logMock.Object, LogLevel.Debug, count => string.Format("x{0}y", count));
      var enumerator = result.GetEnumerator();
      enumerator.MoveNext();
      enumerator.MoveNext();
      enumerator.MoveNext();
      Assert.That(enumerator.Current, Is.EqualTo("C"));
      logMock.Verify(mock => mock.Log(LogLevel.Debug, null, "x3y", null), Times.Never());
      enumerator.MoveNext();
      logMock.Verify(mock => mock.Log(LogLevel.Debug, null, "x3y", null), Times.AtLeastOnce());
    }
  }
}
