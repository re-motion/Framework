// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Linq;
using Nuke.Common.Tools.Docker;
using Remotion.BuildScript.Test;
using Remotion.BuildScript.Test.Runtimes;

namespace Customizations;

public class DockerNetworkDockerRunSettingsCustomizer : IDockerRunSettingsCustomizer
{
  public DockerRunSettings CustomizeDockerRunSettings (TestExecutionContext context, DockerRunSettings settings)
  {
    var dockerNetworkResource = context.TestResources
        .OfType<DockerNetworkResource>()
        .SingleOrDefault();

    return dockerNetworkResource != null
        ? settings.SetNetwork(dockerNetworkResource.Network.Name)
        : settings;
  }
}
