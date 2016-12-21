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
using Remotion.Data.DomainObjects.Web.Test.WxeFunctions;
using Remotion.ObjectBinding;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test
{
public class SearchObjectPage : WxePage
{
  protected Remotion.Web.UI.Controls.FormGridManager SearchFormGridManager;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue StringPropertyValue;
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl FoundObjects;
  protected System.Web.UI.WebControls.Button SearchButton;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocList ResultList;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BytePropertyFromTextBox;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocTextValue BytePropertyToTextBox;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue EnumPropertyValue;
  protected System.Web.UI.HtmlControls.HtmlTable SearchFormGrid;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue DatePropertyFromValue;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue DatePropertyToValue;
  protected Remotion.ObjectBinding.Web.UI.Controls.BindableObjectDataSourceControl CurrentSearchObject;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue DateTimeFromValue;
  protected Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue BocDateTimeValue2;
  protected Remotion.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;

  private SearchFunction MyFunction 
  {
    get { return (SearchFunction) CurrentFunction; }
  }

  private void Page_Load(object sender, System.EventArgs e)
	{
    ResultList.Value = MyFunction.Result;

    CurrentSearchObject.BusinessObject = (IBusinessObject) MyFunction.SearchObject;
    CurrentSearchObject.LoadValues (IsPostBack);
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
    this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
    this.ResultList.EditableRowChangesSaved += new Remotion.ObjectBinding.Web.UI.Controls.BocListItemEventHandler (ResultList_EditableRowChangesSaved);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  private void SearchButton_Click(object sender, System.EventArgs e)
  {
    if (SearchFormGridManager.Validate ())
    {
      CurrentSearchObject.SaveValues (false);
      
      MyFunction.Requery ();
      ResultList.Value = MyFunction.Result;
      ResultList.LoadValue (false);
    }
  }

  private void ResultList_EditableRowChangesSaved (object sender, Remotion.ObjectBinding.Web.UI.Controls.BocListItemEventArgs e)
  {
    ClientTransactionScope.CurrentTransaction.Commit ();
  }
}
}
