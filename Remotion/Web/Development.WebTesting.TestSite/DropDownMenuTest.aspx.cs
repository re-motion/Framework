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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class DropDownMenuTest : WxePage
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      MyDropDownMenu.EventCommandClick += MyDropDownMenuOnCommandClick;
      MyDropDownMenu.WxeFunctionCommandClick += MyDropDownMenuOnCommandClick;

      MyDropDownMenu_Delayed.SetLoadMenuItemStatus(
          "DropDownMenuWebService.asmx",
          nameof(DropDownMenuWebService.GetMenuItemStatusWithDelay),
          new Dictionary<string, string> { { "arguments", 500.ToString() } });

      MyDropDownMenu_DelayedLongerThanTimeout.SetLoadMenuItemStatus(
          "DropDownMenuWebService.asmx",
          nameof(DropDownMenuWebService.GetMenuItemStatusWithDelay),
          new Dictionary<string, string> { { "arguments", (31 * 1000).ToString() } });

      MyDropDownMenu_Error.SetLoadMenuItemStatus(
          "DropDownMenuWebService.asmx",
          nameof(DropDownMenuWebService.GetMenuItemStatusWithError),
          new Dictionary<string, string>());
    }

    private void MyDropDownMenuOnCommandClick (object sender, WebMenuItemClickEventArgs webMenuItemClickEventArgs)
    {
      ((Layout) Master).SetTestOutput(webMenuItemClickEventArgs.Item.ItemID + "|" + webMenuItemClickEventArgs.Command.Type);
    }
  }
}