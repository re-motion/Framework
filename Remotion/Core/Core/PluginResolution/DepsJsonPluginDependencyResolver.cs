// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using Remotion.Utilities;

namespace Remotion.PluginResolution;

public class DepsJsonPluginDependencyResolver : IPluginDependencyResolver
{
  private readonly AssemblyDependencyResolver _assemblyDependencyResolver;

  public DepsJsonPluginDependencyResolver (AssemblyDependencyResolver assemblyDependencyResolver)
  {
    ArgumentUtility.CheckNotNull(nameof(assemblyDependencyResolver), assemblyDependencyResolver);

    _assemblyDependencyResolver = assemblyDependencyResolver;
  }

  /// <inheritdoc />
  public Assembly? Load (PluginAssemblyLoadContext context, AssemblyName assemblyName)
  {
    var assemblyPath = _assemblyDependencyResolver.ResolveAssemblyToPath(assemblyName);
    return assemblyPath != null
        ? context.LoadFromAssemblyPath(assemblyPath)
        : null;
  }

  /// <inheritdoc />
  public IntPtr LoadUnmanagedDll (PluginAssemblyLoadContext context, string unmanagedDllName)
  {
    var assemblyPath = _assemblyDependencyResolver.ResolveUnmanagedDllToPath(unmanagedDllName);
    return assemblyPath != null
        ? NativeLibrary.Load(assemblyPath)
        : IntPtr.Zero;
  }
}
