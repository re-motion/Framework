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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting
{
  /// <summary>
  /// Default implementation of <see cref="IDockerClient"/>, using Docker's command line interface.
  /// </summary>
  public class DockerCommandLineClient : IDockerClient
  {
    private static readonly ILog s_log = LogManager.GetLogger(typeof(DockerCommandLineClient));

    private readonly string _dockerExeFullPath;
    private readonly TimeSpan _pullTimeout;
    private readonly TimeSpan _commandTimeout = TimeSpan.FromSeconds(15);

    public DockerCommandLineClient (TimeSpan pullTimeout)
    {
      _pullTimeout = pullTimeout;
      _dockerExeFullPath = GetDockerExeFullPath();
    }

    /// <inheritdoc />
    public void Pull (string imageName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("imageName", imageName);

      RunDockerCommand($"pull {imageName}", timeout: _pullTimeout);
    }

    /// <inheritdoc />
    public string Run (
        IDictionary<int, int> ports,
        IDictionary<string, string> mounts,
        string imageName,
        string? hostname,
        bool remove,
        string? entryPoint,
        string? args)
    {
      ArgumentUtility.CheckNotNull("ports", ports);
      ArgumentUtility.CheckNotNull("mounts", mounts);
      ArgumentUtility.CheckNotNullOrEmpty("imageName", imageName);
      ArgumentUtility.CheckNotEmpty("hostname", hostname);
      ArgumentUtility.CheckNotEmpty("entryPoint", entryPoint);
      ArgumentUtility.CheckNotEmpty("args", args);

      var commandBuilder = new StringBuilder()
          .Append("run").Append(' ')
          .Append("-d").Append(' ');

      if (remove)
        commandBuilder.Append("--rm").Append(' ');

      if (ports.Any())
      {
        var portFlags = string.Join(" ", ports.Select(kvp => $"-p {kvp.Key}:{kvp.Value}"));
        commandBuilder.Append(portFlags).Append(' ');
      }

      if (mounts.Any())
      {
        var mountFlags = string.Join(" ", mounts.Select(kvp => $@"-v ""{kvp.Key}"":""{kvp.Value}"""));
        commandBuilder.Append(mountFlags).Append(' ');
      }

      if (entryPoint != null)
        commandBuilder.Append($@"--entrypoint=""{entryPoint}""").Append(' ');

      if (hostname != null)
        commandBuilder.Append($@"--hostname ""{hostname}""").Append(' ');

      commandBuilder.Append(imageName).Append(' ');

      if (args != null)
        commandBuilder.Append(args);

      var command = commandBuilder.ToString();

      return RunDockerCommand(command).TrimEnd('\r', '\n');
    }

    /// <inheritdoc />
    public bool ContainerExists (string containerName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("containerName", containerName);

      using (var p = Process.Start(_dockerExeFullPath, $"inspect {containerName}"))
      {
        p.WaitForExit((int) _commandTimeout.TotalMilliseconds);

        return p.ExitCode == 0;
      }
    }

    /// <inheritdoc />
    public void Remove (string containerName, bool force = false)
    {
      ArgumentUtility.CheckNotNullOrEmpty("containerName", containerName);

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
      ArgumentUtility.CheckNotNullOrEmpty("containerName", containerName);

      var commandBuilder = new StringBuilder()
          .Append("stop").Append(' ')
          .Append(containerName);

      var command = commandBuilder.ToString();

      RunDockerCommand(command);
    }

    private string RunDockerCommand (string dockerCommand, string? workingDirectory = null, TimeSpan? timeout = null)
    {
      s_log.Info($"Running: 'docker {dockerCommand}'");

      var startInfo = new ProcessStartInfo
                      {
                          WindowStyle = ProcessWindowStyle.Hidden,
                          ErrorDialog = false,
                          LoadUserProfile = true,
                          CreateNoWindow = false,
                          UseShellExecute = false,
                          RedirectStandardOutput = true,
                          RedirectStandardError = true
                      };

      if (!string.IsNullOrEmpty(workingDirectory))
        startInfo.WorkingDirectory = workingDirectory;

      startInfo.FileName = _dockerExeFullPath;
      startInfo.Arguments = dockerCommand;

      using (var dockerProcess = new Process { StartInfo = startInfo })
      {
        dockerProcess.Start();

        dockerProcess.OutputDataReceived += (sender, outputLine) =>
        {
          if (outputLine.Data != null)
            s_log.Info(outputLine.Data);
        };

        var error = dockerProcess.StandardError.ReadToEnd();
        var output = dockerProcess.StandardOutput.ReadToEnd();

        WaitForExit(dockerProcess, dockerCommand, timeout ?? _commandTimeout);

        if (dockerProcess.ExitCode != 0)
        {
          var errorMessage = $"Docker command '{dockerCommand}' failed: {error}";

          s_log.Error(errorMessage);
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
        dockerProcess.WaitForExit(timeout.Milliseconds);

        if (stopwatch.ElapsedMilliseconds > timeout.TotalMilliseconds)
          throw new TimeoutException($"Docker command '{dockerCommand}' ran longer than the configured timeout of '{timeout}'.");
      }
    }

    private static string GetDockerExeFullPath ()
    {
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

        s_log.Fatal(errorMessage);
        throw new FileNotFoundException(errorMessage);
      }

      return foundDockers.First();
    }
  }
}