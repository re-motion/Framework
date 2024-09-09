// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;

namespace Remotion.PluginResolution;

/// <summary>
/// Provides an interface for classes that find plugins.
/// </summary>
/// <threadsafety static="true" instance="true" />
public interface IPluginFinder
{
    /// <summary>
    /// Finds plugins as defined by implementers of this interface.
    /// </summary>
    IEnumerable<Plugin> FindPlugins ();
}
