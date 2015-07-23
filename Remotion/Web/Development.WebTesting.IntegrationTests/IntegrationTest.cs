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
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.PageObjects;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  /// <summary>
  /// Base class for all integration tests.
  /// </summary>
  public abstract class IntegrationTest
  {
    private readonly WebTestHelper _webTestHelper = WebTestHelper.CreateFromConfiguration();

    protected WebTestHelper Helper
    {
      get { return _webTestHelper; }
    }

    [TestFixtureSetUp]
    public void IntegrationTestTestFixtureSetUp ()
    {
      _webTestHelper.OnFixtureSetUp();
    }

    [SetUp]
    public void IntegrationTestSetUp ()
    {
      _webTestHelper.OnSetUp (GetType().Name + "_" + TestContext.CurrentContext.Test.Name);

      // Prevent failing IE tests due to topmost windows
      if (WebTestingConfiguration.Current.BrowserIsInternetExplorer())
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

    protected TPageObject Start<TPageObject> (string page)
      where TPageObject : PageObject
    {
      var url = WebTestingConfiguration.Current.WebApplicationRoot + page;
      _webTestHelper.MainBrowserSession.Visit (url);

      return _webTestHelper.CreateInitialPageObject<TPageObject> (_webTestHelper.MainBrowserSession);
    }

    protected DownloadHelper NewDownloadHelper (string fileName)
    {
      return _webTestHelper.NewDownloadHelper (fileName);
    }

    private static void KillAnyExistingWindowsErrorReportingProcesses ()
    {
      ProcessUtils.KillAllProcessesWithName ("WerFault");
    }
  }
}