// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Reflection;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding;

/// <summary>
/// Provides an interface for filtering the assemblies found by the <see cref="PluginAssemblyFinder"/>.
/// </summary>
public interface IPluginAssemblyFilter
{
  /// <summary>
  /// Determines whether the assembly of the given name should be considered for inclusion by the <see cref="PluginAssemblyFinder"/>.
  /// </summary>
  /// <param name="assemblyName">The name of the assembly to be checked.</param>
  /// <returns><see langword="true" /> if the <see cref="PluginAssemblyFinder"/> should consider this assembly; otherwise <see langword="false" />.</returns>
  bool ShouldConsiderAssembly (AssemblyName assemblyName);
}
