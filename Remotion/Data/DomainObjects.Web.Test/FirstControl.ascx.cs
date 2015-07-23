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
using System.Web.UI;
using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.Data.DomainObjects.Web.Test.WxeFunctions;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test
{
  public partial class FirstControl : WxeUserControl
  {
    public WxeUserControlTestPageFunction MyFunction
    {
      get
      {
        return (WxeUserControlTestPageFunction) CurrentPageStep.ParentFunction;
      }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      RefreshText();
    }

    private void RefreshText()
    {
      GuidInLabel.Text = GetObjectString (MyFunction.ObjectPassedIntoSecondControl);
      var outObject = MyFunction.ObjectReadFromSecondControl;
      GuidOutLabel.Text = GetObjectString (outObject);
      ClientTransactionLabel.Text = ClientTransaction.Current.ToString ();
    }

    private string GetObjectString(ClassWithAllDataTypes obj)
    {
      return obj == null ? "<null>" : obj.ID + " (byte property: " + obj.ByteProperty + ", State: " + obj.State + ")";
    }

    protected void NonTransactionUserControlStepButton_Click (object sender, EventArgs e)
    {
      MyFunction.ObjectReadFromSecondControl = SecondControl.Call (WxePage, this, (Control)sender, WxeTransactionMode.None, MyFunction.ObjectPassedIntoSecondControl);
      RefreshText ();
    }

    protected void RootTransactionUserControlStepButton_Click (object sender, EventArgs e)
    {
      MyFunction.ObjectReadFromSecondControl = SecondControl.Call (WxePage, this, (Control) sender, WxeTransactionMode.CreateRoot, MyFunction.ObjectPassedIntoSecondControl);
      RefreshText ();
    }

    protected void SubTransactionUserControlStepButton_Click (object sender, EventArgs e)
    {
      MyFunction.ObjectReadFromSecondControl = SecondControl.Call (WxePage, this, (Control) sender, WxeTransactionMode.CreateChildIfParent, MyFunction.ObjectPassedIntoSecondControl);
      RefreshText ();
    }

    protected void SaveButton_Click (object sender, EventArgs e)
    {
      ClientTransaction.Current.Commit ();
      RefreshText ();
    }

    protected void ReturnButton_Click (object sender, EventArgs e)
    {
      ExecuteNextStep ();
    }
  }
}
