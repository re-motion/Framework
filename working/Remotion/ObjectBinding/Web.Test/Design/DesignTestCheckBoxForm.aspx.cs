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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace OBWTest.Design
{
public class DesignTestCheckBoxForm : DesignTestWxeBasePage
{
  protected BindableObjectDataSourceControl CurrentObject;
  protected WebButton PostBackButton;
  protected BocCheckBox BocCheckBox1;
  protected BocCheckBox BocCheckBox2;
  protected BocCheckBox BocCheckBox3;
  protected BocCheckBox BocCheckBox4;
  protected BocCheckBox BocCheckBox17;
  protected BocCheckBox BocCheckBox18;
  protected BocCheckBox BocCheckBox5;
  protected BocCheckBox BocCheckBox6;
  protected BocCheckBox BocCheckBox7;
  protected BocCheckBox BocCheckBox8;
  protected BocCheckBox BocCheckBox19;
  protected BocCheckBox BocCheckBox9;
  protected BocCheckBox BocCheckBox10;
  protected BocCheckBox BocCheckBox11;
  protected BocCheckBox BocCheckBox12;
  protected BocCheckBox BocCheckBox22;
  protected BocCheckBox BocCheckBox23;
  protected BocCheckBox BocCheckBox13;
  protected BocCheckBox BocCheckBox14;
  protected BocCheckBox BocCheckBox15;
  protected BocCheckBox BocCheckBox16;
  protected BocCheckBox BocCheckBox20;
  protected BocCheckBox BocCheckBox21;
  protected BocCheckBox BocCheckBox24;
  protected BocCheckBox BocCheckBox25;
  protected BocCheckBox BocCheckBox26;
  protected BocCheckBox BocCheckBox27;
  protected BocCheckBox BocCheckBox28;
  protected BocCheckBox BocCheckBox29;
  protected BocCheckBox BocCheckBox30;
  protected BocCheckBox BocCheckBox31;
  protected BocCheckBox BocCheckBox32;
  protected BocCheckBox BocCheckBox33;
  protected BocCheckBox BocCheckBox34;
  protected BocCheckBox BocCheckBox35;
  protected HtmlHeadContents HtmlHeadContents;

  private void Page_Load(object sender, EventArgs e)
	{
    Guid personID = new Guid(0,0,0,0,0,0,0,0,0,0,1);
    Person person = Person.GetObject (personID);
    Person partner = person.Partner;

    CurrentObject.BusinessObject = (IBusinessObject) person;
    CurrentObject.LoadValues (IsPostBack);
  }

	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);

    if (!IsPostBack)
      XmlReflectionBusinessObjectStorageProvider.Current.Reset();
  }

	#region Web Form Designer generated code
	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.EnableAbort = false;
    this.ShowAbortConfirmation = Remotion.Web.UI.ShowAbortConfirmation.Always;
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion
}

}
