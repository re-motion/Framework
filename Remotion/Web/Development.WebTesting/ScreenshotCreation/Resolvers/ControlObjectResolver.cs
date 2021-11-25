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
using OpenQA.Selenium;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Resolvers
{
  /// <summary>
  /// Resolves <see cref="ControlObject"/>s for screenshot annotations.
  /// </summary>
  public class ControlObjectResolver : IScreenshotElementResolver<ControlObject>
  {
    /// <summary>
    /// Singleton instance of <see cref="ControlObjectResolver"/>.
    /// </summary>
    public static readonly ControlObjectResolver Instance = new ControlObjectResolver();

    private ControlObjectResolver ()
    {
    }

    /// <inheritdoc />
    public ResolvedScreenshotElement ResolveBrowserCoordinates (ControlObject target)
    {
      ArgumentUtility.CheckNotNull("target", target);

      return WebElementResolver.Instance.ResolveBrowserCoordinates((IWebElement) target.Scope.Native);
    }

    /// <inheritdoc />
    public ResolvedScreenshotElement ResolveDesktopCoordinates (ControlObject target, IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull("target", target);
      ArgumentUtility.CheckNotNull("locator", locator);

      return WebElementResolver.Instance.ResolveDesktopCoordinates((IWebElement) target.Scope.Native, locator);
    }
  }
}