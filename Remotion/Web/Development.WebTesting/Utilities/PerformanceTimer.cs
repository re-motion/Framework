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
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Utility class to measure the performance of code and log the results.
  /// </summary>
  public class PerformanceTimer : IDisposable
  {
    private readonly ILogger _logger;
    private readonly string _message;
    private readonly Stopwatch _stopwatch;

    public PerformanceTimer ([NotNull] ILogger logger, string message)
    {
      ArgumentUtility.CheckNotNull("logger", logger);

      _logger = logger;
      _message = message;
      _stopwatch = Stopwatch.StartNew();
    }

    public void Dispose ()
    {
      _stopwatch.Stop();
      _logger.LogDebug(_message + " [took: {0}]", _stopwatch.Elapsed);
    }
  }
}
