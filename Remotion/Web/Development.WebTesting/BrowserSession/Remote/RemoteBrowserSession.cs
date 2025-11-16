// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Coypu.Drivers;
using Remotion.Web.Development.WebTesting.BrowserSession.Chrome;
using Remotion.Web.Development.WebTesting.BrowserSession.Edge;
using Remotion.Web.Development.WebTesting.BrowserSession.Firefox;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Remote;

namespace Remotion.Web.Development.WebTesting.BrowserSession.Remote;

/// <summary>
/// Implements <see cref="IBrowserSession"/> for Selenium's remote driver.
/// Instead of using a local browser, a remote browser provided by Selenium Grid is automated.
/// </summary>
public class RemoteBrowserSession : BrowserSessionBase<IRemoteBrowserConfiguration>
{
  public RemoteBrowserSession (Coypu.BrowserSession value, IRemoteBrowserConfiguration browserConfiguration, int driverProcessId, bool headless)
      : base(value, browserConfiguration, driverProcessId, headless)
  {
    // The remote browser is still one of the supported browsers, which is why we want to apply
    // the features from the corresponding browser session.
    var browser = browserConfiguration.Browser;
    if (browser == Browser.Chrome)
    {
      ChromeBrowserSession.ApplyDefaultWebTestFeatures(FeaturesMutable, this);
    }
    else if (browser == Browser.Edge)
    {
      EdgeBrowserSession.ApplyDefaultWebTestFeatures(FeaturesMutable, this);
    }
    else if (browser == Browser.Firefox)
    {
      FirefoxBrowserSession.ApplyDefaultWebTestFeatures(FeaturesMutable, this);
    }
    else
    {
      throw new NotSupportedException("RemoteDriver is only supported with Chrome, Edge, or Firefox.");
    }
  }
}
