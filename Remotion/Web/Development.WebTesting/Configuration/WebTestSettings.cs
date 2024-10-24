﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Remotion.Web.Development.WebTesting.Configuration.IConfiguration;

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Provides access to <see cref="IWebTestSettings"/>.
  /// </summary>
  public static class WebTestSettings
  {
    private static readonly object s_lock = new();

    private static IWebTestSettings? s_current;

    /// <summary>
    /// Gets the current <see cref="IWebTestSettings"/> and can be set to override the default settings.
    /// </summary>
    public static IWebTestSettings Current
    {
      get
      {
        var current = s_current;
        if (current != null)
          return current;

        lock (s_lock)
        {
          if (s_current != null)
            return s_current;

          throw new InvalidOperationException(
            """
            The WebTestSettings.SetCurrent(...) method must be called before using the WebTest infrastructure.

            Example based on log4net:
            1. Add reference to Remotion.Logging.Log4Net Nuget package.
            2. Add the following code in your SetupFixture class, before calling WebTestSetUpFixtureHelper.CreateFromConfiguration();
                var loggerFactory = new LoggerFactory(new[] { new Log4NetLoggerProvider() });
                WebTestSettings.SetCurrent(WebTestSettings.CreateAppConfigBasedWebTestSettings(loggerFactory));
                var setupFixtureHelper = WebTestSetUpFixtureHelper.CreateFromConfiguration<MyWebTestConfigurationFactory>();
                // additional initialization logic
                XmlConfigurator.Configure();
                setupFixtureHelper.OnSetUp();
            Alternatively, you can supply a different logging framework or chose to have no logging at all by passing the NullLoggerFactory.Instance. 
            """);
        }
      }
    }

    public static void SetCurrent (IWebTestSettings? value)
    {
      lock (s_lock)
      {
        s_current = value;
      }
    }

    public static IWebTestSettings CreateAppConfigBasedWebTestSettings (ILoggerFactory loggerFactory)
    {
      ArgumentUtility.CheckNotNull("loggerFactory", loggerFactory);

      var settings = (WebTestConfigurationSection)ConfigurationManager.GetSection("remotion.webTesting");
      Assertion.IsNotNull(settings, "Configuration section 'remotion.webTesting' missing.");
      settings.SetLoggerFactory(loggerFactory);

      return settings;
    }

    /// <summary>
    /// Creates <see cref="IWebTestSettings"/> using the default <see cref="Host"/> configuration.
    /// Reads settings from appsettings.json and environment variables.
    /// </summary>
    /// <remarks>
    /// Minimal config:
    /// <code>
    /// {
    ///   "Remotion": {
    ///     "WebTesting": {
    ///       "BrowserName": "Edge",
    ///       "SearchTimeout": "00:00:06",
    ///       "RetryInterval": "00:00:00.011",
    ///       "WebApplicationRoot": "http://localhost:60401/",
    ///       "Hosting": {
    ///         "Name": "IisExpress",
    ///         "Type": "IisExpressType"
    ///       },
    ///       "TestSiteLayout": {
    ///       "RootPath": "myRootPath"
    ///       }
    ///     }
    ///   }
    /// }
    /// </code>
    /// To override the configuration using environment variables, use the full path to the key you want to override as a name.
    /// Use two underscores '__' to separate object nesting levels. For example:
    /// <list type="bullet">
    ///   <item>'Remotion__WebTesting__BrowserName=Chrome'</item>
    ///   <item>'Remotion__WebTesting__TestSiteLayout__RootPath=newRootPath'</item>
    /// </list>
    /// </remarks>
    public static IWebTestSettings CreateAppSettingsJsonBasedWebTestSettings (ILoggerFactory loggerFactory)
    {
      ArgumentUtility.CheckNotNull("loggerFactory", loggerFactory);

      var hostApplicationBuilder = Host.CreateApplicationBuilder();
      hostApplicationBuilder.Services
          .AddOptions<WebTestSettingsDto>()
          .Bind(hostApplicationBuilder.Configuration.GetSection("Remotion:WebTesting"))
          .ValidateDataAnnotations();

      var host = hostApplicationBuilder.Build();
      var value = host.Services.GetRequiredService<IOptions<WebTestSettingsDto>>().Value;
      value.SetLoggerFactory(loggerFactory);

      return value;
    }
  }
}
