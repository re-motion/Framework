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
using System.Collections.Generic;
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
      testAction(Helper, e => e.WebTreeViews(), "webTreeView");
    }

    [Test]
    public void GetBadgeText ()
    {
      var home = Start();

      var treeView = home.WebTreeViews().GetByLocalID("MyWebTreeView3");
      var treeViewNode = treeView.GetNode("Node2");

      Assert.That(treeViewNode.GetBadgeText(), Is.EqualTo("1"));
    }

    [Test]
    public void GetBadgeText_OnNodeWithoutBadge_ReturnsEmpty ()
    {
      var home = Start();

      var treeView = home.WebTreeViews().GetByLocalID("MyWebTreeView3");
      var treeViewNode = treeView.GetNode("Node1");

      Assert.That(treeViewNode.GetBadgeText(), Is.Empty);
    }

    [Test]
    public void GetBadgeDescription ()
    {
      var home = Start();

      var treeView = home.WebTreeViews().GetByLocalID("MyWebTreeView3");
      var treeViewNode = treeView.GetNode("Node3");

      Assert.That(treeViewNode.GetBadgeDescription(), Is.EqualTo("2 description"));
    }

    [Test]
    public void GetBadgeDescription_OnNodeWithoutBadge_ReturnsEmpty ()
    {
      var home = Start();

      var treeView = home.WebTreeViews().GetByLocalID("MyWebTreeView3");
      var treeViewNode = treeView.GetNode("Node2");

      Assert.That(treeViewNode.GetBadgeDescription(), Is.Empty);
    }

    // Exists as unused member for future WebTreeView tests.
    // ReSharper disable once UnusedMember.Local
    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject>("WebTreeViewTest.aspx");
    }

    [Test]
    public void OrderedWebTree_Does_Order_Nodes_By_Category ()
    {
      var home = Start();

      var treeView = home.WebTreeViews().GetByLocalID("MyOrderedWebTreeView");
      var nodeTexts = new List<string>();
      for (var i = 1; i <= 7; i++)
      {
        nodeTexts.Add(treeView.GetNode(i).GetText());
      }

      var expectedNodeTexts = new[] { "1", "7", "2", "4", "3", "5", "6" };
      Assert.That(nodeTexts, Is.EqualTo(expectedNodeTexts));
    }

    [Test]
    public void UnorderedWebTree_Does_Not_Order_Nodes_By_Category ()
    {
      var home = Start();

      var treeView = home.WebTreeViews().GetByLocalID("MyUnorderedWebTreeView");
      var nodeTexts = new List<string>();
      for (var i = 1; i <= 7; i++)
      {
        nodeTexts.Add(treeView.GetNode(i).GetText());
      }

      var expectedNodeTexts = new[] { "1", "2", "3", "4", "5", "6", "7" };
      Assert.That(nodeTexts, Is.EqualTo(expectedNodeTexts));
    }

    [Test]
    public void GetCategory_OnNodeWithCategory_ReturnsCategory ()
    {
      var home = Start();

      var treeView = home.WebTreeViews().GetByLocalID("MyWebTreeViewWithCategories");
      var treeViewNode = treeView.GetNode(1);

      Assert.That(treeViewNode.GetCategory(), Is.EqualTo("a category"));
    }

    [Test]
    public void GetCategory_OnNodeWithOutCategory_ReturnsEmpty ()
    {
      var home = Start();

      var treeView = home.WebTreeViews().GetByLocalID("MyWebTreeViewWithoutCategories");
      var treeViewNode = treeView.GetNode(1);

      Assert.That(treeViewNode.GetCategory(), Is.Empty);
    }
  }
}