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
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.RelationEndPointNamesAreConsistentValidationRule;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Reflection
{
  [TestFixture]
  public class RelationEndPointNamesAreConsistentValidationRuleTest : ValidationRuleTestBase
  {
    private RelationEndPointNamesAreConsistentValidationRule _validationRule;
    private TypeDefinition _typeDefinition1;
    private TypeDefinition _typeDefinition2;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new RelationEndPointNamesAreConsistentValidationRule();
      _typeDefinition1 = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(RelationEndPointPropertyClass1));
      _typeDefinition2 = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(RelationEndPointPropertyClass2));
    }

    [Test]
    public void ValidRelation ()
    {
      var endPointDefinition1 = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition1,
          "RelationProperty2",
          false,
          PropertyInfoAdapter.Create(typeof(RelationEndPointPropertyClass1).GetProperty("RelationProperty2")));
      var endPointDefinition2 = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition2,
          "RelationProperty2",
          false,
          PropertyInfoAdapter.Create(typeof(RelationEndPointPropertyClass2).GetProperty("RelationProperty2")));

      var relationDefinition = CreateRelationDefinitionAndSetBackReferences("Test", endPointDefinition1, endPointDefinition2);

      var validationResult = _validationRule.Validate(relationDefinition);
      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void LeftEndPointIsAnonymous ()
    {
      var endPointDefinition1 = new AnonymousRelationEndPointDefinition(_typeDefinition1);
      var endPointDefinition2 = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition2,
          "RelationProperty2",
          false,
          PropertyInfoAdapter.Create(typeof(RelationEndPointPropertyClass2).GetProperty("RelationProperty2")));

      var relationDefinition = CreateRelationDefinitionAndSetBackReferences("Test", endPointDefinition1, endPointDefinition2);

      var validationResult = _validationRule.Validate(relationDefinition);
      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RightEndPointIsAnonymous ()
    {
      var endPointDefinition1 = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition1,
          "RelationProperty2",
          false,
          PropertyInfoAdapter.Create(typeof(RelationEndPointPropertyClass1).GetProperty("RelationProperty2")));
      var endPointDefinition2 = new AnonymousRelationEndPointDefinition(_typeDefinition2);

      var relationDefinition = CreateRelationDefinitionAndSetBackReferences("Test", endPointDefinition1, endPointDefinition2);

      var validationResult = _validationRule.Validate(relationDefinition);
      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void OppositeRelationPropertyHasNoBidirectionalRelationAttributeDefined ()
    {
      var endPointDefinition1 = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition1,
          "RelationProperty1",
          false,
         PropertyInfoAdapter.Create(typeof(RelationEndPointPropertyClass1).GetProperty("RelationProperty1")));
      var endPointDefinition2 = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition2,
          "RelationPopertyWithoutBidirectionalRelationAttribute",
          false,
          PropertyInfoAdapter.Create(typeof(RelationEndPointPropertyClass2).GetProperty("RelationPopertyWithoutBidirectionalRelationAttribute")));

      var relationDefinition = CreateRelationDefinitionAndSetBackReferences("Test", endPointDefinition1, endPointDefinition2);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "Opposite relation property 'RelationPopertyWithoutBidirectionalRelationAttribute' declared on type "
          + "'RelationEndPointPropertyClass2' does not define a matching 'DBBidirectionalRelationAttribute'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
          + "RelationEndPointNamesAreConsistentValidationRule.RelationEndPointPropertyClass1\r\n"
          + "Property: RelationProperty1";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void OppositeRelationPropertyNameDoesNotMatch ()
    {
      var endPointDefinition1 = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition1,
          "RelationProperty3",
          false,
          PropertyInfoAdapter.Create(typeof(RelationEndPointPropertyClass1).GetProperty("RelationProperty3")));
      var endPointDefinition2 = new VirtualObjectRelationEndPointDefinition(
          _typeDefinition2,
          "RelationPopertyWithNonMatchingPropertyName",
          false,
          PropertyInfoAdapter.Create(typeof(RelationEndPointPropertyClass2).GetProperty("RelationPopertyWithNonMatchingPropertyName")));

      var relationDefinition = CreateRelationDefinitionAndSetBackReferences("Test", endPointDefinition1, endPointDefinition2);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
        "Opposite relation property 'RelationPopertyWithNonMatchingPropertyName' declared on type 'RelationEndPointPropertyClass2' "
        + "defines a 'DBBidirectionalRelationAttribute' whose opposite property does not match.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
        + "RelationEndPointNamesAreConsistentValidationRule.RelationEndPointPropertyClass1\r\n"
        + "Property: RelationProperty3";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }
  }
}
