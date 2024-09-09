// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.PluginResolution;

public class CompositePluginFinderDecorator : IPluginFinder
{
  private readonly ReadOnlyCollection<IPluginFinder> _innerPluginFinders;

  public CompositePluginFinderDecorator (IEnumerable<IPluginFinder> assemblyFinders)
  {
    ArgumentUtility.CheckNotNull(nameof(assemblyFinders), assemblyFinders);

    _innerPluginFinders = assemblyFinders.ToList().AsReadOnly();
  }

  /// <inheritdoc />
  public IEnumerable<Plugin> FindPlugins ()
  {
    return _innerPluginFinders.SelectMany(e => e.FindPlugins());
  }
}
