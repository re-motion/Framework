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
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UnitTests.Domain;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocListTableBlockRendererTest : BocListRendererTestBase
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private BocColumnRenderer[] _stubColumnRenderers;

    [SetUp]
    public void SetUp ()
    {
      _bocListCssClassDefinition = new BocListCssClassDefinition();
    }

    [Test]
    public void RenderPopulatedList ()
    {
      InitializePopulatedList();
      CommonInitialize();

      XmlNode tbody;
      RenderAndAssertTable(out tbody);

      var trData1 = Html.GetAssertedChildElement(tbody, "tr", 0);
      Html.AssertAttribute(trData1, "class", "dataStub");

      var trData2 = Html.GetAssertedChildElement(tbody, "tr", 1);
      Html.AssertAttribute(trData2, "class", "dataStub");
    }


    [Test]
    public void RenderEmptyList ()
    {
      Initialize(false);
      CommonInitialize();
      List.Setup(mock => mock.ShowEmptyListMessage).Returns(true);
      List.Setup(mock => mock.ShowEmptyListEditMode).Returns(true);

      XmlNode tbody;
      RenderAndAssertTable(out tbody);

      var trData1 = Html.GetAssertedChildElement(tbody, "tr", 0);
      Html.AssertAttribute(trData1, "class", "emptyStub");
    }

    [Test]
    public void RenderDummyTable ()
    {
      Initialize(false);
      CommonInitialize();

      RenderList();

      var document = Html.GetResultDocument();
      var outerSpan = Html.GetAssertedChildElement(document, "span", 0);

      var table = Html.GetAssertedChildElement(outerSpan, "table", 0);
      var tr = Html.GetAssertedChildElement(table, "tr", 0);
      Html.AssertAttribute(table, StubLabelReferenceRenderer.LabelReferenceAttribute, "Label");
      Html.AssertAttribute(table, StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");
      Html.AssertAttribute(table, StubValidationErrorRenderer.ValidationErrorsIDAttribute, "MyList_ValidationErrors");
      Html.AssertAttribute(table, StubValidationErrorRenderer.ValidationErrorsAttribute, "ValidationError");

      var td = Html.GetAssertedChildElement(tr, "td", 0);
      Html.AssertTextNode(td, HtmlHelper.WhiteSpace, 0);

      var validationErrors = Html.GetAssertedChildElement(outerSpan, "fake", 1);
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsIDAttribute, "MyList_ValidationErrors");
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsAttribute, "ValidationError");
    }

    private void RenderAndAssertTable (out XmlNode tbody)
    {
      RenderList();

      var document = Html.GetResultDocument();
      var outerSpan = Html.GetAssertedChildElement(document, "span", 0);

      var tableContainer = Html.GetAssertedChildElement(outerSpan, "div", 0);
      Html.AssertAttribute(tableContainer, "class", _bocListCssClassDefinition.TableContainer);
      Html.AssertAttribute(tableContainer, "role", "table");
      Html.AssertAttribute(tableContainer, StubLabelReferenceRenderer.LabelReferenceAttribute, "Label");
      Html.AssertAttribute(tableContainer, StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, "");
      Html.AssertAttribute(tableContainer, StubValidationErrorRenderer.ValidationErrorsIDAttribute, "MyList_ValidationErrors");
      Html.AssertAttribute(tableContainer, StubValidationErrorRenderer.ValidationErrorsAttribute, "ValidationError");

      var tableScrollContainer = Html.GetAssertedChildElement(tableContainer, "div", 0);
      Html.AssertAttribute(tableScrollContainer, "class", _bocListCssClassDefinition.TableScrollContainer);
      Html.AssertAttribute(tableScrollContainer, "role", "none");

      var table = Html.GetAssertedChildElement(tableScrollContainer, "table", 0);
      Html.AssertAttribute(table, "class", _bocListCssClassDefinition.Table);
      Html.AssertAttribute(table, "role", "none");

      var colgroup = Html.GetAssertedChildElement(table, "colgroup", 0);

      Html.GetAssertedChildElement(colgroup, "col", 0);
      Html.GetAssertedChildElement(colgroup, "col", 1);
      Html.GetAssertedChildElement(colgroup, "col", 2);

      var thead = Html.GetAssertedChildElement(table, "thead", 1);
      Html.AssertAttribute(thead, "role", "rowgroup");

      var trTitle = Html.GetAssertedChildElement(thead, "tr", 0);
      Html.AssertAttribute(trTitle, "class", "titleStub");

      tbody = Html.GetAssertedChildElement(table, "tbody", 2);
      Html.AssertAttribute(tbody, "role", "rowgroup");

      var validationErrors = Html.GetAssertedChildElement(outerSpan, "fake", 1);
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsIDAttribute, "MyList_ValidationErrors");
      Html.AssertAttribute(validationErrors, StubValidationErrorRenderer.ValidationErrorsAttribute, "ValidationError");
    }

    private void RenderList ()
    {
      Html.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      IBocListTableBlockRenderer renderer = new BocListTableBlockRenderer(
          _bocListCssClassDefinition,
          new StubRowRenderer(),
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      renderer.Render(new BocListRenderingContext(HttpContext, Html.Writer, List.Object, businessObjectWebServiceContext, _stubColumnRenderers));
      Html.Writer.RenderEndTag();
    }

    private void CommonInitialize ()
    {
      List.Setup(list => list.IsSelectionEnabled).Returns(true);
      var stubColumnDefinition1 = new StubColumnDefinition();
      var stubColumnDefinition2 = new StubColumnDefinition();
      var stubColumnDefinition3 = new StubColumnDefinition();
      List.Setup(mock => mock.IsPagingEnabled).Returns(true);
      List.Setup(mock => mock.PageSize).Returns(5);

      _stubColumnRenderers = new[]
                             {
                                 new BocColumnRenderer(
                                     new StubColumnRenderer(new FakeResourceUrlFactory()),
                                     stubColumnDefinition1,
                                     columnIndex: 0,
                                     visibleColumnIndex: 0,
                                     isRowHeader: false,
                                     showIcon: false,
                                     SortingDirection.Ascending,
                                     orderIndex: 0),
                                 new BocColumnRenderer(
                                     new StubColumnRenderer(new FakeResourceUrlFactory()),
                                     stubColumnDefinition2,
                                     columnIndex: 1,
                                     visibleColumnIndex: 1,
                                     isRowHeader: false,
                                     showIcon: false,
                                     SortingDirection.Ascending,
                                     orderIndex: 1),
                                 new BocColumnRenderer(
                                     new StubColumnRenderer(new FakeResourceUrlFactory()),
                                     stubColumnDefinition2,
                                     columnIndex: 2,
                                     visibleColumnIndex: 2,
                                     isRowHeader: false,
                                     showIcon: false,
                                     SortingDirection.Ascending,
                                     orderIndex: 3)
                             };
    }

    private void InitializePopulatedList ()
    {
      Initialize(true);

      IBusinessObject firstObject = (IBusinessObject)((TypeWithReference)BusinessObject).FirstValue;
      IBusinessObject secondObject = (IBusinessObject)((TypeWithReference)BusinessObject).SecondValue;
      BocListRowRenderingContext[] rows = new[]
                          {
                            new BocListRowRenderingContext(new BocListRow(0, firstObject), 0, false),
                            new BocListRowRenderingContext(new BocListRow(1, secondObject), 1, false)
                          };
      List.Setup(list => list.GetRowsToRender()).Returns(rows);
    }
  }
}
