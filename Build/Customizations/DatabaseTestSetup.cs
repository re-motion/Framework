using System;
using Nuke.Common;
using Nuke.Common.IO;
using Remotion.BuildScript;
using Remotion.BuildScript.Test;
using Remotion.BuildScript.Test.Dimensions;
using Serilog;

namespace Customizations;

public class DatabaseTestSetup : ITestExecutionWrapper, IRequiresTestParameters
{
  private const string c_dataSourceParameterNameTemplate = "DataSourceMsSql{0}";
  private const string c_databaseDirectoryParameterNameTemplate = "DatabaseDirectoryMsSql{0}";
  private const string c_integratedSecurityParameterNameTemplate = "IntegratedSecurityMsSql{0}";
  private const string c_usernameParameterNameTemplate = "UsernameMsSql{0}";
  private const string c_passwordParameterNameTemplate = "PasswordMsSql{0}";
  private const string c_databaseNamePrefixParameterNameTemplate = "DatabaseNamePrefixMsSql{0}";

  public void ConfigureTestParameters (TestParameterBuilder builder)
  {
    foreach (var sqlServer in builder.EnabledTestDimensions.OfType<SqlServers>())
    {
      if (!sqlServer.HasSpecificVersion)
        continue;

      builder.AddRequiredParameter(string.Format(c_dataSourceParameterNameTemplate, sqlServer.Version));
      builder.AddRequiredParameter(string.Format(c_databaseDirectoryParameterNameTemplate, sqlServer.Version));
      builder.AddOptionalParameter(string.Format(c_databaseNamePrefixParameterNameTemplate, sqlServer.Version), "");
      builder.AddRequiredParameter(string.Format(c_integratedSecurityParameterNameTemplate, sqlServer.Version));
      builder.AddRequiredParameter(string.Format(c_usernameParameterNameTemplate, sqlServer.Version));
      builder.AddRequiredParameter(string.Format(c_passwordParameterNameTemplate, sqlServer.Version));
    }
  }

  public void ExecuteTests (TestExecutionContext context, Action<TestExecutionContext> next)
  {
    var sqlServer = context.TestMatrixRow.GetDimension<SqlServers>();
    var dataSource = context.GetTestParameter(string.Format(c_dataSourceParameterNameTemplate, sqlServer.Version));
    var databaseDirectory = context.GetTestParameter(string.Format(c_databaseDirectoryParameterNameTemplate, sqlServer.Version));
    var databaseNamePrefix = context.GetTestParameter(string.Format(c_databaseNamePrefixParameterNameTemplate, sqlServer.Version));
    var integratedSecurity = context.GetTestParameter(string.Format(c_integratedSecurityParameterNameTemplate, sqlServer.Version));
    var username = context.GetTestParameter(string.Format(c_usernameParameterNameTemplate, sqlServer.Version));
    var password = context.GetTestParameter(string.Format(c_passwordParameterNameTemplate, sqlServer.Version));

    var configuration = context.TestMatrixRow.GetDimension<Configurations>().Value;
    var targetFramework = context.TestMatrixRow.GetDimension<TargetFrameworks>().Identifier;

    var assemblyName = context.Project.GetMetadata(RemotionBuildMetadataProperties.AssemblyName);

    var configFile = context.Project.FolderPath / "bin" / configuration / targetFramework / $"{assemblyName}.dll.config";
    Assert.FileExists(configFile);

    var appConfig = AppConfig.Read(configFile);

    Log.Information("Updating Database Test configuration file:");
    Log.Information($" - Database system: '{sqlServer}'");
    Log.Information($" - Data source: '{dataSource}'");
    Log.Information($" - Database directory: '{databaseDirectory}'");
    Log.Information($" - Database name prefix: '{databaseNamePrefix}'");
    Log.Information($" - Integrated security: '{integratedSecurity}'");
    Log.Information($" - Username: '{username}'");
    Log.Information($" - Password: '{password}'");

    appConfig.SetAppSetting("DataSource", dataSource);
    appConfig.SetAppSetting("DatabaseDirectory", databaseDirectory);
    appConfig.SetAppSetting("DatabaseNamePrefix", databaseNamePrefix);
    appConfig.SetAppSetting("IntegratedSecurity", integratedSecurity);
    appConfig.SetAppSetting("Username", username);
    appConfig.SetAppSetting("Password", password);

    next(context);
  }

  private static void UpdateConfigFile (AbsolutePath configFile, string xPath, string value)
  {
    XmlTasks.XmlPoke(
        configFile,
        xPath,
        value);
  }
}
