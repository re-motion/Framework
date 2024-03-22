// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it
// and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 2.1 of the
// License, or (at your option) any later version.
//
// re-motion is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Configuration.Legacy
{
  /// <summary>
  /// Implements the <see cref="IWebTestHostingSettings"/> to wrap a <see cref="ProviderSettings"/> instance.
  /// </summary>
  public class WebTestHostingAdapter : IWebTestHostingSettings
  {
    private class NameValueCollectionAdapter : IReadOnlyDictionary<string, string>
    {
      private readonly NameValueCollection _parameters;

      public NameValueCollectionAdapter (NameValueCollection parameters)
      {
        _parameters = parameters;
      }

      /// <inheritdoc />
      public int Count => _parameters.Count;

      /// <inheritdoc />
      public IEnumerable<string> Keys => _parameters.Keys.Cast<string>();

      /// <inheritdoc />
      public IEnumerable<string> Values => Keys.Select(e => _parameters[e]!);

      /// <inheritdoc />
      public bool ContainsKey (string key)
      {
        ArgumentUtility.CheckNotNull(nameof(key), key);

        return Keys.Any(e => e == key);
      }

      /// <inheritdoc />
      public bool TryGetValue (string key, out string value)
      {
        ArgumentUtility.CheckNotNull(nameof(key), key);

        if (ContainsKey(key))
        {
          value = _parameters.Get(key)!;
          return true;
        }

        value = default!;
        return false;
      }

      /// <inheritdoc />
      public IEnumerator<KeyValuePair<string, string>> GetEnumerator () => Keys.Select(e => new KeyValuePair<string, string>(e, _parameters[e]!)).GetEnumerator();

      /// <inheritdoc />
      IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

      /// <inheritdoc />
      public string this [string key]
      {
        get
        {
          ArgumentUtility.CheckNotNull(nameof(key), key);

          return TryGetValue(key, out var value) ? value : throw new KeyNotFoundException($"Could not find a value for the key '{key}'.");
        }
      }
    }

    private readonly ProviderSettings _providerSettings;

    private NameValueCollectionAdapter? _nameValueCollectionAdapter;

    public WebTestHostingAdapter (ProviderSettings providerSettings)
    {
      ArgumentUtility.CheckNotNull(nameof(providerSettings), providerSettings);

      _providerSettings = providerSettings;
    }

    /// <inheritdoc />
    public string Name => _providerSettings.Name;

    /// <inheritdoc />
    public string Type => _providerSettings.Type;

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Parameters
    {
      get
      {
        return _nameValueCollectionAdapter ??= new NameValueCollectionAdapter(_providerSettings.Parameters);
      }
    }
  }
}
