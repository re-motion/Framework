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
  /// Interface for all <see cref="ControlObject"/> implementations representing a tab strip.
  /// </summary>
  /// <seealso cref="WebTabStripControlObject"/>
  /// <seealso cref="TabbedMultiViewControlObject"/>
  public interface IControlObjectWithTabs
  {
    /// <summary>
    /// Returns the currently selected tab. Note that the <see cref="WebTabStripTabDefinition.Index"/> is set to -1.
    /// </summary>
    WebTabStripTabDefinition GetSelectedTab ();

    /// <summary>
    /// Returns all available tabs. 
    /// Warning: this method does not wait until "the element" is available but detects all available tabs at the moment of calling.
    /// </summary>
    IReadOnlyList<WebTabStripTabDefinition> GetTabDefinitions ();

    /// <summary>
    /// Start of the fluent interface for switching to a tab.
    /// </summary>
    IFluentControlObjectWithTabs SwitchTo ();

    /// <summary>
    /// Short for explicitly implemented <see cref="IFluentControlObjectWithTabs.WithItemID"/>.
    /// </summary>
    UnspecifiedPageObject SwitchTo ([NotNull] string itemID, [CanBeNull] IWebTestActionOptions actionOptions = null);
  }

  /// <summary>
  /// Fluent interface for completing the <see cref="IControlObjectWithTabs.SwitchTo()"/> call.
  /// </summary>
  public interface IFluentControlObjectWithTabs
  {
    /// <summary>
    /// Switches to the tab using the given <paramref name="itemID"/>.
    /// </summary>
    UnspecifiedPageObject WithItemID ([NotNull] string itemID, [CanBeNull] IWebTestActionOptions actionOptions = null);

    /// <summary>
    /// Switches to the tab using the given <paramref name="index"/>.
    /// </summary>
    UnspecifiedPageObject WithIndex (int index, [CanBeNull] IWebTestActionOptions actionOptions = null);

    /// <summary>
    /// Switches to the tab using the given <paramref name="htmlID"/>.
    /// </summary>
    UnspecifiedPageObject WithHtmlID ([NotNull] string htmlID, [CanBeNull] IWebTestActionOptions actionOptions = null);

    /// <summary>
    /// Switches to the tab using the given <paramref name="displayText"/>.
    /// </summary>
    UnspecifiedPageObject WithDisplayText ([NotNull] string displayText, [CanBeNull] IWebTestActionOptions actionOptions = null);

    /// <summary>
    /// Switches to the tab using the given <paramref name="containsDisplayText"/>.
    /// </summary>
    UnspecifiedPageObject WithDisplayTextContains ([NotNull] string containsDisplayText, [CanBeNull] IWebTestActionOptions actionOptions = null);
  }
}