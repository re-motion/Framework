using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Remotion.Utilities;
using IMicrosoftLogger = Microsoft.Extensions.Logging.ILogger;
using MicrosoftLoglevel = Microsoft.Extensions.Logging.LogLevel;

namespace Remotion.Logging;

/// <summary>
/// Provides extension methods used for logging.
/// </summary>
public static class LoggerExtensionsObsolete
{
  /// <summary>
  /// Log a message object with the specified <paramref name="logLevel"/> and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="logLevel">The <see cref="MicrosoftLoglevel"/> of the message to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  public static void Log (this IMicrosoftLogger logger, MicrosoftLoglevel logLevel, int eventID, object? message)
  {
    logger.Log(logLevel, new EventId(eventID), message?.ToString());
  }

  /// <summary>
  /// Log a message object with the specified <paramref name="logLevel"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="logLevel">The <see cref="MicrosoftLoglevel"/> of the message to be logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  public static void Log (this IMicrosoftLogger logger, MicrosoftLoglevel logLevel, object? message, Exception exceptionObject)
  {
    logger.Log(logLevel, exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the specified <paramref name="logLevel"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="logLevel">The <see cref="MicrosoftLoglevel"/> of the message to be logged.</param>
  /// <param name="message">The message object to log.</param>
  public static void Log (this IMicrosoftLogger logger, MicrosoftLoglevel logLevel, object? message)
  {
    Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, logLevel, message?.ToString());
  }

  /// <overloads>Log a formatted string with the specified <paramref name="logLevel"/>.</overloads>
  /// <summary>
  /// Log a formatted string with the specified <paramref name="logLevel"/> and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="logLevel">The <see cref="MicrosoftLoglevel"/> of the message to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogFormat (this IMicrosoftLogger logger, MicrosoftLoglevel logLevel, int eventID, string? format, params object?[] args)
  {
    logger.Log(logLevel, new EventId(eventID), format, args);
  }

  /// <summary>
  /// Log a formatted string with the specified <paramref name="logLevel"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="logLevel">The <see cref="MicrosoftLoglevel"/> of the message to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogFormat (this IMicrosoftLogger logger, MicrosoftLoglevel logLevel, string? format, params object?[] args)
  {
    logger.Log(logLevel, format, args);
  }

  /// <summary>
  /// Log a formatted string with the specified <paramref name="logLevel"/>,  including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="logLevel">The <see cref="MicrosoftLoglevel"/> of the message to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void LogFormat (this IMicrosoftLogger logger, MicrosoftLoglevel logLevel, Exception exceptionObject, string? format, params object?[] args)
  {
    logger.Log(logLevel, exceptionObject, format, args);
  }

  /// <overloads>Log a message object with the <see cref="MicrosoftLoglevel.Debug"/> level.</overloads>
  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Debug"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  public static void Debug (this IMicrosoftLogger logger, int eventID, object? message, Exception exceptionObject)
  {
    logger.LogDebug(new EventId(eventID), exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Debug"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  public static void Debug (this IMicrosoftLogger logger, int eventID, object? message)
  {
    logger.LogDebug(new EventId(eventID), message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Debug"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  public static void Debug (this IMicrosoftLogger logger, object? message, Exception exceptionObject)
  {
    logger.LogDebug(exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Debug"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  public static void Debug (this IMicrosoftLogger logger, object? message)
  {
    logger.LogDebug(message?.ToString());
  }

  /// <overloads>Log a formatted string with the <see cref="MicrosoftLoglevel.Debug"/> level.</overloads>
  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Debug"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void DebugFormat (this IMicrosoftLogger logger, int eventID, Exception exceptionObject, string? format, params object?[] args)
  {
    logger.LogDebug(new EventId(eventID), exceptionObject, format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Debug"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void DebugFormat (this IMicrosoftLogger logger, int eventID, string? format, params object?[] args)
  {
    logger.LogDebug(new EventId(eventID), format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Debug"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void DebugFormat (this IMicrosoftLogger logger, string? format, params object?[] args)
  {
    logger.LogDebug(format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Debug"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void DebugFormat (this IMicrosoftLogger logger, Exception exceptionObject, string? format, params object?[] args)
  {
    logger.LogDebug(exceptionObject, format, args);
  }

  /// <overloads>Log a message object with the <see cref="MicrosoftLoglevel.Information"/> level.</overloads>
  /// <summary>
  /// Log a message object with the <see cref="LogLevel.Info"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  public static void Info (this IMicrosoftLogger logger, int eventID, object? message, Exception exceptionObject)
  {
    logger.LogInformation(new EventId(eventID), exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Information"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  public static void Info (this IMicrosoftLogger logger, int eventID, object? message)
  {
    logger.LogInformation(new EventId(eventID), message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Information"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  public static void Info (this IMicrosoftLogger logger, object? message, Exception exceptionObject)
  {
    logger.LogInformation(exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Information"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  public static void Info (this IMicrosoftLogger logger, object? message)
  {
    logger.LogInformation(message?.ToString());
  }

  /// <overloads>Log a formatted string with the <see cref="MicrosoftLoglevel.Information"/> level.</overloads>
  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Information"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void InfoFormat (this IMicrosoftLogger logger, int eventID, Exception exceptionObject, string? format, params object?[] args)
  {
    logger.LogInformation(new EventId(eventID), exceptionObject, format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Information"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void InfoFormat (this IMicrosoftLogger logger, int eventID, string? format, params object?[] args)
  {
    logger.LogInformation(new EventId(eventID), format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Information"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void InfoFormat (this IMicrosoftLogger logger, string? format, params object?[] args)
  {
    logger.LogInformation(format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Information"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void InfoFormat (this IMicrosoftLogger logger, Exception exceptionObject, string? format, params object?[] args)
  {
    logger.LogInformation(exceptionObject, format, args);
  }

  /// <overloads>Log a message object with the <see cref="MicrosoftLoglevel.Warning"/> level.</overloads>
  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Warning"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  public static void Warn (this IMicrosoftLogger logger, int eventID, object? message, Exception exceptionObject)
  {
    logger.LogWarning(new EventId(eventID), exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Warning"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  public static void Warn (this IMicrosoftLogger logger, int eventID, object? message)
  {
    logger.LogWarning(new EventId(eventID), message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Warning"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  public static void Warn (this IMicrosoftLogger logger, object? message, Exception exceptionObject)
  {
    logger.LogWarning(exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Warning"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  public static void Warn (this IMicrosoftLogger logger, object? message)
  {
    logger.LogWarning(message?.ToString());
  }

  /// <overloads>Log a formatted string with the <see cref="MicrosoftLoglevel.Warning"/> level.</overloads>
  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Warning"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void WarnFormat (this IMicrosoftLogger logger, int eventID, Exception exceptionObject, string? format, params object?[] args)
  {
    logger.LogWarning(new EventId(eventID), exceptionObject, format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Warning"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void WarnFormat (this IMicrosoftLogger logger, int eventID, string? format, params object?[] args)
  {
    logger.LogWarning(new EventId(eventID), format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Warning"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void WarnFormat (this IMicrosoftLogger logger, string? format, params object?[] args)
  {
    logger.LogWarning(format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Warning"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void WarnFormat (this IMicrosoftLogger logger, Exception exceptionObject, string? format, params object?[] args)
  {
    logger.LogWarning(exceptionObject, format, args);
  }

  /// <overloads>Log a message object with the <see cref="MicrosoftLoglevel.Error"/> level.</overloads>
  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Error"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  public static void Error (this IMicrosoftLogger logger, int eventID, object? message, Exception exceptionObject)
  {
    logger.LogError(new EventId(eventID), exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Error"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  public static void Error (this IMicrosoftLogger logger, int eventID, object? message)
  {
    logger.LogError(new EventId(eventID), message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Error"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  public static void Error (this IMicrosoftLogger logger, object? message, Exception exceptionObject)
  {
    logger.LogError(exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Error"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  public static void Error (this IMicrosoftLogger logger, object? message)
  {
    logger.LogError(message?.ToString());
  }

  /// <overloads>Log a formatted string with the <see cref="MicrosoftLoglevel.Error"/> level.</overloads>
  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Error"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void ErrorFormat (this IMicrosoftLogger logger, int eventID, Exception exceptionObject, string? format, params object?[] args)
  {
    logger.LogError(new EventId(eventID), exceptionObject, format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Error"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void ErrorFormat (this IMicrosoftLogger logger, int eventID, string? format, params object?[] args)
  {
    logger.LogError(new EventId(eventID), format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Error"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void ErrorFormat (this IMicrosoftLogger logger, string? format, params object?[] args)
  {
    logger.LogError(format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Error"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void ErrorFormat (this IMicrosoftLogger logger, Exception exceptionObject, string? format, params object?[] args)
  {
    logger.LogError(exceptionObject, format, args);
  }

  /// <overloads>Log a message object with the <see cref="MicrosoftLoglevel.Critical"/> level.</overloads>
  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Critical"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  public static void Fatal (this IMicrosoftLogger logger, int eventID, object? message, Exception exceptionObject)
  {
    logger.LogCritical(new EventId(eventID), exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Critical"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="message">The message object to log.</param>
  public static void Fatal (this IMicrosoftLogger logger, int eventID, object? message)
  {
    logger.LogCritical(new EventId(eventID), message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Critical"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  public static void Fatal (this IMicrosoftLogger logger, object? message, Exception exceptionObject)
  {
    logger.LogCritical(exceptionObject, message?.ToString());
  }

  /// <summary>
  /// Log a message object with the <see cref="MicrosoftLoglevel.Critical"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="message">The message object to log.</param>
  public static void Fatal (this IMicrosoftLogger logger, object? message)
  {
    logger.LogCritical(message?.ToString());
  }

  /// <overloads>Log a formatted string with the <see cref="MicrosoftLoglevel.Critical"/> level.</overloads>
  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Critical"/> level and <paramref name="eventID"/>,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void FatalFormat (this IMicrosoftLogger logger, int eventID, Exception exceptionObject, string? format, params object?[] args)
  {
    logger.LogCritical(new EventId(eventID), exceptionObject, format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Critical"/> level and <paramref name="eventID"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="eventID">The numeric identifier for the event.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void FatalFormat (this IMicrosoftLogger logger, int eventID, string? format, params object?[] args)
  {
    logger.LogCritical(new EventId(eventID), format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Critical"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void FatalFormat (this IMicrosoftLogger logger, string? format, params object?[] args)
  {
    logger.LogCritical(format, args);
  }

  /// <summary>
  /// Log a formatted string with the <see cref="MicrosoftLoglevel.Critical"/> level,
  /// including the stack trace of <paramref name="exceptionObject"/>.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [StringFormatMethod("format")]
  public static void FatalFormat (this IMicrosoftLogger logger, Exception exceptionObject, string? format, params object?[] args)
  {
    logger.LogCritical(exceptionObject, format, args);
  }

  /// <summary>
  /// Checks if <paramref name="logger"/> is enabled for the <see cref="MicrosoftLoglevel.Debug"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  public static bool IsDebugEnabled (this IMicrosoftLogger logger)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    return logger.IsEnabled(MicrosoftLoglevel.Debug);
  }

  /// <summary>
  /// Checks if <paramref name="logger"/> is enabled for the <see cref="MicrosoftLoglevel.Information"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  public static bool IsInfoEnabled (this IMicrosoftLogger logger)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    return logger.IsEnabled(MicrosoftLoglevel.Information);
  }

  /// <summary>
  /// Checks if <paramref name="logger"/> is enabled for the <see cref="MicrosoftLoglevel.Warning"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  public static bool IsWarnEnabled (this IMicrosoftLogger logger)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    return logger.IsEnabled(MicrosoftLoglevel.Warning);
  }

  /// <summary>
  /// Checks if <paramref name="logger"/> is enabled for the <see cref="MicrosoftLoglevel.Error"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  public static bool IsErrorEnabled (this IMicrosoftLogger logger)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    return logger.IsEnabled(MicrosoftLoglevel.Error);
  }

  /// <summary>
  /// Checks if <paramref name="logger"/> is enabled for the <see cref="MicrosoftLoglevel.Critical"/> level.
  /// </summary>
  /// <param name="logger">The <see cref="IMicrosoftLogger"/> instance where the message is to be logged.</param>
  public static bool IsFatalEnabled (this IMicrosoftLogger logger)
  {
    ArgumentUtility.CheckNotNull("logger", logger);
    return logger.IsEnabled(MicrosoftLoglevel.Critical);
  }
}
