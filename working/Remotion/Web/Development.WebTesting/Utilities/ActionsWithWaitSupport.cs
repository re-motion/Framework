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
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Enhanced version of Selenium's <see cref="Actions"/> class. It enables us to add WaitFor predicates after actions.
  /// </summary>
  public class ActionsWithWaitSupport : Actions
  {
    private class ActionAdapter : IAction
    {
      private readonly Action _action;

      public ActionAdapter (Action action)
      {
        _action = action;
      }

      public void Perform ()
      {
        _action();
      }
    }

    /// <summary>
    /// Pre-defined predicate for "is element visible" condition.
    /// </summary>
    public static readonly Func<IWebElement, bool> IsVisible = we => we.Displayed;

    private readonly IWebDriver _driver;

    public ActionsWithWaitSupport (IWebDriver driver)
        : base (driver)
    {
      _driver = driver;
    }

    /// <summary>
    /// Adds a WaitFor predicate after the last added action.
    /// </summary>
    /// <param name="webElement">The web element we want to pass to the predicate.</param>
    /// <param name="predicate">The wait condition.</param>
    /// <param name="timeout">A maximum timeout until the condition must be fulfilled.</param>
    /// <returns>Itself, allows chaining of calls.</returns>
    /// <exception cref="WebDriverException">Depending on the predicate, a variety of <see cref="WebDriverException"/>s may be thrown.
    /// <see cref="StaleElementReferenceException"/> may only appear after the timeout has been reached, before the timeout, it is ignored and the
    /// check of the condition is retried.</exception>
    public ActionsWithWaitSupport WaitFor (IWebElement webElement, Func<IWebElement, bool> predicate, TimeSpan timeout)
    {
      AddAction (
          new ActionAdapter (
              () =>
              {
                var webDriverWait = new WebDriverWait (_driver, timeout);
                webDriverWait.IgnoreExceptionTypes (typeof (StaleElementReferenceException));
                webDriverWait.Until (_ => predicate (webElement));
              }));

      return this;
    }
  }
}