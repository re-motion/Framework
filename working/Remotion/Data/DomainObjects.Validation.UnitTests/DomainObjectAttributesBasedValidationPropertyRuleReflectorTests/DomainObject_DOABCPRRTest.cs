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
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain;
using Remotion.Validation.MetaValidation.Rules.Custom;

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

      _propertyWithoutAttributeReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector (
          _propertyWithoutAttribute,
          _propertyWithoutAttribute);
      _propertyWithMandatoryAttributeReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector (
          _propertyWithMandatoryAttribute,
          _propertyWithMandatoryAttribute);
      _propertyWithNullableStringPropertyAttributeReflector =
          new DomainObjectAttributesBasedValidationPropertyRuleReflector (
              _propertyWithNullableStringPropertyAttribute,
              _propertyWithNullableStringPropertyAttribute);
      _propertyWithMandatoryStringPropertyAttributeReflector =
          new DomainObjectAttributesBasedValidationPropertyRuleReflector (
              _propertyWithMandatoryStringPropertyAttribute,
              _propertyWithMandatoryStringPropertyAttribute);
      _binaryPropertyReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector (
          _binaryProperty,
          _binaryProperty);
      _collectionPropertyReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector (
          _collectionProperty,
          _collectionProperty); 
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
      var result = _propertyWithMandatoryAttributeReflector.GetHardConstraintPropertyValidators().ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (NotNullValidator)));
    }

    [Test]
    public void GettAddingPropertyValidators_NullableStringPropertyAttribute ()
    {
      var result = _propertyWithNullableStringPropertyAttributeReflector.GetAddingPropertyValidators().ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (LengthValidator)));
      Assert.That (((LengthValidator) result[0]).Max, Is.EqualTo (10));
    }

    [Test]
    public void GettAddingPropertyValidators_MandatoryStringPropertyAttribute ()
    {
      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetAddingPropertyValidators().ToArray();

      Assert.That (result.Count(), Is.EqualTo (2));
      Assert.That (result[0], Is.TypeOf (typeof (LengthValidator)));
      Assert.That (((LengthValidator) result[0]).Max, Is.EqualTo (20));
      Assert.That (result[1], Is.TypeOf (typeof (NotEmptyValidator)));
    }

    [Test]
    public void GettAddingPropertyValidators_BinaryProperty ()
    {
      var result = _binaryPropertyReflector.GetAddingPropertyValidators ().ToArray ();

      Assert.That (result.Count (), Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (NotEmptyValidator)));
    }

    [Test]
    public void GettAddingPropertyValidators_CollectionProperty ()
    {
      var result = _collectionPropertyReflector.GetHardConstraintPropertyValidators ().ToArray ();

      Assert.That (result.Count (), Is.EqualTo (2));
      Assert.That (result[0], Is.TypeOf (typeof (NotNullValidator)));
      Assert.That (result[1], Is.TypeOf (typeof (NotEmptyValidator)));

      Assert.That (_collectionPropertyReflector.GetAddingPropertyValidators().ToArray(), Is.Empty);
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
      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetHardConstraintPropertyValidators().ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (NotNullValidator)));
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