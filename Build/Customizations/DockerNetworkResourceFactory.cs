// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.BuildScript.Test;

namespace Customizations;

public class DockerNetworkResourceFactory : ITestResourceFactory
{
  public string Name { get; }

  public string NetworkName { get; }

  public DockerNetworkResourceFactory ()
  {
    var name = Guid.NewGuid().ToString("D");

    Name = $"docker:network:{name}";
    NetworkName = name;
  }

  public void ConfigureTestParameters (TestParameterBuilder builder)
  {
  }

  public ITestResource Start (TestResourceFactoryContext context)
  {
    return new DockerNetworkResource(Name, NetworkName);
  }
}
