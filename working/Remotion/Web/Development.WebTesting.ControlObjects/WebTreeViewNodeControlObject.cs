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
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a node within a <see cref="T:Remotion.Web.UI.Controls.WebTreeView"/>.
  /// </summary>
  public class WebTreeViewNodeControlObject
      : WebFormsControlObjectWithDiagnosticMetadata,
          IControlObjectWithNodes<WebTreeViewNodeControlObject>,
          IFluentControlObjectWithNodes<WebTreeViewNodeControlObject>,
          IControlObjectWithText
  {
    public WebTreeViewNodeControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      return Scope[DiagnosticMetadataAttributes.Content];
    }

    /// <summary>
    /// Returns whether the node is currently selected.
    /// </summary>
    public bool IsSelected ()
    {
      return Scope[DiagnosticMetadataAttributes.WebTreeViewIsSelectedNode] != null;
    }

    /// <summary>
    /// Returns the number of child nodes.
    /// </summary>
    public int GetNumberOfChildren ()
    {
      var numberChildren = Scope[DiagnosticMetadataAttributes.WebTreeViewNumberOfChildren];
      if (numberChildren == DiagnosticMetadataAttributes.Null)
        throw new MissingHtmlException ("TreeViewNode is not evaluated.");

      return int.Parse (numberChildren);
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithNodes<WebTreeViewNodeControlObject> GetNode ()
    {
      return this;
    }

    /// <inheritdoc/>
    public WebTreeViewNodeControlObject GetNode (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return GetNode().WithItemID (itemID);
    }

    /// <inheritdoc/>
    public WebTreeViewNodeControlObject GetNode (int oneBasedIndex)
    {
      return GetNode().WithIndex (oneBasedIndex);
    }

    /// <inheritdoc/>
    WebTreeViewNodeControlObject IFluentControlObjectWithNodes<WebTreeViewNodeControlObject>.WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var nodeScope = Scope.FindTagWithAttribute ("ul li", DiagnosticMetadataAttributes.ItemID, itemID);
      return new WebTreeViewNodeControlObject (Context.CloneForControl (nodeScope));
    }

    /// <inheritdoc/>
    WebTreeViewNodeControlObject IFluentControlObjectWithNodes<WebTreeViewNodeControlObject>.WithIndex (int oneBasedIndex)
    {
      var nodeScope = Scope.FindTagWithAttribute ("ul li", DiagnosticMetadataAttributes.IndexInCollection, oneBasedIndex.ToString());
      return new WebTreeViewNodeControlObject (Context.CloneForControl (nodeScope));
    }

    /// <inheritdoc/>
    WebTreeViewNodeControlObject IFluentControlObjectWithNodes<WebTreeViewNodeControlObject>.WithDisplayText (string displayText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("displayText", displayText);

      var nodeScope = Scope.FindTagWithAttribute ("ul li", DiagnosticMetadataAttributes.Content, displayText);
      return new WebTreeViewNodeControlObject (Context.CloneForControl (nodeScope));
    }

    /// <inheritdoc/>
    WebTreeViewNodeControlObject IFluentControlObjectWithNodes<WebTreeViewNodeControlObject>.WithDisplayTextContains (string containsDisplayText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("containsDisplayText", containsDisplayText);

      var nodeScope = Scope.FindTagWithAttributeUsingOperator (
          "ul li",
          CssComparisonOperator.SubstringMatch,
          DiagnosticMetadataAttributes.Content,
          containsDisplayText);
      return new WebTreeViewNodeControlObject (Context.CloneForControl (nodeScope));
    }

    /// <summary>
    /// Returns <see langword="true" /> if the node has been evaluated, otherwise <see langword="false" />.
    /// </summary>
    public bool IsEvaluated ()
    {
      int nodeCount;
      return int.TryParse (Scope[DiagnosticMetadataAttributes.WebTreeViewNumberOfChildren], out nodeCount);
    }

    /// <summary>
    /// Returns <see langword="true" /> if the node can be expanded/collapsed - it has at 
    /// least one child or is not evaluated yet, otherwise <see langword="false" />.
    /// </summary>
    public bool IsExpandable ()
    {
      var numberOfChildren = Scope[DiagnosticMetadataAttributes.WebTreeViewNumberOfChildren];
      if (numberOfChildren == DiagnosticMetadataAttributes.Null)
        return true;

      return int.Parse (numberOfChildren) != 0;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the node is expanded, otherwise <see langword="false" />.
    /// </summary>
    public bool IsExpanded ()
    {
      return Scope[DiagnosticMetadataAttributes.WebTreeViewIsExpanded] == "true";
    }

    /// <summary>
    /// Expands the node.
    /// </summary>
    public WebTreeViewNodeControlObject Expand ()
    {
      if (!IsExpandable())
        throw new MissingHtmlException ("The WebTreeViewNode can not be expanded as it has no children.");
      if (IsExpanded())
        throw new MissingHtmlException ("TreeViewNode is already expanded.");

      ToggleExpansion();

      return this;
    }

    /// <summary>
    /// Collapses the node.
    /// </summary>
    public WebTreeViewNodeControlObject Collapse ()
    {
      if (!IsExpandable())
        throw new MissingHtmlException ("The WebTreeViewNode can not be collapsed as it has no children.");
      if (!IsExpanded())
        throw new MissingHtmlException ("TreeViewNode is already collapsed.");

      ToggleExpansion();

      return this;
    }

    /// <summary>
    /// Selects the node by clicking on it, returns the node.
    /// </summary>
    public WebTreeViewNodeControlObject Select ([CanBeNull] IWebTestActionOptions actionOptions = null)
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
      var selectAnchorScope = Scope.FindCss ("span > span > a");

      var actualCompletionDetector = MergeWithDefaultActionOptions (selectAnchorScope, actionOptions);
      new ClickAction (this, selectAnchorScope).Execute (actualCompletionDetector);
    }

    public ContextMenuControlObject GetContextMenu ()
    {
      var contextMenuScope = Scope.FindXPath ("./span/span");

      return new ContextMenuControlObject (Context.CloneForControl (contextMenuScope));
    }

    private void ToggleExpansion ()
    {
      var toggleAnchor = Scope.FindCss ("span > a");

      var actionOptions = MergeWithDefaultActionOptions (toggleAnchor, null);
      new ClickAction (this, toggleAnchor).Execute (actionOptions);
    }
  }
}