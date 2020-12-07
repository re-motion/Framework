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
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.BrowserSession
{
  /// <summary>
  /// Represents a log entry from the browser console.
  /// </summary>
  public class BrowserLogEntry
  {
    /// <summary>
    /// Gets the logging level of the log entry.
    /// </summary>
    public LogLevel Level { get; }

    /// <summary>
    /// Gets the message of the log entry.
    /// </summary>
    [NotNull]
    public string Message { get; }

    /// <summary>
    /// Gets the timestamp value of the log entry.
    /// </summary>
    public DateTime Timestamp { get; }

    public BrowserLogEntry ([NotNull] LogEntry logEntry)
        : this (logEntry.Level, logEntry.Message, logEntry.Timestamp)
    {
      ArgumentUtility.CheckNotNull ("logEntry", logEntry);
    }

    public BrowserLogEntry (LogLevel level, [NotNull] string message, DateTime timestamp)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("message", message);

      Level = level;
      Message = message;
      Timestamp = timestamp;
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="BrowserLogEntry"/>.
    /// </summary>
    public override string ToString ()
    {
      return string.Format (CultureInfo.InvariantCulture, "[{0:yyyy-MM-ddTHH:mm:ssZ}] [{1}] {2}", Timestamp, Level, Message);
    }
  }
}