// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using Coypu;
using OpenQA.Selenium;

namespace Remotion.Web.Development.WebTesting.BrowserSession;

/// <summary>
/// Provides methods for retrieving the browser log entries using the Selenium APIs.
/// </summary>
/// <remarks>
/// Selenium logs only work in Chromium browsers (Chrome and Edge).
/// For Firefox use <see cref="BiDiBrowserLogProvider"/>.
/// </remarks>
public class SeleniumBrowserLogProvider : IBrowserLogProvider
{
  private readonly IDriver _webDriver;
  private readonly List<BrowserLogEntry> _browserLogEntries = new();

  public SeleniumBrowserLogProvider (IDriver webDriver)
  {
    ArgumentNullException.ThrowIfNull(webDriver);

    _webDriver = webDriver;
  }

  /// <inheritdoc />
  public IReadOnlyCollection<BrowserLogEntry> GetBrowserLogs ()
  {
    var newEntries = ((IWebDriver)_webDriver.Native).Manage().Logs.GetLog(LogType.Browser)
        .Select(logEntry => new BrowserLogEntry(logEntry));
    _browserLogEntries.AddRange(newEntries);
    return _browserLogEntries;
  }

  /// <inheritdoc />
  public void ResetBrowserLogs ()
  {
    GetBrowserLogs(); // fetch the pending entries so that they are cleared as well
    _browserLogEntries.Clear();
  }
}
