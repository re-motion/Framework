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
using System.Linq;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.BrowserSession.Chrome;
using Remotion.Web.Development.WebTesting.BrowserSession.Edge;
using Remotion.Web.Development.WebTesting.BrowserSession.Firefox;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Extension methods for <see cref="IBrowserSession"/> implementations.
  /// </summary>
  public static class BrowserSessionExtensions
  {
    /// <summary>
    /// Returns the new browser log entries of the <see cref="IBrowserSession"/> since the last call of <see cref="GetBrowserLogs"/> or
    /// the last refresh of the page, if no <see cref="GetBrowserLogs"/> call was made.
    /// </summary>
    /// <remarks>
    /// This method only works for Google Chrome v74 and higher and Microsoft Edge v76 and higher
    /// while other browsers only return a single log entry informing that the feature is not available.
    /// </remarks>
    public static IReadOnlyCollection<BrowserLogEntry> GetBrowserLogs ([NotNull] this IBrowserSession browserSession)
    {
      ArgumentUtility.CheckNotNull ("browserSession", browserSession);

      if (browserSession is ChromeBrowserSession || browserSession is EdgeBrowserSession)
      {
        return ((IWebDriver) browserSession.Driver.Native).Manage().Logs.GetLog (LogType.Browser)
            .Select (logEntry => new BrowserLogEntry (logEntry))
            .ToArray();
      }

      if (browserSession is FirefoxBrowserSession)
      {
        return new[] { new BrowserLogEntry (LogLevel.Info, "Firefox does not support getting browser logs.", DateTime.Now) };
      }

      throw new NotSupportedException ($"Getting browser logs of '{browserSession.GetType()}' is not supported.");
    }
  }
}