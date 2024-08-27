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
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Remotion.Context;

namespace Remotion.UnitTests.Context
{
  [TestFixture]
  [SuppressMessage("Usage", "RMCORE0001: Use Safecontext instead of typical API")]
  public class AsyncLocalStorageProviderTest
  {
    private const string c_testKey = "Foo";
    private AsyncLocalStorageProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new();
    }

    [Test]
    public void GetData_WithoutSetData_ReturnsNull ()
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
    public void FreeData ()
    {
      _provider.SetData(c_testKey, 45);
      _provider.FreeData(c_testKey);
      Assert.That(_provider.GetData(c_testKey), Is.Null);
    }

    [Test]
    public void FreeData_WithoutSetData ()
    {
      _provider.FreeData(c_testKey);
      Assert.That(_provider.GetData(c_testKey), Is.Null);
    }

    [Test]
    public void SetData_FromThreadWithNoExecutionContext_DoesNotSetDataOnMainThread ()
    {
      var t = new Thread(() => _provider.SetData(c_testKey, "1"));
      t.Start();
      t.Join();

      Assert.That(_provider.GetData(c_testKey), Is.Null);
    }

    [Test]
    public void SetData_FromThread_OverridesValueOnMainThread ()
    {
      _provider.SetData(c_testKey, "1");

      var t = new Thread(() => _provider.SetData(c_testKey, "2"));
      t.Start();
      t.Join();

      // difference to CallContext: we need a guard to prevent flow in/out
      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("2"));
    }

    [Test]
    public void SetData_FromThreadWithSafeContextBoundary_DoesNotOverrideValueOnMainThread ()
    {
      _provider.SetData(c_testKey, "1");

      var t = new Thread(() =>
      {
        using (_provider.OpenSafeContextBoundary())
        {
          _provider.SetData(c_testKey, "2");
        }
      });
      t.Start();
      t.Join();

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("1"));
    }

    [Test]
    public void SetData_FromThreadWithUnclosedSafeContextBoundary_DoesNotOverrideValueOnMainThread ()
    {
      _provider.SetData(c_testKey, "1");

      var t = new Thread(() =>
      {
        _provider.OpenSafeContextBoundary();
        _provider.SetData(c_testKey, "2");
      });
      t.Start();
      t.Join();

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("1"));
    }

    [Test]
    public void GetData_FromThread_CanAccessValuesFromMainThread ()
    {
      _provider.SetData(c_testKey, "1");

      object result = null;
      var t = new Thread(() => result = _provider.GetData(c_testKey));
      t.Start();
      t.Join();

      // difference to CallContext: we need a guard to prevent flow in/out
      Assert.That(result, Is.EqualTo("1"));
    }

    [Test]
    public void GetData_FromThreadWithSafeContextBoundary_CannotAccessValuesFromMainThread ()
    {
      _provider.SetData(c_testKey, "1");

      object result = null;
      var t = new Thread(() =>
      {
        using (_provider.OpenSafeContextBoundary())
        {
          result = _provider.GetData(c_testKey);
        }
      });
      t.Start();
      t.Join();

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetData_FromThreadWithUnclosedSafeContextBoundary_CannotAccessValuesFromMainThread ()
    {
      _provider.SetData(c_testKey, "1");

      object result = null;
      var t = new Thread(() =>
      {
        _provider.OpenSafeContextBoundary();
        result = _provider.GetData(c_testKey);
      });
      t.Start();
      t.Join();

      Assert.That(result, Is.Null);
    }

    [Test]
    public void SetData_FromThreadPoolThread_IsResetWhenThreadIsReused ()
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

        if (!resetEvent.WaitOne(TimeSpan.FromSeconds(1)))
          Assert.Fail("Queued thread pool work item did not complete within the expected time.");
      }

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetData_InExecutionContextRunWithSameExecutionContext_CanAccessValuesFromPrevious ()
    {
      _provider.SetData(c_testKey, 34);

      var ec = ExecutionContext.Capture();
      Assert.That(ec, Is.Not.Null);

      ExecutionContext.Run(
          ec,
          _ =>
          {
            // difference to CallContext: we need a guard to prevent flow in/out
            Assert.That(_provider.GetData(c_testKey), Is.EqualTo(34));
          },
          null);
    }

    [Test]
    public void GetData_InExecutionContextRunWithSameExecutionContextAndSafeContextBoundary_CannotAccessValuesFromPrevious ()
    {
      _provider.SetData(c_testKey, 34);

      var ec = ExecutionContext.Capture();
      Assert.That(ec, Is.Not.Null);

      ExecutionContext.Run(
          ec,
          _ =>
          {
            using (_provider.OpenSafeContextBoundary())
            {
              Assert.That(_provider.GetData(c_testKey), Is.Null);
            }
          },
          null);
    }

    [Test]
    public void GetData_InExecutionContextRunWithSameExecutionContextAndUnclosedSafeContextBoundary_CannotAccessValuesFromPrevious ()
    {
      _provider.SetData(c_testKey, 34);

      var ec = ExecutionContext.Capture();
      Assert.That(ec, Is.Not.Null);

      ExecutionContext.Run(
          ec,
          _ =>
          {
            _provider.OpenSafeContextBoundary();
            Assert.That(_provider.GetData(c_testKey), Is.Null);
          },
          null);
    }

    [Test]
    public void SetData_InExecutionContextRunWithDifferentExecutionContext_GetsNewerVersionOfValue ()
    {
      _provider.SetData(c_testKey, 12);

      var ec = ExecutionContext.Capture();
      Assert.That(ec, Is.Not.Null);

      _provider.SetData(c_testKey, 34);

      ExecutionContext.Run(
          ec,
          _ =>
          {
            // difference to CallContext: we need a guard to prevent flow in/out
            Assert.That(_provider.GetData(c_testKey), Is.EqualTo(34));
          },
          null);
    }

    [Test]
    public void SetData_InExecutionContextRunWithDifferentExecutionContextAndSafeContextBoundary_CannotAccessOuterValue ()
    {
      _provider.SetData(c_testKey, 12);

      var ec = ExecutionContext.Capture();
      Assert.That(ec, Is.Not.Null);

      _provider.SetData(c_testKey, 34);

      ExecutionContext.Run(
          ec,
          _ =>
          {
            using (_provider.OpenSafeContextBoundary())
            {
              Assert.That(_provider.GetData(c_testKey), Is.Null);
            }
          },
          null);
    }

    [Test]
    public void SetData_InExecutionContextRunWithDifferentExecutionContextAndUnclosedSafeContextBoundary_CannotAccessOuterValue ()
    {
      _provider.SetData(c_testKey, 12);

      var ec = ExecutionContext.Capture();
      Assert.That(ec, Is.Not.Null);

      _provider.SetData(c_testKey, 34);

      ExecutionContext.Run(
          ec,
          _ =>
          {
            _provider.OpenSafeContextBoundary();
            Assert.That(_provider.GetData(c_testKey), Is.Null);
          },
          null);
    }

    [Test]
    public void SetData_InExecutionContextRunWithNullExecutionContext_CannotAccessValues ()
    {
      var ec = ExecutionContext.Capture();
      Assert.That(ec, Is.Not.Null);

      _provider.SetData(c_testKey, 34);

      ExecutionContext.Run(
          ec,
          _ => { Assert.That(_provider.GetData(c_testKey), Is.Null); },
          null);
    }

    [Test]
    public void SetData_InExecutionContextRunWithSameContext_ChangesValuesInOuterExecutionContext ()
    {
      _provider.SetData(c_testKey, 34);

      var ec = ExecutionContext.Capture();
      Assert.That(ec, Is.Not.Null);

      ExecutionContext.Run(
          ec,
          _ => { _provider.SetData(c_testKey, 123); },
          null);

      // difference to CallContext: we need a guard to prevent flow in/out
      Assert.That(_provider.GetData(c_testKey), Is.EqualTo(123));
    }

    [Test]
    public void SetData_InExecutionContextRunWithSameContextAndSafeContextBoundary_DoesNotChangeValuesInOuterExecutionContext ()
    {
      _provider.SetData(c_testKey, 34);

      var ec = ExecutionContext.Capture();
      Assert.That(ec, Is.Not.Null);

      ExecutionContext.Run(
          ec,
          _ =>
          {
            using (_provider.OpenSafeContextBoundary())
            {
              _provider.SetData(c_testKey, 123);
            }
          },
          null);

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo(34));
    }

    [Test]
    public void SetData_InExecutionContextRunWithSameContextAndUnclosedSafeContextBoundary_DoesNotChangeValuesInOuterExecutionContext ()
    {
      _provider.SetData(c_testKey, 34);

      var ec = ExecutionContext.Capture();
      Assert.That(ec, Is.Not.Null);

      ExecutionContext.Run(
          ec,
          _ =>
          {
            _provider.OpenSafeContextBoundary();
            _provider.SetData(c_testKey, 123);
          },
          null);

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo(34));
    }

    [Test]
    public void SetData_InExecutionContextRunWithDifferentContext_ChangesValuesInOuterNewerExecutionContext ()
    {
      _provider.SetData(c_testKey, 12);

      var ec = ExecutionContext.Capture();
      Assert.That(ec, Is.Not.Null);

      _provider.SetData(c_testKey, 34);

      ExecutionContext.Run(
          ec,
          _ => { _provider.SetData(c_testKey, 123); },
          null);

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo(123));
    }

    [Test]
    public void SetData_InExecutionContextRunWithNullContext_DoesNotChangeValueInOuterExecutionContext ()
    {
      var ec = ExecutionContext.Capture();

      _provider.SetData(c_testKey, "1");

      ExecutionContext.Run(
          ec,
          _ => { _provider.SetData(c_testKey, "2"); },
          null);

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("1"));
    }

    [Test]
    public void SuppressFlowWorksCorrectly ()
    {
      var ec = ExecutionContext.Capture().CreateCopy();

      try
      {
        ExecutionContext.SuppressFlow();

        _provider.SetData(c_testKey, "1");

        ExecutionContext.Run(
            ec,
            _ => { Assert.That(_provider.GetData(c_testKey), Is.Null); },
            null);
      }
      finally
      {
        ExecutionContext.RestoreFlow();
      }

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("1"));
    }

    [Test]
    public void GetData_InTaskRun_ReturnsValueFromOuterContext ()
    {
      _provider.SetData(c_testKey, 34);

      object result = null;

      Task.Run(
          () => { result = _provider.GetData(c_testKey); }).GetAwaiter().GetResult();

      // difference to CallContext: we need a guard to prevent flow in/out
      Assert.That(result, Is.EqualTo(34));
    }

    [Test]
    public void GetData_InTaskRunWithSafeContextBoundary_DoesNotReturnValueFromOuterContext ()
    {
      _provider.SetData(c_testKey, 34);

      object result = null;

      Task.Run(
          () =>
          {
            using (_provider.OpenSafeContextBoundary())
            {
              result = _provider.GetData(c_testKey);
            }
          }).GetAwaiter().GetResult();

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetData_InTaskRunWithUnclosedSafeContextBoundary_DoesNotReturnValueFromOuterContext ()
    {
      _provider.SetData(c_testKey, 34);

      object result = null;

      Task.Run(
          () =>
          {
            _provider.OpenSafeContextBoundary();
            result = _provider.GetData(c_testKey);
          }).GetAwaiter().GetResult();

      Assert.That(result, Is.Null);
    }

    [Test]
    public void SetData_InTaskRun_SetsValueInOuterExecutionContext ()
    {
      _provider.SetData(c_testKey, 34);

      Task.Run(
          () => { _provider.SetData(c_testKey, 129); }).GetAwaiter().GetResult();

      // difference to CallContext: we need a guard to prevent flow in/out
      Assert.That(_provider.GetData(c_testKey), Is.EqualTo(129));
    }

    [Test]
    public void SetData_InTaskRunWithSafeContextBoundary_DoesNotSetValueInOuterExecutionContext ()
    {
      _provider.SetData(c_testKey, 34);

      Task.Run(
          () =>
          {
            using (_provider.OpenSafeContextBoundary())
            {
              _provider.SetData(c_testKey, 129);
            }
          }).GetAwaiter().GetResult();

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo(34));
    }

    [Test]
    public void SetData_InTaskRunWithUnclosedSafeContextBoundary_DoesNotSetValueInOuterExecutionContext ()
    {
      _provider.SetData(c_testKey, 34);

      Task.Run(
          () =>
          {
            _provider.OpenSafeContextBoundary();
            _provider.SetData(c_testKey, 129);
          }).GetAwaiter().GetResult();

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo(34));
    }

    [Test]
    public async Task Async ()
    {
      _provider.SetData(c_testKey, "1");

      await AsyncTask();

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("3"));
    }

    private async Task AsyncTask ()
    {
      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("1"));

      _provider.SetData(c_testKey, "2");

      await Task.Delay(10);

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("2"));

      _provider.SetData(c_testKey, "3");
    }

    [Test]
    public void GetData_InParallelForeach_ReturnsValueFromOuterScope ()
    {
      _provider.SetData(c_testKey, "initial");

      Parallel.For(
          0,
          100,
          _ => { Assert.That(_provider.GetData(c_testKey), Is.EqualTo("initial")); });

      // difference to CallContext: we need a guard to prevent flow in/out
      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("initial"));
    }

    [Test]
    public void GetData_InParallelForeachWithSafeContextBoundary_DoesNotReturnValueFromOuterScope ()
    {
      _provider.SetData(c_testKey, "initial");

      Parallel.For(
          0,
          100,
          _ =>
          {
            using (_provider.OpenSafeContextBoundary())
            {
              Assert.That(_provider.GetData(c_testKey), Is.Null);
            }
          });

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("initial"));
    }

    [Test]
    public void GetData_InParallelForeachWithUnclosedSafeContextBoundary_DoesNotReturnValueFromOuterScope ()
    {
      _provider.SetData(c_testKey, "initial");

      Parallel.For(
          0,
          100,
          _ =>
          {
            _provider.OpenSafeContextBoundary();
            Assert.That(_provider.GetData(c_testKey), Is.Null);
          });

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("initial"));
    }

    [Test]
    public void SetData_InParallelForeach_SetsValueForOuterScope ()
    {
      _provider.SetData(c_testKey, "initial");

      Parallel.For(
          0,
          100,
          v => { _provider.SetData(c_testKey, v); });

      // difference to CallContext: we need a guard to prevent flow in/out
      Assert.That(_provider.GetData(c_testKey), Is.TypeOf<int>());
    }

    [Test]
    public void SetData_InParallelForeachWithSafeContextBoundary_DoesNotSetValueForOuterScope ()
    {
      _provider.SetData(c_testKey, "initial");

      Parallel.For(
          0,
          100,
          v =>
          {
            using (_provider.OpenSafeContextBoundary())
            {
              _provider.SetData(c_testKey, v);
            }
          });

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("initial"));
    }

    [Test]
    public void SetData_InParallelForeachWithUnclosedSafeContextBoundary_DoesNotSetValueForOuterScope ()
    {
      _provider.SetData(c_testKey, "initial");

      Parallel.For(
          0,
          100,
          v =>
          {
            _provider.OpenSafeContextBoundary();
            _provider.SetData(c_testKey, v);
          });

      Assert.That(_provider.GetData(c_testKey), Is.EqualTo("initial"));
    }
  }
}
