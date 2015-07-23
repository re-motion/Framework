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
  public class ListMenuControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var listMenu = home.GetListMenu().ByID ("body_MyListMenu");
      Assert.That (listMenu.Scope.Id, Is.EqualTo ("body_MyListMenu"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var listMenu = home.GetListMenu().ByIndex (2);
      Assert.That (listMenu.Scope.Id, Is.EqualTo ("body_MyListMenu2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var listMenu = home.GetListMenu().ByLocalID ("MyListMenu");
      Assert.That (listMenu.Scope.Id, Is.EqualTo ("body_MyListMenu"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var listMenu = home.GetListMenu().First();
      Assert.That (listMenu.Scope.Id, Is.EqualTo ("body_MyListMenu"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = home.GetScope().ByID ("scope");

      var listMenu = scope.GetListMenu().Single();
      Assert.That (listMenu.Scope.Id, Is.EqualTo ("body_MyListMenu2"));

      try
      {
        home.GetListMenu().Single();
        Assert.Fail ("Should not be able to unambigously find a list menu.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestGetItemDefinitions ()
    {
      var home = Start();

      var listMenu = home.GetListMenu().ByLocalID ("MyListMenu");
      
      var items = listMenu.GetItemDefinitions();
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

      var listMenu = home.GetListMenu().ByLocalID ("MyListMenu");

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