// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Collections.Generic;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Remote;

namespace Remotion.Web.Development.WebTesting.BrowserSession.Remote;

public class RemoteBrowserSession : BrowserSessionBase<IRemoteBrowserConfiguration>
{
  public RemoteBrowserSession (Coypu.BrowserSession value, IRemoteBrowserConfiguration browserConfiguration, int driverProcessId, bool headless)
      : base(value, browserConfiguration, driverProcessId, headless)
  {
  }

  public override IReadOnlyCollection<BrowserLogEntry> GetBrowserLogs ()
  {
    return [];
  }

  public override void ResetBrowserLogs ()
  {
  }
}
