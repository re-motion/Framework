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
using System.Drawing;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.CompletionDetectionStrategies;
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
  public class BocDateTimeValueControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource(typeof(DisabledTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    [TestCaseSource(typeof(ReadOnlyTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    [TestCaseSource(typeof(LabelTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    [TestCaseSource(typeof(ValidationErrorTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    public void GenericTests (GenericSelectorTestAction<BocDateTimeValueSelector, BocDateTimeValueControlObject> testAction)
    {
      testAction(Helper, e => e.DateTimeValues(), "dateTimeValue");
    }

    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    [TestCaseSource(typeof(IndexControlSelectorTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    [TestCaseSource(typeof(FirstControlSelectorTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    [TestCaseSource(typeof(SingleControlSelectorTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    [TestCaseSource(typeof(DomainPropertyControlSelectorTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    [TestCaseSource(typeof(DisplayNameControlSelectorTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<BocDateTimeValueSelector, BocDateTimeValueControlObject> testAction)
    {
      testAction(Helper, e => e.DateTimeValues(), "dateTimeValue");
    }

    /// <summary>
    /// Tests that the various parts of the <see cref="BocDateTimeValueControlObject"/> can be annotated when using the screenshot API.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest ()
    {
      var home = Start();

      var control = home.DateTimeValues().GetByID("body_DataEditControl_DateOfBirthField_Normal");
      var fluentControl = control.ForControlObjectScreenshot();

      Helper.RunScreenshotTestExact<FluentScreenshotElement<BocDateTimeValueControlObject>, BocDateTimeValueControlObjectTest>(
          fluentControl,
          ScreenshotTestingType.Both,
          (builder, target) =>
          {
            builder.AnnotateBox(target, Pens.Black, WebPadding.Inner);

            builder.AnnotateBox(target.GetDateField(), Pens.Red, WebPadding.Inner);
            builder.AnnotateBox(target.GetDatePickerIcon(), Pens.Blue, WebPadding.Inner);
            builder.AnnotateBox(target.GetTimeField(), Pens.Green, WebPadding.Inner);

            builder.Crop(target, new WebPadding(1));
          });
    }

    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_DatePicker ()
    {
      var home = Start();

      var control = home.DateTimeValues().GetByID("body_DataEditControl_DateOfBirthField_Normal");
      var fluentControl = control.ForControlObjectScreenshot();
      var datePicker = fluentControl.GetDatePicker();

      datePicker.Open();

      Helper.RunScreenshotTestExact<FluentScreenshotElement<ScreenshotBocDateTimeValuePicker>, BocDateTimeValueControlObjectTest>(
          datePicker,
          ScreenshotTestingType.Both,
          (builder, target) =>
          {
            builder.AnnotateBox(target, Pens.DeepPink, WebPadding.Inner);

            builder.AnnotateBox(target.GetNavigationBar(), Pens.Orange, WebPadding.Inner);
            builder.AnnotateBox(target.GetPreviousMonthButton(), Pens.Green, WebPadding.Inner);
            builder.AnnotateBox(target.GetTitle(), Pens.Red, WebPadding.Inner);
            builder.AnnotateBox(target.GetNextMonthButton(), Pens.Blue, WebPadding.Inner);
            builder.AnnotateBox(target.GetWeekdayRow(), Pens.Magenta, WebPadding.Inner);
            builder.AnnotateBox(target.GetSelectedDay(), Pens.Yellow, WebPadding.Inner);

            builder.Crop(target, WebPadding.None);
          });
    }

    [Category("Screenshot")]
    [Test]
    public void ScreenshotTest_DerivedType ()
    {
      var home = Start();
      var controlObjectContext = home.DateTimeValues().GetByLocalID("body_DataEditControl_DateOfBirthField_Normal").Context;
      var controlObject = new DerivedBocDateTimeValueControlObject(controlObjectContext);
      var fluentControlObject = controlObject.ForControlObjectScreenshot();

      Assert.That(fluentControlObject.GetDateField(), Is.Not.Null);
      Assert.That(fluentControlObject.GetTimeField(), Is.Not.Null);
      Assert.That(fluentControlObject.GetDatePickerIcon(), Is.Not.Null);
      Assert.That(fluentControlObject.IsReadOnly(), Is.Not.Null);
      Assert.That(fluentControlObject.GetDateTime(), Is.Not.Null);
      Assert.That(fluentControlObject.GetDateTimeAsString(), Is.Not.Null);
      Assert.That(fluentControlObject.HasTimeField(), Is.Not.Null);

      var datePicker = fluentControlObject.GetDatePicker();
      Assert.That(datePicker, Is.Not.Null);

      var derivedSelectList = SelfResolvableFluentScreenshot.Create(new DerivedScreenshotBocDateTimeValuePicker(fluentControlObject));

      Assert.That(() => datePicker.Open(), Throws.Nothing);
      Assert.That(derivedSelectList.GetNavigationBar(), Is.Not.Null);
      Assert.That(derivedSelectList.GetTitle(), Is.Not.Null);
      Assert.That(derivedSelectList.GetNextMonthButton(), Is.Not.Null);
      Assert.That(derivedSelectList.GetPreviousMonthButton(), Is.Not.Null);
      Assert.That(derivedSelectList.GetWeekdayRow(), Is.Not.Null);
      Assert.That(derivedSelectList.GetSelectedDay(), Is.Not.Null);
      Assert.That(derivedSelectList.GetElement(), Is.Not.Null);
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.DateTimeValues().GetByLocalID("DateOfBirthField_Disabled");

      Assert.That(control.IsDisabled(), Is.True);
      Assert.That(
          () => control.SetDate(DateTime.MinValue),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SetDate").Message));
      Assert.That(() => control.SetDate(""),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SetDate").Message));
      Assert.That(
          () => control.SetDateTime(DateTime.MinValue),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SetDateTime").Message));
      Assert.That(
          () => control.SetTime(TimeSpan.MinValue),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SetTime").Message));
      Assert.That(
          () => control.SetTime(""),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SetTime").Message));
    }

    [Test]
    public void TestIsReadOnly_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.DateTimeValues().GetByLocalID("DateOfBirthField_ReadOnly");

      Assert.That(control.IsReadOnly(), Is.True);
      Assert.That(
          () => control.SetDate(DateTime.MinValue),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
      Assert.That(
          () => control.SetDate(""),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
      Assert.That(
          () => control.SetDateTime(DateTime.MinValue),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
      Assert.That(
          () => control.SetTime(TimeSpan.MinValue),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
      Assert.That(
          () => control.SetTime(""),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
    }

    [Test]
    public void TestHasTimeField ()
    {
      var home = Start();

      var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Normal");
      Assert.That(bocDateTimeValue.HasTimeField(), Is.True);

      bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_ReadOnlyDateOnly");
      Assert.That(bocDateTimeValue.HasTimeField(), Is.False);
    }

    [Test]
    public void TestGetDateTime ()
    {
      var home = Start();

      var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Normal");
      Assert.That(bocDateTimeValue.GetDateTime(), Is.EqualTo(new DateTime(2008, 4, 4, 12, 0, 0)));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_ReadOnly");
      Assert.That(bocDateTimeValue.GetDateTime(), Is.EqualTo(new DateTime(2008, 4, 4, 12, 0, 0)));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Disabled");
      Assert.That(bocDateTimeValue.GetDateTime(), Is.EqualTo(new DateTime(2008, 4, 4, 12, 0, 0)));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_NoAutoPostBack");
      Assert.That(bocDateTimeValue.GetDateTime(), Is.EqualTo(new DateTime(2008, 4, 4, 12, 0, 0)));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_DateOnly");
      Assert.That(bocDateTimeValue.GetDateTime(), Is.EqualTo(new DateTime(2008, 4, 4, 0, 0, 0)));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_ReadOnlyDateOnly");
      Assert.That(bocDateTimeValue.GetDateTime(), Is.EqualTo(new DateTime(2008, 4, 4, 0, 0, 0)));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_WithSeconds");
      bocDateTimeValue.SetTime(new TimeSpan(13, 37, 42));
      Assert.That(bocDateTimeValue.GetDateTime(), Is.EqualTo(new DateTime(2008, 4, 4, 13, 37, 42)));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfCitizenship_DateOnlyType");
      Assert.That(bocDateTimeValue.GetDateTime(), Is.EqualTo(new DateTime(2009, 5, 2, 0, 0, 0)));
    }

    [Test]
    public void TestGetDateTimeAsString ()
    {
      var home = Start();

      var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Normal");
      Assert.That(bocDateTimeValue.GetDateTimeAsString(), Is.EqualTo("04.04.2008 12:00"));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_ReadOnly");
      Assert.That(bocDateTimeValue.GetDateTimeAsString(), Is.EqualTo("04.04.2008 12:00"));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Disabled");
      Assert.That(bocDateTimeValue.GetDateTimeAsString(), Is.EqualTo("04.04.2008 12:00"));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_NoAutoPostBack");
      Assert.That(bocDateTimeValue.GetDateTimeAsString(), Is.EqualTo("04.04.2008 12:00"));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_DateOnly");
      Assert.That(bocDateTimeValue.GetDateTimeAsString(), Is.EqualTo("04.04.2008"));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_ReadOnlyDateOnly");
      Assert.That(bocDateTimeValue.GetDateTimeAsString(), Is.EqualTo("04.04.2008"));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_WithSeconds");
      bocDateTimeValue.SetTime(new TimeSpan(13, 37, 42));
      Assert.That(bocDateTimeValue.GetDateTimeAsString(), Is.EqualTo("04.04.2008 13:37:42"));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfCitizenship_DateOnlyType");
      Assert.That(bocDateTimeValue.GetDateTimeAsString(), Is.EqualTo("02.05.2009"));
    }

    [Test]
    public void TestSetDateTime ()
    {
      var home = Start();

      var initDateTime = new DateTime(2008, 4, 4, 12, 0, 0);
      var dateTime = new DateTime(1988, 3, 20, 4, 2, 0);
      var withSeconds = new DateTime(1988, 3, 20, 4, 2, 17);

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocDateTimeValue);
        bocDateTimeValue.SetDateTime(dateTime);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("NormalCurrentValueLabel").Text), Is.EqualTo(dateTime));
      }

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_NoAutoPostBack");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocDateTimeValue);
        bocDateTimeValue.SetDateTime(dateTime); // no auto post back
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo(initDateTime));
      }

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocDateTimeValue);
        bocDateTimeValue.SetDateTime(dateTime, Opt.ContinueImmediately()); // same value, does not trigger post back
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo(initDateTime));
      }

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Normal");
        bocDateTimeValue.SetDateTime(initDateTime);
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("NormalCurrentValueLabel").Text), Is.EqualTo(initDateTime));
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo(dateTime));
      }

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_DateOnly");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocDateTimeValue);
        bocDateTimeValue.SetDateTime(dateTime);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("DateOnlyCurrentValueLabel").Text), Is.EqualTo(dateTime.Date));
      }

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_WithSeconds");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocDateTimeValue);
        bocDateTimeValue.SetDateTime(withSeconds);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("WithSecondsCurrentValueLabel").Text), Is.EqualTo(withSeconds));
      }

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfCitizenship_DateOnlyType");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocDateTimeValue);
        bocDateTimeValue.SetDateTime(dateTime);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("DateOnlyTypeLabel").Text), Is.EqualTo(dateTime.Date));
      }
    }

    [Test]
    public void TestSetDate ()
    {
      var home = Start();

      var initDateTime = new DateTime(2008, 4, 4, 12, 0, 0);
      var dateTime = new DateTime(1988, 3, 20, 4, 2, 0);
      var setDateTime = new DateTime(1988, 3, 20, 12, 0, 0);
      var withSeconds = new DateTime(1988, 3, 20, 4, 2, 17);
      var setWithSeconds = new DateTime(1988, 3, 20, 12, 0, 0);

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocDateTimeValue);
        bocDateTimeValue.SetDate(dateTime);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("NormalCurrentValueLabel").Text), Is.EqualTo(setDateTime));
      }

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_NoAutoPostBack");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocDateTimeValue);
        bocDateTimeValue.SetDate(dateTime); // no auto post back
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo(initDateTime));
      }

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocDateTimeValue);
        bocDateTimeValue.SetDate(dateTime, Opt.ContinueImmediately()); // same value, does not trigger post back
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo(initDateTime));
      }

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Normal");
        bocDateTimeValue.SetDate(initDateTime);
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("NormalCurrentValueLabel").Text), Is.EqualTo(initDateTime));
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo(setDateTime));
      }

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_DateOnly");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocDateTimeValue);
        bocDateTimeValue.SetDate(dateTime);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("DateOnlyCurrentValueLabel").Text), Is.EqualTo(setDateTime.Date));
      }

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_WithSeconds");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocDateTimeValue);
        bocDateTimeValue.SetDate(withSeconds);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("WithSecondsCurrentValueLabel").Text), Is.EqualTo(setWithSeconds));
      }

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfCitizenship_DateOnlyType");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocDateTimeValue);
        bocDateTimeValue.SetDate(dateTime);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("DateOnlyTypeLabel").Text), Is.EqualTo(setDateTime.Date));
      }
    }

    [Test]
    public void TestSetTime ()
    {
      var home = Start();

      var initTime = new TimeSpan(12, 0, 0);
      var setInitTime = new DateTime(2008, 4, 4, 12, 0, 0);
      var time = new TimeSpan(4, 2, 0);
      var setTime = new DateTime(2008, 4, 4, 4, 2, 0);
      var withSeconds = new TimeSpan(4, 2, 17);
      var setWithSeconds = new DateTime(2008, 4, 4, 4, 2, 17);

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocDateTimeValue);
        bocDateTimeValue.SetTime(time);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("NormalCurrentValueLabel").Text), Is.EqualTo(setTime));
      }

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_NoAutoPostBack");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocDateTimeValue);
        bocDateTimeValue.SetTime(time); // no auto post back
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo(setInitTime));
      }

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocDateTimeValue);
        bocDateTimeValue.SetTime(time, Opt.ContinueImmediately()); // same value, does not trigger post back
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo(setInitTime));
      }

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Normal");
        bocDateTimeValue.SetTime(initTime);
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("NormalCurrentValueLabel").Text), Is.EqualTo(setInitTime));
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo(setTime));
      }

      {
        var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_WithSeconds");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocDateTimeValue);
        bocDateTimeValue.SetTime(withSeconds);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(DateTime.Parse(home.Scope.FindIdEndingWith("WithSecondsCurrentValueLabel").Text), Is.EqualTo(setWithSeconds));
      }
    }

    [Test]
    public void TestGetDateValidationError ()
    {
      var home = Start();

      var validateButton = home.GetValidateButton();
      var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Normal");

      bocDateTimeValue.SetDate("");

      validateButton.Click();

      var validateErrors = bocDateTimeValue.GetDateValidationErrors();

      Assert.That(validateErrors, Is.EquivalentTo(new [] { "Enter a date.", "Localized invalid dateOfBirth." }));
    }

    [Test]
    public void TestGetTimeValidationError ()
    {
      var home = Start();

      var validateButton = home.GetValidateButton();
      var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Normal");

      bocDateTimeValue.SetTime("");

      validateButton.Click();

      var validateErrors = bocDateTimeValue.GetTimeValidationErrors();

      Assert.That(validateErrors, Is.EquivalentTo(new [] { "Enter a time.", "Localized invalid dateOfBirth." }));
    }

    [Test]
    public void TestValidationErrorTimeValidationError ()
    {
      var home = Start();

      var validateButton = home.GetValidateButton();
      var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Normal");

      bocDateTimeValue.SetTime("");

      validateButton.Click();

      // Currently, Validation Errors are rendered on both the date and the time scope.
      // Therefore, GetValidationErrors() only returns the ValidationError of one Field (Date) to not duplicate the validation errors.
      // This test documents this behavior and will fail when it is changed
      // to remind the updater to update bocDateTimeValue.GetValidationErrors () to return the errors of both Fields.
      var validateErrors = bocDateTimeValue.GetValidationErrors();

      Assert.That(validateErrors, Is.EquivalentTo(new [] { "Enter a time.", "Localized invalid dateOfBirth." }));
    }

    [Test]
    public void TestGetDateTimeValidationError ()
    {
      var home = Start();

      var validateButton = home.GetValidateButton();
      var bocDateTimeValue = home.DateTimeValues().GetByLocalID("DateOfBirthField_Normal");

      bocDateTimeValue.SetDate("");

      bocDateTimeValue.SetTime("");

      validateButton.Click();

      var validateErrors = bocDateTimeValue.GetValidationErrors();

      Assert.That(validateErrors, Is.EquivalentTo(new [] { "Enter a date and time.", "Localized invalid dateOfBirth." }));
    }

    private WxePageObject Start ()
    {
      return Start("BocDateTimeValue");
    }

    private class DerivedBocDateTimeValueControlObject : BocDateTimeValueControlObject
    {
      public DerivedBocDateTimeValueControlObject (ControlObjectContext context)
          : base(context)
      {
      }
    }

    private class DerivedScreenshotBocDateTimeValuePicker : ScreenshotBocDateTimeValuePicker
    {
      public DerivedScreenshotBocDateTimeValuePicker (IFluentScreenshotElementWithCovariance<BocDateTimeValueControlObject> fluentDateTimeValue)
          : base(fluentDateTimeValue)
      {
      }
    }
  }
}
