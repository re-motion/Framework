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
using System.IO;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Tools
{
  /// <summary>
  /// Base class for executing code that should run in a different base directory and or use a different config file.
  /// </summary>
  public abstract class CustomAppContextRunnerBase
  {
    private readonly string _baseDirectory;
    private readonly string? _configFile;

    protected CustomAppContextRunnerBase (string baseDirectory, string? configFile)
    {
      ArgumentUtility.CheckNotNull("baseDirectory", baseDirectory);

      _baseDirectory = baseDirectory;
      _configFile = configFile;
    }

    protected abstract void RunImplementation ();

    public void Run ()
    {
      var baseDirectory = Path.GetFullPath(_baseDirectory);
      AppDomain.CurrentDomain.SetData("APP_CONTEXT_BASE_DIRECTORY", baseDirectory);

      if (!string.IsNullOrEmpty(_configFile))
      {
        var configFile = Path.GetFullPath(_configFile);
        if (!File.Exists(configFile))
        {
          throw new FileNotFoundException(
              $"The configuration file supplied by the 'config' parameter was not found.\r\nFile: {configFile}",
              configFile);
        }

        // We check the internal state of ConfigurationManager to ensure that no configuration has been loaded yet
        // Otherwise, setting APP_CONFIG_FILE won't have the desired effect
        var configurationStateField = Assertion.IsNotNull(
            typeof(ConfigurationManager).GetField("s_initState", BindingFlags.NonPublic | BindingFlags.Static),
            $"Field ConfigurationManager.{"s_initState"} does not exist.");
        var configurationState = (int)configurationStateField.GetValue(null)!;
        if (configurationState != 0)
          throw new InvalidOperationException("Cannot set the active config file as the configuration is already loaded.");

        AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", configFile);
      }

      // Changing the base directory won't load assemblies from there so we need to resolve them manually
      AppDomain.CurrentDomain.AssemblyResolve += (_, eventArgs) =>
      {
        var assemblyName = new AssemblyName(eventArgs.Name);

        var assemblyFilePath = Path.Combine(baseDirectory, assemblyName.Name + ".dll");
        if (File.Exists(assemblyFilePath))
          return Assembly.LoadFile(assemblyFilePath);

        return null;
      };

      RunImplementation();
    }
  }
}
