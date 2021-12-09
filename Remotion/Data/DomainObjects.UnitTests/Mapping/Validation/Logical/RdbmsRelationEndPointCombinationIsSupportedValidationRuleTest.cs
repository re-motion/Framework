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
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Logical
{
  [TestFixture]
  public class RdbmsRelationEndPointCombinationIsSupportedValidationRuleTest : ValidationRuleTestBase
  {
    private RelationEndPointCombinationIsSupportedValidationRule _validationRule;

    private TypeDefinition _orderClass;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new RdbmsRelationEndPointCombinationIsSupportedValidationRule();

      _orderClass = FakeMappingConfiguration.Current.TypeDefinitions[typeof(Order)];
    }

    [Test]
    public void PropertyNotFoundRelationEndPointDefinition_LeftEndPoint ()
    {
      var anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition(_orderClass);
      var invalidRelationEndPointDefinition = new PropertyNotFoundRelationEndPointDefinition(_orderClass, "Invalid", typeof(object));
      var relationDefinition = new RelationDefinition("Test", invalidRelationEndPointDefinition, anonymousEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(mappingValidationResult, true, null);
    }

    [Test]
    public void PropertyNotFoundRelationEndPointDefinition_RightEndPoint ()
    {
      var anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition(_orderClass);
      var invalidRelationEndPointDefinition = new PropertyNotFoundRelationEndPointDefinition(_orderClass, "Invalid", typeof(object));
      var relationDefinition = new RelationDefinition("Test", anonymousEndPointDefinition, invalidRelationEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(mappingValidationResult, true, null);
    }

    [Test]
    public void TypeNotObjectIDRelationEndPointDefinition_LeftEndPoint ()
    {
      var anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition(_orderClass);
      var invalidRelationEndPointDefinition = new TypeNotObjectIDRelationEndPointDefinition(_orderClass, "Invalid", typeof(string));
      var relationDefinition = new RelationDefinition("Test", invalidRelationEndPointDefinition, anonymousEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(mappingValidationResult, true, null);
    }

    [Test]
    public void TypeNotCompatibleWithVirtualRelationEndPointDefinition_LeftEndPoint ()
    {
      var anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition(_orderClass);
      var invalidRelationEndPointDefinition = new TypeNotCompatibleWithVirtualRelationEndPointDefinition(_orderClass, "Invalid", typeof(string));
      var relationDefinition = new RelationDefinition("Test", invalidRelationEndPointDefinition, anonymousEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(mappingValidationResult, true, null);
    }

    [Test]
    public void TypeNotObjectIDRelationEndPointDefinition_RightEndPoint ()
    {
      var anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition(_orderClass);
      var invalidRelationEndPointDefinition = new TypeNotObjectIDRelationEndPointDefinition(_orderClass, "Invalid", typeof(string));
      var relationDefinition = new RelationDefinition("Test", anonymousEndPointDefinition, invalidRelationEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(mappingValidationResult, true, null);
    }

    [Test]
    public void TypeNotCompatibleWithVirtualRelationEndPointDefinition_RightEndPoint ()
    {
      var anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition(_orderClass);
      var invalidRelationEndPointDefinition = new TypeNotCompatibleWithVirtualRelationEndPointDefinition(_orderClass, "Invalid", typeof(string));
      var relationDefinition = new RelationDefinition("Test", anonymousEndPointDefinition, invalidRelationEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(mappingValidationResult, true, null);
    }

    [Test]
    public void ValidRelationDefinition ()
    {
      var customerToOrder =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order:Remotion.Data.DomainObjects.UnitTests.Mapping."
              + "TestDomain.Integration.Order.Customer->Remotion.Data.DomainObjects.UnitTests.Mapping."
              + "TestDomain.Integration.Customer.Orders"];

      var mappingValidationResult = _validationRule.Validate(customerToOrder);

      AssertMappingValidationResult(mappingValidationResult, true, null);
    }

    [Test]
    public void TwoAnonymousRelationEndPoints ()
    {
      var anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition(_orderClass);
      var relationDefinition = new RelationDefinition("Test", anonymousEndPointDefinition, anonymousEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage = "Relation 'Test' cannot have two anonymous end points.";
      AssertMappingValidationResult(mappingValidationResult, false, expectedMessage);
    }

    [Test]
    public void TwoVirtualRelationEndPoints ()
    {
      var virtualEndPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _orderClass, "OrderNumber", false, PropertyInfoAdapter.Create(typeof(Order).GetProperty("OrderNumber")));
      var relationDefinition = new RelationDefinition("Test", virtualEndPointDefinition, virtualEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "The relation between property 'OrderNumber', declared on type 'Order', and property 'OrderNumber' declared on type "
          + "'Order', contains two virtual end points. One of the two properties must set 'ContainsForeignKey' to 'true' on the "
          + "'DBBidirectionalRelationAttribute'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: OrderNumber\r\n"
          + "Relation ID: Test";
      AssertMappingValidationResult(mappingValidationResult, false, expectedMessage);
    }

    [Test]
    public void TwoVirtualRelationEndPoint_FirstEndPointIsAnonymous ()
    {
      var anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition(_orderClass);
      var virtualEndPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _orderClass, "OrderNumber", false, PropertyInfoAdapter.Create(typeof(Order).GetProperty("OrderNumber")));
      var relationDefinition = new RelationDefinition("Test", virtualEndPointDefinition, anonymousEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "Relation 'Test' contains one virtual and one anonymous end point. "
          + "One of the two properties must set 'ContainsForeignKey' to 'true' on the 'DBBidirectionalRelationAttribute'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: OrderNumber\r\n"
          + "Relation ID: Test";
      AssertMappingValidationResult(mappingValidationResult, false, expectedMessage);
    }

    [Test]
    public void TwoVirtualRelationEndPoint_SecondEndPointIsAnonymous ()
    {
      var anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition(_orderClass);
      var virtualEndPointDefinition = new VirtualObjectRelationEndPointDefinition(
          _orderClass, "OrderNumber", false, PropertyInfoAdapter.Create(typeof(Order).GetProperty("OrderNumber")));
      var relationDefinition = new RelationDefinition("Test", virtualEndPointDefinition, anonymousEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "Relation 'Test' contains one virtual and one anonymous end point. "
          + "One of the two properties must set 'ContainsForeignKey' to 'true' on the 'DBBidirectionalRelationAttribute'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: OrderNumber\r\n"
          + "Relation ID: Test";
      AssertMappingValidationResult(mappingValidationResult, false, expectedMessage);
    }

    [Test]
    public void TwoNonVirtualRelationEndPoints ()
    {
      var customerToOrder =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order:Remotion.Data.DomainObjects.UnitTests.Mapping."
              + "TestDomain.Integration.Order.Customer->Remotion.Data.DomainObjects.UnitTests.Mapping."
              + "TestDomain.Integration.Customer.Orders"];
      var relationDefinition = new RelationDefinition("Test", customerToOrder.EndPointDefinitions[1], customerToOrder.EndPointDefinitions[1]);

      var mappingValidationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
        "The relation between property 'Customer', declared on type 'Order', and property 'Customer' declared on type 'Order', "
        +"contains two non-virtual end points. One of the two properties must set 'ContainsForeignKey' to 'false' on the 'DBBidirectionalRelationAttribute'.\r\n\r\n"
        +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
        +"Property: Customer\r\n"
        +"Relation ID: Test";
      AssertMappingValidationResult(mappingValidationResult, false, expectedMessage);
    }
  }
}
