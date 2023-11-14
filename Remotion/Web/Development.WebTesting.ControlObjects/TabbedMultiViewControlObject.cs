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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="T:Remotion.Web.UI.Controls.TabbedMultiView"/>.
  /// </summary>
  public class TabbedMultiViewControlObject
      : WebFormsControlObjectWithDiagnosticMetadata, IControlHost, IControlObjectWithTabs, IFluentControlObjectWithTabs
  {
    public TabbedMultiViewControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Returns the top control scope.
    /// </summary>
    public ScopeControlObject GetTopControls ()
    {
      var scope = Scope.FindChild("TopControl");
      return new ScopeControlObject(Context.CloneForControl(scope));
    }

    /// <summary>
    /// Returns the tabbed multi view's active view.
    /// </summary>
    public ScopeControlObject GetActiveView ()
    {
      var scope = Scope.FindChild("ActiveView");
      return new ScopeControlObject(Context.CloneForControl(scope));
    }

    /// <summary>
    /// Returns the bottom control scope.
    /// </summary>
    public ScopeControlObject GetBottomControls ()
    {
      var scope = Scope.FindChild("BottomControl");
      return new ScopeControlObject(Context.CloneForControl(scope));
    }

    /// <inheritdoc/>
    public WebTabStripTabDefinition GetSelectedTab ()
    {
      var tabDefinition = GetTabStrip().GetSelectedTab();
      return ConvertToTabbedMultiViewTab(tabDefinition);
    }

    /// <inheritdoc/>
    public IReadOnlyList<WebTabStripTabDefinition> GetTabDefinitions ()
    {
      return GetTabStrip().GetTabDefinitions().Select(ConvertToTabbedMultiViewTab).ToList();
    }

    private WebTabStripTabDefinition ConvertToTabbedMultiViewTab ([NotNull] WebTabStripTabDefinition tabDefinition)
    {
      ArgumentUtility.CheckNotNull("tabDefinition", tabDefinition);

      return new WebTabStripTabDefinition(
          tabDefinition.ItemID.Substring(0, tabDefinition.ItemID.Length - "_Tab".Length),
          tabDefinition.Index,
          tabDefinition.Title,
          tabDefinition.IsDisabled,
          tabDefinition.AccessKey);
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithTabs SwitchTo ()
    {
      return this;
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject SwitchTo (string itemID, IWebTestActionOptions? actionOptions = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty("itemID", itemID);

      return SwitchTo().WithItemID(itemID, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithTabs.WithItemID (string itemID, IWebTestActionOptions? actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty("itemID", itemID);

      return GetTabStrip().SwitchTo(itemID + "_Tab", actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithTabs.WithIndex (int index, IWebTestActionOptions? actionOptions)
    {
      return GetTabStrip().SwitchTo().WithIndex(index, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithTabs.WithHtmlID (string htmlID, IWebTestActionOptions? actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty("htmlID", htmlID);

      return GetTabStrip().SwitchTo().WithHtmlID(htmlID, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithTabs.WithDisplayText (string displayText, IWebTestActionOptions? actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty("displayText", displayText);

      return GetTabStrip().SwitchTo().WithDisplayText(displayText, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithTabs.WithDisplayTextContains (string containsDisplayText, IWebTestActionOptions? actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty("containsDisplayText", containsDisplayText);

      return GetTabStrip().SwitchTo().WithDisplayTextContains(containsDisplayText, actionOptions);
    }

    private WebTabStripControlObject GetTabStrip ()
    {
      var scope = Scope.FindChild("TabStrip");
      return new WebTabStripControlObject(Context.CloneForControl(scope));
    }

    /// <inheritdoc/>
    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull("controlSelectionCommand", controlSelectionCommand);

      return Children.GetControl(controlSelectionCommand);
    }

    /// <inheritdoc/>
    public TControlObject? GetControlOrNull<TControlObject> (IControlOptionalSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull("controlSelectionCommand", controlSelectionCommand);

      return Children.GetControlOrNull(controlSelectionCommand);
    }

    /// <inheritdoc/>
    public bool HasControl (IControlExistsCommand controlSelectionCommand)
    {
      ArgumentUtility.CheckNotNull("controlSelectionCommand", controlSelectionCommand);

      return Children.HasControl(controlSelectionCommand);
    }
  }
}
