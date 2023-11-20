using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Moq;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Validation.Globalization.UnitTests.TestDomain;
using Remotion.Validation.Globalization.UnitTests.TestHelpers;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Globalization.UnitTests
{
  [TestFixture]
  public class LocalizedValidationMessageFactoryTest
  {
    private enum TestEnum
    {
    }

    private LocalizedValidationMessageFactory _factory;
    private Mock<IPropertyInformation> _propertyStub;
    private Mock<ValidationMessage> _validationMessageStub;

    [SetUp]
    public void SetUp ()
    {
      _factory = new LocalizedValidationMessageFactory(SafeServiceLocator.Current.GetInstance<IGlobalizationService>());
      _propertyStub = new Mock<IPropertyInformation>();
      _validationMessageStub = new Mock<ValidationMessage>();
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithEqualValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(object));
      var validator = new EqualValidator(new object(), _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter the value \"{0}\"."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustBeEqualValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithExactLengthValidatorAndLengthIsOne_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(string));
      var validator = new ExactLengthValidator(1, _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter 1 character."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustHaveExactLengthOfOneValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithExactLengthValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(string));
      var validator = new ExactLengthValidator(5, _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter exactly {0} characters."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustHaveExactLengthValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithExclusiveRangeValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(object));
      var validator = new ExclusiveRangeValidator(5, 10, _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a value between {0} and {1}."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustMatchExclusiveRangeValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithGreaterThanOrEqualValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(object));
      var validator = new GreaterThanOrEqualValidator(5, _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a value greater than or equal to {0}."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustBeGreaterThanOrEqualValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithGreaterThanValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(object));
      var validator = new GreaterThanValidator(5, _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a value greater than {0}."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustBeGreaterThanValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithInclusiveRangeValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(object));
      var validator = new InclusiveRangeValidator(5, 10, _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a value from {0} to {1}."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustMatchInclusiveRangeValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithLengthValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(string));
      var validator = new LengthValidator(5, 10, _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter at least {0} and no more than {1} characters."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustMatchMinAndMaxLengthValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithLessThanOrEqualValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(object));
      var validator = new LessThanOrEqualValidator(5, _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a value less than or equal to {0}."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustBeLessThanOrEqualValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithLessThanValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(object));
      var validator = new LessThanValidator(5, _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a value less than {0}."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustBeLessThanValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithMaximumLengthValidatorAndLengthIsOne_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(string));
      var validator = new MaximumLengthValidator(1, _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter no more than 1 character."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustHaveMaximumLengthOfOneValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithMaximumLengthValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(string));
      var validator = new MaximumLengthValidator(5, _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter no more than {0} characters."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustHaveMaximumLengthValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithMinimumLengthValidatorAndLengthIsOne_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(string));
      var validator = new MinimumLengthValidator(1, _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter at least 1 character."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustHaveMinimumLengthOfOneValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithMinimumLengthValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(string));
      var validator = new MinimumLengthValidator(5, _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter at least {0} characters."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustHaveMinimumLengthValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotEmptyBinaryValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(byte[]));
      var validator = new NotEmptyBinaryValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a value."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeEmptyBinaryValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotEmptyCollectionValidator_WithReadOnlyCollection_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(IReadOnlyCollection<object>));
      var validator = new NotEmptyCollectionValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Add an entry."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeEmptyCollectionValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotEmptyCollectionValidator_WithCollection_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(ICollection));
      var validator = new NotEmptyCollectionValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Add an entry."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeEmptyCollectionValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotEmptyCollectionValidator_WithGenericCollection_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(ICollection<object>));
      var validator = new NotEmptyCollectionValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Add an entry."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeEmptyCollectionValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotEmptyOrWhitespaceValidatorAndPropertyIsString_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(string));
      var validator = new NotEmptyOrWhitespaceValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter at least one visible character."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeEmptyOrWhitespaceValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotEmptyOrWhitespaceValidatorAndPropertyIsStringArray_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(string[]));
      var validator = new NotEmptyOrWhitespaceValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter at least one visible character."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeEmptyOrWhitespaceValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotEqualValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(object));
      var validator = new NotEqualValidator(5, _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a valid value."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeEqualValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsObject_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Object));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Select an entry."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullReferenceValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsReferenceType_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Assert));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Select an entry."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullReferenceValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsString_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(String));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a value."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullStringValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsGuid_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Guid));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a value."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullStringValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableGuid_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Nullable<Guid>));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a value."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullStringValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyIsIEnumerable_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(IEnumerable));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Add an entry."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullCollectionValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsByte_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Byte));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter an integer."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullIntegerValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableByte_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Nullable<Byte>));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter an integer."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullIntegerValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsSByte_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(SByte));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter an integer."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullIntegerValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableSByte_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Nullable<SByte>));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter an integer."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullIntegerValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsInt16_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Int16));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter an integer."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullIntegerValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableInt16_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Nullable<Int16>));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter an integer."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullIntegerValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsUInt16_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(UInt16));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter an integer."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullIntegerValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableUInt16_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Nullable<UInt16>));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter an integer."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullIntegerValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsInt32_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Int32));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter an integer."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullIntegerValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableInt32_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Nullable<Int32>));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter an integer."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullIntegerValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsUInt32_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(UInt32));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter an integer."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullIntegerValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableUInt32_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Nullable<UInt32>));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter an integer."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullIntegerValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsInt64_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Int64));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter an integer."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullIntegerValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableInt64_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Nullable<Int64>));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter an integer."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullIntegerValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsSingle_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Single));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a number."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullDecimalValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableSingle_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Nullable<Single>));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a number."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullDecimalValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsDouble_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Double));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a number."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullDecimalValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableDouble_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Nullable<Double>));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a number."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullDecimalValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsDecimal_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Decimal));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a number."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullDecimalValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableDecimal_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Nullable<Decimal>));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a number."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullDecimalValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsDateTime_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(DateTime));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a date and time."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullDateTimeValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableDateTime_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Nullable<DateTime>));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a date and time."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullDateTimeValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsEnum_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(TestEnum));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Select an entry."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullEnumValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidatorAndPropertyTypeIsNullableEnum_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Nullable<TestEnum>));
      var validator = new NotNullValidator(_validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Select an entry."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotBeNullEnumValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithPredicateValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(object));
      var validator = new PredicateValidator((validatedInstance, validatedValue, context) => true, _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Is.EqualTo("Enter a value that meets the specified condition."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustMatchPredicateValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithRegularExpressionValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(string));
      var validator = new RegularExpressionValidator(new Regex(".*"), _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      var localizedValidationMessageString = validationMessage.ToString();
      Assert.That(localizedValidationMessageString, Is.EqualTo("Enter a value in the correct format."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustMatchRegularExpressionValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithDecimalValidator_ReturnsLocalizedValidationMessage ()
    {
      _propertyStub.Setup(_ => _.PropertyType).Returns(typeof(Decimal));
      var validator = new DecimalValidator(5, 10, true, _validationMessageStub.Object);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(validationMessage, Is.Not.Null);
      Assert.That(validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That(validationMessage.ToString(), Does.StartWith("Enter a number with no more than {0} integer digits and {1} decimal places."));
      Assert.That(
          ((ResourceManagerBasedValidationMessage)validationMessage).ResourceIdentifier,
          Is.EqualTo(LocalizedValidationMessageFactory.ResourceIdentifier.ValueMustNotExceedDecimalConstraintsValidationMessage));
    }

    [Test]
    public void CreateValidationMessageForObjectValidator_NoLocalizationsAvailableForObjectValidators_ReturnsNull ()
    {
      var validator = new StubObjectValidator();
      var validationMessage = _factory.CreateValidationMessageForObjectValidator(validator, TypeAdapter.Create(typeof(Customer)));

      Assert.That(validationMessage, Is.Null);
    }
  }
}
