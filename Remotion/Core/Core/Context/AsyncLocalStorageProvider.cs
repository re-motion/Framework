// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections;
using System.Threading;
#if !NETFRAMEWORK
using Remotion.ServiceLocation;
#endif
using Remotion.Utilities;

namespace Remotion.Context
{
  /// <summary>
  /// Implements a storage mechanism for <see cref="SafeContext"/> that uses <see cref="AsyncLocal{T}"/> to store the values.
  /// Values flow by default and <see cref="OpenSafeContextBoundary"/> is necessary to prevent flow. 
  /// </summary>
#if !NETFRAMEWORK
  [ImplementationFor(typeof(ISafeContextStorageProvider), Lifetime = LifetimeKind.Singleton, Position = 1)]
#endif
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

#if DEBUG && NETFRAMEWORK
      if (value is System.Runtime.Remoting.Messaging.ILogicalThreadAffinative)
        throw new NotSupportedException("Remoting is not supported.");
#endif

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
