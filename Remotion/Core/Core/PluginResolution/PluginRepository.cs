// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Remotion.Utilities;

namespace Remotion.PluginResolution;

public class PluginRepository : IPluginRepository
{
  private readonly ImmutableArray<Plugin> _plugins;

  public PluginRepository (IPluginFinder pluginFinder)
  {
    ArgumentUtility.CheckNotNull(nameof(pluginFinder), pluginFinder);

    _plugins = [..pluginFinder.FindPlugins()];
  }

  /// <inheritdoc />
  public IReadOnlyList<Plugin> GetPlugins ()
  {
    return _plugins;
  }
}
