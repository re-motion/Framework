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
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocAutoCompleteReferenceValue;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Provides fluent extension methods for controlling a <see cref="BocAutoCompleteReferenceValueControlObject"/> information-popup.
  /// </summary>
  public static class ScreenshotBocAutoCompleteReferenceValueInformationPopupExtensions
  {
    private const string c_getElementScript = "return Remotion.jQuery(arguments[0]).getAutoCompleteInformationPopUp().getElement();";
    private const string c_hideScript = "Remotion.jQuery(arguments[0]).getAutoCompleteInformationPopUp().hide();";
    private const string c_showScript = "Remotion.jQuery(arguments[0]).getAutoCompleteInformationPopUp().show (arguments[1]);";
    private const string c_isVisibleScript = "return Remotion.jQuery(arguments[0]).getAutoCompleteInformationPopUp().visible();";

    /// <summary>
    /// Returns a fluent <see cref="IWebElement"/> representing the popup DOM element.
    /// </summary>
    /// <exception cref="InvalidOperationException">The popup is not visible.</exception>
    internal static FluentScreenshotElement<IWebElement> GetElement (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueInformationPopup> fluentInformationPopup)
    {
      ArgumentUtility.CheckNotNull ("fluentInformationPopup", fluentInformationPopup);

      if (!fluentInformationPopup.IsVisible())
        throw new InvalidOperationException ("The popup is not visible.");

      var result = JavaScriptExecutor.ExecuteStatement<IWebElement> (
          fluentInformationPopup.GetExecutor(),
          c_getElementScript,
          fluentInformationPopup.GetInputField());

      return result.ForWebElementScreenshot();
    }

    /// <summary>
    /// Displays the popup with the specified <paramref name="message"/>.
    /// </summary>
    public static void Display (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueInformationPopup> fluentInformationPopup,
        [NotNull] string message)
    {
      ArgumentUtility.CheckNotNull ("fluentInformationPopup", fluentInformationPopup);
      ArgumentUtility.CheckNotNull ("message", message);

      JavaScriptExecutor.ExecuteVoidStatement (fluentInformationPopup.GetExecutor(), c_showScript, fluentInformationPopup.GetInputField(), message);
    }

    /// <summary>
    /// Hides the popup.
    /// </summary>
    public static void Hide (
          [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueInformationPopup> fluentInformationPopup)
    {
      ArgumentUtility.CheckNotNull ("fluentInformationPopup", fluentInformationPopup);

      JavaScriptExecutor.ExecuteVoidStatement (fluentInformationPopup.GetExecutor(), c_hideScript, fluentInformationPopup.GetInputField());
    }

    /// <summary>
    /// Returns <see langword="true" /> if the popup is visible, <see langword="false" /> if otherwise.
    /// </summary>
    public static bool IsVisible (
          [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueInformationPopup> fluentInformationPopup)
    {
      ArgumentUtility.CheckNotNull ("fluentInformationPopup", fluentInformationPopup);

      return JavaScriptExecutor.ExecuteStatement<bool> (
          fluentInformationPopup.GetExecutor(),
          c_isVisibleScript,
          fluentInformationPopup.GetInputField());
    }

    /// <summary>
    /// Waits until the popup is visible or the <paramref name="timeout"/> is reached.
    /// </summary>
    /// <exception cref="TimeoutException">The <paramref name="timeout"/> has been reached.</exception>
    public static void WaitUntilVisible (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueInformationPopup> fluentInformationPopup,
        int timeout = 3000)
    {
      ArgumentUtility.CheckNotNull ("fluentInformationPopup", fluentInformationPopup);

      var watch = new Stopwatch();
      watch.Start();

      do
      {
        if (fluentInformationPopup.IsVisible())
          return;

        if (watch.ElapsedMilliseconds >= timeout)
          throw new TimeoutException ("Could not wait for the timeout in the specified amount of time.");

        Thread.Sleep (50);
      } while (true);
    }

    private static IJavaScriptExecutor GetExecutor (
        this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueInformationPopup> fluentInformationPopup)
    {
      return JavaScriptExecutor.GetJavaScriptExecutor (fluentInformationPopup.Target.AutoComplete);
    }

    private static IWebElement GetInputField (
        this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueInformationPopup> fluentInformationPopup)
    {
      return (IWebElement) fluentInformationPopup.Target.AutoComplete.ForControlObjectScreenshot().GetValue().GetTarget().Native;
    }
  }
}