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

public class WindowsSqlServerDockerImage : ISqlDockerImage
{
  private readonly IContainer _container;
  private readonly string _connectionString;

  public WindowsSqlServerDockerImage (string imageName, string name, [CanBeNull] INetwork network)
  {
    // Using a hard-coded password here makes debugging a bit easier as you don't have to determine the current password.
    // This is not a problem as we don't expose the docker image outside the localhost binding.
    const string password = "P@ssw0rd";

    var containerBuilder = new ContainerBuilder()
        .WithImage(imageName)
        .WithHostname(name)
        .WithName(name)
        .WithEnvironment("SaPassword", password)
        .WithEnvironment("MinSqlServerMemory", "0")
        .WithEnvironment("MaxSqlServerMemory", "8192")
        .WithCreateParameterModifier(e => e.HostConfig.Isolation = "hyperv");

    if (network != null)
      containerBuilder = containerBuilder.WithNetwork(network);

    _container = containerBuilder.Build();

    Log.Information($"Starting SQL Server using image '{imageName}'.");
    _container.StartAsync().GetAwaiter().GetResult();

    var properties = new Dictionary<string, string>
                     {
                         { "Server", name },
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
