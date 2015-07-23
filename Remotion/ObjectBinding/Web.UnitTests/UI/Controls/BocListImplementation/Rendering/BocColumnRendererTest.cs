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
using System.Web;
using System.Web.UI;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocColumnRendererTest
  {
    private BocColumnRenderer _columnRendererAdapter;
    private IBocColumnRenderer _columnRenderMock;
    private StubColumnDefinition _columnDefinition;
    private HtmlTextWriter _htmlTextWriterStub;
    private HttpContextBase _httpContextStub;
    private IBocList _bocListStub;
    private BocListRenderingContext _renderingContext;

    [SetUp]
    public void SetUp ()
    {
      _columnDefinition = new StubColumnDefinition();
      _columnRenderMock = MockRepository.GenerateStrictMock<IBocColumnRenderer>();
      _columnRendererAdapter = new BocColumnRenderer (_columnRenderMock, _columnDefinition, 0, 0, true, SortingDirection.None, 0);
      _htmlTextWriterStub = MockRepository.GenerateStub<HtmlTextWriter>();
      _httpContextStub = MockRepository.GenerateStub<HttpContextBase>();
      _bocListStub = MockRepository.GenerateStub<IBocList>();
      _renderingContext = new BocListRenderingContext (_httpContextStub, _htmlTextWriterStub, _bocListStub, new[] { _columnRendererAdapter });
    }

    [Test]
    public void RenderTitleCell ()
    {
      _columnRenderMock.Expect (
          mock =>
              mock.RenderTitleCell (
                  Arg<BocColumnRenderingContext>.Matches (
                      rc =>
                          rc.HttpContext == _httpContextStub && rc.Control == _bocListStub && rc.Writer == _htmlTextWriterStub && rc.ColumnIndex == 0
                          && rc.ColumnDefinition == _columnDefinition),
                  Arg.Is (SortingDirection.None),
                  Arg.Is (0)));
      _columnRenderMock.Replay();

      _columnRendererAdapter.RenderTitleCell (_renderingContext);

      _columnRenderMock.VerifyAllExpectations();
    }

    [Test]
    public void RenderDataColumnDeclaration ()
    {
      _columnRenderMock.Expect (
          mock => mock.RenderDataColumnDeclaration (
              Arg<BocColumnRenderingContext>.Matches (
                  rc =>
                      rc.HttpContext == _httpContextStub && rc.Control == _bocListStub && rc.Writer == _htmlTextWriterStub && rc.ColumnIndex == 0
                      && rc.ColumnDefinition == _columnDefinition),
              Arg.Is (false)));
      _columnRenderMock.Replay();

      _columnRendererAdapter.RenderDataColumnDeclaration (_renderingContext, false);

      _columnRenderMock.VerifyAllExpectations();
    }

    [Test]
    public void RenderDataCell ()
    {
      var dataRowRenderEventArgs = new BocListDataRowRenderEventArgs (0, null, true, true);

      _columnRenderMock.Expect (
          mock => mock.RenderDataCell (
              Arg<BocColumnRenderingContext>.Matches (
                  rc =>
                      rc.HttpContext == _httpContextStub && rc.Control == _bocListStub && rc.Writer == _htmlTextWriterStub && rc.ColumnIndex == 0
                      && rc.ColumnDefinition == _columnDefinition),
              Arg.Is (0),
              Arg.Is (true),
              Arg.Is (dataRowRenderEventArgs)));
      _columnRenderMock.Replay();

      _columnRendererAdapter.RenderDataCell (_renderingContext, 0, dataRowRenderEventArgs);

      _columnRenderMock.VerifyAllExpectations();
    }
  }
}