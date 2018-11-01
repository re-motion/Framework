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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using Remotion.Configuration;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Configuration
{
  /// <summary>
  /// Concrete implementation of <see cref="ConfigurationWrapper"/> that fakes the access to the configuration system. Use this class for setting up
  /// unit tests.
  /// </summary>
  public sealed class FakeConfigurationWrapper : ConfigurationWrapper
  {
    private Dictionary<string, object> _sections = new Dictionary<string, object> ();
    private ConnectionStringsSection _connectionStringsSection = new ConnectionStringsSection();
    private NameValueCollection _appSettings = new NameValueCollection();

    public FakeConfigurationWrapper()
    {
    }

    public void SetUpSection (string configKey, object section)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("configKey", configKey);
      ArgumentUtility.CheckNotNull ("section", section);

      _sections.Add (configKey, section);
    }

    public void SetUpConnectionString (string name, string connectionString, string providerName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNullOrEmpty ("connectionString", connectionString);

      _connectionStringsSection.ConnectionStrings.Add (new ConnectionStringSettings (name, connectionString, providerName));
    }

    public void SetUpAppSetting (string name, string value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("value", value);

      _appSettings.Add (name, value);
    }

    public override object GetSection (string sectionName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sectionName", sectionName);

      object value;
      if (_sections.TryGetValue (sectionName, out value))
        return value;
      return null;
    }

    public override ConnectionStringSettings GetConnectionString (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return _connectionStringsSection.ConnectionStrings[name];
    }

    public override string GetAppSetting (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return _appSettings[name];
    }
  }
}
