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
using System.Web.UI;
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ServiceLocation;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocColumnRendererBaseTest : BocListRendererTestBase
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private const string c_whitespace = "&nbsp;";
    private const string c_screenReaderText = "screenReaderText";
    private const string c_columnCssClass = "cssClassColumn";

    private BocSimpleColumnDefinition Column { get; set; }

    [SetUp]
    public void SetUp ()
    {
      Column = new BocSimpleColumnDefinition();
      Column.ColumnTitle = WebString.CreateFromText("TestColumn1");
      Column.CssClass = c_columnCssClass;

      Initialize();

      var editModeController = new Mock<IEditModeController>();
      editModeController
          .Setup(mock => mock.RenderTitleCellMarkers(Html.Writer, Column, 0))
          .Callback((HtmlTextWriter writer, BocColumnDefinition column, int columnIndex) => writer.Write(string.Empty));

      List.Setup(mock => mock.EditModeController).Returns(editModeController.Object);

      List.Setup(mock => mock.IsClientSideSortingEnabled).Returns(true);
      List.Setup(mock => mock.IsShowSortingOrderEnabled).Returns(true);

      _bocListCssClassDefinition = new BocListCssClassDefinition();
    }

    [Test]
    public void IsNull ()
    {
      Assert.That(Column.GetRenderer(DefaultServiceLocator.Create()).IsNull, Is.False);
    }

    [Test]
    public void RenderTitleCellAscendingZero ()
    {
      RenderTitleCell(SortingDirection.Ascending, 0, "sprite.svg#SortAscending", "Sorted ascending");
    }

    [Test]
    public void RenderTitleCellDescendingZero ()
    {
      RenderTitleCell(SortingDirection.Descending, 0, "sprite.svg#SortDescending", "Sorted descending");
    }

    [Test]
    public void RenderTitleCellAscendingThree ()
    {
      RenderTitleCell(SortingDirection.Ascending, 3, "sprite.svg#SortAscending", "Sorted ascending");
    }

    [Test]
    public void RenderTitleCellDescendingFour ()
    {
      RenderTitleCell(SortingDirection.Descending, 4, "sprite.svg#SortDescending", "Sorted descending");
    }

    [Test]
    public void RenderTitleCellNoSorting ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      var renderingContext = CreateRenderingContext();
      renderer.RenderTitleCell(renderingContext, CreateBocTitleCellRenderArguments(cellID: "TheColumnID"));

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement(document, "th", 0);
      Html.AssertAttribute(th, "id", "TheColumnID");
      Html.AssertAttribute(th, "class", _bocListCssClassDefinition.TitleCell, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute(th, "class", c_columnCssClass, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute(th, "role", "columnheader");

      Assert.Less(0, th.ChildNodes.Count);
      var sortCommandLink = Html.GetAssertedChildElement(th, "a", 0);
      Html.AssertAttribute(sortCommandLink, "id", List.Object.ClientID + "_0_SortCommand");
      Html.AssertChildElementCount(sortCommandLink, 1);

      var titleSpan = Html.GetAssertedChildElement(sortCommandLink, "span", 0);
      Html.AssertTextNode(titleSpan, Column.ColumnTitleDisplayValue.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks), 0);
      Html.AssertChildElementCount(titleSpan, 0);
    }

    [Test]
    public void RenderColumnTitleWebString ()
    {
      Column.ColumnTitle = WebString.CreateFromText("Multiline\nColumnTitle");

      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      var renderingContext = CreateRenderingContext();
      renderer.RenderTitleCell(renderingContext, CreateBocTitleCellRenderArguments());

      var document = Html.GetResultDocument();
      var th = document.GetAssertedChildElement("th", 0);
      var sortCommandLink = Html.GetAssertedChildElement(th, "a", 0);
      var title = sortCommandLink.GetAssertedChildElement("span", 0);
      Assert.That(title.InnerXml, Is.EqualTo("Multiline<br />ColumnTitle"));
    }

    [Test]
    public void RenderSortableTitleCellWithHiddenTitleText ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      var renderingContext = CreateRenderingContext();
      renderingContext.ColumnDefinition.ColumnTitleStyle = BocColumnTitleStyle.None;

      renderer.RenderTitleCell(renderingContext, CreateBocTitleCellRenderArguments());

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement(document, "th", 0);

      Assert.Less(0, th.ChildNodes.Count);
      var sortCommandLink = Html.GetAssertedChildElement(th, "a", 0);
      Html.AssertChildElementCount(sortCommandLink, 1);

      var titleSpan = Html.GetAssertedChildElement(sortCommandLink, "span", 0);
      Html.AssertAttribute(titleSpan, "class", c_screenReaderText, HtmlHelperBase.AttributeValueCompareMode.Equal);
      Html.AssertTextNode(titleSpan, Column.ColumnTitleDisplayValue.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks), 0);
      Html.AssertChildElementCount(titleSpan, 0);

      Html.AssertTextNode(sortCommandLink, c_whitespace, 1);
    }

    [Test]
    public void RenderNonSortableTitleCellWithHiddenTitleText ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      var renderingContext = CreateRenderingContext();
      renderingContext.ColumnDefinition.ColumnTitleStyle = BocColumnTitleStyle.None;
      renderingContext.ColumnDefinition.IsSortable = false;

      renderer.RenderTitleCell(renderingContext, CreateBocTitleCellRenderArguments());

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement(document, "th", 0);

      Assert.Less(0, th.ChildNodes.Count);
      var cellBody = Html.GetAssertedChildElement(th, "span", 0);
      Html.AssertChildElementCount(cellBody, 1);

      var titleSpan = Html.GetAssertedChildElement(cellBody, "span", 0);
      Html.AssertAttribute(titleSpan, "class", c_screenReaderText, HtmlHelperBase.AttributeValueCompareMode.Equal);
      Html.AssertTextNode(titleSpan, Column.ColumnTitleDisplayValue.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks), 0);
      Html.AssertChildElementCount(titleSpan, 0);

      Html.AssertTextNode(cellBody, c_whitespace, 1);
    }

    [Test]
    public void RenderSortableTitleCellWithColumnTitleIcon ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      var renderingContext = CreateRenderingContext();
      renderingContext.ColumnDefinition.ColumnTitleStyle = BocColumnTitleStyle.Icon;
      renderingContext.ColumnDefinition.ColumnTitleIcon = new IconInfo("testUrl");

      renderer.RenderTitleCell(renderingContext, CreateBocTitleCellRenderArguments());

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement(document, "th", 0);

      Assert.Less(0, th.ChildNodes.Count);
      var sortCommandLink = Html.GetAssertedChildElement(th, "a", 0);
      Assert.That(sortCommandLink.ChildNodes.Count, Is.EqualTo(2));

      var img = Html.GetAssertedChildElement(sortCommandLink, "img", 0);
      Html.AssertAttribute(img, "src", "testUrl");

      var titleSpan = Html.GetAssertedChildElement(sortCommandLink, "span", 1);
      Html.AssertAttribute(titleSpan, "class", c_screenReaderText, HtmlHelperBase.AttributeValueCompareMode.Equal);
      Html.AssertTextNode(titleSpan, Column.ColumnTitleDisplayValue.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks), 0);
      Html.AssertChildElementCount(titleSpan, 0);
    }

    [Test]
    public void RenderSortableTitleCellWithColumnTitleIconAndText ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      var renderingContext = CreateRenderingContext();
      renderingContext.ColumnDefinition.ColumnTitleStyle = BocColumnTitleStyle.IconAndText;
      renderingContext.ColumnDefinition.ColumnTitleIcon = new IconInfo("testUrl");

      renderer.RenderTitleCell(renderingContext, CreateBocTitleCellRenderArguments());

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement(document, "th", 0);

      Assert.Less(0, th.ChildNodes.Count);
      var sortCommandLink = Html.GetAssertedChildElement(th, "a", 0);
      Assert.That(sortCommandLink.ChildNodes.Count, Is.EqualTo(2));

      var img = Html.GetAssertedChildElement(sortCommandLink, "img", 0);
      Html.AssertAttribute(img, "src", "testUrl");

      var titleSpan = Html.GetAssertedChildElement(sortCommandLink, "span", 1);
      Html.AssertNoAttribute(titleSpan, "class");
      Html.AssertTextNode(titleSpan, Column.ColumnTitleDisplayValue.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks), 0);
      Html.AssertChildElementCount(titleSpan, 0);
    }

    [Test]
    public void RenderSortableTitleCellWithColumnTitleTextAndInvalidColumnTitleIcon ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      var renderingContext = CreateRenderingContext();
      renderingContext.ColumnDefinition.ColumnTitleStyle = BocColumnTitleStyle.IconAndText;

      renderer.RenderTitleCell(renderingContext, CreateBocTitleCellRenderArguments());

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement(document, "th", 0);

      Assert.Less(0, th.ChildNodes.Count);
      var sortCommandLink = Html.GetAssertedChildElement(th, "a", 0);
      Assert.That(sortCommandLink.ChildNodes.Count, Is.EqualTo(1));

      var titleSpan = Html.GetAssertedChildElement(sortCommandLink, "span", 0);
      Html.AssertNoAttribute(titleSpan, "class");
      Html.AssertTextNode(titleSpan, Column.ColumnTitleDisplayValue.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks), 0);
      Html.AssertChildElementCount(titleSpan, 0);
    }

    [Test]
    public void TestRowHeaderRendering_HeaderColumn ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());

      var renderingContext = CreateRenderingContext();

      renderer.RenderDataCell(renderingContext, CreateBocDataCellRenderArguments(cellID: "HeaderCell"));

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "role", "rowheader");
      Html.AssertAttribute(td, "id", "HeaderCell");
    }

    [Test]
    public void TestRowHeaderRendering_DataColumnReferencingRowHeader ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());

      var renderingContext = CreateRenderingContext();

      var headerIDs = new[] { "c1", "c2" };
      renderer.RenderDataCell(renderingContext, CreateBocDataCellRenderArguments(headerIDs: headerIDs));

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "role", "cell");
      Html.AssertNoAttribute(td, "id");
      // Rendering the header IDs is problematic for split tables and doesn't help with columns to the left of the header column.
      // Therefor, the header IDs are simply not rendered in the first place.
      Html.AssertNoAttribute(td, "headers");
    }

    [Test]
    public void TestDiagnosticMetadataRendering ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());

      var renderingContext = CreateRenderingContext();

      renderer.RenderDataCell(renderingContext, CreateBocDataCellRenderArguments());

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, 7.ToString());
    }

    [Test]
    public void TestDiagnosticMetadataRenderingInTitle ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());

      Column.ItemID = "TestItemID";

      var renderingContext = CreateRenderingContext();

      renderer.RenderTitleCell(renderingContext, CreateBocTitleCellRenderArguments(orderIndex: 0));

      var document = Html.GetResultDocument();
      var th = Html.GetAssertedChildElement(document, "th", 0);
      Html.AssertAttribute(th, DiagnosticMetadataAttributes.ItemID, Column.ItemID);
      Html.AssertAttribute(th, DiagnosticMetadataAttributes.Content, Column.ColumnTitleDisplayValue.GetValue());
      Html.AssertAttribute(th, DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, 7.ToString());
      Html.AssertAttribute(th, DiagnosticMetadataAttributesForObjectBinding.BocListColumnHasContentAttribute, "true");
      Html.AssertAttribute(th, DiagnosticMetadataAttributesForObjectBinding.BocListColumnIsRowHeader, "false");
    }

    [Test]
    public void TestDiagnosticMetadataRenderingWithTitleIsEmpty ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      Column.ColumnTitle = WebString.Empty;
      var renderingContext = CreateRenderingContext();

      renderer.RenderTitleCell(renderingContext, CreateBocTitleCellRenderArguments(orderIndex: 0));

      var document = Html.GetResultDocument();
      var th = Html.GetAssertedChildElement(document, "th", 0);
      Assert.That(Column.ColumnTitleDisplayValue.ToString(), Is.Empty);
      Html.AssertAttribute(th, DiagnosticMetadataAttributes.Content, string.Empty);
    }

    [Test]
    public void TestDiagnosticMetadataRenderingWithColumnIsRowHeader ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      var renderingContext = CreateRenderingContext();

      renderer.RenderTitleCell(renderingContext, CreateBocTitleCellRenderArguments(isRowHeader: true));

      var document = Html.GetResultDocument();
      var th = Html.GetAssertedChildElement(document, "th", 0);
      Html.AssertAttribute(th, DiagnosticMetadataAttributesForObjectBinding.BocListColumnIsRowHeader, "true");
    }

    [Test]
    public void TestValidationFailureInCell ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      var renderingContext = CreateRenderingContext();

      renderingContext.Control.ValidationFailureRepository.AddValidationFailuresForDataCell(
          EventArgs.BusinessObject,
          Column,
          new[] { BusinessObjectValidationFailure.Create("error message"), });

      renderer.RenderDataCell(renderingContext, CreateBocDataCellRenderArguments(columnsWithValidationFailures: new [] { true }));

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertChildElementCount(td, 1);

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);
      Html.AssertChildElementCount(cellStructureDiv, 3);

      var iconSpan = Html.GetAssertedChildElement(cellStructureDiv, "span", 0);
      Html.AssertAttribute(iconSpan, "class", _bocListCssClassDefinition.ValidationErrorMarker);
      Html.AssertAttribute(iconSpan, "title", "error message\r\n");
      Html.AssertAttribute(iconSpan, "aria-hidden", "true");
      Html.AssertChildElementCount(iconSpan, 2);

      var jumpMarker = Html.GetAssertedChildElement(iconSpan, "span", 0);
      Html.AssertAttribute(jumpMarker, "id", "MyList_C6_R0_ValidationMarker");
      Html.AssertAttribute(jumpMarker, "tabindex", "-1");

      var img = Html.GetAssertedChildElement(iconSpan, "img", 1);
      Html.AssertAttribute(img, "src", "/fake/Remotion.Web/Themes/Fake/Image/sprite.svg#ValidationError");

      var div = Html.GetAssertedChildElement(cellStructureDiv, "div", 1);
      Html.AssertAttribute(div, "class", _bocListCssClassDefinition.Content);

      var screenReaderSpan = Html.GetAssertedChildElement(cellStructureDiv, "div", 2);
      Html.AssertAttribute(screenReaderSpan, "class", _bocListCssClassDefinition.CssClassScreenReaderText);
      Html.AssertChildElementCount(screenReaderSpan, 1);

      var ul = Html.GetAssertedChildElement(screenReaderSpan, "ul", 0);
      Html.AssertAttribute(ul, "aria-label", "Invalid input in current cell");
      Html.AssertChildElementCount(ul, 1);

      var li = Html.GetAssertedChildElement(ul, "li", 0);
      Html.AssertChildElementCount(li, 1);

      var errorMessageSpan = Html.GetAssertedChildElement(li, "span", 0);
      Assert.That(errorMessageSpan.InnerText, Is.EqualTo("error message"));
    }

    [Test]
    public void TestValidationFailureInOtherCell ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      var renderingContext = CreateRenderingContext();

      renderer.RenderDataCell(renderingContext, CreateBocDataCellRenderArguments(columnsWithValidationFailures: new [] { true }));

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertChildElementCount(td, 1);

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);
      Html.AssertChildElementCount(cellStructureDiv, 2);

      var span = Html.GetAssertedChildElement(cellStructureDiv, "span", 0);
      Html.AssertAttribute(span, "class", _bocListCssClassDefinition.ValidationErrorMarker);
      Html.AssertChildElementCount(span, 1);

      var img = Html.GetAssertedChildElement(span, "img", 0);
      Html.AssertAttribute(img, "src", "/fake/Remotion.Development.Web/Image/Spacer.gif");

      var div = Html.GetAssertedChildElement(cellStructureDiv, "div", 1);
      Html.AssertAttribute(div, "class", _bocListCssClassDefinition.Content);
    }

    private void RenderTitleCell (
        SortingDirection sortDirection,
        int sortIndex,
        string iconFilename,
        string iconAltText)
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      var renderingContext = CreateRenderingContext();
      renderer.RenderTitleCell(renderingContext, CreateBocTitleCellRenderArguments(sortDirection, sortIndex));

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement(document, "th", 0);
      Html.AssertAttribute(th, "class", _bocListCssClassDefinition.TitleCell, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute(th, "class", c_columnCssClass, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute(th, "role", "columnheader");

      Assert.Less(0, th.ChildNodes.Count);
      var sortCommandLink = Html.GetAssertedChildElement(th, "a", 0);
      Html.AssertAttribute(sortCommandLink, "id", List.Object.ClientID + "_0_SortCommand");
      Html.AssertChildElementCount(sortCommandLink, 2);

      var titleSpan = Html.GetAssertedChildElement(sortCommandLink, "span", 0);
      Html.AssertTextNode(titleSpan, Column.ColumnTitleDisplayValue.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks), 0);

      Html.AssertTextNode(sortCommandLink, HtmlHelper.WhiteSpace, 1);

      var sortOrderSpan = Html.GetAssertedChildElement(sortCommandLink, "span", 2);
      Html.AssertAttribute(sortOrderSpan, "class", _bocListCssClassDefinition.SortingOrder, HtmlHelperBase.AttributeValueCompareMode.Contains);

      var sortIcon = Html.GetAssertedChildElement(sortOrderSpan, "img", 0);
      Html.AssertAttribute(sortIcon, "src", iconFilename, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute(sortIcon, "alt", iconAltText);

      Html.AssertTextNode(sortOrderSpan, HtmlHelper.WhiteSpace + (sortIndex + 1), 1);
    }

    private BocColumnRenderingContext<BocSimpleColumnDefinition> CreateRenderingContext ()
    {
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(List.Object.DataSource, List.Object.Property, "Args");

      return new BocColumnRenderingContext<BocSimpleColumnDefinition>(
          new BocColumnRenderingContext(
              HttpContext,
              Html.Writer,
              List.Object,
              businessObjectWebServiceContext,
              Column,
              ColumnIndexProvider.Object,
              0,
              6));
    }
  }
}
