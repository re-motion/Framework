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

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.Web.UI.Controls.WebTreeView"/>.
  /// </summary>
  public class WebTreeViewControlObject
      : WebFormsControlObjectWithDiagnosticMetadata,
          IControlObjectWithNodes<WebTreeViewNodeControlObject>
  {
    private readonly WebTreeViewNodeControlObject _metaRootNode;

    public WebTreeViewControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
      _metaRootNode = new WebTreeViewNodeControlObject(context);
    }

    /// <summary>
    /// Returns the tree's first root node.
    /// </summary>
    [Obsolete ("This method is equivalent to .GetNode().WithIndex (1), which should be used instead. (Version 3.0.0-alpha.14)", false)]
    public WebTreeViewNodeControlObject GetRootNode ()
    {
      return _metaRootNode.GetNode().WithIndex(1);
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithNodes<WebTreeViewNodeControlObject> GetNode ()
    {
      return _metaRootNode.GetNode();
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
      return _metaRootNode.GetNodeInHierarchy();
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
  }
}
