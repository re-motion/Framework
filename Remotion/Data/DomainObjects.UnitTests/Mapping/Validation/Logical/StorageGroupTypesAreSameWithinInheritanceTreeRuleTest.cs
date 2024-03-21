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
using Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Logical
{
  [TestFixture]
  public class StorageGroupTypesAreSameWithinInheritanceTreeRuleTest : ValidationRuleTestBase
  {
    private StorageGroupTypesAreSameWithinInheritanceTreeRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new StorageGroupTypesAreSameWithinInheritanceTreeRule();
    }

    [Test]
    public void ClassWithoutBaseClass ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(
          classType: typeof(BaseOfBaseValidationDomainObjectClass),
          storageGroupType: typeof(DBStorageGroupAttribute));
      var validationResult = _validationRule.Validate(typeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ClassWithBaseClass_ClassesWithoutStorageGroupAttribute ()
    {
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(BaseOfBaseValidationDomainObjectClass), storageGroupType: null);
      var derivedClassDefinition = TypeDefinitionObjectMother.CreateClassDefinition(
          classType: typeof(BaseOfBaseValidationDomainObjectClass),
          baseClass: baseClassDefinition,
          storageGroupType: null);

      var validationResult = _validationRule.Validate(derivedClassDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ClassWithBaseClass_ClassesWithSameStorageGroupAttribute ()
    {
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(
          classType: typeof(BaseOfBaseValidationDomainObjectClass),
          storageGroupType: typeof(DBStorageGroupAttribute));
      var derivedClassDefinition = TypeDefinitionObjectMother.CreateClassDefinition(
          classType: typeof(BaseOfBaseValidationDomainObjectClass),
          baseClass: baseClassDefinition,
          storageGroupType: typeof(DBStorageGroupAttribute));

      var validationResult = _validationRule.Validate(derivedClassDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ClassWithBaseClass_ClassesWithDifferentStorageGroupAttribute ()
    {
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(
          classType: typeof(BaseOfBaseValidationDomainObjectClass),
          storageGroupType: typeof(DBStorageGroupAttribute));
      var derivedClassDefinition = TypeDefinitionObjectMother.CreateClassDefinition(
          classType: typeof(BaseValidationDomainObjectClass),
          baseClass: baseClassDefinition,
          storageGroupType: typeof(StubStorageGroup1Attribute));

      var validationResult = _validationRule.Validate(derivedClassDefinition);

      var expectedMessage =
          "Class 'BaseValidationDomainObjectClass' must have the same storage group type as its base class 'BaseOfBaseValidationDomainObjectClass'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.BaseValidationDomainObjectClass";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void IgnoresArgumentsOfTypeOtherThanClassDefinition ()
    {
      var typeDefinitionForUnresolvedRelationPropertyType = new TypeDefinitionForUnresolvedRelationPropertyType(typeof(string), new NullPropertyInformation());

      Assert.That(() => _validationRule.Validate(typeDefinitionForUnresolvedRelationPropertyType),
          Throws.InvalidOperationException.With
              .Message.EqualTo("Only class definitions are supported"));
    }
  }
}
