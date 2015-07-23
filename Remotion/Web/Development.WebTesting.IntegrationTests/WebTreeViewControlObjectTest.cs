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
  public class WebTreeViewControlObjectTest : IntegrationTest
  {
    // Note: functionality is integration tested via BocTreeViewControlObject in BocTreeViewControlObjectTest.

    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var webTreeView = home.GetWebTreeView().ByID ("body_MyWebTreeView");
      Assert.That (webTreeView.Scope.Id, Is.EqualTo ("body_MyWebTreeView"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var webTreeView = home.GetWebTreeView().ByIndex (2);
      Assert.That (webTreeView.Scope.Id, Is.EqualTo ("body_MyWebTreeView2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var webTreeView = home.GetWebTreeView().ByLocalID ("MyWebTreeView");
      Assert.That (webTreeView.Scope.Id, Is.EqualTo ("body_MyWebTreeView"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var webTreeView = home.GetWebTreeView().First();
      Assert.That (webTreeView.Scope.Id, Is.EqualTo ("body_MyWebTreeView"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = home.GetScope().ByID ("scope");

      var webTreeView = scope.GetWebTreeView().Single();
      Assert.That (webTreeView.Scope.Id, Is.EqualTo ("body_MyWebTreeView2"));

      try
      {
        home.GetWebTreeView().Single();
        Assert.Fail ("Should not be able to unambigously find a web tree view.");
      }
      catch (AmbiguousException)
      {
      }
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject> ("WebTreeViewTest.aspx");
    }
  }
}