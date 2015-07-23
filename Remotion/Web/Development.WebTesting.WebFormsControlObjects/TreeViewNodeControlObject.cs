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
  /// Control object representing a node within a <see cref="T:System.Web.UI.WebControls.TreeView"/>.
  /// </summary>
  public class TreeViewNodeControlObject
      : WebFormsControlObject,
          IControlObjectWithNodes<TreeViewNodeControlObject>,
          IFluentControlObjectWithNodes<TreeViewNodeControlObject>,
          IControlObjectWithText
  {
    public TreeViewNodeControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      const string xpath = "./tbody/tr/td[last()]";
      return Scope.FindXPath (xpath).Text.Trim();
    }

    /// <summary>
    /// Returns whether the node is checked.
    /// </summary>
    public bool IsChecked ()
    {
      var checkedAttr = GetCheckboxScope()["checked"];
      return checkedAttr != null && checkedAttr == "true";
    }

    /// <summary>
    /// Returns whether the node is the currently selected node.
    /// </summary>
    public bool IsSelected ()
    {
      throw new NotSupportedException (
          "The ASP.NET TreeView control does not indicate which node is currently selected, therefore IsSelected() is not supported.");
    }

    /// <summary>
    /// Returns the number of child nodes.
    /// </summary>
    public int GetNumberOfChildren ()
    {
      return RetryUntilTimeout.Run (() => GetChildrenScope().FindAllXPath ("./table").Count());
    }

    /// <summary>
    /// Expands the node.
    /// </summary>
    public TreeViewNodeControlObject Expand ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      var actualCompletionDetector = MergeWithDefaultActionOptions (Scope, actionOptions);

      const string xpath = "./tbody/tr/td/a[contains(@href,\"','t\")]";
      var expandLinkScope = Scope.FindXPath (xpath);
      new SimpleClickAction (this, expandLinkScope).Execute (actualCompletionDetector);
      return this;
    }

    /// <summary>
    /// Collapses the node.
    /// </summary>
    public TreeViewNodeControlObject Collapse ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      return Expand (actionOptions);
    }

    /// <summary>
    /// Checks the node's checkbox.
    /// </summary>
    public UnspecifiedPageObject Check ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      var actualCompletionDetector = actionOptions ?? Opt.ContinueImmediately();
      new CheckAction (this, GetCheckboxScope()).Execute (actualCompletionDetector);
      return UnspecifiedPage();
    }

    /// <summary>
    /// Unchecks the node's checkbox.
    /// </summary>
    public UnspecifiedPageObject Uncheck ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      var actualCompletionDetector = actionOptions ?? Opt.ContinueImmediately();
      new UncheckAction (this, GetCheckboxScope()).Execute (actualCompletionDetector);
      return UnspecifiedPage();
    }

    /// <summary>
    /// Selects the node by clicking on it, returns the node.
    /// </summary>
    public TreeViewNodeControlObject Select ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      ClickNode (actionOptions);
      return this;
    }

    /// <summary>
    /// Selects the node by clicking on it, returns the following page.
    /// </summary>
    public UnspecifiedPageObject Click ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      ClickNode (actionOptions);
      return UnspecifiedPage();
    }

    private void ClickNode (IWebTestActionOptions actionOptions)
    {
      var actualCompletionDetector = MergeWithDefaultActionOptions (Scope, actionOptions);
      const string nodeClickScopeXpath = "./tbody/tr/td[a[contains(@onclick, 'TreeView_SelectNode')]][last()]/a[last()]";
      new ClickAction (this, Scope.FindXPath (nodeClickScopeXpath)).Execute (actualCompletionDetector);
    }

    private ElementScope GetChildrenScope ()
    {
      const string xpath = "./following-sibling::div[1]";
      return Scope.FindXPath (xpath);
    }

    private ElementScope GetCheckboxScope ()
    {
      const string xpath = "./tbody/tr/td[a[contains(@onclick, 'TreeView_SelectNode')]]/input[@type='checkbox']";
      return Scope.FindXPath (xpath);
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithNodes<TreeViewNodeControlObject> GetNode ()
    {
      return this;
    }

    /// <inheritdoc/>
    public TreeViewNodeControlObject GetNode (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return GetNode().WithItemID (itemID);
    }

    /// <inheritdoc/>
    public TreeViewNodeControlObject GetNode (int index)
    {
      return GetNode().WithIndex (index);
    }

    /// <inheritdoc/>
    TreeViewNodeControlObject IFluentControlObjectWithNodes<TreeViewNodeControlObject>.WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      throw new NotSupportedException ("The TreeViewNodeControlObject does not support node selection by item ID.");
    }

    /// <inheritdoc/>
    TreeViewNodeControlObject IFluentControlObjectWithNodes<TreeViewNodeControlObject>.WithIndex (int index)
    {
      var xpath = string.Format ("[{0}]", index);
      return FindAndCreateNode (xpath);
    }

    /// <inheritdoc/>
    TreeViewNodeControlObject IFluentControlObjectWithNodes<TreeViewNodeControlObject>.WithDisplayText (string displayText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("displayText", displayText);

      var xpath = string.Format ("[normalize-space(tbody/tr/td[last()]//*)='{0}']", displayText);
      return FindAndCreateNode (xpath);
    }

    /// <inheritdoc/>
    TreeViewNodeControlObject IFluentControlObjectWithNodes<TreeViewNodeControlObject>.WithDisplayTextContains (string containsDisplayText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("containsDisplayText", containsDisplayText);

      var xpath = string.Format ("[contains(tbody/tr/td[last()]//*, '{0}')]", containsDisplayText);
      return FindAndCreateNode (xpath);
    }

    private TreeViewNodeControlObject FindAndCreateNode (string xpathSuffix)
    {
      var nodeScope = GetChildrenScope().FindXPath ("./table" + xpathSuffix);
      return new TreeViewNodeControlObject (Context.CloneForControl (nodeScope));
    }
  }
}