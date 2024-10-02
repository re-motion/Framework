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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocAutoCompleteReferenceValue;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocAutoCompleteReferenceValueControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource(typeof(DisabledTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [TestCaseSource(typeof(ReadOnlyTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [TestCaseSource(typeof(LabelTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [TestCaseSource(typeof(ValidationErrorTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    public void GenericTests (GenericSelectorTestAction<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject> testAction)
    {
      testAction(Helper, e => e.AutoCompletes(), "autoCompleteReferenceValue");
    }

    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [TestCaseSource(typeof(IndexControlSelectorTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [TestCaseSource(typeof(FirstControlSelectorTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [TestCaseSource(typeof(SingleControlSelectorTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [TestCaseSource(typeof(DomainPropertyControlSelectorTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [TestCaseSource(typeof(DisplayNameControlSelectorTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject> testAction)
    {
      testAction(Helper, e => e.AutoCompletes(), "autoCompleteReferenceValue");
    }

    /// <summary>
    /// Tests that the various parts of the <see cref="BocAutoCompleteReferenceValueControlObject"/> can be annotated when using the screenshot API.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest ()
    {
      var home = Start();

      var control = home.AutoCompletes().GetByID("body_DataEditControl_PartnerField_NoAutoPostBack");
      var fluentControl = control.ForControlObjectScreenshot();

      Helper.RunScreenshotTestExact<FluentScreenshotElement<BocAutoCompleteReferenceValueControlObject>, BocAutoCompleteReferenceValueControlObjectTest>(
          fluentControl,
          ScreenshotTestingType.Both,
          (builder, target) =>
          {
            builder.AnnotateBox(target, Pens.Black, WebPadding.Inner);

            builder.AnnotateBox(target.GetDropDownButton(), Pens.Red, WebPadding.Inner);
            builder.AnnotateBox(target.GetOptionsMenu(), Pens.Blue, WebPadding.Inner);
            builder.AnnotateBox(target.GetValue(), Pens.Green, WebPadding.Inner);

            builder.Crop(target, new WebPadding(1));
          });
    }

    /// <summary>
    /// Tests that all the navigation and select methods are working and that 
    /// the items are correctly annotated when using the screenshot API.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_AutoComplete ()
    {
      var home = Start();

      var control = home.AutoCompletes().GetByID("body_DataEditControl_PartnerField_NoAutoPostBack");
      var input = control.ForControlObjectScreenshot();
      var selectList = input.GetSelectList();

      if (control.IsReadOnly())
        Assert.Fail("This test requires the control to be not read-only.");

      input.SetValue(string.Empty);
      selectList.WaitUntilVisible();

      Helper
          .RunScreenshotTestExact
          <FluentScreenshotElement<ScreenshotBocAutoCompleteReferenceValueSelectList>, BocAutoCompleteReferenceValueControlObjectTest>(
              selectList,
              ScreenshotTestingType.Desktop,
              (builder, target) =>
              {
                builder.AnnotateBox(target, Pens.Transparent, WebPadding.Inner);

                builder.AnnotateBox(target.GetSelectedItem(), Pens.Blue, WebPadding.Inner);

                target.NextPage();
                builder.AnnotateBox(target.GetSelectedItem(), Pens.Green, WebPadding.Inner);

                target.NextItem();
                builder.AnnotateBox(target.GetSelectedItem(), Pens.Red, WebPadding.Inner);

                target.PreviousPage();
                builder.AnnotateBox(target.GetSelectedItem(), Pens.Yellow, WebPadding.Inner);

                target.Select().WithIndex(5);
                builder.AnnotateBox(target.GetSelectedItem(), Pens.Magenta, WebPadding.Inner);

                target.PreviousItem();
                builder.AnnotateBox(target.GetSelectedItem(), Pens.Pink, WebPadding.Inner);

                target.Select().WithDisplayText("B, B");
                builder.AnnotateBox(target.GetSelectedItem(), Pens.Chartreuse, WebPadding.Inner);

                builder.Crop(target);
              });
    }

    /// <summary>
    /// Tests the visibility of the auto-complete (show via input, wait until visible, hide).
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_AutoCompleteVisibility ()
    {
      var home = Start();

      var control = home.AutoCompletes().GetByID("body_DataEditControl_PartnerField_NoAutoPostBack");
      var input = control.ForControlObjectScreenshot();
      var selectList = input.GetSelectList();

      if (control.IsReadOnly())
        Assert.Fail("This test requires the control to be not read-only.");

      Assert.That(selectList.IsVisible(), Is.False);

      input.SetValue(string.Empty);
      selectList.WaitUntilVisible();
      Assert.That(selectList.IsVisible(), Is.True);

      selectList.Hide();
      Assert.That(selectList.IsVisible(), Is.False);

      selectList.Show();
      Assert.That(selectList.IsVisible(), Is.True);
    }

    /// <summary>
    /// Tests that the bounding box is correctly displayed when using the screenshot API.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_Popup ()
    {
      const string nonBreakingSpace = " ";

      var home = Start();

      var control = home.AutoCompletes().GetByID("body_DataEditControl_PartnerField_NoAutoPostBack");
      var fluentControl = control.ForControlObjectScreenshot();
      var informationPopup = fluentControl.GetInformationPopup();

      if (control.IsReadOnly())
        Assert.Fail("This test requires the control to be not read-only.");

      informationPopup.Display(nonBreakingSpace);
      informationPopup.WaitUntilVisible();

      Helper
          .RunScreenshotTestExact
          <FluentScreenshotElement<ScreenshotBocAutoCompleteReferenceValueInformationPopup>, BocAutoCompleteReferenceValueControlObjectTest>(
              informationPopup,
              ScreenshotTestingType.Both,
              (builder, target) =>
              {
                builder.AnnotateBox(target, Pens.Transparent, WebPadding.Inner);

                builder.Crop(target);
              });
    }

    /// <summary>
    /// Tests that the popup can display custom text via the <c>Display</c> method.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_PopupDisplay ()
    {
      const string text = "hello";

      var home = Start();

      var control = home.AutoCompletes().GetByID("body_DataEditControl_PartnerField_NoAutoPostBack");
      var fluentControl = control.ForControlObjectScreenshot();
      var informationPopup = fluentControl.GetInformationPopup();

      if (control.IsReadOnly())
        Assert.Fail("This test requires the control to be not read-only.");

      Assert.That(informationPopup.IsVisible(), Is.False);

      informationPopup.Display(text);
      Assert.That(informationPopup.IsVisible(), Is.True);
      Assert.That(informationPopup.GetTarget().WebElement.Text, Is.EqualTo(text));
    }

    /// <summary>
    /// Tests the visibility of the popup (show via input, wait until visible, hide).
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_PopupVisibility ()
    {
      const string search = "do not find anything please";

      var home = Start();

      var control = home.AutoCompletes().GetByID("body_DataEditControl_PartnerField_NoAutoPostBack");
      var fluentControl = control.ForControlObjectScreenshot();
      var informationPopup = fluentControl.GetInformationPopup();

      if (control.IsReadOnly())
        Assert.Fail("This test requires the control to be not read-only.");

      Assert.That(informationPopup.IsVisible(), Is.False);

      fluentControl.SetValue(search);

      informationPopup.WaitUntilVisible();
      Assert.That(informationPopup.IsVisible(), Is.True);

      informationPopup.Hide();
      Assert.That(informationPopup.IsVisible(), Is.False);
    }

    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_DerivedType ()
    {
      var home = Start();
      var controlObjectContext = home.AutoCompletes().GetByLocalID("body_DataEditControl_PartnerField_NoAutoPostBack").Context;
      var controlObject = new DerivedBocAutoCompleteReferenceValueControlObject(controlObjectContext);
      var fluentControlObject = controlObject.ForControlObjectScreenshot();

      Assert.That(fluentControlObject.GetSelectList(), Is.Not.Null);
      var fluentInformationPopup = fluentControlObject.GetInformationPopup();
      Assert.That(fluentInformationPopup, Is.Not.Null);
      Assert.That(fluentControlObject.GetDropDownButton(), Is.Not.Null);
      Assert.That(fluentControlObject.GetOptionsMenu(), Is.Not.Null);
      Assert.That(fluentControlObject.GetValue(), Is.Not.Null);
      Assert.That(fluentControlObject.IsReadOnly(), Is.Not.Null);
      Assert.That(() => fluentControlObject.SetValue(""), Throws.Nothing);

      var derivedInformationPopup = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocAutoCompleteReferenceValueInformationPopup(fluentInformationPopup.GetTarget().FluentAutoComplete));
      const string nonBreakingSpace = " ";
      Assert.That(() => derivedInformationPopup.Display(nonBreakingSpace), Throws.Nothing);
      Assert.That(() => derivedInformationPopup.WaitUntilVisible(), Throws.Nothing);
      Assert.That(derivedInformationPopup.IsVisible(), Is.Not.Null);
      Assert.That(() => derivedInformationPopup.Hide(), Throws.Nothing);

      var derivedSelectList = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotBocAutoCompleteReferenceValueSelectList(fluentInformationPopup.GetTarget().FluentAutoComplete));
      Assert.That(() => derivedSelectList.Hide(), Throws.Nothing);
      fluentControlObject.SetValue(string.Empty);
      Assert.That(() => derivedSelectList.Show(), Throws.Nothing);
      Assert.That(() => derivedSelectList.WaitUntilVisible(), Throws.Nothing);
      Assert.That(derivedSelectList.IsVisible(), Is.Not.Null);
      Assert.That(() => derivedSelectList.NextItem(), Throws.Nothing);
      Assert.That(() => derivedSelectList.PreviousItem(), Throws.Nothing);
      Assert.That(() => derivedSelectList.NextPage(), Throws.Nothing);
      Assert.That(() => derivedSelectList.PreviousPage(), Throws.Nothing);
      Assert.That(() => derivedSelectList.Select(), Throws.Nothing);
      Assert.That(() => derivedSelectList.Select(0), Throws.Nothing);
      Assert.That(derivedSelectList.GetSelectedItem(), Is.Not.Null);
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.AutoCompletes().GetByLocalID("Disabled");

      Assert.That(control.IsDisabled(), Is.True);
      Assert.That(
          () => control.FillWith("text"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "FillWith").Message));
      Assert.That(
          () => control.FillWith("text", FinishInput.Promptly),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "FillWith").Message));
      Assert.That(
          () => control.SelectFirstMatch("DoesntMatter"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SelectFirstMatch").Message));
      Assert.That(
          () => control.SelectFirstMatch("DoesntMatter", FinishInput.WithTab),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SelectFirstMatch").Message));
    }

    [Test]
    public void TestIsReadOnly_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.AutoCompletes().GetByLocalID("PartnerField_ReadOnly");

      Assert.That(control.IsReadOnly(), Is.True);
      Assert.That(() => control.FillWith("text"), Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
      Assert.That(
          () => control.FillWith("text", FinishInput.Promptly),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
      Assert.That(
          () => control.SelectFirstMatch("DoesntMatter"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
      Assert.That(
          () => control.SelectFirstMatch("DoesntMatter", FinishInput.WithTab),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");
      Assert.That(bocAutoComplete.GetText(), Is.EqualTo("D, A"));

      bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_ReadOnly");
      Assert.That(bocAutoComplete.GetText(), Is.EqualTo("D, A"));

      bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Disabled");
      Assert.That(bocAutoComplete.GetText(), Is.EqualTo("D, A"));

      bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_NoAutoPostBack");
      Assert.That(bocAutoComplete.GetText(), Is.EqualTo("D, A"));

      bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_NoCommandNoMenu");
      Assert.That(bocAutoComplete.GetText(), Is.EqualTo("D, A"));
    }

    [Test]
    public void TestFillWith ()
    {
      var home = Start();

      const string baLabel = "c8ace752-55f6-4074-8890-130276ea6cd1";
      const string daLabel = "00000000-0000-0000-0000-000000000009";

      {
        var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocAutoComplete);
        bocAutoComplete.FillWith("Invalid");
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindIdEndingWith("BOUINormalLabel").Text, Is.Empty);
      }

      {
        var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");
        bocAutoComplete.FillWith("B, A");
        Assert.That(home.Scope.FindIdEndingWith("BOUINormalLabel").Text, Is.EqualTo(baLabel));
      }

      {
        var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_NoAutoPostBack");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocAutoComplete);
        bocAutoComplete.FillWith("B, A"); // no auto post back
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindIdEndingWith("BOUINoAutoPostBackLabel").Text, Is.EqualTo(daLabel));
      }

      {
        var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocAutoComplete);
        bocAutoComplete.FillWith("B, A", Opt.ContinueImmediately()); // same value, does not trigger post back
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindIdEndingWith("BOUINoAutoPostBackLabel").Text, Is.EqualTo(daLabel));
      }

      {
        var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");
        bocAutoComplete.FillWith("D, A");
        Assert.That(home.Scope.FindIdEndingWith("BOUINormalLabel").Text, Is.EqualTo(daLabel));
        Assert.That(home.Scope.FindIdEndingWith("BOUINoAutoPostBackLabel").Text, Is.EqualTo(baLabel));
      }
    }

    [Test]
    public void TestSelectFirstMatch ()
    {
      var home = Start();

      const string baLabel = "c8ace752-55f6-4074-8890-130276ea6cd1"; //B, A
      const string daLabel = "00000000-0000-0000-0000-000000000009"; //D, D
      const string dLabel = "a2752869-e46b-4cfa-b89f-0b824e42b250"; //D, 

      {
        var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocAutoComplete);
        bocAutoComplete.SelectFirstMatch("B,");
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(bocAutoComplete.GetText(), Is.EqualTo("B, A"));
        Assert.That(home.Scope.FindIdEndingWith("BOUINormalLabel").Text, Is.EqualTo(baLabel));
      }

      {
        var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_NoAutoPostBack");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocAutoComplete);
        bocAutoComplete.SelectFirstMatch("B,"); // no auto post back
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
        Assert.That(bocAutoComplete.GetText(), Is.EqualTo("B, A"));
        Assert.That(home.Scope.FindIdEndingWith("BOUINoAutoPostBackLabel").Text, Is.EqualTo(daLabel));
      }

      {
        var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocAutoComplete);
        bocAutoComplete.SelectFirstMatch("B,", Opt.ContinueImmediately()); // same value, does not trigger post back
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
        Assert.That(bocAutoComplete.GetText(), Is.EqualTo("B, A"));
        Assert.That(home.Scope.FindIdEndingWith("BOUINoAutoPostBackLabel").Text, Is.EqualTo(daLabel));
      }

      {
        var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");
        bocAutoComplete.SelectFirstMatch("D");
        Assert.That(home.Scope.FindIdEndingWith("BOUINormalLabel").Text, Is.EqualTo(dLabel));
        Assert.That(home.Scope.FindIdEndingWith("BOUINoAutoPostBackLabel").Text, Is.EqualTo(baLabel));
      }
    }

    [Test]
    public void TestSelectFirstMatch_NoFilterMatches_ThrowsWebTestException ()
    {
      var home = Start();

      var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");
      Assert.That(
          () => bocAutoComplete.SelectFirstMatch("Invalid"),
          Throws.Exception.InstanceOf<WebTestException>()
              .With.Message.EqualTo(AssertionExceptionUtility.CreateExpectationException(Driver, "No matches were found for the specified filter: 'Invalid'.").Message));
    }

    [Test]
    public void TestGetDropDownMenu ()
    {
      var home = Start();

      var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");
      var dropDownMenu = bocAutoComplete.GetDropDownMenu();
      dropDownMenu.SelectItem("OptCmd2");
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderLabel").Text, Is.EqualTo("PartnerField_Normal"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedLabel").Text, Is.EqualTo("MenuItemClick"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedParameterLabel").Text, Is.EqualTo("OptCmd2|My menu command 2"));

      bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_ReadOnly");
      dropDownMenu = bocAutoComplete.GetDropDownMenu();
      dropDownMenu.SelectItem("OptCmd2");
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedSenderLabel").Text, Is.EqualTo("PartnerField_ReadOnly"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedLabel").Text, Is.EqualTo("MenuItemClick"));
      Assert.That(home.Scope.FindIdEndingWith("ActionPerformedParameterLabel").Text, Is.EqualTo("OptCmd2|My menu command 2"));
    }

    [Test]
    public void TestGetSearchServiceResults ()
    {
      var home = Start();

      var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");

      var searchResults = bocAutoComplete.GetSearchServiceResults("D", 0, 1);
      Assert.That(searchResults.Count, Is.EqualTo(1));
      Assert.That(searchResults[0].UniqueIdentifier, Is.EqualTo("a2752869-e46b-4cfa-b89f-0b824e42b250"));
      Assert.That(searchResults[0].DisplayName, Is.EqualTo("D, "));
      Assert.That(searchResults[0].IconUrl, Does.EndWith("/Remotion.ObjectBinding.Sample.Person.gif"));

      searchResults = bocAutoComplete.GetSearchServiceResults("D", 0, 5);
      Assert.That(searchResults.Count, Is.EqualTo(4));
      Assert.That(searchResults[0].DisplayName, Is.EqualTo("D, "));

      var offsetSearchResults = bocAutoComplete.GetSearchServiceResults("D", 1, 4, "1");
      Assert.That(offsetSearchResults, Is.EqualTo(searchResults.Skip(1)));

      searchResults = bocAutoComplete.GetSearchServiceResults("unexistentValue", 0, 5);
      Assert.That(searchResults.Count, Is.EqualTo(0));
    }

    [Test]
    public void TestGetSearchServiceResultsException ()
    {
      var home = Start();

      var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");

      Assert.That(() => bocAutoComplete.GetSearchServiceResults("throw", 0, 1), Throws.Exception.InstanceOf<WebServiceExecutionException>());
    }

    [Test]
    public void TestDropDownItemDisabledWithRequiredSelection ()
    {
      var home = Start();

      var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");
      var dropDownMenu = bocAutoComplete.GetDropDownMenu();

      dropDownMenu.Open();
      var menuItem = GetDropDownMenuItem3();
      Assert.That(menuItem.IsDisabled, Is.False);

      bocAutoComplete.FillWith("");
      menuItem = GetDropDownMenuItem3();
      Assert.That(menuItem.IsDisabled, Is.True);

      bocAutoComplete.SelectFirstMatch("D");
      menuItem = GetDropDownMenuItem3();
      Assert.That(menuItem.IsDisabled, Is.False);

      ItemDefinition GetDropDownMenuItem3 () => dropDownMenu.GetItemDefinitions().Single(x => x.ItemID == "OptCmd3");
    }

    [Test]
    public void JS_clear_WithAutoPostBack_ClearsBothInputs ()
    {
      var logger = Helper.LoggerFactory.CreateLogger<BocAutoCompleteReferenceValueControlObjectTest>();

      var home = Start();
      var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");
      var jsExecutor = JavaScriptExecutor.GetJavaScriptExecutor(bocAutoComplete);
      var inputScope = bocAutoComplete.Scope.FindChild("TextValue");
      var hiddenInputScope = bocAutoComplete.Scope.FindChild("KeyValue");

      var completionDetectionStrategy = new WxePostBackCompletionDetectionStrategy(1);
      var sequenceNumber = completionDetectionStrategy.PrepareWaitForCompletion(home.Context, logger);
      JavaScriptExecutor.ExecuteVoidStatement(jsExecutor, "arguments[0].clear()", inputScope.Native);
      completionDetectionStrategy.WaitForCompletion(home.Context, sequenceNumber, logger);

      var inputText = bocAutoComplete.GetText();
      var hiddenInputValue = hiddenInputScope.Value;
      Assert.That(inputText, Is.Empty);
      Assert.That(hiddenInputValue, Is.EqualTo("==null=="));
      Assert.That(home.Scope.FindIdEndingWith("BOUINormalLabel").Text, Is.Empty);
    }

    [Test]
    public void JS_clear_WithNoAutoPostBack_ClearsBothInputs ()
    {
      const string daLabel = "00000000-0000-0000-0000-000000000009";

      var home = Start();
      var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_NoAutoPostBack");
      var jsExecutor = JavaScriptExecutor.GetJavaScriptExecutor(bocAutoComplete);
      var inputScope = bocAutoComplete.Scope.FindChild("TextValue");
      var hiddenInputScope = bocAutoComplete.Scope.FindChild("KeyValue");

      JavaScriptExecutor.ExecuteVoidStatement(jsExecutor, "arguments[0].clear()", inputScope.Native);

      var inputText = bocAutoComplete.GetText();
      var hiddenInputValue = hiddenInputScope.Value;
      Assert.That(inputText, Is.Empty);
      Assert.That(hiddenInputValue, Is.EqualTo("==null=="));
      Assert.That(home.Scope.FindIdEndingWith("BOUINormalLabel").Text, Is.EqualTo(daLabel));
    }

    [Test]
    public void TestLoadMoreResults ()
    {
      var home = Start();

      var bocAutoComplete = home.AutoCompletes().GetByLocalID("PartnerField_Normal");

      var searchBox = bocAutoComplete.OpenSearchResults("testlist");
      Assert.That(searchBox.IsVisible, Is.True);

      var searchResults = searchBox.GetItems(e => e.Length > 0);
      Assert.That(
          searchResults.Select(e => e.Text),
          Is.EqualTo(CreatePersonItemsTexts(5, true)));

      // Explicit click on the placeholder to load more items
      searchResults[searchResults.Length - 1].Click();

      searchResults = searchBox.GetItems(e => e.Length >= 10);
      Assert.That(
          searchResults.Select(e => e.Text),
          Is.EqualTo(CreatePersonItemsTexts(10, true)));

      // Explicit expand again as the results aren't long enough to cause a scrollbar
      searchResults[searchResults.Length - 1].Click();

      // Scroll second to last element into view to load more items
      searchResults = searchBox.GetItems(e => e.Length >= 15);
      searchResults[searchResults.Length - 2].ScrollIntoView();

      var moreExpandedResults = searchBox.GetItems(e =>
      {
        if (e.Length >= 20)
          return true;

        // The initial ScrollIntoView might not work correctly, probably because the layout was not
        // updated yet. As such, we retry here again to trigger the load on scroll.
        searchResults[searchResults.Length - 2].ScrollIntoView();

        return false;
      });
      Assert.That(
          moreExpandedResults.Select(e => e.Text),
          Is.EqualTo(CreatePersonItemsTexts(20, false)));
    }

    private WxePageObject Start ()
    {
      return Start("BocAutoCompleteReferenceValue");
    }

    private string[] CreatePersonItemsTexts (int count, bool addPlaceholder)
    {
      var results = new List<string>();
      for (var i = 0; i < count; i++)
        results.Add($"testlist Person {i}");

      if (addPlaceholder)
        results.Add("...");

      return results.ToArray();
    }

    private class DerivedBocAutoCompleteReferenceValueControlObject : BocAutoCompleteReferenceValueControlObject
    {
      public DerivedBocAutoCompleteReferenceValueControlObject (ControlObjectContext context)
          : base(context)
      {
      }
    }

    private class DerivedScreenshotBocAutoCompleteReferenceValueInformationPopup : ScreenshotBocAutoCompleteReferenceValueInformationPopup
    {
      public DerivedScreenshotBocAutoCompleteReferenceValueInformationPopup (
          IFluentScreenshotElementWithCovariance<BocAutoCompleteReferenceValueControlObject> fluentControl)
          : base(fluentControl)
      {
      }
    }

    private class DerivedScreenshotBocAutoCompleteReferenceValueSelectList : ScreenshotBocAutoCompleteReferenceValueSelectList
    {
      public DerivedScreenshotBocAutoCompleteReferenceValueSelectList (
          IFluentScreenshotElementWithCovariance<BocAutoCompleteReferenceValueControlObject> fluentAutoComplete)
          : base(fluentAutoComplete)
      {
      }
    }
  }
}
