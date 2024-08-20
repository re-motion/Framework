// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.HostingStrategies.Configuration;

namespace Remotion.Web.Development.WebTesting.HostingStrategies;

/// <summary>
/// Hosts the WebApplication using ASP.NET Core locally.
/// </summary>
public class AspNetCoreHostingStrategy : IHostingStrategy
{
  private readonly ITestSiteLayoutConfiguration _testSiteLayoutConfiguration;
  private readonly int _port;
  private readonly string _hostname;
  private readonly bool _useHttps;
  private readonly CancellationTokenSource _cancellationTokenSource;
  private Task? _webServerTask;

  /// <summary>
  /// Initializes a new instance of the type.
  /// </summary>
  /// <param name="testSiteLayoutConfiguration">The configuration of the layout of the used test site.</param>
  /// <param name="port">Port to be used.</param>
  /// <param name="hostname">Hostname to be used..</param>
  /// <param name="useHttps">Determines whether the site is hosted using https.</param>
  public AspNetCoreHostingStrategy (ITestSiteLayoutConfiguration testSiteLayoutConfiguration, int port, string hostname, bool useHttps)
  {
    ArgumentUtility.CheckNotNull(nameof(testSiteLayoutConfiguration), testSiteLayoutConfiguration);
    ArgumentUtility.CheckNotNull(nameof(port), port);
    ArgumentUtility.CheckNotEmpty(nameof(hostname), hostname);

    _testSiteLayoutConfiguration = testSiteLayoutConfiguration;
    _port = port;
    _hostname = hostname;
    _useHttps = useHttps;
    _cancellationTokenSource = new CancellationTokenSource();
  }

  /// <summary>
  /// Initializes a new instance of the type. Used by the when the AspNetCore hosting type is selected.
  /// </summary>
  /// <param name="testSiteLayoutConfiguration">The configuration of the layout of the used test site.</param>
  /// <param name="properties">The configuration properties.</param>
  [UsedImplicitly]
  public AspNetCoreHostingStrategy ( ITestSiteLayoutConfiguration testSiteLayoutConfiguration, IReadOnlyDictionary<string, string> properties)
      : this(
          ArgumentUtility.CheckNotNull(nameof(testSiteLayoutConfiguration), testSiteLayoutConfiguration),
          int.Parse(ArgumentUtility.CheckNotNull(nameof(properties), properties)["port"]!),
          properties["hostname"],
          properties["useHttps"].Equals("true", StringComparison.OrdinalIgnoreCase))
  {
  }

  /// <inheritdoc/>
  public void DeployAndStartWebApplication ()
  {
    if (_webServerTask != null)
      return;

    if (_testSiteLayoutConfiguration.ProcessPath == null)
    {
      throw new ArgumentException(
          "The executable's path was not set while trying to host with ASP.NET Core. Please add the 'processPath' attribute to the 'testSiteLayout' configuration.");
    }

    var protocol = _useHttps ? "https" : "http";

    var processWrapper = new AspNetCoreHostingProcessWrapper(
        _testSiteLayoutConfiguration.ProcessPath,
        _testSiteLayoutConfiguration.RootPath,
        $"{protocol}://{_hostname}:{_port}");

    _webServerTask = processWrapper.RunWebServerAsync(_cancellationTokenSource.Token);
  }

  /// <inheritdoc/>
  public void StopAndUndeployWebApplication ()
  {
    _cancellationTokenSource.Cancel();
    if (_webServerTask != null && !_webServerTask.Wait(TimeSpan.FromSeconds(1)))
    {
      throw new InvalidOperationException(
          "Web server process did not shut down in a reasonable amount of time.");
    }
  }
}
