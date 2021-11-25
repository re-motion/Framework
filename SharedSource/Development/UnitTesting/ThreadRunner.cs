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

      // Use anonymous delegate to catch and store exceptions from the thread in the current scope.
      Thread otherThread =
        new Thread((ThreadStart)
          delegate
          {
            try
            {
              _threadStart();
            }
            catch (ThreadAbortException)
            {
              // Explicitely reset the ThreadAbortException
              Thread.ResetAbort();
              // Do not report exception in lastException, since aborting is expected behavior.
            }
            catch (Exception e)
            {
              lastException = e;
            }
          }
         );

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