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
using System.Linq;
using System.Reflection;
using System.Threading;
using Coypu;
using JetBrains.Annotations;
using log4net;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;
using Keys = OpenQA.Selenium.Keys;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Extension methods for Coypu's <see cref="ElementScope"/> class fixing <see cref="ElementScope.FillInWith"/> and
  /// <see cref="ElementScope.SendKeys"/>.
  /// </summary>
  public static class CoypuElementScopeFillInWithAndSendKeysExtensions
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (CoypuElementScopeFillInWithAndSendKeysExtensions));

    private static readonly Lazy<FieldInfo> s_driverFieldInfo = new Lazy<FieldInfo> (
        () => Assertion.IsNotNull (
            typeof (ElementScope).GetField ("_driver", BindingFlags.NonPublic | BindingFlags.Instance),
            "Coypu has changed, please update CoypuElementScopeFillInWithAndSendKeysExtensions.GetDriver() method."),
        LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// ASP.NET WebForms-ready &amp; IE-compatible version for Coypu's <see cref="ElementScope.SendKeys"/> method.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="value">The value to fill in.</param>
    public static void SendKeysFixed ([NotNull] this ElementScope scope, [NotNull] string value)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("value", value);

      const bool clearValue = false;
      scope.FillInWithFixed (value, FinishInput.Promptly, clearValue);
    }

    /// <summary>
    /// ASP.NET WebForms-ready &amp; IE-compatible version for Coypu's <see cref="ElementScope.FillInWith"/> method.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="value">The value to fill in.</param>
    /// <param name="finishInputWithAction"><see cref="FinishInputWithAction"/> for this action.</param>
    public static void FillInWithFixed (
        [NotNull] this ElementScope scope,
        [NotNull] string value,
        [NotNull] FinishInputWithAction finishInputWithAction)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("finishInputWithAction", finishInputWithAction);

      const bool clearValue = true;
      scope.FillInWithFixed (value, finishInputWithAction, clearValue);
    }

    /// <summary>
    /// ASP.NET WebForms-ready &amp; IE-compatible version for Coypu's <see cref="ElementScope.FillInWith"/> method.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="value">The value to fill in.</param>
    /// <param name="finishInputWithAction"><see cref="FinishInputWithAction"/> for this action.</param>
    /// <param name="clearValue">Determines whether the old content should be cleared before filling in the new value.</param>
    private static void FillInWithFixed (
        [NotNull] this ElementScope scope,
        [NotNull] string value,
        [NotNull] FinishInputWithAction finishInputWithAction,
        bool clearValue)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("finishInputWithAction", finishInputWithAction);

#pragma warning disable 618
      if (scope.Browser.IsInternetExplorer() && ContainsNonEmptyText (value))
#pragma warning restore 618
        scope.FillInWithFixedForInternetExplorer (value);
      else
        scope.FillInWithFixedForNormalBrowsers (value, clearValue);

      finishInputWithAction (scope);
    }

    /// <summary>
    /// We cannot use Coypu's <see cref="ElementScope.FillInWith"/> here, as it internally calls Selenium's <see cref="IWebElement.Clear"/> which
    /// unfortunately triggers a post back. See https://groups.google.com/forum/#!topic/selenium-users/fBWLmL8iEzA for more information.
    /// </summary>
    private static void FillInWithFixedForNormalBrowsers ([NotNull] this ElementScope scope, [NotNull] string value, bool clearValue)
    {
      // GeckoDriver treats \r as its own newline character, which causes double newlines inserts. Since all browsers can deal with \n, we can simply remove \r.
      value = value.Replace ("\r", "");

      if (clearValue)
      {
        var clearTextBox = Keys.Control + "a" + Keys.Control + Keys.Delete;
        value = clearTextBox + value;
      }

      s_log.DebugFormat ("FillInWith for normal browsers: '{0}'.", value);

      scope.SendKeys (value);
    }

    /// <summary>
    /// Unfortunately, Selenium's Internet Explorer driver (with native events enabled) does not send required modifier keys when sending keyboard
    /// input (e.g. "@!" would result in "q1"). Therefore we must use <see cref="System.Windows.Forms.SendKeys.SendWait"/> instead.
    /// </summary>
    private static void FillInWithFixedForInternetExplorer ([NotNull] this ElementScope scope, [NotNull] string value)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("value", value);

      if (ContainsKeysAndChars (value))
        throw new ArgumentException ("Internet Explorer does not support filling in values that contain both text and keys at the same time.", "value");

      if (scope.Value == value)
        return;

      var driver = scope.GetDriver();
      var id = scope.Id;
      var valueWithoutLastCharacter = GetStringWithoutLastCharacter (value);
      var lastCharacter = GetLastCharacter (value);

      var command = GetFillInJavaScriptCommand (id);

      s_log.DebugFormat ("FillInWith using JavaScript: '{0}'.", value);

      driver.ExecuteScript (command, scope, valueWithoutLastCharacter);
      scope.SendKeys (Keys.End);
      scope.SendKeys (lastCharacter);
    }

    private static bool ContainsNonEmptyText (string value)
    {
      return ContainsChars (value) && value != "";
    }

    private static string GetStringWithoutLastCharacter ([NotNull] string value)
    {
      if (SingleCharacterOrEmpty (value))
        return "";

      var lastCharacterIndex = value.Length - 1;
      return value.Remove (lastCharacterIndex);
    }

    private static string GetFillInJavaScriptCommand ([NotNull] string id)
    {
      const string javascriptFormat = "document.getElementById('{0}').value = arguments[0]";
      return string.Format (javascriptFormat, id);
    }

    private static string GetLastCharacter ([NotNull] string value)
    {
      if (SingleCharacterOrEmpty (value))
        return value;

      var lastCharacterIndex = value.Length - 1;
      return value[lastCharacterIndex].ToString();
    }

    private static bool SingleCharacterOrEmpty (string value)
    {
      return value.Length < 2;
    }

    private static IDriver GetDriver ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      return (IDriver) s_driverFieldInfo.Value.GetValue (scope);
    }

    private static bool ContainsKeysAndChars ([NotNull] string value)
    {
      return ContainsKeys (value) && ContainsChars (value);
    }

    private static bool ContainsKeys ([NotNull] string value)
    {
      return value.Any (c => c >= Keys.Null[0]);
    }

    private static bool ContainsChars ([NotNull] string value)
    {
      return value.Any (c => c < Keys.Null[0]);
    }
  }
}