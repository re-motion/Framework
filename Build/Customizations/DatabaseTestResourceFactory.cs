// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Linq;
using Customizations.SqlServer;
using Remotion.BuildScript;
using Remotion.BuildScript.Test;

namespace Customizations;

public class DatabaseTestResourceFactory : ITestResourceFactory
{
  private const string c_imageNameParameter = "ImageName";
  private const string c_hostNameParameter = "HostName";

  public Databases Database { get; }

  public string Name => $"SQL Server {(Database.HasSpecificVersion ? Database.Version : "(default)")}";

  public DatabaseTestResourceFactory (Databases database)
  {
    Database = database;
  }

  public void ConfigureTestParameters (TestParameterBuilder builder)
  {
    builder.AddOptionalParameter(Database, c_imageNameParameter, "");
    builder.AddOptionalParameter(Database, c_hostNameParameter, $"{Database}HostName");
  }

  public void Start (TestResourceFactoryContext context)
  {
    if (Database == Databases.NoDB)
      return;

    var imageName = context.TestParameters.GetTestParameter(Database, c_imageNameParameter);
    var hostName = context.TestParameters.GetTestParameter(Database, c_hostNameParameter);

    if (string.IsNullOrEmpty(imageName))
      return;

    var network = context.TestResources
        .OfType<DockerNetworkResource>()
        .SingleOrDefault()
        ?.Network;

    ISqlDockerImage sqlServerDockerImage = OperatingSystem.IsWindows()
        ? new WindowsSqlServerDockerImage(imageName, hostName, network)
        : new LinuxSqlServerDockerImage(imageName, hostName, network);

    var databaseTestResource = new DatabaseTestResource(Name, Database, sqlServerDockerImage);
    context.TestResources.Add(databaseTestResource);
  }
}
