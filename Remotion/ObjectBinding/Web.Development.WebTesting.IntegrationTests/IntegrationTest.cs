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
using System.Diagnostics;
using Coypu;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  /// <summary>
  /// Base class for all integration tests.
  /// </summary>
  public abstract class IntegrationTest
  {
    private WebTestHelper _webTestHelper;

    protected virtual WindowSize WindowSize
    {
      get { return new WindowSize(1296, 868); }
    }

    protected WebTestHelper Helper
    {
      get { return _webTestHelper; }
    }

    protected IDriver Driver
    {
      get { return _webTestHelper.MainBrowserSession?.Driver; }
    }

    [OneTimeSetUp]
    public void IntegrationTestOneTimeSetUp ()
    {
      DriverConfigurationOverride driverConfigurationOverride = null;
      if (Debugger.IsAttached)
        driverConfigurationOverride = new DriverConfigurationOverride { Headless = false };

      _webTestHelper = WebTestHelper.CreateFromConfiguration<CustomWebTestConfigurationFactory>();

      _webTestHelper.OnFixtureSetUp(WindowSize, driverConfigurationOverride);
    }

    [SetUp]
    public void IntegrationTestSetUp ()
    {
      _webTestHelper.OnSetUp(GetType().Name + "_" + TestContext.CurrentContext.Test.Name);
    }

    [TearDown]
    public void IntegrationTestTearDown ()
    {
      var hasSucceeded = TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Failed;
      _webTestHelper.OnTearDown(hasSucceeded);
    }

    [OneTimeTearDown]
    public void IntegrationTestTestFixtureTearDown ()
    {
      _webTestHelper.OnFixtureTearDown();
    }

    protected WxePageObject Start (string userControl)
    {
      var userControlUrl = string.Format("Controls/{0}UserControl.ascx", userControl);

      var url = string.Format("{0}ControlTest.wxe?UserControl={1}", _webTestHelper.TestInfrastructureConfiguration.WebApplicationRoot, userControlUrl);
      _webTestHelper.MainBrowserSession.Window.Visit(url);
      _webTestHelper.AcceptPossibleModalDialog();

      return _webTestHelper.CreateInitialPageObject<WxePageObject>(_webTestHelper.MainBrowserSession);
    }
  }
}
