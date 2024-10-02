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
using System.IO;
using System.Net;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.HostingStrategies;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Helper class for web test set up fixtures. Initializes log4net and hosts the configured web application under test.
  /// </summary>
  /// <remarks>
  /// Use the factory methods <see cref="CreateFromConfiguration"/> or <see cref="CreateFromConfiguration{TFactory}"/> 
  /// for instantiating an instance of type <see cref="WebTestSetUpFixtureHelper"/>.
  /// </remarks>
  public class WebTestSetUpFixtureHelper
  {
    /// <summary>
    /// Creates a new <see cref="WebTestSetUpFixtureHelper"/> with configuration based on <see cref="WebTestConfigurationFactory"/>.
    /// </summary>
    [PublicAPI]
    public static WebTestSetUpFixtureHelper CreateFromConfiguration ()
    {
      return new WebTestSetUpFixtureHelper(new WebTestConfigurationFactory());
    }

    /// <summary>
    /// Creates a new <see cref="WebTestSetUpFixtureHelper"/> with configuration based on <typeparamref name="TFactory"/>.
    /// </summary>
    /// <remarks>
    /// Use this overload when you have to provide test-project specific configuration settings (e.g. custom <see cref="IHostingStrategy"/>) 
    /// via custom <see cref="WebTestConfigurationFactory"/>.
    /// </remarks>
    [PublicAPI]
    public static WebTestSetUpFixtureHelper CreateFromConfiguration<TFactory> () where TFactory : WebTestConfigurationFactory, new()
    {
      return new WebTestSetUpFixtureHelper(new TFactory());
    }

    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;
    private readonly IHostingStrategy _hostingStrategy;
    private readonly TimeSpan _verifyWebApplicationStartedTimeout;
    private readonly string _screenshotDirectory;
    private readonly string _logDirectory;
    private readonly Uri _webApplicationRoot;

    [PublicAPI]
    protected WebTestSetUpFixtureHelper ([NotNull] WebTestConfigurationFactory webTestConfigurationFactory)
    {
      ArgumentUtility.CheckNotNull("webTestConfigurationFactory", webTestConfigurationFactory);

      _loggerFactory = webTestConfigurationFactory.LoggerFactory;
      _logger = _loggerFactory.CreateLogger<WebTestSetUpFixtureHelper>();

      var hostingConfiguration = webTestConfigurationFactory.CreateHostingConfiguration();
      _hostingStrategy = hostingConfiguration.GetHostingStrategy();
      _verifyWebApplicationStartedTimeout = hostingConfiguration.VerifyWebApplicationStartedTimeout;

      var testInfrastructureConfiguration = webTestConfigurationFactory.CreateTestInfrastructureConfiguration();
      _screenshotDirectory = testInfrastructureConfiguration.ScreenshotDirectory;
      _logDirectory = testInfrastructureConfiguration.ScreenshotDirectory;
      _webApplicationRoot = new Uri(testInfrastructureConfiguration.WebApplicationRoot);
    }

    public ILoggerFactory LoggerFactory
    {
      get { return _loggerFactory; }
    }

    public string ScreenshotDirectory
    {
      get { return _screenshotDirectory; }
    }

    public string LogDirectory
    {
      get { return _logDirectory; }
    }

    /// <summary>
    /// One-time SetUp method for all web tests.
    /// </summary>
    public void OnSetUp ()
    {
      HostWebApplication();

      try
      {
        VerifyWebApplicationStarted(_webApplicationRoot, _verifyWebApplicationStartedTimeout);
      }
      catch
      {
        try
        {
          UnhostWebApplication();
        }
        catch
        {
        }

        throw;
      }
    }

    /// <summary>
    /// One-time TearDown method for all web tests.
    /// </summary>
    public void OnTearDown ()
    {
      UnhostWebApplication();
    }

    private void HostWebApplication ()
    {
      _hostingStrategy.DeployAndStartWebApplication();
    }

    private void VerifyWebApplicationStarted (Uri webApplicationRoot, TimeSpan applicationPingTimeout)
    {
      _logger.LogInformation($"Verifying that '{webApplicationRoot}' is accessible within {applicationPingTimeout}.");

      var stopwatch = Stopwatch.StartNew();

      HttpStatusCode statusCode = default;
      Assertion.DebugAssert(statusCode != HttpStatusCode.OK);

      WebResponse? lastResponse = null;
      string? lastResponseText = null;
      while (true)
      {
#pragma warning disable SYSLIB0014
        var webRequest = (HttpWebRequest)HttpWebRequest.Create(webApplicationRoot); // TODO RM-8492: Replace with HttpClient
#pragma warning restore SYSLIB0014
        webRequest.Method = WebRequestMethods.Http.Get;
        webRequest.AllowAutoRedirect = true;
        webRequest.Host = webApplicationRoot.GetComponents(UriComponents.Host | UriComponents.Port, UriFormat.UriEscaped);
        webRequest.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;

        var remainingTimeout = (int)(applicationPingTimeout.TotalMilliseconds - stopwatch.Elapsed.TotalMilliseconds);
        if (remainingTimeout <= 0)
        {
          _logger.LogWarning($"Checking the web application root '{webApplicationRoot}' timed out (HTTP status code: '{statusCode}').");

          // Embed the response text as inner exception
          var innerException = lastResponseText != null
              ? new Exception($"Last response text:{Environment.NewLine}{lastResponseText}")
              : null;

          throw new WebException(
              $"Checking the web application root '{webApplicationRoot}' did not return '{HttpStatusCode.OK}' "
              + $"in the defined {nameof(applicationPingTimeout)} ({applicationPingTimeout}). "
              + $"Last received HTTP status code was: '{statusCode}'.",
              innerException,
              WebExceptionStatus.Timeout,
              lastResponse);
        }

        try
        {
          webRequest.Timeout = remainingTimeout;

          using var response = (HttpWebResponse)webRequest.GetResponse();
          _logger.LogInformation($"Verified that '{webApplicationRoot}' is accessible after {stopwatch.Elapsed.TotalMilliseconds:N0} ms.");
          return;
        }
        catch (WebException ex)
        {
          lastResponse = ex.Response;
          if (ex.Response is HttpWebResponse httpWebResponse)
            statusCode = httpWebResponse.StatusCode;

          _logger.LogWarning(
              $"Checking the web application root '{webApplicationRoot}' failed with WebException: '{ex.Message}' (HTTP status code: '{statusCode}'). "
              + $"Retrying until {nameof(applicationPingTimeout)} ({applicationPingTimeout}) is reached.");

          try
          {
            if (ex.Response != null)
            {
              using var responseStream = ex.Response.GetResponseStream();
              using var reader = new StreamReader(responseStream);
              lastResponseText = reader.ReadToEnd();
            }
          }
          catch (Exception)
          {
            // Ignore errors caused by reading the failed request
          }
        }

        Thread.Sleep(TimeSpan.FromMilliseconds(500));
      }
    }

    private void UnhostWebApplication ()
    {
      _hostingStrategy.StopAndUndeployWebApplication();
    }
  }
}
