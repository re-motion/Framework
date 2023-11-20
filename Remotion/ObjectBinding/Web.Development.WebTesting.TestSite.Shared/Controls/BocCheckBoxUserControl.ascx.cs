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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.Controls
{
  public partial class BocCheckBoxUserControl : DataEditUserControl, IDataControlWithValidationDispatcher
  {
    public BindableObjectDataSourceControlValidationResultDispatchingValidator DataSourceDispatchingValidator => CurrentObjectValidationResultDispatchingValidator;

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);
      AlwaysInvalidValidator.ServerValidate += (source, args) => args.IsValid = false;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender(e);
      SetTestOutput();
    }

    private void SetTestOutput ()
    {
      TestOutput.SetCurrentValueNormal(DeceasedField_Normal.Value.ToString());
      TestOutput.SetCurrentValueNoAutoPostBack(DeceasedField_NoAutoPostBack.Value.ToString());
    }

    private BocCheckBoxUserControlTestOutput TestOutput
    {
      get { return (BocCheckBoxUserControlTestOutput)((Layout)Page.Master).GetTestOutputControl(); }
    }
  }
}
