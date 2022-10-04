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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Utilities;
using static Remotion.UnitTests.Context.SafeContextTests.SafeContextTaskTest.ContinueWithTestFlags;

namespace Remotion.UnitTests.Context.SafeContextTests
{
  /// <remarks>
  /// The tests in this class are generated via <see cref="TestCaseSourceAttribute"/> from the SafeContext.Task.ContinueWith overloads (see <see cref="ContinueWithTestSource"/>).
  /// <see cref="RunContinueWithTest{TTask,TContinuation}"/> is the method that does the actual test execution.
  /// <see cref="ContinueWithTestFlags"/> is used to indicate which parameters are available for a certain overload and should be tested.
  /// See the descriptions for <see cref="ContinueWithTestSource"/> and <see cref="RunContinueWithTest{TTask,TContinuation}"/> for more information.
  /// </remarks>
  public partial class SafeContextTaskTest
  {
    [Flags]
    public enum ContinueWithTestFlags
    {
      None = 0,
      HasState = 1 << 0,
      HasCancellationToken = 1 << 1,
      HasTaskCompletionOptions = 1 << 2,
      HasTaskScheduler = 1 << 3
    }

    public class ContinueWithTestContext<TTask, TContinuation>
        where TTask : Task
        where TContinuation : Delegate
    {
      public TTask Task { get; }

      public TContinuation Continuation { get; }

      public object State { get; }

      public CancellationToken CancellationToken { get; }

      public TaskContinuationOptions Options { get; }

      public TaskScheduler TaskScheduler { get; }

      public ContinueWithTestContext (TTask task, TContinuation continuation, object state, CancellationToken cancellationToken, TaskContinuationOptions options, TaskScheduler taskScheduler)
      {
        Task = task;
        Continuation = continuation;
        State = state;
        CancellationToken = cancellationToken;
        Options = options;
        TaskScheduler = taskScheduler;
      }
    }

    private class ContinueWithTestTaskScheduler : TaskScheduler
    {
      public bool WasCalled { get; set; }

      protected override void QueueTask (Task task)
      {
        if ((task.CreationOptions & TaskCreationOptions.RunContinuationsAsynchronously) != 0)
        {
          WasCalled = true;
          ThreadPool.QueueUserWorkItem(_ => { TryExecuteTask(task); });
        }

        // Synchronous calls will call TryExecuteTaskInline afterwards
      }

      protected override bool TryExecuteTaskInline (Task task, bool taskWasPreviouslyQueued)
      {
        if ((task.CreationOptions & TaskCreationOptions.RunContinuationsAsynchronously) == 0)
        {
          WasCalled = true;
          return TryExecuteTask(task);
        }

        return false;
      }

      protected override IEnumerable<Task> GetScheduledTasks ()
      {
        throw new NotSupportedException();
      }
    }

    private static readonly MethodInfo s_createTestInvocationMethod = typeof(SafeContextTaskTest).GetMethod(nameof(CreateTestInvocation), BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo s_runContinueWithTestMethod = typeof(SafeContextTaskTest).GetMethod(nameof(RunContinueWithTest), BindingFlags.NonPublic | BindingFlags.Instance);

    [Test]
    [TestCaseSource(nameof(ContinueWithTestSource), new object[] { true })]
    public void ContinueWith_ImplicitSafeContextProvider (Type taskType, Type continuationType, Func<ISafeContextStorageProvider, object> continuationFactory, ContinueWithTestFlags flags)
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;
      using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
      {
        var runTestMethod = s_runContinueWithTestMethod.MakeGenericMethod(taskType, continuationType);
        var continuation = continuationFactory(null);
        InvokeWithInnerExceptionRethrow(runTestMethod, this, continuation, flags);
      }
    }

    [Test]
    [TestCaseSource(nameof(ContinueWithTestSource), new object[] { false })]
    public void ContinueWith_ExplicitSafeContextProvider (Type taskType, Type continuationType, Func<ISafeContextStorageProvider, object> continuationFactory, ContinueWithTestFlags flags)
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;
      var runTestMethod = s_runContinueWithTestMethod.MakeGenericMethod(taskType, continuationType);
      var continuation = continuationFactory(safeContextStorageProvider);
      InvokeWithInnerExceptionRethrow(runTestMethod, this, continuation, flags);
    }

    /// <summary>
    /// Converts the method overloads for <see cref="SafeContext"/>.<see cref="SafeContext.Task"/>.ContinueWith(...) to <see cref="TestCaseData"/>
    /// that can be used to invoke the tests through <see cref="RunContinueWithTest{TTask,TContinuation}"/>.
    /// </summary>
    /// <seealso cref="ContinueWith_ImplicitSafeContextProvider"/>
    /// <seealso cref="ContinueWith_ExplicitSafeContextProvider"/>
    private static IEnumerable<TestCaseData> ContinueWithTestSource (bool implicitSafeContextProvider)
    {
      var methods = typeof(SafeContext.Task).GetMethods(BindingFlags.Public | BindingFlags.Static)
          .Where(e => e.Name == "ContinueWith")
          .ToArray();

      foreach (var method in methods)
      {
        var parameters = method.GetParameters();
        var isImplicit = parameters[0].ParameterType != typeof(ISafeContextStorageProvider);
        if (isImplicit != implicitSafeContextProvider)
          continue;

        var taskType = ResolveGenericParameters(parameters[isImplicit ? 0 : 1].ParameterType);
        Assertion.IsTrue(typeof(Task).IsAssignableFrom(taskType), "typeof(Task).IsAssignableFrom(taskType)");
        Assertion.IsTrue(!taskType.ContainsGenericParameters, "!taskType.ContainsGenericParameters");

        var continuationType = ResolveGenericParameters(parameters[isImplicit ? 1 : 2].ParameterType);
        Assertion.IsTrue(typeof(Delegate).IsAssignableFrom(continuationType), "typeof(Delegate).IsAssignableFrom(continuationType)");
        Assertion.IsTrue(!continuationType.ContainsGenericParameters, "!continuationType.ContainsGenericParameters");

        var flags = None;
        if (parameters.Any(e => e.ParameterType == typeof(object)))
          flags |= HasState;
        if (parameters.Any(e => e.ParameterType == typeof(CancellationToken)))
          flags |= HasCancellationToken;
        if (parameters.Any(e => e.ParameterType == typeof(TaskContinuationOptions)))
          flags |= HasTaskCompletionOptions;
        if (parameters.Any(e => e.ParameterType == typeof(TaskScheduler)))
          flags |= HasTaskScheduler;

        var resolvedMethod = method;
        if (resolvedMethod.ContainsGenericParameters)
        {
          var array = method.GetGenericArguments().Select(_ => typeof(int)).ToArray();
          resolvedMethod = method.MakeGenericMethod(array);
        }

        var concreteCreateTestInvocation = s_createTestInvocationMethod.MakeGenericMethod(taskType, continuationType);
        var continuation = concreteCreateTestInvocation.Invoke(null, new object[] { resolvedMethod, flags });

        var testCaseData = new TestCaseData(taskType, continuationType, continuation, flags);
        testCaseData.TestName = $"{(isImplicit ? "Implicit" : "Explicit")}_{taskType.Name}_{continuationType.Name}_{flags}";

        yield return testCaseData;
      }
    }

    private static Type ResolveGenericParameters (Type target)
    {
      if (target.IsGenericParameter)
        return typeof(int);
      if (!target.IsGenericType)
        return target;

      var resolvedGenericArguments = target.GenericTypeArguments
          .Select(ResolveGenericParameters)
          .ToArray();

      return target.GetGenericTypeDefinition().MakeGenericType(resolvedGenericArguments);
    }

    private static Func<ISafeContextStorageProvider, object> CreateTestInvocation<TTask, TContinuation> (MethodInfo target, ContinueWithTestFlags flags)
        where TTask : Task
        where TContinuation : Delegate
    {
      return provider =>
      {
        return new Func<ContinueWithTestContext<TTask, TContinuation>, Task>(
            context =>
            {
              var arguments = new List<object>();
              if (provider != null)
                arguments.Add(provider);
              arguments.Add(context.Task);
              arguments.Add(context.Continuation);
              if ((flags & HasState) != 0)
                arguments.Add(context.State);
              if ((flags & HasCancellationToken) != 0)
                arguments.Add(context.CancellationToken);
              if ((flags & HasTaskCompletionOptions) != 0)
                arguments.Add(context.Options);
              if ((flags & HasTaskScheduler) != 0)
                arguments.Add(context.TaskScheduler);

              return (Task)InvokeWithInnerExceptionRethrow(target, null, arguments.ToArray());
            });
      };
    }

    /// <summary>
    /// This method is responsible for testing one overload of <see cref="SafeContext"/>.<see cref="SafeContext.Task"/>.ContinueWith(...).
    /// See remarks for more information on how the tests are done.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Each methods signature to test has different test settings:
    ///   <list type="number">
    ///     <item><description>Implicit or explicit SafeContextProvider</description></item>
    ///     <item><description>Task or Task&lt;T&gt; as source</description></item>
    ///     <item><description>The type of continuation passed (four options)</description></item>
    ///     <item><description>The type of arguments passed (five options)</description></item>
    ///   </list>
    /// </para>
    /// <para>
    ///   A <paramref cref="executor"/> is specified which calls the overload that should be tested.
    ///   The <paramref name="flags"/> indicate what aspects should be verified.
    /// </para>
    /// </remarks>
    private void RunContinueWithTest<TTask, TContinuation> (Func<ContinueWithTestContext<TTask, TContinuation>, Task> executor, ContinueWithTestFlags flags)
        where TTask : Task
        where TContinuation : Delegate
    {
      var state = new object();
      var cancellationTokenSource = new CancellationTokenSource();
      var cancellationToken = cancellationTokenSource.Token;
      var taskContinuationOptions = TaskContinuationOptions.RunContinuationsAsynchronously;
      var taskScheduler = new ContinueWithTestTaskScheduler();

      var threadId = Thread.CurrentThread.ManagedThreadId;

      var continuationCalled = false;
      var completedTask = GetCompletedTask<TTask>();
      Action<Task, object> innerContinuation = (task, arg) =>
      {
        continuationCalled = true;

        Assert.That(task, Is.SameAs(completedTask), "Task was not passed correctly.");
        Assert.That(arg, (flags & HasState) != 0 ? Is.SameAs(state) : Is.Null, "State was not passed correctly.");
        if ((flags & HasTaskCompletionOptions) != 0)
          Assert.That(Thread.CurrentThread.ManagedThreadId, Is.Not.EqualTo(threadId), "TaskContinuationOptions were not passed correctly.");

        SafeContextProviderMock.Verify(e => e.OpenSafeContextBoundary(), "No safe context boundary was opened.");

        cancellationTokenSource.Cancel();
        cancellationToken.ThrowIfCancellationRequested();
      };

      var continuation = CreateContinuation<TContinuation>(innerContinuation);

      var context = new ContinueWithTestContext<TTask, TContinuation>(
          completedTask,
          continuation,
          state,
          cancellationTokenSource.Token,
          taskContinuationOptions,
          taskScheduler);

      Task resultingTask = null;
      try
      {
        resultingTask = executor(context);
        resultingTask.GetAwaiter().GetResult();
      }
      catch (OperationCanceledException ex)
      {
        Assert.That(resultingTask, Is.Not.Null);
        Assert.That(ex.CancellationToken, Is.EqualTo(cancellationToken));
        Assert.That(resultingTask.Status, Is.EqualTo((flags & HasCancellationToken) != 0 ? TaskStatus.Canceled : TaskStatus.Faulted), "CancellationToken was not passed correctly.");
      }

      Assert.That(continuationCalled, Is.True, "Continuation was not called.");
      Assert.That(taskScheduler.WasCalled, Is.EqualTo((flags & HasTaskScheduler) != 0));
    }

    private static TTask GetCompletedTask<TTask> ()
        where TTask : Task
    {
      if (typeof(TTask) == typeof(Task))
        return (TTask)Task.CompletedTask;

      Debug.Assert(typeof(TTask) == typeof(Task<int>));
      return (TTask)(object)Task.FromResult(1);
    }

    private static TContinuation CreateContinuation<TContinuation> (Action<Task, object> innerContinuation)
        where TContinuation : Delegate
    {
      if (typeof(TContinuation) == typeof(Action<Task>))
      {
        return (TContinuation)(object)new Action<Task>(t => innerContinuation(t, null));
      }
      else if (typeof(TContinuation) == typeof(Action<Task, object>))
      {
        return (TContinuation)(object)new Action<Task, object>(innerContinuation);
      }
      else if (typeof(TContinuation) == typeof(Func<Task, int>))
      {
        return (TContinuation)(object)new Func<Task, int>(
            t =>
            {
              innerContinuation(t, null);
              return default;
            });
      }
      else if (typeof(TContinuation) == typeof(Func<Task, object, int>))
      {
        return (TContinuation)(object)new Func<Task, object, int>(
            (t, state) =>
            {
              innerContinuation(t, state);
              return default;
            });
      }
      else if (typeof(TContinuation) == typeof(Action<Task<int>>))
      {
        return (TContinuation)(object)new Action<Task<int>>(t => innerContinuation(t, null));
      }
      else if (typeof(TContinuation) == typeof(Action<Task<int>, object>))
      {
        return (TContinuation)(object)new Action<Task<int>, object>(innerContinuation);
      }
      else if (typeof(TContinuation) == typeof(Func<Task<int>, int>))
      {
        return (TContinuation)(object)new Func<Task<int>, int>(
          t =>
          {
            innerContinuation(t, null);
            return default;
          });
      }
      else if (typeof(TContinuation) == typeof(Func<Task<int>, object, int>))
      {
        return (TContinuation)(object)new Func<Task<int>, object, int>(
            (t, state) =>
            {
              innerContinuation(t, state);
              return default;
            });
      }
      else
      {
        throw new InvalidOperationException($"Unsupported type of continuation '{typeof(TContinuation)}'.");
      }
    }

    private static object InvokeWithInnerExceptionRethrow (MethodInfo method, object instance, params object[] arguments)
    {
      try
      {
        return method.Invoke(instance, arguments);
      }
      catch (TargetInvocationException ex)
      {
        if (ex.InnerException != null)
          ExceptionDispatchInfo.Capture(ex.InnerException).Throw();

        throw;
      }
    }
  }
}
