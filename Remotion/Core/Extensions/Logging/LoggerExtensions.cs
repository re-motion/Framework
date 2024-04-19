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
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;

using IMicrosoftLogger = Microsoft.Extensions.Logging.ILogger;
using MicrosoftLoglevel = Microsoft.Extensions.Logging.LogLevel;

namespace Remotion.Logging
{
  /// <summary>
  /// Provides extension methods used for logging.
  /// </summary>
  public static class LoggerExtensions
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
    public static void LogFormat (this IMicrosoftLogger logger, MicrosoftLoglevel logLevel, Enum messageEnum, Exception? exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);

      Microsoft.Extensions.Logging.LoggerExtensions.Log(logger, logLevel, exceptionObject, s_globalizationService.Value.GetEnumerationValueDisplayName(messageEnum), args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the specified <paramref name="logLevel"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void LogFormat (this IMicrosoftLogger logger, MicrosoftLoglevel logLevel, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(logLevel, messageEnum, null, args);
    }

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="MicrosoftLoglevel.Debug"/> level, including the stack
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>

    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void DebugFormat (this IMicrosoftLogger logger, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(MicrosoftLoglevel.Debug, messageEnum, exceptionObject, args);
    }

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="MicrosoftLoglevel.Debug"/> level.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void DebugFormat (this IMicrosoftLogger logger, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(MicrosoftLoglevel.Debug, messageEnum, (Exception?)null, args);
    }

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="MicrosoftLoglevel.Information"/> level, including the stack
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>

    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void InfoFormat (this IMicrosoftLogger logger, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(MicrosoftLoglevel.Information, messageEnum, exceptionObject, args);
    }

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="MicrosoftLoglevel.Information"/> level.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void InfoFormat (this IMicrosoftLogger logger, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(MicrosoftLoglevel.Information, messageEnum, (Exception?)null, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="MicrosoftLoglevel.Warning"/> level, including the stack
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>

    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void WarnFormat (this IMicrosoftLogger logger, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(MicrosoftLoglevel.Warning, messageEnum, exceptionObject, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="MicrosoftLoglevel.Warning"/> level.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void WarnFormat (this IMicrosoftLogger logger, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(MicrosoftLoglevel.Warning, messageEnum, (Exception?)null, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="MicrosoftLoglevel.Error"/> level, including the stack
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>

    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void ErrorFormat (this IMicrosoftLogger logger, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(MicrosoftLoglevel.Error, messageEnum, exceptionObject, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="MicrosoftLoglevel.Error"/> level.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void ErrorFormat (this IMicrosoftLogger logger, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(MicrosoftLoglevel.Error, messageEnum, (Exception?)null, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="MicrosoftLoglevel.Critical"/> level, including the stack
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>

    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void CriticalFormat (this IMicrosoftLogger logger, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(MicrosoftLoglevel.Critical, messageEnum, exceptionObject, args);
    }

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="MicrosoftLoglevel.Critical"/> level.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer.
    /// </exception>
    public static void CriticalFormat (this IMicrosoftLogger logger, Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      logger.LogFormat(MicrosoftLoglevel.Critical, messageEnum, (Exception?)null, args);
    }
  }
}
