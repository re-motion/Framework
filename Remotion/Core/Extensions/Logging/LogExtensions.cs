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
using MicrosoftLogging = Microsoft.Extensions.Logging;

namespace Remotion.Logging
{
  /// <summary>
  /// Provides extension methods used for logging.
  /// </summary>
  public static class LogExtensions
  {
    private static readonly DoubleCheckedLockingContainer<IEnumerationGlobalizationService> s_globalizationService =
        new DoubleCheckedLockingContainer<IEnumerationGlobalizationService>(
            () => SafeServiceLocator.Current.GetInstance<IEnumerationGlobalizationService>());


    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the specified <paramref name="logLevel"/>, including the stack 
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="logLevel" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormatWithEnum/remarks' />
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void LogFormat (this ILog log, LogLevel logLevel, Enum messageEnum, Exception? exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("log", log);

      log.LogFormat(
          logLevel,
          Convert.ToInt32(messageEnum),
          exceptionObject,
          s_globalizationService.Value.GetEnumerationValueDisplayName(messageEnum),
          args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the specified <paramref name="logLevel"/>.
    /// </summary>
    public static void LogFormat (this ILogger logger, MicrosoftLogging.LogLevel logLevel, Enum messageEnum, Exception? exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);

      logger.Log(logLevel,s_globalizationService.Value.GetEnumerationValueDisplayName(messageEnum), exceptionObject, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the specified <paramref name="logLevel"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="logLevel"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormatWithEnum/remarks' />
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void LogFormat (this ILog log, LogLevel logLevel, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(logLevel, messageEnum, (Exception?)null, args);
    }

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Debug"/> level, including the stack 
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormatWithEnum/remarks' />
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void DebugFormat (this ILog log, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Debug, messageEnum, exceptionObject, args);
    }

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormatWithEnum/remarks' />
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void DebugFormat (this ILog log, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Debug, messageEnum, (Exception?)null, args);
    }

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Info"/> level, including the stack 
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormatWithEnum/remarks' />
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void InfoFormat (this ILog log, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Info, messageEnum, exceptionObject, args);
    }

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormatWithEnum/remarks' />
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void InfoFormat (this ILog log, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Info, messageEnum, (Exception?)null, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Warn"/> level, including the stack 
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormatWithEnum/remarks' />
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void WarnFormat (this ILog log, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Warn, messageEnum, exceptionObject, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormatWithEnum/remarks' />
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void WarnFormat (this ILog log, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Warn, messageEnum, (Exception?)null, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Error"/> level, including the stack 
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormatWithEnum/remarks' />
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void ErrorFormat (this ILog log, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Error, messageEnum, exceptionObject, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormatWithEnum/remarks' />
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void ErrorFormat (this ILog log, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Error, messageEnum, (Exception?)null, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Fatal"/> level, including the stack 
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log" or @name="exceptionObject"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormatWithEnum/remarks' />
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void FatalFormat (this ILog log, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Fatal, messageEnum, exceptionObject, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/Log/param[@name="log"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='..\doc\include\Logging\LogExtensions.xml' path='LogExtensions/LogFormatWithEnum/remarks' />
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void FatalFormat (this ILog log, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("log", log);
      log.LogFormat(LogLevel.Fatal, messageEnum, (Exception?)null, args);
    }
  }
}
