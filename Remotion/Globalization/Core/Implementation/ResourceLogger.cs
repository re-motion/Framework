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
using Microsoft.Extensions.Logging;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Used for logging globalization specfic log messages.
  /// </summary>
  /// <remarks>
  /// In log4net, the logger name is set to <c>Remotion.Globalization.Implementation.ResourceLogger</c>. The log level is <c>Debug</c>.
  /// </remarks>
  public static class ResourceLogger
  {
    private const LogLevel c_logLevel = LogLevel.Debug;
    private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger(typeof(ResourceLogger));

    public static bool IsEnabled
    {
      get { return s_logger.IsEnabled(c_logLevel); }
    }

    [StringFormatMethod("idFormat")]
    public static void LogResourceEntryNotFound (string idFormat, params object[] args)
    {
      ArgumentUtility.CheckNotNullOrEmpty("idFormat", idFormat);
      ArgumentUtility.CheckNotNullOrEmpty("args", args);

      s_logger.Log(c_logLevel, "No resource entry exists for the following element: " + idFormat, args);
    }
  }
}
