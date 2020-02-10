using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.Globalization.UnitTests
{
  [TestFixture]
  public class LocalizedValidationMessageFactoryTest
  {
    private enum TestEnum
    {
    }

    private LocalizedValidationMessageFactory _factory;
    private IPropertyInformation _propertyStub;
    private ValidationMessage _validationMessageStub;

    [SetUp]
    public void SetUp ()
    {
      _factory = new LocalizedValidationMessageFactory (SafeServiceLocator.Current.GetInstance<IGlobalizationService>());
      _propertyStub = MockRepository.GenerateStub<IPropertyInformation>();
      _validationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithEqualValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (object));
      var validator = new EqualValidator (new object(), _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      //TODO RM-5906: for all tests Assert.That (((ResourceManagerBasedValidationMessage) validationMessage).ResourceIdentifier, Is.EqualTo (...));

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter the value \"{0}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithExactLengthValidatorAndLengthIsOne_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (string));
      var validator = new ExactLengthValidator (1, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter 1 character."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithExactLengthValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (string));
      var validator = new ExactLengthValidator (5, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter exactly {0} characters."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithExclusiveRangeValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (object));
      var validator = new ExclusiveRangeValidator (5, 10, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value between \"{0}\" and \"{1}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithGreaterThanOrEqualValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (object));
      var validator = new GreaterThanOrEqualValidator (5, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value greater than or equal to \"{0}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithGreaterThanValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (object));
      var validator = new GreaterThanValidator (5, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value greater than \"{0}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithInclusiveRangeValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (object));
      var validator = new InclusiveRangeValidator (5, 10, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value from \"{0}\" to \"{1}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithLengthValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (string));
      var validator = new LengthValidator (5, 10, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter at least {0} and no more than {1} characters."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithLessThanOrEqualValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (object));
      var validator = new LessThanOrEqualValidator (5, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value less than or equal to \"{0}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithLessThanValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (object));
      var validator = new LessThanValidator (5, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value less than \"{0}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithMaximumLengthValidatorAndLengthIsOne_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (string));
      var validator = new MaximumLengthValidator (1, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter no more than 1 character."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithMaximumLengthValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (string));
      var validator = new MaximumLengthValidator (5, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter no more than {0} characters."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithMinimumLengthValidatorAndLengthIsOne_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (string));
      var validator = new MinimumLengthValidator (1, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter at least 1 character."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithMinimumLengthValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (string));
      var validator = new MinimumLengthValidator (5, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter at least {0} characters."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotEmptyValidatorAndPropertyIsObject_ReturnsNul ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Object));
      var validator = new NotEmptyValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Null);
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotEmptyValidatorAndPropertyIsReferenceType_ReturnsNull ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Assert));
      var validator = new NotEmptyValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Null);
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotEmptyValidatorAndPropertyIsString_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (string));
      var validator = new NotEmptyValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotEmptyValidatorAndPropertyIsIEnumerable_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (IEnumerable));
      var validator = new NotEmptyValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Add an entry."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotEqualValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (object));
      var validator = new NotEqualValidator (5, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a valid value."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsObject_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Object));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Select an entry."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsReferenceType_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Assert));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Select an entry."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsString_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (String));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsGuid_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Guid));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableGuid_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Nullable<Guid>));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyIsIEnumerable_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (IEnumerable));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Add an entry."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsByte_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Byte));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter an integer."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableByte_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Nullable<Byte>));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter an integer."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsSByte_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (SByte));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter an integer."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableSByte_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Nullable<SByte>));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter an integer."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsInt16_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Int16));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter an integer."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableInt16_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Nullable<Int16>));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter an integer."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsUInt16_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (UInt16));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter an integer."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableUInt16_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Nullable<UInt16>));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter an integer."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsInt32_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Int32));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter an integer."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableInt32_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Nullable<Int32>));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter an integer."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsUInt32_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (UInt32));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter an integer."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableUInt32_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Nullable<UInt32>));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter an integer."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsInt64_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Int64));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter an integer."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableInt64_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Nullable<Int64>));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter an integer."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsSingle_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Single));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a number."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableSingle_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Nullable<Single>));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a number."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsDouble_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Double));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a number."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableDouble_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Nullable<Double>));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a number."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsDecimal_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Decimal));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a number."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableDecimal_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Nullable<Decimal>));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a number."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsDateTime_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (DateTime));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a date and time."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableDateTime_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Nullable<DateTime>));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a date and time."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsEnum_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (TestEnum));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Select an entry."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableEnum_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Nullable<TestEnum>));
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Select an entry."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithPredicateValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (object));
      var validator = new PredicateValidator ((validatedInstance, validatedValue, context) => true, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value that meets the specified condition."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithRegularExpressionValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (string));
      var validator = new RegularExpressionValidator (new Regex (".*"), _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      var localizedValidationMessageString = validationMessage.ToString();
      Assert.That (localizedValidationMessageString, Is.EqualTo ("Enter a value in the correct format."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithScalePrecisionValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Stub (_ => _.PropertyType).Return (typeof (Decimal));
      var validator = new ScalePrecisionValidator (5, 10, true, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.StringStarting ("Enter a decimal number"));
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void CreateValidationMessageForObjectValidator_NoLocalizationsAvailableForObjectValidators_ReturnsNull ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void CreateValidationMessageForObjectValidator_WithLocalizedType_ReturnsLocalizedValidationMessage ()
    {
    }
  }
}