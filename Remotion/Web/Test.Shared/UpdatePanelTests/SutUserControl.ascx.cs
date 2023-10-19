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
using System.Threading;
using System.Web.UI;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.PostBackTargets;

namespace Remotion.Web.Test.Shared.UpdatePanelTests
{
  public partial class SutUserControl : System.Web.UI.UserControl
  {
    private PostBackEventHandler _postBackEventHandler;

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);
      AsyncPostBackInsideUpdatePanelButton.Click += HandlePostBack;
      AsyncPostBackOutsideUpdatePanelButton.Click += HandlePostBack;
      SyncPostBackInsideUpdatePanelButton.Click += HandlePostBack;
      SyncPostBackOutsideUpdatePanelButton.Click += HandlePostBack;
      AsyncPostBackInsideUpdatePanelLinkButton.Click += HandlePostBack;
      AsyncPostBackOutsideUpdatePanelLinkButton.Click += HandlePostBack;
      SyncPostBackInsideUpdatePanelLinkButton.Click += HandlePostBack;
      SyncPostBackOutsideUpdatePanelLinkButton.Click += HandlePostBack;
      AsyncPostBackInsideUpdatePanelWebButton.Click += HandlePostBack;
      AsyncPostBackOutsideUpdatePanelWebButton.Click += HandlePostBack;
      SyncPostBackInsideUpdatePanelWebButton.Click += HandlePostBack;
      SyncPostBackOutsideUpdatePanelWebButton.Click += HandlePostBack;
      DropDownMenuInsideUpdatePanel.EventCommandClick += HandlePostBack;
      ListMenuInsideUpdatePanel.EventCommandClick += HandlePostBack;

      _postBackEventHandler = new PostBackEventHandler();
      _postBackEventHandler.ID = "PostBackEventHandler";
      _postBackEventHandler.PostBack += HandlePostBack;
      Controls.Add(_postBackEventHandler);

      string asyncPostBackCommandInsideUpdatePanelID = "AsyncPostBackCommandInsideUpdatePanel";
      AsyncCommandInsideUpdatePanelHyperLink.NavigateUrl = "#";
      AsyncCommandInsideUpdatePanelHyperLink.Attributes["onclick"] =
          Page.ClientScript.GetPostBackEventReference(_postBackEventHandler, asyncPostBackCommandInsideUpdatePanelID) + ";return false;";

      string syncPostBackCommandInsideUpdatePanelID = "SyncPostBackCommandInsideUpdatePanel";
      ((ISmartPage)Page).RegisterCommandForSynchronousPostBack(_postBackEventHandler, syncPostBackCommandInsideUpdatePanelID);
      SyncCommandInsideUpdatePanelHyperLink.NavigateUrl = "#";
      SyncCommandInsideUpdatePanelHyperLink.Attributes["onclick"] =
          Page.ClientScript.GetPostBackEventReference(_postBackEventHandler, syncPostBackCommandInsideUpdatePanelID) + ";return false;";
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad(e);

      UpdateStatus(null);

      WebMenuItem menuItem = new WebMenuItem();
      menuItem.ItemID = "Item" + PostBackCount;
      menuItem.Text = WebString.CreateFromText("Item " + PostBackCount);
      DropDownMenuInsideUpdatePanel.MenuItems.Add(menuItem);
      PlaceHolderInsideUpdatePanel.SetRenderMethodDelegate(
          delegate (HtmlTextWriter output, Control container)
          {
            output.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "lime");
            output.RenderBeginTag("section");
            output.Write("section: " + DateTime.Now.Ticks);
            output.RenderEndTag();

            output.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            output.RenderBeginTag("script");
            output.Write("window.console.log('direct render: inside updatepanel: " + DateTime.Now.Ticks + "');");
            output.RenderEndTag();

            ((ISmartPage)Page).ClientScript.RegisterStartupScriptBlock(
                (ISmartPage)Page,
                typeof(Page),
                "insideUpdatePanel",
                "window.console.log('ScriptManager: inside updatepanel: " + DateTime.Now.Ticks + "');");
          });
      PlaceHolderOutsideUpdatePanel.SetRenderMethodDelegate(
          delegate (HtmlTextWriter output, Control container)
          {
            output.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "pink");
            output.RenderBeginTag("nav");
            output.Write("nav: " + DateTime.Now.Ticks);
            output.RenderEndTag();

            output.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            output.RenderBeginTag("script");
            output.Write("window.console.log('direct render: outside updatepanel: " + DateTime.Now.Ticks + "');");
            output.RenderEndTag();

            ((ISmartPage)Page).ClientScript.RegisterStartupScriptBlock(
                (ISmartPage)Page,
                typeof(Page),
                "outsideUpdatePanel",
                "window.console.log('ScriptManager: outside updatepanel: " + DateTime.Now.Ticks + "');");
          });

      ((ISmartPage)Page).ClientScript.RegisterStartupScriptBlock(
          (ISmartPage)Page,
          typeof(Page),
          "before Render",
          "window.console.log('ScriptManager: before Render: " + DateTime.Now.Ticks + "');");
    }

    protected int PostBackCount
    {
      get { return (int?)ViewState["PostBackCount"] ?? 0; }
      set { ViewState["PostBackCount"] = value; }
    }

    private void HandlePostBack (object sender, EventArgs e)
    {
      Thread.Sleep(1000);
      PostBackCount++;
      UpdateStatus(sender);
    }

    private void UpdateStatus (object sender)
    {
      PostBackCountInsideUpdatePanelLabel.Text = PostBackCount.ToString();
      PostBackCountOutsideUpdatePanelLabel.Text = PostBackCount.ToString();

      string lastPostBack = "undefined";
      if (sender != null)
        lastPostBack = ((Control)sender).ID;
      LastPostBackInsideUpdatePanelLabel.Text = lastPostBack;
      LastPostBackOutsideUpdatePanelLabel.Text = lastPostBack;
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ((ISmartPage)Page).ClientScript.RegisterStartupScriptBlock(
          (ISmartPage)Page,
          typeof(Page),
          "inside child Render",
          "window.console.log('ScriptManager: inside child Render: " + DateTime.Now.Ticks + "');");
      base.Render(writer);
    }
  }
}
