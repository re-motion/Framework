// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using OpenQA.Selenium.BiDi;

namespace Remotion.Web.Development.WebTesting.BrowserSession;

/// <summary>
/// Provides methods for retrieving the browser log entries using the Selenium BiDi APIs.
/// </summary>
/// <remarks>
/// BiDi logging currently only works in Firefox and requires web socket support to be enabled when the driver is created.
/// For Chromium browsers use <see cref="SeleniumBrowserLogProvider"/>.
/// </remarks>
public class BiDiBrowserLogProvider : IBrowserLogProvider, IDisposable
{
  private readonly ConcurrentQueue<BrowserLogEntry> _logEntries = new();
  private readonly Subscription _eventSubscription;

  public BiDiBrowserLogProvider (IBidiConnectionProvider bidiProvider)
  {
    ArgumentNullException.ThrowIfNull(bidiProvider);

    bidiProvider.OpenBidiConnection();
    _eventSubscription = bidiProvider.BiDiConnection.Log.OnEntryAddedAsync(entry => _logEntries.Enqueue(new BrowserLogEntry(entry)), new SubscriptionOptions
      {
          Timeout = bidiProvider.DefaultBidiTimeout
      }).GetAwaiter().GetResult();
}

  /// <inheritdoc />
  public IReadOnlyCollection<BrowserLogEntry> GetBrowserLogs ()
  {
    return _logEntries;
  }

  /// <inheritdoc />
  public void ResetBrowserLogs ()
  {
    _logEntries.Clear();
  }

  public void Dispose ()
  {
    try
    {
      _eventSubscription.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
    catch (Exception)
    {
      //ignored
    }
  }
}
