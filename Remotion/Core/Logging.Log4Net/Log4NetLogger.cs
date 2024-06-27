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
using System.Globalization;
using log4net.Core;
using log4net.Util;
using Microsoft.Extensions.Logging;

namespace Remotion.Logging.Log4Net;

/// <summary>
/// Implements <see cref="Microsoft.Extensions.Logging.ILogger"/> to provide an implementation with log4net.
/// </summary>
public class Log4NetLogger : Microsoft.Extensions.Logging.ILogger
{
  public static Level Convert (LogLevel logLevel)
  {
    return logLevel switch
    {
        LogLevel.Trace => Level.Trace,
        LogLevel.Debug => Level.Debug,
        LogLevel.Information => Level.Info,
        LogLevel.Warning => Level.Warn,
        LogLevel.Error => Level.Error,
        LogLevel.Critical => Level.Critical,
        LogLevel.None => Level.Off,
        _ => throw new ArgumentException(string.Format("LogLevel does not support value {0}.", logLevel), "logLevel")
    };
  }

  public log4net.Core.ILogger Logger { get; }

  public Log4NetLogger (log4net.Core.ILogger logger)
  {
    if (logger == null)
      throw new ArgumentNullException(nameof(logger));

    Logger = logger;
  }

  public void Log<TState> (LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
  {
    var level = Convert(logLevel);
    if (Logger.IsEnabledFor(level))
      Logger.Log(CreateLoggingEvent(level, eventId.Id, formatter(state, exception), exception));
  }

  public bool IsEnabled (LogLevel logLevel)
  {
    return Logger.IsEnabledFor(Convert(logLevel));
  }

  public IDisposable? BeginScope<TState> (TState state)
      where TState : notnull
  {
    return null;
  }

  private LoggingEvent CreateLoggingEvent (Level level, int? eventID, object message, Exception? exceptionObject)
  {
    var loggingEvent = new LoggingEvent(typeof(Log4NetLogger), null, Logger.Name, level, message, exceptionObject);

    if (eventID.HasValue)
    {
      if (eventID < 0 || eventID > 0xFFFF)
      {
        LogLoggingError(eventID.Value, exceptionObject, message);

        throw new ArgumentOutOfRangeException(
            "eventID", string.Format("An event id of value {0} is not supported. Valid event ids must be within a range of 0 and 65535.", eventID));
      }

      loggingEvent.Properties["EventID"] = eventID;
    }

    return loggingEvent;
  }

  private void LogLoggingError (int eventID, Exception? exceptionObject, object message)
  {
    int safeEventID;
    if (eventID < 0)
      safeEventID  = 0;
    else if (eventID > 0xFFFF)
      safeEventID  = 0xFFFF;
    else
      safeEventID = eventID;

    Level level = Level.Error;

    if (Logger.IsEnabledFor(level))
    {
      Logger.Log(
          CreateLoggingEvent(
              level,
              safeEventID,
              new SystemStringFormat(
                  CultureInfo.InvariantCulture,
                  "Failure during logging of message:\r\n{0}\r\nEvent ID: {1}",
                  new object?[] { message, eventID }),
              exceptionObject));
    }
  }
}
