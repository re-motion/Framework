// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Remotion.Utilities;

namespace Remotion.Logging;

public static partial class LoggerExtensions
{
  /// <summary>
  /// Log a formatted string with the specified <paramref name="logLevel"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="logLevel">The <see cref="Microsoft.Extensions.Logging.LogLevel"/> of the message to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogFormat (this ILogger logger, LogLevel logLevel, string format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    if (logger.IsEnabled(logLevel))
      Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, logLevel, string.Format(format, args));
  }

  /// <summary>
  /// Log a formatted string with the specified <paramref name="logLevel"/>,  including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="logLevel">The <see cref="Microsoft.Extensions.Logging.LogLevel"/> of the message to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogFormat (this ILogger logger, LogLevel logLevel, Exception? exceptionObject, string format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    if (logger.IsEnabled(logLevel))
      Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, logLevel, exceptionObject, string.Format(format, args));
  }

  /// <summary>
  /// Log a formatted string with the <see cref="LogLevel.Trace"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogTraceFormat (this ILogger logger, string format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    if (logger.IsEnabled(LogLevel.Trace))
      Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, LogLevel.Trace, string.Format(format, args));
  }

  /// <summary>
  /// Log a formatted string with the <see cref="LogLevel.Trace"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogTraceFormat (this ILogger logger, Exception? exceptionObject, string format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    if (logger.IsEnabled(LogLevel.Trace))
      Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, LogLevel.Trace, exceptionObject, string.Format(format, args));
  }

  /// <summary>
  /// Log a formatted string with the <see cref="LogLevel.Debug"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogDebugFormat (this ILogger logger, string format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    if (logger.IsEnabled(LogLevel.Debug))
      Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, LogLevel.Debug, string.Format(format, args));
  }

  /// <summary>
  /// Log a formatted string with the <see cref="LogLevel.Debug"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogDebugFormat (this ILogger logger, Exception? exceptionObject, string format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    if (logger.IsEnabled(LogLevel.Debug))
      Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, LogLevel.Debug, exceptionObject, string.Format(format, args));
  }

  /// <summary>
  /// Log a formatted string with the <see cref="LogLevel.Information"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogInformationFormat (this ILogger logger, string format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    if (logger.IsEnabled(LogLevel.Information))
      Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, LogLevel.Information, string.Format(format, args));
  }

  /// <summary>
  /// Log a formatted string with the <see cref="LogLevel.Information"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogInformationFormat (this ILogger logger, Exception? exceptionObject, string format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    if (logger.IsEnabled(LogLevel.Information))
      Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, LogLevel.Information, exceptionObject, string.Format(format, args));
  }

  /// <summary>
  /// Log a formatted string with the <see cref="LogLevel.Warning"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogWarningFormat (this ILogger logger, string format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    if (logger.IsEnabled(LogLevel.Warning))
      Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, LogLevel.Warning, string.Format(format, args));
  }

  /// <summary>
  /// Log a formatted string with the <see cref="LogLevel.Warning"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogWarningFormat (this ILogger logger, Exception? exceptionObject, string format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    if (logger.IsEnabled(LogLevel.Warning))
      Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, LogLevel.Warning, exceptionObject, string.Format(format, args));
  }

  /// <summary>
  /// Log a formatted string with the <see cref="LogLevel.Error"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogErrorFormat (this ILogger logger, string format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    if (logger.IsEnabled(LogLevel.Error))
      Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, LogLevel.Error, string.Format(format, args));
  }

  /// <summary>
  /// Log a formatted string with the <see cref="LogLevel.Error"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogErrorFormat (this ILogger logger, Exception? exceptionObject, string format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    if (logger.IsEnabled(LogLevel.Error))
      Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, LogLevel.Error, exceptionObject, string.Format(format, args));
  }

  /// <summary>
  /// Log a formatted string with the <see cref="LogLevel.Critical"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogCriticalFormat (this ILogger logger, string format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    if (logger.IsEnabled(LogLevel.Critical))
      Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, LogLevel.Critical, string.Format(format, args));
  }

  /// <summary>
  /// Log a formatted string with the <see cref="LogLevel.Critical"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogCriticalFormat (this ILogger logger, Exception? exceptionObject, string format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    if (logger.IsEnabled(LogLevel.Critical))
      Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, LogLevel.Critical, exceptionObject, string.Format(format, args));
  }
}
