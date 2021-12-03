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
using Remotion.ServiceLocation;
using Remotion.Utilities;
using System.Runtime.CompilerServices;
using Remotion.Logging;

namespace Remotion.Context
{
#if !NETFRAMEWORK
  [ImplementationFor(typeof(ISafeContextStorageProvider), Lifetime = LifetimeKind.Singleton, Position = 1)]
#endif
  [Obsolete("This type is experimental and might have unintended side effects. (Version 3.0.0-alpha.22)")]
  public class AsyncLocalStorageProvider : ISafeContextStorageProvider
  {
    /// <summary>
    /// Represents the context which stores values and is itself stored in the <see cref="AsyncLocal{T}"/>.
    /// Is immutable because of the strange immutability constraints of <see cref="ExecutionContext"/>.
    /// </summary>
    /// <remarks>
    /// In addition to the data stored in a hashtabe it also contains a reference to the previous context.
    /// This is used to detect if a context is restored and reapply the values (see <see cref="AsyncLocalStorageProvider.AsyncLocalValueChangedHandler"/> below).
    /// </remarks>
    private class StorageContext
    {
      public static readonly StorageContext Empty = new StorageContext(null, null);

      public static StorageContext FromPrevious (StorageContext? previous) => previous != null ? new StorageContext(null, new WeakReference<StorageContext>(previous)) : Empty;

      private readonly WeakReference<StorageContext>? _previous;
      private readonly Hashtable? _hashtable;

      public StorageContext? Previous => _previous != null && _previous.TryGetTarget(out var result) ? result : null;

      private StorageContext (Hashtable? hashtable, WeakReference<StorageContext>? previous)
      {
        _previous = previous;
        _hashtable = hashtable;
      }

      public object? GetData (string key) => _hashtable?[key];

      public StorageContext SetData (string key, object? newValue)
      {
        // We do not need to store null values as getting a non-existing value will return null (and there is no "Contains")
        if (newValue == null)
          return FreeData(key);

        if (_hashtable == null)
        {
          var hashtable = new Hashtable();
          hashtable[key] = newValue;

          return new StorageContext(hashtable, _previous);
        }

        // Make sure that this set will change anything to avoid creating another container for nothing
        var oldValue = _hashtable[key];
        if (oldValue == newValue)
          return this;

        var newHashTable = (Hashtable)_hashtable.Clone();
        newHashTable[key] = newValue;

        return new StorageContext(newHashTable, _previous);
      }

      public StorageContext FreeData (string key)
      {
        if (_hashtable == null)
          return this;

        if (!_hashtable.ContainsKey(key))
          return this;

        var newHashtable = (Hashtable)_hashtable.Clone();
        newHashtable.Remove(key);

        return new StorageContext(newHashtable, _previous);
      }
    }

    private class ObjectIdentifier
    {
      public int Value;

      public ObjectIdentifier ()
      {
        Value = -1;
      }
    }

    private static readonly Lazy<ILog> s_log = new(() => LogManager.GetLogger(typeof(AsyncLocalStorageProvider)));

    private static readonly ConditionalWeakTable<StorageContext, ObjectIdentifier> s_objectIdentifiers = new();
    private static int s_nextObjectIdentifier;

    public static AsyncLocalStorageProvider CreateWithTracing () => new(true);

    private static string GetObjectIdentifier (StorageContext? context)
    {
      if (context == null)
        return "null";

      var orCreateValue = s_objectIdentifiers.GetOrCreateValue(context);
      if (orCreateValue.Value < 0)
      {
        orCreateValue.Value = Interlocked.Increment(ref s_nextObjectIdentifier);
      }

      return $"${orCreateValue.Value}";
    }

    private readonly bool _tracingEnabled;
    private readonly AsyncLocal<StorageContext> _context;

    public AsyncLocalStorageProvider ()
        : this(false)
    {
    }

    private AsyncLocalStorageProvider (bool tracingEnabled)
    {
      _tracingEnabled = tracingEnabled;
      _context = new AsyncLocal<StorageContext>(AsyncLocalValueChangedHandler);
    }

    private void AsyncLocalValueChangedHandler (AsyncLocalValueChangedArgs<StorageContext> args)
    {
      var previousContainer = args.PreviousValue;
      var currentContainer = args.CurrentValue;

      if (_tracingEnabled)
      {
        var threadID = Thread.CurrentThread.ManagedThreadId;
        var contextChanged = args.ThreadContextChanged ? "yes" : "no";
        var previousContainerIdentifier = GetObjectIdentifier(previousContainer);
        var currentContainerIdentifier = GetObjectIdentifier(currentContainer);
        var message = $"[{threadID}] change(cnx change {contextChanged}, prev {previousContainerIdentifier}, curr: {currentContainerIdentifier})";
        s_log.Value.Info(message);
      }

      if (!args.ThreadContextChanged)
      {
        Assertion.DebugAssert(args.CurrentValue != null, "args.CurrentValue != null");
        return;
      }

      // If the thread context has changed there are two possibilities:
      //  a) we have flowed to a new context, or
      //  b) we were restored back to an old context
      // Sadly, this information is not available here, but it should be possible to differentiate the two scenarios.
      // In scenario a), we always have a transition from null -> new container
      //  -> We use a new empty container
      // In scenario b), we are first switch from old -> null and then switch to null -> new. The restore is then a new -> old notification
      //  -> We use the old container without changes
      //  To differentiate scenario b) from a) we have to set a new context when going from old -> null
      // 
      // (null, null) -> ignore
      // (null, *)    -> scenario a)
      // (*, null)    -> remember * and set special context
      // (*, *)       -> scenario a) if no special context, otherwise b)

      StorageContext newStorageContext;
      if (previousContainer == null)
      {
        // (null, null) -> ignore
        if (currentContainer == null)
          return;

        // (null, *)    -> scenario a)
        newStorageContext = StorageContext.Empty;
      }
      else
      {
        // (*, null)    -> remember * and set special context
        if (currentContainer == null)
        {
          newStorageContext = StorageContext.FromPrevious(previousContainer);
        }
        // (*, *)       -> scenario a) if no special context, otherwise b)
        else
        {
          newStorageContext = previousContainer.Previous == currentContainer
              ? currentContainer
              : StorageContext.FromPrevious(previousContainer);
        }
      }

      // Update only if the container changed to prevent unnecessary updates
      if (currentContainer != newStorageContext)
        _context.Value = newStorageContext;
    }

    /// <inheritdoc />
    public object? GetData (string key)
    {
      ArgumentUtility.CheckNotNull("key", key);

      return _context.Value?.GetData(key);
    }

    /// <inheritdoc />
    public void SetData (string key, object? value)
    {
      ArgumentUtility.CheckNotNull("key", key);

#if DEBUG && NETFRAMEWORK
      if (value is System.Runtime.Remoting.Messaging.ILogicalThreadAffinative)
        throw new NotSupportedException("Remoting is not supported.");
#endif

      var oldValue = _context.Value;
      var newValue = (oldValue ?? StorageContext.Empty).SetData(key, value);
      if (oldValue != newValue)
        _context.Value = newValue;
    }

    /// <inheritdoc />
    public void FreeData (string key)
    {
      ArgumentUtility.CheckNotNull("key", key);

      var oldValue = _context.Value;
      if (oldValue == null)
        return;

      var newValue = oldValue.FreeData(key);
      if (oldValue != newValue)
        _context.Value = newValue;
    }
  }
}
