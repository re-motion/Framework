using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Rhino.Mocks;
using MockRepository = Rhino.Mocks.MockRepository;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation
{
  [TestFixture]
  public class CompatibilityBusinessObjectBoundEditableWebControlValidatorConfigurationTest
  {
    [Test]
    public void AreOptionalValidatorsEnabled_ReturnsTrue ()
    {
      var configuration = new CompatibilityBusinessObjectBoundEditableWebControlValidatorConfiguration();

      Assert.That (configuration.AreOptionalValidatorsEnabled (new Mock<IBusinessObjectBoundEditableWebControl>().Object), Is.True);
    }
  }
}