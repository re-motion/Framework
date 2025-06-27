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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using JetBrains.Annotations;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.WebDriver;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;

namespace Remotion.Web.Development.WebTesting.BrowserLog;

/// <summary>
/// Provides utility methods to deal with checking the browser logs at the end of web tests.
/// </summary>
public static class BrowserLogUtility
{
  /// <summary>
  /// Gets a flag indicating whether browser logs should be checked.
  /// </summary>
  public static bool IsBrowserLogCheckActive (ITestContext testContext)
  {
    ArgumentNullException.ThrowIfNull(testContext);

    return testContext.GetValueOrDefault(PerformBrowserLogCheckAttribute.PropertyKey, false);
  }

  /// <summary>
  /// Gets the minimum <see cref="LogLevel"/> below which a <see cref="BrowserLogEntry"/> is ignored. 
  /// </summary>
  public static LogLevel GetMinimumLogLevel (ITestContext testContext)
  {
    ArgumentNullException.ThrowIfNull(testContext);

    return testContext.GetValueOrDefault(BrowserLogMinimumLevelAttribute.PropertyKey, LogLevel.Off);
  }

  /// <summary>
  /// Gets the <see cref="Regex"/> filters for ignoring a <see cref="BrowserLogEntry"/> based on its <see cref="BrowserLogEntry.Message"/>.
  /// </summary>
  public static IReadOnlyCollection<Regex> GetBrowserLogIgnoredEntryRegexes (ITestContext testContext)
  {
    ArgumentNullException.ThrowIfNull(testContext);

    return testContext.GetCollection<Regex>(IgnoreBrowserLogMessageAttribute.PropertyKey);
  }

  /// <summary>
  /// Gets the browser log entries that indicate that the test should fail, because their <see cref="LogLevel"/> is at least <paramref name="minLogLevel"/>, and they do not match
  /// any <paramref name="messageFilter"/>.
  /// </summary>
  public static IReadOnlyCollection<BrowserLogEntry> GetUnexpectedBrowserLogEntries (
      [NotNull] IBrowserSession session,
      [NotNull] IBrowserConfiguration configuration,
      LogLevel minLogLevel,
      [NotNull] IReadOnlyCollection<Regex> messageFilter)
  {
    ArgumentNullException.ThrowIfNull(session);
    ArgumentNullException.ThrowIfNull(configuration);
    ArgumentNullException.ThrowIfNull(messageFilter);

    IReadOnlyCollection<BrowserLogEntry> browserLogs;
    if (configuration.UseBidiLog() && session.Window.Location.Scheme != "chrome")
    {
      // For BiDi-Logging (bidirectional logging), log messages take some time to get to us in async ways.
      // To reduce the chance of missing log messages, we send a marker log message and wait until we
      // receive it again. This ensures that any in-flight log messages should have arrived and thus provides
      // a good-enough(tm) way to ensure we don't miss any log messages.
      // BiDi-Logging does not work on internal pages (like the blank page) so we don't do this logic for chrome:// URLs
      const string browserLogMarker = "REMOTION_FINAL_BROWSER_LOG_MARKER";
      const int maxRetries = 25;

      ((IWebDriver)session.Driver.Native).ExecuteJavaScript($"console.error('{browserLogMarker}');");

      var i = 0;
      while (true)
      {
        if (i > maxRetries)
        {
          throw new InvalidOperationException(
              "Waiting for unexpected browser messages failed because"
              + " the marker log message was not returned within the timeout.");
        }

        browserLogs = session.GetBrowserLogs();
        if (browserLogs.Any(e => e.Message.Contains(browserLogMarker)))
          break;

        i += 1;
        Thread.Sleep(1);
      }

      browserLogs = browserLogs
          .Where(e => !e.Message.Contains(browserLogMarker))
          .ToList();
    }
    else
    {
      browserLogs = session.GetBrowserLogs();
    }

    return browserLogs
        .Where(e => e.Level >= minLogLevel)
        .Where(entry => !messageFilter.Any(filter => filter.IsMatch(entry.Message)))
        .ToList();
  }

  /// <summary>
  /// If browser log checking is active, this method checks whether there are unexpected browser log entries that meet the minimum <see cref="LogLevel"/>. If so, the test result
  /// is set to "failed", with a message containing the relevant browser log entries.
  /// </summary>
  /// <seealso cref="ITestContext.SetFailure"/>
  /// <remarks>
  /// Call this in the tear-down method.
  /// </remarks>
  public static bool IsBrowserLogOkay ([NotNull] IBrowserSession session, [NotNull] IBrowserConfiguration configuration, [NotNull] ITestContext context)
  {
    ArgumentNullException.ThrowIfNull(session);
    ArgumentNullException.ThrowIfNull(configuration);
    ArgumentNullException.ThrowIfNull(context);

    var isActive = IsBrowserLogCheckActive(context);
    if (!isActive)
      return true;

    var minLogLevel = GetMinimumLogLevel(context);
    var messageFilters = GetBrowserLogIgnoredEntryRegexes(context);

    var entries = GetUnexpectedBrowserLogEntries(session, configuration, minLogLevel, messageFilters);
    if (entries.Any())
    {
      var messageBuilder = new StringBuilder();
      messageBuilder.AppendLine("There are unexpected browser log entries at the end of the test:");
      messageBuilder.AppendJoin(Environment.NewLine, entries.Select(entry => entry.Message));
      context.SetFailure(messageBuilder.ToString());
      return false;
    }

    return true;
  }
}
