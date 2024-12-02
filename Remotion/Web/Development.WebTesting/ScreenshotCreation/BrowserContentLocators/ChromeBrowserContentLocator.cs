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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.BrowserContentLocators
{
  /// <summary>
  /// Locates the browser content area for Chrome.
  /// </summary>
  public class ChromeBrowserContentLocator : IBrowserContentLocator
  {
    private const string c_setWindowTitle =
        "var w = window; while (w.frameElement) w = w.frameElement.ownerDocument.defaultView; var t = w.document.title; w.document.title = arguments[0]; return t;";
    public ChromeBrowserContentLocator ()
    {
    }

    /// <inheritdoc />
    public Rectangle GetBrowserContentBounds (IWebDriver driver)
    {
      ArgumentUtility.CheckNotNull("driver", driver);

      var windows = AutomationElement.RootElement.FindAll(
              TreeScope.Children,
              new AndCondition(
                  new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane),
                  new PropertyCondition(AutomationElement.ClassNameProperty, "Chrome_WidgetWin_1")))
          .Cast<AutomationElement>()
          .ToArray();

      if (windows.Length == 1)
        return ResolveBoundsFromWindow(windows[0]);

      if (windows.Length == 0)
        throw new InvalidOperationException("Could not find a Chrome window in order to resolve the bounds of the content area.");

      // If the result are ambiguous we try to find the browser by changing the window title
      var automationElement = ResolveByChangingWindowTitle(driver, windows);

      return ResolveBoundsFromWindow(automationElement);
    }

    private AutomationElement ResolveByChangingWindowTitle (IWebDriver driver, IReadOnlyCollection<AutomationElement> windows)
    {
      var id = Guid.NewGuid().ToString();

      var executor = (IJavaScriptExecutor)driver;
      var previousTitle = Assertion.IsNotNull(
          JavaScriptExecutor.ExecuteStatement<string>(executor, c_setWindowTitle, id),
          "The Javascript code changing and fetching the window title must not return null.");

      AutomationElement? result;
      try
      {
        result = RetryUntilValueChanges(() => windows.SingleOrDefault(w => w.Current.Name.StartsWith(id)), null, 30, TimeSpan.FromMilliseconds(10));
      }
      finally
      {
        JavaScriptExecutor.ExecuteStatement<string>(executor, c_setWindowTitle, previousTitle);
      }

      if (result == null)
        throw new InvalidOperationException("Could not find a matching Chrome window by changing its window title.");

      return result;
    }

    private Rectangle ResolveBoundsFromWindow (AutomationElement window)
    {
      // Sometimes we do not find a window on the first try
      var contentElement = RetryUntilValueChanges(() => GetContentElement(window), null, 5, TimeSpan.Zero);
      if (contentElement == null)
        throw new InvalidOperationException("Could not find the content window of the found Chrome browser window.");

      var rawBounds = contentElement.Current.BoundingRectangle;
      if (rawBounds == Rect.Empty)
        throw new InvalidOperationException("Could not resolve the bounds of the Chrome browser window.");

      return new Rectangle(
          (int)Math.Round(rawBounds.X),
          (int)Math.Round(rawBounds.Y),
          (int)Math.Round(rawBounds.Width),
          (int)Math.Round(rawBounds.Height));
    }

    private AutomationElement? GetContentElement (AutomationElement window)
    {

      var automationElement = window.FindFirst(
          TreeScope.Children,
          new AndCondition(
              new OrCondition(
                  // UI Automation tools tell us that the ControlTypeProperty == "ControlType.Document", but when executing the code, the value is returned as
                  // a "ControlType.Pane". In order to prevent strange effects, we check for both values since there is a) no ambiguity and b) the underlying reason for this
                  // mismatch is not easily discoverable.
                  new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Document),
                  new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane)
              ),
              new PropertyCondition(AutomationElement.ClassNameProperty, "Chrome_RenderWidgetHostHWND"),

              new OrCondition(
                  // UI Automation tools tell us that the FrameworkIdProperty == "Chrome", but when executing the code, the value is returned as "Win32".
                  // In order to prevent strange effects, both values are tested since there is a) no ambiguity and b) the underlying reason for this
                  // mismatch is not easily discoverable. It may be related to a timing effect.
                  new PropertyCondition(AutomationElement.FrameworkIdProperty, "Chrome"),
                  new PropertyCondition(AutomationElement.FrameworkIdProperty, "Win32"))
          )
      );
      return automationElement;
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
  }
}
