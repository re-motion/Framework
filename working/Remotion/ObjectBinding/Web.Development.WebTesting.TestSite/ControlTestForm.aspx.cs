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
using Remotion.Web.ExecutionEngine;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
{
  public partial class ControlTestForm : WxePage
  {
    private IDataEditControl _dataEditControl;

    private new ControlTestFunction CurrentFunction
    {
      get { return (ControlTestFunction) base.CurrentFunction; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      LoadUserControl();
      LoadTestOutputUserControl();
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      PopulateDataSources();
      LoadValues (IsPostBack);
    }

    private void LoadUserControl ()
    {
      var control = LoadControl (CurrentFunction.UserControl);
      _dataEditControl = (IDataEditControl) control;
      _dataEditControl.ID = "DataEditControl";

      ControlPlaceHolder.Controls.Add (control);
    }

    private void LoadTestOutputUserControl ()
    {
      var testOutputControlPath = CurrentFunction.UserControl.Replace (".ascx", "TestOutput.ascx");
      var testOutputControl = LoadControl (testOutputControlPath);

      TestOutputControlPlaceHolder.Controls.Add (testOutputControl);
    }

    private void PopulateDataSources ()
    {
      if (_dataEditControl != null)
        _dataEditControl.BusinessObject = (IBusinessObject) CurrentFunction.Person;
    }

    private void LoadValues (bool interim)
    {
      if (_dataEditControl != null)
        _dataEditControl.LoadValues (interim);
    }

    private bool SaveValues (bool interim)
    {
      if (_dataEditControl == null)
        return true;

      return _dataEditControl.SaveValues (interim);
    }

    private bool ValidateDataSources ()
    {
      PrepareValidation();

      if (_dataEditControl == null)
        return true;

      return _dataEditControl.Validate();
    }
  }
}