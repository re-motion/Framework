// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;

namespace Remotion.Logging
{
  [Obsolete("Dummy declaration for DependDB. Moved to Remotion.Extensions.dll", true)]
  public static class LogManager
  {
    public static ILog GetLogger (string name)
    {
      throw new NotImplementedException();
    }

    public static ILog GetLogger (Type type)
    {
      throw new NotImplementedException();
    }

    public static void Initialize ()
    {
      throw new NotImplementedException();
    }

    public static void InitializeConsole ()
    {
      throw new NotImplementedException();
    }

    public static void InitializeConsole (LogLevel defaultThreshold, params LogThreshold[] logThresholds)
    {
      throw new NotImplementedException();
    }
  }
}
