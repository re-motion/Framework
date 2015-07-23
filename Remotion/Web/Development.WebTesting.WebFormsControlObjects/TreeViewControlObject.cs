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
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.Web.Development.WebTesting.WebFormsControlObjects
{
  /// <summary>
  /// Control object for <see cref="T:System.Web.UI.WebControls.TreeView"/>.
  /// </summary>
  public class TreeViewControlObject
      : WebFormsControlObject,
          IControlObjectWithNodes<TreeViewNodeControlObject>,
          IFluentControlObjectWithNodes<TreeViewNodeControlObject>
  {
    public TreeViewControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns the tree's root node.
    /// </summary>
    public TreeViewNodeControlObject GetRootNode ()
    {
      return GetNode().WithIndex (1);
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

      throw new NotSupportedException ("The TreeViewControlObject does not support node selection by item ID.");
    }

    /// <inheritdoc/>
    TreeViewNodeControlObject IFluentControlObjectWithNodes<TreeViewNodeControlObject>.WithIndex (int index)
    {
      var xpath = string.Format ("./table[{0}]", index);
      return FindAndCreateNode (xpath);
    }

    /// <inheritdoc/>
    TreeViewNodeControlObject IFluentControlObjectWithNodes<TreeViewNodeControlObject>.WithDisplayText (string displayText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("displayText", displayText);

      var xpath = string.Format ("./table[normalize-space(tbody/tr/td[last()])='{0}']", displayText);
      return FindAndCreateNode (xpath);
    }

    /// <inheritdoc/>
    TreeViewNodeControlObject IFluentControlObjectWithNodes<TreeViewNodeControlObject>.WithDisplayTextContains (string containsDisplayText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("containsDisplayText", containsDisplayText);

      var xpath = string.Format ("./table[contains(tbody/tr/td[last()], '{0}')]", containsDisplayText);
      return FindAndCreateNode (xpath);
    }

    private TreeViewNodeControlObject FindAndCreateNode (string xpath)
    {
      var nodeScope = Scope.FindXPath (xpath);
      return new TreeViewNodeControlObject (Context.CloneForControl (nodeScope));
    }
  }
}