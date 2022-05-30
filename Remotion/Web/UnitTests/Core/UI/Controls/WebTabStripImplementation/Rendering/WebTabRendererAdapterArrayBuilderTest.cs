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
using Remotion.Development.UnitTesting;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Hotkey;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation;
using Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering;

namespace Remotion.Web.UnitTests.Core.UI.Controls.WebTabStripImplementation.Rendering
{
  [TestFixture]
  public class WebTabRendererAdapterArrayBuilderTest
  {
    [Test]
    public void GetWebTabRenderers_OneTab ()
    {
      var webTabMock = new Mock<IWebTab>();
      var fakeWebTabRenderer = CreateWebTabRenderer();
      var styleStub = new Mock<WebTabStyle>().Object;
      var builder = new WebTabRendererAdapterArrayBuilder(new[] { webTabMock.Object }, styleStub, styleStub);

      webTabMock.Setup(mock => mock.GetRenderer()).Returns(fakeWebTabRenderer).Verifiable();

      var result = builder.GetWebTabRenderers();

      webTabMock.Verify();
      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof(WebTabRendererAdapter)));
      Assert.That(result[0].IsLast, Is.True);
      Assert.That(result[0].WebTab, Is.SameAs(webTabMock.Object));
      Assert.That(PrivateInvoke.GetNonPublicField(result[0], "_webTabRenderer"), Is.SameAs(fakeWebTabRenderer));
    }

    [Test]
    public void GetWebTabRenderers_ThreeTabs ()
    {
      var webTabMock1 = new Mock<IWebTab>();
      var webTabMock2 = new Mock<IWebTab>();
      var webTabMock3 = new Mock<IWebTab>();
      var fakeWebTabRenderer = CreateWebTabRenderer();
      var styleStub = new Mock<WebTabStyle>().Object;
      var builder = new WebTabRendererAdapterArrayBuilder(new[] { webTabMock1.Object, webTabMock2.Object, webTabMock3.Object }, styleStub, styleStub);

      webTabMock1.Setup(mock => mock.GetRenderer()).Returns(fakeWebTabRenderer).Verifiable();
      webTabMock2.Setup(mock => mock.GetRenderer()).Returns(fakeWebTabRenderer).Verifiable();
      webTabMock3.Setup(mock => mock.GetRenderer()).Returns(fakeWebTabRenderer).Verifiable();

      var result = builder.GetWebTabRenderers();

      webTabMock1.Verify();
      webTabMock2.Verify();
      webTabMock3.Verify();
      Assert.That(result.Length, Is.EqualTo(3));

      Assert.That(result[0], Is.TypeOf(typeof(WebTabRendererAdapter)));
      Assert.That(result[0].IsLast, Is.False);
      Assert.That(result[0].WebTab, Is.SameAs(webTabMock1.Object));
      Assert.That(PrivateInvoke.GetNonPublicField(result[0], "_webTabRenderer"), Is.SameAs(fakeWebTabRenderer));

      Assert.That(result[1], Is.TypeOf(typeof(WebTabRendererAdapter)));
      Assert.That(result[1].IsLast, Is.False);
      Assert.That(result[1].WebTab, Is.SameAs(webTabMock2.Object));
      Assert.That(PrivateInvoke.GetNonPublicField(result[1], "_webTabRenderer"), Is.SameAs(fakeWebTabRenderer));

      Assert.That(result[2], Is.TypeOf(typeof(WebTabRendererAdapter)));
      Assert.That(result[2].IsLast, Is.True);
      Assert.That(result[2].WebTab, Is.SameAs(webTabMock3.Object));
      Assert.That(PrivateInvoke.GetNonPublicField(result[2], "_webTabRenderer"), Is.SameAs(fakeWebTabRenderer));
    }

    private WebTabRenderer CreateWebTabRenderer ()
    {
      return new WebTabRenderer(new NoneHotkeyFormatter(), RenderingFeatures.Default);
    }
  }
}
