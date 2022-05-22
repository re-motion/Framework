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
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class DomainObjectGraphTraverserTest : ClientTransactionBaseTest
  {
    private Order GetTestGraph ()
    {
      Order root = Order.NewObject();
      root.Official = Official.NewObject();
      root.OrderTicket = OrderTicket.NewObject();
      root.OrderItems.Add(OrderItem.NewObject());
      root.OrderItems.Add(OrderItem.NewObject());
      root.Customer = Customer.NewObject();
      root.Customer.Ceo = Ceo.NewObject();
      return root;
    }

    private Order GetDeepTestGraph ()
    {
      Order root = Order.NewObject();
      root.Official = Official.NewObject();
      root.OrderTicket = OrderTicket.NewObject();
      root.OrderItems.Add(OrderItem.NewObject());
      root.OrderItems.Add(OrderItem.NewObject());
      root.Customer = Customer.NewObject();
      root.Customer.Ceo = Ceo.NewObject();
      root.Customer.IndustrialSector = IndustrialSector.NewObject();
      root.Customer.IndustrialSector.Companies.Add(Company.NewObject());
      root.Customer.IndustrialSector.Companies[1].Ceo = Ceo.NewObject();
      root.Customer.IndustrialSector.Companies.Add(Company.NewObject());
      root.Customer.IndustrialSector.Companies[2].Ceo = Ceo.NewObject();
      return root;
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_ContainsRoot ()
    {
      Order order = GetTestGraph();
      HashSet<IDomainObject> graph = new DomainObjectGraphTraverser(order, FullGraphTraversalStrategy.Instance).GetFlattenedRelatedObjectGraph();

      Assert.That(graph, Has.Member(order));
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_ContainsRelatedObjects ()
    {
      Order order = GetTestGraph();
      HashSet<IDomainObject> graph = new DomainObjectGraphTraverser(order, FullGraphTraversalStrategy.Instance).GetFlattenedRelatedObjectGraph();

      foreach (DomainObject relatedObject in order.Properties.GetAllRelatedObjects())
        Assert.That(graph, Has.Member(relatedObject));
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_ContainsIndirectRelatedObjects ()
    {
      Order order = GetTestGraph();
      HashSet<IDomainObject> graph = new DomainObjectGraphTraverser(order, FullGraphTraversalStrategy.Instance).GetFlattenedRelatedObjectGraph();

      Assert.That(graph, Has.Member(order.Customer.Ceo));
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_TraversalFilter ()
    {
      Order order = GetDeepTestGraph();
      var strategy = new Mock<IGraphTraversalStrategy>(MockBehavior.Strict);

      strategy.Setup(mock => mock.ShouldProcessObject(order)).Returns(true).Verifiable();
      strategy.Setup(mock => mock.ShouldProcessObject(order.Official)).Returns(true).Verifiable();
      strategy.Setup(mock => mock.ShouldProcessObject(order.OrderTicket)).Returns(true).Verifiable();
      strategy.Setup(mock => mock.ShouldProcessObject(order.OrderItems[0])).Returns(true).Verifiable();
      strategy.Setup(mock => mock.ShouldProcessObject(order.OrderItems[1])).Returns(true).Verifiable();
      strategy.Setup(mock => mock.ShouldProcessObject(order.Customer)).Returns(true).Verifiable();
      strategy.Setup(mock => mock.ShouldProcessObject(order.Customer.Ceo)).Returns(true).Verifiable();
      strategy.Setup(mock => mock.ShouldProcessObject(order.Customer.IndustrialSector)).Returns(true).Verifiable();
      strategy.Setup(mock => mock.ShouldProcessObject(order.Customer.IndustrialSector.Companies[1])).Returns(true).Verifiable();
      strategy.Setup(mock => mock.ShouldProcessObject(order.Customer.IndustrialSector.Companies[1].Ceo)).Returns(true).Verifiable();
      strategy.Setup(mock => mock.ShouldProcessObject(order.Customer.IndustrialSector.Companies[2])).Returns(true).Verifiable();
      strategy.Setup(mock => mock.ShouldProcessObject(order.Customer.IndustrialSector.Companies[2].Ceo)).Returns(true).Verifiable();

      strategy.Setup(mock => mock.ShouldFollowLink(order, order, 0, order.Properties[typeof(Order), "Official"])).Returns(true).Verifiable();
      strategy.Setup(mock => mock.ShouldFollowLink(order, order, 0, order.Properties[typeof(Order), "OrderTicket"])).Returns(true).Verifiable();
      strategy.Setup(mock => mock.ShouldFollowLink(order, order, 0, order.Properties[typeof(Order), "OrderItems"])).Returns(true).Verifiable();
      strategy.Setup(mock => mock.ShouldFollowLink(order, order, 0, order.Properties[typeof(Order), "Customer"])).Returns(true).Verifiable();

      strategy
          .Setup(mock => mock.ShouldFollowLink(order, order.Official, 1, order.Official.Properties[typeof(Official), "Orders"]))
          .Returns(true)
          .Verifiable();
      strategy
          .Setup(mock => mock.ShouldFollowLink(order, order.OrderTicket, 1, order.OrderTicket.Properties[typeof(OrderTicket), "Order"]))
          .Returns(true)
          .Verifiable();
      strategy
          .Setup(mock => mock.ShouldFollowLink(order, order.OrderItems[0], 1, order.OrderItems[0].Properties[typeof(OrderItem), "Order"]))
          .Returns(true)
          .Verifiable();
      strategy
          .Setup(mock => mock.ShouldFollowLink(order, order.OrderItems[1], 1, order.OrderItems[1].Properties[typeof(OrderItem), "Order"]))
          .Returns(true)
          .Verifiable();
      strategy
          .Setup(mock => mock.ShouldFollowLink(order, order.Customer, 1, order.Customer.Properties[typeof(Customer), "Orders"]))
          .Returns(true)
          .Verifiable();
      strategy
          .Setup(mock => mock.ShouldFollowLink(order, order.Customer, 1, order.Customer.Properties[typeof(Customer), "ContactPerson"]))
          .Returns(true)
          .Verifiable();
      strategy
          .Setup(mock => mock.ShouldFollowLink(order, order.Customer, 1, order.Customer.Properties[typeof(Company), "Ceo"]))
          .Returns(true)
          .Verifiable();
      strategy
          .Setup(mock => mock.ShouldFollowLink(order, order.Customer, 1, order.Customer.Properties[typeof(Company), "IndustrialSector"]))
          .Returns(true)
          .Verifiable();

      strategy
          .Setup(mock => mock.ShouldFollowLink(order, order.Customer.Ceo, 2, order.Customer.Ceo.Properties[typeof(Ceo), "Company"]))
          .Returns(true)
          .Verifiable();
      strategy
          .Setup(mock => mock.ShouldFollowLink(order, order.Customer.IndustrialSector, 2, order.Customer.IndustrialSector.Properties[typeof(IndustrialSector), "Companies"]))
          .Returns(true)
          .Verifiable();

      strategy
          .Setup(
              mock => mock.ShouldFollowLink(
                  order,
                  order.Customer.IndustrialSector.Companies[1],
                  3,
                  order.Customer.IndustrialSector.Companies[1].Properties[typeof(Company), "IndustrialSector"]))
          .Returns(true)
          .Verifiable();
      strategy
          .Setup(
              mock => mock.ShouldFollowLink(
                  order,
                  order.Customer.IndustrialSector.Companies[1],
                  3,
                  order.Customer.IndustrialSector.Companies[1].Properties[typeof(Company), "Ceo"]))
          .Returns(true)
          .Verifiable();

      strategy
          .Setup(
              mock => mock.ShouldFollowLink(
                  order,
                  order.Customer.IndustrialSector.Companies[2],
                  3,
                  order.Customer.IndustrialSector.Companies[2].Properties[typeof(Company), "IndustrialSector"]))
          .Returns(true)
          .Verifiable();
      strategy
          .Setup(
              mock => mock.ShouldFollowLink(
                  order,
                  order.Customer.IndustrialSector.Companies[2],
                  3,
                  order.Customer.IndustrialSector.Companies[2].Properties[typeof(Company), "Ceo"]))
          .Returns(true)
          .Verifiable();

      strategy
          .Setup(
              mock => mock.ShouldFollowLink(
                  order,
                  order.Customer.IndustrialSector.Companies[1].Ceo,
                  4,
                  order.Customer.IndustrialSector.Companies[1].Ceo.Properties[typeof(Ceo), "Company"]))
          .Returns(true)
          .Verifiable();
      strategy
          .Setup(
              mock => mock.ShouldFollowLink(
                  order,
                  order.Customer.IndustrialSector.Companies[2].Ceo,
                  4,
                  order.Customer.IndustrialSector.Companies[2].Ceo.Properties[typeof(Ceo), "Company"]))
          .Returns(true)
          .Verifiable();

      HashSet<IDomainObject> result = new DomainObjectGraphTraverser(order, strategy.Object).GetFlattenedRelatedObjectGraph();
      var expected =
          new DomainObject[]
          {
              order, order.Official, order.OrderTicket, order.OrderItems[0], order.OrderItems[1],
              order.Customer, order.Customer.Ceo, order.Customer.IndustrialSector,
              order.Customer.IndustrialSector.Companies[1], order.Customer.IndustrialSector.Companies[1].Ceo,
              order.Customer.IndustrialSector.Companies[2], order.Customer.IndustrialSector.Companies[2].Ceo
          };

      strategy.Verify();
      Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_WithTraversalFilter_FollowLink ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      HashSet<IDomainObject> graph = new DomainObjectGraphTraverser(order, new TestTraversalStrategy(true, false)).GetFlattenedRelatedObjectGraph();

      var expected =
          new HashSet<IDomainObject>
          {
              order,
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.OrderTicket1, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.OrderItem1, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.OrderItem2, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Customer1, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Official1, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.IndustrialSector1, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Partner1, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.PartnerWithoutCeo, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Supplier1, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Distributor2, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Person1, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Person7, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Person3, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Person6, false)
          };

      Assert.That(graph, Is.EquivalentTo(expected));
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_WithTraversalFilter_FollowLink_IncludeObject ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      var graph = new DomainObjectGraphTraverser(order, new TestTraversalStrategy(false, false)).GetFlattenedRelatedObjectGraph();

      var expected =
          new HashSet<IDomainObject>
          {
              order,
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.OrderTicket1, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.OrderItem1, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.OrderItem2, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Customer1, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Official1, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.IndustrialSector1, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Partner1, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.PartnerWithoutCeo, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Supplier1, false),
              LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Distributor2, false)
          };

      Assert.That(graph, Is.EquivalentTo(expected));
    }

    [Test]
    public void Traversal_NotAffectedByNotProcessingAnObject ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      HashSet<IDomainObject> graph = new DomainObjectGraphTraverser(order, new TestTraversalStrategy(false, true)).GetFlattenedRelatedObjectGraph();

      var expected = new HashSet<IDomainObject> { LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Distributor2, false) };

      Assert.That(graph, Is.EquivalentTo(expected));
    }

    class TestTraversalStrategy : IGraphTraversalStrategy
    {
      private readonly bool _includePersons;
      private readonly bool _includeOnlyDistributors;

      public TestTraversalStrategy (bool includePersons, bool includeOnlyDistributors)
      {
        _includePersons = includePersons;
        _includeOnlyDistributors = includeOnlyDistributors;
      }

      public bool ShouldProcessObject (IDomainObject domainObject)
      {
        return (!_includeOnlyDistributors || domainObject is Distributor)
            && (_includePersons || !(domainObject is Person));
      }

      public bool ShouldFollowLink (IDomainObject root, IDomainObject currentObject, int currentDepth, PropertyAccessor linkProperty)
      {
        return !typeof(Ceo).IsAssignableFrom(linkProperty.PropertyData.PropertyType)
          && !typeof(Order).IsAssignableFrom(linkProperty.PropertyData.PropertyType)
          && !typeof(ObjectList<Order>).IsAssignableFrom(linkProperty.PropertyData.PropertyType)
          && !typeof(IObjectList<ProductReview>).IsAssignableFrom(linkProperty.PropertyData.PropertyType);
      }
    }
  }
}
