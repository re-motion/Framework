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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocEnumValueControlObjectTest : IntegrationTest
  {
    [Test]
    [RemotionTestCaseSource (typeof (DisabledTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (ReadOnlyTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (LabelTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (ValidationErrorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    public void GenericTests_DropDownList (GenericSelectorTestAction<BocEnumValueSelector, BocEnumValueControlObject> testAction)
    {
      testAction (Helper, e => e.EnumValues(), "dropDownList");
    }

    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (DomainPropertyControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (DisplayNameControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    public void TestControlSelectors_DropDownList (GenericSelectorTestAction<BocEnumValueSelector, BocEnumValueControlObject> testAction)
    {
      testAction (Helper, e => e.EnumValues(), "dropDownList");
    }

    [Test]
    [RemotionTestCaseSource (typeof (DisabledTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (ReadOnlyTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (LabelTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (ValidationErrorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    public void GenericTests_ListBox (GenericSelectorTestAction<BocEnumValueSelector, BocEnumValueControlObject> testAction)
    {
      testAction (Helper, e => e.EnumValues(), "listBox");
    }

    [Test]
    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (DomainPropertyControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (DisplayNameControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    public void TestControlSelectors_ListBox (GenericSelectorTestAction<BocEnumValueSelector, BocEnumValueControlObject> testAction)
    {
      testAction (Helper, e => e.EnumValues(), "listBox");
    }

    [Test]
    [RemotionTestCaseSource (typeof (DisabledTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (ReadOnlyTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (LabelTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (ValidationErrorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    public void GenericTests_RadioButtonList (GenericSelectorTestAction<BocEnumValueSelector, BocEnumValueControlObject> testAction)
    {
      testAction (Helper, e => e.EnumValues(), "radioButtonList");
    }

    [Test]
    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (DomainPropertyControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    [RemotionTestCaseSource (typeof (DisplayNameControlSelectorTestCaseFactory<BocEnumValueSelector, BocEnumValueControlObject>))]
    public void TestControlSelectors_RadioButtonList (GenericSelectorTestAction<BocEnumValueSelector, BocEnumValueControlObject> testAction)
    {
      testAction (Helper, e => e.EnumValues(), "radioButtonList");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var dropDownControl = home.EnumValues().GetByLocalID ("DataEditControl_MarriageStatusField_DropDownListDisabled");
      Assert.That (dropDownControl.IsDisabled(), Is.True);
      Assert.That (
          () => dropDownControl.SelectOption().WithDisplayText ("Married"),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("SelectOption.WithDisplayText").Message));
      Assert.That (
          () => dropDownControl.SelectOption().WithIndex (1),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("SelectOption.WithIndex").Message));
      Assert.That (
          () => dropDownControl.SelectOption().WithItemID ("Married"),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("SelectOption.WithItemID").Message));
      Assert.That (
          () => dropDownControl.SelectOption ("Married"),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("SelectOption(itemID)").Message));

      var listBoxControl = home.EnumValues().GetByLocalID ("DataEditControl_MarriageStatusField_ListBoxDisabled");
      Assert.That (listBoxControl.IsDisabled(), Is.True);
      Assert.That (
          () => listBoxControl.SelectOption().WithDisplayText ("Married"),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("SelectOption.WithDisplayText").Message));
      Assert.That (
          () => listBoxControl.SelectOption().WithIndex (1),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("SelectOption.WithIndex").Message));
      Assert.That (
          () => listBoxControl.SelectOption().WithItemID ("Married"),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("SelectOption.WithItemID").Message));
      Assert.That (
          () => listBoxControl.SelectOption ("Married"),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("SelectOption(itemID)").Message));

      var radioButton = home.EnumValues().GetByLocalID ("DataEditControl_MarriageStatusField_RadioButtonListDisabled");
      Assert.That (radioButton.IsDisabled(), Is.True);
      Assert.That (
          () => radioButton.SelectOption().WithDisplayText ("Married"),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("SelectOption.WithDisplayText").Message));
      Assert.That (
          () => radioButton.SelectOption().WithIndex (1),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("SelectOption.WithIndex").Message));
      Assert.That (
          () => radioButton.SelectOption().WithItemID ("Married"),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("SelectOption.WithItemID").Message));
      Assert.That (
          () => radioButton.SelectOption ("Married"),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("SelectOption(itemID)").Message));
    }

    [Test]
    public void TestGetSelectedOption ()
    {
      var home = Start();

      var dropDownListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_DropDownListNormal");
      AssertSelectedOption (dropDownListBocEnumValue, "Married", -1, "Married");

      dropDownListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_DropDownListReadOnly");
      AssertSelectedOption (dropDownListBocEnumValue, "Married", -1, "Married");

      dropDownListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_DropDownListReadOnlyWithoutSelectedValue");
      AssertSelectedOption (dropDownListBocEnumValue, "==null==", -1, "");

      dropDownListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_DropDownListDisabled");
      AssertSelectedOption (dropDownListBocEnumValue, "Married", -1, "Married");

      dropDownListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_DropDownListNoAutoPostBack");
      AssertSelectedOption (dropDownListBocEnumValue, "Married", -1, "Married");

      var listBoxBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_ListBoxNormal");
      AssertSelectedOption (listBoxBocEnumValue, "Married", -1, "Married");

      listBoxBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_ListBoxReadOnly");
      AssertSelectedOption (listBoxBocEnumValue, "Married", -1, "Married");

      listBoxBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_ListBoxReadOnlyWithoutSelectedValue");
      AssertSelectedOption (listBoxBocEnumValue, "==null==", -1, "");

      listBoxBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_ListBoxDisabled");
      AssertSelectedOption (listBoxBocEnumValue, "Married", -1, "Married");

      listBoxBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_ListBoxNoAutoPostBack");
      AssertSelectedOption (listBoxBocEnumValue, "Married", -1, "Married");

      var radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListNormal");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListReadOnly");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListReadOnlyWithoutSelectedValue");
      AssertSelectedOption (radioButtonListBocEnumValue, "==null==", -1, "");

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListDisabled");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListNoAutoPostBack");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListMultiColumn");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListFlow");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListOrderedList");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListUnorderedList");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListLabelLeft");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListRequiredWithoutSelectedValue");
      AssertSelectedOption (radioButtonListBocEnumValue, "==null==", -1, "");

      radioButtonListBocEnumValue =
          home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListWithoutSelectedValueAndWithoutVisibleNullValue");

      AssertSelectedOption (radioButtonListBocEnumValue, "==null==", -1, "");
    }

    private static void AssertSelectedOption (
        BocEnumValueControlObject radioButtonListBocEnumValue,
        string expectedItemID,
        int expectedIndex,
        string expectedText)
    {
      var optionDefinition = radioButtonListBocEnumValue.GetSelectedOption();

      Assert.That (optionDefinition.ItemID, Is.EqualTo (expectedItemID));
      Assert.That (optionDefinition.Index, Is.EqualTo (expectedIndex));
      Assert.That (optionDefinition.Text, Is.EqualTo (expectedText));
      Assert.That (optionDefinition.IsSelected, Is.True);
    }

    [Test]
    public void TestGetOptionDefinitions ()
    {
      var home = Start();

      var dropDownListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_DropDownListNormal");
      AssertOptions (dropDownListBocEnumValue);

      dropDownListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_DropDownListReadOnly");
      Assert.That (
          () => dropDownListBocEnumValue.GetOptionDefinitions(),
          Throws.InvalidOperationException.With.Message.EqualTo ("Cannot obtain option definitions on read-only control."));

      var listBoxBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_ListBoxNormal");
      AssertOptions (listBoxBocEnumValue);

      listBoxBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_ListBoxReadOnly");
      Assert.That (
          () => listBoxBocEnumValue.GetOptionDefinitions(),
          Throws.InvalidOperationException.With.Message.EqualTo ("Cannot obtain option definitions on read-only control."));

      var radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListNormal");
      AssertOptions (radioButtonListBocEnumValue);

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListReadOnly");
      Assert.That (
          () => radioButtonListBocEnumValue.GetOptionDefinitions(),
          Throws.InvalidOperationException.With.Message.EqualTo ("Cannot obtain option definitions on read-only control."));
    }

    private static void AssertOptions (BocEnumValueControlObject dropDownListBocEnumValue)
    {
      var options = dropDownListBocEnumValue.GetOptionDefinitions();
      Assert.That (options.Count, Is.EqualTo (4));

      Assert.That (options[0].ItemID, Is.EqualTo ("==null=="));
      Assert.That (options[0].Index, Is.EqualTo (1));
      Assert.That (options[0].Text, Is.EqualTo ("Is_So_Undefined"));
      Assert.That (options[0].IsSelected, Is.False);

      Assert.That (options[3].ItemID, Is.EqualTo ("Divorced"));
      Assert.That (options[3].Index, Is.EqualTo (4));
      Assert.That (options[3].Text, Is.EqualTo ("Divorced"));
      Assert.That (options[3].IsSelected, Is.False);

      Assert.That (options[1].IsSelected, Is.True);
    }

    [Test]
    public void TestHasNullOptionDefinition ()
    {
      var home = Start();

      var dropDownListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_DropDownListNormal");
      Assert.That (dropDownListBocEnumValue.HasNullOptionDefinition(), Is.True);

      dropDownListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_DropDownListReadOnly");
      Assert.That (
          () => dropDownListBocEnumValue.HasNullOptionDefinition(),
          Throws.InvalidOperationException.With.Message.EqualTo ("A read-only control cannot contain a null option definition."));

      dropDownListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_DropDownListReadOnlyWithoutSelectedValue");
      Assert.That (
          () => dropDownListBocEnumValue.HasNullOptionDefinition(),
          Throws.InvalidOperationException.With.Message.EqualTo ("A read-only control cannot contain a null option definition."));

      dropDownListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_DropDownListDisabled");
      Assert.That (dropDownListBocEnumValue.HasNullOptionDefinition(), Is.False);

      dropDownListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_DropDownListNoAutoPostBack");
      Assert.That (dropDownListBocEnumValue.HasNullOptionDefinition(), Is.False);

      var listBoxBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_ListBoxNormal");
      Assert.That (listBoxBocEnumValue.HasNullOptionDefinition(), Is.True);

      listBoxBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_ListBoxReadOnly");
      Assert.That (
          () => listBoxBocEnumValue.HasNullOptionDefinition(),
          Throws.InvalidOperationException.With.Message.EqualTo ("A read-only control cannot contain a null option definition."));

      listBoxBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_ListBoxReadOnlyWithoutSelectedValue");
      Assert.That (
          () => listBoxBocEnumValue.HasNullOptionDefinition(),
          Throws.InvalidOperationException.With.Message.EqualTo ("A read-only control cannot contain a null option definition."));

      listBoxBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_ListBoxDisabled");
      AssertSelectedOption (listBoxBocEnumValue, "Married", -1, "Married");

      listBoxBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_ListBoxNoAutoPostBack");
      Assert.That (listBoxBocEnumValue.HasNullOptionDefinition(), Is.False);

      var radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListNormal");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.True);

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListReadOnly");
      Assert.That (
          () => radioButtonListBocEnumValue.HasNullOptionDefinition(),
          Throws.InvalidOperationException.With.Message.EqualTo ("A read-only control cannot contain a null option definition."));

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListReadOnlyWithoutSelectedValue");
      Assert.That (
          () => radioButtonListBocEnumValue.HasNullOptionDefinition(),
          Throws.InvalidOperationException.With.Message.EqualTo ("A read-only control cannot contain a null option definition."));

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListDisabled");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.False);

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListNoAutoPostBack");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.False);

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListMultiColumn");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.True);

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListFlow");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.True);

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListOrderedList");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.True);

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListUnorderedList");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.True);

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListLabelLeft");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.True);

      radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListRequiredWithoutSelectedValue");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.False);

      radioButtonListBocEnumValue =
          home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListWithoutSelectedValueAndWithoutVisibleNullValue");

      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.False);
    }

    [Test]
    public void TestSelectOption ()
    {
      var home = Start();

      const string single = "Single";
      const string divorced = "Divorced";

      var dropDownListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_DropDownListNormal");

      dropDownListBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNormalCurrentValueLabel").Text, Is.EqualTo (single));

      dropDownListBocEnumValue.SelectOption().WithIndex (1);
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNormalCurrentValueLabel").Text, Is.Empty);

      dropDownListBocEnumValue.SelectOption().WithDisplayText (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNormalCurrentValueLabel").Text, Is.EqualTo (divorced));

      dropDownListBocEnumValue.SelectOption().WithDisplayText ("Is_So_Undefined");
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNormalCurrentValueLabel").Text, Is.Empty);

      var listBoxBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_ListBoxNormal");

      listBoxBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNormalCurrentValueLabel").Text, Is.EqualTo (single));

      listBoxBocEnumValue.SelectOption().WithIndex (1);
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNormalCurrentValueLabel").Text, Is.Empty);

      listBoxBocEnumValue.SelectOption().WithDisplayText (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNormalCurrentValueLabel").Text, Is.EqualTo (divorced));

      listBoxBocEnumValue.SelectOption().WithDisplayText ("Is_So_Undefined");
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNormalCurrentValueLabel").Text, Is.Empty);

      var radioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListNormal");

      radioButtonListBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNormalCurrentValueLabel").Text, Is.EqualTo (single));

      radioButtonListBocEnumValue.SelectOption().WithIndex (1);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNormalCurrentValueLabel").Text, Is.Empty);

      radioButtonListBocEnumValue.SelectOption().WithDisplayText (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNormalCurrentValueLabel").Text, Is.EqualTo (divorced));

      radioButtonListBocEnumValue.SelectOption().WithDisplayText ("Is_So_Undefined");
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNormalCurrentValueLabel").Text, Is.Empty);

      var multiColumnradioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListMultiColumn");

      multiColumnradioButtonListBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListMultiColumnCurrentValueLabel").Text, Is.EqualTo (single));

      multiColumnradioButtonListBocEnumValue.SelectOption().WithIndex (1);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListMultiColumnCurrentValueLabel").Text, Is.Empty);

      multiColumnradioButtonListBocEnumValue.SelectOption().WithDisplayText (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListMultiColumnCurrentValueLabel").Text, Is.EqualTo (divorced));

      multiColumnradioButtonListBocEnumValue.SelectOption().WithDisplayText ("Is_So_Undefined");
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListMultiColumnCurrentValueLabel").Text, Is.Empty);

      var flowRadioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListFlow");

      flowRadioButtonListBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListFlowCurrentValueLabel").Text, Is.EqualTo (single));

      flowRadioButtonListBocEnumValue.SelectOption().WithIndex (1);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListFlowCurrentValueLabel").Text, Is.Empty);

      flowRadioButtonListBocEnumValue.SelectOption().WithDisplayText (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListFlowCurrentValueLabel").Text, Is.EqualTo (divorced));

      flowRadioButtonListBocEnumValue.SelectOption().WithDisplayText ("Is_So_Undefined");
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListFlowCurrentValueLabel").Text, Is.Empty);

      var orderedListRadioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListOrderedList");

      orderedListRadioButtonListBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListOrderedListCurrentValueLabel").Text, Is.EqualTo (single));

      orderedListRadioButtonListBocEnumValue.SelectOption().WithIndex (1);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListOrderedListCurrentValueLabel").Text, Is.Empty);

      orderedListRadioButtonListBocEnumValue.SelectOption().WithDisplayText (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListOrderedListCurrentValueLabel").Text, Is.EqualTo (divorced));

      orderedListRadioButtonListBocEnumValue.SelectOption().WithDisplayText ("Is_So_Undefined");
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListOrderedListCurrentValueLabel").Text, Is.Empty);

      var labelLeftRadioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListLabelLeft");

      labelLeftRadioButtonListBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListLabelLeftCurrentValueLabel").Text, Is.EqualTo (single));

      labelLeftRadioButtonListBocEnumValue.SelectOption().WithIndex (1);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListLabelLeftCurrentValueLabel").Text, Is.Empty);

      labelLeftRadioButtonListBocEnumValue.SelectOption().WithDisplayText (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListLabelLeftCurrentValueLabel").Text, Is.EqualTo (divorced));

      labelLeftRadioButtonListBocEnumValue.SelectOption().WithDisplayText ("Is_So_Undefined");
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListLabelLeftCurrentValueLabel").Text, Is.Empty);
    }

    // Due to a workaround for a Marionette bug (RM-7279), asserting the completion detection after selecting the already selected value does not work.
    [Test]
    public void TestSelectOptionPostBack ()
    {
      var home = Start();

      const string married = "Married";
      const string single = "Single";
      const string divorced = "Divorced";

      var normalDropDownListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_DropDownListNormal");
      var normalDropDownListBocEnumValueCompletionDetection = new CompletionDetectionStrategyTestHelper (normalDropDownListBocEnumValue);
      var noAutoPostBackDropDownListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_DropDownListNoAutoPostBack");
      var noAutoPostBackDropDownListBocEnumValueCompletionDetection =
          new CompletionDetectionStrategyTestHelper (noAutoPostBackDropDownListBocEnumValue);
      var normalListBoxBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_ListBoxNormal");
      var normalListBoxBocEnumValueCompletionDetection = new CompletionDetectionStrategyTestHelper (normalListBoxBocEnumValue);
      var noAutoPostBackListBoxBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_ListBoxNoAutoPostBack");
      var noAutoPostBackListBoxBocEnumValueCompletionDetection = new CompletionDetectionStrategyTestHelper (noAutoPostBackListBoxBocEnumValue);
      var normalRadioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListNormal");
      var normalRadioButtonListBocEnumValueCompletionDetection = new CompletionDetectionStrategyTestHelper (normalRadioButtonListBocEnumValue);
      var noAutoPostBackRadioButtonListBocEnumValue = home.EnumValues().GetByLocalID ("MarriageStatusField_RadioButtonListNoAutoPostBack");
      var noAutoPostBackRadioButtonListBocEnumValueCompletionDetection =
          new CompletionDetectionStrategyTestHelper (noAutoPostBackRadioButtonListBocEnumValue);

      normalDropDownListBocEnumValue.SelectOption ("==null==");
      Assert.That (normalDropDownListBocEnumValueCompletionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNormalCurrentValueLabel").Text, Is.Empty);

      normalDropDownListBocEnumValue.SelectOption (single);
      Assert.That (normalDropDownListBocEnumValueCompletionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNormalCurrentValueLabel").Text, Is.EqualTo (single));

      noAutoPostBackDropDownListBocEnumValue.SelectOption (single); // no auto post back
      if (!Helper.BrowserConfiguration.IsFirefox())
        Assert.That (noAutoPostBackDropDownListBocEnumValueCompletionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (married));

      normalDropDownListBocEnumValue.SelectOption (single, Opt.ContinueImmediately()); // same value, does not trigger post back
      if (!Helper.BrowserConfiguration.IsFirefox())
        Assert.That (normalDropDownListBocEnumValueCompletionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (married));

      normalDropDownListBocEnumValue.SelectOption (divorced);
      Assert.That (normalDropDownListBocEnumValueCompletionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNormalCurrentValueLabel").Text, Is.EqualTo (divorced));
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (single));

      normalListBoxBocEnumValue.SelectOption ("==null==");
      Assert.That (normalListBoxBocEnumValueCompletionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNormalCurrentValueLabel").Text, Is.Empty);

      normalListBoxBocEnumValue.SelectOption (single);
      Assert.That (normalListBoxBocEnumValueCompletionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNormalCurrentValueLabel").Text, Is.EqualTo (single));

      noAutoPostBackListBoxBocEnumValue.SelectOption (single); // no auto post back
      Assert.That (noAutoPostBackListBoxBocEnumValueCompletionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (married));

      normalListBoxBocEnumValue.SelectOption (single, Opt.ContinueImmediately()); // same value, does not trigger post back
      if (!Helper.BrowserConfiguration.IsFirefox())
        Assert.That (normalListBoxBocEnumValueCompletionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (married));

      normalListBoxBocEnumValue.SelectOption (divorced);
      Assert.That (normalListBoxBocEnumValueCompletionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNormalCurrentValueLabel").Text, Is.EqualTo (divorced));
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (single));

      normalRadioButtonListBocEnumValue.SelectOption ("==null==");
      Assert.That (normalRadioButtonListBocEnumValueCompletionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNormalCurrentValueLabel").Text, Is.Empty);

      normalRadioButtonListBocEnumValue.SelectOption (single);
      Assert.That (normalRadioButtonListBocEnumValueCompletionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNormalCurrentValueLabel").Text, Is.EqualTo (single));

      noAutoPostBackRadioButtonListBocEnumValue.SelectOption (single); // no auto post back
      Assert.That (noAutoPostBackRadioButtonListBocEnumValueCompletionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (married));

      normalRadioButtonListBocEnumValue.SelectOption (single, Opt.ContinueImmediately()); // same value, does not trigger post back
      if (!Helper.BrowserConfiguration.IsFirefox())
        Assert.That (normalRadioButtonListBocEnumValueCompletionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (married));

      normalRadioButtonListBocEnumValue.SelectOption (divorced);
      Assert.That (normalRadioButtonListBocEnumValueCompletionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNormalCurrentValueLabel").Text, Is.EqualTo (divorced));
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (single));
    }

    private WxePageObject Start ()
    {
      return Start ("BocEnumValue");
    }
  }
}