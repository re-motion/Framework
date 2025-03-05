// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.BiDi;
using OpenQA.Selenium.BiDi.Modules.BrowsingContext;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Firefox;

namespace Remotion.Web.Development.WebTesting.BrowserSession.Firefox
{
  /// <summary>
  /// Implements <see cref="IBrowserSession"/> for the Firefox browser.
  /// </summary>
  public class FirefoxBrowserSession : BrowserSessionBase<IFirefoxConfiguration>
  {
    private readonly ConcurrentQueue<BrowserLogEntry> _logEntries;
    private readonly Subscription _entryAddedSubscription;

    public FirefoxBrowserSession (Coypu.BrowserSession value, IFirefoxConfiguration browserConfiguration, int driverProcessId, bool headless)
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

    public override void Dispose ()
    {
      _entryAddedSubscription.DisposeAsync().GetAwaiter().GetResult();

      var driver = (IWebDriver)Driver.Native;
      var biDi = driver.AsBiDiAsync().GetAwaiter().GetResult();
      biDi.DisposeAsync().GetAwaiter().GetResult();

      base.Dispose();
    }
  }
}
