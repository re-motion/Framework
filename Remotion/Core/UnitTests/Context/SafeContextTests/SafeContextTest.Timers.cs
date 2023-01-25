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
using System.Threading;
using System.Timers;
using NUnit.Framework;
using Remotion.Context;
using Timer = System.Timers.Timer;

namespace Remotion.UnitTests.Context.SafeContextTests
{
  [TestFixture]
  public class SafeContextTimersTest : SafeContextTestBase
  {
    private class TestCallback
    {
      private readonly ManualResetEvent _callbackSignal = new(false);

      public ElapsedEventHandler Callback { get; }

      public object Sender { get; private set; }

      public ElapsedEventArgs Args { get; private set; }

      public TestCallback ()
      {
        Callback = (sender, args) =>
        {
          Sender = sender;
          Args = args;

          _callbackSignal.Set();
        };
      }

      public void WaitForCallback (int timeout)
      {
        Assert.That(_callbackSignal.WaitOne(timeout), Is.True);
      }
    }

    [Test]
    public void AddElapsedEventHandler_ImplicitSafeContextProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;
      using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
      {
        var testCallback = new TestCallback();
        using var timer = new Timer();
        timer.Interval = 10d;
        timer.AutoReset = false;
        SafeContext.Timers.AddElapsedEventHandler(timer, testCallback.Callback);

        timer.Start();
        testCallback.WaitForCallback(100);

        SafeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
      }
    }

    [Test]
    public void AddElapsedEventHandler_ImplicitSafeContextProvider_CanBeUndone ()
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;
      using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
      {
        var wasTriggered = false;

        using var timer = new Timer();
        timer.Interval = 10d;
        timer.AutoReset = false;
        var addElapsedEventHandler = SafeContext.Timers.AddElapsedEventHandler(timer, (_, _) => { wasTriggered = true; });
        timer.Elapsed -= addElapsedEventHandler;

        var manualResetEvent = new ManualResetEvent(false);
        timer.Elapsed += (_, _) => manualResetEvent.Set();

        timer.Start();
        manualResetEvent.WaitOne(50);
        Assert.That(wasTriggered, Is.False);
      }
    }

    [Test]
    public void AddElapsedEventHandler_ExplicitSafeContextProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;

      var testCallback = new TestCallback();
      using var timer = new Timer();
      timer.Interval = 10d;
      timer.AutoReset = false;
      SafeContext.Timers.AddElapsedEventHandler(safeContextStorageProvider, timer, testCallback.Callback);

      timer.Start();
      testCallback.WaitForCallback(100);

      SafeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
    }

    [Test]
    public void AddElapsedEventHandler_ExplicitSafeContextProvider_CanBeUndone ()
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;

      var wasTriggered = false;

      using var timer = new Timer();
      timer.Interval = 10d;
      timer.AutoReset = false;
      var addElapsedEventHandler = SafeContext.Timers.AddElapsedEventHandler(safeContextStorageProvider, timer, (_, _) => { wasTriggered = true; });
      timer.Elapsed -= addElapsedEventHandler;

      var manualResetEvent = new ManualResetEvent(false);
      timer.Elapsed += (_, _) => manualResetEvent.Set();

      timer.Start();
      manualResetEvent.WaitOne(50);
      Assert.That(wasTriggered, Is.False);
    }
  }
}
