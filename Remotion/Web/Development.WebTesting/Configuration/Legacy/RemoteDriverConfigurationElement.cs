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
using System.Configuration;

namespace Remotion.Web.Development.WebTesting.Configuration.Legacy
{
  /// <summary>
  /// Configures the remote driver feature where a remote browser is used for the browser automation.
  /// </summary>
  public class RemoteDriverConfigurationElement : ConfigurationElement, IWebTestRemoteDriverSettings
  {
    private readonly ConfigurationProperty _enabledProperty;
    private readonly ConfigurationProperty _urlProperty;
    private readonly ConfigurationProperty _hostRemoteDriverProperty;
    private readonly ConfigurationProperty _dockerImageNameProperty;
    private readonly ConfigurationProperty _dockerCustomArgumentsProperty;

    public RemoteDriverConfigurationElement ()
    {
      _enabledProperty = new ConfigurationProperty("enabled", typeof(bool), false);
      _urlProperty = new ConfigurationProperty("url", typeof(string), "");
      _hostRemoteDriverProperty = new ConfigurationProperty("hostRemoteDriverInDocker", typeof(bool), false);
      _dockerImageNameProperty = new ConfigurationProperty("dockerImageName", typeof(string), null);
      _dockerCustomArgumentsProperty = new ConfigurationProperty("dockerCustomArguments", typeof(string), null);
    }

    /// <inheritdoc cref="IWebTestRemoteDriverSettings.Enabled" />
    public bool Enabled => (bool)this[_enabledProperty];

    /// <inheritdoc cref="IWebTestRemoteDriverSettings.Url" />
    public string Url => (string)this[_urlProperty];

    /// <inheritdoc cref="IWebTestRemoteDriverSettings.HostRemoteDriverInDocker" />
    public bool HostRemoteDriverInDocker => (bool)this[_hostRemoteDriverProperty];

    /// <inheritdoc cref="IWebTestRemoteDriverSettings.DockerImageName" />
    public string? DockerImageName => (string?)this[_dockerImageNameProperty];

    /// <inheritdoc cref="IWebTestRemoteDriverSettings.DockerCustomArguments" />
    public string? DockerCustomArguments => (string?)this[_dockerCustomArgumentsProperty];

    /// <inheritdoc />
    protected override ConfigurationPropertyCollection Properties => new() { _enabledProperty, _urlProperty, _hostRemoteDriverProperty, _dockerImageNameProperty, _dockerCustomArgumentsProperty };
  }
}
