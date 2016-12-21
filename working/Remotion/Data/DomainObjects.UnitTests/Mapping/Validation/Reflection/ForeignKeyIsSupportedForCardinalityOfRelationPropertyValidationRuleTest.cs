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
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Reflection
{
  [TestFixture]
  public class ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRuleTest : ValidationRuleTestBase
  {
    private ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule _validationRule;
    private ClassDefinition _classDefinition;
    
    [SetUp]
    public void SetUp ()
    {
      _validationRule = new ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule();
      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (ForeignKeyIsSupportedClass));
    }

    [Test]
    public void RelationDefinitionWithAnonymousEndPointDefinitions ()
    {
      var endPoint1Stub = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      var endPoint2Stub = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      var relationDefinition = new RelationDefinition ("Test", endPoint1Stub, endPoint2Stub);

      endPoint1Stub.Stub (stub => stub.IsAnonymous).Return (true);
      endPoint2Stub.Stub (stub => stub.IsAnonymous).Return (true);

      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void RelationDefinitionPropertyInfoIsNotResolved ()
    {
      var endPoint1Stub = MockRepository.GenerateStub<IRelationEndPointDefinition> ();
      var endPoint2Stub = MockRepository.GenerateStub<IRelationEndPointDefinition> ();
      var relationDefinition = new RelationDefinition ("Test", endPoint1Stub, endPoint2Stub);

      endPoint1Stub.Stub (stub => stub.PropertyInfo).Return (null);
      endPoint2Stub.Stub (stub => stub.PropertyInfo).Return (null);

      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void NoReflectionBasedVirtualRelationEndPointDefinition ()
    {
      var endPointDefinition = new AnonymousRelationEndPointDefinition (_classDefinition);
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyWithNoDBBidirectionalRelationAttribute ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _classDefinition,
          "PropertyWithNoDbBidirectionalRelationAttribute",
          false,
          CardinalityType.One,
          null,
          PropertyInfoAdapter.Create(typeof (ForeignKeyIsSupportedClass).GetProperty ("PropertyWithNoDbBidirectionalRelationAttribute")));
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void NoCollectionProperty_ContainsForeignKeyIsTrue ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _classDefinition,
          "NoCollectionProperty_ContainsForeignKey",
          false,
          CardinalityType.One,
          null,
          PropertyInfoAdapter.Create(typeof (ForeignKeyIsSupportedClass).GetProperty ("NoCollectionProperty_ContainsForeignKey")));
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void NoCollectionProperty_ContainsForeignKeyIsFalse ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _classDefinition,
          "NoCollectionProperty_ContainsNoForeignKey",
          false,
          CardinalityType.One,
          null,
          PropertyInfoAdapter.Create(typeof (ForeignKeyIsSupportedClass).GetProperty ("NoCollectionProperty_ContainsNoForeignKey")));
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void CollectionProperty_ContainsForeignKeyIsTrue ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _classDefinition,
          "CollectionProperty_ContainsForeignKey",
          false,
          CardinalityType.One,
          null,
          PropertyInfoAdapter.Create(typeof (ForeignKeyIsSupportedClass).GetProperty ("CollectionProperty_ContainsForeignKey")));
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      var expectedMessage =
          "Only relation end points with a property type of 'DomainObject' can contain the foreign key.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
          + "ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule.ForeignKeyIsSupportedClass\r\n"
          + "Property: CollectionProperty_ContainsForeignKey";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void CollectionProperty_ContainsForeignKeyIsTrue_BothEndPoints ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _classDefinition,
          "CollectionProperty_ContainsForeignKey",
          false,
          CardinalityType.One,
          null,
          PropertyInfoAdapter.Create(typeof (ForeignKeyIsSupportedClass).GetProperty ("CollectionProperty_ContainsForeignKey")));
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      var expectedMessage =
          "Only relation end points with a property type of 'DomainObject' can contain the foreign key.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
          + "ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule.ForeignKeyIsSupportedClass\r\n"
          + "Property: CollectionProperty_ContainsForeignKey";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void CollectionProperty_ContainsForeignKeyIsFalse ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _classDefinition,
          "CollectionProperty_ContainsNoForeignKey",
          false,
          CardinalityType.One,
          null,
          PropertyInfoAdapter.Create(typeof (ForeignKeyIsSupportedClass).GetProperty ("CollectionProperty_ContainsNoForeignKey")));
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }
  }
}