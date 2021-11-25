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
using System.Web.UI.WebControls;
using Moq;
using NUnit.Framework;
using Remotion.Web.Services;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.Services
{
  [TestFixture]
  public class IconProxyTest
  {
    private Mock<HttpContextBase> _httpContextStub;

    [SetUp]
    public void SetUp ()
    {
      _httpContextStub = new Mock<HttpContextBase>();
      var httpRequestStub = new Mock<HttpRequestBase>();
      httpRequestStub.Setup(_ => _.Url).Returns(new Uri("http://localhost/root/file"));
      httpRequestStub.Setup(_ => _.ApplicationPath).Returns("/root");
      _httpContextStub.Setup(stub => stub.Request).Returns(httpRequestStub.Object);
    }

    [Test]
    public void Create_Complete ()
    {
      var iconInfo = new IconInfo("~/url") { AlternateText = "Alt", ToolTip = "ToolTip", Width = Unit.Pixel(4), Height = Unit.Percentage(8) };

      var iconProxy = IconProxy.Create(_httpContextStub.Object, iconInfo);

      Assert.That(iconProxy.Url, Is.EqualTo("/root/url"));
      Assert.That(iconProxy.AlternateText, Is.EqualTo("Alt"));
      Assert.That(iconProxy.ToolTip, Is.EqualTo("ToolTip"));
      Assert.That(iconProxy.Width, Is.EqualTo("4px"));
      Assert.That(iconProxy.Height, Is.EqualTo("8%"));
    }

    [Test]
    public void Create_OnlyUrl ()
    {
      var iconInfo = new IconInfo("~/url");

      var iconProxy = IconProxy.Create(_httpContextStub.Object, iconInfo);

      Assert.That(iconProxy.Url, Is.EqualTo("/root/url"));
      Assert.That(iconProxy.AlternateText, Is.Null);
      Assert.That(iconProxy.ToolTip, Is.Null);
      Assert.That(iconProxy.Width, Is.Null);
      Assert.That(iconProxy.Height, Is.Null);
    }

    [Test]
    public void Create_UrlMissing ()
    {
      var iconInfo = new IconInfo();

      Assert.That(
          () => IconProxy.Create(_httpContextStub.Object, iconInfo),
          Throws.ArgumentException.With.Message.StartsWith("IconProxy does not support IconInfo objects without an empty Url."));
    }
  }
}
