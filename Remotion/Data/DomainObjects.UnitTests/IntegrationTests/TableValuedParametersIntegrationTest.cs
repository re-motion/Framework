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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests;

[TestFixture]
public class TableValuedParametersIntegrationTest : ClientTransactionBaseTest
{
  [Test]
  public void Contains_WithEmptyCollection ()
  {
    var possibleItems = SetupCollection<ICollection<Guid>>([]);

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT * FROM [Order] o WHERE o.[ID] IN (SELECT [Value] FROM @1)", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", possibleItems } });

    var queryResult = TestableClientTransaction.QueryManager.GetCollection<Order>(query);
    CheckQueryResult(queryResult, []);
  }

  [Test]
  public void Contains_WithCollectionOfT ()
  {
    var possibleItems = SetupCollection<ICollection<Guid>>([ (Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value, (Guid)DomainObjectIDs.Order3.Value ]);

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT * FROM [Order] o WHERE o.[ID] IN (SELECT [Value] FROM @1)", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", possibleItems } });

    var queryResult = TestableClientTransaction.QueryManager.GetCollection<Order>(query);
    CheckQueryResult(queryResult, DomainObjectIDs.Order1, DomainObjectIDs.Order3);
  }

  [Test]
  public void Contains_WithReadOnlyCollectionOfT ()
  {
    var possibleItems = SetupCollection<IReadOnlyCollection<Guid>>([ (Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value, (Guid)DomainObjectIDs.Order3.Value ]);

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT * FROM [Order] o WHERE o.[ID] IN (SELECT [Value] FROM @1)", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", possibleItems } });

    var queryResult = TestableClientTransaction.QueryManager.GetCollection<Order>(query);
    CheckQueryResult(queryResult, DomainObjectIDs.Order1, DomainObjectIDs.Order3);
  }

  [Test]
  public void Contains_WithPlainCollection ()
  {
    var possibleItems = (IEnumerable<Guid>)SetupCollection<ICollection>([ (Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value, (Guid)DomainObjectIDs.Order3.Value ]);

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT * FROM [Order] o WHERE o.[ID] IN (SELECT [Value] FROM @1)", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", possibleItems } });

    var queryResult = TestableClientTransaction.QueryManager.GetCollection<Order>(query);
    CheckQueryResult(queryResult, DomainObjectIDs.Order1, DomainObjectIDs.Order3);
  }

  [Test]
  public void Contains_WithSetOfT ()
  {
    var possibleItems = SetupCollection<ISet<Guid>>([(Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value]);

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT * FROM [Order] o WHERE o.[ID] IN (SELECT [Value] FROM @1)", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", possibleItems } });

    var queryResult = TestableClientTransaction.QueryManager.GetCollection<Order>(query);
    CheckQueryResult(queryResult, DomainObjectIDs.Order1, DomainObjectIDs.Order3);
  }

  [Test]
  public void Contains_WithSetOfT_UsesTableTypeWithUniqueConstraint ()
  {
    // set up a set with duplicates to get a database exception when we insert these duplicates into a table with unique constraint
    var possibleItems = SetupCollection<ISet<Guid>>([(Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value, (Guid)DomainObjectIDs.Order3.Value]);

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT * FROM [Order] o WHERE o.[ID] IN (SELECT [Value] FROM @1)", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", possibleItems } });

    Assert.That(() => TestableClientTransaction.QueryManager.GetCollection<Order>(query),
        Throws.InstanceOf<RdbmsProviderException>()
            .With.Message.Contains("Error while executing SQL command: Violation of UNIQUE KEY constraint")
            .With.Message.Contains("Cannot insert duplicate key in object 'dbo.@1'. The duplicate key value is (83445473-844a-4d3f-a8c3-c27f8d98e8ba)."));
  }

  [Test]
  public void Contains_WithReadOnlySetOfT ()
  {
    var possibleItems = SetupCollection<IReadOnlySet<Guid>>([ (Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value ]);

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT * FROM [Order] o WHERE o.[ID] IN (SELECT [Value] FROM @1)", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", possibleItems } });

    var queryResult = TestableClientTransaction.QueryManager.GetCollection<Order>(query);
    CheckQueryResult(queryResult, DomainObjectIDs.Order1, DomainObjectIDs.Order3);
  }

  [Test]
  public void Contains_WithReadOnlySetOfT_UsesTableTypeWithUniqueConstraint ()
  {
    // set up a set with duplicates to get a database exception when we insert these duplicates into a table with unique constraint
    var possibleItems = SetupCollection<IReadOnlySet<Guid>>([(Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value, (Guid)DomainObjectIDs.Order3.Value]);

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT * FROM [Order] o WHERE o.[ID] IN (SELECT [Value] FROM @1)", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", possibleItems } });

    Assert.That(() => TestableClientTransaction.QueryManager.GetCollection<Order>(query).ToArray(),
        Throws.InstanceOf<RdbmsProviderException>()
            .With.Message.Contains("Error while executing SQL command: Violation of UNIQUE KEY constraint")
            .With.Message.Contains("Cannot insert duplicate key in object 'dbo.@1'. The duplicate key value is (83445473-844a-4d3f-a8c3-c27f8d98e8ba)."));
  }

  [Test]
  public void Contains_UsingTableValuedParameter_NullValue ()
  {
    var orderNumbers = new int?[] { 2, null, 99 };

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT * FROM [Order] o WHERE o.[OrderNumber] IN (SELECT [Value] FROM @1)", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", orderNumbers } });

    Assert.That(
        () => TestableClientTransaction.QueryManager.GetCollection<Order>(query).ToArray(),
        Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo(
            "Items within enumerable parameter values must not be null, because this would result in table rows with all columns being NULL. "
            + "This does not work with WHERE IN, and is not useful in JOIN scenarios, wherefore it is not supported."));
  }

  [Test]
  public void Contains_UsingTableValuedParameter_Int ()
  {
    var orderNumbers = new[] { 2, 99 };

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT * FROM [Order] o WHERE o.[OrderNo] IN (SELECT [Value] FROM @1)", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", orderNumbers } });

    var queryResult = TestableClientTransaction.QueryManager.GetCollection<Order>(query);
    CheckQueryResult(
        queryResult,
        new ObjectID(typeof(Order), new Guid("F4016F41-F4E4-429E-B8D1-659C8C480A67")),
        new ObjectID(typeof(Order), new Guid("F7607CBC-AB34-465C-B282-0531D51F3B04")));
  }

  [Test]
  public void Contains_UsingTableValuedParameter_DateTime ()
  {
    var deliveryDates = new[] { new DateTime(2005,2,1), new DateTime(2013,3,7) };

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT * FROM [Order] o WHERE o.[DeliveryDate] IN (SELECT [Value] FROM @1)", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", deliveryDates } });

    var queryResult = TestableClientTransaction.QueryManager.GetCollection<Order>(query);
    CheckQueryResult(
        queryResult,
        new ObjectID(typeof(Order), new Guid("F4016F41-F4E4-429E-B8D1-659C8C480A67")),
        new ObjectID(typeof(Order), new Guid("F7607CBC-AB34-465C-B282-0531D51F3B04")));
  }

  [Test]
  public void Contains_UsingTableValuedParameter_String ()
  {
    var products = new[] { "Hitchhiker's guide", "Rubik's Cube" };

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT * FROM [OrderItem] o WHERE o.[Product] IN (SELECT [Value] FROM @1)", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", products } });

    var queryResult = TestableClientTransaction.QueryManager.GetCollection<OrderItem>(query);
    CheckQueryResult(
        queryResult,
        new ObjectID(typeof(OrderItem), new Guid("DC20E0EB-4B55-4F23-89CF-6D6478F96D3B")),
        new ObjectID(typeof(OrderItem), new Guid("386D99F9-B0BA-4C55-8F22-BF194A3D745A")));
  }

  [Test]
  public void Contains_UsingTableValuedParameter_Decimal ()
  {
    var prices = new[] { 0.01m, 1m };

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT * FROM [Product] o WHERE o.[Price] IN (SELECT [Value] FROM @1)", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", prices } });

    var queryResult = TestableClientTransaction.QueryManager.GetCollection<Product>(query);
    CheckQueryResult(
        queryResult,
        new ObjectID(typeof(Product), new Guid("8DD65EF7-BDDA-433E-B081-725B4D53317D")),
        new ObjectID(typeof(Product), new Guid("BA684594-CF77-4009-A010-B70B30396A01")));
  }

  [Test]
  public void Contains_UsingTableValuedParameter_Enum ()
  {
    var values = new[] { ClassWithAllDataTypes.EnumType.Value1 };

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT * FROM [TableWithAllDataTypes] o WHERE o.[Enum] IN (SELECT [Value] FROM @1)", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", values } });

    var queryResult = TestableClientTransaction.QueryManager.GetCollection<ClassWithAllDataTypes>(query);
    CheckQueryResult(
        queryResult,
        new ObjectID(typeof(ClassWithAllDataTypes), new Guid("3F647D79-0CAF-4A53-BAA7-A56831F8CE2D")));
  }

  [Test]
  public void Contains_UsingTableValuedParameter_Double ()
  {
    var values = new[] { 987654.321 };

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT * FROM [TableWithAllDataTypes] o WHERE o.[Double] IN (SELECT [Value] FROM @1)", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", values } });

    var queryResult = TestableClientTransaction.QueryManager.GetCollection<ClassWithAllDataTypes>(query);
    CheckQueryResult(
        queryResult,
        new ObjectID(typeof(ClassWithAllDataTypes), new Guid("3F647D79-0CAF-4A53-BAA7-A56831F8CE2D")));
  }

  [Test]
  public void Contains_UsingTableValuedParameter_ExtensibleEnum ()
  {
    var values = new[] { Color.Values.Blue() };

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT * FROM [TableWithAllDataTypes] o WHERE o.[ExtensibleEnum] IN (SELECT [Value] FROM @1)", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", values } });

    var queryResult = TestableClientTransaction.QueryManager.GetCollection<ClassWithAllDataTypes>(query);
    CheckQueryResult(
        queryResult,
        new ObjectID(typeof(ClassWithAllDataTypes), new Guid("583EC716-8443-4B55-92BF-09F7C8768529")));
  }

  [Test]
  public void From ()
  {
    var possibleItems = new[] { (Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value };

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT [possible].[Value] FROM @1 [possible]", QueryType.CustomReadOnly),
        new QueryParameterCollection { { "@1", possibleItems } });

    var results = TestableClientTransaction.QueryManager.GetCustom(query, row => row.GetConvertedValue<Guid>(0)).ToArray();

    Assert.That(
        results.Length,
        Is.EqualTo(possibleItems.Length),
        "Number of returned objects doesn't match; returned: " + string.Join(", ", results.Select(obj => obj.ToString())));
    Assert.That(results, Is.EquivalentTo(possibleItems));
  }

  [Test]
  public void Join ()
  {
    var possibleItems = new[] { (Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value };

    var query = QueryFactory.CreateQuery(
        new QueryDefinition("Test", TestDomainStorageProviderDefinition, "SELECT o.* FROM [Order] o INNER JOIN @1 [possible] ON o.[ID] = [possible].[Value]", QueryType.CollectionReadOnly),
        new QueryParameterCollection { { "@1", possibleItems } });

    var orders = TestableClientTransaction.QueryManager.GetCollection<Order>(query);

    CheckQueryResult(orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3);
  }

  private T SetupCollection<T> (Guid[] array)
      where T : class
  {
    var collectionStub = new Mock<T>();
    collectionStub.As<IEnumerable>().As<IEnumerable>().Setup(_ => _.GetEnumerator()).Returns(() => ((IEnumerable)array).GetEnumerator());
    collectionStub.As<IEnumerable<Guid>>().Setup(_ => _.GetEnumerator()).Returns(() => ((IEnumerable<Guid>)array).GetEnumerator());
    return collectionStub.Object;
  }

  private void CheckQueryResult<T> (QueryResult<T> queryResult, params ObjectID[] expectedObjectIDs)
      where T : DomainObject
  {
    T[] results = queryResult.ToArray();
    T[] expected = GetExpectedObjects<T>(expectedObjectIDs);
    if (expectedObjectIDs != null)
    {
      Assert.That(
          results.Length,
          Is.EqualTo(expected.Length),
          "Number of returned objects doesn't match; returned: " + string.Join(", ", results.Select(obj => obj.ID.ToString())));
    }
    Assert.That(results, Is.EquivalentTo(expected));
  }

  private T[] GetExpectedObjects<T> (params ObjectID[] expectedObjectIDs)
      where T : DomainObject
  {
    if (expectedObjectIDs == null)
      return [null];
    return (from id in expectedObjectIDs
        select (id == null ? null : (T)LifetimeService.GetObject(TestableClientTransaction, id, false))).ToArray();
  }
}
