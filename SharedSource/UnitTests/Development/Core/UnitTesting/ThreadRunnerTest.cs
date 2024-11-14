// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Threading;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Development.UnitTesting;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Development.Core.UnitTesting
{
  [TestFixture]
  public class ThreadRunnerTest
  {

    [SetUp]
    public void SetUp ()
    {
    }

    [Test]
    public void Run ()
    {
      bool threadRun = false;
      ThreadRunner.Run(delegate { threadRun = true; });

      Assert.That(threadRun, Is.True);
    }

    [Test]
    public void Ctor_WithTimeout ()
    {
      var threadRunner = new ThreadRunner(delegate { }, TimeSpan.FromSeconds(1.0));
      Assert.That(threadRunner.Timeout, Is.EqualTo(TimeSpan.FromSeconds(1.0)));
    }

    [Test]
    public void Ctor_WithoutTimeout_HasInfiniteTimeout ()
    {
      var threadRunner = new ThreadRunner(delegate { });
      Assert.That(threadRunner.Timeout.TotalMilliseconds, Is.EqualTo(Timeout.Infinite));
    }

    [Test]
    public void Run_CallsJoin ()
    {
      TimeSpan timeout = TimeSpan.FromSeconds(1.0);
      var threadRunnerMock = new Mock<ThreadRunner>((ThreadStart)delegate { }, timeout) { CallBase = true };
      threadRunnerMock
          .Protected()
          .Setup<bool>("JoinThread", true, ItExpr.IsAny<Thread>())
          .Returns(true)
          .Verifiable();

      threadRunnerMock.Object.Run();
      threadRunnerMock.Verify();
    }

    [Test]
    public void Run_CallsJoin_WithRightThread ()
    {
      using (var waitHandle = new ManualResetEvent(false))
      {
        Thread? threadRunnerThread = null;
        var threadMethod = (ThreadStart)delegate { threadRunnerThread = Thread.CurrentThread; waitHandle.Set(); };

        TimeSpan timeout = TimeSpan.MaxValue;
        var threadRunnerMock = new Mock<ThreadRunner>(threadMethod, timeout) { CallBase = true };

        threadRunnerMock
            .Protected()
            .Setup<bool>("JoinThread", true, ItExpr.IsAny<Thread>())
            .Returns(true)
            .Callback(
            // when this expectation is reached, assert that the threadRunnerThread was passed to the method
            (Thread thread) =>
            {
              waitHandle.WaitOne();
              Assert.That(thread, Is.SameAs(threadRunnerThread));
            })
            .Verifiable();

        threadRunnerMock.Object.Run();
        threadRunnerMock.Verify();
      }
    }

    [Test]
    public void Run_WithTimedOutThread_ThrowsTimeoutException ()
    {
      ManualResetEvent wait = new ManualResetEvent(false);
      TimeSpan timeout = TimeSpan.FromMinutes(2.0);
      var threadRunnerMock = new Mock<ThreadRunner>((ThreadStart)delegate { wait.Set(); }, timeout) { CallBase = true };
      threadRunnerMock
          .Protected()
          .Setup<bool>("JoinThread", true, ItExpr.IsAny<Thread>())
          .Returns(false)
          .Callback((Thread thread) => { wait.WaitOne(); })
          .Verifiable();

      Assert.That(
          () => threadRunnerMock.Object.Run(),
          Throws.TypeOf<TimeoutException>()
              .With.Message.EqualTo("The thread has not finished executing within the timeout (00:02:00) and was not cleaned up.")
              .With.InnerException.Null);
      threadRunnerMock.Verify();
    }

    [Test]
    public void Run_WithExceptionOnTimedOutThread_ThrowsTimeoutException ()
    {
      ManualResetEvent wait = new ManualResetEvent(false);
      var exception = new InvalidOperationException("xy");
      TimeSpan timeout = TimeSpan.FromMinutes(1.0);
      var threadRunnerMock = new Mock<ThreadRunner>(
                             (ThreadStart)delegate
                             {
                               wait.Set();
                               throw exception;
                             },
                             timeout) { CallBase = true };
      threadRunnerMock
          .Protected()
          .Setup<bool>("JoinThread", true, ItExpr.IsAny<Thread>())
          .Returns(false)
          .Callback((Thread thread) =>
              {
                wait.WaitOne();
                Thread.Sleep(TimeSpan.FromSeconds(1));
              });

      Assert.That(
          () => threadRunnerMock.Object.Run(),
          Throws.TypeOf<TimeoutException>()
              .With.Message.EqualTo("The thread has not finished executing within the timeout (00:01:00) and was not cleaned up.")
              .With.InnerException.SameAs(exception));
      threadRunnerMock.Verify();
    }

    [Test]
    public void WithTimeout ()
    {
      ThreadRunner threadRunner = new ThreadRunner(delegate { }, TimeSpan.FromMinutes(250));
      Assert.That(threadRunner.Timeout, Is.EqualTo(TimeSpan.FromMinutes(250)));
    }

    [Test]
    public void Run_WithException ()
    {
      var exception = new InvalidOperationException("xy");
      Assert.That(
          () => ThreadRunner.Run(() => { throw exception; }),
          Throws.InvalidOperationException.With.SameAs(exception));
    }
  }
}
