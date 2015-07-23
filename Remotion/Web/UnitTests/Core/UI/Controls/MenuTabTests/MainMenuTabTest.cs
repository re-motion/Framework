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
using NUnit.Framework;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls.MenuTabTests
{
  [TestFixture]
  public class MainMenuTabTest
  {
    private MockRepository _mocks;
    private NavigationCommand _mockNavigationCommand1;
    private NavigationCommand _mockNavigationCommand2;
    private SubMenuTab _mockSubMenuTab1;
    private SubMenuTab _mockSubMenuTab2;
    private TabbedMenu _mockTabbedMenu;
    private MainMenuTab _mainMenuTab;

    [SetUp]
    public void Setup ()
    {
      _mocks = new MockRepository();

      _mockTabbedMenu = _mocks.PartialMock<TabbedMenu>();

      _mockNavigationCommand1 = _mocks.PartialMock<NavigationCommand>();
      _mocks.Replay (_mockNavigationCommand1);

      _mockNavigationCommand2 = _mocks.PartialMock<NavigationCommand>();
      _mocks.Replay (_mockNavigationCommand2);

      _mainMenuTab = new MainMenuTab();
      _mainMenuTab.ItemID = "MainMenuTab";
      _mainMenuTab.Command.Type = CommandType.None;
      _mainMenuTab.OwnerControl = _mockTabbedMenu;

      _mockSubMenuTab1 = CreateSubMenuTab ("SubMenuTab1", _mockNavigationCommand1);
      _mainMenuTab.SubMenuTabs.Add (_mockSubMenuTab1);

      _mockSubMenuTab2 = CreateSubMenuTab ("SubMenuTab2", _mockNavigationCommand2);
      _mainMenuTab.SubMenuTabs.Add (_mockSubMenuTab2);

      _mocks.BackToRecord (_mockSubMenuTab1);
      _mocks.BackToRecord (_mockSubMenuTab2);
      _mocks.BackToRecord (_mockNavigationCommand1);
      _mocks.BackToRecord (_mockNavigationCommand2);
    }

    private SubMenuTab CreateSubMenuTab (string itemID, NavigationCommand command)
    {
      SubMenuTab tab = _mocks.PartialMock<SubMenuTab>();
      _mocks.Replay (tab);
      tab.ItemID = itemID;
      tab.Command = command;

      return tab;
    }
  }
}
