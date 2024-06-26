// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Threading;

namespace Remotion.Context
{
  public static partial class SafeContext
  {
    /// <summary>
    /// Provides methods for creating <see cref="System.Threading.Thread"/>s which are <see cref="SafeContext"/> aware.
    /// <see cref="SafeContext"/> values do not flow into the created thread.
    /// </summary>
    public static class Thread
    {
      /// <inheritdoc cref="M:System.Threading.Thread.#ctor(System.Threading.ThreadStart)" />
      public static System.Threading.Thread New (ThreadStart start) => New(Instance, start);

      /// <inheritdoc cref="M:System.Threading.Thread.#ctor(System.Threading.ThreadStart)" />
      public static System.Threading.Thread New (ISafeContextStorageProvider provider, ThreadStart start) => new(CreateWrapper(provider, start));


      /// <inheritdoc cref="M:System.Threading.Thread.#ctor(System.Threading.ThreadStart,System.Int32)" />
      public static System.Threading.Thread New (ThreadStart start, int maxStackSize) => New(Instance, start, maxStackSize);

      /// <inheritdoc cref="M:System.Threading.Thread.#ctor(System.Threading.ThreadStart,System.Int32)" />
      public static System.Threading.Thread New (ISafeContextStorageProvider provider, ThreadStart start, int maxStackSize) => new(CreateWrapper(provider, start), maxStackSize);


      /// <inheritdoc cref="M:System.Threading.Thread.#ctor(System.Threading.ParameterizedThreadStart)" />
      public static System.Threading.Thread New (ParameterizedThreadStart start) => New(Instance, start);

      /// <inheritdoc cref="M:System.Threading.Thread.#ctor(System.Threading.ParameterizedThreadStart)" />
      public static System.Threading.Thread New (ISafeContextStorageProvider provider, ParameterizedThreadStart start) => new(CreateWrapper(provider, start));


      /// <inheritdoc cref="M:System.Threading.Thread.#ctor(System.Threading.ParameterizedThreadStart,System.Int32)" />
      public static System.Threading.Thread New (ParameterizedThreadStart start, int maxStackSize) => New(Instance, start, maxStackSize);

      /// <inheritdoc cref="M:System.Threading.Thread.#ctor(System.Threading.ParameterizedThreadStart,System.Int32)" />
      public static System.Threading.Thread New (ISafeContextStorageProvider provider, ParameterizedThreadStart start, int maxStackSize) =>
          new(CreateWrapper(provider, start), maxStackSize);


      private static ThreadStart CreateWrapper (ISafeContextStorageProvider provider, ThreadStart start)
      {
        return () =>
        {
          provider.OpenSafeContextBoundary();
          start();
        };
      }

      private static ParameterizedThreadStart CreateWrapper (ISafeContextStorageProvider provider, ParameterizedThreadStart start)
      {
        return obj =>
        {
          provider.OpenSafeContextBoundary();
          start(obj);
        };
      }
    }
  }
}
