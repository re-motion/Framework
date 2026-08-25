// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
namespace Remotion.Web.Development.WebTesting.Playwright;

public class PlaywrightBrowserSession : IBrowserSession
{
  public BrowserWindow Window { get; }

  public PlaywrightBrowserSession (BrowserWindow window)
  {
    Window = window;
  }

  public BrowserWindow FindWindow (string windowLocator)
  {
    throw new System.NotImplementedException();
  }
}
