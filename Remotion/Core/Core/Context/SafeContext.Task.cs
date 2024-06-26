// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using SystemTask = System.Threading.Tasks.Task;

namespace Remotion.Context
{
  [SuppressMessage("Usage", "RMCORE0001:Use SafeContext instead of typical API")]
  public static partial class SafeContext
  {
    /// <summary>
    /// Provides methods for running <see cref="SystemTask"/>s which are <see cref="SafeContext"/> aware.
    /// <see cref="SafeContext"/> values do not flow into the executed tasks.
    /// </summary>
    public static partial class Task
    {
      /// <inheritdoc cref="M:Task_.Run(System.Action)" />
      public static SystemTask Run (Action action) => Run(Instance, action);

      /// <inheritdoc cref="M:Task_.Run(System.Action)" />
      public static SystemTask Run (ISafeContextStorageProvider provider, Action action) => SystemTask.Run(CreateWrapper(provider, action));

      /// <inheritdoc cref="M:Task_.Run(System.Action,System.Threading.CancellationToken)" />
      public static SystemTask Run (Action action, CancellationToken cancellationToken) => Run(Instance, action, cancellationToken);

      /// <inheritdoc cref="M:Task_.Run(System.Action,System.Threading.CancellationToken)" />
      public static SystemTask Run (ISafeContextStorageProvider provider, Action action, CancellationToken cancellationToken) =>
          SystemTask.Run(CreateWrapper(provider, action), cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run``1(System.Func{``0})" />
      public static Task<T> Run<T> (Func<T> function) => Run(Instance, function);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run``1(System.Func{``0})" />
      public static Task<T> Run<T> (ISafeContextStorageProvider provider, Func<T> function) => SystemTask.Run(CreateWrapper(provider, function));

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run``1(System.Func{``0},System.Threading.CancellationToken)" />
      public static Task<T> Run<T> (Func<T> function, CancellationToken cancellationToken) => Run(Instance, function, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run``1(System.Func{``0},System.Threading.CancellationToken)" />
      public static Task<T> Run<T> (ISafeContextStorageProvider provider, Func<T> function, CancellationToken cancellationToken) =>
          SystemTask.Run(CreateWrapper(provider, function), cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run(System.Func{System.Threading.Tasks.Task})" />
      public static SystemTask Run (Func<SystemTask> function) => Run(Instance, function);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run(System.Func{System.Threading.Tasks.Task})" />
      public static SystemTask Run (ISafeContextStorageProvider provider, Func<SystemTask> function) => SystemTask.Run(CreateWrapper(provider, function));

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run(System.Func{System.Threading.Tasks.Task},System.Threading.CancellationToken)" />
      public static SystemTask Run (Func<SystemTask> function, CancellationToken cancellationToken) => Run(Instance, function, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run(System.Func{System.Threading.Tasks.Task},System.Threading.CancellationToken)" />
      public static SystemTask Run (ISafeContextStorageProvider provider, Func<SystemTask> function, CancellationToken cancellationToken) =>
          SystemTask.Run(CreateWrapper(provider, function), cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run``1(System.Func{System.Threading.Tasks.Task{``0}})" />
      public static Task<T> Run<T> (Func<Task<T>> function) => Run(Instance, function);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run``1(System.Func{System.Threading.Tasks.Task{``0}})" />
      public static Task<T> Run<T> (ISafeContextStorageProvider provider, Func<Task<T>> function) => SystemTask.Run(CreateWrapper(provider, function));

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run``1(System.Func{System.Threading.Tasks.Task{``0}},System.Threading.CancellationToken)" />
      public static Task<T> Run<T> (Func<Task<T>> function, CancellationToken cancellationToken) => Run(Instance, function, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.Run``1(System.Func{System.Threading.Tasks.Task{``0}},System.Threading.CancellationToken)" />
      public static Task<T> Run<T> (ISafeContextStorageProvider provider, Func<Task<T>> function, CancellationToken cancellationToken) =>
          SystemTask.Run(CreateWrapper(provider, function), cancellationToken);

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

      private static Func<SystemTask> CreateWrapper (
          ISafeContextStorageProvider provider,
          Func<SystemTask> function)
      {
        return () =>
        {
          provider.OpenSafeContextBoundary();
          return function();
        };
      }

      private static Func<Task<T>> CreateWrapper<T> (
          ISafeContextStorageProvider provider,
          Func<Task<T>> function)
      {
        return () =>
        {
          provider.OpenSafeContextBoundary();
          return function();
        };
      }
    }
  }
}
