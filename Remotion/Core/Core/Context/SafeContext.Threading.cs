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
      public static Timer NewTimer (ISafeContextStorageProvider provider, TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period) => new(CreateWrapper(provider, callback), state, dueTime, period);

      /// <inheritdoc cref="M:System.Threading.Timer.#ctor(System.Threading.TimerCallback,System.Object,System.Int32,System.Int32)" />
      public static Timer NewTimer (TimerCallback callback, object state, int dueTime, int period) => NewTimer(Instance, callback, state, dueTime, period);

      /// <inheritdoc cref="M:System.Threading.Timer.#ctor(System.Threading.TimerCallback,System.Object,System.Int32,System.Int32)" />
      public static Timer NewTimer (ISafeContextStorageProvider provider, TimerCallback callback, object state, int dueTime, int period) => new(CreateWrapper(provider, callback), state, dueTime, period);

      /// <inheritdoc cref="M:System.Threading.Timer.#ctor(System.Threading.TimerCallback,System.Object,System.Int64,System.Int64)" />
      public static Timer NewTimer (TimerCallback callback, object state, long dueTime, long period) => NewTimer(Instance, callback, state, dueTime, period);

      /// <inheritdoc cref="M:System.Threading.Timer.#ctor(System.Threading.TimerCallback,System.Object,System.Int64,System.Int64)" />
      public static Timer NewTimer (ISafeContextStorageProvider provider, TimerCallback callback, object state, long dueTime, long period) => new(CreateWrapper(provider, callback), state, dueTime, period);

      // Ignore uint not being CLS compliant
#pragma warning disable CS3001

      /// <inheritdoc cref="M:System.Threading.Timer.#ctor(System.Threading.TimerCallback,System.Object,System.UInt32,System.UInt32)" />
      public static Timer NewTimer (TimerCallback callback, object state, uint dueTime, uint period) => NewTimer(Instance, callback, state, dueTime, period);

      /// <inheritdoc cref="M:System.Threading.Timer.#ctor(System.Threading.TimerCallback,System.Object,System.UInt32,System.UInt32)" />
      public static Timer NewTimer (ISafeContextStorageProvider provider, TimerCallback callback, object state, uint dueTime, uint period) => new(CreateWrapper(provider, callback), state, dueTime, period);

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
