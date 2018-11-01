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
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocTreeViewUserControl : DataEditUserControl
  {
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      Normal.MenuItemProvider = new TestBocTreeViewContextMenu();
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      SetTestOutput();
    }

    private void SetTestOutput ()
    {
      TestOutput.SetNormalSelectedNodeLabel (Normal.SelectedNode != null ? Normal.SelectedNode.ItemID + "|" + Normal.SelectedNode.Text : "");
      TestOutput.SetNoTopLevelExpanderSelectedNodeLabel (
          NoTopLevelExpander.SelectedNode != null ? NoTopLevelExpander.SelectedNode.ItemID + "|" + NoTopLevelExpander.SelectedNode.Text : "");
      TestOutput.SetNoLookAheadEvaluationSelectedNodeLabel (
          NoLookAheadEvaluation.SelectedNode != null
              ? NoLookAheadEvaluation.SelectedNode.ItemID + "|" + NoLookAheadEvaluation.SelectedNode.Text
              : "");
      TestOutput.SetNoPropertyIdentifierSelectedNodeLabel (
          NoPropertyIdentifier.SelectedNode != null ? NoPropertyIdentifier.SelectedNode.ItemID + "|" + NoPropertyIdentifier.SelectedNode.Text : "");
    }

    private BocTreeViewUserControlTestOutput TestOutput
    {
      get { return (BocTreeViewUserControlTestOutput) ((Layout) Page.Master).GetTestOutputControl(); }
    }
  }
}