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
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class FormGridControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var formGrid = home.GetFormGrid().ByID ("body_My1FormGrid");
      Assert.That (formGrid.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (formGrid.Scope.Text, Is.Not.StringContaining ("DoNotFindMe1"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var formGrid = home.GetFormGrid().ByIndex (2);
      Assert.That (formGrid.Scope.Text, Is.StringContaining ("Content2"));
      Assert.That (formGrid.Scope.Text, Is.Not.StringContaining ("DoNotFindMe2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var formGrid = home.GetFormGrid().ByLocalID ("My1FormGrid");
      Assert.That (formGrid.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (formGrid.Scope.Text, Is.Not.StringContaining ("DoNotFindMe1"));
    }

    [Test]
    public void TestSelection_ByTitle ()
    {
      var home = Start();

      var formGrid = home.GetFormGrid().ByTitle ("MyFormGrid2");
      Assert.That (formGrid.Scope.Text, Is.StringContaining ("Content2"));
      Assert.That (formGrid.Scope.Text, Is.Not.StringContaining ("DoNotFindMe2"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var formGrid = home.GetFormGrid().First();
      Assert.That (formGrid.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (formGrid.Scope.Text, Is.Not.StringContaining ("DoNotFindMe1"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = home.GetScope().ByID ("scope");

      var formGrid = scope.GetFormGrid().Single();
      Assert.That (formGrid.Scope.Text, Is.StringContaining ("Content2"));
      Assert.That (formGrid.Scope.Text, Is.Not.StringContaining ("DoNotFindMe2"));

      try
      {
        home.GetFormGrid().Single();
        Assert.Fail ("Should not be able to unambigously find a form grid.");
      }
      catch (AmbiguousException)
      {
      }
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject> ("FormGridTest.aspx");
    }
  }
}