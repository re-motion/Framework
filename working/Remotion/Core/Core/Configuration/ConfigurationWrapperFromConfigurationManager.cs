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
using Remotion.Utilities;

namespace Remotion.Configuration
{
  /// <summary>
  /// Concrete implementation of <see cref="ConfigurationWrapper"/> that uses the <see cref="ConfigurationManager"/>. Create the instance by
  /// invoking <see cref="ConfigurationWrapper.CreateFromConfigurationManager"/>.
  /// </summary>
  internal sealed class ConfigurationWrapperFromConfigurationManager: ConfigurationWrapper
  {
    public ConfigurationWrapperFromConfigurationManager()
    {
    }

    public override object GetSection (string sectionName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sectionName", sectionName);

      return ConfigurationManager.GetSection (sectionName);
    }

    public override ConnectionStringSettings GetConnectionString (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return ConfigurationManager.ConnectionStrings[name];
    }

    public override string GetAppSetting (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return ConfigurationManager.AppSettings[name];
    }
  }
}
