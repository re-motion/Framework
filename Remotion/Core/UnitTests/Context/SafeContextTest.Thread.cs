using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using Moq;
using NUnit.Framework;
using Remotion.Context;

namespace Remotion.UnitTests.Context
{
  public partial class SafeContextTest
  {
    [TestFixture]
    public class ThreadTest
    {
      private const int c_maxStackSize = 4096;

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
      public void New_WithCallbackAndImplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
          RunThreadStartTest(SafeContext.Thread.New);
      }

      [Test]
      public void New_WithCallbackAndExplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        RunThreadStartTest(start => SafeContext.Thread.New(safeContextStorageProvider, start));
      }

      [Test]
      public void New_WithCallbackAndImplicitSafeContextStorageProviderAndMaxStackSize_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
          RunThreadStartTest(start => SafeContext.Thread.New(start, c_maxStackSize));
      }

      [Test]
      public void New_WithCallbackAndExplicitSafeContextStorageProviderAndMaxStackSize_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        RunThreadStartTest(start => SafeContext.Thread.New(safeContextStorageProvider, start, c_maxStackSize));
      }

      [Test]
      public void New_WithParameterizedCallbackAndImplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
          RunParameterizedThreadStartTest(SafeContext.Thread.New);
      }

      [Test]
      public void New_WithParameterizedCallbackAndExplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        RunParameterizedThreadStartTest(start => SafeContext.Thread.New(safeContextStorageProvider, start));
      }

      [Test]
      public void New_WithParameterizedCallbackAndImplicitSafeContextStorageProviderAndMaxStackSize_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
          RunParameterizedThreadStartTest(start => SafeContext.Thread.New(start, c_maxStackSize));
      }

      [Test]
      public void New_WithParameterizedCallbackAndExplicitSafeContextStorageProviderAndMaxStackSize_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        RunParameterizedThreadStartTest(start => SafeContext.Thread.New(safeContextStorageProvider, start, c_maxStackSize));
      }

      private void RunThreadStartTest (Func<ThreadStart, Thread> threadFactory)
      {
        ExceptionDispatchInfo exceptionDispatchInfo = null;
        var delegateExecuted = false;

        var thread = threadFactory(ThreadMain);
        thread.Start();
        thread.Join();

        exceptionDispatchInfo?.Throw();
        Assert.That(delegateExecuted, Is.True);

        void ThreadMain ()
        {
          try
          {
            _safeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
            delegateExecuted = true;
          }
          catch (Exception ex)
          {
            exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
          }
        }
      }

      private void RunParameterizedThreadStartTest (Func<ParameterizedThreadStart, Thread> threadFactory)
      {
        var parameter = new object();
        ExceptionDispatchInfo exceptionDispatchInfo = null;
        var delegateExecuted = false;

        var thread = threadFactory(ThreadMain);
        thread.Start(parameter);
        thread.Join();

        exceptionDispatchInfo?.Throw();
        Assert.That(delegateExecuted, Is.True);

        void ThreadMain (object v)
        {
          try
          {
            Assert.That(v, Is.EqualTo(parameter));
            _safeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
            delegateExecuted = true;
          }
          catch (Exception ex)
          {
            exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
          }
        }
      }
    }
  }
}
