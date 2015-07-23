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
using System.Reflection;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Various extension methods for Coypu's <see cref="BrowserWindow"/> class.
  /// </summary>
  public static class CoypuBrowserWindowExtensions
  {
    /// <summary>
    /// Returns the scope of the &lt;html&gt; element (= the root scope) on the given window.
    /// </summary>
    public static ElementScope GetRootScope ([NotNull] this BrowserWindow window)
    {
      ArgumentUtility.CheckNotNull ("window", window);

      return window.FindCss ("html");
    }

    /// <summary>
    /// Closes the window specified in the given <paramref name="context"/>.
    /// </summary>
    public static void CloseWindow ([NotNull] this PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      context.Window.ExecuteScript (CommonJavaScripts.SelfClose);
      EnsureParentWindowIsActive (context);
    }

    /// <summary>
    /// Ensures that Coypu's active window is the given <paramref name="window"/>.
    /// </summary>
    public static void EnsureWindowIsActive ([NotNull] this BrowserWindow window)
    {
      // ReSharper disable once UnusedVariable
      var temp = window.Title;
    }

    /// <summary>
    /// Returns the native Selenium <see cref="IWebDriver"/> instance which correspons to the current window.
    /// </summary>
    public static IWebDriver GetNativeFromBrowserWindow ([NotNull] this BrowserWindow window)
    {
      ArgumentUtility.CheckNotNull ("window", window);

      var driverFieldInfo = typeof (BrowserWindow).GetField ("driver", BindingFlags.NonPublic | BindingFlags.Instance);
      Assertion.IsNotNull (driverFieldInfo, "Coypu has changed, please update CoypuBrowserWindowExtensions.GetNativeFromBrowserWindow() method.");

      var driver = (Driver) driverFieldInfo.GetValue (window);
      return (IWebDriver) driver.Native;
    }

    /// <summary>
    /// WebDriver implementations hang when trying to find an element within an <see cref="ElementScope"/> which is on an already closed window.
    /// Therefore, after closing a window via non-Coypu means, it is important to ensure that a non-closed window is active.
    /// </summary>
    private static void EnsureParentWindowIsActive ([NotNull] this PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      context.ParentContext.Window.EnsureWindowIsActive();
    }
  }
}