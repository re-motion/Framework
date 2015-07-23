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
  /// Defines the log levels available when logging through the <see cref="ILog"/> interface.
  /// </summary>
  public enum LogLevel
  {
    /// <summary>
    /// The <see cref="LogLevel.Debug"/> level designates fine-grained informational events that are most useful to debug an application.
    /// </summary>
    Debug = 0,

    /// <summary>
    /// The <see cref="LogLevel.Info"/> level designates informational messages that highlight the progress of the application at coarse-grained level. 
    /// </summary>
    Info,

    /// <summary>
    /// The <see cref="LogLevel.Warn"/> level designates potentially harmful situations.
    /// </summary>
    Warn,

    /// <summary>
    /// The <see cref="LogLevel.Error"/> level designates error events that might still allow the application to continue running. 
    /// </summary>
    Error,

    /// <summary>
    /// The <see cref="LogLevel.Fatal"/> level designates very severe error events that will presumably lead the application to abort.
    /// </summary>
    Fatal
  }
}
