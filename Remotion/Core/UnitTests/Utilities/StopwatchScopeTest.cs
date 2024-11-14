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
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class StopwatchScopeTest
  {
    [Test]
    public void CreateScope_WithAction ()
    {
      string resultContext = "?";
      var resultTotalTimeSpan = TimeSpan.Zero;
      var resultCheckpointTimeSpan = TimeSpan.Zero;

      using (StopwatchScope.CreateScope((context, s) =>
      {
        resultContext = context;
        resultTotalTimeSpan = s.ElapsedTotal;
        resultCheckpointTimeSpan = s.ElapsedSinceLastCheckpoint;
      }))
      {
        Wait(TimeSpan.FromMilliseconds(5.0));
      }

      Assert.That(resultContext, Is.EqualTo("end"));
      Assert.That(resultTotalTimeSpan, Is.GreaterThan(TimeSpan.FromMilliseconds(5.0)));
      Assert.That(resultTotalTimeSpan, Is.LessThan(TimeSpan.FromSeconds(10.0)));
      Assert.That(resultCheckpointTimeSpan, Is.EqualTo(resultTotalTimeSpan));
    }

    [Test]
    public void Checkpoint ()
    {
      var times = new List<Tuple<string, TimeSpan, TimeSpan>>();
      using (var scope = StopwatchScope.CreateScope((context, s) => times.Add(Tuple.Create(context, s.ElapsedTotal, s.ElapsedSinceLastCheckpoint))))
      {
        Wait(TimeSpan.FromMilliseconds(5.0));
        scope.Checkpoint("One");
        Wait(TimeSpan.FromMilliseconds(5.0));
        scope.Checkpoint("Two");
        Wait(TimeSpan.FromMilliseconds(5.0));
      }
      Assert.That(times.Count, Is.EqualTo(3));

      Assert.That(times[0].Item1, Is.EqualTo("One"));
      Assert.That(times[0].Item2, Is.GreaterThan(TimeSpan.FromMilliseconds(5.0))); // total
      Assert.That(times[0].Item3, Is.GreaterThan(TimeSpan.FromMilliseconds(5.0))); // since last checkpoint

      Assert.That(times[1].Item1, Is.EqualTo("Two"));
      Assert.That(times[1].Item2, Is.GreaterThan(times[0].Item2 + TimeSpan.FromMilliseconds(5.0))); // total
      Assert.That(times[1].Item3, Is.GreaterThan(TimeSpan.FromMilliseconds(5.0))); // since last checkpoint
      Assert.That(times[1].Item3, Is.LessThan(times[1].Item2));

      Assert.That(times[2].Item1, Is.EqualTo("end"));
      Assert.That(times[2].Item2, Is.GreaterThan(times[1].Item2 + TimeSpan.FromMilliseconds(5.0))); // total
      Assert.That(times[2].Item3, Is.GreaterThan(TimeSpan.FromMilliseconds(5.0))); // since last checkpoint
      Assert.That(times[2].Item3, Is.LessThan(times[2].Item2));
    }

    [Test]
    public void Checkpoint_AfterDispose ()
    {
      var scope = StopwatchScope.CreateScope((context, s) => { });
      scope.Dispose();
      Assert.That(
          () => scope.Checkpoint(""),
          Throws.InstanceOf<ObjectDisposedException>());
    }

    [Test]
    public void ElapsedSinceLastCheckpoint_AfterScopeDisposed ()
    {
      var scope = StopwatchScope.CreateScope((context, s) => { });
      scope.Checkpoint("test");
      Wait(TimeSpan.FromMilliseconds(1.0));
      Assert.That(scope.ElapsedSinceLastCheckpoint, Is.GreaterThan(TimeSpan.Zero));

      scope.Dispose();

      Assert.That(scope.ElapsedSinceLastCheckpoint, Is.EqualTo(TimeSpan.Zero));
    }

    [Test]
    public void Pause ()
    {
      var scope = StopwatchScope.CreateScope((context, s) => { });

      scope.Pause();
      var elapsedBefore = scope.ElapsedTotal;
      Wait(TimeSpan.FromMilliseconds(5.0));
      Assert.That(scope.ElapsedTotal, Is.EqualTo(elapsedBefore));
    }

    [Test]
    public void Pause_Twice ()
    {
      var scope = StopwatchScope.CreateScope((context, s) => { });

      scope.Pause();
      scope.Pause();
      var elapsedBefore = scope.ElapsedTotal;
      Wait(TimeSpan.FromMilliseconds(5.0));
      Assert.That(scope.ElapsedTotal, Is.EqualTo(elapsedBefore));
    }

    [Test]
    public void Pause_AfterDispose ()
    {
      var scope = StopwatchScope.CreateScope((context, s) => { });
      scope.Dispose();
      Assert.That(
          () => scope.Pause(),
          Throws.InstanceOf<ObjectDisposedException>());
    }

    [Test]
    public void Resume ()
    {
      var scope = StopwatchScope.CreateScope((context, s) => { });

      scope.Pause();
      var elapsedBefore = scope.ElapsedTotal;
      Wait(TimeSpan.FromMilliseconds(5.0));
      Assert.That(scope.ElapsedTotal, Is.EqualTo(elapsedBefore));

      scope.Resume();
      Wait(TimeSpan.FromMilliseconds(1.0));
      Assert.That(scope.ElapsedTotal, Is.GreaterThan(elapsedBefore));
    }

    [Test]
    public void Resume_AfterDispose ()
    {
      var scope = StopwatchScope.CreateScope((context, s) => { });
      scope.Dispose();
      Assert.That(
          () => scope.Resume(),
          Throws.InstanceOf<ObjectDisposedException>());
    }

    [Test]
    public void Dispose_AfterDispose ()
    {
      int counter = 0;
      var scope = StopwatchScope.CreateScope((context, s) => ++counter);

      scope.Dispose();
      Assert.That(counter, Is.EqualTo(1));

      scope.Dispose();
      Assert.That(counter, Is.EqualTo(1));
    }

    [Test]
    public void CreateScope_Writer ()
    {
      var writerMock = new Mock<TextWriter>();

      var scope = StopwatchScope.CreateScope(writerMock.Object, "{context}#{elapsed}#{elapsed:ms}#{elapsedCP}#{elapsedCP:ms}");

      Wait(TimeSpan.FromMilliseconds(1.0));

      scope.Pause();

      var firstElapsed = scope.ElapsedTotal;
      var firstElapsedCP = scope.ElapsedSinceLastCheckpoint;
      scope.Checkpoint("one");

      scope.Resume();

      Wait(TimeSpan.FromMilliseconds(1.0));

      scope.Pause();
      var secondElapsed = scope.ElapsedTotal;
      var secondElapsedCP = scope.ElapsedSinceLastCheckpoint;
      scope.Dispose();

      var expectedFirstArgs = new[] {
          "one",
          firstElapsed.ToString(),
          firstElapsed.TotalMilliseconds.ToString(),
          firstElapsedCP.ToString(),
          firstElapsedCP.TotalMilliseconds.ToString()
      };
      writerMock.Verify(mock => mock.WriteLine("{0}#{1}#{2}#{3}#{4}", expectedFirstArgs), Times.AtLeastOnce());

      var expectedSecondArgs = new[] {
          "end",
          secondElapsed.ToString(),
          secondElapsed.TotalMilliseconds.ToString(),
          secondElapsedCP.ToString(),
          secondElapsedCP.TotalMilliseconds.ToString()
      };
      writerMock.Verify(mock => mock.WriteLine("{0}#{1}#{2}#{3}#{4}", expectedSecondArgs), Times.AtLeastOnce());
    }

    [Test]
    public void CreateScope_Log ()
    {
      var fakeLogger = new FakeLogger();

      var scope = StopwatchScope.CreateScope(fakeLogger, LogLevel.Error, "{context}#{elapsed}#{elapsed:ms}#{elapsedCP}#{elapsedCP:ms}");

      Wait(TimeSpan.FromMilliseconds(1.0));

      scope.Pause();

      var firstElapsed = scope.ElapsedTotal;
      var firstElapsedCP = scope.ElapsedSinceLastCheckpoint;
      scope.Checkpoint("one");

      Assert.That(fakeLogger.Collector.Count, Is.EqualTo(1));

      scope.Resume();

      Wait(TimeSpan.FromMilliseconds(1.0));

      scope.Pause();
      var secondElapsed = scope.ElapsedTotal;
      var secondElapsedCP = scope.ElapsedSinceLastCheckpoint;
      scope.Dispose();

      var logEntries = fakeLogger.Collector.GetSnapshot();
      Assert.That(logEntries.Count, Is.EqualTo(2));
      Assert.That(logEntries[0].Level, Is.EqualTo(LogLevel.Error));
      Assert.That(logEntries[0].Message, Is.EqualTo($"one#{firstElapsed}#{firstElapsed.TotalMilliseconds}#{firstElapsedCP}#{firstElapsedCP.TotalMilliseconds}"));
      Assert.That(logEntries[1].Level, Is.EqualTo(LogLevel.Error));
      Assert.That(logEntries[1].Message, Is.EqualTo($"end#{secondElapsed}#{secondElapsed.TotalMilliseconds}#{secondElapsedCP}#{secondElapsedCP.TotalMilliseconds}"));
    }

    [Test]
    public void CreateScope_Log_WithPartialFormatStringPlaceholders ()
    {
      var fakeLogger = new FakeLogger();

      var scope = StopwatchScope.CreateScope(fakeLogger, LogLevel.Information, "{context}#{elapsed:ms}#{elapsedCP:ms}");

      Wait(TimeSpan.FromMilliseconds(1.0));

      scope.Pause();

      var firstElapsed = scope.ElapsedTotal;
      var firstElapsedCP = scope.ElapsedSinceLastCheckpoint;
      scope.Checkpoint("one");

      scope.Dispose();

      var logEntries = fakeLogger.Collector.GetSnapshot();
      Assert.That(logEntries.Count, Is.EqualTo(2));
      Assert.That(logEntries[0].Level, Is.EqualTo(LogLevel.Information));
      Assert.That(logEntries[0].Message, Is.EqualTo($"one#{firstElapsed.TotalMilliseconds}#{firstElapsedCP.TotalMilliseconds}"));
    }

    [Test]
    public void CreateScope_Console ()
    {
      var oldOut = Console.Out;
      var writerMock = new Mock<TextWriter>();
      Console.SetOut(writerMock.Object);

      try
      {
        var scope = StopwatchScope.CreateScope("{context}#{elapsed}#{elapsed:ms}#{elapsedCP}#{elapsedCP:ms}");

        Wait(TimeSpan.FromMilliseconds(1.0));

        scope.Pause();

        var firstElapsed = scope.ElapsedTotal;
        var firstElapsedCP = scope.ElapsedSinceLastCheckpoint;
        scope.Checkpoint("one");

        scope.Resume();

        Wait(TimeSpan.FromMilliseconds(1.0));

        scope.Pause();
        var secondElapsed = scope.ElapsedTotal;
        var secondElapsedCP = scope.ElapsedSinceLastCheckpoint;
        scope.Dispose();

        var expectedFirstArgs = new[]
                                {
                                    "one",
                                    firstElapsed.ToString(),
                                    firstElapsed.TotalMilliseconds.ToString(),
                                    firstElapsedCP.ToString(),
                                    firstElapsedCP.TotalMilliseconds.ToString()
                                };
        writerMock.Verify(mock => mock.WriteLine("{0}#{1}#{2}#{3}#{4}", expectedFirstArgs), Times.AtLeastOnce());

        var expectedSecondArgs = new[]
                                 {
                                     "end",
                                     secondElapsed.ToString(),
                                     secondElapsed.TotalMilliseconds.ToString(),
                                     secondElapsedCP.ToString(),
                                     secondElapsedCP.TotalMilliseconds.ToString()
                                 };
        writerMock.Verify(mock => mock.WriteLine("{0}#{1}#{2}#{3}#{4}", expectedSecondArgs), Times.AtLeastOnce());
      }
      finally
      {
        Console.SetOut(oldOut);
      }
    }

    private void Wait (TimeSpan timeSpan)
    {
      Stopwatch sw = Stopwatch.StartNew();
      while (sw.Elapsed <= timeSpan)
      {
      }
      sw.Stop();
    }
  }
}
