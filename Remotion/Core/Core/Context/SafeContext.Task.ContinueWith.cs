// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Threading;
using System.Threading.Tasks;
using SystemTask = System.Threading.Tasks.Task;

namespace Remotion.Context
{
  public static partial class SafeContext
  {
    public static partial class Task
    {
      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task})" />
      public static SystemTask ContinueWith (SystemTask task, Action<SystemTask> continuationAction) => ContinueWith(Instance, task, continuationAction);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task})" />
      public static SystemTask ContinueWith (ISafeContextStorageProvider provider, SystemTask task, Action<SystemTask> continuationAction) =>
          task.ContinueWith(CreateWrapper(provider, continuationAction));

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task},System.Threading.CancellationToken)" />
      public static SystemTask ContinueWith (SystemTask task, Action<SystemTask> continuationAction, CancellationToken cancellationToken) =>
          ContinueWith(Instance, task, continuationAction, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task},System.Threading.CancellationToken)" />
      public static SystemTask ContinueWith (ISafeContextStorageProvider provider, SystemTask task, Action<SystemTask> continuationAction, CancellationToken cancellationToken) =>
          task.ContinueWith(CreateWrapper(provider, continuationAction), cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task},System.Threading.Tasks.TaskScheduler)" />
      public static SystemTask ContinueWith (SystemTask task, Action<SystemTask> continuationAction, TaskScheduler taskScheduler) =>
          ContinueWith(Instance, task, continuationAction, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task},System.Threading.Tasks.TaskScheduler)" />
      public static SystemTask ContinueWith (ISafeContextStorageProvider provider, SystemTask task, Action<SystemTask> continuationAction, TaskScheduler taskScheduler) =>
          task.ContinueWith(CreateWrapper(provider, continuationAction), taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task},System.Threading.Tasks.TaskContinuationOptions)" />
      public static SystemTask ContinueWith (SystemTask task, Action<SystemTask> continuationAction, TaskContinuationOptions taskContinuationOptions) =>
          ContinueWith(Instance, task, continuationAction, taskContinuationOptions);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task},System.Threading.Tasks.TaskContinuationOptions)" />
      public static SystemTask ContinueWith (
          ISafeContextStorageProvider provider,
          SystemTask task,
          Action<SystemTask> continuationAction,
          TaskContinuationOptions taskContinuationOptions) => task.ContinueWith(CreateWrapper(provider, continuationAction), taskContinuationOptions);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task},System.Threading.CancellationToken,System.Threading.Tasks.TaskContinuationOptions,System.Threading.Tasks.TaskScheduler)" />
      public static SystemTask ContinueWith (
          SystemTask task,
          Action<SystemTask> continuationAction,
          CancellationToken cancellationToken,
          TaskContinuationOptions taskContinuationOptions,
          TaskScheduler taskScheduler) => ContinueWith(Instance, task, continuationAction, cancellationToken, taskContinuationOptions, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task},System.Threading.CancellationToken,System.Threading.Tasks.TaskContinuationOptions,System.Threading.Tasks.TaskScheduler)" />
      public static SystemTask ContinueWith (
          ISafeContextStorageProvider provider,
          SystemTask task,
          Action<SystemTask> continuationAction,
          CancellationToken cancellationToken,
          TaskContinuationOptions taskContinuationOptions,
          TaskScheduler taskScheduler) => task.ContinueWith(CreateWrapper(provider, continuationAction), cancellationToken, taskContinuationOptions, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task,System.Object},System.Object)" />
      public static SystemTask ContinueWith (SystemTask task, Action<SystemTask, object?> continuationAction, object? arg) =>
          ContinueWith(Instance, task, continuationAction, arg);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task,System.Object},System.Object)" />
      public static SystemTask ContinueWith (ISafeContextStorageProvider provider, SystemTask task, Action<SystemTask, object?> continuationAction, object? arg) =>
          task.ContinueWith(CreateWrapper(provider, continuationAction), arg);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task,System.Object},System.Object,System.Threading.CancellationToken)" />
      public static SystemTask ContinueWith (SystemTask task, Action<SystemTask, object?> continuationAction, object? arg, CancellationToken cancellationToken) =>
          ContinueWith(Instance, task, continuationAction, arg, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task,System.Object},System.Object,System.Threading.CancellationToken)" />
      public static SystemTask ContinueWith (
          ISafeContextStorageProvider provider,
          SystemTask task,
          Action<SystemTask, object?> continuationAction,
          object? arg,
          CancellationToken cancellationToken) => task.ContinueWith(CreateWrapper(provider, continuationAction), arg, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task,System.Object},System.Object,System.Threading.Tasks.TaskScheduler)" />
      public static SystemTask ContinueWith (SystemTask task, Action<SystemTask, object?> continuationAction, object? arg, TaskScheduler taskScheduler) =>
          ContinueWith(Instance, task, continuationAction, arg, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task,System.Object},System.Object,System.Threading.Tasks.TaskScheduler)" />
      public static SystemTask ContinueWith (
          ISafeContextStorageProvider provider,
          SystemTask task,
          Action<SystemTask, object?> continuationAction,
          object? arg,
          TaskScheduler taskScheduler) => task.ContinueWith(CreateWrapper(provider, continuationAction), arg, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task,System.Object},System.Object,System.Threading.Tasks.TaskContinuationOptions)" />
      public static SystemTask ContinueWith (SystemTask task, Action<SystemTask, object?> continuationAction, object? arg, TaskContinuationOptions taskContinuationOptions) =>
          ContinueWith(Instance, task, continuationAction, arg, taskContinuationOptions);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task,System.Object},System.Object,System.Threading.Tasks.TaskContinuationOptions)" />
      public static SystemTask ContinueWith (
          ISafeContextStorageProvider provider,
          SystemTask task,
          Action<SystemTask, object?> continuationAction,
          object? arg,
          TaskContinuationOptions taskContinuationOptions) => task.ContinueWith(CreateWrapper(provider, continuationAction), arg, taskContinuationOptions);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task,System.Object},System.Object,System.Threading.CancellationToken,System.Threading.Tasks.TaskContinuationOptions,System.Threading.Tasks.TaskScheduler)" />
      public static SystemTask ContinueWith (
          SystemTask task,
          Action<SystemTask, object?> continuationAction,
          object? arg,
          CancellationToken cancellationToken,
          TaskContinuationOptions taskContinuationOptions,
          TaskScheduler taskScheduler) => ContinueWith(Instance, task, continuationAction, arg, cancellationToken, taskContinuationOptions, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith(System.Action{System.Threading.Tasks.Task,System.Object},System.Object,System.Threading.CancellationToken,System.Threading.Tasks.TaskContinuationOptions,System.Threading.Tasks.TaskScheduler)" />
      public static SystemTask ContinueWith (
          ISafeContextStorageProvider provider,
          SystemTask task,
          Action<SystemTask, object?> continuationAction,
          object? arg,
          CancellationToken cancellationToken,
          TaskContinuationOptions taskContinuationOptions,
          TaskScheduler taskScheduler) => task.ContinueWith(CreateWrapper(provider, continuationAction), arg, cancellationToken, taskContinuationOptions, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,``0})" />
      public static Task<TResult> ContinueWith<TResult> (SystemTask task, Func<SystemTask, TResult> continuationFunction) => ContinueWith(Instance, task, continuationFunction);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,``0})" />
      public static Task<TResult> ContinueWith<TResult> (ISafeContextStorageProvider provider, SystemTask task, Func<SystemTask, TResult> continuationFunction) =>
          task.ContinueWith(CreateWrapper(provider, continuationFunction));

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,``0},System.Threading.CancellationToken)" />
      public static Task<TResult> ContinueWith<TResult> (SystemTask task, Func<SystemTask, TResult> continuationFunction, CancellationToken cancellationToken) =>
          ContinueWith(Instance, task, continuationFunction, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,``0},System.Threading.CancellationToken)" />
      public static Task<TResult> ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          SystemTask task,
          Func<SystemTask, TResult> continuationFunction,
          CancellationToken cancellationToken) => task.ContinueWith(CreateWrapper(provider, continuationFunction), cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,``0},System.Threading.Tasks.TaskScheduler)" />
      public static Task<TResult> ContinueWith<TResult> (SystemTask task, Func<SystemTask, TResult> continuationFunction, TaskScheduler taskScheduler) =>
          ContinueWith(Instance, task, continuationFunction, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,``0},System.Threading.Tasks.TaskScheduler)" />
      public static Task<TResult> ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          SystemTask task,
          Func<SystemTask, TResult> continuationFunction,
          TaskScheduler taskScheduler) => task.ContinueWith(CreateWrapper(provider, continuationFunction), taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,``0},System.Threading.Tasks.TaskContinuationOptions)" />
      public static Task<TResult> ContinueWith<TResult> (SystemTask task, Func<SystemTask, TResult> continuationFunction, TaskContinuationOptions taskContinuationOptions) =>
          ContinueWith(Instance, task, continuationFunction, taskContinuationOptions);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,``0},System.Threading.Tasks.TaskContinuationOptions)" />
      public static Task<TResult> ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          SystemTask task,
          Func<SystemTask, TResult> continuationFunction,
          TaskContinuationOptions taskContinuationOptions) => task.ContinueWith(CreateWrapper(provider, continuationFunction), taskContinuationOptions);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,``0},System.Threading.CancellationToken,System.Threading.Tasks.TaskContinuationOptions,System.Threading.Tasks.TaskScheduler)" />
      public static Task<TResult> ContinueWith<TResult> (
          SystemTask task,
          Func<SystemTask, TResult> continuationFunction,
          CancellationToken cancellationToken,
          TaskContinuationOptions taskContinuationOptions,
          TaskScheduler taskScheduler) => ContinueWith(Instance, task, continuationFunction, cancellationToken, taskContinuationOptions, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,``0},System.Threading.CancellationToken,System.Threading.Tasks.TaskContinuationOptions,System.Threading.Tasks.TaskScheduler)" />
      public static Task<TResult> ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          SystemTask task,
          Func<SystemTask, TResult> continuationFunction,
          CancellationToken cancellationToken,
          TaskContinuationOptions taskContinuationOptions,
          TaskScheduler taskScheduler) => task.ContinueWith(CreateWrapper(provider, continuationFunction), cancellationToken, taskContinuationOptions, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,System.Object,``0},System.Object)" />
      public static Task<TResult> ContinueWith<TResult> (SystemTask task, Func<SystemTask, object?, TResult> continuationFunction, object? arg) =>
          ContinueWith(Instance, task, continuationFunction, arg);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,System.Object,``0},System.Object)" />
      public static Task<TResult> ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          SystemTask task,
          Func<SystemTask, object?, TResult> continuationFunction,
          object? arg) => task.ContinueWith(CreateWrapper(provider, continuationFunction), arg);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,System.Object,``0},System.Object,System.Threading.CancellationToken)" />
      public static Task<TResult> ContinueWith<TResult> (
          SystemTask task,
          Func<SystemTask, object?, TResult> continuationFunction,
          object? arg,
          CancellationToken cancellationToken) =>
          ContinueWith(Instance, task, continuationFunction, arg, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,System.Object,``0},System.Object,System.Threading.CancellationToken)" />
      public static Task<TResult> ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          SystemTask task,
          Func<SystemTask, object?, TResult> continuationFunction,
          object? arg,
          CancellationToken cancellationToken) => task.ContinueWith(CreateWrapper(provider, continuationFunction), arg, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,System.Object,``0},System.Object,System.Threading.Tasks.TaskScheduler)" />
      public static Task<TResult> ContinueWith<TResult> (SystemTask task, Func<SystemTask, object?, TResult> continuationFunction, object? arg, TaskScheduler taskScheduler) =>
          ContinueWith(Instance, task, continuationFunction, arg, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,System.Object,``0},System.Object,System.Threading.Tasks.TaskScheduler)" />
      public static Task<TResult> ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          SystemTask task,
          Func<SystemTask, object?, TResult> continuationFunction,
          object? arg,
          TaskScheduler taskScheduler) => task.ContinueWith(CreateWrapper(provider, continuationFunction), arg, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,System.Object,``0},System.Object,System.Threading.Tasks.TaskContinuationOptions)" />
      public static Task<TResult> ContinueWith<TResult> (
          SystemTask task,
          Func<SystemTask, object?, TResult> continuationFunction,
          object? arg,
          TaskContinuationOptions taskContinuationOptions) => ContinueWith(Instance, task, continuationFunction, arg, taskContinuationOptions);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,System.Object,``0},System.Object,System.Threading.Tasks.TaskContinuationOptions)" />
      public static Task<TResult> ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          SystemTask task,
          Func<SystemTask, object?, TResult> continuationFunction,
          object? arg,
          TaskContinuationOptions taskContinuationOptions) => task.ContinueWith(CreateWrapper(provider, continuationFunction), arg, taskContinuationOptions);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,System.Object,``0},System.Object,System.Threading.CancellationToken,System.Threading.Tasks.TaskContinuationOptions,System.Threading.Tasks.TaskScheduler)" />
      public static Task<TResult> ContinueWith<TResult> (
          SystemTask task,
          Func<SystemTask, object?, TResult> continuationFunction,
          object? arg,
          CancellationToken cancellationToken,
          TaskContinuationOptions taskContinuationOptions,
          TaskScheduler taskScheduler) => ContinueWith(Instance, task, continuationFunction, arg, cancellationToken, taskContinuationOptions, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task.ContinueWith``1(System.Func{System.Threading.Tasks.Task,System.Object,``0},System.Object,System.Threading.CancellationToken,System.Threading.Tasks.TaskContinuationOptions,System.Threading.Tasks.TaskScheduler)" />
      public static Task<TResult> ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          SystemTask task,
          Func<SystemTask, object?, TResult> continuationFunction,
          object? arg,
          CancellationToken cancellationToken,
          TaskContinuationOptions taskContinuationOptions,
          TaskScheduler taskScheduler) => task.ContinueWith(CreateWrapper(provider, continuationFunction), arg, cancellationToken, taskContinuationOptions, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0}}})" />
      public static SystemTask ContinueWith<TResult> (Task<TResult> task, Action<Task<TResult>> continuationAction) => ContinueWith(Instance, task, continuationAction);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0}}})" />
      public static SystemTask ContinueWith<TResult> (ISafeContextStorageProvider provider, Task<TResult> task, Action<Task<TResult>> continuationAction) =>
          task.ContinueWith(CreateWrapper(provider, continuationAction));

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0}}},System.Threading.CancellationToken)" />
      public static SystemTask ContinueWith<TResult> (Task<TResult> task, Action<Task<TResult>> continuationAction, CancellationToken cancellationToken) =>
          ContinueWith(Instance, task, continuationAction, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0}}},System.Threading.CancellationToken)" />
      public static SystemTask ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Action<Task<TResult>> continuationAction,
          CancellationToken cancellationToken) => task.ContinueWith(CreateWrapper(provider, continuationAction), cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0}}},System.Threading.Tasks.TaskScheduler)" />
      public static SystemTask ContinueWith<TResult> (Task<TResult> task, Action<Task<TResult>> continuationAction, TaskScheduler taskScheduler) =>
          ContinueWith(Instance, task, continuationAction, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0}}},System.Threading.Tasks.TaskScheduler)" />
      public static SystemTask ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Action<Task<TResult>> continuationAction,
          TaskScheduler taskScheduler) => task.ContinueWith(CreateWrapper(provider, continuationAction), taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0}}},System.Threading.Tasks.TaskContinuationOptions)" />
      public static SystemTask ContinueWith<TResult> (Task<TResult> task, Action<Task<TResult>> continuationAction, TaskContinuationOptions taskContinuationOptions) =>
          ContinueWith(Instance, task, continuationAction, taskContinuationOptions);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0}}},System.Threading.Tasks.TaskContinuationOptions)" />
      public static SystemTask ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Action<Task<TResult>> continuationAction,
          TaskContinuationOptions taskContinuationOptions) => task.ContinueWith(CreateWrapper(provider, continuationAction), taskContinuationOptions);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0}}},System.Threading.CancellationToken,System.Threading.Tasks.TaskContinuationOptions,System.Threading.Tasks.TaskScheduler)" />
      public static SystemTask ContinueWith<TResult> (
          Task<TResult> task,
          Action<Task<TResult>> continuationAction,
          CancellationToken cancellationToken,
          TaskContinuationOptions taskContinuationOptions,
          TaskScheduler taskScheduler) => ContinueWith(Instance, task, continuationAction, cancellationToken, taskContinuationOptions, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0}}},System.Threading.CancellationToken,System.Threading.Tasks.TaskContinuationOptions,System.Threading.Tasks.TaskScheduler)" />
      public static SystemTask ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Action<Task<TResult>> continuationAction,
          CancellationToken cancellationToken,
          TaskContinuationOptions taskContinuationOptions,
          TaskScheduler taskScheduler) => task.ContinueWith(CreateWrapper(provider, continuationAction), cancellationToken, taskContinuationOptions, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0},System.Object},System.Object)" />
      public static SystemTask ContinueWith<TResult> (Task<TResult> task, Action<Task<TResult>, object?> continuationAction, object? arg) =>
          ContinueWith(Instance, task, continuationAction, arg);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0},System.Object},System.Object)" />
      public static SystemTask ContinueWith<TResult> (ISafeContextStorageProvider provider, Task<TResult> task, Action<Task<TResult>, object?> continuationAction, object? arg) =>
          task.ContinueWith(CreateWrapper(provider, continuationAction), arg);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0},System.Object},System.Object,System.Threading.CancellationToken)" />
      public static SystemTask ContinueWith<TResult> (Task<TResult> task, Action<Task<TResult>, object?> continuationAction, object? arg, CancellationToken cancellationToken) =>
          ContinueWith(Instance, task, continuationAction, arg, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0},System.Object},System.Object,System.Threading.CancellationToken)" />
      public static SystemTask ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Action<Task<TResult>, object?> continuationAction,
          object? arg,
          CancellationToken cancellationToken) => task.ContinueWith(CreateWrapper(provider, continuationAction), arg, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0},System.Object},System.Object,System.Threading.Tasks.TaskScheduler)" />
      public static SystemTask ContinueWith<TResult> (Task<TResult> task, Action<Task<TResult>, object?> continuationAction, object? arg, TaskScheduler taskScheduler) =>
          ContinueWith(Instance, task, continuationAction, arg, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0},System.Object},System.Object,System.Threading.Tasks.TaskScheduler)" />
      public static SystemTask ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Action<Task<TResult>, object?> continuationAction,
          object? arg,
          TaskScheduler taskScheduler) => task.ContinueWith(CreateWrapper(provider, continuationAction), arg, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0},System.Object},System.Object,System.Threading.Tasks.TaskContinuationOptions)" />
      public static SystemTask ContinueWith<TResult> (
          Task<TResult> task,
          Action<Task<TResult>, object?> continuationAction,
          object? arg,
          TaskContinuationOptions taskContinuationOptions) => ContinueWith(Instance, task, continuationAction, arg, taskContinuationOptions);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0},System.Object},System.Object,System.Threading.Tasks.TaskContinuationOptions)" />
      public static SystemTask ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Action<Task<TResult>, object?> continuationAction,
          object? arg,
          TaskContinuationOptions taskContinuationOptions) => task.ContinueWith(CreateWrapper(provider, continuationAction), arg, taskContinuationOptions);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0},System.Object},System.Object,System.Threading.CancellationToken,System.Threading.Tasks.TaskContinuationOptions,System.Threading.Tasks.TaskScheduler)" />
      public static SystemTask ContinueWith<TResult> (
          Task<TResult> task,
          Action<Task<TResult>, object?> continuationAction,
          object? arg,
          CancellationToken cancellationToken,
          TaskContinuationOptions taskContinuationOptions,
          TaskScheduler taskScheduler) => ContinueWith(Instance, task, continuationAction, arg, cancellationToken, taskContinuationOptions, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task{`0},System.Object},System.Object,System.Threading.CancellationToken,System.Threading.Tasks.TaskContinuationOptions,System.Threading.Tasks.TaskScheduler)" />
      public static SystemTask ContinueWith<TResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Action<Task<TResult>, object?> continuationAction,
          object? arg,
          CancellationToken cancellationToken,
          TaskContinuationOptions taskContinuationOptions,
          TaskScheduler taskScheduler) => task.ContinueWith(CreateWrapper(provider, continuationAction), arg, cancellationToken, taskContinuationOptions, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},``0})" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (Task<TResult> task, Func<Task<TResult>, TNewResult> continuationFunction) =>
          ContinueWith(Instance, task, continuationFunction);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},``0})" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Func<Task<TResult>, TNewResult> continuationFunction) => task.ContinueWith(CreateWrapper(provider, continuationFunction));

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},``0},System.Threading.CancellationToken)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          Task<TResult> task,
          Func<Task<TResult>, TNewResult> continuationFunction,
          CancellationToken cancellationToken) => ContinueWith(Instance, task, continuationFunction, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},``0},System.Threading.CancellationToken)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Func<Task<TResult>, TNewResult> continuationFunction,
          CancellationToken cancellationToken) => task.ContinueWith(CreateWrapper(provider, continuationFunction), cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},``0},System.Threading.Tasks.TaskScheduler)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (Task<TResult> task, Func<Task<TResult>, TNewResult> continuationFunction, TaskScheduler taskScheduler) =>
          ContinueWith(Instance, task, continuationFunction, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},``0},System.Threading.Tasks.TaskScheduler)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Func<Task<TResult>, TNewResult> continuationFunction,
          TaskScheduler taskScheduler) => task.ContinueWith(CreateWrapper(provider, continuationFunction), taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},``0},System.Threading.Tasks.TaskContinuationOptions)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          Task<TResult> task,
          Func<Task<TResult>, TNewResult> continuationFunction,
          TaskContinuationOptions taskContinuationOptions) => ContinueWith(Instance, task, continuationFunction, taskContinuationOptions);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},``0},System.Threading.Tasks.TaskContinuationOptions)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Func<Task<TResult>, TNewResult> continuationFunction,
          TaskContinuationOptions taskContinuationOptions) => task.ContinueWith(CreateWrapper(provider, continuationFunction), taskContinuationOptions);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},``0},System.Threading.CancellationToken,System.Threading.Tasks.TaskContinuationOptions,System.Threading.Tasks.TaskScheduler)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          Task<TResult> task,
          Func<Task<TResult>, TNewResult> continuationFunction,
          CancellationToken cancellationToken,
          TaskContinuationOptions taskContinuationOptions,
          TaskScheduler taskScheduler) => ContinueWith(Instance, task, continuationFunction, cancellationToken, taskContinuationOptions, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},``0},System.Threading.CancellationToken,System.Threading.Tasks.TaskContinuationOptions,System.Threading.Tasks.TaskScheduler)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Func<Task<TResult>, TNewResult> continuationFunction,
          CancellationToken cancellationToken,
          TaskContinuationOptions taskContinuationOptions,
          TaskScheduler taskScheduler) => task.ContinueWith(CreateWrapper(provider, continuationFunction), cancellationToken, taskContinuationOptions, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},System.Object,``0},System.Object)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (Task<TResult> task, Func<Task<TResult>, object?, TNewResult> continuationFunction, object? arg) =>
          ContinueWith(Instance, task, continuationFunction, arg);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},System.Object,``0},System.Object)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Func<Task<TResult>, object?, TNewResult> continuationFunction,
          object? arg) => task.ContinueWith(CreateWrapper(provider, continuationFunction), arg);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},System.Object,``0},System.Object,System.Threading.CancellationToken)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          Task<TResult> task,
          Func<Task<TResult>, object?, TNewResult> continuationFunction,
          object? arg,
          CancellationToken cancellationToken) => ContinueWith(Instance, task, continuationFunction, arg, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},System.Object,``0},System.Object,System.Threading.CancellationToken)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Func<Task<TResult>, object?, TNewResult> continuationFunction,
          object? arg,
          CancellationToken cancellationToken) => task.ContinueWith(CreateWrapper(provider, continuationFunction), arg, cancellationToken);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},System.Object,``0},System.Object,System.Threading.Tasks.TaskScheduler)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          Task<TResult> task,
          Func<Task<TResult>, object?, TNewResult> continuationFunction,
          object? arg,
          TaskScheduler taskScheduler) => ContinueWith(Instance, task, continuationFunction, arg, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},System.Object,``0},System.Object,System.Threading.Tasks.TaskScheduler)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Func<Task<TResult>, object?, TNewResult> continuationFunction,
          object? arg,
          TaskScheduler taskScheduler) => task.ContinueWith(CreateWrapper(provider, continuationFunction), arg, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},System.Object,``0},System.Object,System.Threading.Tasks.TaskContinuationOptions)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          Task<TResult> task,
          Func<Task<TResult>, object?, TNewResult> continuationFunction,
          object? arg,
          TaskContinuationOptions taskContinuationOptions) => ContinueWith(Instance, task, continuationFunction, arg, taskContinuationOptions);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},System.Object,``0},System.Object,System.Threading.Tasks.TaskContinuationOptions)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Func<Task<TResult>, object?, TNewResult> continuationFunction,
          object? arg,
          TaskContinuationOptions taskContinuationOptions) => task.ContinueWith(CreateWrapper(provider, continuationFunction), arg, taskContinuationOptions);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},System.Object,``0},System.Object,System.Threading.CancellationToken,System.Threading.Tasks.TaskContinuationOptions,System.Threading.Tasks.TaskScheduler)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          Task<TResult> task,
          Func<Task<TResult>, object?, TNewResult> continuationFunction,
          object? arg,
          CancellationToken cancellationToken,
          TaskContinuationOptions taskContinuationOptions,
          TaskScheduler taskScheduler) => ContinueWith(Instance, task, continuationFunction, arg, cancellationToken, taskContinuationOptions, taskScheduler);

      /// <inheritdoc cref="M:System.Threading.Tasks.Task`1.ContinueWith``1(System.Func{System.Threading.Tasks.Task{`0},System.Object,``0},System.Object,System.Threading.CancellationToken,System.Threading.Tasks.TaskContinuationOptions,System.Threading.Tasks.TaskScheduler)" />
      public static Task<TNewResult> ContinueWith<TResult, TNewResult> (
          ISafeContextStorageProvider provider,
          Task<TResult> task,
          Func<Task<TResult>, object?, TNewResult> continuationFunction,
          object? arg,
          CancellationToken cancellationToken,
          TaskContinuationOptions taskContinuationOptions,
          TaskScheduler taskScheduler) => task.ContinueWith(CreateWrapper(provider, continuationFunction), arg, cancellationToken, taskContinuationOptions, taskScheduler);

      private static Action<SystemTask> CreateWrapper (ISafeContextStorageProvider provider, Action<SystemTask> action)
      {
        return task =>
        {
          provider.OpenSafeContextBoundary();
          action(task);
        };
      }

      private static Action<Task<TResult>> CreateWrapper<TResult> (ISafeContextStorageProvider provider, Action<Task<TResult>> action)
      {
        return task =>
        {
          provider.OpenSafeContextBoundary();
          action(task);
        };
      }

      private static Action<SystemTask, object?> CreateWrapper (ISafeContextStorageProvider provider, Action<SystemTask, object?> action)
      {
        return (task, arg) =>
        {
          provider.OpenSafeContextBoundary();
          action(task, arg);
        };
      }

      private static Action<Task<TResult>, object?> CreateWrapper<TResult> (ISafeContextStorageProvider provider, Action<Task<TResult>, object?> action)
      {
        return (task, arg) =>
        {
          provider.OpenSafeContextBoundary();
          action(task, arg);
        };
      }

      private static Func<SystemTask, TResult> CreateWrapper<TResult> (ISafeContextStorageProvider provider, Func<SystemTask, TResult> func)
      {
        return task =>
        {
          provider.OpenSafeContextBoundary();
          return func(task);
        };
      }

      private static Func<Task<TResult>, TNewResult> CreateWrapper<TResult, TNewResult> (ISafeContextStorageProvider provider, Func<Task<TResult>, TNewResult> func)
      {
        return task =>
        {
          provider.OpenSafeContextBoundary();
          return func(task);
        };
      }

      private static Func<SystemTask, object?, TResult> CreateWrapper<TResult> (ISafeContextStorageProvider provider, Func<SystemTask, object?, TResult> func)
      {
        return (task, arg) =>
        {
          provider.OpenSafeContextBoundary();
          return func(task, arg);
        };
      }

      private static Func<Task<TResult>, object?, TNewResult> CreateWrapper<TResult, TNewResult> (
          ISafeContextStorageProvider provider,
          Func<Task<TResult>, object?, TNewResult> func)
      {
        return (task, arg) =>
        {
          provider.OpenSafeContextBoundary();
          return func(task, arg);
        };
      }
    }
  }
}
