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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.TabbedMenuImplementation;
using Remotion.Web.UI.Controls.TabbedMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls.TabbedMenuImplementation.Rendering
{
  [TestFixture]
  public class TabbedMenuRendererTest : RendererTestBase
  {
    private ITabbedMenu _control;
    private HttpContextBase _httpContext;
    private HtmlHelper _htmlHelper;
    private TabbedMenuRenderer _renderer;

    [SetUp]
    public void SetUp ()
    {
      _htmlHelper = new HtmlHelper ();
      _httpContext = MockRepository.GenerateStub<HttpContextBase> ();

      _control = MockRepository.GenerateStub<ITabbedMenu>();
      _control.Stub (stub => stub.ClientID).Return ("MyTabbedMenu");
      _control.Stub (stub => stub.ControlType).Return ("TabbedMenu");
      _control.Stub (stub => stub.MainMenuTabStrip).Return (MockRepository.GenerateStub<IWebTabStrip>());
      _control.Stub (stub => stub.SubMenuTabStrip).Return (MockRepository.GenerateStub<IWebTabStrip> ());

      StateBag stateBag = new StateBag ();
      _control.Stub (stub => stub.Attributes).Return (new AttributeCollection (stateBag));
      _control.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));
      _control.Stub (stub => stub.StatusStyle).Return (new Style (stateBag));

      _control.SubMenuTabStrip.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));
      _control.SubMenuTabStrip.Stub (stub => stub.Style).Return (_control.SubMenuTabStrip.ControlStyle.GetStyleAttributes(_control));

      IPage pageStub = MockRepository.GenerateStub<IPage>();
      _control.Stub (stub => stub.Page).Return (pageStub);

      _renderer = new TabbedMenuRenderer (new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.Default);
    }

    [Test]
    public void RenderEmptyMenu ()
    {
      AssertControl (false, false, false);
    }

    [Test]
    public void RenderEmptyMenuInDesignMode ()
    {
      _control.Stub (stub => stub.IsDesignMode).Return (true);
      AssertControl (true, false, false);
    }

    [Test]
    public void RenderEmptyMenuWithStatusText ()
    {
      _control.Stub (stub => stub.StatusText).Return ("Status");
      AssertControl (false, true, false);
    }

    [Test]
    public void RenderEmptyMenuWithStatusTextInDesignMode ()
    {
      _control.Stub (stub => stub.IsDesignMode).Return (true);
      _control.Stub (stub => stub.StatusText).Return ("Status");
      AssertControl (true, true, false);
    }

    [Test]
    public void RenderEmptyMenuWithCssClass ()
    {
      _control.CssClass = "CustomCssClass";
      AssertControl (false, false, true);
    }

    [Test]
    public void RenderEmptyMenuWithBackgroundColor ()
    {
      _control.Stub (stub => stub.SubMenuBackgroundColor).Return (Color.Yellow);
      AssertControl (false, false, false);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      _renderer = new TabbedMenuRenderer (new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);

      var table = AssertControl (false, false, false);

      table.AssertAttributeValueEquals (DiagnosticMetadataAttributes.ControlType, "TabbedMenu");
    }

    private XmlNode AssertControl (bool isDesignMode, bool hasStatusText, bool hasCssClass)
    {
      _renderer.Render (new TabbedMenuRenderingContext (_httpContext, _htmlHelper.Writer, _control));
      // _control.RenderControl (_htmlHelper.Writer);

      var document = _htmlHelper.GetResultDocument();
      var table = document.GetAssertedChildElement ("table", 0);
      table.AssertAttributeValueEquals ("class", hasCssClass ? "CustomCssClass" : "tabbedMenu");
      if (isDesignMode)
        table.AssertStyleAttribute ("width", "100%");
      table.AssertChildElementCount (2);

      var trMainMenu = table.GetAssertedChildElement ("tr", 0);
      trMainMenu.AssertChildElementCount (1);

      var tdMainMenu = trMainMenu.GetAssertedChildElement ("td", 0);
      tdMainMenu.AssertAttributeValueEquals ("colspan", "2");
      tdMainMenu.AssertAttributeValueEquals ("class", "tabbedMainMenuCell");
      tdMainMenu.AssertChildElementCount (0);

      var trSubMenu = table.GetAssertedChildElement ("tr", 1);
      trSubMenu.AssertChildElementCount (2);

      var tdSubMenu = trSubMenu.GetAssertedChildElement ("td", 0);
      tdSubMenu.AssertAttributeValueEquals ("class", "tabbedSubMenuCell");
      if (!_control.SubMenuBackgroundColor.IsEmpty)
        tdSubMenu.AssertStyleAttribute ("background-color", ColorTranslator.ToHtml (Color.Yellow));
      tdSubMenu.AssertChildElementCount (0);

      var tdMenuStatus = trSubMenu.GetAssertedChildElement ("td", 1);
      tdMenuStatus.AssertAttributeValueEquals ("class", "tabbedMenuStatusCell");
      tdMenuStatus.AssertChildElementCount (0);
      tdMenuStatus.AssertTextNode (hasStatusText ? "Status" : "&nbsp;", 0);

      return table;
    }
  }
}