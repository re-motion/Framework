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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Logical
{
  [TestFixture]
  public class PropertyTypeIsSupportedValidationRuleTest : ValidationRuleTestBase
  {
    private PropertyTypeIsSupportedValidationRule _validationRule;
    private ClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new PropertyTypeIsSupportedValidationRule();

      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(DerivedValidationDomainObjectClass));
    }

    [Test]
    public void NoRelationProperty_SupportedType ()
    {
      var propertyDefinition = new PropertyDefinition(
          _classDefinition,
          PropertyInfoAdapter.Create(typeof(DerivedValidationDomainObjectClass).GetProperty("PropertyWithStorageClassPersistent")),
          "PropertyWithStorageClassPersistent",
          false,
          true,
          20,
          StorageClass.Persistent,
          null);
      propertyDefinition.SetStorageProperty(new FakeStoragePropertyDefinition("PropertyWithStorageClassPersistent"));
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void NoRelationProperty_UnsupportedType ()
    {
      var propertyDefinition = new PropertyDefinition(
          _classDefinition,
          PropertyInfoAdapter.Create(typeof(DerivedValidationDomainObjectClass).GetProperty("PropertyWithTypeObjectWithStorageClassPersistent")),
          "PropertyWithTypeObjectWithStorageClassPersistent",
          false,
          true,
          null,
          StorageClass.Persistent,
          null);
      propertyDefinition.SetStorageProperty(new FakeStoragePropertyDefinition("PropertyWithTypeObjectWithStorageClassPersistent"));
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      var expectedMessage =
          "The property type 'Object' is not supported. If you meant to declare a relation, 'Object' must be derived from 'DomainObject'. "
          + "For non-mapped properties, use the 'StorageClassNoneAttribute'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass\r\n"
          + "Property: PropertyWithTypeObjectWithStorageClassPersistent";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void RelationProperty ()
    {
      var propertyDefinition = new PropertyDefinition(
          _classDefinition,
          PropertyInfoAdapter.Create(typeof(DerivedValidationDomainObjectClass).GetProperty("RelationPropertyWithStorageClassPersistent")),
          "RelationPropertyWithStorageClassPersistent",
          true,
          true,
          null,
          StorageClass.Persistent,
          null);
      propertyDefinition.SetStorageProperty(new FakeStoragePropertyDefinition("RelationPropertyWithStorageClassPersistent"));
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }
  }
}
