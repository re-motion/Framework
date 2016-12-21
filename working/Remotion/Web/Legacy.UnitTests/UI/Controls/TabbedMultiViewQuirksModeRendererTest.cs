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
using Remotion.Web.Infrastructure;
using Remotion.Web.Legacy.UI.Controls.Rendering;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.TabbedMultiViewImplementation;
using Remotion.Web.UI.Controls.TabbedMultiViewImplementation.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation;
using Rhino.Mocks;

namespace Remotion.Web.Legacy.UnitTests.UI.Controls
{
  [TestFixture]
  public class TabbedMultiViewQuirksModeRendererTest : RendererTestBase
  {
    private const string c_cssClass = "SomeCssClass";

    private ITabbedMultiView _control;
    private HttpContextBase _httpContext;
    private HtmlHelper _htmlHelper;
    private IResourceUrlFactory _resourceUrlFactory;

    public override void SetUp ()
    {
      base.SetUp();

      _htmlHelper = new HtmlHelper ();
      _httpContext = MockRepository.GenerateStub<HttpContextBase> ();
  
      _control = MockRepository.GenerateStub<ITabbedMultiView>();
      _control.Stub (stub => stub.ClientID).Return ("MyTabbedMultiView");
      _control.Stub (stub => stub.TopControl).Return (new PlaceHolder { ID = "MyTabbedMultiView_TopControl" });
      _control.Stub (stub => stub.BottomControl).Return (new PlaceHolder { ID = "MyTabbedMultiView_BottomControl" });

      var tabStrip = MockRepository.GenerateStub<IWebTabStrip>();
      _control.Stub (stub => stub.TabStrip).Return (tabStrip);

      _control.Stub (stub => stub.ActiveViewClientID).Return (_control.ClientID + "_ActiveView");

      StateBag stateBag = new StateBag ();
      _control.Stub (stub => stub.Attributes).Return (new AttributeCollection (stateBag));
      _control.Stub (stub => stub.TopControlsStyle).Return (new Style (stateBag));
      _control.Stub (stub => stub.BottomControlsStyle).Return (new Style (stateBag));
      _control.Stub (stub => stub.ActiveViewStyle).Return (new WebTabStyle ());
      _control.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));

      var clientScriptStub = MockRepository.GenerateStub<IClientScriptManager>();

      var pageStub = MockRepository.GenerateStub<IPage>();
      pageStub.Stub (stub => stub.ClientScript).Return (clientScriptStub);

      _control.Stub (stub => stub.Page).Return (pageStub);

      _resourceUrlFactory = new FakeResourceUrlFactory();
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

    private void PopulateControl ()
    {
      _control.TopControl.Controls.Add (new LiteralControl ("TopControls"));
      
      var view1 = new TabView { ID="View1ID", Title = "View1Title" };
      view1.LazyControls.Add (new LiteralControl ("View1Contents"));
      _control.Stub(stub=>stub.GetActiveView()).Return (view1);

      _control.BottomControl.Controls.Add (new LiteralControl ("BottomControls"));
    }

    private void AssertControl (bool withCssClass, bool inAttributes, bool isDesignMode, bool isEmpty)
    {
      var renderer = new TabbedMultiViewQuirksModeRenderer (_resourceUrlFactory);
      renderer.Render (new TabbedMultiViewRenderingContext (_httpContext, _htmlHelper.Writer, _control));

      var table = GetAssertedTableElement (withCssClass, inAttributes, isDesignMode, renderer);
      AssertTopRow (table, withCssClass, isEmpty, renderer);
      AssertTabStripRow (table, renderer);
      AssertViewRow (table, withCssClass, isDesignMode, renderer);
      AssertBottomRow (table, withCssClass, isEmpty, renderer);
    }

    private XmlNode GetAssertedTableElement (bool withCssClass, bool inAttributes, bool isDesignMode, TabbedMultiViewQuirksModeRenderer renderer)
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

      var table = outerDiv.GetAssertedChildElement ("table", 0);
      table.AssertAttributeValueEquals ("class", cssClass);
      table.AssertChildElementCount (4);
      return table;
    }

    private void AssertBottomRow (XmlNode table, bool withCssClass, bool isEmpty, TabbedMultiViewQuirksModeRenderer renderer)
    {
      string cssClass = renderer.CssClassBottomControls;
      if (withCssClass)
        cssClass = c_cssClass;

      var trBottom = table.GetAssertedChildElement ("tr", 3);
      trBottom.AssertChildElementCount (1);

      var tdBottom = trBottom.GetAssertedChildElement ("td", 0);
      tdBottom.AssertAttributeValueContains ("class", cssClass);
      if( isEmpty )
        tdBottom.AssertAttributeValueContains ("class", renderer.CssClassEmpty);
      tdBottom.AssertChildElementCount (1);

      var divBottomControl = tdBottom.GetAssertedChildElement ("div", 0);
      divBottomControl.AssertAttributeValueEquals ("id", _control.ClientID + "_BottomControl");
      divBottomControl.AssertAttributeValueEquals ("class", cssClass);
      divBottomControl.AssertChildElementCount (1);

      var divBottomContent = divBottomControl.GetAssertedChildElement ("div", 0);
      divBottomContent.AssertAttributeValueEquals ("class", renderer.CssClassContent);
      divBottomContent.AssertChildElementCount (0);
    }

    private void AssertViewRow (XmlNode table, bool withCssClass, bool isDesignMode, TabbedMultiViewQuirksModeRenderer renderer)
    {
      string cssClassActiveView = renderer.CssClassActiveView;
      if (withCssClass)
        cssClassActiveView = c_cssClass;

      var trActiveView = table.GetAssertedChildElement ("tr", 2);
      trActiveView.AssertChildElementCount (1);

      var tdActiveView = trActiveView.GetAssertedChildElement ("td", 0);
      tdActiveView.AssertAttributeValueEquals ("class", cssClassActiveView);
      if( isDesignMode )
        tdActiveView.AssertStyleAttribute ("border", "solid 1px black");

      tdActiveView.AssertChildElementCount (1);

      var divActiveView = tdActiveView.GetAssertedChildElement ("div", 0);
      divActiveView.AssertAttributeValueEquals ("id", _control.ClientID + "_ActiveView");
      divActiveView.AssertAttributeValueEquals ("class", cssClassActiveView);
      divActiveView.AssertChildElementCount (1);

      var divBody = divActiveView.GetAssertedChildElement ("div", 0);
      divBody.AssertAttributeValueEquals ("class", renderer.CssClassViewBody);
      divBody.AssertChildElementCount (1);

      var divActiveViewContent = divBody.GetAssertedChildElement ("div", 0);
      divActiveViewContent.AssertAttributeValueEquals ("id", _control.ClientID + "_ActiveView_Content");
      divActiveViewContent.AssertAttributeValueEquals ("class", renderer.CssClassContent);
      divActiveViewContent.AssertChildElementCount (0);
    }

    private void AssertTabStripRow (XmlNode table, TabbedMultiViewQuirksModeRenderer renderer)
    {
      string cssClass = renderer.CssClassTabStrip;

      var trTabStrip = table.GetAssertedChildElement ("tr", 1);
      trTabStrip.AssertChildElementCount (1);

      var tdTabStrip = trTabStrip.GetAssertedChildElement ("td", 0);
      tdTabStrip.AssertAttributeValueEquals ("class", cssClass);
      tdTabStrip.AssertChildElementCount (0);
    }

    private void AssertTopRow (XmlNode table, bool withCssClass, bool isEmpty, TabbedMultiViewQuirksModeRenderer renderer)
    {
      string cssClass = renderer.CssClassTopControls;
      if (withCssClass)
        cssClass = c_cssClass;

      var trTop = table.GetAssertedChildElement ("tr", 0);
      trTop.AssertChildElementCount (1);

      var tdTop = trTop.GetAssertedChildElement ("td", 0);
      tdTop.AssertAttributeValueContains ("class", cssClass);
      if( isEmpty )
        tdTop.AssertAttributeValueContains ("class", renderer.CssClassEmpty);

      tdTop.AssertChildElementCount (1);

      var divTopControl = tdTop.GetAssertedChildElement ("div", 0);
      divTopControl.AssertAttributeValueEquals ("id", _control.ClientID + "_TopControl");
      divTopControl.AssertAttributeValueEquals ("class", cssClass);
      divTopControl.AssertChildElementCount (1);

      var divTopContent = divTopControl.GetAssertedChildElement ("div", 0);
      divTopContent.AssertAttributeValueEquals ("class", renderer.CssClassContent);
      divTopContent.AssertChildElementCount (0);
    }
  }
}