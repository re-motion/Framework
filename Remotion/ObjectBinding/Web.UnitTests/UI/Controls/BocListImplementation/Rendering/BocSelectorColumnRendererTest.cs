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
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocSelectorColumnRendererTest : BocListRendererTestBase
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private BocListRenderingContext _bocListRenderingContext;
    private ILabelReferenceRenderer _stubLabelReferenceRenderer;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      List.Setup(mock => mock.IsSelectionEnabled).Returns(true);

      _bocListCssClassDefinition = new BocListCssClassDefinition();
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      _bocListRenderingContext = new BocListRenderingContext(
          HttpContext,
          Html.Writer,
          List.Object,
          businessObjectWebServiceContext,
          new BocColumnRenderer[0]);
      _stubLabelReferenceRenderer = new StubLabelReferenceRenderer();
    }

    [Test]
    public void RenderTitleCellForMultiSelect ()
    {
      List.Setup(mock => mock.Selection).Returns(RowSelection.Multiple);
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer);
      renderer.RenderTitleCell(_bocListRenderingContext, "TitleCellID");

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "id", "TitleCellID");
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.TitleCell + " " + _bocListCssClassDefinition.Themed + " " + _bocListCssClassDefinition.TitleCellSelector);
      Html.AssertAttribute(td, "role", "cell");

      var input = Html.GetAssertedChildElement(td, "input", 0);
      Html.AssertAttribute(input, "type", "checkbox");
      Html.AssertAttribute(input, "name", List.Object.GetSelectAllControlName());
      Html.AssertNoAttribute(input, "id");
      Html.AssertNoAttribute(input, "value");
      Html.AssertAttribute(input, "title", "Select all rows");
    }

    [Test]
    public void RenderDataCellForMultiSelect_WithIndexDisabled ()
    {
      var row = new BocListRow(1, BusinessObject);
      List.Setup(mock => mock.Selection).Returns(RowSelection.Multiple);
      List.Setup(mock => mock.GetSelectorControlValue(row)).Returns("row1");
      List.Setup(mock => mock.IsIndexEnabled).Returns(false);
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer);
      renderer.RenderDataCell(
          _bocListRenderingContext,
          new BocListRowRenderingContext(row, 0, false),
          new[] { "rowID1", "rowID2", "columnID" });

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", "bocListDataCell remotion-themed bocListDataCellRowSelector");
      Html.AssertNoAttribute(td, "headers");

      var input = Html.GetAssertedChildElement(td, "input", 0);
      Html.AssertAttribute(input, "type", "checkbox");
      Html.AssertAttribute(input, "id", "SelectRowControl_UnqiueID_1");
      Html.AssertAttribute(input, "name", "SelectRowControl$UnqiueID");
      Html.AssertAttribute(input, "value", "row1");
      Html.AssertAttribute(input, StubLabelReferenceRenderer.LabelReferenceAttribute, "rowID1 rowID2 SelectRowControl_UnqiueID_1_Label");

      var checkboxSpan = Html.GetAssertedChildElement(td, "span", 1);
      Html.AssertChildElementCount(checkboxSpan, 0);

      var labelSpan = Html.GetAssertedChildElement(td, "span", 2);
      Html.AssertAttribute(labelSpan, "id", "SelectRowControl_UnqiueID_1_Label");
      Html.AssertAttribute(labelSpan, "hidden", "hidden");
      Html.AssertTextNode(labelSpan, "Select this row", 0);
    }

    [Test]
    public void RenderDataCellForMultiSelect_WithIndexColumn ()
    {
      var row = new BocListRow(1, BusinessObject);
      List.Setup(mock => mock.Selection).Returns(RowSelection.Multiple);
      List.Setup(mock => mock.GetSelectorControlValue(row)).Returns("row1");
      List.Setup(mock => mock.IsIndexEnabled).Returns(true);
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer);
      renderer.RenderDataCell(
          _bocListRenderingContext,
          new BocListRowRenderingContext(row, 0, false),
          new[] { "rowID1", "rowID2", "columnID" });

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);

      var input = Html.GetAssertedChildElement(td, "input", 0);
      Html.AssertAttribute(input, "id", "SelectRowControl_UnqiueID_1");
      Html.AssertAttribute(input, StubLabelReferenceRenderer.LabelReferenceAttribute, "SelectRowControl_UnqiueID_1_Label");
    }

    [Test]
    public void RenderDataCellForMultiSelect_WithoutHeaders ()
    {
      var row = new BocListRow(1, BusinessObject);
      List.Setup(mock => mock.Selection).Returns(RowSelection.Multiple);
      List.Setup(mock => mock.GetSelectorControlValue(row)).Returns("row1");
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer);
      renderer.RenderDataCell(
          _bocListRenderingContext,
          new BocListRowRenderingContext(row, 0, false),
          Array.Empty<string>());

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);

      var input = Html.GetAssertedChildElement(td, "input", 0);
      Html.AssertAttribute(input, "id", "SelectRowControl_UnqiueID_1");
      Html.AssertAttribute(input, StubLabelReferenceRenderer.LabelReferenceAttribute, "SelectRowControl_UnqiueID_1_Label");
    }

    [Test]
    public void RenderTitleCellForSingleSelect ()
    {
      List.Setup(mock => mock.Selection).Returns(RowSelection.SingleRadioButton);
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer);
      renderer.RenderTitleCell(_bocListRenderingContext, "TitleCellID");

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "id", "TitleCellID");
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.TitleCell + " " + _bocListCssClassDefinition.Themed + " " + _bocListCssClassDefinition.TitleCellSelector);
      Html.AssertAttribute(td, "role", "cell");

      var span = Html.GetAssertedChildElement(td, "span", 0);
      Html.AssertAttribute(span, "class", _bocListCssClassDefinition.CssClassScreenReaderText);
      Html.AssertTextNode(span, "Row selection", 0);
    }

    [Test]
    public void RenderDataCellForSingleSelect_WithoutIndexColumn ()
    {
      var row = new BocListRow(1, BusinessObject);
      List.Setup(mock => mock.Selection).Returns(RowSelection.SingleRadioButton);
      List.Setup(mock => mock.GetSelectorControlValue(row)).Returns("row1");
      List.Setup(mock => mock.IsIndexEnabled).Returns(false);
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer);
      renderer.RenderDataCell(
          _bocListRenderingContext,
          new BocListRowRenderingContext(row, 0, false),
          new[] { "rowID3", "rowID4", "columnID" });
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", "bocListDataCell remotion-themed bocListDataCellRowSelector");
      Html.AssertAttribute(td, "role", "cell");
      Html.AssertNoAttribute(td, "headers");

      var input = Html.GetAssertedChildElement(td, "input", 0);
      Html.AssertAttribute(input, "type", "radio");
      Html.AssertAttribute(input, "id", "SelectRowControl_UnqiueID_1");
      Html.AssertAttribute(input, "name", "SelectRowControl$UnqiueID");
      Html.AssertAttribute(input, "value", "row1");
      Html.AssertAttribute(input, StubLabelReferenceRenderer.LabelReferenceAttribute, "rowID3 rowID4 SelectRowControl_UnqiueID_1_Label");

      var checkboxSpan = Html.GetAssertedChildElement(td, "span", 1);
      Html.AssertChildElementCount(checkboxSpan, 0);

      var labelSpan = Html.GetAssertedChildElement(td, "span", 2);
      Html.AssertAttribute(labelSpan, "id", "SelectRowControl_UnqiueID_1_Label");
      Html.AssertAttribute(labelSpan, "hidden", "hidden");
      Html.AssertTextNode(labelSpan, "Select this row", 0);
    }

    [Test]
    public void RenderDataCellForSingleSelect_WithIndexColumn ()
    {
      var row = new BocListRow(1, BusinessObject);
      List.Setup(mock => mock.Selection).Returns(RowSelection.SingleRadioButton);
      List.Setup(mock => mock.GetSelectorControlValue(row)).Returns("row1");
      List.Setup(mock => mock.IsIndexEnabled).Returns(true);
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer);
      renderer.RenderDataCell(
          _bocListRenderingContext,
          new BocListRowRenderingContext(row, 0, false),
          new[] { "rowID3", "rowID4", "columnID" });
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);

      var input = Html.GetAssertedChildElement(td, "input", 0);
      Html.AssertAttribute(input, "id", "SelectRowControl_UnqiueID_1");
      Html.AssertAttribute(input, StubLabelReferenceRenderer.LabelReferenceAttribute, "SelectRowControl_UnqiueID_1_Label");
    }

    [Test]
    public void RenderDataCellForSingleSelect_WithoutHeaders ()
    {
      var row = new BocListRow(1, BusinessObject);
      List.Setup(mock => mock.Selection).Returns(RowSelection.SingleRadioButton);
      List.Setup(mock => mock.GetSelectorControlValue(row)).Returns("row1");
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer);
      renderer.RenderDataCell(
          _bocListRenderingContext,
          new BocListRowRenderingContext(row, 0, false),
          Array.Empty<string>());
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);

      var input = Html.GetAssertedChildElement(td, "input", 0);
      Html.AssertAttribute(input, "id", "SelectRowControl_UnqiueID_1");
      Html.AssertAttribute(input, StubLabelReferenceRenderer.LabelReferenceAttribute, "SelectRowControl_UnqiueID_1_Label");
    }

    [Test]
    public void RenderDataCell_WithRowHeader ()
    {
      var row = new BocListRow(1, BusinessObject);
      List.Setup(mock => mock.Selection).Returns(RowSelection.SingleRadioButton);
      List.Setup(mock => mock.GetSelectorControlValue(row)).Returns("row1");
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer);
      renderer.RenderDataCell(
          _bocListRenderingContext,
          new BocListRowRenderingContext(row, 0, false),
          new[]{"h1", "h2"});
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      // Rendering the header IDs is problematic for split tables and doesn't help with columns to the left of the header column.
      // Therefor, the header IDs are simply not rendered in the first place.
      Html.AssertNoAttribute(td, "headers");
    }

    [Test]
    public void TestDiagnosticMetadataRenderingInTitleCell ()
    {
      List.Setup(mock => mock.Selection).Returns(RowSelection.Multiple);
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer(RenderingFeatures.WithDiagnosticMetadata, _bocListCssClassDefinition, _stubLabelReferenceRenderer);
      renderer.RenderTitleCell(_bocListRenderingContext, "TitleCellID");

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, 1.ToString());

      var input = Html.GetAssertedChildElement(td, "input", 0);
      Html.AssertAttribute(input, DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownSelectAllControl, "true");
    }

    [Test]
    public void TestDiagnosticMetadataRenderingInDataCell ()
    {
      var row = new BocListRow(1, BusinessObject);
      List.Setup(mock => mock.Selection).Returns(RowSelection.SingleRadioButton);
      List.Setup(mock => mock.GetSelectorControlValue(row)).Returns("row1");
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer(RenderingFeatures.WithDiagnosticMetadata, _bocListCssClassDefinition, _stubLabelReferenceRenderer);
      renderer.RenderDataCell(
          _bocListRenderingContext,
          new BocListRowRenderingContext(row, 0, false),
          Array.Empty<string>());
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, 1.ToString());

      var input = Html.GetAssertedChildElement(td, "input", 0);
      Html.AssertAttribute(input, "id", "SelectRowControl_UnqiueID_1");
    }
  }
}
