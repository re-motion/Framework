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
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.ExecutionEngine.Obsolete;

namespace Remotion.Web.Test.Shared.ExecutionEngine
{
  /// <summary>
  /// Summary description for WebForm1.
  /// </summary>
  public class WebForm1: WxePage
  {
    protected System.Web.UI.WebControls.TextBox TextBox1;
    protected System.Web.UI.WebControls.Button Stay;
    protected System.Web.UI.WebControls.Button Next;
    protected System.Web.UI.WebControls.CheckBox IsPostBackCheck;
    protected System.Web.UI.WebControls.Button Sub;
    protected System.Web.UI.WebControls.Label StackLabel;
    protected System.Web.UI.WebControls.Label Label2;
    protected System.Web.UI.WebControls.Label Label3;
    protected System.Web.UI.WebControls.Label Var1Label;
    protected System.Web.UI.WebControls.Label Label5;
    protected System.Web.UI.WebControls.Label Var2Label;
    protected System.Web.UI.WebControls.Button Throw;
    protected System.Web.UI.WebControls.Label Label1;
    protected System.Web.UI.WebControls.Label RetValLabel;
    protected System.Web.UI.WebControls.Button SubExtButton;
    protected System.Web.UI.WebControls.Button SubNoReturnButton;
    protected System.Web.UI.WebControls.TextBox SubNoReturnField;
    protected System.Web.UI.WebControls.Button ThrowText;
    protected System.Web.UI.WebControls.Calendar Calendar1;

    public readonly WxeParameterDeclaration[] PageParameters = {
        new WxeParameterDeclaration("text", true, WxeParameterDirection.InOut, typeof(string)),
        new WxeParameterDeclaration("invocations", false, WxeParameterDirection.Out, typeof(int))
    };

    private ISampleFunctionVariables Function
    {
      get { return (ISampleFunctionVariables)CurrentFunction; }
    }

    private void Page_Load (object sender, System.EventArgs e)
    {

      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      for (WxeStep step = CurrentPageStep; step != null; step = step.ParentStep)
        sb.AppendFormat("{0}<br>", step.ToString());
      StackLabel.Text = sb.ToString();

      Var1Label.Text = Function.Var1;
      Var2Label.Text = Function.Var2;
      IsPostBackCheck.Checked = IsPostBack;

      if (! IsPostBack)
      {
        Calendar1.SelectedDate = DateTime.Now.Date;
      }
    }

    #region Web Form Designer generated code
    override protected void OnInit (EventArgs e)
    {
      //
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      //
      InitializeComponent();
      base.OnInit(e);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      this.ThrowText.Click += new System.EventHandler(this.ThrowText_Click);
      this.Stay.Click += new System.EventHandler(this.Stay_Click);
      this.Next.Click += new System.EventHandler(this.Next_Click);
      this.SubNoReturnField.TextChanged += new System.EventHandler(this.SubNoReturnField_TextChanged);
      this.SubNoReturnButton.Click += new System.EventHandler(this.SubNoReturnButton_Click);
      this.SubExtButton.Click += new System.EventHandler(this.SubExtButton_Click);
      this.Throw.Click += new System.EventHandler(this.Throw_Click);
      this.Sub.Click += new System.EventHandler(this.Sub_Click);
      this.Calendar1.SelectionChanged += new System.EventHandler(this.Calendar1_SelectionChanged);
      this.Load += new System.EventHandler(this.Page_Load);

    }
    #endregion

    private void Stay_Click (object sender, System.EventArgs e)
    {

    }

    private void Next_Click (object sender, System.EventArgs e)
    {
      ExecuteNextStep();
      //      WxeFunction currentFunction = ((WxeFunction) Session["CurrentFunction"]);
      //      currentFunction.ExecutingStep.ExecuteNextStep (Context);
    }

    private void Sub_Click (object sender, System.EventArgs e)
    {
      // CurrentPageStep.ExecuteFunction ((Control)sender, this, new SubFunction("call var1", "call var2"));
      if (! IsReturningPostBack)
      {
        var subFunction = new SampleWxeSubFunction("sub var 1", "vall var2");
        this.ExecuteFunction(subFunction);
      }
      else
      {
        var subFunction = (SampleWxeSubFunction)ReturningFunction;
        RetValLabel.Text = subFunction.Var1;
      }
    }

    private void Throw_Click (object sender, System.EventArgs e)
    {
      throw new ApplicationException(string.Empty);
    }

    private void ThrowText_Click (object sender, System.EventArgs e)
    {
      throw new ApplicationException("test");
    }

    private void SubExtButton_Click (object sender, System.EventArgs e)
    {
      if (! IsReturningPostBack)
      {
        SubFunction subFunction = new SubFunction("subext var 1", "vall var2");
        this.ExecuteFunctionExternal(
            subFunction,
            "_blank",
            "width=700,height=500,top=100,left=100,resizable=yes,location=no,menubar=no,status=no,toolbar=no,scrollbars=no",
            (Control)sender,
            true);
      }
      else
      {
        SubFunction subFunction = (SubFunction)ReturningFunction;
        RetValLabel.Text = subFunction.Var1;
      }
    }

    private void SubNoReturnButton_Click (object sender, System.EventArgs e)
    {
      this.ExecuteFunctionNoRepost(new SubFunction("v1", "button"), (Control)sender);
    }

    private void SubNoReturnField_TextChanged (object sender, System.EventArgs e)
    {
      this.ExecuteFunctionNoRepost(new SubFunction("v1", "textbox"), (Control)sender);
    }

    private void Calendar1_SelectionChanged (object sender, System.EventArgs e)
    {
      if (! IsReturningPostBack)
      {
        this.ExecuteFunctionExternal(new SubFunction("v1", "calendar"), "_blank", (Control)sender, true);
      }
      else
      {
        Calendar1.SelectedDate = Calendar1.SelectedDate.AddDays(1);
      }
    }

    public class SubFunction : WxeFunction, ISampleFunctionVariables
    {
      public SubFunction (string var1, string var2)
          : base(new NoneTransactionMode(), var1, var2)
      {
      }

      public SubFunction (params object[] args)
          : base(new NoneTransactionMode(), args)
      {
      }

      [WxeParameter(1, true, WxeParameterDirection.InOut)]
      public string Var1
      {
        get { return (string)Variables["Var1"]; }
        set { Variables["Var1"] = value; }
      }

      [WxeParameter(2, WxeParameterDirection.InOut)]
      public string Var2
      {
        get { return (string)Variables["Var2"]; }
        set { Variables["Var2"] = value; }
      }

      private void Step1 (WxeContext context)
      {
        Var2 = "SubFunction Step1";
      }

      private WxeStep Step2 = new WxeResourcePageStep(typeof(WebForm1), @"ExecutionEngine\WebForm1.aspx");

      private static int counter = 0;
      private void Step3 (WxeContext context)
      {
        ++counter;
        Var1 += " SubFunction " + counter.ToString();
      }
    }
  }
}
