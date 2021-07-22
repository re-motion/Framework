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
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Test.ExecutionEngine
{
  public partial class SecondControl : WxeUserControl
  {
    public static void Call (IWxePage page, WxeUserControl userControl, Control sender)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("userControl", userControl);
      ArgumentUtility.CheckNotNull ("sender", sender);

      ShowSecondUserControlFormFunction function;
      if ((page.IsReturningPostBack == false))
      {
        function = new ShowSecondUserControlFormFunction ();
        function.ExceptionHandler.SetCatchExceptionTypes (typeof (System.Exception));
        WxeUserControl actualUserControl = (WxeUserControl) page.FindControl (userControl.PermanentUniqueID);
        Assertion.IsNotNull (actualUserControl);
        actualUserControl.ExecuteFunction (function, sender, null);
        throw new System.Exception ("(Unreachable code)");
      }
      else
      {
        function = ((ShowSecondUserControlFormFunction) (page.ReturningFunction));
        if ((function.ExceptionHandler.Exception != null))
        {
          throw function.ExceptionHandler.Exception;
        }
      }
    }

    protected void ExecuteNextStep_Click (object sender, EventArgs e)
    {
      ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss");
      ExecuteNextStep ();
    }

    protected void Cancel_Click (object sender, EventArgs e)
    {
      throw new WxeUserCancelException ();
    }

    protected void ExecuteSecondUserControlButton_Click (object sender, EventArgs e)
    {
      throw new InvalidOperationException ("This event handler should never be called.");
    }

    protected void ExecuteThirdUserControlButton_Click (object sender, EventArgs e)
    {
      if (!WxePage.IsReturningPostBack)
      {
        ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss") + ": Executed";
        ExecuteFunction (new ShowThirdUserControlFormFunction (), (Control) sender, null);
      }
      else
      {
        ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss") + ": Returned";
      }
    }

    protected override void OnInitComplete (EventArgs e)
    {
      base.OnInitComplete (e);
      Page.RegisterRequiresControlState (this);
      ViewStateLabel.Text = "#";
      ControlStateLabel.Text = "#";
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (ControlStateValue == 0)
      {
        Assertion.IsTrue (IsPostBack);
        Assertion.IsFalse (IsUserControlPostBack);
      }
      else
      {
        Assertion.IsTrue (IsPostBack);
        Assertion.IsTrue (IsUserControlPostBack);
      }
      Assertion.IsTrue (CurrentFunction is ShowSecondUserControlFormFunction);
      Assertion.IsTrue (WxePage.CurrentFunction is ShowUserControlFormFunction);
      Assertion.IsTrue (WxePage.Variables != this.Variables);

      ViewStateValue++;
      ViewStateLabel.Text = ViewStateValue.ToString();

      ControlStateValue++;
      ControlStateLabel.Text = ControlStateValue.ToString();

      if (!IsUserControlPostBack)
      {
        Assertion.IsNull (SubControlWithState.ValueInViewState);
        SubControlWithState.ValueInViewState = 1.ToString ();

        Assertion.IsNull (SubControlWithState.ValueInControlState);
        SubControlWithState.ValueInControlState = 1.ToString ();
      }
      else
      {
        Assertion.IsNotNull (SubControlWithState.ValueInViewState);
        SubControlWithState.ValueInViewState = (int.Parse (SubControlWithState.ValueInViewState) + 1).ToString ();

        Assertion.IsNotNull (SubControlWithState.ValueInControlState);
        SubControlWithState.ValueInControlState = (int.Parse (SubControlWithState.ValueInControlState) + 1).ToString ();
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (!IsUserControlPostBack)
      {
        Assertion.IsTrue (string.IsNullOrEmpty (SubControlWithFormElement.Text));
        SubControlWithFormElement.Text = 1.ToString ();
      }
      else
      {

        Assertion.IsFalse (string.IsNullOrEmpty (SubControlWithFormElement.Text));
        SubControlWithFormElement.Text = (int.Parse (SubControlWithFormElement.Text) + 1).ToString ();
      }
    }

    protected override void LoadControlState (object savedState)
    {
      var controlState = (Tuple<object, int, Type>) savedState;
      base.LoadControlState (controlState.Item1);
      ControlStateValue = controlState.Item2;
      Assertion.IsTrue (controlState.Item3 == typeof (SecondControl), "Expected ControlState from 'SecondControl' but was '{0}'.", controlState.Item3.Name);
    }

    protected override object SaveControlState ()
    {
      return new Tuple<object, int, Type> (base.SaveControlState (), ControlStateValue, typeof (SecondControl));
    }

    protected override void LoadViewState (object savedState)
    {
      Assertion.IsNotNull (savedState, "Missing ViewState.");

      var  statePair =  (Tuple<object, Type>) savedState;
      base.LoadViewState (statePair.Item1);

      Assertion.IsTrue (statePair.Item2 == typeof (SecondControl), "Expected ViewState from 'SecondControl' but was '{0}'.", statePair.Item2.Name);
    }

    protected override object SaveViewState ()
    {
      return new Tuple<object, Type> (base.SaveViewState (), typeof (SecondControl));
    }

    private int ViewStateValue
    {
      get { return (int?) ViewState["Value"] ?? 0; }
      set { ViewState["Value"] = value; }
    }

    private int ControlStateValue { get; set; }
  }
}
