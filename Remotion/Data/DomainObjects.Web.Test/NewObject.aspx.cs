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
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test
{
	public class NewObjectPage : WxePage
	{
    protected Remotion.Web.UI.Controls.HtmlHeadContents Htmlheadcontents1;

    protected ControlWithAllDataTypes ControlWithAllDataTypesControl;

    private NewObjectFunction MyFunction
    {
      get { return (NewObjectFunction) CurrentFunction; }
    }

		private void Page_Load(object sender, System.EventArgs e)
		{
      ControlWithAllDataTypesControl.ObjectWithAllDataTypes = MyFunction.ObjectWithAllDataTypes;
      if (!IsPostBack)
        MyFunction.ObjectWithAllDataTypes.ByteProperty = MyFunction.ObjectWithAllDataTypes.ByteProperty;
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
      this.Load += new System.EventHandler(this.Page_Load);

    }
    #endregion

  }
}
