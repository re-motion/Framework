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
using System.Drawing;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  /// <summary>
  /// Base class for all integration tests.
  /// </summary>
  public abstract class IntegrationTest
  {
    private readonly Brush _screenshotWhiteBrush = new SolidBrush (Color.White);
    private readonly Brush _screenshotTransparentBrush = new SolidBrush (Color.Transparent);

    private WebTestHelper _webTestHelper;

    protected virtual bool MaximizeMainBrowserSession
    {
      get { return true; }
    }

    protected WebTestHelper Helper
    {
      get { return _webTestHelper; }
    }

    /// <summary>
    /// This brush is used to cover up element details that are not relevant when testing the fluent screenshot API.
    /// This allows a test screenshot to be used with different browsers, although the browsers render details differently.
    /// If irrelevant test details would not be covered up a test screenshot for each configuration would be needed (see DropDownMenu).
    /// In order to debug test screenshot errors use the <see cref="ScreenshotBackgroundBrushDebug"/> instead of <see cref="ScreenshotBackgroundBrush"/>.
    /// </summary>
    protected Brush ScreenshotBackgroundBrush
    {
      get { return _screenshotWhiteBrush; }
    }

    /// <summary>
    /// Brush that should be used when debugging fluent screenshot API.
    /// For deployment use <see cref="ScreenshotBackgroundBrush"/>.
    /// For more information about the use case <see cref="ScreenshotBackgroundBrush"/>.
    /// </summary>
    protected Brush ScreenshotBackgroundBrushDebug
    {
      get { return _screenshotTransparentBrush; }
    }

    [TestFixtureSetUp]
    public void IntegrationTestTestFixtureSetUp ()
    {
      _webTestHelper = WebTestHelper.CreateFromConfiguration<CustomWebTestConfigurationFactory>();

      _webTestHelper.OnFixtureSetUp (MaximizeMainBrowserSession);
    }

    [SetUp]
    public void IntegrationTestSetUp ()
    {
      _webTestHelper.OnSetUp (GetType().Name + "_" + TestContext.CurrentContext.Test.Name);

      // Prevent failing IE tests due to topmost windows
      if (_webTestHelper.BrowserConfiguration.IsInternetExplorer())
        KillAnyExistingWindowsErrorReportingProcesses();
    }

    [TearDown]
    public void IntegrationTestTearDown ()
    {
      var hasSucceeded = TestContext.CurrentContext.Result.Status != TestStatus.Failed;
      _webTestHelper.OnTearDown (hasSucceeded);
    }

    [TestFixtureTearDown]
    public void IntegrationTestTestFixtureTearDown ()
    {
      _webTestHelper.OnFixtureTearDown();
    }

    protected WxePageObject Start (string userControl)
    {
      var userControlUrl = string.Format ("Controls/{0}UserControl.ascx", userControl);

      var url = string.Format ("{0}ControlTest.wxe?UserControl={1}", _webTestHelper.TestInfrastructureConfiguration.WebApplicationRoot, userControlUrl);
      _webTestHelper.MainBrowserSession.Window.Visit (url);
      _webTestHelper.AcceptPossibleModalDialog();

      return _webTestHelper.CreateInitialPageObject<WxePageObject> (_webTestHelper.MainBrowserSession);
    }

    private static void KillAnyExistingWindowsErrorReportingProcesses ()
    {
      ProcessUtils.KillAllProcessesWithName ("WerFault");
    }
  }
}