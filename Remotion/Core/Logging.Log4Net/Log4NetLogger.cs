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
using Remotion.Utilities;

using IMicrosoftLogger = Microsoft.Extensions.Logging.ILogger;
using MicrosoftLoglevel = Microsoft.Extensions.Logging.LogLevel;
using MicrosoftEventId = Microsoft.Extensions.Logging.EventId;
using RemotionLoglevel = Remotion.Logging.LogLevel;

namespace Remotion.Logging.Log4Net;

/// <summary>
/// Implements <see cref="Microsoft.Extensions.Logging.ILogger"/> to provide an implementation with log4net.
/// </summary>
public class Log4NetLogger : IMicrosoftLogger
{
  public static Level Convert (MicrosoftLoglevel logLevel)
  {
    return logLevel switch
    {
        MicrosoftLoglevel.Trace => Level.Trace,
        MicrosoftLoglevel.Debug => Level.Debug,
        MicrosoftLoglevel.Information => Level.Info,
        MicrosoftLoglevel.Warning => Level.Warn,
        MicrosoftLoglevel.Error => Level.Error,
        MicrosoftLoglevel.Critical => Level.Critical,
        MicrosoftLoglevel.None => Level.Off,
        _ => throw new ArgumentException(string.Format("LogLevel does not support value {0}.", logLevel), "logLevel")
    };
  }

  public static Level Convert (RemotionLoglevel logLevel)
  {
    return logLevel switch
    {
        RemotionLoglevel.Debug => Level.Debug,
        RemotionLoglevel.Info => Level.Info,
        RemotionLoglevel.Warn => Level.Warn,
        RemotionLoglevel.Error => Level.Error,
        RemotionLoglevel.Fatal => Level.Fatal,
        _ => throw new ArgumentException(string.Format("LogLevel does not support value {0}.", logLevel), "logLevel")
    };
  }

  public log4net.Core.ILogger Logger { get; }

  public Log4NetLogger (ILogger logger)
  {
    ArgumentUtility.CheckNotNull("logger", logger);

    Logger = logger;
  }

  public void Log<TState> (MicrosoftLoglevel logLevel, MicrosoftEventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
  {
    var level = Convert(logLevel);
    if (Logger.IsEnabledFor(level))
      Logger.Log(CreateLoggingEvent(level, eventId.Id, formatter(state, exception), exception));
  }

  public bool IsEnabled (MicrosoftLoglevel logLevel)
  {
    return Logger.IsEnabledFor(Convert(logLevel));
  }

  public IDisposable? BeginScope<TState> (TState state)
      where TState : notnull
  {
    return null;
  }

  private LoggingEvent CreateLoggingEvent (Level level, int? eventID, string message, Exception? exceptionObject)
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

  private void LogLoggingError (int eventID, Exception? exceptionObject, string message)
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
              string.Format(
                  CultureInfo.InvariantCulture,
                  "Failure during logging of message:\r\n{0}\r\nEvent ID: {1}",
                  // TODO RM-7803: message or the ToString() result being null should result in a fallback value being used.
                  new object?[] { message!.ToString(), eventID }),
              exceptionObject));
    }
  }
}
