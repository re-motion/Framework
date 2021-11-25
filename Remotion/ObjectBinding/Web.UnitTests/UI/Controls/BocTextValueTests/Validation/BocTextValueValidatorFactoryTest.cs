using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Moq;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocTextValueTests.Validation
{
  [TestFixture]
  public class BocTextValueValidatorFactoryTest
  {
    private IBocTextValueValidatorFactory _validatorFactory;

    [SetUp]
    public void SetUp ()
    {
      _validatorFactory = new BocTextValueValidatorFactory();
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/Not ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator) }, Description = "Required/NotReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new Type[0], Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsEnabledAndUndefinedProperty (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsEnabled<IBusinessObjectProperty>(isRequired, BocTextValueType.Undefined);
      var validators = _validatorFactory.CreateValidators(control, isReadonly).ToArray();

      Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(expectedValidatorTypes));
      Assert.That(validators, Has.All.Property("EnableViewState").False);
    }

    [Test]
    [TestCase (true, true, true, true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, true, true, true, new[] { typeof (RequiredFieldValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, true, true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (false, false, true, true, true, new Type[0], Description = "Required/Not ReadOnly")]
    [TestCase (true, false, false, false, true, new Type[0], Description = "Required/Not ReadOnly/No DataSource")]
    [TestCase (true, false, true, false, true, new Type[0], Description = "Required/Not ReadOnly/No BusinessObject")]
    [TestCase (true, false, true, true, false, new Type[0], Description = "Required/Not ReadOnly/No Property")]
    public void CreateValidators_WithOptionalValidatorsDisabledAndUndefinedProperty (bool isRequired, bool isReadonly, bool hasDataSource, bool hasBusinessObject, bool hasProperty, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsDisabled<IBusinessObjectProperty>(isRequired, hasDataSource, hasBusinessObject, hasProperty, BocTextValueType.Undefined);
      var validators = _validatorFactory.CreateValidators(control, isReadonly).ToArray();

      Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(expectedValidatorTypes));
      Assert.That(validators, Has.All.Property("EnableViewState").False);
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (ControlCharactersCharactersValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (ControlCharactersCharactersValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsEnabledAndStringProperty (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsEnabled<IBusinessObjectStringProperty>(isRequired, BocTextValueType.String);
      var validators = _validatorFactory.CreateValidators(control, isReadonly).ToArray();

      Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(expectedValidatorTypes));
      Assert.That(validators, Has.All.Property("EnableViewState").False);
    }

    [Test]
    [TestCase (true, true, true, true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, true, true, true, new[] { typeof (RequiredFieldValidator), typeof (ControlCharactersCharactersValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, true, true, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, true, true, true, new[] { typeof (ControlCharactersCharactersValidator) }, Description = "Not Required/Not ReadOnly")]
    [TestCase (true, false, false, false, true, new[] { typeof (ControlCharactersCharactersValidator) }, Description = "Required/Not ReadOnly/No DataSource")]
    [TestCase (true, false, true, false, true, new [] { typeof (ControlCharactersCharactersValidator) }, Description = "Required/Not ReadOnly/No BusinessObject")]
    [TestCase (true, false, true, true, false, new [] { typeof (ControlCharactersCharactersValidator) }, Description = "Required/Not ReadOnly/No Property")]
    public void CreateValidators_WithOptionalValidatorsDisabledAndStringProperty (bool isRequired, bool isReadonly, bool hasDataSource, bool hasBusinessObject, bool hasProperty, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsDisabled<IBusinessObjectStringProperty>(isRequired, hasDataSource, hasBusinessObject, hasProperty, BocTextValueType.String);
      var validators = _validatorFactory.CreateValidators(control, isReadonly).ToArray();

      Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(expectedValidatorTypes));
      Assert.That(validators, Has.All.Property("EnableViewState").False);
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (LengthValidator), typeof (ControlCharactersCharactersValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (LengthValidator), typeof (ControlCharactersCharactersValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsEnabledAndStringPropertyWithMaxLength (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsEnabled<IBusinessObjectStringProperty>(isRequired, BocTextValueType.String);
      control.TextBoxStyle.MaxLength = 10;
      var validators = _validatorFactory.CreateValidators(control, isReadonly).ToArray();

      Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(expectedValidatorTypes));
      Assert.That(validators, Has.All.Property("EnableViewState").False);
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (ControlCharactersCharactersValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (ControlCharactersCharactersValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsDisabledAndStringPropertyWithMaxLength (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsDisabled<IBusinessObjectStringProperty>(isRequired, true, true, true, BocTextValueType.String);
      control.TextBoxStyle.MaxLength = 10;
      var validators = _validatorFactory.CreateValidators(control, isReadonly).ToArray();

      Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(expectedValidatorTypes));
      Assert.That(validators, Has.All.Property("EnableViewState").False);
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (DateTimeValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (DateTimeValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsEnabledAndDateTimeProperty (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsEnabled<IBusinessObjectDateTimeProperty>(isRequired, BocTextValueType.DateTime);
      var validators = _validatorFactory.CreateValidators(control, isReadonly).ToArray();

      Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(expectedValidatorTypes));
      Assert.That(validators, Has.All.Property("EnableViewState").False);
    }

    [Test]
    [TestCase (true, true, true, true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, true, true, true, new[] { typeof (RequiredFieldValidator), typeof (DateTimeValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, true, true, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, true, true, true, new[] { typeof (DateTimeValidator) }, Description = "Not Required/Not ReadOnly")]
    [TestCase (true, false, false, false, true, new[] { typeof (DateTimeValidator) }, Description = "Required/Not ReadOnly/No DataSource")]
    [TestCase (true, false, true, false, true, new[] { typeof (DateTimeValidator) }, Description = "Required/Not ReadOnly/No BusinessObject")]
    [TestCase (true, false, true, true, false, new[] { typeof (DateTimeValidator) }, Description = "Required/Not ReadOnly/No Property")]
    public void CreateValidators_WithOptionalValidatorsDisabledAndDateTimeProperty (bool isRequired, bool isReadonly, bool hasDataSource, bool hasBusinessObject, bool hasProperty, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsDisabled<IBusinessObjectDateTimeProperty>(isRequired, hasDataSource, hasBusinessObject, hasProperty, BocTextValueType.DateTime);
      var validators = _validatorFactory.CreateValidators(control, isReadonly).ToArray();

      Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(expectedValidatorTypes));
      Assert.That(validators, Has.All.Property("EnableViewState").False);
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (CompareValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (CompareValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsEnabledAndDateProperty (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsEnabled<IBusinessObjectDateTimeProperty>(isRequired, BocTextValueType.Date);
      var validators = _validatorFactory.CreateValidators(control, isReadonly).ToArray();

      Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(expectedValidatorTypes));
      Assert.That(validators, Has.All.Property("EnableViewState").False);
    }

    [Test]
    [TestCase (true, true, true, true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, true, true, true, new[] { typeof (RequiredFieldValidator), typeof (CompareValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, true, true, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, true, true, true, new[] { typeof (CompareValidator) }, Description = "Not Required/Not ReadOnly")]
    [TestCase (true, false, false, false, true, new[] { typeof (CompareValidator) }, Description = "Required/Not ReadOnly/No DataSource")]
    [TestCase (true, false, true, false, true, new[] { typeof (CompareValidator) }, Description = "Required/Not ReadOnly/No BusinessObject")]
    [TestCase (true, false, true, true, false, new[] { typeof (CompareValidator) }, Description = "Required/Not ReadOnly/No Property")]
    public void CreateValidators_WithOptionalValidatorsDisabledAndDateProperty (bool isRequired, bool isReadonly, bool hasDataSource, bool hasBusinessObject, bool hasProperty, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsDisabled<IBusinessObjectDateTimeProperty>(isRequired, hasDataSource, hasBusinessObject, hasProperty, BocTextValueType.Date);
      var validators = _validatorFactory.CreateValidators(control, isReadonly).ToArray();

      Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(expectedValidatorTypes));
      Assert.That(validators, Has.All.Property("EnableViewState").False);
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (NumericValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (NumericValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsEnabledAndNumericProperty (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      Check(BocTextValueType.Byte);
      Check(BocTextValueType.Int16);
      Check(BocTextValueType.Int32);
      Check(BocTextValueType.Int64);
      Check(BocTextValueType.Decimal);
      Check(BocTextValueType.Double);
      Check(BocTextValueType.Single);

      void Check (BocTextValueType type)
      {
        var control = GetControlWithOptionalValidatorsEnabled<IBusinessObjectNumericProperty>(isRequired, type);
        var validators = _validatorFactory.CreateValidators(control, isReadonly).ToArray();

        Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(expectedValidatorTypes));
        Assert.That(validators, Has.All.Property("EnableViewState").False);
      }
    }

    [Test]
    [TestCase (true, true, true, true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, true, true, true, new[] { typeof (RequiredFieldValidator), typeof (NumericValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, true, true, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, true, true, true, new[] { typeof (NumericValidator) }, Description = "Not Required/Not ReadOnly")]
    [TestCase (true, false, false, false, true, new[] { typeof (NumericValidator) }, Description = "Required/Not ReadOnly/No DataSource")]
    [TestCase (true, false, true, false, true, new[] { typeof (NumericValidator) }, Description = "Required/Not ReadOnly/No BusinessObject")]
    [TestCase (true, false, true, true, false, new[] { typeof (NumericValidator) }, Description = "Required/Not ReadOnly/No Property")]
    public void CreateValidators_WithOptionalValidatorsDisabledAndNumericProperty (bool isRequired, bool isReadonly, bool hasDataSource, bool hasBusinessObject, bool hasProperty, Type[] expectedValidatorTypes)
    {
      Check(BocTextValueType.Byte);
      Check(BocTextValueType.Int16);
      Check(BocTextValueType.Int32);
      Check(BocTextValueType.Int64);
      Check(BocTextValueType.Decimal);
      Check(BocTextValueType.Double);
      Check(BocTextValueType.Single);

      void Check (BocTextValueType type)
      {
        var control = GetControlWithOptionalValidatorsDisabled<IBusinessObjectNumericProperty>(isRequired, hasDataSource, hasBusinessObject, hasProperty, type);
        var validators = _validatorFactory.CreateValidators(control, isReadonly).ToArray();

        Assert.That(validators.Select(v => v.GetType()), Is.EquivalentTo(expectedValidatorTypes));
        Assert.That(validators, Has.All.Property("EnableViewState").False);
      }
    }

    private IBocTextValue GetControlWithOptionalValidatorsEnabled<T> (bool isRequired, BocTextValueType valueType)
        where T : class
    {
      var outValue = "MockValue";
      var dataSourceStub = new Mock<IBusinessObjectDataSource>();
      var propertyStub = new Mock<T>().As<IBusinessObjectProperty>();
      propertyStub.Setup(p => p.IsNullable).Throws(new InvalidOperationException("Property.IsNullable is not relevant with optional validators enabled."));
      propertyStub.Setup(p => p.IsRequired).Throws(new InvalidOperationException("Property.IsRequired is not relevant."));

      var controlMock = new Mock<IBocTextValue>();
      controlMock.Setup(c => c.ActualValueType).Returns(valueType).Verifiable();
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

    private IBocTextValue GetControlWithOptionalValidatorsDisabled<T> (bool isRequired, bool hasDataSource, bool hasBusinessObject, bool hasProperty, BocTextValueType valueType)
      where T : class
    {
      var outValue = "MockValue";
      var dataSourceStub = new Mock<IBusinessObjectDataSource>();
      dataSourceStub.SetupProperty(_ => _.BusinessObject);
      dataSourceStub.Object.BusinessObject = hasBusinessObject ? new Mock<IBusinessObject>().Object : null;
      var propertyStub = new Mock<T>().As<IBusinessObjectProperty>();
      propertyStub.Setup(p => p.IsNullable).Returns(!isRequired);
      propertyStub.Setup(p => p.IsRequired).Throws(new InvalidOperationException("Property.IsRequired is not relevant."));

      var controlMock = new Mock<IBocTextValue>();
      controlMock.Setup(c => c.ActualValueType).Returns(valueType).Verifiable();
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