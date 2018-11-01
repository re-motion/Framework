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

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Development.Core.UnitTesting
{
  [TestFixture]
  public class ThreadRunnerTest
  {
    private MockRepository _mockRepository;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();
    }

    [Test]
    public void Run ()
    {
      bool threadRun = false;
      ThreadRunner.Run (delegate { threadRun = true; });

      Assert.That (threadRun, Is.True);
    }

    [Test]
    public void Ctor_WithTimeout ()
    {
      var threadRunner = new ThreadRunner (delegate { }, TimeSpan.FromSeconds (1.0));
      Assert.That (threadRunner.Timeout, Is.EqualTo (TimeSpan.FromSeconds (1.0)));
    }

    [Test]
    public void Ctor_WithoutTimeout_HasInfiniteTimeout ()
    {
      var threadRunner = new ThreadRunner (delegate { });
      Assert.That (threadRunner.Timeout.TotalMilliseconds, Is.EqualTo (Timeout.Infinite));
    }

    [Test]
    public void Run_CallsJoin ()
    {
      TimeSpan timeout = TimeSpan.FromSeconds (1.0);
      var threadRunnerMock = _mockRepository.PartialMock<ThreadRunner> ((ThreadStart) delegate { }, timeout);
      threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "JoinThread", Arg<Thread>.Is.Anything)).Return (true);

      threadRunnerMock.Replay ();
      threadRunnerMock.Run();
      threadRunnerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Run_CallsJoin_WithRightThread ()
    {
      using (var waitHandle = new ManualResetEvent (false))
      {
        Thread threadRunnerThread = null;
        var threadMethod = (ThreadStart) delegate { threadRunnerThread = Thread.CurrentThread; waitHandle.Set (); };

        TimeSpan timeout = TimeSpan.MaxValue;
        var threadRunnerMock = _mockRepository.PartialMock<ThreadRunner> (threadMethod, timeout);

        threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "JoinThread", Arg<Thread>.Is.Anything)).
            WhenCalled (
            // when this expectation is reached, assert that the threadRunnerThread was passed to the method
            invocation =>
            {
              waitHandle.WaitOne ();
              Assert.That (invocation.Arguments[0], Is.SameAs (threadRunnerThread));
            }).Return (true);

        threadRunnerMock.Replay();
        threadRunnerMock.Run();
        threadRunnerMock.VerifyAllExpectations();
      }
    }

    [Test]
    public void Run_UsesJoinResult_ToIndicateTimedOut_False ()
    {
      TimeSpan timeout = TimeSpan.FromSeconds (1.0);
      var threadRunnerMock = _mockRepository.PartialMock<ThreadRunner> ((ThreadStart) delegate { }, timeout);
      threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "JoinThread", Arg<Thread>.Is.Anything)).Return (true);

      threadRunnerMock.Replay ();
      Assert.That (threadRunnerMock.Run (), Is.False);
      threadRunnerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Run_UsesJoinResult_ToIndicateTimedOut_True ()
    {
      TimeSpan timeout = TimeSpan.FromSeconds (1.0);
      var threadRunnerMock = _mockRepository.PartialMock<ThreadRunner> ((ThreadStart) delegate { }, timeout);
      threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "JoinThread", Arg<Thread>.Is.Anything)).Return (false);

      threadRunnerMock.Replay ();
      Assert.That (threadRunnerMock.Run (), Is.True);
      threadRunnerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Run_WithTimedOutThread_CallsAbort ()
    {
      TimeSpan timeout = TimeSpan.FromSeconds (1.0);
      var threadRunnerMock = _mockRepository.PartialMock<ThreadRunner> ((ThreadStart) delegate { }, timeout);
      threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "JoinThread", Arg<Thread>.Is.Anything)).Return (false);
      threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "AbortThread", Arg<Thread>.Is.Anything));

      threadRunnerMock.Replay ();
      threadRunnerMock.Run ();
      threadRunnerMock.VerifyAllExpectations ();
    }

    [Test]
    public void WithMillisecondsTimeout ()
    {
      ThreadRunner threadRunner = ThreadRunner.WithMillisecondsTimeout (delegate { }, 250);
      Assert.That (threadRunner.Timeout, Is.EqualTo (TimeSpan.FromMilliseconds (250)));
    }

    [Test]
    public void WithSecondsTimeout ()
    {
      ThreadRunner threadRunner = ThreadRunner.WithSecondsTimeout (delegate { }, 250);
      Assert.That (threadRunner.Timeout, Is.EqualTo (TimeSpan.FromSeconds (250)));
    }

    [Test]
    public void WithTimeout ()
    {
      ThreadRunner threadRunner = ThreadRunner.WithTimeout (delegate { }, TimeSpan.FromMinutes (250));
      Assert.That (threadRunner.Timeout, Is.EqualTo (TimeSpan.FromMinutes (250)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "xy")]
    public void Run_WithException ()
    {
      var exception = new InvalidOperationException ("xy");
      ThreadRunner.Run (() => { throw exception; });
    }

    [Test]
    public void RunWithTimeout ()
    {
      bool timedOut = ThreadRunner.WithTimeout (RunTimesOutEndlessLoop, TimeSpan.FromSeconds (0.1)).Run();
      Assert.That (timedOut, Is.True);
    }

    [Test]
    public void RunWithoutTimeout ()
    {
      bool timedOut = ThreadRunner.WithTimeout (RunTimesOutVeryFastFunction, TimeSpan.FromMilliseconds (int.MaxValue)).Run ();
      Assert.That (timedOut, Is.False);
    }

    private static void RunTimesOutEndlessLoop ()
    {
      while (true) { }
    }

    private static void RunTimesOutVeryFastFunction ()
    {
    }
  }
}
