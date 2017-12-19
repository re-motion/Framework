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
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.ScreenshotCreation;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocDateTimeValueControlObjectTest : IntegrationTest
  {
    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    [RemotionTestCaseSource (typeof (DomainPropertyControlSelectorTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    [RemotionTestCaseSource (typeof (DisplayNameControlSelectorTestCaseFactory<BocDateTimeValueSelector, BocDateTimeValueControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<BocDateTimeValueSelector, BocDateTimeValueControlObject> testAction)
    {
      testAction (Helper, e => e.DateTimeValues(), "dateTimeValue");
    }

    /// <summary>
    /// Tests that the various parts of the <see cref="BocDateTimeValueControlObject"/> can be annotated when using the screenshot API.
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ScreenshotTest ()
    {
      var home = Start();

      var control = home.DateTimeValues().GetByID ("body_DataEditControl_DateOfBirthField_Normal");
      var fluentControl = control.ForControlObjectScreenshot();

      Helper.RunScreenshotTestExact<FluentScreenshotElement<BocDateTimeValueControlObject>, BocDateTimeValueControlObjectTest> (
          fluentControl,
          ScreenshotTestingType.Both,
          (builder, target) =>
          {
            builder.AnnotateBox (target, Pens.Black, WebPadding.Inner);

            builder.AnnotateBox (target.GetDateField(), Pens.Red, WebPadding.Inner);
            builder.AnnotateBox (target.GetDatePickerIcon(), Pens.Blue, WebPadding.Inner);
            builder.AnnotateBox (target.GetTimeField(), Pens.Green, WebPadding.Inner);

            builder.Crop (target, new WebPadding (1));
          });
    }

    [Category ("Screenshot")]
    [Test]
    public void ScreenshotTest_DatePicker ()
    {
      var home = Start();

      var control = home.DateTimeValues().GetByID ("body_DataEditControl_DateOfBirthField_Normal");
      var fluentControl = control.ForControlObjectScreenshot();
      var datePicker = fluentControl.GetDatePicker();

      datePicker.Open();

      Helper.RunScreenshotTestExact<FluentScreenshotElement<ScreenshotBocDateTimeValuePicker>, BocDateTimeValueControlObjectTest> (
          datePicker,
          ScreenshotTestingType.Both,
          (builder, target) =>
          {
            builder.AnnotateBox (target, Pens.DeepPink, WebPadding.Inner);

            builder.AnnotateBox (target.GetNavigationBar(), Pens.Orange, WebPadding.Inner);
            builder.AnnotateBox (target.GetPreviousMonthButton(), Pens.Green, WebPadding.Inner);
            builder.AnnotateBox (target.GetTitle(), Pens.Red, WebPadding.Inner);
            builder.AnnotateBox (target.GetNextMonthButton(), Pens.Blue, WebPadding.Inner);
            builder.AnnotateBox (target.GetWeekdayRow(), Pens.Magenta, WebPadding.Inner);
            builder.AnnotateBox (target.GetSelectedDay(), Pens.Yellow, WebPadding.Inner);

            builder.Crop (target, WebPadding.None);
          });
    }

    [Test]
    public void TestIsReadOnly ()
    {
      var home = Start();

      var bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Normal");
      Assert.That (bocDateTimeValue.IsReadOnly(), Is.False);

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_ReadOnly");
      Assert.That (bocDateTimeValue.IsReadOnly(), Is.True);
    }

    [Test]
    public void TestHasTimeField ()
    {
      var home = Start();

      var bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Normal");
      Assert.That (bocDateTimeValue.HasTimeField(), Is.True);

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_ReadOnlyDateOnly");
      Assert.That (bocDateTimeValue.HasTimeField(), Is.False);
    }

    [Test]
    public void TestGetDateTime ()
    {
      var home = Start();

      var bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Normal");
      Assert.That (bocDateTimeValue.GetDateTime(), Is.EqualTo (new DateTime (2008, 4, 4, 12, 0, 0)));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_ReadOnly");
      Assert.That (bocDateTimeValue.GetDateTime(), Is.EqualTo (new DateTime (2008, 4, 4, 12, 0, 0)));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Disabled");
      Assert.That (bocDateTimeValue.GetDateTime(), Is.EqualTo (new DateTime (2008, 4, 4, 12, 0, 0)));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_NoAutoPostBack");
      Assert.That (bocDateTimeValue.GetDateTime(), Is.EqualTo (new DateTime (2008, 4, 4, 12, 0, 0)));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_DateOnly");
      Assert.That (bocDateTimeValue.GetDateTime(), Is.EqualTo (new DateTime (2008, 4, 4, 0, 0, 0)));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_ReadOnlyDateOnly");
      Assert.That (bocDateTimeValue.GetDateTime(), Is.EqualTo (new DateTime (2008, 4, 4, 0, 0, 0)));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_WithSeconds");
      bocDateTimeValue.SetTime (new TimeSpan (13, 37, 42));
      Assert.That (bocDateTimeValue.GetDateTime(), Is.EqualTo (new DateTime (2008, 4, 4, 13, 37, 42)));
    }

    [Test]
    public void TestGetDateTimeAsString ()
    {
      var home = Start();

      var bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Normal");
      Assert.That (bocDateTimeValue.GetDateTimeAsString(), Is.EqualTo ("04.04.2008 12:00"));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_ReadOnly");
      Assert.That (bocDateTimeValue.GetDateTimeAsString(), Is.EqualTo ("04.04.2008 12:00"));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Disabled");
      Assert.That (bocDateTimeValue.GetDateTimeAsString(), Is.EqualTo ("04.04.2008 12:00"));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_NoAutoPostBack");
      Assert.That (bocDateTimeValue.GetDateTimeAsString(), Is.EqualTo ("04.04.2008 12:00"));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_DateOnly");
      Assert.That (bocDateTimeValue.GetDateTimeAsString(), Is.EqualTo ("04.04.2008"));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_ReadOnlyDateOnly");
      Assert.That (bocDateTimeValue.GetDateTimeAsString(), Is.EqualTo ("04.04.2008"));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_WithSeconds");
      bocDateTimeValue.SetTime (new TimeSpan (13, 37, 42));
      Assert.That (bocDateTimeValue.GetDateTimeAsString(), Is.EqualTo ("04.04.2008 13:37:42"));
    }

    [Test]
    public void TestSetDateTime ()
    {
      var home = Start();

      var initDateTime = new DateTime (2008, 4, 4, 12, 0, 0);
      var dateTime = new DateTime (1988, 3, 20, 4, 2, 0);
      var withSeconds = new DateTime (1988, 3, 20, 4, 2, 17);

      var bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetDateTime (dateTime);
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text), Is.EqualTo (dateTime));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_NoAutoPostBack");
      bocDateTimeValue.SetDateTime (dateTime); // no auto post back
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (initDateTime));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetDateTime (dateTime, Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (initDateTime));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetDateTime (initDateTime);
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text), Is.EqualTo (initDateTime));
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (dateTime));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_DateOnly");
      bocDateTimeValue.SetDateTime (dateTime);
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("DateOnlyCurrentValueLabel").Text), Is.EqualTo (dateTime.Date));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_WithSeconds");
      bocDateTimeValue.SetDateTime (withSeconds);
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("WithSecondsCurrentValueLabel").Text), Is.EqualTo (withSeconds));
    }

    [Test]
    public void TestSetDate ()
    {
      var home = Start();

      var initDateTime = new DateTime (2008, 4, 4, 12, 0, 0);
      var dateTime = new DateTime (1988, 3, 20, 4, 2, 0);
      var setDateTime = new DateTime (1988, 3, 20, 12, 0, 0);
      var withSeconds = new DateTime (1988, 3, 20, 4, 2, 17);
      var setWithSeconds = new DateTime (1988, 3, 20, 12, 0, 0);

      var bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetDate (dateTime);
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text), Is.EqualTo (setDateTime));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_NoAutoPostBack");
      bocDateTimeValue.SetDate (dateTime); // no auto post back
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (initDateTime));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetDate (dateTime, Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (initDateTime));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetDate (initDateTime);
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text), Is.EqualTo (initDateTime));
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (setDateTime));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_DateOnly");
      bocDateTimeValue.SetDate (dateTime);
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("DateOnlyCurrentValueLabel").Text), Is.EqualTo (setDateTime.Date));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_WithSeconds");
      bocDateTimeValue.SetDate (withSeconds);
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("WithSecondsCurrentValueLabel").Text), Is.EqualTo (setWithSeconds));
    }

    [Test]
    public void TestSetTime ()
    {
      var home = Start();

      var initTime = new TimeSpan (12, 0, 0);
      var setInitTime = new DateTime (2008, 4, 4, 12, 0, 0);
      var time = new TimeSpan (4, 2, 0);
      var setTime = new DateTime (2008, 4, 4, 4, 2, 0);
      var withSeconds = new TimeSpan (4, 2, 17);
      var setWithSeconds = new DateTime (2008, 4, 4, 4, 2, 17);

      var bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetTime (time);
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text), Is.EqualTo (setTime));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_NoAutoPostBack");
      bocDateTimeValue.SetTime (time); // no auto post back
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (setInitTime));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetTime (time, Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (setInitTime));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetTime (initTime);
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text), Is.EqualTo (setInitTime));
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text), Is.EqualTo (setTime));

      bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_WithSeconds");
      bocDateTimeValue.SetTime (withSeconds);
      Assert.That (DateTime.Parse (home.Scope.FindIdEndingWith ("WithSecondsCurrentValueLabel").Text), Is.EqualTo (setWithSeconds));
    }

    private WxePageObject Start ()
    {
      return Start ("BocDateTimeValue");
    }
  }
}