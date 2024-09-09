// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Reflection;

namespace Remotion.PluginResolution;

/// <summary>
/// Provides an interface for classes that resolve a plugin's dependencies.
/// </summary>
/// <threadsafety static="true" instance="true" />
public interface IPluginDependencyResolver
{
  Assembly? Load (PluginAssemblyLoadContext context, AssemblyName assemblyName);

  IntPtr LoadUnmanagedDll (PluginAssemblyLoadContext context, string unmanagedDllName);
}
