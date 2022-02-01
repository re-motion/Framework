using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.Context
{
  public partial class SafeContextTest
  {
    [TestFixture]
    public class ExecutionContextTest
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
      public void Run_WithImplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
          RunThreadStartTest((callback, state) => SafeContext.ExecutionContext.Run(ExecutionContext.Capture()!, callback, state));
      }

      [Test]
      public void Run_WithExplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        RunThreadStartTest((callback, state) => SafeContext.ExecutionContext.Run(safeContextStorageProvider, ExecutionContext.Capture()!, callback, state));
      }

      private void RunThreadStartTest (Action<ContextCallback, object> executionContextRun)
      {
        var delegateExecuted = false;

        var shouldState = new object();
        executionContextRun(ContextCallback, shouldState);
        Assert.That(delegateExecuted, Is.True);

        void ContextCallback (object state)
        {
          Assert.That(state, Is.SameAs(shouldState));
          _safeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
          delegateExecuted = true;
        }
      }
    }
  }
}
