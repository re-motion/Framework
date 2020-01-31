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
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests;
using Remotion.Web.Development.WebTesting.Accessibility;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.ObjectBinding.Web.IntegrationTests.BocTreeView
{
  [TestFixture]
  public class AccessibilityBocTreeViewTest : IntegrationTest
  {
    [Test]
    public void Normal ()
    {
      var home = Start();
      var bocTreeView = home.TreeViews().GetByLocalID ("Normal");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocTreeView.Analyze (analyzer);

      // TODO RM-7335 remove ignore once issue is resolved
      var violations = result.Violations.IgnoreByRuleIDAndXPath (AccessibilityRuleID.AriaAllowedAttr, "/a[@id='Head_E56F9F6BC665123527D1124D63684664']");
      Assert.That (violations, Is.Empty);
    }

    [Test]
    public void NoTopLevelExpander ()
    {
      var home = Start();
      var bocTreeView = home.TreeViews().GetByLocalID ("NoTopLevelExpander");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocTreeView.Analyze (analyzer);

      // TODO RM-7335 remove ignore once issue is resolved
      var violations = result.Violations.IgnoreByRuleID (AccessibilityRuleID.AriaAllowedAttr);
      Assert.That (violations, Is.Empty);
    }

    [Test]
    public void NoPropertyIdentifier ()
    {
      var home = Start();
      var bocTreeView = home.TreeViews().GetByLocalID ("NoPropertyIdentifier");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocTreeView.Analyze (analyzer);

      Assert.That (result.Violations, Is.Empty);
    }

    [Test]
    public void ContextMenu ()
    {
      var home = Start();
      var bocTreeView = home.TreeViews().GetByLocalID ("ContextMenu_Delayed");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocTreeView.Analyze (analyzer);

      // TODO RM-7335 remove ignore once issue is resolved
      var violations = result.Violations.IgnoreByRuleIDAndXPath (AccessibilityRuleID.AriaAllowedAttr, "/a[@id='Head_639FA12F26E70352535ED937F171C296']");
      Assert.That (violations, Is.Empty);
    }

    private WxePageObject Start ()
    {
      return Start ("BocTreeView");
    }
  }
}