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
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain;
using Remotion.Development.UnitTesting.NUnit;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation.Rules.Custom;
using Remotion.Validation.Validators;
using Rhino.Mocks;

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
    private IValidationMessageFactory _validationMessageFactoryStub;
    private DomainModelConstraintProvider _domainModelConstraintProvider;

    [SetUp]
    public void SetUp ()
    {
      _propertyWithoutAttribute = typeof (TypeWithDomainObjectAttributes).GetProperty("PropertyWithoutAttribute");
      _propertyWithMandatoryAttribute = typeof (TypeWithDomainObjectAttributes).GetProperty("PropertyWithMandatoryAttribute");
      _propertyWithNullableStringPropertyAttribute =
          typeof (TypeWithDomainObjectAttributes).GetProperty("PropertyWithNullableStringPropertyAttribute");
      _propertyWithMandatoryStringPropertyAttribute =
          typeof (TypeWithDomainObjectAttributes).GetProperty("PropertyWithMandatoryStringPropertyAttribute");
      _binaryProperty = typeof (TypeWithDomainObjectAttributes).GetProperty("BinaryProperty");
      _domainObjectCollectionProperty = typeof (TypeWithDomainObjectAttributes).GetProperty("DomainObjectCollectionProperty");
      _virtualCollectionProperty = typeof (TypeWithDomainObjectAttributes).GetProperty("VirtualCollectionProperty");

      _domainModelConstraintProvider = new DomainModelConstraintProvider();
      _validationMessageFactoryStub = MockRepository.GenerateStub<IValidationMessageFactory>();

      _propertyWithoutAttributeReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          _propertyWithoutAttribute,
          _propertyWithoutAttribute,
          _domainModelConstraintProvider,
          _validationMessageFactoryStub);
      _propertyWithMandatoryAttributeReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          _propertyWithMandatoryAttribute,
          _propertyWithMandatoryAttribute,
          _domainModelConstraintProvider,
          _validationMessageFactoryStub);
      _propertyWithNullableStringPropertyAttributeReflector =
          new DomainObjectAttributesBasedValidationPropertyRuleReflector(
              _propertyWithNullableStringPropertyAttribute,
              _propertyWithNullableStringPropertyAttribute,
              _domainModelConstraintProvider,
              _validationMessageFactoryStub);
      _propertyWithMandatoryStringPropertyAttributeReflector =
          new DomainObjectAttributesBasedValidationPropertyRuleReflector(
              _propertyWithMandatoryStringPropertyAttribute,
              _propertyWithMandatoryStringPropertyAttribute,
              _domainModelConstraintProvider,
              _validationMessageFactoryStub);
      _binaryPropertyReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          _binaryProperty,
          _binaryProperty,
          _domainModelConstraintProvider,
          _validationMessageFactoryStub);
      _domainObjectCollectionPropertyReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          _domainObjectCollectionProperty,
          _domainObjectCollectionProperty,
          _domainModelConstraintProvider,
          _validationMessageFactoryStub); 
      _virtualCollectionPropertyReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          _virtualCollectionProperty,
          _virtualCollectionProperty,
          _domainModelConstraintProvider,
          _validationMessageFactoryStub); 
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
      var validationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub(
              _ => _.CreateValidationMessageForPropertyValidator(
                  Arg<NotNullValidator>.Is.TypeOf,
                  Arg.Is(PropertyInfoAdapter.Create(_propertyWithMandatoryAttribute))))
          .Return(validationMessageStub);

      var result = _propertyWithMandatoryAttributeReflector.GetNonRemovablePropertyValidators().ToArray();

      validationMessageStub.Stub(_ => _.ToString()).Return("Stub Message");
      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof (NotNullValidator)));
      Assert.That(((NotNullValidator) result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void GetRemovablePropertyValidators_MandatoryAttributeAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetRemovablePropertyValidators_NullableStringPropertyAttribute ()
    {
      var validationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub(
              _ => _.CreateValidationMessageForPropertyValidator(
                  Arg<MaximumLengthValidator>.Is.TypeOf,
                  Arg.Is(PropertyInfoAdapter.Create(_propertyWithNullableStringPropertyAttribute))))
          .Return(validationMessageStub);

      var result = _propertyWithNullableStringPropertyAttributeReflector.GetRemovablePropertyValidators().ToArray();

      validationMessageStub.Stub(_ => _.ToString()).Return("Stub Message");
      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof (MaximumLengthValidator)));
      Assert.That(((MaximumLengthValidator) result[0]).Max, Is.EqualTo(10));
      Assert.That(((MaximumLengthValidator) result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void GetRemovablePropertyValidators_NullableStringPropertyAttributeAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetRemovablePropertyValidators_MandatoryStringPropertyAttribute ()
    {
      var lengthValidationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub(
              _ => _.CreateValidationMessageForPropertyValidator(
                  Arg<MaximumLengthValidator>.Is.TypeOf,
                  Arg.Is(PropertyInfoAdapter.Create(_propertyWithMandatoryStringPropertyAttribute))))
          .Return(lengthValidationMessageStub);

      var notEmptyValidationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub(
              _ => _.CreateValidationMessageForPropertyValidator(
                  Arg<NotEmptyValidator>.Is.TypeOf,
                  Arg.Is(PropertyInfoAdapter.Create(_propertyWithMandatoryStringPropertyAttribute))))
          .Return(notEmptyValidationMessageStub);

      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetRemovablePropertyValidators().ToArray();

      lengthValidationMessageStub.Stub(_ => _.ToString()).Return("Stub Message for Length");
      Assert.That(result.Length, Is.EqualTo(2));
      Assert.That(result[0], Is.TypeOf(typeof (MaximumLengthValidator)));
      Assert.That(((MaximumLengthValidator) result[0]).Max, Is.EqualTo(20));
      Assert.That(((MaximumLengthValidator) result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message for Length"));

      notEmptyValidationMessageStub.Stub(_ => _.ToString()).Return("Stub Message for NotEmpty");
      Assert.That(result[1], Is.TypeOf(typeof (NotEmptyValidator)));
      Assert.That(((NotEmptyValidator) result[1]).ValidationMessage.ToString(), Is.EqualTo("Stub Message for NotEmpty"));
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void GetRemovablePropertyValidators_MandatoryStringPropertyAttributeAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetRemovablePropertyValidators_BinaryProperty ()
    {
      var validationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub(
              _ => _.CreateValidationMessageForPropertyValidator(
                  Arg<NotEmptyValidator>.Is.TypeOf,
                  Arg.Is(PropertyInfoAdapter.Create(_binaryProperty))))
          .Return(validationMessageStub);

      var result = _binaryPropertyReflector.GetRemovablePropertyValidators().ToArray();

      validationMessageStub.Stub(_ => _.ToString()).Return("Stub Message");
      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof (NotEmptyValidator)));
      Assert.That(((NotEmptyValidator) result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void GetRemovablePropertyValidators_BinaryPropertyAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetNonRemovablePropertyValidators_DomainObjectCollectionProperty ()
    {
      var notNullValidationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub(
              _ => _.CreateValidationMessageForPropertyValidator(
                  Arg<NotNullValidator>.Is.TypeOf,
                  Arg.Is(PropertyInfoAdapter.Create(_domainObjectCollectionProperty))))
          .Return(notNullValidationMessageStub);

      var notEmptyValidationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub(
              _ => _.CreateValidationMessageForPropertyValidator(
                  Arg<NotEmptyValidator>.Is.TypeOf,
                  Arg.Is(PropertyInfoAdapter.Create(_domainObjectCollectionProperty))))
          .Return(notEmptyValidationMessageStub);

      var result = _domainObjectCollectionPropertyReflector.GetNonRemovablePropertyValidators().ToArray();

      notNullValidationMessageStub.Stub(_ => _.ToString()).Return("Stub Message for NotNull");
      Assert.That(result.Count(), Is.EqualTo(2));
      Assert.That(result[0], Is.TypeOf(typeof (NotNullValidator)));
      Assert.That(((NotNullValidator) result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message for NotNull"));

      notEmptyValidationMessageStub.Stub(_ => _.ToString()).Return("Stub Message for NotEmpty");
      Assert.That(result[1], Is.TypeOf(typeof (NotEmptyValidator)));
      Assert.That(((NotEmptyValidator) result[1]).ValidationMessage.ToString(), Is.EqualTo("Stub Message for NotEmpty"));
    }

    [Test]
    public void GetRemovablePropertyValidators_DomainObjectCollectionProperty ()
    {
      Assert.That(_domainObjectCollectionPropertyReflector.GetRemovablePropertyValidators().ToArray(), Is.Empty);
    }

    [Test]
    public void GetNonRemovablePropertyValidators_VirtualCollectionProperty ()
    {
      var notNullValidationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub(
              _ => _.CreateValidationMessageForPropertyValidator(
                  Arg<NotNullValidator>.Is.TypeOf,
                  Arg.Is(PropertyInfoAdapter.Create(_virtualCollectionProperty))))
          .Return(notNullValidationMessageStub);

      var notEmptyValidationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub(
              _ => _.CreateValidationMessageForPropertyValidator(
                  Arg<NotEmptyValidator>.Is.TypeOf,
                  Arg.Is(PropertyInfoAdapter.Create(_virtualCollectionProperty))))
          .Return(notEmptyValidationMessageStub);

      var result = _virtualCollectionPropertyReflector.GetNonRemovablePropertyValidators().ToArray();

      notNullValidationMessageStub.Stub(_ => _.ToString()).Return("Stub Message for NotNull");
      Assert.That(result.Count(), Is.EqualTo(2));
      Assert.That(result[0], Is.TypeOf(typeof (NotNullValidator)));
      Assert.That(((NotNullValidator) result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message for NotNull"));

      notEmptyValidationMessageStub.Stub(_ => _.ToString()).Return("Stub Message for NotEmpty");
      Assert.That(result[1], Is.TypeOf(typeof (NotEmptyValidator)));
      Assert.That(((NotEmptyValidator) result[1]).ValidationMessage.ToString(), Is.EqualTo("Stub Message for NotEmpty"));
    }

    [Test]
    public void GetRemovablePropertyValidators_VirtualCollectionProperty ()
    {
      Assert.That(_virtualCollectionPropertyReflector.GetRemovablePropertyValidators().ToArray(), Is.Empty);
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void GetRemovablePropertyValidators_DomainObjectCollectionPropertyAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void GetRemovablePropertyValidators_VirtualCollectionPropertyAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetNonRemovablePropertyValidators_NullableStringPropertyAttribute ()
    {
      var result = _propertyWithNullableStringPropertyAttributeReflector.GetNonRemovablePropertyValidators().ToArray();

      Assert.That(result.Any(), Is.False);
    }

    [Test]
    public void GetNonRemovablePropertyValidators_MandatoryStringPropertyAttribute ()
    {
      var validationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub(
              _ => _.CreateValidationMessageForPropertyValidator(
                  Arg<NotNullValidator>.Is.TypeOf,
                  Arg.Is(PropertyInfoAdapter.Create(_propertyWithMandatoryStringPropertyAttribute))))
          .Return(validationMessageStub);

      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetNonRemovablePropertyValidators().ToArray();

      validationMessageStub.Stub(_ => _.ToString()).Return("Stub Message");
      Assert.That(result.Count(), Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof (NotNullValidator)));
      Assert.That(((NotNullValidator) result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
    }

    [Test]
    public void GetRemovingPropertyRegistrations ()
    {
      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetRemovingValidatorRegistrations().ToArray();

      Assert.That(result.Any(), Is.False);
    }

    [Test]
    public void GetMetaValidationRules_MandatoryAttribute ()
    {
      var result = _propertyWithMandatoryAttributeReflector.GetMetaValidationRules().ToArray();

      Assert.That(result.Any(), Is.False);
    }

    [Test]
    public void GetMetaValidationRules_NullableStringPropertyAttribute ()
    {
      var result = _propertyWithNullableStringPropertyAttributeReflector.GetMetaValidationRules().ToArray();

      Assert.That(result.Count(), Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof (RemotionMaxLengthPropertyMetaValidationRule)));
      Assert.That(((RemotionMaxLengthPropertyMetaValidationRule) result[0]).MaxLength, Is.EqualTo(10));
    }

    [Test]
    public void Initialize_WithDerivedImplementationProperty_ThrowsArgumentException ()
    {
      var overriddenProperty = typeof (DerivedTypeWithDomainObjectAttributes).GetProperty("PropertyWithNullableStringPropertyAttribute");

      Assert.That(
          () => new DomainObjectAttributesBasedValidationPropertyRuleReflector(
              interfaceProperty: _propertyWithNullableStringPropertyAttribute,
              implementationProperty: overriddenProperty,
              domainModelConstraintProvider: _domainModelConstraintProvider,
              validationMessageFactory: _validationMessageFactoryStub),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The property 'PropertyWithNullableStringPropertyAttribute' was used from the overridden declaration on type "
              + "'Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain.DerivedTypeWithDomainObjectAttributes' "
              + "but only original declarations are supported.", "implementationProperty"));
    }
  }
}