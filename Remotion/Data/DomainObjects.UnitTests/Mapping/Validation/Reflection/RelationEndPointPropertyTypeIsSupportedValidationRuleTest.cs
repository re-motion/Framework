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
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.RelationEndPointPropertyTypeIsSupportedValidationRule;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Reflection
{
  [TestFixture]
  public class RelationEndPointPropertyTypeIsSupportedValidationRuleTest : ValidationRuleTestBase
  {
    private RelationEndPointPropertyTypeIsSupportedValidationRule _validationRule;
    private TypeDefinition _typeDefinition;
    private IRelationEndPointDefinition _validEndPointDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new RelationEndPointPropertyTypeIsSupportedValidationRule();
      _typeDefinition = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(RelationEndPointPropertyClass));

      _validEndPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition,
          "DomainObjectPropertyWithBidirectionalAttribute",
          false,
          PropertyInfoAdapter.Create(typeof(RelationEndPointPropertyClass).GetProperty("DomainObjectPropertyWithBidirectionalAttribute")));
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
    public void NoBidirectionalRelation_LeftPropertyTypeNoDomainObject ()
    {
      var endPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition,
          "PropertyWithoutBidirectionalAttribute",
          false,
          PropertyInfoAdapter.Create(typeof(RelationEndPointPropertyClass).GetProperty("PropertyWithoutBidirectionalAttribute")));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, _validEndPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "The property type of an uni-directional relation property must be assignable to 'DomainObject'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
          + "RelationEndPointPropertyTypeIsSupportedValidationRule.RelationEndPointPropertyClass\r\n"
          + "Property: PropertyWithoutBidirectionalAttribute";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void NoBidirectionalRelation_RightPropertyTypeNoDomainObject ()
    {
      var endPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition,
          "PropertyWithoutBidirectionalAttribute",
          false,
          PropertyInfoAdapter.Create(typeof(RelationEndPointPropertyClass).GetProperty("PropertyWithoutBidirectionalAttribute")));
      var relationDefinition = new RelationDefinition("Test", _validEndPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "The property type of an uni-directional relation property must be assignable to 'DomainObject'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
          + "RelationEndPointPropertyTypeIsSupportedValidationRule.RelationEndPointPropertyClass\r\n"
          + "Property: PropertyWithoutBidirectionalAttribute";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void NoBidirectionalRelation_PropertyTypeDomainObject ()
    {
      var endPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition,
          "DomainObjectPropertyWithoutBidirectionalAttribute",
          false,
          PropertyInfoAdapter.Create(typeof(RelationEndPointPropertyClass).GetProperty("DomainObjectPropertyWithoutBidirectionalAttribute")));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void BidirectionalRelation_PropertyTypeDomainObject ()
    {
      var endPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition,
          "DomainObjectPropertyWithBidirectionalAttribute",
          false,
          PropertyInfoAdapter.Create(typeof(RelationEndPointPropertyClass).GetProperty("DomainObjectPropertyWithBidirectionalAttribute")));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void BidirectionalRelation_PropertyTypeNoDomainObject ()
    {
      var endPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition,
          "PropertyWithBidirectionalAttribute",
          false,
          PropertyInfoAdapter.Create(typeof(RelationEndPointPropertyClass).GetProperty("PropertyWithBidirectionalAttribute")));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }
  }
}
