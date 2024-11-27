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
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Resolvers
{
  /// <summary>
  /// Resolves <see cref="Rectangle"/>s for screenshot annotations.
  /// </summary>
  public class RectangleResolver : IScreenshotElementResolver<Rectangle>
  {
    /// <summary>
    /// Singleton instance of <see cref="RectangleResolver"/>.
    /// </summary>
    public static readonly RectangleResolver Instance = new RectangleResolver();

    private readonly IWebDriver? _driver;
    private readonly bool _relative;

    private RectangleResolver ()
    {
      _driver = null;
      _relative = false;
    }

    public RectangleResolver ([NotNull] IWebDriver driver)
    {
      ArgumentUtility.CheckNotNull("driver", driver);

      _driver = driver;
      _relative = true;
    }

    /// <inheritdoc />
    public ResolvedScreenshotElement ResolveBrowserCoordinates (Rectangle target)
    {
      var unresolvedBounds = target;

      return new ResolvedScreenshotElement(CoordinateSystem.Browser, target, ElementVisibility.FullyVisible, null, unresolvedBounds);
    }

    /// <inheritdoc />
    public ResolvedScreenshotElement ResolveDesktopCoordinates (Rectangle target, IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull("locator", locator);
      Assertion.IsNotNull(_driver, "'{0}' must not be null when resolving the desktop coordinates.", nameof(_driver));

      var unresolvedBounds = target;

      if (_relative)
        target.Offset(locator.GetBrowserContentBounds(_driver).Location);

      return new ResolvedScreenshotElement(CoordinateSystem.Desktop, target, ElementVisibility.FullyVisible, null, unresolvedBounds);
    }
  }
}
