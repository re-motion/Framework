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
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent.Selectors;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocAutoCompleteReferenceValue
{
  /// <summary>
  /// Selector for selecting a <see cref="BocAutoCompleteReferenceValueControlObject"/> select list item.
  /// </summary>
  public class ScreenshotBocAutoCompleteReferenceValueSelectListSelector : IFluentDisplayTextSelector, IFluentIndexSelector
  {
    private const string c_selectWithIndex = "arguments[0].getAutoCompleteSelectList().selectItem (arguments[1] - 1);";

    private const string c_selectWithDisplayText = @"var a = arguments[0].getAutoCompleteSelectList();
var text = arguments[1]; 
var predicate = (function (data) 
{ 
  return data.data.DisplayName === text;
}); 

var b = a.findItemPositionWhere (predicate); 
if (b === -1)
  return false; 

a.selectItem (b); 
return true;";

    private readonly IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> _fluentAutoComplete;

    public ScreenshotBocAutoCompleteReferenceValueSelectListSelector (
        [NotNull] IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> fluentAutoComplete)
    {
      ArgumentUtility.CheckNotNull("fluentAutoComplete", fluentAutoComplete);

      _fluentAutoComplete = fluentAutoComplete;
    }

    /// <inheritdoc />
    public void WithDisplayText (string displayText)
    {
      ArgumentUtility.CheckNotNull("displayText", displayText);

      var executor = JavaScriptExecutor.GetJavaScriptExecutor(_fluentAutoComplete.Target.AutoComplete);
      var ok = JavaScriptExecutor.ExecuteStatement<bool>(executor, c_selectWithDisplayText, GetInputField(_fluentAutoComplete), displayText);
      if (!ok)
        throw new InvalidOperationException(string.Format("Could not find an item with the Display text '{0}'", displayText));
    }

    /// <inheritdoc />
    public void WithIndex (int oneBasedIndex)
    {
      var executor = JavaScriptExecutor.GetJavaScriptExecutor(_fluentAutoComplete.Target.AutoComplete);
      JavaScriptExecutor.ExecuteVoidStatement(executor, c_selectWithIndex, GetInputField(_fluentAutoComplete), oneBasedIndex);
    }

    private static IWebElement GetInputField (IFluentScreenshotElementWithCovariance<ScreenshotBocAutoCompleteReferenceValueSelectList> fluentPopup)
    {
      return (IWebElement) fluentPopup.Target.AutoComplete.ForControlObjectScreenshot().GetValue().GetTarget().Native;
    }
  }
}