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
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Test.Shared
{
/// <summary>
/// Summary description for RepeaterTest.
/// </summary>
public partial class RepeaterTest : SmartPage
{
  private void Page_Load (object sender, EventArgs e)
  {
    Guid personID = new Guid(0,0,0,0,0,0,0,0,0,0,1);
    Person person = Person.GetObject(personID);

    CurrentObject.BusinessObject = (IBusinessObject)person;
    CurrentObject.LoadValues(IsPostBack);
  }

  override protected void OnInit (EventArgs e)
  {
    this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
    this.Load += new System.EventHandler(this.Page_Load);
    base.OnInit(e);
  }

  private void SaveButton_Click (object sender, EventArgs e)
  {
    PrepareValidation();
    bool isValid = CurrentObject.Validate();
    if (isValid)
      CurrentObject.SaveValues(false);
  }
}
}
