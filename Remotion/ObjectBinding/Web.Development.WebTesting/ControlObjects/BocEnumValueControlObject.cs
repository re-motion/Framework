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
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue"/> control.
  /// </summary>
  public class BocEnumValueControlObject
      : BocControlObject,
          IControlObjectWithSelectableOptions,
          IFluentControlObjectWithSelectableOptions,
          IControlObjectWithFormElements,
          ISupportsValidationErrors,
          ISupportsValidationErrorsForReadOnly
  {
    private readonly IBocEnumValueControlObjectVariant _variantImpl;

    public BocEnumValueControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
      var style = Scope[DiagnosticMetadataAttributesForObjectBinding.BocEnumValueStyle];
      _variantImpl = CreateVariant(style);
      _variantImpl.ActionExecute += OnActionExecute;
    }

    /// <summary>
    /// Gets a flag that indicates if the control displays the value that represents <see langword="null" /> in the domain (i.e. the 'undefined' value).
    /// </summary>
    public bool HasNullOptionDefinition ()
    {
      if (IsReadOnly())
        throw new InvalidOperationException("A read-only control cannot contain a null option definition.");

      return _variantImpl.HasNullOptionDefinition();
    }

    /// <inheritdoc/>
    public OptionDefinition GetSelectedOption ()
    {
      return _variantImpl.GetSelectedOption();
    }

    /// <inheritdoc/>
    public IReadOnlyList<OptionDefinition> GetOptionDefinitions ()
    {
      if (IsReadOnly())
        throw new InvalidOperationException("Cannot obtain option definitions on read-only control.");

      return _variantImpl.GetOptionDefinitions();
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

      return SelectOption().WithItemID(itemID, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithItemID (string itemID, IWebTestActionOptions? actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty("itemID", itemID);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: "SelectOption.WithItemID");

      // Workaround for Marionette issue. (RM-7279)
      if (Scope.Browser.IsFirefox() && GetSelectedOption().ItemID == itemID)
        return UnspecifiedPage();

      return _variantImpl.SelectOption(itemID, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithIndex (int oneBasedIndex, IWebTestActionOptions? actionOptions)
    {
      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: "SelectOption.WithIndex");

      // Workaround for Marionette issue. (RM-7279)
      if (Scope.Browser.IsFirefox() && GetSelectedOption().Index == oneBasedIndex)
        return UnspecifiedPage();

      return _variantImpl.SelectOption(oneBasedIndex, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithDisplayText (string displayText, IWebTestActionOptions? actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty("displayText", displayText);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver, operationName: "SelectOption.WithDisplayText");

      // Workaround for Marionette issue. (RM-7279)
      if (Scope.Browser.IsFirefox() && GetSelectedOption().Text == displayText)
        return UnspecifiedPage();

      return _variantImpl.SelectOptionByText(displayText, actionOptions);
    }

    /// <summary>
    /// See <see cref="IControlObjectWithFormElements.GetFormElementNames"/>. Returns the select (value) as only element if the display style is
    /// either <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.ListControlType.DropDownList"/> or
    /// <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.ListControlType.ListBox"/>. Returns the common name of the input[type=radio] elements if
    /// the display style is <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.ListControlType.RadioButtonList"/>.
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

    /// <summary>
    /// Factory method, creates a <see cref="IBocEnumValueControlObjectVariant"/> from the given <paramref name="style"/>, which must be one of
    /// <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.ListControlType.ToString"/>.
    /// </summary>
    /// <param name="style">One of <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.ListControlType.ToString"/>.</param>
    /// <returns>The corresponding <see cref="IBocEnumValueControlObjectVariant"/> implementation.</returns>
    private IBocEnumValueControlObjectVariant CreateVariant ([NotNull] string style)
    {
      ArgumentUtility.CheckNotNullOrEmpty("style", style);

      switch (style)
      {
        case "DropDownList":
          return new BocEnumValueSelectBasedControlObjectVariant(this);

        case "ListBox":
          return new BocEnumValueSelectBasedControlObjectVariant(this);

        case "RadioButtonList":
          return new BocEnumValueRadioButtonBasedControlObjectVariant(this);
      }

      throw new ArgumentException("style argument must be one of Remotion.ObjectBinding.Web.UI.Controls.ListControlType.", "style");
    }

    /// <summary>
    /// Declares all methods a boc enum rendering variant must support.
    /// </summary>
    private interface IBocEnumValueControlObjectVariant : IControlObjectNotifier
    {
      OptionDefinition GetSelectedOption ();
      IReadOnlyList<OptionDefinition> GetOptionDefinitions ();

      UnspecifiedPageObject SelectOption ([NotNull] string itemID, IWebTestActionOptions? actionOptions);
      UnspecifiedPageObject SelectOption (int oneBasedIndex, IWebTestActionOptions? actionOptions);
      UnspecifiedPageObject SelectOptionByText ([NotNull] string text, IWebTestActionOptions? actionOptions);
      bool HasNullOptionDefinition ();
    }

    /// <summary>
    /// <see cref="IBocEnumValueControlObjectVariant"/> implementation for the
    /// <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.ListControlType.DropDownList"/> and
    /// <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.ListControlType.ListBox"/> style.
    /// </summary>
    private class BocEnumValueSelectBasedControlObjectVariant : IBocEnumValueControlObjectVariant
    {
      public event Action<WebTestAction, IWebTestActionOptions>? ActionExecute;

      private readonly BocEnumValueControlObject _controlObject;

      public BocEnumValueSelectBasedControlObjectVariant ([NotNull] BocEnumValueControlObject controlObject)
      {
        ArgumentUtility.CheckNotNull("controlObject", controlObject);

        _controlObject = controlObject;
      }

      public OptionDefinition GetSelectedOption ()
      {
        if (_controlObject.IsReadOnly())
        {
          var scope = _controlObject.GetReadOnlyValueScope();
          return new OptionDefinition(scope["data-value"], -1, scope.FindXPath("../span[@aria-hidden='true']").Text, true);
        }
        else
        {
          var scope = _controlObject.GetValueScope();
          return scope.GetSelectedOption(_controlObject.Logger);
        }
      }

      public IReadOnlyList<OptionDefinition> GetOptionDefinitions ()
      {
        return RetryUntilTimeout.Run(
            _controlObject.Logger,
            () => _controlObject.GetValueScope().FindAllCss("option")
                .Select((optionScope, i) => new OptionDefinition(optionScope.Value, i + 1, optionScope.Text, optionScope.Selected))
                .ToList());
      }

      public UnspecifiedPageObject SelectOption (string itemID, IWebTestActionOptions? actionOptions)
      {
        ArgumentUtility.CheckNotNull("itemID", itemID);

        Action<ElementScope> selectAction = s => s.SelectOptionByValue(itemID, _controlObject.Logger);
        return SelectOption(selectAction, actionOptions);
      }

      public UnspecifiedPageObject SelectOption (int oneBasedIndex, IWebTestActionOptions? actionOptions)
      {
        Action<ElementScope> selectAction = s => s.SelectOptionByIndex(oneBasedIndex, _controlObject.Logger);
        return SelectOption(selectAction, actionOptions);
      }

      public UnspecifiedPageObject SelectOptionByText (string text, IWebTestActionOptions? actionOptions)
      {
        ArgumentUtility.CheckNotNull("text", text);

        Action<ElementScope> selectAction = s => s.SelectOption(text);
        return SelectOption(selectAction, actionOptions);
      }

      private UnspecifiedPageObject SelectOption ([NotNull] Action<ElementScope> selectAction, IWebTestActionOptions? actionOptions)
      {
        ArgumentUtility.CheckNotNull("selectAction", selectAction);

        var actualActionOptions = _controlObject.MergeWithDefaultActionOptions(_controlObject.Scope, actionOptions);

        var customAction = new CustomAction(_controlObject, _controlObject.GetValueScope(), "Select", selectAction, _controlObject.Logger);
        ActionExecute?.Invoke(customAction, actualActionOptions);
        customAction.Execute(actualActionOptions);

        return _controlObject.UnspecifiedPage();
      }

      public bool HasNullOptionDefinition ()
      {
        var nullIdentifier = _controlObject.Scope[DiagnosticMetadataAttributesForObjectBinding.NullIdentifier];
        return GetOptionDefinitions().Any(o => o.ItemID == nullIdentifier);
      }
    }

    /// <summary>
    /// <see cref="IBocEnumValueControlObjectVariant"/> implementation for the
    /// <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.ListControlType.RadioButtonList"/> style.
    /// </summary>
    private class BocEnumValueRadioButtonBasedControlObjectVariant : IBocEnumValueControlObjectVariant
    {
      public event Action<WebTestAction, IWebTestActionOptions>? ActionExecute;

      private readonly BocEnumValueControlObject _controlObject;

      public BocEnumValueRadioButtonBasedControlObjectVariant ([NotNull] BocEnumValueControlObject controlObject)
      {
        ArgumentUtility.CheckNotNull("controlObject", controlObject);

        _controlObject = controlObject;
      }

      public OptionDefinition GetSelectedOption ()
      {
        if (_controlObject.IsReadOnly())
        {
          var scope = _controlObject.GetReadOnlyValueScope();
          return new OptionDefinition(scope.FindCss("span[data-value]")["data-value"], -1, scope.FindXPath("../span[@aria-hidden='true']").Text, true);
        }

        var selectedOption = GetOptionDefinitions().FirstOrDefault(o => o.IsSelected);
        if (selectedOption != null)
          return new OptionDefinition(selectedOption.ItemID, -1, selectedOption.Text, true);

        var nullIdentifier = _controlObject.Scope[DiagnosticMetadataAttributesForObjectBinding.NullIdentifier];
        return new OptionDefinition(nullIdentifier, -1, "", true);
      }

      public IReadOnlyList<OptionDefinition> GetOptionDefinitions ()
      {
        return RetryUntilTimeout.Run(
            _controlObject.Logger,
            () => _controlObject.Scope.FindAllCss("input[type='radio']")
                .Select((radioScope, i) => CreateOptionDefinitionFromRadioScope(radioScope, i + 1))
                .ToList());
      }

      private OptionDefinition CreateOptionDefinitionFromRadioScope (ElementScope radioScope, int oneBasedIndex)
      {
        var text = radioScope.FindXPath("../label").Text;
        var isSelected = !string.IsNullOrEmpty(radioScope["checked"]);
        return new OptionDefinition(radioScope.Value, oneBasedIndex, text, isSelected);
      }

      public UnspecifiedPageObject SelectOption (string itemID, IWebTestActionOptions? actionOptions)
      {
        ArgumentUtility.CheckNotNull("itemID", itemID);

        var scope = _controlObject.Scope.FindTagWithAttribute("span", DiagnosticMetadataAttributes.ItemID, itemID).FindCss("input");
        return CheckScope(scope, actionOptions);
      }

      public UnspecifiedPageObject SelectOption (int oneBasedIndex, IWebTestActionOptions? actionOptions)
      {
        var scope =
            _controlObject.Scope.FindTagWithAttribute("span", DiagnosticMetadataAttributes.IndexInCollection, oneBasedIndex.ToString()).FindCss("input");
        return CheckScope(scope, actionOptions);
      }

      public UnspecifiedPageObject SelectOptionByText (string text, IWebTestActionOptions? actionOptions)
      {
        ArgumentUtility.CheckNotNull("text", text);

        var scope = _controlObject.Scope.FindTagWithAttribute("span", DiagnosticMetadataAttributes.Content, text).FindCss("input");
        return CheckScope(scope, actionOptions);
      }

      private UnspecifiedPageObject CheckScope ([NotNull] ElementScope scope, IWebTestActionOptions? actionOptions)
      {
        ArgumentUtility.CheckNotNull("scope", scope);

        var actualActionOptions = _controlObject.MergeWithDefaultActionOptions(_controlObject.Scope, actionOptions);

        var checkAction = new CheckAction(_controlObject, scope, _controlObject.Logger);
        ActionExecute?.Invoke(checkAction, actualActionOptions);
        checkAction.Execute(actualActionOptions);

        return _controlObject.UnspecifiedPage();
      }

      public bool HasNullOptionDefinition ()
      {
        var nullIdentifier = _controlObject.Scope[DiagnosticMetadataAttributesForObjectBinding.NullIdentifier];
        return GetOptionDefinitions().Any(o => o.ItemID == nullIdentifier);
      }
    }
  }
}
