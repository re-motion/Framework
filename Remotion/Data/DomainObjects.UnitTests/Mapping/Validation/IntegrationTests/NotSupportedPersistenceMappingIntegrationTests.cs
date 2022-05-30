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

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.IntegrationTests
{
  [TestFixture]
  public class NotSupportedPersistenceMappingIntegrationTests : ValidationIntegrationTestBase
  {
    //ColumnNamesAreUniqueWithinInheritanceTreeValidationRule
    [Test]
    public void SamePropertyNameInInheritanceHierarchy ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedPersistenceMapping.SamePropertyNameInInheritanceHierarchy"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Property 'DuplicatedPropertyInTree' of class 'DerivedDerivedClass' must not define storage specific name 'DuplicatedPropertyInTree', "
                  +"because class 'BaseClass' in same inheritance hierarchy already defines property 'DuplicatedPropertyInTree' with the same storage "
                  +"specific name.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPersistenceMapping."
                  + "SamePropertyNameInInheritanceHierarchy.BaseClass\r\n"
                  + "Property: DuplicatedPropertyInTree"));
    }

    //Exception is thrown in ClassDefinitionCollection
    [Test]
    public void SameClassNameInInheritanceHierarchy ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedPersistenceMapping.SameClassNameInInheritanceHierarchy"),
          Throws.InstanceOf<MappingException>()
              .With.Message.Matches(
                  @"Class 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration."
                  +@"NotSupportedPersistenceMapping.SameClassNameInInheritanceHierarchy.BaseClass' and 'Remotion.Data.DomainObjects.UnitTests.Mapping."
                  +@"TestDomain.Validation.Integration.NotSupportedPersistenceMapping.SameClassNameInInheritanceHierarchy.DerivedDerivedClass' both have "
                  +@"the same class ID 'SameClassNameInInheritanceHierarchy_DuplicatedClassName'\. Use the ClassIDAttribute to define unique IDs for these classes\. "
                  +@"The assemblies involved are 'Remotion.Data.DomainObjects.UnitTests, Version=.*, Culture=.*, PublicKeyToken=.*' and "
                  +@"'Remotion.Data.DomainObjects.UnitTests, Version=.*, Culture=.*, PublicKeyToken=.*'\."));
    }

    //StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule
    [Test]
    public void DuplicatedStorageGroupAttributeInInheritanceHierarchy ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedPersistenceMapping.DuplicatedStorageGroupAttributeInInheritanceHierarchy"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The domain object type cannot redefine the 'StorageGroupAttribute' already defined on base type 'BaseClass'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPersistenceMapping."
                  +"DuplicatedStorageGroupAttributeInInheritanceHierarchy.DerivedDerivedClass"));
    }

    //ClassAboveTableIsAbstractValidationRule
    [Test]
    public void ConcreteClassAboveInheritanceRoot ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedPersistenceMapping.ConcreteClassAboveInheritanceRoot"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Neither class 'ClassAboveInheritanceRoot' nor its base classes are mapped to a table. Make class 'ClassAboveInheritanceRoot' abstract or define "
                  +"a table for it or one of its base classes.\r\n\r\n"
                  +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPersistenceMapping."
                  +"ConcreteClassAboveInheritanceRoot.ClassAboveInheritanceRoot"));
    }

    //MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule
    [Test]
    public void MappingAttributeAppliedOnOverriddenProperty ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedPersistenceMapping.MappingAttributeAppliedOnOverriddenProperty"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The 'StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPersistenceMapping."
                  +"MappingAttributeAppliedOnOverriddenProperty.DerivedClass\r\nProperty: Property"));
    }

    //OnlyOneTablePerHierarchyValidationRule
    [Test]
    public void SameEntityNamesInInheritanceHierarchy ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedPersistenceMapping.SameEntityNamesInInheritanceHierarchy"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Class 'DerivedClass' must not define a table when its base class 'BaseClass' also defines one.\r\n\r\n"
                  +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPersistenceMapping."
                  +"SameEntityNamesInInheritanceHierarchy.DerivedClass"));
    }

    //PropertyStorageClassIsSupportedByStorageProviderValidationRule
    [Test]
    public void PropertyWithStorageClassPersistentOnNonPersistentDomainObject ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedPersistenceMapping.PropertyWithUnsupportedStorageClass"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "StorageClass.Persistent is not supported for properties of classes that belong to the 'NonPersistentProviderDefinition'.\r\n\r\n"
                  +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPersistenceMapping."
                  +"PropertyWithUnsupportedStorageClass.NonPersistentDomainObjectWithPersistentProperty\r\n"
                  +"Property: PersistentProperty"));
    }

    // RelationPropertyStorageClassMatchesReferencedClassDefinitionStorageClassValidationRule
    [Test]
    public void PropertyWithStorageClassPersistentReferencesNonPersistentDomainObject ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedPersistenceMapping.PropertyWithStorageClassPersistentReferencesNonPersistentDomainObject"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The relation property is defined as persistent but the referenced type "
                  + "'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPersistenceMapping."
                  + "PropertyWithStorageClassPersistentReferencesNonPersistentDomainObject.NonPersistentRelationClass' is non-persistent. "
                  + "Persistent relation properties may only reference persistent types.\r\n\r\n"
                  +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPersistenceMapping."
                  +"PropertyWithStorageClassPersistentReferencesNonPersistentDomainObject.PersistentRelationClass\r\n"
                  +"Property: PersistentRelationProperty"));
    }
  }
}
