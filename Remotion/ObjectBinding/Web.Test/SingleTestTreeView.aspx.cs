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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace OBWTest
{
  public class SingleTestTreeView : SingleBocTestBasePage
  {
    protected Label TreeViewLabel;
    protected Button PostBackButton;
    protected FormGridManager FormGridManager;
    protected BindableObjectDataSourceControl CurrentObject;
    protected WebTreeView WebTreeView;
    protected PersonTreeView PersonTreeView;
    protected PersonTreeView PersonTreeViewWithMenus;
    protected Button RefreshPesonTreeViewButton;
    protected Button Node332Button;
    protected HtmlHeadContents HtmlHeadContents;

    public SingleTestTreeView ()
    {
    }

    protected override void OnPreInit (EventArgs e)
    {
      MasterPageFile = Global.PreferQuirksModeRendering ? "~/QuirksMode.Master" : "~/StandardMode.Master";
      base.OnPreInit (e);
    }

    private void Page_Load (object sender, EventArgs e)
    {
      Guid personID = new Guid (0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
      Person person = Person.GetObject (personID);

      CurrentObject.BusinessObject = (IBusinessObject) person;


      DataBind();

      CurrentObject.LoadValues (IsPostBack);
      BocTreeNode node = PersonTreeView.SelectedNode;
    }

    protected override void OnPreRender (EventArgs e)
    {
      BocTreeNode node = PersonTreeView.SelectedNode;
      base.OnPreRender (e);
    }

    protected override void OnInit (EventArgs e)
    {
      InitializeComponent();
      base.OnInit (e);

      WebTreeNodeCollection nodes;

      // Use '|' in the tree node IDs to simulate serialized ObjectIDs and detect conflicts.

      nodes = WebTreeView.Nodes;
      nodes.Add (
          new WebTreeNode ("node0|id", "Node 0", "Hello", new IconInfo ("Images/Remotion.ObjectBinding.Sample.Job.gif", "Icon", "ToolTip", Unit.Pixel (16), Unit.Pixel (16))));
      nodes.Add (new WebTreeNode ("node1|id", "Node 1"));
      nodes.Add (new WebTreeNode ("node2|id", "Node 2"));
      nodes.Add (new WebTreeNode ("node3|id", "Node 3"));
      nodes.Add (new WebTreeNode ("node4|id", "Node 4"));

      nodes = ((WebTreeNode) WebTreeView.Nodes[0]).Children;
      nodes.Add (new WebTreeNode ("node00|id", "Node 0-0", "Images/Remotion.ObjectBinding.Sample.Job.gif"));
      nodes.Add (new WebTreeNode ("node01|id", "Node 0-1"));
      nodes.Add (new WebTreeNode ("node02|id", "Node 0-2"));
      nodes.Add (new WebTreeNode ("node03|id", "Node 0-3"));
      ((WebTreeNode) WebTreeView.Nodes[0]).IsEvaluated = true;

      nodes = ((WebTreeNode) ((WebTreeNode) WebTreeView.Nodes[0]).Children[0]).Children;
      nodes.Add (new WebTreeNode ("node000|id", "Node 0-0-0"));
      nodes.Add (new WebTreeNode ("node001|id", "Node 0-0-1", "Hello", new IconInfo ("Images/Remotion.ObjectBinding.Sample.Job.gif")));
      nodes.Add (
          new WebTreeNode ("node002|id", "Node 0-0-2", "Hello", new IconInfo ("Images/Remotion.ObjectBinding.Sample.Job.gif", "Icon", null, Unit.Pixel (16), Unit.Pixel (16))));
      nodes.Add (new WebTreeNode ("node003|id", "Node 0-0-3", "Images/Remotion.ObjectBinding.Sample.Job.gif"));
      ((WebTreeNode) ((WebTreeNode) WebTreeView.Nodes[0]).Children[0]).IsEvaluated = true;

      nodes = ((WebTreeNode) WebTreeView.Nodes[3]).Children;
      nodes.Add (new WebTreeNode ("node30|id", "Node 3-0"));
      nodes.Add (new WebTreeNode ("node31|id", "Node 3-1"));
      nodes.Add (new WebTreeNode ("node32|id", "Node 3-2"));
      nodes.Add (new WebTreeNode ("node33|id", "Node 3-3"));
      ((WebTreeNode) WebTreeView.Nodes[3]).IsEvaluated = true;

      nodes = ((WebTreeNode) ((WebTreeNode) WebTreeView.Nodes[3]).Children[3]).Children;
      nodes.Add (new WebTreeNode ("node330|id", "Node 3-3-0"));
      nodes.Add (new WebTreeNode ("node331|id", "Node 3-3-1"));
      nodes.Add (new WebTreeNode ("node332|id", "Node 3-3-2"));
      nodes.Add (new WebTreeNode ("node333|id", "Node 3-3-3"));
      ((WebTreeNode) ((WebTreeNode) WebTreeView.Nodes[3]).Children[3]).IsEvaluated = true;

      nodes = ((WebTreeNode) WebTreeView.Nodes[2]).Children;
      nodes.Add (new WebTreeNode ("node20|id", "Node 2-0"));
      nodes.Add (new WebTreeNode ("node21|id", "Node 2-1"));
      nodes.Add (new WebTreeNode ("node22|id", "Node 2-2"));
      nodes.Add (new WebTreeNode ("node23|id", "Node 2-3"));
      ((WebTreeNode) WebTreeView.Nodes[2]).IsEvaluated = true;

      nodes = ((WebTreeNode) WebTreeView.Nodes[4]).Children;
      nodes.Add (new WebTreeNode ("node40|id", "Node 4-0"));
      nodes.Add (new WebTreeNode ("node41|id", "Node 4-1"));
      nodes.Add (new WebTreeNode ("node42|id", "Node 4-2"));
      nodes.Add (new WebTreeNode ("node43|id", "Node 4-3"));
      ((WebTreeNode) WebTreeView.Nodes[4]).IsEvaluated = true;

      var currentNodes = WebTreeView.Nodes;
      for (int i = 0; i < 55; i++)
      {
        WebTreeNode node = new WebTreeNode ("nodeNestingLevel" + i, "Node with nesting level " + i);
        node.IsEvaluated = true;
        node.IsExpanded = true;
        currentNodes.Add (node);
        currentNodes = node.Children;
      }
      ((WebTreeNode) WebTreeView.Nodes[5]).IsExpanded = false;

      WebTreeView.SetEvaluateTreeNodeDelegate (new EvaluateWebTreeNode (EvaluateTreeNode));

      WebTreeView.MenuItemProvider = new TestWebTreeViewMenuItemProvider();
      PersonTreeViewWithMenus.MenuItemProvider = new TestBocTreeViewMenuItemProvider();
    }

    private void EvaluateTreeNode (WebTreeNode node)
    {
      node.IsEvaluated = true;
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      PersonTreeView.Click += new BocTreeNodeClickEventHandler (PersonTreeView_Click);
      PersonTreeView.SelectionChanged += new BocTreeNodeEventHandler (PersonTreeView_SelectionChanged);
      RefreshPesonTreeViewButton.Click += new EventHandler (RefreshPesonTreeViewButton_Click);
      WebTreeView.Click += new WebTreeNodeClickEventHandler (TreeView_Click);
      Node332Button.Click += new EventHandler (Node332Button_Click);
      Load += new EventHandler (Page_Load);
    }

    private void TreeView_Click (object sender, WebTreeNodeClickEventArgs e)
    {
      TreeViewLabel.Text = "Node = " + e.Node.Text;
    }

    private void PersonTreeView_Click (object sender, BocTreeNodeClickEventArgs e)
    {
      TreeViewLabel.Text = "Node = " + e.Node.Text;
    }

    private void RefreshPesonTreeViewButton_Click (object sender, EventArgs e)
    {
      PersonTreeView.RefreshTreeNodes();
    }

    private void PersonTreeView_SelectionChanged (object sender, BocTreeNodeEventArgs e)
    {
    }

    private void Node332Button_Click (object sender, EventArgs e)
    {
      WebTreeNode node3 = (WebTreeNode) WebTreeView.Nodes[3];
      node3.EvaluateExpand();
      WebTreeNode node33 = (WebTreeNode) node3.Children[3];
      node33.EvaluateExpand();
      WebTreeNode node332 = (WebTreeNode) node33.Children[2];
      node332.EvaluateExpand();
      node332.IsSelected = true;
    }
  }
}
