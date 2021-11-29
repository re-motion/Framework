﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.WebFormsControlObjects
{
  /// <summary>
  /// Control object for <see cref="T:System.Web.UI.WebControls.TreeView"/>.
  /// </summary>
  public class TreeViewControlObject
      : WebFormsControlObject,
          IControlObjectWithNodes<TreeViewNodeControlObject>
  {
    private class GetNodeImplementationForChildren : IFluentControlObjectWithNodes<TreeViewNodeControlObject>
    {
      private readonly TreeViewControlObject _treeView;

      public GetNodeImplementationForChildren (TreeViewControlObject treeView)
      {
        _treeView = treeView;
      }

      public TreeViewNodeControlObject WithItemID (string itemID)
      {
        throw new NotSupportedException("The TreeViewControlObject does not support node selection by item ID.");
      }

      public TreeViewNodeControlObject WithIndex (int oneBasedIndex)
      {
        var xpath = string.Format("./table[{0}]", oneBasedIndex);
        return FindAndCreateNode(xpath);
      }

      public TreeViewNodeControlObject WithDisplayText (string displayText)
      {
        var xpath = string.Format("./table[normalize-space(tbody/tr/td[last()])={0}]", DomSelectorUtility.CreateMatchValueForXPath(displayText));
        return FindAndCreateNode(xpath);
      }

      public TreeViewNodeControlObject WithDisplayTextContains (string containsDisplayText)
      {
        var xpath = string.Format("./table[contains(tbody/tr/td[last()], {0})]", DomSelectorUtility.CreateMatchValueForXPath(containsDisplayText));
        return FindAndCreateNode(xpath);
      }

      private TreeViewNodeControlObject FindAndCreateNode (string xpath)
      {
        var nodeScope = _treeView.Scope.FindXPath(xpath);
        return new TreeViewNodeControlObject(_treeView.Context.CloneForControl(nodeScope));
      }
    }

    private class GetNodeImplementationForHierarchy : IFluentControlObjectWithNodes<TreeViewNodeControlObject>
    {
      private readonly TreeViewControlObject _treeView;

      public GetNodeImplementationForHierarchy (TreeViewControlObject treeView)
      {
        _treeView = treeView;
      }

      public TreeViewNodeControlObject WithIndex (int oneBasedIndex)
      {
        var xpath = string.Format(".//table[{0}]", oneBasedIndex);
        var foundNodes = _treeView.Scope.FindAllXPath(xpath).ToArray();

        if (foundNodes.Length > 1)
          throw AssertionExceptionUtility.CreateExpectationException(_treeView.Driver, $"Multiple nodes with the index '{oneBasedIndex}' were found.");

        if (foundNodes.Length == 0)
          throw AssertionExceptionUtility.CreateExpectationException(_treeView.Driver, $"No node with the index '{oneBasedIndex}' was found.");

        var nodeScope = foundNodes.Single();

        return new TreeViewNodeControlObject(_treeView.Context.CloneForControl(nodeScope));
      }

      public TreeViewNodeControlObject WithDisplayText (string displayText)
      {
        var xpath = string.Format("//table[normalize-space(tbody/tr/td[last()])={0}]", DomSelectorUtility.CreateMatchValueForXPath(displayText));
        return FindAndCreateNode(xpath);
      }

      public TreeViewNodeControlObject WithDisplayTextContains (string containsDisplayText)
      {
        var xpath = string.Format("//table[contains(tbody/tr/td[last()], {0})]", DomSelectorUtility.CreateMatchValueForXPath(containsDisplayText));
        return FindAndCreateNode(xpath);
      }

      public TreeViewNodeControlObject WithItemID (string itemID)
      {
        throw new NotSupportedException("The TreeViewControlObject does not support node selection by item ID.");
      }

      private TreeViewNodeControlObject FindAndCreateNode (string xpath)
      {
        var nodeScope = _treeView.Scope.FindXPath(xpath);
        return new TreeViewNodeControlObject(_treeView.Context.CloneForControl(nodeScope));
      }
    }

    public TreeViewControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns the tree's first root node.
    /// </summary>
    [Obsolete ("This method is equivalent to .GetNode().WithIndex (1), which should be used instead. (Version 3.0.0-alpha.14)", false)]
    public TreeViewNodeControlObject GetRootNode ()
    {
      return GetNode().WithIndex(1);
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithNodes<TreeViewNodeControlObject> GetNode ()
    {
      return new GetNodeImplementationForChildren(this);
    }

    /// <inheritdoc/>
    public TreeViewNodeControlObject GetNode (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("itemID", itemID);

      return GetNode().WithItemID(itemID);
    }

    /// <inheritdoc/>
    public TreeViewNodeControlObject GetNode (int oneBasedIndex)
    {
      return GetNode().WithIndex(oneBasedIndex);
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithNodes<TreeViewNodeControlObject> GetNodeInHierarchy ()
    {
      return new GetNodeImplementationForHierarchy(this);
    }

    /// <inheritdoc/>
    public TreeViewNodeControlObject GetNodeInHierarchy (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("itemID", itemID);

      return GetNodeInHierarchy().WithItemID(itemID);
    }

    /// <inheritdoc/>
    public TreeViewNodeControlObject GetNodeInHierarchy (int oneBaseIndex)
    {
      return GetNodeInHierarchy().WithIndex(oneBaseIndex);
    }
  }
}