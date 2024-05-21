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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Queries
{
  [TestFixture]
  public class CollectionQueryTest : QueryTestBase
  {
    [Test]
    public void GetCollectionWithExistingObjects ()
    {
      var computer2 = DomainObjectIDs.Computer2.GetObject<Computer>();
      var computer1 = DomainObjectIDs.Computer1.GetObject<Computer>();

      IQueryManager queryManager = QueryManager;
      var query = QueryFactory.CreateCollectionQuery("test", DomainObjectIDs.Computer1.ClassDefinition.StorageEntityDefinition.StorageProviderDefinition,
          "SELECT [Computer].* FROM [Computer] "
          + "WHERE [Computer].[ID] IN (@1, @2, @3) "
          + "ORDER BY [Computer].[ID] asc",
          new QueryParameterCollection(), typeof(DomainObjectCollection));

      query.Parameters.Add("@1", DomainObjectIDs.Computer2); // preloaded
      query.Parameters.Add("@2", DomainObjectIDs.Computer3);
      query.Parameters.Add("@3", DomainObjectIDs.Computer1); // preloaded

      var resultArray = queryManager.GetCollection(query).ToArray();
      Assert.That(resultArray, Is.EqualTo(new[] { computer2, DomainObjectIDs.Computer3.GetObject<Computer>(), computer1 }));
    }

    [Test]
    public void GetCollectionWithNullValues ()
    {
      var query = QueryFactory.CreateCollectionQuery("test", DomainObjectIDs.Computer1.ClassDefinition.StorageEntityDefinition.StorageProviderDefinition,
          "SELECT [Employee].* FROM [Computer] LEFT OUTER JOIN [Employee] ON [Computer].[EmployeeID] = [Employee].[ID] "
          + "WHERE [Computer].[ID] IN (@1, @2, @3) "
          + "ORDER BY [Computer].[ID] asc",
          new QueryParameterCollection(), typeof(DomainObjectCollection));

      query.Parameters.Add("@1", DomainObjectIDs.Computer5); // no employee
      query.Parameters.Add("@3", DomainObjectIDs.Computer4); // no employee
      query.Parameters.Add("@2", DomainObjectIDs.Computer1); // employee 3

      var result = QueryManager.GetCollection(query);
      Assert.That(result.ContainsNulls(), Is.True);
      Assert.That(result.ToArray(), Is.EqualTo(new[] { null, null, DomainObjectIDs.Employee3.GetObject<Employee>() }));
    }

    [Test]
    public void GetCollectionWithDuplicates ()
    {
      var query = QueryFactory.CreateCollectionQuery("test", DomainObjectIDs.Computer1.ClassDefinition.StorageEntityDefinition.StorageProviderDefinition,
          "SELECT [Order].* FROM [OrderItem] INNER JOIN [Order] ON [OrderItem].[OrderID] = [Order].[ID] WHERE [Order].[OrderNo] = 1",
          new QueryParameterCollection(), typeof(DomainObjectCollection));
      Assert.That(
          () => QueryManager.GetCollection(query),
          Throws.InstanceOf<RdbmsProviderException>()
              .With.Message.EqualTo(
                  "A database query returned duplicates of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', which is not allowed."));
    }

    [Test]
    public void CollectionQuery ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("CustomerTypeQuery"));
      query.Parameters.Add("@customerType", Customer.CustomerType.Standard);

      var customers = QueryManager.GetCollection(query);

      Assert.That(customers, Is.Not.Null);
      Assert.That(customers.Count, Is.EqualTo(1));
      Assert.That(customers.ToArray()[0].ID, Is.EqualTo(DomainObjectIDs.Customer1));
      Assert.That(customers.ToArray()[0].GetPublicDomainObjectType(), Is.EqualTo(typeof(Customer)));
    }

    [Test]
    public void CollectionQuery_WithObjectList ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("CustomerTypeQuery"));
      query.Parameters.Add("@customerType", Customer.CustomerType.Standard);

      var customers = QueryManager.GetCollection<Customer>(query).ToObjectList();
      Assert.That(customers, Is.Not.Null);
      Assert.That(customers.Count, Is.EqualTo(1));
      Assert.That(customers[0].ID, Is.EqualTo(DomainObjectIDs.Customer1));
      Assert.That(query.CollectionType.IsAssignableFrom(customers.GetType()), Is.True);
    }

    [Test]
    public void CollectionQuery_WithObjectList_WorksWhenAssignableCollectionType ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderByOfficialQuery"));
      query.Parameters.Add("@officialID", DomainObjectIDs.Official1);

      var orders = QueryManager.GetCollection<Order>(query).ToCustomCollection();
      Assert.That(orders.Count, Is.EqualTo(5));
      Assert.That(orders, Is.EquivalentTo(new object[]
      {
        DomainObjectIDs.Order1.GetObject<Order>(),
        DomainObjectIDs.Order3.GetObject<Order>(),
        DomainObjectIDs.Order2.GetObject<Order>(),
        DomainObjectIDs.Order4.GetObject<Order>(),
        DomainObjectIDs.Order5.GetObject<Order>(),
      }));
      Assert.That(query.CollectionType.IsAssignableFrom(orders.GetType()), Is.True);
    }

    [Test]
    public void CollectionQuery_WithObjectList_ThrowsWhenInvalidT ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("CustomerTypeQuery"));
      query.Parameters.Add("@customerType", Customer.CustomerType.Standard);
      Assert.That(
          () => QueryManager.GetCollection<Order>(query),
          Throws.InstanceOf<UnexpectedQueryResultException>()
              .With.Message.EqualTo(
                  "The query returned an object of type "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer', but a query result of type "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' was expected."));
    }

    [Test]
    public void CollectionQuery_WithObjectList_WorksWhenUnassignableCollectionType ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("QueryWithSpecificCollectionType"));

      var result = QueryManager.GetCollection<Order>(query);
      Assert.That(result.Count, Is.GreaterThan(0));
    }


    [Test]
    public void GetStoredProcedureResult ()
    {
      var orders = (OrderCollection)QueryManager.GetCollection(QueryFactory.CreateQuery(Queries.GetMandatory("StoredProcedureQuery"))).ToCustomCollection();

      Assert.That(orders, Is.Not.Null, "OrderCollection is null");
      Assert.That(orders.Count, Is.EqualTo(2), "Order count");
      Assert.That(orders[0].ID, Is.EqualTo(DomainObjectIDs.Order1), "Order1");
      Assert.That(orders[1].ID, Is.EqualTo(DomainObjectIDs.Order3), "Order3");
    }

    [Test]
    public void GetStoredProcedureResultWithParameter ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("StoredProcedureQueryWithParameter"));
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer1.Value);
      var orders = (OrderCollection)QueryManager.GetCollection(query).ToCustomCollection();

      Assert.That(orders, Is.Not.Null, "OrderCollection is null");
      Assert.That(orders.Count, Is.EqualTo(2), "Order count");
      Assert.That(orders[0].ID, Is.EqualTo(DomainObjectIDs.Order1), "Order1");
      Assert.That(orders[1].ID, Is.EqualTo(DomainObjectIDs.Order2), "Order2");
    }

    [Test]
    public void QueryingEnlists ()
    {
      DomainObjectIDs.Order1.GetObject<Order>(); // ensure Order1 already exists in transaction

      var orders = (OrderCollection)QueryManager.GetCollection(QueryFactory.CreateQuery(Queries.GetMandatory("StoredProcedureQuery"))).ToCustomCollection();
      Assert.That(orders.Count, Is.EqualTo(2), "Order count");

      foreach (Order order in orders)
        Assert.That(TestableClientTransaction.IsEnlisted(order), Is.True);

      int orderNumberSum = orders.Sum(order => order.OrderNumber);

      Assert.That(orderNumberSum, Is.EqualTo(DomainObjectIDs.Order1.GetObject<Order>().OrderNumber + DomainObjectIDs.Order3.GetObject<Order>().OrderNumber));
    }

    [Test]
    public void CollectionQuery_ReturnsDeletedObjects ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      order1.Delete(); // mark as deleted
      var order3 = DomainObjectIDs.Order3.GetObject<Order>();

      var query = QueryFactory.CreateCollectionQuery(
          "test",
          order1.ID.StorageProviderDefinition,
          "SELECT * FROM [Order] WHERE OrderNo=1 OR OrderNo=3 ORDER BY OrderNo ASC",
          new QueryParameterCollection(),
          typeof(DomainObjectCollection));
      var result = ClientTransaction.Current.QueryManager.GetCollection(query);

      Assert.That(result.Count, Is.EqualTo(2));
      Assert.That(result.ToArray(), Is.EqualTo(new[] { order1, order3 }));
      Assert.That(order1.State.IsDeleted, Is.True);
      Assert.That(order3.State.IsUnchanged, Is.True);
    }

    [Test]
    public void CollectionQuery_AllowsInvalidObjects ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      order1.Delete();
      TestableClientTransaction.DataManager.Commit();

      var query = QueryFactory.CreateCollectionQuery(
          "test",
          order1.ID.StorageProviderDefinition,
          "SELECT * FROM [Order] WHERE OrderNo=1",
          new QueryParameterCollection(),
          typeof(DomainObjectCollection));

      var result = ClientTransaction.Current.QueryManager.GetCollection(query);
      Assert.That(result.ToArray(), Is.EqualTo(new[] { order1 }));
    }

    [Test]
    public void CollectionQuery_CallsFilterQueryResult_AndAllowsGetObjectDuringFiltering ()
    {
      var listenerMock = new Mock<IClientTransactionListener>();
      listenerMock
          .Setup(mock => mock.FilterQueryResult(TestableClientTransaction, It.IsAny<QueryResult<DomainObject>>()))
          .Returns(TestQueryFactory.CreateTestQueryResult(StorageSettings))
          .Callback((ClientTransaction clientTransaction, QueryResult<DomainObject> queryResult) => DomainObjectIDs.OrderItem1.GetObject<OrderItem>())
          .Verifiable();
      TestableClientTransaction.AddListener(listenerMock.Object);

      var query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderQuery"));
      query.Parameters.Add("customerID", DomainObjectIDs.Customer1);
      TestableClientTransaction.QueryManager.GetCollection(query);

      listenerMock.Verify();
      listenerMock.Reset(); // For Discarding
    }

    [Test]
    public void QueryWithExtensibleEnums ()
    {
      var query = QueryFactory.CreateCollectionQuery("test", DomainObjectIDs.ClassWithAllDataTypes1.ClassDefinition.StorageEntityDefinition.StorageProviderDefinition,
          "SELECT [TableWithAllDataTypes].* FROM [TableWithAllDataTypes] WHERE ([TableWithAllDataTypes].[ExtensibleEnum] = @1)",
          new QueryParameterCollection(), typeof(DomainObjectCollection));

      query.Parameters.Add("@1", Color.Values.Blue());

      var result = QueryManager.GetCollection(query);
      Assert.That(result.ToArray(), Is.EqualTo(new[] { DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes>() }));
    }

  }
}
