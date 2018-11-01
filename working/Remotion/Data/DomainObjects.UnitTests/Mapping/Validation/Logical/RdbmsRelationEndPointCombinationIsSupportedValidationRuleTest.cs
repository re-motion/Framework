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

    private ClassDefinition _orderClass;
    private RelationDefinition _customerToOrder;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new RdbmsRelationEndPointCombinationIsSupportedValidationRule();

      _orderClass = FakeMappingConfiguration.Current.TypeDefinitions[typeof (Order)];
      _customerToOrder =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order:Remotion.Data.DomainObjects.UnitTests.Mapping."
              + "TestDomain.Integration.Order.Customer->Remotion.Data.DomainObjects.UnitTests.Mapping."
              + "TestDomain.Integration.Customer.Orders"];
    }

    [Test]
    public void PropertyNotFoundRelationEndPointDefinition_LeftEndPoint ()
    {
      var anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition (_orderClass);
      var invalidRelationEndPointDefinition = new PropertyNotFoundRelationEndPointDefinition (_orderClass, "Invalid");
      var relationDefinition = new RelationDefinition ("Test", invalidRelationEndPointDefinition, anonymousEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (mappingValidationResult, true, null);
    }

    [Test]
    public void PropertyNotFoundRelationEndPointDefinition_RightEndPoint ()
    {
      var anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition (_orderClass);
      var invalidRelationEndPointDefinition = new PropertyNotFoundRelationEndPointDefinition (_orderClass, "Invalid");
      var relationDefinition = new RelationDefinition ("Test", anonymousEndPointDefinition, invalidRelationEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (mappingValidationResult, true, null);
    }

    [Test]
    public void TypeNotObjectIDRelationEndPointDefinition_LeftEndPoint ()
    {
      var anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition (_orderClass);
      var invalidRelationEndPointDefinition = new TypeNotObjectIDRelationEndPointDefinition (_orderClass, "Invalid", typeof(string));
      var relationDefinition = new RelationDefinition ("Test", invalidRelationEndPointDefinition, anonymousEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (mappingValidationResult, true, null);
    }

    [Test]
    public void TypeNotObjectIDRelationEndPointDefinition_RightEndPoint ()
    {
      var anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition (_orderClass);
      var invalidRelationEndPointDefinition = new TypeNotObjectIDRelationEndPointDefinition (_orderClass, "Invalid", typeof(string));
      var relationDefinition = new RelationDefinition ("Test", anonymousEndPointDefinition, invalidRelationEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (mappingValidationResult, true, null);
    }

    [Test]
    public void ValidRelationDefinition ()
    {
      var mappingValidationResult = _validationRule.Validate (_customerToOrder);

      AssertMappingValidationResult (mappingValidationResult, true, null);
    }

    [Test]
    public void TwoAnonymousRelationEndPoints ()
    {
      var anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition (_orderClass);
      var relationDefinition = new RelationDefinition ("Test", anonymousEndPointDefinition, anonymousEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate (relationDefinition);

      var expectedMessage = "Relation 'Test' cannot have two anonymous end points.";
      AssertMappingValidationResult (mappingValidationResult, false, expectedMessage);
    }

    [Test]
    public void TwoVirtualRelationEndPoints ()
    {
      var virtualEndPointDefinition = new VirtualRelationEndPointDefinition (
          _orderClass, "OrderNumber", false, CardinalityType.One, "", PropertyInfoAdapter.Create(typeof (Order).GetProperty ("OrderNumber")));
      var relationDefinition = new RelationDefinition ("Test", virtualEndPointDefinition, virtualEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate (relationDefinition);

      var expectedMessage =
          "The relation between property 'OrderNumber', declared on type 'Order', and property 'OrderNumber' declared on type "
          + "'Order', contains two virtual end points. One of the two properties must set 'ContainsForeignKey' to 'true' on the "
          + "'DBBidirectionalRelationAttribute'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: OrderNumber\r\n"
          + "Relation ID: Test";
      AssertMappingValidationResult (mappingValidationResult, false, expectedMessage);
    }

    [Test]
    public void TwoVirtualRelationEndPoint_OneEndPointIsAnonymous ()
    {
      var anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition (_orderClass);
      var virtualEndPointDefinition = new VirtualRelationEndPointDefinition (
          _orderClass, "OrderNumber", false, CardinalityType.One, "", PropertyInfoAdapter.Create(typeof (Order).GetProperty ("OrderNumber")));
      var relationDefinition = new RelationDefinition ("Test", virtualEndPointDefinition, anonymousEndPointDefinition);

      var mappingValidationResult = _validationRule.Validate (relationDefinition);

      var expectedMessage =
          "Relation 'Test' cannot have two virtual end points.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: OrderNumber\r\n"
          + "Relation ID: Test";
      AssertMappingValidationResult (mappingValidationResult, false, expectedMessage);
    }

    [Test]
    public void TwoNonVirtualRelationEndPoints ()
    {
      var relationDefinition = new RelationDefinition ("Test", _customerToOrder.EndPointDefinitions[1], _customerToOrder.EndPointDefinitions[1]);

      var mappingValidationResult = _validationRule.Validate (relationDefinition);

      var expectedMessage = 
        "The relation between property 'Customer', declared on type 'Order', and property 'Customer' declared on type 'Order', "
        +"contains two non-virtual end points. One of the two properties must set 'ContainsForeignKey' to 'false' on the 'DBBidirectionalRelationAttribute'.\r\n\r\n"
        +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\n"
        +"Property: Customer\r\n"
        +"Relation ID: Test";
      AssertMappingValidationResult (mappingValidationResult, false, expectedMessage);
    }
  }
}