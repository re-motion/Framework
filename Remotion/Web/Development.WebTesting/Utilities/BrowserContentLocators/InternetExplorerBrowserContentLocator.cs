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
using System.Linq;
using OpenQA.Selenium;

namespace Remotion.Web.Development.WebTesting.Utilities.BrowserContentLocators
{
  /// <summary>
  /// Locates the browser content area for InternetExplorer.
  /// </summary>
  public class InternetExplorerBrowserContentLocator : IBrowserContentLocator
  {
    private const string c_getWindowRectangleJs =
        "var w = window; while (w && w.frameElement) w = w.frameElement.ownerDocument.defaultView; return [w.screenLeft, w.screenTop, w.innerWidth, w.innerHeight];";

    public InternetExplorerBrowserContentLocator ()
    {
    }

    /// <inheritdoc />
    public Rectangle GetBrowserContentBounds (IWebDriver driver)
    {
      var jsExecutor = (IJavaScriptExecutor) driver;
      var data = ((ReadOnlyCollection<object>) jsExecutor.ExecuteScript (c_getWindowRectangleJs)).Select (e => (int) (long) e).ToArray();
      if (data.Length != 4)
        throw new InvalidOperationException ("JS script for getting the IE content bounds is outdated.");
      return new Rectangle (data[0], data[1], data[2], data[3]);
    }
  }
}