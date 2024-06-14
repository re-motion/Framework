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
using Remotion.Web.Compilation;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.UI
{
  [FileLevelControlBuilder(typeof(CodeProcessingUserControlBuilder))]
  public partial class SecurityManagerNavigationTabs : UserControl
  {
    protected override void OnPreRender (EventArgs e)
    {
      var mainMenuTab = (MainMenuTab)TabbedMenu.Tabs.FindMandatory("OrganizationalStructureTab");
      mainMenuTab.Text = GlobalResourcesHelper.GetText(GlobalResources.OrganizationalStructure);
      mainMenuTab.SubMenuTabs.FindMandatory("UserTab").Text = GlobalResourcesHelper.GetText(GlobalResources.User);
      mainMenuTab.SubMenuTabs.FindMandatory("GroupTab").Text = GlobalResourcesHelper.GetText(GlobalResources.Group);
      mainMenuTab.SubMenuTabs.FindMandatory("TenantTab").Text = GlobalResourcesHelper.GetText(GlobalResources.Tenant);
      mainMenuTab.SubMenuTabs.FindMandatory("PositionTab").Text = GlobalResourcesHelper.GetText(GlobalResources.Position);
      mainMenuTab.SubMenuTabs.FindMandatory("GroupTypeTab").Text = GlobalResourcesHelper.GetText(GlobalResources.GroupType);

      var accessControlTab = (MainMenuTab)TabbedMenu.Tabs.FindMandatory("AccessControlTab");
      accessControlTab.Text = GlobalResourcesHelper.GetText(GlobalResources.AccessControl);
      accessControlTab.SubMenuTabs.FindMandatory("SecurableClassDefinitionTab").Text =
          GlobalResourcesHelper.GetText(GlobalResources.SecurableClassDefinition);

      base.OnPreRender(e);
    }
  }
}
