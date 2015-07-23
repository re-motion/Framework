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
using Remotion.Web.Legacy.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.SingleViewImplementation;
using Remotion.Web.UI.Controls.SingleViewImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.Web.Legacy.UnitTests.UI.Controls
{
  [TestFixture]
  public class SingleViewQuirksModeRendererTest : RendererTestBase
  {
    private const string c_cssClass = "CssClass";

    private ISingleView _singleView;
    private HttpContextBase _httpContext;
    private HtmlHelper _htmlHelper;
    private IResourceUrlFactory _resourceUrlFactory;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _htmlHelper = new HtmlHelper ();
      _httpContext = MockRepository.GenerateStub<HttpContextBase> ();

      _singleView = MockRepository.GenerateStub<ISingleView>();
      _singleView.Stub (stub => stub.ClientID).Return ("SingleView");
      _singleView.Stub (stub => stub.TopControl).Return (new PlaceHolder { ID = "TopControl" });
      _singleView.Stub (stub => stub.BottomControl).Return (new PlaceHolder { ID = "BottomControl" });
      _singleView.Stub (stub => stub.View).Return (new PlaceHolder { ID = "ViewControl" });
      _singleView.Stub (stub => stub.ViewClientID).Return ("ViewClientID");

      StateBag stateBag = new StateBag ();
      _singleView.Stub (stub => stub.Attributes).Return (new AttributeCollection (stateBag));
      _singleView.Stub (stub => stub.TopControlsStyle).Return (new Style (stateBag));
      _singleView.Stub (stub => stub.BottomControlsStyle).Return (new Style (stateBag));
      _singleView.Stub (stub => stub.ViewStyle).Return (new Style (stateBag));
      _singleView.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));

      _resourceUrlFactory = new FakeResourceUrlFactory();
    }

    [Test]
    public void RenderView ()
    {
      PopulateControl();
      AssertRendering (false, false, false, false);
    }

    [Test]
    public void RenderViewWithCssClass ()
    {
      PopulateControl();
      AssertRendering (false, true, false, false);
    }

    [Test]
    public void RenderViewWithCssClassInAttributes ()
    {
      PopulateControl();
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
      PopulateControl();
      _singleView.Stub (stub => stub.IsDesignMode).Return (true);
      AssertRendering (false, false, false, true);
    }

    [Test]
    public void RenderViewWithCssClassInDesignMode ()
    {
      PopulateControl();
      _singleView.Stub (stub => stub.IsDesignMode).Return (true);
      AssertRendering (false, true, false, true);
    }

    [Test]
    public void RenderViewWithCssClassInAttributesInDesignMode ()
    {
      PopulateControl();
      _singleView.Stub (stub => stub.IsDesignMode).Return (true);
      AssertRendering (false, true, true, true);
    }

    [Test]
    public void RenderEmptyViewInDesignMode ()
    {
      _singleView.Stub (stub => stub.IsDesignMode).Return (true);
      AssertRendering (true, false, false, true);
    }

    [Test]
    public void RenderEmptyViewWithCssClassInDesignMode ()
    {
      _singleView.Stub (stub => stub.IsDesignMode).Return (true);
      AssertRendering (true, true, false, true);
    }

    [Test]
    public void RenderEmptyViewWithCssClassInAttributesInDesignMode ()
    {
      _singleView.Stub (stub => stub.IsDesignMode).Return (true);
      AssertRendering (true, true, true, true);
    }

    private void PopulateControl ()
    {
      _singleView.TopControl.Controls.Add (new LiteralControl ("TopControl"));
      _singleView.BottomControl.Controls.Add (new LiteralControl ("BottomControl"));
      _singleView.View.Controls.Add (new LiteralControl ("View"));
    }

    private void AssertRendering (bool isEmpty, bool withCssClasses, bool inAttributes, bool isDesignMode)
    {
      var renderer = new SingleViewQuirksModeRenderer (_resourceUrlFactory);

      string controlCssClass = renderer.CssClassBase;
      string topControlsCssClass = renderer.CssClassTopControls;
      string bottomControlsCssClass = renderer.CssClassBottomControls;

      string contentCssClass = renderer.CssClassContent;
      string viewCssClass = renderer.CssClassView;
      string viewBodyCssClass = renderer.CssClassViewBody;

      if (withCssClasses)
      {
        controlCssClass = c_cssClass;
        topControlsCssClass = c_cssClass;
        bottomControlsCssClass = c_cssClass;
        viewCssClass = c_cssClass;
        if (inAttributes)
          _singleView.Attributes["class"] = controlCssClass;
        else
          _singleView.CssClass = controlCssClass;

        _singleView.TopControlsStyle.CssClass = topControlsCssClass;
        _singleView.BottomControlsStyle.CssClass = bottomControlsCssClass;
        _singleView.ViewStyle.CssClass = viewCssClass;
      }

      renderer.Render (new SingleViewRenderingContext (_httpContext, _htmlHelper.Writer, _singleView));
      //_singleView.RenderControl (_htmlHelper.Writer);

      var document = _htmlHelper.GetResultDocument();

      XmlNode outerDiv = GetAssertedOuterDiv (document, controlCssClass, isDesignMode);
      XmlNode table = GetAssertedTable (outerDiv, controlCssClass);


      XmlNode tdTop = GetAssertedTdTop (table, topControlsCssClass, isEmpty, renderer.CssClassEmpty);
      XmlNode divTopControls = GetAssertedDivTopControls (tdTop, topControlsCssClass);

      var divTopContent = _htmlHelper.GetAssertedChildElement (divTopControls, "div", 0);

      _htmlHelper.AssertAttribute (divTopContent, "class", contentCssClass);


      XmlNode tdView = GetAssertedTdView (table, viewCssClass);
      XmlNode divViewControls = GetAssertedDivViewControls (tdView, viewCssClass);


      XmlNode divViewBody = GetAssertedDivViewBody (divViewControls, viewBodyCssClass);

      var divViewContent = _htmlHelper.GetAssertedChildElement (divViewBody, "div", 0);
      _htmlHelper.AssertAttribute (divViewContent, "class", contentCssClass);


      XmlNode tdBottom = GetAssertedTdBottom (table, bottomControlsCssClass, isEmpty, renderer.CssClassEmpty);
      XmlNode divBottomControls = GetAssertedDivBottomControls (tdBottom, bottomControlsCssClass);

      var divBottomContent = _htmlHelper.GetAssertedChildElement (divBottomControls, "div", 0);
      _htmlHelper.AssertAttribute (divBottomContent, "class", contentCssClass);
    }

    private XmlNode GetAssertedDivViewBody (XmlNode divViewControls, string cssClass)
    {
      var divViewBody = _htmlHelper.GetAssertedChildElement (divViewControls, "div", 0);
      _htmlHelper.AssertAttribute (divViewBody, "class", cssClass);
      _htmlHelper.AssertChildElementCount (divViewBody, 1);
      return divViewBody;
    }

    private XmlNode GetAssertedDivBottomControls (XmlNode tdBottom, string cssClass)
    {
      var divBottomControls = _htmlHelper.GetAssertedChildElement (tdBottom, "div", 0);
      _htmlHelper.AssertAttribute (divBottomControls, "id", _singleView.BottomControl.ClientID);
      _htmlHelper.AssertAttribute (divBottomControls, "class", cssClass);
      _htmlHelper.AssertChildElementCount (divBottomControls, 1);
      return divBottomControls;
    }

    private XmlNode GetAssertedDivViewControls (XmlNode tdView, string cssClass)
    {
      var divViewControls = _htmlHelper.GetAssertedChildElement (tdView, "div", 0);
      _htmlHelper.AssertAttribute (divViewControls, "class", cssClass);
      _htmlHelper.AssertAttribute (divViewControls, "id", _singleView.ViewClientID);
      _htmlHelper.AssertChildElementCount (divViewControls, 1);
      return divViewControls;
    }

    private XmlNode GetAssertedDivTopControls (XmlNode tdTop, string cssClass)
    {
      var divTopControls = _htmlHelper.GetAssertedChildElement (tdTop, "div", 0);
      _htmlHelper.AssertAttribute (divTopControls, "id", _singleView.TopControl.ClientID);
      _htmlHelper.AssertAttribute (divTopControls, "class", cssClass);
      _htmlHelper.AssertChildElementCount (divTopControls, 1);
      return divTopControls;
    }

    private XmlNode GetAssertedTdBottom (XmlNode table, string cssClass, bool isEmpty, string cssClassEmpty)
    {
      var trBottom = _htmlHelper.GetAssertedChildElement (table, "tr", 2);
      _htmlHelper.AssertChildElementCount (trBottom, 1);

      var tdBottom = _htmlHelper.GetAssertedChildElement (trBottom, "td", 0);
      _htmlHelper.AssertAttribute (tdBottom, "class", cssClass, HtmlHelperBase.AttributeValueCompareMode.Contains);
      if (isEmpty)
        _htmlHelper.AssertAttribute (tdBottom, "class", cssClassEmpty, HtmlHelperBase.AttributeValueCompareMode.Contains);
      _htmlHelper.AssertChildElementCount (tdBottom, 1);
      return tdBottom;
    }

    private XmlNode GetAssertedTdView (XmlNode table, string cssClass)
    {
      var trView = _htmlHelper.GetAssertedChildElement (table, "tr", 1);
      _htmlHelper.AssertChildElementCount (trView, 1);

      var tdView = _htmlHelper.GetAssertedChildElement (trView, "td", 0);
      _htmlHelper.AssertAttribute (tdView, "class", cssClass);
      if (_singleView.IsDesignMode)
        _htmlHelper.AssertStyleAttribute (tdView, "border", "solid 1px black");

      _htmlHelper.AssertChildElementCount (tdView, 1);
      return tdView;
    }

    private XmlNode GetAssertedTdTop (XmlNode table, string cssClass, bool isEmpty, string cssClassEmpty)
    {
      var trTop = _htmlHelper.GetAssertedChildElement (table, "tr", 0);
      _htmlHelper.AssertChildElementCount (trTop, 1);

      var tdTop = _htmlHelper.GetAssertedChildElement (trTop, "td", 0);
      _htmlHelper.AssertAttribute (tdTop, "class", cssClass, HtmlHelperBase.AttributeValueCompareMode.Contains);
      if (isEmpty)
        _htmlHelper.AssertAttribute (tdTop, "class", cssClassEmpty, HtmlHelperBase.AttributeValueCompareMode.Contains);
      _htmlHelper.AssertChildElementCount (tdTop, 1);
      return tdTop;
    }

    private XmlNode GetAssertedTable (XmlNode outerDiv, string cssClass)
    {
      var table = _htmlHelper.GetAssertedChildElement (outerDiv, "table", 0);
      _htmlHelper.AssertAttribute (table, "class", cssClass);
      _htmlHelper.AssertChildElementCount (table, 3);
      return table;
    }

    private XmlNode GetAssertedOuterDiv (XmlDocument document, string cssClass, bool isDesignMode)
    {
      var outerDiv = _htmlHelper.GetAssertedChildElement (document, "div", 0);
      _htmlHelper.AssertAttribute (outerDiv, "class", cssClass);
      if (isDesignMode)
      {
        _htmlHelper.AssertStyleAttribute (outerDiv, "width", "100%");
        _htmlHelper.AssertStyleAttribute (outerDiv, "height", "75%");
      }
      _htmlHelper.AssertChildElementCount (outerDiv, 1);
      return outerDiv;
    }
  }
}