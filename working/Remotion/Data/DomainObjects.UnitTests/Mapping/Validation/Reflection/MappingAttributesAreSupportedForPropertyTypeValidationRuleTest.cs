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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreSupportedForPropertyTypeValidationRule;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Reflection
{
  [TestFixture]
  public class MappingAttributesAreSupportedForPropertyTypeValidationRuleTest : ValidationRuleTestBase
  {
    private MappingAttributesAreSupportedForPropertyTypeValidationRule _validtionRule;
    private ClassDefinition _classDefinition;
    private Type _validType;
    private Type _invalidType;

    [SetUp]
    public void SetUp ()
    {
      _validtionRule = new MappingAttributesAreSupportedForPropertyTypeValidationRule();
      _validType = typeof (ClassWithValidPropertyAttributes);
      _invalidType = typeof (ClassWithInvalidPropertyAttributes);
      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: _validType);
    }

    [Test]
    public void ValidPropertyWithStringPropertyAttribute ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (_classDefinition, _validType, "StringProperty");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validtionRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InvalidPropertyWithStringPropertyAttribute ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (_classDefinition, _invalidType, "IntPropertyWithStringPropertyAttribute");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, false, 
        "The 'StringPropertyAttribute' may be only applied to properties of type 'String'.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
        +"MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        +"Property: IntPropertyWithStringPropertyAttribute");
    }

    [Test]
    public void ValidPropertyWithBinaryPropertyAttribute ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (_classDefinition, _validType, "BinaryProperty");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InvalidPropertyWithBinaryPropertyAttribute ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (_classDefinition, _invalidType, "BoolPropertyWithBinaryPropertyAttribute");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, false, "The 'BinaryPropertyAttribute' may be only applied to properties of type 'Byte[]'.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
        + "MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        +"Property: BoolPropertyWithBinaryPropertyAttribute");
    }

    [Test]
    public void ValidPropertyWithExtensibleEnumPropertyAttribute ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (_classDefinition, _validType, "ExtensibleEnumProperty");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InvalidPropertyWithExtensibleEnumPropertyAttribute ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (_classDefinition, _invalidType, "StringPropertyWithExtensibleEnumPropertyAttribute");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, false, "The 'ExtensibleEnumPropertyAttribute' may be only applied to properties of type 'IExtensibleEnum'.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
        +"MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        + "Property: StringPropertyWithExtensibleEnumPropertyAttribute");
    }

    [Test]
    public void ValidPropertyWithMandatoryPropertyAttribute ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (_classDefinition, _validType, "MandatoryProperty");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InvalidPropertyWithMandatoryPropertyAttribute ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (_classDefinition, _invalidType, "StringPropertyWithMandatoryPropertyAttribute");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, false, 
        "The 'MandatoryAttribute' may be only applied to properties assignable to types 'DomainObject' or 'ObjectList`1'.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
        +"MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        + "Property: StringPropertyWithMandatoryPropertyAttribute");
    }

    [Test]
    public void InvalidPropertyWithBidirectionalRelationAttribute ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (
          _classDefinition, 
          _invalidType, 
          "StringPropertyWithMandatoryPropertyAttribute");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, false,
        "The 'MandatoryAttribute' may be only applied to properties assignable to types 'DomainObject' or 'ObjectList`1'.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
        +"MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        + "Property: StringPropertyWithMandatoryPropertyAttribute");
    }

    [Test]
    public void ValidValidPropertyWithBidirectionalRelationAttribute ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (
          _classDefinition,
          _validType,
          "BidirectionalRelationProperty");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InvalidValidPropertyWithBidirectionalRelationAttribute ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (_classDefinition, _invalidType, "StringPropertyWithBidirectionalRelationAttribute");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, false,
        "The 'DBBidirectionalRelationAttribute' may be only applied to properties assignable to types 'DomainObject' or 'ObjectList`1'.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
        +"MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        + "Property: StringPropertyWithBidirectionalRelationAttribute");
    }

    [Test]
    public void SeveralInvalidProperties ()
    {
      var propertyDefinition1 = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (_classDefinition, _invalidType, "StringPropertyWithMandatoryPropertyAttribute");
      var propertyDefinition2 = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (_classDefinition, _invalidType, "StringPropertyWithExtensibleEnumPropertyAttribute");
      var propertyDefinition3 = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (_classDefinition, _invalidType, "BoolPropertyWithBinaryPropertyAttribute");

      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition1, propertyDefinition2, propertyDefinition3}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validtionRule.Validate (_classDefinition).ToArray();

      var expectedMessage1 = "The 'MandatoryAttribute' may be only applied to properties assignable to types 'DomainObject' or 'ObjectList`1'.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
        +"MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        + "Property: StringPropertyWithMandatoryPropertyAttribute";
      var expectedMessage2 = "The 'ExtensibleEnumPropertyAttribute' may be only applied to properties of type 'IExtensibleEnum'.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
        + "MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        + "Property: StringPropertyWithExtensibleEnumPropertyAttribute";
      var expectedMessage3 = "The 'BinaryPropertyAttribute' may be only applied to properties of type 'Byte[]'.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
        + "MappingAttributesAreSupportedForPropertyTypeValidationRule.ClassWithInvalidPropertyAttributes\r\n"
        + "Property: BoolPropertyWithBinaryPropertyAttribute";
      Assert.That (validationResult.Length, Is.EqualTo (3));
      AssertMappingValidationResult (validationResult[0], false, expectedMessage1);
      AssertMappingValidationResult (validationResult[1], false, expectedMessage2);
      AssertMappingValidationResult (validationResult[2], false, expectedMessage3);
    }
  }
}