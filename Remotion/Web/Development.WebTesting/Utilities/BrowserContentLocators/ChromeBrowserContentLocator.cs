﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using OpenQA.Selenium;

namespace Remotion.Web.Development.WebTesting.Utilities.BrowserContentLocators
{
  /// <summary>
  /// Locates the browser content area for Chrome.
  /// </summary>
  public class ChromeBrowserContentLocator : IBrowserContentLocator
  {
    [DllImport ("user32.dll")]
    private static extern IntPtr GetForegroundWindow ();

    [DllImport ("user32.dll", SetLastError = true)]
    static extern uint GetWindowThreadProcessId (IntPtr handle, out uint processID);

    public ChromeBrowserContentLocator ()
    {
    }

    /// <inheritdoc />
    public Rectangle GetBrowserContentBounds (IWebDriver driver)
    {
      // Chrome does not support getting the content area from JS
      // which is why we need to search the Automation tree for the
      // correct browser window in order to retrieve the content area

      var foregroundWindowHandle = GetForegroundWindow();
      uint processID;
      if (foregroundWindowHandle != IntPtr.Zero)
        GetWindowThreadProcessId (foregroundWindowHandle, out processID);
      else
        processID = 0;

      var windows = AutomationElement.RootElement.FindAll (
          TreeScope.Children,
          new AndCondition (
              new PropertyCondition (AutomationElement.ControlTypeProperty, ControlType.Window),
              new PropertyCondition (AutomationElement.ClassNameProperty, "Chrome_WidgetWin_1")))
          .Cast<AutomationElement>()
          .Select (w => RateWindow (driver, w, (int) processID))
          .ToArray();

      if (windows.Length == 1)
        return ResolveBoundsFromWindow (windows[0].Value);

      var highestRating = windows.Max (w => w.Key);
      var results = windows.Where (w => w.Key == highestRating).Take (2).ToArray();
      if (windows.Length == 0 || highestRating == 0 || results.Length == 2)
        throw new InvalidOperationException ("Could not find a chrome window in order to resolve the bounds of the content area.");

      return ResolveBoundsFromWindow (results[0].Value);
    }

    private Rectangle ResolveBoundsFromWindow (AutomationElement window)
    {
      var element = window.FindFirst (
          TreeScope.Children,
          new AndCondition (
              new PropertyCondition (AutomationElement.NameProperty, "Chrome Legacy Window"),
              new PropertyCondition (AutomationElement.ClassNameProperty, "Chrome_RenderWidgetHostHWND")));
      if (element == null)
        throw new InvalidOperationException ("Can not find the content window of the found chrome browser window.");

      var rawBounds = element.Current.BoundingRectangle;
      return new Rectangle (
          (int) Math.Round (rawBounds.X),
          (int) Math.Round (rawBounds.Y),
          (int) Math.Round (rawBounds.Width),
          (int) Math.Round (rawBounds.Height));
    }

    private KeyValuePair<int, AutomationElement> RateWindow (IWebDriver driver, AutomationElement automationWindow, int processID)
    {
      var rating = 0;

      // Check if the title matches
      var name = automationWindow.Current.Name;
      if (name == driver.Title || name == driver.Url)
        rating += 2;
      else if (name.Contains (driver.Url))
        rating += 1;

      // Check if the bounds match the ones specified by the driver
      var rawBounds = automationWindow.Current.BoundingRectangle;
      var bounds = new Rectangle (
          (int) Math.Round (rawBounds.X),
          (int) Math.Round (rawBounds.Y),
          (int) Math.Round (rawBounds.Width),
          (int) Math.Round (rawBounds.Height));

      var window = driver.Manage().Window;
      var windowBounds = new Rectangle (window.Position, window.Size);
      if (bounds == windowBounds)
        rating += 2;

      // Check if the window belongs to the right process
      if (processID != 0 && automationWindow.Current.ProcessId == processID)
        rating += 4;

      return new KeyValuePair<int, AutomationElement> (rating, automationWindow);
    }
  }
}