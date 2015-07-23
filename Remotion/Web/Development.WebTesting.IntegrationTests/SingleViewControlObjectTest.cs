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
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class SingleViewControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var singleView = home.GetSingleView().ByID ("body_MySingleView");
      Assert.That (singleView.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (singleView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var singleView = home.GetSingleView().ByIndex (1);
      Assert.That (singleView.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (singleView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var singleView = home.GetSingleView().ByLocalID ("MySingleView");
      Assert.That (singleView.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (singleView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var singleView = home.GetSingleView().First();
      Assert.That (singleView.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (singleView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var singleView = home.GetSingleView().Single();
      Assert.That (singleView.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (singleView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSubScope_TopControls ()
    {
      var home = Start();

      var singleView = home.GetSingleView().Single();

      var topControls = singleView.GetTopControls();
      Assert.That (topControls.Scope.Text, Is.StringContaining ("TopControls"));
      Assert.That (topControls.Scope.Text, Is.Not.StringContaining ("Content"));
    }

    [Test]
    public void TestSubScope_View ()
    {
      var home = Start();

      var singleView = home.GetSingleView().Single();

      var view = singleView.GetView();
      Assert.That (view.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (view.Scope.Text, Is.Not.StringContaining ("TopControls"));
      Assert.That (view.Scope.Text, Is.Not.StringContaining ("BottomControls"));
    }

    [Test]
    public void TestSubScope_BottomControls ()
    {
      var home = Start();

      var singleView = home.GetSingleView().Single();

      var bottomControls = singleView.GetBottomControls();
      Assert.That (bottomControls.Scope.Text, Is.StringContaining ("BottomControls"));
      Assert.That (bottomControls.Scope.Text, Is.Not.StringContaining ("Content"));
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject> ("SingleViewTest.aspx");
    }
  }
}