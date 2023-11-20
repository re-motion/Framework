// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Data.DomainObjects.Validation.UnitTests.DomainObjectAttributesBasedValidationPropertyRuleReflectorTests
{
  [TestFixture]
// ReSharper disable InconsistentNaming
    public class DomainObject_DomainObjectAttributesBasedValidationPropertyRuleReflectorTest
// ReSharper enable InconsistentNaming
  {
    private PropertyInfo _propertyWithoutAttribute;
    private PropertyInfo _propertyWithMandatoryAttribute;
    private PropertyInfo _propertyWithNullableStringPropertyAttribute;
    private PropertyInfo _propertyWithMandatoryStringPropertyAttribute;
    private PropertyInfo _binaryProperty;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _propertyWithoutAttributeReflector;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _propertyWithNullableStringPropertyAttributeReflector;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _propertyWithMandatoryStringPropertyAttributeReflector;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _propertyWithMandatoryAttributeReflector;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _binaryPropertyReflector;
    private PropertyInfo _domainObjectCollectionProperty;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _domainObjectCollectionPropertyReflector;
    private PropertyInfo _virtualCollectionProperty;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _virtualCollectionPropertyReflector;
    private Mock<IValidationMessageFactory> _validationMessageFactoryStub;
    private DomainModelConstraintProvider _domainModelConstraintProvider;

    [SetUp]
    public void SetUp ()
    {
      _propertyWithoutAttribute = typeof(TypeWithDomainObjectAttributes).GetProperty("PropertyWithoutAttribute");
      _propertyWithMandatoryAttribute = typeof(TypeWithDomainObjectAttributes).GetProperty("PropertyWithMandatoryAttribute");
      _propertyWithNullableStringPropertyAttribute =
          typeof(TypeWithDomainObjectAttributes).GetProperty("PropertyWithNullableStringPropertyAttribute");
      _propertyWithMandatoryStringPropertyAttribute =
          typeof(TypeWithDomainObjectAttributes).GetProperty("PropertyWithMandatoryStringPropertyAttribute");
      _binaryProperty = typeof(TypeWithDomainObjectAttributes).GetProperty("BinaryProperty");
      _domainObjectCollectionProperty = typeof(TypeWithDomainObjectAttributes).GetProperty("DomainObjectCollectionProperty");
      _virtualCollectionProperty = typeof(TypeWithDomainObjectAttributes).GetProperty("VirtualCollectionProperty");

      _domainModelConstraintProvider = new DomainModelConstraintProvider();
      _validationMessageFactoryStub = new Mock<IValidationMessageFactory>();

      _propertyWithoutAttributeReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          _propertyWithoutAttribute,
          _propertyWithoutAttribute,
          _domainModelConstraintProvider,
          _validationMessageFactoryStub.Object);
      _propertyWithMandatoryAttributeReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          _propertyWithMandatoryAttribute,
          _propertyWithMandatoryAttribute,
          _domainModelConstraintProvider,
          _validationMessageFactoryStub.Object);
      _propertyWithNullableStringPropertyAttributeReflector =
          new DomainObjectAttributesBasedValidationPropertyRuleReflector(
              _propertyWithNullableStringPropertyAttribute,
              _propertyWithNullableStringPropertyAttribute,
              _domainModelConstraintProvider,
              _validationMessageFactoryStub.Object);
      _propertyWithMandatoryStringPropertyAttributeReflector =
          new DomainObjectAttributesBasedValidationPropertyRuleReflector(
              _propertyWithMandatoryStringPropertyAttribute,
              _propertyWithMandatoryStringPropertyAttribute,
              _domainModelConstraintProvider,
              _validationMessageFactoryStub.Object);
      _binaryPropertyReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          _binaryProperty,
          _binaryProperty,
          _domainModelConstraintProvider,
          _validationMessageFactoryStub.Object);
      _domainObjectCollectionPropertyReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          _domainObjectCollectionProperty,
          _domainObjectCollectionProperty,
          _domainModelConstraintProvider,
          _validationMessageFactoryStub.Object);
      _virtualCollectionPropertyReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          _virtualCollectionProperty,
          _virtualCollectionProperty,
          _domainModelConstraintProvider,
          _validationMessageFactoryStub.Object);
    }

    [Test]
    public void NoAttributes ()
    {
      Assert.That(_propertyWithoutAttributeReflector.GetRemovablePropertyValidators().Any(), Is.False);
      Assert.That(_propertyWithoutAttributeReflector.GetNonRemovablePropertyValidators().Any(), Is.False);
      Assert.That(_propertyWithoutAttributeReflector.GetRemovingValidatorRegistrations().Any(), Is.False);
      Assert.That(_propertyWithoutAttributeReflector.GetMetaValidationRules().Any(), Is.False);
    }

    [Test]
    public void GetNonRemovablePropertyValidators_MandatoryAttribute ()
    {
      var validationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsAny<NotNullValidator>(),
                  PropertyInfoAdapter.Create(_propertyWithMandatoryAttribute)))
          .Returns(validationMessageStub.Object);

      var result = _propertyWithMandatoryAttributeReflector.GetNonRemovablePropertyValidators().ToArray();

      validationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message");
      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof(NotNullValidator)));
      Assert.That(((NotNullValidator)result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void GetRemovablePropertyValidators_MandatoryAttributeAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetRemovablePropertyValidators_NullableStringPropertyAttribute ()
    {
      var result = _propertyWithNullableStringPropertyAttributeReflector.GetRemovablePropertyValidators().ToArray();

      Assert.That(result, Is.Empty);
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void GetRemovablePropertyValidators_NullableStringPropertyAttributeAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetRemovablePropertyValidators_MandatoryStringPropertyAttribute ()
    {
      var notEmptyValidationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsAny<NotEmptyOrWhitespaceValidator>(),
                  PropertyInfoAdapter.Create(_propertyWithMandatoryStringPropertyAttribute)))
          .Returns(notEmptyValidationMessageStub.Object);

      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetRemovablePropertyValidators().ToArray();

      Assert.That(result.Length, Is.EqualTo(1));
      notEmptyValidationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message for NotEmpty");
      Assert.That(result[0], Is.TypeOf(typeof(NotEmptyOrWhitespaceValidator)));
      Assert.That(((NotEmptyOrWhitespaceValidator)result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message for NotEmpty"));
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void GetRemovablePropertyValidators_MandatoryStringPropertyAttributeAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetRemovablePropertyValidators_BinaryProperty ()
    {
      var validationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsAny<NotEmptyBinaryValidator>(),
                  PropertyInfoAdapter.Create(_binaryProperty)))
          .Returns(validationMessageStub.Object);

      var result = _binaryPropertyReflector.GetRemovablePropertyValidators().ToArray();

      validationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message");
      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof(NotEmptyBinaryValidator)));
      Assert.That(((NotEmptyBinaryValidator)result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void GetRemovablePropertyValidators_BinaryPropertyAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetNonRemovablePropertyValidators_DomainObjectCollectionProperty ()
    {
      var notNullValidationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsAny<NotNullValidator>(),
                  PropertyInfoAdapter.Create(_domainObjectCollectionProperty)))
          .Returns(notNullValidationMessageStub.Object);

      var notEmptyValidationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsAny<NotEmptyCollectionValidator>(),
                  PropertyInfoAdapter.Create(_domainObjectCollectionProperty)))
          .Returns(notEmptyValidationMessageStub.Object);

      var result = _domainObjectCollectionPropertyReflector.GetNonRemovablePropertyValidators().ToArray();

      notNullValidationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message for NotNull");
      Assert.That(result.Count(), Is.EqualTo(2));
      Assert.That(result[0], Is.TypeOf(typeof(NotNullValidator)));
      Assert.That(((NotNullValidator)result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message for NotNull"));

      notEmptyValidationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message for NotEmpty");
      Assert.That(result[1], Is.TypeOf(typeof(NotEmptyCollectionValidator)));
      Assert.That(((NotEmptyCollectionValidator)result[1]).ValidationMessage.ToString(), Is.EqualTo("Stub Message for NotEmpty"));
    }

    [Test]
    public void GetRemovablePropertyValidators_DomainObjectCollectionProperty ()
    {
      Assert.That(_domainObjectCollectionPropertyReflector.GetRemovablePropertyValidators().ToArray(), Is.Empty);
    }

    [Test]
    public void GetNonRemovablePropertyValidators_VirtualCollectionProperty ()
    {
      var notNullValidationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsAny<NotNullValidator>(),
                  PropertyInfoAdapter.Create(_virtualCollectionProperty)))
          .Returns(notNullValidationMessageStub.Object);

      var notEmptyValidationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsAny<NotEmptyCollectionValidator>(),
                  PropertyInfoAdapter.Create(_virtualCollectionProperty)))
          .Returns(notEmptyValidationMessageStub.Object);

      var result = _virtualCollectionPropertyReflector.GetNonRemovablePropertyValidators().ToArray();

      notNullValidationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message for NotNull");
      Assert.That(result.Count(), Is.EqualTo(2));
      Assert.That(result[0], Is.TypeOf(typeof(NotNullValidator)));
      Assert.That(((NotNullValidator)result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message for NotNull"));

      notEmptyValidationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message for NotEmpty");
      Assert.That(result[1], Is.TypeOf(typeof(NotEmptyCollectionValidator)));
      Assert.That(((NotEmptyCollectionValidator)result[1]).ValidationMessage.ToString(), Is.EqualTo("Stub Message for NotEmpty"));
    }

    [Test]
    public void GetRemovablePropertyValidators_VirtualCollectionProperty ()
    {
      Assert.That(_virtualCollectionPropertyReflector.GetRemovablePropertyValidators().ToArray(), Is.Empty);
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void GetRemovablePropertyValidators_DomainObjectCollectionPropertyAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void GetRemovablePropertyValidators_VirtualCollectionPropertyAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetNonRemovablePropertyValidators_NullableStringPropertyAttribute ()
    {
      var validationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsAny<MaximumLengthValidator>(),
                  PropertyInfoAdapter.Create(_propertyWithNullableStringPropertyAttribute)))
          .Returns(validationMessageStub.Object);

      var result = _propertyWithNullableStringPropertyAttributeReflector.GetNonRemovablePropertyValidators().ToArray();

      validationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message");

      Assert.That(result.Count(), Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof(MaximumLengthValidator)));
      Assert.That(((MaximumLengthValidator)result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
    }

    [Test]
    public void GetNonRemovablePropertyValidators_MandatoryStringPropertyAttribute ()
    {
      var notNullValidationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsAny<NotNullValidator>(),
                  PropertyInfoAdapter.Create(_propertyWithMandatoryStringPropertyAttribute)))
          .Returns(notNullValidationMessageStub.Object);

      var lengthValidationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsAny<MaximumLengthValidator>(),
                  PropertyInfoAdapter.Create(_propertyWithMandatoryStringPropertyAttribute)))
          .Returns(lengthValidationMessageStub.Object);

      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetNonRemovablePropertyValidators().ToArray();

      lengthValidationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message for length");
      Assert.That(result.Count(), Is.EqualTo(2));
      Assert.That(result[0], Is.TypeOf(typeof(MaximumLengthValidator)));
      Assert.That(((MaximumLengthValidator)result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message for length"));

      notNullValidationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message for not null");

      Assert.That(result[1], Is.TypeOf(typeof(NotNullValidator)));
      Assert.That(((NotNullValidator)result[1]).ValidationMessage.ToString(), Is.EqualTo("Stub Message for not null"));
    }

    [Test]
    public void GetRemovingPropertyRegistrations ()
    {
      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetRemovingValidatorRegistrations().ToArray();

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetMetaValidationRules_MandatoryAttribute ()
    {
      var result = _propertyWithMandatoryAttributeReflector.GetMetaValidationRules().ToArray();

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetMetaValidationRules_NullableStringPropertyAttribute ()
    {
      var result = _propertyWithNullableStringPropertyAttributeReflector.GetMetaValidationRules().ToArray();

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void Initialize_WithDerivedImplementationProperty_ThrowsArgumentException ()
    {
      var overriddenProperty = typeof(DerivedTypeWithDomainObjectAttributes).GetProperty("PropertyWithNullableStringPropertyAttribute");

      Assert.That(
          () => new DomainObjectAttributesBasedValidationPropertyRuleReflector(
              interfaceProperty: _propertyWithNullableStringPropertyAttribute,
              implementationProperty: overriddenProperty,
              domainModelConstraintProvider: _domainModelConstraintProvider,
              validationMessageFactory: _validationMessageFactoryStub.Object),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The property 'PropertyWithNullableStringPropertyAttribute' was used from the overridden declaration on type "
              + "'Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain.DerivedTypeWithDomainObjectAttributes' "
              + "but only original declarations are supported.", "implementationProperty"));
    }
  }
}
