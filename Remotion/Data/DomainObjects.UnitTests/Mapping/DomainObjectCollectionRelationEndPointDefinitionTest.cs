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
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class DomainObjectCollectionRelationEndPointDefinitionTest : MappingReflectionTestBase
  {
    private ClassDefinition _customerClassDefinition;
    private DomainObjectCollectionRelationEndPointDefinition _customerOrdersEndPoint;

    private ClassDefinition _orderClassDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _customerClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (Customer));
      _customerOrdersEndPoint = DomainObjectCollectionRelationEndPointDefinitionFactory.Create (
          _customerClassDefinition,
          "Orders",
          false,
          typeof (OrderCollection),
          "OrderNumber desc");

      _orderClassDefinition = CreateOrderDefinition_WithEmptyMembers_AndDerivedClasses ();
    }

    [Test]
    public void InitializeWithPropertyType ()
    {
      var endPoint = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
          _orderClassDefinition,
          "VirtualEndPoint",
          true,
          typeof (OrderCollection),
          null);

      Assert.That (endPoint.PropertyInfo.PropertyType, Is.SameAs (typeof (OrderCollection)));
    }

    [Test]
    public void InitializeWithSortExpression ()
    {
      var endPointDefinition = DomainObjectCollectionRelationEndPointDefinitionFactory.Create (
          _customerClassDefinition,
          "Orders",
          false,
          typeof (OrderCollection),
          "OrderNumber desc");

      Assert.That (endPointDefinition.SortExpressionText, Is.EqualTo ("OrderNumber desc"));
    }

    [Test]
    public void IsAnonymous ()
    {
      Assert.That (_customerOrdersEndPoint.IsAnonymous, Is.False);
    }

    [Test]
    public void RelationDefinition_Null ()
    {
      Assert.That (_customerOrdersEndPoint.RelationDefinition, Is.Null);
    }

    [Test]
    public void RelationDefinition_NonNull ()
    {
      _customerOrdersEndPoint.SetRelationDefinition (new RelationDefinition ("Test", _customerOrdersEndPoint, _customerOrdersEndPoint));
      Assert.That (_customerOrdersEndPoint.RelationDefinition, Is.Not.Null);
    }

    [Test]
    public void GetSortExpression_Null ()
    {
      var endPoint = DomainObjectCollectionRelationEndPointDefinitionFactory.Create (
          _orderClassDefinition,
          "OrderItems",
          false,
          typeof (ObjectList<OrderItem>),
          null);
      Assert.That (endPoint.SortExpressionText, Is.Null);

      Assert.That (endPoint.GetSortExpression(), Is.Null);
    }

    [Test]
    public void GetSortExpression_NonNull ()
    {
      var endPoint = CreateFullVirtualEndPointAndClassDefinition_WithProductProperty ("Product asc");

      Assert.That (endPoint.GetSortExpression(), Is.Not.Null);
      Assert.That (
          endPoint.GetSortExpression().ToString(),
          Is.EqualTo ("Product ASC"));
    }

    [Test]
    public void GetSortExpression_Error ()
    {
      var endPoint = CreateFullVirtualEndPointAndClassDefinition_WithProductProperty ("Product asc asc");
      Assert.That (
          () => endPoint.GetSortExpression(),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo (
                  "SortExpression 'Product asc asc' cannot be parsed: Expected 1 or 2 parts (a property name and an optional identifier), found 3 parts instead.\r\n\r\n"+
                  "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\nProperty: OrderItems"));
    }

    [Test]
    public void PropertyInfo ()
    {
      ClassDefinition orderClassDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Order));
      DomainObjectCollectionRelationEndPointDefinition relationEndPointDefinition =
          (DomainObjectCollectionRelationEndPointDefinition) orderClassDefinition.GetRelationEndPointDefinition (typeof (Order) + ".OrderItems");
      Assert.That (relationEndPointDefinition.PropertyInfo, Is.EqualTo (PropertyInfoAdapter.Create(typeof (Order).GetProperty ("OrderItems"))));
    }

    private DomainObjectCollectionRelationEndPointDefinition CreateFullVirtualEndPointAndClassDefinition_WithProductProperty (string sortExpressionString)
    {
      var endPoint = DomainObjectCollectionRelationEndPointDefinitionFactory.Create (
          _orderClassDefinition,
          "OrderItems",
          false,
          typeof (ObjectList<OrderItem>),
          sortExpressionString);
      var orderItemClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (OrderItem));
      var oppositeProperty = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID (orderItemClassDefinition, "Order");
      var productProperty = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (orderItemClassDefinition, "Product");
      orderItemClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{oppositeProperty, productProperty}, true));
      orderItemClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var oppositeEndPoint = new RelationEndPointDefinition (oppositeProperty, false);
      var relationDefinition = new RelationDefinition ("test", endPoint, oppositeEndPoint);
      orderItemClassDefinition.SetReadOnly ();
      endPoint.SetRelationDefinition (relationDefinition);
      return endPoint;
    }

    private static ClassDefinition CreateOrderDefinition_WithEmptyMembers_AndDerivedClasses ()
    {
      return ClassDefinitionObjectMother.CreateClassDefinition_WithEmptyMembers_AndDerivedClasses ("Order", classType: typeof (Order));
    }
  }
}
