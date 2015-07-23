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
using Remotion.Utilities;

namespace Remotion.Logging
{
  /// <summary>
  /// Implementation of interface <see cref="ILog"/> for <b>log4net</b>.
  /// </summary>
  /// <remarks>
  /// Use <see cref="LogManager"/> to instantiate <see cref="Log4NetLog"/> via <see cref="Remotion.Logging.LogManager.GetLogger(string)"/>.
  /// <note type="warning">
  /// <see cref="Log4NetLog"/> does not allow event ids outside the range of unsigned 16-bit integers (0 - 65535) and will throw an
  /// <see cref="ArgumentOutOfRangeException"/> if an event id outside this range is encountered. The original message will be logged using a 
  /// truncated event id before the exception is thrown.
  /// </note>
  /// </remarks>
  public class Log4NetLog : ILog, ILoggerWrapper
  {
    /// <summary>
    /// Converts <see cref="LogLevel"/> to <see cref="Level"/>.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/> to be converted.</param>
    /// <returns>Corresponding <see cref="Level"/> needed for logging to the <b>log4net </b> <see cref="log4net.ILog"/> interface.</returns>
    public static Level Convert (LogLevel logLevel)
    {
      switch (logLevel)
      {
        case LogLevel.Debug:
          return Level.Debug;
        case LogLevel.Info:
          return Level.Info;
        case LogLevel.Warn:
          return Level.Warn;
        case LogLevel.Error:
          return Level.Error;
        case LogLevel.Fatal:
          return Level.Fatal;
        default:
          throw new ArgumentException (string.Format ("LogLevel does not support value {0}.", logLevel), "logLevel");
      }
    }

    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Log4NetLog"/> class 
    /// using the specified <see cref="log4net.Core.ILogger"/>.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> the log messages are written to.</param>
    public Log4NetLog (ILogger logger)
    {
      ArgumentUtility.CheckNotNull ("logger", logger);

      _logger = logger;
    }

    /// <summary>
    /// Gets the <see cref="ILogger"/> used by this <see cref="Log4NetLog"/>.
    /// </summary>
    public ILogger Logger
    {
      get { return _logger; }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void Log (LogLevel logLevel, int? eventID, object message, Exception exceptionObject)
    {
      var level = Convert (logLevel);
      if (_logger.IsEnabledFor (level))
        _logger.Log (CreateLoggingEvent (level, eventID, message, exceptionObject));
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void LogFormat (LogLevel logLevel, int? eventID, Exception exceptionObject, string format, params object[] args)
    {
      var level = Convert (logLevel);
      if (_logger.IsEnabledFor (level))
        _logger.Log (CreateLoggingEvent (level, eventID, new SystemStringFormat (CultureInfo.InvariantCulture, format, args), exceptionObject));
    }

    /// <inheritdoc />
    public bool IsEnabled (LogLevel logLevel)
    {
      return _logger.IsEnabledFor (Convert (logLevel));
    }

    private LoggingEvent CreateLoggingEvent (Level level, int? eventID, object message, Exception exceptionObject)
    {
      LoggingEvent loggingEvent = new LoggingEvent (typeof (Log4NetLog), null, _logger.Name, level, message, exceptionObject);

      if (eventID.HasValue)
      {
        if (eventID < 0 || eventID > 0xFFFF)
        {
          LogLoggingError (eventID.Value, exceptionObject, message);

          throw new ArgumentOutOfRangeException (
             "eventID", string.Format ("An event id of value {0} is not supported. Valid event ids must be within a range of 0 and 65535.", eventID));
        }

        loggingEvent.Properties["EventID"] = eventID;
      }

      return loggingEvent;
    }

    private void LogLoggingError (int eventID, Exception exceptionObject, object message)
    {
      int safeEventID;
      if (eventID < 0)
        safeEventID  = 0;
      else if (eventID > 0xFFFF)
        safeEventID  = 0xFFFF;
      else
        safeEventID = eventID;

      Level level = Level.Error;
      if (_logger.IsEnabledFor (level))
      {
        if (_logger.IsEnabledFor (level))
        {
          _logger.Log (
              CreateLoggingEvent (
                  level,
                  safeEventID,
                  new SystemStringFormat (
                      CultureInfo.InvariantCulture,
                      "Failure during logging of message:\r\n{0}\r\nEvent ID: {1}",
                      new object[] { message.ToString(), eventID }),
                  exceptionObject));
        }
      }
    }
  }
}
