// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;

namespace Remotion.Logging
{
  [Obsolete("Dummy declaration for DependDB. Moved to Remotion.Extensions.dll", true)]
  public class LogThreshold
  {
    public LogThreshold (ILog logger, LogLevel threshold)
    {
    }

    public ILog Logger
    {
      get { throw new NotImplementedException(); }
    }

    public LogLevel Threshold
    {
      get { throw new NotImplementedException(); }
    }
  }
}
