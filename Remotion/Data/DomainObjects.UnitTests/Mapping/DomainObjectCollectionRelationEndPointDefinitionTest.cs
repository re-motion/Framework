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
          typeof (OrderCollection));

      _orderClassDefinition = CreateOrderDefinition_WithEmptyMembers_AndDerivedClasses ();
    }

    [Test]
    public void InitializeWithPropertyType ()
    {
      var endPoint = DomainObjectCollectionRelationEndPointDefinitionFactory.Create(
          _orderClassDefinition,
          "VirtualEndPoint",
          true,
          typeof (OrderCollection));

      Assert.That (endPoint.PropertyInfo.PropertyType, Is.SameAs (typeof (OrderCollection)));
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
          new Lazy<SortExpressionDefinition> (() => null));

      Assert.That (endPoint.GetSortExpression(), Is.Null);

#pragma warning disable 618
      Assert.That (endPoint.SortExpressionText, Is.Null);
#pragma warning restore 618
    }

    [Test]
    public void GetSortExpression_NonNull ()
    {
      var sortExpressionDefinition = new SortExpressionDefinition (
          new[]
          {
              SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending (
                  PropertyDefinitionObjectMother.CreateForFakePropertyInfo ("ProductNumber", StorageClass.Persistent))
          });

      var endPoint = DomainObjectCollectionRelationEndPointDefinitionFactory.Create (
          _orderClassDefinition,
          "OrderItems",
          false,
          typeof (ObjectList<OrderItem>),
          new Lazy<SortExpressionDefinition> (() => sortExpressionDefinition));

      Assert.That (endPoint.GetSortExpression(), Is.SameAs (sortExpressionDefinition));

#pragma warning disable 618
      Assert.That (endPoint.SortExpressionText, Is.EqualTo ("ProductNumber DESC"));
#pragma warning restore 618
    }

    [Test]
    public void PropertyInfo ()
    {
      ClassDefinition orderClassDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Order));
      DomainObjectCollectionRelationEndPointDefinition relationEndPointDefinition =
          (DomainObjectCollectionRelationEndPointDefinition) orderClassDefinition.GetRelationEndPointDefinition (typeof (Order) + ".OrderItems");
      Assert.That (relationEndPointDefinition.PropertyInfo, Is.EqualTo (PropertyInfoAdapter.Create(typeof (Order).GetProperty ("OrderItems"))));
    }

    private static ClassDefinition CreateOrderDefinition_WithEmptyMembers_AndDerivedClasses ()
    {
      return ClassDefinitionObjectMother.CreateClassDefinition_WithEmptyMembers_AndDerivedClasses ("Order", classType: typeof (Order));
    }
  }
}
