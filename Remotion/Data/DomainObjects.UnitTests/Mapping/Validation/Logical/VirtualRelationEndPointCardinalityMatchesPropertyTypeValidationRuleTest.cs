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
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Order = Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Logical
{
  [TestFixture]
  public class VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRuleTest : ValidationRuleTestBase
  {
    private class DerivedObjectList<T> : ObjectList<T> where T : DomainObject
    {
    }

    private VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRule _validationRule;
    private TypeDefinition _typeDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRule();
      _typeDefinition = FakeMappingConfiguration.Current.TypeDefinitions[typeof(Order)];
    }

    [Test]
    public void NoVirtualRelationEndPointDefinition ()
    {
      var endPointDefinition = new AnonymousRelationEndPointDefinition(_typeDefinition);
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityOne_And_PropertyTypeNotAssignableToDomainObject ()
    {
      var leftEndPointDefinition =
          VirtualObjectRelationEndPointDefinitionFactory.Create(_typeDefinition, "Left", false, typeof(string));
      var rightEndPointDefinition =
          VirtualObjectRelationEndPointDefinitionFactory.Create(_typeDefinition, "Right", false, typeof(DomainObject));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "The property type of a virtual end point of a one-to-one relation must be assignable to 'DomainObject'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: Left";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityOne_And_PropertyTypeIsDomainObject ()
    {
      var endPointDefinition = VirtualObjectRelationEndPointDefinitionFactory.Create(
         _typeDefinition, "PropertyName", false, typeof(DomainObject));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityOne_And_PropertyTypeDerivedFromDomainObject ()
    {
      var endPointDefinition = VirtualObjectRelationEndPointDefinitionFactory.Create(
         _typeDefinition, "PropertyName", false, typeof(BaseOfBaseValidationDomainObjectClass));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityMany_And_PropertyTypeIsDomainObjectCollection ()
    {
      var endPointDefinition = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
          _typeDefinition, "PropertyName", false, typeof(DomainObjectCollection));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "The property type of a virtual end point of a one-to-many relation must be assignable to 'ObjectList`1'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: PropertyName";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityMany_And_PropertyTypeIsObjectList ()
    {
      var endPointDefinition = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
         _typeDefinition, "PropertyName", false, typeof(ObjectList<BaseOfBaseValidationDomainObjectClass>));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityMany_And_PropertyTypeIsDerivedFromObjectList ()
    {
      var endPointDefinition = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
         _typeDefinition, "PropertyName", false, typeof(DerivedObjectList<BaseOfBaseValidationDomainObjectClass>));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityMany_And_PropertyTypeIsNotAssignableToObjectList ()
    {
      var leftEndPointDefinition = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
          _typeDefinition, "PropertyName", false, typeof(DomainObjectCollection));
      var rightEndPointDefinition = VirtualObjectRelationEndPointDefinitionFactory.Create(
          _typeDefinition, "PropertyName", false, typeof(DomainObject));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "The property type of a virtual end point of a one-to-many relation must be assignable to 'ObjectList`1'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: PropertyName";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityMany_And_PropertyTypeIsIObjectList ()
    {
      var endPointDefinition = VirtualCollectionRelationEndPointDefinitionFactory.Create(
          _typeDefinition, "PropertyName", false, typeof(IObjectList<BaseOfBaseValidationDomainObjectClass>));
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityMany_And_PropertyTypeIsNotIObjectList ()
    {
      var leftEndPointDefinition = VirtualCollectionRelationEndPointDefinitionFactory.Create(
          _typeDefinition, "PropertyName", false, typeof(DomainObjectCollection));
      var rightEndPointDefinition = VirtualObjectRelationEndPointDefinitionFactory.Create(
          _typeDefinition, "PropertyName", false, typeof(DomainObject));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "The property type of a virtual end point of a one-to-many relation must be 'IObjectList`1'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: PropertyName";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityMany_And_PropertyTypeIsDerivedFromIObjectList ()
    {
      var leftEndPointDefinition = VirtualCollectionRelationEndPointDefinitionFactory.Create(
          _typeDefinition, "PropertyName", false, typeof(IDerivedObjectList<BaseOfBaseValidationDomainObjectClass>));
      var rightEndPointDefinition = VirtualObjectRelationEndPointDefinitionFactory.Create(
          _typeDefinition, "PropertyName", false, typeof(DomainObject));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "The property type of a virtual end point of a one-to-many relation must be 'IObjectList`1'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: PropertyName";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }
  }
}
