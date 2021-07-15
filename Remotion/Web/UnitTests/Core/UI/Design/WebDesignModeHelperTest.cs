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
using System.ComponentModel.Design;
using System.Configuration;
using System.Web.UI.Design;
using Moq;
using NUnit.Framework;
using Remotion.Web.UI.Design;

namespace Remotion.Web.UnitTests.Core.UI.Design
{
  [TestFixture]
  public class WebDesignModeHelperTest
  {
    private Mock<IDesignerHost> _mockDesignerHost;
    private Mock<IWebApplication> _mockWebApplication;

    [SetUp]
    public void SetUp()
    {
      _mockDesignerHost = new Mock<IDesignerHost> (MockBehavior.Strict);
      _mockWebApplication = new Mock<IWebApplication> (MockBehavior.Strict);
    }

    [Test]
    public void Initialize()
    {
      WebDesginModeHelper webDesginModeHelper = new WebDesginModeHelper (_mockDesignerHost.Object);

      _mockDesignerHost.Verify();
      _mockWebApplication.Verify();
      Assert.That (webDesginModeHelper.DesignerHost, Is.SameAs (_mockDesignerHost.Object));
    }

    [Test]
    public void GetConfiguration()
    {
      System.Configuration.Configuration expected = ConfigurationManager.OpenExeConfiguration (ConfigurationUserLevel.None);
      _mockDesignerHost.Setup (_ => _.GetService (typeof (IWebApplication))).Returns (_mockWebApplication.Object).Verifiable();
      _mockWebApplication.Setup (_ => _.OpenWebConfiguration (true)).Returns (expected).Verifiable();

      WebDesginModeHelper webDesginModeHelper = new WebDesginModeHelper (_mockDesignerHost.Object);

      System.Configuration.Configuration actual = webDesginModeHelper.GetConfiguration();

      _mockDesignerHost.Verify();
      _mockWebApplication.Verify();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void GetProjectPath()
    {
      var mockProjectItem = new Mock<IProjectItem> (MockBehavior.Strict);

      _mockDesignerHost.Setup (_ => _.GetService (typeof (IWebApplication))).Returns (_mockWebApplication.Object).Verifiable();
      _mockWebApplication.Setup (_ => _.RootProjectItem).Returns (mockProjectItem.Object).Verifiable();
      mockProjectItem.Setup (_ => _.PhysicalPath).Returns ("TheProjectPath").Verifiable();

      WebDesginModeHelper webDesginModeHelper = new WebDesginModeHelper (_mockDesignerHost.Object);

      string actual = webDesginModeHelper.GetProjectPath();

      _mockDesignerHost.Verify();
      _mockWebApplication.Verify();
      mockProjectItem.Verify();
      Assert.That (actual, Is.EqualTo ("TheProjectPath"));
    }
  }
}
