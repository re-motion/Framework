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
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocSimpleColumnRendererTest : ColumnRendererTestBase<BocSimpleColumnDefinition>
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private BocColumnRenderingContext<BocSimpleColumnDefinition> _renderingContext;

    [SetUp]
    public override void SetUp ()
    {
      Column = new BocSimpleColumnDefinition();
      Column.Command = null;
      Column.IsDynamic = false;
      Column.IsReadOnly = false;
      Column.ColumnTitle = WebString.CreateFromText("FirstColumn");
      Column.PropertyPathIdentifier = "DisplayName";
      Column.FormatString = "unusedWithReferenceValue";

      base.SetUp();
      Column.OwnerControl = List.Object;

      _bocListCssClassDefinition = new BocListCssClassDefinition();

      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      _renderingContext = new BocColumnRenderingContext<BocSimpleColumnDefinition>(
          new BocColumnRenderingContext(
              HttpContext,
              Html.Writer,
              List.Object,
              businessObjectWebServiceContext,
              Column,
              ColumnIndexProvider.Object,
              0,
              0));
    }

    [Test]
    public void RenderBasicCell ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());

      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments());
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute(td, "role", "cell");

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      var div = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      Html.AssertAttribute(div, "class", _bocListCssClassDefinition.Content);

      var textWrapper = Html.GetAssertedChildElement(div, "span", 0);
      Html.AssertTextNode(textWrapper, "referencedObject1", 0);
    }

    [Test]
    public void TestDiagnosticMetadataRenderingWithEmptyDisplayName ()
    {
      var businessObject = TypeWithReference.Create("");
      EventArgs = new BocListDataRowRenderEventArgs(10, (IBusinessObject)businessObject, false, true);
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());

      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments());

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement(document, "td", 0);
      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      var span = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      var textWrapper = Html.GetAssertedChildElement(span, "span", 0);
      Html.AssertAttribute(textWrapper, DiagnosticMetadataAttributesForObjectBinding.BocListCellContents, string.Empty);
    }

    [Test]
    public void TestDiagnosticMetadataRenderingWithNullValue ()
    {
      var businessObject = TypeWithReference.Create();
      EventArgs = new BocListDataRowRenderEventArgs(10, (IBusinessObject)businessObject, false, true);
      Column.PropertyPathIdentifier = "ReferenceValue";
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());

      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments());

      Assert.That(businessObject.ReferenceValue, Is.Null);
      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement(document, "td", 0);
      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      var span = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      var textWrapper = Html.GetAssertedChildElement(span, "span", 0);
      Html.AssertAttribute(textWrapper, DiagnosticMetadataAttributesForObjectBinding.BocListCellContents, string.Empty);
    }

    [Test]
    public void RenderBasicCell_WithNewLineAndEncoding ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());

      var renderArgs = new BocListDataRowRenderEventArgs(0, (IBusinessObject)TypeWithReference.Create("value\r\nExtraText<html>"), false, true);
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments(renderArgs));
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute(td, "role", "cell");

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      var span = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      Html.AssertAttribute(span, "class", _bocListCssClassDefinition.Content);

      var textWrapper = Html.GetAssertedChildElement(span, "span", 0);
      Html.AssertTextNode(textWrapper, "value", 0);
      Html.GetAssertedChildElement(textWrapper, "br", 1);
      Html.AssertTextNode(textWrapper, "ExtraText<html>", 2); //This is actually encoded inside the asserted XmlDocument
    }

    [Test]
    public void RenderCommandCell ()
    {
      Column.Command = new BocListItemCommand(CommandType.Href);
      Column.Command.HrefCommand.Href = "url";

      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());

      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments(rowIndex: 5));
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute(td, "role", "cell");

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      var a = Html.GetAssertedChildElement(cellStructureDiv, "a", 0);
      Html.AssertAttribute(a, "id", List.Object.ClientID + "_Column_0_Command_Row_10");
      Html.AssertAttribute(a, "href", "url");
      Html.AssertAttribute(a, "onclick", "BocList.OnCommandClick();");

      var span = Html.GetAssertedChildElement(a, "span", 0);
      Html.AssertTextNode(span, "referencedObject1", 0);
    }

    [Test]
    public void RenderIconCell ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());

      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments(showIcon: true));
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute(td, "role", "cell");

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      var span = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      Html.AssertAttribute(span, "class", _bocListCssClassDefinition.Content);

      Html.AssertIcon(span, EventArgs.BusinessObject, null);
      Html.AssertTextNode(span, HtmlHelper.WhiteSpace, 1);

      var textWrapper = Html.GetAssertedChildElement(span, "span", 2);
      Html.AssertTextNode(textWrapper, BusinessObject.GetPropertyString("FirstValue"), 0);
    }

    [Test]
    public void RenderEditModeControl ()
    {
      var firstObject = (IBusinessObject)((TypeWithReference)BusinessObject).FirstValue;

      var editableRow = new Mock<IEditableRow>();
      editableRow.Setup(mock => mock.HasEditControl(It.IsAny<int>())).Returns(true);
      editableRow.Setup(mock => mock.GetEditControl(It.IsAny<int>())).Returns(new Mock<IBocTextValue>().Object);

      Mock.Get(List.Object.EditModeController).Setup(mock => mock.GetEditableRow(EventArgs.ListIndex)).Returns(editableRow.Object);

      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments(headerIDs: new[] { "r1 c1" }));

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute(td, "role", "cell");

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      var span = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      Html.AssertAttribute(span, "class", _bocListCssClassDefinition.Content);

      var clickSpan = Html.GetAssertedChildElement(span, "div", 0);
      Html.AssertAttribute(clickSpan, "onclick", "BocList.OnCommandClick();");

      editableRow.Verify(
          mock => mock.RenderSimpleColumnCellEditModeControl(
              Html.Writer,
              Column,
              firstObject,
              0,
              new[] { "r1 c1" }),
          Times.AtLeastOnce());
    }

    [Test]
    public void TestDiagnosticMetadataRenderingWithEditModeControl ()
    {
      var firstObject = (IBusinessObject)((TypeWithReference)BusinessObject).FirstValue;

      var editableRow = new Mock<IEditableRow>();
      editableRow.Setup(mock => mock.HasEditControl(It.IsAny<int>())).Returns(true);
      editableRow.Setup(mock => mock.GetEditControl(It.IsAny<int>())).Returns(new Mock<IBocTextValue>().Object);

      Mock.Get(List.Object.EditModeController).Setup(mock => mock.GetEditableRow(EventArgs.ListIndex)).Returns(editableRow.Object);

      IBocColumnRenderer renderer = new BocSimpleColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments(headerIDs: new[] { "r1 c1" }));

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, "1");
      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      var span = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      var clickSpan = Html.GetAssertedChildElement(span, "div", 0);
      Html.AssertAttribute(clickSpan, "onclick", "BocList.OnCommandClick();");
      Html.AssertAttribute(clickSpan, DiagnosticMetadataAttributesForObjectBinding.BocListCellContents, "referencedObject1");

      editableRow.Verify(
          mock => mock.RenderSimpleColumnCellEditModeControl(
              Html.Writer,
              Column,
              firstObject,
              0,
              new[] { "r1 c1" }),
          Times.AtLeastOnce());
    }
  }
}
