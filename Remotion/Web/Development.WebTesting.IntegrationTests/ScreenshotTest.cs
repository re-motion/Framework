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
using System.Threading;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.PageObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Resolvers;
using Remotion.Web.Development.WebTesting.WebDriver;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects.FluentScreenshots.Extensions;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  [RequiresUserInterface]
  public class ScreenshotTest : IntegrationTest
  {
    private const string c_nonBreakingSpace = " ";

    private static readonly WebPadding s_rainbowPaddingBig = new WebPadding(10, 40, 30, 20);
    private static readonly WebPadding s_rainbowPaddingSmall = new WebPadding(1, 4, 3, 2);
    private static readonly WebPadding s_uniformPaddingSmall = new WebPadding(5);
    private static readonly WebPadding s_uniformPaddingMedium = new WebPadding(10);
    private static readonly WebPadding s_uniformPaddingBig = new WebPadding(100);

    private static readonly IScreenshotElementResolver<ControlObject> s_controlObjectResolver = ControlObjectResolver.Instance;
    private static readonly IScreenshotElementResolver<ElementScope> s_elementScopeResolver = ElementScopeResolver.Instance;

    private static readonly Color s_colorA = Color.FromArgb(0x00, 0x00, 0xFF);
    private static readonly Color s_colorB = Color.FromArgb(0x00, 0xFF, 0x33);
    private static readonly Color s_colorC = Color.FromArgb(0x66, 0x33, 0x00);
    private static readonly Color s_colorD = Color.FromArgb(0x99, 0x00, 0x99);
    private static readonly Color s_colorE = Color.FromArgb(0x99, 0xFF, 0x33);

    private static readonly Font s_font = new Font("Consolas", 8.25f);
    private static readonly Brush s_foregroundBrush = new SolidBrush(Color.FromArgb(0x00, 0x00, 0x00));
    private static readonly Brush s_backgroundBrush = new SolidBrush(Color.FromArgb(0xCC, 0xCC, 0xFF));

    private static readonly StringFormat s_stringFormat = new StringFormat
                                                          {
                                                              Alignment = StringAlignment.Center,
                                                              LineAlignment = StringAlignment.Center,
                                                              Trimming = StringTrimming.Word
                                                          };

    [Test]
    public void GetTarget ()
    {
      var home = Start();
      var controlObject = home.DropDownLists().GetByLocalID("MyDropDownList");
      FluentScreenshotElement<DropDownListControlObject> fluentControlObject = controlObject.ForControlObjectScreenshot();

      Assert.That(fluentControlObject.GetTarget(), Is.SameAs(controlObject));
      Assert.That(((IFluentScreenshotElement<DropDownListControlObject>)fluentControlObject).GetTarget(), Is.SameAs(controlObject));
      Assert.That(((IFluentScreenshotElementWithCovariance<DropDownListControlObject>)fluentControlObject).GetTarget(), Is.SameAs(controlObject));
    }

    /// <summary>
    /// Tests cropping without a border.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ElementCroppingTest ()
    {
      ScreenshotTestingDelegate<ControlObject> test =
          (builder, target) => builder.Crop(target, s_controlObjectResolver, new ScreenshotCropping(WebPadding.None));

      Helper.RunScreenshotTestExact<ScreenshotTest>(PrepareTest(), ScreenshotTestingType.Both, test);
    }

    /// <summary>
    /// Tests cropping with a different padding on each side.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ElementCroppingWithPaddingTest ()
    {
      ScreenshotTestingDelegate<ControlObject> test =
          (builder, target) => builder.Crop(target, s_controlObjectResolver, new ScreenshotCropping(s_rainbowPaddingBig));

      Helper.RunScreenshotTestExact<ScreenshotTest>(PrepareTest(), ScreenshotTestingType.Both, test);
    }

    /// <summary>
    /// Tests that a the box annotation is drawing a box around the target.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ElementBoxAnnotationTest ()
    {
      ScreenshotTestingDelegate<ControlObject> test = (builder, target) =>
      {
        builder.Annotate(target, s_controlObjectResolver, new ScreenshotBoxAnnotation(new Pen(s_colorA), s_rainbowPaddingSmall, null));
        builder.Crop(target, s_controlObjectResolver, new ScreenshotCropping(s_uniformPaddingSmall));
      };

      ScreenshotTestingDelegate<ControlObject> testInverse = (builder, target) =>
      {
        builder.Crop(target, s_controlObjectResolver, new ScreenshotCropping(s_uniformPaddingSmall));
        builder.Annotate(target, s_controlObjectResolver, new ScreenshotBoxAnnotation(new Pen(s_colorA), s_rainbowPaddingSmall, null));
      };

      Helper.RunScreenshotTestExact<ScreenshotTest>(PrepareTest(), ScreenshotTestingType.Both, test);
      Helper.RunScreenshotTestExact<ScreenshotTest>(PrepareTest(), ScreenshotTestingType.Both, testInverse);
    }

    /// <summary>
    /// Tests that box annotations with different widths do not intersect with the element bounds.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ElementBoxAnnotationWithWidthTest ()
    {
      ScreenshotTestingDelegate<ControlObject> test = (builder, target) =>
      {
        builder.Annotate(target, s_controlObjectResolver, new ScreenshotBoxAnnotation(new Pen(s_colorA, 5f), s_rainbowPaddingSmall, null));
        builder.Annotate(target, s_controlObjectResolver, new ScreenshotBoxAnnotation(new Pen(s_colorB, 4f), s_rainbowPaddingSmall, null));
        builder.Annotate(
            target,
            s_controlObjectResolver,
            new ScreenshotBoxAnnotation(new Pen(s_colorC, 3f), s_rainbowPaddingSmall, null));
        builder.Annotate(
            target,
            s_controlObjectResolver,
            new ScreenshotBoxAnnotation(new Pen(s_colorD, 2f), s_rainbowPaddingSmall, null));
        builder.Annotate(
            target,
            s_controlObjectResolver,
            new ScreenshotBoxAnnotation(new Pen(s_colorE, 1f), s_rainbowPaddingSmall, null));
        builder.Crop(target, s_controlObjectResolver, new ScreenshotCropping(s_uniformPaddingMedium));
      };

      Helper.RunScreenshotTestExact<ScreenshotTest>(PrepareTest(), ScreenshotTestingType.Both, test);
    }

    /// <summary>
    /// Tests that the <see cref="ScreenshotBadgeAnnotation"/> is drawn correctly.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void BadgeTest ()
    {
      ScreenshotTestingDelegate<IFluentScreenshotElement<ElementScope>> test = (builder, target) =>
      {
        builder.AnnotateBadge(target, "123", borderPen: Pens.Black);
        builder.Crop(target, new WebPadding(25, 25, 25, 25));
      };

      var home = Start();
      var element = home.Scope.FindId("tooltipTarget");
      Helper.RunScreenshotTestExact<IFluentScreenshotElement<ElementScope>, ScreenshotTest>(element.ForElementScopeScreenshot(), ScreenshotTestingType.Both, test);
    }

    /// <summary>
    /// Tests that the <see cref="ScreenshotTooltipAnnotation"/> is drawn correctly.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void DrawTooltip ()
    {
      ScreenshotTestingDelegate<ElementScope> test = (builder, target) =>
      {
        Helper.BrowserConfiguration.BrowserAnnotateHelper.DrawTooltip(builder, target, GetTooltipStyleForCurrentBrowser());

        builder.Crop(target.ForElementScopeScreenshot(), new WebPadding(30, 60, 30, 60));
      };

      var home = Start();
      var element = home.Scope.FindId("tooltipTarget");
      Helper.RunScreenshotTestExact<ElementScope, ScreenshotTest>(element, ScreenshotTestingType.Both, test);
    }

    /// <summary>
    /// Tests that the <see cref="ScreenshotTooltipAnnotation"/> is annotated correctly.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void DrawCursorTooltip ()
    {
      ScreenshotTestingDelegate<ElementScope> test = (builder, target) =>
      {
        Helper.BrowserConfiguration.BrowserAnnotateHelper.DrawCursorTooltip(builder, Helper.MainBrowserSession, "title", GetTooltipStyleForCurrentBrowser());

        builder.Crop(target.ForElementScopeScreenshot(), new WebPadding(30, 60, 30, 60));
      };

      var home = Start();
      var mouseHelper = Helper.BrowserConfiguration.MouseHelper;
      var element = home.Scope.FindId("tooltipTarget");

      // ElementScope.Hover() cannot be used due to Chromedriver not moving the cursor to trigger the hover event.
      mouseHelper.Hover(element);

      // When drawing the Cursor tooltip, only ScreenshotTestingType.Desktop is supported.
      Helper.RunScreenshotTestExact<ElementScope, ScreenshotTest>(element, ScreenshotTestingType.Desktop, test);

      // Reset the cursor to the initial position (0, 0).
      mouseHelper.Hover(Point.Empty);
    }

    /// <summary>
    /// Tests that the <see cref="ScreenshotTooltipAnnotation"/> is annotated correctly.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void DrawTooltip_WithFollowingBoxAnnotation ()
    {
      ScreenshotTestingDelegate<ElementScope> test = (builder, target) =>
      {
        var annotationTarget = Helper.BrowserConfiguration.BrowserAnnotateHelper.DrawTooltip(builder, target, GetTooltipStyleForCurrentBrowser());
        builder.AnnotateBox(annotationTarget, Pens.Red);

        builder.Crop(target.ForElementScopeScreenshot(), new WebPadding(30, 60, 30, 60));
      };

      var home = Start();
      var element = home.Scope.FindId("tooltipTarget");
      Helper.RunScreenshotTestExact<ElementScope, ScreenshotTest>(element, ScreenshotTestingType.Both, test);
    }

    /// <summary>
    /// Tests that the <see cref="ScreenshotTooltipAnnotation"/> is annotated correctly.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void DrawCursorTooltip_WithFollowingBoxAnnotation ()
    {
      ScreenshotTestingDelegate<ElementScope> test = (builder, target) =>
      {
        var annotationTarget = Helper.BrowserConfiguration.BrowserAnnotateHelper.DrawCursorTooltip(
            builder,
            Helper.MainBrowserSession,
            "title",
            GetTooltipStyleForCurrentBrowser());
        builder.AnnotateBox(annotationTarget, Pens.Red);

        builder.Crop(target.ForElementScopeScreenshot(), new WebPadding(30, 60, 30, 60));
      };

      var home = Start();
      var mouseHelper = Helper.BrowserConfiguration.MouseHelper;
      var element = home.Scope.FindId("tooltipTarget");

      // ElementScope.Hover() cannot be used due to Chromedriver not moving the mouse to trigger the hover event.
      mouseHelper.Hover(element);

      Helper.RunScreenshotTestExact<ElementScope, ScreenshotTest>(element, ScreenshotTestingType.Both, test);

      // Reset the cursor to the initial position (0, 0).
      mouseHelper.Hover(Point.Empty);
    }

    [Test]
    public void TooltipBounds_NotDrawnYet_ThrowsInvalidOperationException ()
    {
      var tooltipAnnotation = new ScreenshotTooltipAnnotation("title", ScreenshotTooltipStyle.Chrome, WebPadding.None);

      Assert.That(
          () => tooltipAnnotation.TooltipBounds,
          Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("The tooltip needs to be drawn before its bounds can be accessed."));
    }

    /// <summary>
    /// Tests that the text annotation is drawing text correctly.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ElementTextAnnotations ()
    {
      ScreenshotTestingDelegate<ControlObject> test = (builder, target) =>
      {
        foreach (var value in Enum.GetValues(typeof(ContentAlignment)))
        {
          var name = Enum.GetName(typeof(ContentAlignment), value);
          builder.Annotate(
              target,
              s_controlObjectResolver,
              new ScreenshotTextAnnotation(
                  name,
                  s_font,
                  s_foregroundBrush,
                  s_backgroundBrush,
                  s_stringFormat,
                  (ContentAlignment)value,
                  s_rainbowPaddingSmall,
                  -1f,
                  null));
        }
        builder.Crop(target, s_controlObjectResolver, new ScreenshotCropping(s_uniformPaddingBig));
      };

      Helper.RunScreenshotTestExact<ScreenshotTest>(PrepareTest(), ScreenshotTestingType.Both, test);
    }

    /// <summary>
    /// Tests that the <see cref="WebElementResolver"/> (indirect also <see cref="ElementScopeResolver"/> and
    /// <see cref="ControlObjectResolver"/>) is capable of resolving an elements position if the element is
    /// contained in one or more frames.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ResolveElementInFrame ()
    {
      var home = Start();

      ScreenshotTestingDelegate<ElementScope> test =
          (builder, target) => { builder.Crop(target, s_elementScopeResolver, new ScreenshotCropping(new WebPadding(1))); };

      // Give the browser some time to load the iframe
      Thread.Sleep(1000);

      var element = home.Scope.FindFrame("frame").FindId("target");

      Helper.RunScreenshotTestExact<ElementScope, ScreenshotTest>(element, ScreenshotTestingType.Both, test);
    }

    [Category("Screenshot")]
    [Test]
    public void ResolveAbsolutePositionedElement ()
    {
      var home = Start();

      // Chromium browsers show the remote control overlay since RM-7184, which is drawn shortly after the page load. The
      // content however needs a small timespan to be adapted to the slightly smaller viewport.
      if (Helper.BrowserConfiguration.IsChromium())
        Thread.Sleep(100);

      ScreenshotTestingDelegate<IFluentScreenshotElement<ElementScope>> test =
          (builder, target) =>
          {
            builder.AnnotateBox(target, Pens.Red, new WebPadding(1));
            builder.Crop(target, new WebPadding(3));
          };

      var element = home.Scope.FindId("absoluteElement").ForElementScopeScreenshot();

      Helper.RunScreenshotTestExact<IFluentScreenshotElement<ElementScope>, ScreenshotTest>(element, ScreenshotTestingType.Both, test);
    }

    [Category("Screenshot")]
    [Test]
    public void ResolveBorderElementA ()
    {
      // TODO: Remove when RM-7187 is resolved, use the RetryAttribute provided by NUnit instead.
      RetryTest(
          () =>
          {
            var home = Start();

            // Chromium browsers show the remote control overlay since RM-7184, which is drawn shortly after the page load. The
            // content however needs a small timespan to be adapted to the slightly smaller viewport.
            if (Helper.BrowserConfiguration.IsChromium())
              Thread.Sleep(100);

            ScreenshotTestingDelegate<IFluentScreenshotElement<ElementScope>> test =
                (builder, target) => { builder.Crop(target, new WebPadding(1)); };

            var element = home.Scope.FindId("borderElementA").ForElementScopeScreenshot();

            Helper.RunScreenshotTestExact<IFluentScreenshotElement<ElementScope>, ScreenshotTest>(element, ScreenshotTestingType.Both, test);
          },
          2);
    }

    [Category("Screenshot")]
    [Test]
    public void ResolveBorderElementB ()
    {
      // TODO: Remove when RM-7187 is resolved, use the RetryAttribute provided by NUnit instead.
      RetryTest(
          () =>
          {
            var home = Start();

            // Chromium browsers show the remote control overlay since RM-7184, which is drawn shortly after the page load. The
            // content however needs a small timespan to be adapted to the slightly smaller viewport.
            if (Helper.BrowserConfiguration.IsChromium())
              Thread.Sleep(100);

            ScreenshotTestingDelegate<IFluentScreenshotElement<ElementScope>> test =
                (builder, target) => { builder.Crop(target, new WebPadding(1)); };

            var element = home.Scope.FindId("borderElementB").ForElementScopeScreenshot();

            Helper.RunScreenshotTestExact<IFluentScreenshotElement<ElementScope>, ScreenshotTest>(element, ScreenshotTestingType.Both, test);
          },
          2);
    }

    [Category("Screenshot")]
    [Test]
    public void DropDownList ()
    {
      WebPadding padding;
      if (Helper.BrowserConfiguration.IsChromium())
      {
        padding = new WebPadding(2, 3, 2, 53);
      }
      else if (Helper.BrowserConfiguration.IsFirefox())
      {
        padding = new WebPadding(6, 1, 2, 68);
      }
      else
      {
        Assert.Fail("The current browser is not supported by this test.");
        // ReSharper disable once HeuristicUnreachableCode
        return;
      }

      ScreenshotTestingDelegate<FluentScreenshotElement<DropDownListControlObject>> test = (builder, target) =>
      {
        builder.AnnotateBox(target, Pens.Red, WebPadding.Inner);

        builder.Crop(target, padding);
      };

      var home = Start();
      var fluentDropDownList = home.DropDownLists().GetByLocalID("MyDropDownList").ForControlObjectScreenshot();
      fluentDropDownList.Open();
      Thread.Sleep(250);

      Helper.RunScreenshotTest<FluentScreenshotElement<DropDownListControlObject>, DropDownListControlObjectTest>(
          fluentDropDownList,
          ScreenshotTestingType.Desktop,
          test);
    }

    [Test]
    public void DropDownList_WithDerivedControlObject ()
    {
      var home = Start();
      var controlObject = new DerivedDropDownListControlObject(home.DropDownMenus().GetByLocalID("MyDropDownList").Context);
      var fluentControlObject = controlObject.ForControlObjectScreenshot();

      Assert.That(() => fluentControlObject.Open(), Throws.Nothing);
    }

    [Category("Screenshot")]
    [Test]
    public void DropDownMenu ()
    {
      ScreenshotTestingDelegate<FluentScreenshotElement<DropDownMenuControlObject>> test = (builder, target) =>
      {
        builder.AnnotateBox(target.GetMenu(), Pens.Magenta, WebPadding.Inner);

        builder.AnnotateBox(target.SelectItem().WithDisplayText("  "), Pens.Red, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithDisplayTextContains(c_nonBreakingSpace), Pens.Green, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithHtmlID("body_MyDropDownMenu_2"), Pens.Blue, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithItemID("ItemID4"), Pens.Orange, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithIndex(5), Pens.Yellow, WebPadding.Inner);

        builder.Crop(target.GetMenu(), new WebPadding(1));
      };

      var home = Start();
      var fluentDropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenu").ForControlObjectScreenshot();
      fluentDropDownMenu.OpenMenu();
      Thread.Sleep(250);

      Helper.RunScreenshotTestExact<FluentScreenshotElement<DropDownMenuControlObject>, DropDownMenuControlObjectTest>(
          fluentDropDownMenu,
          ScreenshotTestingType.Both,
          test);
    }

    [Category("Screenshot")]
    [Test]
    public void DropDownMenu_WithHiddenTitle ()
    {
      ScreenshotTestingDelegate<FluentScreenshotElement<DropDownMenuControlObject>> test = (builder, target) =>
      {
        builder.AnnotateBox(target, Pens.Magenta, WebPadding.Inner);

        builder.Crop(target, new WebPadding(1));
      };

      var home = Start();
      var fluentDropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenuWithHiddenTitle").ForControlObjectScreenshot();

      Helper.RunScreenshotTestExact<FluentScreenshotElement<DropDownMenuControlObject>, DropDownMenuControlObjectTest>(
          fluentDropDownMenu,
          ScreenshotTestingType.Both,
          test);
    }

    [Test]
    public void DropDownMenu_WithDerivedControlObject ()
    {
      var home = Start();
      var controlObject = new DerivedDropDownMenuControlObject(home.DropDownMenus().GetByLocalID("MyDropDownMenu").Context);
      var fluentControlObject = controlObject.ForControlObjectScreenshot();

      Assert.That(() => fluentControlObject.OpenMenu(), Throws.Nothing);
      Assert.That(fluentControlObject.GetMenu(), Is.Not.Null);
      Assert.That(fluentControlObject.SelectItem(), Is.Not.Null);
    }

    [Category("Screenshot")]
    [Test]
    public void ListMenu ()
    {
      ScreenshotTestingDelegate<FluentScreenshotElement<ListMenuControlObject>> test = (builder, target) =>
      {
        builder.AnnotateBox(target, Pens.Magenta, WebPadding.Inner);

        builder.AnnotateBox(target.SelectItem().WithDisplayText("  "), Pens.Red, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithDisplayTextContains(c_nonBreakingSpace), Pens.Green, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithHtmlID("body_MyListMenu_2"), Pens.Blue, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithItemID("ItemID4"), Pens.Orange, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithIndex(5), Pens.Yellow, WebPadding.Inner);

        builder.Crop(target, new WebPadding(1));
      };

      var home = Start();
      var fluentListMenu = home.ListMenus().GetByLocalID("MyListMenu").ForControlObjectScreenshot();

      Helper.RunScreenshotTestExact<FluentScreenshotElement<ListMenuControlObject>, ListMenuControlObjectTest>(
          fluentListMenu,
          ScreenshotTestingType.Both,
          test);
    }

    [Test]
    public void ListMenu_WithDerivedControlObject ()
    {
      var home = Start();
      var controlObject = new DerivedListMenuControlObject(home.ListMenus().GetByLocalID("MyListMenu").Context);
      var fluentControlObject = controlObject.ForControlObjectScreenshot();

      Assert.That(fluentControlObject.SelectItem(), Is.Not.Null);
    }

    [Category("Screenshot")]
    [Test]
    public void TabbedMenu ()
    {
      ScreenshotTestingDelegate<FluentScreenshotElement<TabbedMenuControlObject>> test = (builder, target) =>
      {
        builder.AnnotateBox(target, Pens.Magenta, WebPadding.Inner);

        builder.AnnotateBox(target.SelectItem().WithDisplayText("  "), Pens.Red, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithDisplayTextContains(c_nonBreakingSpace), Pens.Green, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithHtmlID("body_MyTabbedMenu_MainMenuTabStrip_ItemID3"), Pens.Blue, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithItemID("ItemID4"), Pens.Orange, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithIndex(5), Pens.Yellow, WebPadding.Inner);

        var subMenu = target.GetSubMenu();
        builder.AnnotateBox(subMenu.SelectItem().WithDisplayText("  "), Pens.Red, WebPadding.Inner);
        builder.AnnotateBox(subMenu.SelectItem().WithDisplayTextContains(c_nonBreakingSpace), Pens.Green, WebPadding.Inner);
        builder.AnnotateBox(subMenu.SelectItem().WithHtmlID("body_MyTabbedMenu_SubMenuTabStrip_ItemID3"), Pens.Blue, WebPadding.Inner);
        builder.AnnotateBox(subMenu.SelectItem().WithItemID("ItemID4"), Pens.Orange, WebPadding.Inner);
        builder.AnnotateBox(subMenu.SelectItem().WithIndex(5), Pens.Yellow, WebPadding.Inner);

        builder.Crop(target, new WebPadding(1));
      };

      var home = Start();
      var menu = home.TabbedMenus().GetByLocalID("MyTabbedMenu");
      var fluentTabbedMenu = menu.ForControlObjectScreenshot();

      Helper.RunScreenshotTestExact<FluentScreenshotElement<TabbedMenuControlObject>, TabbedMenuControlObjectTest>(
          fluentTabbedMenu,
          ScreenshotTestingType.Both,
          test);
    }

    [Test]
    public void TabbedMenu_WithDerivedControlObject ()
    {
      var home = Start();
      var controlObject = new DerivedTabbedMenuControlObject(home.TabbedMenus().GetByLocalID("MyTabbedMenu").Context);
      var fluentControlObject = controlObject.ForControlObjectScreenshot();

      Assert.That(fluentControlObject.SelectItem(), Is.Not.Null);
      var subMenu = fluentControlObject.GetSubMenu();
      Assert.That(subMenu, Is.Not.Null);
      var derivedSubMenu = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotTabbedSubMenu(subMenu.GetTarget().FluentTabbedMenu, subMenu.GetTarget().FluentElement));
      Assert.That(derivedSubMenu.SelectItem(), Is.Not.Null);
    }

    [Category("Screenshot")]
    [Test]
    public void WebTabStrip ()
    {
      ScreenshotTestingDelegate<FluentScreenshotElement<WebTabStripControlObject>> test = (builder, target) =>
      {
        builder.AnnotateBox(target, Pens.Magenta, WebPadding.Inner);

        builder.AnnotateBox(target.SelectItem().WithDisplayText("  "), Pens.Red, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithDisplayTextContains(c_nonBreakingSpace), Pens.Green, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithHtmlID("body_MyTabStrip_ItemID3"), Pens.Blue, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithItemID("ItemID4"), Pens.Orange, WebPadding.Inner);
        builder.AnnotateBox(target.SelectItem().WithIndex(5), Pens.Yellow, WebPadding.Inner);

        builder.Crop(target, new WebPadding(1));
      };

      var home = Start();
      var fluentTabStrip = home.WebTabStrips().GetByLocalID("MyTabStrip").ForControlObjectScreenshot();

      Helper.RunScreenshotTestExact<FluentScreenshotElement<WebTabStripControlObject>, WebTabStripControlObjectTest>(
          fluentTabStrip,
          ScreenshotTestingType.Both,
          test);
    }

    [Category("Screenshot")]
    [Test]
    public void ElementOutOfBounds ()
    {
      var home = Start();

      // Chromium browsers show the remote control overlay since RM-7184, which is drawn shortly after the page load. The
      // content however needs a small timespan to be adapted to the slightly smaller viewport.
      if (Helper.BrowserConfiguration.IsChromium())
        Thread.Sleep(100);

      ScreenshotTestingDelegate<IFluentScreenshotElement<ElementScope>> test =
          (builder, target) =>
          {
            builder.Crop(target, new WebPadding(5, 5, 5, -5));

            builder.AnnotateBox(target.AllowPartialVisibility(), Pens.Red, WebPadding.Inner);
          };

      var element = home.Scope.FindId("screenshotTarget").ForElementScopeScreenshot();

      Helper.RunScreenshotTestExact<IFluentScreenshotElement<ElementScope>, ScreenshotTest>(element, ScreenshotTestingType.Both, test);
    }

    [Category("Screenshot")]
    [Test]
    public void ElementOutOfBounds_WithMinimumVisibilityFullyVisible_ThrowsException ()
    {
      var home = Start();

      // Chromium browsers show the remote control overlay since RM-7184, which is drawn shortly after the page load. The
      // content however needs a small timespan to be adapted to the slightly smaller viewport.
      if (Helper.BrowserConfiguration.IsChromium())
        Thread.Sleep(100);

      using (var builder = Helper.CreateBrowserScreenshot())
      {
        var target = home.Scope.FindId("screenshotTarget").ForElementScopeScreenshot();

        builder.Crop(target, new WebPadding(5, 5, 5, -5));

        Assert.That(
            () => builder.AnnotateBox(target, Pens.Red, WebPadding.Inner),
            Throws.InvalidOperationException
                .With.Message.EqualTo(
                    "The visibility of the resolved element is smaller than the specified minimum visibility."
                    + " (visibility: PartiallyVisible; required: FullyVisible)"));
      }
    }

    [Test]
    public void WebTabStrip_WithDerivedControlObject ()
    {
      var home = Start();
      var controlObject = new DerivedWebTabStripControlObject(home.TabbedMenus().GetByLocalID("MyTabStrip").Context);
      var fluentControlObject = controlObject.ForControlObjectScreenshot();

      Assert.That(fluentControlObject.SelectItem(), Is.Not.Null);
    }

    [Category("Screenshot")]
    [Test]
    public void WebTreeView ()
    {
      ScreenshotTestingDelegate<FluentScreenshotElement<ScreenshotWebTreeViewNodeControlObject>> test = (builder, target) =>
      {
        builder.AnnotateBox(target, Pens.Red, WebPadding.Inner);
        builder.AnnotateBox(target.GetLabel(), Pens.Green, WebPadding.Inner);
        builder.AnnotateBox(target.GetChildren(), Pens.Blue, WebPadding.Inner);

        builder.Crop(target, new WebPadding(1));
      };

      var home = Start();
      var webTreeView = home.WebTreeViews().GetByLocalID("MyWebTreeView");
      var fluentNode = webTreeView.GetNode().WithItemID("ItemA").ForScreenshot();

      Helper.RunScreenshotTestExact<FluentScreenshotElement<ScreenshotWebTreeViewNodeControlObject>, WebTreeViewControlObjectTest>(
          fluentNode,
          ScreenshotTestingType.Both,
          test);
    }

    [Test]
    public void WebTreeView_WithDerivedControlObject ()
    {
      var home = Start();
      var controlObject = new DerivedWebTreeViewControlObject(home.WebTreeViews().GetByLocalID("MyWebTreeView").Context);
      var fluentControlObject = controlObject.ForControlObjectScreenshot();
      Assert.That(fluentControlObject, Is.Not.Null);

      var fluentNode = controlObject.GetNode().WithItemID("ItemA").ForScreenshot();
      var derivedNode = SelfResolvableFluentScreenshot.Create(
          new DerivedScreenshotWebTreeViewNodeControlObject(fluentNode.GetTarget().FluentWebTreeViewNode, fluentNode.GetTarget().FluentElement));

      Assert.That(derivedNode.GetChildren(), Is.Not.Null);
      Assert.That(derivedNode.GetLabel(), Is.Not.Null);
    }

    /// <summary>
    /// Tests using an implementation of <see cref="IScreenshotTransformation{T}"/>, drawing three overlapping ellipses with specified Z indices
    /// and manipulating the height of the resolved element.
    /// </summary>
    [Category("Screenshot")]
    [Test]
    public void ScreenshotTransformationTest ()
    {
      ScreenshotTestingDelegate<ControlObject> test =
          (builder, target) =>
          {
            var transformations = new ScreenshotTransformationCollection<ControlObject>
                                  {
                                      new EllipseScreenshotTransformation<ControlObject>(50, 50, Brushes.Aqua, 3),
                                      new EllipseScreenshotTransformation<ControlObject>(50, 50, Brushes.Red, 2, new Point(30, 0)),
                                      new EllipseScreenshotTransformation<ControlObject>(50, 50, Brushes.Yellow, 4, new Point(60, 0)),
                                      new RemovePixelsFromBottomScreenshotTransformation<ControlObject>(30)
                                  };
            var fluentScreenshotElement = target.ForControlObjectScreenshot();
            ((IFluentScreenshotElement<ControlObject>)fluentScreenshotElement).Transformations = transformations;

            builder.Crop(fluentScreenshotElement);
          };

      Helper.RunScreenshotTestExact<ScreenshotTest>(PrepareTest(), ScreenshotTestingType.Browser, test);
    }

    private void RetryTest (Action action, int retries)
    {
      if (retries < 0)
        throw new ArgumentOutOfRangeException("retries", "Retries must be greater than or equal to zero.");

      for (int i = 0; i <= retries; i++)
      {
        try
        {
          action();
        }
        catch (AssertionException) when (i < retries)
        {
          continue;
        }

        return;
      }
    }

    private ScreenshotTooltipStyle GetTooltipStyleForCurrentBrowser ()
    {
      if (Helper.BrowserConfiguration.IsChrome())
        return ScreenshotTooltipStyle.Chrome;

      if (Helper.BrowserConfiguration.IsEdge())
        return ScreenshotTooltipStyle.Edge;

      if (Helper.BrowserConfiguration.IsFirefox())
        return ScreenshotTooltipStyle.Firefox;

      throw new NotSupportedException("No ScreenshotTooltipStyle has been specified for the current browser.");
    }

    private ControlObject PrepareTest ()
    {
      var home = Start();

      var target = home.Scopes().GetByIDOrNull("screenshotTarget");
      if (target == null)
        throw new InvalidOperationException("Can not find the screenshot test target.");

      return target;
    }

    private HtmlPageObject Start ()
    {
      return Start<HtmlPageObject>("ScreenshotTest.wxe");
    }

    private class DerivedDropDownListControlObject : DropDownListControlObject
    {
      public DerivedDropDownListControlObject (ControlObjectContext context)
          : base(context)
      {
      }
    }

    private class DerivedDropDownMenuControlObject : DropDownMenuControlObject
    {
      public DerivedDropDownMenuControlObject (ControlObjectContext context)
          : base(context)
      {
      }
    }

    private class DerivedListMenuControlObject : ListMenuControlObject
    {
      public DerivedListMenuControlObject (ControlObjectContext context)
          : base(context)
      {
      }
    }

    private class DerivedTabbedMenuControlObject : TabbedMenuControlObject
    {
      public DerivedTabbedMenuControlObject (ControlObjectContext context)
          : base(context)
      {
      }
    }

    private class DerivedScreenshotTabbedSubMenu : ScreenshotTabbedSubMenu
    {
      public DerivedScreenshotTabbedSubMenu (
          IFluentScreenshotElementWithCovariance<TabbedMenuControlObject> fluentTabbedMenu,
          IFluentScreenshotElement<ElementScope> fluentElement)
          : base(fluentTabbedMenu, fluentElement)
      {
      }
    }

    private class DerivedWebTabStripControlObject : WebTabStripControlObject
    {
      public DerivedWebTabStripControlObject (ControlObjectContext context)
          : base(context)
      {
      }
    }

    private class DerivedWebTreeViewControlObject : WebTreeViewControlObject
    {
      public DerivedWebTreeViewControlObject (ControlObjectContext context)
          : base(context)
      {
      }
    }

    private class DerivedScreenshotWebTreeViewNodeControlObject : ScreenshotWebTreeViewNodeControlObject
    {
      public DerivedScreenshotWebTreeViewNodeControlObject (
          IFluentScreenshotElementWithCovariance<WebTreeViewNodeControlObject> fluentWebTreeViewNode,
          IFluentScreenshotElement<ElementScope> fluentElement)
          : base(fluentWebTreeViewNode, fluentElement)
      {
      }
    }

    private class EllipseScreenshotTransformation<T> : IScreenshotTransformation<T>
    {
      public int ZIndex { get; }
      private int Width { get; }
      private int Height { get; }
      private Brush Brush { get; }
      private Point Offset { get; }

      public EllipseScreenshotTransformation (int width, int height, Brush brush, int zIndex = default, Point offset = default)
      {
        Width = width;
        Height = height;
        Brush = brush;
        ZIndex = zIndex;
        Offset = offset;
      }

      public ScreenshotTransformationContext<T> BeginApply (ScreenshotTransformationContext<T> context)
      {
        context.Graphics.FillEllipse(
            Brush,
            context.ResolvedElement.ElementBounds.X + Offset.X,
            context.ResolvedElement.ElementBounds.Y + Offset.Y,
            Width,
            Height);

        return context;
      }

      public void EndApply (ScreenshotTransformationContext<T> context)
      {
      }
    }

    private class RemovePixelsFromBottomScreenshotTransformation<T> : IScreenshotTransformation<T>
    {
      private readonly int _pixelsToRemove;
      public int ZIndex { get; }

      public RemovePixelsFromBottomScreenshotTransformation (int pixelsToRemove)
      {
        _pixelsToRemove = pixelsToRemove;
      }

      public ScreenshotTransformationContext<T> BeginApply (ScreenshotTransformationContext<T> context)
      {
        var resolvedElement = context.ResolvedElement.CloneWith(
            elementBounds: new Rectangle(
                context.ResolvedElement.ElementBounds.X,
                context.ResolvedElement.ElementBounds.Y,
                context.ResolvedElement.ElementBounds.Width,
                context.ResolvedElement.ElementBounds.Height - _pixelsToRemove));

        return context.CloneWith(resolvedElement: resolvedElement);
      }

      public void EndApply (ScreenshotTransformationContext<T> context)
      {
      }
    }
  }
}
