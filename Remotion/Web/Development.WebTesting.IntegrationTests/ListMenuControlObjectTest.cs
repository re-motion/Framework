﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ListMenuControlObjectTest : IntegrationTest
  {
    [Test]
    [RemotionTestCaseSource (typeof (DisabledTestCaseFactory<ListMenuSelector, ListMenuControlObject>))]
    public void GenericTests (GenericSelectorTestAction<ListMenuSelector, ListMenuControlObject> testAction)
    {
      testAction (Helper, e => e.ListMenus(), "listMenu");
    }

    [Test]
    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<ListMenuSelector, ListMenuControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<ListMenuSelector, ListMenuControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<ListMenuSelector, ListMenuControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<ListMenuSelector, ListMenuControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<ListMenuSelector, ListMenuControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<ListMenuSelector, ListMenuControlObject> testAction)
    {
      testAction (Helper, e => e.ListMenus(), "listMenu");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var disabledControl = home.ListMenus().GetByLocalID ("MyListMenu_Disabled");
      Assert.That (disabledControl.IsDisabled(), Is.True);
      Assert.That (() => disabledControl.SelectItem().WithDisplayText ("EventItem"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException().Message));
      Assert.That (() => disabledControl.SelectItem().WithDisplayTextContains ("Href"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException().Message));
      Assert.That (() => disabledControl.SelectItem().WithIndex (1), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException().Message));
      Assert.That (() => disabledControl.SelectItem().WithHtmlID ("body_MyListMenu_Disabled_3"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException().Message));
      Assert.That (() => disabledControl.SelectItem().WithItemID ("ItemID4"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException().Message));
      Assert.That (() => disabledControl.SelectItem ("ItemID4"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException().Message));
    }

    [Test]
    public void TestListMenuItemIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var enabledControl = home.ListMenus().GetByLocalID ("MyListMenu");
      Assert.That (() => enabledControl.SelectItem ("ItemID3"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("Command").Message));
      Assert.That (() => enabledControl.SelectItem().WithDisplayText ("NoneItem"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("Command").Message));
      Assert.That (() => enabledControl.SelectItem().WithDisplayTextContains ("None"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("Command").Message));
      Assert.That (() => enabledControl.SelectItem ("ItemID6"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("Command").Message));
      Assert.That (() => enabledControl.SelectItem().WithDisplayText ("DisabledItem"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("Command").Message));
      Assert.That (() => enabledControl.SelectItem().WithDisplayTextContains ("Disabled"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("Command").Message));
    
    }

    [Test]
    public void TestGetItemDefinitions ()
    {
      var home = Start();

      var listMenu = home.ListMenus().GetByLocalID ("MyListMenu");

      var items = listMenu.GetItemDefinitions();
      Assert.That (items.Count, Is.EqualTo (6));
      
      Assert.That (items[0].ItemID, Is.EqualTo ("ItemID1"));
      Assert.That (items[0].Index, Is.EqualTo (1));
      Assert.That (items[0].Text, Is.EqualTo ("EventItem"));
      Assert.That (items[0].IsDisabled, Is.False);

      Assert.That (items[2].IsDisabled, Is.True);

      Assert.That (items[4].ItemID, Is.EqualTo ("ItemID5"));
      Assert.That (items[4].Index, Is.EqualTo (5));
      Assert.That (items[4].Text, Is.EqualTo (""));
      Assert.That (items[4].IsDisabled, Is.False);
    }

    [Test]
    public void TestClickItem ()
    {
      var home = Start();

      var listMenu = home.ListMenus().GetByLocalID ("MyListMenu");

      listMenu.SelectItem ("ItemID5");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID5|Event"));

      listMenu.SelectItem().WithIndex (2);
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);

      listMenu.SelectItem().WithHtmlID ("body_MyListMenu_3");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID4|WxeFunction"));

      listMenu.SelectItem().WithDisplayText ("EventItem");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID1|Event"));

      listMenu.SelectItem().WithDisplayTextContains ("xeFun");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID4|WxeFunction"));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("ListMenuTest.wxe");
    }
  }
}