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
    private ClassDefinition _classDefinition;
    
    [SetUp]
    public void SetUp ()
    {
      _validationRule = new RelationEndPointPropertyTypeIsSupportedValidationRule();
      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (RelationEndPointPropertyClass));
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
    public void NoBidirectionalRelation_PropertyTypeNoDomainObject ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _classDefinition,
          "PropertyWithoutBidirectionalAttribute",
          false,
          CardinalityType.One,
          null,
          PropertyInfoAdapter.Create(typeof (RelationEndPointPropertyClass).GetProperty ("PropertyWithoutBidirectionalAttribute")));
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      var expectedMessage =
          "The property type of an uni-directional relation property must be assignable to 'DomainObject'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
          + "RelationEndPointPropertyTypeIsSupportedValidationRule.RelationEndPointPropertyClass\r\n"
          + "Property: PropertyWithoutBidirectionalAttribute";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void NoBidirectionalRelation_PropertyTypeDomainObject ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _classDefinition,
          "DomainObjectPropertyWithoutBidirectionalAttribute",
          false,
          CardinalityType.One,
          null,
          PropertyInfoAdapter.Create(typeof (RelationEndPointPropertyClass).GetProperty ("DomainObjectPropertyWithoutBidirectionalAttribute")));
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);
      
      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void BidirectionalRelation_PropertyTypeDomainObject ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _classDefinition,
          "DomainObjectPropertyWithBidirectionalAttribute",
          false,
          CardinalityType.One,
          null,
          PropertyInfoAdapter.Create(typeof (RelationEndPointPropertyClass).GetProperty ("DomainObjectPropertyWithBidirectionalAttribute")));
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void BidirectionalRelation_PropertyTypeNoDomainObject ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _classDefinition,
          "PropertyWithBidirectionalAttribute",
          false,
          CardinalityType.One,
          null,
          PropertyInfoAdapter.Create(typeof (RelationEndPointPropertyClass).GetProperty ("PropertyWithBidirectionalAttribute")));
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }
  }
}