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
  public class NotSupportedPropertyTypesIntegrationTests : ValidationIntegrationTestBase
  {
    //PropertyTypeIsSupportedValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The property type 'Object' is not supported. If you meant to declare a relation, 'Object' must be derived from 'DomainObject'. "
        + "For non-mapped properties, use the 'StorageClassNoneAttribute'.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPropertyTypes."
        + "PropertyTypeOfObjectWithoutStorageClassNone.ClassWithInvalidPropertyType\r\n"
        + "Property: InvalidProperty")]
    public void PropertyTypeOfObjectWithoutStorageClassNone ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeOfObjectWithoutStorageClassNone");
    }

    //CheckForTypeNotFoundClassDefinitionValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The relation property 'InvalidProperty' has return type 'DomainObject', which is not a part of the mapping. Relation properties must not point "
        +"to classes above the inheritance root.\r\n\r\n"
        +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPropertyTypes."
        +"PropertyTypeOfDomainObjectWithoutStorageClassNone.ClassWithInvalidPropertyType\r\n"
        + "Property: InvalidProperty")]
    public void PropertyTypeOfDomainObjectWithoutStorageClassNone ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeOfDomainObjectWithoutStorageClassNone");
    }

    //CheckForTypeNotFoundClassDefinitionValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The relation property 'InvalidProperty' has return type 'ClassAboveInheritanceRoot', which is not a part of the mapping. Relation properties "
        +"must not point to classes above the inheritance root.\r\n\r\n"
        +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPropertyTypes."
        +"PropertyTypeToDomainObjectAboveTheInheritanceRoot.InheritanceRootClass\r\n"
        + "Property: InvalidProperty")]
    public void PropertyTypeToDomainObjectAboveTheInheritanceRoot ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeToDomainObjectAboveTheInheritanceRoot");
    }

    //PropertyTypeIsSupportedValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
      "The property type 'ObjectList`1' is not supported. If you meant to declare a relation, 'ObjectList`1' must be derived from 'DomainObject'. "
      +"For non-mapped properties, use the 'StorageClassNoneAttribute'.\r\n\r\n"
      +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPropertyTypes."
      +"PropertyTypeOfObjectList_DomainObject.ClassWithInvalidProperty\r\n"
      +"Property: InvalidProperty1\r\n"
      +"----------\r\n"
      +"The property type 'ObjectList`1' is not supported. If you meant to declare a relation, 'ObjectList`1' must be derived from 'DomainObject'. "
      +"For non-mapped properties, use the 'StorageClassNoneAttribute'.\r\n\r\n"
      +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPropertyTypes."
      +"PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot.ClassWithInvalidProperty\r\n"
      +"Property: InvalidProperty")]
    public void PropertyTypeOfObjectList_DomainObject ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeOfObjectList_DomainObject");
    }

    //PropertyTypeIsSupportedValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
      "The property type 'ObjectList`1' is not supported. If you meant to declare a relation, 'ObjectList`1' must be derived from 'DomainObject'. "
      +"For non-mapped properties, use the 'StorageClassNoneAttribute'.\r\n\r\n"
      +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPropertyTypes."
      +"PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot.ClassWithInvalidProperty\r\n"
      +"Property: InvalidProperty")]
    public void PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot");
    }

    //PropertyTypeIsSupportedValidationRule
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
       "The property type 'ObjectList`1' is not supported. If you meant to declare a relation, 'ObjectList`1' must be derived from 'DomainObject'. "
       +"For non-mapped properties, use the 'StorageClassNoneAttribute'.\r\n\r\n"
       +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPropertyTypes."
       +"PropertyTypeOfObjectList_DerivedDomainObject_Unidirectional.ClassWithInvalidPropertyType\r\n"
       +"Property: InvalidProperty")]
    public void PropertyTypeOfObjectList_DerivedDomainObject_Unidirectional ()
    {
      ValidateMapping ("NotSupportedPropertyTypes.PropertyTypeOfObjectList_DerivedDomainObject_Unidirectional");
    }
  }
}