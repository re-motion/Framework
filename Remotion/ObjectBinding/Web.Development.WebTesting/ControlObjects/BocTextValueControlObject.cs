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
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocTextValue"/>.
  /// </summary>
  public class BocTextValueControlObject : BocControlObject, IFillableControlObject, IControlObjectWithFormElements
  {
    public BocTextValueControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      var valueScope = Scope.FindChild ("Value");

      if (IsReadOnly())
        return valueScope.Text; // do not trim

      return valueScope.Value; // do not trim
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject FillWith (string text, IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNull ("text", text);

      return FillWith (text, FinishInput.WithTab, actionOptions);
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject FillWith (string text, FinishInputWithAction finishInputWith, IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNull ("text", text);
      ArgumentUtility.CheckNotNull ("finishInputWith", finishInputWith);

      var actualActionOptions = MergeWithDefaultActionOptions (actionOptions, finishInputWith);
      new FillWithAction (this, Scope.FindChild ("Value"), text, finishInputWith).Execute (actualActionOptions);
      return UnspecifiedPage();
    }

    /// <summary>
    /// See <see cref="IControlObjectWithFormElements.GetFormElementNames"/>. Returns the input[type=text] (value) as only element.
    /// </summary>
    ICollection<string> IControlObjectWithFormElements.GetFormElementNames ()
    {
      return new[] { string.Format ("{0}_Value", GetHtmlID()) };
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