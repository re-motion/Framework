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
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  public class BrowserHelper
  {
    public readonly IBrowserConfiguration BrowserConfiguration;

    public BrowserHelper (IBrowserConfiguration browserConfiguration)
    {
      ArgumentUtility.CheckNotNull("browserConfiguration", browserConfiguration);

      BrowserConfiguration = browserConfiguration;
    }

    /// <summary>
    /// Returns the window bounds of the window associated with the specified <paramref name="browserWindow"/>.
    /// </summary>
    public Rectangle GetWindowBounds ([NotNull] BrowserWindow browserWindow)
    {
      ArgumentUtility.CheckNotNull("browserWindow", browserWindow);

      return GetWindowBounds(browserWindow.GetWebDriver());
    }

    /// <summary>
    /// Returns the window bounds of the window associated with the specified <paramref name="browserSession"/>.
    /// </summary>
    public Rectangle GetWindowBounds ([NotNull] IBrowserSession browserSession)
    {
      ArgumentUtility.CheckNotNull("browserSession", browserSession);

      return GetWindowBounds((IWebDriver)browserSession.Driver.Native);
    }

    /// <summary>
    /// Returns the window bounds of the window associated with the specified <paramref name="controlObject"/>.
    /// </summary>
    public Rectangle GetWindowBounds ([NotNull] ControlObject controlObject)
    {
      ArgumentUtility.CheckNotNull("controlObject", controlObject);

      return GetWindowBounds(((IWrapsDriver)controlObject.Scope.Native).WrappedDriver);
    }

    /// <summary>
    /// Returns the window bounds of the window associated with the specified <paramref name="element"/>.
    /// </summary>
    public Rectangle GetWindowBounds ([NotNull] ElementScope element)
    {
      ArgumentUtility.CheckNotNull("element", element);

      return GetWindowBounds(((IWrapsDriver)element.Native).WrappedDriver);
    }

    /// <summary>
    /// Returns the window bounds of the window associated with the specified <paramref name="webElement"/>.
    /// </summary>
    public Rectangle GetWindowBounds ([NotNull] IWebElement webElement)
    {
      ArgumentUtility.CheckNotNull("webElement", webElement);

      return GetWindowBounds(((IWrapsDriver)webElement).WrappedDriver);
    }

    /// <summary>
    /// Returns the window bounds of the window associated with the specified <paramref name="webDriver"/>.
    /// </summary>
    public Rectangle GetWindowBounds ([NotNull] IWebDriver webDriver)
    {
      ArgumentUtility.CheckNotNull("webDriver", webDriver);

      var window = webDriver.Manage().Window;
      return new Rectangle(window.Position, window.Size);
    }

    /// <summary>
    /// Moves the browser window to the specified <paramref name="location"/>.
    /// </summary>
    public void MoveBrowserWindowTo ([NotNull] BrowserWindow window, Point location)
    {
      ArgumentUtility.CheckNotNull("window", window);

      var driver = window.GetWebDriver();
      driver.Manage().Window.Position = location;
    }

    /// <summary>
    /// Resizes the browser window to the specified <paramref name="size"/>.
    /// </summary>
    public void ResizeBrowserWindowTo ([NotNull] BrowserWindow window, Size size)
    {
      ArgumentUtility.CheckNotNull("window", window);
      if (size.Width <= 0)
        throw new ArgumentOutOfRangeException("size", "The window width can not be smaller or equal to zero.");
      if (size.Height <= 0)
        throw new ArgumentOutOfRangeException("size", "The window height can not be smaller or equal to zero.");

      var driver = window.GetWebDriver();
      driver.Manage().Window.Size = size;
    }
  }
}
