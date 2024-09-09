// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using Remotion.Utilities;

namespace Remotion.PluginResolution;

/// <summary>
/// Locates plugins as nested directories of a specified root directory.
/// </summary>
public class NestedDirectoriesPluginFinder : IPluginFinder
{
  private readonly string _rootDirectory;

  public NestedDirectoriesPluginFinder (string rootDirectory)
  {
    ArgumentUtility.CheckNotNull(nameof(rootDirectory), rootDirectory);

    _rootDirectory = Path.GetFullPath(rootDirectory);
  }

  /// <inheritdoc />
  public IEnumerable<Plugin> FindPlugins ()
  {
    if (!Directory.Exists(_rootDirectory))
      yield break;

    foreach (var pluginDirectory in Directory.EnumerateDirectories(_rootDirectory))
    {
      var pluginName = Path.GetDirectoryName(pluginDirectory) ?? string.Empty;

      var dllFilesWithDepsJson = Directory.GetFiles(pluginDirectory, "*.deps.json")
          .Select(e => e[..^".deps.json".Length] + ".dll")
          .ToArray();

      if (dllFilesWithDepsJson.Length == 0)
      {
        yield return new Plugin(
            pluginName,
            new NaiveDirectoryPluginDependencyResolver(pluginDirectory),
            pluginDirectory);
      }
      else if (dllFilesWithDepsJson.Length == 1)
      {
        var componentAssemblyPath = dllFilesWithDepsJson[0];

        AssemblyDependencyResolver assemblyDependencyResolver;
        try
        {
          assemblyDependencyResolver = new AssemblyDependencyResolver(componentAssemblyPath);
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException($"Could not read plugin's deps json file '{componentAssemblyPath}'.", ex);
        }

        yield return new Plugin(
            pluginName,
            new DepsJsonPluginDependencyResolver(assemblyDependencyResolver),
            pluginDirectory);
      }
      else
      {
        throw new InvalidOperationException("Plugin directory contains multiple .deps.json files. Cannot determine which one to load.");
      }
    }
  }
}
