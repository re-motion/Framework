// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Threading;

namespace Remotion.Context
{
  public static partial class SafeContext
  {
    /// <summary>
    /// Provides methods for creating <see cref="System.Threading.Timer"/>s which are <see cref="SafeContext"/> aware.
    /// </summary>
    public static class Threading
    {
      /// <inheritdoc cref="M:System.Threading.Timer.#ctor(System.Threading.TimerCallback)" />
      public static Timer NewTimer (TimerCallback callback) => NewTimer(Instance, callback);

      /// <inheritdoc cref="M:System.Threading.Timer.#ctor(System.Threading.TimerCallback)" />
      public static Timer NewTimer (ISafeContextStorageProvider provider, TimerCallback callback) => new(CreateWrapper(provider, callback));

      /// <inheritdoc cref="M:System.Threading.Timer.#ctor(System.Threading.TimerCallback,System.Object,System.TimeSpan,System.TimeSpan)" />
      public static Timer NewTimer (TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period) => NewTimer(Instance, callback, state, dueTime, period);

      /// <inheritdoc cref="M:System.Threading.Timer.#ctor(System.Threading.TimerCallback,System.Object,System.TimeSpan,System.TimeSpan)" />
      public static Timer NewTimer (ISafeContextStorageProvider provider, TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period) =>
          new(CreateWrapper(provider, callback), state, dueTime, period);

      /// <inheritdoc cref="M:System.Threading.Timer.#ctor(System.Threading.TimerCallback,System.Object,System.Int32,System.Int32)" />
      public static Timer NewTimer (TimerCallback callback, object state, int dueTime, int period) => NewTimer(Instance, callback, state, dueTime, period);

      /// <inheritdoc cref="M:System.Threading.Timer.#ctor(System.Threading.TimerCallback,System.Object,System.Int32,System.Int32)" />
      public static Timer NewTimer (ISafeContextStorageProvider provider, TimerCallback callback, object state, int dueTime, int period) =>
          new(CreateWrapper(provider, callback), state, dueTime, period);

      /// <inheritdoc cref="M:System.Threading.Timer.#ctor(System.Threading.TimerCallback,System.Object,System.Int64,System.Int64)" />
      public static Timer NewTimer (TimerCallback callback, object state, long dueTime, long period) => NewTimer(Instance, callback, state, dueTime, period);

      /// <inheritdoc cref="M:System.Threading.Timer.#ctor(System.Threading.TimerCallback,System.Object,System.Int64,System.Int64)" />
      public static Timer NewTimer (ISafeContextStorageProvider provider, TimerCallback callback, object state, long dueTime, long period) =>
          new(CreateWrapper(provider, callback), state, dueTime, period);

      // Ignore uint not being CLS compliant
#pragma warning disable CS3001

      /// <inheritdoc cref="M:System.Threading.Timer.#ctor(System.Threading.TimerCallback,System.Object,System.UInt32,System.UInt32)" />
      public static Timer NewTimer (TimerCallback callback, object state, uint dueTime, uint period) => NewTimer(Instance, callback, state, dueTime, period);

      /// <inheritdoc cref="M:System.Threading.Timer.#ctor(System.Threading.TimerCallback,System.Object,System.UInt32,System.UInt32)" />
      public static Timer NewTimer (ISafeContextStorageProvider provider, TimerCallback callback, object state, uint dueTime, uint period) =>
          new(CreateWrapper(provider, callback), state, dueTime, period);

#pragma warning restore CS3001

      private static TimerCallback CreateWrapper (ISafeContextStorageProvider provider, TimerCallback callback)
      {
        return state =>
        {
          provider.OpenSafeContextBoundary();
          callback(state);
        };
      }
    }
  }
}
