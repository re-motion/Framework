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
using System.Web.UI;
using Remotion.Web.ExecutionEngine.Obsolete;
using Remotion.Web.UI.Controls.PostBackTargets;

namespace Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest
{
  public partial class Frame : MultiWindowTestPageBase
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      SimplePostBack.Click += SimplePostBackOnClick;
      NextStep.Click += NextStepOnClick;
      LoadWindowFunctionInNewWindow.Click += LoadWindowFunctionInNewWindowOnClick;
      RefreshMainUpdatePanel.Click += RefreshMainUpdatePanelOnClick;

      RegisterControlForDirtyStateTracking (_MyTextBox);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      SetTestOutput (FrameLabel);
    }

    protected override void AddPostBackEventHandlerToPage (PostBackEventHandler postBackEventHandler)
    {
      UpdatePanel.TemplateControl.Controls.Add (postBackEventHandler);
    }

    private void SimplePostBackOnClick (object sender, EventArgs eventArgs)
    {
      var frameFunction = ((FrameFunction) CurrentFunction);
      if (frameFunction.AlwaysRefreshMain)
        RefreshMainFrame();
    }

    private void NextStepOnClick (object sender, EventArgs eventArgs)
    {
      ExecuteNextStep();
    }

    private void LoadWindowFunctionInNewWindowOnClick (object sender, EventArgs eventArgs)
    {
      if (!IsReturningPostBack)
        this.ExecuteFunctionExternal (new WindowFunction(), "_blank", WindowOpenFeatures, (Control) sender, true, false, false);
      else if (ReturningFunction.Variables["Refresh"] != null)
        RefreshMainFrame();
    }

    private void RefreshMainUpdatePanelOnClick (object sender, EventArgs eventArgs)
    {
      RefreshMainFrame();
    }

    private void RefreshMainFrame ()
    {
      ExecuteCommandOnClient_InParent (RefreshCommand, false);
    }
  }
}