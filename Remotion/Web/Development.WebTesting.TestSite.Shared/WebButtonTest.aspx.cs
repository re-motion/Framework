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
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Development.WebTesting.TestSite.Shared
{
  public partial class WebButtonTest : WxePage
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      MyWebButtonWithIcon.Icon.Url = this.ResolveImageResource("SampleIcon.gif");

      MyWebButton1Sync.Command += Command;
      MyWebButtonPrimary1Sync.Command += Command;
      MyWebButtonSupplemental1Sync.Command += Command;
      MyWebButton2Async.Command += Command;
      MyWebButtonPrimary2Async.Command += Command;
      MyWebButtonSupplemental2Async.Command += Command;
    }

    private void Command (object sender, CommandEventArgs e)
    {
      ((Layout)Master).SetTestOutput(e.CommandName);
    }
  }
}
