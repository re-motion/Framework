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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a node within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocTreeView"/>.
  /// </summary>
  public class BocTreeViewNodeControlObject
      : WebFormsControlObjectWithDiagnosticMetadata,
          IControlObjectWithNodes<BocTreeViewNodeControlObject>,
          IControlObjectWithText
  {
    private class GetNodeImplementationForChildren : IFluentControlObjectWithNodes<BocTreeViewNodeControlObject>
    {
      private readonly IFluentControlObjectWithNodes<WebTreeViewNodeControlObject> _impl;

      public GetNodeImplementationForChildren (WebTreeViewNodeControlObject webTreeViewNode)
      {
        _impl = webTreeViewNode.GetNode();
      }

      public BocTreeViewNodeControlObject WithItemID (string itemID)
      {
        var webTreeViewNode = _impl.WithItemID(itemID);
        return new BocTreeViewNodeControlObject(webTreeViewNode);
      }

      public BocTreeViewNodeControlObject WithIndex (int oneBasedIndex)
      {
        var webTreeViewNode = _impl.WithIndex(oneBasedIndex);
        return new BocTreeViewNodeControlObject(webTreeViewNode);
      }

      public BocTreeViewNodeControlObject WithDisplayText (string displayText)
      {
        var webTreeViewNode = _impl.WithDisplayText(displayText);
        return new BocTreeViewNodeControlObject(webTreeViewNode);
      }

      public BocTreeViewNodeControlObject WithDisplayTextContains (string containsDisplayText)
      {
        var webTreeViewNode = _impl.WithDisplayTextContains(containsDisplayText);
        return new BocTreeViewNodeControlObject(webTreeViewNode);
      }
    }

    private class GetNodeImplementationForHierarchy : IFluentControlObjectWithNodes<BocTreeViewNodeControlObject>
    {
      private readonly IFluentControlObjectWithNodes<WebTreeViewNodeControlObject> _impl;

      public GetNodeImplementationForHierarchy (WebTreeViewNodeControlObject webTreeViewNode)
      {
        _impl = webTreeViewNode.GetNodeInHierarchy();
      }

      public BocTreeViewNodeControlObject WithItemID (string itemID)
      {
        var webTreeViewNode = _impl.WithItemID(itemID);
        return new BocTreeViewNodeControlObject(webTreeViewNode);
      }

      public BocTreeViewNodeControlObject WithIndex (int oneBasedIndex)
      {
        var webTreeViewNode = _impl.WithIndex(oneBasedIndex);
        return new BocTreeViewNodeControlObject(webTreeViewNode);
      }

      public BocTreeViewNodeControlObject WithDisplayText (string displayText)
      {
        var webTreeViewNode = _impl.WithDisplayText(displayText);
        return new BocTreeViewNodeControlObject(webTreeViewNode);
      }

      public BocTreeViewNodeControlObject WithDisplayTextContains (string containsDisplayText)
      {
        var webTreeViewNode = _impl.WithDisplayTextContains(containsDisplayText);
        return new BocTreeViewNodeControlObject(webTreeViewNode);
      }
    }

    private readonly WebTreeViewNodeControlObject _webTreeViewNode;

    [UsedImplicitly]
    public BocTreeViewNodeControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
      _webTreeViewNode = new WebTreeViewNodeControlObject(context);
      ((IControlObjectNotifier) _webTreeViewNode).ActionExecute += OnActionExecute;
    }

    internal BocTreeViewNodeControlObject ([NotNull] WebTreeViewNodeControlObject webTreeViewNode)
        : base(webTreeViewNode.Context)
    {
      _webTreeViewNode = webTreeViewNode;
      ((IControlObjectNotifier) _webTreeViewNode).ActionExecute += OnActionExecute;
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      return _webTreeViewNode.GetText();
    }

    /// <summary>
    /// Returns whether the node is currently selected.
    /// </summary>
    public bool IsSelected ()
    {
      return _webTreeViewNode.IsSelected();
    }

    /// <summary>
    /// Returns the number of child nodes.
    /// </summary>
    public int GetNumberOfChildren ()
    {
      return _webTreeViewNode.GetNumberOfChildren();
    }

    /// <summary>
    /// Returns <see langword="true" /> if the node has been evaluated, otherwise <see langword="false" />.
    /// </summary>
    public bool IsEvaluated ()
    {
      return _webTreeViewNode.IsEvaluated();
    }

    /// <summary>
    /// Returns <see langword="true" /> if the node can be expanded/collapsed - it has at 
    /// least one child or is not evaluated yet, otherwise <see langword="false" />.
    /// </summary>
    public bool IsExpandable ()
    {
      return _webTreeViewNode.IsExpandable();
    }

    /// <summary>
    /// Returns <see langword="true" /> if the node is expanded, otherwise <see langword="false" />.
    /// </summary>
    public bool IsExpanded ()
    {
      return _webTreeViewNode.IsExpanded();
    }

    /// <summary>
    /// Expands the node.
    /// </summary>
    public BocTreeViewNodeControlObject Expand ()
    {
      var webTreeViewNode = _webTreeViewNode.Expand();
      return new BocTreeViewNodeControlObject(webTreeViewNode);
    }

    /// <summary>
    /// Collapses the node.
    /// </summary>
    public BocTreeViewNodeControlObject Collapse ()
    {
      var webTreeViewNode = _webTreeViewNode.Collapse();
      return new BocTreeViewNodeControlObject(webTreeViewNode);
    }

    /// <summary>
    /// Selects the node by clicking on it, returns the node.
    /// </summary>
    public BocTreeViewNodeControlObject Select ([CanBeNull] IWebTestActionOptions? actionOptions = null)
    {
      var webTreeViewNode = _webTreeViewNode.Select(actionOptions);
      return new BocTreeViewNodeControlObject(webTreeViewNode);
    }

    /// <summary>
    /// Selects the node by clicking on it, returns the following page.
    /// </summary>
    public UnspecifiedPageObject Click ([CanBeNull] IWebTestActionOptions? actionOptions = null)
    {
      return _webTreeViewNode.Click(actionOptions);
    }

    /// <summary>
    /// Opens the node's context menu.
    /// </summary>
    public ContextMenuControlObject GetContextMenu ()
    {
      return _webTreeViewNode.GetContextMenu();
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithNodes<BocTreeViewNodeControlObject> GetNode ()
    {
      return new GetNodeImplementationForChildren(_webTreeViewNode);
    }

    /// <inheritdoc/>
    public BocTreeViewNodeControlObject GetNode (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("itemID", itemID);

      return GetNode().WithItemID(itemID);
    }

    /// <inheritdoc/>
    public BocTreeViewNodeControlObject GetNode (int oneBasedIndex)
    {
      return GetNode().WithIndex(oneBasedIndex);
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithNodes<BocTreeViewNodeControlObject> GetNodeInHierarchy ()
    {
      return new GetNodeImplementationForHierarchy(_webTreeViewNode);
    }

    /// <inheritdoc/>
    public BocTreeViewNodeControlObject GetNodeInHierarchy (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("itemID", itemID);

      return GetNodeInHierarchy().WithItemID(itemID);
    }

    /// <inheritdoc/>
    public BocTreeViewNodeControlObject GetNodeInHierarchy (int oneBasedIndex)
    {
      return GetNodeInHierarchy().WithIndex(oneBasedIndex);
    }
  }
}
