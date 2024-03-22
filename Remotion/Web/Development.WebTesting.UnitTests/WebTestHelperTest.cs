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
using System.Linq;
using Coypu;
using Moq;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;
using Remotion.Web.Development.WebTesting.WebDriver.Factories;

namespace Remotion.Web.Development.WebTesting.UnitTests
{
  [TestFixture]
  public class WebTestHelperTest
  {
    private readonly TimeSpan _configCommandTimeout = TimeSpan.FromSeconds((13 * 60) + 37);
    private readonly TimeSpan _configSearchTimeout = TimeSpan.FromSeconds(30);
    private readonly TimeSpan _configRetryInterval = TimeSpan.FromMilliseconds(25);
    private readonly TimeSpan _configAsyncJavaScriptTimeout = TimeSpan.FromHours(12) + TimeSpan.FromMinutes(34);

    [Test]
    public void CreateNewBrowserSession_ValuesFromConfiguration_WithoutOverride ()
    {
      var webTestHelper = WebTestHelper.CreateFromConfiguration<TestWebTestConfigurationFactory>();
      var browserFactoryStub = webTestHelper.BrowserConfiguration.BrowserFactory;

      webTestHelper.CreateNewBrowserSession(new WindowSize(500, 500));

      var driverConfigurationArgument = GetBrowserFactoryStubCreateBrowserArgument(browserFactoryStub);
      Assert.That(driverConfigurationArgument.CommandTimeout, Is.EqualTo(_configCommandTimeout));
      Assert.That(driverConfigurationArgument.SearchTimeout, Is.EqualTo(_configSearchTimeout));
      Assert.That(driverConfigurationArgument.RetryInterval, Is.EqualTo(_configRetryInterval));
      Assert.That(driverConfigurationArgument.AsyncJavaScriptTimeout, Is.EqualTo(_configAsyncJavaScriptTimeout));
    }

    [Test]
    public void CreateNewBrowserSession_CommandTimeoutOverride ()
    {
      var webTestHelper = WebTestHelper.CreateFromConfiguration<TestWebTestConfigurationFactory>();
      var configurationOverride = new DriverConfigurationOverride { CommandTimeout = TimeSpan.FromMinutes(5) };
      var browserFactoryStub = webTestHelper.BrowserConfiguration.BrowserFactory;

      webTestHelper.CreateNewBrowserSession(new WindowSize(500, 500), configurationOverride);

      var driverConfigurationArgument = GetBrowserFactoryStubCreateBrowserArgument(browserFactoryStub);
      Assert.That(driverConfigurationArgument.CommandTimeout, Is.EqualTo(TimeSpan.FromMinutes(5)));
      Assert.That(driverConfigurationArgument.SearchTimeout, Is.EqualTo(_configSearchTimeout));
      Assert.That(driverConfigurationArgument.RetryInterval, Is.EqualTo(_configRetryInterval));
      Assert.That(driverConfigurationArgument.AsyncJavaScriptTimeout, Is.EqualTo(_configAsyncJavaScriptTimeout));
    }

    [Test]
    public void CreateNewBrowserSession_SearchTimeoutOverride ()
    {
      var webTestHelper = WebTestHelper.CreateFromConfiguration<TestWebTestConfigurationFactory>();
      var configurationOverride = new DriverConfigurationOverride { SearchTimeout = TimeSpan.FromMinutes(5) };
      var browserFactoryStub = webTestHelper.BrowserConfiguration.BrowserFactory;

      webTestHelper.CreateNewBrowserSession(new WindowSize(500, 500), configurationOverride);

      var driverConfigurationArgument = GetBrowserFactoryStubCreateBrowserArgument(browserFactoryStub);
      Assert.That(driverConfigurationArgument.CommandTimeout, Is.EqualTo(_configCommandTimeout));
      Assert.That(driverConfigurationArgument.SearchTimeout, Is.EqualTo(TimeSpan.FromMinutes(5)));
      Assert.That(driverConfigurationArgument.RetryInterval, Is.EqualTo(_configRetryInterval));
      Assert.That(driverConfigurationArgument.AsyncJavaScriptTimeout, Is.EqualTo(_configAsyncJavaScriptTimeout));
    }

    [Test]
    public void CreateNewBrowserSession_RetryIntervalOverride ()
    {
      var webTestHelper = WebTestHelper.CreateFromConfiguration<TestWebTestConfigurationFactory>();
      var configurationOverride = new DriverConfigurationOverride { RetryInterval = TimeSpan.FromMinutes(5) };
      var browserFactoryStub = webTestHelper.BrowserConfiguration.BrowserFactory;

      webTestHelper.CreateNewBrowserSession(new WindowSize(500, 500), configurationOverride);

      var driverConfigurationArgument = GetBrowserFactoryStubCreateBrowserArgument(browserFactoryStub);
      Assert.That(driverConfigurationArgument.CommandTimeout, Is.EqualTo(_configCommandTimeout));
      Assert.That(driverConfigurationArgument.SearchTimeout, Is.EqualTo(_configSearchTimeout));
      Assert.That(driverConfigurationArgument.RetryInterval, Is.EqualTo(TimeSpan.FromMinutes(5)));
      Assert.That(driverConfigurationArgument.AsyncJavaScriptTimeout, Is.EqualTo(_configAsyncJavaScriptTimeout));
    }

    [Test]
    public void CreateNewBrowserSession_AsyncJavaScriptTimeoutOverride ()
    {
      var webTestHelper = WebTestHelper.CreateFromConfiguration<TestWebTestConfigurationFactory>();
      var configurationOverride = new DriverConfigurationOverride { AsyncJavaScriptTimeout = TimeSpan.FromMinutes(5) };
      var browserFactoryStub = webTestHelper.BrowserConfiguration.BrowserFactory;

      webTestHelper.CreateNewBrowserSession(new WindowSize(500, 500), configurationOverride);

      var driverConfigurationArgument = GetBrowserFactoryStubCreateBrowserArgument(browserFactoryStub);
      Assert.That(driverConfigurationArgument.CommandTimeout, Is.EqualTo(_configCommandTimeout));
      Assert.That(driverConfigurationArgument.SearchTimeout, Is.EqualTo(_configSearchTimeout));
      Assert.That(driverConfigurationArgument.RetryInterval, Is.EqualTo(_configRetryInterval));
      Assert.That(driverConfigurationArgument.AsyncJavaScriptTimeout, Is.EqualTo(TimeSpan.FromMinutes(5)));
    }

    private DriverConfiguration GetBrowserFactoryStubCreateBrowserArgument (IBrowserFactory browserFactoryStub)
    {
      return (DriverConfiguration)Mock.Get(browserFactoryStub).Invocations.Single().Arguments.Single();
    }

    private class TestWebTestConfigurationFactory : WebTestConfigurationFactory
    {
      protected override IChromeConfiguration CreateChromeConfiguration (IWebTestSettings configSettings)
      {
        var browserFactoryStub = new Mock<IBrowserFactory>();
        var browserSessionStub = new Mock<IBrowserSession>();

        browserFactoryStub.Setup(_ => _.CreateBrowser(It.IsAny<DriverConfiguration>())).Returns(browserSessionStub.Object);
        browserSessionStub.Setup(_ => _.Driver).Returns(Mock.Of<IDriver>());

        var chromeConfigurationStub = new Mock<IChromeConfiguration>();
        chromeConfigurationStub
            .Setup(_ => _.BrowserFactory)
            .Returns(browserFactoryStub.Object);

        return chromeConfigurationStub.Object;
      }
    }
  }
}
