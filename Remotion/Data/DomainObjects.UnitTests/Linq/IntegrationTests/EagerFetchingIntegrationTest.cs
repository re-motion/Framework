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
using System.Linq;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using EagerFetching_BaseClass = Remotion.Data.DomainObjects.UnitTests.Linq.TestDomain.Success.EagerFetching.BaseClass;
using EagerFetching_DerivedClass1 = Remotion.Data.DomainObjects.UnitTests.Linq.TestDomain.Success.EagerFetching.DerivedClass1;
using EagerFetching_DerivedClass2 = Remotion.Data.DomainObjects.UnitTests.Linq.TestDomain.Success.EagerFetching.DerivedClass2;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests
{
  [TestFixture]
  public class EagerFetchingIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void QueryWithSingleAndPredicate ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>()
                   select o).Single(i => i.OrderNumber == 5);
      Assert.That(query, Is.EqualTo(DomainObjectIDs.Order5.GetObject<TestDomainBase>()));
    }

    [Test]
    public void EagerFetching ()
    {
      var query = (from c in QueryFactory.CreateLinqQuery<Customer>()
                   where new[] { "Kunde 1", "Kunde 2" }.Contains(c.Name)
                   select c).FetchMany(c => c.Orders).ThenFetchMany(o => o.OrderItems);

      CheckQueryResult(query, DomainObjectIDs.Customer1, DomainObjectIDs.Customer2);

      CheckDataContainersRegistered(DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2, DomainObjectIDs.OrderItem6);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Customer1, "Orders", true, DomainObjectIDs.Order1, DomainObjectIDs.Order2);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Order2, "OrderItems", false, DomainObjectIDs.OrderItem6);
    }

    [Test]
    public void EagerFetching_FetchAfterMultipleFromsWithDistinct ()
    {
      var query = (from c1 in QueryFactory.CreateLinqQuery<Customer>()
                   from c2 in QueryFactory.CreateLinqQuery<Customer>()
                   where new[] { "Kunde 1", "Kunde 2" }.Contains(c1.Name)
                   select c1).Distinct().FetchMany(x => x.Orders).ThenFetchMany(y => y.OrderItems);

      CheckQueryResult(query, DomainObjectIDs.Customer1, DomainObjectIDs.Customer2);
      CheckDataContainersRegistered(DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2, DomainObjectIDs.OrderItem6);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Customer1, "Orders", true, DomainObjectIDs.Order1, DomainObjectIDs.Order2);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Order2, "OrderItems", false, DomainObjectIDs.OrderItem6);
    }

    [Test]
    public void EagerFetching_FetchAfterMultipleFromsWithoutSelectClauseInCallChain ()
    {
      var query = (from o1 in QueryFactory.CreateLinqQuery<Order>()
                   from o2 in QueryFactory.CreateLinqQuery<Order>()
                   where o1.OrderNumber < 6
                   select o1).Distinct().FetchMany(x => x.OrderItems);

      CheckQueryResult(query, DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4, DomainObjectIDs.Order5,
                        DomainObjectIDs.Order2);

      CheckDataContainersRegistered(DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4, DomainObjectIDs.Order5,
                                     DomainObjectIDs.Order2);
      CheckDataContainersRegistered(
          DomainObjectIDs.OrderItem1,
          DomainObjectIDs.OrderItem2,
          DomainObjectIDs.OrderItem3,
          DomainObjectIDs.OrderItem4,
          DomainObjectIDs.OrderItem5,
          DomainObjectIDs.OrderItem6);

      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Order3, "OrderItems", false, DomainObjectIDs.OrderItem3);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Order4, "OrderItems", false, DomainObjectIDs.OrderItem4);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Order5, "OrderItems", false, DomainObjectIDs.OrderItem5);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Order2, "OrderItems", false, DomainObjectIDs.OrderItem6);
    }

    [Test]
    public void EagerFetching_FetchOne ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>()
                   where o.OrderNumber == 1
                   select o).FetchOne(o => o.OrderTicket);

      CheckQueryResult(query, DomainObjectIDs.Order1);

      CheckDataContainersRegistered(DomainObjectIDs.Order1, DomainObjectIDs.OrderTicket1);
      CheckObjectRelationRegistered(DomainObjectIDs.Order1, "OrderTicket", DomainObjectIDs.OrderTicket1);
      CheckObjectRelationRegistered(DomainObjectIDs.OrderTicket1, "Order", DomainObjectIDs.Order1);
    }

    [Test]
    public void EagerFetching_ThenFetchOne ()
    {
      var query = (from c in QueryFactory.CreateLinqQuery<Customer>()
                   where new[] { "Kunde 1", "Kunde 2" }.Contains(c.Name)
                   select c).FetchMany(c => c.Orders).ThenFetchOne(o => o.OrderTicket);

      CheckQueryResult(query, DomainObjectIDs.Customer1, DomainObjectIDs.Customer2);

      CheckDataContainersRegistered(
          DomainObjectIDs.Customer1, DomainObjectIDs.Customer2,
          DomainObjectIDs.Order1, DomainObjectIDs.Order2,
          DomainObjectIDs.OrderTicket1, DomainObjectIDs.OrderTicket2);

      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Customer1, "Orders", false, DomainObjectIDs.Order1, DomainObjectIDs.Order2);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Customer2, "Orders", false);
      CheckObjectRelationRegistered(DomainObjectIDs.Order1, "OrderTicket", DomainObjectIDs.OrderTicket1);
      CheckObjectRelationRegistered(DomainObjectIDs.Order2, "OrderTicket", DomainObjectIDs.OrderTicket2);
    }

    [Test]
    public void EagerFetching_MultipleFetches ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>()
                   where o.OrderNumber == 1
                   select o)
          .Distinct()
          .FetchMany(o => o.OrderItems)
          .FetchOne(o => o.Customer).ThenFetchMany(c => c.Orders).ThenFetchOne(o => o.Customer).ThenFetchOne(c => c.Ceo);

      CheckQueryResult(query, DomainObjectIDs.Order1);

      CheckDataContainersRegistered(
          DomainObjectIDs.Order1, // the original order
          DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2, // their items
          DomainObjectIDs.Customer1, // their customers
          DomainObjectIDs.Order1, DomainObjectIDs.Order2, // their customer's orders
          DomainObjectIDs.Customer1, // their customer's orders' customers
          DomainObjectIDs.Ceo3); // their customer's orders' customers' ceos
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
      CheckObjectRelationRegistered(DomainObjectIDs.Order1, "Customer", DomainObjectIDs.Customer1);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Customer1, "Orders", true, DomainObjectIDs.Order1, DomainObjectIDs.Order2);
      CheckObjectRelationRegistered(DomainObjectIDs.Customer1, typeof(Company), "Ceo", DomainObjectIDs.Ceo3);
    }

    [Test]
    public void EagerFetching_MultipleFetches_OnSameLevel ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>()
                   where o.OrderNumber == 1
                   select o)
          .Distinct()
          .FetchMany(o => o.OrderItems)
          .FetchOne(o => o.Customer).ThenFetchMany(c => c.Orders).ThenFetchOne(c => c.OrderTicket)
          .FetchOne(o => o.Customer).ThenFetchMany(c => c.Orders).ThenFetchMany(c => c.OrderItems)
          .FetchMany(o => o.OrderItems);

      CheckQueryResult(query, DomainObjectIDs.Order1);

      CheckDataContainersRegistered(
          DomainObjectIDs.Order1, // the original order
          DomainObjectIDs.Customer1, // the customer
          DomainObjectIDs.Order1, DomainObjectIDs.Order2, // the customer's orders
          DomainObjectIDs.OrderTicket1, DomainObjectIDs.OrderTicket2, // the customer's orders' tickets
          DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2, DomainObjectIDs.OrderItem6 // the customer's orders' items
          );

      CheckObjectRelationRegistered(DomainObjectIDs.Order1, "Customer", DomainObjectIDs.Customer1);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Customer1, "Orders", true, DomainObjectIDs.Order1, DomainObjectIDs.Order2);

      CheckObjectRelationRegistered(DomainObjectIDs.Order1, "OrderTicket", DomainObjectIDs.OrderTicket1);
      CheckObjectRelationRegistered(DomainObjectIDs.OrderTicket1, "Order", DomainObjectIDs.Order1);
      CheckObjectRelationRegistered(DomainObjectIDs.Order2, "OrderTicket", DomainObjectIDs.OrderTicket2);
      CheckObjectRelationRegistered(DomainObjectIDs.OrderTicket2, "Order", DomainObjectIDs.Order2);

      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
      CheckObjectRelationRegistered(DomainObjectIDs.OrderItem1, "Order", DomainObjectIDs.Order1);
      CheckObjectRelationRegistered(DomainObjectIDs.OrderItem2, "Order", DomainObjectIDs.Order1);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Order2, "OrderItems", false, DomainObjectIDs.OrderItem6);
    }

    [Test]
    public void EagerFetching_WithTakeResultOperator ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>()
                   orderby o.OrderNumber
                   select o)
                   .Take(1)
                   .FetchMany(o => o.OrderItems);

      CheckQueryResult(query, DomainObjectIDs.Order1);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void EagerFetching_WithResultOperator_AfterFetch_ThrowsNotSupportedException ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>()
                   where o.OrderNumber == 1
                   select o)
                   .FetchMany(o => o.OrderItems)
                   .Take(1);

      Assert.That(
          () => query.ToArray(),
          Throws.TypeOf<NotSupportedException>().And.Message.EqualTo(
              "There was an error preparing or resolving query "
              + "'from Order o in DomainObjectQueryable<Order> where ([o].OrderNumber == 1) select [o] => Fetch (Order.OrderItems) => Take(1)' for "
              + "SQL generation. The fetch query operator methods must be the last query operators in a LINQ query. All calls to Where, Select, Take, etc. "
              + "must go before the fetch operators.\r\n\r\n"
              + "E.g., instead of 'QueryFactory.CreateLinqQuery<Order>().FetchMany (o => o.OrderItems).Where (o => o.OrderNumber > 1)', "
              + "write 'QueryFactory.CreateLinqQuery<Order>().Where (o => o.OrderNumber > 1).FetchMany (o => o.OrderItems)'."));
    }

    [Test]
    public void EagerFetching_WithFetch_InASubQuery_ThrowsNotSupportedException ()
    {
      var query = QueryFactory.CreateLinqQuery<Order>()
                   .FetchMany(o => o.OrderItems)
                   .Where(o => o.OrderNumber == 1);

      Assert.That(
          () => query.ToArray(),
          Throws.TypeOf<NotSupportedException>().And.Message.EqualTo(
              "There was an error preparing or resolving query "
              + "'from Order o in {DomainObjectQueryable<Order> => Fetch (Order.OrderItems)} where ([o].OrderNumber == 1) select [o]' for SQL generation. "
              + "The fetch query operator methods must be the last query operators in a LINQ query. All calls to Where, Select, Take, etc. must go before "
              + "the fetch operators.\r\n\r\n"
              + "E.g., instead of 'QueryFactory.CreateLinqQuery<Order>().FetchMany (o => o.OrderItems).Where (o => o.OrderNumber > 1)', "
              + "write 'QueryFactory.CreateLinqQuery<Order>().Where (o => o.OrderNumber > 1).FetchMany (o => o.OrderItems)'."));
    }

    [Test]
    public void EagerFetching_WithOrderBy_WithoutTake ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>()
                   where o.OrderNumber == 1
                   orderby o.OrderNumber
                   select o)
                   .FetchMany(o => o.OrderItems);

      CheckQueryResult(query, DomainObjectIDs.Order1);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void EagerFetching_FetchNull_VirtualSide ()
    {
      var query = (from employee in QueryFactory.CreateLinqQuery<Employee>()
                   where employee.ID == DomainObjectIDs.Employee1
                   select employee).FetchOne(e => e.Computer);

      CheckQueryResult(query, DomainObjectIDs.Employee1);

      CheckDataContainersRegistered(DomainObjectIDs.Employee1);
      CheckObjectRelationRegistered(DomainObjectIDs.Employee1, "Computer", null);
    }

    [Test]
    public void EagerFetching_FetchNull_NonVirtualSide ()
    {
      var query = (from computer in QueryFactory.CreateLinqQuery<Computer>()
                   where computer.ID == DomainObjectIDs.Computer4
                   select computer).FetchOne(c => c.Employee);

      CheckQueryResult(query, DomainObjectIDs.Computer4);

      CheckDataContainersRegistered(DomainObjectIDs.Computer4);
      CheckObjectRelationRegistered(DomainObjectIDs.Computer4, "Employee", null);
    }

    [Test]
    public void EagerFetching_WithDifferentEntityThanInMainFromClause ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>()
                   where o.OrderNumber == 1
                   select o.Customer).FetchMany(c => c.Orders);

      CheckQueryResult(query, DomainObjectIDs.Customer1);

      CheckDataContainersRegistered(DomainObjectIDs.Customer1, DomainObjectIDs.Order1, DomainObjectIDs.Order2);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Customer1, "Orders", false, DomainObjectIDs.Order1, DomainObjectIDs.Order2);
    }

    [Test]
    public void EagerFetching_FetchEmptyCollection ()
    {
      var query = (from customer in QueryFactory.CreateLinqQuery<Customer>()
                   where customer.ID == DomainObjectIDs.Customer2
                   select customer).FetchMany(o => o.Orders);

      CheckQueryResult(query, DomainObjectIDs.Customer2);

      CheckDataContainersRegistered(DomainObjectIDs.Customer2);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Customer2, "Orders", false);
    }

    [Test]
    public void EagerFetching_WithCustomProjectionQuery_ThrowsNotSupportedException ()
    {
      var query = QueryFactory.CreateLinqQuery<Customer>()
                  .Select(c => new { c.Name })
                  .FetchOne(x => x.Name);

      Assert.That(
          () => query.ToArray(),
          Throws.TypeOf<NotSupportedException>().And.Message.EqualTo("Only queries returning DomainObjects can perform eager fetching."));
    }

    [Test]
    public void EagerFetching_RedirectedProperty ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>()
                   where o.OrderNumber == 1
                   select o).FetchMany(o => o.RedirectedOrderItems);

      CheckQueryResult(query, DomainObjectIDs.Order1);

      CheckDataContainersRegistered(DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
      CheckDomainObjectCollectionRelationRegistered(DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void EagerFetching_CollectionPropertyVirtualSide_ViaDownCastInSelect ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<EagerFetching_BaseClass>()
          where o.ID == DomainObjectIDs.EagerFetching_DerivedClass1_WithCollectionVirtualEndPoint
                || o.ID == DomainObjectIDs.EagerFetching_BaseClass
          select o)
          .FetchMany(o => ((EagerFetching_DerivedClass1)o).CollectionPropertyManySide);

      CheckQueryResult(query, DomainObjectIDs.EagerFetching_BaseClass, DomainObjectIDs.EagerFetching_DerivedClass1_WithCollectionVirtualEndPoint);
      CheckDataContainersRegistered(
          DomainObjectIDs.EagerFetching_RelationTarget_WithCollectionRealEndPoint1,
          DomainObjectIDs.EagerFetching_RelationTarget_WithCollectionRealEndPoint2);
      CheckDomainObjectCollectionRelationRegistered(
          DomainObjectIDs.EagerFetching_DerivedClass1_WithCollectionVirtualEndPoint,
          "CollectionPropertyManySide",
          false,
          DomainObjectIDs.EagerFetching_RelationTarget_WithCollectionRealEndPoint1,
          DomainObjectIDs.EagerFetching_RelationTarget_WithCollectionRealEndPoint2);
    }

    [Test]
    public void EagerFetching_ScalarPropertyVirtualSide_ViaDownCastInSelect ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<EagerFetching_BaseClass>()
          where o.ID == DomainObjectIDs.EagerFetching_DerivedClass1_WithScalarVirtualEndPoint
                || o.ID == DomainObjectIDs.EagerFetching_BaseClass
          select o)
          .FetchOne(o => ((EagerFetching_DerivedClass1)o).ScalarProperty1VirtualSide);

      CheckQueryResult(query, DomainObjectIDs.EagerFetching_BaseClass, DomainObjectIDs.EagerFetching_DerivedClass1_WithScalarVirtualEndPoint);
      CheckDataContainersRegistered(DomainObjectIDs.EagerFetching_RelationTarget_WithScalarRealEndPoint);
      CheckObjectRelationRegistered(
          DomainObjectIDs.EagerFetching_DerivedClass1_WithScalarVirtualEndPoint,
          "ScalarProperty1VirtualSide",
          DomainObjectIDs.EagerFetching_RelationTarget_WithScalarRealEndPoint);
    }

    [Test]
    public void EagerFetching_ScalarPropertyRealSide_ViaDownCastInSelect ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<EagerFetching_BaseClass>()
          where o.ID == DomainObjectIDs.EagerFetching_DerivedClass2_WithScalarRealEndPoint
                || o.ID == DomainObjectIDs.EagerFetching_BaseClass
          select o)
          .FetchOne(o => ((EagerFetching_DerivedClass2)o).ScalarProperty2RealSide);

      CheckQueryResult(query, DomainObjectIDs.EagerFetching_BaseClass, DomainObjectIDs.EagerFetching_DerivedClass2_WithScalarRealEndPoint);
      CheckDataContainersRegistered(DomainObjectIDs.EagerFetching_RelationTarget_WithScalarVirtualEndPoint);
      CheckObjectRelationRegistered(
          DomainObjectIDs.EagerFetching_RelationTarget_WithScalarVirtualEndPoint,
          "ScalarProperty2VirtualSide",
          DomainObjectIDs.EagerFetching_DerivedClass2_WithScalarRealEndPoint);
    }

    [Test]
    public void EagerFetching_UnidirectionalProperty_ViaDownCastInSelect ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<EagerFetching_BaseClass>()
          where o.ID == DomainObjectIDs.EagerFetching_DerivedClass2_WithUnidirectionalEndPoint
                || o.ID == DomainObjectIDs.EagerFetching_BaseClass
          select o)
          .FetchOne(o => ((EagerFetching_DerivedClass2)o).UnidirectionalProperty);

      CheckQueryResult(query, DomainObjectIDs.EagerFetching_BaseClass, DomainObjectIDs.EagerFetching_DerivedClass2_WithUnidirectionalEndPoint);
      //CheckDataContainersRegistered (DomainObjectIDs.TargetClassReceivingReferenceToDerivedClass2, DomainObjectIDs.DerivedClassWithBaseReferenceViaMixin1);
      //CheckObjectRelationRegistered (DomainObjectIDs.DerivedClassWithBaseReferenceViaMixin1, "MyBase", DomainObjectIDs.TargetClassReceivingReferenceToDerivedClass2);
    }

    [Test]
    public void EagerFetching_MixedProperty_ViaCastInFetchClause ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin>()
          where o.ID == DomainObjectIDs.TargetClassForPersistentMixins2
          select o)
          .FetchMany(o => ((IMixinAddingPersistentProperties)o).CollectionProperty1Side);

      CheckQueryResult(query, DomainObjectIDs.TargetClassForPersistentMixins2);

      CheckFetchedCollectionProperty1SideForTargetClass2();
    }

    [Test]
    public void EagerFetching_MixedProperty_ViaCastInSelect ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin>()
          where o.ID == DomainObjectIDs.TargetClassForPersistentMixins2
          select (IMixinAddingPersistentProperties)o)
          .FetchMany(o => o.CollectionProperty1Side);

      CheckQueryResult(query.AsEnumerable().Cast<DomainObject>(), DomainObjectIDs.TargetClassForPersistentMixins2);

      CheckFetchedCollectionProperty1SideForTargetClass2();
    }

    [Test]
    public void EagerFetching_MixedProperty_ViaRedirection ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin>()
          where o.ID == DomainObjectIDs.TargetClassForPersistentMixins2
          select o)
          .FetchMany(o => o.RedirectedCollectionProperty1Side);

      CheckQueryResult(query, DomainObjectIDs.TargetClassForPersistentMixins2);

      CheckFetchedCollectionProperty1SideForTargetClass2();
    }

    [Test]
    public void EagerFetching_MixedProperty_ViaLinqCast ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin>()
          where o.ID == DomainObjectIDs.TargetClassForPersistentMixins2
          select o)
          .FetchMany(o => o.MixedMembers.CollectionProperty1Side);

      CheckQueryResult(query, DomainObjectIDs.TargetClassForPersistentMixins2);

      CheckFetchedCollectionProperty1SideForTargetClass2();
    }

    [Test]
    public void EagerFetching_RelationSortExpressionUsesMixedProperty ()
    {
      var query = QueryFactory.CreateLinqQuery<RelationTargetForPersistentMixin>()
          .Where(o => o.ID == DomainObjectIDs.RelationTargetForPersistentMixin4)
          .Select(o => o)
          .FetchMany(o => o.RelationProperty4)
          .ToArray();

      CheckQueryResult(query, DomainObjectIDs.RelationTargetForPersistentMixin4);

      CheckDataContainersRegistered(DomainObjectIDs.RelationTargetForPersistentMixin4);
      CheckDomainObjectCollectionRelationRegistered(
          DomainObjectIDs.RelationTargetForPersistentMixin4,
          typeof(RelationTargetForPersistentMixin),
          "RelationProperty4",
          false,
          DomainObjectIDs.TargetClassForPersistentMixins2);
    }

    [Test]
    [Ignore("RM-6131: Add integration test for sort property from derived type")]
    public void EagerFetching_RelationSortExpressionUsesPropertyFromDerivedType ()
    {
    }

    [Test]
    public void EagerFetching_FailswhenUsingFetchClausesWithEnumerableQueryDueToDotNetFrameworkImplementaion_ ()
    {
      var orders = new[] { (Order)LifetimeService.GetObject(ClientTransaction.Current, DomainObjectIDs.Order1, false) };
      var queryable = orders.AsQueryable().FetchOne(o => o.Customer);

      //Note: EnumerableQuery<T> will throw an InvalidOperationException when encountering non-linq generic extension methods.
      Assert.That(() => queryable.ToArray(), Throws.InvalidOperationException);
    }

    [Test]
    public void EagerFetching_TransparentlyIgnoreFetchClausesOnNonRelinqBasedQueries_ ()
    {
      var orders = new[] { (Order)LifetimeService.GetObject(ClientTransaction.Current, DomainObjectIDs.Order1, false) };
      var queryProviderStub = new Mock<IQueryProvider>();
      queryProviderStub.Setup(_ => _.Execute<IEnumerable<Order>>(It.IsAny<Expression>())).Returns(orders);

      var queryableStub = new Mock<IQueryable<Order>>();
      queryableStub.Setup(_ => _.Expression).Returns(Expression.Constant(null, typeof(IQueryable<Order>)));
      queryableStub.Setup(_ => _.Provider).Returns(queryProviderStub.Object);
      var queryableWithFetch = EagerFetchingExtensionMethods.FetchOne(queryableStub.Object, o => o.Customer);

      var result = queryableWithFetch.ToArray();

      Assert.That(result, Is.EqualTo(orders));
    }

    private void CheckFetchedCollectionProperty1SideForTargetClass2 ()
    {
      CheckDataContainersRegistered(DomainObjectIDs.RelationTargetForPersistentMixin3);
      CheckDomainObjectCollectionRelationRegistered(
          DomainObjectIDs.TargetClassForPersistentMixins2,
          typeof(MixinAddingPersistentProperties),
          "CollectionProperty1Side",
          false,
          DomainObjectIDs.RelationTargetForPersistentMixin3);
    }
  }
}
