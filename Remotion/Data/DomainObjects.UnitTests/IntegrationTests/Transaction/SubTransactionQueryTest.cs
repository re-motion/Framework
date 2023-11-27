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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionQueryTest : ClientTransactionBaseTest
  {
    [Test]
    public void ScalarQueryInSubTransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var query = QueryFactory.CreateQuery(Queries.GetMandatory("QueryWithoutParameter"));

        Assert.That(ClientTransactionScope.CurrentTransaction.QueryManager.GetScalar(query), Is.EqualTo(42));
      }
    }

    [Test]
    public void CustomQueryInSubTransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var query = QueryFactory.CreateQuery(Queries.GetMandatory("CustomQuery"));

        var result = ClientTransactionScope.CurrentTransaction.QueryManager.GetCustom(query, qrr => qrr.GetRawValue(0));

        CollectionAssert.AreEquivalent(new[] { "üäöfedcba", "abcdeföäü" }, result);
      }
    }

    [Test]
    public void ObjectQueryInSubTransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var query = QueryFactory.CreateQuery(Queries.GetMandatory("CustomerTypeQuery"));
        query.Parameters.Add("@customerType", Customer.CustomerType.Standard);

        var queriedObjects = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(query);
        var array = queriedObjects.ToArray();
        var queriedObject = (Customer)array[0];

        Assert.That(queriedObjects, Is.Not.Null);
        Assert.That(queriedObjects.Count, Is.EqualTo(1));
        Assert.That(array[0].ID, Is.EqualTo(DomainObjectIDs.Customer1));

        Assert.That(queriedObject.CustomerSince, Is.EqualTo(new DateTime(2000, 1, 1)));
        Assert.That(queriedObject.Orders[0], Is.SameAs(DomainObjectIDs.Order1.GetObject<Order>()));
      }
    }

    [Test]
    public void ObjectQueryWithObjectListInSubTransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var query = QueryFactory.CreateQuery(Queries.GetMandatory("CustomerTypeQuery"));
        query.Parameters.Add("@customerType", Customer.CustomerType.Standard);

        var queriedObjects = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection<Customer>(query);
        var array = queriedObjects.ToArray();
        Customer queriedObject = array[0];

        Assert.That(queriedObjects, Is.Not.Null);
        Assert.That(queriedObjects.Count, Is.EqualTo(1));
        Assert.That(array[0].ID, Is.EqualTo(DomainObjectIDs.Customer1));

        Assert.That(queriedObject.CustomerSince, Is.EqualTo(new DateTime(2000, 1, 1)));
        Assert.That(queriedObject.Orders[0], Is.SameAs(DomainObjectIDs.Order1.GetObject<Order>()));
      }
    }

    [Test]
    public void ObjectQueryInSubAndRootTransaction ()
    {
      IQueryResult queriedObjectsInSub;
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var query = QueryFactory.CreateQuery(Queries.GetMandatory("CustomerTypeQuery"));
        query.Parameters.Add("@customerType", Customer.CustomerType.Standard);

        queriedObjectsInSub = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(query);
      }

      var queryInRoot = QueryFactory.CreateQuery(Queries.GetMandatory("CustomerTypeQuery"));
      queryInRoot.Parameters.Add("@customerType", Customer.CustomerType.Standard);

      IQueryResult queriedObjectsInRoot = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(queryInRoot);
      Assert.That(queriedObjectsInRoot.ToArray(), Is.EqualTo(queriedObjectsInSub.ToArray()));
    }

    [Test]
    public void QueriedObjectsCanBeUsedInParentTransaction ()
    {
      IQueryResult queriedObjects;

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var query = QueryFactory.CreateQuery(Queries.GetMandatory("CustomerTypeQuery"));
        query.Parameters.Add("@customerType", Customer.CustomerType.Standard);

        queriedObjects = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(query);
      }

      var queriedObject = (Customer)queriedObjects.ToArray()[0];

      Assert.That(queriedObjects, Is.Not.Null);
      Assert.That(queriedObjects.Count, Is.EqualTo(1));

      Assert.That(queriedObject.ID, Is.EqualTo(DomainObjectIDs.Customer1));
      Assert.That(queriedObject.CustomerSince, Is.EqualTo(new DateTime(2000, 1, 1)));
      Assert.That(queriedObject.Orders[0], Is.SameAs(DomainObjectIDs.Order1.GetObject<Order>()));
    }

    [Test]
    public void ChangedComittedQueriedObjectsCanBeUsedInParentTransaction ()
    {
      IQueryResult queriedObjects;
      Customer queriedObject;

      Order newOrder;
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var query = QueryFactory.CreateQuery(Queries.GetMandatory("CustomerTypeQuery"));
        query.Parameters.Add("@customerType", Customer.CustomerType.Standard);

        queriedObjects = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(query);
        queriedObject = (Customer)queriedObjects.ToArray() [0];

        newOrder = Order.NewObject();
        newOrder.Official = Official.NewObject();
        newOrder.OrderTicket = OrderTicket.NewObject();
        newOrder.OrderItems.Add(OrderItem.NewObject());
        queriedObject.Orders.Insert(0, newOrder);
        queriedObject.CustomerSince = null;

        ClientTransactionScope.CurrentTransaction.Commit();
      }

      Assert.That(queriedObjects, Is.Not.Null);
      Assert.That(queriedObjects.Count, Is.EqualTo(1));
      Assert.That(queriedObjects.ToArray()[0].ID, Is.EqualTo(DomainObjectIDs.Customer1));

      Assert.That(queriedObject.CustomerSince, Is.Null);
      Assert.That(queriedObject.Orders[0], Is.SameAs(newOrder));
    }

    [Test]
    public void AccessObjectInFilterQueryResult ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var extensionMock = new Mock<IClientTransactionExtension>();

        DomainObjectIDs.Order1.GetObject<Order>();
        extensionMock.Setup(stub => stub.Key).Returns("stub");
        TestableClientTransaction.Extensions.Add(extensionMock.Object);
        try
        {
          extensionMock.Reset();

          var query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderQuery"));
          query.Parameters.Add("@customerID", DomainObjectIDs.Customer3);

          var newQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject>();

          extensionMock
              .Setup(mock => mock.FilterQueryResult(It.IsAny<ClientTransaction>(), It.IsAny<QueryResult<DomainObject>>()))
              .Callback((ClientTransaction clientTransaction, QueryResult<DomainObject> queryResult) => DomainObjectIDs.Order1.GetObject<Order>())
              .Returns(newQueryResult)
              .Verifiable();

          ClientTransaction.Current.QueryManager.GetCollection(query);
          extensionMock.Verify();
        }
        finally
        {
          TestableClientTransaction.Extensions.Remove("stub");
        }
      }
    }

    [Test]
    public void QueryInSubtransaction_CausesObjectsInSubtransactionToBeLoaded ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderQuery"));
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer4);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var finalResult = ClientTransaction.Current.QueryManager.GetCollection(query);
        var loadedObjects = finalResult.ToArray();

        Assert.That(loadedObjects.Length, Is.EqualTo(2));
        Assert.That(loadedObjects[0].State.IsUnchanged, Is.True);
        Assert.That(loadedObjects[1].State.IsUnchanged, Is.True);
      }
    }

    [Test]
    public void QueryInSubtransaction_CausesObjectsInSubtransactionToBeLoaded_WhenKnownInParent ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderQuery"));
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer4);

      var result = ClientTransaction.Current.QueryManager.GetCollection(query).ToArray(); // preload query result in parent transaction
      Assert.That(result.Length, Is.EqualTo(2));
      Assert.That(result[0].State.IsUnchanged, Is.True);
      Assert.That(result[1].State.IsUnchanged, Is.True);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var finalResult = ClientTransaction.Current.QueryManager.GetCollection(query);
        var loadedObjects = finalResult.ToArray();

        Assert.That(loadedObjects[0].State.IsUnchanged, Is.True);
        Assert.That(loadedObjects[1].State.IsUnchanged, Is.True);
      }
    }

    [Test]
    public void QueryInSubtransaction_ReturningObjectDeletedInParentTransaction ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderQuery"));
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer3);

      var outerResult = ClientTransaction.Current.QueryManager.GetCollection<Order>(query).ToArray();
      Assert.That(outerResult.Length, Is.EqualTo(1));

      outerResult[0].Delete();
      Assert.That(outerResult[0].State.IsDeleted, Is.True);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var finalResult = ClientTransaction.Current.QueryManager.GetCollection(query);
        var loadedObjects = finalResult.ToArray();

        Assert.That(loadedObjects[0], Is.SameAs(outerResult[0]));
        Assert.That(loadedObjects[0].State.IsInvalid, Is.True);
      }
    }

    [Test]
    public void QueryInSubtransaction_ReturningObjectInvalidInParentTransaction ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderQuery"));
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer3);

      var outerResult = ClientTransaction.Current.QueryManager.GetCollection<Order>(query).ToArray();
      Assert.That(outerResult.Length, Is.EqualTo(1));

      outerResult[0].Delete();
      Assert.That(outerResult[0].State.IsDeleted, Is.True);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(outerResult[0].State.IsInvalid, Is.True);

        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          var finalResult = ClientTransaction.Current.QueryManager.GetCollection(query);
          var loadedObjects = finalResult.ToArray();

          Assert.That(loadedObjects[0], Is.SameAs(outerResult[0]));
          Assert.That(loadedObjects[0].State.IsInvalid, Is.True);
        }
      }
    }

    [Test]
    public void GetCollectionWithNullValues ()
    {
      var query = QueryFactory.CreateCollectionQuery(
          "test",
          DomainObjectIDs.Computer1.ClassDefinition.StorageEntityDefinition.StorageProviderDefinition,
          "SELECT [Employee].* FROM [Computer] LEFT OUTER JOIN [Employee] ON [Computer].[EmployeeID] = [Employee].[ID] "
          + "WHERE [Computer].[ID] IN (@1, @2, @3) "
          + "ORDER BY [Computer].[ID] asc",
          new QueryParameterCollection(),
          typeof(DomainObjectCollection));

      query.Parameters.Add("@1", DomainObjectIDs.Computer5); // no employee
      query.Parameters.Add("@3", DomainObjectIDs.Computer4); // no employee
      query.Parameters.Add("@2", DomainObjectIDs.Computer1); // employee 3

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var result = ClientTransaction.Current.QueryManager.GetCollection(query);
        Assert.That(result.ContainsNulls(), Is.True);
        Assert.That(result.ToArray(), Is.EqualTo(new[] { null, null, DomainObjectIDs.Employee3.GetObject<Employee>() }));
      }
    }
  }
}
