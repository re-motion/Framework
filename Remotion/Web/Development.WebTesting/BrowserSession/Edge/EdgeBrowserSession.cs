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
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Edge;

namespace Remotion.Web.Development.WebTesting.BrowserSession.Edge
{
  /// <summary>
  /// Implements <see cref="IBrowserSession"/> for the Edge browser.
  /// </summary>
  public class EdgeBrowserSession : BrowserSessionBase<IEdgeConfiguration>
  {
    public static void ApplyDefaultWebTestFeatures (
        WebTestFeatureCollection features,
        IBrowserSession browserSession)
    {
      ArgumentNullException.ThrowIfNull(features);
      ArgumentNullException.ThrowIfNull(browserSession);

      features.Set<IBrowserLogProvider>(new SeleniumBrowserLogProvider(browserSession.Driver));
    }

    private readonly IReadOnlyCollection<IBrowserSessionCleanUpStrategy> _cleanUpStrategies;

    public EdgeBrowserSession (
        [NotNull] Coypu.BrowserSession value,
        [NotNull] IEdgeConfiguration configuration,
        int driverProcessID,
        bool headless,
        [CanBeNull] [ItemNotNull] IReadOnlyCollection<IBrowserSessionCleanUpStrategy>? cleanUpStrategies = null)
        : base(value, configuration, driverProcessID, headless)
    {
      _cleanUpStrategies = cleanUpStrategies ?? Array.Empty<IBrowserSessionCleanUpStrategy>();

      ApplyDefaultWebTestFeatures(FeaturesMutable, this);
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
