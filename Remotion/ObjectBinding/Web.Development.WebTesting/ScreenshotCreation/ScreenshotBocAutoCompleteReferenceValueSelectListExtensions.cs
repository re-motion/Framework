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
  /// Provides fluent extension methods for controlling a <see cref="BocAutoCompleteReferenceValueControlObject"/> select-list.
  /// </summary>
  public static class ScreenshotBocAutoCompleteReferenceValueSelectListExtensions
  {
    private const string c_currentItemScript = "return Remotion.jQuery(arguments[0]).getAutoCompleteSelectList().current();";
    private const string c_getElementScript = "return Remotion.jQuery(arguments[0]).getAutoCompleteSelectList().getElement();";
    private const string c_hideScript = "return Remotion.jQuery(arguments[0]).getAutoCompleteSelectList().hide();";
    private const string c_nextScript = "return Remotion.jQuery(arguments[0]).getAutoCompleteSelectList().next();";
    private const string c_pageDownScript = "return Remotion.jQuery(arguments[0]).getAutoCompleteSelectList().pageDown();";
    private const string c_pageUpScript = "return Remotion.jQuery(arguments[0]).getAutoCompleteSelectList().pageUp();";
    private const string c_previousScript = "return Remotion.jQuery(arguments[0]).getAutoCompleteSelectList().prev();";
    private const string c_showScript = "return Remotion.jQuery(arguments[0]).getAutoCompleteSelectList().show();";
    private const string c_isVisibleScript = "return Remotion.jQuery(arguments[0]).getAutoCompleteSelectList().visible();";

    /// <summary>
    /// Returns an <see cref="IWebElement"/> representing the select-list.
    /// </summary>
    internal static IFluentScreenshotElement<IWebElement> GetElement (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> fluentSelectList)
    {
      ArgumentUtility.CheckNotNull ("fluentSelectList", fluentSelectList);

      if (!fluentSelectList.IsVisible())
        throw new InvalidOperationException ("The auto-complete is not visible.");

      var result = JavaScriptExecutor.ExecuteStatement<IWebElement> (
          fluentSelectList.GetExecutor(),
          c_getElementScript,
          fluentSelectList.GetInputField());

      Assertion.IsNotNull (result, "The result of the executed statement must not be null.");

      return result.ForWebElementScreenshot();
    }

    /// <summary>
    /// Returns an <see cref="IWebElement"/> representing the currently selected item of the select-list.
    /// </summary>
    public static FluentScreenshotElement<IWebElement> GetSelectedItem (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> fluentSelectList)
    {
      ArgumentUtility.CheckNotNull ("fluentSelectList", fluentSelectList);

      if (!fluentSelectList.IsVisible())
        throw new InvalidOperationException ("The auto-complete is not visible.");

      var result = JavaScriptExecutor.ExecuteStatement<IWebElement> (
          fluentSelectList.GetExecutor(),
          c_currentItemScript,
          fluentSelectList.GetInputField());

      Assertion.IsNotNull (result, "The result of the executed statement must not be null.");

      return result.ForWebElementScreenshot();
    }

    /// <summary>
    /// Hides the select-list.
    /// </summary>
    public static void Hide (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> fluentSelectList)
    {
      ArgumentUtility.CheckNotNull ("fluentSelectList", fluentSelectList);

      JavaScriptExecutor.ExecuteVoidStatement (fluentSelectList.GetExecutor(), c_hideScript, fluentSelectList.GetInputField());
    }

    /// <summary>
    /// Returns <see langword="true" /> if the select-list is visible, otherwise <see langword="false" />.
    /// </summary>
    public static bool IsVisible (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> fluentSelectList)
    {
      ArgumentUtility.CheckNotNull ("fluentSelectList", fluentSelectList);

      return JavaScriptExecutor.ExecuteStatement<bool> (
          fluentSelectList.GetExecutor(),
          c_isVisibleScript,
          fluentSelectList.GetInputField());
    }

    /// <summary>
    /// Selects the next item of the select-list.
    /// </summary>
    public static void NextItem (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> fluentSelectList)
    {
      ArgumentUtility.CheckNotNull ("fluentSelectList", fluentSelectList);

      JavaScriptExecutor.ExecuteVoidStatement (fluentSelectList.GetExecutor(), c_nextScript, fluentSelectList.GetInputField());
    }

    /// <summary>
    /// Selects the next page of the select-list (1 page = 8 items).
    /// </summary>
    public static void NextPage (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> fluentSelectList)
    {
      ArgumentUtility.CheckNotNull ("fluentSelectList", fluentSelectList);

      JavaScriptExecutor.ExecuteVoidStatement (fluentSelectList.GetExecutor(), c_pageDownScript, fluentSelectList.GetInputField());
    }

    /// <summary>
    /// Selects the previous item of the select-list.
    /// </summary>
    public static void PreviousItem (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> fluentSelectList)
    {
      ArgumentUtility.CheckNotNull ("fluentSelectList", fluentSelectList);

      JavaScriptExecutor.ExecuteVoidStatement (fluentSelectList.GetExecutor(), c_previousScript, fluentSelectList.GetInputField());
    }

    /// <summary>
    /// Selects the previous page of the select-list (1 page = 8 items).
    /// </summary>
    public static void PreviousPage (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> fluentSelectList)
    {
      ArgumentUtility.CheckNotNull ("fluentSelectList", fluentSelectList);

      JavaScriptExecutor.ExecuteVoidStatement (fluentSelectList.GetExecutor(), c_pageUpScript, fluentSelectList.GetInputField());
    }

    /// <summary>
    /// Starts the fluent selection for a specific item of the select-list.
    /// </summary>
    public static ScreenshotBocAutoCompleteReferenceValueSelectListSelector Select (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> fluentSelectList)
    {
      ArgumentUtility.CheckNotNull ("fluentSelectList", fluentSelectList);

      return new ScreenshotBocAutoCompleteReferenceValueSelectListSelector (fluentSelectList);
    }

    /// <summary>
    /// Selects the select-list item with the specified <paramref name="oneBasedIndex"/>.
    /// </summary>
    public static void Select (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> fluentSelectList,
        int oneBasedIndex)
    {
      ArgumentUtility.CheckNotNull ("fluentSelectList", fluentSelectList);

      fluentSelectList.Select().WithIndex (oneBasedIndex);
    }

    /// <summary>
    /// Re-displays the previously hidden select-list. 
    /// </summary>
    /// <remarks>
    /// This method does not initialize nor initiate the auto-complete and will fail if the select-list is not initialized.
    /// </remarks>
    public static void Show (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> fluentSelectList)
    {
      ArgumentUtility.CheckNotNull ("fluentSelectList", fluentSelectList);

      JavaScriptExecutor.ExecuteVoidStatement (fluentSelectList.GetExecutor(), c_showScript, fluentSelectList.GetInputField());
    }

    /// <summary>
    /// Waits until the select-list is visible or the specified <paramref name="timeout"/> (in ms) ran out.
    /// </summary>
    /// <exception cref="TimeoutException">The waiting time exceeded the <paramref name="timeout"/>.</exception>
    public static void WaitUntilVisible (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> fluentSelectList,
        int timeout = 3000)
    {
      ArgumentUtility.CheckNotNull ("fluentSelectList", fluentSelectList);

      var watch = new Stopwatch();
      watch.Start();

      do
      {
        if (fluentSelectList.IsVisible())
          return;

        if (watch.ElapsedMilliseconds >= timeout)
          throw new TimeoutException ("Could not wait for the timeout in the specified amount of time.");

        Thread.Sleep (50);
      } while (true);
    }

    private static IJavaScriptExecutor GetExecutor (
        this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> fluentSelectList)
    {
      return JavaScriptExecutor.GetJavaScriptExecutor (fluentSelectList.Target.AutoComplete);
    }

    private static IWebElement GetInputField (
        this IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> fluentSelectList)
    {
      return (IWebElement) fluentSelectList.Target.AutoComplete.ForControlObjectScreenshot().GetValue().GetTarget().Native;
    }
  }
}