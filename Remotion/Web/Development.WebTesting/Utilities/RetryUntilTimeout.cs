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
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Executes a given <see cref="Action"/> repeatedly (using the given retry interval) until no exception is thrown during execution or until the
  /// given timeout has been reached (in which case the final exception is rethrown).
  /// </summary>
  /// <remarks>
  /// This utility shall be used whenever JavaScript scripts may alter the DOM during the <see cref="Action"/> is executed.
  /// </remarks>
  public class RetryUntilTimeout
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (RetryUntilTimeout));

    private readonly RetryUntilTimeout<object> _retryUntilTimeout;

    public RetryUntilTimeout ([NotNull] Action action, TimeSpan timeout, TimeSpan retryInterval)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      _retryUntilTimeout = new RetryUntilTimeout<object> (
          () =>
          {
            action();
            return null;
          },
          timeout,
          retryInterval);
    }

    public void Run ()
    {
      _retryUntilTimeout.Run();
    }

    public static void Run ([NotNull] Action action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      var retryUntilTimeout = new RetryUntilTimeout (
          action,
          WebTestingConfiguration.Current.SearchTimeout,
          WebTestingConfiguration.Current.RetryInterval);
      retryUntilTimeout.Run();
    }

    public static TReturnType Run<TReturnType> ([NotNull] Func<TReturnType> func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      var retryUntilTimeout = new RetryUntilTimeout<TReturnType> (
          func,
          WebTestingConfiguration.Current.SearchTimeout,
          WebTestingConfiguration.Current.RetryInterval);
      return retryUntilTimeout.Run();
    }
  }

  /// <summary>
  /// Executes a given <see cref="Func{TReturnType}"/> repeatedly (using the given retry interval) until no exception is thrown during execution or
  /// until the given timeout has been reached (in which case the final exception is rethrown).
  /// </summary>
  /// <remarks>
  /// This utility shall be used whenever JavaScript scripts may alter the DOM during the <see cref="Func{TReturnType}"/> is executed.
  /// </remarks>
  public class RetryUntilTimeout<TReturnType>
  {
    // Todo RM-6337: Find out why DriverScope.RetryUntilTimeout is so slow.

    private static readonly ILog s_log = LogManager.GetLogger (typeof (RetryUntilTimeout<TReturnType>));

    private readonly Func<TReturnType> _func;
    private readonly TimeSpan _timeout;
    private readonly TimeSpan _retryInterval;

    public RetryUntilTimeout ([NotNull] Func<TReturnType> func, TimeSpan timeout, TimeSpan retryInterval)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      _func = func;
      _timeout = timeout;
      _retryInterval = retryInterval;
    }

    public TReturnType Run ()
    {
      var stopwatch = Stopwatch.StartNew();

      do
      {
        try
        {
          return _func();
        }
        catch (Exception ex)
        {
          if (stopwatch.ElapsedMilliseconds < _timeout.TotalMilliseconds)
          {
            s_log.Debug ("RetryUntilTimeout failed with " + ex.GetType().Name + " - trying again.");
            Thread.Sleep (_retryInterval);
          }
          else
          {
            s_log.Warn ("RetryUntilTimeout failed with " + ex.GetType().Name + " - timeout elapsed, failing.");
            throw;
          }
        }
      } while (true);
    }
  }
}