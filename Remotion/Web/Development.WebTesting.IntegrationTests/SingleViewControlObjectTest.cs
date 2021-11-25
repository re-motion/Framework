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
  public class SingleViewControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<SingleViewSelector, SingleViewControlObject>))]
    [TestCaseSource(typeof(IndexControlSelectorTestCaseFactory<SingleViewSelector, SingleViewControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<SingleViewSelector, SingleViewControlObject>))]
    [TestCaseSource(typeof(FirstControlSelectorTestCaseFactory<SingleViewSelector, SingleViewControlObject>))]
    [TestCaseSource(typeof(SingleControlSelectorTestCaseFactory<SingleViewSelector, SingleViewControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<SingleViewSelector, SingleViewControlObject> testAction)
    {
      testAction(Helper, e => e.SingleViews(), "singleView");
    }

    [Test]
    public void TestSubScope_TopControls ()
    {
      var home = Start();

      var singleView = home.SingleViews().Single();

      var topControls = singleView.GetTopControls();
      Assert.That(topControls.Scope.Text, Does.Contain("TopControls"));
      Assert.That(topControls.Scope.Text, Does.Not.Contains("Content"));
    }

    [Test]
    public void TestSubScope_View ()
    {
      var home = Start();

      var singleView = home.SingleViews().Single();

      var view = singleView.GetView();
      Assert.That(view.Scope.Text, Does.Contain("Content"));
      Assert.That(view.Scope.Text, Does.Not.Contains("TopControls"));
      Assert.That(view.Scope.Text, Does.Not.Contains("BottomControls"));
    }

    [Test]
    public void TestSubScope_BottomControls ()
    {
      var home = Start();

      var singleView = home.SingleViews().Single();

      var bottomControls = singleView.GetBottomControls();
      Assert.That(bottomControls.Scope.Text, Does.Contain("BottomControls"));
      Assert.That(bottomControls.Scope.Text, Does.Not.Contains("Content"));
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject>("SingleViewTest.aspx");
    }
  }
}
