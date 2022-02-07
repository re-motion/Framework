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
using System.Threading.Tasks;
// We must use an alias as the class name is the same
using Task_ = System.Threading.Tasks.Task;

namespace Remotion.Context
{
  public static partial class SafeContext
  {
    /// <summary>
    /// Provides methods for running <see cref="Task_"/>s which are <see cref="SafeContext"/> aware.
    /// <see cref="SafeContext"/> values do not flow into the executed tasks.
    /// </summary>
    public static class Task
    {
      /// <inheritdoc cref="M:Task_.Run(System.Action)" />
      public static Task_ Run (Action action) => Run(Instance, action);

      /// <inheritdoc cref="M:Task_.Run(System.Action)" />
      public static Task_ Run (ISafeContextStorageProvider provider, Action action) => Task_.Run(CreateWrapper(provider, action));

      /// <inheritdoc cref="M:Task_.Run(System.Action,System.Threading.CancellationToken)" />
      public static Task_ Run (Action action, CancellationToken cancellationToken) => Run(Instance, action, cancellationToken);

      /// <inheritdoc cref="M:Task_.Run(System.Action,System.Threading.CancellationToken)" />
      public static Task_ Run (ISafeContextStorageProvider provider, Action action, CancellationToken cancellationToken) =>
          Task_.Run(CreateWrapper(provider, action), cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run``1(System.Func{``0})" />
      public static Task<T> Run<T> (Func<T> function) => Run(Instance, function);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run``1(System.Func{``0})" />
      public static Task<T> Run<T> (ISafeContextStorageProvider provider, Func<T> function) => Task_.Run(CreateWrapper(provider, function));

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run``1(System.Func{``0},System.Threading.CancellationToken)" />
      public static Task<T> Run<T> (Func<T> function, CancellationToken cancellationToken) => Run(Instance, function, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run``1(System.Func{``0},System.Threading.CancellationToken)" />
      public static Task<T> Run<T> (ISafeContextStorageProvider provider, Func<T> function, CancellationToken cancellationToken) =>
          Task_.Run(CreateWrapper(provider, function), cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run(System.Func{System.Threading.Tasks.Task})" />
      public static Task_ Run (Func<Task_> function) => Run(Instance, function);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run(System.Func{System.Threading.Tasks.Task})" />
      public static Task_ Run (ISafeContextStorageProvider provider, Func<Task_> function) => Task_.Run(CreateWrapper(provider, function));

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run(System.Func{System.Threading.Tasks.Task},System.Threading.CancellationToken)" />
      public static Task_ Run (Func<Task_> function, CancellationToken cancellationToken) => Run(Instance, function, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run(System.Func{System.Threading.Tasks.Task},System.Threading.CancellationToken)" />
      public static Task_ Run (ISafeContextStorageProvider provider, Func<Task_> function, CancellationToken cancellationToken) =>
          Task_.Run(CreateWrapper(provider, function), cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run``1(System.Func{System.Threading.Tasks.Task{``0}})" />
      public static Task<T> Run<T> (Func<Task<T>> function) => Run(Instance, function);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run``1(System.Func{System.Threading.Tasks.Task{``0}})" />
      public static Task<T> Run<T> (ISafeContextStorageProvider provider, Func<Task<T>> function) => Task_.Run(CreateWrapper(provider, function));

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run``1(System.Func{System.Threading.Tasks.Task{``0}},System.Threading.CancellationToken)" />
      public static Task<T> Run<T> (Func<Task<T>> function, CancellationToken cancellationToken) => Run(Instance, function, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run``1(System.Func{System.Threading.Tasks.Task{``0}},System.Threading.CancellationToken)" />
      public static Task<T> Run<T> (ISafeContextStorageProvider provider, Func<Task<T>> function, CancellationToken cancellationToken) =>
          Task_.Run(CreateWrapper(provider, function), cancellationToken);

      private static Action CreateWrapper (ISafeContextStorageProvider provider, Action action)
      {
        return () =>
        {
          provider.OpenSafeContextBoundary();
          action();
        };
      }

      private static Func<T> CreateWrapper<T> (ISafeContextStorageProvider provider, Func<T> function)
      {
        return () =>
        {
          provider.OpenSafeContextBoundary();
          return function();
        };
      }

      private static Func<Task_> CreateWrapper (
          ISafeContextStorageProvider provider,
          Func<Task_> function)
      {
        return async () =>
        {
          provider.OpenSafeContextBoundary();
          await function();
        };
      }

      private static Func<Task<T>> CreateWrapper<T> (
          ISafeContextStorageProvider provider,
          Func<Task<T>> function)
      {
        return async () =>
        {
          provider.OpenSafeContextBoundary();
          return await function();
        };
      }
    }
  }
}
