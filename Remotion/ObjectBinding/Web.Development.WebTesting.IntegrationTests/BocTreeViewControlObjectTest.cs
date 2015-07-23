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
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocTreeViewControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByID ("body_DataEditControl_Normal");
      Assert.That (bocTreeView.Scope.Id, Is.EqualTo ("body_DataEditControl_Normal"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByIndex (2);
      Assert.That (bocTreeView.Scope.Id, Is.EqualTo ("body_DataEditControl_NoTopLevelExpander"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByLocalID ("Normal");
      Assert.That (bocTreeView.Scope.Id, Is.EqualTo ("body_DataEditControl_Normal"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().First();
      Assert.That (bocTreeView.Scope.Id, Is.EqualTo ("body_DataEditControl_Normal"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();

      try
      {
        home.GetTreeView().Single();
        Assert.Fail ("Should not be able to unambigously find a BOC tree view.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSelection_DisplayName ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByDisplayName ("Children");
      Assert.That (bocTreeView.Scope.Id, Is.EqualTo ("body_DataEditControl_Normal"));
    }

    [Test]
    public void TestSelection_DomainProperty ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByDomainProperty ("Children");
      Assert.That (bocTreeView.Scope.Id, Is.EqualTo ("body_DataEditControl_Normal"));

      bocTreeView = home.GetTreeView().ByDomainProperty ("null");
      Assert.That (bocTreeView.Scope.Id, Is.EqualTo ("body_DataEditControl_NoPropertyIdentifier"));
    }

    [Test]
    public void TestSelection_DomainPropertyAndClass ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByDomainProperty ("Children", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocTreeView.Scope.Id, Is.EqualTo ("body_DataEditControl_Normal"));

      bocTreeView = home.GetTreeView().ByDomainProperty ("null", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocTreeView.Scope.Id, Is.EqualTo ("body_DataEditControl_NoPropertyIdentifier"));
    }

    [Test]
    public void TestIsReadOnly ()
    {
      var home = Start();

      var bocTreeView = home.GetTextValue().ByLocalID ("Normal");
      Assert.That (bocTreeView.IsReadOnly(), Is.True);
    }

    [Test]
    public void TestGetNode ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByLocalID ("Normal");

      var rootNode = bocTreeView.GetRootNode().Expand();
      Assert.That (home.Scope.FindIdEndingWith ("NormalSelectedNodeLabel").Text, Is.Empty);

      rootNode.GetNode ("c8ace752-55f6-4074-8890-130276ea6cd1").Select();
      Assert.That (home.Scope.FindIdEndingWith ("NormalSelectedNodeLabel").Text, Is.EqualTo ("c8ace752-55f6-4074-8890-130276ea6cd1|B, A"));

      rootNode.GetNode (3).Select();
      Assert.That (home.Scope.FindIdEndingWith ("NormalSelectedNodeLabel").Text, Is.EqualTo ("9e713934-1226-4669-880e-c07c22cdab19|B, C"));

      rootNode.GetNode().WithItemID ("c8ace752-55f6-4074-8890-130276ea6cd1").Select();
      Assert.That (home.Scope.FindIdEndingWith ("NormalSelectedNodeLabel").Text, Is.EqualTo ("c8ace752-55f6-4074-8890-130276ea6cd1|B, A"));

      rootNode.GetNode().WithIndex (3).Select();
      Assert.That (home.Scope.FindIdEndingWith ("NormalSelectedNodeLabel").Text, Is.EqualTo ("9e713934-1226-4669-880e-c07c22cdab19|B, C"));

      rootNode.GetNode().WithDisplayText ("B, B").Select();
      Assert.That (home.Scope.FindIdEndingWith ("NormalSelectedNodeLabel").Text, Is.EqualTo ("a97d84b0-c1c9-4580-a6c1-1fed1ee8c041|B, B"));

      rootNode.GetNode().WithDisplayTextContains (", B").Select();
      Assert.That (home.Scope.FindIdEndingWith ("NormalSelectedNodeLabel").Text, Is.EqualTo ("a97d84b0-c1c9-4580-a6c1-1fed1ee8c041|B, B"));
    }

    [Test]
    public void TestGetNodeOnNoTopLevelExpander ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByLocalID ("NoTopLevelExpander");

      var rootNode = bocTreeView.GetRootNode();
      Assert.That (home.Scope.FindIdEndingWith ("NoTopLevelExpanderSelectedNodeLabel").Text, Is.Empty);

      rootNode.GetNode ("c8ace752-55f6-4074-8890-130276ea6cd1").Select();
      Assert.That (home.Scope.FindIdEndingWith ("NoTopLevelExpanderSelectedNodeLabel").Text, Is.EqualTo ("c8ace752-55f6-4074-8890-130276ea6cd1|B, A"));
    }

    [Test]
    public void TestNodeGetText ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByLocalID ("Normal");
      var node = bocTreeView.GetRootNode();

      Assert.That (node.GetText(), Is.EqualTo ("Doe, John"));
    }

    [Test]
    public void TestNodeIsSelected ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByLocalID ("Normal");
      var rootNode = bocTreeView.GetRootNode();
      Assert.That (rootNode.IsSelected(), Is.False);

      rootNode.Select();
      Assert.That (rootNode.IsSelected(), Is.True);

      var bANode = rootNode.Expand().GetNode ("c8ace752-55f6-4074-8890-130276ea6cd1");
      var aBNode = bANode.Expand().GetNode ("eb94bfdb-1140-46f8-971f-e4b41dae13b8").Select();

      Assert.That (rootNode.IsSelected(), Is.False);
      Assert.That (bANode.IsSelected(), Is.False);
      Assert.That (aBNode.IsSelected(), Is.True);
    }

    [Test]
    public void TestGetNumberOfChildren ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByLocalID ("Normal");
      var rootNode = bocTreeView.GetRootNode();
      Assert.That (rootNode.GetNumberOfChildren(), Is.EqualTo (6));

      var bANode = rootNode.Expand().GetNode ("c8ace752-55f6-4074-8890-130276ea6cd1");
      Assert.That (bANode.GetNumberOfChildren(), Is.EqualTo (1));
    }

    [Test]
    public void TestNodeExpand ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByLocalID ("Normal");
      var node = bocTreeView.GetRootNode().Expand();
      node = node.GetNode ("c8ace752-55f6-4074-8890-130276ea6cd1").Expand();
      Assert.That (home.Scope.FindIdEndingWith ("NormalSelectedNodeLabel").Text, Is.Empty);

      node.GetNode ("eb94bfdb-1140-46f8-971f-e4b41dae13b8").Select();

      Assert.That (home.Scope.FindIdEndingWith ("NormalSelectedNodeLabel").Text, Is.EqualTo ("eb94bfdb-1140-46f8-971f-e4b41dae13b8|A, B"));
    }

    [Test]
    public void TestNodeExpandOnNoLookAheadEvaluation ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByLocalID ("NoLookAheadEvaluation");
      var node = bocTreeView.GetRootNode().Expand();
      node = node.GetNode ("c8ace752-55f6-4074-8890-130276ea6cd1").Expand();
      node = node.GetNode ("eb94bfdb-1140-46f8-971f-e4b41dae13b8").Expand();
      Assert.That (home.Scope.FindIdEndingWith ("NoLookAheadEvaluationSelectedNodeLabel").Text, Is.Empty);

      node.Select();

      Assert.That (
          home.Scope.FindIdEndingWith ("NoLookAheadEvaluationSelectedNodeLabel").Text,
          Is.EqualTo ("eb94bfdb-1140-46f8-971f-e4b41dae13b8|A, B"));
    }

    [Test]
    public void TestNodeCollapse ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByLocalID ("Normal");

      bocTreeView.GetRootNode().Expand().Collapse().Expand().GetNode ("c8ace752-55f6-4074-8890-130276ea6cd1").Select();

      Assert.That (home.Scope.FindIdEndingWith ("NormalSelectedNodeLabel").Text, Is.EqualTo ("c8ace752-55f6-4074-8890-130276ea6cd1|B, A"));
    }

    [Test]
    public void TestNodeSelect ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByLocalID ("Normal");

      var node = bocTreeView.GetRootNode();
      Assert.That (home.Scope.FindIdEndingWith ("NormalSelectedNodeLabel").Text, Is.Empty);

      node.Select();
      Assert.That (home.Scope.FindIdEndingWith ("NormalSelectedNodeLabel").Text, Is.EqualTo ("00000000-0000-0000-0000-000000000001|Doe, John"));
    }

    [Test]
    public void TestNodeSelectOnNoPropertyIdentifier ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByLocalID ("NoPropertyIdentifier");

      var node = bocTreeView.GetRootNode().Expand();

      node = node.GetNode ("Children").Expand().GetNode ("c8ace752-55f6-4074-8890-130276ea6cd1").Select();
      Assert.That (
          home.Scope.FindIdEndingWith ("NoPropertyIdentifierSelectedNodeLabel").Text,
          Is.EqualTo ("c8ace752-55f6-4074-8890-130276ea6cd1|B, A"));

      node.Expand().GetNode ("Jobs").Select();
      Assert.That (home.Scope.FindIdEndingWith ("NoPropertyIdentifierSelectedNodeLabel").Text, Is.EqualTo ("Jobs|Jobs"));
    }

    [Test]
    public void TestNodeContextMenu ()
    {
      var home = Start();

      var bocTreeView = home.GetTreeView().ByLocalID ("Normal");
      var node = bocTreeView.GetRootNode();
      node = node.Expand().GetNode ("c8ace752-55f6-4074-8890-130276ea6cd1").Select();

      node.OpenContextMenu().SelectItem ("MenuItem");

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("Normal_Boc_TreeView"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("NodeContextMenuClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("c8ace752-55f6-4074-8890-130276ea6cd1|B, A"));
    }

    private WxePageObject Start ()
    {
      return Start ("BocTreeView");
    }
  }
}