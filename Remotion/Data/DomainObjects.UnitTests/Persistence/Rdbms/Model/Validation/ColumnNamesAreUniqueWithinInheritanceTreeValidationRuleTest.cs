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
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation;
using Remotion.Data.DomainObjects.UnitTests.Mapping.Validation;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Validation
{
  [TestFixture]
  public class ColumnNamesAreUniqueWithinInheritanceTreeValidationRuleTest : ValidationRuleTestBase
  {
    private ColumnNamesAreUniqueWithinInheritanceTreeValidationRule _validationRule;
    private ClassDefinition _derivedBaseClass1;
    private ClassDefinition _derivedBaseClass2;
    private ClassDefinition _derivedClass;
    private ClassDefinition _baseOfBaseClass;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new ColumnNamesAreUniqueWithinInheritanceTreeValidationRule(new RdbmsPersistenceModelProvider());
      _baseOfBaseClass = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(BaseOfBaseValidationDomainObjectClass));
      _derivedBaseClass1 = ClassDefinitionObjectMother.CreateClassDefinitionWithDefaultProperties(classType: typeof(BaseValidationDomainObjectClass), baseClass: _baseOfBaseClass);
      _derivedBaseClass2 = ClassDefinitionObjectMother.CreateClassDefinitionWithDefaultProperties(classType: typeof(DerivedValidationDomainObjectClass), baseClass: _derivedBaseClass1);
      _derivedClass = ClassDefinitionObjectMother.CreateClassDefinitionWithDefaultProperties(classType: typeof(OtherDerivedValidationHierarchyClass), baseClass: _baseOfBaseClass);

      _baseOfBaseClass.SetDerivedClasses(new[] { _derivedBaseClass1, _derivedClass });
      _derivedBaseClass1.SetDerivedClasses(new[] { _derivedBaseClass2 });
      _derivedBaseClass2.SetDerivedClasses(new ClassDefinition[0]);
      _derivedClass.SetDerivedClasses(new ClassDefinition[0]);
    }

    [Test]
    public void NoInheritanceRoot ()
    {
      var validationResult = _validationRule.Validate(_derivedClass);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_NonPersistentPropertiesWithSameStorageSpecificPropertyName ()
    {
      var propertyDefinition1 = CreateNonPersistentPropertyDefinition(
          _derivedBaseClass2, "Test1", SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Column"));
      var propertyDefinition2 = CreatePersistentPropertyDefinition(
          _derivedBaseClass2, "Test2", SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Column"));

      _baseOfBaseClass.SetPropertyDefinitions(new PropertyDefinitionCollection());
      _derivedBaseClass1.SetPropertyDefinitions(new PropertyDefinitionCollection());
      _derivedBaseClass2.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{ propertyDefinition1, propertyDefinition2}, true));
      _derivedBaseClass2.SetReadOnly();

      var validationResult = _validationRule.Validate(_derivedBaseClass2);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesWithSameStorageSpecificPropertyNameInSameClass ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixinsWithDefaultProperties(typeof(DerivedValidationDomainObjectClass));
      var propertyDefinition1 = CreatePersistentPropertyDefinition(
          classDefinition,
          "Property",
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Property"));
      var propertyDefinition2 = CreatePersistentPropertyDefinition(
          classDefinition,
          "PropertyWithStorageClassPersistent",
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Property"));

      classDefinition.SetDerivedClasses(new ClassDefinition[0]);
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition1, propertyDefinition2}, true));
      classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(classDefinition);

      var expectedMessage =
          "Property 'PropertyWithStorageClassPersistentFakeProperty' of class 'DerivedValidationDomainObjectClass' must not define storage specific "
          + "name 'Property', because class 'DerivedValidationDomainObjectClass' in same inheritance hierarchy already defines property "
          + "'PropertyFakeProperty' with the same storage specific name.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass\r\n"
          + "Property: PropertyFakeProperty";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesWithDifferentStorageSpecificPropertyNameInSameClass ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixinsWithDefaultProperties(typeof(DerivedValidationDomainObjectClass));

      var propertyDefinition1 = CreatePersistentPropertyDefinition(
          classDefinition,
          "FirstName1",
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Property1"));
      var propertyDefinition2 = CreatePersistentPropertyDefinition(
          classDefinition,
          "FirstName2",
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Property2"));

      classDefinition.SetDerivedClasses(new ClassDefinition[0]);
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition1, propertyDefinition2}, true));
      classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_TwoPersistentPropertiesWithSameStorageSpecificPropertyNameInSameInheritanceHierarchieLevel ()
    {
      var propertyDefinition1 = CreatePersistentPropertyDefinition(
          _derivedBaseClass1,
          "FirstName1",
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Property"));
      var propertyDefinition2 = CreatePersistentPropertyDefinition(
          _derivedBaseClass2,
          "FirstName2",
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Property"));

      _baseOfBaseClass.SetPropertyDefinitions(new PropertyDefinitionCollection());
      _derivedBaseClass1.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{ propertyDefinition1}, true));
      _derivedBaseClass2.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition2}, true));
      _derivedClass.SetPropertyDefinitions(new PropertyDefinitionCollection());
      _derivedBaseClass1.SetReadOnly();
      _derivedBaseClass2.SetReadOnly();

      var validationResult = _validationRule.Validate(_baseOfBaseClass);

      var expectedMessage =
          "Property 'FirstName2FakeProperty' of class 'DerivedValidationDomainObjectClass' must not define storage specific name 'Property', because "
          + "class 'BaseValidationDomainObjectClass' in same inheritance hierarchy already defines property 'FirstName1FakeProperty' with the same "
          + "storage specific name.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.BaseValidationDomainObjectClass\r\n"
          + "Property: FirstName1FakeProperty";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void InheritanceRoot_ThreePersistentPropertiesWithSameStorageSpecificPropertyName ()
    {
      var propertyDefinition1 = CreatePersistentPropertyDefinition(
          _baseOfBaseClass,
          "FirstName1",
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Property"));
      var propertyDefinition2 = CreatePersistentPropertyDefinition(
          _derivedBaseClass1,
          "FirstName2",
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Property"));
      var propertyDefinition3 = CreatePersistentPropertyDefinition(
          _derivedClass,
          "FirstName3",
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Property"));

      _baseOfBaseClass.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition1 }, true));
      _derivedBaseClass1.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition2 }, true));
      _derivedBaseClass2.SetPropertyDefinitions(new PropertyDefinitionCollection());
      _derivedClass.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition3 }, true));
      _derivedBaseClass1.SetReadOnly();

      var validationResults = _validationRule.Validate(_baseOfBaseClass).ToArray();

      Assert.That(validationResults.Length, Is.EqualTo(2));

      var expectedMessage1 =
          "Property 'FirstName2FakeProperty' of class 'BaseValidationDomainObjectClass' must not define storage specific name 'Property', because "
          + "class 'BaseOfBaseValidationDomainObjectClass' in same inheritance hierarchy already defines property 'FirstName1FakeProperty' with the "
          + "same storage specific name.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.BaseOfBaseValidationDomainObjectClass\r\n"
          + "Property: FirstName1FakeProperty";
      var expectedMessage2 =
          "Property 'FirstName3FakeProperty' of class 'OtherDerivedValidationHierarchyClass' must not define storage specific name 'Property', "
          + "because class 'BaseOfBaseValidationDomainObjectClass' in same inheritance hierarchy already defines property 'FirstName1FakeProperty' "
          + "with the same storage specific name.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.BaseOfBaseValidationDomainObjectClass\r\n"
          + "Property: FirstName1FakeProperty";
      AssertMappingValidationResult(validationResults[0], false, expectedMessage1);
      AssertMappingValidationResult(validationResults[1], false, expectedMessage2);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesWithDifferentStorageSpecificPropertyNameInSameInheritanceHierarchieLevel ()
    {
      var propertyDefinition1 = CreatePersistentPropertyDefinition(
          _derivedBaseClass1,
          "FirstName1",
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Property1"));
      var propertyDefinition2 = CreatePersistentPropertyDefinition(
          _derivedBaseClass2,
          "FirstName2",
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Property2"));

      _baseOfBaseClass.SetPropertyDefinitions(new PropertyDefinitionCollection());
      _derivedBaseClass1.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition1}, true));
      _derivedBaseClass2.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition2}, true));
      _derivedClass.SetPropertyDefinitions(new PropertyDefinitionCollection());
      _derivedBaseClass1.SetReadOnly();
      _derivedBaseClass2.SetReadOnly();

      var validationResult = _validationRule.Validate(_baseOfBaseClass);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesWithSameStorageSpecificPropertyNameInDifferentLeavesOfInheritanceTree ()
    {
      var propertyDefinition1 = CreatePersistentPropertyDefinition(
          _derivedClass,
          "FirstName1",
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Property"));
      var propertyDefinition2 = CreatePersistentPropertyDefinition(
          _derivedBaseClass2,
          "FirstName2",
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Property"));

      _baseOfBaseClass.SetPropertyDefinitions(new PropertyDefinitionCollection());
      _derivedBaseClass1.SetPropertyDefinitions(new PropertyDefinitionCollection());
      _derivedBaseClass2.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition2 }, true));
      _derivedClass.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition1}, true));
      _derivedClass.SetReadOnly();
      _derivedBaseClass2.SetReadOnly();

      var validationResult = _validationRule.Validate(_baseOfBaseClass);

      var expectedMessage =
          "Property 'FirstName1FakeProperty' of class 'OtherDerivedValidationHierarchyClass' must not define storage specific name 'Property', "
          + "because class 'DerivedValidationDomainObjectClass' in same inheritance hierarchy already defines property 'FirstName2FakeProperty' with "
          + "the same storage specific name.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass\r\n"
          + "Property: FirstName2FakeProperty";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesWithDifferentStorageSpecificPropertyNameInDifferentLeavesOfInheritanceTree ()
    {
      var propertyDefinition1 = CreatePersistentPropertyDefinition(
          _derivedClass,
          "FirstName1",
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Property1"));
      var propertyDefinition2 = CreatePersistentPropertyDefinition(
          _derivedBaseClass2,
          "FirstName2",
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Property2"));

      _baseOfBaseClass.SetPropertyDefinitions(new PropertyDefinitionCollection());
      _derivedBaseClass1.SetPropertyDefinitions(new PropertyDefinitionCollection());
      _derivedClass.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition1}, true));
      _derivedBaseClass2.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition2}, true));
      _derivedClass.SetReadOnly();
      _derivedBaseClass2.SetReadOnly();

      var validationResult = _validationRule.Validate(_baseOfBaseClass);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_PersistentPropertiesFromSamePropertyInformation_InDifferentLeavesOfInheritanceTree ()
    {
      var propertyInformation = new NullPropertyInformation();
      var propertyDefinition1 = CreatePersistentPropertyDefinition(
          _derivedClass,
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Length"),
          propertyInformation);
      var propertyDefinition2 = CreatePersistentPropertyDefinition(
          _derivedBaseClass2,
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Length"),
          propertyInformation);

      _baseOfBaseClass.SetPropertyDefinitions(new PropertyDefinitionCollection());
      _derivedBaseClass1.SetPropertyDefinitions(new PropertyDefinitionCollection());
      _derivedClass.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition1 }, true));
      _derivedBaseClass2.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition2 }, true));
      _derivedClass.SetReadOnly();
      _derivedBaseClass2.SetReadOnly();

      var validationResult = _validationRule.Validate(_baseOfBaseClass);

      AssertMappingValidationResult(validationResult, true, null);
    }

    private PropertyDefinition CreatePersistentPropertyDefinition (ClassDefinition classDefinition, string propertyName, IStoragePropertyDefinition storagePropertyDefinition)
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(classDefinition, propertyName, StorageClass.Persistent);
      propertyDefinition.SetStorageProperty(storagePropertyDefinition);
      return propertyDefinition;
    }

    private PropertyDefinition CreatePersistentPropertyDefinition (
        ClassDefinition classDefinition, IStoragePropertyDefinition storagePropertyDefinition, IPropertyInformation propertyInformation)
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForPropertyInformation(classDefinition, StorageClass.Persistent, propertyInformation);
      propertyDefinition.SetStorageProperty(storagePropertyDefinition);
      return propertyDefinition;
    }

    private PropertyDefinition CreateNonPersistentPropertyDefinition (
        ClassDefinition classDefinition,
        string propertyName,
        IStoragePropertyDefinition storagePropertyDefinition)
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(classDefinition, propertyName, StorageClass.None);
      propertyDefinition.SetStorageProperty(storagePropertyDefinition);
      return propertyDefinition;
    }
  }
}
