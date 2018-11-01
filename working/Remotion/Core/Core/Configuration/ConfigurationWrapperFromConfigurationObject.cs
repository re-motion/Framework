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
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Configuration
{
  /// <summary>
  /// Concrete implementation of <see cref="ConfigurationWrapper"/> that uses an instance of the <see cref="System.Configuration.Configuration"/>
  /// type. Create the instance by invoking <see cref="ConfigurationWrapper.CreateFromConfigurationObject"/>.
  /// </summary>
  internal sealed class ConfigurationWrapperFromConfigurationObject : ConfigurationWrapper
  {
    private System.Configuration.Configuration _configuration;
    private NameValueCollection _appSettings;

    public ConfigurationWrapperFromConfigurationObject (System.Configuration.Configuration configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      _configuration = configuration;
      
      MethodInfo getRuntimeObject = configuration.AppSettings.GetType ().GetMethod (
        "GetRuntimeObject",
        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.ExactBinding);
      Assertion.IsNotNull (getRuntimeObject, "System.Configuration.AppSettingsSection.GetRuntimeObject() does not exist.");
      _appSettings = (NameValueCollection) getRuntimeObject.Invoke (configuration.AppSettings, new object[0]) ?? new NameValueCollection();
    }

    public override object GetSection (string sectionName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sectionName", sectionName);

      return _configuration.GetSection (sectionName);
    }

    public override ConnectionStringSettings GetConnectionString (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return _configuration.ConnectionStrings.ConnectionStrings[name];
    }

    public override string GetAppSetting (string name)
    {
       ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      
      return _appSettings[name];
    }
  }
}
