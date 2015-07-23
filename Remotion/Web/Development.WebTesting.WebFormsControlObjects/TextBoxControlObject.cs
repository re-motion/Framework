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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.Web.Development.WebTesting.WebFormsControlObjects
{
  /// <summary>
  /// Control object for <see cref="T:System.Web.UI.WebControls.TextBox"/> and all its derivatives (none in re-motion).
  /// </summary>
  public class TextBoxControlObject : WebFormsControlObject, IFillableControlObject, IControlObjectWithFormElements
  {
    public TextBoxControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      return Scope.Value; // do not trim
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject FillWith (string text, IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNull ("text", text);

      return FillWith (text, FinishInput.WithTab, actionOptions);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The default <see cref="ICompletionDetectionStrategy"/> for <see cref="TextBoxControlObject"/> does expect a WXE auto postback!
    /// </remarks>
    public UnspecifiedPageObject FillWith (string text, FinishInputWithAction finishInputWith, IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNull ("text", text);
      ArgumentUtility.CheckNotNull ("finishInputWith", finishInputWith);

      var actualActionOptions = MergeWithDefaultActionOptions (actionOptions, finishInputWith);
      new FillWithAction (this, Scope, text, finishInputWith).Execute (actualActionOptions);
      return UnspecifiedPage();
    }

    /// <inheritdoc/>
    ICollection<string> IControlObjectWithFormElements.GetFormElementNames ()
    {
      return new[] { Scope.Name };
    }

    private IWebTestActionOptions MergeWithDefaultActionOptions (
        IWebTestActionOptions userDefinedActionOptions,
        FinishInputWithAction finishInputWith)
    {
      if (finishInputWith == FinishInput.Promptly)
      {
        userDefinedActionOptions = userDefinedActionOptions ?? new WebTestActionOptions();
        userDefinedActionOptions.CompletionDetectionStrategy = Continue.Immediately;
      }

      return MergeWithDefaultActionOptions (Scope, userDefinedActionOptions);
    }
  }
}