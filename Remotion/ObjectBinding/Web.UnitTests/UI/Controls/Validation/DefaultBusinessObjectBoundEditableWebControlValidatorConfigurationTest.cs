using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation
{
  [TestFixture]
  public class DefaultBusinessObjectBoundEditableWebControlValidatorConfigurationTest
  {
    [Test]
    public void AreOptionalValidatorsEnabled_Bound_ReturnsFalse ()
    {
      var configuration = new DefaultBusinessObjectBoundEditableWebControlValidatorConfiguration();

      var propertyStub = new Mock<IBusinessObjectProperty>();
      var dataSourceStub = new Mock<IBusinessObjectDataSource>();
      var controlStub = new Mock<IBusinessObjectBoundEditableWebControl>();
      controlStub.SetupProperty (_ => _.DataSource);
      controlStub.SetupProperty (_ => _.Property);
      controlStub.Object.DataSource = dataSourceStub.Object;
      controlStub.Object.Property = propertyStub.Object;

      Assert.That (configuration.AreOptionalValidatorsEnabled (controlStub.Object), Is.False);
    }

    [Test]
    public void AreOptionalValidatorsEnabled_UnboundWithoutDataSourceAndWithoutProperty_ReturnsTrue ()
    {
      var configuration = new DefaultBusinessObjectBoundEditableWebControlValidatorConfiguration();

      var controlStub = new Mock<IBusinessObjectBoundEditableWebControl>();
      controlStub.Object.DataSource = null;
      controlStub.Object.Property = null;

      Assert.That (configuration.AreOptionalValidatorsEnabled (controlStub.Object), Is.True);
    }

    [Test]
    public void AreOptionalValidatorsEnabled_UnboundWithoutDataSource_ReturnsTrue ()
    {
      var configuration = new DefaultBusinessObjectBoundEditableWebControlValidatorConfiguration();

      var propertyStub = new Mock<IBusinessObjectProperty>();
      var controlStub = new Mock<IBusinessObjectBoundEditableWebControl>();
      controlStub.Object.DataSource = null;
      controlStub.Object.Property = propertyStub.Object;

      Assert.That (configuration.AreOptionalValidatorsEnabled (controlStub.Object), Is.True);
    }

    [Test]
    public void AreOptionalValidatorsEnabled_UnboundWithoutProperty_ReturnsTrue ()
    {
      var configuration = new DefaultBusinessObjectBoundEditableWebControlValidatorConfiguration();

      var dataSourceStub = new Mock<IBusinessObjectDataSource>();
      var controlStub = new Mock<IBusinessObjectBoundEditableWebControl>();
      controlStub.Object.DataSource = dataSourceStub.Object;
      controlStub.Object.Property = null;

      Assert.That (configuration.AreOptionalValidatorsEnabled (controlStub.Object), Is.True);
    }
  }
}