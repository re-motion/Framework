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
  public class DropDownListControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var dropDownList = home.GetDropDownList().ByID ("body_MyDropDownList");
      Assert.That (dropDownList.Scope.Id, Is.EqualTo ("body_MyDropDownList"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var dropDownList = home.GetDropDownList().ByIndex (2);
      Assert.That (dropDownList.Scope.Id, Is.EqualTo ("body_MyDropDownList2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var dropDownList = home.GetDropDownList().ByLocalID ("MyDropDownList");
      Assert.That (dropDownList.Scope.Id, Is.EqualTo ("body_MyDropDownList"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var dropDownList = home.GetDropDownList().First();
      Assert.That (dropDownList.Scope.Id, Is.EqualTo ("body_MyDropDownList"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = home.GetScope().ByID ("scope");

      var dropDownList = scope.GetDropDownList().Single();
      Assert.That (dropDownList.Scope.Id, Is.EqualTo ("body_MyDropDownList2"));

      try
      {
        home.GetDropDownList().Single();
        Assert.Fail ("Should not be able to unambigously find a drop down list.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestGetSelectedOption ()
    {
      var home = Start();

      var dropDownList = home.GetDropDownList().ByLocalID ("MyDropDownList");
      Assert.That (dropDownList.GetSelectedOption().ItemID, Is.EqualTo ("Item1Value"));
      Assert.That (dropDownList.GetSelectedOption().Index, Is.EqualTo (-1));
      Assert.That (dropDownList.GetSelectedOption().Text, Is.EqualTo ("Item1"));
    }

    [Test]
    public void TestGetOptionDefinitions ()
    {
      var home = Start();

      var dropDownList = home.GetDropDownList().ByLocalID ("MyDropDownList");

      var options = dropDownList.GetOptionDefinitions();
      Assert.That (options.Count, Is.EqualTo (3));

      Assert.That (options[0].ItemID, Is.EqualTo ("Item1Value"));
      Assert.That (options[0].Index, Is.EqualTo (1));
      Assert.That (options[0].Text, Is.EqualTo ("Item1"));

      Assert.That (options[2].ItemID, Is.EqualTo ("Item3Value"));
      Assert.That (options[2].Index, Is.EqualTo (3));
      Assert.That (options[2].Text, Is.EqualTo ("Item3"));
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var dropDownList = home.GetDropDownList().ByLocalID ("MyDropDownList");
      dropDownList.SelectOption ("Item2Value");
      Assert.That (dropDownList.GetText(), Is.EqualTo ("Item2"));
    }

    [Test]
    public void TestClickItem ()
    {
      var home = Start();

      var dropDownList = home.GetDropDownList().ByLocalID ("MyDropDownList");

      dropDownList.SelectOption ("Item3Value");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("Item3|Item3Value"));

      dropDownList.SelectOption().WithIndex (2);
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("Item2|Item2Value"));

      dropDownList.SelectOption().WithDisplayText ("Item1");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("Item1|Item1Value"));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("DropDownListTest.wxe");
    }
  }
}