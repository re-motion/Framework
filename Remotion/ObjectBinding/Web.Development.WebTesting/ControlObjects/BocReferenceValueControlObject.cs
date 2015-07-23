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
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValue"/>.
  /// </summary>
  public class BocReferenceValueControlObject
      : BocControlObject,
          ICommandHost,
          IDropDownMenuHost,
          IControlObjectWithSelectableOptions,
          IFluentControlObjectWithSelectableOptions,
          IControlObjectWithText,
          IControlObjectWithFormElements
  {
    public BocReferenceValueControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public OptionDefinition GetSelectedOption ()
    {
      var scope = Scope.FindChild ("Value");
      if (IsReadOnly())
        return new OptionDefinition (scope["data-value"], -1, scope.Text);
      else
        return scope.GetSelectedOption();
    }

    /// <inheritdoc/>
    public IReadOnlyList<OptionDefinition> GetOptionDefinitions ()
    {
      if (IsReadOnly())
        throw new InvalidOperationException ("Cannot obtain option definitions on read-only control.");

      return RetryUntilTimeout.Run (
          () => Scope.FindChild("Value").FindAllCss ("option")
              .Select ((optionScope, i) => new OptionDefinition (optionScope.Value, i + 1, optionScope.Text))
              .ToList());
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      var scope = Scope.FindChild ("Value");
      if (IsReadOnly())
        return scope.Text; // do not trim
      else
        return scope.GetSelectedOption().Text;
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithSelectableOptions SelectOption ()
    {
      return this;
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject SelectOption (string itemID, IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return SelectOption().WithItemID (itemID, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithItemID (string itemID, IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNull ("itemID", itemID);

      Action<ElementScope> selectAction = s => s.SelectOptionByValue (itemID);
      return SelectOption (selectAction, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithIndex (int index, IWebTestActionOptions actionOptions)
    {
      Action<ElementScope> selectAction = s => s.SelectOptionByIndex (index);
      return SelectOption (selectAction, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithDisplayText (string displayText, IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNull ("displayText", displayText);

      Action<ElementScope> selectAction = s => s.SelectOption (displayText);
      return SelectOption (selectAction, actionOptions);
    }

    private UnspecifiedPageObject SelectOption ([NotNull] Action<ElementScope> selectAction, IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNull ("selectAction", selectAction);

      var actualActionOptions = MergeWithDefaultActionOptions (Scope, actionOptions);
      new CustomAction (this, Scope.FindChild ("Value"), "Select", selectAction).Execute (actualActionOptions);
      return UnspecifiedPage();
    }

    /// <inheritdoc/>
    public CommandControlObject GetCommand ()
    {
      var commandScope = Scope.FindChild ("Command");
      return new CommandControlObject (Context.CloneForControl (commandScope));
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject ExecuteCommand (IWebTestActionOptions actionOptions = null)
    {
      return GetCommand().Click (actionOptions);
    }

    /// <inheritdoc/>
    public DropDownMenuControlObject GetDropDownMenu ()
    {
      var dropDownMenuScope = Scope.FindChild ("Boc_OptionsMenu");
      return new DropDownMenuControlObject (Context.CloneForControl (dropDownMenuScope));
    }

    /// <summary>
    /// See <see cref="IControlObjectWithFormElements.GetFormElementNames"/>. Returns the select (value) as only element.
    /// </summary>
    ICollection<string> IControlObjectWithFormElements.GetFormElementNames ()
    {
      return new[] { string.Format ("{0}_Value", GetHtmlID()) };
    }
  }
}