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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.UI;

namespace Remotion.Web.Test.DoublePostBackHandling
{
  public partial class SmartForm : SmartPage
  {
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      var counter = string.IsNullOrEmpty (CounterTextBox.Text) ? 0 : int.Parse (CounterTextBox.Text);
      counter++;
      CounterTextBox.Text = counter.ToString();

      if (IsPostBack)
        Thread.Sleep (500);
    }

    protected void AsyncTextBox_OnTextChanged (object sender, EventArgs e)
    {
      StatusLabel.Text = CounterTextBox.Text + " Async TextBox: " + AsyncTextBox.Text + "<br/>" + StatusLabel.Text;
    }

    protected void SyncTextBox_OnTextChanged (object sender, EventArgs e)
    {
      StatusLabel.Text = CounterTextBox.Text + " Sync TextBox: " + SyncTextBox.Text + "<br/>" + StatusLabel.Text;
    }

    protected void AsyncSubmitButton_OnClick (object sender, EventArgs e)
    {
      StatusLabel.Text = CounterTextBox.Text + " Async Button<br/>" + StatusLabel.Text;
    }

    protected void SyncSubmitButton_OnClick (object sender, EventArgs e)
    {
      StatusLabel.Text = CounterTextBox.Text + " Sync Button<br/>" + StatusLabel.Text;
    }

    protected void AsyncLinkButton_OnClick (object sender, EventArgs e)
    {
      StatusLabel.Text = CounterTextBox.Text + " Async Link<br/>" + StatusLabel.Text;
    }

    protected void SyncLinkButton_OnClick (object sender, EventArgs e)
    {
      StatusLabel.Text = CounterTextBox.Text + " Sync Link<br/>" + StatusLabel.Text;
    }  }
}