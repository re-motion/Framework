// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Collections.Generic;

namespace Remotion.Web.Development.WebTesting.BrowserSession;

/// <summary>
/// Provides methods for retrieving browser console log entries.
/// </summary>
public interface IBrowserLogProvider
{
  /// <summary>
  /// Returns the new browser log entries since the last call of <see cref="GetBrowserLogs"/>, <see cref="ResetBrowserLogs"/>, or
  /// the last refresh of the page, if no <see cref="GetBrowserLogs"/> or <see cref="ResetBrowserLogs"/> call was made.
  /// </summary>
  IReadOnlyCollection<BrowserLogEntry> GetBrowserLogs ();

  /// <summary>
  /// Resets the <see cref="BrowserLogEntry"/> collection returned by <see cref="GetBrowserLogs"/>.
  /// </summary>
  void ResetBrowserLogs ();
}
