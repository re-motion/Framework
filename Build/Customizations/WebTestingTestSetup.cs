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
using System.IO;
using System.Linq;
using Nuke.Common;
using Remotion.BuildScript;
using Remotion.BuildScript.Test;
using Remotion.BuildScript.Test.Dimensions;
using Serilog;

namespace Customizations;

public class WebTestingTestSetup : ITestExecutionWrapper, IRequiresTestParameters
{
  private const string c_testSiteHostingDockerImageParameterName = "TestSiteHostingDockerImage";
  private const string c_testSiteHostingDockerIsolationModeParameterName = "TestSiteHostingDockerIsolationMode";
  private const string c_chromeVersionArchiveParameterName = "ChromeVersionArchive";
  private const string c_edgeVersionArchiveParameterName = "EdgeVersionArchive";
  private const string c_firefoxVersionArchiveParameterName = "FirefoxVersionArchive";

  private const string c_dockerHostName = "RemotionWebTestContainer";
  private const string c_dockerPortNumber = "60402";
  private const string c_dockerWebApplicationRootLocalhost = $"http://localhost:{c_dockerPortNumber}/";
  private const string c_dockerWebApplicationRootRemoteDriver = $"http://{c_dockerHostName}:{c_dockerPortNumber}/";
  private const bool c_dockerUseHttps = false;
  private const string c_dockerPullTimeout = "00:15:00";
  private const string c_dockerVerifyWebApplicationStartedTimeout = "00:01:30";

  public WebTestingTestSetup ()
  {
  }

  public void ConfigureTestParameters (TestParameterBuilder builder)
  {
    // We need the archive parameters on Windows but not on Linux as we use
    // docker images on Linux to host the browser
    if (OperatingSystem.IsWindows())
    {
      builder.AddRequiredParameter(c_chromeVersionArchiveParameterName);
      builder.AddRequiredParameter(c_edgeVersionArchiveParameterName);
      builder.AddRequiredParameter(c_firefoxVersionArchiveParameterName);
    }
    else
    {
      builder.AddOptionalParameter(c_chromeVersionArchiveParameterName, "");
      builder.AddOptionalParameter(c_edgeVersionArchiveParameterName, "");
      builder.AddOptionalParameter(c_firefoxVersionArchiveParameterName, "");
    }
  }

  public void ExecuteTests (TestExecutionContext context, Action<TestExecutionContext> next)
  {
    var executionRuntime = context.TestMatrixRow.GetDimension<ExecutionRuntimes>();

    bool hostTestSitesInDocker;
    string dockerImage, dockerIsolationMode;
    if (executionRuntime is EnforcedLocalMachineExecutionRuntimes enforcedLocalMachineExecutionRuntime)
    {
      hostTestSitesInDocker = true;
      dockerImage = enforcedLocalMachineExecutionRuntime.DockerExecutionRuntime.GetImage(context);
      dockerIsolationMode = enforcedLocalMachineExecutionRuntime.DockerExecutionRuntime.GetIsolationMode(context);
    }
    else
    {
      hostTestSitesInDocker = false;
      dockerImage = "";
      dockerIsolationMode = "";
    }

    // On Linux we are going to use remote driver docker images
    var useRemoteDriver = OperatingSystem.IsLinux();

    var chromeVersionArchive = context.GetTestParameter(c_chromeVersionArchiveParameterName);
    var edgeVersionArchive = context.GetTestParameter(c_edgeVersionArchiveParameterName);
    var firefoxVersionArchive = context.GetTestParameter(c_firefoxVersionArchiveParameterName);

    var browser = context.TestMatrixRow.GetDimension<Browsers>().Value;
    var platform = context.TestMatrixRow.GetDimension<Platforms>().Value;
    var configuration = context.TestMatrixRow.GetDimension<Configurations>().Value;
    var targetFramework = context.TestMatrixRow.GetDimension<TargetFrameworks>().Identifier;

    var assemblyName = context.Project.GetMetadata(RemotionBuildMetadataProperties.AssemblyName);
    var projectName = context.Project.FilePath.NameWithoutExtension;
    var variantName = $"{browser}_{platform}_{configuration}";

    var logsDirectory = context.Build.LogFolder / "WebTesting" / variantName / projectName;
    var logFile = logsDirectory / $"{projectName}.log";

    var configFile = context.Project.FolderPath / "bin" / configuration / targetFramework / $"{assemblyName}.dll.config";
    Assert.FileExists(configFile);

    Log.Information("Updating Web Test configuration file:");
    Log.Information($" - Browser: '{browser}'");
    Log.Information($" - Logs directory: '{logsDirectory}'");
    Log.Information($" - Log file: '{logFile}'");
    Log.Information($" - Chrome version archive: '{chromeVersionArchive}'");
    Log.Information($" - Edge version archive: '{edgeVersionArchive}'");
    Log.Information($" - Firefox version archive: '{firefoxVersionArchive}'");
    Log.Information($" - Host test site in docker: '{(hostTestSitesInDocker ? "yes" : "no")}'");
    if (hostTestSitesInDocker)
    {
      Log.Information($" - Docker image: '{dockerImage}'");
      Log.Information($" - Docker isolation mode: '{dockerIsolationMode}'");
    }
    Log.Information($" - Host remote driver: '{(useRemoteDriver ? "yes" : "no")}'");

    var appConfig = AppConfig.Read(configFile, ("rwt", "http://www.re-motion.org/WebTesting/Configuration/2.0"));

    appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting", "browser", browser);
    appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting", "logsDirectory", logsDirectory);
    appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting", "screenshotDirectory", logsDirectory);
    appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting", "closeBrowserWindowsOnSetUpAndTearDown", "true");
    appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting", "cleanUpUnmatchedDownloadedFiles", "true");
    appConfig.SetOrAddAttribute("/configuration/log4net/appender[@name='FileAppender']/file", "value", logFile);
    appConfig.SetAppSetting("ChromeVersionArchive", chromeVersionArchive);
    appConfig.SetAppSetting("EdgeVersionArchive", edgeVersionArchive);
    appConfig.SetAppSetting("FirefoxVersionArchive", firefoxVersionArchive);

    var dockerNetworkResource = context.TestResources
        .OfType<DockerNetworkResource>()
        .SingleOrDefault();

    if (hostTestSitesInDocker)
    {
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting", "webApplicationRoot", c_dockerWebApplicationRootLocalhost);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting", "verifyWebApplicationStartedTimeout", c_dockerVerifyWebApplicationStartedTimeout);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "name", "Docker");
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "type", "Docker");
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "innerType", "aspnetcore");
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "port", c_dockerPortNumber);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "dockerImageName", dockerImage);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "dockerIsolationMode", dockerIsolationMode);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "dockerPullTimeout", c_dockerPullTimeout);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "hostname", c_dockerHostName);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "useHttps", c_dockerUseHttps.ToString());

      if (dockerNetworkResource != null)
        appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "dockerCustomArguments", $"--network {dockerNetworkResource.Network.Name}");
    }

    if (useRemoteDriver)
    {
      // When using remote driver, the browser is no longer on the host so localhost changed and the exposed ports don't work.
      // As such, we use the hostname instead as that will resolve from the container.
      // But then we also have to change the testsite startup check URL as the hostname won't resolve from the host on Linux, so we use the localhost URL there.
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting", "webApplicationRoot", c_dockerWebApplicationRootRemoteDriver);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting", "testSiteStartupCheckUrl", c_dockerWebApplicationRootLocalhost);

      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:remoteDriver", "enabled", "true");
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:remoteDriver", "url", "http://localhost:4444");
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:remoteDriver", "hostRemoteDriverInDocker", "true");

      var networkAddition = dockerNetworkResource != null
          ? $" --network {dockerNetworkResource.Network.Name}"
          : "";
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:remoteDriver", "dockerCustomArguments", $"-p 4444:4444 -p 7900:7900 --shm-size=2g {networkAddition}");
    }

    appConfig.WriteToFile(configFile);
    File.Copy(configFile, configFile.Parent / "testhost.dll.config", true);
    File.Copy(configFile, configFile.Parent / "testhost.x86.dll.config", true);

    next(context);
  }
}
