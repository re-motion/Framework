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
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;
using Remotion.Data.DomainObjects.Persistence.NonPersistent.Model;
using Remotion.Data.DomainObjects.Persistence.NonPersistent.Validation;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Data.DomainObjects.UnitTests.Mapping.Validation;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.NonPersistent.Model.Validation
{
  [TestFixture]
  public class RelationPropertyStorageClassMatchesReferencedClassDefinitionStorageClassValidationRuleTest: ValidationRuleTestBase
  {
    private RelationPropertyStorageClassMatchesReferencedClassDefinitionStorageClassValidationRule _validationRule;
    private ClassDefinition _persistentClassDefinition;
    private ClassDefinition _nonPersistentClassDefinition;
    private PropertyDefinition _persistentPropertyDefinition;
    private PropertyDefinition _transactionPropertyDefinitionOnPersistentClassDefinition;
    private PropertyDefinition _transactionPropertyDefinitionOnNonPersistentClassDefinition;

    private IStorageEntityDefinition _persistentStorageEntityDefinition;
    private IStorageEntityDefinition _nonPersistentStorageEntityDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new RelationPropertyStorageClassMatchesReferencedClassDefinitionStorageClassValidationRule();

      _persistentClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition (
          "Order",
          typeof (Order),
          storageGroupType: typeof (TestDomainAttribute));
      _persistentStorageEntityDefinition = MockRepository.GenerateStub<IStorageEntityDefinition>();
      _persistentClassDefinition.SetStorageEntity (_persistentStorageEntityDefinition);

      _nonPersistentClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition (
          "OrderViewModel",
          typeof (OrderViewModel),
          storageGroupType: typeof (NonPersistentTestDomainAttribute));
      _nonPersistentStorageEntityDefinition =
          new NonPersistentStorageEntity(new NonPersistentProviderDefinition("NonPersistent", new NonPersistentStorageObjectFactory()));
      _nonPersistentClassDefinition.SetStorageEntity (_nonPersistentStorageEntityDefinition);

      _persistentPropertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (
          _persistentClassDefinition,
          "PersistentProperty1",
          isObjectID: true,
          typeof (OrderViewModel),
          true,
          null,
          StorageClass.Persistent);

      _transactionPropertyDefinitionOnPersistentClassDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (
          _persistentClassDefinition,
          "TransactionPropertyPersistentClassDefinition",
          isObjectID: true,
          typeof (OrderViewModel),
          true,
          null,
          StorageClass.Transaction);

      _transactionPropertyDefinitionOnNonPersistentClassDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (
          _nonPersistentClassDefinition,
          "TransactionPropertyOnNonPersistentClassDefinition",
          isObjectID: true,
          typeof (OrderViewModel),
          true,
          null,
          StorageClass.Transaction);
    }

    [Test]
    public void RelationEndPointHasStorageClassTransaction_AndAnonymousEndPointHasPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition (_transactionPropertyDefinitionOnPersistentClassDefinition, false);
      var rightEndPointDefinition = new AnonymousRelationEndPointDefinition (_persistentClassDefinition);
      var relationDefinition = new RelationDefinition ("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition (relationDefinition);
      rightEndPointDefinition.SetRelationDefinition (relationDefinition);

      _persistentClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { leftEndPointDefinition }, true));
      _persistentClassDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_persistentClassDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassTransaction_AndVirtualRelationEndPointWithCardinalityOneHasPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition (_transactionPropertyDefinitionOnPersistentClassDefinition, false);
      var rightEndPointDefinition = VirtualRelationEndPointDefinitionFactory.Create (
          _persistentClassDefinition,
          "Right",
          false,
          CardinalityType.One,
          typeof (DomainObject));
      var relationDefinition = new RelationDefinition ("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition (relationDefinition);
      rightEndPointDefinition.SetRelationDefinition (relationDefinition);

      _persistentClassDefinition.SetRelationEndPointDefinitions (
          new RelationEndPointDefinitionCollection (new IRelationEndPointDefinition[] { leftEndPointDefinition, rightEndPointDefinition }, true));
      _persistentClassDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_persistentClassDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassTransaction_AndVirtualRelationEndPointWithCardinalityManyHasPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition (_transactionPropertyDefinitionOnPersistentClassDefinition, false);
      var rightEndPointDefinition = VirtualRelationEndPointDefinitionFactory.Create (
          _persistentClassDefinition,
          "Right",
          false,
          CardinalityType.Many,
          typeof (ObjectList<DomainObject>));
      var relationDefinition = new RelationDefinition ("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition (relationDefinition);
      rightEndPointDefinition.SetRelationDefinition (relationDefinition);

      _persistentClassDefinition.SetRelationEndPointDefinitions (
          new RelationEndPointDefinitionCollection (new IRelationEndPointDefinition[] { leftEndPointDefinition, rightEndPointDefinition }, true));
      _persistentClassDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_persistentClassDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassTransaction_AndAnonymousEndPointHasNonPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition (_transactionPropertyDefinitionOnPersistentClassDefinition, false);
      var rightEndPointDefinition = new AnonymousRelationEndPointDefinition (_nonPersistentClassDefinition);
      var relationDefinition = new RelationDefinition ("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition (relationDefinition);
      rightEndPointDefinition.SetRelationDefinition (relationDefinition);

      _persistentClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { leftEndPointDefinition }, true));
      _persistentClassDefinition.SetReadOnly();

      _nonPersistentClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new IRelationEndPointDefinition[0], true));
      _nonPersistentClassDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_persistentClassDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassTransaction_AndVirtualRelationEndPointWithCardinalityOneHasNonPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition (_transactionPropertyDefinitionOnPersistentClassDefinition, false);
      var rightEndPointDefinition = VirtualRelationEndPointDefinitionFactory.Create (
          _nonPersistentClassDefinition,
          "Right",
          false,
          CardinalityType.One,
          typeof (DomainObject));
      var relationDefinition = new RelationDefinition ("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition (relationDefinition);
      rightEndPointDefinition.SetRelationDefinition (relationDefinition);

      _persistentClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new [] { leftEndPointDefinition }, true));
      _persistentClassDefinition.SetReadOnly();

      _nonPersistentClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new [] { rightEndPointDefinition }, true));
      _nonPersistentClassDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_persistentClassDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassTransaction_AndVirtualRelationEndPointWithCardinalityManyHasNonPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition (_transactionPropertyDefinitionOnPersistentClassDefinition, false);
      var rightEndPointDefinition = VirtualRelationEndPointDefinitionFactory.Create (
          _nonPersistentClassDefinition,
          "Right",
          false,
          CardinalityType.Many,
          typeof (ObjectList<DomainObject>));
      var relationDefinition = new RelationDefinition ("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition (relationDefinition);
      rightEndPointDefinition.SetRelationDefinition (relationDefinition);

      _persistentClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new [] { leftEndPointDefinition }, true));
      _persistentClassDefinition.SetReadOnly();

      _nonPersistentClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new [] { rightEndPointDefinition }, true));
      _nonPersistentClassDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_persistentClassDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_AndAnonymousEndPointHasPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition (_persistentPropertyDefinition, false);
      var rightEndPointDefinition = new AnonymousRelationEndPointDefinition (_persistentClassDefinition);
      var relationDefinition = new RelationDefinition ("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition (relationDefinition);
      rightEndPointDefinition.SetRelationDefinition (relationDefinition);

      _persistentClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { leftEndPointDefinition }, true));
      _persistentClassDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_persistentClassDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_AndVirtualRelationEndPointWithCardinalityOneHasPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition (_persistentPropertyDefinition, false);
      var rightEndPointDefinition = VirtualRelationEndPointDefinitionFactory.Create (
          _persistentClassDefinition,
          "Right",
          false,
          CardinalityType.One,
          typeof (DomainObject));
      var relationDefinition = new RelationDefinition ("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition (relationDefinition);
      rightEndPointDefinition.SetRelationDefinition (relationDefinition);

      _persistentClassDefinition.SetRelationEndPointDefinitions (
          new RelationEndPointDefinitionCollection (new IRelationEndPointDefinition[] { leftEndPointDefinition, rightEndPointDefinition }, true));
      _persistentClassDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_persistentClassDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_AndVirtualRelationEndPointWithCardinalityManyHasPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition (_persistentPropertyDefinition, false);
      var rightEndPointDefinition = VirtualRelationEndPointDefinitionFactory.Create (
          _persistentClassDefinition,
          "Right",
          false,
          CardinalityType.Many,
          typeof (ObjectList<DomainObject>));
      var relationDefinition = new RelationDefinition ("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition (relationDefinition);
      rightEndPointDefinition.SetRelationDefinition (relationDefinition);

      _persistentClassDefinition.SetRelationEndPointDefinitions (
          new RelationEndPointDefinitionCollection (new IRelationEndPointDefinition[] { leftEndPointDefinition, rightEndPointDefinition }, true));
      _persistentClassDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_persistentClassDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_AndAnonymousEndPointHasNonPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition (_persistentPropertyDefinition, false);
      var rightEndPointDefinition = new AnonymousRelationEndPointDefinition (_nonPersistentClassDefinition);
      var relationDefinition = new RelationDefinition ("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition (relationDefinition);
      rightEndPointDefinition.SetRelationDefinition (relationDefinition);

      _persistentClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { leftEndPointDefinition }, true));
      _persistentClassDefinition.SetReadOnly();

      _nonPersistentClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new IRelationEndPointDefinition[0], true));
      _nonPersistentClassDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_persistentClassDefinition);

      var expectedMessage =
          "The relation property is defined as persistent but the referenced type "
          + "'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderViewModel' is non-persistent. "
          + "Persistent relation properties may only reference persistent types.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: PersistentProperty1FakeProperty";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_AndVirtualRelationEndPointWithCardinalityOneHasNonPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition (_persistentPropertyDefinition, false);
      var rightEndPointDefinition = VirtualRelationEndPointDefinitionFactory.Create (
          _nonPersistentClassDefinition,
          "Right",
          false,
          CardinalityType.One,
          typeof (DomainObject));
      var relationDefinition = new RelationDefinition ("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition (relationDefinition);
      rightEndPointDefinition.SetRelationDefinition (relationDefinition);

      _persistentClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new [] { leftEndPointDefinition }, true));
      _persistentClassDefinition.SetReadOnly();

      _nonPersistentClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new [] { rightEndPointDefinition }, true));
      _nonPersistentClassDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_persistentClassDefinition);

      var expectedMessage =
          "The relation property is defined as persistent but the referenced type "
          + "'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderViewModel' is non-persistent. "
          + "Persistent relation properties may only reference persistent types.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: PersistentProperty1FakeProperty";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_AndVirtualRelationEndPointWithCardinalityManyHasNonPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition (_persistentPropertyDefinition, false);
      var rightEndPointDefinition = VirtualRelationEndPointDefinitionFactory.Create (
          _nonPersistentClassDefinition,
          "Right",
          false,
          CardinalityType.Many,
          typeof (ObjectList<DomainObject>));
      var relationDefinition = new RelationDefinition ("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition (relationDefinition);
      rightEndPointDefinition.SetRelationDefinition (relationDefinition);

      _persistentClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new [] { leftEndPointDefinition }, true));
      _persistentClassDefinition.SetReadOnly();

      _nonPersistentClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new [] { rightEndPointDefinition }, true));
      _nonPersistentClassDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_persistentClassDefinition);

      var expectedMessage =
          "The relation property is defined as persistent but the referenced type "
          + "'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderViewModel' is non-persistent. "
          + "Persistent relation properties may only reference persistent types.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: PersistentProperty1FakeProperty";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void RelationEndPointHasStorageClassPersistent_AndSecondEndPointIsAlsoRealButHasNonPersistentClassDefinition ()
    {
      var leftEndPointDefinition = new RelationEndPointDefinition (_persistentPropertyDefinition, false);
      var rightEndPointDefinition = new RelationEndPointDefinition (_transactionPropertyDefinitionOnNonPersistentClassDefinition, false);
      var relationDefinition = new RelationDefinition ("Test", leftEndPointDefinition, rightEndPointDefinition);
      leftEndPointDefinition.SetRelationDefinition (relationDefinition);
      rightEndPointDefinition.SetRelationDefinition (relationDefinition);

      _persistentClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { leftEndPointDefinition }, true));
      _persistentClassDefinition.SetReadOnly();

      _nonPersistentClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { rightEndPointDefinition }, true));
      _nonPersistentClassDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_persistentClassDefinition);

      var expectedMessage =
          "The relation property is defined as persistent but the referenced type "
          + "'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderViewModel' is non-persistent. "
          + "Persistent relation properties may only reference persistent types.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: PersistentProperty1FakeProperty";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
  }
}