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
using System.ComponentModel;
using System.Drawing.Design;
using Remotion.Utilities;
using Remotion.Web.UI.Design;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{
  /// <summary> A collection of <see cref="WebTreeNode"/> objects. </summary>
  [Editor (typeof (WebTreeNodeCollectionEditor), typeof (UITypeEditor))]
  public class WebTreeNodeCollection : ControlItemCollection
  {
    private WebTreeView _treeView;
    private WebTreeNode _parentNode;

    /// <summary> Initializes a new instance. </summary>
    public WebTreeNodeCollection (IControl ownerControl, Type[] supportedTypes)
        : base (ownerControl, supportedTypes)
    {
    }

    /// <summary> Initializes a new instance. </summary>
    public WebTreeNodeCollection (IControl ownerControl)
        : this (ownerControl, new[] { typeof (WebTreeNode) })
    {
    }

    //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
    protected internal new WebTreeNode this [int index]
    {
      get { return (WebTreeNode) List[index]; }
      set { List[index] = value; }
    }

    protected override void ValidateNewValue (object value)
    {
      WebTreeNode node = ArgumentUtility.CheckNotNullAndType<WebTreeNode> ("value", value);

      EnsureDesignModeTreeNodeInitialized (node);
      if (string.IsNullOrEmpty (node.ItemID))
        throw new ArgumentException ("The node does not contain an 'ItemID' and can therfor not be inserted into the collection.", "value");

      base.ValidateNewValue (value);
    }

    private void EnsureDesignModeTreeNodeInitialized (WebTreeNode node)
    {
      ArgumentUtility.CheckNotNull ("node", node);
      if (string.IsNullOrEmpty (node.ItemID)
          && _treeView != null && ControlHelper.IsDesignMode (_treeView))
      {
        int index = InnerList.Count;
        do
        {
          index++;
          string itemID = "Node" + index;
          if (Find (itemID) == null)
          {
            node.ItemID = itemID;
            if (string.IsNullOrEmpty (node.Text))
              node.Text = "Node " + index;
            break;
          }
        } while (true);
      }
    }

    protected override void OnInsertComplete (int index, object value)
    {
      WebTreeNode node = ArgumentUtility.CheckNotNullAndType<WebTreeNode> ("value", value);

      base.OnInsertComplete (index, value);
      node.SetParent (_treeView, _parentNode);
    }

    protected override void OnSetComplete (int index, object oldValue, object newValue)
    {
      WebTreeNode node = ArgumentUtility.CheckNotNullAndType<WebTreeNode> ("newValue", newValue);

      base.OnSetComplete (index, oldValue, newValue);
      node.SetParent (_treeView, _parentNode);
    }

    protected internal void SetParent (WebTreeView treeView, WebTreeNode parentNode)
    {
      _treeView = treeView;
      _parentNode = parentNode;
      for (int i = 0; i < InnerList.Count; i++)
      {
        WebTreeNode node = (WebTreeNode) InnerList[i];
        node.SetParent (_treeView, parentNode);
      }
    }

    /// <summary>
    ///   Finds the <see cref="WebTreeNode"/> with a <see cref="WebTreeNode.ItemID"/> of <paramref name="id"/>.
    /// </summary>
    /// <param name="id"> The ID to look for. </param>
    /// <returns> A <see cref="WebTreeNode"/> or <see langword="null"/> if no mathcing node was found. </returns>
    public new WebTreeNode Find (string id)
    {
      return (WebTreeNode) base.Find (id);
    }

    //  /// <summary>
    //  ///   Sets the <see cref="WebTreeNode.IsExpanded"/> of all nodes in this collection, including all child nodes.
    //  /// </summary>
    //  /// <param name="expand"> <see langword="true"/> to expand all nodes, <see langword="false"/> to collapse them. </param>
    //  public void SetExpansion (bool expand)
    //  {
    //    for (int i = 0; i < InnerList.Count; i++)
    //    {
    //      WebTreeNode node = (WebTreeNode) InnerList[i];
    //      node.IsExpanded = expand;
    //      if (expand)
    //        node.ExpandAll();
    //      else
    //        node.CollapseAll();
    //    }
    //  }
  }
}
