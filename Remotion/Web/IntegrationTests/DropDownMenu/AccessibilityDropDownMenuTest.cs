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
using Remotion.Web.Development.WebTesting.Accessibility;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests;

namespace Remotion.Web.IntegrationTests.DropDownMenu
{
  [TestFixture]
  public class AccessibilityDropDownMenuTest : IntegrationTest
  {
    [Test]
    public void DropDownMenu ()
    {
      var home = Start();
      var dropDownMenu = home.DropDownMenus().GetByLocalID ("MyDropDownMenu");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = analyzer.Analyze (dropDownMenu);

      // TODO RM-7341 remove ignore once issue is resolved
      var violations = result.Violations.IgnoreByRuleIDAndXPath (AccessibilityRuleID.ButtonName, "/a[@id='body_MyDropDownMenu_DropDownMenuButton']");
      Assert.That (violations, Is.Empty);
    }

    [Test]
    public void DropDownMenu_Open ()
    {
      var home = Start();
      var dropDownMenu = home.DropDownMenus().GetByLocalID ("MyDropDownMenu");
      dropDownMenu.Open();
      Assert.That (dropDownMenu.IsOpen(), Is.True);
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = analyzer.Analyze (dropDownMenu);

      // TODO RM-7341 remove ignore once issue is resolved
      var violations = result.Violations.IgnoreByRuleIDAndXPath (AccessibilityRuleID.ButtonName, "/a[@id='body_MyDropDownMenu_DropDownMenuButton']");
      Assert.That (violations, Is.Empty);
    }

    [Test]
    public void DropDownMenu2 ()
    {
      var home = Start();
      var dropDownMenu = home.DropDownMenus().GetByLocalID ("MyDropDownMenu2");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = analyzer.Analyze (dropDownMenu);

      Assert.That (result.Violations, Is.Empty);
    }

    [Test]
    public void DropDownMenu_Disabled ()
    {
      var home = Start();
      var dropDownMenu = home.DropDownMenus().GetByLocalID ("MyDropDownMenu_Disabled");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = analyzer.Analyze (dropDownMenu);

      Assert.That (result.Violations, Is.Empty);
    }

    [Test]
    public void DropDownMenu_Error ()
    {
      var home = Start();
      var dropDownMenu = home.DropDownMenus().GetByLocalID ("MyDropDownMenu_Error");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = analyzer.Analyze (dropDownMenu);

      Assert.That (result.Violations, Is.Empty);
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("DropDownMenuTest.wxe");
    }
  }
}