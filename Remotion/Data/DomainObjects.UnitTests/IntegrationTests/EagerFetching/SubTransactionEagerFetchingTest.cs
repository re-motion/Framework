// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.EagerFetching
{
  [TestFixture]
  public class SubTransactionEagerFetchingTest : ClientTransactionBaseTest
  {
    [Test]
    public void EagerFetching ()
    {
      var ordersQuery = QueryFactory.CreateCollectionQuery (
          "test",
          TestDomainStorageProviderDefinition,
          "SELECT * FROM [Order] WHERE OrderNo IN (1, 3)",
          new QueryParameterCollection(),
          typeof (DomainObjectCollection));

      var relationEndPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");

      var orderItemsFetchQuery = QueryFactory.CreateCollectionQuery (
          "test fetch",
          TestDomainStorageProviderDefinition,
          "SELECT oi.* FROM [Order] o LEFT OUTER JOIN OrderItem oi ON o.ID = oi.OrderID WHERE o.OrderNo IN (1, 3)",
          new QueryParameterCollection(),
          typeof (DomainObjectCollection));
      ordersQuery.EagerFetchQueries.Add (relationEndPointDefinition, orderItemsFetchQuery);

      var id1 = RelationEndPointID.Create (DomainObjectIDs.Order1, relationEndPointDefinition);
      var id2 = RelationEndPointID.Create (DomainObjectIDs.Order3, relationEndPointDefinition);

      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (id1), Is.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (id2), Is.Null);

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var result = ClientTransaction.Current.QueryManager.GetCollection (ordersQuery);
        Assert.That (
            result.ToArray(), Is.EquivalentTo (new[] { DomainObjectIDs.Order1.GetObject<Order>(), DomainObjectIDs.Order3.GetObject<Order>() }));

        var subDataManager = ClientTransactionTestHelper.GetIDataManager (ClientTransaction.Current);
        Assert.That (subDataManager.GetRelationEndPointWithoutLoading (id1), Is.Null);
        Assert.That (subDataManager.GetRelationEndPointWithoutLoading (id2), Is.Null);
      }

      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (id1), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (id2), Is.Not.Null);

      Assert.That (
          ((ICollectionEndPoint) TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (id1)).Collection,
          Is.EquivalentTo (new[] { DomainObjectIDs.OrderItem1.GetObject<OrderItem>(), DomainObjectIDs.OrderItem2.GetObject<OrderItem>() }));
      Assert.That (
          ((ICollectionEndPoint) TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (id2)).Collection,
          Is.EquivalentTo (new[] { DomainObjectIDs.OrderItem3.GetObject<OrderItem>() }));
    }

    [Test]
    public void EagerFetching_OnlyExecutesQueryOnce ()
    {
      var outerQuery = QueryFactory.CreateCollectionQuery (
          "test",
          TestDomainStorageProviderDefinition,
          "outerQuery",
          new QueryParameterCollection(),
          typeof (DomainObjectCollection));

      var relationEndPointDefinition = GetEndPointDefinition (typeof (Customer), "Orders");

      var fetchQuery = QueryFactory.CreateCollectionQuery (
          "test fetch",
          TestDomainStorageProviderDefinition,
          "fetchQuery",
          new QueryParameterCollection(),
          typeof (DomainObjectCollection));
      outerQuery.EagerFetchQueries.Add (relationEndPointDefinition, fetchQuery);

      var persistenceStrategyMock = MockRepository.GenerateStrictMock<IFetchEnabledPersistenceStrategy>();
      var customerDataContainer = TestDataContainerObjectMother.CreateCustomer1DataContainer();
      var orderDataContainer = TestDataContainerObjectMother.CreateOrder1DataContainer();
      persistenceStrategyMock
          .Expect (mock => mock.ExecuteCollectionQuery (Arg.Is (outerQuery), Arg<ILoadedObjectDataProvider>.Is.Anything))
          .Return (new[] { new FreshlyLoadedObjectData (customerDataContainer) });
      persistenceStrategyMock
          .Expect (mock => mock.ExecuteFetchQuery (Arg.Is (fetchQuery), Arg<ILoadedObjectDataProvider>.Is.Anything))
          .Return (new[] { new LoadedObjectDataWithDataSourceData (new FreshlyLoadedObjectData (orderDataContainer), orderDataContainer) })
          .Repeat.Once();
      persistenceStrategyMock.Replay();

      var clientTransaction = ClientTransactionObjectMother.CreateTransactionWithPersistenceStrategy<ClientTransaction> (persistenceStrategyMock);
      using (clientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var result = ClientTransaction.Current.QueryManager.GetCollection<Customer> (outerQuery).ToArray();
        Assert.That (result, Is.EquivalentTo (new[] { DomainObjectIDs.Customer1.GetObject<Customer>() }));
        Assert.That (result[0].Orders, Is.EquivalentTo (new[] { DomainObjectIDs.Order1.GetObject<Order>() }));
      }

      persistenceStrategyMock.VerifyAllExpectations();
    }

    [Test]
    public void EagerFetching_UsesRelationDataFromParent_InsteadOfFetching ()
    {
      // Load - and change - relation data prior to executing the query
      var customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      Assert.That (
          customer.Orders,
          Is.EquivalentTo (new[] { DomainObjectIDs.Order1.GetObject<Order>(), DomainObjectIDs.Order2.GetObject<Order>() }));

      var customersQuery = QueryFactory.CreateCollectionQuery (
          "test",
          TestDomainStorageProviderDefinition,
          "SELECT * FROM [Company] WHERE ID = '" + DomainObjectIDs.Customer1.Value + "'",
          new QueryParameterCollection(),
          typeof (DomainObjectCollection));

      var relationEndPointDefinition = GetEndPointDefinition (typeof (Customer), "Orders");

      // This will yield different orders (none) than the actual relation query above - simulating the database has changed in between
      var orderItemsFetchQuery = QueryFactory.CreateCollectionQuery (
          "test fetch",
          TestDomainStorageProviderDefinition,
          "SELECT NULL WHERE 1 = 0",
          new QueryParameterCollection(),
          typeof (DomainObjectCollection));
      customersQuery.EagerFetchQueries.Add (relationEndPointDefinition, orderItemsFetchQuery);

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        // This executes the fetch query, but should discard the result (since the relation data already exists in the parent transaction)
        ClientTransaction.Current.QueryManager.GetCollection (customersQuery);

        Assert.That (
            customer.Orders,
            Is.EquivalentTo (new[] { DomainObjectIDs.Order1.GetObject<Order>(), DomainObjectIDs.Order2.GetObject<Order>() }));
      }
    }
  }
}