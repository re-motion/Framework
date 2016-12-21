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
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class DropDownMenuControlObjectTest : IntegrationTest
  {
    // Note: the <see cref="T:DropDownMenu.Mode"/>=<see cref="T:MenuMode.ContextMenu"/> option is tested indirectly by the BocTreeViewControlObjectTest.

    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var dropDownMenu = home.GetDropDownMenu().ByID ("body_MyDropDownMenu");
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var dropDownMenu = home.GetDropDownMenu().ByIndex (2);
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var dropDownMenu = home.GetDropDownMenu().ByLocalID ("MyDropDownMenu");
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var dropDownMenu = home.GetDropDownMenu().First();
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = home.GetScope().ByID ("scope");

      var dropDownMenu = scope.GetDropDownMenu().Single();
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu2"));

      try
      {
        home.GetDropDownMenu().Single();
        Assert.Fail ("Should not be able to unambigously find a drop down menu.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSelection_Text ()
    {
      var home = Start();

      var dropDownMenu = home.GetDropDownMenu().ByTextContent ("MyTitleText");
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu2"));
    }

    [Test]
    public void TestGetItemDefinitions ()
    {
      var home = Start();

      var dropDownMenu = home.GetDropDownMenu().ByLocalID ("MyDropDownMenu");
      
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

      var dropDownMenu = home.GetDropDownMenu().ByLocalID ("MyDropDownMenu");

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

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("DropDownMenuTest.wxe");
    }
  }
}