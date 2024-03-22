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
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace OBWTest.IndividualControlTests
{

  public partial class BocLiteralUserControl : BaseUserControl
  {
    protected override void RegisterEventHandlers ()
    {
      base.RegisterEventHandlers();

      this.CVTestSetNullButton.Click += new EventHandler(this.CVTestSetNullButton_Click);
      this.CVTestSetNewValueButton.Click += new EventHandler(this.CVTestSetNewValueButton_Click);
    }

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    public override BindableObjectDataSourceControlValidationResultDispatchingValidator DataSourceValidationResultDispatchingValidator
    {
      get { return CurrentObjectValidationResultDispatchingValidator; }
    }

    override protected void OnLoad (EventArgs e)
    {
      base.OnLoad(e);

      Person person = (Person)CurrentObject.BusinessObject;

      UnboundCVField.LoadUnboundValue(person.CVStringLiteral, IsPostBack);
    }

    override protected void OnPreRender (EventArgs e)
    {
      base.OnPreRender(e);
    }

    private void CVTestSetNullButton_Click (object sender, EventArgs e)
    {
      CVField.Value = null;
    }

    private void CVTestSetNewValueButton_Click (object sender, EventArgs e)
    {
      CVField.Value = "Foo<br/>Bar";
    }
  }

}
