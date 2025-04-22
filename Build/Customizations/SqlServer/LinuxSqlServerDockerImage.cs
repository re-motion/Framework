// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using JetBrains.Annotations;
using Serilog;

namespace Customizations.SqlServer;

public class LinuxSqlServerDockerImage : ISqlDockerImage
{
  private readonly IContainer _container;
  private readonly string _connectionString;

  public LinuxSqlServerDockerImage (string imageName, string hostName, [CanBeNull] INetwork network)
  {
    const string password = "P@ssw0rd";
    var containerBuilder = new ContainerBuilder()
        .WithImage(imageName)
        .WithHostname(hostName)
        .WithName(hostName)
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithEnvironment("MSSQL_SA_PASSWORD", password);

    if (network != null)
      containerBuilder = containerBuilder.WithNetwork(network);

    _container = containerBuilder.Build();

    Log.Information($"Starting SQL Server using image '{imageName}'.");
    _container.StartAsync().GetAwaiter().GetResult();

    var properties = new Dictionary<string, string>
                     {
                         { "Server", hostName },
                         { "Initial Catalog", "master" },
                         { "User Id", "sa" },
                         { "Password", password },
                         { "TrustServerCertificate", bool.TrueString }
                     };
    _connectionString = string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    Log.Information($"Connection string: '{_connectionString}'");
  }

  public void Dispose ()
  {
    _container.DisposeAsync().GetAwaiter().GetResult();
  }

  public string GetConnectionString () => _connectionString;
}
