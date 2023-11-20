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
using System.IO;
using System.Web;
using System.Web.UI;
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocColumnRendererTest
  {
    private BocColumnRenderer _columnRendererAdapter;
    private Mock<IBocColumnRenderer> _columnRenderMock;
    private StubColumnDefinition _columnDefinition;
    private Mock<HtmlTextWriter> _htmlTextWriterStub;
    private Mock<HttpContextBase> _httpContextStub;
    private Mock<IBocList> _bocListStub;
    private BocListRenderingContext _renderingContext;

    [SetUp]
    public void SetUp ()
    {
      _columnDefinition = new StubColumnDefinition();
      _columnRenderMock = new Mock<IBocColumnRenderer>(MockBehavior.Strict);
      _columnRendererAdapter = new BocColumnRenderer(
          _columnRenderMock.Object,
          _columnDefinition,
          columnIndex: 0,
          visibleColumnIndex: 0,
          isRowHeader: false,
          showIcon: true,
          SortingDirection.Ascending,
          orderIndex: 3);
      _htmlTextWriterStub = new Mock<HtmlTextWriter>(TextWriter.Null);
      _httpContextStub = new Mock<HttpContextBase>();
      _bocListStub = new Mock<IBocList>();
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      _renderingContext = new BocListRenderingContext(
          _httpContextStub.Object,
          _htmlTextWriterStub.Object,
          _bocListStub.Object,
          businessObjectWebServiceContext,
          new[] { _columnRendererAdapter });
    }

    [Test]
    public void RenderTitleCell ()
    {
      _columnRenderMock
          .Setup(
              mock => mock.RenderTitleCell(
                  It.Is<BocColumnRenderingContext>(
                      rc =>
                          rc.HttpContext == _httpContextStub.Object
                          && rc.Control == _bocListStub.Object
                          && rc.Writer == _htmlTextWriterStub.Object
                          && rc.ColumnIndex == 0
                          && rc.ColumnDefinition == _columnDefinition),
                  new BocTitleCellRenderArguments(SortingDirection.Ascending, 3, "TitleCellID", false)))
          .Verifiable();

      _columnRendererAdapter.RenderTitleCell(_renderingContext, cellID: "TitleCellID");

      _columnRenderMock.Verify();
    }

    [Test]
    public void RenderDataColumnDeclaration ()
    {
      _columnRenderMock
          .Setup(
              mock => mock.RenderDataColumnDeclaration(
                  It.Is<BocColumnRenderingContext>(
                      rc =>
                          rc.HttpContext == _httpContextStub.Object
                          && rc.Control == _bocListStub.Object
                          && rc.Writer == _htmlTextWriterStub.Object
                          && rc.ColumnIndex == 0
                          && rc.ColumnDefinition == _columnDefinition),
                  false))
          .Verifiable();

      _columnRendererAdapter.RenderDataColumnDeclaration(_renderingContext, false);

      _columnRenderMock.Verify();
    }

    [Test]
    public void RenderDataCell ()
    {
      var dataRowRenderEventArgs = new BocListDataRowRenderEventArgs(0, Mock.Of<IBusinessObject>(), true, true);
      var rowIndex = 13;
      var cellID = "DataCell_0";
      var headerIDs = new string[] { "ID1" };
      var columnsWithValidationFailures = new bool[_renderingContext.ColumnRenderers.Length];

      _columnRenderMock
          .Setup(
              mock => mock.RenderDataCell(
                  It.Is<BocColumnRenderingContext>(
                      rc =>
                          rc.HttpContext == _httpContextStub.Object
                          && rc.Control == _bocListStub.Object
                          && rc.Writer == _htmlTextWriterStub.Object
                          && rc.ColumnIndex == 0
                          && rc.ColumnDefinition == _columnDefinition),
                  new BocDataCellRenderArguments(dataRowRenderEventArgs, rowIndex, true, cellID, headerIDs, columnsWithValidationFailures)))
          .Verifiable();

      _columnRendererAdapter.RenderDataCell(
              _renderingContext,
              rowIndex: rowIndex,
              cellID: cellID,
              headerIDs: headerIDs,
              columnsWithValidationFailures: columnsWithValidationFailures,
              dataRowRenderEventArgs);

      _columnRenderMock.Verify();
    }
  }
}
