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
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class WebTreeViewTest : SmartPage
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      var webTreeNodeNoBadge = new WebTreeNode ("Node1", "No badge");
      MyWebTreeView3.Nodes.Add (webTreeNodeNoBadge);

      var webTreeNodeBadgeNoDescription = new WebTreeNode ("Node2", "Badge with value");
      webTreeNodeBadgeNoDescription.Badge = new Badge ("1", string.Empty);
      MyWebTreeView3.Nodes.Add (webTreeNodeBadgeNoDescription);

      var webTreeNodeBadge = new WebTreeNode ("Node3", "Badge with value and description");
      webTreeNodeBadge.Badge = new Badge ("2", "2 description");
      MyWebTreeView3.Nodes.Add (webTreeNodeBadge);
    }
  }
}