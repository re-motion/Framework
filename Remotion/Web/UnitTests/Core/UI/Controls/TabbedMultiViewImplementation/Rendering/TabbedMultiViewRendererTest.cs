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
using Remotion.Web.UI.Controls.TabbedMultiViewImplementation;
using Remotion.Web.UI.Controls.TabbedMultiViewImplementation.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls.TabbedMultiViewImplementation.Rendering
{
  [TestFixture]
  public class TabbedMultiViewRendererTest : RendererTestBase
  {
    private const string c_cssClass = "SomeCssClass";

    private ITabbedMultiView _control;
    private HttpContextBase _httpContext;
    private HtmlHelper _htmlHelper;
    private TabbedMultiViewRenderer _renderer;

    [SetUp]
    public void SetUp ()
    {
      _htmlHelper = new HtmlHelper ();
      _httpContext = MockRepository.GenerateStub<HttpContextBase> ();

      _control = MockRepository.GenerateStub<ITabbedMultiView>();
      _control.Stub (stub => stub.ClientID).Return ("MyTabbedMultiView");
      _control.Stub (stub => stub.ControlType).Return ("TabbedMultiView");
      _control.Stub (stub => stub.TopControl).Return (new PlaceHolder { ID = "MyTabbedMultiView_TopControl" });
      _control.Stub (stub => stub.BottomControl).Return (new PlaceHolder { ID = "MyTabbedMultiView_BottomControl" });

      var tabStrip = MockRepository.GenerateStub<IWebTabStrip>();
      tabStrip.Stub (stub => stub.RenderControl (_htmlHelper.Writer)).WhenCalled (
          delegate (MethodInvocation obj)
          {
            HtmlTextWriter writer = (HtmlTextWriter) obj.Arguments[0];
            writer.AddAttribute (HtmlTextWriterAttribute.Class, tabStrip.CssClass);
            writer.RenderBeginTag ("tabStrip");
            writer.RenderEndTag ();
          });

      _control.Stub (stub => stub.TabStrip).Return (tabStrip);

      _control.Stub (stub => stub.ActiveViewClientID).Return (_control.ClientID + "_ActiveView");
      _control.Stub (stub => stub.ActiveViewContentClientID).Return (_control.ActiveViewClientID + "_Content");
      _control.Stub (stub => stub.WrapperClientID).Return ("WrapperClientID");
      

      StateBag stateBag = new StateBag ();
      _control.Stub (stub => stub.Attributes).Return (new AttributeCollection (stateBag));
      _control.Stub (stub => stub.TopControlsStyle).Return (new Style (stateBag));
      _control.Stub (stub => stub.BottomControlsStyle).Return (new Style (stateBag));
      _control.Stub (stub => stub.ActiveViewStyle).Return (new WebTabStyle ());
      _control.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));

      var clientScriptStub = MockRepository.GenerateStub<IClientScriptManager> ();

      var pageStub = MockRepository.GenerateStub<IPage> ();
      pageStub.Stub (stub => stub.ClientScript).Return (clientScriptStub);

      _control.Stub (stub => stub.Page).Return (pageStub);

      _renderer = new TabbedMultiViewRenderer (new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.Default);
    }

    [Test]
    public void RenderEmptyControl ()
    {
      AssertControl (false, false, false, true);
    }

    [Test]
    public void RenderEmptyControlWithCssClass ()
    {
      _control.CssClass = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, false, false, true);
    }

    [Test]
    public void RenderEmptyControlWithCssClassInAttributes ()
    {
      _control.Attributes["class"] = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, true, false, true);
    }

    [Test]
    public void RenderEmptyControlInDesignMode ()
    {
      _control.Stub (stub => stub.IsDesignMode).Return (true);

      AssertControl (false, false, true, true);
    }

    [Test]
    public void RenderEmptyControlWithCssClassInDesignMode ()
    {
      _control.Stub (stub => stub.IsDesignMode).Return (true);
      _control.CssClass = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, false,true, true);
    }

    [Test]
    public void RenderPopulatedControl ()
    {
      PopulateControl();

      AssertControl (false, false, false, false);
    }

    [Test]
    public void RenderPopulatedControlWithCssClass ()
    {
      PopulateControl();

      _control.CssClass = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, false, false, false);
    }

    [Test]
    public void RenderPopulatedControlInDesignMode ()
    {
      PopulateControl ();
      _control.Stub (stub => stub.IsDesignMode).Return (true);

      AssertControl (false, false, true, false);
    }

    [Test]
    public void RenderPopulatedControlWithCssClassInDesignMode ()
    {
      PopulateControl();

      _control.Stub (stub => stub.IsDesignMode).Return (true);
      _control.CssClass = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, false, true, false);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      PopulateControl();

      _renderer = new TabbedMultiViewRenderer (new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);
      
      var div = AssertControl (false, false, false, false);

      div.AssertAttributeValueEquals (DiagnosticMetadataAttributes.ControlType, "TabbedMultiView");
    }

    private void PopulateControl ()
    {
      _control.TopControl.Controls.Add (new LiteralControl ("TopControls"));
      
      var view1 = new TabView { ID="View1ID", Title = "View1Title" };
      view1.LazyControls.Add (new LiteralControl ("View1Contents"));
      _control.Stub(stub=>stub.GetActiveView()).Return (view1);

      _control.BottomControl.Controls.Add (new LiteralControl ("BottomControls"));
    }

    private XmlNode AssertControl (bool withCssClass, bool inAttributes, bool isDesignMode, bool isEmpty)
    {
      _renderer.Render (new TabbedMultiViewRenderingContext (_httpContext, _htmlHelper.Writer, _control));

      var outerDiv = GetAssertedElement (withCssClass, inAttributes, isDesignMode, _renderer);
      var contentDiv = outerDiv.GetAssertedChildElement ("div", 0);
      contentDiv.AssertAttributeValueEquals ("class", _renderer.CssClassWrapper);

      AssertTopControls (contentDiv, withCssClass, isEmpty, _renderer);
      AssertTabStrip (contentDiv, _renderer);
      AssertView (contentDiv, withCssClass, isDesignMode, _renderer);
      AssertBottomControls (contentDiv, withCssClass, isEmpty, _renderer);

      return outerDiv;
    }

    private XmlNode GetAssertedElement (bool withCssClass, bool inAttributes, bool isDesignMode, TabbedMultiViewRenderer renderer)
    {
      string cssClass = renderer.CssClassBase;
      if (withCssClass)
      {
        cssClass = inAttributes ? _control.Attributes["class"] : _control.CssClass;
      }

      var document = _htmlHelper.GetResultDocument();
      var outerDiv = document.GetAssertedChildElement ("div", 0);
      
      outerDiv.AssertAttributeValueEquals ("class", cssClass);
      if (isDesignMode)
      {
        outerDiv.AssertStyleAttribute ("width", "100%");
        outerDiv.AssertStyleAttribute ("height", "75%");
      }
      outerDiv.AssertChildElementCount (1);

      return outerDiv;
    }

    private void AssertBottomControls (XmlNode container, bool withCssClass, bool isEmpty, TabbedMultiViewRenderer renderer)
    {
      string cssClass = renderer.CssClassBottomControls;
      if (withCssClass)
        cssClass = c_cssClass;

      var divBottomControls = container.GetAssertedChildElement ("div", 3);

      divBottomControls.AssertAttributeValueEquals ("id", _control.BottomControl.ClientID);
      divBottomControls.AssertAttributeValueContains ("class", cssClass);
      if( isEmpty )
        divBottomControls.AssertAttributeValueContains ("class", renderer.CssClassEmpty);

      divBottomControls.AssertChildElementCount (1);

      var divContent = divBottomControls.GetAssertedChildElement ("div", 0);
      divContent.AssertAttributeValueEquals ("class", renderer.CssClassContent);

      if (!isEmpty)
        divContent.AssertTextNode ("BottomControls", 0);
    }

    private void AssertView (XmlNode container, bool withCssClass, bool isDesignMode, TabbedMultiViewRenderer renderer)
    {
      string cssClassActiveView = renderer.CssClassActiveView;
      if (withCssClass)
        cssClassActiveView = c_cssClass;

      var divActiveView = container.GetAssertedChildElement ("div", 2);
      divActiveView.AssertAttributeValueEquals ("id", _control.ActiveViewClientID);
      divActiveView.AssertAttributeValueEquals ("class", cssClassActiveView);
      if( isDesignMode )
        divActiveView.AssertStyleAttribute ("border", "solid 1px black");
      divActiveView.AssertChildElementCount (1);

      var divContentBorder = divActiveView.GetAssertedChildElement ("div", 0);
      divContentBorder.AssertAttributeValueEquals ("class", renderer.CssClassContentBorder);

      var divContent = divContentBorder.GetAssertedChildElement ("div", 0);
      divContent.AssertAttributeValueEquals ("class", renderer.CssClassContent);
    }

    private void AssertTabStrip (XmlNode container, TabbedMultiViewRenderer renderer)
    {
      var divTabStrip = container.GetAssertedChildElement ("tabStrip", 1);
      divTabStrip.AssertChildElementCount (0);

      divTabStrip.AssertAttributeValueEquals ("class", renderer.CssClassTabStrip);
    }

    private void AssertTopControls (XmlNode container, bool withCssClass, bool isEmpty, TabbedMultiViewRenderer renderer)
    {
      string cssClass = renderer.CssClassTopControls;
      if (withCssClass)
        cssClass = c_cssClass;

      var divTopControls = container.GetAssertedChildElement ("div", 0);
      divTopControls.AssertAttributeValueEquals ("id", _control.TopControl.ClientID);
      divTopControls.AssertAttributeValueContains ("class", cssClass);
      if (isEmpty)
        divTopControls.AssertAttributeValueContains ("class", renderer.CssClassEmpty);

      divTopControls.AssertChildElementCount (1);

      var divContent = divTopControls.GetAssertedChildElement ("div", 0);
      divContent.AssertAttributeValueEquals ("class", renderer.CssClassContent);

      if (!isEmpty)
        divContent.AssertTextNode ("TopControls", 0);
    }
  }
}