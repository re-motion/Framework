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
