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
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class DropDownMenuControlObjectTest : IntegrationTest
  {
    // Note: the <see cref="T:DropDownMenu.Mode"/>=<see cref="T:MenuMode.ContextMenu"/> option is tested indirectly by the BocTreeViewControlObjectTest.

    [Test]
    [RemotionTestCaseSource (typeof (DisabledTestCaseFactory<DropDownMenuSelector, DropDownMenuControlObject>))]
    public void GenericTests (GenericSelectorTestAction<DropDownMenuSelector, DropDownMenuControlObject> testAction)
    {
      testAction (Helper, e => e.DropDownMenus(), "dropDownMenu");
    }

    [Test]
    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<DropDownMenuSelector, DropDownMenuControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<DropDownMenuSelector, DropDownMenuControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<DropDownMenuSelector, DropDownMenuControlObject>))]
    [RemotionTestCaseSource (typeof (TextContentControlSelectorTestCaseFactory<DropDownMenuSelector, DropDownMenuControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<DropDownMenuSelector, DropDownMenuControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<DropDownMenuSelector, DropDownMenuControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<DropDownMenuSelector, DropDownMenuControlObject> testAction)
    {
      testAction (Helper, e => e.DropDownMenus(), "dropDownMenu");
    }

    [Test]
    public void TestGetItemDefinitions ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID ("MyDropDownMenu");

      var items = dropDownMenu.GetItemDefinitions();
      Assert.That (items.Count, Is.EqualTo (5));

      Assert.That (items[0].ItemID, Is.EqualTo ("ItemID1"));
      Assert.That (items[0].Index, Is.EqualTo (1));
      Assert.That (items[0].Text, Is.EqualTo ("EventItem"));
      Assert.That (items[0].IsEnabled, Is.True);

      Assert.That (items[2].IsEnabled, Is.False);

      Assert.That (items[4].ItemID, Is.EqualTo ("ItemID5"));
      Assert.That (items[4].Index, Is.EqualTo (5));
      Assert.That (items[4].Text, Is.EqualTo (""));
      Assert.That (items[4].IsEnabled, Is.True);
    }

    [Test]
    public void TestClickItem ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID ("MyDropDownMenu");

      dropDownMenu.SelectItem ("ItemID5");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID5|Event"));

      dropDownMenu.SelectItem().WithIndex (2);
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);

      dropDownMenu.SelectItem().WithHtmlID ("body_MyDropDownMenu_3");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID4|WxeFunction"));

      dropDownMenu.SelectItem().WithDisplayText ("EventItem");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID1|Event"));

      dropDownMenu.SelectItem().WithDisplayTextContains ("xeFun");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID4|WxeFunction"));
    }

    [Test]
    public void TestDropDownSelectItem_AfterGetItemDefinitions ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID ("MyDropDownMenu");

      dropDownMenu.GetItemDefinitions();
      Assert.That (() => dropDownMenu.SelectItem ("ItemID5"), Throws.Nothing);
    }

    [Test]
    public void TestDropDownMenuControlObject_IsOpen ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID ("MyDropDownMenu");
      Assert.That (dropDownMenu.IsOpen(), Is.False);
    }

    [Test]
    public void TestDropDownMenuControlObject_OpenDropDownMenu ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID ("MyDropDownMenu");

      dropDownMenu.Open();
      Assert.That (dropDownMenu.IsOpen(), Is.True);

      //Open it a second time to ensure it stays open
      dropDownMenu.Open();
      Assert.That (dropDownMenu.IsOpen(), Is.True);
    }

    [Test]
    public void TestDropDownMenuControlObject_CloseDropDownMenu ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID ("MyDropDownMenu");

      dropDownMenu.Open();

      dropDownMenu.Close();
      Assert.That (dropDownMenu.IsOpen(), Is.False);

      //Close it a second time to ensure it stays closed
      dropDownMenu.Close();
      Assert.That (dropDownMenu.IsOpen(), Is.False);
    }

    [Test]
    public void TestDropDownMenuControlObject_IsOpen_OnOtherDropDownMenu ()
    {
      var home = Start();

      var myDropDownMenu = home.DropDownMenus().GetByLocalID ("MyDropDownMenu");
      var myDropDownMenu2 = home.DropDownMenus().GetByLocalID ("MyDropDownMenu2");

      myDropDownMenu.Open();
      Assert.That (myDropDownMenu2.IsOpen(), Is.False);
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("DropDownMenuTest.wxe");
    }
  }
}