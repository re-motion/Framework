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
using System.Globalization;
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  [SetUICulture("en-US")]
  public class BocListNavigationBlockRendererTest : BocListRendererTestBase
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private const string c_pageLabelText = "Page";
    private const string c_totalPageCountText = "of";

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      _bocListCssClassDefinition = new BocListCssClassDefinition();

      List.Setup(mock => mock.HasNavigator).Returns(true);
    }

    [Test]
    public void RenderOnlyPage ()
    {
      var currentPageIndex = 0;
      var totalPageCount = 1;

      List.Setup(mock => mock.CurrentPageIndex).Returns(currentPageIndex);
      List.Setup(mock => mock.PageCount).Returns(totalPageCount);

      var renderer = new BocListNavigationBlockRenderer(
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.Render(CreateRenderingContext());

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement(document, "div", 0);
      Html.AssertAttribute(div, "class", _bocListCssClassDefinition.Navigator);

      var manualInput = Html.GetAssertedChildElement(div, "span", 0);
      Html.AssertTextNode(manualInput, string.Format("{0} 1 {1} 1", c_pageLabelText, c_totalPageCountText), 0);

      var firstIcon = Html.GetAssertedChildElement(div, "a", 1);
      AssertInactiveIcon(firstIcon, "First");

      var previousIcon = Html.GetAssertedChildElement(div, "a", 2);
      AssertInactiveIcon(previousIcon, "Previous");

      var nextIcon = Html.GetAssertedChildElement(div, "a", 3);
      AssertInactiveIcon(nextIcon, "Next");

      var lastIcon = Html.GetAssertedChildElement(div, "a", 4);
      AssertInactiveIcon(lastIcon, "Last");

      Html.AssertChildElementCount(div, 5);
    }

    [Test]
    public void RenderFirstPage ()
    {
      var currentPageIndex = 0;
      var totalPageCount = 2;

      List.Setup(mock => mock.CurrentPageIndex).Returns(currentPageIndex);
      List.Setup(mock => mock.PageCount).Returns(totalPageCount);

      var renderer = new BocListNavigationBlockRenderer(
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.Render(CreateRenderingContext());

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement(document, "div", 0);
      Html.AssertAttribute(div, "class", _bocListCssClassDefinition.Navigator);

      var manualInput = Html.GetAssertedChildElement(div, "span", 0);
      AssertManualInputArea(manualInput, currentPageIndex, totalPageCount);

      var firstIcon = Html.GetAssertedChildElement(div, "a", 1);
      AssertInactiveIcon(firstIcon, "First");

      var previousIcon = Html.GetAssertedChildElement(div, "a", 2);
      AssertInactiveIcon(previousIcon, "Previous");

      var nextIcon = Html.GetAssertedChildElement(div, "a", 3);
      AssertActiveIcon(nextIcon, "Next", 1);

      var lastIcon = Html.GetAssertedChildElement(div, "a", 4);
      AssertActiveIcon(lastIcon, "Last", 1);

      var pageIndexField = Html.GetAssertedChildElement(div, "input", 5);
      AssertPageIndexHiddenField(pageIndexField, currentPageIndex);
    }

    [Test]
    public void RenderLastPage ()
    {
      var currentPageIndex = 1;
      var totalPageCount = 2;

      List.Setup(mock => mock.CurrentPageIndex).Returns(currentPageIndex);
      List.Setup(mock => mock.PageCount).Returns(totalPageCount);

      var renderer = new BocListNavigationBlockRenderer(
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.Render(CreateRenderingContext());

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement(document, "div", 0);
      Html.AssertAttribute(div, "class", _bocListCssClassDefinition.Navigator);

      var manualInput = Html.GetAssertedChildElement(div, "span", 0);
      AssertManualInputArea(manualInput, currentPageIndex, totalPageCount);

      var firstIcon = Html.GetAssertedChildElement(div, "a", 1);
      AssertActiveIcon(firstIcon, "First", 0);

      var previousIcon = Html.GetAssertedChildElement(div, "a", 2);
      AssertActiveIcon(previousIcon, "Previous", 0);

      var nextIcon = Html.GetAssertedChildElement(div, "a", 3);
      AssertInactiveIcon(nextIcon, "Next");

      var lastIcon = Html.GetAssertedChildElement(div, "a", 4);
      AssertInactiveIcon(lastIcon, "Last");

      var pageIndexField = Html.GetAssertedChildElement(div, "input", 5);
      AssertPageIndexHiddenField(pageIndexField, currentPageIndex);
    }

    [Test]
    public void RenderMiddlePage ()
    {
      var currentPageIndex = 3;
      var totalPageCount = 7;

      List.Setup(mock => mock.CurrentPageIndex).Returns(currentPageIndex);
      List.Setup(mock => mock.PageCount).Returns(totalPageCount);

      var renderer = new BocListNavigationBlockRenderer(
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.Render(CreateRenderingContext());

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement(document, "div", 0);
      Html.AssertAttribute(div, "class", _bocListCssClassDefinition.Navigator);

      var manualInput = Html.GetAssertedChildElement(div, "span", 0);
      AssertManualInputArea(manualInput, currentPageIndex, totalPageCount);

      var firstIcon = Html.GetAssertedChildElement(div, "a", 1);
      AssertActiveIcon(firstIcon, "First", 0);

      var previousIcon = Html.GetAssertedChildElement(div, "a", 2);
      AssertActiveIcon(previousIcon, "Previous", 2);

      var nextIcon = Html.GetAssertedChildElement(div, "a", 3);
      AssertActiveIcon(nextIcon, "Next", 4);

      var lastIcon = Html.GetAssertedChildElement(div, "a", 4);
      AssertActiveIcon(lastIcon, "Last", 6);

      var pageIndexField = Html.GetAssertedChildElement(div, "input", 5);
      AssertPageIndexHiddenField(pageIndexField, currentPageIndex);
    }

    [Test]
    public void TestDiagnosticMetadataRendering ()
    {
      var currentPageIndex = 3;
      var totalPageCount = 7;

      List.Setup(mock => mock.CurrentPageIndex).Returns(currentPageIndex);
      List.Setup(mock => mock.PageCount).Returns(totalPageCount);

      var renderer = new BocListNavigationBlockRenderer(
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.Render(CreateRenderingContext());

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement(document, "div", 0);
      Html.AssertAttribute(div, DiagnosticMetadataAttributesForObjectBinding.BocListNumberOfPages, totalPageCount.ToString());
      Html.AssertAttribute(div, DiagnosticMetadataAttributesForObjectBinding.BocListCurrentPageNumber, (currentPageIndex + 1).ToString());
    }

    private void AssertManualInputArea (XmlNode manualInputArea, int currentPageIndex, int totalPageCount)
    {
      var inputID = List.Object.GetCurrentPageControlName().Replace('$', '_') + "_TextBox";

      var pageInputLabel = Html.GetAssertedChildElement(manualInputArea, "label", 0);
      Html.AssertTextNode(pageInputLabel, c_pageLabelText, 0);
      Html.AssertAttribute(pageInputLabel, "for", inputID);

      var pageNumberField = Html.GetAssertedChildElement(manualInputArea, "input", 1);
      Html.AssertAttribute(pageNumberField, "value", (currentPageIndex + 1).ToString(CultureInfo.InvariantCulture));
      Html.AssertAttribute(pageNumberField, "id", inputID);
      Html.AssertNoAttribute(pageNumberField, "name");
      Html.AssertAttribute(pageNumberField, "type", "text");
      Html.AssertAttribute(
          pageNumberField,
          "maxlength",
          totalPageCount.ToString(CultureInfo.InvariantCulture).Length.ToString(CultureInfo.InvariantCulture));
      Html.AssertAttribute(
          pageNumberField,
          "size",
          totalPageCount.ToString(CultureInfo.InvariantCulture).Length.ToString(CultureInfo.InvariantCulture));
      Html.AssertStyleAttribute(
          pageNumberField,
          "--size",
          totalPageCount.ToString(CultureInfo.InvariantCulture).Length.ToString(CultureInfo.InvariantCulture));

      Html.AssertTextNode(manualInputArea, c_totalPageCountText + " " + totalPageCount, 2);
    }

    private void AssertPageIndexHiddenField (XmlNode pageIndexField, int currentPageIndex)
    {
      var inputID = List.Object.GetCurrentPageControlName().Replace('$', '_');

      Html.AssertAttribute(pageIndexField, "value", (currentPageIndex).ToString(CultureInfo.InvariantCulture));
      Html.AssertAttribute(pageIndexField, "id", inputID);
      Html.AssertAttribute(pageIndexField, "name", List.Object.GetCurrentPageControlName());
      Html.AssertAttribute(pageIndexField, "type", "hidden");
    }

    private void AssertActiveIcon (XmlNode link, string command, int pageIndex)
    {
      Html.AssertAttribute(link, "id", List.Object.ClientID + "_Navigation_" + command);
      Html.AssertAttribute(
          link,
          "onclick",
          string.Format(
              "let element = document.getElementById('CurrentPageControl_UniqueID');element.value = {0};element.dispatchEvent(new Event('change'));return false;",
              pageIndex));
      Html.AssertAttribute(link, "href", "fakeFallbackUrl");

      var icon = Html.GetAssertedChildElement(link, "img", 0);
      Html.AssertAttribute(icon, "src", string.Format("/sprite.svg#Move{0}", command), HtmlHelperBase.AttributeValueCompareMode.Contains);
    }

    private void AssertInactiveIcon (XmlNode link, string command)
    {
      Html.AssertAttribute(link, "id", List.Object.ClientID + "_Navigation_" + command);
      Html.AssertNoAttribute(link, "onclick");
      Html.AssertNoAttribute(link, "href");

      var icon = Html.GetAssertedChildElement(link, "img", 0);
      Html.AssertAttribute(icon, "src", string.Format("/sprite.svg#Move{0}Inactive", command), HtmlHelperBase.AttributeValueCompareMode.Contains);
    }

    private BocListRenderingContext CreateRenderingContext ()
    {
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      return new BocListRenderingContext(HttpContext, Html.Writer, List.Object, businessObjectWebServiceContext, new BocColumnRenderer[0]);
    }
  }
}
