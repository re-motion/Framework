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
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class VirtualObjectRelationEndPointDefinitionTest : MappingReflectionTestBase
  {
    private ClassDefinition _orderClassDefinition;
    private VirtualObjectRelationEndPointDefinition _orderOrderTicketEndPoint;

    public override void SetUp ()
    {
      base.SetUp();

      _orderClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Order));
      _orderOrderTicketEndPoint = VirtualObjectRelationEndPointDefinitionFactory.Create(
          _orderClassDefinition,
          "OrderTicket",
          false,
          typeof(OrderTicket));

    }

    [Test]
    public void InitializeWithPropertyType ()
    {
      var classDefinition = CreateOrderDefinition_WithEmptyMembers_AndDerivedClasses();
      var endPoint = VirtualObjectRelationEndPointDefinitionFactory.Create(
          classDefinition,
          "VirtualEndPoint",
          true,
          typeof(OrderItem));

      Assert.That(endPoint.PropertyInfo.PropertyType, Is.SameAs(typeof(OrderItem)));
    }

    [Test]
    public void IsAnonymous ()
    {
      Assert.That(_orderOrderTicketEndPoint.IsAnonymous, Is.False);
    }

    [Test]
    public void RelationDefinition_Null ()
    {
      Assert.That(_orderOrderTicketEndPoint.RelationDefinition, Is.Null);
    }

    [Test]
    public void RelationDefinition_NonNull ()
    {
      _orderOrderTicketEndPoint.SetRelationDefinition(new RelationDefinition("Test", _orderOrderTicketEndPoint, _orderOrderTicketEndPoint));
      Assert.That(_orderOrderTicketEndPoint.RelationDefinition, Is.Not.Null);
    }

    [Test]
    public void PropertyInfo ()
    {
      ClassDefinition employeeClassDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Employee));
      VirtualObjectRelationEndPointDefinition relationEndPointDefinition =
          (VirtualObjectRelationEndPointDefinition) employeeClassDefinition.GetRelationEndPointDefinition(typeof(Employee) + ".Computer");
      Assert.That(relationEndPointDefinition.PropertyInfo, Is.EqualTo(PropertyInfoAdapter.Create(typeof(Employee).GetProperty("Computer"))));
    }

    private static ClassDefinition CreateOrderDefinition_WithEmptyMembers_AndDerivedClasses ()
    {
      return ClassDefinitionObjectMother.CreateClassDefinition_WithEmptyMembers_AndDerivedClasses("Order", classType: typeof(Order));
    }
  }
}
