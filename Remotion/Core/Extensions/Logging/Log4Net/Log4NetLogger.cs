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
using log4net.Core;
using Microsoft.Extensions.Logging;

namespace Remotion.Logging.Log4Net;

public class Log4NetLogger
    : Microsoft.Extensions.Logging.ILogger
{
  public void Log<TState> (Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
  {
    throw new NotImplementedException();
  }

  public bool IsEnabled (Microsoft.Extensions.Logging.LogLevel logLevel)
  {
    throw new NotImplementedException();
  }

  public IDisposable? BeginScope<TState> (TState state)
      where TState : notnull
  {
    throw new NotImplementedException();
  }
}
