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

namespace Test
{
	//[WxePageFunction ("AutoPage.aspx", typeof (WxeFunction))]
	//[WxePageParameter (1, "InArg", typeof (string), true)]
	//[WxePageParameter (2, "InOutArg", typeof (string), true, WxeParameterDirection.InOut)]
	//[WxePageParameter (3, "OutArg", typeof (string), WxeParameterDirection.Out, IsReturnValue = true)]
	//[WxePageVariable ("Suffix", typeof (string))]

  //[WxeFunctionTargetPage (typeof (AutoPage))]
  //public class AutoPageFunction
  //{
  //}

  // <WxeFunction codeBehindType="Test.AutoPage" markupFile="AutoPage.aspx" functionBaseType="WxeFunction" mode="Page">
	//   <Parameter name="InArg" type="String" required="true" />
	//   <Parameter name="InOutArg" type="String" required="true" direction="InOut" />
	//   <ReturnValue name="OutArg" type="String"/>
	//   <Variable name="Suffix" type="String" />
  // </WxeFunction>
  public partial class AutoPage: WxePage
  {
    protected void Page_Load (object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        InArgField.Text = InArg;
        Suffix = "'";
      }
    }

    protected void ExecSelfButton_Click (object sender, EventArgs e)
    {
      string inOutParam = InOutArgField.Text + Suffix;
      OutArgField.Text = AutoPage.Call (this, InArgField.Text + Suffix, ref inOutParam);
      InOutArgField.Text = inOutParam;
    }

    protected void ExecCalledPageButton_Click (object sender, EventArgs e)
    {
      string a, b = null;
      InArgField.Text = CalledPage.Call (this, "hallo", null, out a, ref b);
    }

    protected void ExecUserControlButton_Click (object sender, EventArgs e)
    {
      try
      {
        string a = null;
        InArgField.Text = AutoUserControl.Call (this, AutoUserControl, (Control) sender, "hallo", ref a);
      }
      catch (WxeIgnorableException)
      {
      }
    }

    protected void ReturnButton_Click (object sender, EventArgs e)
    {
      InOutArg = InOutArgField.Text + Suffix;
      OutArg = OutArgField.Text + Suffix;
      Return ();
    }
  }


  //[WxePageFunction ("AutoPage.aspx", typeof (WxeFunction))]
  //[WxePageParameter (1, "InArg", typeof (string), true)]
  //[WxePageParameter (2, "InOutArg", typeof (string), true, WxeParameterDirection.InOut)]
  //[WxePageParameter (3, "OutArg", typeof (string), WxeParameterDirection.Out, IsReturnValue = true)]
  //[WxePageVariable ("LocalVariable", typeof (string))]
  //public partial class AutoPage : WxePage
  //{
  //  // for ASP.NET 1.1
  //  // new AutoPageVariables Variables { get { return ((AutoPageFunction) CurrentFunction).PageVariables; } }

  //  protected void Page_Load (object sender, EventArgs e)
  //  {
  //    if (!IsPostBack)
  //    {
  //      // use input parameters for control initialization

  //      // for ASP.NET 2.0
  //      InArgField.Text = InArg;
  //      InOutArgField.Text = InOutArg;

  //      // for ASP.NET 1.1
  //      //InArgField.Text = Variables.InArg;
  //      //InOutArgField.Text = Variables.InOutArg;
  //    }
  //  }

  //  //public static string Call (IWxePage currentPage, string InArg, ref string InOutArg)
  //  //{
  //  //  AutoPageFunction function;
  //  //  if (!currentPage.IsReturningPostBack)
  //  //  {
  //  //    function = new AutoPageFunction ();
  //  //    function.InArg = InArg;
  //  //    function.InOutArg = InOutArg;
  //  //    currentPage.ExecuteFunction (function);
  //  //    throw new Exception ("This code cannot be reached.");
  //  //  }
  //  //  else
  //  //  {
  //  //    function = (AutoPageFunction) currentPage.ReturningFunction;
  //  //    InOutArg = function.InOutArg;
  //  //    return function.OutArg;
  //  //  }
  //  //}

  //  protected void ExecSelfButton_Click (object sender, EventArgs e)
  //  {
  //    string inOutParam = InOutArgField.Text + "'";
  //    OutArgField.Text = AutoPageFunction.Call (this, InArgField.Text + "'", ref inOutParam);
  //    InOutArgField.Text = inOutParam;

  //    //if (!IsReturningPostBack)
  //    //{
  //    //  // call function recursively
  //    //  ExecuteFunction (new AutoPageFunction (
  //    //      InArgField.Text + "'",
  //    //      InOutArgField.Text + "'"));
  //    //}
  //    //else
  //    //{
  //    //  // when call returns, use output parameters 
  //    //  AutoPageFunction function = (AutoPageFunction) ReturningFunction;
  //    //  OutArgField.Text = function.OutArg;
  //    //  InOutArgField.Text = function.InOutArg;
  //    //}
  //  }

  //  protected void ReturnButton_Click (object sender, EventArgs e)
  //  {
  //    // set output parameters and return

  //    // for ASP.NET 2.0
  //    InOutArg = InOutArgField.Text + "'";
  //    OutArg = OutArgField.Text + "'";
  //    Return ();

  //    // obsolete
  //    // Return (InOutArgField.Text + "'", OutArgField.Text + "'");

  //    // for ASP.NET 1.1
  //    //Variables.InOutArg = InOutArgField.Text + "'";
  //    //Variables.OutArg = OutArgField.Text + "'";
  //    //ExecuteNextStep ();
  //  }
  //}

  //internal struct AutoPageVariables
  //{
  //  private /*readonly*/ Remotion.Collections.NameObjectCollection Variables;

  //  public AutoPageVariables (Remotion.Collections.NameObjectCollection variables)
  //  {
  //    Variables = variables;
  //  }

  //  public string InArg
  //  {
  //    get { return (string) Variables["InArg"]; }
  //  }

  //  public string InOutArg
  //  {
  //    get { return (string) Variables["InOutArg"]; }
  //    set { Variables["InOutArg"] = value; }
  //  }

  //  public string OutArg
  //  {
  //    set { Variables["OutArg"] = value; }
  //  }
  //}

  //public class AutoPageFunction: WxeFunction
  //{
  //  public AutoPageVariables PageVariables
  //  {
  //    get { return new AutoPageVariables (Variables); } 
  //  }
  //}


  //public partial class AutoPage
  //{
  //  public string InArg
  //  {
  //    get { return (string)Variables["InArg"]; }
  //  }

  //  public string InOutArg
  //  {
  //    get { return (string)Variables["InOutArg"]; }
  //    set { Variables["InOutArg"] = value; }
  //  }

  //  public string OutArg
  //  {
  //    set { Variables["OutArg"] = value; }
  //  }
  //}
  
  //public class AutoPageFunction : WxeFunction
  //{
  //  public AutoPageFunction()
  //  {
  //  }

  //  public AutoPageFunction(params object[] args)
  //    : base(args)
  //  {
  //  }

  //  public AutoPageFunction(string InArg, string InOutArg)
  //    : base(InArg, InOutArg)
  //  {
  //  }

  //  [WxeParameter(1, true, WxeParameterDirection.In)]
  //  public string InArg
  //  {
  //    set { Variables["InArg"] = value; }
  //  }

  //  [WxeParameter(2, true, WxeParameterDirection.InOut)]
  //  public string InOutArg
  //  {
  //    get { return (string)Variables["InOutArg"]; }
  //    set { Variables["InOutArg"] = value; }
  //  }

  //  [WxeParameter(3, WxeParameterDirection.Out)]
  //  public string OutArg
  //  {
  //    get { return (string)Variables["OutArg"]; }
  //  }

  //  WxeStep Step1 = new WxePageStep ("AutoPage.aspx");
  //}
}
