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
    [ExpectedException(typeof(MappingException), ExpectedMessage =
      "Property 'DuplicatedPropertyInTree' of class 'DerivedDerivedClass' must not define storage specific name 'DuplicatedPropertyInTree', "
      +"because class 'BaseClass' in same inheritance hierarchy already defines property 'DuplicatedPropertyInTree' with the same storage "
      +"specific name.\r\n\r\n"
      + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPersistenceMapping."
      + "SamePropertyNameInInheritanceHierarchy.BaseClass\r\n"
      + "Property: DuplicatedPropertyInTree")]
    public void SamePropertyNameInInheritanceHierarchy ()
    {
      ValidateMapping ("NotSupportedPersistenceMapping.SamePropertyNameInInheritanceHierarchy");
    }

    //Exception is thrown in ClassDefinitionCollection
    [Test]
    [ExpectedException (typeof (MappingException), 
      ExpectedMessage = @"Class 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration."
      +@"NotSupportedPersistenceMapping.SameClassNameInInheritanceHierarchy.BaseClass' and 'Remotion.Data.DomainObjects.UnitTests.Mapping."
      +@"TestDomain.Validation.Integration.NotSupportedPersistenceMapping.SameClassNameInInheritanceHierarchy.DerivedDerivedClass' both have "
      +@"the same class ID 'SameClassNameInInheritanceHierarchy_DuplicatedClassName'\. Use the ClassIDAttribute to define unique IDs for these classes\. "
      +@"The assemblies involved are 'Remotion.Data.DomainObjects.UnitTests, Version=.*, Culture=.*, PublicKeyToken=.*' and "
      +@"'Remotion.Data.DomainObjects.UnitTests, Version=.*, Culture=.*, PublicKeyToken=.*'\.", MatchType = MessageMatch.Regex)]
    public void SameClassNameInInheritanceHierarchy ()
    {
      ValidateMapping ("NotSupportedPersistenceMapping.SameClassNameInInheritanceHierarchy");
    }

    //StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), 
      ExpectedMessage = "The domain object type cannot redefine the 'StorageGroupAttribute' already defined on base type 'BaseClass'.\r\n\r\n"
      + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPersistenceMapping."
      +"DuplicatedStorageGroupAttributeInInheritanceHierarchy.DerivedDerivedClass")]
    public void DuplicatedStorageGroupAttributeInInheritanceHierarchy ()
    {
      ValidateMapping ("NotSupportedPersistenceMapping.DuplicatedStorageGroupAttributeInInheritanceHierarchy");
    }

    //ClassAboveTableIsAbstractValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
      "Neither class 'ClassAboveInheritanceRoot' nor its base classes are mapped to a table. Make class 'ClassAboveInheritanceRoot' abstract or define "
      +"a table for it or one of its base classes.\r\n\r\n"
      +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPersistenceMapping."
      +"ConcreteClassAboveInheritanceRoot.ClassAboveInheritanceRoot")]
    public void ConcreteClassAboveInheritanceRoot ()
    {
      ValidateMapping ("NotSupportedPersistenceMapping.ConcreteClassAboveInheritanceRoot");
    }

    //MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), 
      ExpectedMessage = "The 'StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n\r\n"
      + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPersistenceMapping."
      +"MappingAttributeAppliedOnOverriddenProperty.DerivedClass\r\nProperty: Property")]
    public void MappingAttributeAppliedOnOverriddenProperty ()
    {
      ValidateMapping ("NotSupportedPersistenceMapping.MappingAttributeAppliedOnOverriddenProperty");
    }

    //OnlyOneTablePerHierarchyValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
      "Class 'DerivedClass' must not define a table when its base class 'BaseClass' also defines one.\r\n\r\n"
      +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPersistenceMapping."
      +"SameEntityNamesInInheritanceHierarchy.DerivedClass")]
    public void SameEntityNamesInInheritanceHierarchy ()
    {
      ValidateMapping ("NotSupportedPersistenceMapping.SameEntityNamesInInheritanceHierarchy");
    }
  
  }
}