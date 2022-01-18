using Moq;
using NUnit.Framework;
using Remotion.Context;

namespace Remotion.UnitTests.Context
{
  [TestFixture]
  public class SafeContextBoundaryTests
  {
    [Test]
    public void Dispose ()
    {
      var boundaryStrategyMock = new Mock<ISafeContextBoundaryStrategy>();
      var state = new object();

      var safeContextBoundary = new SafeContextBoundary(boundaryStrategyMock.Object, state);
      safeContextBoundary.Dispose();

      boundaryStrategyMock.Verify(e => e.RestorePreviousSafeContext(state));
    }

    [Test]
    public void Dispose_WithDefaultInitialization_ThrowsNothing ()
    {
      SafeContextBoundary safeContextBoundary = default;

      Assert.That(() => safeContextBoundary.Dispose(), Throws.Nothing);
    }
  }
}
