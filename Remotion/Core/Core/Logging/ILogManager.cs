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

namespace Remotion.Logging
{
  /// <summary>
  /// The <see cref="ILogManager"/> interface declares the methods available for retrieving a logger that implements
  /// <see cref="ILog"/> and initializing the respective logging framework.
  /// </summary>
  public interface ILogManager
  {
    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="name">The name of the logger to retrieve.</param>
    /// <returns>A logger for the <paramref name="name"/> specified.</returns>
    ILog GetLogger (string name);

    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="type">The full name of <paramref name="type"/> will be used as the name of the logger to retrieve.</param>
    /// <returns>A logger for the fully qualified name of the <paramref name="type"/> specified.</returns>
    ILog GetLogger (Type type);

    /// <summary>
    /// Initializes the logging framework abstracted through the <see cref="ILogManager"/> interface.
    /// </summary>
    void Initialize ();

    /// <summary>
    /// Initializes the logging framework to log to the console.
    /// </summary>
    void InitializeConsole ();

    /// <summary>
    /// Initializes the current logging framework to log to the console, configuring a given <see cref="LogLevel"/> as the default threshold as well
    /// as specific thresholds for specific loggers.
    /// </summary>
    /// <param name="defaultThreshold">The threshold for logging. Only log messages of at least this <see cref="LogLevel"/> are output to the console
    /// for all loggers not specifically configured via <paramref name="logThresholds"/>.</param>
    /// <param name="logThresholds">The thresholds for specific loggers.</param>
    void InitializeConsole (LogLevel defaultThreshold, params LogThreshold[] logThresholds);
  }
}
