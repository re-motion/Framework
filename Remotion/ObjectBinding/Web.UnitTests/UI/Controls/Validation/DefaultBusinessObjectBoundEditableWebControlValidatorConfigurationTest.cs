using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation
{
  [TestFixture]
  public class DefaultBusinessObjectBoundEditableWebControlValidatorConfigurationTest
  {
    [Test]
    public void AreOptionalValidatorsEnabled_Bound_ReturnsFalse ()
    {
      var configuration = new DefaultBusinessObjectBoundEditableWebControlValidatorConfiguration();

      var propertyStub = MockRepository.GenerateStub<IBusinessObjectProperty>();
      var dataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      var controlStub = MockRepository.GenerateStub<IBusinessObjectBoundEditableWebControl>();
      controlStub.DataSource = dataSourceStub;
      controlStub.Property = propertyStub;

      Assert.That (configuration.AreOptionalValidatorsEnabled (controlStub), Is.False);
    }

    [Test]
    public void AreOptionalValidatorsEnabled_UnboundWithoutDataSourceAndWithoutProperty_ReturnsTrue ()
    {
      var configuration = new DefaultBusinessObjectBoundEditableWebControlValidatorConfiguration();

      var controlStub = MockRepository.GenerateStub<IBusinessObjectBoundEditableWebControl>();
      controlStub.DataSource = null;
      controlStub.Property = null;

      Assert.That (configuration.AreOptionalValidatorsEnabled (controlStub), Is.True);
    }

    [Test]
    public void AreOptionalValidatorsEnabled_UnboundWithoutDataSource_ReturnsTrue ()
    {
      var configuration = new DefaultBusinessObjectBoundEditableWebControlValidatorConfiguration();

      var propertyStub = MockRepository.GenerateStub<IBusinessObjectProperty>();
      var controlStub = MockRepository.GenerateStub<IBusinessObjectBoundEditableWebControl>();
      controlStub.DataSource = null;
      controlStub.Property = propertyStub;

      Assert.That (configuration.AreOptionalValidatorsEnabled (controlStub), Is.True);
    }

    [Test]
    public void AreOptionalValidatorsEnabled_UnboundWithoutProperty_ReturnsTrue ()
    {
      var configuration = new DefaultBusinessObjectBoundEditableWebControlValidatorConfiguration();

      var dataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      var controlStub = MockRepository.GenerateStub<IBusinessObjectBoundEditableWebControl>();
      controlStub.DataSource = dataSourceStub;
      controlStub.Property = null;

      Assert.That (configuration.AreOptionalValidatorsEnabled (controlStub), Is.True);
    }
  }
}