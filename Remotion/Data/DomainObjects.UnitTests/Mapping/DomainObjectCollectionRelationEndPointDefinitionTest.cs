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
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.UnitTests.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class DomainObjectCollectionRelationEndPointDefinitionTest : MappingReflectionTestBase
  {
    private TypeDefinition _customerTypeDefinition;
    private DomainObjectCollectionRelationEndPointDefinition _customerOrdersEndPoint;

    private TypeDefinition _orderTypeDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _customerTypeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(Customer));
      _customerOrdersEndPoint = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
          _customerTypeDefinition,
          "Orders",
          false,
          typeof(OrderCollection));

      _orderTypeDefinition = CreateOrderDefinition_WithEmptyMembers_AndDerivedClasses();
    }

    [Test]
    public void InitializeWithPropertyType ()
    {
      var endPoint = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
          _orderTypeDefinition,
          "VirtualEndPoint",
          true,
          typeof(OrderCollection));

      Assert.That(endPoint.PropertyInfo.PropertyType, Is.SameAs(typeof(OrderCollection)));
    }

    [Test]
    public void IsAnonymous ()
    {
      Assert.That(_customerOrdersEndPoint.IsAnonymous, Is.False);
    }

    [Test]
    public void RelationDefinition_NotSet ()
    {
      Assert.That(_customerOrdersEndPoint.HasRelationDefinitionBeenSet, Is.False);
      Assert.That(
          () => _customerOrdersEndPoint.RelationDefinition,
          Throws.InvalidOperationException.With.Message.EqualTo("RelationDefinition has not been set for this relation end point."));
    }

    [Test]
    public void SetRelationDefinition ()
    {
      var relationDefinition = new RelationDefinition("Test", _customerOrdersEndPoint, _customerOrdersEndPoint);
      _customerOrdersEndPoint.SetRelationDefinition(relationDefinition);

      Assert.That(_customerOrdersEndPoint.HasRelationDefinitionBeenSet, Is.True);
      Assert.That(_customerOrdersEndPoint.RelationDefinition, Is.SameAs(relationDefinition));
    }

    [Test]
    public void GetSortExpression_Null ()
    {
      var endPoint = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
          _orderTypeDefinition,
          "OrderItems",
          false,
          typeof(ObjectList<OrderItem>),
          new Lazy<SortExpressionDefinition>(() => null));

      Assert.That(endPoint.GetSortExpression(), Is.Null);

#pragma warning disable 618
      Assert.That(endPoint.SortExpressionText, Is.Null);
#pragma warning restore 618
    }

    [Test]
    public void GetSortExpression_NonNull ()
    {
      var sortExpressionDefinition = new SortExpressionDefinition(
          new[]
          {
              SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending(
                  PropertyDefinitionObjectMother.CreateForFakePropertyInfo("ProductNumber", StorageClass.Persistent))
          });

      var endPoint = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
          _orderTypeDefinition,
          "OrderItems",
          false,
          typeof(ObjectList<OrderItem>),
          new Lazy<SortExpressionDefinition>(() => sortExpressionDefinition));

      Assert.That(endPoint.GetSortExpression(), Is.SameAs(sortExpressionDefinition));

#pragma warning disable 618
      Assert.That(endPoint.SortExpressionText, Is.EqualTo("ProductNumber DESC"));
#pragma warning restore 618
    }

    [Test]
    public void PropertyInfo ()
    {
      TypeDefinition orderTypeDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Order));
      DomainObjectCollectionRelationEndPointDefinition relationEndPointDefinition =
          (DomainObjectCollectionRelationEndPointDefinition)orderTypeDefinition.GetRelationEndPointDefinition(typeof(Order) + ".OrderItems");
      Assert.That(relationEndPointDefinition.PropertyInfo, Is.EqualTo(PropertyInfoAdapter.Create(typeof(Order).GetProperty("OrderItems"))));
    }

    private static TypeDefinition CreateOrderDefinition_WithEmptyMembers_AndDerivedClasses ()
    {
      return TypeDefinitionObjectMother.CreateClassDefinition_WithEmptyMembers_AndDerivedClasses("Order", classType: typeof(Order));
    }
  }
}
