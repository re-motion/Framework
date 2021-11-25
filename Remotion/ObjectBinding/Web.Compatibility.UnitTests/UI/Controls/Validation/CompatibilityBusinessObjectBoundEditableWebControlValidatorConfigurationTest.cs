using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation
{
  [TestFixture]
  public class CompatibilityBusinessObjectBoundEditableWebControlValidatorConfigurationTest
  {
    [Test]
    public void AreOptionalValidatorsEnabled_ReturnsTrue ()
    {
      var configuration = new CompatibilityBusinessObjectBoundEditableWebControlValidatorConfiguration();

      Assert.That(configuration.AreOptionalValidatorsEnabled(new Mock<IBusinessObjectBoundEditableWebControl>().Object), Is.True);
    }
  }
}