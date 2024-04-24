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
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Extension methods for Coypu's <see cref="ElementScope"/> class fixing <see cref="ElementScope.FillInWith"/> and
  /// <see cref="ElementScope.SendKeys"/>.
  /// </summary>
  public static class CoypuElementScopeFillInWithAndSendKeysExtensions
  {
    private static readonly ILogger s_logger = LogManager.GetLogger(typeof(CoypuElementScopeFillInWithAndSendKeysExtensions));

    /// <summary>
    /// ASP.NET WebForms-ready &amp; IE-compatible version for Coypu's <see cref="ElementScope.SendKeys"/> method.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="value">The value to fill in.</param>
    [Obsolete("Use scope.SendKeys (string) instead to send OpenQA.Selenium.Keys or individual characters. (Version 1.21.3)", false)]
    public static void SendKeysFixed ([NotNull] this ElementScope scope, [NotNull] string value)
    {
      ArgumentUtility.CheckNotNull("scope", scope);
      ArgumentUtility.CheckNotNull("value", value);

      scope.SendKeys(value);
    }

    /// <summary>
    /// ASP.NET WebForms-ready &amp; IE-compatible version for Coypu's <see cref="ElementScope.FillInWith"/> method.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="value">A string consisting only of printable characters or only of values defined in <see cref="Keys"/>.</param>
    /// <param name="finishInputWithAction"><see cref="FinishInputWithAction"/> for this action.</param>
    public static void FillInWithFixed (
        [NotNull] this ElementScope scope,
        [NotNull] string value,
        [NotNull] FinishInputWithAction finishInputWithAction)
    {
      ArgumentUtility.CheckNotNull("scope", scope);
      ArgumentUtility.CheckNotNull("value", value);
      ArgumentUtility.CheckNotNull("finishInputWithAction", finishInputWithAction);

      if (ContainsKeysAndChars(value))
        throw new ArgumentException("Value may not contain both text and keys at the same time.", "value");

      scope.SetValueUsingSendKeys(value);

      finishInputWithAction(scope);
    }

    /// <summary>
    /// IEDriverServer gets stuck after sending large amounts of characters with Coypu's FillInWith. Using JavaScript has proven to be reliable.
    /// </summary>
    private static void SetValueUsingJavaScriptAndSendKeys ([NotNull] this ElementScope scope, [NotNull] string value)
    {
      ArgumentUtility.CheckNotNull("scope", scope);
      ArgumentUtility.CheckNotNull("value", value);

      if (scope.Value == value)
        return;

      var driver = scope.GetDriver();
      var id = scope.Id;
      var valueWithoutLastCharacter = GetStringWithoutLastCharacter(value);
      var lastCharacter = GetLastCharacter(value);

      var command = GetFillInJavaScriptCommand(id);

      s_logger.LogDebug("FillInWith using JavaScript: '{0}'.", value);

      driver.ExecuteScript(command, scope, valueWithoutLastCharacter);
      scope.SendKeys(Keys.End);
      scope.SendKeys(lastCharacter);
    }

    /// <summary>
    /// We cannot use Coypu's <see cref="ElementScope.FillInWith"/> here, as it internally calls Selenium's <see cref="IWebElement.Clear"/> which
    /// unfortunately triggers a post back. See https://groups.google.com/forum/#!topic/selenium-users/fBWLmL8iEzA for more information.
    /// </summary>
    private static void SetValueUsingSendKeys ([NotNull] this ElementScope scope, [NotNull] string value)
    {
      // GeckoDriver treats \r as its own newline character, which causes double newlines inserts. Since all browsers can deal with \n, we can simply remove \r.
      value = value.Replace("\r", "");

      var clearTextBoxWithoutTriggeringPostBack = Keys.Control + "a" + Keys.Control + Keys.Delete;
      value = clearTextBoxWithoutTriggeringPostBack + value;

      s_logger.LogDebug("FillInWith without triggering PostBack on clear: '{0}'.", value);

      scope.SendKeys(value);
    }

    private static string GetFillInJavaScriptCommand ([NotNull] string id)
    {
      const string javascriptFormat = "document.getElementById('{0}').value = arguments[0]";
      return string.Format(javascriptFormat, id);
    }

    private static string GetStringWithoutLastCharacter ([NotNull] string value)
    {
      if (SingleCharacterOrEmpty(value))
        return "";

      var lastCharacterIndex = value.Length - 1;
      return value.Remove(lastCharacterIndex);
    }

    private static string GetLastCharacter ([NotNull] string value)
    {
      if (SingleCharacterOrEmpty(value))
        return value;

      var lastCharacterIndex = value.Length - 1;
      return value[lastCharacterIndex].ToString();
    }

    private static bool SingleCharacterOrEmpty (string value)
    {
      return value.Length < 2;
    }

    private static bool ContainsKeys ([NotNull] string value)
    {
      return value.Any(c => c >= Keys.Null[0]);
    }

    private static bool ContainsChars ([NotNull] string value)
    {
      return value.Any(c => c < Keys.Null[0]);
    }

    private static bool ContainsKeysAndChars ([NotNull] string value)
    {
      return ContainsKeys(value) && ContainsChars(value);
    }

    private static bool ContainsNonEmptyText (string value)
    {
      return ContainsChars(value) && value != "";
    }
  }
}
