using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Remotion.Context;

namespace Remotion.UnitTests.Context
{
  public partial class SafeContextTest
  {
    [TestFixture]
    public class TaskTest
    {
      private Mock<ISafeContextStorageProvider> _safeContextProviderMock;

      [SetUp]
      public void SetUp ()
      {
        ResetSafeContextStorageProvider();

        _safeContextProviderMock = new Mock<ISafeContextStorageProvider>(MockBehavior.Strict);
        _safeContextProviderMock.Setup(e => e.OpenSafeContextBoundary()).Returns((SafeContextBoundary)default);
      }

      [TearDown]
      public void TearDown ()
      {
        ResetSafeContextStorageProvider();
      }

      [Test]
      public void Assumption_TaskWithRegisteredCancellationToken_MarksTaskCancelled ()
      {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var task = Task.Run(() => cts.Token.ThrowIfCancellationRequested(), cts.Token);
        Task.WaitAny(task);

        Assert.That(task.Status, Is.EqualTo(TaskStatus.Canceled));
      }

      [Test]
      public void Assumption_AsyncTaskWithRegisteredCancellationToken_MarksTaskCancelled ()
      {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var task = Task.Run(() => cts.Token.ThrowIfCancellationRequested(), cts.Token);
        Task.WaitAny(task);

        Assert.That(task.Status, Is.EqualTo(TaskStatus.Canceled));
      }

      [Test]
      public void Assumption_TaskWithUnregisteredCancellationToken_DoesNotMarkTaskCancelled ()
      {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // ReSharper disable once MethodSupportsCancellation
        var task = Task.Run(() => cts.Token.ThrowIfCancellationRequested());
        Task.WaitAny(task);

        Assert.That(task.Status, Is.EqualTo(TaskStatus.Faulted));
      }

      [Test]
      public void Run_ActionWithoutCancellationAndImplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
        {
          // ReSharper disable once MethodSupportsCancellation
          RunTestAction((action, _) => SafeContext.Task.Run(action), false);
        }
      }

      [Test]
      public void Run_ActionWithoutCancellationAndExplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        // ReSharper disable once MethodSupportsCancellation
        RunTestAction((action, _) => SafeContext.Task.Run(safeContextStorageProvider, action), false);
      }

      [Test]
      public void Run_ActionWithCancellationAndImplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
          RunTestAction(SafeContext.Task.Run, true);
      }

      [Test]
      public void Run_ActionWithCancellationAndExplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        RunTestAction((action, cancellationToken) => SafeContext.Task.Run(safeContextStorageProvider, action, cancellationToken), true);
      }

      [Test]
      public void Run_FuncTWithoutCancellationAndImplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
        {
          // ReSharper disable once MethodSupportsCancellation
          RunTestFuncT((action, _) => SafeContext.Task.Run(action), false);
        }
      }

      [Test]
      public void Run_FuncTWithoutCancellationAndExplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        // ReSharper disable once MethodSupportsCancellation
        RunTestFuncT((action, _) => SafeContext.Task.Run(safeContextStorageProvider, action), false);
      }

      [Test]
      public void Run_FuncTWithCancellationAndImplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
          RunTestFuncT(SafeContext.Task.Run, true);
      }

      [Test]
      public void Run_FuncTWithCancellationAndExplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        RunTestFuncT((action, cancellationToken) => SafeContext.Task.Run(safeContextStorageProvider, action, cancellationToken), true);
      }

      [Test]
      public void Run_ActionTaskWithoutCancellationAndImplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
        {
          // ReSharper disable once MethodSupportsCancellation
          RunTestActionTask((action, _) => SafeContext.Task.Run(action));
        }
      }

      [Test]
      public void Run_ActionTaskWithoutCancellationAndExplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        // ReSharper disable once MethodSupportsCancellation
        RunTestActionTask((action, _) => SafeContext.Task.Run(safeContextStorageProvider, action));
      }

      [Test]
      public void Run_ActionTaskWithCancellationAndImplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
          RunTestActionTask(SafeContext.Task.Run);
      }

      [Test]
      public void Run_ActionTaskWithCancellationAndExplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        RunTestActionTask((action, cancellationToken) => SafeContext.Task.Run(safeContextStorageProvider, action, cancellationToken));
      }

      [Test]
      public void Run_FuncTaskTWithoutCancellationAndImplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
        {
          // ReSharper disable once MethodSupportsCancellation
          RunTestFuncTaskT((action, _) => SafeContext.Task.Run(action));
        }
      }

      [Test]
      public void Run_FuncTaskTWithoutCancellationAndExplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        // ReSharper disable once MethodSupportsCancellation
        RunTestFuncTaskT((action, _) => SafeContext.Task.Run(safeContextStorageProvider, action));
      }

      [Test]
      public void Run_FuncTaskTWithCancellationAndImplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
          RunTestFuncTaskT(SafeContext.Task.Run);
      }

      [Test]
      public void Run_FunTaskTWithCancellationAndExplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        RunTestFuncTaskT((action, cancellationToken) => SafeContext.Task.Run(safeContextStorageProvider, action, cancellationToken));
      }

      private void RunTestAction (Func<Action, CancellationToken, Task> taskFactory, bool cancellationShouldWork)
      {
        var delegateExecuted = false;
        var cts = new CancellationTokenSource();

        var task = taskFactory(TaskAction, cts.Token);
        task.GetAwaiter().GetResult();
        Assert.That(delegateExecuted, Is.True);

        var uncancelledTask = taskFactory(CancelAction, cts.Token);
        Task.WaitAny(uncancelledTask);
        Assert.That(uncancelledTask.Status, Is.EqualTo(TaskStatus.RanToCompletion));

        cts.Cancel();
        var cancelledTask = taskFactory(CancelAction, cts.Token);
        Task.WaitAny(cancelledTask);
        Assert.That(cancelledTask.Status, Is.EqualTo(cancellationShouldWork ? TaskStatus.Canceled : TaskStatus.Faulted));

        void TaskAction ()
        {
          _safeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
          delegateExecuted = true;
        }

        void CancelAction () => cts.Token.ThrowIfCancellationRequested();
      }

      private void RunTestFuncT (Func<Func<int>, CancellationToken, Task<int>> taskFactory, bool cancellationShouldWork)
      {
        var delegateExecuted = false;
        var cts = new CancellationTokenSource();

        var task = taskFactory(TaskAction, cts.Token);
        Assert.That(task.GetAwaiter().GetResult(), Is.EqualTo(1337));
        Assert.That(delegateExecuted, Is.True);

        var uncancelledTask = taskFactory(CancelAction, cts.Token);
        Task.WaitAny(uncancelledTask);
        Assert.That(uncancelledTask.Status, Is.EqualTo(TaskStatus.RanToCompletion));

        cts.Cancel();
        var cancelledTask = taskFactory(CancelAction, cts.Token);
        Task.WaitAny(cancelledTask);
        Assert.That(cancelledTask.Status, Is.EqualTo(cancellationShouldWork ? TaskStatus.Canceled : TaskStatus.Faulted));

        int TaskAction ()
        {
          _safeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
          delegateExecuted = true;
          return 1337;
        }

        int CancelAction ()
        {
          cts.Token.ThrowIfCancellationRequested();
          return default;
        }
      }

      private void RunTestActionTask (Func<Func<Task>, CancellationToken, Task> taskFactory)
      {
        var cts = new CancellationTokenSource();
        var tcs = new TaskCompletionSource<int>();
        var semaphore = new SemaphoreSlim(0, 1);

        var task = taskFactory(TaskAction, cts.Token);
        // ReSharper disable once MethodSupportsCancellation
        semaphore.Wait();
        Assert.That(task.Wait(TimeSpan.FromMilliseconds(5)), Is.False);
        tcs.SetResult(1337);
        task.GetAwaiter().GetResult();

        var uncancelledTask = taskFactory(CancelAction, cts.Token);
        Task.WaitAny(uncancelledTask);
        Assert.That(uncancelledTask.Status, Is.EqualTo(TaskStatus.RanToCompletion));

        cts.Cancel();
        var cancelledTask = taskFactory(CancelAction, cts.Token);
        Task.WaitAny(cancelledTask);
        // Task.Run with async callbacks always set cancelled regardless of if the cancellation token is set
        Assert.That(cancelledTask.Status, Is.EqualTo(TaskStatus.Canceled));

        Task TaskAction ()
        {
          _safeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
          semaphore.Release();
          return tcs.Task;
        }

        Task CancelAction ()
        {
          cts.Token.ThrowIfCancellationRequested();
          return Task.FromResult(0);
        }
      }

      private void RunTestFuncTaskT (Func<Func<Task<int>>, CancellationToken, Task<int>> taskFactory)
      {
        var delegateExecuted = false;
        var cts = new CancellationTokenSource();
        var tcs = new TaskCompletionSource<int>();
        tcs.SetResult(1337);

        var task = taskFactory(TaskAction, cts.Token);
        Assert.That(task.GetAwaiter().GetResult(), Is.EqualTo(1337));
        Assert.That(delegateExecuted, Is.True);

        var uncancelledTask = taskFactory(CancelAction, cts.Token);
        Task.WaitAny(uncancelledTask);
        Assert.That(uncancelledTask.Status, Is.EqualTo(TaskStatus.RanToCompletion));

        cts.Cancel();
        var cancelledTask = taskFactory(CancelAction, cts.Token);
        Task.WaitAny(cancelledTask);
        // Task.Run with async callbacks always set cancelled regardless of if the cancellation token is set
        Assert.That(cancelledTask.Status, Is.EqualTo(TaskStatus.Canceled));

        Task<int> TaskAction ()
        {
          _safeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
          delegateExecuted = true;
          return tcs.Task;
        }

        Task<int> CancelAction ()
        {
          cts.Token.ThrowIfCancellationRequested();
          return Task.FromResult(0);
        }
      }
    }
  }
}
