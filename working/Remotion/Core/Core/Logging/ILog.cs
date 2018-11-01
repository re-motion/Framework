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
using JetBrains.Annotations;

namespace Remotion.Logging
{
  /// <summary>
  /// The <see cref="ILog"/> interface declares methods for logging messages.
  /// </summary>
  /// <remarks>
  ///     The <see cref="ILog"/> interface is intended for implementing adapters to various logging frameworks.
  ///   <note>
  ///     The range of valid event ids is only guarenteed within the range of unsigned 16-bit integers.
  ///   </note>
  ///   <note type="inheritinfo">
  ///     Implementors must support event ids within the range of unsigned 16-bit integers. The behavior outside this range is can be either truncation 
  ///     of the event id or an <see cref="ArgumentOutOfRangeException"/>. Implementors must ensure that the log message is logged before an 
  ///     <see cref="ArgumentOutOfRangeException"/> is thrown.
  ///  </note>
  /// </remarks>
  public interface ILog
  {
    /// <summary>
    /// Log a message object with the specified <paramref name="logLevel"/> and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/> of the message to be logged.</param>
    /// <param name="eventID">The numeric identifier for the event.</param>
    /// <param name="message">The message object to log.</param>
    /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
    void Log (LogLevel logLevel, int? eventID, object message, Exception exceptionObject);

    /// <summary>
    /// Log a formatted string with the specified <paramref name="logLevel"/> and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/> of the message to be logged.</param>
    /// <param name="eventID">The numeric identifier for the event.</param>
    /// <param name="format">A String containing zero or more format items.</param>
    /// <param name="args">An Object array containing zero or more objects to format.</param>
    /// <param name="exceptionObject">The <see cref="Exception"/> to log, including its stack trace. Pass <see langword="null"/> to not log an exception.</param>
    [StringFormatMethod ("format")]
    void LogFormat (LogLevel logLevel, int? eventID, Exception exceptionObject, string format, params object[] args);

    /// <summary>
    /// Checks if this logger is enabled for the given <see cref="LogLevel"/>.
    /// </summary>
    /// <param name="logLevel">The log level to check for.</param>
    /// <returns>
    ///   <see langword="true"/> if the specified log level is enabled; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsEnabled (LogLevel logLevel);
  }
}
