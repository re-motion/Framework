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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocRowRendererTest : BocListRendererTestBase
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private BocColumnRenderer[] _columnRenderers;
    private StubLabelReferenceRenderer _stubLabelReferenceRenderer;
    private StubValidationSummaryRenderer _stubValidationSummaryRenderer;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      List.Setup(mock => mock.Selection).Returns(RowSelection.Multiple);
      var stubColumnDefinition = new StubColumnDefinition();
      List.Setup(mock => mock.AreDataRowsClickSensitive()).Returns(true);

      _columnRenderers = new[]
                         {
                             new BocColumnRenderer(
                                 new StubColumnRenderer(new FakeResourceUrlFactory()),
                                 stubColumnDefinition,
                                 columnIndex: 7,
                                 visibleColumnIndex: 13,
                                 isRowHeader: false,
                                 showIcon: false,
                                 SortingDirection.Ascending,
                                 orderIndex: 5)
                         };

      _bocListCssClassDefinition = new BocListCssClassDefinition();
      _stubLabelReferenceRenderer = new StubLabelReferenceRenderer();
      _stubValidationSummaryRenderer = new StubValidationSummaryRenderer();
    }

    [Test]
    public void RenderTitlesRow ()
    {
      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer),
          RenderingFeatures.Default,
          _stubValidationSummaryRenderer);
      renderer.RenderTitlesRow(CreateRenderingContext());

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);
      Html.AssertAttribute(tr, "role", "row");

      var th = Html.GetAssertedChildElement(tr, "th", 0);
      Html.AssertAttribute(th, "arguments-CellID", List.Object.ClientID + "_C13");
    }

    [Test]
    public void RenderTitlesRowWithIndex ()
    {
      List.Setup(mock => mock.IsIndexEnabled).Returns(true);
      List.Setup(mock => mock.Index).Returns(RowIndex.InitialOrder);

      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer),
          RenderingFeatures.Default,
          _stubValidationSummaryRenderer);
      renderer.RenderTitlesRow(CreateRenderingContext());

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);
      Html.AssertAttribute(tr, "role", "row");

      var thIndex = Html.GetAssertedChildElement(tr, "th", 0);
      Html.AssertAttribute(thIndex, "id", "MyList_C0");
      Html.AssertAttribute(thIndex, "class", _bocListCssClassDefinition.TitleCell, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute(thIndex, "class", _bocListCssClassDefinition.TitleCellIndex, HtmlHelperBase.AttributeValueCompareMode.Contains);

      Html.GetAssertedChildElement(tr, "th", 1);
    }

    [Test]
    public void RenderTitlesRowWithSelector ()
    {
      List.Setup(mock => mock.IsSelectionEnabled).Returns(true);
      List.Setup(mock => mock.Selection).Returns(RowSelection.Multiple);

      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer),
          RenderingFeatures.Default,
          _stubValidationSummaryRenderer);
      renderer.RenderTitlesRow(CreateRenderingContext());

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);
      Html.AssertAttribute(tr, "role", "row");

      Html.GetAssertedChildElement(tr, "td", 0);

      Html.GetAssertedChildElement(tr, "th", 1);
    }

    [Test]
    public void RenderDataRow ()
    {
      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer),
          RenderingFeatures.Default,
          _stubValidationSummaryRenderer);
      renderer.RenderDataRow(
          CreateRenderingContext(),
          new BocListRowRenderingContext(new BocListRow(0, BusinessObject), 1, false),
          new BocRowRenderArguments(0, new bool[_columnRenderers.Length]));

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);
      Html.AssertAttribute(tr, "class", _bocListCssClassDefinition.DataRow + " " + _bocListCssClassDefinition.DataRowOdd);
      Html.AssertAttribute(tr, "role", "row");

      var td = Html.GetAssertedChildElement(tr, "td", 0);
      Html.AssertAttribute(td, "arguments-CellID", "null");
      Html.AssertAttribute(td, "arguments-HeaderIDs", "MyList_C13");
    }

    [Test]
    public void RenderDataRow_WithRowHeaders ()
    {
      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer),
          RenderingFeatures.Default,
          _stubValidationSummaryRenderer);
      var columnRenderers = new[]
                             {
                                 CreateBocColumnRenderer(3, isRowHeader: false),
                                 CreateBocColumnRenderer(4, isRowHeader: true),
                                 CreateBocColumnRenderer(5, isRowHeader: false),
                                 CreateBocColumnRenderer(6, isRowHeader: true),
                                 CreateBocColumnRenderer(7, isRowHeader: false),
                                 CreateBocColumnRenderer(8, isRowHeader: true),
                                 CreateBocColumnRenderer(9, isRowHeader: false)
                             };
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      renderer.RenderDataRow(
          new BocListRenderingContext(HttpContext, Html.Writer, List.Object, businessObjectWebServiceContext, columnRenderers),
          new BocListRowRenderingContext(new BocListRow(0, BusinessObject), 1, false),
          new BocRowRenderArguments(17, new bool[columnRenderers.Length]));

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);

      var td0 = Html.GetAssertedChildElement(tr, "td", 0);
      Html.AssertAttribute(td0, "arguments-CellID", "null");
      Html.AssertAttribute(td0, "arguments-HeaderIDs", "MyList_C4_R17, MyList_C6_R17, MyList_C8_R17, MyList_C3");

      var td1 = Html.GetAssertedChildElement(tr, "td", 1);
      Html.AssertAttribute(td1, "arguments-CellID", "MyList_C4_R17");
      Html.AssertAttribute(td1, "arguments-HeaderIDs", "MyList_C4");

      var td2 = Html.GetAssertedChildElement(tr, "td", 2);
      Html.AssertAttribute(td2, "arguments-CellID", "null");
      Html.AssertAttribute(td2, "arguments-HeaderIDs", "MyList_C4_R17, MyList_C6_R17, MyList_C8_R17, MyList_C5");

      var td3 = Html.GetAssertedChildElement(tr, "td", 3);
      Html.AssertAttribute(td3, "arguments-CellID", "MyList_C6_R17");
      Html.AssertAttribute(td3, "arguments-HeaderIDs", "MyList_C4_R17, MyList_C6");

      var td4 = Html.GetAssertedChildElement(tr, "td", 4);
      Html.AssertAttribute(td4, "arguments-CellID", "null");
      Html.AssertAttribute(td4, "arguments-HeaderIDs", "MyList_C4_R17, MyList_C6_R17, MyList_C8_R17, MyList_C7");

      var td5 = Html.GetAssertedChildElement(tr, "td", 5);
      Html.AssertAttribute(td5, "arguments-CellID", "MyList_C8_R17");
      Html.AssertAttribute(td5, "arguments-HeaderIDs", "MyList_C4_R17, MyList_C6_R17, MyList_C8");

      var td6 = Html.GetAssertedChildElement(tr, "td", 6);
      Html.AssertAttribute(td6, "arguments-CellID", "null");
      Html.AssertAttribute(td6, "arguments-HeaderIDs", "MyList_C4_R17, MyList_C6_R17, MyList_C8_R17, MyList_C9");

      BocColumnRenderer CreateBocColumnRenderer (int visibleColumnIndex, bool isRowHeader)
      {
        return new BocColumnRenderer(
            new StubColumnRenderer(new FakeResourceUrlFactory()),
            new StubColumnDefinition(),
            columnIndex: 0,
            visibleColumnIndex: visibleColumnIndex,
            isRowHeader: isRowHeader,
            showIcon: false,
            SortingDirection.None,
            orderIndex: -1);
      }
    }

    [Test]
    public void RenderDataRow_WithIndex_WithRowHeaders ()
    {
      List.Setup(_ => _.IsIndexEnabled).Returns(true);
      List.Setup(_ => _.IsSelectionEnabled).Returns(false);

      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer),
          RenderingFeatures.Default,
          _stubValidationSummaryRenderer);
      var columnRenderers = new[]
                            {
                                new BocColumnRenderer(
                                    new StubColumnRenderer(new FakeResourceUrlFactory()),
                                    new StubColumnDefinition(),
                                    columnIndex: 0,
                                    visibleColumnIndex: 5,
                                    isRowHeader: true,
                                    showIcon: false,
                                    SortingDirection.None,
                                    orderIndex: -1)
                            };
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      renderer.RenderDataRow(
          new BocListRenderingContext(HttpContext, Html.Writer, List.Object, businessObjectWebServiceContext, columnRenderers),
          new BocListRowRenderingContext(new BocListRow(0, BusinessObject), 1, false),
          new BocRowRenderArguments(17, new bool[_columnRenderers.Length]));

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);

      var td0 = Html.GetAssertedChildElement(tr, "td", 0);
      Html.AssertNoAttribute(td0, "id");
      // Rendering the header IDs is problematic for split tables and doesn't help with columns to the left of the header column.
      // Therefor, the header IDs are simply not rendered in the first place.
      Html.AssertNoAttribute(td0, "headers");
    }

    [Test]
    public void RenderDataRow_WithSelection_WithRowHeaders ()
    {
      List.Setup(_ => _.IsIndexEnabled).Returns(false);
      List.Setup(_ => _.IsSelectionEnabled).Returns(true);
      List.Setup(_ => _.GetSelectorControlValue(It.IsAny<BocListRow>())).Returns("Value");

      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer),
          RenderingFeatures.Default,
          _stubValidationSummaryRenderer);
      var columnRenderers = new[]
                            {
                                new BocColumnRenderer(
                                    new StubColumnRenderer(new FakeResourceUrlFactory()),
                                    new StubColumnDefinition(),
                                    columnIndex: 0,
                                    visibleColumnIndex: 5,
                                    isRowHeader: true,
                                    showIcon: false,
                                    SortingDirection.None,
                                    orderIndex: -1)
                            };
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      renderer.RenderDataRow(
          new BocListRenderingContext(HttpContext, Html.Writer, List.Object, businessObjectWebServiceContext, columnRenderers),
          new BocListRowRenderingContext(new BocListRow(0, BusinessObject), 1, false),
          new BocRowRenderArguments(17, new bool[_columnRenderers.Length]));

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);

      var td0 = Html.GetAssertedChildElement(tr, "td", 0);
      Html.AssertNoAttribute(td0, "id");
      // Rendering the header IDs is problematic for split tables and doesn't help with columns to the left of the header column.
      // Therefor, the header IDs are simply not rendered in the first place.
      Html.AssertNoAttribute(td0, "headers");
    }

    [Test]
    public void RenderDataRow_WithIndexAndSelection_WithRowHeaders ()
    {
      List.Setup(_ => _.IsIndexEnabled).Returns(true);
      List.Setup(_ => _.IsSelectionEnabled).Returns(true);
      List.Setup(_ => _.GetSelectorControlValue(It.IsAny<BocListRow>())).Returns("Value");

      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer),
          RenderingFeatures.Default,
          _stubValidationSummaryRenderer);
      var columnRenderers = new[]
                            {
                                new BocColumnRenderer(
                                    new StubColumnRenderer(new FakeResourceUrlFactory()),
                                    new StubColumnDefinition(),
                                    columnIndex: 0,
                                    visibleColumnIndex: 5,
                                    isRowHeader: true,
                                    showIcon: false,
                                    SortingDirection.None,
                                    orderIndex: -1)
                            };
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      renderer.RenderDataRow(
          new BocListRenderingContext(HttpContext, Html.Writer, List.Object, businessObjectWebServiceContext, columnRenderers),
          new BocListRowRenderingContext(new BocListRow(0, BusinessObject), 1, false),
          new BocRowRenderArguments(17, new bool[_columnRenderers.Length]));

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);

      var td0 = Html.GetAssertedChildElement(tr, "td", 0);
      Html.AssertNoAttribute(td0, "id");
      // Rendering the header IDs is problematic for split tables and doesn't help with columns to the left of the header column.
      // Therefor, the header IDs are simply not rendered in the first place.
      Html.AssertNoAttribute(td0, "headers");

      var td1 = Html.GetAssertedChildElement(tr, "td", 1);
      Html.AssertNoAttribute(td1, "id");
      // Rendering the header IDs is problematic for split tables and doesn't help with columns to the left of the header column.
      // Therefor, the header IDs are simply not rendered in the first place.
      Html.AssertNoAttribute(td1, "headers");
    }

    [Test]
    public void RenderDataRowSelected ()
    {
      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer),
          RenderingFeatures.Default,
          _stubValidationSummaryRenderer);
      renderer.RenderDataRow(
          CreateRenderingContext(),
          new BocListRowRenderingContext(new BocListRow(0, BusinessObject), 1, true),
          new BocRowRenderArguments(0, new bool[_columnRenderers.Length]));

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);
      Html.AssertAttribute(
          tr,
          "class",
          _bocListCssClassDefinition.DataRow + " " + _bocListCssClassDefinition.DataRowOdd + " " + _bocListCssClassDefinition.DataRowSelected);
      Html.AssertAttribute(tr, "role", "row");

      Html.GetAssertedChildElement(tr, "td", 0);
    }

    [Test]
    public void RenderEmptyDataRow ()
    {
      List.Setup(mock => mock.IsIndexEnabled).Returns(true);
      List.Setup(mock => mock.IsSelectionEnabled).Returns(true);

      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer),
          RenderingFeatures.Default,
          _stubValidationSummaryRenderer);
      renderer.RenderEmptyListDataRow(CreateRenderingContext());

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);
      Html.AssertAttribute(tr, "role", "row");

      Html.GetAssertedChildElement(tr, "td", 0);
    }

    [Test]
    public void RenderEmptyDataRowEmptyListMessageWebString ()
    {
      List.Setup(mock => mock.EmptyListMessage).Returns(WebString.CreateFromText("Multiline\nEmptyListMessage"));

      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer),
          RenderingFeatures.Default,
          _stubValidationSummaryRenderer);
      renderer.RenderEmptyListDataRow(CreateRenderingContext());

      var document = Html.GetResultDocument();
      var messageElement = document.SelectSingleNode("/tr/td");
      Assert.That(messageElement.InnerXml, Is.EqualTo("Multiline<br />EmptyListMessage"));
    }

    [Test]
    public void TestDiagnosticMetadataRendering ()
    {
      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.WithDiagnosticMetadata, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.WithDiagnosticMetadata, _bocListCssClassDefinition, _stubLabelReferenceRenderer),
          RenderingFeatures.WithDiagnosticMetadata,
          _stubValidationSummaryRenderer);
      renderer.RenderDataRow(
          CreateRenderingContext(),
          new BocListRowRenderingContext(new BocListRow(0, BusinessObject), 1, false),
          new BocRowRenderArguments(0, new bool[_columnRenderers.Length]));

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);
      Html.AssertAttribute(tr, DiagnosticMetadataAttributes.ItemID, ((IBusinessObjectWithIdentity)BusinessObject).UniqueIdentifier);
      Html.AssertAttribute(tr, DiagnosticMetadataAttributesForObjectBinding.BocListRowIndex, 1.ToString());
    }

    [Test]
    public void RenderValidationRow ()
    {
      var validationSummaryRendererStub = new StubValidationSummaryRenderer();
      BocListValidationSummaryRenderArguments? args = null;
      BocListRenderingContext ctx = null;
      validationSummaryRendererStub.Callback = (BocRenderingContext<IBocList> context, in BocListValidationSummaryRenderArguments arguments) =>
      {
        ctx = context as BocListRenderingContext;
        args = arguments;
      };

      var validationFailureRepository = new BocListValidationFailureRepository();
      validationFailureRepository.AddValidationFailuresForDataRow(BusinessObject, new []{BusinessObjectValidationFailure.Create("Big error")});

      List.Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepository);

      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer),
          RenderingFeatures.Default,
          validationSummaryRendererStub);
      var columnRenderers = new[]
                            {
                                new BocColumnRenderer(
                                    new StubColumnRenderer(new FakeResourceUrlFactory()),
                                    new StubColumnDefinition(),
                                    columnIndex: 0,
                                    visibleColumnIndex: 5,
                                    isRowHeader: true,
                                    showIcon: false,
                                    SortingDirection.None,
                                    orderIndex: -1)
                            };
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      Html.Writer.RenderBeginTag("root");
      renderer.RenderDataRow(
          new BocListRenderingContext(HttpContext, Html.Writer, List.Object, businessObjectWebServiceContext, columnRenderers),
          new BocListRowRenderingContext(new BocListRow(0, BusinessObject), 1, false),
          new BocRowRenderArguments(17, new bool[_columnRenderers.Length]));

      Html.Writer.RenderEndTag();

      var document = Html.GetResultDocument();

      var root = Html.GetAssertedChildElement(document, "root", 0);

      var trRow = Html.GetAssertedChildElement(root, "tr", 0);
      trRow.AssertAttributeValueContains("class", "hasValidationRow");

      var trValidationRow = Html.GetAssertedChildElement(root, "tr", 1);
      trValidationRow.AssertAttributeValueContains("class", "bocListValidationRow");

      var td = Html.GetAssertedChildElement(trValidationRow, "td", 0);
      td.AssertAttributeValueEquals("colspan", "6");

      var outerDiv = Html.GetAssertedChildElement(td, "div", 0);

      var innerDiv = Html.GetAssertedChildElement(outerDiv, "div", 0);
      Html.AssertAttribute(innerDiv, "aria-hidden", "true");

      var validationSummary = Html.GetAssertedChildElement(innerDiv, "validation-summary", 0);

      Assert.That(args.Value.ValidationFailures.Single().Failure.ErrorMessage, Is.EqualTo("Big error"));
      Assert.That(ctx.ColumnRenderers, Is.EqualTo(columnRenderers));
    }

    [Test]
    public void RenderValidationRow_WithValidationFailureColumn_AddsTDBeforeErrorMessageContainer ()
    {
      var validationSummaryRendererStub = new StubValidationSummaryRenderer();
      BocListValidationSummaryRenderArguments? args = null;
      BocListRenderingContext ctx = null;
      validationSummaryRendererStub.Callback = (BocRenderingContext<IBocList> context, in BocListValidationSummaryRenderArguments arguments) =>
      {
        ctx = context as BocListRenderingContext;
        args = arguments;
      };

      var validationFailureRepository = new BocListValidationFailureRepository();
      validationFailureRepository.AddValidationFailuresForDataRow(BusinessObject, new []{BusinessObjectValidationFailure.Create("Big error")});

      List.Setup(_ => _.ValidationFailureRepository).Returns(validationFailureRepository);

      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition, _stubLabelReferenceRenderer),
          RenderingFeatures.Default,
          validationSummaryRendererStub);
      var columnRenderers = new[]
                            {
                                new BocColumnRenderer(
                                    new StubColumnRenderer(new FakeResourceUrlFactory()),
                                    new StubColumnDefinition(),
                                    columnIndex: 0,
                                    visibleColumnIndex: 2,
                                    isRowHeader: true,
                                    showIcon: false,
                                    SortingDirection.None,
                                    orderIndex: -1),
                                new BocColumnRenderer(
                                    new StubColumnRenderer(new FakeResourceUrlFactory()),
                                    new BocValidationErrorIndicatorColumnDefinition(),
                                    columnIndex: 1,
                                    visibleColumnIndex: 3,
                                    isRowHeader: true,
                                    showIcon: false,
                                    SortingDirection.None,
                                    orderIndex: -1)
                            };
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      Html.Writer.RenderBeginTag("root");
      renderer.RenderDataRow(
          new BocListRenderingContext(HttpContext, Html.Writer, List.Object, businessObjectWebServiceContext, columnRenderers),
          new BocListRowRenderingContext(new BocListRow(0, BusinessObject), 1, false),
          new BocRowRenderArguments(17, new bool[columnRenderers.Length]));

      Html.Writer.RenderEndTag();

      var document = Html.GetResultDocument();

      var root = Html.GetAssertedChildElement(document, "root", 0);

      var trRow = Html.GetAssertedChildElement(root, "tr", 0);
      trRow.AssertAttributeValueContains("class", "hasValidationRow");

      var trValidationRow = Html.GetAssertedChildElement(root, "tr", 1);
      trValidationRow.AssertAttributeValueContains("class", "bocListValidationRow");

      var tdBeforeErrorMessages = Html.GetAssertedChildElement(trValidationRow, "td", 0);
      tdBeforeErrorMessages.AssertAttributeValueEquals("colspan", "3");

      var tdWithErrorMessages = Html.GetAssertedChildElement(trValidationRow, "td", 1);
      tdWithErrorMessages.AssertAttributeValueEquals("colspan", "1");

      var outerDiv = Html.GetAssertedChildElement(tdWithErrorMessages, "div", 0);

      var innerDiv = Html.GetAssertedChildElement(outerDiv, "div", 0);

      var validationSummary = Html.GetAssertedChildElement(innerDiv, "validation-summary", 0);

      Assert.That(args.Value.ValidationFailures.Single().Failure.ErrorMessage, Is.EqualTo("Big error"));
      Assert.That(ctx.ColumnRenderers, Is.EqualTo(columnRenderers));
    }

    private BocListRenderingContext CreateRenderingContext ()
    {
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      return new BocListRenderingContext(HttpContext, Html.Writer, List.Object, businessObjectWebServiceContext, _columnRenderers);
    }
  }
}
