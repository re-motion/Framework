// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Threading;
using Remotion.Utilities;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting
{
  partial class ThreadRunner
  {
    public static void Run (ThreadStart threadStart)
    {
      ArgumentUtility.CheckNotNull("threadStart", threadStart);
      new ThreadRunner(threadStart).Run();
    }

    private readonly ThreadStart _threadStart;
    private readonly TimeSpan _timeoutTimeSpan;

    public ThreadRunner (ThreadStart threadStart)
      : this(threadStart, System.Threading.Timeout.InfiniteTimeSpan)
    {
    }

    public ThreadRunner (ThreadStart threadStart, TimeSpan timeoutTimeSpan)
    {
      ArgumentUtility.CheckNotNull("threadStart", threadStart);
      _threadStart = threadStart;
      _timeoutTimeSpan = timeoutTimeSpan;
    }

    public TimeSpan Timeout
    {
      get { return _timeoutTimeSpan; }
    }

    public void Run ()
    {
      Exception? lastException = null;
#pragma warning disable RMCORE0001
      // Use anonymous delegate to catch and store exceptions from the thread in the current scope.
      Thread otherThread =
        new Thread((ThreadStart)delegate
          {
            try
            {
              _threadStart();
            }
#if FEATURE_THREAD_ABORT
            catch (ThreadAbortException)
            {
              // Explicitely reset the ThreadAbortException
              Thread.ResetAbort();
              // Do not report exception in lastException, since aborting is expected behavior.
            }
#endif
            catch (Exception e)
            {
              lastException = e;
            }
          }
         );
#pragma warning restore RMCORE0001
      otherThread.Start();
      bool timedOut = !JoinThread(otherThread);

      // If the thread has timed out, it remains running since a Thread cannot be aborted in .NET Core.

      if (timedOut)
      {
        throw new TimeoutException(
            string.Format("The thread has not finished executing within the timeout ({0}) and was not cleaned up.", Timeout),
            lastException);
      }

      if (lastException != null)
        throw lastException; // TODO: wrap exception to preserve stack trace
    }

    protected virtual bool JoinThread (Thread otherThread)
    {
      return otherThread.Join(_timeoutTimeSpan);
    }
  }
}
