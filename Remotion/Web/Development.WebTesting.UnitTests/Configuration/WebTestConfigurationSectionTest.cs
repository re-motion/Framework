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
using System.Configuration;
using System.IO;
using System.Xml.Schema;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chromium;

namespace Remotion.Web.Development.WebTesting.UnitTests.Configuration
{
  [TestFixture]
  public class WebTestConfigurationSectionTest
  {
    private const string c_fullConfigurationXml = @"
<remotion.webTesting xmlns=""http://www.re-motion.org/WebTesting/Configuration/2.0""
    browser=""Chrome""
    commandTimeout=""00:00:41""
    searchTimeout=""00:00:43""
    retryInterval=""00:00:00.042""
    webApplicationRoot=""http://some.url:1337/""
    screenshotDirectory="".\SomeScreenshotDirectory""
    downloadStartedTimeout=""00:00:13""
    downloadUpdatedTimeout=""00:00:37""
    asyncJavaScriptTimeout=""00:42:00""
    logsDirectory="".\SomeLogsDirectory""
    closeBrowserWindowsOnSetUpAndTearDown=""false""
    cleanUpUnmatchedDownloadedFiles=""false""
    requestErrorDetectionStrategy=""requestErrorDetectionStrategy"">
  <hosting name=""IisExpress"" type=""IisExpress"" port=""60042"" />
  <testSiteLayout rootPath="".\Some\Path"">
    <resources>
      <add path="".\Some\Resource"" />
    </resources>
  </testSiteLayout>
  <chrome disableSecurityWarningsBehavior=""Require"" />
  <edge disableSecurityWarningsBehavior=""Automatic"" />
</remotion.webTesting>";

    private WebTestConfigurationSection _section;

    [SetUp]
    public void SetUp ()
    {
      _section = (WebTestConfigurationSection)Activator.CreateInstance(typeof(WebTestConfigurationSection), true);
    }

    [Test]
    public void FullConfiguration ()
    {
      DeserializeSection(c_fullConfigurationXml);

      Assert.That(_section.BrowserName, Is.EqualTo("Chrome"));
      Assert.That(_section.CommandTimeout, Is.EqualTo(TimeSpan.FromSeconds(41)));
      Assert.That(_section.SearchTimeout, Is.EqualTo(TimeSpan.FromSeconds(43)));
      Assert.That(_section.RetryInterval, Is.EqualTo(TimeSpan.FromMilliseconds(42)));
      Assert.That(_section.WebApplicationRoot, Is.EqualTo("http://some.url:1337/"));
      Assert.That(_section.ScreenshotDirectory, Is.EqualTo(Path.GetFullPath(@".\SomeScreenshotDirectory")));
      Assert.That(_section.HostingProviderSettings.Name, Is.EqualTo("IisExpress"));
      Assert.That(_section.HostingProviderSettings.Parameters["port"], Is.EqualTo("60042"));
      Assert.That(_section.HostingProviderSettings.Type, Is.EqualTo("IisExpress"));
      Assert.That(_section.TestSiteLayoutConfiguration.RootPath, Is.EqualTo(@".\Some\Path"));
      Assert.That(_section.TestSiteLayoutConfiguration.Resources.Count, Is.EqualTo(1));
      Assert.That(_section.TestSiteLayoutConfiguration.Resources[0].Path, Is.EqualTo(@".\Some\Resource"));
      Assert.That(_section.DownloadStartedTimeout, Is.EqualTo(TimeSpan.FromSeconds(13)));
      Assert.That(_section.DownloadUpdatedTimeout, Is.EqualTo(TimeSpan.FromSeconds(37)));
      Assert.That(_section.AsyncJavaScriptTimeout, Is.EqualTo(TimeSpan.FromMinutes(42)));
      Assert.That(_section.LogsDirectory, Is.EqualTo(@".\SomeLogsDirectory"));
      Assert.That(_section.CloseBrowserWindowsOnSetUpAndTearDown, Is.EqualTo(false));
      Assert.That(_section.CleanUpUnmatchedDownloadedFiles, Is.EqualTo(false));
      Assert.That(_section.RequestErrorDetectionStrategyTypeName, Is.EqualTo("requestErrorDetectionStrategy"));
      Assert.That(_section.Chrome.DisableSecurityWarningsBehavior, Is.EqualTo(ChromiumDisableSecurityWarningsBehavior.Require));
      Assert.That(_section.Edge.DisableSecurityWarningsBehavior, Is.EqualTo(ChromiumDisableSecurityWarningsBehavior.Automatic));
    }

    [Test]
    public void BrowserAttribute_Required ()
    {
      const string configurationWithoutBrowser = @"<remotion.webTesting xmlns=""http://www.re-motion.org/WebTesting/Configuration/2.0"" />";
      Assert.That(
          () => DeserializeSection(configurationWithoutBrowser),
          Throws.InstanceOf<ConfigurationErrorsException>().With.Message.EqualTo("Required attribute 'browser' not found."));
    }

    [Test]
    public void SearchTimeoutAttribute_Required ()
    {
      const string configurationWithoutSearchTimeout = @"<remotion.webTesting xmlns=""http://www.re-motion.org/WebTesting/Configuration/2.0"" browser=""Chrome"" />";
      Assert.That(
          () => DeserializeSection(configurationWithoutSearchTimeout),
          Throws.InstanceOf<ConfigurationErrorsException>().With.Message.EqualTo("Required attribute 'searchTimeout' not found."));
    }

    [Test]
    public void RetryIntervalAttribute_Required ()
    {
      const string configurationWithoutRetryInterval =
          @"<remotion.webTesting xmlns=""http://www.re-motion.org/WebTesting/Configuration/2.0"" browser=""Chrome"" searchTimeout=""00:00:43"" />";
      Assert.That(
          () => DeserializeSection(configurationWithoutRetryInterval),
          Throws.InstanceOf<ConfigurationErrorsException>().With.Message.EqualTo("Required attribute 'retryInterval' not found."));
    }

    [Test]
    public void WebApplicationRootAttribute_Required ()
    {
      const string configurationWithoutWebApplicationRoot =
          @"<remotion.webTesting xmlns=""http://www.re-motion.org/WebTesting/Configuration/2.0"" browser=""Chrome"" searchTimeout=""00:00:43"" retryInterval=""00:00:00.042"" />";
      Assert.That(
          () => DeserializeSection(configurationWithoutWebApplicationRoot),
          Throws.InstanceOf<ConfigurationErrorsException>().With.Message.EqualTo("Required attribute 'webApplicationRoot' not found."));
    }

    [Test]
    public void MissingHostingProperty_FailingXmlSchemaValidation ()
    {
      const string failingSchemaValidation =
          @"<remotion.webTesting xmlns=""http://www.re-motion.org/WebTesting/Configuration/2.0"" browser=""Chrome"" searchTimeout=""00:00:43"" retryInterval=""00:00:00.042"" webApplicationRoot=""http://some.url:1337/"" />";
      Assert.That(
          () => DeserializeSection(failingSchemaValidation),
          Throws.InstanceOf<XmlSchemaValidationException>());
    }

    [Test]
    public void WithoutBrowserConfigurationElements_NoException ()
    {
      const string configurationWithoutChromiumConfigurationElements =
          @"<remotion.webTesting xmlns=""http://www.re-motion.org/WebTesting/Configuration/2.0"" browser=""Chrome"" searchTimeout=""00:00:43"" retryInterval=""00:00:00.042"" webApplicationRoot=""http://some.url:1337/""><hosting name=""IisExpress"" type=""IisExpress"" port=""60042""/><testSiteLayout rootPath="".\Some\Path""/></remotion.webTesting>";
      Assert.That(
          () => DeserializeSection(configurationWithoutChromiumConfigurationElements),
          Throws.Nothing);
    }

    [Test]
    public void WithoutChromeConfigurationElement_NoException ()
    {
      const string configurationWithoutChromeConfigurationElement =
          @"<remotion.webTesting xmlns=""http://www.re-motion.org/WebTesting/Configuration/2.0"" browser=""Chrome"" searchTimeout=""00:00:43"" retryInterval=""00:00:00.042"" webApplicationRoot=""http://some.url:1337/""><hosting name=""IisExpress"" type=""IisExpress"" port=""60042""/><testSiteLayout rootPath="".\Some\Path""/><edge /></remotion.webTesting>";
      Assert.That(
          () => DeserializeSection(configurationWithoutChromeConfigurationElement),
          Throws.Nothing);
    }

    [Test]
    public void WithoutEdgeConfigurationElement_NoException ()
    {
      const string configurationWithoutEdgeConfigurationElement =
          @"<remotion.webTesting xmlns=""http://www.re-motion.org/WebTesting/Configuration/2.0"" browser=""Chrome"" searchTimeout=""00:00:43"" retryInterval=""00:00:00.042"" webApplicationRoot=""http://some.url:1337/""><hosting name=""IisExpress"" type=""IisExpress"" port=""60042""/><testSiteLayout rootPath="".\Some\Path""/><chrome /></remotion.webTesting>";
      Assert.That(
          () => DeserializeSection(configurationWithoutEdgeConfigurationElement),
          Throws.Nothing);
    }

    private void DeserializeSection (string xmlFragment)
    {
      var xsdContent = GetSchemaContent();

      ConfigurationHelper.DeserializeSection(_section, xmlFragment, xsdContent);
    }

    private string GetSchemaContent ()
    {
      var assembly = typeof(WebTestConfigurationSection).Assembly;
      using (var resourceStream = assembly.GetManifestResourceStream("Remotion.Web.Development.WebTesting.Schemas.WebTestingConfiguration.xsd"))
      using (var reader = new StreamReader(resourceStream))
        return reader.ReadToEnd();
    }
  }
}
