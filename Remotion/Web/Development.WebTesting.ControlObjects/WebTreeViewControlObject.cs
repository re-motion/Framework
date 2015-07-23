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
          IControlObjectWithNodes<WebTreeViewNodeControlObject>,
          IFluentControlObjectWithNodes<WebTreeViewNodeControlObject>
  {
    private readonly WebTreeViewNodeControlObject _metaRootNode;

    public WebTreeViewControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
      _metaRootNode = new WebTreeViewNodeControlObject (context);
    }

    /// <summary>
    /// Returns the tree's root node.
    /// </summary>
    public WebTreeViewNodeControlObject GetRootNode ()
    {
      return _metaRootNode.GetNode().WithIndex (1);
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
    public WebTreeViewNodeControlObject GetNode (int index)
    {

      return GetNode().WithIndex (index);
    }

    /// <inheritdoc/>
    WebTreeViewNodeControlObject IFluentControlObjectWithNodes<WebTreeViewNodeControlObject>.WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return _metaRootNode.GetNode (itemID);
    }

    /// <inheritdoc/>
    WebTreeViewNodeControlObject IFluentControlObjectWithNodes<WebTreeViewNodeControlObject>.WithIndex (int index)
    {
      return _metaRootNode.GetNode().WithIndex (index);
    }

    /// <inheritdoc/>
    WebTreeViewNodeControlObject IFluentControlObjectWithNodes<WebTreeViewNodeControlObject>.WithDisplayText (string displayText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("displayText", displayText);

      return _metaRootNode.GetNode().WithDisplayText (displayText);
    }

    /// <inheritdoc/>
    WebTreeViewNodeControlObject IFluentControlObjectWithNodes<WebTreeViewNodeControlObject>.WithDisplayTextContains (string containsDisplayText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("containsDisplayText", containsDisplayText);

      return _metaRootNode.GetNode().WithDisplayTextContains (containsDisplayText);
    }
  }
}