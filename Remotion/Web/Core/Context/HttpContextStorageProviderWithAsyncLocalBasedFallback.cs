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
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Web;
using Remotion.Context;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Web.Context
{
  /// <summary>
  /// Implements a storage mechanism for <see cref="SafeContext"/> that uses a <see cref="Hashtable"/> instance in <see cref="HttpContext"/>.<see cref="HttpContext.Current"/>
  /// to store the values.
  /// </summary>
  /// <remarks>
  /// Due to the <see cref="Hashtable"/> being used instead of storing the values directly into
  /// <see cref="HttpContext"/>.<see cref="HttpContext.Current"/>.<see cref="HttpContext.Items"/>, the values cannot be retrieved via the
  /// <see cref="HttpContext.Items"/> property.
  /// </remarks>
  [Obsolete("Registering AsyncLocalStorageProvider is sufficient in a .NET context. (Version: 6.0.0)")]
  public class HttpContextStorageProviderWithAsyncLocalBasedFallback : ISafeContextStorageProvider, ISafeContextBoundaryStrategy
  {
    private struct HashTableAccessor
    {
      private class WrappedHashtable
      {
        public HttpContext Context { get; }
        public Hashtable Data { get; }

        public WrappedHashtable (HttpContext context, Hashtable data)
        {
          Context = context;
          Data = data;
        }
      }

      private const string c_dataDictionaryKey = "Rm.CtxP";

      private readonly ThreadLocal<WeakReference<WrappedHashtable?>> _threadLocal;

      public HashTableAccessor (bool _)
      {
        // TODO RM-8401: Change to parameterless ctor when C# 10 is available in re-motion

        _threadLocal = new(() => new WeakReference<WrappedHashtable?>(null));
      }

      public bool TryGetHashtable (HttpContext httpContext, [MaybeNullWhen(false)] out Hashtable data)
      {
        if (_threadLocal.Value!.TryGetTarget(out var target) && ReferenceEquals(target.Context, httpContext))
        {
          data = target.Data;
          return true;
        }

        var wrappedHashtable = (WrappedHashtable?)httpContext.Items[c_dataDictionaryKey];
        if (wrappedHashtable != null)
        {
          data = wrappedHashtable.Data;
          _threadLocal.Value!.SetTarget(wrappedHashtable);
          return true;
        }

        data = null;
        return false;
      }

      public Hashtable GetOrCreateHashtable (HttpContext context)
      {
        if (TryGetHashtable(context, out var cachedHashtable))
        {
          return cachedHashtable;
        }

        var hashtable = new Hashtable();
        SetHashtable(context, hashtable);
        return hashtable;
      }

      public Hashtable? RemoveHashtable (HttpContext context)
      {
        if (TryGetHashtable(context, out var cachedHashtable))
        {
          context.Items.Remove(c_dataDictionaryKey);
          _threadLocal.Value!.SetTarget(null!);
        }

        return cachedHashtable;
      }

      public void SetHashtable (HttpContext context, Hashtable hashtable)
      {
        var wrappedHashtable = new WrappedHashtable(context, hashtable);
        context.Items[c_dataDictionaryKey] = wrappedHashtable;
        _threadLocal.Value!.SetTarget(wrappedHashtable);
      }
    }

    private readonly AsyncLocalStorageProvider _fallbackProvider = new();

    private HashTableAccessor _hashtableAccessor = new(true);

    public SafeContextBoundary OpenSafeContextBoundary ()
    {
      var httpContext = HttpContext.Current;
      if (httpContext == null)
        return _fallbackProvider.OpenSafeContextBoundary();

      var hashtableBackup = _hashtableAccessor.RemoveHashtable(httpContext);

      return new SafeContextBoundary(this, hashtableBackup);
    }

    public object? GetData (string key)
    {
      ArgumentUtility.CheckNotNull("key", key);

      var httpContext = HttpContext.Current;
      if (httpContext == null)
        return _fallbackProvider.GetData(key);

      if (_hashtableAccessor.TryGetHashtable(httpContext, out var hashtable))
        return hashtable[key];

      return null;
    }

    public void SetData (string key, object? value)
    {
      ArgumentUtility.CheckNotNull("key", key);

      var httpContext = HttpContext.Current;
      if (httpContext == null)
      {
        _fallbackProvider.SetData(key, value);
        return;
      }

      var hashtable = _hashtableAccessor.GetOrCreateHashtable(httpContext);
      hashtable[key] = value;
    }

    public void FreeData (string key)
    {
      ArgumentUtility.CheckNotNull("key", key);

      var httpContext = HttpContext.Current;
      if (httpContext == null)
      {
        _fallbackProvider.FreeData(key);
        return;
      }

      if (_hashtableAccessor.TryGetHashtable(httpContext, out var cachedHashtable))
        cachedHashtable.Remove(key);
    }

    void ISafeContextBoundaryStrategy.RestorePreviousSafeContext (object? state)
    {
      var httpContext = HttpContext.Current;

      if (state is Hashtable hashtable)
        _hashtableAccessor.SetHashtable(httpContext, hashtable);
      else
        _hashtableAccessor.RemoveHashtable(httpContext);
    }
  }
}
