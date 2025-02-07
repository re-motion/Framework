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
using System.Drawing;
using Coypu;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocTreeViewControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource(typeof(DisabledTestCaseFactory<BocTreeViewSelector, BocTreeViewControlObject>))]
    [TestCaseSource(typeof(LabelTestCaseFactory<BocTreeViewSelector, BocTreeViewControlObject>))]
    public void GenericTests (GenericSelectorTestAction<BocTreeViewSelector, BocTreeViewControlObject> testAction)
    {
      testAction(Helper, e => e.TreeViews(), "treeView");
    }

    [Test]
    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<BocTreeViewSelector, BocTreeViewControlObject>))]
    [TestCaseSource(typeof(IndexControlSelectorTestCaseFactory<BocTreeViewSelector, BocTreeViewControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<BocTreeViewSelector, BocTreeViewControlObject>))]
    [TestCaseSource(typeof(FirstControlSelectorTestCaseFactory<BocTreeViewSelector, BocTreeViewControlObject>))]
    [TestCaseSource(typeof(SingleControlSelectorTestCaseFactory<BocTreeViewSelector, BocTreeViewControlObject>))]
    [TestCaseSource(typeof(DomainPropertyControlSelectorTestCaseFactory<BocTreeViewSelector, BocTreeViewControlObject>))]
    [TestCaseSource(typeof(DisplayNameControlSelectorTestCaseFactory<BocTreeViewSelector, BocTreeViewControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<BocTreeViewSelector, BocTreeViewControlObject> testAction)
    {
      testAction(Helper, e => e.TreeViews(), "treeView");
    }

    [Category("Screenshot")]
    [Test]
    public void WebTreeView ()
    {
      ScreenshotTestingDelegate<FluentScreenshotElement<ScreenshotBocTreeViewNodeControlObject>> test = (builder, target) =>
      {
        builder.AnnotateBox(target, Pens.Red, WebPadding.Inner);
        builder.AnnotateBox(target.GetLabel(), Pens.Green, WebPadding.Inner);
        builder.AnnotateBox(target.GetChildren(), Pens.Blue, WebPadding.Inner);

        builder.Crop(target, new WebPadding(1));
      };

      var home = Start();
      var webTreeView = home.TreeViews().GetByLocalID("NoTopLevelExpander");
      var fluentNode = webTreeView.GetNode().WithIndex(1).ForScreenshot();

      Helper.RunScreenshotTest<FluentScreenshotElement<ScreenshotBocTreeViewNodeControlObject>, BocTreeViewControlObjectTest>(
          fluentNode,
          ScreenshotTestingType.Both,
          test);
    }

    [Category("Screenshot")]
    [Test]
    public void WebTreeViewScreenshot_WithMarkedBadge ()
    {
      ScreenshotTestingDelegate<FluentScreenshotElement<ScreenshotBocTreeViewNodeControlObject>> test = (builder, target) =>
      {
        builder.AnnotateBox(target.GetBadge(), Pens.Blue, WebPadding.Inner);

        builder.Crop(target, new WebPadding(1));
      };

      var home = Start();
      var webTreeView = home.TreeViews().GetByLocalID("ContextMenu_Person");

      var rootNode = webTreeView.GetNode().WithIndex(1);
      rootNode.Expand();

      var fluentNode = rootNode.GetNode(1).ForScreenshot();

      Helper.RunScreenshotTest<FluentScreenshotElement<ScreenshotBocTreeViewNodeControlObject>, BocTreeViewControlObjectTest>(
          fluentNode,
          ScreenshotTestingType.Both,
          test);
    }

    [Test]
    public void WebTreeView_WithDerivedControlObject ()
    {
      var home = Start();
      var controlObject = new DerivedBocTreeViewControlObject(home.TreeViews().GetByLocalID("NoTopLevelExpander").Context);
      var fluentControlObject = controlObject.ForControlObjectScreenshot();
      Assert.That(fluentControlObject, Is.Not.Null);

      var fluentNode = controlObject.GetNode().WithIndex(1).ForScreenshot();
      var derivedNode = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocTreeViewNodeControlObject(fluentNode.GetTarget().FluentBocTreeViewNode, fluentNode.GetTarget().FluentElement));

      Assert.That(derivedNode.GetChildren(), Is.Not.Null);
      Assert.That(derivedNode.GetLabel(), Is.Not.Null);
    }

    private class DerivedBocTreeViewControlObject : BocTreeViewControlObject
    {
      public DerivedBocTreeViewControlObject (ControlObjectContext context)
          : base(context)
      {
      }
    }

    private class DerivedScreenshotBocTreeViewNodeControlObject : ScreenshotBocTreeViewNodeControlObject
    {
      public DerivedScreenshotBocTreeViewNodeControlObject (
          IFluentScreenshotElementWithCovariance<BocTreeViewNodeControlObject> fluentBocTreeViewNode,
          IFluentScreenshotElement<ElementScope> fluentElement)
          : base(fluentBocTreeViewNode, fluentElement)
      {
      }
    }

    [Test]
    public void TestIsReadOnly ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");
      Assert.That(bocTreeView.IsReadOnly(), Is.True);
    }

    [Test]
    public void TestGetNode ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");

      var rootNode = bocTreeView.GetNode().WithIndex(1).Expand();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.Empty);

      rootNode.GetNode("c8ace752-55f6-4074-8890-130276ea6cd1").Select();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("c8ace752-55f6-4074-8890-130276ea6cd1|B, A"));

      rootNode.GetNode(3).Select();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("9e713934-1226-4669-880e-c07c22cdab19|B, C"));

      rootNode.GetNode().WithItemID("c8ace752-55f6-4074-8890-130276ea6cd1").Select();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("c8ace752-55f6-4074-8890-130276ea6cd1|B, A"));

      rootNode.GetNode().WithIndex(3).Select();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("9e713934-1226-4669-880e-c07c22cdab19|B, C"));

      rootNode.GetNode().WithDisplayText("B, B").Select();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("a97d84b0-c1c9-4580-a6c1-1fed1ee8c041|B, B"));

      rootNode.GetNode().WithDisplayTextContains(", B").Select();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("a97d84b0-c1c9-4580-a6c1-1fed1ee8c041|B, B"));
    }

    [Test]
    public void TestTreeGetNode ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");

      bocTreeView.GetNode().WithIndex(1).Select();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("00000000-0000-0000-0000-000000000001|Doe, John"));
      bocTreeView.GetNode(1).Select();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("00000000-0000-0000-0000-000000000001|Doe, John"));
      Assert.That(() => bocTreeView.GetNode().WithDisplayText("Doe, John").Select(), Throws.Nothing);
      Assert.That(() => bocTreeView.GetNode().WithDisplayTextContains("Doe").Select(), Throws.Nothing);
      Assert.That(() => bocTreeView.GetNode().WithItemID("00000000-0000-0000-0000-000000000001").Select(), Throws.Nothing);
      Assert.That(() => bocTreeView.GetNode("00000000-0000-0000-0000-000000000001").Select(), Throws.Nothing);
    }

    [Test]
    public void TestGetNodeOnNoTopLevelExpander ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("NoTopLevelExpander");

      var rootNode = bocTreeView.GetNode().WithIndex(1);
      Assert.That(home.Scope.FindIdEndingWith("NoTopLevelExpanderSelectedNodeLabel").Text, Is.Empty);

      rootNode.GetNode("c8ace752-55f6-4074-8890-130276ea6cd1").Select();
      Assert.That(home.Scope.FindIdEndingWith("NoTopLevelExpanderSelectedNodeLabel").Text, Is.EqualTo("c8ace752-55f6-4074-8890-130276ea6cd1|B, A"));
    }

    [Test]
    public void TestNodeGetText ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");
      var node = bocTreeView.GetNode().WithIndex(1);

      Assert.That(node.GetText(), Is.EqualTo("Doe, John"));
    }

    [Test]
    public void TestNodeSelectOnlyChildren ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");
      var rootNode = bocTreeView.GetNode().WithIndex(1);

      rootNode.Expand();
      var firstChildOfRootNode = rootNode.GetNode().WithDisplayTextContains("B, A");
      firstChildOfRootNode.Expand();

      bocTreeView.Scope.ElementFinder.Options.Timeout = TimeSpan.Zero;
      Assert.That(
          () => bocTreeView.GetNode().WithDisplayTextContains("B, A").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(Driver, "Unable to find xpath: ((.//ul)[1])/li[contains(@data-content, 'B, A')]").Message));
      Assert.That(
          () => bocTreeView.GetNode().WithDisplayText("B, A").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(Driver, "Unable to find xpath: ((.//ul)[1])/li[@data-content='B, A']").Message));
      Assert.That(
          () => bocTreeView.GetNode().WithItemID("c8ace752-55f6-4074-8890-130276ea6cd1").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(
                      Driver,
                      "Unable to find xpath: ((.//ul)[1])/li[@data-item-id='c8ace752-55f6-4074-8890-130276ea6cd1']").Message));
      Assert.That(
          () => bocTreeView.GetNode("c8ace752-55f6-4074-8890-130276ea6cd1").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(
                      Driver,
                      "Unable to find xpath: ((.//ul)[1])/li[@data-item-id='c8ace752-55f6-4074-8890-130276ea6cd1']").Message));

      rootNode.Scope.ElementFinder.Options.Timeout = TimeSpan.Zero;
      Assert.That(
          () => rootNode.GetNode().WithDisplayTextContains("A, B").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(Driver, "Unable to find xpath: ((.//ul)[1])/li[contains(@data-content, 'A, B')]").Message));
      Assert.That(
          () => rootNode.GetNode().WithDisplayText("A, B").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(Driver, "Unable to find xpath: ((.//ul)[1])/li[@data-content='A, B']").Message));
      Assert.That(
          () => rootNode.GetNode().WithItemID("eb94bfdb-1140-46f8-971f-e4b41dae13b8").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(
                      Driver,
                      "Unable to find xpath: ((.//ul)[1])/li[@data-item-id='eb94bfdb-1140-46f8-971f-e4b41dae13b8']").Message));
      Assert.That(
          () => rootNode.GetNode("eb94bfdb-1140-46f8-971f-e4b41dae13b8").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(
                      Driver,
                      "Unable to find xpath: ((.//ul)[1])/li[@data-item-id='eb94bfdb-1140-46f8-971f-e4b41dae13b8']").Message));
    }

    [Test]
    public void TestSelectNodeWithIndexInHierarchy ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("NoPropertyIdentifier");
      var rootNode = bocTreeView.GetNode().WithIndex(1);
      rootNode.Expand();
      rootNode.GetNode("Children").Expand();

      rootNode.GetNodeInHierarchy().WithIndex(4).Select();
      Assert.That(home.Scope.FindIdEndingWith("NoPropertyIdentifierSelectedNodeLabel").Text, Is.EqualTo("88ca0bf3-2a08-4d93-bee7-0454052a922d|C, A"));

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
    public void TestSelectNodeInHierarchy ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");
      var rootNode = bocTreeView.GetNode().WithIndex(1);
      rootNode.Expand();
      var firstChildOfRootNode = rootNode.GetNode().WithDisplayTextContains("B, A");
      firstChildOfRootNode.Expand();

      rootNode.GetNodeInHierarchy().WithDisplayTextContains("A, B").Select();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("eb94bfdb-1140-46f8-971f-e4b41dae13b8|A, B"));

      rootNode.GetNodeInHierarchy().WithDisplayText("E, ").Select();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("6866ca48-8957-4f26-ae5f-78a3f6dcc4de|E,"));

      rootNode.GetNodeInHierarchy().WithItemID("eb94bfdb-1140-46f8-971f-e4b41dae13b8").Select();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("eb94bfdb-1140-46f8-971f-e4b41dae13b8|A, B"));

      rootNode.GetNodeInHierarchy("6866ca48-8957-4f26-ae5f-78a3f6dcc4de").Select();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("6866ca48-8957-4f26-ae5f-78a3f6dcc4de|E,"));
    }

    [Test]
    public void TestSelectNodeInHierarchyOnlyRootNodeExpanded ()
    {
      var expectedExceptionMessage =
          AssertionExceptionUtility.CreateExpectationException(
                  Driver,
                  "The element cannot be found: This element has been removed from the DOM. Coypu will normally re-find elements using the original locators in this situation, "
                  + "except if you have captured a snapshot list of all matching elements using FindAllCss() or FindAllXPath()")
              .Message;
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");
      var rootNode = bocTreeView.GetNode().WithIndex(1);
      rootNode.Expand();

      rootNode.Scope.ElementFinder.Options.Timeout = TimeSpan.Zero;
      Assert.That(
          () => rootNode.GetNodeInHierarchy().WithDisplayText("E, ").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(expectedExceptionMessage));
      Assert.That(
          () => rootNode.GetNodeInHierarchy().WithItemID("eb94bfdb-1140-46f8-971f-e4b41dae13b8").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(expectedExceptionMessage));
      Assert.That(
          () => rootNode.GetNodeInHierarchy().WithDisplayTextContains("A, B").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(expectedExceptionMessage));
      Assert.That(
          () => rootNode.GetNodeInHierarchy("6866ca48-8957-4f26-ae5f-78a3f6dcc4de").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(expectedExceptionMessage));
    }

    [Test]
    public void TestTreeSelectNodeInHierarchy ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");
      var rootNode = bocTreeView.GetNode().WithIndex(1);
      rootNode.Expand();
      var firstChildOfRootNode = rootNode.GetNode().WithDisplayTextContains("B, A");
      firstChildOfRootNode.Expand();

      bocTreeView.GetNodeInHierarchy().WithDisplayTextContains("A, B").Select();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("eb94bfdb-1140-46f8-971f-e4b41dae13b8|A, B"));

      bocTreeView.GetNodeInHierarchy().WithDisplayText("E, ").Select();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("6866ca48-8957-4f26-ae5f-78a3f6dcc4de|E,"));

      bocTreeView.GetNodeInHierarchy().WithItemID("eb94bfdb-1140-46f8-971f-e4b41dae13b8").Select();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("eb94bfdb-1140-46f8-971f-e4b41dae13b8|A, B"));

      bocTreeView.GetNodeInHierarchy("6866ca48-8957-4f26-ae5f-78a3f6dcc4de").Select();
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("6866ca48-8957-4f26-ae5f-78a3f6dcc4de|E,"));
    }

    [Test]
    public void TestTreeSelectNodeWithIndexInHierarchy ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("NoPropertyIdentifier");
      var rootNode = bocTreeView.GetNode().WithIndex(1);
      rootNode.Expand();
      rootNode.GetNode("Children").Expand();

      bocTreeView.GetNodeInHierarchy().WithIndex(4).Select();
      Assert.That(home.Scope.FindIdEndingWith("NoPropertyIdentifierSelectedNodeLabel").Text, Is.EqualTo("88ca0bf3-2a08-4d93-bee7-0454052a922d|C, A"));

      Assert.That(
          () => bocTreeView.GetNodeInHierarchy().WithIndex(999),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(AssertionExceptionUtility.CreateExpectationException(Driver, "No node with the index '999' was found.").Message));

      Assert.That(
          () => bocTreeView.GetNodeInHierarchy().WithIndex(1),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(AssertionExceptionUtility.CreateExpectationException(Driver, "Multiple nodes with the index '1' were found.").Message));
    }

    [Test]
    public void TestTreeSelectNodeInHierarchyOnlyRootNodeExpanded ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");
      var rootNode = bocTreeView.GetNode().WithIndex(1);
      rootNode.Expand();

      bocTreeView.Scope.ElementFinder.Options.Timeout = TimeSpan.Zero;
      Assert.That(
          () => bocTreeView.GetNodeInHierarchy().WithDisplayTextContains("A, B").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(Driver, "Unable to find css: ul li[data-content*='A, B']").Message));
      Assert.That(
          () => bocTreeView.GetNodeInHierarchy().WithDisplayText("E, ").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(Driver, "Unable to find css: ul li[data-content='E, ']").Message));
      Assert.That(
          () => bocTreeView.GetNodeInHierarchy().WithItemID("eb94bfdb-1140-46f8-971f-e4b41dae13b8").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(Driver, "Unable to find css: ul li[data-item-id='eb94bfdb-1140-46f8-971f-e4b41dae13b8']").Message));
      Assert.That(
          () => bocTreeView.GetNodeInHierarchy("6866ca48-8957-4f26-ae5f-78a3f6dcc4de").Select(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateControlMissingException(Driver, "Unable to find css: ul li[data-item-id='6866ca48-8957-4f26-ae5f-78a3f6dcc4de']").Message));
    }

    [Test]
    public void TestNodeIsSelected ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");
      var rootNode = bocTreeView.GetNode().WithIndex(1);
      Assert.That(rootNode.IsSelected(), Is.False);

      rootNode.Select();
      Assert.That(rootNode.IsSelected(), Is.True);

      var bANode = rootNode.Expand().GetNode("c8ace752-55f6-4074-8890-130276ea6cd1");
      var aBNode = bANode.Expand().GetNode("eb94bfdb-1140-46f8-971f-e4b41dae13b8").Select();

      Assert.That(rootNode.IsSelected(), Is.False);
      Assert.That(bANode.IsSelected(), Is.False);
      Assert.That(aBNode.IsSelected(), Is.True);
    }

    [Test]
    public void TestGetNumberOfChildren ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");
      var rootNode = bocTreeView.GetNode().WithIndex(1);
      Assert.That(rootNode.GetNumberOfChildren(), Is.EqualTo(6));

      var bANode = rootNode.Expand().GetNode("c8ace752-55f6-4074-8890-130276ea6cd1");
      Assert.That(bANode.GetNumberOfChildren(), Is.EqualTo(2));

      var child = bANode.Expand().GetNode(1);
      Assert.That(child.GetNumberOfChildren(), Is.EqualTo(0));
    }

    [Test]
    public void TestNodeEnsureExpanded ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");

      var rootNode = bocTreeView.GetNode().WithIndex(1);
      rootNode.Expand();

      Assert.That(
          () => rootNode.Expand(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(AssertionExceptionUtility.CreateExpectationException(Driver, "TreeViewNode is already expanded.").Message));
    }

    [Test]
    public void TestNodeExpand ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");
      var node = bocTreeView.GetNode().WithIndex(1).Expand();
      node = node.GetNode("c8ace752-55f6-4074-8890-130276ea6cd1");
      var completionDetection = new CompletionDetectionStrategyTestHelper(node);
      node.Expand();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.Empty);

      node.GetNode("eb94bfdb-1140-46f8-971f-e4b41dae13b8").Select();

      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("eb94bfdb-1140-46f8-971f-e4b41dae13b8|A, B"));
    }

    [Test]
    public void TestEvaluatedNodeExpanded ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");

      var rootNode = bocTreeView.GetNode().WithIndex(1);
      Assert.That(rootNode.IsExpanded(), Is.EqualTo(false));

      rootNode.Expand();
      Assert.That(rootNode.IsExpanded(), Is.EqualTo(true));
    }

    [Test]
    public void TestEvaluatedNodeEvaluated ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("NoLookAheadEvaluation");

      var rootNode = bocTreeView.GetNode().WithIndex(1);
      Assert.That(rootNode.IsEvaluated(), Is.EqualTo(false));

      rootNode.Expand();
      Assert.That(rootNode.IsEvaluated(), Is.EqualTo(true));
    }

    [Test]
    public void TestEvaluatedNodeExpandable ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");

      var rootNode = bocTreeView.GetNode().WithIndex(1).Expand();

      var node = rootNode.GetNode().WithDisplayText("B, C");
      Assert.That(node.IsExpandable(), Is.EqualTo(false));

      Assert.That(
          () => node.Expand(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "The WebTreeViewNode cannot be expanded as it has no children.").Message));
      Assert.That(
          () => node.Collapse(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(
                  AssertionExceptionUtility.CreateExpectationException(Driver, "The WebTreeViewNode cannot be collapsed as it has no children.").Message));
    }

    [Test]
    public void TestNodeExpandOnNoLookAheadEvaluation ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("NoLookAheadEvaluation");
      var node = bocTreeView.GetNode().WithIndex(1).Expand();
      node = node.GetNode("c8ace752-55f6-4074-8890-130276ea6cd1").Expand();
      node = node.GetNode("eb94bfdb-1140-46f8-971f-e4b41dae13b8").Expand();
      Assert.That(home.Scope.FindIdEndingWith("NoLookAheadEvaluationSelectedNodeLabel").Text, Is.Empty);

      node.Select();

      Assert.That(
          home.Scope.FindIdEndingWith("NoLookAheadEvaluationSelectedNodeLabel").Text,
          Is.EqualTo("eb94bfdb-1140-46f8-971f-e4b41dae13b8|A, B"));
    }

    [Test]
    public void TestNodeEnsureCollapsed ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");

      var rootNode = bocTreeView.GetNode().WithIndex(1);

      Assert.That(
          () => rootNode.Collapse(),
          Throws.InstanceOf<WebTestException>()
              .With.Message.EqualTo(AssertionExceptionUtility.CreateExpectationException(Driver, "TreeViewNode is already collapsed.").Message));
    }

    [Test]
    public void TestNodeCollapse ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");

      var node = bocTreeView.GetNode().WithIndex(1).Expand();
      var completionDetection = new CompletionDetectionStrategyTestHelper(node);
      node = node.Collapse();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      node.Expand().GetNode("c8ace752-55f6-4074-8890-130276ea6cd1").Select();

      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("c8ace752-55f6-4074-8890-130276ea6cd1|B, A"));
    }

    [Test]
    public void TestNodeSelect ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");

      var node = bocTreeView.GetNode().WithIndex(1);
      var completionDetection = new CompletionDetectionStrategyTestHelper(node);
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.Empty);

      node.Select();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("00000000-0000-0000-0000-000000000001|Doe, John"));
    }

    [Test]
    public void TestNodeClick ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");

      var node = bocTreeView.GetNode().WithIndex(1);
      var completionDetection = new CompletionDetectionStrategyTestHelper(node);
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.Empty);

      node.Click();
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindIdEndingWith("NormalSelectedNodeLabel").Text, Is.EqualTo("00000000-0000-0000-0000-000000000001|Doe, John"));
    }

    [Test]
    public void TestNodeSelectOnNoPropertyIdentifier ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("NoPropertyIdentifier");

      var node = bocTreeView.GetNode().WithIndex(1).Expand();

      node = node.GetNode("Children").Expand().GetNode("c8ace752-55f6-4074-8890-130276ea6cd1").Select();
      Assert.That(
          home.Scope.FindIdEndingWith("NoPropertyIdentifierSelectedNodeLabel").Text,
          Is.EqualTo("c8ace752-55f6-4074-8890-130276ea6cd1|B, A"));

      node.Expand().GetNode("Jobs").Select();
      Assert.That(home.Scope.FindIdEndingWith("NoPropertyIdentifierSelectedNodeLabel").Text, Is.EqualTo("Jobs|Jobs"));
    }

    [Test]
    public void TestNodeContextMenu ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");
      var node = bocTreeView.GetNode().WithIndex(1);
      node = node.Expand().GetNode("c8ace752-55f6-4074-8890-130276ea6cd1").Select();

      node.GetContextMenu().SelectItem("MenuItem");

      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderLabel").Text, Is.EqualTo("Normal_Boc_TreeView"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedLabel").Text, Is.EqualTo("NodeContextMenuClick"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedParameterLabel").Text, Is.EqualTo("c8ace752-55f6-4074-8890-130276ea6cd1|B, A"));
    }

    [Test]
    public void TestContextMenuControlObject_SelectItem_AfterGetItemDefinitions ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");
      var node = bocTreeView.GetNode().WithIndex(1);
      node = node.Expand().GetNode("c8ace752-55f6-4074-8890-130276ea6cd1").Select();

      var contextMenu = node.GetContextMenu();

      contextMenu.GetItemDefinitions();

      Assert.That(() => contextMenu.SelectItem("MenuItem"), Throws.Nothing);
    }

    [Test]
    public void TestContextMenuControlObject_IsOpen ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");
      var node = bocTreeView.GetNode().WithIndex(1);
      node = node.Expand().GetNode("c8ace752-55f6-4074-8890-130276ea6cd1").Select();

      var contextMenu = node.GetContextMenu();

      Assert.That(contextMenu.IsOpen(), Is.False);
    }

    [Test]
    public void TestContextMenuControlObject_OpenDropDownMenu ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");
      var node = bocTreeView.GetNode().WithIndex(1);
      node = node.Expand().GetNode("c8ace752-55f6-4074-8890-130276ea6cd1").Select();

      var contextMenu = node.GetContextMenu();

      contextMenu.Open();
      Assert.That(contextMenu.IsOpen(), Is.True);

      //Open a second time to ensure it does not close it
      contextMenu.Open();
      Assert.That(contextMenu.IsOpen(), Is.True);
    }

    [Test]
    public void TestContextMenuControlObject_OpenDropDownMenuWithDelay_WaitsUntilDropDownIsOpen ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("ContextMenu_Delayed");
      var node = bocTreeView.GetNode().WithIndex(1);

      var contextMenu = node.GetContextMenu();

      contextMenu.Open();
      Assert.That(contextMenu.IsOpen(), Is.True);
    }

    [Test]
    [Category("LongRunning")]
    public void TestContextMenuControlObject_OpenDropDownMenuWithDelayGreaterThanTimeout_FailsWithException ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("ContextMenu_DelayedLongerThanTimeout");
      var node = bocTreeView.GetNode().WithIndex(1);

      var contextMenu = node.GetContextMenu();

      Assert.That(
          () => contextMenu.Open(),
          Throws.TypeOf<WebTestException>()
              .With.Message.EqualTo(AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to open the menu.").Message));
      Assert.That(contextMenu.IsOpen(), Is.False);
    }

    [Test]
    public void TestContextMenuControlObject_OpenDropDownMenuWithError_FailsWithException ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("ContextMenu_Error");
      var node = bocTreeView.GetNode().WithIndex(1);

      var contextMenu = node.GetContextMenu();

      Assert.That(
          () => contextMenu.Open(),
          Throws.TypeOf<WebTestException>()
              .With.Message.EqualTo(AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to open the menu.").Message));
      Assert.That(contextMenu.IsOpen(), Is.False);
    }

    [Test]
    public void TestContextMenuControlObject_CloseDropDownMenu ()
    {
      var home = Start();

      var bocTreeView = home.TreeViews().GetByLocalID("Normal");
      var node = bocTreeView.GetNode().WithIndex(1);
      node = node.Expand().GetNode("c8ace752-55f6-4074-8890-130276ea6cd1").Select();

      var contextMenu = node.GetContextMenu();

      contextMenu.Close();
      Assert.That(contextMenu.IsOpen(), Is.False);

      //Close it a second time to ensure it stays closed
      contextMenu.Close();
      Assert.That(contextMenu.IsOpen(), Is.False);
    }

    private WxePageObject Start ()
    {
      return Start("BocTreeView");
    }
  }
}
