// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Threading;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Development.Core.UnitTesting
{
  [TestFixture]
  public class ThreadRunnerTest
  {
    private MockRepository _mockRepository = default!;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
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
      var threadRunnerMock = _mockRepository.PartialMock<ThreadRunner>((ThreadStart) delegate { }, timeout);
      threadRunnerMock.Expect(mock => PrivateInvoke.InvokeNonPublicMethod(mock, "JoinThread", Arg<Thread>.Is.Anything)).Return(true);

      threadRunnerMock.Replay();
      threadRunnerMock.Run();
      threadRunnerMock.VerifyAllExpectations();
    }

    [Test]
    public void Run_CallsJoin_WithRightThread ()
    {
      using (var waitHandle = new ManualResetEvent(false))
      {
        Thread? threadRunnerThread = null;
        var threadMethod = (ThreadStart) delegate { threadRunnerThread = Thread.CurrentThread; waitHandle.Set(); };

        TimeSpan timeout = TimeSpan.MaxValue;
        var threadRunnerMock = _mockRepository.PartialMock<ThreadRunner>(threadMethod, timeout);

        threadRunnerMock.Expect(mock => PrivateInvoke.InvokeNonPublicMethod(mock, "JoinThread", Arg<Thread>.Is.Anything)).
            WhenCalled(
            // when this expectation is reached, assert that the threadRunnerThread was passed to the method
            invocation =>
            {
              waitHandle.WaitOne();
              Assert.That(invocation.Arguments[0], Is.SameAs(threadRunnerThread));
            }).Return(true);

        threadRunnerMock.Replay();
        threadRunnerMock.Run();
        threadRunnerMock.VerifyAllExpectations();
      }
    }

    [Test]
    public void Run_WithTimedOutThread_ThrowsTimeoutException ()
    {
      ManualResetEvent wait = new ManualResetEvent(false);
      TimeSpan timeout = TimeSpan.FromMinutes(2.0);
      var threadRunnerMock = _mockRepository.PartialMock<ThreadRunner>((ThreadStart)delegate { wait.Set(); }, timeout);
      threadRunnerMock
          .Expect(mock => PrivateInvoke.InvokeNonPublicMethod(mock, "JoinThread", Arg<Thread>.Is.Anything))
          .WhenCalled(mi => { wait.WaitOne(); })
          .Return(false);

      threadRunnerMock.Replay();
      Assert.That(
          () => threadRunnerMock.Run(),
          Throws.TypeOf<TimeoutException>()
              .With.Message.EqualTo("The thread has not finished executing within the timeout (00:02:00) and was not cleaned up.")
              .With.InnerException.Null);
      threadRunnerMock.VerifyAllExpectations();
    }

    [Test]
    public void Run_WithExceptionOnTimedOutThread_ThrowsTimeoutException ()
    {
      ManualResetEvent wait = new ManualResetEvent(false);
      var exception = new InvalidOperationException("xy");
      TimeSpan timeout = TimeSpan.FromMinutes(1.0);
      var threadRunnerMock = _mockRepository.PartialMock<ThreadRunner>(
          (ThreadStart)delegate
          {
            wait.Set();
            throw exception;
          },
          timeout);
      threadRunnerMock
          .Expect(mock => PrivateInvoke.InvokeNonPublicMethod(mock, "JoinThread", Arg<Thread>.Is.Anything))
          .WhenCalled(
              mi =>
              {
                wait.WaitOne();
                Thread.Sleep(TimeSpan.FromSeconds(1));
              })
          .Return(false);

      threadRunnerMock.Replay();
      Assert.That(
          () => threadRunnerMock.Run(),
          Throws.TypeOf<TimeoutException>()
              .With.Message.EqualTo("The thread has not finished executing within the timeout (00:01:00) and was not cleaned up.")
              .With.InnerException.SameAs(exception));
      threadRunnerMock.VerifyAllExpectations();
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
