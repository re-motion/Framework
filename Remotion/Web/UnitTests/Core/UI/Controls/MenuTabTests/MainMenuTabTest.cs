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
using Moq;
using NUnit.Framework;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls.MenuTabTests
{
  [TestFixture]
  public class MainMenuTabTest
  {
    private Mock<NavigationCommand> _mockNavigationCommand1;
    private Mock<NavigationCommand> _mockNavigationCommand2;
    private SubMenuTab _mockSubMenuTab1;
    private SubMenuTab _mockSubMenuTab2;
    private Mock<TabbedMenu> _mockTabbedMenu;
    private MainMenuTab _mainMenuTab;

    [SetUp]
    public void Setup ()
    {
      _mockTabbedMenu = new Mock<TabbedMenu>() { CallBase = true };

      _mockNavigationCommand1 = new Mock<NavigationCommand>() { CallBase = true };

      _mockNavigationCommand2 = new Mock<NavigationCommand>() { CallBase = true };

      _mainMenuTab = new MainMenuTab();
      _mainMenuTab.ItemID = "MainMenuTab";
      _mainMenuTab.Command.Type = CommandType.None;
      _mainMenuTab.OwnerControl = _mockTabbedMenu.Object;

      _mockSubMenuTab1 = CreateSubMenuTab("SubMenuTab1", _mockNavigationCommand1.Object);
      _mainMenuTab.SubMenuTabs.Add(_mockSubMenuTab1);

      _mockSubMenuTab2 = CreateSubMenuTab("SubMenuTab2", _mockNavigationCommand2.Object);
      _mainMenuTab.SubMenuTabs.Add(_mockSubMenuTab2);

      Mock.Get(_mockSubMenuTab1).Reset();
      Mock.Get(_mockSubMenuTab2).Reset();
      Mock.Get(_mockNavigationCommand1).Reset();
      Mock.Get(_mockNavigationCommand2).Reset();
    }

    private SubMenuTab CreateSubMenuTab (string itemID, NavigationCommand command)
    {
      var tab = new Mock<SubMenuTab>() { CallBase = true };
      tab.Object.ItemID = itemID;
      tab.Object.Command = command;

      return tab.Object;
    }
  }
}
