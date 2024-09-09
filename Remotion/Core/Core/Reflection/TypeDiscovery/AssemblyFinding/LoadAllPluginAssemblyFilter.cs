// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Reflection;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding;

/// <summary>
/// Provides an implementation of <see cref="IPluginAssemblyFilter"/> that loads all assemblies.
/// </summary>
public class LoadAllPluginAssemblyFilter : IPluginAssemblyFilter
{
  public LoadAllPluginAssemblyFilter ()
  {
  }

  /// <inheritdoc />
  public bool ShouldConsiderAssembly (AssemblyName assemblyName)
  {
    return true;
  }
}
