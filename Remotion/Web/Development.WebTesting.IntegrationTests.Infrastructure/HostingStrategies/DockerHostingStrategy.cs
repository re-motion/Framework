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
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using JetBrains.Annotations;
using Remotion.Logging;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.HostingStrategies;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.HostingStrategies.DockerHosting;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.HostingStrategies
{
  /// <summary>
  /// Hosts the WebApplication using a Docker Container.
  /// </summary>
  public class DockerHostingStrategy : IHostingStrategy
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (DockerHostingStrategy));

    private readonly IisDockerContainerWrapper _iisDockerContainerWrapper;
    private readonly TimeSpan _startTimeout;
    private readonly Uri _webApplicationRoot;

    public DockerHostingStrategy (
        [NotNull] string path,
        int port,
        [NotNull] string dockerImageName,
        TimeSpan dockerPullTimeout,
        TimeSpan startTimeout,
        [CanBeNull] string hostname,
        [NotNull] string mounts)
    {
      ArgumentUtility.CheckNotNull ("path", path);
      ArgumentUtility.CheckNotNullOrEmpty ("dockerImageName", dockerImageName);
      ArgumentUtility.CheckNotEmpty ("hostname", hostname);
      ArgumentUtility.CheckNotNullOrEmpty ("mounts", mounts);

      _startTimeout = startTimeout;
      _webApplicationRoot = new Uri ($"http://{hostname ?? "localhost"}:{port}/");

      var absoluteWebApplicationPath = GetRootedPath (path);

      var is32BitProcess = !Environment.Is64BitProcess;

      var docker = new DockerCommandLineClient (dockerPullTimeout);

      var resources = mounts.Split (new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
          .Select (p => GetAbsolutePath (p, absoluteWebApplicationPath).TrimEnd ('\\'))
          .Distinct (StringComparer.OrdinalIgnoreCase)
          .ToArray();

      var configurationParameters = new IisDockerContainerConfigurationParameters (
          absoluteWebApplicationPath,
          port,
          dockerImageName,
          hostname,
          is32BitProcess,
          resources);

      _iisDockerContainerWrapper = new IisDockerContainerWrapper (docker, configurationParameters);
    }

    /// <summary>
    /// Constructor required for direct usage in <see cref="WebTestConfigurationSection"/>.
    /// </summary>
    /// <param name="properties">The configuration properties.</param>
    [UsedImplicitly]
    public DockerHostingStrategy ([NotNull] NameValueCollection properties)
        : this (
            ArgumentUtility.CheckNotNull ("properties", properties)["path"],
            int.Parse (properties["port"]),
            properties["dockerImageName"],
            TimeSpan.Parse (properties["dockerPullTimeout"]),
            TimeSpan.Parse (properties["dockerVerifyWebApplicationStartedTimeout"]),
            properties["hostname"],
            properties["dockerMounts"])
    {
    }

    /// <inheritdoc />
    public void DeployAndStartWebApplication ()
    {
      _iisDockerContainerWrapper.Run();
      VerifyWebApplicationStarted (_webApplicationRoot, _startTimeout);
    }

    /// <inheritdoc />
    public void StopAndUndeployWebApplication ()
    {
      _iisDockerContainerWrapper.Dispose();
    }

    private string GetAbsolutePath (string path, string workingDirectory)
    {
      if (Path.IsPathRooted (path))
        return Path.GetFullPath (path);

      var absolutePath = Path.Combine (workingDirectory, path);

      return Path.GetFullPath (absolutePath);
    }

    private string GetRootedPath ([NotNull] string path)
    {
      return GetAbsolutePath (path, AppDomain.CurrentDomain.BaseDirectory);
    }

    private void VerifyWebApplicationStarted (Uri webApplicationRoot, TimeSpan applicationPingTimeout)
    {
      var resolvedUri = ResolveHostname (webApplicationRoot);
      s_log.Info ($"Verifying that '{resolvedUri}' is accessible within {applicationPingTimeout}.");

      var stopwatch = Stopwatch.StartNew();

      var webRequest = (HttpWebRequest) HttpWebRequest.Create (resolvedUri);
      webRequest.Method = WebRequestMethods.Http.Head;
      webRequest.AllowAutoRedirect = true;
      webRequest.Host = webApplicationRoot.Host;

      HttpStatusCode statusCode = default;
      Assertion.DebugAssert (statusCode != HttpStatusCode.OK);

      while (statusCode != HttpStatusCode.OK)
      {
        try
        {
          var remainingTimeout = (int) (applicationPingTimeout.TotalMilliseconds - stopwatch.Elapsed.TotalMilliseconds);
          webRequest.Timeout = Math.Max (remainingTimeout, 0);

          using (var response = (HttpWebResponse) webRequest.GetResponse())
          {
            statusCode = response.StatusCode;
          }
        }
        catch (WebException ex)
        {
          CheckTimeout (webApplicationRoot, applicationPingTimeout, stopwatch, $"Failed with WebException '{ex.Message}'");
        }

        CheckTimeout (webApplicationRoot, applicationPingTimeout, stopwatch, $"Failed with HttpStatusCode '{statusCode}'");

        Thread.Sleep (TimeSpan.FromMilliseconds (500));
      }

      stopwatch.Stop();

      s_log.Info ($"Verified that '{resolvedUri}' is accessible after {stopwatch.Elapsed.TotalMilliseconds:N0} ms.");
    }

    private void CheckTimeout (Uri webApplicationRoot, TimeSpan applicationPingTimeout, Stopwatch stopwatch, string failureReason)
    {
      if (stopwatch.ElapsedMilliseconds > applicationPingTimeout.TotalMilliseconds)
      {
        throw new WebException (
            $"Checking the web application root '{webApplicationRoot}' did not return '{HttpStatusCode.OK}' in the defined {nameof (applicationPingTimeout)} ({applicationPingTimeout}). "
            + $"{failureReason}.",
            WebExceptionStatus.Timeout);
      }

      s_log.Warn ($"Checking the web application root '{webApplicationRoot}' failed with following reason: {failureReason}. Retrying until {nameof (applicationPingTimeout)} ({applicationPingTimeout}) is reached.");
    }

    private Uri ResolveHostname (Uri uri)
    {
      var host = new RetryUntilTimeout<IPHostEntry> (() => Dns.GetHostEntry (uri.Host), TimeSpan.FromSeconds (30), TimeSpan.FromSeconds (1)).Run();
      var address = host.AddressList.First (a => a.AddressFamily == AddressFamily.InterNetwork).MapToIPv4();
      var uriBuilder = new UriBuilder (uri);
      uriBuilder.Host = address.ToString();
      return uriBuilder.Uri;
    }
  }
}