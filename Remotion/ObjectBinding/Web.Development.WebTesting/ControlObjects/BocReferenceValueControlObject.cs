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
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValue"/>.
  /// </summary>
  public class BocReferenceValueControlObject
      : BocControlObject,
          IDropDownMenuHost,
          IControlObjectWithSelectableOptions,
          IFluentControlObjectWithSelectableOptions,
          IControlObjectWithText,
          IControlObjectWithFormElements,
          ISupportsValidationErrors,
          ISupportsValidationErrorsForReadOnly
  {
    public BocReferenceValueControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Gets a flag that indicates if the control displays the value that represents <see langword="null" /> in the domain (i.e. the 'undefined' value).
    /// </summary>
    public bool HasNullOptionDefinition ()
    {
      var nullIdentifier = Scope[DiagnosticMetadataAttributesForObjectBinding.NullIdentifier];

      if (IsReadOnly())
      {
        var scope = GetReadOnlyValueScope();
        return scope["data-value"] == nullIdentifier;
      }

      return GetOptionDefinitions().Any(o => o.ItemID == nullIdentifier);
    }

    /// <inheritdoc/>
    public OptionDefinition GetSelectedOption ()
    {
      if (IsReadOnly())
      {
        var scope = GetReadOnlyValueScope();
        return new OptionDefinition(scope["data-value"], -1, scope.Value, true);
      }
      else
      {
        var scope = GetValueScope();
        return scope.GetSelectedOption(Logger);
      }
    }

    /// <inheritdoc/>
    public IReadOnlyList<OptionDefinition> GetOptionDefinitions ()
    {
      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      return RetryUntilTimeout.Run(
          Logger,
          () => GetValueScope().FindAllCss("option")
              .Select((optionScope, i) => new OptionDefinition(optionScope.Value, i + 1, optionScope.Text, optionScope.Selected))
              .ToList());
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      if (IsReadOnly())
      {
        var scope = GetReadOnlyValueScope();
        return scope.Value; // do not trim
      }
      else
      {
        var scope = GetValueScope();
        return scope.GetSelectedOption(Logger).Text;
      }
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithSelectableOptions SelectOption ()
    {
      return this;
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject SelectOption (string itemID, IWebTestActionOptions? actionOptions = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty("itemID", itemID);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: "SelectOption(itemID)");

      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      return SelectOption().WithItemID(itemID, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithItemID (string itemID, IWebTestActionOptions? actionOptions)
    {
      ArgumentUtility.CheckNotNull("itemID", itemID);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: "SelectOption.WithItemID");

      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      // Workaround for Marionette issue. (RM-7279)
      if (Scope.Browser.IsFirefox() && GetSelectedOption().ItemID == itemID)
        return UnspecifiedPage();

      Action<ElementScope> selectAction = s => s.SelectOptionByValue(itemID, Logger);
      return SelectOption(selectAction, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithIndex (int oneBasedIndex, IWebTestActionOptions? actionOptions)
    {
      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: "SelectOption.WithIndex");

      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      // Workaround for Marionette issue. (RM-7279)
      if (Scope.Browser.IsFirefox() && GetSelectedOption().Index == oneBasedIndex)
        return UnspecifiedPage();

      Action<ElementScope> selectAction = s => s.SelectOptionByIndex(oneBasedIndex, Logger);
      return SelectOption(selectAction, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithDisplayText (string displayText, IWebTestActionOptions? actionOptions)
    {
      ArgumentUtility.CheckNotNull("displayText", displayText);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: "SelectOption.WithDisplayText");

      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      // Workaround for Marionette issue. (RM-7279)
      if (Scope.Browser.IsFirefox() && GetSelectedOption().Text == displayText)
        return UnspecifiedPage();

      Action<ElementScope> selectAction = s => s.SelectOption(displayText);
      return SelectOption(selectAction, actionOptions);
    }

    private UnspecifiedPageObject SelectOption ([NotNull] Action<ElementScope> selectAction, IWebTestActionOptions? actionOptions)
    {
      ArgumentUtility.CheckNotNull("selectAction", selectAction);

      var actualActionOptions = MergeWithDefaultActionOptions(Scope, actionOptions);
      ExecuteAction(new CustomAction(this, GetValueScope(), "Select", selectAction, Logger), actualActionOptions);
      return UnspecifiedPage();
    }

    /// <inheritdoc/>
    public DropDownMenuControlObject GetDropDownMenu ()
    {
      var dropDownMenuScope = Scope.FindChild("Boc_OptionsMenu");
      return new DropDownMenuControlObject(Context.CloneForControl(dropDownMenuScope));
    }

    /// <summary>
    /// See <see cref="IControlObjectWithFormElements.GetFormElementNames"/>. Returns the select (value) as only element.
    /// </summary>
    ICollection<string> IControlObjectWithFormElements.GetFormElementNames ()
    {
      return new[] { string.Format("{0}_Value", GetHtmlID()) };
    }

    public IReadOnlyList<string> GetValidationErrors ()
    {
      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      return GetValidationErrors(GetValueScope());
    }

    public IReadOnlyList<string> GetValidationErrorsForReadOnly ()
    {
      return GetValidationErrorsForReadOnly(GetLabeledElementScope());
    }

    protected override ElementScope GetLabeledElementScope ()
    {
      if (IsReadOnly())
        return GetReadOnlyValueScope();
      else
        return GetValueScope();
    }

    private ElementScope GetReadOnlyValueScope ()
    {
      return Scope.FindChild("TextValue");
    }

    private ElementScope GetValueScope ()
    {
      return Scope.FindChild("Value");
    }
  }
}
