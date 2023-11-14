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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.SingleViewImplementation;
using Remotion.Web.UI.Controls.SingleViewImplementation.Rendering;

namespace Remotion.Web.UnitTests.Core.UI.Controls.SingleViewImplementation.Rendering
{
  [TestFixture]
  public class SingleViewRendererTest : RendererTestBase
  {
    private Mock<ISingleView> _control;
    private Mock<HttpContextBase> _httpContext;
    private HtmlHelper _htmlHelper;
    private SingleViewRenderer _renderer;

    [SetUp]
    public void SetUp ()
    {
      _htmlHelper = new HtmlHelper();
      _httpContext = new Mock<HttpContextBase>();

      _control = new Mock<ISingleView>();
      _control.Setup(stub => stub.ClientID).Returns("MySingleView");
      _control.Setup(stub => stub.ControlType).Returns("SingleView");
      _control.Setup(stub => stub.TopControl).Returns(new PlaceHolder { ID = "TopControl" });
      _control.Setup(stub => stub.BottomControl).Returns(new PlaceHolder { ID = "BottomControl" });
      _control.Setup(stub => stub.View).Returns(new PlaceHolder { ID = "ViewControl" });
      _control.Setup(stub => stub.ViewClientID).Returns("ViewClientID");
      _control.Setup(stub => stub.ViewContentClientID).Returns(_control.Object.ViewClientID + "_Content");
      _control.Setup(stub => stub.WrapperClientID).Returns("WrapperClientID");

      StateBag stateBag = new StateBag();
      _control.Setup(stub => stub.Attributes).Returns(new AttributeCollection(stateBag));
      _control.Setup(stub => stub.TopControlsStyle).Returns(new Style(stateBag));
      _control.Setup(stub => stub.BottomControlsStyle).Returns(new Style(stateBag));
      _control.Setup(stub => stub.ViewStyle).Returns(new Style(stateBag));
      _control.Setup(stub => stub.ControlStyle).Returns(new Style(stateBag));

      var clientScriptStub = new Mock<IClientScriptManager>();

      var pageStub = new Mock<IPage>();
      pageStub.Setup(stub => stub.ClientScript).Returns(clientScriptStub.Object);

      _control.Setup(stub => stub.Page).Returns(pageStub.Object);

      _renderer = new SingleViewRenderer(new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.Default);
    }

    [Test]
    public void RenderView ()
    {
      PopulateControl();
      AssertRendering(false, false, false, false);
    }

    [Test]
    public void RenderViewWithCssClass ()
    {
      PopulateControl();
      AssertRendering(false, true, false, false);
    }

    [Test]
    public void RenderViewWithCssClassInAttributes ()
    {
      PopulateControl();
      AssertRendering(false, true, true, false);
    }

    [Test]
    public void RenderEmptyView ()
    {
      AssertRendering(true, false, false, false);
    }

    [Test]
    public void RenderEmptyViewWithCssClass ()
    {
      AssertRendering(true, true, false, false);
    }

    [Test]
    public void RenderEmptyViewWithCssClassInAttributes ()
    {
      AssertRendering(true, true, true, false);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      _renderer = new SingleViewRenderer(new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);

      PopulateControl();
      var div = AssertRendering(false, false, false, false);

      div.AssertAttributeValueEquals(DiagnosticMetadataAttributes.ControlType, "SingleView");
    }

    private void PopulateControl ()
    {
      _control.Object.TopControl.Controls.Add(new LiteralControl("TopControl"));
      _control.Object.BottomControl.Controls.Add(new LiteralControl("BottomControl"));
      _control.Object.View.Controls.Add(new LiteralControl("View"));
    }

    private XmlNode AssertRendering (bool isEmpty, bool withCssClasses, bool inAttributes, bool isDesignMode)
    {
      _renderer.Render(new SingleViewRenderingContext(_httpContext.Object, _htmlHelper.Writer, _control.Object));

      var document = _htmlHelper.GetResultDocument();
      document.AssertChildElementCount(1);

      var outerDiv = document.GetAssertedChildElement("div", 0);
      outerDiv.AssertAttributeValueEquals(
          "class", withCssClasses ? (inAttributes ? _control.Object.Attributes["class"] : _control.Object.CssClass) : _renderer.CssClassBase);

      if (isDesignMode)
      {
        _htmlHelper.AssertStyleAttribute(outerDiv, "width", "100%");
        _htmlHelper.AssertStyleAttribute(outerDiv, "height", "75%");
      }

      var contentDiv = outerDiv.GetAssertedChildElement("div", 0);
      contentDiv.AssertAttributeValueEquals("class", _renderer.CssClassWrapper);

      var topControls = contentDiv.GetAssertedChildElement("div", 0);
      topControls.AssertAttributeValueEquals("id", _control.Object.TopControl.ClientID);
      topControls.AssertAttributeValueContains("class", _renderer.CssClassTopControls);
      if (isEmpty)
        topControls.AssertAttributeValueContains("class", _renderer.CssClassEmpty);
      var topContent = topControls.GetAssertedChildElement("div", 0);
      topContent.AssertAttributeValueContains("class", _renderer.CssClassContent);

      var bottomControls = contentDiv.GetAssertedChildElement("div", 2);
      bottomControls.AssertAttributeValueEquals("id", _control.Object.BottomControl.ClientID);
      bottomControls.AssertAttributeValueContains("class", _renderer.CssClassBottomControls);
      if (isEmpty)
        bottomControls.AssertAttributeValueContains("class", _renderer.CssClassEmpty);
      var bottomContent = bottomControls.GetAssertedChildElement("div", 0);
      bottomContent.AssertAttributeValueEquals("class", _renderer.CssClassContent);

      var viewContainer = contentDiv.GetAssertedChildElement("div", 1);
      viewContainer.AssertAttributeValueEquals("id", _control.Object.ViewClientID);
      viewContainer.AssertAttributeValueEquals("class", _renderer.CssClassView);

      var viewContentBorder = viewContainer.GetAssertedChildElement("div", 0);
      viewContentBorder.AssertAttributeValueEquals("class", _renderer.CssClassContentBorder);

      var viewContent = viewContentBorder.GetAssertedChildElement("div", 0);
      viewContent.AssertAttributeValueEquals("class", _renderer.CssClassContent);

      if (!isEmpty)
      {
        topContent.AssertTextNode("TopControl", 0);
        bottomContent.AssertTextNode("BottomControl", 0);
        viewContent.AssertTextNode("View", 0);
      }

      return outerDiv;
    }
  }
}
