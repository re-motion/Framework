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
  public class TreeViewControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var treeView = home.GetTreeView().ByID ("body_MyTreeView");
      Assert.That (treeView.Scope.Id, Is.EqualTo ("body_MyTreeView"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var treeView = home.GetTreeView().ByLocalID ("MyTreeView");
      Assert.That (treeView.Scope.Id, Is.EqualTo ("body_MyTreeView"));
    }

    [Test]
    public void TestGetRootNode ()
    {
      var home = Start();

      var treeView = home.GetTreeView().ByLocalID ("MyTreeView");
      var rootNode = treeView.GetRootNode();
      Assert.That (rootNode.GetText(), Is.EqualTo ("Root node"));
    }

    [Test]
    public void TestGetNode ()
    {
      var home = Start();

      var treeView = home.GetTreeView().ByLocalID ("MyTreeView");

      var rootNode = treeView.GetRootNode().Expand();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo("Expanded: Root node|RootValue (None)"));
      
      rootNode.GetNode().WithIndex (2).Select();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Selected: Child node 2|Child2Value (None)"));

      rootNode.GetNode().WithDisplayText ("Child node 1").Select();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Selected: Child node 1|Child1Value (None)"));

      rootNode.GetNode().WithDisplayTextContains ("ode 2").Select();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Selected: Child node 2|Child2Value (None)"));
    }
    
    [Test]
    public void TestNodeGetText ()
    {
      var home = Start();

      var treeView = home.GetTreeView().ByLocalID ("MyTreeView");
      var node = treeView.GetRootNode();

      Assert.That (node.GetText(), Is.EqualTo ("Root node"));
    }

    [Test]
    public void TestNodeIsChecked ()
    {
      var home = Start();

      var treeView = home.GetTreeView().ByLocalID ("MyTreeView");
      var rootNode = treeView.GetRootNode();
      Assert.Throws<MissingHtmlException> (() => rootNode.IsChecked());

      var checkableNode = rootNode.Expand().GetNode (1).Expand().GetNode (1);
      checkableNode.Check();
      Assert.That (checkableNode.IsChecked(), Is.True);

      checkableNode.Select();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Selected: Child node 11|Child11Value (Child node 11|Child11Value)"));

      checkableNode.Uncheck();
      Assert.That (checkableNode.IsChecked(), Is.False);

      rootNode.Select();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Selected: Root node|RootValue (None)"));
    }

    [Test]
    public void TestGetNumberOfChildren ()
    {
      var home = Start();

      var treeView = home.GetTreeView().ByLocalID ("MyTreeView");
      var rootNode = treeView.GetRootNode();
      Assert.That (rootNode.GetNumberOfChildren(), Is.EqualTo (2));

      var child1Node = rootNode.Expand().GetNode (1);
      Assert.That (child1Node.GetNumberOfChildren(), Is.EqualTo (2));
    }

    [Test]
    public void TestNodeExpand ()
    {
      var home = Start();

      var treeView = home.GetTreeView().ByLocalID ("MyTreeView");
      var node = treeView.GetRootNode().Expand();
      node = node.GetNode (1).Expand();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Expanded: Child node 1|Child1Value (None)"));

      node.GetNode (2).Select();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Selected: Child node 12|Child12Value (None)"));
    }

    [Test]
    public void TestNodeCollapse ()
    {
      var home = Start();

      var treeView = home.GetTreeView().ByLocalID ("MyTreeView");
      var node = treeView.GetRootNode().Expand().Collapse().Expand();
      node = node.GetNode (2).Expand().Collapse().Expand();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Expanded: Child node 2|Child2Value (None)"));

      node.GetNode (1).Select();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Selected: Child node 21|Child21Value (None)"));
    }

    [Test]
    public void TestNodeSelect ()
    {
      var home = Start();

      var treeView = home.GetTreeView().ByLocalID ("MyTreeView");

      var node = treeView.GetRootNode();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.Empty);

      node.Select();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Selected: Root node|RootValue (None)"));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("TreeViewTest.wxe");
    }
  }
}