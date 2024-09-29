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
using Microsoft.Extensions.Logging;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Executes a given <see cref="Action"/> repeatedly (using the given retry interval) until no exception is thrown during execution or until the
  /// given timeout has been reached (in which case the last exception is rethrown).
  /// </summary>
  /// <remarks>
  /// This utility shall be used whenever JavaScript scripts may alter the DOM during the <see cref="Action"/> is executed.
  /// </remarks>
  public class RetryUntilTimeout
  {
    private readonly RetryUntilTimeout<object?> _retryUntilTimeout;

    public RetryUntilTimeout ([NotNull] ILogger logger, [NotNull] Action action, TimeSpan timeout, TimeSpan retryInterval)
    {
      ArgumentUtility.CheckNotNull("action", action);

      _retryUntilTimeout = new RetryUntilTimeout<object?>(
          logger,
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

    /// <summary>
    /// Executes a given <see cref="Action"/> repeatedly (using the given retry interval) until no exception is thrown during execution or until the
    /// given timeout has been reached (in which case the last exception is rethrown).
    /// </summary>
    /// <param name="logger">
    /// The <see cref="ILogger"/> used by the web testing infrastructure for diagnostic output. The <paramref name="logger"/> can be retrieved from
    /// <see cref="WebTestObject{TWebTestObjectContext}"/>.<see cref="WebTestObject{TWebTestObjectContext}.Logger"/>.
    /// </param>
    /// <param name="action">The <see cref="Action"/> to be executed repeatedly.</param>
    /// <param name="timeout">
    /// The timeout after which no more retries are made and the last exception is rethrown. Defaults to the value of the <see cref="DriverConfiguration.SearchTimeout"/>
    /// property of the <see cref="DriverConfiguration"/> retrieved by calling <see cref="WebTestConfigurationFactory.CreateDriverConfiguration"/> if no value is provided.
    /// </param>
    /// <param name="retryInterval">
    /// The interval to wait between two executions. Defaults to the value of the <see cref="DriverConfiguration.RetryInterval"/> property of the
    /// <see cref="DriverConfiguration"/> retrieved by calling <see cref="WebTestConfigurationFactory.CreateDriverConfiguration"/> if no value is provided.
    /// </param>
    public static void Run ([NotNull] ILogger logger, [NotNull] Action action, TimeSpan? timeout = null, TimeSpan? retryInterval = null)
    {
      ArgumentUtility.CheckNotNull("action", action);
      var configuration = new WebTestConfigurationFactory().CreateDriverConfiguration();

      var retryUntilTimeout = new RetryUntilTimeout(
          logger,
          action,
          timeout ?? configuration.SearchTimeout,
          retryInterval ?? configuration.RetryInterval);
      retryUntilTimeout.Run();
    }

    /// <summary>
    /// Executes a given <see cref="Func{TReturnType}"/> repeatedly (using the given retry interval) until no exception is thrown during execution or
    /// until the given timeout has been reached (in which case the last exception is rethrown).
    /// </summary>
    /// <param name="logger">
    /// The <see cref="ILogger"/> used by the web testing infrastructure for diagnostic output. The <paramref name="logger"/> can be retrieved from
    /// <see cref="WebTestObject{TWebTestObjectContext}"/>.<see cref="WebTestObject{TWebTestObjectContext}.Logger"/>.
    /// </param>
    /// <param name="func">The <see cref="Func{TReturnType}"/> to be executed repeatedly.</param>
    /// <param name="timeout">
    /// The timeout after which no more retries are made and the last exception is rethrown. Defaults to the value of the <see cref="DriverConfiguration.SearchTimeout"/>
    /// property of the <see cref="DriverConfiguration"/> retrieved by calling <see cref="WebTestConfigurationFactory.CreateDriverConfiguration"/> if no value is provided.
    /// </param>
    /// <param name="retryInterval">
    /// The interval to wait between two executions. Defaults to the value of the <see cref="DriverConfiguration.RetryInterval"/> property of the
    /// <see cref="DriverConfiguration"/> retrieved by calling <see cref="WebTestConfigurationFactory.CreateDriverConfiguration"/> if no value is provided.
    /// </param>
    /// <returns>Returns the <typeparamref name="TReturnType"/> object returned by <paramref name="func"/>.</returns>
    public static TReturnType Run<TReturnType> ([NotNull] ILogger logger, [NotNull] Func<TReturnType> func, TimeSpan? timeout = null, TimeSpan? retryInterval = null)
    {
      ArgumentUtility.CheckNotNull("func", func);
      var configuration = new WebTestConfigurationFactory().CreateDriverConfiguration();

      var retryUntilTimeout = new RetryUntilTimeout<TReturnType>(
          logger,
          func,
          timeout ?? configuration.SearchTimeout,
          retryInterval ?? configuration.RetryInterval);
      return retryUntilTimeout.Run();
    }
  }

  /// <summary>
  /// Executes a given <see cref="Func{TReturnType}"/> repeatedly (using the given retry interval) until no exception is thrown during execution or
  /// until the given timeout has been reached (in which case the last exception is rethrown).
  /// </summary>
  /// <remarks>
  /// This utility shall be used whenever JavaScript scripts may alter the DOM during the <see cref="Func{TReturnType}"/> is executed.
  /// </remarks>
  public class RetryUntilTimeout<TReturnType>
  {
    // Todo RM-6337: Find out why DriverScope.RetryUntilTimeout is so slow.

    private readonly ILogger _logger;
    private readonly Func<TReturnType> _func;
    private readonly TimeSpan _timeout;
    private readonly TimeSpan _retryInterval;

    public RetryUntilTimeout ([NotNull] ILogger logger, [NotNull] Func<TReturnType> func, TimeSpan timeout, TimeSpan retryInterval)
    {
      ArgumentUtility.CheckNotNull("func", func);
      ArgumentUtility.CheckNotNull("logger", logger);

      _logger = logger;
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
            _logger.LogDebug("RetryUntilTimeout failed with " + ex.GetType().Name + " - trying again.");
            Thread.Sleep(_retryInterval);
          }
          else
          {
            _logger.LogWarning("RetryUntilTimeout failed with " + ex.GetType().Name + " - timeout elapsed, failing.");
            throw;
          }
        }
      } while (true);
    }
  }
}
