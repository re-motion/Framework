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

namespace Remotion.UnitTests.Context
{
  [TestFixture]
  public class AsyncLocalStorageProviderTest
  {
    private const string c_testKey = "Foo";
    private AsyncLocalStorageProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new();
      _provider.FreeData (c_testKey);
    }

    [TearDown]
    public void TearDown ()
    {
      _provider.FreeData (c_testKey);
    }

    [Test]
    public void GetData_WithoutValue ()
    {
      Assert.That (_provider.GetData (c_testKey), Is.Null);
    }

    [Test]
    public void SetData ()
    {
      _provider.SetData (c_testKey, 45);
      Assert.That (_provider.GetData (c_testKey), Is.EqualTo (45));
    }

    [Test]
    public void SetData_Null ()
    {
      _provider.SetData (c_testKey, 45);
      _provider.SetData (c_testKey, null);
      Assert.That (_provider.GetData (c_testKey), Is.Null);
    }

    [Test]
    public void FreeData ()
    {
      _provider.SetData (c_testKey, 45);
      _provider.FreeData (c_testKey);
      Assert.That (_provider.GetData (c_testKey), Is.Null);
    }

    [Test]
    public void FreeData_WithoutValue ()
    {
      _provider.FreeData (c_testKey);
      Assert.That (_provider.GetData (c_testKey), Is.Null);
    }

    [Test]
    public void SetData_FromOtherThread_NotAccessible ()
    {
      var t = new Thread (() => _provider.SetData (c_testKey, "1"));
      t.Start();
      t.Join();

      Assert.That (_provider.GetData (c_testKey), Is.Null);
    }

    [Test]
    public void SetData_FromOtherThread_DoesNotOverride ()
    {
      _provider.SetData (c_testKey, "1");

      var t = new Thread (() => _provider.SetData (c_testKey, "2"));
      t.Start();
      t.Join();

      Assert.That (_provider.GetData (c_testKey), Is.EqualTo("1"));
    }

    [Test]
    public void SetData_OnThreadPoolThread_IsResetForNextWorkItem ()
    {
      object result = string.Empty;
      int threadId1 = -1;
      int threadId2 = -2;
      ThreadPool.GetMinThreads (out var minWorkerThreads, out var minCompletionPortThreads);
      ThreadPool.GetMaxThreads (out var maxWorkerThreads, out var maxCompletionPortThreads);
      ThreadPool.GetAvailableThreads (out var availableWorkerThreads, out _);
      try
      {
        ThreadPool.SetMinThreads (1, 0);
        ThreadPool.SetMaxThreads (maxWorkerThreads - availableWorkerThreads + 1, 0);
        using var resetEvent = new AutoResetEvent (false);

        ThreadPool.QueueUserWorkItem (
            _ =>
            {
              threadId1 = Thread.CurrentThread.ManagedThreadId;
              _provider.SetData (c_testKey, "1");
              resetEvent.Set();
            });
        resetEvent.WaitOne();
        ThreadPool.QueueUserWorkItem (
            _ =>
            {
              threadId2 = Thread.CurrentThread.ManagedThreadId;
              result = _provider.GetData (c_testKey);
              resetEvent.Set();
            });
        resetEvent.WaitOne();
      }
      finally
      {
        ThreadPool.SetMaxThreads (maxWorkerThreads, maxCompletionPortThreads);
        ThreadPool.SetMinThreads (minWorkerThreads, minCompletionPortThreads);
      }

      Assert.That (result, Is.Null);
      Assert.That (threadId1, Is.EqualTo(threadId2));
    }

#if NETFRAMEWORK && DEBUG

    [Test]
    public void SetData_WithClassImplementingILogicalThreadAffinative_ThrowsPlatformNotSupportedException ()
    {
      var data = new ClassImplementingILogicalThreadAffinative();
      Assert.That (
          () => _provider.SetData (c_testKey, data),
          Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo ("Remoting is not supported."));
    }

    private class ClassImplementingILogicalThreadAffinative : System.Runtime.Remoting.Messaging.ILogicalThreadAffinative
    {
    }

#endif
  }
}
