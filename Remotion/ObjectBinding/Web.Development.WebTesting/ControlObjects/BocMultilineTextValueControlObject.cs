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
using System.Collections.Generic;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocMultilineTextValue"/>.
  /// </summary>
  public class BocMultilineTextValueControlObject
      : BocControlObject, IFillableControlObject, IControlObjectWithFormElements, ISupportsValidationErrors, ISupportsValidationErrorsForReadOnly, IControlObjectWithText
  {
    public BocMultilineTextValueControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    /// <inheritdoc cref="IFillableControlObject" />
    public string GetText ()
    {
      var valueScope = GetValueScope();
      return valueScope.Value; // do not trim
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject FillWith (string text, IWebTestActionOptions? actionOptions = null)
    {
      ArgumentUtility.CheckNotNull("text", text);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver);

      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      return FillWith(text, FinishInput.WithTab, actionOptions);
    }

    /// <summary>
    /// Calls <see cref="FillWith(string,IWebTestActionOptions)"/> by joining the given lines with new line characters.
    /// </summary>
    public UnspecifiedPageObject FillWith ([NotNull] string[] lines, IWebTestActionOptions? actionOptions = null)
    {
      ArgumentUtility.CheckNotNull("lines", lines);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver);

      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      return FillWith(string.Join(Environment.NewLine, lines), actionOptions);
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject FillWith (string text, FinishInputWithAction finishInputWith, IWebTestActionOptions? actionOptions = null)
    {
      ArgumentUtility.CheckNotNull("text", text);
      ArgumentUtility.CheckNotNull("finishInputWith", finishInputWith);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver);

      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      var actualActionOptions = MergeWithDefaultActionOptions(actionOptions, finishInputWith);
      ExecuteAction(new FillWithAction(this, GetValueScope(), text, finishInputWith, Logger), actualActionOptions);
      return UnspecifiedPage();
    }

    /// <summary>
    /// Calls <see cref="FillWith(string,FinishInputWithAction,IWebTestActionOptions)"/> by joining the given lines with new line
    /// characters.
    /// </summary>
    public UnspecifiedPageObject FillWith (
        [NotNull] string[] lines,
        FinishInputWithAction finishInputWith,
        IWebTestActionOptions? actionOptions = null)
    {
      ArgumentUtility.CheckNotNull("lines", lines);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver);

      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      return FillWith(string.Join(Environment.NewLine, lines), finishInputWith, actionOptions);
    }

    /// <summary>
    /// See <see cref="IControlObjectWithFormElements.GetFormElementNames"/>. Returns the textarea (value) as only element.
    /// </summary>
    ICollection<string> IControlObjectWithFormElements.GetFormElementNames ()
    {
      return new[] { string.Format("{0}_Value", GetHtmlID()) };
    }

    public IReadOnlyList<string> GetValidationErrors ()
    {
      return GetValidationErrors(GetValueScope());
    }

    public IReadOnlyList<string> GetValidationErrorsForReadOnly ()
    {
      return GetValidationErrorsForReadOnly(GetValueScope());
    }

    protected override ElementScope GetLabeledElementScope ()
    {
      return GetValueScope();
    }

    private ElementScope GetValueScope ()
    {
      return Scope.FindChild("Value");
    }

    private IWebTestActionOptions MergeWithDefaultActionOptions (
        IWebTestActionOptions? userDefinedActionOptions,
        FinishInputWithAction finishInputWith)
    {
      if (finishInputWith == FinishInput.Promptly)
      {
        userDefinedActionOptions = userDefinedActionOptions ?? new WebTestActionOptions();
        userDefinedActionOptions.CompletionDetectionStrategy = Continue.Immediately;
      }

      return MergeWithDefaultActionOptions(Scope, userDefinedActionOptions);
    }
  }
}
