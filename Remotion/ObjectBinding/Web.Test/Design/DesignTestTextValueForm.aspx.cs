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
public class DesignTestTextValueForm : DesignTestWxeBasePage
{
  protected BindableObjectDataSourceControl CurrentObject;
  protected WebButton PostBackButton;
  protected BocTextValue BocTextValue1;
  protected BocTextValue Boctextvalue36;
  protected BocTextValue Boctextvalue37;
  protected BocTextValue BocTextValue2;
  protected BocTextValue Boctextvalue38;
  protected BocTextValue Boctextvalue39;
  protected BocTextValue BocTextValue3;
  protected BocTextValue Boctextvalue40;
  protected BocTextValue Boctextvalue41;
  protected BocTextValue BocTextValue4;
  protected BocTextValue Boctextvalue42;
  protected BocTextValue Boctextvalue43;
  protected BocTextValue BocTextValue17;
  protected BocTextValue BocTextValue18;
  protected BocTextValue BocTextValue5;
  protected BocTextValue Boctextvalue44;
  protected BocTextValue Boctextvalue45;
  protected BocTextValue BocTextValue6;
  protected BocTextValue Boctextvalue46;
  protected BocTextValue Boctextvalue47;
  protected BocTextValue BocTextValue7;
  protected BocTextValue Boctextvalue48;
  protected BocTextValue Boctextvalue49;
  protected BocTextValue Boctextvalue51;
  protected BocTextValue Boctextvalue50;
  protected BocTextValue Boctextvalue52;
  protected BocTextValue BocTextValue8;
  protected BocTextValue BocTextValue19;
  protected BocTextValue BocTextValue9;
  protected BocTextValue BocTextValue10;
  protected BocTextValue BocTextValue11;
  protected BocTextValue BocTextValue12;
  protected BocTextValue BocTextValue22;
  protected BocTextValue BocTextValue23;
  protected BocTextValue BocTextValue13;
  protected BocTextValue BocTextValue14;
  protected BocTextValue BocTextValue15;
  protected BocTextValue BocTextValue16;
  protected BocTextValue BocTextValue20;
  protected BocTextValue BocTextValue21;
  protected BocTextValue BocTextValue24;
  protected BocTextValue BocTextValue25;
  protected BocTextValue BocTextValue26;
  protected BocTextValue BocTextValue27;
  protected BocTextValue BocTextValue28;
  protected BocTextValue BocTextValue29;
  protected BocTextValue BocTextValue30;
  protected BocTextValue BocTextValue31;
  protected BocTextValue BocTextValue32;
  protected BocTextValue BocTextValue33;
  protected BocTextValue BocTextValue34;
  protected BocTextValue BocTextValue35;
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
