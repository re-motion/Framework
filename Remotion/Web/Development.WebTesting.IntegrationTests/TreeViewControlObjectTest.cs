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
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects.Selectors;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class TreeViewControlObjectTest : IntegrationTest
  {
    [Test]
    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<TreeViewSelector, TreeViewControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<TreeViewSelector, TreeViewControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<TreeViewSelector, TreeViewControlObject> testAction)
    {
      testAction (Helper, e => e.TreeViews(), "treeView");
    }

    [Test]
    public void TestGetRootNode ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID ("MyTreeView");
      var rootNode = treeView.GetNode().WithIndex (1);
      Assert.That (rootNode.GetText(), Is.EqualTo ("Root node"));
    }

    [Test]
    public void TestGetNode ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID ("MyTreeView");

      var rootNode = treeView.GetNode().WithIndex (1).Expand();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Expanded: Root node|RootValue (None)"));

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

      var treeView = home.TreeViews().GetByLocalID ("MyTreeView");
      var node = treeView.GetNode().WithIndex (1);

      Assert.That (node.GetText(), Is.EqualTo ("Root node"));
    }

    [Test]
    public void TestNodeIsChecked ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID ("MyTreeView");
      var rootNode = treeView.GetNode().WithIndex (1);

      // Set Timeout to Zero so we don't have to wait the full timeout for the exception
      var backupTimeout = rootNode.Scope.ElementFinder.Options.Timeout;
      rootNode.Scope.ElementFinder.Options.Timeout = TimeSpan.Zero;

      Assert.That (() => rootNode.IsChecked(), Throws.InstanceOf<MissingHtmlException>());

      rootNode.Scope.ElementFinder.Options.Timeout = backupTimeout;

      var checkableNode = rootNode.Expand().GetNode (1).Expand().GetNode (1);

      checkableNode.Check();
      Assert.That (checkableNode.IsChecked(), Is.True);

      checkableNode.Select();
      Assert.That (
          home.Scope.FindIdEndingWith ("TestOutputLabel").Text,
          Is.EqualTo ("Selected: Child node 11|Child11Value (Child node 11|Child11Value)"));

      checkableNode.Uncheck();
      Assert.That (checkableNode.IsChecked(), Is.False);

      rootNode.Select();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Selected: Root node|RootValue (None)"));
    }

    [Test]
    public void TestGetNumberOfChildren ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID ("MyTreeView");
      var rootNode = treeView.GetNode().WithIndex (1);
      Assert.That (rootNode.GetNumberOfChildren(), Is.EqualTo (2));

      var child1Node = rootNode.Expand().GetNode (1);
      Assert.That (child1Node.GetNumberOfChildren(), Is.EqualTo (2));
    }

    [Test]
    public void TestNodeExpand ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID ("MyTreeView");
      var node = treeView.GetNode().WithIndex (1).Expand();
      node = node.GetNode (1).Expand();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Expanded: Child node 1|Child1Value (None)"));

      node.GetNode (2).Select();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Selected: Child node 12|Child12Value (None)"));
    }

    [Test]
    public void TestNodeCollapse ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID ("MyTreeView");
      var node = treeView.GetNode().WithIndex (1).Expand().Collapse().Expand();
      node = node.GetNode (2).Expand().Collapse().Expand();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Expanded: Child node 2|Child2Value (None)"));

      node.GetNode (1).Select();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Selected: Child node 21|Child21Value (None)"));
    }

    [Test]
    public void TestNodeSelect ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID ("MyTreeView");

      var node = treeView.GetNode().WithIndex (1);
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.Empty);

      node.Select();
      Assert.That (home.Scope.FindIdEndingWith ("TestOutputLabel").Text, Is.EqualTo ("Selected: Root node|RootValue (None)"));
    }

    [Test]
    public void TestNodeSelectByDisplayText_WithSingleQuote ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID ("MyTreeViewWithSpecialChildren");
      
      var rootNode = treeView.GetNode().WithIndex (1);
      var node = rootNode.GetNode().WithDisplayText ("With'SingleQuote");
      Assert.That (node.GetText(), Is.EqualTo ("With'SingleQuote"));
    }

    [Test]
    public void TestNodeSelectByDisplayText_WithSingleQuoteAndDoubleQuote ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID ("MyTreeViewWithSpecialChildren");
      
      var rootNode = treeView.GetNode().WithIndex (1);
      var node = rootNode.GetNode().WithDisplayText ("With'SingleQuoteAndDouble\"Quote");
      Assert.That (node.GetText(), Is.EqualTo ("With'SingleQuoteAndDouble\"Quote"));
    }
    
    [Test]
    public void TestNodeSelectByDisplayTextContains_WithSingleQuote ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID ("MyTreeViewWithSpecialChildren");
      
      var rootNode = treeView.GetNode().WithIndex (1);
      var node = rootNode.GetNode().WithDisplayTextContains ("ith'SingleQuot");
      Assert.That (node.GetText(), Is.EqualTo ("With'SingleQuote"));
    }

    [Test]
    public void TestNodeSelectByDisplayTextContains_WithSingleQuoteAndDoubleQuote ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID ("MyTreeViewWithSpecialChildren");
      
      var rootNode = treeView.GetNode().WithIndex (1);
      var node = rootNode.GetNode().WithDisplayTextContains ("ith'SingleQuoteAndDouble\"Quot");
      Assert.That (node.GetText(), Is.EqualTo ("With'SingleQuoteAndDouble\"Quote"));
    }

    
    [Test]
    public void TestTreeViewSelectByDisplayText_WithSingleQuote ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID ("TreeViewWithOnlyRootWithSingleQuote");
      
      var node = treeView.GetNode().WithDisplayText ("With'SingleQuote");
      Assert.That (node.GetText(), Is.EqualTo ("With'SingleQuote"));
    }

    [Test]
    public void TestTreeViewSelectByDisplayText_WithSingleQuoteAndDoubleQuote ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID ("TreeViewWithOnlyRootWithDoubleQuote");
      
      var node = treeView.GetNode().WithDisplayText ("With'SingleQuoteAndDouble\"Quote");
      Assert.That (node.GetText(), Is.EqualTo ("With'SingleQuoteAndDouble\"Quote"));
    }
    
    [Test]
    public void TestTreeViewSelectByDisplayTextContains_WithSingleQuote ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID ("TreeViewWithOnlyRootWithSingleQuote");
      
      var node = treeView.GetNode().WithDisplayTextContains ("ith'SingleQuot");
      Assert.That (node.GetText(), Is.EqualTo ("With'SingleQuote"));
    }

    [Test]
    public void TestTreeViewSelectByDisplayTextContains_WithSingleQuoteAndDoubleQuote ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID ("TreeViewWithOnlyRootWithDoubleQuote");
      
      var node = treeView.GetNode().WithDisplayTextContains ("ith'SingleQuoteAndDouble\"Quot");
      Assert.That (node.GetText(), Is.EqualTo ("With'SingleQuoteAndDouble\"Quote"));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("TreeViewTest.wxe");
    }
  }
}