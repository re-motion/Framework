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
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Web;
using Remotion.Web.Configuration;
using Remotion.Web.UI.Controls;

namespace OBWTest.UI
{
  /// <summary>
  ///		Summary description for NavigationTabs.
  /// </summary>
  public class NavigationTabs : UserControl
  {
    private const string c_mainContentScrollableKey = "MainContentScrollableKey";

    protected TabbedMenu TabbedMenu;
    protected CheckBox MainContentScrollableCheckBox;

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender(e);

      string mode = Global.PreferQuirksModeRendering ? "Quirks" : "Standard";
      string theme = Global.PreferQuirksModeRendering ? "" : SafeServiceLocator.Current.GetInstance<ResourceTheme>().Name;
      TabbedMenu.StatusText = WebString.CreateFromText(mode + " " + theme);

      var mainContentScrollable = (bool?)Session[c_mainContentScrollableKey] ?? false;
      MainContentScrollableCheckBox.Checked = mainContentScrollable;
      if (mainContentScrollable)
        (Page.Master as StandardMode)?.SetPageContentScrollable();
    }

    protected void MainContentScrollableCheckBox_OnCheckedChanged (object sender, EventArgs e)
    {
      var mainContentScrollable = MainContentScrollableCheckBox.Checked;
      Session[c_mainContentScrollableKey] = mainContentScrollable;
    }
  }
}
