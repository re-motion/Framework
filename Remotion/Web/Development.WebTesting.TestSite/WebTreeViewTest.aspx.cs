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
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class WebTreeViewTest : SmartPage
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      var node = new WebTreeNode (
          "Item1",
          WebString.CreateFromText ("This is the beginning of very long text that should be placed correctly beside the icon"),
          "~/Images/SampleIcon.gif");
      MyWebTreeView2.Nodes.Add (node);

      var webTreeNodeNoBadge = new WebTreeNode ("Node1", WebString.CreateFromText ("No badge"));
      MyWebTreeView3.Nodes.Add (webTreeNodeNoBadge);

      var webTreeNodeBadgeNoDescription = new WebTreeNode ("Node2", WebString.CreateFromText ("Badge with value"));
      webTreeNodeBadgeNoDescription.Badge = new Badge (WebString.CreateFromText ("1"), WebString.Empty);
      MyWebTreeView3.Nodes.Add (webTreeNodeBadgeNoDescription);

      var webTreeNodeBadge = new WebTreeNode ("Node3", WebString.CreateFromText ("Badge with value and description"));
      webTreeNodeBadge.Badge = new Badge (WebString.CreateFromText ("2"), WebString.CreateFromText ("2 description"));
      MyWebTreeView3.Nodes.Add (webTreeNodeBadge);

      var treeViewNodes = new IControlItem[]
                          {
                              new WebTreeNode ("Node1", WebString.CreateFromText ("1")) { Category = "first category" },
                              new WebTreeNode ("Node2", WebString.CreateFromText ("2")) { Category = "second category" },
                              new WebTreeNode ("Node3", WebString.CreateFromText ("3")) { Category = "third category" },
                              new WebTreeNode ("Node4", WebString.CreateFromText ("4")) { Category = "second category" },
                              new WebTreeNode ("Node5", WebString.CreateFromText ("5")) { Category = "third category" },
                              new WebTreeNode ("Node6", WebString.CreateFromText ("6")) { Category = "third category" },
                              new WebTreeNode ("Node7", WebString.CreateFromText ("7")) { Category = "first category" }
                          };

      MyOrderedWebTreeView.Nodes.AddRange (treeViewNodes);
      MyUnorderedWebTreeView.Nodes.AddRange (treeViewNodes);
      MyWebTreeViewWithCategories.Nodes.Add (new WebTreeNode ("Node1", WebString.CreateFromText ("1")) { Category = "a category" });
      MyWebTreeViewWithoutCategories.Nodes.Add (new WebTreeNode ("Node1", WebString.CreateFromText ("1")));
    }
  }
}