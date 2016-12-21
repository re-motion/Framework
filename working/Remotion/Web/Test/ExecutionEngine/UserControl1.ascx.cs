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

namespace Remotion.Web.Test.ExecutionEngine
{
  public class UserControl1 : WxeUserControl
  {
    protected System.Web.UI.WebControls.TextBox TextBox1;
    protected System.Web.UI.WebControls.Button Stay;
    protected System.Web.UI.WebControls.Button Sub;
    protected System.Web.UI.WebControls.Label Label1;
    protected System.Web.UI.WebControls.Button Next;

    private static int s_counter = 0;

    private void Page_Load (object sender, System.EventArgs e)
    {
      if (! IsPostBack)
      {
        ++ s_counter;
        ViewState["Counter"] = s_counter.ToString();
      }
      Label1.Text = (string) ViewState["Counter"];		
    }

    #region Web Form Designer generated code
    override protected void OnInitComplete(EventArgs e)
    {
      //
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      //
      InitializeComponent();
      base.OnInitComplete(e);
    }
	
    /// <summary>
    ///		Required method for Designer support - do not modify
    ///		the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.Stay.Click += new System.EventHandler(this.Stay_Click);
      this.Sub.Click += new System.EventHandler(this.Sub_Click);
      this.Next.Click += new System.EventHandler(this.Next_Click);
      this.Load += new System.EventHandler(this.Page_Load);

    }
    #endregion

    private void Stay_Click (object sender, System.EventArgs e)
    {
  
    }

    private void Sub_Click (object sender, System.EventArgs e)
    {
      ViewState["Counter"] += " Sub_Click";
      WxePage.ExecuteFunctionNoRepost (new WebForm1.SubFunction ("usercontrol var1", "usercontrol var2"), (Control) sender);  
    }

    private void Next_Click (object sender, System.EventArgs e)
    {
      WxePage.ExecuteNextStep ();
    }

  }
}
