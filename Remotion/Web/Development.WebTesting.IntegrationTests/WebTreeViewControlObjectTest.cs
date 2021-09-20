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
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class WebTreeViewControlObjectTest : IntegrationTest
  {
    // Note: functionality is integration tested via BocTreeViewControlObject in BocTreeViewControlObjectTest.

    [Test]
    [TestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<WebTreeViewSelector, WebTreeViewControlObject>))]
    [TestCaseSource (typeof (IndexControlSelectorTestCaseFactory<WebTreeViewSelector, WebTreeViewControlObject>))]
    [TestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<WebTreeViewSelector, WebTreeViewControlObject>))]
    [TestCaseSource (typeof (FirstControlSelectorTestCaseFactory<WebTreeViewSelector, WebTreeViewControlObject>))]
    [TestCaseSource (typeof (SingleControlSelectorTestCaseFactory<WebTreeViewSelector, WebTreeViewControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<WebTreeViewSelector, WebTreeViewControlObject> testAction)
    {
      testAction (Helper, e => e.WebTreeViews(), "webTreeView");
    }

    [Test]
    public void GetBadgeText ()
    {
      var home = Start();

      var treeView = home.WebTreeViews().GetByLocalID ("MyWebTreeView3");
      var treeViewNode = treeView.GetNode ("Node2");

      Assert.That (treeViewNode.GetBadgeText(), Is.EqualTo ("1"));
    }

    [Test]
    public void GetBadgeText_OnNodeWithoutBadge_ReturnsNull ()
    {
      var home = Start();

      var treeView = home.WebTreeViews().GetByLocalID ("MyWebTreeView3");
      var treeViewNode = treeView.GetNode ("Node1");

      Assert.That (treeViewNode.GetBadgeText(), Is.Null);
    }

    [Test]
    public void GetBadgeDescription ()
    {
      var home = Start();

      var treeView = home.WebTreeViews().GetByLocalID ("MyWebTreeView3");
      var treeViewNode = treeView.GetNode ("Node3");

      Assert.That (treeViewNode.GetBadgeDescription(), Is.EqualTo ("2 description"));
    }

    [Test]
    public void GetBadgeDescription_OnNodeWithoutBadge_ReturnsNull ()
    {
      var home = Start();

      var treeView = home.WebTreeViews().GetByLocalID ("MyWebTreeView3");
      var treeViewNode = treeView.GetNode ("Node2");

      Assert.That (treeViewNode.GetBadgeDescription(), Is.Null);
    }

    // Exists as unused member for future WebTreeView tests.
    // ReSharper disable once UnusedMember.Local
    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject> ("WebTreeViewTest.aspx");
    }
  }
}