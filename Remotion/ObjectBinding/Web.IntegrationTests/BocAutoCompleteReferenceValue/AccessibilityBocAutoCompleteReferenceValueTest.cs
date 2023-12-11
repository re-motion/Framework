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
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.Accessibility;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.ObjectBinding.Web.IntegrationTests.BocAutoCompleteReferenceValue
{
  [TestFixture]
  public class AccessibilityBocAutoCompleteReferenceValueTest : IntegrationTest
  {
    [Test]
    public void Normal ()
    {
      var home = Start();
      var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocAutoComplete.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void Normal_WithValidationErrors ()
    {
      var home = Start();
      var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");
      bocAutoComplete.FillWith("InvalidInput");
      var validateButton = home.GetValidateButton();
      validateButton.Click();
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocAutoComplete.Analyze(analyzer);

      Assert.That(bocAutoComplete.GetValidationErrors(), Is.Not.Empty);
      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void ReadOnly ()
    {
      var home = Start();
      var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_ReadOnly");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocAutoComplete.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void Disabled ()
    {
      var home = Start();
      var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Disabled");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocAutoComplete.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void NoCommandNoMenu ()
    {
      var home = Start();
      var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_NoCommandNoMenu");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocAutoComplete.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void NoCommandNoMenu_ReadOnly ()
    {
      var home = Start();
      var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_NoCommandNoMenu_ReadOnly");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocAutoComplete.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void Required_ExpandedMenu ()
    {
      var home = Start();
      var dropDownButton = home.WebButtons().GetByID("body_DataEditControl_PartnerField_Normal_Required_DropDownButton");
      var analyzer = Helper.CreateAccessibilityAnalyzer();
      var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal_Required");
      bocAutoComplete.FillWith("");
      dropDownButton.Click();

      Assert.That(FluentScreenshotExtensions.ForControlObjectScreenshot(bocAutoComplete).GetSelectList().IsVisible(), Is.True);
      var result = bocAutoComplete.Analyze(analyzer);
      Assert.That(bocAutoComplete.ForControlObjectScreenshot().GetSelectList().IsVisible(), Is.True);

      // Violation ignored as the relevant code part conforms to ARIA 1.2 and passes on develop but here we check for an older ARIA standard
      var violations = result.Violations.IgnoreByRuleID(AccessibilityRuleID.AriaRequiredChildren);

      Assert.That(violations, Is.Empty);
    }

    private WxePageObject Start ()
    {
      return Start("BocAutoCompleteReferenceValue");
    }
  }
}
