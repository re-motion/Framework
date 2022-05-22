using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.InterfaceMapping;

namespace Remotion.Data.DomainObjects.UnitTests.Linq
{
  [TestFixture]
  public class RelationToInterfacesTests : ClientTransactionBaseTest
  {
    [Test]
    public void InterfaceWithNoImplementors ()
    {
      var results = QueryFactory.CreateLinqQuery<IInterfaceWithoutImplementors>().ToArray();

      Assert.That(results, Is.Empty);
    }

    [Test]
    public void InterfaceWithOneImplementor ()
    {
      var results = QueryFactory.CreateLinqQuery<IInterfaceWithOneImplementor>()
          .ToArray()
          .OrderBy(e => e.ID)
          .ToArray();

      Assert.That(results.Length, Is.EqualTo(2));

      Assert.That(results[0].ID, Is.EqualTo(new ObjectID(typeof(OnlyImplementor), Guid.Parse("CDA944EF-4ABC-48DA-AFCB-CEFD1F826597"))));
      Assert.That(results[0], Is.InstanceOf<OnlyImplementor>());
      Assert.That(results[0].InterfaceProperty, Is.EqualTo(5));
      Assert.That(((OnlyImplementor)results[0]).OnlyImplementorProperty, Is.EqualTo(1338));

      Assert.That(results[1].ID, Is.EqualTo(new ObjectID(typeof(OnlyImplementor), Guid.Parse("D20AD209-61CC-4E01-B570-EE4554038DB0"))));
      Assert.That(results[1], Is.InstanceOf<OnlyImplementor>());
      Assert.That(results[1].InterfaceProperty, Is.EqualTo(3));
      Assert.That(((OnlyImplementor)results[1]).OnlyImplementorProperty, Is.EqualTo(1337));
    }

    [Test]
    public void TestJoin ()
    {
      var results = QueryFactory.CreateLinqQuery<IOrder>()
          .Where(e => e.OrderItems.Any(e => e.Product.Contains("water")))
          .ToArray();

      Assert.That(results.Length, Is.EqualTo(1));

      Assert.That(results[0].ID, Is.EqualTo(new ObjectID(typeof(SimpleOrder), Guid.Parse("6B71C813-86A6-44D0-B0E7-846C37F41D47"))));
      Assert.That(results[0], Is.InstanceOf<SimpleOrder>());
      Assert.That(results[0].OrderNumber, Is.EqualTo(2));
      Assert.That(((SimpleOrder)results[0]).SimpleOrderName, Is.EqualTo("Coffee and Milk"));
    }

    [Test]
    public void GetAllOrders ()
    {
      var orders = QueryFactory.CreateLinqQuery<IOrder>()
          .ToArray()
          .OrderBy(e => e.ID)
          .ToArray();

      Assert.That(orders.Length, Is.EqualTo(3));
      Assert.That(orders[0].ID, Is.EqualTo(new ObjectID(typeof(SimpleOrder), Guid.Parse("6B71C813-86A6-44D0-B0E7-846C37F41D47"))));
      Assert.That(orders[1].ID, Is.EqualTo(new ObjectID(typeof(SimpleOrder), Guid.Parse("DFFF4E9C-1EE1-4D36-977C-037909173CE0"))));
      Assert.That(orders[2].ID, Is.EqualTo(new ObjectID(typeof(ComplexOrder), Guid.Parse("E4B34F09-466B-437C-922F-045588CA4CA3"))));
    }

    [Test]
    public void TestLazyLoading ()
    {
      var order = QueryFactory.CreateLinqQuery<IOrder>()
          .OrderByDescending(e => e.OrderNumber)
          .First();
      var orderItem = order.OrderItems.First();

      Assert.That(order.ID, Is.EqualTo(new ObjectID(typeof(SimpleOrder), Guid.Parse("6B71C813-86A6-44D0-B0E7-846C37F41D47"))));
      Assert.That(orderItem.ID, Is.EqualTo(new ObjectID(typeof(SimpleOrderItem), Guid.Parse("FD01262F-FAD2-48BC-B91E-65B7A74F3AA3"))));
      Assert.That(orderItem, Is.InstanceOf<SimpleOrderItem>());
      Assert.That(orderItem.Position, Is.EqualTo(1));
      Assert.That(((SimpleOrderItem)orderItem).SimpleOrderItemName, Is.EqualTo("black arabian coffee with milk"));
    }

    [Test]
    public void TestSelectManyInJoin ()
    {
      var results = QueryFactory.CreateLinqQuery<IOrder>()
          .Where(e => e.OrderNumber > 1)
          .SelectMany(e => e.OrderItems)
          .ToArray();

      Assert.That(results.Length, Is.EqualTo(2));

      Assert.That(results[0].ID, Is.EqualTo(new ObjectID(typeof(SimpleOrderItem), Guid.Parse("FD01262F-FAD2-48BC-B91E-65B7A74F3AA3"))));
      Assert.That(results[0], Is.InstanceOf<SimpleOrderItem>());
      Assert.That(results[0].Position, Is.EqualTo(1));
      Assert.That(((SimpleOrderItem)results[0]).SimpleOrderItemName, Is.EqualTo("black arabian coffee with milk"));

      Assert.That(results[1].ID, Is.EqualTo(new ObjectID(typeof(SimpleOrderItem), Guid.Parse("003A1474-66E7-4998-A1EF-F29690B41955"))));
      Assert.That(results[1], Is.InstanceOf<SimpleOrderItem>());
      Assert.That(results[1].Position, Is.EqualTo(2));
      Assert.That(((SimpleOrderItem)results[1]).SimpleOrderItemName, Is.EqualTo("tap water"));
    }

    // ObjectList example
    // Setting foreign key (ObjectList + IObjectList)
    // ObjectList: Items.Add / Items.Remove
    // ObjectList<T> = new ObjectList<T>(...);
    // Multiple implementations for a class
    // Code generation

    // Items with sort order
    // relation property on interface and points to interface

    // navigation using property
    // lazy loading

    // test JOIN with SelectMany
    // Query<IOrder>().SelectMany(e => e.Items)

    // test JOIN on Interface property
    // Query<IOrder>().Where(e => e.Items.Any(e => e.XXX == XXX));
  }
}
