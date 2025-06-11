// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Collections.Concurrent;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.BiDi;
using OpenQA.Selenium.BiDi.Modules.BrowsingContext;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Remote;

namespace Remotion.Web.Development.WebTesting.BrowserSession.Remote;

public class RemoteBrowserSession : BrowserSessionBase<IRemoteBrowserConfiguration>
{
    private readonly ConcurrentQueue<BrowserLogEntry> _logEntries;
    private readonly Subscription _entryAddedSubscription;

  public RemoteBrowserSession (Coypu.BrowserSession value, IRemoteBrowserConfiguration browserConfiguration, int driverProcessId, bool headless)
      : base(value, browserConfiguration, driverProcessId, headless)
  {
      _logEntries = new ConcurrentQueue<BrowserLogEntry>();

      var bidi = ((IWebDriver)Driver.Native).AsBiDiAsync().GetAwaiter().GetResult();
      _entryAddedSubscription = bidi.Log.OnEntryAddedAsync(entry => _logEntries.Enqueue(new BrowserLogEntry(entry))).GetAwaiter().GetResult();

      // Accept all user prompts as they come up - IWebTestHelper.AcceptPossibleModalDialog() does not work with BiDi
      // because the WebTest-Thread is not continued when a user prompt is shown.
      bidi.BrowsingContext.OnUserPromptOpenedAsync(args => args.BiDi.BrowsingContext.HandleUserPromptAsync(args.Context, new HandleUserPromptOptions { Accept = true })).Wait();
  }

  public override IReadOnlyCollection<BrowserLogEntry> GetBrowserLogs ()
  {
    return _logEntries;
  }

  public override void ResetBrowserLogs ()
  {
    _logEntries.Clear();
  }
}
