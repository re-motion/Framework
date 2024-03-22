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
#if !NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.Configuration.IConfiguration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chromium;

namespace Remotion.Web.Development.WebTesting.UnitTests.Configuration.IConfiguration
{
  [TestFixture]
  public class WebTestSettingsDtoTest
  {
    [Test]
    public void Deserialize_MinimalConfig ()
    {
      var webTestSettings = DeserializeFromString(
          """
          {
            "Remotion": {
              "WebTesting": {
                "BrowserName": "Edge",
                "SearchTimeout": "00:00:06",
                "RetryInterval": "00:00:00.011",
                "WebApplicationRoot": "http://localhost:60401/",
                "Hosting": {
                  "Name": "IisExpress",
                  "Type": "IisExpressType"
                },
                "TestSiteLayout": {
                  "RootPath": "myRootPath"
                },
              }
            }
          }
          """);

      Assert.That(webTestSettings, Is.Not.Null);
      Assert.That(webTestSettings.BrowserName, Is.EqualTo("Edge"));
      Assert.That(webTestSettings.SearchTimeout, Is.EqualTo(TimeSpan.FromSeconds(6)));
      Assert.That(webTestSettings.CommandTimeout, Is.EqualTo(TimeSpan.FromMinutes(1)));
      Assert.That(webTestSettings.DownloadStartedTimeout, Is.EqualTo(TimeSpan.FromSeconds(10)));
      Assert.That(webTestSettings.DownloadUpdatedTimeout, Is.EqualTo(TimeSpan.FromSeconds(10)));
      Assert.That(webTestSettings.VerifyWebApplicationStartedTimeout, Is.EqualTo(TimeSpan.FromMinutes(1)));
      Assert.That(webTestSettings.RetryInterval, Is.EqualTo(TimeSpan.FromMilliseconds(11)));
      Assert.That(webTestSettings.AsyncJavaScriptTimeout, Is.EqualTo(TimeSpan.FromSeconds(10)));
      Assert.That(webTestSettings.Headless, Is.False);
      Assert.That(webTestSettings.WebApplicationRoot, Is.EqualTo("http://localhost:60401/"));
      Assert.That(webTestSettings.ScreenshotDirectory, Is.EqualTo(""));
      Assert.That(webTestSettings.LogsDirectory, Is.EqualTo("."));
      Assert.That(webTestSettings.CloseBrowserWindowsOnSetUpAndTearDown, Is.False);
      Assert.That(webTestSettings.RequestErrorDetectionStrategyTypeName, Is.EqualTo("None"));

      var hostingSettings = webTestSettings.Hosting;
      Assert.That(hostingSettings, Is.Not.Null);
      Assert.That(hostingSettings.Name, Is.EqualTo("IisExpress"));
      Assert.That(hostingSettings.Type, Is.EqualTo("IisExpressType"));
      Assert.That(hostingSettings.Parameters, Is.Empty);

      var testSiteLayout = webTestSettings.TestSiteLayout;
      Assert.That(testSiteLayout, Is.Not.Null);
      Assert.That(testSiteLayout.RootPath, Is.EqualTo("myRootPath"));
      Assert.That(testSiteLayout.Resources, Is.Empty);

      var chromeSettings = webTestSettings.Chrome;
      Assert.That(chromeSettings, Is.Not.Null);
      Assert.That(chromeSettings.DisableSecurityWarningsBehavior, Is.EqualTo(ChromiumDisableSecurityWarningsBehavior.Ignore));

      var edgeSettings = webTestSettings.Edge;
      Assert.That(edgeSettings, Is.Not.Null);
      Assert.That(edgeSettings.DisableSecurityWarningsBehavior, Is.EqualTo(ChromiumDisableSecurityWarningsBehavior.Ignore));
    }

    [Test]
    public void Deserialize_FromTestConfig ()
    {
      var webTestSettings = DeserializeFromString(
          """
          {
            "Remotion": {
              "WebTesting": {
                "BrowserName": "Edge",
                "SearchTimeout": "00:00:06",
                "CommandTimeout": "00:00:07",
                "DownloadStartedTimeout": "00:00:08",
                "DownloadUpdatedTimeout": "00:00:09",
                "VerifyWebApplicationStartedTimeout": "00:00:10",
                "RetryInterval": "00:00:00.011",
                "AsyncJavaScriptTimeout": "00:00:12",
                "Headless": "true",
                "WebApplicationRoot": "http://localhost:60401/",
                "ScreenshotDirectory": ".\\WebTestingOutput\\Screenshots",
                "LogsDirectory": ".\\WebTestingOutput\\Logs",
                "CloseBrowserWindowsOnSetUpAndTearDown": "true",
                "CleanUpUnmatchedDownloadedFiles": "true",
                "RequestErrorDetectionStrategy": "myRequestErrorDetectionStrategy",
                "Hosting": {
                  "Name": "IisExpress",
                  "Type": "IisExpressType",
                  "Parameters": {
                    "Port": 60401
                  }
                },
                "TestSiteLayout": {
                  "RootPath": "myRootPath",
                  "Resources": [
                    "resource1",
                    "Test\\resource2",
                  ]
                },
                "Chrome": {
                  "DisableSecurityWarningsBehavior": "Automatic"
                },
                "Edge": {
                  "DisableSecurityWarningsBehavior": "Automatic"
                }
              }
            }
          }
          """);

      Assert.That(webTestSettings, Is.Not.Null);
      Assert.That(webTestSettings.BrowserName, Is.EqualTo("Edge"));
      Assert.That(webTestSettings.SearchTimeout, Is.EqualTo(TimeSpan.FromSeconds(6)));
      Assert.That(webTestSettings.CommandTimeout, Is.EqualTo(TimeSpan.FromSeconds(7)));
      Assert.That(webTestSettings.DownloadStartedTimeout, Is.EqualTo(TimeSpan.FromSeconds(8)));
      Assert.That(webTestSettings.DownloadUpdatedTimeout, Is.EqualTo(TimeSpan.FromSeconds(9)));
      Assert.That(webTestSettings.VerifyWebApplicationStartedTimeout, Is.EqualTo(TimeSpan.FromSeconds(10)));
      Assert.That(webTestSettings.RetryInterval, Is.EqualTo(TimeSpan.FromMilliseconds(11)));
      Assert.That(webTestSettings.AsyncJavaScriptTimeout, Is.EqualTo(TimeSpan.FromSeconds(12)));
      Assert.That(webTestSettings.Headless, Is.True);
      Assert.That(webTestSettings.WebApplicationRoot, Is.EqualTo("http://localhost:60401/"));
      Assert.That(webTestSettings.ScreenshotDirectory, Is.EqualTo(".\\WebTestingOutput\\Screenshots"));
      Assert.That(webTestSettings.LogsDirectory, Is.EqualTo(".\\WebTestingOutput\\Logs"));
      Assert.That(webTestSettings.CloseBrowserWindowsOnSetUpAndTearDown, Is.True);
      Assert.That(webTestSettings.RequestErrorDetectionStrategyTypeName, Is.EqualTo("myRequestErrorDetectionStrategy"));

      var hostingSettings = webTestSettings.Hosting;
      Assert.That(hostingSettings, Is.Not.Null);
      Assert.That(hostingSettings.Name, Is.EqualTo("IisExpress"));
      Assert.That(hostingSettings.Type, Is.EqualTo("IisExpressType"));
      Assert.That(hostingSettings.Parameters, Is.EquivalentTo(new KeyValuePair<string, string>[] { new("Port", "60401") }));

      var testSiteLayout = webTestSettings.TestSiteLayout;
      Assert.That(testSiteLayout, Is.Not.Null);
      Assert.That(testSiteLayout.RootPath, Is.EqualTo("myRootPath"));
      Assert.That(testSiteLayout.Resources, Is.EquivalentTo(new[] { "resource1", "Test\\resource2" }));

      var chromeSettings = webTestSettings.Chrome;
      Assert.That(chromeSettings, Is.Not.Null);
      Assert.That(chromeSettings.DisableSecurityWarningsBehavior, Is.EqualTo(ChromiumDisableSecurityWarningsBehavior.Automatic));

      var edgeSettings = webTestSettings.Edge;
      Assert.That(edgeSettings, Is.Not.Null);
      Assert.That(edgeSettings.DisableSecurityWarningsBehavior, Is.EqualTo(ChromiumDisableSecurityWarningsBehavior.Automatic));
    }

    [Test]
    public void Deserialize_EnvironmentVariables ()
    {
      const string browserNameKey = "Remotion__WebTesting__BrowserName";
      const string rootPathKey = "Remotion__WebTesting__TestSiteLayout__RootPath";

      try
      {
        Environment.SetEnvironmentVariable(browserNameKey, "Chrome");
        Environment.SetEnvironmentVariable(rootPathKey, "C:\\test");

        var webTestSettings = DeserializeFromString(
            """
            {
              "Remotion": {
                "WebTesting": {
                  "BrowserName": "Edge",
                  "SearchTimeout": "00:00:06",
                  "RetryInterval": "00:00:00.011",
                  "WebApplicationRoot": "http://localhost:60401/",
                  "Hosting": {
                    "Name": "IisExpress",
                    "Type": "IisExpressType"
                  },
                  "TestSiteLayout": {
                    "RootPath": "myRootPath"
                  },
                }
              }
            }
            """);

        Assert.That(webTestSettings.BrowserName, Is.EqualTo("Chrome"));
        Assert.That(webTestSettings.TestSiteLayout.RootPath, Is.EqualTo("C:\\test"));
      }
      finally
      {
        Environment.SetEnvironmentVariable(browserNameKey, null);
        Environment.SetEnvironmentVariable(rootPathKey, null);
      }
    }

    private IWebTestSettings DeserializeFromString (string config)
    {
      var hostApplicationBuilder = Host.CreateEmptyApplicationBuilder(null);

      // Manually create the configuration sources as we don't want appsettings.json to affect our tests
      var jsonStream = ConvertStringToStream(config);
      hostApplicationBuilder.Configuration.AddJsonStream(jsonStream);
      hostApplicationBuilder.Configuration.AddEnvironmentVariables();

      hostApplicationBuilder.Services
          .AddOptions<WebTestSettingsDto>()
          .Bind(hostApplicationBuilder.Configuration.GetSection("Remotion:WebTesting"))
          .ValidateDataAnnotations();

      var host = hostApplicationBuilder.Build();
      var value = host.Services.GetRequiredService<IOptions<WebTestSettingsDto>>().Value;
      return value;
    }

    private MemoryStream ConvertStringToStream (string value)
    {
      var memoryStream = new MemoryStream();
      using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
        writer.WriteLine(value);

      memoryStream.Position = 0;
      return memoryStream;
    }
  }
}
#endif
