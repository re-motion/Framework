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
using NUnit.Framework;
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.Logging;
using Rhino.Mocks;

namespace Remotion.UnitTests.Logging
{
  [TestFixture]
  public class LogExtensionsTest
  {
    [Test]
    public void LogAndReturnValue_ReturnsValue ()
    {
      var logMock = MockRepository.GenerateMock<ILog> ();

      var result = "test".LogAndReturnValue (logMock, LogLevel.Debug, value => string.Format ("x{0}y", value));
      Assert.That (result, Is.EqualTo ("test"));
    }

    [Test]
    public void LogAndReturnValue_DoesNotLog_IfNotConfigured ()
    {
      var logMock = MockRepository.GenerateMock<ILog> ();

      "test".LogAndReturnValue (logMock, LogLevel.Debug, value => { throw new Exception ("Should not be called"); });
      logMock.AssertWasNotCalled (mock => mock.Log (Arg<LogLevel>.Is.Anything, Arg<int?>.Is.Anything, Arg<object>.Is.Anything, Arg<Exception>.Is.Anything));
    }

    [Test]
    public void LogAndReturnValue_Logs_IfConfigured ()
    {
      var logMock = MockRepository.GenerateMock<ILog> ();
      logMock.Expect (mock => mock.IsEnabled (LogLevel.Debug)).Return (true);
      logMock.Replay ();

      "test".LogAndReturnValue (logMock, LogLevel.Debug, value => string.Format ("x{0}y", value));
      logMock.AssertWasCalled (mock => mock.Log (LogLevel.Debug, (int?) null, "xtesty", (Exception) null));
    }

    [Test]
    public void LogAndReturnItems_ReturnsValue ()
    {
      var logMock = MockRepository.GenerateMock<ILog>();
      logMock.Expect (mock => mock.IsEnabled (LogLevel.Debug)).Return (true);
      logMock.Replay ();

      var input = new[] { "A", "B", "C" };
      var result = input.LogAndReturnItems (logMock, LogLevel.Debug, count => string.Format ("x{0}y", count));
      Assert.That (result, Is.EqualTo (new[] { "A", "B", "C" }));
      Assert.That (result, Is.Not.SameAs (input));
    }

    [Test]
    public void LogAndReturnItems_DoesNotIterate_AndDoesNotLog_IfNotConfigured ()
    {
      var logMock = MockRepository.GenerateMock<ILog> ();
      var sequenceMock = MockRepository.GenerateStrictMock<IEnumerable<int>>();

      var result = sequenceMock.LogAndReturnItems (logMock, LogLevel.Debug, value => { throw new Exception ("Should not be called"); });
      Assert.That (result, Is.SameAs (sequenceMock));
      logMock.AssertWasNotCalled (mock => mock.Log (Arg<LogLevel>.Is.Anything, Arg<int?>.Is.Anything, Arg<object>.Is.Anything, Arg<Exception>.Is.Anything));
    }

    [Test]
    public void LogAndReturnItems_LogsAfterIterationIsComplete_IfConfigured ()
    {
      var logMock = MockRepository.GenerateMock<ILog>();
      logMock.Expect (mock => mock.IsEnabled (LogLevel.Debug)).Return (true);
      logMock.Replay();

      var result = new[] { "A", "B", "C" }.LogAndReturnItems (logMock, LogLevel.Debug, count => string.Format ("x{0}y", count));
      var enumerator = result.GetEnumerator();
      enumerator.MoveNext();
      enumerator.MoveNext();
      enumerator.MoveNext();
      Assert.That (enumerator.Current, Is.EqualTo ("C"));
      logMock.AssertWasNotCalled (mock => mock.Log (LogLevel.Debug, "x3y"));
      enumerator.MoveNext();
      logMock.AssertWasCalled (mock => mock.Log (LogLevel.Debug, "x3y"));
    }
  }
}
