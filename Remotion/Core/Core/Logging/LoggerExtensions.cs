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
using IMicrosoftLogger = Microsoft.Extensions.Logging.ILogger;
using MicrosoftLogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Remotion.Logging
{
  /// <summary>
  /// Provides extension methods used for logging.
  /// </summary>
  public static class LoggerExtensions
  {
    /// <summary>
    /// Logs the given value and returns it to the caller. This is typically used to logger a value returned by a method directly in the return
    /// statement.
    /// </summary>
    /// <typeparam name="T">The (inferred) type of the value to be logged.</typeparam>
    /// <param name="value">The value to be logged.</param>
    /// <param name="logger">The <see cref="IMicrosoftLogger"/> to logger the value with.</param>
    /// <param name="logLevel">The <see cref="MicrosoftLogLevel"/> to logger the value at. If the <paramref name="logger"/> does not support this level, the
    /// <paramref name="messageCreator"/> is not called.</param>
    /// <param name="messageCreator">A function object building the message to be logged.</param>
    /// <returns>The <paramref name="value"/> passed in to the method.</returns>
    public static T LogAndReturnValue<T> (this T value, IMicrosoftLogger logger, MicrosoftLogLevel logLevel, Func<T, string?> messageCreator)
    {
      if (logger.IsEnabled(logLevel))
      {
        logger.Log(logLevel, messageCreator(value));
      }

      return value;
    }

    public static IEnumerable<T> LogAndReturnItems<T> (
        this IEnumerable<T> sequence,
        IMicrosoftLogger logger,
        MicrosoftLogLevel logLevel,
        Func<int, string?> iterationCompletedMessageCreator)
    {
      if (logger.IsEnabled(logLevel))
        return LogAndReturnWithIteration(sequence, logger, logLevel, iterationCompletedMessageCreator);
      return sequence;
    }

    private static IEnumerable<T> LogAndReturnWithIteration<T> (
        IEnumerable<T> sequence,
        IMicrosoftLogger logger,
        MicrosoftLogLevel logLevel,
        Func<int, string?> iterationCompletedMessageCreator)
    {
      int count = 0;
      foreach (var item in sequence)
      {
        count++;
        yield return item;
      }

      logger.Log(logLevel, iterationCompletedMessageCreator(count));
    }
  }
}
