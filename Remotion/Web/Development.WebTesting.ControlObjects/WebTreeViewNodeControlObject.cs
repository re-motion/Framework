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
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a node within a <see cref="T:Remotion.Web.UI.Controls.WebTreeView"/>.
  /// </summary>
  public class WebTreeViewNodeControlObject
      : WebFormsControlObjectWithDiagnosticMetadata,
          IControlObjectWithNodes<WebTreeViewNodeControlObject>,
          IControlObjectWithText
  {
    private class GetNodeImplementationForChildren : IFluentControlObjectWithNodes<WebTreeViewNodeControlObject>
    {
      private readonly WebTreeViewNodeControlObject _webTreeViewNode;

      public GetNodeImplementationForChildren (WebTreeViewNodeControlObject webTreeViewNode)
      {
        _webTreeViewNode = webTreeViewNode;
      }

      public WebTreeViewNodeControlObject WithItemID (string itemID)
      {
        var xpath = string.Format("((.//ul)[1])/li[@data-item-id={0}]", DomSelectorUtility.CreateMatchValueForXPath(itemID));
        var nodeScope = _webTreeViewNode.Scope.FindXPath(xpath);
        return new WebTreeViewNodeControlObject(_webTreeViewNode.Context.CloneForControl(nodeScope));
      }

      public WebTreeViewNodeControlObject WithIndex (int oneBasedIndex)
      {
        var xpath = string.Format("((.//ul)[1])/li[@data-index={0}]", oneBasedIndex);
        var nodeScope = _webTreeViewNode.Scope.FindXPath(xpath);
        return new WebTreeViewNodeControlObject(_webTreeViewNode.Context.CloneForControl(nodeScope));
      }

      public WebTreeViewNodeControlObject WithDisplayText (string displayText)
      {
        var xpath = string.Format("((.//ul)[1])/li[@data-content={0}]", DomSelectorUtility.CreateMatchValueForXPath(displayText));
        var nodeScope = _webTreeViewNode.Scope.FindXPath(xpath);
        return new WebTreeViewNodeControlObject(_webTreeViewNode.Context.CloneForControl(nodeScope));
      }

      public WebTreeViewNodeControlObject WithDisplayTextContains (string containsDisplayText)
      {
        var xpath = string.Format("((.//ul)[1])/li[contains(@data-content, {0})]", DomSelectorUtility.CreateMatchValueForXPath(containsDisplayText));
        var nodeScope = _webTreeViewNode.Scope.FindXPath(xpath);
        return new WebTreeViewNodeControlObject(_webTreeViewNode.Context.CloneForControl(nodeScope));
      }
    }

    private class GetNodeImplementationForHierarchy : IFluentControlObjectWithNodes<WebTreeViewNodeControlObject>
    {
      private readonly WebTreeViewNodeControlObject _webTreeViewNode;

      public GetNodeImplementationForHierarchy (WebTreeViewNodeControlObject webTreeViewNode)
      {
        _webTreeViewNode = webTreeViewNode;
      }

      public WebTreeViewNodeControlObject WithItemID (string itemID)
      {
        var nodeScope = _webTreeViewNode.Scope.FindTagWithAttribute("ul li", DiagnosticMetadataAttributes.ItemID, itemID);
        return CreateWebTreeViewNodeControlObject(nodeScope);
      }

      public WebTreeViewNodeControlObject WithIndex (int oneBasedIndex)
      {
        var foundNodes = _webTreeViewNode.Scope.FindTagsWithAttribute("ul li", DiagnosticMetadataAttributes.IndexInCollection, oneBasedIndex.ToString()).ToArray();
        if (foundNodes.Length > 1)
          throw AssertionExceptionUtility.CreateExpectationException(_webTreeViewNode.Driver, $"Multiple nodes with the index '{oneBasedIndex}' were found.");

        if (foundNodes.Length == 0)
          throw AssertionExceptionUtility.CreateExpectationException(_webTreeViewNode.Driver, $"No node with the index '{oneBasedIndex}' was found.");

        var nodeScope = foundNodes.Single();

        return CreateWebTreeViewNodeControlObject(nodeScope);
      }

      public WebTreeViewNodeControlObject WithDisplayText (string displayText)
      {
        var nodeScope = _webTreeViewNode.Scope.FindTagWithAttribute("ul li", DiagnosticMetadataAttributes.Content, displayText);
        return CreateWebTreeViewNodeControlObject(nodeScope);
      }

      public WebTreeViewNodeControlObject WithDisplayTextContains (string containsDisplayText)
      {
        var nodeScope = _webTreeViewNode.Scope.FindTagWithAttributeUsingOperator(
            "ul li",
            CssComparisonOperator.SubstringMatch,
            DiagnosticMetadataAttributes.Content,
            containsDisplayText);

        return CreateWebTreeViewNodeControlObject(nodeScope);
      }

      private WebTreeViewNodeControlObject CreateWebTreeViewNodeControlObject (ElementScope nodeScope)
      {
        try
        {
          return new WebTreeViewNodeControlObject(_webTreeViewNode.Context.CloneForControl(nodeScope));
        }
        catch (StaleElementException ex)
        {
          throw AssertionExceptionUtility.CreateControlMissingException(_webTreeViewNode.Driver, ex.Message);
        }
      }
    }

    public WebTreeViewNodeControlObject ([NotNull] ControlObjectContext context)
        : base(context)
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
        throw AssertionExceptionUtility.CreateExpectationException(Driver, "TreeViewNode is not evaluated.");

      return int.Parse(numberChildren);
    }

    /// <summary>
    /// Returns the 
    /// </summary>
    public string GetBadgeText ()
    {
      return Scope[DiagnosticMetadataAttributes.WebTreeViewBadgeValue] ?? string.Empty;
    }

    /// <summary>
    /// Returns the number of child nodes.
    /// </summary>
    public string GetBadgeDescription ()
    {
      return Scope[DiagnosticMetadataAttributes.WebTreeViewBadgeDescription] ?? string.Empty;
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithNodes<WebTreeViewNodeControlObject> GetNode ()
    {
      return new GetNodeImplementationForChildren(this);
    }

    /// <inheritdoc/>
    public WebTreeViewNodeControlObject GetNode (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("itemID", itemID);

      return GetNode().WithItemID(itemID);
    }

    /// <inheritdoc/>
    public WebTreeViewNodeControlObject GetNode (int oneBasedIndex)
    {
      return GetNode().WithIndex(oneBasedIndex);
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithNodes<WebTreeViewNodeControlObject> GetNodeInHierarchy ()
    {
      return new GetNodeImplementationForHierarchy(this);
    }

    /// <inheritdoc/>
    public WebTreeViewNodeControlObject GetNodeInHierarchy (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("itemID", itemID);

      return GetNodeInHierarchy().WithItemID(itemID);
    }

    /// <inheritdoc/>
    public WebTreeViewNodeControlObject GetNodeInHierarchy (int oneBasedIndex)
    {
      return GetNodeInHierarchy().WithIndex(oneBasedIndex);
    }

    /// <summary>
    /// Returns <see langword="true" /> if the node has been evaluated, otherwise <see langword="false" />.
    /// </summary>
    public bool IsEvaluated ()
    {
      int nodeCount;
      return int.TryParse(Scope[DiagnosticMetadataAttributes.WebTreeViewNumberOfChildren], out nodeCount);
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

      return int.Parse(numberOfChildren) != 0;
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
        throw AssertionExceptionUtility.CreateExpectationException(Driver, "The WebTreeViewNode cannot be expanded as it has no children.");
      if (IsExpanded())
        throw AssertionExceptionUtility.CreateExpectationException(Driver, "TreeViewNode is already expanded.");

      ToggleExpansion();

      return this;
    }

    /// <summary>
    /// Collapses the node.
    /// </summary>
    public WebTreeViewNodeControlObject Collapse ()
    {
      if (!IsExpandable())
        throw AssertionExceptionUtility.CreateExpectationException(Driver, "The WebTreeViewNode cannot be collapsed as it has no children.");
      if (!IsExpanded())
        throw AssertionExceptionUtility.CreateExpectationException(Driver, "TreeViewNode is already collapsed.");

      ToggleExpansion();

      return this;
    }

    /// <summary>
    /// Selects the node by clicking on it, returns the node.
    /// </summary>
    public WebTreeViewNodeControlObject Select ([CanBeNull] IWebTestActionOptions? actionOptions = null)
    {
      ClickNode(actionOptions);
      return this;
    }

    /// <summary>
    /// Selects the node by clicking on it, returns the following page.
    /// </summary>
    public UnspecifiedPageObject Click ([CanBeNull] IWebTestActionOptions? actionOptions = null)
    {
      ClickNode(actionOptions);
      return UnspecifiedPage();
    }

    private void ClickNode (IWebTestActionOptions? actionOptions)
    {
      var selectAnchorScope = Scope.FindCss("span > span > a");

      var actualCompletionDetector = MergeWithDefaultActionOptions(selectAnchorScope, actionOptions);
      ExecuteAction(new ClickAction(this, selectAnchorScope, Logger), actualCompletionDetector);
    }

    public ContextMenuControlObject GetContextMenu ()
    {
      var contextMenuScope = Scope.FindXPath("./span/span");

      return new ContextMenuControlObject(Context.CloneForControl(contextMenuScope));
    }

    /// <summary>
    /// Returns the category of a node
    /// </summary>
    public string GetCategory ()
    {
      return Scope[DiagnosticMetadataAttributes.WebTreeViewNodeCategory] ?? string.Empty;
    }

    private void ToggleExpansion ()
    {
      var toggleAnchor = Scope.FindCss("span > a");

      var actionOptions = MergeWithDefaultActionOptions(toggleAnchor, null);
      ExecuteAction(new ClickAction(this, toggleAnchor, Logger), actionOptions);
    }
  }
}
