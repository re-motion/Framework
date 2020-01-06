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
    private PropertyInfo _collectionProperty;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _collectionPropertyReflector;
    private IValidationMessageFactory _validationMessageFactoryStub;


    [SetUp]
    public void SetUp ()
    {
      _propertyWithoutAttribute = typeof (TypeWithDomainObjectAttributes).GetProperty ("PropertyWithoutAttribute");
      _propertyWithMandatoryAttribute = typeof (TypeWithDomainObjectAttributes).GetProperty ("PropertyWithMandatoryAttribute");
      _propertyWithNullableStringPropertyAttribute =
          typeof (TypeWithDomainObjectAttributes).GetProperty ("PropertyWithNullableStringPropertyAttribute");
      _propertyWithMandatoryStringPropertyAttribute =
          typeof (TypeWithDomainObjectAttributes).GetProperty ("PropertyWithMandatoryStringPropertyAttribute");
      _binaryProperty = typeof (TypeWithDomainObjectAttributes).GetProperty ("BinaryProperty");
      _collectionProperty = typeof (TypeWithDomainObjectAttributes).GetProperty ("CollectionProperty");

      var domainModelConstraintProvider = new DomainModelConstraintProvider();
      _validationMessageFactoryStub = MockRepository.GenerateStub<IValidationMessageFactory>();

      _propertyWithoutAttributeReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector (
          _propertyWithoutAttribute,
          _propertyWithoutAttribute,
          domainModelConstraintProvider,
          _validationMessageFactoryStub);
      _propertyWithMandatoryAttributeReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector (
          _propertyWithMandatoryAttribute,
          _propertyWithMandatoryAttribute,
          domainModelConstraintProvider,
          _validationMessageFactoryStub);
      _propertyWithNullableStringPropertyAttributeReflector =
          new DomainObjectAttributesBasedValidationPropertyRuleReflector (
              _propertyWithNullableStringPropertyAttribute,
              _propertyWithNullableStringPropertyAttribute,
              domainModelConstraintProvider,
              _validationMessageFactoryStub);
      _propertyWithMandatoryStringPropertyAttributeReflector =
          new DomainObjectAttributesBasedValidationPropertyRuleReflector (
              _propertyWithMandatoryStringPropertyAttribute,
              _propertyWithMandatoryStringPropertyAttribute,
              domainModelConstraintProvider,
              _validationMessageFactoryStub);
      _binaryPropertyReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector (
          _binaryProperty,
          _binaryProperty,
          domainModelConstraintProvider,
          _validationMessageFactoryStub);
      _collectionPropertyReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector (
          _collectionProperty,
          _collectionProperty,
          domainModelConstraintProvider,
          _validationMessageFactoryStub); 
    }

    [Test]
    public void NoAttributes ()
    {
      Assert.That (_propertyWithoutAttributeReflector.GetAddingPropertyValidators().Any(), Is.False);
      Assert.That (_propertyWithoutAttributeReflector.GetHardConstraintPropertyValidators().Any(), Is.False);
      Assert.That (_propertyWithoutAttributeReflector.GetRemovingPropertyRegistrations().Any(), Is.False);
      Assert.That (_propertyWithoutAttributeReflector.GetMetaValidationRules().Any(), Is.False);
    }

    [Test]
    public void GetHardConstraintPropertyValidators_MandatoryAttribute ()
    {
      var validationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub (
              _ => _.CreateValidationMessageForPropertyValidator (
                  typeof (NotNullValidator),
                  PropertyInfoAdapter.Create (_propertyWithMandatoryAttribute)))
          .Return (validationMessageStub);

      var result = _propertyWithMandatoryAttributeReflector.GetHardConstraintPropertyValidators().ToArray();

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (NotNullValidator)));
      Assert.That (((NotNullValidator) result[0]).ValidationMessage, Is.SameAs (validationMessageStub));
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void GetAddingPropertyValidators_MandatoryAttributeAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetAddingPropertyValidators_NullableStringPropertyAttribute ()
    {
      var validationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub (
              _ => _.CreateValidationMessageForPropertyValidator (
                  typeof (LengthValidator),
                  PropertyInfoAdapter.Create (_propertyWithNullableStringPropertyAttribute)))
          .Return (validationMessageStub);

      var result = _propertyWithNullableStringPropertyAttributeReflector.GetAddingPropertyValidators().ToArray();

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (LengthValidator)));
      Assert.That (((LengthValidator) result[0]).Max, Is.EqualTo (10));
      Assert.That (((LengthValidator) result[0]).ValidationMessage, Is.SameAs (validationMessageStub));
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void GetAddingPropertyValidators_NullableStringPropertyAttributeAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetAddingPropertyValidators_MandatoryStringPropertyAttribute ()
    {
      var lengthValidationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub (_ => _.CreateValidationMessageForPropertyValidator (typeof (LengthValidator),
              PropertyInfoAdapter.Create (_propertyWithMandatoryStringPropertyAttribute)))
          .Return (lengthValidationMessageStub);

      var notEmptyValidationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub (_ => _.CreateValidationMessageForPropertyValidator (typeof (NotEmptyValidator),
              PropertyInfoAdapter.Create (_propertyWithMandatoryStringPropertyAttribute)))
          .Return (notEmptyValidationMessageStub);

      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetAddingPropertyValidators().ToArray();

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result[0], Is.TypeOf (typeof (LengthValidator)));
      Assert.That (((LengthValidator) result[0]).Max, Is.EqualTo (20));
      Assert.That (((LengthValidator) result[0]).ValidationMessage, Is.SameAs (lengthValidationMessageStub));

      Assert.That (result[1], Is.TypeOf (typeof (NotEmptyValidator)));
      Assert.That (((NotEmptyValidator) result[1]).ValidationMessage, Is.SameAs (notEmptyValidationMessageStub));
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void GetAddingPropertyValidators_MandatoryStringPropertyAttributeAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetAddingPropertyValidators_BinaryProperty ()
    {
      var validationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub (
              _ => _.CreateValidationMessageForPropertyValidator (
                  typeof (NotEmptyValidator),
                  PropertyInfoAdapter.Create (_binaryProperty)))
          .Return (validationMessageStub);

      var result = _binaryPropertyReflector.GetAddingPropertyValidators().ToArray();

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (NotEmptyValidator)));
      Assert.That (((NotEmptyValidator) result[0]).ValidationMessage, Is.SameAs (validationMessageStub));
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void GetAddingPropertyValidators_BinaryPropertyAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetAddingPropertyValidators_CollectionProperty ()
    {
      var notNullValidationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub (
              _ => _.CreateValidationMessageForPropertyValidator (
                  typeof (NotNullValidator),
                  PropertyInfoAdapter.Create (_collectionProperty)))
          .Return (notNullValidationMessageStub);

      var notEmptyValidationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub (
              _ => _.CreateValidationMessageForPropertyValidator (
                  typeof (NotEmptyValidator),
                  PropertyInfoAdapter.Create (_collectionProperty)))
          .Return (notEmptyValidationMessageStub);

      var result = _collectionPropertyReflector.GetHardConstraintPropertyValidators ().ToArray ();

      Assert.That (result.Count (), Is.EqualTo (2));
      Assert.That (result[0], Is.TypeOf (typeof (NotNullValidator)));
      Assert.That (((NotNullValidator) result[0]).ValidationMessage, Is.SameAs (notNullValidationMessageStub));
      Assert.That (result[1], Is.TypeOf (typeof (NotEmptyValidator)));
      Assert.That (((NotEmptyValidator) result[1]).ValidationMessage, Is.SameAs (notEmptyValidationMessageStub));

      Assert.That (_collectionPropertyReflector.GetAddingPropertyValidators().ToArray(), Is.Empty);
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void GetAddingPropertyValidators_CollectionPropertyAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetHardConstraintPropertyValidators_NullableStringPropertyAttribute ()
    {
      var result = _propertyWithNullableStringPropertyAttributeReflector.GetHardConstraintPropertyValidators().ToArray();

      Assert.That (result.Any(), Is.False);
    }

    [Test]
    public void GetHardConstraintPropertyValidators_MandatoryStringPropertyAttribute ()
    {
      var validationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub (
              _ => _.CreateValidationMessageForPropertyValidator (
                  typeof (NotNullValidator),
                  PropertyInfoAdapter.Create (_propertyWithMandatoryStringPropertyAttribute)))
          .Return (validationMessageStub);

      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetHardConstraintPropertyValidators().ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (NotNullValidator)));
      Assert.That (((NotNullValidator) result[0]).ValidationMessage, Is.SameAs (validationMessageStub));
    }

    [Test]
    public void GetRemovingPropertyRegistrations ()
    {
      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetRemovingPropertyRegistrations().ToArray();

      Assert.That (result.Any(), Is.False);
    }

    [Test]
    public void GetMetaValidationRules_MandatoryAttribute ()
    {
      var result = _propertyWithMandatoryAttributeReflector.GetMetaValidationRules().ToArray();

      Assert.That (result.Any(), Is.False);
    }

    [Test]
    public void GetMetaValidationRules_NullableStringPropertyAttribute ()
    {
      var result = _propertyWithNullableStringPropertyAttributeReflector.GetMetaValidationRules().ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (RemotionMaxLengthMetaValidationRule)));
      Assert.That (((RemotionMaxLengthMetaValidationRule) result[0]).MaxLength, Is.EqualTo (10));
    }
  }
}