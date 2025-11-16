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
using System.Collections.ObjectModel;
using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Provides a default implementation for <see cref="IBrowserContentLocator"/> that works via JavaScript.
  /// Does not return a position component for the browser content bounds as this is not possible via JavaScript.
  /// </summary>
  public class DefaultBrowserContentLocator : IBrowserContentLocator
  {
    public static readonly DefaultBrowserContentLocator Instance = new();

    private DefaultBrowserContentLocator ()
    {
    }

    /// <inheritdoc />
    public Rectangle GetBrowserContentBounds (IWebDriver driver)
    {
      var dimensions = driver.ExecuteJavaScript<ReadOnlyCollection<object>>("return [window.innerWidth, window.innerHeight]");
      if (dimensions == null)
        throw new InvalidOperationException("Cannot determine the browser content size.");

      return new Rectangle(
          0,
          0,
          (int)(long)dimensions[0],
          (int)(long)dimensions[1]);
    }
  }
}
