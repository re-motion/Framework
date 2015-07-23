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
public class DesignTestMultilineTextValueForm : DesignTestWxeBasePage
{
  protected BindableObjectDataSourceControl CurrentObject;
  protected WebButton PostBackButton;
  protected BocMultilineTextValue BocMultilineTextValue1;
  protected BocMultilineTextValue BocMultilineTextValue36;
  protected BocMultilineTextValue BocMultilineTextValue37;
  protected BocMultilineTextValue BocMultilineTextValue2;
  protected BocMultilineTextValue BocMultilineTextValue38;
  protected BocMultilineTextValue BocMultilineTextValue39;
  protected BocMultilineTextValue BocMultilineTextValue3;
  protected BocMultilineTextValue BocMultilineTextValue40;
  protected BocMultilineTextValue BocMultilineTextValue41;
  protected BocMultilineTextValue BocMultilineTextValue4;
  protected BocMultilineTextValue BocMultilineTextValue42;
  protected BocMultilineTextValue BocMultilineTextValue43;
  protected BocMultilineTextValue BocMultilineTextValue17;
  protected BocMultilineTextValue BocMultilineTextValue18;
  protected BocMultilineTextValue BocMultilineTextValue5;
  protected BocMultilineTextValue BocMultilineTextValue44;
  protected BocMultilineTextValue BocMultilineTextValue45;
  protected BocMultilineTextValue BocMultilineTextValue6;
  protected BocMultilineTextValue BocMultilineTextValue46;
  protected BocMultilineTextValue BocMultilineTextValue47;
  protected BocMultilineTextValue BocMultilineTextValue7;
  protected BocMultilineTextValue BocMultilineTextValue48;
  protected BocMultilineTextValue BocMultilineTextValue49;
  protected BocMultilineTextValue BocMultilineTextValue51;
  protected BocMultilineTextValue BocMultilineTextValue50;
  protected BocMultilineTextValue BocMultilineTextValue52;
  protected BocMultilineTextValue BocMultilineTextValue8;
  protected BocMultilineTextValue BocMultilineTextValue19;
  protected BocMultilineTextValue BocMultilineTextValue9;
  protected BocMultilineTextValue BocMultilineTextValue10;
  protected BocMultilineTextValue BocMultilineTextValue11;
  protected BocMultilineTextValue BocMultilineTextValue12;
  protected BocMultilineTextValue BocMultilineTextValue22;
  protected BocMultilineTextValue BocMultilineTextValue23;
  protected BocMultilineTextValue BocMultilineTextValue13;
  protected BocMultilineTextValue BocMultilineTextValue14;
  protected BocMultilineTextValue BocMultilineTextValue15;
  protected BocMultilineTextValue BocMultilineTextValue16;
  protected BocMultilineTextValue BocMultilineTextValue20;
  protected BocMultilineTextValue BocMultilineTextValue21;
  protected BocMultilineTextValue BocMultilineTextValue24;
  protected BocMultilineTextValue BocMultilineTextValue25;
  protected BocMultilineTextValue BocMultilineTextValue26;
  protected BocMultilineTextValue BocMultilineTextValue27;
  protected BocMultilineTextValue BocMultilineTextValue28;
  protected BocMultilineTextValue BocMultilineTextValue29;
  protected BocMultilineTextValue BocMultilineTextValue30;
  protected BocMultilineTextValue BocMultilineTextValue31;
  protected BocMultilineTextValue BocMultilineTextValue32;
  protected BocMultilineTextValue BocMultilineTextValue33;
  protected BocMultilineTextValue BocMultilineTextValue34;
  protected BocMultilineTextValue BocMultilineTextValue35;
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
