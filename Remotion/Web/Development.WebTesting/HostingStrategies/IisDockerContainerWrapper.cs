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
using Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

namespace Remotion.Web.Development.WebTesting.HostingStrategies
{
  /// <summary>
  /// Represents an IIS Docker container and manages its lifecycle.
  /// </summary>
  public class IisDockerContainerWrapper : DockerContainerWrapperBase
  {
    public IisDockerContainerWrapper (
        IDockerClient docker,
        DockerContainerConfigurationParameters configurationParameters)
        : base(docker, configurationParameters)
    {
    }

    protected override string GetEntryPoint ()
    {
      return "powershell";
    }

    protected override string? GetWorkingDirectory ()
    {
      return null;
    }

    protected override string GetArguments ()
    {
      return $@"-Command ""
C:\Windows\System32\inetsrv\appcmd.exe set apppool /apppool.name:DefaultAppPool /enable32bitapponwin64:{ConfigurationParameters.Is32BitProcess};
C:\Windows\System32\inetsrv\appcmd.exe add site /name:WebTestingSite /physicalPath:""{ConfigurationParameters.AbsoluteWebApplicationPath}"" /bindings:""http://*:{ConfigurationParameters.WebApplicationPort}"";
C:\ServiceMonitor.exe w3svc;
""";
    }
  }
}
