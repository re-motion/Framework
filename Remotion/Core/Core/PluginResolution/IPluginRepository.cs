// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;

namespace Remotion.PluginResolution;

public interface IPluginRepository
{
  IReadOnlyList<Plugin> GetPlugins ();
}
