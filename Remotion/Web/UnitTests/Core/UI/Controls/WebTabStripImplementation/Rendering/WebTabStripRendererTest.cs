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
using Moq;
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
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core.UI.Controls.WebTabStripImplementation.Rendering
{
  [TestFixture]
  public class WebTabStripRendererTest : RendererTestBase
  {
    private Mock<IWebTabStrip> _webTabStrip;
    private WebTabStripRenderer _renderer;
    private Mock<IPage> _pageStub;
    private Mock<IMenuTab> _tab0, _tab1, _tab2, _tab3, _tab4;
    private Mock<HttpContextBase> _httpContextStub;
    private HtmlHelper _htmlHelper;
    private WebTabStyle _style;
    private readonly UnderscoreHotkeyFormatter _hotkeyFormatter = new UnderscoreHotkeyFormatter();
    private readonly NoneHotkeyFormatter _noneHotkeyFormatter = new NoneHotkeyFormatter();

    [SetUp]
    public void SetUp ()
    {
      _htmlHelper = new HtmlHelper();
      _httpContextStub = new Mock<HttpContextBase>();

      _webTabStrip = new Mock<IWebTabStrip>();
      _webTabStrip.Setup(stub => stub.ClientID).Returns("WebTabStrip");
      _webTabStrip.Setup(stub => stub.ControlType).Returns("WebTabStrip");
      _webTabStrip.Setup(stub => stub.ResolveClientUrl(It.IsAny<string>())).Returns((string relativeUrl) => relativeUrl.TrimStart('~'));

      _pageStub = new Mock<IPage>();
      var clientScriptStub = new Mock<IClientScriptManager>();
      clientScriptStub.Setup(stub => stub.GetPostBackClientHyperlink(It.IsAny<IControl>(), It.IsAny<string>())).Returns("PostBackHyperlink");
      _pageStub.Setup(stub => stub.ClientScript).Returns(clientScriptStub.Object);

      _webTabStrip.Setup(stub => stub.Page).Returns(_pageStub.Object);
      _webTabStrip.Setup(stub => stub.Tabs).Returns(new WebTabCollection(_webTabStrip.Object));
      _webTabStrip.Setup(stub => stub.GetVisibleTabs()).Returns(new List<IWebTab>());

      StateBag stateBag = new StateBag();
      _webTabStrip.Setup(stub => stub.Attributes).Returns(new AttributeCollection(stateBag));
      _webTabStrip.Setup(stub => stub.SelectedTabStyle).Returns(new WebTabStyle());
      _webTabStrip.Setup(stub => stub.TabStyle).Returns(new WebTabStyle());
      _webTabStrip.Setup(stub => stub.ControlStyle).Returns(new Style(stateBag));

      _style = new WebTabStyle();
    }

    [Test]
    public void RenderEmptyStrip ()
    {
      var renderingContext = new WebTabStripRenderingContext(
          _httpContextStub.Object,
          _htmlHelper.Writer,
          _webTabStrip.Object,
          new WebTabRendererAdapter[0]);
      AssertControl(false, true, false, 0, renderingContext);
    }

    [Test]
    public void RenderPopulatedStrip ()
    {
      PopulateTabStrip();
      _webTabStrip.Object.GetVisibleTabs().RemoveAt(4);

      _tab0.Setup(stub => stub.EvaluateEnabled()).Returns(true);

      var renderingContext= new WebTabStripRenderingContext(
          _httpContextStub.Object,
          _htmlHelper.Writer,
          _webTabStrip.Object,
          new[]
          {
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab0.Object, false, true, _style),
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab1.Object, false, true, _style),
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab2.Object, false, true, _style),
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab3.Object, true, true, _style)
          });

      AssertControl(false, false, false, 4, renderingContext);
    }

    [Test]
    public void RenderPopulatedStripWithCssClass ()
    {
      _webTabStrip.Object.CssClass = "SomeCssClass";
      PopulateTabStrip();
      _webTabStrip.Object.GetVisibleTabs().RemoveAt(4);

      _tab0.Setup(stub => stub.EvaluateEnabled()).Returns(true);

      var renderingContext = new WebTabStripRenderingContext(
          _httpContextStub.Object,
          _htmlHelper.Writer,
          _webTabStrip.Object,
          new[]
          {
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab0.Object, false, true, _style),
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab1.Object, false, true, _style),
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab2.Object, false, true, _style),
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab3.Object, true, true, _style)
          });

      AssertControl(true, false, false, 4, renderingContext);
    }

    [Test]
    public void RenderPopulatedStripWithEnableSelectedTab ()
    {
      _webTabStrip.Setup(stub => stub.EnableSelectedTab).Returns(true);

      PopulateTabStrip();
      _webTabStrip.Object.GetVisibleTabs().RemoveAt(4);

      _tab0.Setup(stub => stub.EvaluateEnabled()).Returns(true);

      var renderingContext = new WebTabStripRenderingContext(
          _httpContextStub.Object,
          _htmlHelper.Writer,
          _webTabStrip.Object,
          new[]
          {
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab0.Object, false, true, _style),
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab1.Object, false, true, _style),
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab2.Object, false, true, _style),
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab3.Object, true, true, _style)
          });

      AssertControl(false, false, false, 4, renderingContext);
    }

    [Test]
    public void RenderPopulatedStripWithDisabledTab ()
    {
      PopulateTabStrip();
      _webTabStrip.Object.GetVisibleTabs().RemoveAt(4);

      var renderingContext = new WebTabStripRenderingContext(
          _httpContextStub.Object,
          _htmlHelper.Writer,
          _webTabStrip.Object,
          new[]
          {
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab0.Object, false, true, _style),
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab1.Object, false, true, _style),
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab2.Object, false, true, _style),
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab3.Object, true, true, _style)
          });

      AssertControl(false, false, false, 4, renderingContext);
    }

    [Test]
    public void RenderPopulatedStripWithInvisibleTab ()
    {
      PopulateTabStrip();
      _webTabStrip.Object.GetVisibleTabs().RemoveAt(2);
      _webTabStrip.Object.GetVisibleTabs().RemoveAt(3);

      var renderingContext = new WebTabStripRenderingContext(
          _httpContextStub.Object,
          _htmlHelper.Writer,
          _webTabStrip.Object,
          new[]
          {
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab0.Object, false, true, _style),
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab1.Object, false, true, _style),
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab3.Object, true, true, _style)
          });

      AssertControl(false, false, false, 3, renderingContext);
    }

    [Test]
    public void RenderPopulatedStripWithEncodedContent ()
    {
      PopulateTabStrip();
      _webTabStrip.Object.GetVisibleTabs().RemoveAt(0);
      _webTabStrip.Object.GetVisibleTabs().RemoveAt(0);
      _webTabStrip.Object.GetVisibleTabs().RemoveAt(0);
      _webTabStrip.Object.GetVisibleTabs().RemoveAt(0);

      var renderingContext = new WebTabStripRenderingContext(
          _httpContextStub.Object,
          _htmlHelper.Writer,
          _webTabStrip.Object,
          new[]
          {
              new WebTabRendererAdapter(CreateWebTabRenderer(), _tab4.Object, true, true, _style),
          });

      AssertControl(false, false, false, 1, renderingContext);
    }

    [Test]
    public void TestDiagnosticMetadataRendering ()
    {
      PopulateTabStrip();
      var renderingContext = new WebTabStripRenderingContext(
          _httpContextStub.Object,
          _htmlHelper.Writer,
          _webTabStrip.Object,
          new[]
          {
              new WebTabRendererAdapter(CreateWebTabRenderer(RenderingFeatures.WithDiagnosticMetadata), _tab0.Object, true, true, _style),
          });

      _renderer = new WebTabStripRenderer(new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);
      _renderer.Render(renderingContext);

      var document = _htmlHelper.GetResultDocument();
      var outerDiv = document.GetAssertedChildElement("div", 0);
      var innerDiv = outerDiv.GetAssertedChildElement("div", 0);
      outerDiv.AssertAttributeValueEquals(DiagnosticMetadataAttributes.ControlType, _webTabStrip.Object.ControlType);
      var list = innerDiv.GetAssertedChildElement("ul", 0);
      var item = list.GetAssertedChildElement("li", 0);
      var wrapper = item.GetAssertedChildElement("span", 0);
      var tab = wrapper.GetAssertedChildElement("span", 1);
      tab.AssertAttributeValueEquals(DiagnosticMetadataAttributes.ItemID, _tab0.Object.ItemID);
      tab.AssertAttributeValueEquals(DiagnosticMetadataAttributes.Content, HtmlUtility.ExtractPlainText(_noneHotkeyFormatter.GetFormattedText(_tab0.Object.Text)));
      tab.AssertAttributeValueEquals(DiagnosticMetadataAttributes.IsDisabled, (!_tab0.Object.EvaluateEnabled()).ToString().ToLower());
    }

    [Test]
    public void TestDiagnosticMetadataRenderingWithEncodedContent ()
    {
      PopulateTabStrip();
      var renderingContext = new WebTabStripRenderingContext(
          _httpContextStub.Object,
          _htmlHelper.Writer,
          _webTabStrip.Object,
          new[]
          {
              new WebTabRendererAdapter(CreateWebTabRenderer(RenderingFeatures.WithDiagnosticMetadata), _tab4.Object, true, true, _style),
          });

      _renderer = new WebTabStripRenderer(new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);
      _renderer.Render(renderingContext);

      var document = _htmlHelper.GetResultDocument();
      var outerDiv = document.GetAssertedChildElement("div", 0);
      var innerDiv = outerDiv.GetAssertedChildElement("div", 0);
      outerDiv.AssertAttributeValueEquals(DiagnosticMetadataAttributes.ControlType, _webTabStrip.Object.ControlType);
      var list = innerDiv.GetAssertedChildElement("ul", 0);
      var item = list.GetAssertedChildElement("li", 0);
      var wrapper = item.GetAssertedChildElement("span", 0);
      var tab = wrapper.GetAssertedChildElement("span", 1);
      tab.AssertAttributeValueEquals(DiagnosticMetadataAttributes.ItemID, _tab4.Object.ItemID);
      if (_tab4.Object.Text.Type == WebStringType.Encoded)
        tab.AssertAttributeValueEquals(DiagnosticMetadataAttributes.Content, HtmlUtility.ExtractPlainText(_tab4.Object.Text));
      else
        tab.AssertAttributeValueEquals(DiagnosticMetadataAttributes.Content, _tab4.Object.Text.GetValue());
      tab.AssertAttributeValueEquals(DiagnosticMetadataAttributes.IsDisabled, (!_tab4.Object.EvaluateEnabled()).ToString().ToLower());
    }

    [Test]
    public void RenderWebStrings ()
    {
      _tab0 = new Mock<IMenuTab>();
      _tab0.Setup(_ => _.ItemID).Returns("MyWebTab");
      _tab0.Setup(_ => _.Text).Returns(WebString.CreateFromText("Test\nTest"));
      _webTabStrip.Object.GetVisibleTabs().Add(_tab0.Object);
      _renderer = new WebTabStripRenderer(new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);
      var renderingContext = new WebTabStripRenderingContext(
          _httpContextStub.Object,
          _htmlHelper.Writer,
          _webTabStrip.Object,
          new[]
          {
              new WebTabRendererAdapter(CreateWebTabRenderer(RenderingFeatures.WithDiagnosticMetadata), _tab0.Object, true, true, _style),
          });
      _renderer.Render(renderingContext);

      var document = _htmlHelper.GetResultDocument();
      var webTab = document.GetAssertedElementByID(_webTabStrip.Object.ClientID + "_MyWebTab");
      webTab.AssertAttributeValueEquals("data-content", "Test\nTest");
    }

    private void PopulateTabStrip ()
    {
      _tab0 = new Mock<IMenuTab>();
      _tab0.Setup(stub => stub.ItemID).Returns("Tab0");
      _tab0.Setup(stub => stub.Text).Returns(WebString.CreateFromText("First &Tab"));
      _tab0.Setup(stub => stub.Icon).Returns(new IconInfo());
      _tab0.Setup(stub => stub.EvaluateEnabled()).Returns(true);
      _tab0.Setup(stub => stub.GetPostBackClientEvent()).Returns(_pageStub.Object.ClientScript.GetPostBackClientHyperlink(_webTabStrip.Object, _tab0.Object.ItemID));
      _tab0.Setup(stub => stub.GetActiveTab()).Returns(_tab0.Object);
      _tab0.Setup(stub => stub.Command).Returns(new NavigationCommand(CommandType.Event));
      _tab0.Setup(stub => stub.IsSelected).Returns(true);
      _tab0.Setup(stub => stub.GetRenderer()).Returns(CreateWebTabRenderer());

      _tab1 = new Mock<IMenuTab>();
      _tab1.Setup(stub => stub.ItemID).Returns("Tab1");
      _tab1.Setup(stub => stub.Text).Returns(WebString.CreateFromText("Second Tab"));
      _tab1.Setup(stub => stub.Icon).Returns(new IconInfo("/myImageUrl"));
      _tab1.Setup(stub => stub.EvaluateEnabled()).Returns(true);
      _tab1.Setup(stub => stub.GetPostBackClientEvent()).Returns(_pageStub.Object.ClientScript.GetPostBackClientHyperlink(_webTabStrip.Object, _tab1.Object.ItemID));
      _tab1.Setup(stub => stub.GetActiveTab()).Returns(_tab1.Object);
      _tab1.Setup(stub => stub.Command).Returns(new NavigationCommand(CommandType.Event));
      _tab1.Setup(stub => stub.GetRenderer()).Returns(CreateWebTabRenderer());

      _tab2 = new Mock<IMenuTab>();
      _tab2.Setup(stub => stub.ItemID).Returns("Tab2");
      _tab2.Setup(stub => stub.Text).Returns(WebString.CreateFromText("Third Tab"));
      _tab2.Setup(stub => stub.Icon).Returns(new IconInfo());
      _tab2.Setup(stub => stub.EvaluateEnabled()).Returns(true);
      _tab2.Setup(stub => stub.GetPostBackClientEvent()).Returns(_pageStub.Object.ClientScript.GetPostBackClientHyperlink(_webTabStrip.Object, _tab2.Object.ItemID));
      _tab2.Setup(stub => stub.GetActiveTab()).Returns(_tab2.Object);
      _tab2.Setup(stub => stub.Command).Returns(new NavigationCommand(CommandType.Event));
      _tab2.Setup(stub => stub.GetRenderer()).Returns(CreateWebTabRenderer());

      _tab3 = new Mock<IMenuTab>();
      _tab3.Setup(stub => stub.ItemID).Returns("Tab3");
      _tab3.Setup(stub => stub.Text).Returns(WebString.CreateFromText((string)null));
      _tab3.Setup(stub => stub.Icon).Returns(new IconInfo());
      _tab3.Setup(stub => stub.EvaluateEnabled()).Returns(true);
      _tab3.Setup(stub => stub.GetPostBackClientEvent()).Returns(_pageStub.Object.ClientScript.GetPostBackClientHyperlink(_webTabStrip.Object, _tab3.Object.ItemID));
      _tab3.Setup(stub => stub.GetActiveTab()).Returns(_tab3.Object);
      _tab3.Setup(stub => stub.Command).Returns(new NavigationCommand(CommandType.Event));
      _tab3.Setup(stub => stub.GetRenderer()).Returns(CreateWebTabRenderer());

      _tab4 = new Mock<IMenuTab>();
      _tab4.Setup(stub => stub.ItemID).Returns("Tab4");
      _tab4.Setup(stub => stub.Text).Returns(WebString.CreateFromHtml("<span>AAAAAAAAA</span>"));
      _tab4.Setup(stub => stub.Icon).Returns(new IconInfo());
      _tab4.Setup(stub => stub.EvaluateEnabled()).Returns(true);
      _tab4.Setup(stub => stub.GetPostBackClientEvent()).Returns(_pageStub.Object.ClientScript.GetPostBackClientHyperlink(_webTabStrip.Object, _tab4.Object.ItemID));
      _tab4.Setup(stub => stub.GetActiveTab()).Returns(_tab4.Object);
      _tab4.Setup(stub => stub.Command).Returns(new NavigationCommand(CommandType.Event));
      _tab4.Setup(stub => stub.GetRenderer()).Returns(CreateWebTabRenderer());

      _webTabStrip.Object.GetVisibleTabs().Add(_tab0.Object);
      _webTabStrip.Object.GetVisibleTabs().Add(_tab1.Object);
      _webTabStrip.Object.GetVisibleTabs().Add(_tab2.Object);
      _webTabStrip.Object.GetVisibleTabs().Add(_tab3.Object);
      _webTabStrip.Object.GetVisibleTabs().Add(_tab4.Object);
    }

    private void AssertControl (bool withCssClass, bool isEmpty, bool isDesignMode, int tabCount, WebTabStripRenderingContext renderingContext)
    {
      _renderer = new WebTabStripRenderer(new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.Default);
      _renderer.Render(renderingContext);

      var document = _htmlHelper.GetResultDocument();
      XmlNode list = GetAssertedTabList(document, withCssClass, isEmpty, tabCount, isDesignMode);
      AssertTabs(list, isDesignMode);
    }

    private void AssertTabs (XmlNode list, bool isDesignMode)
    {
      var tabs = _webTabStrip.Object.GetVisibleTabs();
      int itemCount = list.ChildNodes.Count;
      if (!isDesignMode)
        Assert.That(itemCount, Is.EqualTo(tabs.Count));

      for (int i = 0; i < itemCount; i++)
      {
        IMenuTab tab = (IMenuTab)tabs[i];
        bool isLastItem = (i == itemCount - 1);

        var item = list.GetAssertedChildElement("li", i);
        AssertItem(item, tab, isLastItem, isDesignMode);

        if (isLastItem)
        {
          var lastSpan = item.GetAssertedChildElement("span", 1);
          lastSpan.AssertAttributeValueEquals("class", "last");
        }
      }
    }

    private XmlNode GetAssertedTabList (XmlDocument document, bool withCssClass, bool isEmpty, int tabCount, bool isDesignMode)
    {
      var outerDiv = document.GetAssertedChildElement("div", 0);
      outerDiv.AssertAttributeValueEquals("class", withCssClass ? _webTabStrip.Object.CssClass : _renderer.CssClassBase);
      outerDiv.AssertChildElementCount(2);

      var innerDiv = outerDiv.GetAssertedChildElement("div", 0);
      innerDiv.AssertAttributeValueContains("class", _renderer.CssClassTabsPane);
      if (isEmpty)
        innerDiv.AssertAttributeValueContains("class", _renderer.CssClassTabsPaneEmpty);
      innerDiv.AssertAttributeValueEquals("role", "tablist");

      innerDiv.AssertChildElementCount(1);

      var clearingPaneDiv = outerDiv.GetAssertedChildElement("div", 1);
      clearingPaneDiv.AssertAttributeValueContains("class", _renderer.CssClassClearingPane);

      var list = innerDiv.GetAssertedChildElement("ul", 0);
      list.AssertAttributeValueEquals("role", "none");
      if (isDesignMode)
      {
        list.AssertStyleAttribute("list-style", "none");
        list.AssertStyleAttribute("width", "100%");
        list.AssertStyleAttribute("display", "inline");
      }
      list.AssertChildElementCount(tabCount);
      return list;
    }

    private void AssertItem (XmlNode item, IMenuTab webTab, bool isLastItem, bool isDesignMode)
    {
      item.AssertAttributeValueEquals("role", "none");
      if (isDesignMode)
      {
        item.AssertStyleAttribute("float", "left");
        item.AssertStyleAttribute("display", "block");
        item.AssertStyleAttribute("white-space", "nowrap");
      }
      item.AssertChildElementCount(isLastItem ? 2 : 1);

      var wrapper = item.GetAssertedChildElement("span", 0);
      wrapper.AssertAttributeValueEquals("class", _renderer.CssClassTabWrapper);

      var separator = wrapper.GetAssertedChildElement("span", 0);
      separator.AssertAttributeValueEquals("class", _renderer.CssClassSeparator);
      separator.AssertChildElementCount(1);

      var empty = separator.GetAssertedChildElement("span", 0);
      empty.AssertChildElementCount(0);

      var tab = wrapper.GetAssertedChildElement("span", 1);
      tab.AssertAttributeValueEquals("id", _webTabStrip.Object.ClientID + "_" + webTab.ItemID);
      tab.AssertAttributeValueContains("class", webTab.IsSelected ? _renderer.CssClassTabSelected : _renderer.CssClassTab);
      tab.AssertAttributeValueEquals("role", "none");
      if (!webTab.EvaluateEnabled())
        tab.AssertAttributeValueContains("class", _renderer.CssClassDisabled);
      var link = tab.GetAssertedChildElement("a", 0);
      link.AssertAttributeValueEquals("id", _webTabStrip.Object.ClientID + "_" + webTab.ItemID + "_Command");
      link.AssertAttributeValueEquals("role", "tab");

      if (webTab.IsSelected)
      {
        link.AssertAttributeValueEquals("tabindex", "0");
        link.AssertAttributeValueEquals("aria-selected", "true");
        //link.AssertAttributeValueEquals ("aria-controls", XX);
      }
      else
      {
        link.AssertAttributeValueEquals("tabindex", "-1");
        link.AssertAttributeValueEquals("aria-selected", "false");
        link.AssertNoAttribute("aria-controls");
      }

      // Currently, no test case exists for disabled tabs.
      link.AssertNoAttribute("aria-disabled");

      bool isDisabledBySelection = webTab.IsSelected && !_webTabStrip.Object.EnableSelectedTab;
      if (webTab.EvaluateEnabled())
      {
        link.AssertAttributeValueEquals("href", "#");
      }
      if (webTab.EvaluateEnabled() && !isDisabledBySelection)
      {
        string clickScript = _pageStub.Object.ClientScript.GetPostBackClientHyperlink(_webTabStrip.Object, webTab.ItemID) + ";return false;";
        link.AssertAttributeValueEquals("onclick", clickScript);
      }

      AssertAnchor(link, webTab);
    }

    private void AssertAnchor (XmlNode link, IMenuTab tab)
    {
      var anchorBody = link.GetAssertedChildElement("span", 0);
      anchorBody.AssertAttributeValueEquals("class", _renderer.CssClassTabAnchorBody);

      var text = tab.Text;
      var hasIcon = tab.Icon != null && !string.IsNullOrEmpty(tab.Icon.Url);
      var childIndex = 0;
      if (hasIcon)
      {
        string url = tab.Icon.Url.TrimStart('~');
        string alt = tab.Icon.AlternateText ?? string.Empty;

        var image = anchorBody.GetAssertedChildElement("img", childIndex);
        image.AssertAttributeValueEquals("src", url);
        image.AssertAttributeValueEquals("alt", alt);

        childIndex++;
      }

      var hotkey = _hotkeyFormatter.GetAccessKey(text);
      if (hotkey.HasValue)
        link.AssertAttributeValueEquals("accesskey", hotkey.ToString());

      if (!text.IsEmpty)
      {
        if (text.Type == WebStringType.PlainText)
        {
          var textString = _hotkeyFormatter.GetFormattedText(text);
          var textBody = anchorBody.GetAssertedChildElement("span", childIndex);
          Assert.That(textBody.InnerXml, Is.EqualTo(textString.GetValue()));
        }
        else
        {
          var textBody = anchorBody.GetAssertedChildElement("span", childIndex);
          var node = textBody.GetAssertedChildElement("span", 0);
          Assert.That(node.OuterXml, Is.EqualTo(text.GetValue()));
        }
      }
    }

    private WebTabRenderer CreateWebTabRenderer ()
    {
      return CreateWebTabRenderer(RenderingFeatures.Default);
    }

    private WebTabRenderer CreateWebTabRenderer (IRenderingFeatures renderingFeatures)
    {
      return new WebTabRenderer(_hotkeyFormatter, renderingFeatures);
    }
  }
}
