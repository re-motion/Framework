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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocAutoCompleteReferenceValue;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Provides fluent extension methods for controlling a <see cref="BocAutoCompleteReferenceValueControlObject"/>.
  /// </summary>
  public static class ScreenshotBocAutoCompleteReferenceValueExtensions
  {
    /// <summary>
    /// Returns the auto-complete of this <see cref="BocAutoCompleteReferenceValueControlObject"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">The <see cref="BocAutoCompleteReferenceValueControlObject"/> is read-only.</exception>
    public static FluentScreenshotElement<ScreenshotBocAutoCompleteReferenceValueSelectList> GetSelectList (
        [NotNull] this IFluentScreenshotElementWithCovariance<BocAutoCompleteReferenceValueControlObject> fluentAutoComplete)
    {
      ArgumentUtility.CheckNotNull ("fluentAutoComplete", fluentAutoComplete);

      if (fluentAutoComplete.IsReadOnly())
        throw new InvalidOperationException ("Can not get the auto-complete as the AutoCompleteReferenceValue is read-only.");

      return SelfResolvableFluentScreenshot.Create (new ScreenshotBocAutoCompleteReferenceValueSelectList (fluentAutoComplete));
    }

    /// <summary>
    /// Returns the command of this <see cref="BocAutoCompleteReferenceValueControlObject"/>.
    /// </summary>
    /// <exception cref="MissingHtmlException">The <see cref="BocAutoCompleteReferenceValueControlObject"/> has no command.</exception>
    public static FluentScreenshotElement<CommandControlObject> GetCommand (
        [NotNull] this IFluentScreenshotElementWithCovariance<BocAutoCompleteReferenceValueControlObject> fluentAutoComplete)
    {
      ArgumentUtility.CheckNotNull ("fluentAutoComplete", fluentAutoComplete);

      return fluentAutoComplete.Target.GetCommand().ForControlObjectScreenshot();
    }

    /// <summary>
    /// Returns the drop-down-button of this <see cref="BocAutoCompleteReferenceValueControlObject"/>.
    /// </summary>
    /// <exception cref="MissingHtmlException">The <see cref="BocAutoCompleteReferenceValueControlObject"/> has no drop-down-button as it is read-only.</exception>
    public static FluentScreenshotElement<ElementScope> GetDropDownButton (
        [NotNull] this IFluentScreenshotElementWithCovariance<BocAutoCompleteReferenceValueControlObject> fluentAutoComplete)
    {
      ArgumentUtility.CheckNotNull ("fluentAutoComplete", fluentAutoComplete);

      var target = fluentAutoComplete.Target.Scope.FindChild ("DropDownButton", Options.NoWait);
      target.EnsureExistence();

      return target.ForElementScopeScreenshot();
    }

    /// <summary>
    /// Returns the command of this <see cref="BocAutoCompleteReferenceValueControlObject"/>.
    /// </summary>
    /// <exception cref="MissingHtmlException">The <see cref="BocAutoCompleteReferenceValueControlObject"/> has no option menu.</exception>
    public static FluentScreenshotElement<DropDownMenuControlObject> GetOptionsMenu (
        [NotNull] this IFluentScreenshotElementWithCovariance<BocAutoCompleteReferenceValueControlObject> fluentAutoComplete)
    {
      ArgumentUtility.CheckNotNull ("fluentAutoComplete", fluentAutoComplete);

      var target = fluentAutoComplete.Target.Scope.FindChild ("Boc_OptionsMenu", Options.NoWait);
      target.EnsureExistence();

      return FluentUtility.CreateFluentControlObject (new DropDownMenuControlObject (fluentAutoComplete.Target.Context.CloneForControl (target)));
    }

    /// <summary>
    /// Returns the popup of this <see cref="BocAutoCompleteReferenceValueControlObject"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">The <see cref="BocAutoCompleteReferenceValueControlObject"/> is read-only.</exception>
    public static FluentScreenshotElement<ScreenshotBocAutoCompleteReferenceValueInformationPopup> GetInformationPopup (
        [NotNull] this IFluentScreenshotElementWithCovariance<BocAutoCompleteReferenceValueControlObject> fluentAutoComplete)
    {
      ArgumentUtility.CheckNotNull ("fluentAutoComplete", fluentAutoComplete);

      if (fluentAutoComplete.Target.IsReadOnly())
        throw new InvalidOperationException ("Can not get the popup as the AutoCompleteReferenceValue is read-only.");

      return SelfResolvableFluentScreenshot.Create (new ScreenshotBocAutoCompleteReferenceValueInformationPopup (fluentAutoComplete));
    }

    /// <summary>
    /// Returns the input of this <see cref="BocAutoCompleteReferenceValueControlObject"/>, or in case the 
    /// <see cref="BocAutoCompleteReferenceValueControlObject"/> is read-only, the text field.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetValue (
        [NotNull] this IFluentScreenshotElementWithCovariance<BocAutoCompleteReferenceValueControlObject> fluentAutoComplete)
    {
      ArgumentUtility.CheckNotNull ("fluentAutoComplete", fluentAutoComplete);

      ElementScope target;
      if (fluentAutoComplete.IsReadOnly())
        target = fluentAutoComplete.Target.Scope.FindChild ("Value", Options.NoWait);
      else
        target = fluentAutoComplete.Target.Scope.FindChild ("TextValue", Options.NoWait);
      target.EnsureExistence();

      return target.ForElementScopeScreenshot();
    }

    /// <summary>
    /// Returns <see langword="true" /> if this <see cref="BocAutoCompleteReferenceValueControlObject"/> is read-only, otherwise <see langword="false" />.
    /// </summary>
    public static bool IsReadOnly ([NotNull] this IFluentScreenshotElementWithCovariance<BocAutoCompleteReferenceValueControlObject> fluentAutoComplete)
    {
      ArgumentUtility.CheckNotNull ("fluentAutoComplete", fluentAutoComplete);

      return fluentAutoComplete.Target.IsReadOnly();
    }

    /// <summary>
    /// Sets the value of the specified <paramref name="fluentAutoComplete"/> and triggers the auto completion.
    /// </summary>
    public static void SetValue (
        [NotNull] this IFluentScreenshotElementWithCovariance<BocAutoCompleteReferenceValueControlObject> fluentAutoComplete,
        [NotNull] string value)
    {
      ArgumentUtility.CheckNotNull ("fluentAutoComplete", fluentAutoComplete);
      ArgumentUtility.CheckNotNull ("value", value);

      if (fluentAutoComplete.IsReadOnly())
        throw new InvalidOperationException ("Can not set as the control is read-only.");

      fluentAutoComplete.GetValue().GetTarget().FillInWith (value);
      fluentAutoComplete.GetDropDownButton().GetTarget().Click (Options.NoWait);
    }
  }
}