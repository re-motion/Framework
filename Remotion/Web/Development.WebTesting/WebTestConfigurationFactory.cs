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
using Coypu.Drivers;
using JetBrains.Annotations;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Accessibility;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.HostingStrategies.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Edge;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Firefox;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// The <see cref="WebTestConfigurationFactory"/> is used by the <see cref="WebTestHelper"/> and <see cref="WebTestSetUpFixtureHelper"/> 
  /// to create configuration objects based on the app.config and/or project specific logic.
  /// </summary>
  /// <remarks>
  /// In order to customize the test setup, derive from <see cref="WebTestConfigurationFactory"/>, override the template factory methods,
  /// and supply the new factory type via <see cref="WebTestHelper"/>.<see cref="WebTestHelper.CreateFromConfiguration{TFactory}()"/> and 
  /// <see cref="WebTestSetUpFixtureHelper"/>.<see cref="WebTestSetUpFixtureHelper.CreateFromConfiguration{TFactory}()"/>.
  /// </remarks>
  public class WebTestConfigurationFactory
  {
    /// <summary>
    /// Represents the latest tested version of Chrome, compatible with the framework.
    /// In order to achieve a stable testing environment, a standalone Chrome browser with a matching ChromeDriver version should be used.
    /// </summary>
    protected const string LatestTestedChromeVersion = "110";

    /// <summary>
    /// Represents the latest version of Edge verified to be compatible with the framework.
    /// In order to achieve a stable testing environment, a standalone Edge browser with a matching MSEdgeDriver version should be used.
    /// </summary>
    protected const string LatestTestedEdgeVersion = "110";

    /// <summary>
    /// Represents the latest version of Firefox verified to be compatible with Selenium WebDriver.
    /// In order to achieve a stable testing environment a standalone Firefox with a matching GeckoDriver version should be used.
    /// </summary>
    protected const string LatestTestedFirefoxVersion = "121";

    /// <summary>
    /// Creates a new <see cref="IBrowserConfiguration"/> from app.config.
    /// </summary>
    /// <remarks>
    /// In order to customize how the respective WebDriver for Chrome/other browsers is instantiated,
    /// override the <see cref="CreateChromeConfiguration"/>,
    /// and <see cref="CreateCustomBrowserConfiguration"/> methods.
    /// </remarks>
    public IBrowserConfiguration CreateBrowserConfiguration ()
    {
      var configSettings = WebTestSettings.Current;

      var configuredBrowser = Browser.Parse(configSettings.BrowserName);

      if (configuredBrowser == Browser.Chrome)
        return CreateChromeConfiguration(configSettings);

      if (configuredBrowser == Browser.Edge)
        return CreateEdgeConfiguration(configSettings);

      if (configuredBrowser == Browser.Firefox)
        return CreateFirefoxConfiguration(configSettings);

      return CreateCustomBrowserConfiguration(configSettings);
    }

    /// <summary>
    /// Creates a new <see cref="DriverConfiguration"/> from app.config.
    /// </summary>
    public DriverConfiguration CreateDriverConfiguration ()
    {
      var configSettings = WebTestSettings.Current;

      return new DriverConfiguration(
          configSettings.CommandTimeout,
          configSettings.SearchTimeout,
          configSettings.RetryInterval,
          configSettings.AsyncJavaScriptTimeout,
          configSettings.Headless);
    }

    /// <summary>
    /// Creates a new <see cref="ITestInfrastructureConfiguration"/> from app.config.
    /// </summary>
    public ITestInfrastructureConfiguration CreateTestInfrastructureConfiguration ()
    {
      var configSettings = WebTestSettings.Current;

      //Not extensible as it is used in user code and test infrastructure utilities 
      return new TestInfrastructureConfiguration(configSettings);
    }

    /// <summary>
    /// Creates a new <see cref="IHostingConfiguration"/> from app.config
    /// </summary>
    /// <returns></returns>
    public IHostingConfiguration CreateHostingConfiguration ()
    {
      var configSettings = WebTestSettings.Current;

      return CreateHostingConfiguration(configSettings);
    }

    /// <summary>
    /// Responsible for handling Browsers other than Chrome, Edge, and Firefox.
    /// Note that the <see cref="WebTestConfigurationFactory"/> itself does not provide support for other browsers so this implementation throws an not supported exception.
    /// </summary>
    /// <param name="configSettings">Receives app.config settings when called in <see cref="CreateBrowserConfiguration"/></param>
    protected virtual IBrowserConfiguration CreateCustomBrowserConfiguration ([NotNull] IWebTestSettings configSettings)
    {
      ArgumentUtility.CheckNotNull("configSettings", configSettings);

      throw new NotSupportedException(string.Format("Browser '{0}' is not supported by the '{1}'.", configSettings.BrowserName, GetType().Name));
    }

    /// <summary>
    /// Responsible for creating a Chrome specific configuration object.
    /// </summary>
    /// <param name="configSettings">Receives app.config settings when called in <see cref="CreateBrowserConfiguration"/></param>
    /// <remarks>
    /// Override this method to customize the configuration settings, e.g. the location of the chrome.exe and the user directory 
    /// via <see cref="ChromeExecutable"/> or custom <see cref="ChromeOptions"/> by extending <see cref="ChromeConfiguration"/> itself.
    /// </remarks>
    protected virtual IChromeConfiguration CreateChromeConfiguration ([NotNull] IWebTestSettings configSettings)
    {
      ArgumentUtility.CheckNotNull("configSettings", configSettings);

      return new ChromeConfiguration(configSettings);
    }

    /// <summary>
    /// Responsible for creating an Edge specific configuration object.
    /// </summary>
    /// <param name="configSettings">Receives app.config settings when called in <see cref="CreateBrowserConfiguration"/></param>
    /// <remarks>
    /// Override this method to customize the configuration settings, e.g. the location of the msedge.exe and the user directory 
    /// via <see cref="EdgeExecutable"/> or custom <see cref="EdgeOptions"/> by extending <see cref="EdgeConfiguration"/> itself.
    /// </remarks>
    protected virtual IEdgeConfiguration CreateEdgeConfiguration ([NotNull] IWebTestSettings configSettings)
    {
      ArgumentUtility.CheckNotNull("configSettings", configSettings);

      return new EdgeConfiguration(configSettings);
    }

    protected virtual IFirefoxConfiguration CreateFirefoxConfiguration (IWebTestSettings configSettings)
    {
      ArgumentUtility.CheckNotNull("configSettings", configSettings);

      return new FirefoxConfiguration(configSettings);
    }

    /// <summary>
    /// Responsible for creating a configuration object for the hosting infrastructure.
    /// </summary>
    /// <param name="configSettings">Receives app.config settings when called in <see cref="CreateBrowserConfiguration"/></param>
    /// <remarks>
    /// Override this method when you need to introduce a new hosting strategy.
    /// </remarks>
    protected virtual IHostingConfiguration CreateHostingConfiguration ([NotNull] IWebTestSettings configSettings)
    {
      ArgumentUtility.CheckNotNull("configSettings", configSettings);

      var testSiteLayoutConfiguration = CreateTestSiteLayoutConfiguration();

      return new HostingConfiguration(configSettings, testSiteLayoutConfiguration);
    }

    /// <summary>
    /// Creates an instance of <see cref="AccessibilityConfiguration"/> with default values.
    /// </summary>
    public virtual IAccessibilityConfiguration CreateAccessibilityConfiguration ()
    {
      return new AccessibilityConfiguration();
    }

    private ITestSiteLayoutConfiguration CreateTestSiteLayoutConfiguration ()
    {
      var configSettings = WebTestSettings.Current;

      return new TestSiteLayoutConfiguration(configSettings);
    }
  }
}
