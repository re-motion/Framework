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
using MicrosoftLogging = Microsoft.Extensions.Logging;
using RemotionLogging = Remotion.Logging;

namespace Remotion.Logging.Log4Net;

/// <summary>
/// Implements <see cref="Microsoft.Extensions.Logging.ILogger"/> to provide an implementation with log4net.
/// </summary>
public class Log4NetLogger : MicrosoftLogging.ILogger
{
  private readonly log4net.Core.ILogger _logger;

  public Log4NetLogger (string categoryName)
  {
    _logger = log4net.LogManager.GetLogger(categoryName).Logger;
  }

  public static Level Convert (MicrosoftLogging.LogLevel logLevel)
  {
    return logLevel switch
    {
        MicrosoftLogging.LogLevel.Trace => Level.Trace,
        MicrosoftLogging.LogLevel.Debug => Level.Debug,
        MicrosoftLogging.LogLevel.Information => Level.Info,
        MicrosoftLogging.LogLevel.Warning => Level.Warn,
        MicrosoftLogging.LogLevel.Error => Level.Error,
        MicrosoftLogging.LogLevel.Critical => Level.Critical,
        MicrosoftLogging.LogLevel.None => Level.Off,
        _ => throw new ArgumentException(string.Format("LogLevel does not support value {0}.", logLevel), "logLevel")
    };
  }

  public static Level Convert (RemotionLogging.LogLevel logLevel)
  {
    return logLevel switch
    {
        RemotionLogging.LogLevel.Debug => Level.Debug,
        RemotionLogging.LogLevel.Info => Level.Info,
        RemotionLogging.LogLevel.Warn => Level.Warn,
        RemotionLogging.LogLevel.Error => Level.Error,
        RemotionLogging.LogLevel.Fatal => Level.Fatal,
        _ => throw new ArgumentException(string.Format("LogLevel does not support value {0}.", logLevel), "logLevel")
    };
  }

  public void Log<TState> (MicrosoftLogging.LogLevel logLevel, MicrosoftLogging.EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
  {
    var level = Convert(logLevel);
    if (_logger.IsEnabledFor(level))
      _logger.Log(CreateLoggingEvent(level, eventId.Id, formatter(state, exception), exception));
  }

  public bool IsEnabled (MicrosoftLogging.LogLevel logLevel)
  {
    return _logger.IsEnabledFor(Convert(logLevel));
  }

  public IDisposable? BeginScope<TState> (TState state)
      where TState : notnull
  {
    return null;
  }

  private LoggingEvent CreateLoggingEvent (Level level, int? eventID, object? message, Exception? exceptionObject)
  {
    var loggingEvent = new LoggingEvent(typeof(Log4NetLogger), null, _logger.Name, level, message, exceptionObject);

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

  private void LogLoggingError (int eventID, Exception? exceptionObject, object? message)
  {
    int safeEventID;
    if (eventID < 0)
      safeEventID  = 0;
    else if (eventID > 0xFFFF)
      safeEventID  = 0xFFFF;
    else
      safeEventID = eventID;

    Level level = Level.Error;
    if (_logger.IsEnabledFor(level))
    {
      if (_logger.IsEnabledFor(level))
      {
        _logger.Log(
            CreateLoggingEvent(
                level,
                safeEventID,
                new SystemStringFormat(
                    CultureInfo.InvariantCulture,
                    "Failure during logging of message:\r\n{0}\r\nEvent ID: {1}",
                    // TODO RM-7803: message or the ToString() result being null should result in a fallback value being used.
                    new object?[] { message!.ToString(), eventID }),
                exceptionObject));
      }
    }
  }
}
