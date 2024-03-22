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

namespace Remotion.ObjectBinding.Web.IntegrationTests.BocEnumValue
{
  [TestFixture]
  public class AccessibilityBocEnumValueTest : IntegrationTest
  {
    [Test]
    public void Normal ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_DropDownListNormal");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void DropDownListReadOnly ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_DropDownListReadOnly");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void DropDownListReadOnlyWithoutSelectedValue ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_DropDownListReadOnlyWithoutSelectedValue");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void DropDownListDisabled ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_DropDownListDisabled");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void ListBoxNormal ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_ListBoxNormal");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void ListBoxReadOnly ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_ListBoxReadOnly");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void ListBoxReadOnlyWithoutSelectedValue ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_ListBoxReadOnlyWithoutSelectedValue");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void ListBoxDisabled ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_ListBoxDisabled");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void RadioButtonListNormal ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_RadioButtonListNormal");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void RadioButtonListReadOnly ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_RadioButtonListReadOnly");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void RadioButtonListReadOnlyWithoutSelectedValue ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_RadioButtonListReadOnlyWithoutSelectedValue");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void RadioButtonListDisabled ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_RadioButtonListDisabled");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void RadioButtonListMultiColumn ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_RadioButtonListMultiColumn");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void RadioButtonListFlow ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_RadioButtonListFlow");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void RadioButtonListOrderedList ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_RadioButtonListOrderedList");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      // TODO RM-7331 remove ignore once issue is resolved
      var violations = result.Violations
          .IgnoreByRuleIDAndXPath(
              AccessibilityRuleID.ListItem,
              "/ol[@id='body_DataEditControl_MarriageStatusField_RadioButtonListOrderedList_Value']/li")
          .IgnoreByRuleIDAndXPath(
              AccessibilityRuleID.ListItem,
              "/ol[@id='body_DataEditControl_MarriageStatusField_RadioButtonListOrderedList_Value']/li[2]")
          .IgnoreByRuleIDAndXPath(
              AccessibilityRuleID.ListItem,
              "/ol[@id='body_DataEditControl_MarriageStatusField_RadioButtonListOrderedList_Value']/li[3]")
          .IgnoreByRuleIDAndXPath(
              AccessibilityRuleID.ListItem,
              "/ol[@id='body_DataEditControl_MarriageStatusField_RadioButtonListOrderedList_Value']/li[4]");
      Assert.That(violations, Is.Empty);
    }

    [Test]
    public void RadioButtonListUnorderedList ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_RadioButtonListUnorderedList");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      // TODO RM-7331 remove ignore once issue is resolved
      var violations = result.Violations
          .IgnoreByRuleIDAndXPath(
              AccessibilityRuleID.ListItem,
              "/ul[@id='body_DataEditControl_MarriageStatusField_RadioButtonListUnorderedList_Value']/li")
          .IgnoreByRuleIDAndXPath(
              AccessibilityRuleID.ListItem,
              "/ul[@id='body_DataEditControl_MarriageStatusField_RadioButtonListUnorderedList_Value']/li[2]")
          .IgnoreByRuleIDAndXPath(
              AccessibilityRuleID.ListItem,
              "/ul[@id='body_DataEditControl_MarriageStatusField_RadioButtonListUnorderedList_Value']/li[3]")
          .IgnoreByRuleIDAndXPath(
              AccessibilityRuleID.ListItem,
              "/ul[@id='body_DataEditControl_MarriageStatusField_RadioButtonListUnorderedList_Value']/li[4]");
      Assert.That(violations, Is.Empty);
    }

    [Test]
    public void RadioButtonListLabelLeft ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_RadioButtonListLabelLeft");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void RadioButtonListNoSelectedValueNoNullValue ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_RadioButtonListWithoutSelectedValueAndWithoutVisibleNullValue");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void RadioButtonListRequiredNoSelectedValue_WithValidationErrors ()
    {
      var home = Start();
      var bocEnumValue = home.EnumValues().GetByLocalID("MarriageStatusField_RadioButtonListRequiredWithoutSelectedValue");
      var validateButton = home.GetValidateButton();
      validateButton.Click();
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocEnumValue.Analyze(analyzer);

      Assert.That(bocEnumValue.GetValidationErrors(), Is.Not.Empty);
      Assert.That(result.Violations, Is.Empty);
    }

    private WxePageObject Start ()
    {
      return Start("BocEnumValue");
    }
  }
}
