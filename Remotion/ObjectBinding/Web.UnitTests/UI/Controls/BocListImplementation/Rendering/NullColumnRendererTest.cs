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
  public class NullColumnRendererTest
  {
    private NullColumnRenderer _nullColumnRenderer;
    private HtmlTextWriter _htmlTextWriterMock;
    private HttpContextBase _httpContextStub;
    private IBocList _bocListStub;
    private BocColumnRenderingContext<StubColumnDefinition> _renderingContext;
    private StubColumnDefinition _columnDefinition;

    [SetUp]
    public void SetUp ()
    {
      _nullColumnRenderer = new NullColumnRenderer();
      _columnDefinition = new StubColumnDefinition();
      _htmlTextWriterMock = MockRepository.GenerateStrictMock<HtmlTextWriter> ();
      _httpContextStub = MockRepository.GenerateStub<HttpContextBase> ();
      _bocListStub = MockRepository.GenerateStub<IBocList> ();
      _renderingContext = new BocColumnRenderingContext<StubColumnDefinition> (
          new BocColumnRenderingContext(_httpContextStub, _htmlTextWriterMock, _bocListStub, _columnDefinition, 0, 0));
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_nullColumnRenderer.IsNull, Is.True);
      Assert.That (_nullColumnRenderer.Column, Is.Null);
    }

    [Test]
    public void RenderTitleCell ()
    {
      _htmlTextWriterMock.Replay();

      _nullColumnRenderer.RenderTitleCell (_renderingContext, SortingDirection.None, 0);

      _htmlTextWriterMock.VerifyAllExpectations();
    }

    [Test]
    public void RenderDataCell ()
    {
      _htmlTextWriterMock.Replay ();

      _nullColumnRenderer.RenderDataCell (_renderingContext, 0, true, null);

      _htmlTextWriterMock.VerifyAllExpectations ();
    }

    [Test]
    public void RenderDataColumnDeclaration ()
    {
      _htmlTextWriterMock.Replay();

      _nullColumnRenderer.RenderDataColumnDeclaration (_renderingContext, false);

      _htmlTextWriterMock.VerifyAllExpectations();
    }

  }
}