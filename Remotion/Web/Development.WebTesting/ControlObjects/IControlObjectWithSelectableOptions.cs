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

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing a collection of selectable options, e.g. a BOC reference value.
  /// </summary>
  /// <seealso cref="T:Remotion.Web.Development.WebTesting.ControlObjects.DropDownListControlObject"/>
  public interface IControlObjectWithSelectableOptions
  {
    /// <summary>
    /// Returns the currently selected option. Note that the <see cref="OptionDefinition.Index"/> is set to -1.
    /// </summary>
    OptionDefinition GetSelectedOption ();

    /// <summary>
    /// Returns all available options. 
    /// Warning: this method does not wait until "the element" is available but detects all available options at the moment of calling.
    /// </summary>
    IReadOnlyList<OptionDefinition> GetOptionDefinitions ();

    /// <summary>
    /// Start of the fluent interface for selecting an option.
    /// </summary>
    IFluentControlObjectWithSelectableOptions SelectOption ();

    /// <summary>
    /// Short for explicitly implemented <see cref="IFluentControlObjectWithSelectableOptions.WithItemID"/>.
    /// </summary>
    UnspecifiedPageObject SelectOption ([NotNull] string itemID, [CanBeNull] IWebTestActionOptions actionOptions = null);
  }

  /// <summary>
  /// Fluent interface for completing the <see cref="IControlObjectWithSelectableOptions.SelectOption()"/> call.
  /// </summary>
  public interface IFluentControlObjectWithSelectableOptions
  {
    /// <summary>
    /// Selects the option using the given <paramref name="itemID"/>.
    /// </summary>
    UnspecifiedPageObject WithItemID ([NotNull] string itemID, [CanBeNull] IWebTestActionOptions actionOptions = null);

    /// <summary>
    /// Selects the option using the given <paramref name="index"/>.
    /// </summary>
    UnspecifiedPageObject WithIndex (int index, [CanBeNull] IWebTestActionOptions actionOptions = null);

    /// <summary>
    /// Selects the option using the given <paramref name="displayText"/>.
    /// </summary>
    UnspecifiedPageObject WithDisplayText ([NotNull] string displayText, [CanBeNull] IWebTestActionOptions actionOptions = null);
  }
}