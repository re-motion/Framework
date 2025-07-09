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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting
{
  /// <summary>
  /// Default implementation of <see cref="IDockerClient"/>, using Docker's command line interface.
  /// </summary>
  public class DockerCommandLineClient : IDockerClient
  {
    private  readonly ILogger _logger;
    private readonly string _dockerExecutablePath;
    private readonly TimeSpan _pullTimeout;
    private readonly TimeSpan _commandTimeout = TimeSpan.FromSeconds(15);

    public DockerCommandLineClient (TimeSpan pullTimeout, ILoggerFactory loggerFactory)
    {
      ArgumentNullException.ThrowIfNull(loggerFactory);

      _logger = loggerFactory.CreateLogger<DockerCommandLineClient>();
      _pullTimeout = pullTimeout;
      _dockerExecutablePath = GetDockerExecutablePath();
    }

    /// <inheritdoc />
    public void Pull (string imageName)
    {
      ArgumentException.ThrowIfNullOrEmpty(imageName);

      RunDockerCommand($"pull {imageName}", timeout: _pullTimeout);
    }

    /// <inheritdoc />
    public string Run (DockerRunSettings settings)
    {
      ArgumentNullException.ThrowIfNull(settings);

      var commandBuilder = new StringBuilder()
          .Append("run").Append(' ')
          .Append("-d").Append(' ');

      if (settings.Remove)
        commandBuilder.Append("--rm").Append(' ');

      if (settings.IsolationMode != null)
        commandBuilder.Append($@"--isolation=""{settings.IsolationMode}""").Append(' ');

      if (settings.Ports.Any())
      {
        var portFlags = string.Join(" ", settings.Ports.Select(kvp => $"-p {kvp.Key}:{kvp.Value}"));
        commandBuilder.Append(portFlags).Append(' ');
      }

      if (settings.Mounts.Any())
      {
        var mountFlags = string.Join(" ", settings.Mounts.Select(kvp => $@"-v ""{kvp.Key}"":""{kvp.Value.Trim('\\')}"""));
        commandBuilder.Append(mountFlags).Append(' ');
      }

      if (settings.EnvironmentVariables.Any())
      {
        var environmentFlags = string.Join(" ", settings.EnvironmentVariables.Select(kvp => $@"-e ""{kvp.Key}""=""{kvp.Value}"""));
        commandBuilder.Append(environmentFlags).Append(' ');
      }

      if (settings.EntryPoint != null)
        commandBuilder.Append($@"--entrypoint=""{settings.EntryPoint}""").Append(' ');

      if (settings.WorkingDirectory != null)
        commandBuilder.Append($@"--workdir ""{settings.WorkingDirectory.Trim('\\')}""").Append(' ');

      if (settings.Hostname != null)
        commandBuilder.Append($@"--hostname ""{settings.Hostname}""").Append(' ');

      if (settings.CustomArguments != null)
      {
        commandBuilder.Append(settings.CustomArguments);
        if (settings.CustomArguments.Length > 0 && settings.CustomArguments[^1] != ' ')
          commandBuilder.Append(' ');
      }

      commandBuilder.Append(settings.ImageName).Append(' ');

      if (settings.Args != null)
        commandBuilder.Append(settings.Args);

      var command = commandBuilder.ToString();

      return RunDockerCommand(command).TrimEnd('\r', '\n');
    }

    /// <inheritdoc />
    public bool ContainerExists (string containerName)
    {
      ArgumentException.ThrowIfNullOrEmpty(containerName);

      using (var p = Process.Start(_dockerExecutablePath, $"inspect {containerName}"))
      {
        p.WaitForExit((int)_commandTimeout.TotalMilliseconds);

        return p.ExitCode == 0;
      }
    }

    /// <inheritdoc />
    public void Remove (string containerName, bool force = false)
    {
      ArgumentException.ThrowIfNullOrEmpty(containerName);

      var commandBuilder = new StringBuilder()
          .Append("rm").Append(' ');

      if (force)
        commandBuilder.Append("-f").Append(' ');

      commandBuilder.Append(containerName);

      var command = commandBuilder.ToString();

      RunDockerCommand(command);
    }

    /// <inheritdoc />
    public void Stop (string containerName)
    {
      ArgumentException.ThrowIfNullOrEmpty(containerName);

      var commandBuilder = new StringBuilder()
          .Append("stop").Append(' ')
          .Append(containerName);

      var command = commandBuilder.ToString();

      RunDockerCommand(command);
    }

    private string RunDockerCommand (string dockerCommand, string? workingDirectory = null, TimeSpan? timeout = null)
    {
      _logger.LogInformation($"Running: 'docker {dockerCommand}'");

      var startInfo = new ProcessStartInfo
                      {
                          WindowStyle = ProcessWindowStyle.Hidden,
                          ErrorDialog = false,
                          CreateNoWindow = false,
                          UseShellExecute = false,
                          RedirectStandardOutput = true,
                          RedirectStandardError = true
                      };
      if (OperatingSystem.IsWindows())
        startInfo.LoadUserProfile = true;

      if (!string.IsNullOrEmpty(workingDirectory))
        startInfo.WorkingDirectory = workingDirectory;

      startInfo.FileName = _dockerExecutablePath;
      startInfo.Arguments = dockerCommand;

      using (var dockerProcess = new Process { StartInfo = startInfo })
      {
        dockerProcess.Start();

        dockerProcess.OutputDataReceived += (sender, outputLine) =>
        {
          if (outputLine.Data != null)
            _logger.LogInformation(outputLine.Data);
        };

        var error = dockerProcess.StandardError.ReadToEnd();
        var output = dockerProcess.StandardOutput.ReadToEnd();

        WaitForExit(dockerProcess, dockerCommand, timeout ?? _commandTimeout);

        if (dockerProcess.ExitCode != 0)
        {
          var errorMessage = $"Docker command '{dockerCommand}' failed: {error}";

          _logger.LogError(errorMessage);
          throw new DockerOperationException(errorMessage, dockerProcess.ExitCode);
        }

        return output;
      }
    }

    private void WaitForExit (Process dockerProcess, string dockerCommand, TimeSpan timeout)
    {
      var stopwatch = Stopwatch.StartNew();

      while (!dockerProcess.HasExited)
      {
        dockerProcess.WaitForExit((int)timeout.TotalMilliseconds);

        if (stopwatch.ElapsedMilliseconds > timeout.TotalMilliseconds)
          throw new TimeoutException($"Docker command '{dockerCommand}' ran longer than the configured timeout of '{timeout}'.");
      }
    }

    private string GetDockerExecutablePath ()
    {
      // On non-windows system we assume docker is part of the path
      if (!OperatingSystem.IsWindows())
        return "docker";

      var programFiles = Environment.GetEnvironmentVariable("ProgramW6432");
      if (string.IsNullOrEmpty(programFiles))
        programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

      var listOfKnownDockerLocations = new List<string>
                                       {
                                           Path.Combine(programFiles, "Docker", "docker.exe"),
                                           Path.Combine(programFiles, "Docker", "Docker", "Resources", "bin", "docker.exe")
                                       };

      var foundDockers = listOfKnownDockerLocations.Where(File.Exists).ToList();

      if (!foundDockers.Any())
      {
        var errorMessage = "Could not find Docker installed on this system. Checked paths:" +
                           Environment.NewLine +
                           string.Join(Environment.NewLine, listOfKnownDockerLocations.ToArray());

        _logger.LogCritical(errorMessage);
        throw new FileNotFoundException(errorMessage);
      }

      return foundDockers.First();
    }
  }
}
