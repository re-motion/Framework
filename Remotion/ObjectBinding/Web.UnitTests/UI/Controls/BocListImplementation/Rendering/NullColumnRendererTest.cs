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
  public class NullColumnRendererTest
  {
    private NullColumnRenderer _nullColumnRenderer;
    private Mock<HtmlTextWriter> _htmlTextWriterMock;
    private Mock<HttpContextBase> _httpContextStub;
    private Mock<IBocList> _bocListStub;
    private Mock<IBocListColumnIndexProvider> _columnIndexProviderStub;
    private BocColumnRenderingContext<StubColumnDefinition> _renderingContext;
    private StubColumnDefinition _columnDefinition;

    [SetUp]
    public void SetUp ()
    {
      _nullColumnRenderer = new NullColumnRenderer();
      _columnDefinition = new StubColumnDefinition();
      _htmlTextWriterMock = new Mock<HtmlTextWriter>(MockBehavior.Strict, TextWriter.Null);
      _httpContextStub = new Mock<HttpContextBase>();
      _bocListStub = new Mock<IBocList>();
      _columnIndexProviderStub = new Mock<IBocListColumnIndexProvider>();
      _renderingContext = new BocColumnRenderingContext<StubColumnDefinition>(
          new BocColumnRenderingContext(
              _httpContextStub.Object,
              _htmlTextWriterMock.Object,
              _bocListStub.Object,
              BusinessObjectWebServiceContext.Create(null, null, null),
              _columnDefinition,
              _columnIndexProviderStub.Object,
              0,
              0));
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_nullColumnRenderer.IsNull, Is.True);
      Assert.That(_nullColumnRenderer.Column, Is.Null);
    }

    [Test]
    public void RenderTitleCell ()
    {
      _nullColumnRenderer.RenderTitleCell(_renderingContext, new BocTitleCellRenderArguments(SortingDirection.None, 0, "TitleCellID", isRowHeader: false));

      _htmlTextWriterMock.Verify();
    }

    [Test]
    public void RenderDataCell ()
    {
      _nullColumnRenderer.RenderDataCell(
          _renderingContext,
          new BocDataCellRenderArguments(
              new BocListDataRowRenderEventArgs(0, Mock.Of<IBusinessObject>(), false, false),
              0,
              showIcon: false,
              cellID: null,
              headerIDs: Array.Empty<string>(),
              columnsWithValidationFailures: Array.Empty<bool>()));

      _htmlTextWriterMock.Verify();
    }

    [Test]
    public void RenderDataColumnDeclaration ()
    {
      _nullColumnRenderer.RenderDataColumnDeclaration(_renderingContext, false);

      _htmlTextWriterMock.Verify();
    }

  }
}
