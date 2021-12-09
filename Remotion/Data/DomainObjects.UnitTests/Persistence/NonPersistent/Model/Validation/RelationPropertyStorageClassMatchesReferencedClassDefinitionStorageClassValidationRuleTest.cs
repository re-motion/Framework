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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;
using Remotion.Data.DomainObjects.Persistence.NonPersistent.Model;
using Remotion.Data.DomainObjects.Persistence.NonPersistent.Validation;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Data.DomainObjects.UnitTests.Mapping.Validation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.NonPersistent.Model.Validation
{
  [TestFixture]
  public class RelationPropertyStorageClassMatchesReferencedClassDefinitionStorageClassValidationRuleTest: ValidationRuleTestBase
  {
    private RelationPropertyStorageClassMatchesReferencedTypeDefinitionStorageClassValidationRule _validationRule;
    private TypeDefinition _persistentTypeDefinition;
    private TypeDefinition _nonPersistentTypeDefinition;
    private PropertyDefinition _persistentPropertyDefinition;
    private PropertyDefinition _transactionPropertyDefinitionOnPersistentClassDefinition;
    private PropertyDefinition _transactionPropertyDefinitionOnNonPersistentClassDefinition;

    private Mock<IStorageEntityDefinition> _persistentStorageEntityDefinition;
    private IStorageEntityDefinition _nonPersistentStorageEntityDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new RelationPropertyStorageClassMatchesReferencedTypeDefinitionStorageClassValidationRule();

      _persistentTypeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(
          "Order",
          typeof(Order),
          storageGroupType: typeof(TestDomainAttribute));
      _persistentStorageEntityDefinition = new Mock<IStorageEntityDefinition>();
      _persistentTypeDefinition.SetStorageEntity(_persistentStorageEntityDefinition.Object);

      _nonPersistentTypeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(
          "OrderViewModel",
          typeof(OrderViewModel),
          storageGroupType: typeof(NonPersistentTestDomainAttribute));
      _nonPersistentStorageEntityDefinition =
          new NonPersistentStorageEntity(new NonPersistentProviderDefinition("NonPersistent", new NonPersistentStorageObjectFactory()));
      _nonPersistentTypeDefinition.SetStorageEntity(_nonPersistentStorageEntityDefinition);

      _persistentPropertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(
          _persistentTypeDefinition,
          "PersistentProperty1",
          isObjectID: true,
          typeof(OrderViewModel),
          true,
          null,
          StorageClass.Persistent);

      _transactionPropertyDefinitionOnPersistentClassDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(
          _persistentTypeDefinition,
          "TransactionPropertyPersistentClassDefinition",
          isObjectID: true,
          typeof(OrderViewModel),
          true,
          null,
          StorageClass.Transaction);

      _transactionPropertyDefinitionOnNonPersistentClassDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(
          _nonPersistentTypeDefinition,
          "TransactionPropertyOnNonPersistentClassDefinition",
          isObjectID: true,
          typeof(OrderViewModel),
          true,
          null,
          StorageClass.Transaction);
    }

    [Test]
    public void RelationEndPointHasStorageClassTransaction_AndAnonymousEndPointHasPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_transactionPropertyDefinitionOnPersistentClassDefinition, false);
      var rightEndPointDefinition = new AnonymousRelationEndPointDefinition(_persistentTypeDefinition);
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { leftEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassTransaction_AndVirtualObjectRelationEndPointHasPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_transactionPropertyDefinitionOnPersistentClassDefinition, false);
      var rightEndPointDefinition = VirtualObjectRelationEndPointDefinitionFactory.Create(
          _persistentTypeDefinition,
          "Right",
          false,
          typeof(DomainObject));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(
          new RelationEndPointDefinitionCollection(new IRelationEndPointDefinition[] { leftEndPointDefinition, rightEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassTransaction_AndDomainObjectCollectionRelationEndPointDefinitionHasPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_transactionPropertyDefinitionOnPersistentClassDefinition, false);
      var rightEndPointDefinition = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
          _persistentTypeDefinition,
          "Right",
          false,
          typeof(ObjectList<DomainObject>));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(
          new RelationEndPointDefinitionCollection(new IRelationEndPointDefinition[] { leftEndPointDefinition, rightEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassTransaction_AndQueryCollectionRelationEndPointDefinitionHasPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_transactionPropertyDefinitionOnPersistentClassDefinition, false);
      var rightEndPointDefinition = VirtualCollectionRelationEndPointDefinitionFactory.Create(
          _persistentTypeDefinition,
          "Right",
          false,
          typeof(IObjectList<DomainObject>));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(
          new RelationEndPointDefinitionCollection(new IRelationEndPointDefinition[] { leftEndPointDefinition, rightEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassTransaction_AndAnonymousEndPointHasNonPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_transactionPropertyDefinitionOnPersistentClassDefinition, false);
      var rightEndPointDefinition = new AnonymousRelationEndPointDefinition(_nonPersistentTypeDefinition);
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { leftEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      _nonPersistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new IRelationEndPointDefinition[0], true));
      _nonPersistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassTransaction_AndVirtualObjectRelationEndPointHasNonPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_transactionPropertyDefinitionOnPersistentClassDefinition, false);
      var rightEndPointDefinition = VirtualObjectRelationEndPointDefinitionFactory.Create(
          _nonPersistentTypeDefinition,
          "Right",
          false,
          typeof(DomainObject));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new [] { leftEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      _nonPersistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new [] { rightEndPointDefinition }, true));
      _nonPersistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassTransaction_AndDomainObjectCollectionRelationEndPointDefinitionHasNonPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_transactionPropertyDefinitionOnPersistentClassDefinition, false);
      var rightEndPointDefinition = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
          _nonPersistentTypeDefinition,
          "Right",
          false,
          typeof(ObjectList<DomainObject>));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new [] { leftEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      _nonPersistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new [] { rightEndPointDefinition }, true));
      _nonPersistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassTransaction_AndVirtualCollectionRelationEndPointDefinitionHasNonPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_transactionPropertyDefinitionOnPersistentClassDefinition, false);
      var rightEndPointDefinition = VirtualCollectionRelationEndPointDefinitionFactory.Create(
          _nonPersistentTypeDefinition,
          "Right",
          false,
          typeof(IObjectList<DomainObject>));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new [] { leftEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      _nonPersistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new [] { rightEndPointDefinition }, true));
      _nonPersistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_AndAnonymousEndPointHasPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_persistentPropertyDefinition, false);
      var rightEndPointDefinition = new AnonymousRelationEndPointDefinition(_persistentTypeDefinition);
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { leftEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_AndVirtualObjectRelationEndPointHasPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_persistentPropertyDefinition, false);
      var rightEndPointDefinition = VirtualObjectRelationEndPointDefinitionFactory.Create(
          _persistentTypeDefinition,
          "Right",
          false,
          typeof(DomainObject));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(
          new RelationEndPointDefinitionCollection(new IRelationEndPointDefinition[] { leftEndPointDefinition, rightEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_AndDomainObjectCollectionRelationEndPointDefinitionHasPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_persistentPropertyDefinition, false);
      var rightEndPointDefinition = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
          _persistentTypeDefinition,
          "Right",
          false,
          typeof(ObjectList<DomainObject>));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(
          new RelationEndPointDefinitionCollection(new IRelationEndPointDefinition[] { leftEndPointDefinition, rightEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_AndVirtualCollectionRelationEndPointDefinitionHasPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_persistentPropertyDefinition, false);
      var rightEndPointDefinition = VirtualCollectionRelationEndPointDefinitionFactory.Create(
          _persistentTypeDefinition,
          "Right",
          false,
          typeof(IObjectList<DomainObject>));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(
          new RelationEndPointDefinitionCollection(new IRelationEndPointDefinition[] { leftEndPointDefinition, rightEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_RelationDefinitionIsNotSet ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_persistentPropertyDefinition, false);
      var rightEndPointDefinition = VirtualCollectionRelationEndPointDefinitionFactory.Create(
          _persistentTypeDefinition,
          "Right",
          false,
          typeof(IObjectList<DomainObject>));

      _persistentTypeDefinition.SetRelationEndPointDefinitions(
          new RelationEndPointDefinitionCollection(new IRelationEndPointDefinition[] { leftEndPointDefinition, rightEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_AndAnonymousEndPointHasNonPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_persistentPropertyDefinition, false);
      var rightEndPointDefinition = new AnonymousRelationEndPointDefinition(_nonPersistentTypeDefinition);
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { leftEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      _nonPersistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new IRelationEndPointDefinition[0], true));
      _nonPersistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      var expectedMessage =
          "The relation property is defined as persistent but the referenced type "
          + "'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderViewModel' is non-persistent. "
          + "Persistent relation properties may only reference persistent types.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: PersistentProperty1FakeProperty";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_AndVirtualObjectRelationEndPointHasNonPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_persistentPropertyDefinition, false);
      var rightEndPointDefinition = VirtualObjectRelationEndPointDefinitionFactory.Create(
          _nonPersistentTypeDefinition,
          "Right",
          false,
          typeof(DomainObject));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new [] { leftEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      _nonPersistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new [] { rightEndPointDefinition }, true));
      _nonPersistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      var expectedMessage =
          "The relation property is defined as persistent but the referenced type "
          + "'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderViewModel' is non-persistent. "
          + "Persistent relation properties may only reference persistent types.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: PersistentProperty1FakeProperty";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_AndDomainObjectCollectionRelationEndPointDefinitionHasNonPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_persistentPropertyDefinition, false);
      var rightEndPointDefinition = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
          _nonPersistentTypeDefinition,
          "Right",
          false,
          typeof(ObjectList<DomainObject>));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new [] { leftEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      _nonPersistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new [] { rightEndPointDefinition }, true));
      _nonPersistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      var expectedMessage =
          "The relation property is defined as persistent but the referenced type "
          + "'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderViewModel' is non-persistent. "
          + "Persistent relation properties may only reference persistent types.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: PersistentProperty1FakeProperty";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_AndVirtualCollectionRelationEndPointDefinitionHasNonPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_persistentPropertyDefinition, false);
      var rightEndPointDefinition = VirtualCollectionRelationEndPointDefinitionFactory.Create(
          _nonPersistentTypeDefinition,
          "Right",
          false,
          typeof(IObjectList<DomainObject>));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new [] { leftEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      _nonPersistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new [] { rightEndPointDefinition }, true));
      _nonPersistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      var expectedMessage =
          "The relation property is defined as persistent but the referenced type "
          + "'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderViewModel' is non-persistent. "
          + "Persistent relation properties may only reference persistent types.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: PersistentProperty1FakeProperty";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_AndSecondEndPointIsAlsoRealButHasNonPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition(_persistentPropertyDefinition, false);
      var rightEndPointDefinition = new RelationEndPointDefinition(_transactionPropertyDefinitionOnNonPersistentClassDefinition, false);
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition(relationDefinition);
      rightEndPointDefinition.SetRelationDefinition(relationDefinition);

      _persistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { leftEndPointDefinition }, true));
      _persistentTypeDefinition.SetReadOnly();

      _nonPersistentTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { rightEndPointDefinition }, true));
      _nonPersistentTypeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_persistentTypeDefinition);

      var expectedMessage =
          "The relation property is defined as persistent but the referenced type "
          + "'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderViewModel' is non-persistent. "
          + "Persistent relation properties may only reference persistent types.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: PersistentProperty1FakeProperty";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }
  }
}
