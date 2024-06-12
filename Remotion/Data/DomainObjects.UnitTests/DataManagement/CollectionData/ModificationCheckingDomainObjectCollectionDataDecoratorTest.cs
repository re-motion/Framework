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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.CollectionData
{
  [TestFixture]
  public class ModificationCheckingDomainObjectCollectionDataDecoratorTest : ClientTransactionBaseTest
  {
    private Mock<IDomainObjectCollectionData> _wrappedDataMock;
    private ModificationCheckingDomainObjectCollectionDataDecorator _modificationCheckingDecorator;
    private ModificationCheckingDomainObjectCollectionDataDecorator _modificationCheckingDecoratorWithoutRequiredItemType;

    private Order _order1;
    private Order _order3;
    private OrderItem _orderItem1;

    public override void SetUp ()
    {
      base.SetUp();

      _wrappedDataMock = new Mock<IDomainObjectCollectionData>();
      _modificationCheckingDecorator = new ModificationCheckingDomainObjectCollectionDataDecorator(typeof(Order), _wrappedDataMock.Object);
      _modificationCheckingDecoratorWithoutRequiredItemType = new ModificationCheckingDomainObjectCollectionDataDecorator(null, _wrappedDataMock.Object);

      _order1 = DomainObjectIDs.Order1.GetObject<Order>();
      _order3 = DomainObjectIDs.Order3.GetObject<Order>();
      _orderItem1 = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
    }

    [Test]
    public void RequiredItemType ()
    {
      Assert.That(_modificationCheckingDecorator.RequiredItemType, Is.SameAs(typeof(Order)));
      Assert.That(_modificationCheckingDecoratorWithoutRequiredItemType.RequiredItemType, Is.Null);
    }

    [Test]
    public void Insert ()
    {
      StubInnerData(_order1);

      _modificationCheckingDecorator.Insert(0, _order3);

      _wrappedDataMock.Verify(mock => mock.Insert(0, _order3), Times.AtLeastOnce());
    }

    [Test]
    public void Insert_Duplicate ()
    {
      StubInnerData(_order1);

      Assert.That(
          () => _modificationCheckingDecorator.Insert(0, _order1),
          Throws.InstanceOf<ArgumentException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "The collection already contains an object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'.", "domainObject"));

      _wrappedDataMock.Verify(mock => mock.Insert(It.IsAny<int>(), It.IsAny<DomainObject>()), Times.Never());
    }

    [Test]
    public void Insert_IndexTooHigh ()
    {
      Assert.That(
          () => _modificationCheckingDecorator.Insert(1, _order1),
          Throws.InstanceOf<ArgumentOutOfRangeException>()
              .With.ArgumentOutOfRangeExceptionMessageEqualTo(
                  "Index is out of range. Must be non-negative and less than or equal to the size of the collection.", "index", 1));

      _wrappedDataMock.Verify(mock => mock.Insert(It.IsAny<int>(), It.IsAny<DomainObject>()), Times.Never());
    }

    [Test]
    public void Insert_IndexTooLow ()
    {
      Assert.That(
          () => _modificationCheckingDecorator.Insert(-1, _order1),
          Throws.InstanceOf<ArgumentOutOfRangeException>()
              .With.ArgumentOutOfRangeExceptionMessageEqualTo(
                  "Index is out of range. Must be non-negative and less than or equal to the size of the collection.", "index", -1));

      _wrappedDataMock.Verify(mock => mock.Insert(It.IsAny<int>(), It.IsAny<DomainObject>()), Times.Never());
    }

    [Test]
    public void Insert_WrongType ()
    {
      Assert.That(
          () => _modificationCheckingDecorator.Insert(0, _orderItem1),
          Throws.InstanceOf<ArgumentException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Values of type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem'"
                  + " cannot be added to this collection. Values must be of type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' or derived from "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order'.", "domainObject"));

      _wrappedDataMock.Verify(mock => mock.Insert(It.IsAny<int>(), It.IsAny<DomainObject>()), Times.Never());
    }

    [Test]
    public void Insert_RequiredItemTypeNull ()
    {
      _modificationCheckingDecoratorWithoutRequiredItemType.Insert(0, _order1);
      _modificationCheckingDecoratorWithoutRequiredItemType.Insert(0, _orderItem1);

      _wrappedDataMock.Verify(mock => mock.Insert(0, _order1), Times.AtLeastOnce());
      _wrappedDataMock.Verify(mock => mock.Insert(0, _orderItem1), Times.AtLeastOnce());
    }

    [Test]
    public void Remove ()
    {
      StubInnerData(_order1);

      _modificationCheckingDecorator.Remove(_order1);

      _wrappedDataMock.Verify(mock => mock.Remove(_order1), Times.AtLeastOnce());
    }

    [Test]
    public void Remove_HoldsObjectFromOtherTransaction ()
    {
      _wrappedDataMock.Setup(stub => stub.GetObject(DomainObjectIDs.Order1)).Returns(_order3);

      Assert.That(
          () => _modificationCheckingDecorator.Remove(_order1),
          Throws.InstanceOf<ArgumentException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "The object to be removed has the same ID as an object in this collection, but is a different object reference.", "domainObject"));

      _wrappedDataMock.Verify(mock => mock.Remove(It.IsAny<DomainObject>()), Times.Never());
    }

    [Test]
    public void Replace ()
    {
      StubInnerData(_order1);

      _modificationCheckingDecorator.Replace(0, _order3);

      _wrappedDataMock.Verify(mock => mock.Replace(0, _order3), Times.AtLeastOnce());
    }

    [Test]
    public void Replace_WithSameObject ()
    {
      StubInnerData(_order1);

      _modificationCheckingDecorator.Replace(0, _order1);

      _wrappedDataMock.Verify(mock => mock.Replace(0, _order1), Times.AtLeastOnce());
    }

    [Test]
    public void Replace_WithDuplicate ()
    {
      StubInnerData(_order1, _order3);

      Assert.That(
          () => _modificationCheckingDecorator.Replace(1, _order1),
          Throws.InstanceOf<InvalidOperationException>()
              .With.Message.EqualTo(
                  "The collection already contains an object with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'."));

      _wrappedDataMock.Verify(mock => mock.Replace(It.IsAny<int>(), It.IsAny<DomainObject>()), Times.Never());
    }

    [Test]
    public void Replace_IndexTooLow ()
    {
      StubInnerData(_order1);

      Assert.That(
          () => _modificationCheckingDecorator.Replace(-1, _order1),
          Throws.InstanceOf<ArgumentOutOfRangeException>()
              .With.ArgumentOutOfRangeExceptionMessageEqualTo(
                  "Index is out of range. Must be non-negative and less than the size of the collection.", "index", -1));

      _wrappedDataMock.Verify(mock => mock.Replace(It.IsAny<int>(), It.IsAny<DomainObject>()), Times.Never());
    }

    [Test]
    public void Replace_IndexTooHigh ()
    {
      StubInnerData(_order1);

      Assert.That(
          () => _modificationCheckingDecorator.Replace(1, _order1),
          Throws.InstanceOf<ArgumentOutOfRangeException>()
              .With.ArgumentOutOfRangeExceptionMessageEqualTo(
                  "Index is out of range. Must be non-negative and less than the size of the collection.", "index", 1));

      _wrappedDataMock.Verify(mock => mock.Replace(It.IsAny<int>(), It.IsAny<DomainObject>()), Times.Never());
    }

    [Test]
    public void Replace_WrongType ()
    {
      StubInnerData(_order1);

      Assert.That(
          () => _modificationCheckingDecorator.Replace(0, _orderItem1),
          Throws.InstanceOf<ArgumentException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Values of type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem'"
                  + " cannot be added to this collection. Values must be of type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' or derived from "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order'.", "value"));

      _wrappedDataMock.Verify(mock => mock.Replace(It.IsAny<int>(), It.IsAny<DomainObject>()), Times.Never());
    }

    [Test]
    public void Replace_RequiredItemTypeNull ()
    {
      StubInnerData(_order1);

      _modificationCheckingDecoratorWithoutRequiredItemType.Replace(0, _order3);
      _modificationCheckingDecoratorWithoutRequiredItemType.Replace(0, _orderItem1);

      _wrappedDataMock.Verify(mock => mock.Replace(0, _order3), Times.AtLeastOnce());
      _wrappedDataMock.Verify(mock => mock.Replace(0, _orderItem1), Times.AtLeastOnce());
    }

    private void StubInnerData (params DomainObject[] contents)
    {
      _wrappedDataMock.Setup(stub => stub.Count).Returns(contents.Length);

      for (int i = 0; i < contents.Length; i++)
      {
        int currentIndex = i; // required because Stub creates a closure
        _wrappedDataMock.Setup(stub => stub.ContainsObjectID(contents[currentIndex].ID)).Returns(true);
        _wrappedDataMock.Setup(stub => stub.GetObject(contents[currentIndex].ID)).Returns(contents[currentIndex]);
        _wrappedDataMock.Setup(stub => stub.GetObject(currentIndex)).Returns(contents[currentIndex]);
        _wrappedDataMock.Setup(stub => stub.IndexOf(contents[currentIndex].ID)).Returns(currentIndex);
      }
    }
  }
}
