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
using Remotion.Web.UI.Controls.PostBackTargets;

namespace Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest
{
  public partial class Window : MultiWindowTestPageBase
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      Close.Click += CloseOnClick;
      CloseAndRefreshMainAsWell.Click += CloseAndRefreshMainAsWellOnClick;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      SetTestOutput (WindowLabel);
    }

    protected override void AddPostBackEventHandlerToPage (PostBackEventHandler postBackEventHandler)
    {
      Controls.Add (postBackEventHandler);
    }

    private void CloseOnClick (object sender, EventArgs eventArgs)
    {
      ExecuteNextStep();
    }

    private void CloseAndRefreshMainAsWellOnClick (object sender, EventArgs eventArgs)
    {
      ExecuteNextStep();
      Variables["Refresh"] = true;
    }
  }
}