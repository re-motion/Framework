using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

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
      var control = GetControlWithOptionalValidatorsEnabled (isRequired, BocTextValueType.Undefined, typeof (IBusinessObjectProperty));
      var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
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
      var control = GetControlWithOptionalValidatorsDisabled (isRequired, hasDataSource, hasBusinessObject, hasProperty, BocTextValueType.Undefined, typeof (IBusinessObjectProperty));
      var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (ControlCharactersCharactersValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (ControlCharactersCharactersValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsEnabledAndStringProperty (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsEnabled (isRequired, BocTextValueType.String, typeof (IBusinessObjectStringProperty));
      var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
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
      var control = GetControlWithOptionalValidatorsDisabled (isRequired, hasDataSource, hasBusinessObject, hasProperty, BocTextValueType.String, typeof (IBusinessObjectStringProperty));
      var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (LengthValidator), typeof (ControlCharactersCharactersValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (LengthValidator), typeof (ControlCharactersCharactersValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsEnabledAndStringPropertyWithMaxLength (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsEnabled (isRequired, BocTextValueType.String, typeof (IBusinessObjectStringProperty));
      control.TextBoxStyle.MaxLength = 10;
      var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (ControlCharactersCharactersValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (ControlCharactersCharactersValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsDisabledAndStringPropertyWithMaxLength (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsDisabled (isRequired, true, true, true, BocTextValueType.String, typeof (IBusinessObjectStringProperty));
      control.TextBoxStyle.MaxLength = 10;
      var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (DateTimeValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (DateTimeValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsEnabledAndDateTimeProperty (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsEnabled (isRequired, BocTextValueType.DateTime, typeof (IBusinessObjectDateTimeProperty));
      var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
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
      var control = GetControlWithOptionalValidatorsDisabled (isRequired, hasDataSource, hasBusinessObject, hasProperty, BocTextValueType.DateTime, typeof (IBusinessObjectDateTimeProperty));
      var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (CompareValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (CompareValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsEnabledAndDateProperty (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControlWithOptionalValidatorsEnabled (isRequired, BocTextValueType.Date, typeof (IBusinessObjectDateTimeProperty));
      var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
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
      var control = GetControlWithOptionalValidatorsDisabled (isRequired, hasDataSource, hasBusinessObject, hasProperty, BocTextValueType.Date, typeof (IBusinessObjectDateTimeProperty));
      var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (NumericValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (NumericValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithOptionalValidatorsEnabledAndNumericProperty (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      Check (BocTextValueType.Byte);
      Check (BocTextValueType.Int16);
      Check (BocTextValueType.Int32);
      Check (BocTextValueType.Int64);
      Check (BocTextValueType.Decimal);
      Check (BocTextValueType.Double);
      Check (BocTextValueType.Single);

      void Check (BocTextValueType type)
      {
        var control = GetControlWithOptionalValidatorsEnabled (isRequired, type, typeof (IBusinessObjectNumericProperty));
        var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

        Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
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
      Check (BocTextValueType.Byte);
      Check (BocTextValueType.Int16);
      Check (BocTextValueType.Int32);
      Check (BocTextValueType.Int64);
      Check (BocTextValueType.Decimal);
      Check (BocTextValueType.Double);
      Check (BocTextValueType.Single);

      void Check (BocTextValueType type)
      {
        var control = GetControlWithOptionalValidatorsDisabled (isRequired, hasDataSource, hasBusinessObject, hasProperty, type, typeof (IBusinessObjectNumericProperty));
        var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

        Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
      }
    }

    private IBocTextValue GetControlWithOptionalValidatorsEnabled (bool isRequired, BocTextValueType valueType, Type propertyType)
    {
      var dataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      var propertyStub = (IBusinessObjectProperty) MockRepository.GenerateStub (propertyType);
      propertyStub.Stub (p => p.IsRequired).Throw (new InvalidOperationException ("Property is not relevant with optional validators enabled."));

      var controlMock = MockRepository.GenerateMock<IBocTextValue>();
      controlMock.Expect (c => c.ActualValueType).Return (valueType);
      controlMock.Expect (c => c.IsRequired).Return (isRequired);
      controlMock.Expect (c => c.TextBoxStyle).Return (new TextBoxStyle());
      controlMock.Expect (c => c.AreOptionalValidatorsEnabled).Return (true);
      controlMock.Expect (c => c.Property).Return (propertyStub);
      controlMock.Expect (c => c.DataSource).Return (dataSourceStub);

      var resourceManagerMock = MockRepository.GenerateMock<IResourceManager>();
      resourceManagerMock.Expect (r => r.TryGetString (Arg<string>.Is.Anything, out Arg<string>.Out ("MockValue").Dummy))
          .Return (true);

      controlMock.Expect (c => c.GetResourceManager()).Return (resourceManagerMock);
      controlMock.Expect (c => c.TargetControl).Return (new Control() { ID = "ID" });

      return controlMock;
    }

    private IBocTextValue GetControlWithOptionalValidatorsDisabled (bool isRequired, bool hasDataSource, bool hasBusinessObject, bool hasProperty, BocTextValueType valueType, Type propertyType)
    {
      var dataSourceStub = MockRepository.GenerateStub<IBusinessObjectDataSource>();
      dataSourceStub.BusinessObject = hasBusinessObject ? MockRepository.GenerateStub<IBusinessObject>() : null;
      var propertyStub = (IBusinessObjectProperty) MockRepository.GenerateStub (propertyType);
      propertyStub.Stub (p => p.IsRequired).Return (isRequired);

      var controlMock = MockRepository.GenerateMock<IBocTextValue>();
      controlMock.Expect (c => c.ActualValueType).Return (valueType);
      controlMock.Expect (c => c.IsRequired).Throw (new InvalidOperationException ("Control settings are not relevant with optional validators disabled."));
      controlMock.Expect (c => c.TextBoxStyle).Return (new TextBoxStyle());
      controlMock.Expect (c => c.AreOptionalValidatorsEnabled).Return (false);
      controlMock.Expect (c => c.Property).Return (hasProperty ? propertyStub : null);
      controlMock.Expect (c => c.DataSource).Return (hasDataSource ? dataSourceStub : null);

      var resourceManagerMock = MockRepository.GenerateMock<IResourceManager>();
      resourceManagerMock.Expect (r => r.TryGetString (Arg<string>.Is.Anything, out Arg<string>.Out ("MockValue").Dummy))
          .Return (true);

      controlMock.Expect (c => c.GetResourceManager()).Return (resourceManagerMock);
      controlMock.Expect (c => c.TargetControl).Return (new Control() { ID = "ID" });

      return controlMock;
    }
  }
}