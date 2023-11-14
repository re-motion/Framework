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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Obsolete;
using Remotion.Web.UI.Controls.PostBackTargets;

namespace Remotion.Web.Development.WebTesting.TestSite.Shared.MultiWindowTest
{
  public partial class Main : MultiWindowTestPageBase
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      LoadFrameFunctionInFrame.Click += LoadFrameFunctionInFrameOnClick;
      LoadFrameFunctionAsSubInFrame.Click += LoadFrameFunctionAsSubInFrameOnClick;
      LoadWindowFunctionInFrame.Click += LoadWindowFunctionInFrameOnClick;
      LoadMainAutoRefreshingFrameFunctionInFrame.Click += LoadMainAutoRefreshingFrameFunctionInFrameOnClick;
      LoadWindowFunctionInNewWindow.Click += LoadWindowFunctionInNewWindowOnClick;

      NavigateAway.NavigateUrl = this.ResolveRootResource("Default.aspx");
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender(e);
      SetTestOutput(MainLabel);
    }

    protected override void AddPostBackEventHandlerToPage (PostBackEventHandler postBackEventHandler)
    {
      UpdatePanel.ContentTemplateContainer.Controls.Add(postBackEventHandler);
    }

    private void LoadFrameFunctionInFrameOnClick (object sender, EventArgs e)
    {
      var function = new FrameFunction(false);
      LoadFunctionInFrame(function);
    }

    private void LoadFrameFunctionAsSubInFrameOnClick (object sender, EventArgs e)
    {
      var function = new FrameFunction(false);
      LoadFunctionInFrame(function, true);
    }

    private void LoadWindowFunctionInFrameOnClick (object sender, EventArgs e)
    {
      var function = new WindowFunction();
      LoadFunctionInFrame(function);
    }

    private void LoadFunctionInFrame (WxeFunction function, bool asSub = false)
    {
      var variableKey = "WxeFunctionToOpen_" + Guid.NewGuid();
      Variables[variableKey] = function;
      ExecuteCommandOnClient_InFrame("frame", ExecuteFunctionCommand, true, CurrentFunction.FunctionToken, variableKey, asSub.ToString());
    }

    private void LoadMainAutoRefreshingFrameFunctionInFrameOnClick (object sender, EventArgs e)
    {
      var function = new FrameFunction(true);
      LoadFunctionInFrame(function);
    }

    private void LoadWindowFunctionInNewWindowOnClick (object sender, EventArgs eventArgs)
    {
      if (IsReturningPostBack)
        return;

      this.ExecuteFunctionExternal(new WindowFunction(), "_blank", WindowOpenFeatures, (Control)sender, true, false, false);
    }
  }
}
