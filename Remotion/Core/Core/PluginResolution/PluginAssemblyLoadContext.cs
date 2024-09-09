// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Remotion.Utilities;

namespace Remotion.PluginResolution;

public sealed class PluginAssemblyLoadContext : AssemblyLoadContext
{
  private static readonly ConditionalWeakTable<Plugin, PluginAssemblyLoadContext> s_contextLookup = new();

  public static bool TryGet (Plugin plugin, [NotNullWhen(true)] out PluginAssemblyLoadContext? context)
  {
    // We don't need to lock as ConditionalWeakTable is already thread-safe, and we are not mutating the collection here
    // ReSharper disable once InconsistentlySynchronizedField
    return s_contextLookup.TryGetValue(plugin, out context);
  }

  public static PluginAssemblyLoadContext GetOrCreate (Plugin plugin)
  {
    // Lock to prevent creating the same plugin context twice
    lock (s_contextLookup)
    {
      if (s_contextLookup.TryGetValue(plugin, out var context))
        return context;

      var pluginAssemblyContext = new PluginAssemblyLoadContext(plugin);
      s_contextLookup.Add(plugin, pluginAssemblyContext);

      return pluginAssemblyContext;
    }
  }

  public Plugin Plugin { get; }

  private PluginAssemblyLoadContext (Plugin plugin)
      : base($"Plugin '{ArgumentUtility.CheckNotNull(nameof(plugin), plugin).Name}'")
  {
    Plugin = plugin;
  }

  /// <inheritdoc />
  protected override Assembly? Load (AssemblyName assemblyName)
  {
    return Plugin.DependencyResolver.Load(this, assemblyName);
  }

  /// <inheritdoc />
  protected override IntPtr LoadUnmanagedDll (string unmanagedDllName)
  {
    return Plugin.DependencyResolver.LoadUnmanagedDll(this, unmanagedDllName);
  }
}
