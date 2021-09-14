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

namespace Remotion.ObjectBinding.Web.Test.Shared.Design
{
public partial class DesignTestBooleanValueForm : DesignTestWxeBasePage
{
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
    EnableAbort = false;
    ShowAbortConfirmation = Remotion.Web.UI.ShowAbortConfirmation.Always;
    Load += new EventHandler(Page_Load);
    base.OnInit(e);

    if (!IsPostBack)
      XmlReflectionBusinessObjectStorageProvider.Current.Reset();
  }
}
}
