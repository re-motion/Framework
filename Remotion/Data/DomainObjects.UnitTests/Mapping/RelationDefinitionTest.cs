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
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class RelationDefinitionTest : MappingReflectionTestBase
  {
    private TypeDefinition _customerClass;
    private DomainObjectCollectionRelationEndPointDefinition _customerEndPoint;
    private RelationEndPointDefinition _orderEndPoint;
    private RelationDefinition _customerToOrder;

    public override void SetUp ()
    {
      base.SetUp();

      _customerClass = FakeMappingConfiguration.Current.TypeDefinitions[typeof(Customer)];
      _customerToOrder =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order:Remotion.Data.DomainObjects.UnitTests.Mapping."
              + "TestDomain.Integration.Order.Customer->Remotion.Data.DomainObjects.UnitTests.Mapping."
              + "TestDomain.Integration.Customer.Orders"];
      _customerEndPoint = (DomainObjectCollectionRelationEndPointDefinition)_customerToOrder.EndPointDefinitions[0];
      _orderEndPoint = (RelationEndPointDefinition)_customerToOrder.EndPointDefinitions[1];
    }

    [Test]
    public void EndPointDefinitionsAreReturnedAsReadOnlyCollection ()
    {
      RelationDefinition relation = new RelationDefinition("RelationID", _customerEndPoint, _orderEndPoint);

      Assert.That(relation.EndPointDefinitions.Length, Is.EqualTo(2));
      Assert.That(relation.EndPointDefinitions[0], Is.SameAs(_customerEndPoint));
      Assert.That(relation.EndPointDefinitions[1], Is.SameAs(_orderEndPoint));
    }

    [Test]
    public void GetToString ()
    {
      RelationDefinition relation = new RelationDefinition("RelationID", _customerEndPoint, _orderEndPoint);

      Assert.That(relation.ToString(), Is.EqualTo("RelationDefinition: RelationID"));
    }

    [Test]
    public void IsEndPoint ()
    {
      RelationDefinition relation = new RelationDefinition("myRelation", _customerEndPoint, _orderEndPoint);

      Assert.That(relation.IsEndPoint(typeof(Order), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.Customer"), Is.True);
      Assert.That(relation.IsEndPoint(typeof(Customer), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer.Orders"), Is.True);
      Assert.That(relation.IsEndPoint(typeof(Order), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer.Orders"), Is.False);
      Assert.That(relation.IsEndPoint(typeof(Customer), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.Customer"), Is.False);
    }

    [Test]
    public void Contains ()
    {
      RelationDefinition relation = new RelationDefinition("myRelation", _customerEndPoint, _orderEndPoint);

      Assert.That(relation.Contains(_orderEndPoint), Is.True);
      Assert.That(relation.Contains(_customerEndPoint), Is.True);

      var invalidEndPoint = CreateEquivalentEndPointDefinitionFake(_customerEndPoint);
      Assert.That(relation.Contains(invalidEndPoint), Is.False);
    }

    [Test]
    public void GetEndPointDefinition ()
    {
      Assert.That(
          _customerToOrder.GetEndPointDefinition(typeof(Order), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.Customer"),
          Is.SameAs(_orderEndPoint));
      Assert.That(_customerToOrder.GetEndPointDefinition(
          typeof(Customer), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer.Orders"), Is.SameAs(_customerEndPoint));
    }

    [Test]
    public void GetOppositeEndPointDefinition_Strings ()
    {
      var result1 = _customerToOrder.GetOppositeEndPointDefinition(
          typeof(Order), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.Customer");
      Assert.That(result1, Is.SameAs(_customerEndPoint));

      var result2 = _customerToOrder.GetOppositeEndPointDefinition(
          typeof(Customer), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer.Orders");
      Assert.That(result2, Is.SameAs(_orderEndPoint));
    }

    [Test]
    public void GetOppositeEndPointDefinitionForUndefinedProperty_Strings ()
    {
      var result = _customerToOrder.GetOppositeEndPointDefinition(
          typeof(Order), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.OrderNumber");
      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetOppositeEndPointDefinition_EndPoint ()
    {
      var result1 = _customerToOrder.GetOppositeEndPointDefinition(_customerEndPoint);
      Assert.That(result1, Is.SameAs(_orderEndPoint));

      var result2 = _customerToOrder.GetOppositeEndPointDefinition(_orderEndPoint);
      Assert.That(result2, Is.SameAs(_customerEndPoint));
    }

    [Test]
    public void GetOppositeEndPointDefinitionForUndefinedProperty_EndPoint ()
    {
      var invalidEndPoint = CreateEquivalentEndPointDefinitionFake(_customerEndPoint);

      var result = _customerToOrder.GetOppositeEndPointDefinition(invalidEndPoint);
      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetEndPointDefinitionForUndefinedProperty ()
    {
      var result = _customerToOrder.GetEndPointDefinition(
          typeof(Order), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.OrderNumber");
      Assert.That(result, Is.Null);
    }


    [Test]
    public void GetEndPointDefinitionForUndefinedClass ()
    {
      Assert.That(_customerToOrder.GetEndPointDefinition(
          typeof(OrderTicket), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderTicket.Customer"), Is.Null);
    }

    [Test]
    public void GetOppositeEndPointDefinitionForUndefinedClass ()
    {
      Assert.That(_customerToOrder.GetOppositeEndPointDefinition(
          typeof(OrderTicket), "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderTicket.Customer"), Is.Null);
    }


    [Test]
    public void GetMandatoryOppositeRelationEndPointDefinitionWithNotAssociatedRelationDefinitionID ()
    {
      RelationDefinition orderToOrderItem =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderItem:"
              + "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderItem.Order->"
              +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.OrderItems"];

      Assert.That(
          () => orderToOrderItem.GetMandatoryOppositeRelationEndPointDefinition(
              _customerClass.GetMandatoryRelationEndPointDefinition(
                  "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer.Orders")),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Relation 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderItem:"
                  + "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderItem.Order->"
                  +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.OrderItems' has no association with type "
                  + "'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer' "
                  + "and property 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer.Orders'."));
    }

    [Test]
    public void RelationType_OneToOne ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Computer));
      var relationDefinition = classDefinition.GetRelationEndPointDefinition(typeof(Computer).FullName + ".Employee").RelationDefinition;
      Assert.That(relationDefinition.RelationKind, Is.EqualTo(RelationKindType.OneToOne));
    }

    [Test]
    public void RelationType_OneMany ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(OrderItem));
      var relationDefinition = classDefinition.GetRelationEndPointDefinition(typeof(OrderItem).FullName + ".Order").RelationDefinition;
      Assert.That(relationDefinition.RelationKind, Is.EqualTo(RelationKindType.OneToMany));
    }

    [Test]
    public void RelationType_Unidirectional ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Location));
      var relationDefinition = classDefinition.GetRelationEndPointDefinition(typeof(Location).FullName + ".Client").RelationDefinition;
      Assert.That(relationDefinition.RelationKind, Is.EqualTo(RelationKindType.Unidirectional));
    }

    private IRelationEndPointDefinition CreateEquivalentEndPointDefinitionFake (DomainObjectCollectionRelationEndPointDefinition sourceEndPoint)
    {
      var invalidEndPoint = new Mock<IRelationEndPointDefinition>();
      invalidEndPoint.Setup(stub => stub.TypeDefinition).Returns(sourceEndPoint.TypeDefinition);
      invalidEndPoint.Setup(stub => stub.PropertyName).Returns(sourceEndPoint.PropertyName);
      return invalidEndPoint.Object;
    }

  }
}
