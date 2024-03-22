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
using Remotion.Web.Development.WebTesting.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects.Selectors;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class TreeViewControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<TreeViewSelector, TreeViewControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<TreeViewSelector, TreeViewControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<TreeViewSelector, TreeViewControlObject> testAction)
    {
      testAction(Helper, e => e.TreeViews(), "treeView");
    }

    [Test]
    public void TestGetNode ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeView");

      var rootNode = treeView.GetNode().WithIndex(1).Expand();
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Expanded: Root node|RootValue (None)"));

      rootNode.GetNode().WithIndex(2).Select();
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Selected: Child node 2|Child2Value (None)"));

      rootNode.GetNode().WithDisplayText("Child node 1").Select();
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Selected: Child node 1|Child1Value (None)"));

      rootNode.GetNode().WithDisplayTextContains("ode 2").Select();
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Selected: Child node 2|Child2Value (None)"));
    }

    [Test]
    public void TestNodeGetText ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeView");
      var node = treeView.GetNode().WithIndex(1);

      Assert.That(node.GetText(), Is.EqualTo("Root node"));
    }

    [Test]
    public void TestNodeIsChecked ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeView");
      var rootNode = treeView.GetNode().WithIndex(1);

      // Set Timeout to Zero so we don't have to wait the full timeout for the exception
      var backupTimeout = rootNode.Scope.ElementFinder.Options.Timeout;
      rootNode.Scope.ElementFinder.Options.Timeout = TimeSpan.Zero;

      Assert.That(
          () => rootNode.IsChecked(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(
                      Driver,
                      "The checkbox could not be found: Unable to find xpath: ./tbody/tr/td[a[contains(@onclick, 'TreeView_SelectNode')]]/input[@type='checkbox']").Message));

      rootNode.Scope.ElementFinder.Options.Timeout = backupTimeout;

      var checkableNode = rootNode.Expand().GetNode(1).Expand().GetNode(1);
      var completionDetection = new CompletionDetectionStrategyTestHelper(checkableNode);

      checkableNode.Check();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(checkableNode.IsChecked(), Is.True);

      checkableNode.Select();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(
          home.Scope.FindIdEndingWith("TestOutputLabel").Text,
          Is.EqualTo("Selected: Child node 11|Child11Value (Child node 11|Child11Value)"));

      checkableNode.Uncheck();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(checkableNode.IsChecked(), Is.False);

      rootNode.Select();
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Selected: Root node|RootValue (None)"));
    }

    [Test]
    public void TestGetNumberOfChildren ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeView");
      var rootNode = treeView.GetNode().WithIndex(1);
      Assert.That(rootNode.GetNumberOfChildren(), Is.EqualTo(2));

      var child1Node = rootNode.Expand().GetNode(1);
      Assert.That(child1Node.GetNumberOfChildren(), Is.EqualTo(2));
    }

    [Test]
    public void TestNodeExpand ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeView");

      var rootNode = treeView.GetNode().WithIndex(1);
      var rootNodeCompletionDetection = new CompletionDetectionStrategyTestHelper(rootNode);
      rootNode.Expand();
      Assert.That(rootNodeCompletionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());

      var childNode = rootNode.GetNode(1).Expand();
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Expanded: Child node 1|Child1Value (None)"));

      var selectableNode = childNode.GetNode(2);
      var selectableNodeCompletionDetection = new CompletionDetectionStrategyTestHelper(selectableNode);
      selectableNode.Select();
      Assert.That(selectableNodeCompletionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Selected: Child node 12|Child12Value (None)"));
    }

    [Test]
    public void TestNodeCollapse ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeView");

      var expandedRootNode = treeView.GetNode().WithIndex(1).Expand();
      var rootNodeCompletionDetection = new CompletionDetectionStrategyTestHelper(expandedRootNode);
      var collapsedRootNode = expandedRootNode.Collapse();
      Assert.That(rootNodeCompletionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());

      var childNode = collapsedRootNode.Expand().GetNode(2);
      childNode.Expand().Collapse().Expand();
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Expanded: Child node 2|Child2Value (None)"));

      var selectableNode = childNode.GetNode(1);
      var selectableNodeCompletionDetection = new CompletionDetectionStrategyTestHelper(selectableNode);
      selectableNode.Select();
      Assert.That(selectableNodeCompletionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Selected: Child node 21|Child21Value (None)"));
    }

    [Test]
    public void TestNodeSelect ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeView");

      var rootNode = treeView.GetNode().WithIndex(1);
      var completionDetection = new CompletionDetectionStrategyTestHelper(rootNode);
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.Empty);

      rootNode.Select();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Selected: Root node|RootValue (None)"));
    }

    [Test]
    public void TestTreeNodeSelect ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeView");

      treeView.GetNode().WithIndex(1).Select();
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Selected: Root node|RootValue (None)"));
      treeView.GetNode(1).Select();
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo(""));
      Assert.That(() => treeView.GetNode().WithDisplayText("Root node"), Throws.Nothing);
      Assert.That(() => treeView.GetNode().WithDisplayTextContains("Root node"), Throws.Nothing);
      Assert.That(() => treeView.GetNode().WithItemID("SomeItemID"), Throws.InstanceOf<NotSupportedException>());
      Assert.That(() => treeView.GetNode("SomeItemID"), Throws.InstanceOf<NotSupportedException>());
    }

    [Test]
    public void TestNodeSelectOnlyChildren ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeView");
      var rootNode = treeView.GetNode().WithIndex(1);

      rootNode.Expand();
      var firstChildOfRootNode = rootNode.GetNode().WithDisplayTextContains("1");
      firstChildOfRootNode.Expand();

      treeView.Scope.ElementFinder.Options.Timeout = TimeSpan.Zero;
      Assert.That(
          () => treeView.GetNode().WithDisplayTextContains("1").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(Driver, "Unable to find xpath: ./table[contains(tbody/tr/td[last()], '1')]").Message));
      Assert.That(
          () => treeView.GetNode().WithDisplayText("ChildNode 1").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(Driver, "Unable to find xpath: ./table[normalize-space(tbody/tr/td[last()])='ChildNode 1']").Message));
      Assert.That(
          () => treeView.GetNode().WithItemID("1").Select(),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("The TreeViewControlObject does not support node selection by item ID."));
      Assert.That(
          () => treeView.GetNode("1").Select(),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("The TreeViewControlObject does not support node selection by item ID."));

      rootNode.Scope.ElementFinder.Options.Timeout = TimeSpan.Zero;
      Assert.That(
          () => rootNode.GetNode().WithDisplayTextContains("11").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(Driver, "Unable to find xpath: ./table[contains(tbody/tr/td[last()]//*, '11')]").Message));
      Assert.That(
          () => rootNode.GetNode().WithDisplayText("ChildNode 11").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(
                      Driver,
                      "Unable to find xpath: ./table[normalize-space(tbody/tr/td[last()]//*)='ChildNode 11']").Message));
      Assert.That(
          () => rootNode.GetNode().WithItemID("11").Select(),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("The TreeViewNodeControlObject does not support node selection by item ID."));
      Assert.That(
          () => rootNode.GetNode("11").Select(),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("The TreeViewNodeControlObject does not support node selection by item ID."));
    }

    [Test]
    public void TestSelectNodeInHierarchy ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeView");

      var rootNode = treeView.GetNode().WithIndex(1);
      rootNode.Expand();
      var firstChildOfRootNode = rootNode.GetNodeInHierarchy().WithDisplayTextContains("1");
      firstChildOfRootNode.Expand();

      rootNode.GetNodeInHierarchy().WithDisplayText("Child node 12").Select();
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Selected: Child node 12|Child12Value (None)"));

      rootNode.GetNodeInHierarchy().WithDisplayTextContains("11").Select();
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Selected: Child node 11|Child11Value (None)"));

      Assert.That(() => rootNode.GetNodeInHierarchy().WithItemID("11").Select(), Throws.InstanceOf<NotSupportedException>());
      Assert.That(() => rootNode.GetNodeInHierarchy("11").Select(), Throws.InstanceOf<NotSupportedException>());
    }

    [Test]
    public void TestSelectNodeWithIndexInHierarchy ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeView");

      var rootNode = treeView.GetNode().WithIndex(1);
      rootNode.Expand();
      rootNode.GetNode().WithDisplayTextContains("2").Expand();

      rootNode.GetNodeInHierarchy().WithIndex(3).Select();
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Selected: Child node 23|Child23Value (None)"));

      Assert.That(
          () => rootNode.GetNodeInHierarchy().WithIndex(999),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(AssertionExceptionUtility.CreateExpectationException(Driver, "No node with the index '999' was found.").Message));

      Assert.That(
          () => rootNode.GetNodeInHierarchy().WithIndex(1),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(AssertionExceptionUtility.CreateExpectationException(Driver, "Multiple nodes with the index '1' were found.").Message));
    }

    [Test]
    public void TestSelectNodeInHierarchyOnlyRootNodeExpanded ()
    {
      var expectedExceptionMessage = AssertionExceptionUtility.CreateExpectationException(
              Driver,
              "The element cannot be found: This element has been removed from the DOM. Coypu will normally re-find elements using the original locators in this situation, "
              + "except if you have captured a snapshot list of all matching elements using FindAllCss() or FindAllXPath()")
          .Message;
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeView");

      var rootNode = treeView.GetNode().WithIndex(1);
      rootNode.Expand();

      rootNode.Scope.ElementFinder.Options.Timeout = TimeSpan.Zero;
      Assert.That(
          () => rootNode.GetNodeInHierarchy().WithDisplayText("Child node 12").Select(), Throws.InstanceOf<WebTestException>()
          .With.Message.EqualTo(expectedExceptionMessage));
      Assert.That(
          () => rootNode.GetNodeInHierarchy().WithDisplayTextContains("11").Select(), Throws.InstanceOf<WebTestException>()
          .With.Message.EqualTo(expectedExceptionMessage));
    }

    [Test]
    public void TestTreeSelectNodeInHierarchy ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeView");

      var rootNode = treeView.GetNode().WithIndex(1);
      rootNode.Expand();
      var firstChildOfRootNode = rootNode.GetNodeInHierarchy().WithDisplayTextContains("1");
      firstChildOfRootNode.Expand();

      treeView.GetNodeInHierarchy().WithDisplayText("Child node 12").Select();
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Selected: Child node 12|Child12Value (None)"));

      treeView.GetNodeInHierarchy().WithDisplayTextContains("11").Select();
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Selected: Child node 11|Child11Value (None)"));

      Assert.That(() => treeView.GetNodeInHierarchy().WithItemID("11").Select(), Throws.InstanceOf<NotSupportedException>());
      Assert.That(() => treeView.GetNodeInHierarchy("11").Select(), Throws.InstanceOf<NotSupportedException>());
    }

    [Test]
    public void TestTreeSelectNodeWithIndexInHierarchy ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeView");

      var rootNode = treeView.GetNode().WithIndex(1);
      rootNode.Expand();
      rootNode.GetNode().WithDisplayTextContains("2").Expand();

      treeView.GetNodeInHierarchy().WithIndex(3).Select();
      Assert.That(home.Scope.FindIdEndingWith("TestOutputLabel").Text, Is.EqualTo("Selected: Child node 23|Child23Value (None)"));

      Assert.That(
          () => treeView.GetNodeInHierarchy().WithIndex(999),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(AssertionExceptionUtility.CreateExpectationException(Driver, "No node with the index '999' was found.").Message));

      Assert.That(
          () => treeView.GetNodeInHierarchy().WithIndex(1),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(AssertionExceptionUtility.CreateExpectationException(Driver, "Multiple nodes with the index '1' were found.").Message));
    }

    [Test]
    public void TestTreeSelectNodeInHierarchyOnlyRootNodeExpanded ()
    {
      const string expectedExceptionMessage = "The element cannot be found: ";
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeView");

      var rootNode = treeView.GetNode().WithIndex(1);
      rootNode.Expand();

      treeView.Scope.ElementFinder.Options.Timeout = TimeSpan.Zero;

      Assert.That(
          () => treeView.GetNodeInHierarchy().WithDisplayText("Child node 12").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.StartsWith(expectedExceptionMessage));
      Assert.That(
          () => treeView.GetNodeInHierarchy().WithDisplayTextContains("11").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.StartsWith(expectedExceptionMessage));
    }

    [Test]
    public void TestNodeSelectByDisplayText_WithSingleQuote ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeViewWithSpecialChildren");

      var rootNode = treeView.GetNode().WithIndex(1);
      var node = rootNode.GetNode().WithDisplayText("With'SingleQuote");
      Assert.That(node.GetText(), Is.EqualTo("With'SingleQuote"));
    }

    [Test]
    public void TestNodeSelectByDisplayText_WithSingleQuoteAndDoubleQuote ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeViewWithSpecialChildren");

      var rootNode = treeView.GetNode().WithIndex(1);
      var node = rootNode.GetNode().WithDisplayText("With'SingleQuoteAndDouble\"Quote");
      Assert.That(node.GetText(), Is.EqualTo("With'SingleQuoteAndDouble\"Quote"));
    }

    [Test]
    public void TestNodeSelectByDisplayTextContains_WithSingleQuote ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeViewWithSpecialChildren");

      var rootNode = treeView.GetNode().WithIndex(1);
      var node = rootNode.GetNode().WithDisplayTextContains("ith'SingleQuot");
      Assert.That(node.GetText(), Is.EqualTo("With'SingleQuote"));
    }

    [Test]
    public void TestNodeSelectByDisplayTextContains_WithSingleQuoteAndDoubleQuote ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("MyTreeViewWithSpecialChildren");

      var rootNode = treeView.GetNode().WithIndex(1);
      var node = rootNode.GetNode().WithDisplayTextContains("ith'SingleQuoteAndDouble\"Quot");
      Assert.That(node.GetText(), Is.EqualTo("With'SingleQuoteAndDouble\"Quote"));
    }


    [Test]
    public void TestTreeViewSelectByDisplayText_WithSingleQuote ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("TreeViewWithOnlyRootWithSingleQuote");

      var node = treeView.GetNode().WithDisplayText("With'SingleQuote");
      Assert.That(node.GetText(), Is.EqualTo("With'SingleQuote"));
    }

    [Test]
    public void TestTreeViewSelectByDisplayText_WithSingleQuoteAndDoubleQuote ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("TreeViewWithOnlyRootWithDoubleQuote");

      var node = treeView.GetNode().WithDisplayText("With'SingleQuoteAndDouble\"Quote");
      Assert.That(node.GetText(), Is.EqualTo("With'SingleQuoteAndDouble\"Quote"));
    }

    [Test]
    public void TestTreeViewSelectByDisplayTextContains_WithSingleQuote ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("TreeViewWithOnlyRootWithSingleQuote");

      var node = treeView.GetNode().WithDisplayTextContains("ith'SingleQuot");
      Assert.That(node.GetText(), Is.EqualTo("With'SingleQuote"));
    }

    [Test]
    public void TestTreeViewSelectByDisplayTextContains_WithSingleQuoteAndDoubleQuote ()
    {
      var home = Start();

      var treeView = home.TreeViews().GetByLocalID("TreeViewWithOnlyRootWithDoubleQuote");

      var node = treeView.GetNode().WithDisplayTextContains("ith'SingleQuoteAndDouble\"Quot");
      Assert.That(node.GetText(), Is.EqualTo("With'SingleQuoteAndDouble\"Quote"));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject>("TreeViewTest.wxe");
    }
  }
}
