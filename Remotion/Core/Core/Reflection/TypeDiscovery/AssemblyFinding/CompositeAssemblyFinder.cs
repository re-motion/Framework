// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding;

/// <summary>
/// Composes several <see cref="IAssemblyFinder"/> instances into one, combining all results.
/// </summary>
public class CompositeAssemblyFinder : IAssemblyFinder
{
  private readonly IReadOnlyList<IAssemblyFinder> _assemblyFinders;

  public CompositeAssemblyFinder (IEnumerable<IAssemblyFinder> assemblyFinders)
  {
    ArgumentNullException.ThrowIfNull(assemblyFinders);

    _assemblyFinders = assemblyFinders.ToList().AsReadOnly();
  }

  /// <inheritdoc />
  public IEnumerable<Assembly> FindAssemblies ()
  {
    return _assemblyFinders.SelectMany(e => e.FindAssemblies());
  }
}
