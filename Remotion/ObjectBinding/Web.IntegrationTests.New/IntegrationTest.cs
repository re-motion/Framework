// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.Playwright;
using Remotion.Web.Development.WebTesting.RequestErrorDetectionStrategies;

namespace Remotion.ObjectBinding.Web.IntegrationTests.New;

public class IntegrationTest : ContextTest
{
  protected T Start<T> (string userControl)
      where T : PageObject
  {
    var userControlUrl = string.Format("Controls/{0}UserControl.ascx", userControl);

    var url = string.Format("{0}ControlTest.wxe?UserControl={1}", "http://localhost:60402/", userControlUrl);

    var page = Context.NewPageAsync().GetAwaiter().GetResult();
    page.GotoAsync(url).GetAwaiter().GetResult();

    var browserWindow = new BrowserWindow(page);
    var browserSession = new PlaywrightBrowserSession(browserWindow);

    var pageObjectContext = PageObjectContext.New(browserSession, new AspNetRequestErrorDetectionStrategy(), new NullLoggerFactory());
    pageObjectContext.RequestErrorDetectionStrategy.CheckPageForError(pageObjectContext.Scope);

    return (T)Activator.CreateInstance(typeof(T), pageObjectContext);
  }

  public override Task<BrowserTypeLaunchOptions> LaunchOptionsAsync ()
  {
    return Task.FromResult(
        new BrowserTypeLaunchOptions
        {
            Headless = false
        });
  }
}
