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
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  [RequiresUserInterface]
  public class DropDownMenuControlObjectTest : IntegrationTest
  {
    // Note: the <see cref="T:DropDownMenu.Mode"/>=<see cref="T:MenuMode.ContextMenu"/> option is tested indirectly by the BocTreeViewControlObjectTest.

    [Test]
    [TestCaseSource(typeof(DisabledTestCaseFactory<DropDownMenuSelector, DropDownMenuControlObject>))]
    public void GenericTests (GenericSelectorTestAction<DropDownMenuSelector, DropDownMenuControlObject> testAction)
    {
      testAction(Helper, e => e.DropDownMenus(), "dropDownMenu");
    }

    [Test]
    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<DropDownMenuSelector, DropDownMenuControlObject>))]
    [TestCaseSource(typeof(IndexControlSelectorTestCaseFactory<DropDownMenuSelector, DropDownMenuControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<DropDownMenuSelector, DropDownMenuControlObject>))]
    [TestCaseSource(typeof(TextContentControlSelectorTestCaseFactory<DropDownMenuSelector, DropDownMenuControlObject>))]
    [TestCaseSource(typeof(FirstControlSelectorTestCaseFactory<DropDownMenuSelector, DropDownMenuControlObject>))]
    [TestCaseSource(typeof(SingleControlSelectorTestCaseFactory<DropDownMenuSelector, DropDownMenuControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<DropDownMenuSelector, DropDownMenuControlObject> testAction)
    {
      testAction(Helper, e => e.DropDownMenus(), "dropDownMenu");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var disabledControl = home.DropDownMenus().GetByLocalID("MyDropDownMenu_Disabled");
      Assert.That(disabledControl.IsDisabled(), Is.True);
      Assert.That(
          () => disabledControl.GetItemDefinitions(),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "GetItemDefinitions").Message));
      Assert.That(
          () => disabledControl.SelectItem().WithDisplayText("EventItem"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SelectItem.WithDisplayText").Message));
      Assert.That(
          () => disabledControl.SelectItem().WithDisplayTextContains("Href"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SelectItem.WithDisplayTextContains").Message));
      Assert.That(
          () => disabledControl.SelectItem().WithIndex(1),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SelectItem.WithIndex").Message));
      Assert.That(
          () => disabledControl.SelectItem().WithHtmlID("body_MyDropDownMenu_Disabled_3"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SelectItem.WithHtmlID").Message));
      Assert.That(
          () => disabledControl.SelectItem().WithItemID("ItemID4"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SelectItem.WithItemID").Message));
      Assert.That(
          () => disabledControl.SelectItem("ItemID4"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SelectItem(itemID)").Message));
    }

    [Test]
    public void TestDropDownItemIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var enabledControl = home.DropDownMenus().GetByLocalID("MyDropDownMenu");
      Assert.That(
          () => enabledControl.SelectItem("ItemID3"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateCommandDisabledException(Driver, "SelectItem(itemID)").Message));
      Assert.That(
          () => enabledControl.SelectItem().WithDisplayText("NoneItem"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateCommandDisabledException(Driver, "SelectItem.WithDisplayText").Message));
      Assert.That(
          () => enabledControl.SelectItem().WithDisplayTextContains("None"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateCommandDisabledException(Driver, "SelectItem.WithDisplayTextContains").Message));
      Assert.That(
          () => enabledControl.SelectItem("ItemID6"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateCommandDisabledException(Driver, "SelectItem(itemID)").Message));
      Assert.That(
          () => enabledControl.SelectItem().WithDisplayText("DisabledItem"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateCommandDisabledException(Driver, "SelectItem.WithDisplayText").Message));
      Assert.That(
          () => enabledControl.SelectItem().WithDisplayTextContains("Disabled"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateCommandDisabledException(Driver, "SelectItem.WithDisplayTextContains").Message));
    }

    [Test]
    public void TestIsTitleHidden_GetByTextContent ()
    {
      var home = Start();

      Assert.That(home.DropDownMenus().GetByTextContent("Title should not be displayed"), Is.Not.Null);
    }

    [Test]
    public void TestUmlaut_GetByTextContent ()
    {
      var home = Start();

      Assert.That(home.DropDownMenus().GetByTextContent("UmlautÖ"), Is.Not.Null);
    }

    [Test]
    public void TestGetItemDefinitions ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenu");

      var items = dropDownMenu.GetItemDefinitions();
      Assert.That(items.Count, Is.EqualTo(6));

      Assert.That(items[0].ItemID, Is.EqualTo("ItemID1"));
      Assert.That(items[0].Index, Is.EqualTo(1));
      Assert.That(items[0].Text, Is.EqualTo("EventItem"));
      Assert.That(items[0].IsDisabled, Is.False);

      Assert.That(items[2].IsDisabled, Is.True);

      Assert.That(items[4].ItemID, Is.EqualTo("ItemID5"));
      Assert.That(items[4].Index, Is.EqualTo(5));
      Assert.That(items[4].Text, Is.EqualTo(""));
      Assert.That(items[4].IsDisabled, Is.False);
    }

    [Test]
    public void TestClickItem ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenu");
      var completionDetection = new CompletionDetectionStrategyTestHelper(dropDownMenu);

      dropDownMenu.SelectItem("ItemID5");
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("ItemID5|Event"));

      dropDownMenu.SelectItem().WithIndex(2);
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxeResetCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.Empty);

      dropDownMenu.SelectItem().WithHtmlID("body_MyDropDownMenu_3");
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("ItemID4|WxeFunction"));

      dropDownMenu.SelectItem().WithDisplayText("EventItem");
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("ItemID1|Event"));

      dropDownMenu.SelectItem().WithDisplayTextContains("xeFun");
      Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
      Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("ItemID4|WxeFunction"));
    }

    [Test]
    public void TestDropDownSelectItem_AfterGetItemDefinitions ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenu");

      dropDownMenu.GetItemDefinitions();
      Assert.That(() => dropDownMenu.SelectItem("ItemID5"), Throws.Nothing);
    }

    [Test]
    public void TestDropDownMenuControlObject_IsOpen ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenu");
      Assert.That(dropDownMenu.IsOpen(), Is.False);
    }

    [Test]
    public void TestDropDownMenuControlObject_OpenDropDownMenu ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenu");

      dropDownMenu.Open();
      Assert.That(dropDownMenu.IsOpen(), Is.True);

      //Open it a second time to ensure it stays open
      dropDownMenu.Open();
      Assert.That(dropDownMenu.IsOpen(), Is.True);
    }

    [Test]
    public void TestDropDownMenuControlObject_OpenDropDownMenuWithDelay_WaitsUntilDropDownIsOpen ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenu_Delayed");

      dropDownMenu.Open();
      Assert.That(dropDownMenu.IsOpen(), Is.True);
    }

    [Test]
    [Category("LongRunning")]
    public void TestDropDownMenuControlObject_OpenDropDownMenuWithDelayGreaterThanTimeout_FailsWithException ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenu_DelayedLongerThanTimeout");

      Assert.That(
          () => dropDownMenu.Open(),
          Throws.TypeOf<WebTestException>()
              .With.Message.EqualTo(AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to open the menu.").Message));
      Assert.That(dropDownMenu.IsOpen(), Is.False);
    }

    [Test]
    public void TestDropDownMenuControlObject_OpenDropDownMenuWithError_FailsWithException ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenu_Error");

      Assert.That(
          () => dropDownMenu.Open(),
          Throws.TypeOf<WebTestException>()
              .With.Message.EqualTo(AssertionExceptionUtility.CreateExpectationException(Driver, "Unable to open the menu.").Message));
      Assert.That(dropDownMenu.IsOpen(), Is.False);
    }

    [Test]
    public void TestDropDownMenuControlObject_CloseDropDownMenu ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenu");

      dropDownMenu.Open();

      dropDownMenu.Close();
      Assert.That(dropDownMenu.IsOpen(), Is.False);

      //Close it a second time to ensure it stays closed
      dropDownMenu.Close();
      Assert.That(dropDownMenu.IsOpen(), Is.False);
    }

    [Test]
    public void TestDropDownMenuControlObject_IsOpen_OnOtherDropDownMenu ()
    {
      var home = Start();

      var myDropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenu");
      var myDropDownMenu2 = home.DropDownMenus().GetByLocalID("MyDropDownMenu2");

      myDropDownMenu.Open();
      Assert.That(myDropDownMenu2.IsOpen(), Is.False);
    }

    [Test]
    public void TestHasStandardButtonType ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenu");
      Assert.That(dropDownMenu.GetButtonType(), Is.EqualTo(ButtonType.Standard));
    }

    [Test]
    public void TestHasPrimaryButtonType ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenuPrimary");
      Assert.That(dropDownMenu.GetButtonType(), Is.EqualTo(ButtonType.Primary));
    }

    [Test]
    public void TestHasSupplementalButtonType ()
    {
      var home = Start();

      var dropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenuSupplemental");
      Assert.That(dropDownMenu.GetButtonType(), Is.EqualTo(ButtonType.Supplemental));
    }

    [Category("Screenshot")]
    [Test]
    public void ScrollIntoView_WithLongDropDownMenu_ScrollInvisibleItemIntoView ()
    {
      ScreenshotTestingDelegate<FluentScreenshotElement<DropDownMenuControlObject>> test = (builder, target) =>
      {
        builder.AnnotateBox(target.GetMenu(), Pens.Magenta, WebPadding.Inner);

        builder.AnnotateBox(target.SelectItem().WithIndex(95).ScrollIntoView(), Pens.Chartreuse, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithIndex(92).ScrollIntoView(), Pens.Red, WebPadding.Inner);

        builder.Crop(target.GetMenu(), new WebPadding(1));
      };

      var home = Start();
      try
      {
        ResizeBrowserWindowTo(new Size(600, 400));
        var fluentDropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenu_ManyMenuItems").ForControlObjectScreenshot();
        fluentDropDownMenu.OpenMenu();

        Helper.RunScreenshotTestExact<FluentScreenshotElement<DropDownMenuControlObject>, DropDownMenuControlObjectTest>(
            fluentDropDownMenu,
            ScreenshotTestingType.Both,
            test);
      }
      finally
      {
        home.Driver.MaximiseWindow(home.Scope);
      }
    }

    [Category("Screenshot")]
    [Test]
    public void ScrollIntoView_WithLongDropDownMenu_ScrollsBackUp ()
    {
      ScreenshotTestingDelegate<FluentScreenshotElement<DropDownMenuControlObject>> test = (builder, target) =>
      {
        builder.AnnotateBox(target.GetMenu(), Pens.Magenta, WebPadding.Inner);

        target.SelectItem().WithIndex(90).ScrollIntoView();
        builder.AnnotateBox(target.SelectItem().WithIndex(2).ScrollIntoView(), Pens.Chartreuse, WebPadding.Inner);

        builder.Crop(target.GetMenu(), new WebPadding(1));
      };

      var home = Start();
      try
      {
        ResizeBrowserWindowTo(new Size(600, 400));
        var fluentDropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenu_ManyMenuItems").ForControlObjectScreenshot();
        fluentDropDownMenu.OpenMenu();

        Helper.RunScreenshotTestExact<FluentScreenshotElement<DropDownMenuControlObject>, DropDownMenuControlObjectTest>(
            fluentDropDownMenu,
            ScreenshotTestingType.Both,
            test);
      }
      finally
      {
        home.Driver.MaximiseWindow(home.Scope);
      }
    }

    private void ResizeBrowserWindowTo (Size size)
    {
      Helper.BrowserConfiguration.BrowserHelper.ResizeBrowserWindowTo(Helper.MainBrowserSession.Window, size);
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject>("DropDownMenuTest.wxe");
    }
  }
}
