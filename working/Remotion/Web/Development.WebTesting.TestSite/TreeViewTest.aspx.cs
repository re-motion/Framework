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
using System.Web.UI.WebControls;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class TreeViewTest : WxePage
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      MyTreeView.SelectedNodeChanged += MyTreeViewOnSelectedNodeChanged;
      MyTreeView.TreeNodeCollapsed += MyTreeViewOnTreeNodeCollapsed;
      MyTreeView.TreeNodeExpanded += MyTreeViewOnTreeNodeExpanded;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (!IsPostBack)
        InitializeTreeView();
    }

    private void MyTreeViewOnSelectedNodeChanged (object sender, EventArgs eventArgs)
    {
      ((Layout) Master).SetTestOutput (
          "Selected: " + MyTreeView.SelectedNode.Text + "|" + MyTreeView.SelectedNode.Value + " (" + GetCheckedNodes() + ")");
    }

    private void MyTreeViewOnTreeNodeExpanded (object sender, TreeNodeEventArgs treeNodeEventArgs)
    {
      ((Layout) Master).SetTestOutput (
          "Expanded: " + treeNodeEventArgs.Node.Text + "|" + treeNodeEventArgs.Node.Value + " (" + GetCheckedNodes() + ")");
    }

    private void MyTreeViewOnTreeNodeCollapsed (object sender, TreeNodeEventArgs treeNodeEventArgs)
    {
      ((Layout) Master).SetTestOutput (
          "Collapsed: " + treeNodeEventArgs.Node.Text + "|" + treeNodeEventArgs.Node.Value + " (" + GetCheckedNodes() + ")");
    }

    private string GetCheckedNodes ()
    {
      if (MyTreeView.CheckedNodes.Count == 0)
        return "None";

      return string.Join (",", MyTreeView.CheckedNodes.Cast<TreeNode>().Select (cn => cn.Text + "|" + cn.Value));
    }

    private void InitializeTreeView ()
    {
      var root = new TreeNode ("Root node", "RootValue", "~/Images/SampleIcon.gif");
      var child1 = new TreeNode ("Child node 1", "Child1Value");
      var child2 = new TreeNode ("Child node 2", "Child2Value");
      var child11 = new TreeNode ("Child node 11", "Child11Value");
      var child12 = new TreeNode ("Child node 12", "Child12Value");
      var child21 = new TreeNode ("Child node 21", "Child21Value");
      var child22 = new TreeNode ("Child node 22", "Child22Value");

      child1.ChildNodes.Add (child11);
      child1.ChildNodes.Add (child12);
      child2.ChildNodes.Add (child21);
      child2.ChildNodes.Add (child22);

      root.ChildNodes.Add (child1);
      root.ChildNodes.Add (child2);

      MyTreeView.Nodes.Add (root);

      MyTreeView.CollapseAll();
      MyTreeView.ShowCheckBoxes = TreeNodeTypes.Leaf;
      MyTreeView.ShowLines = true;
    }
  }
}