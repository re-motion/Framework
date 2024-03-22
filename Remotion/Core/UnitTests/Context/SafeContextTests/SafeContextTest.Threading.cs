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
using NUnit.Framework;
using Remotion.Context;

namespace Remotion.UnitTests.Context.SafeContextTests
{
  [TestFixture]
  public class SafeContextThreadingTest : SafeContextTestBase
  {
    private class TestCallback
    {
      private readonly ManualResetEvent _callbackSignal = new(false);

      public TimerCallback Callback { get; }

      public object State { get; private set; }

      public TestCallback ()
      {
        Callback = state =>
        {
          State = state;
          _callbackSignal.Set();
        };
      }

      public void WaitForCallback (int timeout)
      {
        Assert.That(_callbackSignal.WaitOne(timeout), Is.True);
      }
    }

    [Test]
    public void NewTimer_CallbackOnlyConstructorAndImplicitSafeContextProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;
      using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
      {
        var testCallback = new TestCallback();
        using var timer = SafeContext.Threading.NewTimer(testCallback.Callback);

        timer.Change(0, 1000);
        testCallback.WaitForCallback(100);

        Assert.That(testCallback.State, Is.SameAs(timer));
        SafeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
      }
    }

    [Test]
    public void NewTimer_CallbackOnlyConstructorAndExplicitSafeContextProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;

      var testCallback = new TestCallback();
      using var timer = SafeContext.Threading.NewTimer(safeContextStorageProvider, testCallback.Callback);

      timer.Change(0, 1000);
      testCallback.WaitForCallback(100);

      Assert.That(testCallback.State, Is.SameAs(timer));
      SafeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
    }

    [Test]
    public void NewTimer_TimeSpanConstructorAndImplicitSafeContextProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;
      using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
      {
        var testCallback = new TestCallback();
        var state = new object();
        using var timer = SafeContext.Threading.NewTimer(testCallback.Callback, state, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        testCallback.WaitForCallback(100);

        Assert.That(testCallback.State, Is.SameAs(state));
        SafeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
      }
    }

    [Test]
    public void NewTimer_TimeSpanConstructorAndExplicitSafeContextProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;

      var testCallback = new TestCallback();
      var state = new object();
      using var timer = SafeContext.Threading.NewTimer(safeContextStorageProvider, testCallback.Callback, state, TimeSpan.Zero, TimeSpan.FromSeconds(1));

      testCallback.WaitForCallback(100);

      Assert.That(testCallback.State, Is.SameAs(state));
      SafeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
    }

    [Test]
    public void NewTimer_IntConstructorAndImplicitSafeContextProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;
      using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
      {
        var testCallback = new TestCallback();
        var state = new object();
        using var timer = SafeContext.Threading.NewTimer(testCallback.Callback, state, 0, 1000);

        testCallback.WaitForCallback(100);

        Assert.That(testCallback.State, Is.SameAs(state));
        SafeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
      }
    }

    [Test]
    public void NewTimer_IntConstructorAndExplicitSafeContextProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;

      var testCallback = new TestCallback();
      var state = new object();
      using var timer = SafeContext.Threading.NewTimer(safeContextStorageProvider, testCallback.Callback, state, 0, 1000);

      testCallback.WaitForCallback(100);

      Assert.That(testCallback.State, Is.SameAs(state));
      SafeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
    }

    [Test]
    public void NewTimer_LongConstructorAndImplicitSafeContextProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;
      using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
      {
        var testCallback = new TestCallback();
        var state = new object();
        using var timer = SafeContext.Threading.NewTimer(testCallback.Callback, state, 0L, 1000L);

        testCallback.WaitForCallback(100);

        Assert.That(testCallback.State, Is.SameAs(state));
        SafeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
      }
    }

    [Test]
    public void NewTimer_LongConstructorAndExplicitSafeContextProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;

      var testCallback = new TestCallback();
      var state = new object();
      using var timer = SafeContext.Threading.NewTimer(safeContextStorageProvider, testCallback.Callback, state, 0L, 1000L);

      testCallback.WaitForCallback(100);

      Assert.That(testCallback.State, Is.SameAs(state));
      SafeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
    }

    [Test]
    public void NewTimer_UIntConstructorAndImplicitSafeContextProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;
      using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
      {
        var testCallback = new TestCallback();
        var state = new object();
        using var timer = SafeContext.Threading.NewTimer(testCallback.Callback, state, 0u, 1000u);

        testCallback.WaitForCallback(100);

        Assert.That(testCallback.State, Is.SameAs(state));
        SafeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
      }
    }

    [Test]
    public void NewTimer_UIntConstructorAndExplicitSafeContextProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;

      var testCallback = new TestCallback();
      var state = new object();
      using var timer = SafeContext.Threading.NewTimer(safeContextStorageProvider, testCallback.Callback, state, 0u, 1000u);

      testCallback.WaitForCallback(100);

      Assert.That(testCallback.State, Is.SameAs(state));
      SafeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
    }
  }
}
