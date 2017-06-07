﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocEnumValueControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var dropDownListBocEnumValue = home.GetEnumValue().ByID ("body_DataEditControl_MarriageStatusField_DropDownListNormal");
      Assert.That (dropDownListBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_DropDownListNormal"));

      var listBoxBocEnumValue = home.GetEnumValue().ByID ("body_DataEditControl_MarriageStatusField_ListBoxNormal");
      Assert.That (listBoxBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_ListBoxNormal"));

      var radioButtonListBocEnumValue = home.GetEnumValue().ByID ("body_DataEditControl_MarriageStatusField_RadioButtonListNormal");
      Assert.That (radioButtonListBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_RadioButtonListNormal"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var dropDownListBocEnumValue = home.GetEnumValue().ByIndex (2);
      Assert.That (dropDownListBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_DropDownListReadOnly"));

      var listBoxBocEnumValue = home.GetEnumValue().ByIndex (6);
      Assert.That (listBoxBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_ListBoxNormal"));

      var radioButtonListBocEnumValue = home.GetEnumValue().ByIndex (11);
      Assert.That (radioButtonListBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_RadioButtonListNormal"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListNormal");
      Assert.That (dropDownListBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_DropDownListNormal"));

      var listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxNormal");
      Assert.That (listBoxBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_ListBoxNormal"));

      var radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListNormal");
      Assert.That (radioButtonListBocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_RadioButtonListNormal"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var bocEnumValue = home.GetEnumValue().First();
      Assert.That (bocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_DropDownListNormal"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();

      try
      {
        home.GetEnumValue().Single();
        Assert.Fail ("Should not be able to unambiguously find a BOC enum value.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSelection_DisplayName ()
    {
      var home = Start();

      var bocEnumValue = home.GetEnumValue().ByDisplayName ("MarriageStatus");
      Assert.That (bocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_DropDownListNormal"));
    }

    [Test]
    public void TestSelection_DomainProperty ()
    {
      var home = Start();

      var bocEnumValue = home.GetEnumValue().ByDomainProperty ("MarriageStatus");
      Assert.That (bocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_DropDownListNormal"));
    }

    [Test]
    public void TestSelection_DomainPropertyAndClass ()
    {
      var home = Start();

      var bocEnumValue = home.GetEnumValue()
          .ByDomainProperty ("MarriageStatus", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocEnumValue.Scope.Id, Is.EqualTo ("body_DataEditControl_MarriageStatusField_DropDownListNormal"));
    }

    [Test]
    public void TestIsReadOnly ()
    {
      var home = Start();

      var dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListNormal");
      Assert.That (dropDownListBocEnumValue.IsReadOnly(), Is.False);

      dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListReadOnly");
      Assert.That (dropDownListBocEnumValue.IsReadOnly(), Is.True);

      var listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxNormal");
      Assert.That (listBoxBocEnumValue.IsReadOnly(), Is.False);

      listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxReadOnly");
      Assert.That (listBoxBocEnumValue.IsReadOnly(), Is.True);

      var radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListNormal");
      Assert.That (radioButtonListBocEnumValue.IsReadOnly(), Is.False);

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListReadOnly");
      Assert.That (radioButtonListBocEnumValue.IsReadOnly(), Is.True);
    }

    [Test]
    public void TestGetSelectedOption ()
    {
      var home = Start();

      var dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListNormal");
      AssertSelectedOption (dropDownListBocEnumValue, "Married", -1, "Married");

      dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListReadOnly");
      AssertSelectedOption (dropDownListBocEnumValue, "Married", -1, "Married");

      dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListReadOnlyWithoutSelectedValue");
      AssertSelectedOption (dropDownListBocEnumValue, "==null==", -1, "");

      dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListDisabled");
      AssertSelectedOption (dropDownListBocEnumValue, "Married", -1, "Married");

      dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListNoAutoPostBack");
      AssertSelectedOption (dropDownListBocEnumValue, "Married", -1, "Married");

      var listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxNormal");
      AssertSelectedOption (listBoxBocEnumValue, "Married", -1, "Married");

      listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxReadOnly");
      AssertSelectedOption (listBoxBocEnumValue, "Married", -1, "Married");

      listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxReadOnlyWithoutSelectedValue");
      AssertSelectedOption (listBoxBocEnumValue, "==null==", -1, "");

      listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxDisabled");
      AssertSelectedOption (listBoxBocEnumValue, "Married", -1, "Married");

      listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxNoAutoPostBack");
      AssertSelectedOption (listBoxBocEnumValue, "Married", -1, "Married");

      var radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListNormal");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListReadOnly");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListReadOnlyWithoutSelectedValue");
      AssertSelectedOption (radioButtonListBocEnumValue, "==null==", -1, "");

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListDisabled");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListNoAutoPostBack");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListMultiColumn");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListFlow");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListOrderedList");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListUnorderedList");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListLabelLeft");
      AssertSelectedOption (radioButtonListBocEnumValue, "Married", -1, "Married");

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListRequiredWithoutSelectedValue");
      AssertSelectedOption (radioButtonListBocEnumValue, "==null==", -1, "");

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListWithoutSelectedValueAndWithoutVisibleNullValue");
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

      var dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListNormal");
      AssertOptions(dropDownListBocEnumValue);

      dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListReadOnly");
      Assert.That (
          () => dropDownListBocEnumValue.GetOptionDefinitions(),
          Throws.InvalidOperationException.With.Message.EqualTo ("Cannot obtain option definitions on read-only control."));

      var listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxNormal");
      AssertOptions(listBoxBocEnumValue);

      listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxReadOnly");
      Assert.That (
          () => listBoxBocEnumValue.GetOptionDefinitions(),
          Throws.InvalidOperationException.With.Message.EqualTo ("Cannot obtain option definitions on read-only control."));

      var radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListNormal");
      AssertOptions(radioButtonListBocEnumValue);

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListReadOnly");
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

      var dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListNormal");
      Assert.That (dropDownListBocEnumValue.HasNullOptionDefinition(), Is.True);

      dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListReadOnly");
      Assert.That (
          () => dropDownListBocEnumValue.HasNullOptionDefinition(),
          Throws.InvalidOperationException.With.Message.EqualTo ("A read-only control cannot contain a null option definition."));

      dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListReadOnlyWithoutSelectedValue");
      Assert.That (
          () => dropDownListBocEnumValue.HasNullOptionDefinition(),
          Throws.InvalidOperationException.With.Message.EqualTo ("A read-only control cannot contain a null option definition."));

      dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListDisabled");
      Assert.That (dropDownListBocEnumValue.HasNullOptionDefinition(), Is.False);

      dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListNoAutoPostBack");
      Assert.That (dropDownListBocEnumValue.HasNullOptionDefinition(), Is.False);

      var listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxNormal");
      Assert.That (listBoxBocEnumValue.HasNullOptionDefinition(), Is.True);

      listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxReadOnly");
      Assert.That (
          () => listBoxBocEnumValue.HasNullOptionDefinition(),
          Throws.InvalidOperationException.With.Message.EqualTo ("A read-only control cannot contain a null option definition."));

      listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxReadOnlyWithoutSelectedValue");
      Assert.That (
          () => listBoxBocEnumValue.HasNullOptionDefinition(),
          Throws.InvalidOperationException.With.Message.EqualTo ("A read-only control cannot contain a null option definition."));

      listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxDisabled");
      AssertSelectedOption (listBoxBocEnumValue, "Married", -1, "Married");

      listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxNoAutoPostBack");
      Assert.That (listBoxBocEnumValue.HasNullOptionDefinition(), Is.False);

      var radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListNormal");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.True);

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListReadOnly");
      Assert.That (
          () => radioButtonListBocEnumValue.HasNullOptionDefinition(),
          Throws.InvalidOperationException.With.Message.EqualTo ("A read-only control cannot contain a null option definition."));

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListReadOnlyWithoutSelectedValue");
      Assert.That (
          () => radioButtonListBocEnumValue.HasNullOptionDefinition(),
          Throws.InvalidOperationException.With.Message.EqualTo ("A read-only control cannot contain a null option definition."));

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListDisabled");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.False);

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListNoAutoPostBack");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.False);

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListMultiColumn");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.True);

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListFlow");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.True);

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListOrderedList");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.True);

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListUnorderedList");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.True);

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListLabelLeft");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.True);

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListRequiredWithoutSelectedValue");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.False);

      radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListWithoutSelectedValueAndWithoutVisibleNullValue");
      Assert.That (radioButtonListBocEnumValue.HasNullOptionDefinition(), Is.False);
    }

    [Test]
    public void TestSelectOption ()
    {
      var home = Start();

      const string single = "Single";
      const string divorced = "Divorced";

      var dropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListNormal");

      dropDownListBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNormalCurrentValueLabel").Text, Is.EqualTo (single));

      dropDownListBocEnumValue.SelectOption().WithIndex (1);
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNormalCurrentValueLabel").Text, Is.Empty);

      dropDownListBocEnumValue.SelectOption().WithDisplayText (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNormalCurrentValueLabel").Text, Is.EqualTo (divorced));

      dropDownListBocEnumValue.SelectOption().WithDisplayText ("Is_So_Undefined");
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNormalCurrentValueLabel").Text, Is.Empty);

      var listBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxNormal");

      listBoxBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNormalCurrentValueLabel").Text, Is.EqualTo (single));

      listBoxBocEnumValue.SelectOption().WithIndex (1);
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNormalCurrentValueLabel").Text, Is.Empty);

      listBoxBocEnumValue.SelectOption().WithDisplayText (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNormalCurrentValueLabel").Text, Is.EqualTo (divorced));

      listBoxBocEnumValue.SelectOption().WithDisplayText ("Is_So_Undefined");
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNormalCurrentValueLabel").Text, Is.Empty);

      var radioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListNormal");

      radioButtonListBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNormalCurrentValueLabel").Text, Is.EqualTo (single));

      radioButtonListBocEnumValue.SelectOption().WithIndex (1);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNormalCurrentValueLabel").Text, Is.Empty);

      radioButtonListBocEnumValue.SelectOption().WithDisplayText (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNormalCurrentValueLabel").Text, Is.EqualTo (divorced));

      radioButtonListBocEnumValue.SelectOption().WithDisplayText ("Is_So_Undefined");
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNormalCurrentValueLabel").Text, Is.Empty);

      var multiColumnradioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListMultiColumn");

      multiColumnradioButtonListBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListMultiColumnCurrentValueLabel").Text, Is.EqualTo (single));

      multiColumnradioButtonListBocEnumValue.SelectOption().WithIndex (1);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListMultiColumnCurrentValueLabel").Text, Is.Empty);

      multiColumnradioButtonListBocEnumValue.SelectOption().WithDisplayText (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListMultiColumnCurrentValueLabel").Text, Is.EqualTo (divorced));

      multiColumnradioButtonListBocEnumValue.SelectOption().WithDisplayText ("Is_So_Undefined");
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListMultiColumnCurrentValueLabel").Text, Is.Empty);

      var flowRadioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListFlow");

      flowRadioButtonListBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListFlowCurrentValueLabel").Text, Is.EqualTo (single));

      flowRadioButtonListBocEnumValue.SelectOption().WithIndex (1);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListFlowCurrentValueLabel").Text, Is.Empty);

      flowRadioButtonListBocEnumValue.SelectOption().WithDisplayText (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListFlowCurrentValueLabel").Text, Is.EqualTo (divorced));

      flowRadioButtonListBocEnumValue.SelectOption().WithDisplayText ("Is_So_Undefined");
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListFlowCurrentValueLabel").Text, Is.Empty);

      var orderedListRadioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListOrderedList");

      orderedListRadioButtonListBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListOrderedListCurrentValueLabel").Text, Is.EqualTo (single));

      orderedListRadioButtonListBocEnumValue.SelectOption().WithIndex (1);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListOrderedListCurrentValueLabel").Text, Is.Empty);

      orderedListRadioButtonListBocEnumValue.SelectOption().WithDisplayText (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListOrderedListCurrentValueLabel").Text, Is.EqualTo (divorced));

      orderedListRadioButtonListBocEnumValue.SelectOption().WithDisplayText ("Is_So_Undefined");
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListOrderedListCurrentValueLabel").Text, Is.Empty);

      var labelLeftRadioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListLabelLeft");

      labelLeftRadioButtonListBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListLabelLeftCurrentValueLabel").Text, Is.EqualTo (single));

      labelLeftRadioButtonListBocEnumValue.SelectOption().WithIndex (1);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListLabelLeftCurrentValueLabel").Text, Is.Empty);

      labelLeftRadioButtonListBocEnumValue.SelectOption().WithDisplayText (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListLabelLeftCurrentValueLabel").Text, Is.EqualTo (divorced));

      labelLeftRadioButtonListBocEnumValue.SelectOption().WithDisplayText ("Is_So_Undefined");
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListLabelLeftCurrentValueLabel").Text, Is.Empty);
    }

    [Test]
    public void TestSelectOptionPostBack ()
    {
      var home = Start();

      const string married = "Married";
      const string single = "Single";
      const string divorced = "Divorced";

      var normalDropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListNormal");
      var noAutoPostBackDropDownListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_DropDownListNoAutoPostBack");

      normalDropDownListBocEnumValue.SelectOption ("==null==");
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNormalCurrentValueLabel").Text, Is.Empty);

      normalDropDownListBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNormalCurrentValueLabel").Text, Is.EqualTo (single));

      noAutoPostBackDropDownListBocEnumValue.SelectOption (single); // no auto post back
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (married));

      normalDropDownListBocEnumValue.SelectOption (single, Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (married));

      normalDropDownListBocEnumValue.SelectOption (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNormalCurrentValueLabel").Text, Is.EqualTo (divorced));
      Assert.That (home.Scope.FindIdEndingWith ("DropDownListNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (single));

      var normalListBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxNormal");
      var noAutoPostBackListBoxBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_ListBoxNoAutoPostBack");

      normalListBoxBocEnumValue.SelectOption ("==null==");
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNormalCurrentValueLabel").Text, Is.Empty);

      normalListBoxBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNormalCurrentValueLabel").Text, Is.EqualTo (single));

      noAutoPostBackListBoxBocEnumValue.SelectOption (single); // no auto post back
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (married));

      normalListBoxBocEnumValue.SelectOption (single, Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (married));

      normalListBoxBocEnumValue.SelectOption (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNormalCurrentValueLabel").Text, Is.EqualTo (divorced));
      Assert.That (home.Scope.FindIdEndingWith ("ListBoxNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (single));

      var normalRadioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListNormal");
      var noAutoPostBackRadioButtonListBocEnumValue = home.GetEnumValue().ByLocalID ("MarriageStatusField_RadioButtonListNoAutoPostBack");

      normalRadioButtonListBocEnumValue.SelectOption ("==null==");
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNormalCurrentValueLabel").Text, Is.Empty);

      normalRadioButtonListBocEnumValue.SelectOption (single);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNormalCurrentValueLabel").Text, Is.EqualTo (single));

      noAutoPostBackRadioButtonListBocEnumValue.SelectOption (single); // no auto post back
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (married));

      normalRadioButtonListBocEnumValue.SelectOption (single, Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (married));

      normalRadioButtonListBocEnumValue.SelectOption (divorced);
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNormalCurrentValueLabel").Text, Is.EqualTo (divorced));
      Assert.That (home.Scope.FindIdEndingWith ("RadioButtonListNoAutoPostBackCurrentValueLabel").Text, Is.EqualTo (single));
    }

    private WxePageObject Start ()
    {
      return Start ("BocEnumValue");
    }
  }
}