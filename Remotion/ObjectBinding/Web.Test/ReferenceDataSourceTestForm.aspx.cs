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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding;

namespace OBWTest
{
  public partial class ReferenceDataSourceTestForm : TestBasePage<ReferenceDataSourceTestFunction>
  {
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      LevelOneDataSource.BusinessObject =(IBusinessObject) CurrentFunction.RootObject;
      LevelOneDataSource.LoadValues (IsPostBack);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      // Normally only in OnUnload. Call here only to provide diagnostic output
      LevelOneDataSource.SaveValues (true);

      var hasLevelOneInstance = CurrentFunction.RootObject != null;
      var hasLevelTwoInstance = hasLevelOneInstance && CurrentFunction.RootObject.ReferenceValue != null;
      var hasLevelThreeInstance = hasLevelTwoInstance && CurrentFunction.RootObject.ReferenceValue.ReferenceValue != null;

      Stack.Text = "";
      Stack.Text += string.Format ("LevelOne: HasInstance = {0}, IsDirty = {1}<br/>", hasLevelOneInstance, null);
      Stack.Text += string.Format ("LevelTwo (LevelOne.ReferenceValue): HasInstance = {0}, IsDirty = {1}<br/>", hasLevelTwoInstance, LevelTwoDataSource.IsDirty);
      Stack.Text += string.Format ("LevelThree (LevelOne.ReferenceValue.ReferenceValue): HasInstance = {0}, IsDirty = {1}<br/>", hasLevelThreeInstance, LevelThreeDataSource.IsDirty);
    }

    protected override object SaveControlState ()
    {
      if (LevelOneDataSource != null)
        LevelOneDataSource.SaveValues (true);

      return base.SaveControlState ();
    }

    protected void ValidateButton_OnClick (object sender, EventArgs e)
    {
      ValidateDataSource ();
    }

    private bool ValidateDataSource ()
    {
      PrepareValidation ();

     return LevelOneDataSource.Validate();
    }

    protected void SaveButton_OnClick (object sender, EventArgs e)
    {
      if (ValidateDataSource ())
        LevelOneDataSource.SaveValues (false);
    }

    protected void LevelThreeIntValueFieldCustomValidator_OnServerValidate (object sender, ServerValidateEventArgs args)
    {
      if (args.Value == "0")
        args.IsValid = false;
    }
  }
}