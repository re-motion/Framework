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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.EagerFetching
{
  [TestFixture]
  public class EagerFetchingTest : ClientTransactionBaseTest
  {
    [Test]
    public void EagerFetching_WithDomainObjectCollection ()
    {
      var ordersQuery = CreateOrdersQuery("OrderNo IN (1, 3)");
      AddOrderItemsFetchQuery(ordersQuery, "o.OrderNo IN (1, 3)");

      var id1 = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");
      var id2 = RelationEndPointID.Create(DomainObjectIDs.Order3, typeof(Order), "OrderItems");

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(id1), Is.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(id2), Is.Null);

      var result = TestableClientTransaction.QueryManager.GetCollection(ordersQuery);
      Assert.That(result.ToArray(), Is.EquivalentTo(new[] { DomainObjectIDs.Order1.GetObject<Order>(), DomainObjectIDs.Order3.GetObject<Order>() }));

      var orderItemsEndPoint1 = (IDomainObjectCollectionEndPoint)TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(id1);
      Assert.That(orderItemsEndPoint1, Is.Not.Null);
      Assert.That(orderItemsEndPoint1.IsSynchronized, Is.True);
      Assert.That(orderItemsEndPoint1.Collection,
          Is.EquivalentTo(new[] { DomainObjectIDs.OrderItem1.GetObject<OrderItem>(), DomainObjectIDs.OrderItem2.GetObject<OrderItem>() }));

      var orderEndPoint = (IObjectEndPoint)TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(
          RelationEndPointID.Create(DomainObjectIDs.OrderItem1, typeof(OrderItem), "Order"));
      Assert.That(orderEndPoint, Is.Not.Null);
      Assert.That(orderEndPoint.IsSynchronized, Is.True);
      Assert.That(orderEndPoint.OppositeObjectID, Is.EqualTo(DomainObjectIDs.Order1));

      var orderItemsEndPoint2 = (IDomainObjectCollectionEndPoint)TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(id2);
      Assert.That(orderItemsEndPoint2, Is.Not.Null);
      Assert.That(orderItemsEndPoint2.IsSynchronized, Is.True);
      Assert.That(orderItemsEndPoint2.Collection, Is.EquivalentTo(new[] { DomainObjectIDs.OrderItem3.GetObject<OrderItem>() }));
    }

    [Test]
    public void EagerFetching_WithDomainObject ()
    {
      var employeeWithComputer = DomainObjectIDs.Employee3;
      var employeeWithoutComputer = DomainObjectIDs.Employee1;
      var existingComputer = DomainObjectIDs.Computer1;
      var employeeQuery = CreateEmployeeQuery(string.Format("ID IN ('{0}', '{1}')", employeeWithComputer.Value, employeeWithoutComputer.Value));
      AddEmployeeComputerFetchQuery(employeeQuery, string.Format("e.ID IN ('{0}', '{1}')", employeeWithComputer.Value, employeeWithoutComputer.Value));

      var id1 = RelationEndPointID.Create(employeeWithComputer, typeof(Employee), "Computer");
      var id2 = RelationEndPointID.Create(employeeWithoutComputer, typeof(Employee), "Computer");

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(id1), Is.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(id2), Is.Null);

      var result = TestableClientTransaction.QueryManager.GetCollection(employeeQuery);
      Assert.That(
          result.ToArray(),
          Is.EquivalentTo(new[] { employeeWithoutComputer.GetObject<Employee>(), employeeWithComputer.GetObject<Employee>() }));

      var existingComputerEndPoint = (IVirtualObjectEndPoint)TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(id1);
      Assert.That(existingComputerEndPoint, Is.Not.Null);
      Assert.That(existingComputerEndPoint.IsSynchronized, Is.True);
      Assert.That(existingComputerEndPoint.OppositeObjectID, Is.EqualTo(existingComputer));
      Assert.That(existingComputerEndPoint.GetOppositeObject(), Is.EqualTo(existingComputer.GetObject<Computer>()));

      var employeeEndPoint = (IObjectEndPoint)TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(
          RelationEndPointID.Create(existingComputer, typeof(Computer), "Employee"));
      Assert.That(employeeEndPoint, Is.Not.Null);
      Assert.That(employeeEndPoint.IsSynchronized, Is.True);
      Assert.That(employeeEndPoint.OppositeObjectID, Is.EqualTo(employeeWithComputer));

      var missingComputerEndPoint = (IVirtualObjectEndPoint)TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(id2);
      Assert.That(missingComputerEndPoint, Is.Not.Null);
      Assert.That(missingComputerEndPoint.IsSynchronized, Is.True);
      Assert.That(missingComputerEndPoint.OppositeObjectID, Is.Null);
      Assert.That(missingComputerEndPoint.GetOppositeObject(), Is.Null);
    }

    [Test]
    public void EagerFetching_WithExistingRelationData ()
    {
      // Load relation data prior to executing the query.
      var customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      Assert.That(
          customer.Orders,
          Is.EquivalentTo(new[] { DomainObjectIDs.Order1.GetObject<Order>(), DomainObjectIDs.Order2.GetObject<Order>() }));

      var customerQuery = CreateCustomerQuery("ID = '" + DomainObjectIDs.Customer1.Value + "'");
      // This will return a different (empty) relation collection.
      AddOrderFetchQuery(customerQuery, "1 = 0");

      // This executes the fetch query, but should discard the result (since the relation data already exists).
      TestableClientTransaction.QueryManager.GetCollection(customerQuery);

      Assert.That(
          customer.Orders,
          Is.EquivalentTo(new[] { DomainObjectIDs.Order1.GetObject<Order>(), DomainObjectIDs.Order2.GetObject<Order>() }));
    }

    [Test]
    public void EagerFetching_UsesForeignKeyDataFromDatabase_NotTransaction ()
    {
      // This will retrieve Order1.
      var ordersQuery = CreateOrdersQuery("OrderNo = 1");
      // This will fetch OrderItem1 and OrderItem2, both pointing to Order1.
      AddOrderItemsFetchQuery(ordersQuery, "o.OrderNo = 1");

      // Fake OrderItem2 to point to Order3 in memory.
      var orderItem2 = RegisterFakeOrderItem(DomainObjectIDs.OrderItem2, DomainObjectIDs.Order3);

      var result = TestableClientTransaction.QueryManager.GetCollection(ordersQuery);

      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      Assert.That(result.ToArray(), Is.EquivalentTo(new[] { order1 }));

      var orderItemsEndPointID = RelationEndPointID.Resolve(order1, o => o.OrderItems);
      var orderItemsEndPoint = TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(orderItemsEndPointID);
      Assert.That(orderItemsEndPoint, Is.Not.Null);
      Assert.That(orderItemsEndPoint.IsDataComplete, Is.True);

      // The relation contains the fetched result, disregarding the in-memory data. This makes it an unsynchronized relation.
      Assert.That(
          order1.OrderItems,
          Is.EquivalentTo(new[] { DomainObjectIDs.OrderItem1.GetObject<OrderItem>(), orderItem2 }));

      Assert.That(orderItem2.Order, Is.Not.SameAs(order1));
      Assert.That(BidirectionalRelationSyncService.IsSynchronized(TestableClientTransaction, orderItemsEndPointID), Is.False);
    }

    [Test]
    public void EagerFetching_OnLoadedMethod_CanAlreadyAccessFetchedRelation_WithoutGoingToThePersistenceLayer ()
    {
      var ordersQuery = CreateOrdersQuery("OrderNo IN (1)");
      AddOrderItemsFetchQuery(ordersQuery, "o.OrderNo IN (1)");

      var id1 = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");

      bool onLoadedRaised = false;

      var order1Reference = DomainObjectIDs.Order1.GetObjectReference<Order>();
      order1Reference.ProtectedLoaded += (sender, args) =>
      {
        var persistenceExtensionMock = new Mock<IPersistenceExtension>(MockBehavior.Strict);
        using (ScopeWithPersistenceExtension(persistenceExtensionMock.Object))
        {
          Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(id1), Is.Not.Null);
          Assert.That(
              order1Reference.OrderItems,
              Is.EquivalentTo(
                  new[] { DomainObjectIDs.OrderItem1.GetObjectReference<OrderItem>(), DomainObjectIDs.OrderItem2.GetObjectReference<OrderItem>() }));

          persistenceExtensionMock.Verify(mock => mock.ConnectionOpened(It.IsAny<Guid>()), Times.Never());
        }
        onLoadedRaised = true;
      };

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(id1), Is.Null);

      TestableClientTransaction.QueryManager.GetCollection(ordersQuery);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(id1), Is.Not.Null);
      Assert.That(onLoadedRaised, Is.True);
    }

    [Test]
    public void EagerFetching_FetchingSameObjectsMultipleTimes_DoesntThrow ()
    {
      var ordersQuery = CreateOrdersQuery("o.OrderNo IN (1)");
      var orderItemsFetchQuery = AddOrderItemsFetchQuery(ordersQuery, "o.OrderNo IN (1)");
      AddOrderFetchAgainQuery(orderItemsFetchQuery, "o.OrderNo IN (1)");

      Assert.That(() => TestableClientTransaction.QueryManager.GetCollection(ordersQuery), Throws.Nothing);
    }

    private OrderItem RegisterFakeOrderItem (ObjectID objectID, ObjectID fakeOrderID)
    {
      var orderItem = (OrderItem)LifetimeService.GetObjectReference(TestableClientTransaction, objectID);
      var fakeDataContainer = DataContainer.CreateForExisting(
          orderItem.ID,
          null,
          pd => pd.PropertyName.EndsWith("Order") ? fakeOrderID : pd.DefaultValue);
      fakeDataContainer.SetDomainObject(orderItem);
      ClientTransactionTestHelper.RegisterDataContainer(TestableClientTransaction, fakeDataContainer);
      return orderItem;
    }

    private IQuery CreateEmployeeQuery (string whereCondition)
    {
      return QueryFactory.CreateCollectionQuery(
          "test",
          TestDomainStorageProviderDefinition,
          "SELECT * FROM [Employee] o WHERE " + whereCondition,
          new QueryParameterCollection(),
          typeof(DomainObjectCollection));
    }

    private IQuery CreateOrdersQuery (string whereCondition)
    {
      return QueryFactory.CreateCollectionQuery(
          "test",
          TestDomainStorageProviderDefinition,
          "SELECT * FROM [Order] o WHERE " + whereCondition,
          new QueryParameterCollection(),
          typeof(DomainObjectCollection));
    }

    private IQuery CreateCustomerQuery (string whereCondition)
    {
      return QueryFactory.CreateCollectionQuery(
          "test",
          TestDomainStorageProviderDefinition,
          "SELECT * FROM [Company] c WHERE c.ClassID='Customer' AND " + whereCondition,
          new QueryParameterCollection(),
          typeof(DomainObjectCollection));
    }

    private IQuery AddOrderItemsFetchQuery (IQuery ordersQuery, string whereCondition)
    {
      var relationEndPointDefinition = GetEndPointDefinition(typeof(Order), "OrderItems");

      var orderItemsFetchQuery = QueryFactory.CreateCollectionQuery(
          "test fetch",
          TestDomainStorageProviderDefinition,
          "SELECT oi.* FROM [Order] o LEFT OUTER JOIN OrderItem oi ON o.ID = oi.OrderID WHERE " + whereCondition,
          new QueryParameterCollection(),
          typeof(DomainObjectCollection));
      ordersQuery.EagerFetchQueries.Add(relationEndPointDefinition, orderItemsFetchQuery);
      return orderItemsFetchQuery;
    }

    private IQuery AddEmployeeComputerFetchQuery (IQuery ordersQuery, string whereCondition)
    {
      var relationEndPointDefinition = GetEndPointDefinition(typeof(Employee), "Computer");

      var orderItemsFetchQuery = QueryFactory.CreateCollectionQuery(
          "test fetch",
          TestDomainStorageProviderDefinition,
          "SELECT c.* FROM [Employee] e LEFT OUTER JOIN Computer c ON e.ID = c.EmployeeID WHERE " + whereCondition,
          new QueryParameterCollection(),
          typeof(DomainObjectCollection));
      ordersQuery.EagerFetchQueries.Add(relationEndPointDefinition, orderItemsFetchQuery);
      return orderItemsFetchQuery;
    }

    private IQuery AddOrderFetchQuery (IQuery customerQuery, string whereCondition)
    {
      var relationEndPointDefinition = GetEndPointDefinition(typeof(Customer), "Orders");

      var orderItemsFetchQuery = QueryFactory.CreateCollectionQuery(
          "test fetch",
          TestDomainStorageProviderDefinition,
          "SELECT o.* FROM [Company] c LEFT OUTER JOIN [Order] o ON c.ID = o.CustomerID WHERE " + whereCondition,
          new QueryParameterCollection(),
          typeof(DomainObjectCollection));
      customerQuery.EagerFetchQueries.Add(relationEndPointDefinition, orderItemsFetchQuery);
      return orderItemsFetchQuery;
    }

    private void AddOrderFetchAgainQuery (IQuery orderItemsFetchQuery, string originalOrderWhereCondition)
    {
      var relationEndPointDefinition = GetEndPointDefinition(typeof(OrderItem), "Order");

      var ordersFetchQuery = QueryFactory.CreateCollectionQuery(
          "test fetch",
          TestDomainStorageProviderDefinition,
          "SELECT DISTINCT o2.* "
          + "FROM [Order] o "
          + "LEFT OUTER JOIN OrderItem oi ON o.ID = oi.OrderID "
          + "LEFT OUTER JOIN [Order] o2 ON o2.ID = oi.OrderID "
          + "WHERE " + originalOrderWhereCondition,
          new QueryParameterCollection(),
          typeof(DomainObjectCollection));
      orderItemsFetchQuery.EagerFetchQueries.Add(relationEndPointDefinition, ordersFetchQuery);
    }

    private ServiceLocatorScope ScopeWithPersistenceExtension (IPersistenceExtension persistenceExtensionMock)
    {
      var persistenceExtensionFactoryStub = new Mock<IPersistenceExtensionFactory>();
      persistenceExtensionFactoryStub
          .Setup(stub => stub.CreatePersistenceExtensions(It.IsAny<Guid>()))
          .Returns(new[] { persistenceExtensionMock });
      var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      serviceLocator.RegisterSingle<IPersistenceExtensionFactory>(() => persistenceExtensionFactoryStub.Object);
      RegisterStandardConfiguration(serviceLocator);
      return new ServiceLocatorScope(serviceLocator);
    }
  }
}
