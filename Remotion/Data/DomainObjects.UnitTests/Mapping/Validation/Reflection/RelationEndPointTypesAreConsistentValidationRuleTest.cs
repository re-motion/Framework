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
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.RelationEndPointTypesAreConsistentValidationRule;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Reflection
{
  [TestFixture]
  public class RelationEndPointTypesAreConsistentValidationRuleTest : ValidationRuleTestBase
  {
    private RelationEndPointTypesAreConsistentValidationRule _validationRule;
    private ClassDefinition _baseClassDefinition1;
    private ClassDefinition _baseClassDefinition2;
    private TypeDefinition _derivedClassDefinition1;
    private TypeDefinition _derivedClassDefinition2;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new RelationEndPointTypesAreConsistentValidationRule();
      _baseClassDefinition1 = ClassDefinitionObjectMother.CreateClassDefinition("BaseRelationEndPointPropertyClass1", typeof(BaseRelationEndPointPropertyClass1));
      _baseClassDefinition2 = ClassDefinitionObjectMother.CreateClassDefinition("BaseRelationEndPointPropertyClass2", typeof(BaseRelationEndPointPropertyClass2));
      _derivedClassDefinition1 = TypeDefinitionObjectMother.CreateClassDefinition(
          "DerivedRelationEndPointPropertyClass1",
          typeof(DerivedRelationEndPointPropertyClass1),
          baseClass: _baseClassDefinition1);
      _derivedClassDefinition2 = TypeDefinitionObjectMother.CreateClassDefinition(
          "DerivedRelationEndPointPropertyClass2",
          typeof(DerivedRelationEndPointPropertyClass2),
          baseClass: _baseClassDefinition2);
    }

    [Test]
    public void LeftEndPointIsAnonymous ()
    {
      var endPoint1 = new AnonymousRelationEndPointDefinition(_baseClassDefinition1);
      var endPoint2 = new VirtualObjectRelationEndPointDefinition(
          _baseClassDefinition2,
          "RelationProperty1",
          false,
          PropertyInfoAdapter.Create(typeof(BaseRelationEndPointPropertyClass2).GetProperty("RelationProperty1")));

      var relationDefinition = CreateRelationDefinitionAndSetBackReferences("Test", endPoint1, endPoint2);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RightEndPointIsAnonymous ()
    {
      var endPoint1 = new VirtualObjectRelationEndPointDefinition(
          _baseClassDefinition1,
          "RelationProperty1",
          false,
          PropertyInfoAdapter.Create(typeof(BaseRelationEndPointPropertyClass1).GetProperty("RelationProperty1")));
      var endPoint2 = new AnonymousRelationEndPointDefinition(_baseClassDefinition2);

      var relationDefinition = CreateRelationDefinitionAndSetBackReferences("Test", endPoint1, endPoint2);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RightEndPointWithoutPropertyType ()
    {
      var endPoint1 = new VirtualObjectRelationEndPointDefinition(
          _baseClassDefinition1,
          "RelationProperty2",
          false,
          PropertyInfoAdapter.Create(typeof(BaseRelationEndPointPropertyClass1).GetProperty("RelationProperty2")));
      var endPoint2 = new PropertyNotFoundRelationEndPointDefinition(_baseClassDefinition2, "PropertyName", typeof(object));

      var relationDefinition = CreateRelationDefinitionAndSetBackReferences("Test", endPoint1, endPoint2);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void LeftEndPointWithoutPropertyType ()
    {
      var endPoint1 = new PropertyNotFoundRelationEndPointDefinition(_baseClassDefinition2, "PropertyName", typeof(object));
      var endPoint2 = new VirtualObjectRelationEndPointDefinition(
          _baseClassDefinition1,
          "RelationProperty2",
          false,
          PropertyInfoAdapter.Create(typeof(BaseRelationEndPointPropertyClass1).GetProperty("RelationProperty2")));

      var relationDefinition = CreateRelationDefinitionAndSetBackReferences("Test", endPoint1, endPoint2);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void EndPointWithoutBidirectionalRelationAttribute ()
    {
      var endPoint1 = new VirtualObjectRelationEndPointDefinition(
          _baseClassDefinition1,
          "RelationProperty2",
          false,
          PropertyInfoAdapter.Create(typeof(BaseRelationEndPointPropertyClass1).GetProperty("RelationProperty2")));
      var endPoint2 = new AnonymousRelationEndPointDefinition(_baseClassDefinition2);

      var relationDefinition = CreateRelationDefinitionAndSetBackReferences("Test", endPoint1, endPoint2);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ValidRelationDefinition ()
    {
      var endPoint1 = new VirtualObjectRelationEndPointDefinition(
          _baseClassDefinition1,
          "RelationProperty1",
          false,
          PropertyInfoAdapter.Create(typeof(BaseRelationEndPointPropertyClass1).GetProperty("RelationProperty1")));
      var endPoint2 = new VirtualObjectRelationEndPointDefinition(
          _baseClassDefinition2,
          "RelationProperty1",
          false,
          PropertyInfoAdapter.Create(typeof(BaseRelationEndPointPropertyClass2).GetProperty("RelationProperty1")));

      var relationDefinition = CreateRelationDefinitionAndSetBackReferences("Test", endPoint1, endPoint2);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void PropertyDeclaredOnClassDefinitionInMapping_PropertyTypeDoesNotMatch ()
    {
      var endPoint1 = new VirtualObjectRelationEndPointDefinition(
          _derivedClassDefinition1,
          "RelationProperty3",
          false,
          PropertyInfoAdapter.Create(typeof(DerivedRelationEndPointPropertyClass1).GetProperty("RelationProperty3")));
      var endPoint2 = new VirtualObjectRelationEndPointDefinition(
          _baseClassDefinition2,
          "RelationProperty3",
          false,
          PropertyInfoAdapter.Create(typeof(DerivedRelationEndPointPropertyClass2).GetProperty("RelationProperty3")));

      var relationDefinition = CreateRelationDefinitionAndSetBackReferences("Test", endPoint1, endPoint2);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "The type 'BaseRelationEndPointPropertyClass2' does not match the type of the opposite relation propery 'RelationProperty3' "
          + "declared on type 'DerivedRelationEndPointPropertyClass1'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
          + "RelationEndPointTypesAreConsistentValidationRule.BaseRelationEndPointPropertyClass2\r\n"
          + "Property: RelationProperty3";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void RelationToClassNotInMapping ()
    {
      var endPoint1 = new VirtualObjectRelationEndPointDefinition(
          _derivedClassDefinition1,
          "RelationProperty4",
          false,
          PropertyInfoAdapter.Create(typeof(DerivedRelationEndPointPropertyClass1).GetProperty("RelationProperty4")));
      var endPoint2 = new VirtualObjectRelationEndPointDefinition(
          _derivedClassDefinition2,
          "RelationProperty4",
          false,
          PropertyInfoAdapter.Create(typeof(DerivedRelationEndPointPropertyClass2).GetProperty("RelationProperty4")));

      var relationDefinition = CreateRelationDefinitionAndSetBackReferences("Test", endPoint1, endPoint2);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "The type 'BaseRelationEndPointPropertyClass2' cannot be assigned to the type of the opposite relation propery 'RelationProperty4' declared "
          + "on type 'DerivedRelationEndPointPropertyClass1'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
          + "RelationEndPointTypesAreConsistentValidationRule.BaseRelationEndPointPropertyClass2\r\n"
          + "Property: RelationProperty4";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }
  }
}
