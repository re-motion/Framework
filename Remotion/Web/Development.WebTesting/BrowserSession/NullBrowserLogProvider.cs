// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using Remotion.Web.Development.WebTesting.BrowserLog;

namespace Remotion.Web.Development.WebTesting.BrowserSession;

/// <summary>
/// This class implements a <see cref="IBrowserLogProvider"/> that does nothing.
/// Calls to <see cref="GetBrowserLogs"/> will always return an empty list and <see cref="ResetBrowserLogs"/> does nothing.
/// </summary>
public class NullBrowserLogProvider: IBrowserLogProvider
{
  public IReadOnlyCollection<BrowserLogEntry> GetBrowserLogs () => [
    new BrowserLogEntry(LogLevel.Severe, BrowserLogUtility.BrowserLogMarker, DateTime.UnixEpoch)
  ];

  public void ResetBrowserLogs ()
  {
  }
}
