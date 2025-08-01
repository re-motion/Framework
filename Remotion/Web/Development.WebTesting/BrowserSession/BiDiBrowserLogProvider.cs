// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Coypu;
using OpenQA.Selenium;
using OpenQA.Selenium.BiDi;
using OpenQA.Selenium.BiDi.BrowsingContext;

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
  private readonly IDriver _driver;

  private readonly ConcurrentQueue<BrowserLogEntry> _logEntries = new();
  private readonly Subscription _eventSubscription;

  public BiDiBrowserLogProvider (IDriver driver)
  {
    ArgumentNullException.ThrowIfNull(driver);

    _driver = driver;

    var bidi = ((IWebDriver)driver.Native).AsBiDiAsync().GetAwaiter().GetResult();
    _eventSubscription = bidi.Log.OnEntryAddedAsync(entry => _logEntries.Enqueue(new BrowserLogEntry(entry))).GetAwaiter().GetResult();

    // Accept all user prompts as they come up - IWebTestHelper.AcceptPossibleModalDialog() does not work with BiDi
    // because the WebTest-Thread is not continued when a user prompt is shown.
    bidi.BrowsingContext.OnUserPromptOpenedAsync(args => args.BiDi.BrowsingContext.HandleUserPromptAsync(args.Context, new HandleUserPromptOptions { Accept = true })).Wait();
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
    _eventSubscription.DisposeAsync().AsTask().GetAwaiter().GetResult();

    var driver = (IWebDriver)_driver.Native;
    var biDi = driver.AsBiDiAsync().GetAwaiter().GetResult();
    biDi.DisposeAsync().GetAwaiter().GetResult();
  }
}
