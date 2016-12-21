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
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Extension methods for Coypu's <see cref="ElementScope"/> class regarding <see cref="ElementScope.Click"/>.
  /// </summary>
  public static class CoypuElementScopeClickAndFocusExtensions
  {
    /// <summary>
    /// Performs a context click (right click).
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="context">The corresponding control object's context.</param>
    public static void ContextClick ([NotNull] this ElementScope scope, [NotNull] WebTestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("context", context);

      // Hack: Coypu does not directly support the Actions interface, therefore we need to fall back to using Selenium.
      RetryUntilTimeout.Run (
          () =>
          {
            var webDriver = (IWebDriver) context.Browser.Native;
            var nativeScope = (IWebElement) scope.Native;

            var actions = new Actions (webDriver);
            actions.ContextClick (nativeScope);
            actions.Perform();
          });
    }

    /// <summary>
    /// Focuses an element.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    public static void Focus ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      scope.SendKeys ("");
    }

    /// <summary>
    /// Focuses a link before actually clicking it.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    public static void FocusClick ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      scope.Focus();
      scope.Click();
    }
  }
}