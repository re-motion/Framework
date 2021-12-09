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
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Reflection
{
  [TestFixture]
  public class StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRuleTest : ValidationRuleTestBase
  {
    private StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule();
    }

    [Test]
    public void NotInheritanceRoot ()
    {
      var type = typeof(DerivedClassWithoutStorageGroupAttribute);
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(type);

      var validationResult = _validationRule.Validate(typeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_WithoutStorageGroupAttribute ()
    {
      var type = typeof(BaseClassWithoutStorageGroupAttribute);
      var classDefinition = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(type);

      var validationResult = _validationRule.Validate(classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_WithStorageGroupAttribute_And_WithoutStorageGroupAttributeOnBaseClass ()
    {
      var type = typeof(BaseClassWithStorageGroupAttribute);
      var classDefinition = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(type);

      var validationResult = _validationRule.Validate(classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_WithoutBaseClass ()
    {
      var classDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(object));

      var validationResult = _validationRule.Validate(classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_WithStorageGroupAttribute_And_WithStorageGroupAttributeOnBaseClass ()
    {
      var type = typeof(DerivedClassWithStorageGroupAttribute);
      var classDefinition = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(type);

      var validationResult = _validationRule.Validate(classDefinition);

      string message =
          "The domain object type cannot redefine the 'StorageGroupAttribute' already defined on base type 'BaseClassWithStorageGroupAttribute'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
          + "StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule.DerivedClassWithStorageGroupAttribute";
      AssertMappingValidationResult(validationResult, false, message);
    }

    [Test]
    public void InheritanceRoot_WithStorageGroupAttribute_And_WithStorageGroupAttributeOnBaseClass_And_BaseClassIsNotDomainObject ()
    {
      var type = typeof(DerivedClassWithStorageGroupAttributeAndNonDomainObjectBaseTypeWithStorageGroupAttribute);
      var classDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: type);

      var validationResult = _validationRule.Validate(classDefinition);

      string message =
          "The domain object type cannot redefine the 'StorageGroupAttribute' already defined on base type 'NonDomainObjectBaseTypeWithStorageGroupAttribute'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
          + "StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule.DerivedClassWithStorageGroupAttributeAndNonDomainObjectBaseTypeWithStorageGroupAttribute";
      AssertMappingValidationResult(validationResult, false, message);
    }
  }
}
