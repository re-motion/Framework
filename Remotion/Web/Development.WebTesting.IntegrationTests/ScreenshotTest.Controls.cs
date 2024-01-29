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
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.WebDriver;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects.FluentScreenshots.Extensions;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public partial class ScreenshotTest : IntegrationTest
  {
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
  }
}
