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

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing a collection of selectable items, e.g. all kinds of menus like a
  /// list menu or a tabbed menu.
  /// </summary>
  /// <seealso cref="T:Remotion.Web.Development.WebTesting.ControlObjects.ListMenuControlObject"/>
  /// <seealso cref="T:Remotion.Web.Development.WebTesting.ControlObjects.TabbedMenuControlObject"/>
  public interface IControlObjectWithSelectableItems
  {
    /// <summary>
    /// Returns all selectable items. 
    /// Warning: this method does not wait until "the element" is available but detects all available items at the moment of calling.
    /// </summary>
    /// <exception cref="MissingHtmlException">The element is currently disabled.</exception>
    IReadOnlyList<ItemDefinition> GetItemDefinitions ();

    /// <summary>
    /// Start of the fluent interface for selecting an item.
    /// </summary>
    IFluentControlObjectWithSelectableItems SelectItem ();

    /// <summary>
    /// Short for explicitly implemented <see cref="IFluentControlObjectWithSelectableItems.WithItemID"/>.
    /// </summary>
    /// <exception cref="MissingHtmlException">The element is currently disabled.</exception>
    UnspecifiedPageObject SelectItem ([NotNull] string itemID, [CanBeNull] IWebTestActionOptions actionOptions = null);
  }

  /// <summary>
  /// Fluent interface for completing the <see cref="IControlObjectWithSelectableItems.SelectItem()"/> call.
  /// </summary>
  public interface IFluentControlObjectWithSelectableItems
  {
    /// <summary>
    /// Selects the item using the given <paramref name="itemID"/>.
    /// </summary>
    /// <exception cref="MissingHtmlException">The element is currently disabled.</exception>
    UnspecifiedPageObject WithItemID ([NotNull] string itemID, [CanBeNull] IWebTestActionOptions actionOptions = null);

    /// <summary>
    /// Selects item row using the given <paramref name="oneBasedIndex"/>.
    /// </summary>
    /// <exception cref="MissingHtmlException">The element is currently disabled.</exception>
    UnspecifiedPageObject WithIndex (int oneBasedIndex, [CanBeNull] IWebTestActionOptions actionOptions = null);

    /// <summary>
    /// Selects item row using the given <paramref name="htmlID"/>.
    /// </summary>
    /// <exception cref="MissingHtmlException">The element is currently disabled.</exception>
    UnspecifiedPageObject WithHtmlID ([NotNull] string htmlID, [CanBeNull] IWebTestActionOptions actionOptions = null);

    /// <summary>
    /// Selects item row using the given <paramref name="displayText"/>.
    /// </summary>
    /// <exception cref="MissingHtmlException">The element is currently disabled.</exception>
    UnspecifiedPageObject WithDisplayText ([NotNull] string displayText, [CanBeNull] IWebTestActionOptions actionOptions = null);

    /// <summary>
    /// Selects item row using the given <paramref name="containsDisplayText"/>.
    /// </summary>
    /// <exception cref="MissingHtmlException">The element is currently disabled.</exception>
    UnspecifiedPageObject WithDisplayTextContains ([NotNull] string containsDisplayText, [CanBeNull] IWebTestActionOptions actionOptions = null);
  }
}