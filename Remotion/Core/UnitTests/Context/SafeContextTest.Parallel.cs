using System;
using Moq;
using NUnit.Framework;
using Remotion.Context;

namespace Remotion.UnitTests.Context
{
  public partial class SafeContextTest
  {
    [TestFixture]
    public class ParallelTest
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
        {
          using (SafeContext.Parallel.OpenSafeContextBoundary())
            _safeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
        }
      }

      [Test]
      public void Run_WithExplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
      {
        var safeContextStorageProvider = _safeContextProviderMock.Object;
        using (SafeContext.Parallel.OpenSafeContextBoundary(safeContextStorageProvider))
          _safeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
      }
    }
  }
}
