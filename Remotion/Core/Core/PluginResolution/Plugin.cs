// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.IO;
using Remotion.Utilities;

namespace Remotion.PluginResolution;

public class Plugin
{
  public string Name { get; }

  public IPluginDependencyResolver DependencyResolver { get; }

  public string PluginDirectoryPath { get; }

  public Plugin (string name, IPluginDependencyResolver dependencyResolver, string pluginDirectoryPath)
  {
    ArgumentUtility.CheckNotNullOrEmpty(nameof(name), name);
    ArgumentUtility.CheckNotNull(nameof(dependencyResolver), dependencyResolver);
    ArgumentUtility.CheckNotNullOrEmpty(nameof(pluginDirectoryPath), pluginDirectoryPath);

    Name = name;
    DependencyResolver = dependencyResolver;

    PluginDirectoryPath = Path.GetFullPath(pluginDirectoryPath);
    if (!Directory.Exists(PluginDirectoryPath))
      throw new DirectoryNotFoundException($"Cannot find the specified plugin directory '{PluginDirectoryPath}'.");
  }
}
