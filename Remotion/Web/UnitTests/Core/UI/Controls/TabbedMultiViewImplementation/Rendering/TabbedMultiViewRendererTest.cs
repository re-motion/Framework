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
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.TabbedMultiViewImplementation;
using Remotion.Web.UI.Controls.TabbedMultiViewImplementation.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation;

namespace Remotion.Web.UnitTests.Core.UI.Controls.TabbedMultiViewImplementation.Rendering
{
  [TestFixture]
  public class TabbedMultiViewRendererTest : RendererTestBase
  {
    private const string c_cssClass = "SomeCssClass";

    private Mock<ITabbedMultiView> _control;
    private Mock<HttpContextBase> _httpContext;
    private HtmlHelper _htmlHelper;
    private TabbedMultiViewRenderer _renderer;

    [SetUp]
    public void SetUp ()
    {
      _htmlHelper = new HtmlHelper ();
      _httpContext = new Mock<HttpContextBase>();

      _control = new Mock<ITabbedMultiView>();
      _control.Setup (stub => stub.ClientID).Returns ("MyTabbedMultiView");
      _control.Setup (stub => stub.ControlType).Returns ("TabbedMultiView");
      _control.Setup (stub => stub.TopControl).Returns (new PlaceHolder { ID = "MyTabbedMultiView_TopControl" });
      _control.Setup (stub => stub.BottomControl).Returns (new PlaceHolder { ID = "MyTabbedMultiView_BottomControl" });

      var tabStrip = new Mock<IWebTabStrip>();
      tabStrip.SetupProperty (_ => _.CssClass);
      tabStrip.Setup (stub => stub.RenderControl (_htmlHelper.Writer)).Callback (
          delegate (HtmlTextWriter writer)
          {
            writer.AddAttribute (HtmlTextWriterAttribute.Class, tabStrip.Object.CssClass);
            writer.RenderBeginTag ("tabStrip");
            writer.RenderEndTag();
          });
      var tabs = new WebTabCollection(tabStrip.Object);
      tabs.Add (new WebTab { ItemID = "Tab1" });
      tabs.Add (new WebTab { ItemID = "Tab2", IsSelected = true });
      tabs.Add (new WebTab { ItemID = "Tab3" });
      tabStrip.Setup (stub => stub.Tabs).Returns (tabs);
      tabStrip.Setup (stub => stub.ClientID).Returns ("TabStripClientID");

      _control.Setup (stub => stub.TabStrip).Returns (tabStrip.Object);

      _control.Setup (stub => stub.ActiveViewClientID).Returns (_control.Object.ClientID + "_ActiveView");
      _control.Setup (stub => stub.ActiveViewContentClientID).Returns (_control.Object.ActiveViewClientID + "_Content");
      _control.Setup (stub => stub.WrapperClientID).Returns ("WrapperClientID");
      

      StateBag stateBag = new StateBag ();
      _control.Setup (stub => stub.Attributes).Returns (new AttributeCollection (stateBag));
      _control.Setup (stub => stub.TopControlsStyle).Returns (new Style (stateBag));
      _control.Setup (stub => stub.BottomControlsStyle).Returns (new Style (stateBag));
      _control.Setup (stub => stub.ActiveViewStyle).Returns (new WebTabStyle ());
      _control.Setup (stub => stub.ControlStyle).Returns (new Style (stateBag));

      var clientScriptStub = new Mock<IClientScriptManager>();

      var pageStub = new Mock<IPage>();
      pageStub.Setup (stub => stub.ClientScript).Returns (clientScriptStub.Object);

      _control.Setup (stub => stub.Page).Returns (pageStub.Object);

      _renderer = new TabbedMultiViewRenderer (new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.Default, new StubLabelReferenceRenderer());
    }

    [Test]
    public void RenderEmptyControl ()
    {
      AssertControl (false, false, false, true);
    }

    [Test]
    public void RenderEmptyControlWithCssClass ()
    {
      _control.SetupProperty (_ => _.CssClass);
      _control.Object.CssClass = c_cssClass;
      _control.Object.TopControlsStyle.CssClass = c_cssClass;
      _control.Object.ActiveViewStyle.CssClass = c_cssClass;
      _control.Object.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, false, false, true);
    }

    [Test]
    public void RenderEmptyControlWithCssClassInAttributes ()
    {
      _control.Object.Attributes["class"] = c_cssClass;
      _control.Object.TopControlsStyle.CssClass = c_cssClass;
      _control.Object.ActiveViewStyle.CssClass = c_cssClass;
      _control.Object.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, true, false, true);
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

      _control.SetupProperty (_ => _.CssClass);
      _control.Object.CssClass = c_cssClass;
      _control.Object.TopControlsStyle.CssClass = c_cssClass;
      _control.Object.ActiveViewStyle.CssClass = c_cssClass;
      _control.Object.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, false, false, false);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      PopulateControl();

      _renderer = new TabbedMultiViewRenderer (new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.WithDiagnosticMetadata, new StubLabelReferenceRenderer());
      
      var div = AssertControl (false, false, false, false);

      div.AssertAttributeValueEquals (DiagnosticMetadataAttributes.ControlType, "TabbedMultiView");
    }

    private void PopulateControl ()
    {
      _control.Object.TopControl.Controls.Add (new LiteralControl ("TopControls"));
      
      var view1 = new TabView { ID="View1ID", Title = "View1Title" };
      view1.LazyControls.Add (new LiteralControl ("View1Contents"));
      _control.Setup(stub=>stub.GetActiveView()).Returns (view1);

      _control.Object.BottomControl.Controls.Add (new LiteralControl ("BottomControls"));
    }

    private XmlNode AssertControl (bool withCssClass, bool inAttributes, bool isDesignMode, bool isEmpty)
    {
      _renderer.Render (new TabbedMultiViewRenderingContext (_httpContext.Object, _htmlHelper.Writer, _control.Object));

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
        cssClass = inAttributes ? _control.Object.Attributes["class"] : _control.Object.CssClass;
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

      divBottomControls.AssertAttributeValueEquals ("id", _control.Object.BottomControl.ClientID);
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
      divActiveView.AssertAttributeValueEquals ("id", _control.Object.ActiveViewClientID);
      divActiveView.AssertAttributeValueEquals ("class", cssClassActiveView);

      if( isDesignMode )
        divActiveView.AssertStyleAttribute ("border", "solid 1px black");
      divActiveView.AssertChildElementCount (1);

      var divContentBorder = divActiveView.GetAssertedChildElement ("div", 0);
      divContentBorder.AssertAttributeValueEquals ("class", renderer.CssClassContentBorder);
      divContentBorder.AssertAttributeValueEquals ("role", "tabpanel");
      divContentBorder.AssertAttributeValueEquals (StubLabelReferenceRenderer.LabelReferenceAttribute, "TabStripClientID_Tab2_Command");
      divContentBorder.AssertAttributeValueEquals (StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");
      divContentBorder.AssertAttributeValueEquals ("tabindex", "0");

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
      divTopControls.AssertAttributeValueEquals ("id", _control.Object.TopControl.ClientID);
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