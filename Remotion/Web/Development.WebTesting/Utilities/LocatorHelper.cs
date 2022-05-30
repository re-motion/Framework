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
using System.Drawing;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Resolvers;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Provides methods for easier access to <see cref="IBrowserContentLocator"/> functionality.
  /// </summary>
  public class LocatorHelper
  {
    public readonly IBrowserConfiguration BrowserConfiguration;

    public LocatorHelper ([NotNull] IBrowserConfiguration browserConfiguration)
    {
      ArgumentUtility.CheckNotNull("browserConfiguration", browserConfiguration);

      BrowserConfiguration = browserConfiguration;
    }

    /// <summary>
    /// Returns the bounds of <paramref name="control"/> in the specified <paramref name="coordinateSystem"/>.
    /// </summary>
    public Rectangle GetBounds ([NotNull] ControlObject control, CoordinateSystem coordinateSystem)
    {
      ArgumentUtility.CheckNotNull("control", control);

      return Resolve(ControlObjectResolver.Instance, control, coordinateSystem).ElementBounds;
    }

    /// <summary>
    /// Returns the bounds of <paramref name="element"/> in the specified <paramref name="coordinateSystem"/>.
    /// </summary>
    public Rectangle GetBounds ([NotNull] ElementScope element, CoordinateSystem coordinateSystem)
    {
      ArgumentUtility.CheckNotNull("element", element);

      return Resolve(ElementScopeResolver.Instance, element, coordinateSystem).ElementBounds;
    }

    /// <summary>
    /// Returns the bounds of <paramref name="webElement"/> in the specified <paramref name="coordinateSystem"/>.
    /// </summary>
    public Rectangle GetBounds ([NotNull] IWebElement webElement, CoordinateSystem coordinateSystem)
    {
      ArgumentUtility.CheckNotNull("webElement", webElement);

      return Resolve(WebElementResolver.Instance, webElement, coordinateSystem).ElementBounds;
    }

    /// <summary>
    /// Resolves <paramref name="target"/> using the specified <paramref name="resolver"/> for the specified <paramref name="coordinateSystem"/>.
    /// </summary>
    public ResolvedScreenshotElement Resolve<T> (
        [NotNull] IScreenshotElementResolver<T> resolver,
        [NotNull] T target,
        CoordinateSystem coordinateSystem)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("resolver", resolver);
      ArgumentUtility.CheckNotNull("target", target);

      switch (coordinateSystem)
      {
        case CoordinateSystem.Browser:
          return ResolveBrowser(resolver, target);
        case CoordinateSystem.Desktop:
          return ResolveDesktop(resolver, target);
        default:
          throw new ArgumentOutOfRangeException("coordinateSystem", coordinateSystem, null);
      }
    }

    /// <summary>
    /// Resolves <paramref name="target"/> using the specified <paramref name="resolver"/> for the browser <see cref="CoordinateSystem"/>.
    /// </summary>
    public ResolvedScreenshotElement ResolveBrowser<T> ([NotNull] IScreenshotElementResolver<T> resolver, [NotNull] T target)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("resolver", resolver);
      ArgumentUtility.CheckNotNull("target", target);

      return resolver.ResolveBrowserCoordinates(target);
    }

    /// <summary>
    /// Resolves <paramref name="target"/> using the specified <paramref name="resolver"/> for the desktop <see cref="CoordinateSystem"/>.
    /// </summary>
    public ResolvedScreenshotElement ResolveDesktop<T> ([NotNull] IScreenshotElementResolver<T> resolver, [NotNull] T target)
        where T : notnull
    {
      ArgumentUtility.CheckNotNull("resolver", resolver);
      ArgumentUtility.CheckNotNull("target", target);

      return resolver.ResolveDesktopCoordinates(target, BrowserConfiguration.Locator);
    }
  }
}
