// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.HostingStrategies;

/// <summary>
/// Wraps an ASP.NET Core instance hosting the web application under test.
/// </summary>
public class AspNetCoreHostingProcessWrapper
{
  private readonly string _exePath;
  private readonly string _workingDirectory;
  private readonly string _url;
  private const int c_exitTimeoutInMilliseconds = 10_000;

  /// <summary>
  /// Initializes the wrapper, does not yet run the ASP.NET Core process.
  /// </summary>
  /// <param name="exePath">Absolute file path to the executable which runs the web server.</param>
  /// <param name="workingDirectory">Working directory for the ASP.NET core process.</param>
  /// <param name="url">The URL under which the test sites will be accessible from.</param>
  public AspNetCoreHostingProcessWrapper (string exePath, string workingDirectory, string url)
  {
    ArgumentUtility.CheckNotNullOrEmpty(nameof(exePath), exePath);
    ArgumentUtility.CheckNotNullOrEmpty(nameof(workingDirectory), workingDirectory);
    ArgumentUtility.CheckNotNullOrEmpty(nameof(url), url);

    _exePath = exePath;
    _workingDirectory = workingDirectory;
    _url = url;
  }

  public async Task RunWebServerAsync (CancellationToken cancellationToken)
  {
    using var serverProcess = CreateServerProcess();
    serverProcess.Start();

    var exitCode = await ProcessShutdownUtility.RegisterProcessShutdownOnCancellationToken(serverProcess, cancellationToken, c_exitTimeoutInMilliseconds);
    if (exitCode != 0 && exitCode != -1)
      throw new InvalidOperationException($"ASP.NET Core Server process exited with exit code '{exitCode}'.");
  }

  private Process CreateServerProcess ()
  {
    var startInfo = new ProcessStartInfo
                    {
                      WindowStyle = ProcessWindowStyle.Minimized,
                      ErrorDialog = true,
                      LoadUserProfile = true,
                      CreateNoWindow = false,
                      UseShellExecute = false
                    };

    startInfo.FileName = _exePath;
    startInfo.WorkingDirectory = _workingDirectory;
    startInfo.Environment.Add("ASPNETCORE_URLS", _url);

    return new Process { StartInfo = startInfo };
  }
}
