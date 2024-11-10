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
using Microsoft.Extensions.Logging;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Logging
{
  /// <summary>
  /// Provides extension methods used for logging.
  /// </summary>
  public static partial class LoggerExtensions
  {
    private static readonly DoubleCheckedLockingContainer<IEnumerationGlobalizationService> s_globalizationService =
        new DoubleCheckedLockingContainer<IEnumerationGlobalizationService>(
            () => SafeServiceLocator.Current.GetInstance<IEnumerationGlobalizationService>());

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the specified <paramref name="logLevel"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void LogFormat (this ILogger logger, LogLevel logLevel, Enum messageEnum, Exception? exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);

      logger.Log(
          logLevel,
          new EventId(Convert.ToInt32(messageEnum)),
          exceptionObject,
          s_globalizationService.Value.GetEnumerationValueDisplayName(messageEnum),
          args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the specified <paramref name="logLevel"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void LogFormat (this ILogger logger, LogLevel logLevel, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(logLevel, messageEnum, null, args);
    }

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="Microsoft.Extensions.Logging.LogLevel.Trace"/> level, including the stack
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void TraceFormat (this ILogger logger, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(LogLevel.Trace, messageEnum, exceptionObject, args);
    }

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="Microsoft.Extensions.Logging.LogLevel.Trace"/> level.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void TraceFormat (this ILogger logger, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(LogLevel.Trace, messageEnum, (Exception?)null, args);
    }

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="Microsoft.Extensions.Logging.LogLevel.Debug"/> level, including the stack
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void DebugFormat (this ILogger logger, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(LogLevel.Debug, messageEnum, exceptionObject, args);
    }

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="Microsoft.Extensions.Logging.LogLevel.Debug"/> level.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void DebugFormat (this ILogger logger, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(LogLevel.Debug, messageEnum, (Exception?)null, args);
    }

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="Microsoft.Extensions.Logging.LogLevel.Information"/> level, including the stack
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void InfoFormat (this ILogger logger, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(LogLevel.Information, messageEnum, exceptionObject, args);
    }

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="Microsoft.Extensions.Logging.LogLevel.Information"/> level.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void InfoFormat (this ILogger logger, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(LogLevel.Information, messageEnum, (Exception?)null, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="Microsoft.Extensions.Logging.LogLevel.Warning"/> level, including the stack
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void WarnFormat (this ILogger logger, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(LogLevel.Warning, messageEnum, exceptionObject, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="Microsoft.Extensions.Logging.LogLevel.Warning"/> level.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void WarnFormat (this ILogger logger, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(LogLevel.Warning, messageEnum, (Exception?)null, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="Microsoft.Extensions.Logging.LogLevel.Error"/> level, including the stack
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void ErrorFormat (this ILogger logger, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(LogLevel.Error, messageEnum, exceptionObject, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="Microsoft.Extensions.Logging.LogLevel.Error"/> level.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void ErrorFormat (this ILogger logger, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(LogLevel.Error, messageEnum, (Exception?)null, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="Microsoft.Extensions.Logging.LogLevel.Critical"/> level, including the stack
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void CriticalFormat (this ILogger logger, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(LogLevel.Critical, messageEnum, exceptionObject, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="Microsoft.Extensions.Logging.LogLevel.Critical"/> level.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void CriticalFormat (this ILogger logger, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(LogLevel.Critical, messageEnum, (Exception?)null, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <c>LogLevel.Fatal</c>, including the stack
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    [Obsolete("LogLevel.Fatal is not supported by Microsoft Logging. Use LogLevel.Critical instead. (Version 7.0.0)", true)]
    public static void FatalFormat (this ILogger logger, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      throw new NotSupportedException("LogLevel.Fatal is not supported by Microsoft Logging. Use LogLevel.Critical instead. (Version 7.0.0)");
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <c>LogLevel.Fatal</c> level.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    [Obsolete("LogLevel.Fatal is not supported by Microsoft Logging. Use LogLevel.Critical instead. (Version 7.0.0)", true)]
    public static void FatalFormat (this ILogger logger, Enum messageEnum, params object[] args)
    {
      throw new NotSupportedException("LogLevel.Fatal is not supported by Microsoft Logging. Use LogLevel.Critical instead. (Version 7.0.0)");
    }
  }
}
