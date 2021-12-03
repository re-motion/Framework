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
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Remotion.Context;
#if NETFRAMEWORK
using System.Runtime.Remoting.Messaging;
#endif

// Ignore the obsolete warnings for AsyncLocalStorageProvider
#pragma warning disable 618

namespace Remotion.UnitTests.Context
{
  [TestFixture]
  public class AsyncLocalStorageProviderTest
  {
    private const string c_testKey = "Foo";
    private const string c_testKey2 = "Foo2";
    private AsyncLocalStorageProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new();
      _provider.FreeData(c_testKey);
      _provider.FreeData(c_testKey2);
#if NETFRAMEWORK
      CallContext.FreeNamedDataSlot(c_testKey);
      CallContext.FreeNamedDataSlot(c_testKey2);
#endif
    }

    [TearDown]
    public void TearDown ()
    {
      _provider.FreeData(c_testKey);
      _provider.FreeData(c_testKey2);
#if NETFRAMEWORK
      CallContext.FreeNamedDataSlot(c_testKey);
      CallContext.FreeNamedDataSlot(c_testKey2);
#endif
    }

    [Test]
    public void GetData_WithoutValue ()
    {
      Assert.That(_provider.GetData(c_testKey), Is.Null);
    }

    [Test]
    public void SetData ()
    {
      _provider.SetData(c_testKey, 45);
      Assert.That(_provider.GetData(c_testKey), Is.EqualTo(45));
    }

    [Test]
    public void SetData_Null ()
    {
      _provider.SetData(c_testKey, 45);
      _provider.SetData(c_testKey, null);
      Assert.That(_provider.GetData(c_testKey), Is.Null);
    }

    [Test]
    public void FreeData ()
    {
      _provider.SetData(c_testKey, 45);
      _provider.FreeData(c_testKey);
      Assert.That(_provider.GetData(c_testKey), Is.Null);
    }

    [Test]
    public void FreeData_WithoutValue ()
    {
      _provider.FreeData(c_testKey);
      Assert.That(_provider.GetData(c_testKey), Is.Null);
    }

    [Test]
    public void SetData_FromOtherThread_NotAccessible ()
    {
      var t = new Thread(() => _provider.SetData(c_testKey, "1"));
      t.Start();
      t.Join();

      Assert.That(_provider.GetData(c_testKey), Is.Null);
    }

    [Test]
    public void SetData_FromOtherThread_DoesNotOverride ()
    {
      _provider.SetData(c_testKey, "1");

      var t = new Thread(() => _provider.SetData(c_testKey, "2"));
      t.Start();
      t.Join();

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("1"));
    }

    [Test]
    public void SetData_OnThreadPoolThread_IsResetForNextWorkItem ()
    {
      object result = string.Empty;

      using var resetEvent = new AutoResetEvent(false);

      var visitedThreadPoolThreads = new HashSet<int>();
      var completed = false;

      while (!completed)
      {
        ThreadPool.QueueUserWorkItem(
            _ =>
            {
              var threadPoolThreadId = Thread.CurrentThread.ManagedThreadId;
              var didNotVisitThreadPoolThreadYet = visitedThreadPoolThreads.Add(threadPoolThreadId);

              if (didNotVisitThreadPoolThreadYet)
              {
                _provider.SetData(c_testKey, "1");
              }
              else
              {
                result = _provider.GetData(c_testKey);
                completed = true;
              }

              Thread.MemoryBarrier();
              resetEvent.Set();
            });

        if (!resetEvent.WaitOne(TimeSpan.FromSeconds(3)))
          Assert.Fail("Queued thread pool work item did not complete within the expected time.");
      }

      Assert.That(result, Is.Null);
    }

    [Test]
    public void SetValuesAreRestoredWhenReturningToExecutionContext ()
    {
      _provider.SetData(c_testKey, 34);
#if NETFRAMEWORK
      CallContext.SetData(c_testKey, 34);
#endif

      var ec = ExecutionContext.Capture();
      Assert.That(ec, Is.Not.Null);

      ExecutionContext.Run(
          ec,
          _ =>
          {
            // difference: In CallContext this would be a new scope but we cannot differentiate as there is no event fired
            Assert.That(_provider.GetData(c_testKey), Is.EqualTo(34));
#if NETFRAMEWORK
            Assert.That(CallContext.GetData(c_testKey), Is.Null);
#endif

            _provider.SetData(c_testKey, 123);
          },
          null);

      // difference: In CallContext this would restore the original context but since we got no notification for the initial change
      // we cannot detect that we are restoring an old the data is now inaccessible
      Assert.That(_provider.GetData(c_testKey)!, Is.Null);
#if NETFRAMEWORK
      Assert.That(CallContext.GetData(c_testKey), Is.EqualTo(34));
#endif
    }

    [Test]
    public void SetValuesAreRestoredInRunExecutionContext ()
    {
      _provider.SetData(c_testKey, 34);
#if NETFRAMEWORK
      CallContext.SetData(c_testKey, 34);
#endif

      var ec = ExecutionContext.Capture();
      Assert.That(ec, Is.Not.Null);

#if NETFRAMEWORK
#endif
      ExecutionContext.Run(
          ec,
          _ =>
          {
            // difference: In CallContext the delegate would run in a new context, but this is not possibles
            // as there is notification indicating a context switch
            Assert.That(_provider.GetData(c_testKey), Is.EqualTo(34));
#if NETFRAMEWORK
            Assert.That(CallContext.GetData(c_testKey), Is.Null);
#endif
          },
          null);
    }

    [Test]
    public void SetDataInExecutionContextRunDoesNotAffectOuterScope ()
    {
      var ec = ExecutionContext.Capture().CreateCopy();

      _provider.SetData(c_testKey, "1");
#if NETFRAMEWORK
      CallContext.SetData(c_testKey, "1");
#endif

      ExecutionContext.Run(
          ec,
          _ =>
          {
            _provider.SetData(c_testKey, "2");
#if NETFRAMEWORK
            CallContext.SetData(c_testKey, "2");
#endif
          },
          null);

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("1"));
#if NETFRAMEWORK
      Assert.That(CallContext.GetData(c_testKey), Is.EqualTo("1"));
#endif
    }

    [Test]
    public void SuppressFlowWorksCorrectly ()
    {
      var ec = ExecutionContext.Capture().CreateCopy();

      try
      {
        ExecutionContext.SuppressFlow();

        _provider.SetData(c_testKey, "1");
#if NETFRAMEWORK
        CallContext.SetData(c_testKey, "1");
#endif

        ExecutionContext.Run(
            ec,
            _ =>
            {
              Assert.That(_provider.GetData(c_testKey), Is.Null);
#if NETFRAMEWORK
              Assert.That(CallContext.GetData(c_testKey), Is.Null);
#endif
            },
            null);
      }
      finally
      {
        ExecutionContext.RestoreFlow();
      }

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("1"));
#if NETFRAMEWORK
      Assert.That(CallContext.GetData(c_testKey), Is.EqualTo("1"));
#endif
    }

    [Test]
    public void FlowsToThread ()
    {
      _provider.SetData(c_testKey, 34);

      object result = null;
#if NETFRAMEWORK
      object result2 = null;
#endif
      var thread = new Thread(
          _ =>
          {
            result = _provider.GetData(c_testKey);
#if NETFRAMEWORK
            result2 = CallContext.GetData(c_testKey);
#endif
          });
      thread.Start();
      thread.Join();

      Assert.That(result, Is.Null);
#if NETFRAMEWORK
      Assert.That(result2, Is.Null);
#endif
    }

    [Test]
    public void FlowsWithTaskRun ()
    {
      _provider.SetData(c_testKey, 34);

      object result = null;
#if NETFRAMEWORK
      object result2 = null;
#endif

      Task.Run(
          () =>
          {
            result = _provider.GetData(c_testKey);
#if NETFRAMEWORK
            result2 = CallContext.GetData(c_testKey);
#endif
          }).GetAwaiter().GetResult();

      Assert.That(result, Is.Null);
#if NETFRAMEWORK
      Assert.That(result2, Is.Null);
#endif
    }

    [Test]
    public async Task Async ()
    {
      _provider.SetData(c_testKey, "1");
#if NETFRAMEWORK
      CallContext.SetData(c_testKey, "1");
#endif

      await AsyncTask();

      Assert.That(_provider.GetData(c_testKey), Is.Null);
#if NETFRAMEWORK
      Assert.That(CallContext.GetData(c_testKey), Is.Null);
#endif
    }

    private async Task AsyncTask ()
    {
      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("1"));
#if NETFRAMEWORK
      Assert.That(CallContext.GetData(c_testKey), Is.EqualTo("1"));
#endif

      _provider.SetData(c_testKey, "2");

      await Task.Delay(10);

      Assert.That(_provider.GetData(c_testKey), Is.Null);
#if NETFRAMEWORK
      Assert.That(CallContext.GetData(c_testKey), Is.Null);
#endif

      _provider.SetData(c_testKey, "3");
    }

#if NETFRAMEWORK && DEBUG
    [Test]
    public void SetData_WithClassImplementingILogicalThreadAffinative_ThrowsPlatformNotSupportedException ()
    {
      var data = new ClassImplementingILogicalThreadAffinative();
      Assert.That(
          () => _provider.SetData(c_testKey, data),
          Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo("Remoting is not supported."));
    }

    private class ClassImplementingILogicalThreadAffinative : System.Runtime.Remoting.Messaging.ILogicalThreadAffinative
    {
    }
#endif
  }
}
