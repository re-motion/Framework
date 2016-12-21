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
using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.Data.DomainObjects.Web.Test.WxeFunctions;
using Remotion.ObjectBinding;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test
{
public class UndefinedEnumTestPage : WxePage
{
  protected System.Web.UI.HtmlControls.HtmlTable SearchFormGrid;
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl ExistingObjectWithUndefinedEnumDataSource;
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl NewObjectWithUndefinedEnumDataSource;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue ExistingObjectEnumProperty;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue NewObjectEnumProperty;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue SearchObjectEnumProperty;
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl SearchObjectWithUndefinedEnumDataSource;
  protected System.Web.UI.WebControls.Button TestButton;
  protected Remotion.Web.UI.Controls.FormGridManager FormGridManager;
  protected Remotion.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;

  private UndefinedEnumTestFunction MyFunction 
  {
    get { return (UndefinedEnumTestFunction) CurrentFunction; }
  }

  private void Page_Load(object sender, System.EventArgs e)
	{
    NewObjectWithUndefinedEnumDataSource.BusinessObject = (IBusinessObject) MyFunction.NewObjectWithUndefinedEnum;
    ExistingObjectWithUndefinedEnumDataSource.BusinessObject = (IBusinessObject) MyFunction.ExistingObjectWithUndefinedEnum;
    SearchObjectWithUndefinedEnumDataSource.BusinessObject = (IBusinessObject) MyFunction.SearchObjectWithUndefinedEnum;

    NewObjectWithUndefinedEnumDataSource.LoadValues (IsPostBack);
    ExistingObjectWithUndefinedEnumDataSource.LoadValues (IsPostBack);
    SearchObjectWithUndefinedEnumDataSource.LoadValues (IsPostBack);
	}

	#region Web Form Designer generated code
	override protected void OnInit(EventArgs e)
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
	private void InitializeComponent()
	{    
    this.TestButton.Click += new System.EventHandler(this.TestButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void TestButton_Click(object sender, System.EventArgs e)
  {
    if (FormGridManager.Validate ())
    {
      NewObjectWithUndefinedEnumDataSource.SaveValues (false);
      ExistingObjectWithUndefinedEnumDataSource.SaveValues (false);
      SearchObjectWithUndefinedEnumDataSource.SaveValues (false);

      AreEqual (UndefinedEnum.Value1, MyFunction.NewObjectWithUndefinedEnum.UndefinedEnum);
      AreEqual (UndefinedEnum.Value1, MyFunction.ExistingObjectWithUndefinedEnum.UndefinedEnum);
      if (!Enum.IsDefined (typeof (UndefinedEnum), MyFunction.SearchObjectWithUndefinedEnum.UndefinedEnum))
        throw new TestFailureException ("SearchObjectWithUndefinedEnum.UndefinedEnum has an invalid value.");

      ExecuteNextStep ();
    }
  }

  private void AreEqual (UndefinedEnum expected, UndefinedEnum actual)
  {
    if (expected != actual)
      throw new TestFailureException (string.Format ("Actual value '{0}' does not match expected value '{1}'.", actual, expected));
  }
}
}
