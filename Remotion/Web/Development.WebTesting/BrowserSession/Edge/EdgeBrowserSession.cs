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
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Edge;

namespace Remotion.Web.Development.WebTesting.BrowserSession.Edge
{
  /// <summary>
  /// Implements <see cref="IBrowserSession"/> for the Edge browser.
  /// </summary>
  public class EdgeBrowserSession : BrowserSessionBase<IEdgeConfiguration>
  {
    private readonly IReadOnlyCollection<IBrowserSessionCleanUpStrategy> _cleanUpStrategies;

    public EdgeBrowserSession (
        [NotNull] Coypu.BrowserSession value,
        [NotNull] IEdgeConfiguration configuration,
        int driverProcessID,
        [CanBeNull] [ItemNotNull] IReadOnlyCollection<IBrowserSessionCleanUpStrategy> cleanUpStrategies = null)
        : base (value, configuration, driverProcessID)
    {
      _cleanUpStrategies = cleanUpStrategies ?? new IBrowserSessionCleanUpStrategy[0];
    }

    /// <inheritdoc />
    public override IReadOnlyCollection<BrowserLogEntry> GetBrowserLogs ()
    {
      return ((IWebDriver) Driver.Native).Manage().Logs.GetLog (LogType.Browser)
          .Select (logEntry => new BrowserLogEntry (logEntry))
          .ToArray();
    }

    /// <inheritdoc />
    public override void Dispose ()
    {
      base.Dispose();

      foreach (var cleanupStrategy in _cleanUpStrategies)
        cleanupStrategy.CleanUp();
    }
  }
}