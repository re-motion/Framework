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
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class TabbedMultiViewControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var tabbedMultiView = home.GetTabbedMultiView().ByID ("body_MyTabbedMultiView");
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var tabbedMultiView = home.GetTabbedMultiView().ByIndex (1);
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var tabbedMultiView = home.GetTabbedMultiView().ByLocalID ("MyTabbedMultiView");
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var tabbedMultiView = home.GetTabbedMultiView().First();
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var tabbedMultiView = home.GetTabbedMultiView().Single();
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSubScope_TopControls ()
    {
      var home = Start();

      var tabbedMultiView = home.GetTabbedMultiView().Single();

      var topControls = tabbedMultiView.GetTopControls();
      Assert.That (topControls.Scope.Text, Is.StringContaining ("TopControls"));
      Assert.That (topControls.Scope.Text, Is.Not.StringContaining ("Content1"));
    }

    [Test]
    public void TestSubScope_View ()
    {
      var home = Start();

      var tabbedMultiView = home.GetTabbedMultiView().Single();

      var view = tabbedMultiView.GetActiveView();
      Assert.That (view.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (view.Scope.Text, Is.Not.StringContaining ("TopControls"));
      Assert.That (view.Scope.Text, Is.Not.StringContaining ("BottomControls"));
    }

    [Test]
    public void TestSubScope_BottomControls ()
    {
      var home = Start();

      var tabbedMultiView = home.GetTabbedMultiView().Single();

      var bottomControls = tabbedMultiView.GetBottomControls();
      Assert.That (bottomControls.Scope.Text, Is.StringContaining ("BottomControls"));
      Assert.That (bottomControls.Scope.Text, Is.Not.StringContaining ("Content1"));
    }

    [Test]
    public void TestGetSelectedTab ()
    {
      var home = Start();

      var tabbedMultiView = home.GetTabbedMultiView().First();
      Assert.That (tabbedMultiView.GetSelectedTab().ItemID, Is.EqualTo ("Tab1"));
      Assert.That (tabbedMultiView.GetSelectedTab().Index, Is.EqualTo (-1));
      Assert.That (tabbedMultiView.GetSelectedTab().Title, Is.EqualTo ("Tab1Title"));

      tabbedMultiView.SwitchTo ("Tab2");
      Assert.That (tabbedMultiView.GetSelectedTab().ItemID, Is.EqualTo ("Tab2"));
      Assert.That (tabbedMultiView.GetSelectedTab().Index, Is.EqualTo (-1));
      Assert.That (tabbedMultiView.GetSelectedTab().Title, Is.EqualTo ("Tab2Title"));
    }

    [Test]
    public void TestGetTabDefinitions ()
    {
      var home = Start();

      var tabbedMultiView = home.GetTabbedMultiView().First();
      var tabs = tabbedMultiView.GetTabDefinitions();
      Assert.That (tabs.Count, Is.EqualTo (2));
      Assert.That (tabs[1].ItemID, Is.EqualTo ("Tab2"));
      Assert.That (tabs[1].Index, Is.EqualTo (2));
      Assert.That (tabs[1].Title, Is.EqualTo ("Tab2Title"));
    }

    [Test]
    public void TestSwitchTo ()
    {
      var home = Start();

      var tabbedMultiView = home.GetTabbedMultiView().Single();

      home = tabbedMultiView.SwitchTo ("Tab2").Expect<WxePageObject>();
      tabbedMultiView = home.GetTabbedMultiView().Single();
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content2"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("Content1"));

      home = tabbedMultiView.SwitchTo().WithDisplayText ("Tab1Title").Expect<WxePageObject>();
      tabbedMultiView = home.GetTabbedMultiView().Single();
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("Content2"));

      home = tabbedMultiView.SwitchTo().WithDisplayTextContains ("b2T").Expect<WxePageObject>();
      tabbedMultiView = home.GetTabbedMultiView().Single();
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content2"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("Content1"));

      home = tabbedMultiView.SwitchTo().WithIndex (1).Expect<WxePageObject>();
      tabbedMultiView = home.GetTabbedMultiView().Single();
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("Content2"));

      home = tabbedMultiView.SwitchTo().WithHtmlID ("body_MyTabbedMultiView_TabStrip_Tab2_Tab").Expect<WxePageObject>();
      tabbedMultiView = home.GetTabbedMultiView().Single();
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content2"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("Content1"));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("TabbedMultiViewTest.wxe");
    }
  }
}