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
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.WebTabStripImplementation;
using Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls.WebTabStripImplementation.Rendering
{
  [TestFixture]
  public class WebTabRendererAdapterTest
  {
    private HttpContextBase _httpContextStub;
    private HtmlTextWriter _htmlTextWriterStub;

    [SetUp]
    public void SetUp ()
    {
      _httpContextStub = MockRepository.GenerateStub<HttpContextBase>();
      _htmlTextWriterStub = MockRepository.GenerateStub<HtmlTextWriter>();
    }

    [Test]
    public void Render ()
    {
      var webTabRendererMock = MockRepository.GenerateStrictMock<IWebTabRenderer>();
      var webTabStub = MockRepository.GenerateStub<IWebTab>();
      var renderingContext = new WebTabStripRenderingContext (
          _httpContextStub, _htmlTextWriterStub, new WebTabStripMock(), new WebTabRendererAdapter[0]);
       var webTabStyle = new WebTabStyle();

      webTabRendererMock.Expect (mock => mock.Render (renderingContext, webTabStub, true, true, webTabStyle));
      webTabRendererMock.Replay();

      var rendererAdapter = new WebTabRendererAdapter (webTabRendererMock, webTabStub, true, true, webTabStyle);
      rendererAdapter.Render (renderingContext);

      webTabRendererMock.VerifyAllExpectations();
    }

  }
}