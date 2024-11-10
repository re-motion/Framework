// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Remotion.Obsolete;
using Remotion.Utilities;

namespace Remotion.Logging;

/// <summary>
/// Provides extension methods used for logging.
/// </summary>
public static class LoggerExtensionsObsolete
{
  /// <summary>
  /// Log a message object with the specified <paramref name="logLevel"/> and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="logLevel">The <see cref="Microsoft.Extensions.Logging.LogLevel"/> of the message to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  [Obsolete("Use logger.Log(logLevel, new EventID(eventID), message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Log (this ILogger logger, LogLevel logLevel, int eventID, object? message)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.Log(logLevel, new EventId(eventID), message?.ToString());
  }

  /// <summary>
  /// Log a message object with the specified <paramref name="logLevel"/> and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="logLevel">The <see cref="Microsoft.Extensions.Logging.LogLevel"/> of the message to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  [Obsolete("Use logger.Log(logLevel, new EventID(eventID), exceptionObject, message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Log (this ILogger logger, LogLevel logLevel, int eventID, object? message, Exception exceptionObject)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.Log(logLevel, new EventId(eventID), exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the specified <paramref name="logLevel"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="logLevel">The <see cref="Microsoft.Extensions.Logging.LogLevel"/> of the message to be logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  [Obsolete("Use logger.Log(logLevel, exceptionObject message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Log (this ILogger logger, LogLevel logLevel, object? message, Exception exceptionObject)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.Log(logLevel, exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the specified <paramref name="logLevel"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="logLevel">The <see cref="Microsoft.Extensions.Logging.LogLevel"/> of the message to be logged.</param>
  /// <param name="message">The message object to log.</param>
  [Obsolete("Use logger.Log(logLevel, message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Log (this ILogger logger, LogLevel logLevel, object? message)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, logLevel, message?.ToString());
  }

  /// <overloads>Log a formatted string with the specified <paramref name="logLevel"/>.</overloads>
  /// <summary>
  /// Log a formatted string with the specified <paramref name="logLevel"/> and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="logLevel">The <see cref="Microsoft.Extensions.Logging.LogLevel"/> of the message to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.Log(logLevel, new EventID(eventID), format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void LogFormat (this ILogger logger, LogLevel logLevel, int eventID, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.Log(logLevel, new EventId(eventID), format, args);
  }

  /// <summary>
  /// Log a formatted string with the specified <paramref name="logLevel"/>,  including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="logLevel">The <see cref="Microsoft.Extensions.Logging.LogLevel"/> of the message to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.Log(logLevel, new EventID(eventID), exceptionObject, format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void LogFormat (this ILogger logger, LogLevel logLevel, int eventID, Exception exceptionObject, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.Log(logLevel, new EventId(eventID), exceptionObject, format, args);
  }

  /// <overloads>Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Debug"/> level.</overloads>
  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Debug"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  [Obsolete("Use logger.LogDebug(new EventID(eventID), exceptionObject, message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Debug (this ILogger logger, int eventID, object? message, Exception exceptionObject)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogDebug(new EventId(eventID), exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Debug"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  [Obsolete("Use logger.LogDebug(new EventID(eventID), message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Debug (this ILogger logger, int eventID, object? message)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogDebug(new EventId(eventID), message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Debug"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  [Obsolete("Use logger.LogDebug(exceptionObject, message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Debug (this ILogger logger, object? message, Exception exceptionObject)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogDebug(exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Debug"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  [Obsolete("Use logger.LogDebug(message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Debug (this ILogger logger, object? message)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogDebug(message?.ToString());
  }

  /// <overloads>Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Debug"/> level.</overloads>
  /// <summary>
  /// Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Debug"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.LogDebug(new EventID(eventID), exceptionObject, format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void DebugFormat (this ILogger logger, int eventID, Exception exceptionObject, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogDebug(new EventId(eventID), exceptionObject, format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Debug"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.LogDebug(new EventID(eventID), format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void DebugFormat (this ILogger logger, int eventID, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogDebug(new EventId(eventID), format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Debug"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.LogDebug(format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void DebugFormat (this ILogger logger, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogDebug(format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Debug"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.LogDebug(exceptionObject, format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void DebugFormat (this ILogger logger, Exception exceptionObject, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogDebug(exceptionObject, format, args);
  }

  /// <overloads>Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Information"/> level.</overloads>
  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Information"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  [Obsolete("Use logger.LogInformation(new EventID(eventID), exceptionObject, message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Info (this ILogger logger, int eventID, object? message, Exception exceptionObject)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogInformation(new EventId(eventID), exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Information"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  [Obsolete("Use logger.LogInformation(new EventID(eventID), message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Info (this ILogger logger, int eventID, object? message)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogInformation(new EventId(eventID), message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Information"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  [Obsolete("Use logger.LogInformation(exceptionObject, message instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Info (this ILogger logger, object? message, Exception exceptionObject)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogInformation(exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Information"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  [Obsolete("Use logger.LogInformation(message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Info (this ILogger logger, object? message)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogInformation(message?.ToString());
  }

  /// <overloads>Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Information"/> level.</overloads>
  /// <summary>
  /// Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Information"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.LogInformation(new EventID(eventID), exceptionObject, format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void InfoFormat (this ILogger logger, int eventID, Exception exceptionObject, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogInformation(new EventId(eventID), exceptionObject, format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Information"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.LogInformation(new EventID(eventID), format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void InfoFormat (this ILogger logger, int eventID, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogInformation(new EventId(eventID), format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Information"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.LogInformation(new EventID(eventID), format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void InfoFormat (this ILogger logger, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogInformation(format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Information"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.LogInformation(exceptionObject, format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void InfoFormat (this ILogger logger, Exception exceptionObject, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogInformation(exceptionObject, format, args);
  }

  /// <overloads>Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Warning"/> level.</overloads>
  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Warning"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  [Obsolete("Use logger.LogWarning(new EventID(eventID), exceptionObject, message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Warn (this ILogger logger, int eventID, object? message, Exception exceptionObject)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogWarning(new EventId(eventID), exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Warning"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  [Obsolete("Use logger.LogWarning(new EventID(eventID), message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Warn (this ILogger logger, int eventID, object? message)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogWarning(new EventId(eventID), message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Warning"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  [Obsolete("Use logger.LogWarning(exceptionObject, message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Warn (this ILogger logger, object? message, Exception exceptionObject)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogWarning(exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Warning"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  [Obsolete("Use logger.LogWarning(message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Warn (this ILogger logger, object? message)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogWarning(message?.ToString());
  }

  /// <overloads>Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Warning"/> level.</overloads>
  /// <summary>
  /// Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Warning"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.LogWarning(new EventID(eventID), exceptionObject, format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void WarnFormat (this ILogger logger, int eventID, Exception exceptionObject, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogWarning(new EventId(eventID), exceptionObject, format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Warning"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.LogWarning(new EventID(eventID), format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void WarnFormat (this ILogger logger, int eventID, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogWarning(new EventId(eventID), format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Warning"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.LogWarning(format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void WarnFormat (this ILogger logger, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogWarning(format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Warning"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.LogWarning(exceptionObject, format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void WarnFormat (this ILogger logger, Exception exceptionObject, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogWarning(exceptionObject, format, args);
  }

  /// <overloads>Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Error"/> level.</overloads>
  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Error"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  [Obsolete("Use logger.LogError(new EventID(eventID), exceptionObject, message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Error (this ILogger logger, int eventID, object? message, Exception exceptionObject)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogError(new EventId(eventID), exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Error"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  [Obsolete("Use logger.LogError(new EventID(eventID), message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Error (this ILogger logger, int eventID, object? message)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogError(new EventId(eventID), message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Error"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  [Obsolete("Use logger.LogError(exceptionObject, message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Error (this ILogger logger, object? message, Exception exceptionObject)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogError(exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Error"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  [Obsolete("Use logger.LogError(message) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void Error (this ILogger logger, object? message)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogError(message?.ToString());
  }

  /// <overloads>Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Error"/> level.</overloads>
  /// <summary>
  /// Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Error"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.LogError(new EventID(eventID), exceptionObject, format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void ErrorFormat (this ILogger logger, int eventID, Exception exceptionObject, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogError(new EventId(eventID), exceptionObject, format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Error"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.LogError(new EventID(eventID), format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void ErrorFormat (this ILogger logger, int eventID, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogError(new EventId(eventID), format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Error"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.LogError(format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void ErrorFormat (this ILogger logger, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogError(format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="Microsoft.Extensions.Logging.LogLevel.Error"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  [Obsolete("Use logger.LogError(exceptionObject, format, args) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static void ErrorFormat (this ILogger logger, Exception exceptionObject, string? format, params object?[] args)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    logger.LogError(exceptionObject, format, args);
  }

  /// <overloads>Log a message object with the <c>LogLevel.Fatal</c>.</overloads>
  /// <summary>
  /// Log a message object with the <see cref="Microsoft.Extensions.Logging.LogLevel.Critical"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  [Obsolete("LogLevel.Fatal is not supported by Microsoft Logging. Use logger.LogCritical(new EventID(eventID), exceptionObject, message) instead. (Version 7.0.0)", true)]
  public static void Fatal (this ILogger logger, int eventID, object? message, Exception exceptionObject)
  {
    throw new NotSupportedException("LogLevel.Fatal is not supported by Microsoft Logging. Use LogLevel.Critical instead. (Version 7.0.0)");
  }

  /// <summary>
  /// Log a message object with the <c>LogLevel.Fatal</c> and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  [Obsolete("LogLevel.Fatal is not supported by Microsoft Logging. Use logger.LogCritical(new EventID(eventID), message) instead. (Version 7.0.0)", true)]
  public static void Fatal (this ILogger logger, int eventID, object? message)
  {
    throw new NotSupportedException("LogLevel.Fatal is not supported by Microsoft Logging. Use LogLevel.Critical instead. (Version 7.0.0)");
  }

  /// <summary>
  /// Log a message object with the <c>LogLevel.Fatal</c>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  [Obsolete("LogLevel.Fatal is not supported by Microsoft Logging. Use logger.LogCritical(exceptionObject, message) instead. (Version 7.0.0)", true)]
  public static void Fatal (this ILogger logger, object? message, Exception exceptionObject)
  {
    throw new NotSupportedException("LogLevel.Fatal is not supported by Microsoft Logging. Use LogLevel.Critical instead. (Version 7.0.0)");
  }

  /// <summary>
  /// Log a message object with the <c>LogLevel.Fatal</c>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  [Obsolete("LogLevel.Fatal is not supported by Microsoft Logging. Use logger.LogCritical(message) instead. (Version 7.0.0)", true)]
  public static void Fatal (this ILogger logger, object? message)
  {
    throw new NotSupportedException("LogLevel.Fatal is not supported by Microsoft Logging. Use LogLevel.Critical instead. (Version 7.0.0)");
  }

  /// <overloads>Log a formatted string with the <c>LogLevel.Fatal</c>.</overloads>
  /// <summary>
  /// Log a formatted string with the <c>LogLevel.Fatal</c> and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [Obsolete("LogLevel.Fatal is not supported by Microsoft Logging. Use logger.LogCritical(new EventID(eventID), exceptionObject, format, args) instead. (Version 7.0.0)", true)]
  [StringFormatMethod("format")]
  public static void FatalFormat (this ILogger logger, int eventID, Exception exceptionObject, string? format, params object?[] args)
  {
    throw new NotSupportedException("LogLevel.Fatal is not supported by Microsoft Logging. Use LogLevel.Critical instead. (Version 7.0.0)");
  }

  /// <summary>
  /// Log a formatted string with the <c>LogLevel.Fatal</c> and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [Obsolete("LogLevel.Fatal is not supported by Microsoft Logging. Use logger.LogCritical(new EventID(eventID), format, args) instead. (Version 7.0.0)", true)]
  [StringFormatMethod("format")]
  public static void FatalFormat (this ILogger logger, int eventID, string? format, params object?[] args)
  {
    throw new NotSupportedException("LogLevel.Fatal is not supported by Microsoft Logging. Use LogLevel.Critical instead. (Version 7.0.0)");
  }

  /// <summary>
  /// Log a formatted string with the <c>LogLevel.Fatal</c>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [Obsolete("LogLevel.Fatal is not supported by Microsoft Logging. Use logger.LogCritical(format, args) instead. (Version 7.0.0)", true)]
  [StringFormatMethod("format")]
  public static void FatalFormat (this ILogger logger, string? format, params object?[] args)
  {
    throw new NotSupportedException("LogLevel.Fatal is not supported by Microsoft Logging. Use LogLevel.Critical instead. (Version 7.0.0)");
  }

  /// <summary>
  /// Log a formatted string with the <c>LogLevel.Fatal</c>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [Obsolete("LogLevel.Fatal is not supported by Microsoft Logging. Use logger.LogCritical(exceptionObject, format, args) instead. (Version 7.0.0)", true)]
  [StringFormatMethod("format")]
  public static void FatalFormat (this ILogger logger, Exception exceptionObject, string? format, params object?[] args)
  {
    throw new NotSupportedException("LogLevel.Fatal is not supported by Microsoft Logging. Use LogLevel.Critical instead. (Version 7.0.0)");
  }

  /// <summary>
  /// Checks if <paramref name="logger"/> is enabled for the <see cref="Microsoft.Extensions.Logging.LogLevel.Debug"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  [Obsolete("Use logger.IsEnabled(LogLevel.Debug) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static bool IsDebugEnabled (this ILogger logger)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    return logger.IsEnabled(LogLevel.Debug);
  }

  /// <summary>
  /// Checks if <paramref name="logger"/> is enabled for the <see cref="Microsoft.Extensions.Logging.LogLevel.Information"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  [Obsolete("Use logger.IsEnabled(LogLevel.Information) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static bool IsInfoEnabled (this ILogger logger)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    return logger.IsEnabled(LogLevel.Information);
  }

  /// <summary>
  /// Checks if <paramref name="logger"/> is enabled for the <see cref="Microsoft.Extensions.Logging.LogLevel.Warning"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  [Obsolete("Use logger.IsEnabled(LogLevel.Warning) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static bool IsWarnEnabled (this ILogger logger)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    return logger.IsEnabled(LogLevel.Warning);
  }

  /// <summary>
  /// Checks if <paramref name="logger"/> is enabled for the <see cref="Microsoft.Extensions.Logging.LogLevel.Error"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  [Obsolete("Use logger.IsEnabled(LogLevel.Error) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
  public static bool IsErrorEnabled (this ILogger logger)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    return logger.IsEnabled(LogLevel.Error);
  }

  /// <summary>
  /// Checks if <paramref name="logger"/> is enabled for the <c>LogLevel.Fatal</c>.
  /// </summary>
  /// <param name="logger">The <see cref="ILogger"/> instance where the message is to be logged.</param>
  [Obsolete("LogLevel.Fatal is not supported by Microsoft Logging. Use logger.IsEnabled(LogLevel.Critical) instead. (Version 7.0.0)", true)]
  public static bool IsFatalEnabled (this ILogger logger)
  {
    throw new NotSupportedException("LogLevel.Fatal is not supported by Microsoft Logging. Use LogLevel.Critical instead. (Version 7.0.0)");
  }
}
