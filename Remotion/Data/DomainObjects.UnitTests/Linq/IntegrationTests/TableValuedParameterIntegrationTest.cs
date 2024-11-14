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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests;

[TestFixture]
public class TableValuedParameterIntegrationTest : IntegrationTestBase
{
  [Test]
  public void Contains_WithEmptyCollection ()
  {
    var possibleItems = SetupCollection<ICollection<Guid>>([]);

    var orders =
        from o in QueryFactory.CreateLinqQuery<Order>()
        where possibleItems.Contains((Guid)o.ID.Value)
        select o;

    CheckQueryResult(orders, []);
  }

  [Test]
  public void Contains_WithCollectionOfT ()
  {
    var possibleItems = SetupCollection<ICollection<Guid>>([ (Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value, (Guid)DomainObjectIDs.Order3.Value ]);

    var orders =
        from o in QueryFactory.CreateLinqQuery<Order>()
        where possibleItems.Contains((Guid)o.ID.Value)
        select o;

    CheckQueryResult(orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3);
  }

  [Test]
  public void Contains_WithReadOnlyCollectionOfT ()
  {
    var possibleItems = SetupCollection<IReadOnlyCollection<Guid>>([ (Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value, (Guid)DomainObjectIDs.Order3.Value ]);

    var orders =
        from o in QueryFactory.CreateLinqQuery<Order>()
        where possibleItems.Contains((Guid)o.ID.Value)
        select o;

    CheckQueryResult(orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3);
  }

  [Test]
  public void Contains_WithPlainCollection ()
  {
    var possibleItems = (IEnumerable<Guid>)SetupCollection<ICollection>([ (Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value, (Guid)DomainObjectIDs.Order3.Value ]);

    var orders =
        from o in QueryFactory.CreateLinqQuery<Order>()
        where possibleItems.Contains((Guid)o.ID.Value)
        select o;

    CheckQueryResult(orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3);
  }

  [Test]
  public void Contains_WithSetOfT ()
  {
    var possibleItems = SetupCollection<ISet<Guid>>([(Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value]);

    var orders =
        from o in QueryFactory.CreateLinqQuery<Order>()
        where possibleItems.Contains((Guid)o.ID.Value)
        select o;

    CheckQueryResult(orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3);
  }

  [Test]
  public void Contains_WithSetOfT_UsesTableTypeWithUniqueConstraint ()
  {
    // set up a set with duplicates to get a database exception when we insert these duplicates into a table with unique constraint
    var possibleItems = SetupCollection<ISet<Guid>>([(Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value, (Guid)DomainObjectIDs.Order3.Value]);

    var orders =
        from o in QueryFactory.CreateLinqQuery<Order>()
        where possibleItems.Contains((Guid)o.ID.Value)
        select o;

    Assert.That(() => CheckQueryResult(orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3),
        Throws.InstanceOf<RdbmsProviderException>()
            .With.Message.Contains("Error while executing SQL command: Violation of UNIQUE KEY constraint")
            .With.Message.Contains("Cannot insert duplicate key in object 'dbo.@1'. The duplicate key value is (83445473-844a-4d3f-a8c3-c27f8d98e8ba)."));
  }

  [Test]
  public void Contains_WithReadOnlySetOfT ()
  {
    var possibleItems = SetupCollection<IReadOnlySet<Guid>>([ (Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value ]);

    var orders =
        from o in QueryFactory.CreateLinqQuery<Order>()
        where possibleItems.Contains((Guid)o.ID.Value)
        select o;

    CheckQueryResult(orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3);
  }

  [Test]
  public void Contains_WithReadOnlySetOfT_UsesTableTypeWithUniqueConstraint ()
  {
    // set up a set with duplicates to get a database exception when we insert these duplicates into a table with unique constraint
    var possibleItems = SetupCollection<IReadOnlySet<Guid>>([(Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value, (Guid)DomainObjectIDs.Order3.Value]);

    var orders =
        from o in QueryFactory.CreateLinqQuery<Order>()
        where possibleItems.Contains((Guid)o.ID.Value)
        select o;

    Assert.That(() => CheckQueryResult(orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3),
        Throws.InstanceOf<RdbmsProviderException>()
            .With.Message.Contains("Error while executing SQL command: Violation of UNIQUE KEY constraint")
            .With.Message.Contains("Cannot insert duplicate key in object 'dbo.@1'. The duplicate key value is (83445473-844a-4d3f-a8c3-c27f8d98e8ba)."));
  }

  [Test]
  public void Contains_UsingTableValuedParameter_NullValue ()
  {
    var orderNumbers = new int?[] { 2, null, 99 };

    var orders =
        from o in QueryFactory.CreateLinqQuery<Order>()
        where orderNumbers.Contains(o.OrderNumber)
        select o;

    Assert.That(
        () => CheckQueryResult(
            orders,
            new ObjectID(typeof(Order), new Guid("F4016F41-F4E4-429E-B8D1-659C8C480A67")),
            new ObjectID(typeof(Order), new Guid("F7607CBC-AB34-465C-B282-0531D51F3B04"))),
        Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo(
            "Items within enumerable parameter values must not be null, because this would result in table rows with all columns being NULL. "
            + "This does not work with WHERE IN, and is not useful in JOIN scenarios, wherefore it is not supported."));
  }

  [Test]
  public void Contains_UsingTableValuedParameter_Int ()
  {
    var orderNumbers = new[] { 2, 99 };

    var orders =
        from o in QueryFactory.CreateLinqQuery<Order>()
        where orderNumbers.Contains(o.OrderNumber)
        select o;

    CheckQueryResult(
        orders,
        new ObjectID(typeof(Order), new Guid("F4016F41-F4E4-429E-B8D1-659C8C480A67")),
        new ObjectID(typeof(Order), new Guid("F7607CBC-AB34-465C-B282-0531D51F3B04")));
  }

  [Test]
  public void Contains_UsingTableValuedParameter_DateTime ()
  {
    var deliveryDates = new[] { new DateTime(2005,2,1), new DateTime(2013,3,7) };

    var orders =
        from o in QueryFactory.CreateLinqQuery<Order>()
        where deliveryDates.Contains(o.DeliveryDate)
        select o;

    CheckQueryResult(
        orders,
        new ObjectID(typeof(Order), new Guid("F4016F41-F4E4-429E-B8D1-659C8C480A67")),
        new ObjectID(typeof(Order), new Guid("F7607CBC-AB34-465C-B282-0531D51F3B04")));
  }

  [Test]
  public void Contains_UsingTableValuedParameter_String ()
  {
    var products = new[] { "Hitchhiker's guide", "Rubik's Cube" };

    var orderItems =
        from o in QueryFactory.CreateLinqQuery<OrderItem>()
        where products.Contains(o.Product)
        select o;

    CheckQueryResult(
        orderItems,
        new ObjectID(typeof(OrderItem), new Guid("DC20E0EB-4B55-4F23-89CF-6D6478F96D3B")),
        new ObjectID(typeof(OrderItem), new Guid("386D99F9-B0BA-4C55-8F22-BF194A3D745A")));
  }

  [Test]
  public void Contains_UsingTableValuedParameter_Decimal ()
  {
    var prices = new[] { 0.01m, 1m };

    var products =
        from o in QueryFactory.CreateLinqQuery<Product>()
        where prices.Contains(o.Price)
        select o;

    CheckQueryResult(
        products,
        new ObjectID(typeof(Product), new Guid("8DD65EF7-BDDA-433E-B081-725B4D53317D")),
        new ObjectID(typeof(Product), new Guid("BA684594-CF77-4009-A010-B70B30396A01")));
  }

  [Test]
  public void Contains_UsingTableValuedParameter_Enum ()
  {
    var values = new[] { ClassWithAllDataTypes.EnumType.Value1 };

    var objects =
        from o in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
        where values.Contains(o.EnumProperty)
        select o;

    CheckQueryResult(
        objects,
        new ObjectID(typeof(ClassWithAllDataTypes), new Guid("3F647D79-0CAF-4A53-BAA7-A56831F8CE2D")));
  }

  [Test]
  public void Contains_UsingTableValuedParameter_Double ()
  {
    var values = new[] { 987654.321 };

    var objects =
        from o in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
        where values.Contains(o.DoubleProperty)
        select o;

    CheckQueryResult(
        objects,
        new ObjectID(typeof(ClassWithAllDataTypes), new Guid("3F647D79-0CAF-4A53-BAA7-A56831F8CE2D")));
  }

  [Test]
  public void Contains_UsingTableValuedParameter_ExtensibleEnum ()
  {
    var values = new[] { Color.Values.Blue() };

    var objects =
        from o in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
        where values.Contains(o.ExtensibleEnumProperty)
        select o;

    CheckQueryResult(
        objects,
        new ObjectID(typeof(ClassWithAllDataTypes), new Guid("583EC716-8443-4B55-92BF-09F7C8768529")));
  }

  [Test]
  [Ignore("Not supported until RM-9233 is implemented.")]
  public void From ()
  {
    var possibleItems = new[] { (Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value };
    var ids =
        from o in QueryFactory.CreateLinqQuery<Order>()
        from i in possibleItems
        select i;

    var results = ids.ToArray();

    Assert.That(
          results.Length,
          Is.EqualTo(possibleItems.Length),
          "Number of returned objects doesn't match; returned: " + string.Join(", ", results.Select(obj => obj.ToString())));
    Assert.That(results, Is.EquivalentTo(possibleItems));
  }

  [Test]
  [Ignore("Not supported until RM-9233 is implemented.")]
  public void Join ()
  {
    var possibleItems = new[] { (Guid)DomainObjectIDs.Order1.Value, (Guid)DomainObjectIDs.Order3.Value };
    var orders =
        from o in QueryFactory.CreateLinqQuery<Order>()
        join i in possibleItems on o.ID.Value equals i
        select o;

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
}
