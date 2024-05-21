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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class ObjectListTest : ClientTransactionBaseTest
  {
    private Order _order;
    private OrderItem _orderItem1;
    private OrderItem _orderItem2;
    private OrderItem _orderItem3;
    private OrderItem _orderItem4;
    private IList<OrderItem> _orderItemListAsIList;

    public override void SetUp ()
    {
      base.SetUp();

      _order = DomainObjectIDs.Order1.GetObject<Order>();
      _orderItem1 = _order.OrderItems[0];
      _orderItem2 = _order.OrderItems[1];
      _orderItem3 = DomainObjectIDs.OrderItem3.GetObject<OrderItem>();
      _orderItem4 = DomainObjectIDs.OrderItem4.GetObject<OrderItem>();
      _orderItemListAsIList = _order.OrderItems;
    }

    [Test]
    public void Initialization_WithData ()
    {
      var givenData = new ModificationCheckingDomainObjectCollectionDataDecorator(typeof(Customer), new DomainObjectCollectionData());
      var collection = new ObjectList<Customer>(givenData);

      var actualData = DomainObjectCollectionDataTestHelper.GetDataStrategy(collection);
      Assert.That(actualData, Is.SameAs(givenData));
    }

    [Test]
    public void Initialization_WithData_InvalidRequiredItemType ()
    {
      var dataStub = new Mock<IDomainObjectCollectionData>();
      dataStub.Setup(stub => stub.RequiredItemType).Returns((Type)null);
      Assert.That(
          () => new ObjectList<Customer>(dataStub.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The given data strategy must have a required item type of 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer' in order to be used "
                  + "with this collection type.", "dataStrategy"));
    }

    [Test]
    public void Initialization_WithData_NoRequiredItemType_ReadOnly ()
    {
      var dataStub = new Mock<IDomainObjectCollectionData>();
      dataStub.Setup(stub => stub.RequiredItemType).Returns((Type)null);
      dataStub.Setup(stub => stub.IsReadOnly).Returns(true);

      var collection = new ObjectList<Customer>(dataStub.Object);

      var actualData = DomainObjectCollectionDataTestHelper.GetDataStrategy(collection);
      Assert.That(actualData, Is.SameAs(dataStub.Object));
      Assert.That(actualData.RequiredItemType, Is.Null);
    }

    [Test]
    public void Initialization_WithData_DerivedRequiredItemType ()
    {
      var dataStub = new Mock<IDomainObjectCollectionData>();
      dataStub.Setup(stub => stub.RequiredItemType).Returns(typeof(Order));

      var collection = new ObjectList<DomainObject>(dataStub.Object);

      Assert.That(collection.RequiredItemType, Is.SameAs(typeof(Order)));
    }

    [Test]
    public void Initialization_WithIEnumerable_RequiredItemType ()
    {
      var list = new ObjectList<OrderItem>(new OrderItem[0]);
      Assert.That(list.RequiredItemType, Is.SameAs(typeof(OrderItem)));
    }

    [Test]
    public void Initialization_WithIEnumerable ()
    {
      var list = new ObjectList<OrderItem>(new[] { _orderItem1, _orderItem2, _orderItem3, _orderItem4 });
      Assert.That(list, Is.EqualTo(new[] { _orderItem1, _orderItem2, _orderItem3, _orderItem4 }));
      Assert.That(list.IsReadOnly, Is.False);
    }

    [Test]
    public void ObjectList_IsIList ()
    {
      IList<OrderItem> list = new ObjectList<OrderItem>();
      Assert.That(list, Is.InstanceOf(typeof(IList<OrderItem>)));
    }

    [Test]
    public void IList_IndexOf ()
    {
      IList<OrderItem> emptyList = new ObjectList<OrderItem>();
      Assert.That(emptyList.IndexOf(_orderItem1), Is.EqualTo(-1));
      Assert.That(_orderItemListAsIList.IndexOf(_orderItem1), Is.EqualTo(0));
      Assert.That(_orderItemListAsIList.IndexOf(_orderItem2), Is.EqualTo(1));
      Assert.That(_orderItemListAsIList.IndexOf(_orderItem3), Is.EqualTo(-1));
    }

    [Test]
    public void IList_Insert ()
    {
      IList<OrderItem> list = new ObjectList<OrderItem>();
      list.Insert(0, _orderItem2);
      Assert.That(list, Is.EqualTo(new[] { _orderItem2 }));
      list.Insert(0, _orderItem1);
      Assert.That(list, Is.EqualTo(new[] { _orderItem1, _orderItem2 }));
      list.Insert(1, _orderItem3);
      Assert.That(list, Is.EqualTo(new[] { _orderItem1, _orderItem3, _orderItem2 }));
      list.Insert(3, _orderItem4);
      Assert.That(list, Is.EqualTo(new[] { _orderItem1, _orderItem3, _orderItem2, _orderItem4 }));
    }

    [Test]
    public void IList_InsertThrowsIfReadOnly ()
    {
      IList<OrderItem> readOnlyList = new ObjectList<OrderItem>().Clone(true);
      Assert.That(
          () => readOnlyList.Insert(0, _orderItem2),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Cannot insert an item into a read-only collection."));
    }

    [Test]
    public void IList_InsertThrowsOnWrongIndex ()
    {
      IList<OrderItem> list = new ObjectList<OrderItem>();
      Assert.That(
          () => list.Insert(1, _orderItem2),
          Throws.InstanceOf<ArgumentOutOfRangeException>()
              .With.Message.Contains(
                  "Index is out of range. Must be non-negative and less than or equal to the size of the collection."));
    }

    [Test]
    public void IList_Item ()
    {
      Assert.That(_orderItemListAsIList[0], Is.SameAs(_orderItem1));
      Assert.That(_orderItemListAsIList[1], Is.SameAs(_orderItem2));

      Assert.That(_orderItemListAsIList, Is.EqualTo(new object[] { _orderItem1, _orderItem2 }));

      _orderItemListAsIList[0] = _orderItem3;

      Assert.That(_orderItemListAsIList, Is.EqualTo(new object[] { _orderItem3, _orderItem2 }));
    }

    [Test]
    public void IList_ItemGetThrowsOnWrongIndex ()
    {
      Assert.That(
          () => _orderItemListAsIList[2],
          Throws.InstanceOf<ArgumentOutOfRangeException>()
              .With.Message.Contains("Index was out of range. Must be non-negative and less than the size of the collection."));
    }

    [Test]
    public void IList_ItemSetThrowsOnWrongIndex ()
    {
      Assert.That(
          () => _orderItemListAsIList[-1] = null,
          Throws.InstanceOf<ArgumentOutOfRangeException>()
              .With.Message.Contains("Index was out of range. Must be non-negative and less than the size of the collection."));
    }

    [Test]
    public void IList_ItemSetThrowsOnReadOnlyList ()
    {
      IList<OrderItem> readOnlyList = ((ObjectList<OrderItem>)_orderItemListAsIList).Clone(true);
      Assert.That(
          () => readOnlyList[0] = null,
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Cannot modify a read-only collection."));
    }

    [Test]
    public void IList_Add ()
    {
      IList<OrderItem> list = new ObjectList<OrderItem>();
      list.Add(_orderItem1);
      Assert.That(list, Is.EqualTo(new object[] { _orderItem1 }));

      list.Add(_orderItem2);
      Assert.That(list, Is.EqualTo(new object[] { _orderItem1, _orderItem2 }));
    }

    [Test]
    public void IList_AddThrowsOnReadOnlyList ()
    {
      IList<OrderItem> readOnlyList = new ObjectList<OrderItem>().Clone(true);
      Assert.That(
          () => readOnlyList.Add(_orderItem1),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Cannot add an item to a read-only collection."));
    }

    [Test]
    public void IList_Contains ()
    {
      Assert.That(_orderItemListAsIList.Contains(_orderItem1), Is.True);
      Assert.That(_orderItemListAsIList.Contains(_orderItem2), Is.True);
      Assert.That(_orderItemListAsIList.Contains(_orderItem3), Is.False);
    }

    [Test]
    public void IList_CopyToTightFit ()
    {
      var destination = new OrderItem[2];
      _orderItemListAsIList.CopyTo(destination, 0);
      Assert.That(destination, Is.EqualTo(new object[] { _orderItem1, _orderItem2 }));
    }

    [Test]
    public void IList_CopyToLargeFit ()
    {
      var destination = new OrderItem[5];
      _orderItemListAsIList.CopyTo(destination, 0);
      Assert.That(destination, Is.EqualTo(new object[] { _orderItem1, _orderItem2, null, null, null }));
      _orderItemListAsIList.CopyTo(destination, 1);
      Assert.That(destination, Is.EqualTo(new object[] { _orderItem1, _orderItem1, _orderItem2, null, null}));
      _orderItemListAsIList.CopyTo(destination, 3);
      Assert.That(destination, Is.EqualTo(new object[] { _orderItem1, _orderItem1, _orderItem2, _orderItem1, _orderItem2}));
    }

    [Test]
    public void IList_CopyToNegativeIndex ()
    {
      var destination = new OrderItem[5];
      Assert.That(
          () => _orderItemListAsIList.CopyTo(destination, -1),
          Throws.InstanceOf<ArgumentOutOfRangeException>()
#if NET8_0_OR_GREATER
              .With.Message.EqualTo("destinationIndex ('-1') must be greater than or equal to '0'. (Parameter 'destinationIndex')\r\nActual value was -1."));
#else
              .With.Message.Contains("Number was less than the array's lower bound in the first dimension."));
#endif
    }

    [Test]
    public void IList_CopyToGreatIndex ()
    {
      var destination = new OrderItem[5];
      Assert.That(
          () => _orderItemListAsIList.CopyTo(destination, 5),
          Throws.ArgumentException
#if NETFRAMEWORK
              .With.Message.EqualTo(
                  "Destination array was not long enough. Check destIndex and length, and the array's lower bounds.")
#else
              .With.ArgumentExceptionMessageEqualTo(
                  "Destination array was not long enough. Check the destination index, length, and the array's lower bounds.", "destinationArray")
#endif
          );
    }

    [Test]
    public void IList_CopyToTooLittleSpace ()
    {
      var destination = new OrderItem[5];
      Assert.That(
          () => _orderItemListAsIList.CopyTo(destination, 4),
          Throws.ArgumentException
#if NETFRAMEWORK
              .With.Message.EqualTo(
                  "Destination array was not long enough. Check destIndex and length, and the array's lower bounds.")
#else
              .With.ArgumentExceptionMessageEqualTo(
                  "Destination array was not long enough. Check the destination index, length, and the array's lower bounds.", "destinationArray")
#endif
          );
    }

    [Test]
    public void IList_Remove ()
    {
      Assert.That(_orderItemListAsIList.Remove(_orderItem3), Is.False);
      Assert.That(_orderItemListAsIList.Remove(_orderItem1), Is.True);
      Assert.That(_orderItemListAsIList.Remove(_orderItem1), Is.False);
      Assert.That(_orderItemListAsIList.Remove(_orderItem2), Is.True);
      Assert.That(_orderItemListAsIList.Remove(_orderItem2), Is.False);
    }

    [Test]
    public void IList_RemoveThrowsOnReadOnlyList ()
    {
      IList<OrderItem> readOnlyList = new ObjectList<OrderItem>().Clone(true);
      Assert.That(
          () => readOnlyList.Remove(_orderItem1),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Cannot remove an item from a read-only collection."));
    }

    [Test]
    public void IList_RemoveInexistentThrowsOnReadOnlyList ()
    {
      IList<OrderItem> readOnlyList = new ObjectList<OrderItem>().Clone(true);
      Assert.That(
          () => readOnlyList.Remove(_orderItem3),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Cannot remove an item from a read-only collection."));
    }

    [Test]
    public void GetEnumerator ()
    {
      using (IEnumerator<OrderItem> enumerator = new ObjectList<OrderItem>().GetEnumerator())
      {
        Assert.That(enumerator.MoveNext(), Is.False);
      }

      using (IEnumerator<OrderItem> enumerator = _orderItemListAsIList.GetEnumerator())
      {
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.SameAs(_orderItem1));
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.SameAs(_orderItem2));
        Assert.That(enumerator.MoveNext(), Is.False);
      }
    }

    [Test]
    public void ToArray ()
    {
      OrderItem[] orderItems = _order.OrderItems.ToArray();

      Assert.That(orderItems, Is.EquivalentTo(_order.OrderItems));
    }

    [Test]
    public void Linq ()
    {
      var result = from oi in _order.OrderItems where oi.Product == _orderItem1.Product select oi;
      Assert.That(result.ToArray(), Is.EqualTo(new[] {_orderItem1}));
    }

    [Test]
    public void AddRange ()
    {
      var newList = new ObjectList<OrderItem>();
      newList.AddRange(new[] { _orderItem1, _orderItem2 });

      Assert.That(newList, Is.EqualTo(new[] { _orderItem1, _orderItem2 }));
    }
  }
}
