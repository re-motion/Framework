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
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class TabbedMenuControlObjectTest : IntegrationTest
  {
    [Test]
    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<TabbedMenuSelector, TabbedMenuControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<TabbedMenuSelector, TabbedMenuControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<TabbedMenuSelector, TabbedMenuControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<TabbedMenuSelector, TabbedMenuControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<TabbedMenuSelector, TabbedMenuControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<TabbedMenuSelector, TabbedMenuControlObject> testAction)
    {
      testAction (Helper, e => e.TabbedMenus(), "tabbedMenu");
    }

    [Test]
    public void TestIsDisabled_TabsDisabled ()
    {
      var home = Start();

      var control = home.TabbedMenus().GetByLocalID ("MyTabbedMenu");

      var definitions = control.GetItemDefinitions();
      Assert.That (definitions[3].IsDisabled, Is.False);
      Assert.That (definitions[5].IsDisabled, Is.True);
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.TabbedMenus().GetByLocalID ("MyTabbedMenu");
      Assert.That (
          () => control.SelectItem().WithDisplayText ("DisabledCommandTabTitle"),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("Command").Message));
      Assert.That (
          () => control.SelectItem().WithDisplayTextContains ("DisabledCommandTabTitle"),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("Command").Message));
      Assert.That (
          () => control.SelectItem().WithIndex (6),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("Command").Message));
      Assert.That (
          () => control.SelectItem().WithHtmlID ("body_MyTabbedMenu_MainMenuTabStrip_DisabledCommandTab"),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("Command").Message));
      Assert.That (
          () => control.SelectItem().WithItemID ("DisabledCommandTab"),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("Command").Message));
      Assert.That (
          () => control.SelectItem ("DisabledCommandTab"),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("Command").Message));
    }

    [Test]
    public void TestStatusText ()
    {
      var home = Start();

      var tabbedMenu = home.TabbedMenus().Single();
      Assert.That (tabbedMenu.GetStatusText(), Is.EqualTo ("MyStatusText"));
    }

    [Test]
    public void TestGetItemDefinitions ()
    {
      var home = Start();

      var tabbedMenu = home.TabbedMenus().GetByLocalID ("MyTabbedMenu");

      var items = tabbedMenu.GetItemDefinitions();
      Assert.That (items.Count, Is.EqualTo (6));
      
      Assert.That (items[0].ItemID, Is.EqualTo ("EventCommandTab"));
      Assert.That (items[0].Index, Is.EqualTo (1));
      Assert.That (items[0].Text, Is.EqualTo ("EventCommandTabTitle"));
      Assert.That (items[0].IsDisabled, Is.False);

      Assert.That (items[3].IsDisabled, Is.False);

      Assert.That (items[4].ItemID, Is.EqualTo ("TabWithSubMenu"));
      Assert.That (items[4].Index, Is.EqualTo (5));
      Assert.That (items[4].Text, Is.EqualTo ("TabWithSubMenuTitle"));
      Assert.That (items[4].IsDisabled, Is.False);
    }

    [Test]
    public void TestSelectMenuItem ()
    {
      var home = Start();

      var tabbedMenu = home.TabbedMenus().Single();

      tabbedMenu.SelectItem ("EventCommandTab");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));

      tabbedMenu.SelectItem().WithIndex (1);
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));

      tabbedMenu.SelectItem().WithHtmlID ("body_MyTabbedMenu_MainMenuTabStrip_EventCommandTab");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));

      tabbedMenu.SelectItem().WithDisplayText ("EventCommandTabTitle");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));

      tabbedMenu.SelectItem().WithDisplayTextContains ("ntCom");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));
    }

    [Test]
    public void TestSelectMenuItemCommand ()
    {
      var home = Start();

      var tabbedMenu = home.TabbedMenus().Single();
      var completionDetection = new CompletionDetectionStrategyTestHelper (tabbedMenu);

      tabbedMenu.SelectItem ("EventCommandTab");
      Assert.That (completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));

      tabbedMenu.SelectItem ("HrefCommandTab");
      Assert.That (completionDetection.GetAndReset(), Is.TypeOf<WxeResetCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);

      tabbedMenu.SelectItem ("EventCommandTab");
      Assert.That (completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));

      tabbedMenu.SelectItem ("WxeFunctionCommandTab");
      Assert.That (completionDetection.GetAndReset(), Is.TypeOf<WxeResetCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);
    }

    [Test]
    public void TestGetSubItemDefinitions ()
    {
      var home = Start();

      var tabbedMenu = home.TabbedMenus().GetByLocalID ("MyTabbedMenu");

      var subMenuItems = tabbedMenu.SubMenu.GetItemDefinitions();
      Assert.That (subMenuItems, Is.Empty);

      tabbedMenu.SelectItem ("TabWithSubMenu");
      subMenuItems = tabbedMenu.SubMenu.GetItemDefinitions();

      Assert.That (subMenuItems.Count, Is.EqualTo (3));

      Assert.That (subMenuItems[0].ItemID, Is.EqualTo ("SubMenuTab1"));
      Assert.That (subMenuItems[0].Index, Is.EqualTo (1));
      Assert.That (subMenuItems[0].Text, Is.EqualTo ("SubMenuTab1Title"));
      Assert.That (subMenuItems[0].IsDisabled, Is.False);

      Assert.That (subMenuItems[2].ItemID, Is.EqualTo ("SubMenuTab3"));
      Assert.That (subMenuItems[2].Index, Is.EqualTo (3));
      Assert.That (subMenuItems[2].Text, Is.EqualTo ("SubMenuTab3Title"));
      Assert.That (subMenuItems[2].IsDisabled, Is.False);
    }

    [Test]
    public void TestSelectSubMenuItem ()
    {
      var home = Start();

      var tabbedMenu = home.TabbedMenus().Single();
      tabbedMenu.SelectItem ("TabWithSubMenu");

      tabbedMenu.SubMenu.SelectItem ("SubMenuTab1");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("SubMenuTab1|Event"));

      tabbedMenu.SubMenu.SelectItem().WithIndex (3);
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);

      tabbedMenu.SubMenu.SelectItem().WithHtmlID ("body_MyTabbedMenu_SubMenuTabStrip_SubMenuTab1");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("SubMenuTab1|Event"));

      tabbedMenu.SubMenu.SelectItem().WithDisplayText ("SubMenuTab2Title");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("SubMenuTab2|Event"));

      tabbedMenu.SubMenu.SelectItem().WithDisplayTextContains ("nuTab1");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("SubMenuTab1|Event"));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("TabbedMenuTest.wxe");
    }
  }
}