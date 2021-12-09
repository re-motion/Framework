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
    public void PropertyTypeOfObjectWithoutStorageClassNone ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedPropertyTypes.PropertyTypeOfObjectWithoutStorageClassNone"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The property type 'Object' is not supported. If you meant to declare a relation, 'Object' must be derived from 'DomainObject'. "
                  + "For non-mapped properties, use the 'StorageClassNoneAttribute'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPropertyTypes."
                  + "PropertyTypeOfObjectWithoutStorageClassNone.ClassWithInvalidPropertyType\r\n"
                  + "Property: InvalidProperty"));
    }

    //CheckForTypeNotFoundClassDefinitionValidationRule
    [Test]
    public void PropertyTypeOfDomainObjectWithoutStorageClassNone ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedPropertyTypes.PropertyTypeOfDomainObjectWithoutStorageClassNone"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The relation property 'InvalidProperty' has return type 'DomainObject', which is not a part of the mapping. Relation properties must not point "
                  +"to types above the inheritance root.\r\n\r\n"
                  +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPropertyTypes."
                  +"PropertyTypeOfDomainObjectWithoutStorageClassNone.ClassWithInvalidPropertyType\r\n"
                  + "Property: InvalidProperty"));
    }

    //CheckForTypeNotFoundClassDefinitionValidationRule
    [Test]
    public void PropertyTypeToDomainObjectAboveTheInheritanceRoot ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedPropertyTypes.PropertyTypeToDomainObjectAboveTheInheritanceRoot"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The relation property 'InvalidProperty' has return type 'ClassAboveInheritanceRoot', which is not a part of the mapping. Relation properties "
                  +"must not point to types above the inheritance root.\r\n\r\n"
                  +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPropertyTypes."
                  +"PropertyTypeToDomainObjectAboveTheInheritanceRoot.InheritanceRootClass\r\n"
                  + "Property: InvalidProperty"));
    }

    //PropertyTypeIsSupportedValidationRule
    [Test]
    public void PropertyTypeOfObjectList_DomainObject ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedPropertyTypes.PropertyTypeOfObjectList_DomainObject"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
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
                  +"Property: InvalidProperty"));
    }

    //PropertyTypeIsSupportedValidationRule
    [Test]
    public void PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedPropertyTypes.PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The property type 'ObjectList`1' is not supported. If you meant to declare a relation, 'ObjectList`1' must be derived from 'DomainObject'. "
                  +"For non-mapped properties, use the 'StorageClassNoneAttribute'.\r\n\r\n"
                  +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPropertyTypes."
                  +"PropertyTypeOfObjectList_DomainObjectAboveInheritanceRoot.ClassWithInvalidProperty\r\n"
                  +"Property: InvalidProperty"));
    }

    //PropertyTypeIsSupportedValidationRule
    [Test]
    public void PropertyTypeOfObjectList_DerivedDomainObject_Unidirectional ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedPropertyTypes.PropertyTypeOfObjectList_DerivedDomainObject_Unidirectional"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The property type 'ObjectList`1' is not supported. If you meant to declare a relation, 'ObjectList`1' must be derived from 'DomainObject'. "
                  +"For non-mapped properties, use the 'StorageClassNoneAttribute'.\r\n\r\n"
                  +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPropertyTypes."
                  +"PropertyTypeOfObjectList_DerivedDomainObject_Unidirectional.ClassWithInvalidPropertyType\r\n"
                  +"Property: InvalidProperty"));
    }

    //MandatoryExtensibleEnumTypeHasValuesDefinedValidationRule
    [Test]
    public void PropertyTypeOfExtensibleEnum_WithoutValues_Mandatory ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedPropertyTypes.PropertyTypeOfExtensibleEnum_WithoutValues_Mandatory"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Extensible enum type 'ExtensibleEnumNotDefiningAnyValues' cannot be used for property 'InvalidProperty' on type 'ClassWithInvalidPropertyType' "
                  +"because the property is mandatory but there are not values defined for the enum type.\r\n\r\n"
                  +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPropertyTypes."
                  +"PropertyTypeOfExtensibleEnum_WithoutValues_Mandatory.ClassWithInvalidPropertyType\r\n"
                  +"Property: InvalidProperty"));
    }

    //MandatoryNetEnumTypeHasValuesDefinedValidationRule
    [Test]
    public void PropertyTypeOfNetEnum_WithoutValues_Mandatory ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedPropertyTypes.PropertyTypeOfNetEnum_WithoutValues_Mandatory"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Enum type 'NetEnumNotDefiningAnyValues' cannot be used for property 'InvalidProperty' on type 'ClassWithInvalidPropertyType' "
                  +"because the property is mandatory but there are not values defined for the enum type.\r\n\r\n"
                  +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPropertyTypes."
                  +"PropertyTypeOfNetEnum_WithoutValues_Mandatory.ClassWithInvalidPropertyType\r\n"
                  +"Property: InvalidProperty"));
    }
  }
}
