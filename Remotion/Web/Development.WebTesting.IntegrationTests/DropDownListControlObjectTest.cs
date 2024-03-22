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
using Remotion.Web.Development.WebTesting.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects.Selectors;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class DropDownListControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource(typeof(DisabledTestCaseFactory<DropDownListSelector, DropDownListControlObject>))]
    public void GenericTests (GenericSelectorTestAction<DropDownListSelector, DropDownListControlObject> testAction)
    {
      testAction(Helper, e => e.DropDownLists(), "dropDownList");
    }

    [Test]
    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<DropDownListSelector, DropDownListControlObject>))]
    [TestCaseSource(typeof(IndexControlSelectorTestCaseFactory<DropDownListSelector, DropDownListControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<DropDownListSelector, DropDownListControlObject>))]
    [TestCaseSource(typeof(FirstControlSelectorTestCaseFactory<DropDownListSelector, DropDownListControlObject>))]
    [TestCaseSource(typeof(SingleControlSelectorTestCaseFactory<DropDownListSelector, DropDownListControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<DropDownListSelector, DropDownListControlObject> testAction)
    {
      testAction(Helper, e => e.DropDownLists(), "dropDownList");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.DropDownLists().GetByLocalID("DropDownList5_Disabled");
      Assert.That(control.IsDisabled(), Is.True);
      Assert.That(
          () => control.SelectOption().WithDisplayText("EventItem"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SelectOption.WithDisplayText").Message));
      Assert.That(
          () => control.SelectOption().WithIndex(1),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SelectOption.WithIndex").Message));
      Assert.That(
          () => control.SelectOption().WithItemID("ItemID4"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SelectOption.WithItemID").Message));
      Assert.That(
          () => control.SelectOption("ItemID4"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SelectOption(value)").Message));
    }

    [Test]
    public void TestSelectors ()
    {
      var home = Start();

      var dropDownList = home.DropDownLists().GetByLocalID("MyDropDownList4");
      var completionDetection = new CompletionDetectionStrategyTestHelper(dropDownList);

      dropDownList.SelectOption().WithItemID("B");
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(dropDownList.Scope["value"], Is.EqualTo("B"));

      dropDownList.SelectOption().WithIndex(3);
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(dropDownList.Scope["value"], Is.EqualTo("C"));

      dropDownList.SelectOption().WithDisplayText("A");
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(dropDownList.Scope["value"], Is.EqualTo("B"));
    }

    [Test]
    public void TestGetSelectedOption ()
    {
      var home = Start();

      var dropDownList = home.DropDownLists().GetByLocalID("MyDropDownList");
      AssertSelectedOption(dropDownList, "Item1Value", -1, "Item1");
    }

    private static void AssertSelectedOption (
        DropDownListControlObject dropDownList,
        string expectedItemID,
        int expectedIndex,
        string expectedText)
    {
      var optionDefinition = dropDownList.GetSelectedOption();

      Assert.That(optionDefinition.ItemID, Is.EqualTo(expectedItemID));
      Assert.That(optionDefinition.Index, Is.EqualTo(expectedIndex));
      Assert.That(optionDefinition.Text, Is.EqualTo(expectedText));
      Assert.That(optionDefinition.IsSelected, Is.True);
    }

    [Test]
    public void TestGetOptionDefinitions ()
    {
      var home = Start();

      var dropDownList = home.DropDownLists().GetByLocalID("MyDropDownList");

      var options = dropDownList.GetOptionDefinitions();
      Assert.That(options.Count, Is.EqualTo(3));

      Assert.That(options[0].ItemID, Is.EqualTo("Item1Value"));
      Assert.That(options[0].Index, Is.EqualTo(1));
      Assert.That(options[0].Text, Is.EqualTo("Item1"));
      Assert.That(options[0].IsSelected, Is.True);

      Assert.That(options[2].ItemID, Is.EqualTo("Item3Value"));
      Assert.That(options[2].Index, Is.EqualTo(3));
      Assert.That(options[2].Text, Is.EqualTo("Item3"));
      Assert.That(options[2].IsSelected, Is.False);
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var dropDownList = home.DropDownLists().GetByLocalID("MyDropDownList");
      dropDownList.SelectOption("Item2Value");
      Assert.That(dropDownList.GetText(), Is.EqualTo("Item2"));
    }

    [Test]
    public void TestClickItem ()
    {
      var home = Start();

      var dropDownList = home.DropDownLists().GetByLocalID("MyDropDownList");
      var completionDetection = new CompletionDetectionStrategyTestHelper(dropDownList);

      dropDownList.SelectOption("Item3Value");
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("Item3|Item3Value"));

      dropDownList.SelectOption().WithIndex(2);
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("Item2|Item2Value"));

      dropDownList.SelectOption().WithDisplayText("Item1");
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("Item1|Item1Value"));
    }

    [Test]
    public void TestClickItem_NoPostback ()
    {
      var home = Start();

      var dropDownList = home.DropDownLists().GetByLocalID("MyDropDownList3");
      var completionDetection = new CompletionDetectionStrategyTestHelper(dropDownList);

      dropDownList.SelectOption("Item3Value", Opt.ContinueImmediately());
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(dropDownList.GetSelectedOption().ItemID, Is.EqualTo("Item3Value"));

      dropDownList.SelectOption().WithIndex(2, Opt.ContinueImmediately());
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(dropDownList.GetSelectedOption().ItemID, Is.EqualTo("Item2Value"));

      dropDownList.SelectOption().WithDisplayText("Item1", Opt.ContinueImmediately());
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
      Assert.That(dropDownList.GetSelectedOption().ItemID, Is.EqualTo("Item1Value"));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject>("DropDownListTest.wxe");
    }
  }
}
