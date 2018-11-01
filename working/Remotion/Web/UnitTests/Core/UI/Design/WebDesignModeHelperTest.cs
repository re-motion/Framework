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
using NUnit.Framework;
using Remotion.Web.UI.Design;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Design
{
  [TestFixture]
  public class WebDesignModeHelperTest
  {
    private MockRepository _mockRepository;
    private IDesignerHost _mockDesignerHost;
    private IWebApplication _mockWebApplication;

    [SetUp]
    public void SetUp()
    {
      _mockRepository = new MockRepository();
      _mockDesignerHost = _mockRepository.StrictMock<IDesignerHost> ();
      _mockWebApplication = _mockRepository.StrictMock<IWebApplication>();
    }

    [Test]
    public void Initialize()
    {
      _mockRepository.ReplayAll();

      WebDesginModeHelper webDesginModeHelper = new WebDesginModeHelper (_mockDesignerHost);

      _mockRepository.VerifyAll();
      Assert.That (webDesginModeHelper.DesignerHost, Is.SameAs (_mockDesignerHost));
    }

    [Test]
    public void GetConfiguration()
    {
      System.Configuration.Configuration expected = ConfigurationManager.OpenExeConfiguration (ConfigurationUserLevel.None);
      Expect.Call (_mockDesignerHost.GetService (typeof (IWebApplication))).Return (_mockWebApplication);
      Expect.Call (_mockWebApplication.OpenWebConfiguration (true)).Return (expected);
      _mockRepository.ReplayAll();

      WebDesginModeHelper webDesginModeHelper = new WebDesginModeHelper (_mockDesignerHost);

      System.Configuration.Configuration actual = webDesginModeHelper.GetConfiguration();

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void GetProjectPath()
    {
      IProjectItem mockProjectItem = _mockRepository.StrictMock<IProjectItem>();

      Expect.Call (_mockDesignerHost.GetService (typeof (IWebApplication))).Return (_mockWebApplication);
      SetupResult.For (_mockWebApplication.RootProjectItem).Return (mockProjectItem);
      Expect.Call (mockProjectItem.PhysicalPath).Return ("TheProjectPath");
      _mockRepository.ReplayAll();

      WebDesginModeHelper webDesginModeHelper = new WebDesginModeHelper (_mockDesignerHost);

      string actual = webDesginModeHelper.GetProjectPath();

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("TheProjectPath"));
    }
  }
}
