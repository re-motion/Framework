// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding;

/// <summary>
/// Composes several <see cref="IAssemblyFinder"/> instances into a single <see cref="IAssemblyFinder"/>.
/// </summary>
public class CompositeAssemblyFinderDecorator : IAssemblyFinder
{
  private ReadOnlyCollection<IAssemblyFinder> _innerAssemblyFinders;

  public CompositeAssemblyFinderDecorator (IEnumerable<IAssemblyFinder> assemblyFinders)
  {
    ArgumentUtility.CheckNotNull(nameof(assemblyFinders), assemblyFinders);

    _innerAssemblyFinders = assemblyFinders.ToList().AsReadOnly();
  }

  /// <inheritdoc />
  public IEnumerable<Assembly> FindAssemblies ()
  {
    return _innerAssemblyFinders.SelectMany(e => e.FindAssemblies());
  }
}
