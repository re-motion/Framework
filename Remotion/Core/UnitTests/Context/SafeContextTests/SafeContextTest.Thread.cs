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
using System.Runtime.ExceptionServices;
using System.Threading;
using NUnit.Framework;
using Remotion.Context;

namespace Remotion.UnitTests.Context.SafeContextTests
{
  [TestFixture]
  public class SafeContextThreadTest : SafeContextTestBase
  {
    private const int c_maxStackSize = 4096;

    [Test]
    public void New_WithCallbackAndImplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = _safeContextProviderMock.Object;
      using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
        RunThreadStartTest(SafeContext.Thread.New);
    }

    [Test]
    public void New_WithCallbackAndExplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = _safeContextProviderMock.Object;
      RunThreadStartTest(start => SafeContext.Thread.New(safeContextStorageProvider, start));
    }

    [Test]
    public void New_WithCallbackAndImplicitSafeContextStorageProviderAndMaxStackSize_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = _safeContextProviderMock.Object;
      using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
        RunThreadStartTest(start => SafeContext.Thread.New(start, c_maxStackSize));
    }

    [Test]
    public void New_WithCallbackAndExplicitSafeContextStorageProviderAndMaxStackSize_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = _safeContextProviderMock.Object;
      RunThreadStartTest(start => SafeContext.Thread.New(safeContextStorageProvider, start, c_maxStackSize));
    }

    [Test]
    public void New_WithParameterizedCallbackAndImplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = _safeContextProviderMock.Object;
      using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
        RunParameterizedThreadStartTest(SafeContext.Thread.New);
    }

    [Test]
    public void New_WithParameterizedCallbackAndExplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = _safeContextProviderMock.Object;
      RunParameterizedThreadStartTest(start => SafeContext.Thread.New(safeContextStorageProvider, start));
    }

    [Test]
    public void New_WithParameterizedCallbackAndImplicitSafeContextStorageProviderAndMaxStackSize_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = _safeContextProviderMock.Object;
      using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
        RunParameterizedThreadStartTest(start => SafeContext.Thread.New(start, c_maxStackSize));
    }

    [Test]
    public void New_WithParameterizedCallbackAndExplicitSafeContextStorageProviderAndMaxStackSize_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = _safeContextProviderMock.Object;
      RunParameterizedThreadStartTest(start => SafeContext.Thread.New(safeContextStorageProvider, start, c_maxStackSize));
    }

    private void RunThreadStartTest (Func<ThreadStart, Thread> threadFactory)
    {
      ExceptionDispatchInfo exceptionDispatchInfo = null;
      var delegateExecuted = false;

      var thread = threadFactory(ThreadMain);
      thread.Start();
      thread.Join();

      exceptionDispatchInfo?.Throw();
      Assert.That(delegateExecuted, Is.True);

      void ThreadMain ()
      {
        try
        {
          _safeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
          delegateExecuted = true;
        }
        catch (Exception ex)
        {
          exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
        }
      }
    }

    private void RunParameterizedThreadStartTest (Func<ParameterizedThreadStart, Thread> threadFactory)
    {
      var parameter = new object();
      ExceptionDispatchInfo exceptionDispatchInfo = null;
      var delegateExecuted = false;

      var thread = threadFactory(ThreadMain);
      thread.Start(parameter);
      thread.Join();

      exceptionDispatchInfo?.Throw();
      Assert.That(delegateExecuted, Is.True);

      void ThreadMain (object v)
      {
        try
        {
          Assert.That(v, Is.EqualTo(parameter));
          _safeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
          delegateExecuted = true;
        }
        catch (Exception ex)
        {
          exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
        }
      }
    }
  }
}
