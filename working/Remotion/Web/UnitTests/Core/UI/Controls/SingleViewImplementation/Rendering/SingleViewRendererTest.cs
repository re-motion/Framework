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
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.SingleViewImplementation;
using Remotion.Web.UI.Controls.SingleViewImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls.SingleViewImplementation.Rendering
{
  [TestFixture]
  public class SingleViewRendererTest : RendererTestBase
  {
    private ISingleView _control;
    private HttpContextBase _httpContext;
    private HtmlHelper _htmlHelper;
    private SingleViewRenderer _renderer;

    [SetUp]
    public void SetUp ()
    {
      _htmlHelper = new HtmlHelper ();
      _httpContext = MockRepository.GenerateStub<HttpContextBase> ();

      _control = MockRepository.GenerateStub<ISingleView>();
      _control.Stub (stub => stub.ClientID).Return ("MySingleView");
      _control.Stub (stub => stub.ControlType).Return ("SingleView");
      _control.Stub (stub => stub.TopControl).Return (new PlaceHolder { ID = "TopControl" });
      _control.Stub (stub => stub.BottomControl).Return (new PlaceHolder { ID = "BottomControl" });
      _control.Stub (stub => stub.View).Return (new PlaceHolder { ID = "ViewControl" });
      _control.Stub (stub => stub.ViewClientID).Return ("ViewClientID");
      _control.Stub (stub => stub.ViewContentClientID).Return (_control.ViewClientID + "_Content");
      _control.Stub (stub => stub.WrapperClientID).Return ("WrapperClientID");

      StateBag stateBag = new StateBag ();
      _control.Stub (stub => stub.Attributes).Return (new AttributeCollection (stateBag));
      _control.Stub (stub => stub.TopControlsStyle).Return (new Style (stateBag));
      _control.Stub (stub => stub.BottomControlsStyle).Return (new Style (stateBag));
      _control.Stub (stub => stub.ViewStyle).Return (new Style (stateBag));
      _control.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));

      var clientScriptStub = MockRepository.GenerateStub<IClientScriptManager> ();

      var pageStub = MockRepository.GenerateStub<IPage> ();
      pageStub.Stub (stub => stub.ClientScript).Return (clientScriptStub);

      _control.Stub (stub => stub.Page).Return (pageStub);

      _renderer = new SingleViewRenderer (new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.Default);
    }

    [Test]
    public void RenderView ()
    {
      PopulateControl ();
      AssertRendering (false, false, false, false);
    }

    [Test]
    public void RenderViewWithCssClass ()
    {
      PopulateControl ();
      AssertRendering (false, true, false, false);
    }

    [Test]
    public void RenderViewWithCssClassInAttributes ()
    {
      PopulateControl ();
      AssertRendering (false, true, true, false);
    }

    [Test]
    public void RenderEmptyView ()
    {
      AssertRendering (true, false, false, false);
    }

    [Test]
    public void RenderEmptyViewWithCssClass ()
    {
      AssertRendering (true, true, false, false);
    }

    [Test]
    public void RenderEmptyViewWithCssClassInAttributes ()
    {
      AssertRendering (true, true, true, false);
    }

    [Test]
    public void RenderViewInDesignMode ()
    {
      PopulateControl ();
      _control.Stub (stub => stub.IsDesignMode).Return (true);
      AssertRendering (false, false, false, true);
    }

    [Test]
    public void RenderViewWithCssClassInDesignMode ()
    {
      PopulateControl ();
      _control.Stub (stub => stub.IsDesignMode).Return (true);
      AssertRendering (false, true, false, true);
    }

    [Test]
    public void RenderViewWithCssClassInAttributesInDesignMode ()
    {
      PopulateControl ();
      _control.Stub (stub => stub.IsDesignMode).Return (true);
      AssertRendering (false, true, true, true);
    }

    [Test]
    public void RenderEmptyViewInDesignMode ()
    {
      _control.Stub (stub => stub.IsDesignMode).Return (true);
      AssertRendering (true, false, false, true);
    }

    [Test]
    public void RenderEmptyViewWithCssClassInDesignMode ()
    {
      _control.Stub (stub => stub.IsDesignMode).Return (true);
      AssertRendering (true, true, false, true);
    }

    [Test]
    public void RenderEmptyViewWithCssClassInAttributesInDesignMode ()
    {
      _control.Stub (stub => stub.IsDesignMode).Return (true);
      AssertRendering (true, true, true, true);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      _renderer = new SingleViewRenderer (new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);

      PopulateControl ();
      var div = AssertRendering (false, false, false, false);

      div.AssertAttributeValueEquals (DiagnosticMetadataAttributes.ControlType, "SingleView");
    }

    private void PopulateControl ()
    {
      _control.TopControl.Controls.Add (new LiteralControl ("TopControl"));
      _control.BottomControl.Controls.Add (new LiteralControl ("BottomControl"));
      _control.View.Controls.Add (new LiteralControl ("View"));
    }

    private XmlNode AssertRendering (bool isEmpty, bool withCssClasses, bool inAttributes, bool isDesignMode)
    {
      _renderer.Render (new SingleViewRenderingContext (_httpContext, _htmlHelper.Writer, _control));

      var document = _htmlHelper.GetResultDocument();
      document.AssertChildElementCount (1);

      var outerDiv = document.GetAssertedChildElement ("div", 0);
      outerDiv.AssertAttributeValueEquals (
          "class", withCssClasses ? (inAttributes ? _control.Attributes["class"] : _control.CssClass) : _renderer.CssClassBase);

      if (isDesignMode)
      {
        _htmlHelper.AssertStyleAttribute (outerDiv, "width", "100%");
        _htmlHelper.AssertStyleAttribute (outerDiv, "height", "75%");
      }

      var contentDiv = outerDiv.GetAssertedChildElement ("div", 0);
      contentDiv.AssertAttributeValueEquals ("class", _renderer.CssClassWrapper);

      var topControls = contentDiv.GetAssertedChildElement ("div", 0);
      topControls.AssertAttributeValueEquals ("id", _control.TopControl.ClientID);
      topControls.AssertAttributeValueContains ("class", _renderer.CssClassTopControls);
      if (isEmpty)
        topControls.AssertAttributeValueContains ("class", _renderer.CssClassEmpty);
      var topContent = topControls.GetAssertedChildElement ("div", 0);
      topContent.AssertAttributeValueContains ("class", _renderer.CssClassContent);

      var bottomControls = contentDiv.GetAssertedChildElement ("div", 2);
      bottomControls.AssertAttributeValueEquals ("id", _control.BottomControl.ClientID);
      bottomControls.AssertAttributeValueContains ("class", _renderer.CssClassBottomControls);
      if (isEmpty)
        bottomControls.AssertAttributeValueContains ("class", _renderer.CssClassEmpty);
      var bottomContent = bottomControls.GetAssertedChildElement ("div", 0);
      bottomContent.AssertAttributeValueEquals ("class", _renderer.CssClassContent);

      var viewContainer = contentDiv.GetAssertedChildElement ("div", 1);
      viewContainer.AssertAttributeValueEquals ("id", _control.ViewClientID);
      viewContainer.AssertAttributeValueEquals ("class", _renderer.CssClassView);

      var viewContentBorder = viewContainer.GetAssertedChildElement ("div", 0);
      viewContentBorder.AssertAttributeValueEquals ("class", _renderer.CssClassContentBorder);

      var viewContent = viewContentBorder.GetAssertedChildElement ("div", 0);
      viewContent.AssertAttributeValueEquals ("class", _renderer.CssClassContent);

      if (!isEmpty)
      {
        topContent.AssertTextNode ("TopControl", 0);
        bottomContent.AssertTextNode ("BottomControl", 0);
        viewContent.AssertTextNode ("View", 0);
      }

      return outerDiv;
    }
  }
}