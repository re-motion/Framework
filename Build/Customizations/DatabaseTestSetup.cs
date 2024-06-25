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
    foreach (var sqlServer in builder.EnabledTestDimensions.OfType<Databases>())
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
    var sqlServer = context.TestMatrixRow.GetDimension<Databases>();
    string dataSource, databaseDirectory, databaseNamePrefix, integratedSecurity, username, password;
    if (sqlServer == Databases.SqlServerDefault)
    {
      dataSource = "localhost";
      databaseDirectory = @"C:\Databases\";
      databaseNamePrefix = "";
      integratedSecurity = "true";
      username = "";
      password = "";
    }
    else
    {
      Assert.True(sqlServer.HasSpecificVersion);

      dataSource = context.GetTestParameter(string.Format(c_dataSourceParameterNameTemplate, sqlServer.Version));
      databaseDirectory = context.GetTestParameter(string.Format(c_databaseDirectoryParameterNameTemplate, sqlServer.Version));
      databaseNamePrefix = context.GetTestParameter(string.Format(c_databaseNamePrefixParameterNameTemplate, sqlServer.Version));
      integratedSecurity = context.GetTestParameter(string.Format(c_integratedSecurityParameterNameTemplate, sqlServer.Version));
      username = context.GetTestParameter(string.Format(c_usernameParameterNameTemplate, sqlServer.Version));
      password = context.GetTestParameter(string.Format(c_passwordParameterNameTemplate, sqlServer.Version));
    }

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

    appConfig.WriteToFile(configFile);

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
