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
using Remotion.Utilities;

namespace Remotion.Logging;

/// <summary>
/// Wrapper for <see cref="ILogger"/> that defers getting the logger until the first logging operation is performed. Use <see cref="LazyLoggerFactory"/> to create instances. 
/// </summary>
public class LazyLogger : ILogger
{
  private readonly Lazy<ILogger> _lazyLogger;

  public LazyLogger (Lazy<ILogger> lazyLogger)
  {
    ArgumentUtility.CheckNotNull("lazyLogger", lazyLogger);

    _lazyLogger = lazyLogger;
  }

  /// <inheritdoc />
  public void Log<TState> (LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
  {
    _lazyLogger.Value.Log(logLevel, eventId, state, exception, formatter);
  }

  /// <inheritdoc />
  public bool IsEnabled (LogLevel logLevel)
  {
    return _lazyLogger.Value.IsEnabled(logLevel);
  }

  /// <inheritdoc />
  public IDisposable? BeginScope<TState> (TState state)
      where TState : notnull
  {
    return _lazyLogger.Value.BeginScope(state);
  }
}
