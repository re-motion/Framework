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
using System.Collections.Generic;
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
using Remotion.Web.UI.Controls.Hotkey;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.TabbedMenuImplementation;
using Remotion.Web.UI.Controls.WebTabStripImplementation;
using Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls.WebTabStripImplementation.Rendering
{
  [TestFixture]
  public class WebTabStripRendererTest : RendererTestBase
  {
    private IWebTabStrip _webTabStrip;
    private WebTabStripRenderer _renderer;
    private IPage _pageStub;
    private IMenuTab _tab0, _tab1, _tab2, _tab3;
    private HttpContextBase _httpContextStub;
    private HtmlHelper _htmlHelper;
    private WebTabStyle _style;

    [SetUp]
    public void SetUp ()
    {
      _htmlHelper = new HtmlHelper();
      _httpContextStub = MockRepository.GenerateStub<HttpContextBase>();

      _webTabStrip = MockRepository.GenerateStub<IWebTabStrip>();
      _webTabStrip.Stub (stub => stub.ClientID).Return ("WebTabStrip");
      _webTabStrip.Stub (stub => stub.ControlType).Return ("WebTabStrip");
      _webTabStrip.Stub (stub => stub.ResolveClientUrl (null)).IgnoreArguments().Do ((Func<string, string>) (url => url.TrimStart ('~')));

      _pageStub = MockRepository.GenerateStub<IPage>();
      var clientScriptStub = MockRepository.GenerateStub<IClientScriptManager>();
      clientScriptStub.Stub (stub => stub.GetPostBackClientHyperlink (_webTabStrip, null)).IgnoreArguments().Return ("PostBackHyperlink");
      _pageStub.Stub (stub => stub.ClientScript).Return (clientScriptStub);

      _webTabStrip.Stub (stub => stub.Page).Return (_pageStub);
      _webTabStrip.Stub (stub => stub.Tabs).Return (new WebTabCollection (_webTabStrip));
      _webTabStrip.Stub (stub => stub.GetVisibleTabs()).Return (new List<IWebTab>());

      StateBag stateBag = new StateBag();
      _webTabStrip.Stub (stub => stub.Attributes).Return (new AttributeCollection (stateBag));
      _webTabStrip.Stub (stub => stub.SelectedTabStyle).Return (new WebTabStyle());
      _webTabStrip.Stub (stub => stub.TabStyle).Return (new WebTabStyle());
      _webTabStrip.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));

      _style = new WebTabStyle();
    }

    [Test]
    public void RenderEmptyStrip ()
    {
      var renderingContext = new WebTabStripRenderingContext (
          _httpContextStub,
          _htmlHelper.Writer,
          _webTabStrip,
          new WebTabRendererAdapter[0]);
      AssertControl (false, true, false, 0, renderingContext);
    }

    [Test]
    public void RenderPopulatedStrip ()
    {
      PopulateTabStrip();
      _tab0.Stub (stub => stub.EvaluateEnabled()).Return (true);

      var renderingContext= new WebTabStripRenderingContext (
          _httpContextStub,
          _htmlHelper.Writer,
          _webTabStrip,
          new[]
          {
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab0, false, true, _style), 
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab1, false, true, _style),
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab2, false, true, _style), 
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab3, true, true, _style)
          });

      AssertControl (false, false, false, 4, renderingContext);
    }

    [Test]
    public void RenderPopulatedStripWithCssClass ()
    {
      _webTabStrip.CssClass = "SomeCssClass";
      PopulateTabStrip();
      _tab0.Stub (stub => stub.EvaluateEnabled()).Return (true);

      var renderingContext = new WebTabStripRenderingContext (
          _httpContextStub,
          _htmlHelper.Writer,
          _webTabStrip,
          new[]
          {
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab0, false, true, _style), 
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab1, false, true, _style),
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab2, false, true, _style), 
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab3, true, true, _style)
          });

      AssertControl (true, false, false, 4, renderingContext);
    }

    [Test]
    public void RenderPopulatedStripWithEnableSelectedTab ()
    {
      _webTabStrip.Stub (stub => stub.EnableSelectedTab).Return (true);

      PopulateTabStrip();
      _tab0.Stub (stub => stub.EvaluateEnabled()).Return (true);

      var renderingContext = new WebTabStripRenderingContext (
          _httpContextStub,
          _htmlHelper.Writer,
          _webTabStrip,
          new[]
          {
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab0, false, true, _style), 
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab1, false, true, _style),
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab2, false, true, _style), 
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab3, true, true, _style)
          });

      AssertControl (false, false, false, 4, renderingContext);
    }

    [Test]
    public void RenderPopulatedStripWithDisabledTab ()
    {
      PopulateTabStrip();
      _tab0.Stub (stub => stub.EvaluateEnabled()).Return (false);

      var renderingContext = new WebTabStripRenderingContext (
          _httpContextStub,
          _htmlHelper.Writer,
          _webTabStrip,
          new[]
          {
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab0, false, true, _style), 
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab1, false, true, _style),
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab2, false, true, _style), 
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab3, true, true, _style)
          });

      AssertControl (false, false, false, 4, renderingContext);
    }

    [Test]
    public void RenderPopulatedStripWithInvisibleTab ()
    {
      PopulateTabStrip();
      _webTabStrip.GetVisibleTabs().RemoveAt (2);

      var renderingContext = new WebTabStripRenderingContext (
          _httpContextStub,
          _htmlHelper.Writer,
          _webTabStrip,
          new[]
          {
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab0, false, true, _style), 
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab1, false, true, _style),
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab3, true, true, _style)
          });

      AssertControl (false, false, false, 3, renderingContext);
    }

    [Test]
    public void RenderEmptyListInDesignMode ()
    {
      _webTabStrip.Stub (stub => stub.IsDesignMode).Return (true);

      var renderingContext = new WebTabStripRenderingContext (
          _httpContextStub,
          _htmlHelper.Writer,
          _webTabStrip,
          new WebTabRendererAdapter[0]);

      AssertControl (false, true, true, 0, renderingContext);
    }

    [Test]
    public void RenderPopulatedListInDesignMode ()
    {
      _webTabStrip.Stub (stub => stub.IsDesignMode).Return (true);
      PopulateTabStrip();
      var renderingContext = new WebTabStripRenderingContext (
          _httpContextStub,
          _htmlHelper.Writer,
          _webTabStrip,
          new[]
          {
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab0, false, true, _style), 
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab1, false, true, _style),
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab2, false, true, _style), 
              new WebTabRendererAdapter (CreateWebTabRenderer(), _tab3, true, true, _style)
          });

      AssertControl (false, true, true, 4, renderingContext);
    }

    [Test]
    public void TestDiagnosticMetadataRendering ()
    {
      PopulateTabStrip();
      var renderingContext = new WebTabStripRenderingContext (
          _httpContextStub,
          _htmlHelper.Writer,
          _webTabStrip,
          new[]
          {
              new WebTabRendererAdapter (CreateWebTabRenderer(RenderingFeatures.WithDiagnosticMetadata), _tab0, true, true, _style), 
          });

      _renderer = new WebTabStripRenderer (new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);
      _renderer.Render (renderingContext);

      var document = _htmlHelper.GetResultDocument();
      var outerDiv = document.GetAssertedChildElement ("div", 0);
      var innerDiv = outerDiv.GetAssertedChildElement ("div", 0);
      outerDiv.AssertAttributeValueEquals (DiagnosticMetadataAttributes.ControlType, _webTabStrip.ControlType);
      var list = innerDiv.GetAssertedChildElement ("ul", 0);
      var item = list.GetAssertedChildElement ("li", 0);
      var wrapper = item.GetAssertedChildElement ("span", 0);
      var tab = wrapper.GetAssertedChildElement ("span", 1);
      tab.AssertAttributeValueEquals (DiagnosticMetadataAttributes.ItemID, _tab0.ItemID);
      tab.AssertAttributeValueEquals (DiagnosticMetadataAttributes.Content, _tab0.Text);
    }

    private void PopulateTabStrip ()
    {
      _tab0 = MockRepository.GenerateStub<IMenuTab>();
      _tab0.Stub (stub => stub.ItemID).Return ("Tab0");
      _tab0.Stub (stub => stub.Text).Return ("&First Tab");
      _tab0.Stub (stub => stub.Icon).Return (new IconInfo ());
      _tab0.Stub (stub => stub.EvaluateEnabled()).Return (true);
      _tab0.Stub (stub => stub.GetPostBackClientEvent()).Return (_pageStub.ClientScript.GetPostBackClientHyperlink (_webTabStrip, _tab0.ItemID));
      _tab0.Stub (stub => stub.GetActiveTab()).Return (_tab0);
      _tab0.Stub (stub => stub.Command).Return (new NavigationCommand (CommandType.Event));
      _tab0.Stub (stub => stub.GetRenderer ()).IgnoreArguments ().Return (CreateWebTabRenderer());

      _tab1 = MockRepository.GenerateStub<IMenuTab>();
      _tab1.Stub (stub => stub.ItemID).Return ("Tab1");
      _tab1.Stub (stub => stub.Text).Return ("Second Tab");
      _tab1.Stub (stub => stub.Icon).Return (new IconInfo ("/myImageUrl"));
      _tab1.Stub (stub => stub.EvaluateEnabled()).Return (true);
      _tab1.Stub (stub => stub.GetPostBackClientEvent()).Return (_pageStub.ClientScript.GetPostBackClientHyperlink (_webTabStrip, _tab1.ItemID));
      _tab1.Stub (stub => stub.GetActiveTab()).Return (_tab1);
      _tab1.Stub (stub => stub.Command).Return (new NavigationCommand (CommandType.Event));
      _tab1.Stub (stub => stub.GetRenderer ()).IgnoreArguments ().Return (CreateWebTabRenderer());

      _tab2 = MockRepository.GenerateStub<IMenuTab>();
      _tab2.Stub (stub => stub.ItemID).Return ("Tab2");
      _tab2.Stub (stub => stub.Text).Return ("Third Tab");
      _tab2.Stub (stub => stub.Icon).Return (null);
      _tab2.Stub (stub => stub.EvaluateEnabled()).Return (true);
      _tab2.Stub (stub => stub.GetPostBackClientEvent()).Return (_pageStub.ClientScript.GetPostBackClientHyperlink (_webTabStrip, _tab2.ItemID));
      _tab2.Stub (stub => stub.GetActiveTab()).Return (_tab2);
      _tab2.Stub (stub => stub.Command).Return (new NavigationCommand (CommandType.Event));
      _tab2.Stub (stub => stub.GetRenderer ()).IgnoreArguments ().Return (CreateWebTabRenderer());

      _tab3 = MockRepository.GenerateStub<IMenuTab>();
      _tab3.Stub (stub => stub.ItemID).Return ("Tab3");
      _tab3.Stub (stub => stub.Text).Return (null);
      _tab3.Stub (stub => stub.Icon).Return (null);
      _tab3.Stub (stub => stub.EvaluateEnabled()).Return (true);
      _tab3.Stub (stub => stub.GetPostBackClientEvent()).Return (_pageStub.ClientScript.GetPostBackClientHyperlink (_webTabStrip, _tab3.ItemID));
      _tab3.Stub (stub => stub.GetActiveTab()).Return (_tab3);
      _tab3.Stub (stub => stub.Command).Return (new NavigationCommand (CommandType.Event));
      _tab3.Stub (stub => stub.GetRenderer ()).IgnoreArguments ().Return (CreateWebTabRenderer());

      _webTabStrip.GetVisibleTabs().Add (_tab0);
      _webTabStrip.GetVisibleTabs().Add (_tab1);
      _webTabStrip.GetVisibleTabs().Add (_tab2);
      _webTabStrip.GetVisibleTabs().Add (_tab3);
    }

    private void AssertControl (bool withCssClass, bool isEmpty, bool isDesignMode, int tabCount, WebTabStripRenderingContext renderingContext)
    {
      _renderer = new WebTabStripRenderer (new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.Default);
      _renderer.Render (renderingContext);

      var document = _htmlHelper.GetResultDocument();
      XmlNode list = GetAssertedTabList (document, withCssClass, isEmpty, tabCount, isDesignMode);
      AssertTabs (list, isDesignMode);
    }

    private void AssertTabs (XmlNode list, bool isDesignMode)
    {
      var tabs = _webTabStrip.GetVisibleTabs();
      int itemCount = list.ChildNodes.Count;
      if (!isDesignMode)
        Assert.That (itemCount, Is.EqualTo (tabs.Count));

      for (int i = 0; i < itemCount; i++)
      {
        IMenuTab tab = (IMenuTab) tabs[i];
        bool isLastItem = (i == itemCount - 1);

        var item = list.GetAssertedChildElement ("li", i);
        AssertItem (item, tab, isLastItem, isDesignMode);

        if (isLastItem)
        {
          var lastSpan = item.GetAssertedChildElement ("span", 1);
          lastSpan.AssertAttributeValueEquals ("class", "last");
        }
      }
    }

    private XmlNode GetAssertedTabList (XmlDocument document, bool withCssClass, bool isEmpty, int tabCount, bool isDesignMode)
    {
      var outerDiv = document.GetAssertedChildElement ("div", 0);
      outerDiv.AssertAttributeValueEquals ("class", withCssClass ? _webTabStrip.CssClass : _renderer.CssClassBase);
      outerDiv.AssertChildElementCount (2);

      var innerDiv = outerDiv.GetAssertedChildElement ("div", 0);
      innerDiv.AssertAttributeValueContains ("class", _renderer.CssClassTabsPane);
      if (isEmpty)
        innerDiv.AssertAttributeValueContains ("class", _renderer.CssClassTabsPaneEmpty);

      innerDiv.AssertChildElementCount (1);

      var clearingPaneDiv = outerDiv.GetAssertedChildElement ("div", 1);
      clearingPaneDiv.AssertAttributeValueContains ("class", _renderer.CssClassClearingPane);

      var list = innerDiv.GetAssertedChildElement ("ul", 0);
      if (isDesignMode)
      {
        list.AssertStyleAttribute ("list-style", "none");
        list.AssertStyleAttribute ("width", "100%");
        list.AssertStyleAttribute ("display", "inline");
      }
      list.AssertChildElementCount (tabCount);
      return list;
    }

    private void AssertItem (XmlNode item, IMenuTab webTab, bool isLastItem, bool isDesignMode)
    {
      if (isDesignMode)
      {
        item.AssertStyleAttribute ("float", "left");
        item.AssertStyleAttribute ("display", "block");
        item.AssertStyleAttribute ("white-space", "nowrap");
      }
      item.AssertChildElementCount (isLastItem ? 2 : 1);

      var wrapper = item.GetAssertedChildElement ("span", 0);
      wrapper.AssertAttributeValueEquals ("class", _renderer.CssClassTabWrapper);

      var separator = wrapper.GetAssertedChildElement ("span", 0);
      separator.AssertAttributeValueEquals ("class", _renderer.CssClassSeparator);
      separator.AssertChildElementCount (1);

      var empty = separator.GetAssertedChildElement ("span", 0);
      empty.AssertChildElementCount (0);

      var tab = wrapper.GetAssertedChildElement ("span", 1);
      tab.AssertAttributeValueEquals ("id", _webTabStrip.ClientID + "_" + webTab.ItemID);
      tab.AssertAttributeValueContains ("class", webTab.IsSelected ? _renderer.CssClassTabSelected : _renderer.CssClassTab);
      if (!webTab.EvaluateEnabled())
        tab.AssertAttributeValueContains ("class", _renderer.CssClassDisabled);
      var link = tab.GetAssertedChildElement ("a", 0);
      link.AssertAttributeValueEquals ("id", _webTabStrip.ClientID + "_" + webTab.ItemID + "_Command");

      bool isDisabledBySelection = webTab.IsSelected && !_webTabStrip.EnableSelectedTab;
      if (webTab.EvaluateEnabled())
      {
        link.AssertAttributeValueEquals ("href", "#");
      }
      if (webTab.EvaluateEnabled() && !isDisabledBySelection)
      {
        string clickScript = _pageStub.ClientScript.GetPostBackClientHyperlink (_webTabStrip, webTab.ItemID);
        link.AssertAttributeValueEquals ("onclick", clickScript);
      }

      var textWithHotkey = HotkeyParser.Parse (webTab.Text);
      if (webTab.EvaluateEnabled() && textWithHotkey.Hotkey.HasValue)
      {
        var accessKey = new NoneHotkeyFormatter().FormatHotkey (textWithHotkey);
        link.AssertAttributeValueEquals ("accesskey", accessKey);
      }

      AssertAnchor (link, webTab);
    }

    private void AssertAnchor (XmlNode link, IMenuTab tab)
    {
      var anchorBody = link.GetAssertedChildElement ("span", 0);
      anchorBody.AssertAttributeValueEquals ("class", _renderer.CssClassTabAnchorBody);

      var textWithHotkey = HotkeyParser.Parse (tab.Text);
      string text = textWithHotkey.Text ?? string.Empty;
      var hasIcon = tab.Icon != null && !string.IsNullOrEmpty (tab.Icon.Url);
      if (hasIcon)
      {
        string url = tab.Icon.Url.TrimStart ('~');
        string alt = tab.Icon.AlternateText ?? string.Empty;
        text = HtmlHelper.WhiteSpace + text;
        
        var image = anchorBody.GetAssertedChildElement ("img", 0);
        image.AssertAttributeValueEquals ("src", url);
        image.AssertAttributeValueEquals ("alt", alt);
      }

      if (string.IsNullOrEmpty (text))
        text = HtmlHelper.WhiteSpace;

      anchorBody.AssertTextNode (text, hasIcon ? 1 : 0);
    }

    private WebTabRenderer CreateWebTabRenderer ()
    {
      return CreateWebTabRenderer (RenderingFeatures.Default);
    }

    private WebTabRenderer CreateWebTabRenderer (IRenderingFeatures renderingFeatures)
    {
      return new WebTabRenderer (new NoneHotkeyFormatter(), renderingFeatures);
    }
  }
}