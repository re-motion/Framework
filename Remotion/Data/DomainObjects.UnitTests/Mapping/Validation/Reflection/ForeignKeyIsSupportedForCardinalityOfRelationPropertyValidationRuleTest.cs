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
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Reflection
{
  [TestFixture]
  public class ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRuleTest : ValidationRuleTestBase
  {
    private ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule _validationRule;
    private TypeDefinition _typeDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule();
      _typeDefinition = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(ForeignKeyIsSupportedClass));
    }

    [Test]
    public void RelationDefinitionWithAnonymousEndPointDefinitions ()
    {
      var endPoint1Stub = new Mock<IRelationEndPointDefinition>();
      var endPoint2Stub = new Mock<IRelationEndPointDefinition>();
      var relationDefinition = new RelationDefinition("Test", endPoint1Stub.Object, endPoint2Stub.Object);

      endPoint1Stub.Setup(stub => stub.IsAnonymous).Returns(true);
      endPoint2Stub.Setup(stub => stub.IsAnonymous).Returns(true);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RelationDefinitionPropertyInfoIsNotResolved ()
    {
      var endPoint1Stub = new Mock<IRelationEndPointDefinition>();
      var endPoint2Stub = new Mock<IRelationEndPointDefinition>();
      var relationDefinition = new RelationDefinition("Test", endPoint1Stub.Object, endPoint2Stub.Object);

      endPoint1Stub.Setup(stub => stub.PropertyInfo).Returns((IPropertyInformation)null);
      endPoint2Stub.Setup(stub => stub.PropertyInfo).Returns((IPropertyInformation)null);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void NoReflectionBasedVirtualRelationEndPointDefinition ()
    {
      var endPointDefinition = new AnonymousRelationEndPointDefinition(_typeDefinition);
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void PropertyWithNoDBBidirectionalRelationAttribute ()
    {
      var endPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition,
          "PropertyWithNoDbBidirectionalRelationAttribute",
          false,
          PropertyInfoAdapter.Create(typeof(ForeignKeyIsSupportedClass).GetProperty("PropertyWithNoDbBidirectionalRelationAttribute")));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void NoCollectionProperty_ContainsForeignKeyIsTrue ()
    {
      var endPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition,
          "NoCollectionProperty_ContainsForeignKey",
          false,
          PropertyInfoAdapter.Create(typeof(ForeignKeyIsSupportedClass).GetProperty("NoCollectionProperty_ContainsForeignKey")));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void NoCollectionProperty_ContainsForeignKeyIsFalse ()
    {
      var endPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition,
          "NoCollectionProperty_ContainsNoForeignKey",
          false,
          PropertyInfoAdapter.Create(typeof(ForeignKeyIsSupportedClass).GetProperty("NoCollectionProperty_ContainsNoForeignKey")));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void DomainObjectCollectionProperty_ContainsForeignKeyIsTrue ()
    {
      var endPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition,
          "DomainObjectCollectionProperty_ContainsForeignKey",
          false,
          PropertyInfoAdapter.Create(typeof(ForeignKeyIsSupportedClass).GetProperty("DomainObjectCollectionProperty_ContainsForeignKey")));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "Only relation end points with a property type of 'DomainObject' can contain the foreign key.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
          + "ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule.ForeignKeyIsSupportedClass\r\n"
          + "Property: DomainObjectCollectionProperty_ContainsForeignKey";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void VirtualCollectionProperty_ContainsForeignKeyIsTrue ()
    {
      var endPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition,
          "VirtualCollectionProperty_ContainsForeignKey",
          false,
          PropertyInfoAdapter.Create(typeof(ForeignKeyIsSupportedClass).GetProperty("VirtualCollectionProperty_ContainsForeignKey")));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "Only relation end points with a property type of 'DomainObject' can contain the foreign key.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
          + "ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule.ForeignKeyIsSupportedClass\r\n"
          + "Property: VirtualCollectionProperty_ContainsForeignKey";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void DomainObjectCollectionProperty_ContainsForeignKeyIsTrue_BothEndPoints ()
    {
      var endPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition,
          "DomainObjectCollectionProperty_ContainsForeignKey",
          false,
          PropertyInfoAdapter.Create(typeof(ForeignKeyIsSupportedClass).GetProperty("DomainObjectCollectionProperty_ContainsForeignKey")));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "Only relation end points with a property type of 'DomainObject' can contain the foreign key.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
          + "ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule.ForeignKeyIsSupportedClass\r\n"
          + "Property: DomainObjectCollectionProperty_ContainsForeignKey";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void VirtualCollectionProperty_ContainsForeignKeyIsTrue_BothEndPoints ()
    {
      var endPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition,
          "VirtualCollectionProperty_ContainsForeignKey",
          false,
          PropertyInfoAdapter.Create(typeof(ForeignKeyIsSupportedClass).GetProperty("VirtualCollectionProperty_ContainsForeignKey")));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "Only relation end points with a property type of 'DomainObject' can contain the foreign key.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
          + "ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule.ForeignKeyIsSupportedClass\r\n"
          + "Property: VirtualCollectionProperty_ContainsForeignKey";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void DomainObjectCollectionProperty_ContainsForeignKeyIsFalse ()
    {
      var endPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition,
          "DomainObjectCollectionProperty_ContainsNoForeignKey",
          false,
          PropertyInfoAdapter.Create(typeof(ForeignKeyIsSupportedClass).GetProperty("DomainObjectCollectionProperty_ContainsNoForeignKey")));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void VirtualCollectionProperty_ContainsForeignKeyIsFalse ()
    {
      var endPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition,
          "VirtualCollectionProperty_ContainsNoForeignKey",
          false,
          PropertyInfoAdapter.Create(typeof(ForeignKeyIsSupportedClass).GetProperty("VirtualCollectionProperty_ContainsNoForeignKey")));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }
  }
}
