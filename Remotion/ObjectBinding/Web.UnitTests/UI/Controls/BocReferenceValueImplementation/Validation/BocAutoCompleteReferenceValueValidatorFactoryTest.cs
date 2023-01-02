using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Moq;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocReferenceValueImplementation.Validation
{
  [TestFixture]
  public class BocAutoCompleteReferenceValueValidatorFactoryTest
  {
    private IBocAutoCompleteReferenceValueValidatorFactory _validatorFactory;

    [SetUp]
    public void SetUp ()
    {
      _validatorFactory = new BocAutoCompleteReferenceValueValidatorFactory();
    }

    [Test]
    [TestCase(true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase(true, false, new[] { typeof(RequiredFieldValidator), typeof(BocAutoCompleteReferenceValueInvalidDisplayNameValidator) },
        Description = "Required/Not ReadOnly")]
    [TestCase(false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase(false, false, new[] { typeof(BocAutoCompleteReferenceValueInvalidDisplayNameValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsEnabled (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsEnabled(isRequired);
      var validators = _validatorFactory.CreateValidators(control, isReadonly).ToArray();

      Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(expectedValidatorTypes));
      Assert.That(validators, Has.All.Property("EnableViewState").False);
    }

    [Test]
    [TestCase(true, true, true, true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase(
        true,
        false,
        true,
        true,
        true,
        new[] { typeof(RequiredFieldValidator), typeof(BocAutoCompleteReferenceValueInvalidDisplayNameValidator) },
        Description = "Required/Not ReadOnly")]
    [TestCase(false, true, true, true, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase(false, false, true, true, true, new[] { typeof(BocAutoCompleteReferenceValueInvalidDisplayNameValidator) }, Description = "Not Required/Not ReadOnly")]
    [TestCase(true, false, false, false, true, new[] { typeof(BocAutoCompleteReferenceValueInvalidDisplayNameValidator) }, Description = "Required/Not ReadOnly/No DataSource")]
    [TestCase(true, false, true, false, true, new[] { typeof(BocAutoCompleteReferenceValueInvalidDisplayNameValidator) }, Description = "Required/Not ReadOnly/No BusinessObject")]
    [TestCase(true, false, true, true, false, new[] { typeof(BocAutoCompleteReferenceValueInvalidDisplayNameValidator) }, Description = "Required/Not ReadOnly/No Property")]
    public void CreateValidators_WithOptionalValidatorsDisabled (
        bool isRequired,
        bool isReadonly,
        bool hasDataSource,
        bool hasBusinessObject,
        bool hasProperty,
        Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsDisabled(isRequired, hasDataSource, hasBusinessObject, hasProperty);
      var validators = _validatorFactory.CreateValidators(control, isReadonly).ToArray();

      Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(expectedValidatorTypes));
      Assert.That(validators, Has.All.Property("EnableViewState").False);
    }

    private IBocAutoCompleteReferenceValue GetControlWithOptionalValidatorsEnabled (bool isRequired)
    {
      var outValue = "MockValue";
      var dataSourceStub = new Mock<IBusinessObjectDataSource>();
      var propertyStub = new Mock<IBusinessObjectReferenceProperty>();
      propertyStub.Setup(p => p.IsNullable).Throws(new InvalidOperationException("Property.IsNullable is not relevant with optional validators enabled."));
      propertyStub.Setup(p => p.IsRequired).Throws(new InvalidOperationException("Property.IsRequired is not relevant."));

      var controlMock = new Mock<IBocAutoCompleteReferenceValue>();
      controlMock.Setup(c => c.IsRequired).Returns(isRequired).Verifiable();
      controlMock.Setup(c => c.TextBoxStyle).Returns(new TextBoxStyle()).Verifiable();
      controlMock.Setup(c => c.AreOptionalValidatorsEnabled).Returns(true).Verifiable();
      controlMock.Setup(c => c.Property).Returns(propertyStub.Object).Verifiable();
      controlMock.Setup(c => c.DataSource).Returns(dataSourceStub.Object).Verifiable();

      var resourceManagerMock = new Mock<IResourceManager>();
      resourceManagerMock.Setup(r => r.TryGetString(It.IsAny<string>(), out outValue))
          .Returns(true)
          .Verifiable();

      controlMock.Setup(c => c.GetResourceManager()).Returns(resourceManagerMock.Object).Verifiable();
      controlMock.Setup(c => c.TargetControl).Returns(new Control() { ID = "ID" }).Verifiable();

      return controlMock.Object;
    }

    private IBocAutoCompleteReferenceValue GetControlWithOptionalValidatorsDisabled (
        bool isRequired,
        bool hasDataSource = true,
        bool hasBusinessObject = true,
        bool hasProperty = true)
    {
      var outValue = "MockValue";
      var dataSourceStub = new Mock<IBusinessObjectDataSource>();
      dataSourceStub.SetupProperty(_ => _.BusinessObject);
      dataSourceStub.Object.BusinessObject = hasBusinessObject ? new Mock<IBusinessObject>().Object : null;
      var propertyStub = new Mock<IBusinessObjectReferenceProperty>();
      propertyStub.Setup(p => p.IsNullable).Returns(!isRequired);
      propertyStub.Setup(p => p.IsRequired).Throws(new InvalidOperationException("Property.IsRequired is not relevant."));

      var controlMock = new Mock<IBocAutoCompleteReferenceValue>();
      controlMock.Setup(c => c.IsRequired).Throws(new InvalidOperationException("Control settings are not relevant with optional validators disabled.")).Verifiable();
      controlMock.Setup(c => c.TextBoxStyle).Returns(new TextBoxStyle()).Verifiable();
      controlMock.Setup(c => c.AreOptionalValidatorsEnabled).Returns(false).Verifiable();
      controlMock.Setup(c => c.Property).Returns(hasProperty ? propertyStub.Object : null).Verifiable();
      controlMock.Setup(c => c.DataSource).Returns(hasDataSource ? dataSourceStub.Object : null).Verifiable();

      var resourceManagerMock = new Mock<IResourceManager>();
      resourceManagerMock.Setup(r => r.TryGetString(It.IsAny<string>(), out outValue))
          .Returns(true)
          .Verifiable();

      controlMock.Setup(c => c.GetResourceManager()).Returns(resourceManagerMock.Object).Verifiable();
      controlMock.Setup(c => c.TargetControl).Returns(new Control() { ID = "ID" }).Verifiable();

      return controlMock.Object;
    }
  }
}
