// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Remotion.BuildScript.Test;

namespace Customizations;

public class DockerNetworkResource : ITestResource
{
  public string Name { get; }

  public INetwork Network { get; }

  public DockerNetworkResource (string name, string networkName)
  {
    Name = name;
    Network = new NetworkBuilder()
        .WithName(networkName)
        .WithCleanUp(true)
        .Build();
  }

  public void Dispose ()
  {
    Network.DisposeAsync().AsTask().GetAwaiter().GetResult();
  }
}
