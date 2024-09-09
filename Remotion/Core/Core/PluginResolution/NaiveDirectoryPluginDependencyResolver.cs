// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.IO;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.PluginResolution;

public class NaiveDirectoryPluginDependencyResolver : IPluginDependencyResolver
{
  private readonly string _baseDirectory;

  public NaiveDirectoryPluginDependencyResolver (string baseDirectory)
  {
    ArgumentUtility.CheckNotNull(nameof(baseDirectory), baseDirectory);

    _baseDirectory = Path.GetFullPath(baseDirectory);
  }

  /// <inheritdoc />
  public Assembly? Load (PluginAssemblyLoadContext context, AssemblyName assemblyName)
  {
    if (assemblyName.Name == null)
      return null;

    var assemblyPath = Path.Combine(_baseDirectory, assemblyName.Name + ".dll");
    return File.Exists(assemblyPath)
        ? context.LoadFromAssemblyPath(assemblyPath)
        : null;
  }

  /// <inheritdoc />
  public IntPtr LoadUnmanagedDll (PluginAssemblyLoadContext context, string unmanagedDllName)
  {
    // Do not support loading unmanaged DLLs from the plugin directory -> use .deps.json for unmanaged support
    return IntPtr.Zero;
  }
}
