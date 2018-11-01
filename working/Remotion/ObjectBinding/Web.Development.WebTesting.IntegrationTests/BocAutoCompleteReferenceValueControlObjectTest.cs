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
using System.Drawing;
using Coypu;
using NUnit.Framework;
using OpenQA.Selenium;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.ScreenshotCreation;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocAutoCompleteReferenceValue;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
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
    [RemotionTestCaseSource (typeof (DisabledTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [RemotionTestCaseSource (typeof (ReadOnlyTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [RemotionTestCaseSource (typeof (LabelTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [RemotionTestCaseSource (typeof (ValidationErrorTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    public void GenericTests (GenericSelectorTestAction<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject> testAction)
    {
      testAction (Helper, e => e.AutoCompletes(), "autoCompleteReferenceValue");
    }

    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [RemotionTestCaseSource (typeof (DomainPropertyControlSelectorTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    [RemotionTestCaseSource (typeof (DisplayNameControlSelectorTestCaseFactory<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject> testAction)
    {
      testAction (Helper, e => e.AutoCompletes(), "autoCompleteReferenceValue");
    }

    /// <summary>
    /// Tests that the various parts of the <see cref="BocAutoCompleteReferenceValueControlObject"/> can be annotated when using the screenshot API.
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ScreenshotTest ()
    {
      var home = Start();

      var control = home.AutoCompletes().GetByID ("body_DataEditControl_PartnerField_NoAutoPostBack");
      var fluentControl = control.ForControlObjectScreenshot();

      Helper.RunScreenshotTestExact<FluentScreenshotElement<BocAutoCompleteReferenceValueControlObject>, BocAutoCompleteReferenceValueControlObjectTest> (
          fluentControl,
          ScreenshotTestingType.Both,
          (builder, target) =>
          {
            builder.AnnotateBox (target, Pens.Black, WebPadding.Inner);

            builder.AnnotateBox (target.GetCommand(), Pens.Orange, WebPadding.Inner);
            builder.AnnotateBox (target.GetDropDownButton(), Pens.Red, WebPadding.Inner);
            builder.AnnotateBox (target.GetOptionsMenu(), Pens.Blue, WebPadding.Inner);
            builder.AnnotateBox (target.GetValue(), Pens.Green, WebPadding.Inner);

            builder.Crop (target, new WebPadding (1));
          });
    }

    /// <summary>
    /// Tests that all the navigation and select methods are working and that 
    /// the items are correctly annotated when using the screenshot API.
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ScreenshotTest_AutoComplete ()
    {
      const string data =
          "{data:{},result:''},{data:{},result:''},{data:{},result:''},{data:{},result:''},{data:{},result:''},{data:{DisplayName:' '},result:''},{data:{},result:''},{data:{},result:''},{data:{},result:''},{data:{},result:''}";

      var home = Start();

      var control = home.AutoCompletes().GetByID ("body_DataEditControl_PartnerField_NoAutoPostBack");
      var input = control.ForControlObjectScreenshot();
      var selectList = input.GetSelectList();

      if (control.IsReadOnly())
        Assert.Fail ("This test requires the control to be not read-only.");

      var executor = JavaScriptExecutor.GetJavaScriptExecutor (control);
      JavaScriptExecutor.ExecuteVoidStatement (
          executor,
          string.Format (
              "$(arguments[0]).getAutoCompleteSelectList().display ([{0}],'');",
              data),
          (IWebElement) input.GetValue().GetTarget().Native);

      selectList.Show();
      selectList.WaitUntilVisible();

      Helper
          .RunScreenshotTestExact
          <FluentScreenshotElement<ScreenshotBocAutoCompleteReferenceValueSelectList>, BocAutoCompleteReferenceValueControlObjectTest> (
              selectList,
              ScreenshotTestingType.Desktop,
              (builder, target) =>
              {
                builder.AnnotateBox (target, Pens.Transparent, WebPadding.Inner);

                builder.AnnotateBox (target.GetSelectedItem(), Pens.Blue, WebPadding.Inner);

                target.NextPage();
                builder.AnnotateBox (target.GetSelectedItem(), Pens.Green, WebPadding.Inner);

                target.NextItem();
                builder.AnnotateBox (target.GetSelectedItem(), Pens.Red, WebPadding.Inner);

                // bug currently the .PreviousPage goes back 9 items instead of only 8
                target.NextItem();
                target.PreviousPage();
                builder.AnnotateBox (target.GetSelectedItem(), Pens.Yellow, WebPadding.Inner);

                target.Select().WithIndex (5);
                builder.AnnotateBox (target.GetSelectedItem(), Pens.Magenta, WebPadding.Inner);

                target.PreviousItem();
                builder.AnnotateBox (target.GetSelectedItem(), Pens.Pink, WebPadding.Inner);

                target.Select().WithDisplayText (" ");
                builder.AnnotateBox (target.GetSelectedItem(), Pens.Chartreuse, WebPadding.Inner);

                builder.Crop (target);
              });
    }

    /// <summary>
    /// Tests the visibility of the auto-complete (show via input, wait until visible, hide).
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ScreenshotTest_AutoCompleteVisibility ()
    {
      var home = Start();

      var control = home.AutoCompletes().GetByID ("body_DataEditControl_PartnerField_NoAutoPostBack");
      var input = control.ForControlObjectScreenshot();
      var selectList = input.GetSelectList();

      if (control.IsReadOnly())
        Assert.Fail ("This test requires the control to be not read-only.");

      Assert.That (selectList.IsVisible(), Is.False);

      input.SetValue (string.Empty);
      selectList.WaitUntilVisible();
      Assert.That (selectList.IsVisible(), Is.True);

      selectList.Hide();
      Assert.That (selectList.IsVisible(), Is.False);

      selectList.Show();
      Assert.That (selectList.IsVisible(), Is.True);
    }

    /// <summary>
    /// Tests that the bounding box is correctly displayed when using the screenshot API.
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ScreenshotTest_Popup ()
    {
      const string nonBreakingSpace = " ";

      var home = Start();

      var control = home.AutoCompletes().GetByID ("body_DataEditControl_PartnerField_NoAutoPostBack");
      var fluentControl = control.ForControlObjectScreenshot();
      var informationPopup = fluentControl.GetInformationPopup();

      if (control.IsReadOnly())
        Assert.Fail ("This test requires the control to be not read-only.");

      informationPopup.Display (nonBreakingSpace);
      informationPopup.WaitUntilVisible();

      Helper
          .RunScreenshotTestExact
          <FluentScreenshotElement<ScreenshotBocAutoCompleteReferenceValueInformationPopup>, BocAutoCompleteReferenceValueControlObjectTest> (
              informationPopup,
              ScreenshotTestingType.Both,
              (builder, target) =>
              {
                builder.AnnotateBox (target, Pens.Transparent, WebPadding.Inner);

                builder.Crop (target);
              });
    }

    /// <summary>
    /// Tests that the popup can display custom text via the <c>Display</c> method.
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ScreenshotTest_PopupDisplay ()
    {
      const string text = "hello";

      var home = Start();

      var control = home.AutoCompletes().GetByID ("body_DataEditControl_PartnerField_NoAutoPostBack");
      var fluentControl = control.ForControlObjectScreenshot();
      var informationPopup = fluentControl.GetInformationPopup();

      if (control.IsReadOnly())
        Assert.Fail ("This test requires the control to be not read-only.");

      Assert.That (informationPopup.IsVisible(), Is.False);

      informationPopup.Display (text);
      Assert.That (informationPopup.IsVisible(), Is.True);
      Assert.That (informationPopup.GetTarget().WebElement.Text, Is.EqualTo (text));
    }

    /// <summary>
    /// Tests the visibility of the popup (show via input, wait until visible, hide).
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ScreenshotTest_PopupVisibility ()
    {
      const string search = "do not find anything please";

      var home = Start();

      var control = home.AutoCompletes().GetByID ("body_DataEditControl_PartnerField_NoAutoPostBack");
      var fluentControl = control.ForControlObjectScreenshot();
      var informationPopup = fluentControl.GetInformationPopup();

      if (control.IsReadOnly())
        Assert.Fail ("This test requires the control to be not read-only.");

      Assert.That (informationPopup.IsVisible(), Is.False);

      fluentControl.SetValue (search);

      informationPopup.WaitUntilVisible();
      Assert.That (informationPopup.IsVisible(), Is.True);

      informationPopup.Hide();
      Assert.That (informationPopup.IsVisible(), Is.False);
    }

    [Category ("Screenshot")]
    [Test]
    public void ScreenshotTest_DerivedType ()
    {
      var home = Start();
      var controlObjectContext = home.AutoCompletes().GetByLocalID ("body_DataEditControl_PartnerField_NoAutoPostBack").Context;
      var controlObject = new DerivedBocAutoCompleteReferenceValueControlObject (controlObjectContext);
      var fluentControlObject = controlObject.ForControlObjectScreenshot();

      Assert.That (fluentControlObject.GetSelectList(), Is.Not.Null);
      var fluentInformationPopup = fluentControlObject.GetInformationPopup();
      Assert.That (fluentInformationPopup, Is.Not.Null);
      Assert.That (fluentControlObject.GetCommand(), Is.Not.Null);
      Assert.That (fluentControlObject.GetDropDownButton(), Is.Not.Null);
      Assert.That (fluentControlObject.GetOptionsMenu(), Is.Not.Null);
      Assert.That (fluentControlObject.GetValue(), Is.Not.Null);
      Assert.That (fluentControlObject.IsReadOnly(), Is.Not.Null);
      Assert.That (() => fluentControlObject.SetValue (""), Throws.Nothing);

      var derivedInformationPopup = SelfResolvableFluentScreenshot.Create (
          new DerivedScreenshotBocAutoCompleteReferenceValueInformationPopup (fluentInformationPopup.GetTarget().FluentAutoComplete));
      const string nonBreakingSpace = " ";
      Assert.That (() => derivedInformationPopup.Display (nonBreakingSpace), Throws.Nothing);
      Assert.That (() => derivedInformationPopup.WaitUntilVisible(), Throws.Nothing);
      Assert.That (derivedInformationPopup.IsVisible(), Is.Not.Null);
      Assert.That (() => derivedInformationPopup.Hide(), Throws.Nothing);

      var derivedSelectList = SelfResolvableFluentScreenshot.Create (
          new DerivedScreenshotBocAutoCompleteReferenceValueSelectList (fluentInformationPopup.GetTarget().FluentAutoComplete));
      Assert.That (() => derivedSelectList.Hide(), Throws.Nothing);
      fluentControlObject.SetValue (string.Empty);
      Assert.That (() => derivedSelectList.Show (), Throws.Nothing);
      Assert.That (() => derivedSelectList.WaitUntilVisible(), Throws.Nothing);
      Assert.That (derivedSelectList.IsVisible(), Is.Not.Null);
      Assert.That (() => derivedSelectList.NextItem(), Throws.Nothing);
      Assert.That (() => derivedSelectList.PreviousItem(), Throws.Nothing);
      Assert.That (() => derivedSelectList.NextPage(), Throws.Nothing);
      Assert.That (() => derivedSelectList.PreviousPage(), Throws.Nothing);
      Assert.That (() => derivedSelectList.Select(), Throws.Nothing);
      Assert.That (() => derivedSelectList.Select(0), Throws.Nothing);
      Assert.That (derivedSelectList.GetSelectedItem(), Is.Not.Null);
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.AutoCompletes().GetByLocalID ("Disabled");

      Assert.That (control.IsDisabled(), Is.True);
      Assert.That (() => control.FillWith ("text"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException().Message));
      Assert.That (() => control.FillWith ("text", FinishInput.Promptly), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException().Message));
      Assert.That (() => control.ExecuteCommand(), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException().Message));
      Assert.That (() => control.SelectFirstMatch ("DoesntMatter"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException().Message));
      Assert.That (() => control.SelectFirstMatch ("DoesntMatter", FinishInput.WithTab), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException().Message));
    }

    [Test]
    public void TestIsReadOnly_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.AutoCompletes().GetByLocalID ("PartnerField_ReadOnly");

      Assert.That (control.IsReadOnly(), Is.True);
      Assert.That (() => control.FillWith ("text"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlReadOnlyException().Message));
      Assert.That (() => control.FillWith ("text", FinishInput.Promptly), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlReadOnlyException().Message));
      Assert.That (() => control.SelectFirstMatch ("DoesntMatter"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlReadOnlyException().Message));
      Assert.That (() => control.SelectFirstMatch ("DoesntMatter", FinishInput.WithTab), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlReadOnlyException().Message));
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal_AlternativeRendering");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_ReadOnly");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_ReadOnly_AlternativeRendering");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Disabled");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_NoAutoPostBack");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_NoCommandNoMenu");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));
    }

    [Test]
    public void TestFillWith ()
    {
      var home = Start();

      const string baLabel = "c8ace752-55f6-4074-8890-130276ea6cd1";
      const string daLabel = "00000000-0000-0000-0000-000000000009";

      var bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal");
      bocAutoComplete.FillWith ("Invalid");
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.Empty);

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal");
      bocAutoComplete.FillWith ("B, A");
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.EqualTo (baLabel));

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_NoAutoPostBack");
      bocAutoComplete.FillWith ("B, A"); // no auto post back
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (daLabel));

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal");
      bocAutoComplete.FillWith ("B, A", Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (daLabel));

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal");
      bocAutoComplete.FillWith ("D, A");
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.EqualTo (daLabel));
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (baLabel));
    }

    [Test]
    public void TestSelectFirstMatch ()
    {
      var home = Start();

      const string baLabel = "c8ace752-55f6-4074-8890-130276ea6cd1"; //B, A
      const string daLabel = "00000000-0000-0000-0000-000000000009"; //D, D
      const string dLabel = "a2752869-e46b-4cfa-b89f-0b824e42b250"; //D, 

      var bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal");
      bocAutoComplete.SelectFirstMatch ("B,");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("B, A"));
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.EqualTo (baLabel));

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_NoAutoPostBack");
      bocAutoComplete.SelectFirstMatch ("B,"); // no auto post back
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("B, A"));
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (daLabel));

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal");
      bocAutoComplete.SelectFirstMatch ("B,", Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("B, A"));
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (daLabel));

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal");
      bocAutoComplete.SelectFirstMatch ("D");
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.EqualTo (dLabel));
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (baLabel));
    }

    [Test]
    public void TestSelectFirstMatch_NoFilterMatches_ThrowsMissingHtmlException ()
    {
      var home = Start();

      var bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal");
      Assert.That (() => bocAutoComplete.SelectFirstMatch ("Invalid"), Throws.Exception.InstanceOf<MissingHtmlException>().With.Message.EqualTo ("No matches were found for the specified filter: 'Invalid'."));
    }

    [Test]
    public void TestExecuteCommand ()
    {
      var home = Start();

      var bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal");
      bocAutoComplete.ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal_AlternativeRendering");
      bocAutoComplete.ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal_AlternativeRendering"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_ReadOnly");
      bocAutoComplete.ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_ReadOnly"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_ReadOnly_AlternativeRendering");
      bocAutoComplete.ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_ReadOnly_AlternativeRendering"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);
    }

    [Test]
    public void TestGetDropDownMenu ()
    {
      var home = Start();

      var bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal");
      var dropDownMenu = bocAutoComplete.GetDropDownMenu();
      dropDownMenu.SelectItem ("OptCmd2");
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("MenuItemClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|My menu command 2"));

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal_AlternativeRendering");
      dropDownMenu = bocAutoComplete.GetDropDownMenu();
      dropDownMenu.SelectItem ("OptCmd2");
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal_AlternativeRendering"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("MenuItemClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|My menu command 2"));

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_ReadOnly");
      dropDownMenu = bocAutoComplete.GetDropDownMenu();
      dropDownMenu.SelectItem ("OptCmd2");
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_ReadOnly"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("MenuItemClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|My menu command 2"));

      bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_ReadOnly_AlternativeRendering");
      dropDownMenu = bocAutoComplete.GetDropDownMenu();
      dropDownMenu.SelectItem ("OptCmd2");
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_ReadOnly_AlternativeRendering"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("MenuItemClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|My menu command 2"));
    }

    [Test]
    public void TestGetSearchServiceResults ()
    {
      var home = Start();

      var bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal");

      var searchResults = bocAutoComplete.GetSearchServiceResults ("D", 1);
      Assert.That (searchResults.Count, Is.EqualTo (1));
      Assert.That (searchResults[0].UniqueIdentifier, Is.EqualTo ("a2752869-e46b-4cfa-b89f-0b824e42b250"));
      Assert.That (searchResults[0].DisplayName, Is.EqualTo ("D, "));
      Assert.That (searchResults[0].IconUrl, Is.EqualTo ("/Images/Remotion.ObjectBinding.Sample.Person.gif"));

      searchResults = bocAutoComplete.GetSearchServiceResults ("D", 5);
      Assert.That (searchResults.Count, Is.EqualTo (3));
      Assert.That (searchResults[0].DisplayName, Is.EqualTo ("D, "));

      searchResults = bocAutoComplete.GetSearchServiceResults ("unexistentValue", 5);
      Assert.That (searchResults.Count, Is.EqualTo (0));
    }

    [Test]
    public void TestGetExactSearchServiceResult ()
    {
      var home = Start();

      var bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal");

      var searchResult = bocAutoComplete.GetExactSearchServiceResult ("D, ");
      Assert.That (searchResult.UniqueIdentifier, Is.EqualTo ("a2752869-e46b-4cfa-b89f-0b824e42b250"));
      Assert.That (searchResult.DisplayName, Is.EqualTo ("D, "));
      Assert.That (searchResult.IconUrl, Is.EqualTo ("/Images/Remotion.ObjectBinding.Sample.Person.gif"));

      searchResult = bocAutoComplete.GetExactSearchServiceResult ("D");
      Assert.That (searchResult, Is.Null);
    }

    [Test]
    public void TestGetSearchServiceResultsException ()
    {
      var home = Start();

      var bocAutoComplete = home.AutoCompletes().GetByLocalID ("PartnerField_Normal");

      Assert.That (() => bocAutoComplete.GetSearchServiceResults ("throw", 1), Throws.Exception.InstanceOf<WebServiceExceutionException>());
    }

    private WxePageObject Start ()
    {
      return Start ("BocAutoCompleteReferenceValue");
    }

    private class DerivedBocAutoCompleteReferenceValueControlObject : BocAutoCompleteReferenceValueControlObject
    {
      public DerivedBocAutoCompleteReferenceValueControlObject (ControlObjectContext context)
          : base (context)
      {
      }
    }

    private class DerivedScreenshotBocAutoCompleteReferenceValueInformationPopup : ScreenshotBocAutoCompleteReferenceValueInformationPopup
    {
      public DerivedScreenshotBocAutoCompleteReferenceValueInformationPopup (
          IFluentScreenshotElementWithCovariance<BocAutoCompleteReferenceValueControlObject> fluentControl)
          : base (fluentControl)
      {
      }
    }

    private class DerivedScreenshotBocAutoCompleteReferenceValueSelectList : ScreenshotBocAutoCompleteReferenceValueSelectList
    {
      public DerivedScreenshotBocAutoCompleteReferenceValueSelectList (
          IFluentScreenshotElementWithCovariance<BocAutoCompleteReferenceValueControlObject> fluentAutoComplete)
          : base (fluentAutoComplete)
      {
      }
    }
  }
}