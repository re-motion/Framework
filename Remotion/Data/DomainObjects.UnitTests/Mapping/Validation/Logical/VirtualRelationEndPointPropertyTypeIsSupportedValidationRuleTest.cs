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
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Logical
{
  [TestFixture]
  public class VirtualRelationEndPointPropertyTypeIsSupportedValidationRuleTest : ValidationRuleTestBase
  {
    private class DerivedObjectList<T> : ObjectList<T> where T : DomainObject
    {
    }

    private VirtualRelationEndPointPropertyTypeIsSupportedValidationRule _validationRule;
    private TypeDefinition _typeDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new VirtualRelationEndPointPropertyTypeIsSupportedValidationRule();
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
    public void PropertyTypIsObjectList ()
    {
      var propertyType = typeof(ObjectList<BaseOfBaseValidationDomainObjectClass>);
      var endPointDefinition =
          DomainObjectCollectionRelationEndPointDefinitionFactory.Create(_typeDefinition, "Property", false, propertyType);
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void PropertyTypeIsDerivedFromObjectList ()
    {
      var propertyType = typeof(DerivedObjectList<BaseOfBaseValidationDomainObjectClass>);
      var endPointDefinition =
          DomainObjectCollectionRelationEndPointDefinitionFactory.Create(_typeDefinition, "Property", false, propertyType);
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void PropertyTypIsIObjectList ()
    {
      var propertyType = typeof(IObjectList<BaseOfBaseValidationDomainObjectClass>);
      var endPointDefinition =
          VirtualCollectionRelationEndPointDefinitionFactory.Create(_typeDefinition, "Property", false, propertyType);
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void PropertyTypeIsDomainObject ()
    {
      var propertyType = typeof(DomainObject);
      var endPointDefinition =
          VirtualObjectRelationEndPointDefinitionFactory.Create(_typeDefinition, "Property", false, propertyType);
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void PropertyTypeIsSubclassOfDomainObject ()
    {
      var propertyType = typeof(BaseOfBaseValidationDomainObjectClass);
      var endPointDefinition =
          VirtualObjectRelationEndPointDefinitionFactory.Create(_typeDefinition, "Property", false, propertyType);
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void LeftEndpointPropertyType_NotAssignableToObjectListOrDomainObjectOrOfTypeIObjectList ()
    {
      var leftEndPointDefinition =
          VirtualObjectRelationEndPointDefinitionFactory.Create(_typeDefinition, "Left", false, typeof(string));
      var rightEndPointDefinition =
          VirtualObjectRelationEndPointDefinitionFactory.Create(_typeDefinition, "Right", false, typeof(DomainObject));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "Virtual property 'Left' of type 'Order' is of type 'String', but must be assignable to 'DomainObject' or 'ObjectList`1' or be of type 'IObjectList`1'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: Left";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void RightEndpointPropertyType_NotAssignableToIObjectListOrObjectListOrDomainObjectOrOfTypeIObjectList ()
    {
      var leftEndPointDefinition =
          DomainObjectCollectionRelationEndPointDefinitionFactory.Create(_typeDefinition, "Left", false, typeof(ObjectList<>));
      var rightEndPointDefinition =
          VirtualObjectRelationEndPointDefinitionFactory.Create(_typeDefinition, "Right", false, typeof(string));
      var relationDefinition = new RelationDefinition("Test", leftEndPointDefinition, rightEndPointDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "Virtual property 'Right' of type 'Order' is of type 'String', but must be assignable to 'DomainObject' or 'ObjectList`1' or be of type 'IObjectList`1'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: Right";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }
  }
}
