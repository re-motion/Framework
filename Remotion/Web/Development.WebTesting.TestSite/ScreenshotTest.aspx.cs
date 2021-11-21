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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class ScreenshotTest : WxePage
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      var node = new WebTreeNode ("ItemA", WebString.CreateFromText ("A"), "~/Images/SampleIcon.gif") { IsEvaluated = true, IsExpanded = true };
      node.Children.Add (new WebTreeNode ("ItemA1", WebString.CreateFromText ("A1")));
      node.Children.Add (new WebTreeNode ("ItemA2", WebString.CreateFromText ("A2")));
      node.Children.Add (new WebTreeNode ("ItemA3", WebString.CreateFromText ("A3")));
      node.Children.Add (new WebTreeNode ("ItemA4", WebString.CreateFromText ("A4")));

      MyWebTreeView.Nodes.Add (node);
    }
  }
}