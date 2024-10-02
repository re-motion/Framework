// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;

namespace Remotion.Logging
{
  [Obsolete("Dummy declaration for DependDB. Moved to Remotion.Extensions.dll", true)]
  public interface ILog
  {
    void Log (LogLevel logLevel, int? eventID, object message, Exception exceptionObject);

    void LogFormat (LogLevel logLevel, int? eventID, Exception exceptionObject, string format, params object[] args);

    bool IsEnabled (LogLevel logLevel);
  }
}
