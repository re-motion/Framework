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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.Data.DomainObjects.Web.Test.WxeFunctions;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test
{
  public partial class SecondControl : WxeUserControl
  {
    public static ClassWithAllDataTypes Call (IWxePage page, WxeUserControl userControl, Control sender, ITransactionMode transactionMode, ClassWithAllDataTypes inParameter)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("userControl", userControl);
      ArgumentUtility.CheckNotNull ("sender", sender);

      if (page.IsReturningPostBack == false)
      {
        var function = new ShowSecondUserControlFunction (transactionMode, inParameter);
        function.ExceptionHandler.SetCatchExceptionTypes (typeof (System.Exception));
        var actualUserControl = (WxeUserControl) page.FindControl (userControl.PermanentUniqueID);
        Assertion.IsNotNull (actualUserControl);
        actualUserControl.ExecuteFunction (function, sender, null);
        throw new Exception ("(Unreachable code)");
      }
      else
      {
        var function = (ShowSecondUserControlFunction) page.ReturningFunction;
        if (function.ExceptionHandler.Exception != null)
          throw function.ExceptionHandler.Exception;
        return function.ReturnedObjectWithAllDataTypes;
      }
    }

    public ShowSecondUserControlFunction MyFunction
    {
      get
      {
        return (ShowSecondUserControlFunction) CurrentFunction;
      }
    }

    protected new NameObjectCollection Variables
    {
      get { return MyFunction.Variables; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      LoadObjectFromFunction (CurrentUserControlStep.IsPostBack);
      RefreshText ();
    }

    private void LoadObjectFromFunction(bool interim)
    {
      ControlWithAllDataTypes.ObjectWithAllDataTypes = MyFunction.ObjectWithAllDataTypes;
      ControlWithAllDataTypes.LoadValues (interim);
    }

    private void RefreshText()
    {
      GuidInLabel.Text = GetObjectString (MyFunction.ObjectWithAllDataTypes);
      var outObject = MyFunction.ReturnedObjectWithAllDataTypes;
      GuidOutLabel.Text = GetObjectString (outObject);
      ClientTransactionLabel.Text = ClientTransaction.Current.ToString ();
    }

     private string GetObjectString (ClassWithAllDataTypes obj)
     {
       return obj == null ? "<null>" : obj.ID + " (byte property: " + obj.ByteProperty + ", State: " + obj.State + ")";
     }

    protected void ReturnButton_Click (object sender, EventArgs e)
    {
      ExecuteNextStep ();
    }

    protected void ControlWithAllDataTypes_Saved (object sender, EventArgs e)
    {
      MyFunction.ReturnedObjectWithAllDataTypes = ControlWithAllDataTypes.ObjectWithAllDataTypes;
      RefreshText ();
    }

    protected void NewObjectButton_Click (object sender, EventArgs e)
    {
      MyFunction.ObjectWithAllDataTypes = CreateSaveableNewObject();
      LoadObjectFromFunction (false);
      MyFunction.ReturnedObjectWithAllDataTypes = ControlWithAllDataTypes.ObjectWithAllDataTypes;
      RefreshText ();
    }

    private ClassWithAllDataTypes CreateSaveableNewObject()
    {
      var result = ClassWithAllDataTypes.NewObject ();
      result.FillMandatoryProperties();
      return result;
    }

    protected void RefreshButton_Click (object sender, EventArgs e)
    {
      RefreshText ();
    }
  }
}
