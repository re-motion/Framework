// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections;
using System.Threading;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Context
{
  /// <summary>
  /// Implements a storage mechanism for <see cref="SafeContext"/> that uses <see cref="AsyncLocal{T}"/> to store the values.
  /// Values flow by default and <see cref="OpenSafeContextBoundary"/> is necessary to prevent flow. 
  /// </summary>
  [ImplementationFor(typeof(ISafeContextStorageProvider), Lifetime = LifetimeKind.Singleton, Position = 1)]
  public class AsyncLocalStorageProvider : ISafeContextStorageProvider, ISafeContextBoundaryStrategy
  {
    private readonly AsyncLocal<Hashtable?> _context = new();

    public AsyncLocalStorageProvider ()
    {
    }

    /// <inheritdoc />
    public SafeContextBoundary OpenSafeContextBoundary ()
    {
      var hashtable = _context.Value;
      _context.Value = null;

      return new SafeContextBoundary(this, hashtable);
    }

    /// <inheritdoc />
    public object? GetData (string key)
    {
      ArgumentUtility.CheckNotNull(nameof(key), key);

      return _context.Value?[key];
    }

    /// <inheritdoc />
    public void SetData (string key, object? value)
    {
      ArgumentUtility.CheckNotNull(nameof(key), key);

      var hashtable = _context.Value;
      if (hashtable == null)
      {
        hashtable = Hashtable.Synchronized(new Hashtable());
        _context.Value = hashtable;
      }

      hashtable[key] = value;
    }

    /// <inheritdoc />
    public void FreeData (string key)
    {
      ArgumentUtility.CheckNotNull(nameof(key), key);

      _context.Value?.Remove(key);
    }

    /// <inheritdoc />
    void ISafeContextBoundaryStrategy.RestorePreviousSafeContext (object? state)
    {
      _context.Value = (Hashtable?)state;
    }
  }
}
