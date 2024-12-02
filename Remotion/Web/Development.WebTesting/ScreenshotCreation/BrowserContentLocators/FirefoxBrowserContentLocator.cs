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
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.BrowserContentLocators
{
  /// <summary>
  /// Locates the browser content area for Firefox.
  /// </summary>
  public class FirefoxBrowserContentLocator : IBrowserContentLocator
  {
    public Rectangle GetBrowserContentBounds (IWebDriver driver)
    {
#if PLATFORM_WINDOWS
      ArgumentUtility.CheckNotNull("driver", driver);

      var firefoxWindows = AutomationElement.RootElement.FindAll(
              TreeScope.Children,
              new AndCondition(
                  new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window),
                  new PropertyCondition(AutomationElement.ClassNameProperty, "MozillaWindowClass")))
          .Cast<AutomationElement>()
          .Select(window => RateAutomationElement(window, driver))
          .ToArray();

      var maxScore = firefoxWindows.Max(t => t.Item1);
      var bestFit = firefoxWindows.FirstOrDefault(t => t.Item1 == maxScore);

      if (bestFit == null)
        throw new InvalidOperationException("No Matching Firefox window could be found.");

      var firefoxContentElement = GetFirefoxContentElement(bestFit.Item2);

      var firefoxContentBounds = firefoxContentElement.Current.BoundingRectangle;

      var result = new Rectangle(
          (int)Math.Round(firefoxContentBounds.X),
          (int)Math.Round(firefoxContentBounds.Y),
          (int)Math.Round(firefoxContentBounds.Width),
          (int)Math.Round(firefoxContentBounds.Height));

      return result;
#else
      throw new PlatformNotSupportedException("FirefoxBrowserContentLocator is only supported on Windows.");
#endif
    }
#if PLATFORM_WINDOWS
    /// <summary>
    /// Despite the browser being ready, the automation tree used to query Firefox' content location and bounds
    /// sometimes yields outdated/not ready results, which leads to <see cref="AutomationElement.FindFirst"/>
    /// returning <see langword="null"/>, or throwing an <see cref="ElementNotAvailableException"/>. Therefore
    /// retries are performed.
    /// </summary>
    [NotNull]
    private AutomationElement GetFirefoxContentElement (AutomationElement firefoxWindow)
    {
      for (var i = 0; i < 11; i++)
      {
        try
        {
          var result = GetFirefoxDocumentControl(firefoxWindow);

          if (result != null)
            return result;
        }
        catch (ElementNotAvailableException)
        {
        }

        Thread.Sleep(TimeSpan.FromMilliseconds(200));
      }

      throw new InvalidOperationException("The Firefox content window could not be found within a timeout of 2 seconds.");
    }

    [CanBeNull]
    private AutomationElement? GetFirefoxDocumentControl (AutomationElement firefoxWindow)
    {
      return firefoxWindow.FindFirst(
          TreeScope.Subtree,
          new PropertyCondition(
              AutomationElement.ControlTypeProperty,
              ControlType.Document));
    }

    private Tuple<int, AutomationElement> RateAutomationElement (AutomationElement window, IWebDriver driver)
    {
      var rating = 0;

      rating += GetFocusedWindowRating(window);
      rating += GetDriverWindowSizeRating(window, driver);

      return new Tuple<int, AutomationElement>(rating, window);
    }

    private int GetFocusedWindowRating (AutomationElement window)
    {
      var focusedWindow = AutomationElement.FocusedElement;
      if (window.Equals(focusedWindow))
        return 2;
      return 0;
    }

    private int GetDriverWindowSizeRating (AutomationElement window, IWebDriver driver)
    {
      var driverWindow = driver.Manage().Window;
      var driverWindowBounds = new Rectangle(driverWindow.Position, driverWindow.Size);

      var windowBoundingRectangle = window.Current.BoundingRectangle;
      var windowBounds = new Rectangle(
          (int)Math.Round(windowBoundingRectangle.X),
          (int)Math.Round(windowBoundingRectangle.Y),
          (int)Math.Round(windowBoundingRectangle.Width),
          (int)Math.Round(windowBoundingRectangle.Height));

      if (driverWindowBounds == windowBounds)
        return 2;
      return 0;
    }
#endif
  }
}
