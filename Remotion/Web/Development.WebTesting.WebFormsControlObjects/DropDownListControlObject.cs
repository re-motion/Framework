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
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.Web.Development.WebTesting.WebFormsControlObjects
{
  /// <summary>
  /// Control object for <see cref="T:System.Web.UI.WebControls.DropDownList"/>.
  /// </summary>
  public class DropDownListControlObject
      : WebFormsControlObject,
          IControlObjectWithSelectableOptions,
          IFluentControlObjectWithSelectableOptions,
          IControlObjectWithText,
          IControlObjectWithFormElements,
          ISupportsDisabledState
  {
    public DropDownListControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    /// <inheritdoc/>
    public OptionDefinition GetSelectedOption ()
    {
      return Scope.GetSelectedOption(Logger);
    }

    /// <inheritdoc/>
    public IReadOnlyList<OptionDefinition> GetOptionDefinitions ()
    {
      return RetryUntilTimeout.Run(
          Logger,
          () => Scope.FindAllCss("option")
              .Select((optionScope, i) => new OptionDefinition(optionScope.Value, i + 1, optionScope.Text, optionScope.Selected))
              .ToList());
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      return Scope.GetSelectedOption(Logger).Text;
    }

    /// <inheritdoc />
    public bool IsDisabled ()
    {
      return Scope.Disabled;
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithSelectableOptions SelectOption ()
    {
      return this;
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject SelectOption (string value, IWebTestActionOptions? actionOptions = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty("value", value);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: "SelectOption(value)");

      return SelectOption().WithItemID(value, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithItemID (string value, IWebTestActionOptions? actionOptions)
    {
      ArgumentUtility.CheckNotNull("value", value);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: "SelectOption.WithItemID");

      Action<ElementScope> selectAction = s => s.SelectOptionByValue(value, Logger);
      return SelectOption(selectAction, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithIndex (int oneBasedIndex, IWebTestActionOptions? actionOptions)
    {
      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: "SelectOption.WithIndex");

      Action<ElementScope> selectAction = s => s.SelectOptionByIndex(oneBasedIndex, Logger);
      return SelectOption(selectAction, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithDisplayText (string displayText, IWebTestActionOptions? actionOptions)
    {
      ArgumentUtility.CheckNotNull("displayText", displayText);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: "SelectOption.WithDisplayText");

      Action<ElementScope> selectAction = s => s.SelectOptionByDisplayText(displayText, Logger);
      return SelectOption(selectAction, actionOptions);
    }

    /// <inheritdoc/>
    ICollection<string> IControlObjectWithFormElements.GetFormElementNames ()
    {
      return new[] { Scope.Name };
    }

    private UnspecifiedPageObject SelectOption ([NotNull] Action<ElementScope> selectAction, IWebTestActionOptions? actionOptions = null)
    {
      ArgumentUtility.CheckNotNull("selectAction", selectAction);

      var actualActionOptions = MergeWithDefaultActionOptions(Scope, actionOptions);
      ExecuteAction(new CustomAction(this, Scope, "Select", selectAction, Logger), actualActionOptions);
      return UnspecifiedPage();
    }
  }
}
