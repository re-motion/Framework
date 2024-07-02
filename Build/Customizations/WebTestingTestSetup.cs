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

  private readonly bool _hostTestSitesInDocker;

  public WebTestingTestSetup (bool hostTestSitesInDocker)
  {
    _hostTestSitesInDocker = hostTestSitesInDocker;
  }

  public void ConfigureTestParameters (TestParameterBuilder builder)
  {
    if (_hostTestSitesInDocker)
    {
      builder.AddRequiredParameter(c_testSiteHostingDockerImageParameterName);
      builder.AddRequiredParameter(c_testSiteHostingDockerIsolationModeParameterName);
    }
    builder.AddRequiredParameter(c_chromeVersionArchiveParameterName);
    builder.AddRequiredParameter(c_edgeVersionArchiveParameterName);
    builder.AddRequiredParameter(c_firefoxVersionArchiveParameterName);
  }

  public void ExecuteTests (TestExecutionContext context, Action<TestExecutionContext> next)
  {
    var dockerImage = _hostTestSitesInDocker
        ? context.GetTestParameter(c_testSiteHostingDockerImageParameterName)
        : "";
    var dockerIsolationMode = _hostTestSitesInDocker
        ? context.GetTestParameter(c_testSiteHostingDockerIsolationModeParameterName)
        : "";
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
    Log.Information($" - Host test site in docker: '{(_hostTestSitesInDocker ? "yes" : "no")}'");
    if (_hostTestSitesInDocker)
    {
      Log.Information($" - Docker image: '{dockerImage}'");
      Log.Information($" - Docker isolation mode: '{dockerIsolationMode}'");
    }

    Log.Information(configFile.ReadAllText());

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

    if (_hostTestSitesInDocker)
    {
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting", "webApplicationRoot", c_dockerWebApplicationRoot);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting", "verifyWebApplicationStartedTimeout", c_dockerVerifyWebApplicationStartedTimeout);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "name", "Docker");
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "type", "Docker");
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "port", c_dockerPortNumber);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "dockerImageName", dockerImage);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "dockerIsolationMode", dockerIsolationMode);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "dockerPullTimeout", c_dockerPullTimeout);
      appConfig.SetOrAddAttribute("/configuration/rwt:remotion.webTesting/rwt:hosting", "hostname", c_dockerHostName);
    }

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
