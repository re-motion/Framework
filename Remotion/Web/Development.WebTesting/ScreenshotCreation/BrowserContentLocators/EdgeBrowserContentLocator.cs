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
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.BrowserContentLocators
{
  /// <summary>
  /// Locates the browser content area for Edge.
  /// </summary>
  public class EdgeBrowserContentLocator : IBrowserContentLocator
  {
    private const string c_setWindowTitle =
        "var w = window; while (w.frameElement) w = w.frameElement.ownerDocument.defaultView; var t = w.document.title; w.document.title = arguments[0]; return t;";

    /// <summary>
    /// Edge exposes Chrome IDs to the automation api.
    /// </summary>
    private const string c_edgeWindowClassName = "Chrome_WidgetWin_1";

    /// <summary>
    /// Edge exposes Chrome IDs to the automation api.
    /// </summary>
    private const string c_edgeFrameworkID = "Chrome";

    public EdgeBrowserContentLocator ()
    {
    }

    /// <inheritdoc />
    public Rectangle GetBrowserContentBounds (IWebDriver driver)
    {
      ArgumentUtility.CheckNotNull("driver", driver);

      var windows = AutomationElement.RootElement.FindAll(
              TreeScope.Children,
              new AndCondition(
                  new OrCondition(
                      new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window),
                      new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane)),
                  new PropertyCondition(AutomationElement.ClassNameProperty, c_edgeWindowClassName)))
          .Cast<AutomationElement>()
          .ToArray();

      if (windows.Length == 1)
        return ResolveBoundsFromWindow(windows[0]);

      if (windows.Length == 0)
        throw new InvalidOperationException("Could not find an Edge browser window in order to resolve the bounds of the content area.");

      // If the result are ambiguous we try to find the browser by changing the window title
      var automationElement = ResolveByChangingWindowTitle(driver, windows);

      return ResolveBoundsFromWindow(automationElement);
    }

    [CanBeNull]
    private AutomationElement ResolveByChangingWindowTitle (IWebDriver driver, IReadOnlyCollection<AutomationElement> windows)
    {
      var id = Guid.NewGuid().ToString();

      var executor = (IJavaScriptExecutor) driver;
      var previousTitle = Assertion.IsNotNull(
          JavaScriptExecutor.ExecuteStatement<string>(executor, c_setWindowTitle, id),
          "The Javascript code changing and fetching the window title must not return null.");

      AutomationElement? result;
      try
      {
        result = RetryUntilValueChanges(
            () => windows.SingleOrDefault(w => w.Current.Name.StartsWith(id)),
            null,
            3,
            TimeSpan.FromMilliseconds(100));
      }
      finally
      {
        JavaScriptExecutor.ExecuteStatement<string>(executor, c_setWindowTitle, previousTitle);
      }

      if (result == null)
        throw new InvalidOperationException("Could not find a matching Edge window by changing its window title.");

      return result;
    }

    private Rectangle ResolveBoundsFromWindow (AutomationElement window)
    {
      var contentElement = RetryUntilValueChanges<AutomationElement?>(
          () => GetContentElement(window),
          null,
          5,
          TimeSpan.Zero);

      if (contentElement == null)
        throw new InvalidOperationException("Could not find the content window of the found Edge browser window.");

      // The content element must always be fetched anew. If the content element from the first query is reused, this yields wrong coordinates in some cases.
      var rawBounds = RetryUntilValueChanges(() => GetContentElement(window).Current.BoundingRectangle, Rect.Empty, 3, TimeSpan.FromMilliseconds(100));

      if (rawBounds == Rect.Empty)
        throw new InvalidOperationException("Could not resolve the bounds of the Edge browser window.");

      return new Rectangle(
          (int) Math.Round(rawBounds.X),
          (int) Math.Round(rawBounds.Y),
          (int) Math.Round(rawBounds.Width),
          (int) Math.Round(rawBounds.Height));
    }

    private AutomationElement GetContentElement (AutomationElement window)
    {
      return window.FindAll(
              TreeScope.Subtree,
              new AndCondition(
                  new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane),
                  new PropertyCondition(AutomationElement.ClassNameProperty, "View"),
                  new PropertyCondition(AutomationElement.FrameworkIdProperty, c_edgeFrameworkID)))
          .Cast<AutomationElement>()
          .Aggregate(GetElementWithLargerArea);
    }

    private TResult RetryUntilValueChanges<TResult> (Func<TResult> func, TResult value, int retries, TimeSpan interval)
    {
      for (var i = 0; i < retries; i++)
      {
        var result = func();

        if (!EqualityComparer<TResult>.Default.Equals(result, value))
          return result;

        Thread.Sleep(interval);
      }

      return value;
    }

    private AutomationElement GetElementWithLargerArea (AutomationElement firstAutomationElement, AutomationElement secondAutomationElement)
    {
      var firstRectangle = firstAutomationElement.Current.BoundingRectangle;
      var secondRectangle = secondAutomationElement.Current.BoundingRectangle;

      // Attention: These are not System.Drawing.Rectangle but System.Windows.Rect whose empty width and height are not 0 but Infinity,
      // meaning we have to handle these cases separately
      if (firstRectangle == Rect.Empty)
        return secondAutomationElement;

      if (secondRectangle == Rect.Empty)
        return firstAutomationElement;

      var firstRectangleArea = firstRectangle.Height * firstRectangle.Width;
      var secondRectangleArea = secondRectangle.Height * secondRectangle.Width;

      return firstRectangleArea >= secondRectangleArea
          ? firstAutomationElement
          : secondAutomationElement;
    }
  }
}