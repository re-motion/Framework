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
using OpenQA.Selenium;
using System;
using JetBrains.Annotations;
using OpenQA.Selenium.Support.UI;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Provides extensions methods for Selenium's <see cref="IWebElement"/> interface.
  /// </summary>
  public static class WebElementExtensions
  {
    /// <summary>
    /// Waits until an IFrame and a specific element within that IFrame is rendered.
    /// </summary>
    /// <param name="frame">The <see cref="IWebElement"/> representing the IFrame to wait for.</param>
    /// <param name="elementCssSelector">The CSS selector identifying the element which should be waited for.</param>
    /// <param name="timeout">A maximum timeout until the IFrame must be visible.</param>
    public static void WaitUntilFrameIsVisible ([NotNull] this IWebElement frame, [NotNull] string elementCssSelector, TimeSpan timeout)
    {
      ArgumentUtility.CheckNotNull("frame", frame);
      ArgumentUtility.CheckNotNull("elementCssSelector", elementCssSelector);

      var webDriver = ((IWrapsDriver)frame).WrappedDriver;
      var webDriverWait = new WebDriverWait(webDriver, timeout)
                          {
                              PollingInterval = TimeSpan.FromMilliseconds(timeout.TotalMilliseconds / 100)
                          };

      try
      {
        webDriverWait.Until(driver => FrameElementLoaded(driver, frame, elementCssSelector));
      }
      finally
      {
        webDriver.SwitchTo().DefaultContent();
      }
    }

    /// <summary>
    /// Waits until an IFrame is rendered.
    /// </summary>
    /// <param name="frame">The <see cref="IWebElement"/> representing the IFrame to wait for.</param>
    /// <param name="timeout">A maximum timeout until the IFrame must be visible.</param>
    public static void WaitUntilFrameIsVisible ([NotNull] this IWebElement frame, TimeSpan timeout)
    {
      ArgumentUtility.CheckNotNull("frame", frame);

      // a body is always present in an IFrame
      WaitUntilFrameIsVisible(frame, "body", timeout);
    }

    /// <summary>
    /// Determines whether a specific element within an IFrame has already been rendered.
    /// </summary>
    /// <remarks>Sometimes, due to delayed generation of the compiled aspx file, the content of the IFrame is not fully loaded when the IFrame is rendered.</remarks>
    private static bool FrameElementLoaded (IWebDriver driver, IWebElement frame, string elementCssSelector)
    {
      driver.SwitchTo().DefaultContent();
      driver.SwitchTo().Frame(frame);

      return driver.FindElements(By.CssSelector(elementCssSelector)).Count > 0;
    }
  }
}
