// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remotion.Web.ContentSecurityPolicy;

/// <summary>
/// Implements <see cref="ICspClientScriptManager"/> by collecting all the registered scripts.
/// Collected scripts can be retrieved using <see cref="GetCollectedScripts"/>.
/// </summary>
public class CollectingCspClientScriptManager : ICspClientScriptManager
{
  private record struct Entry (int Position, string Script);

  private readonly Dictionary<string, Entry> _scripts = new();

  private int _nextPosition = 0;

  public string GetCollectedScripts ()
  {
    var sb = new StringBuilder();

    var first = true;
    foreach (var entry in _scripts.Values.OrderBy(e => e.Position))
    {
      if (!first)
        sb.AppendLine();
      first = false;

      sb.Append(entry.Script);
    }

    return sb.ToString();
  }

  /// <inheritdoc />
  public void RegisterScript (string key, string script)
  {
    ArgumentException.ThrowIfNullOrEmpty(key);
    ArgumentException.ThrowIfNullOrEmpty(script);

    var position = _nextPosition;
    _nextPosition += 1;

    _scripts[key] = new Entry(position, script);
  }
}
