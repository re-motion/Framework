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
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.TabbedMenuImplementation;
using Remotion.Web.UI.Controls.TabbedMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation;

namespace Remotion.Web.UnitTests.Core.UI.Controls.TabbedMenuImplementation.Rendering
{
  [TestFixture]
  public class TabbedMenuRendererTest : RendererTestBase
  {
    private Mock<ITabbedMenu> _control;
    private Mock<HttpContextBase> _httpContext;
    private HtmlHelper _htmlHelper;
    private TabbedMenuRenderer _renderer;

    [SetUp]
    public void SetUp ()
    {
      _htmlHelper = new HtmlHelper();
      _httpContext = new Mock<HttpContextBase>();

      _control = new Mock<ITabbedMenu>();
      _control.Setup(stub => stub.ClientID).Returns("MyTabbedMenu");
      _control.Setup(stub => stub.ControlType).Returns("TabbedMenu");
      _control.Setup(stub => stub.MainMenuTabStrip).Returns(new Mock<IWebTabStrip>().Object);
      _control.Setup(stub => stub.SubMenuTabStrip).Returns(new Mock<IWebTabStrip>().Object);

      StateBag stateBag = new StateBag();
      _control.Setup(stub => stub.Attributes).Returns(new AttributeCollection(stateBag));
      _control.Setup(stub => stub.ControlStyle).Returns(new Style(stateBag));
      _control.Setup(stub => stub.StatusStyle).Returns(new Style(stateBag));

      Mock.Get(_control.Object.SubMenuTabStrip).Setup(stub => stub.ControlStyle).Returns(new Style(stateBag));
      Mock.Get(_control.Object.SubMenuTabStrip).Setup(stub => stub.Style).Returns(_control.Object.SubMenuTabStrip.ControlStyle.GetStyleAttributes(_control.Object));

      var pageStub = new Mock<IPage>();
      _control.Setup(stub => stub.Page).Returns(pageStub.Object);

      _renderer = new TabbedMenuRenderer(new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.Default);
    }

    [Test]
    public void RenderEmptyMenu ()
    {
      AssertControl(false, false, false);
    }

    [Test]
    public void RenderEmptyMenuWithStatusText ()
    {
      _control.Setup(stub => stub.StatusText).Returns(WebString.CreateFromText("Status"));
      AssertControl(false, true, false);
    }

    [Test]
    public void RenderEmptyMenuWithCssClass ()
    {
      _control.SetupProperty(_ => _.CssClass);
      _control.Object.CssClass = "CustomCssClass";
      AssertControl(false, false, true);
    }

    [Test]
    public void RenderEmptyMenuWithBackgroundColor ()
    {
      _control.Setup(stub => stub.SubMenuBackgroundColor).Returns(Color.Yellow);
      AssertControl(false, false, false);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      _renderer = new TabbedMenuRenderer(new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);

      var table = AssertControl(false, false, false);

      table.AssertAttributeValueEquals(DiagnosticMetadataAttributes.ControlType, "TabbedMenu");
    }

    [Test]
    public void RenderWebStrings ()
    {
      _control.Setup(_ => _.StatusText).Returns(WebString.CreateFromText("Multiline\nStatusText"));

      _renderer.Render(new TabbedMenuRenderingContext(_httpContext.Object, _htmlHelper.Writer, _control.Object));

      var document = _htmlHelper.GetResultDocument();
      var node = document.GetAssertedElementByClass("tabbedMenuStatusCell");
      Assert.That(node.InnerXml, Is.EqualTo("Multiline<br />StatusText"));
    }

    private XmlNode AssertControl (bool isDesignMode, bool hasStatusText, bool hasCssClass)
    {
      _renderer.Render(new TabbedMenuRenderingContext(_httpContext.Object, _htmlHelper.Writer, _control.Object));
      // _control.RenderControl (_htmlHelper.Writer);

      var document = _htmlHelper.GetResultDocument();
      var table = document.GetAssertedChildElement("table", 0);
      table.AssertAttributeValueEquals("class", hasCssClass ? "CustomCssClass" : "tabbedMenu");
      if (isDesignMode)
        table.AssertStyleAttribute("width", "100%");
      table.AssertChildElementCount(2);

      var trMainMenu = table.GetAssertedChildElement("tr", 0);
      trMainMenu.AssertChildElementCount(1);

      var tdMainMenu = trMainMenu.GetAssertedChildElement("td", 0);
      tdMainMenu.AssertAttributeValueEquals("colspan", "2");
      tdMainMenu.AssertAttributeValueEquals("class", "tabbedMainMenuCell");
      tdMainMenu.AssertChildElementCount(0);

      var trSubMenu = table.GetAssertedChildElement("tr", 1);
      trSubMenu.AssertChildElementCount(2);

      var tdSubMenu = trSubMenu.GetAssertedChildElement("td", 0);
      tdSubMenu.AssertAttributeValueEquals("class", "tabbedSubMenuCell");
      if (!_control.Object.SubMenuBackgroundColor.IsEmpty)
        tdSubMenu.AssertStyleAttribute("background-color", ColorTranslator.ToHtml(Color.Yellow));
      tdSubMenu.AssertChildElementCount(0);

      var tdMenuStatus = trSubMenu.GetAssertedChildElement("td", 1);
      tdMenuStatus.AssertAttributeValueEquals("class", "tabbedMenuStatusCell");
      tdMenuStatus.AssertChildElementCount(0);
      tdMenuStatus.AssertTextNode(hasStatusText ? "Status" : "&nbsp;", 0);

      return table;
    }
  }
}
