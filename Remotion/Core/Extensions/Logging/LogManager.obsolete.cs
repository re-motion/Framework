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
using Remotion.Obsolete;

namespace Remotion.Logging
{
  /// <summary>
  /// Use this class to create a logger implementing <see cref="ILogger"/>.
  /// </summary>
  public static class LogManager
  {
    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="name">The name of the logger to retrieve.</param>
    /// <returns>A logger for the <paramref name="name"/> specified.</returns>
    [Obsolete("Use LazyLoggerFactory.CreateLogger(name) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
    public static ILogger GetLogger (string name)
    {
      return LazyLoggerFactory.CreateLogger(name);
    }

    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="type">The full name of <paramref name="type"/> will be used as the name of the logger to retrieve.</param>
    /// <returns>A logger for the fully qualified name of the <paramref name="type"/> specified.</returns>
    [Obsolete("Use LazyLoggerFactory.CreateLogger(type) instead. (Version 7.0.0)", DiagnosticId = ObsoleteDiagnosticIDs.LoggingUtility)]
    public static ILogger GetLogger (Type type)
    {
      return LazyLoggerFactory.CreateLogger(type);
    }

    /// <summary>
    /// Initializes the current logging framework.
    /// </summary>
    [Obsolete("This API is not supported by Microsoft Logging. Use log4net.Config.XmlConfigurator.Configure() directly when depending on log4net. (Version 7.0.0)", true)]
    public static void Initialize ()
    {
      throw new NotSupportedException(
          "This API is not supported by Microsoft Logging. Use log4net.Config.XmlConfigurator.Configure() directly when depending on log4net. (Version 7.0.0)");
    }

    /// <summary>
    /// Initializes the current logging framework to log to the console.
    /// </summary>
    [Obsolete("This API is not supported by Microsoft Logging. Use log4net.Config.XmlConfigurator.Configure() directly when depending on log4net. (Version 7.0.0)", true)]
    public static void InitializeConsole ()
    {
      throw new NotSupportedException(
          "This API is not supported by Microsoft Logging. Use log4net.Config.XmlConfigurator.Configure() directly when depending on log4net. (Version 7.0.0)");
    }
  }
}
