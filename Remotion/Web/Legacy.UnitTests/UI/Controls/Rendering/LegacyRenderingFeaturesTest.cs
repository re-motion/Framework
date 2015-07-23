using System;
using NUnit.Framework;
using Remotion.Web.Legacy.UI.Controls.Rendering;

namespace Remotion.Web.Legacy.UnitTests.UI.Controls.Rendering
{
  [TestFixture]
  public class LegacyRenderingFeaturesTest
  {
    [Test]
    public void TestLegacyRenderingFeatures ()
    {
      Assert.That (LegacyRenderingFeatures.ForLegacy.EnableDiagnosticMetadata, Is.False);
    }
  }
}