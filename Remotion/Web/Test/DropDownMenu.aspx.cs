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
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Test
{
  public partial class DropDownMenu : Page
  {
    protected void Page_Load (object sender, EventArgs e)
    {
    }

    protected override void CreateChildControls ()
    {
      base.CreateChildControls ();

      EditableEnabled.MenuItems.Clear();
      EditableEnabled.MenuItems.Add (
          new WebMenuItem (
              "First",
              "FirstCategory",
              "The first item",
              new IconInfo ("~/res/Remotion.Web/Image/Help.gif"),
              new IconInfo ("~/res/Remotion.Web/Image/Help.gif"),
              WebMenuItemStyle.Text,
              RequiredSelection.Any,
              false,
              null));
    }
  }
}
