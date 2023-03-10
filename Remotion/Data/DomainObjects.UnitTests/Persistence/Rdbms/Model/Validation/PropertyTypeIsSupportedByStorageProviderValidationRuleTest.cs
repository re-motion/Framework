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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation;
using Remotion.Data.DomainObjects.UnitTests.Mapping.Validation;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Validation
{
  [TestFixture]
  public class PropertyTypeIsSupportedByStorageProviderValidationRuleTest : ValidationRuleTestBase
  {
    private PropertyTypeIsSupportedByStorageProviderValidationRule _validationRule;
    private ClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new PropertyTypeIsSupportedByStorageProviderValidationRule();
      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(DerivedValidationDomainObjectClass));
    }

    [Test]
    public void PropertyWithStorageClassNone ()
    {
      var propertyDefinition = new PropertyDefinition(
          _classDefinition,
          PropertyInfoAdapter.Create(typeof(DerivedValidationDomainObjectClass).GetProperty("PropertyWithStorageClassNone")),
          "PropertyWithStorageClassNone",
          false,
          true,
          20,
          StorageClass.None,
          null);
      propertyDefinition.SetStorageProperty(SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("PropertyWithStorageClassNone"));
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void PropertyWithStorageClassPersistent_StoragePropertyIsNotSet ()
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
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void PropertyWithStorageClassPersistent_SupportedType ()
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
      propertyDefinition.SetStorageProperty(SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("PropertyWithStorageClassPersistent"));
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void PropertyWithStorageClassPersistent_UnsupportedType ()
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
      propertyDefinition.SetStorageProperty(new UnsupportedStoragePropertyDefinition(typeof(int), "Message", null));
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      var expectedMessage = "The property type 'Object' is not supported by this storage provider. Message\r\n\r\n"
        +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass\r\n"
        +"Property: PropertyWithTypeObjectWithStorageClassPersistent";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }
  }
}
