// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Remotion.PluginResolution;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding;

public class PluginAssemblyFinder : IAssemblyFinder
{
  private readonly IPluginRepository _pluginRepository;
  private readonly IPluginAssemblyFilter _assemblyFilter;

  public PluginAssemblyFinder (IPluginRepository pluginRepository, IPluginAssemblyFilter assemblyFilter)
  {
    ArgumentUtility.CheckNotNull(nameof(pluginRepository), pluginRepository);
    ArgumentUtility.CheckNotNull(nameof(assemblyFilter), assemblyFilter);

    _pluginRepository = pluginRepository;
    _assemblyFilter = assemblyFilter;
  }

  /// <inheritdoc />
  public IEnumerable<Assembly> FindAssemblies ()
  {
    return (from plugin in _pluginRepository.GetPlugins()
        let assemblyNames = Directory.EnumerateFiles(plugin.PluginDirectoryPath, "*.dll")
            .Select(GetAssemblyNameOrDefault)
            .Where(e => e != null && _assemblyFilter.ShouldConsiderAssembly(e))
            .Select(e => e!)
            .ToArray()
        where assemblyNames.Length != 0
        let pluginAssemblyContext = PluginAssemblyLoadContext.GetOrCreate(plugin)
        from assemblyName in assemblyNames
        select pluginAssemblyContext.LoadFromAssemblyName(assemblyName)).ToList();
  }

  private AssemblyName? GetAssemblyNameOrDefault (string path)
  {
    try
    {
      return AssemblyName.GetAssemblyName(path);
    }
    catch (BadImageFormatException)
    {
      // This is expected for native DLLs, which we ignore here
      return null;
    }
  }
}
