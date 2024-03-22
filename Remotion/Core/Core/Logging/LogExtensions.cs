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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Logging
{
  /// <summary>
  /// Provides extension methods used for logging.
  /// </summary>
  public static class LogExtensions
  {
    /// <summary>
    /// Logs the given value and returns it to the caller. This is typically used to log a value returned by a method directly in the return 
    /// statement.
    /// </summary>
    /// <typeparam name="T">The (inferred) type of the value to be logged.</typeparam>
    /// <param name="value">The value to be logged.</param>
    /// <param name="log">The <see cref="ILog"/> to log the value with.</param>
    /// <param name="logLevel">The <see cref="LogLevel"/> to log the value at. If the <paramref name="log"/> does not support this level, the 
    /// <paramref name="messageCreator"/> is not called.</param>
    /// <param name="messageCreator">A function object building the message to be logged.</param>
    /// <returns>The <paramref name="value"/> passed in to the method.</returns>
    public static T LogAndReturnValue<T> (this T value, ILog log, LogLevel logLevel, Func<T, string?> messageCreator)
    {
      if (log.IsEnabled(logLevel))
      {
        log.Log(logLevel, (int?)null, messageCreator(value), (Exception?)null);
      }
      return value;
    }

    public static IEnumerable<T> LogAndReturnItems<T> (
        this IEnumerable<T> sequence,
        ILog log,
        LogLevel logLevel,
        Func<int, string?> iterationCompletedMessageCreator)
    {
      if (log.IsEnabled(logLevel))
        return LogAndReturnWithIteration(sequence, log, logLevel, iterationCompletedMessageCreator);
      return sequence;
    }


    private static IEnumerable<T> LogAndReturnWithIteration<T> (
        IEnumerable<T> sequence,
        ILog log,
        LogLevel logLevel,
        Func<int, string?> iterationCompletedMessageCreator)
    {
      int count = 0;
      foreach (var item in sequence)
      {
        count++;
        yield return item;
      }


      log.Log(logLevel, iterationCompletedMessageCreator(count));
    }

    /// <summary>
    /// Log a message object with the specified <paramref name="logLevel"/> and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="logLevel" or @name="eventID" or @name="message"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public static void Log (this ILog log, LogLevel logLevel, int eventID, object? message)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(logLevel, eventID, message, (Exception?)null);
    }

    /// <summary>
    /// Log a message object with the specified <paramref name="logLevel"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="logLevel" or @name="message" or @name="exceptionObject"]' />
    public static void Log (this ILog log, LogLevel logLevel, object? message, Exception exceptionObject)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(logLevel, (int?)null, message, exceptionObject);
    }

    /// <summary>
    /// Log a message object with the specified <paramref name="logLevel"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="logLevel" or @name="message"]' />
    public static void Log (this ILog log, LogLevel logLevel, object? message)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(logLevel, (int?)null, message, (Exception?)null);
    }

    /// <overloads>Log a formatted string with the specified <paramref name="logLevel"/>.</overloads>
    /// <summary>
    /// Log a formatted string with the specified <paramref name="logLevel"/> and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="logLevel" or @name="eventID"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    [StringFormatMethod("format")]
    public static void LogFormat (this ILog log, LogLevel logLevel, int eventID, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(logLevel, eventID, (Exception?)null, format, args);
    }

    /// <summary>
    /// Log a formatted string with the specified <paramref name="logLevel"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="logLevel"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    [StringFormatMethod("format")]
    public static void LogFormat (this ILog log, LogLevel logLevel, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(logLevel, (int?)null, (Exception?)null, format, args);
    }

    /// <summary>
    /// Log a formatted string with the specified <paramref name="logLevel"/>,  including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="logLevel" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    [StringFormatMethod("format")]
    public static void LogFormat (this ILog log, LogLevel logLevel, Exception exceptionObject, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(logLevel, (int?)null, exceptionObject, format, args);
    }

    /// <overloads>Log a message object with the <see cref="LogLevel.Debug"/> level.</overloads>
    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Debug"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID" or @name="message" or @name="exceptionObject"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public static void Debug (this ILog log, int eventID, object? message, Exception exceptionObject)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Debug, eventID, message, exceptionObject);
    }

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Debug"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID" or @name="message"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public static void Debug (this ILog log, int eventID, object? message)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Debug, eventID, message, (Exception?)null);
    }

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Debug"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="message" or @name="exceptionObject"]' />
    public static void Debug (this ILog log, object? message, Exception exceptionObject)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Debug, (int?)null, message, exceptionObject);
    }

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="message"]' />
    public static void Debug (this ILog log, object? message)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Debug, (int?)null, message, (Exception?)null);
    }

    /// <overloads>Log a formatted string with the <see cref="LogLevel.Debug"/> level.</overloads>
    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Debug"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    [StringFormatMethod("format")]
    public static void DebugFormat (this ILog log, int eventID, Exception exceptionObject, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Debug, eventID, exceptionObject, format, args);
    }

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Debug"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    [StringFormatMethod("format")]
    public static void DebugFormat (this ILog log, int eventID, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Debug, eventID, (Exception?)null, format, args);
    }
        /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    [StringFormatMethod("format")]
    public static void DebugFormat (this ILog log, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Debug, (int?)null, (Exception?)null, format, args);
    }

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Debug"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    [StringFormatMethod("format")]
    public static void DebugFormat (this ILog log, Exception exceptionObject, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Debug, (int?)null, exceptionObject, format, args);
    }

    /// <overloads>Log a message object with the <see cref="LogLevel.Info"/> level.</overloads>
    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Info"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID" or @name="message" or @name="exceptionObject"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public static void Info (this ILog log, int eventID, object? message, Exception exceptionObject)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Info, eventID, message, exceptionObject);
    }

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Info"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID" or @name="message"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public static void Info (this ILog log, int eventID, object? message)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Info, eventID, message, (Exception?)null);
    }

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Info"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="message" or @name="exceptionObject"]' />
    public static void Info (this ILog log, object? message, Exception exceptionObject)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Info,  (int?)null, message, exceptionObject);
    }

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="message"]' />
    public static void Info (this ILog log, object? message)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Info, (int?)null, message, (Exception?)null);
    }

    /// <overloads>Log a formatted string with the <see cref="LogLevel.Info"/> level.</overloads>
    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Info"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    [StringFormatMethod("format")]
    public static void InfoFormat (this ILog log, int eventID, Exception exceptionObject, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Info, eventID, exceptionObject, format, args);
    }

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Info"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    [StringFormatMethod("format")]
    public static void InfoFormat (this ILog log, int eventID, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Info, eventID, (Exception?)null, format, args);
    }

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    [StringFormatMethod("format")]
    public static void InfoFormat (this ILog log, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Info, (int?)null, (Exception?)null, format, args);
    }

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Info"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    [StringFormatMethod("format")]
    public static void InfoFormat (this ILog log, Exception exceptionObject, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Info, (int?)null, exceptionObject, format, args);
    }

    /// <overloads>Log a message object with the <see cref="LogLevel.Warn"/> level.</overloads>
    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Warn"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID" or @name="message" or @name="exceptionObject"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public static void Warn (this ILog log, int eventID, object? message, Exception exceptionObject)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Warn, eventID, message, exceptionObject);
    }

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Warn"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID" or @name="message"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public static void Warn (this ILog log, int eventID, object? message)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Warn, eventID, message, (Exception?)null);
    }

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Warn"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="message" or @name="exceptionObject"]' />
    public static void Warn (this ILog log, object? message, Exception exceptionObject)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Warn, (int?)null, message, exceptionObject);
    }

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="message"]' />
    public static void Warn (this ILog log, object? message)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Warn, (int?)null, message, (Exception?)null);
    }

    /// <overloads>Log a formatted string with the <see cref="LogLevel.Warn"/> level.</overloads>
    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Warn"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    [StringFormatMethod("format")]
    public static void WarnFormat (this ILog log, int eventID, Exception exceptionObject, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Warn, eventID, exceptionObject, format, args);
    }

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Warn"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    [StringFormatMethod("format")]
    public static void WarnFormat (this ILog log, int eventID, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Warn, eventID, (Exception?)null, format, args);
    }

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    [StringFormatMethod("format")]
    public static void WarnFormat (this ILog log, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Warn, (int?)null, (Exception?)null, format, args);
    }

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Warn"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    [StringFormatMethod("format")]
    public static void WarnFormat (this ILog log, Exception exceptionObject, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Warn, (int?)null, exceptionObject, format, args);
    }

    /// <overloads>Log a message object with the <see cref="LogLevel.Error"/> level.</overloads>
    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Error"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID" or @name="message" or @name="exceptionObject"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public static void Error (this ILog log, int eventID, object? message, Exception exceptionObject)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Error, eventID, message, exceptionObject);
    }

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Error"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID" or @name="message"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public static void Error (this ILog log, int eventID, object? message)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Error, eventID, message, (Exception?)null);
    }

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Error"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="message" or @name="exceptionObject"]' />
    public static void Error (this ILog log, object? message, Exception exceptionObject)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Error, (int?)null, message, exceptionObject);
    }

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="message"]' />
    public static void Error (this ILog log, object? message)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Error, (int?)null, message, (Exception?)null);
    }

    /// <overloads>Log a formatted string with the <see cref="LogLevel.Error"/> level.</overloads>
    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Error"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    [StringFormatMethod("format")]
    public static void ErrorFormat (this ILog log, int eventID, Exception exceptionObject, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Error, eventID, exceptionObject, format, args);
    }

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Error"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    [StringFormatMethod("format")]
    public static void ErrorFormat (this ILog log, int eventID, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Error, eventID, (Exception?)null, format, args);
    }

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    [StringFormatMethod("format")]
    public static void ErrorFormat (this ILog log, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Error, (int?)null, (Exception?)null, format, args);
    }

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Error"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    [StringFormatMethod("format")]
    public static void ErrorFormat (this ILog log, Exception exceptionObject, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Error, (int?)null, exceptionObject, format, args);
    }

    /// <overloads>Log a message object with the <see cref="LogLevel.Fatal"/> level.</overloads>
    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Fatal"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID" or @name="message" or @name="exceptionObject"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public static void Fatal (this ILog log, int eventID, object? message, Exception exceptionObject)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Fatal, eventID, message, exceptionObject);
    }

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Fatal"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID" or @name="message"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public static void Fatal (this ILog log, int eventID, object? message)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Fatal, eventID, message, (Exception?)null);
    }

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Fatal"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="message" or @name="exceptionObject"]' />
    public static void Fatal (this ILog log, object? message, Exception exceptionObject)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Fatal, (int?)null, message, exceptionObject);
    }

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="message"]' />
    public static void Fatal (this ILog log, object? message)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.Log(LogLevel.Fatal, (int?)null, message, (Exception?)null);
    }

    /// <overloads>Log a formatted string with the <see cref="LogLevel.Fatal"/> level.</overloads>
    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Fatal"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    [StringFormatMethod("format")]
    public static void FatalFormat (this ILog log, int eventID, Exception exceptionObject, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Fatal, eventID, exceptionObject, format, args);
    }

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Fatal"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="eventID"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    [StringFormatMethod("format")]
    public static void FatalFormat (this ILog log, int eventID, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Fatal, eventID, (Exception?)null, format, args);
    }

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    [StringFormatMethod("format")]
    public static void FatalFormat (this ILog log, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Fatal, (int?)null, (Exception?)null, format, args);
    }

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Fatal"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="format" or @name="args"]' />
    [StringFormatMethod("format")]
    public static void FatalFormat (this ILog log, Exception exceptionObject, string? format, params object?[]? args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Fatal, (int?)null, exceptionObject, format, args);
    }

    /// <summary>
    /// Checks if <paramref name="log"/> is enabled for the <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static bool IsDebugEnabled (this ILog log)
    {
      ArgumentUtility.CheckNotNull("log", log);
      return log.IsEnabled(LogLevel.Debug);
    }

    /// <summary>
    /// Checks if <paramref name="log"/> is enabled for the <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static bool IsInfoEnabled (this ILog log)
    {
      ArgumentUtility.CheckNotNull("log", log);
      return log.IsEnabled(LogLevel.Info);
    }

    /// <summary>
    /// Checks if <paramref name="log"/> is enabled for the <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static bool IsWarnEnabled (this ILog log)
    {
      ArgumentUtility.CheckNotNull("log", log);
      return log.IsEnabled(LogLevel.Warn);
    }

    /// <summary>
    /// Checks if <paramref name="log"/> is enabled for the <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static bool IsErrorEnabled (this ILog log)
    {
      ArgumentUtility.CheckNotNull("log", log);
      return log.IsEnabled(LogLevel.Error);
    }

    /// <summary>
    /// Checks if <paramref name="log"/> is enabled for the <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static bool IsFatalEnabled (this ILog log)
    {
      ArgumentUtility.CheckNotNull("log", log);
      return log.IsEnabled(LogLevel.Fatal);
    }
  }
}
