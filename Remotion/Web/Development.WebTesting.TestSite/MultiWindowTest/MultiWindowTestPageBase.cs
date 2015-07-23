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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls.PostBackTargets;

namespace Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest
{
  public abstract class MultiWindowTestPageBase : WxePage
  {
    private const string c_executeSyncCommandClientFunctionName = "ExecuteSyncCommand";
    private const string c_executeAsyncCommandClientFunctionName = "ExecuteAsyncCommand";

    protected const string ExecuteFunctionCommand = "ExecuteFunction";
    protected const string RefreshCommand = "Refresh";

    protected const string WindowOpenFeatures =
        "left=100,top=100,width=1000,height=500,resizable=yes,location=no,menubar=no,status=no,toolbar=no,scrollbars=no";

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      var asyncPostBackEventHandlerId = AddAsyncPostBackEventHandlerToPage();
      RegisterExecuteCommandClientFunction (c_executeAsyncCommandClientFunctionName, asyncPostBackEventHandlerId);

      var syncPostBackEventHandlerId = AddSyncPostBackEventHandlerToPage();
      RegisterExecuteCommandClientFunction (c_executeSyncCommandClientFunctionName, syncPostBackEventHandlerId);
    }

    protected abstract void AddPostBackEventHandlerToPage (PostBackEventHandler postBackEventHandler);

    protected void SetTestOutput (Label label)
    {
      label.Text = string.Format (
          "<b>{0}</b>: {1} | {2} | {3} | {4}<br/>Parent: {5} | {6} | {7}<br/>HasReturned: {8}",
          label.ID,
          CurrentFunction.GetType().Name,
          WxeContext.Current.PostBackID,
          CurrentFunction.FunctionToken,
          CurrentPageStep.Page,
          CurrentFunction.ParentFunction != null ? CurrentFunction.ParentFunction.GetType().Name : "&lt;none&gt;",
          CurrentFunction.ParentFunction != null ? CurrentFunction.ParentFunction.FunctionToken : "&lt;none&gt;",
          CurrentFunction.ParentFunction != null ? ((WxePageStep) CurrentFunction.ParentStep).Page : "&lt;none&gt;",
          IsReturningPostBack);
    }

    private string AddAsyncPostBackEventHandlerToPage ()
    {
      var asyncPostBackEventHandler = new PostBackEventHandler { ID = "AsyncPostBackTarget" };
      AddPostBackEventHandlerToPage (asyncPostBackEventHandler);

      var sm = ScriptManager.GetCurrent (Page);
      if (sm != null)
        sm.RegisterAsyncPostBackControl (asyncPostBackEventHandler);

      asyncPostBackEventHandler.PostBack += HandlePostBack;

      return asyncPostBackEventHandler.UniqueID;
    }

    private string AddSyncPostBackEventHandlerToPage ()
    {
      var syncPostBackEventHandler = new PostBackEventHandler { ID = "SyncPostBackTarget" };
      AddPostBackEventHandlerToPage (syncPostBackEventHandler);

      syncPostBackEventHandler.PostBack += HandlePostBack;

      return syncPostBackEventHandler.UniqueID;
    }

    protected void ExecuteCommandOnClient_InParent (string command, bool useSyncPostBack, params string[] arguments)
    {
      const string fullQualification = "window.parent.";
      ExecuteCommandOnClient (fullQualification, useSyncPostBack, command, arguments);
    }

    protected void ExecuteCommandOnClient_InFrame (string frameName, string command, bool useSyncPostBack, params string[] arguments)
    {
      var fullQualification = string.Format ("window.frames.{0}.", frameName);
      ExecuteCommandOnClient (fullQualification, useSyncPostBack, command, arguments);
    }

    private void ExecuteCommandOnClient (string fullQualification, bool useSyncPostBack, string command, string[] arguments)
    {
      var functionName = fullQualification + (useSyncPostBack ? c_executeSyncCommandClientFunctionName : c_executeAsyncCommandClientFunctionName);
      var concatenatedArguments = String.Join (":", arguments);
      var script = String.Format ("if({0}) {0}(\"{1}\",\"{2}\");", functionName, command, concatenatedArguments);
      ClientScript.RegisterStartupScriptBlock (this, GetType(), "CallExecuteCommandScript", script);
    }

    private void HandlePostBack (object sender, PostBackEventHandlerEventArgs postBackEventHandlerEventArgs)
    {
      var eventArgument = postBackEventHandlerEventArgs.EventArgument.Split (':');
      var command = eventArgument[0];
      var arguments = eventArgument.Skip (1).ToArray();

      switch (command)
      {
        case ExecuteFunctionCommand:
          var sourceFunctionToken = arguments[0];
          var variablesKey = arguments[1];
          var shouldExecuteAsSubFunction = bool.Parse (arguments[2]);

          var sourceFunction = WxeFunctionStateManager.Current.GetItem (sourceFunctionToken).Function.ExecutingStep.ParentFunction;
          var sourceFunctionVariables = sourceFunction.Variables;
          var function = sourceFunctionVariables[variablesKey] as WxeFunction;
          sourceFunctionVariables.Remove (variablesKey);
          ExecuteWxeFunction (function, sender, shouldExecuteAsSubFunction);
          break;

        case RefreshCommand:
          // All UpdatePanels are set to UpdateMode=Always, nothing to do.
          break;
      }
    }

    private void ExecuteWxeFunction (WxeFunction function, object sender, bool shouldExecuteAsSubFunction)
    {
      if (shouldExecuteAsSubFunction)
        ExecuteFunction (function, new WxeCallArguments ((Control) sender, new WxeCallOptions()));
      else
      {
        try
        {
          ExecuteFunction (function, new WxeCallArguments ((Control) sender, new WxeCallOptionsExternal ("_self")));
        }
        catch (WxeCallExternalException)
        {
          // ReSharper disable once RedundantJumpStatement
          return;
        }
      }
    }

    private void RegisterExecuteCommandClientFunction (string functionName, string postBackEventHandlerId)
    {
      var script = String.Format (
          "function {0} (command,argument) {{ __doPostBack(\"{1}\",command + \":\" + argument); }}",
          functionName,
          postBackEventHandlerId);
      ClientScript.RegisterClientScriptBlock (this, GetType(), functionName + "Script", script);
    }
  }
}