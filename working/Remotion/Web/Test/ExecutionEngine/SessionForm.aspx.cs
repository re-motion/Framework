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
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Obsolete;
using Remotion.Web.UI;

namespace Remotion.Web.Test.ExecutionEngine
{
  public class SessionForm : WxePage
  {
    protected Remotion.Web.UI.Controls.WebButton PostBackButton;
    protected System.Web.UI.WebControls.LinkButton LinkButton1;
    protected Remotion.Web.UI.Controls.WebButton OpenSelfButton;
    protected System.Web.UI.WebControls.Button Button1;
    protected System.Web.UI.WebControls.ImageButton ImageButton;
    protected Label ImageButtonLabel;
    protected Remotion.Web.UI.Controls.WebButton Button1Button;
    protected Remotion.Web.UI.Controls.WebButton Submit1Button;
    protected Remotion.Web.UI.Controls.WebButton Button2Button;
    protected Remotion.Web.UI.Controls.WebButton ExecuteButton;
    protected Remotion.Web.UI.Controls.WebButton ExecuteNoRepostButton;
    protected System.Web.UI.WebControls.Label FunctionTokenLabel;
    protected System.Web.UI.WebControls.Label PostBackIDLabel;
    protected Remotion.Web.UI.Controls.WebButton OpenSampleFunctionWithPermanentUrlInNewWindowButton;
    protected Remotion.Web.UI.Controls.WebButton OpenSampleFunctionInNewWindowButton;
    protected Remotion.Web.UI.Controls.WebButton OpenSampleFunctionWithPermanentUrlButton;
    protected Remotion.Web.UI.Controls.WebButton OpenSampleFunctionButton;
    protected System.Web.UI.WebControls.HyperLink CurrentFunctionPermaLink;
    protected System.Web.UI.WebControls.HyperLink SampleFunctionPermaLink;
    protected Remotion.Web.UI.Controls.WebButton OpenSessionFunctionWithPermanentUrlInNewWindowButton;
    protected Remotion.Web.UI.Controls.WebButton OpenSessionFunctionInNewWindowButton;
    protected Remotion.Web.UI.Controls.WebButton OpenSessionFunctionButton;
    protected Remotion.Web.UI.Controls.WebButton OpenSessionFunctionWithPermanentUrlButton;
    protected Remotion.Web.UI.Controls.WebButton ContextOpenSampleFunctionInNewWindowButton;
    protected Remotion.Web.UI.Controls.WebButton ContextOpenSampleFunctionWithPermanentUrlInNewWindowButton;
    protected Remotion.Web.UI.Controls.WebButton ContextOpenSampleFunctionButton;
    protected Remotion.Web.UI.Controls.WebButton ContextOpenSampleFunctionWithPermanentUrlButton;
    protected Remotion.Web.UI.Controls.WebButton OpenSampleFunctionByRedirectDoNotReturnButton;
    protected Remotion.Web.UI.Controls.WebButton OpenSampleFunctionWithPermanentUrlByDoNotReturnRedirectButton;
    protected Remotion.Web.UI.Controls.WebButton OpenSampleFunctionByRedirectButton;
    protected Remotion.Web.UI.Controls.WebButton OpenSampleFunctionWithPermanentUrlByRedirectButton;
    protected System.Web.UI.WebControls.Label ViewStateTokenLabel;
    protected Remotion.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;


    private void Page_Load(object sender, System.EventArgs e)
    {
      RegisterClientSidePageEventHandler (SmartPageEvents.OnPostBack, "Page_PostBack", "Page_PostBack");
      RegisterClientSidePageEventHandler (SmartPageEvents.OnPostBack, "Page_Abort", "Page_Abort");
      RegisterClientSidePageEventHandler (SmartPageEvents.OnLoad, "Page_Load", "Page_Load");
      RegisterClientSidePageEventHandler (SmartPageEvents.OnBeforeUnload, "Page_BeforeUnload", "Page_BeforeUnload");
      RegisterClientSidePageEventHandler (SmartPageEvents.OnUnload, "Page_Unload", "Page_Unload");
      FunctionTokenLabel.Text = "Token = " + WxeContext.Current.FunctionToken;
      PostBackIDLabel.Text = "PostBackID = " + WxeContext.Current.PostBackID.ToString();
      ViewStateTokenLabel.Text = "ViewStateToken = " + _viewStateToken.ToLongDateString() + ", " + _viewStateToken.ToLongTimeString();

      CurrentFunctionPermaLink.NavigateUrl = GetPermanentUrl ();
      CurrentFunctionPermaLink.Text = CurrentFunctionPermaLink.NavigateUrl;
      NameValueCollection queryString = new NameValueCollection();
      queryString.Add ("Parameter", "Hello World!");
      SampleFunctionPermaLink.NavigateUrl = GetPermanentUrl (typeof (SampleWxeFunction), queryString);
      SampleFunctionPermaLink.Text = HttpUtility.HtmlEncode (SampleFunctionPermaLink.NavigateUrl);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      HtmlHeadAppender.Current.RegisterPageStylesheetLink ();
    }

    override protected void OnInit(EventArgs e)
    {
      //
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      //
      InitializeComponent();
      base.OnInit(e);
    }
    #region Web Form Designer generated code

	
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {    
      this.PostBackButton.Click += new System.EventHandler(this.PostBackButton_Click);
      this.OpenSelfButton.Click += new System.EventHandler(this.OpenSelfButton_Click);
      this.Button1.Click += new System.EventHandler(this.Button1_Click);
      this.Button1Button.Click += new System.EventHandler(this.Button1Button_Click);
      this.Submit1Button.Click += new System.EventHandler(this.Submit1Button_Click);
      this.ExecuteButton.Click += new System.EventHandler(this.ExecuteButton_Click);
      this.ExecuteNoRepostButton.Click += new System.EventHandler(this.ExecuteNoRepostButton_Click);
      this.Button2Button.Click += new System.EventHandler(this.Button2Button_Click);
      this.ImageButton.Click += new ImageClickEventHandler (this.ImageButton_Click);
      this.OpenSampleFunctionButton.Click += new System.EventHandler(this.OpenSampleFunctionButton_Click);
      this.OpenSampleFunctionWithPermanentUrlButton.Click += new System.EventHandler(this.OpenSampleFunctionWithPermanentUrlButton_Click);
      this.OpenSampleFunctionInNewWindowButton.Click += new System.EventHandler(this.OpenSampleFunctionInNewWindowButton_Click);
      this.OpenSampleFunctionWithPermanentUrlInNewWindowButton.Click += new System.EventHandler(this.OpenSampleFunctionWithPermanentUrlInNewWindowButton_Click);
      this.OpenSessionFunctionButton.Click += new System.EventHandler(this.OpenSessionFunctionButton_Click);
      this.OpenSessionFunctionWithPermanentUrlButton.Click += new System.EventHandler(this.OpenSessionFunctionWithPermanentUrlButton_Click);
      this.OpenSessionFunctionInNewWindowButton.Click += new System.EventHandler(this.OpenSessionFunctionInNewWindowButton_Click);
      this.OpenSessionFunctionWithPermanentUrlInNewWindowButton.Click += new System.EventHandler(this.OpenSessionFunctionWithPermanentUrlInNewWindowButton_Click);
      this.OpenSampleFunctionByRedirectButton.Click += new System.EventHandler(this.OpenSampleFunctionByRedirectButton_Click);
      this.OpenSampleFunctionByRedirectDoNotReturnButton.Click += new System.EventHandler(this.OpenSampleFunctionByRedirectDoNotReturnButton_Click);
      this.OpenSampleFunctionWithPermanentUrlByRedirectButton.Click += new System.EventHandler(this.OpenSampleFunctionWithPermanentUrlByRedirectButton_Click);
      this.OpenSampleFunctionWithPermanentUrlByDoNotReturnRedirectButton.Click += new System.EventHandler(this.OpenSampleFunctionWithPermanentUrlByDoNotReturnRedirectButton_Click);
      this.ContextOpenSampleFunctionButton.Click += new System.EventHandler(this.ContextOpenSampleFunctionButton_Click);
      this.ContextOpenSampleFunctionInNewWindowButton.Click += new System.EventHandler(this.ContextOpenSampleFunctionInNewWindowButton_Click);
      this.ContextOpenSampleFunctionWithPermanentUrlButton.Click += new System.EventHandler(this.ContextOpenSampleFunctionWithPermanentUrlButton_Click);
      this.ContextOpenSampleFunctionWithPermanentUrlInNewWindowButton.Click += new System.EventHandler(this.ContextOpenSampleFunctionWithPermanentUrlInNewWindowButton_Click);
      this.ShowAbortConfirmation = Remotion.Web.UI.ShowAbortConfirmation.Always;
      this.Load += new System.EventHandler(this.Page_Load);

    }
    #endregion

    private DateTime _viewStateToken = DateTime.MinValue;

    protected override void LoadViewState(object savedState)
    {
      if (savedState is Pair)
      {
        Pair pair = (Pair) savedState;
        base.LoadViewState (pair.First);
        _viewStateToken = (DateTime) pair.Second;
      }
      else
      {
        base.LoadViewState (savedState);
      }
    }

    protected override object SaveViewState()
    {
      return new Pair (base.SaveViewState (), DateTime.Now);
    }

    private void PostBackButton_Click(object sender, System.EventArgs e)
    {
      System.Threading.Thread.Sleep (10000);  
    }

    private void OpenSelfButton_Click(object sender, System.EventArgs e)
    {
      if (!IsReturningPostBack)
        this.ExecuteFunctionExternal (new SessionWxeFunction (true), "_blank", OpenSelfButton, true);
    }

    private void Button1_Click(object sender, System.EventArgs e)
    {
  
    }

    private void Button1Button_Click(object sender, System.EventArgs e)
    {
  
    }

    private void Submit1Button_Click(object sender, System.EventArgs e)
    {
  
    }

    private void Button2Button_Click(object sender, System.EventArgs e)
    {
  
    }
    
    private void ImageButton_Click(object sender, System.EventArgs e)
    {
      if (!IsReturningPostBack)
        ExecuteFunction (new SampleWxeFunction(), new WxeCallArguments ((Control) sender, new WxeCallOptions()));
      else
        ImageButtonLabel.Text = "returning postback";
    }

    private void ExecuteButton_Click(object sender, System.EventArgs e)
    {
      if (!IsReturningPostBack)
        this.ExecuteFunction (new SampleWxeFunction ());
    }

    private void ExecuteNoRepostButton_Click (object sender, System.EventArgs e)
    {
      this.ExecuteFunctionNoRepost (new SampleWxeFunction (), (Control) sender);
    }

    private void OpenSampleFunctionButton_Click (object sender, System.EventArgs e)
    {
      if (! IsReturningPostBack)
        this.ExecuteFunction (new SampleWxeFunction (), false, false);
    }

    private void OpenSampleFunctionWithPermanentUrlButton_Click (object sender, System.EventArgs e)
    {
      if (! IsReturningPostBack)
      {
        NameValueCollection queryString = new NameValueCollection();
        queryString.Add ("Parameter", "Hello World!");
        this.ExecuteFunction (new SampleWxeFunction(), true, true, queryString);
      }
    }

    private void OpenSampleFunctionInNewWindowButton_Click (object sender, System.EventArgs e)
    {
      if (!IsReturningPostBack)
        this.ExecuteFunctionExternal (new SampleWxeFunction (), "_blank", (Control) sender, true, false, false);
    }

    private void OpenSampleFunctionWithPermanentUrlInNewWindowButton_Click (object sender, System.EventArgs e)
    {
      if (!IsReturningPostBack)
      {
        NameValueCollection queryString = new NameValueCollection();
        queryString.Add ("Parameter", "Hello World!");
        this.ExecuteFunctionExternal (new SampleWxeFunction (), "_blank", (Control) sender, true, true, true, queryString);
      }
    }

    private void OpenSessionFunctionButton_Click(object sender, System.EventArgs e)
    {
      if (! IsReturningPostBack)
        this.ExecuteFunction (new SessionWxeFunction (true), false, false);
    }

    private void OpenSessionFunctionWithPermanentUrlButton_Click(object sender, System.EventArgs e)
    {
      if (! IsReturningPostBack)
        this.ExecuteFunction (new SessionWxeFunction (true), true, true);
    }

    private void OpenSessionFunctionInNewWindowButton_Click (object sender, System.EventArgs e)
    {
      if (!IsReturningPostBack)
        this.ExecuteFunctionExternal (new SessionWxeFunction (true), "_blank", (Control) sender, true, false, false);
    }

    private void OpenSessionFunctionWithPermanentUrlInNewWindowButton_Click (object sender, System.EventArgs e)
    {
      if (!IsReturningPostBack)
        this.ExecuteFunctionExternal (new SessionWxeFunction (true), "_blank", (Control) sender, true, true, true);
    }

    private void OpenSampleFunctionByRedirectButton_Click(object sender, System.EventArgs e)
    {
      if (!IsReturningPostBack)
      {
        NameValueCollection queryString = new NameValueCollection();
        queryString.Add ("Parameter", "Hello World!");
        this.ExecuteFunctionExternal (new SampleWxeFunction (), (Control) sender, false, true, queryString, true, null);
      }
    }

    private void OpenSampleFunctionByRedirectDoNotReturnButton_Click(object sender, System.EventArgs e)
    {
      if (!IsReturningPostBack)
      {
        NameValueCollection queryString = new NameValueCollection();
        queryString.Add ("Parameter", "Hello World!");
        this.ExecuteFunctionExternal (new SampleWxeFunction (), (Control) sender, false, true, queryString, false, null);
      }
    }

    private void OpenSampleFunctionWithPermanentUrlByRedirectButton_Click(object sender, System.EventArgs e)
    {
      if (!IsReturningPostBack)
      {
        this.ExecuteFunctionExternal (new SampleWxeFunction (), (Control) sender, true, true, null, true, null);
      }
    }

    private void OpenSampleFunctionWithPermanentUrlByDoNotReturnRedirectButton_Click(object sender, System.EventArgs e)
    {
      if (!IsReturningPostBack)
      {
        this.ExecuteFunctionExternal (new SampleWxeFunction (), (Control) sender, true, true, null, false, null);
      }
    }

    private void ContextOpenSampleFunctionButton_Click(object sender, System.EventArgs e)
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Add ("Parameter", "Hello World!");
      WxeContext.ExecuteFunctionExternal (this, new SampleWxeFunction (), queryString, true);
    }

    private void ContextOpenSampleFunctionInNewWindowButton_Click(object sender, System.EventArgs e)
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Add ("Parameter", "Hello World!");
      WxeContext.ExecuteFunctionExternal (this, new SampleWxeFunction (), "_blank", string.Empty, queryString);
    }

    private void ContextOpenSampleFunctionWithPermanentUrlButton_Click(object sender, System.EventArgs e)
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Add ("Parameter", "Hello World!");
      WxeContext.ExecuteFunctionExternal (this, new SampleWxeFunction (), true, queryString, true);
    }

    private void ContextOpenSampleFunctionWithPermanentUrlInNewWindowButton_Click(object sender, System.EventArgs e)
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Add ("Parameter", "Hello World!");
      WxeContext.ExecuteFunctionExternal (this, new SampleWxeFunction (), "_blank", string.Empty, true, queryString);
    }
  }
}
