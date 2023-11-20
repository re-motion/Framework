using Moq;
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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;
using Remotion.Data.DomainObjects.Persistence.NonPersistent.Model;
using Remotion.Data.DomainObjects.Persistence.NonPersistent.Validation;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation;
using Remotion.Data.DomainObjects.UnitTests.Mapping.Validation;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.NonPersistent.Model.Validation
{
  [TestFixture]
  public class PropertyStorageClassIsSupportedByStorageProviderValidationRuleTest : ValidationRuleTestBase
  {
    private PropertyStorageClassIsSupportedByStorageProviderValidationRule _validationRule;
    private ClassDefinition _classDefinition;
    private Mock<IStorageEntityDefinition> _persistentStorageEntityDefinition;
    private IStorageEntityDefinition _nonPersistentStorageEntityDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new PropertyStorageClassIsSupportedByStorageProviderValidationRule();
      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(DerivedValidationDomainObjectClass));
      _persistentStorageEntityDefinition = new Mock<IStorageEntityDefinition>();
      _nonPersistentStorageEntityDefinition =
          new NonPersistentStorageEntity(new NonPersistentProviderDefinition("NonPersistent", new NonPersistentStorageObjectFactory()));
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
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));
      _classDefinition.SetStorageEntity(_nonPersistentStorageEntityDefinition);
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void PropertyWithStorageClassTransaction ()
    {
      var propertyDefinition = new PropertyDefinition(
          _classDefinition,
          PropertyInfoAdapter.Create(typeof(DerivedValidationDomainObjectClass).GetProperty("PropertyWithStorageClassTransaction")),
          "PropertyWithStorageClassTransaction",
          false,
          true,
          20,
          StorageClass.Transaction,
          null);
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));
      _classDefinition.SetStorageEntity(_nonPersistentStorageEntityDefinition);
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void PropertyWithStorageClassPersistent ()
    {
      var propertyDefinition = new PropertyDefinition(
          _classDefinition,
          PropertyInfoAdapter.Create(typeof(DerivedValidationDomainObjectClass).GetProperty("PropertyWithTypeObjectWithStorageClassPersistent")),
          "PropertyWithTypeObjectWithStorageClassPersistent",
          false,
          true,
          20,
          StorageClass.Persistent,
          null);
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));
      _classDefinition.SetStorageEntity(_persistentStorageEntityDefinition.Object);
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void PropertyWithStorageClassPersistent_StorageEntityDefinitionIsNotSet ()
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
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void PropertyWithStorageClassPersistent_UnsupportedStorageEntityDefinition ()
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
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));
      _classDefinition.SetStorageEntity(_nonPersistentStorageEntityDefinition);
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      var expectedMessage =
          "StorageClass.Persistent is not supported for properties of classes that belong to the 'NonPersistentProviderDefinition'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass\r\n"
          + "Property: PropertyWithTypeObjectWithStorageClassPersistent";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

  }
}
