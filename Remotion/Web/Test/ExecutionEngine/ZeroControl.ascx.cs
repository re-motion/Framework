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
  public partial class ZeroControl : WxeUserControl
  {
    protected void ExecuteSecondUserControlButton_Click (object sender, EventArgs e)
    {
      ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss") + ": Executed";
      try
      {
        SecondControl.Call (WxePage, this, (Control) sender);
        ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss") + ": Returned";
      }
      catch (WxeUserCancelException)
      {
        ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss") + ": Canceled";
      }

      //if (!WxePage.IsReturningPostBack)
      //{
      //  ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss") + ": Executed";
      //  ExecuteFunction (new ShowSecondUserControlFormFunction(), (Control)sender, null);
      //}
      //else
      //{
      //  ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss") + ": Returned";
      //}
    }

    protected void ExecuteNextStep_Click (object sender, EventArgs e)
    {
      ControlLabel.Text = DateTime.Now.ToString ("HH:mm:ss");
      ExecuteNextStep ();
    }

    protected void Cancel_Click (object sender, EventArgs e)
    {
      throw new WxeUserCancelException();
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
        Assertion.IsFalse (IsPostBack);
        Assertion.IsFalse (IsUserControlPostBack);
      }
      else
      {
        Assertion.IsTrue (IsPostBack);
        Assertion.IsTrue (IsUserControlPostBack);
      }

      Assertion.IsTrue (WxePage.CurrentFunction == this.CurrentFunction);
      Assertion.IsTrue (CurrentFunction is ShowUserControlFormFunction);
      Assertion.IsTrue (WxePage.Variables == this.Variables);

      ViewStateValue++;
      ViewStateLabel.Text = ViewStateValue.ToString();

      ControlStateValue++;
      ControlStateLabel.Text = ControlStateValue.ToString ();

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
        SubControlWithFormElement.Text = (int.Parse (SubControlWithFormElement.Text) + 1).ToString();
      }
    }

    protected override void LoadControlState (object savedState)
    {
      var controlState = (object[]) savedState;
      base.LoadControlState (controlState[0]);
      ControlStateValue = (int) controlState[1];
      Assertion.IsTrue ((Type) controlState[2] == typeof (ZeroControl), "Expected ControlState from 'ZeroControl' but was '{0}'.", ((Type)controlState[2]).Name);
      HasLoaded = (bool) controlState[3];
      Assertion.IsTrue (((NonSerializeableObject)controlState[4]).Value == "TheValue");
    }

    protected override object SaveControlState ()
    {
      return new [] {base.SaveControlState(), ControlStateValue, typeof (ZeroControl), HasLoaded, new NonSerializeableObject ("TheValue")};
    }

    protected override void LoadViewState (object savedState)
    {
      Assertion.IsNotNull (savedState, "Missing ViewState.");

      var viewState = (Tuple<object, Type>) savedState;
      base.LoadViewState (viewState.Item1);

      Assertion.IsTrue (viewState.Item2 == typeof (ZeroControl), "Expected ViewState from 'ZeroControl' but was '{0}'.", viewState.Item2.Name);
    }

    protected override object SaveViewState ()
    {
      return new Tuple<object, Type> (base.SaveViewState (), typeof (ZeroControl));
    }

    private int ViewStateValue
    {
      get { return (int?) ViewState["Value"] ?? 0; }
      set { ViewState["Value"] = value; }
    }

    private int ControlStateValue { get; set; }
    private bool HasLoaded { get; set; }
  }
}
