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
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Utilities;
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
  private const string c_dockerWebApplicationRoot = $"http://{c_dockerHostName}.local:{c_dockerPortNumber}/";
  private const string c_dockerPullTimeout = "00:15:00";
  private const string c_dockerVerifyWebApplicationStartedTimeout = "00:01:30";

  public WebTestingTestSetup ()
  {
  }

  public void ConfigureTestParameters (TestParameterBuilder builder)
  {
    builder.AddRequiredParameter(c_chromeVersionArchiveParameterName);
    builder.AddRequiredParameter(c_edgeVersionArchiveParameterName);
    builder.AddRequiredParameter(c_firefoxVersionArchiveParameterName);
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

    if (hostTestSitesInDocker)
    {
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting", "webApplicationRoot", c_dockerWebApplicationRoot);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting", "verifyWebApplicationStartedTimeout", c_dockerVerifyWebApplicationStartedTimeout);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "name", "Docker");
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "type", "Docker");
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "innerType", "aspnetcore");
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "port", c_dockerPortNumber);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "dockerImageName", dockerImage);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "dockerIsolationMode", dockerIsolationMode);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "dockerPullTimeout", c_dockerPullTimeout);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "hostname", c_dockerHostName);
    }

    appConfig.WriteToFile(configFile);

    next(context);
  }

  private static void UpdateConfigFile (AbsolutePath configFile, string xPath, string value)
  {
    try
    {
      XmlTasks.XmlPoke(
        configFile,
        xPath,
        value,
        ("rwt", "http://www.re-motion.org/WebTesting/Configuration/2.0"));
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Could not patch config file. XPath: '{xPath}'", ex);
    }
  }
}
