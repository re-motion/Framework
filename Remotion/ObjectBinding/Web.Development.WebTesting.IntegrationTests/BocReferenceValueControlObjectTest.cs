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
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocReferenceValueControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource(typeof(DisabledTestCaseFactory<BocReferenceValueSelector, BocReferenceValueControlObject>))]
    [TestCaseSource(typeof(ReadOnlyTestCaseFactory<BocReferenceValueSelector, BocReferenceValueControlObject>))]
    [TestCaseSource(typeof(LabelTestCaseFactory<BocReferenceValueSelector, BocReferenceValueControlObject>))]
    [TestCaseSource(typeof(ValidationErrorTestCaseFactory<BocReferenceValueSelector, BocReferenceValueControlObject>))]
    public void GenericTests (GenericSelectorTestAction<BocReferenceValueSelector, BocReferenceValueControlObject> testAction)
    {
      testAction(Helper, e => e.ReferenceValues(), "referenceValue");
    }

    [Test]
    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<BocReferenceValueSelector, BocReferenceValueControlObject>))]
    [TestCaseSource(typeof(IndexControlSelectorTestCaseFactory<BocReferenceValueSelector, BocReferenceValueControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<BocReferenceValueSelector, BocReferenceValueControlObject>))]
    [TestCaseSource(typeof(FirstControlSelectorTestCaseFactory<BocReferenceValueSelector, BocReferenceValueControlObject>))]
    [TestCaseSource(typeof(SingleControlSelectorTestCaseFactory<BocReferenceValueSelector, BocReferenceValueControlObject>))]
    [TestCaseSource(typeof(DomainPropertyControlSelectorTestCaseFactory<BocReferenceValueSelector, BocReferenceValueControlObject>))]
    [TestCaseSource(typeof(DisplayNameControlSelectorTestCaseFactory<BocReferenceValueSelector, BocReferenceValueControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<BocReferenceValueSelector, BocReferenceValueControlObject> testAction)
    {
      testAction(Helper, e => e.ReferenceValues(), "referenceValue");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      const string baValue = "c8ace752-55f6-4074-8890-130276ea6cd1";

      var home = Start();

      var control = home.ReferenceValues().GetByLocalID("PartnerField_Disabled");

      Assert.That(control.IsDisabled(), Is.True);
      Assert.That(
          () => control.SelectOption().WithDisplayText("D, A"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SelectOption.WithDisplayText").Message));
      Assert.That(
          () => control.SelectOption().WithIndex(1),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SelectOption.WithIndex").Message));
      Assert.That(
          () => control.SelectOption().WithItemID(baValue),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SelectOption.WithItemID").Message));
      Assert.That(
          () => control.SelectOption(baValue),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SelectOption(itemID)").Message));
    }

    [Test]
    public void TestIsReadOnly_SetMethodsThrow ()
    {
      const string baValue = "c8ace752-55f6-4074-8890-130276ea6cd1";

      var home = Start();

      var control = home.ReferenceValues().GetByLocalID("PartnerField_ReadOnly");

      Assert.That(control.IsReadOnly(), Is.True);
      Assert.That(
          () => control.GetOptionDefinitions(),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
      Assert.That(
          () => control.SelectOption().WithDisplayText("D, A"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
      Assert.That(
          () => control.SelectOption().WithIndex(1),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
      Assert.That(
          () => control.SelectOption().WithItemID(baValue),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
      Assert.That(
          () => control.SelectOption(baValue),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
    }

    [Test]
    public void TestGetSelectedOption_ValueSelected ()
    {
      var home = Start();

      const string daValue = "00000000-0000-0000-0000-000000000009";

      var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Normal");
      AssertSelectedOption(bocReferenceValue, daValue, -1, "D, A");
    }

    [Test]
    public void TestGetSelectedOption_NullSelected ()
    {
      var home = Start();

      var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_WithoutSelectedValue");
      AssertSelectedOption(bocReferenceValue, "==null==", -1, "");
    }

    [Test]
    public void TestDropDownItemDisabledWithRequiredSelection ()
    {
      var home = Start();

      var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Normal");
      var dropDownMenu = bocReferenceValue.GetDropDownMenu();

      dropDownMenu.Open();
      var menuItem = GetDropDownMenuItem3();
      Assert.That(menuItem.IsDisabled, Is.False);

      bocReferenceValue.SelectOption("==null==");
      menuItem = GetDropDownMenuItem3();
      Assert.That(menuItem.IsDisabled, Is.True);

      bocReferenceValue.SelectOption().WithDisplayText("D, A");
      menuItem = GetDropDownMenuItem3();
      Assert.That(menuItem.IsDisabled, Is.False);

      ItemDefinition GetDropDownMenuItem3 () => dropDownMenu.GetItemDefinitions().Single(x => x.ItemID == "OptCmd3");
    }

    [Test]
    public void TestGetSelectedOption_IsReadOnlyAndHasValue ()
    {
      var home = Start();

      const string daValue = "00000000-0000-0000-0000-000000000009";

      var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_ReadOnly");
      AssertSelectedOption(bocReferenceValue, daValue, -1, "D, A");
    }

    [Test]
    public void TestGetSelectedOption_IsReadOnlyAndNullSelected ()
    {
      var home = Start();

      var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_ReadOnlyWithoutSelectedValue");
      AssertSelectedOption(bocReferenceValue, "==null==", -1, "");
    }

    [Test]
    public void TestGetSelectedOption_IsReadOnlyAndValueIs ()
    {
      var home = Start();

      const string daValue = "00000000-0000-0000-0000-000000000009";

      var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Normal");
      AssertSelectedOption(bocReferenceValue, daValue, -1, "D, A");
    }

    private void AssertSelectedOption (
        BocReferenceValueControlObject bocReferenceValue,
        string expectedItemID,
        int expectedIndex,
        string expectedText)
    {
      var optionDefinition = bocReferenceValue.GetSelectedOption();

      Assert.That(optionDefinition.ItemID, Is.EqualTo(expectedItemID));
      Assert.That(optionDefinition.Index, Is.EqualTo(expectedIndex));
      Assert.That(optionDefinition.Text, Is.EqualTo(expectedText));
      Assert.That(optionDefinition.IsSelected, Is.True);
    }

    [Test]
    public void TestGetOptionDefinitions ()
    {
      var home = Start();

      const string fValue = "ef898bb1-7095-46d2-872b-5f732a7c0036";

      var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Normal");

      var options = bocReferenceValue.GetOptionDefinitions();
      Assert.That(options.Count, Is.EqualTo(17));

      Assert.That(options[0].ItemID, Is.EqualTo("==null=="));
      Assert.That(options[0].Index, Is.EqualTo(1));
      Assert.That(options[0].Text, Is.Empty);
      Assert.That(options[0].IsSelected, Is.False);

      Assert.That(options[2].Text, Is.EqualTo("D, A"));
      Assert.That(options[2].IsSelected, Is.True);

      Assert.That(options[16].ItemID, Is.EqualTo(fValue));
      Assert.That(options[16].Index, Is.EqualTo(17));

      Assert.That(options[16].Text, Is.EqualTo("F,"));
      Assert.That(options[16].IsSelected, Is.False);
    }

    [Test]
    public void TestHasNullOptionDefinition_IsReadOnlyAndHasNullValue_ReturnsTrue ()
    {
      var home = Start();

      var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_ReadOnlyWithoutSelectedValue");
      Assert.That(bocReferenceValue.HasNullOptionDefinition(), Is.True);

    }

    [Test]
    public void TestHasNullOptionDefinition_IsReadOnly_ReturnsFalse ()
    {
      var home = Start();

      var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_ReadOnly");
      Assert.That(bocReferenceValue.HasNullOptionDefinition(), Is.False);
    }

    [Test]
    public void TestHasNullOptionDefinition_HasNullValue_ReturnsTrue ()
    {
      var home = Start();

      var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Normal");
      Assert.That(bocReferenceValue.HasNullOptionDefinition(), Is.True);

    }

    [Test]
    public void TestHasNullOptionDefinition_WithoutNullValueInList_ReturnsFalse ()
    {
      var home = Start();

      var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Required");
      Assert.That(bocReferenceValue.HasNullOptionDefinition(), Is.False);

    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Normal");
      Assert.That(bocReferenceValue.GetText(), Is.EqualTo("D, A"));

      bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_ReadOnly");
      Assert.That(bocReferenceValue.GetText(), Is.EqualTo("D, A"));

      bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_ReadOnlyWithoutSelectedValue");
      Assert.That(bocReferenceValue.GetText(), Is.EqualTo(""));

      bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Disabled");
      Assert.That(bocReferenceValue.GetText(), Is.EqualTo("D, A"));

      bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_NoAutoPostBack");
      Assert.That(bocReferenceValue.GetText(), Is.EqualTo("D, A"));

      bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_NoCommandNoMenu");
      Assert.That(bocReferenceValue.GetText(), Is.EqualTo("D, A"));

      bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_WithoutSelectedValue");
      Assert.That(bocReferenceValue.GetText(), Is.EqualTo(""));
    }

    [Test]
    public void TestSelectOption ()
    {
      var home = Start();

      const string baValue = "c8ace752-55f6-4074-8890-130276ea6cd1";
      const string daValue = "00000000-0000-0000-0000-000000000009";

      {
        var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocReferenceValue);
        bocReferenceValue.SelectOption(baValue);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindIdEndingWith("BOUINormalLabel").Text, Is.EqualTo(baValue));
      }

      {
        var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocReferenceValue);
        bocReferenceValue.SelectOption().WithIndex(1);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindIdEndingWith("BOUINormalLabel").Text, Is.Empty);
      }

      {
        var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocReferenceValue);
        bocReferenceValue.SelectOption().WithDisplayText("D, A");
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindIdEndingWith("BOUINormalLabel").Text, Is.EqualTo(daValue));
      }
    }

    [Test]
    public void TestSelectOptionPostBack ()
    {
      var home = Start();

      const string baValue = "c8ace752-55f6-4074-8890-130276ea6cd1";
      const string daValue = "00000000-0000-0000-0000-000000000009";

      var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Normal");
      bocReferenceValue.SelectOption("==null==");
      Assert.That(home.Scope.FindIdEndingWith("BOUINormalLabel").Text, Is.Empty);

      bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Normal");
      bocReferenceValue.SelectOption(baValue);
      Assert.That(home.Scope.FindIdEndingWith("BOUINormalLabel").Text, Is.EqualTo(baValue));

      bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_NoAutoPostBack");
      var completionDetection = new CompletionDetectionStrategyTestHelper(bocReferenceValue);
      bocReferenceValue.SelectOption(baValue); // no auto post back
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindIdEndingWith("BOUINoAutoPostBackLabel").Text, Is.EqualTo(daValue));

      bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Normal");
      bocReferenceValue.SelectOption(baValue, Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That(home.Scope.FindIdEndingWith("BOUINoAutoPostBackLabel").Text, Is.EqualTo(daValue));

      bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Normal");
      bocReferenceValue.SelectOption(daValue);
      Assert.That(home.Scope.FindIdEndingWith("BOUINormalLabel").Text, Is.EqualTo(daValue));
      Assert.That(home.Scope.FindIdEndingWith("BOUINoAutoPostBackLabel").Text, Is.EqualTo(baValue));
    }

    [Test]
    public void TestGetDropDownMenu ()
    {
      var home = Start();

      var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Normal");
      var dropDownMenu = bocReferenceValue.GetDropDownMenu();
      dropDownMenu.SelectItem("OptCmd2");
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderLabel").Text, Is.EqualTo("PartnerField_Normal"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedLabel").Text, Is.EqualTo("MenuItemClick"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedParameterLabel").Text, Is.EqualTo("OptCmd2|My menu command 2"));

      bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_ReadOnly");
      dropDownMenu = bocReferenceValue.GetDropDownMenu();
      dropDownMenu.SelectItem("OptCmd2");
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderLabel").Text, Is.EqualTo("PartnerField_ReadOnly"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedLabel").Text, Is.EqualTo("MenuItemClick"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedParameterLabel").Text, Is.EqualTo("OptCmd2|My menu command 2"));
    }

    private WxePageObject Start ()
    {
      return Start("BocReferenceValue");
    }
  }
}
