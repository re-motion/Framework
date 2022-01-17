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
using System.Threading;
#if !NETFRAMEWORK
using Remotion.ServiceLocation;
#endif
using Remotion.Utilities;

namespace Remotion.Context
{
#if !NETFRAMEWORK
  [ImplementationFor(typeof(ISafeContextStorageProvider), Lifetime = LifetimeKind.Singleton, Position = 1)]
#endif
  public class AsyncLocalStorageProvider : ISafeContextStorageProvider
  {
    private readonly AsyncLocal<Hashtable> _context = new();

    public AsyncLocalStorageProvider ()
    {
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
  }
}
