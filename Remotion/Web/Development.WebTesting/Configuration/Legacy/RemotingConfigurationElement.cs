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
  public class RemotingConfigurationElement : ConfigurationElement, IWebTestRemotingSettings
  {
    private readonly ConfigurationProperty _enabledProperty;
    private readonly ConfigurationProperty _urlProperty;

    public RemotingConfigurationElement ()
    {
      _enabledProperty = new ConfigurationProperty("enabled", typeof(bool), false);
      _urlProperty = new ConfigurationProperty("url", typeof(string), "");
    }

    /// <inheritdoc cref="IWebTestRemotingSettings.Enabled" />
    public bool Enabled => (bool)this[_enabledProperty];

    /// <inheritdoc cref="IWebTestRemotingSettings.Url" />
    public string Url => (string)this[_urlProperty];

    /// <inheritdoc />
    protected override ConfigurationPropertyCollection Properties => new() { _enabledProperty, _urlProperty };
  }
}
