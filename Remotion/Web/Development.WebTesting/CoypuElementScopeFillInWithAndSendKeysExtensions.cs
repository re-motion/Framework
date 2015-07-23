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
using System.Windows.Forms;
using Coypu;
using JetBrains.Annotations;
using log4net;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.Utilities;
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

    /// <summary>
    /// ASP.NET WebForms-ready &amp; IE-compatiable version for Coypu's <see cref="ElementScope.SendKeys"/> method.
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

      if (!WebTestingConfiguration.Current.BrowserIsInternetExplorer())
        scope.FillInWithFixedForNormalBrowsers (value, clearValue);
      else
        scope.FillInWithFixedForInternetExplorer (value, clearValue);

      finishInputWithAction (scope);
    }

    /// <summary>
    /// We cannot use Coypu's <see cref="ElementScope.FillInWith"/> here, as it internally calls Selenium's <see cref="IWebElement.Clear"/> which
    /// unfortunately triggers a post back. See https://groups.google.com/forum/#!topic/selenium-users/fBWLmL8iEzA for more information.
    /// </summary>
    private static void FillInWithFixedForNormalBrowsers ([NotNull] this ElementScope scope, [NotNull] string value, bool clearValue)
    {
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
    private static void FillInWithFixedForInternetExplorer ([NotNull] this ElementScope scope, [NotNull] string value, bool clearValue)
    {
      var convertedValue = SeleniumSendKeysToWindowsFormsSendKeysTransformer.Convert (value);

      if (clearValue)
      {
        const string clearTextBox = "^a{DEL}";
        convertedValue = clearTextBox + convertedValue;
      }

      s_log.DebugFormat ("FillInWith for InternetExplorer: '{0}'.", convertedValue);

      do
      {
        scope.Focus();
        SendKeys.SendWait (convertedValue);
      } while (!IsInputOkay (scope, value));
    }

    /// <summary>
    /// Unfortunately, Internet Explorer sometimes skips single characters of <see cref="SendKeys.SendWait"/> (we don't know if this is an IE issue
    /// or a SendKeys issue). To keep flaky tests to a minimum, we try to check the input before continuing.
    /// </summary>
    private static bool IsInputOkay (ElementScope scope, string value)
    {
      if (value.Any (c => c >= Keys.Null[0]))
      {
        s_log.DebugFormat ("FillInWith for InternetExplorer: value contains special characters, no retry-check possible.");
        return true;
      }

      var isInputOkay = scope.Value == value;
      if (!isInputOkay)
        s_log.DebugFormat ("FillInWith for InternetExplorer: value is different: '{0}' != '{1}' - retrying...", scope.Value, value);
      return isInputOkay;
    }
  }
}