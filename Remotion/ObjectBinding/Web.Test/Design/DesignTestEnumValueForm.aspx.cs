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
public class DesignTestEnumValueForm : DesignTestWxeBasePage
{
  protected BindableObjectDataSourceControl CurrentObject;
  protected WebButton PostBackButton;
  protected BocEnumValue BocEnumValue1;
  protected BocEnumValue BocEnumValue2;
  protected BocEnumValue BocEnumValue3;
  protected BocEnumValue BocEnumValue4;
  protected BocEnumValue Bocenumvalue5;
  protected BocEnumValue Bocenumvalue6;
  protected BocEnumValue Bocenumvalue7;
  protected BocEnumValue Bocenumvalue8;
  protected BocEnumValue Bocenumvalue9;
  protected BocEnumValue Bocenumvalue10;
  protected BocEnumValue Bocenumvalue11;
  protected BocEnumValue Bocenumvalue12;
  protected BocEnumValue Bocenumvalue13;
  protected BocEnumValue Bocenumvalue14;
  protected BocEnumValue BocEnumValue36;
  protected BocEnumValue BocEnumValue37;
  protected BocEnumValue BocEnumValue38;
  protected BocEnumValue BocEnumValue39;
  protected BocEnumValue BocEnumValue40;
  protected BocEnumValue BocEnumValue41;
  protected BocEnumValue BocEnumValue42;
  protected BocEnumValue BocEnumValue43;
  protected BocEnumValue BocEnumValue17;
  protected BocEnumValue BocEnumValue18;
  protected BocEnumValue BocEnumValue44;
  protected BocEnumValue BocEnumValue45;
  protected BocEnumValue BocEnumValue46;
  protected BocEnumValue BocEnumValue47;
  protected BocEnumValue BocEnumValue48;
  protected BocEnumValue BocEnumValue49;
  protected BocEnumValue BocEnumValue51;
  protected BocEnumValue BocEnumValue50;
  protected BocEnumValue BocEnumValue52;
  protected BocEnumValue BocEnumValue19;
  protected BocEnumValue BocEnumValue22;
  protected BocEnumValue BocEnumValue23;
  protected BocEnumValue BocEnumValue15;
  protected BocEnumValue BocEnumValue16;
  protected BocEnumValue BocEnumValue20;
  protected BocEnumValue BocEnumValue21;
  protected BocEnumValue BocEnumValue24;
  protected BocEnumValue BocEnumValue25;
  protected BocEnumValue BocEnumValue26;
  protected BocEnumValue BocEnumValue27;
  protected BocEnumValue BocEnumValue28;
  protected BocEnumValue BocEnumValue29;
  protected BocEnumValue BocEnumValue30;
  protected BocEnumValue BocEnumValue31;
  protected BocEnumValue BocEnumValue32;
  protected BocEnumValue BocEnumValue33;
  protected BocEnumValue BocEnumValue34;
  protected BocEnumValue BocEnumValue35;
  protected BocEnumValue Bocenumvalue53;
  protected BocEnumValue Bocenumvalue54;
  protected BocEnumValue Bocenumvalue55;
  protected BocEnumValue Bocenumvalue56;
  protected BocEnumValue Bocenumvalue57;
  protected BocEnumValue Bocenumvalue58;
  protected BocEnumValue Bocenumvalue59;
  protected BocEnumValue Bocenumvalue60;
  protected BocEnumValue Bocenumvalue61;
  protected BocEnumValue Bocenumvalue62;
  protected BocEnumValue Bocenumvalue63;
  protected BocEnumValue Bocenumvalue64;
  protected BocEnumValue Bocenumvalue65;
  protected BocEnumValue Bocenumvalue66;
  protected HtmlHeadContents HtmlHeadContents;

  private void Page_Load (object sender, EventArgs e)
	{
    Guid personID = new Guid(0,0,0,0,0,0,0,0,0,0,1);
    Person person = Person.GetObject(personID);
    Person partner = person.Partner;

    CurrentObject.BusinessObject = (IBusinessObject)person;
    CurrentObject.LoadValues(IsPostBack);
  }

	override protected void OnInit (EventArgs e)
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
	private void InitializeComponent ()
	{
    this.EnableAbort = false;
    this.ShowAbortConfirmation = Remotion.Web.UI.ShowAbortConfirmation.Always;
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion
}

}
