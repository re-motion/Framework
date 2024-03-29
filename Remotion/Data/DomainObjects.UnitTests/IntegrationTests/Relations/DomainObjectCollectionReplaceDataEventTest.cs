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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Relations
{
  [TestFixture]
  public class DomainObjectCollectionReplaceDataEventTest : ClientTransactionBaseTest
  {
    private Customer _customer;
    private Mock<OrderCollection.ICollectionEventReceiver> _eventReceiverMock;

    private Order _itemA;
    private Order _itemB;

    public override void SetUp ()
    {
      base.SetUp();

      _customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      _eventReceiverMock = new Mock<OrderCollection.ICollectionEventReceiver>(MockBehavior.Strict);

      _itemA = DomainObjectIDs.Order1.GetObject<Order>();
      _itemB = DomainObjectIDs.Order2.GetObject<Order>();
    }

    [Test]
    public void Load ()
    {
      var orderCollection = _customer.Orders;
      _eventReceiverMock
          .Setup(mock => mock.OnReplaceData())
          .Callback(() => Assert.That(orderCollection, Is.EqualTo(new[] { _itemA, _itemB })))
          .Verifiable();

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _customer.Orders.AssociatedEndPointID);
      orderCollection.SetEventReceiver(_eventReceiverMock.Object);

      _customer.Orders.EnsureDataComplete();

      _eventReceiverMock.Verify();
    }

    [Test]
    public void Rollback ()
    {
      _customer.Orders.Clear();

      var orderCollection = _customer.Orders;
      _eventReceiverMock
          .Setup(mock => mock.OnReplaceData())
          .Callback(() => Assert.That(orderCollection, Is.EqualTo(new[] { _itemA, _itemB })))
          .Verifiable();
      _customer.Orders.SetEventReceiver(_eventReceiverMock.Object);

      TestableClientTransaction.Rollback();

      _eventReceiverMock.Verify();
    }

    [Test]
    public void Synchronize ()
    {
      var orderCollection = _customer.Orders;
      _customer.Orders.EnsureDataComplete();

      _eventReceiverMock
          .Setup(mock => mock.OnReplaceData())
          .Callback(() => Assert.That(orderCollection, Is.EqualTo(new[] { _itemA, _itemB })))
          .Verifiable();
      _customer.Orders.SetEventReceiver(_eventReceiverMock.Object);

      BidirectionalRelationSyncService.Synchronize(TestableClientTransaction, _customer.Orders.AssociatedEndPointID);

      _eventReceiverMock.Verify();
    }

    [Test]
    public void SynchronizeOppositeEndPoint ()
    {
      // Prepare an item in unsynchronized state
      Order unsynchronizedOrder = PrepareUnsynchronizedOrder(DomainObjectIDs.Order4, _customer.ID);

      var orderCollection = _customer.Orders;
      orderCollection.EnsureDataComplete();

      _eventReceiverMock
          .Setup(mock => mock.OnReplaceData())
          .Callback(() => Assert.That(orderCollection, Is.EqualTo(new[] { _itemA, _itemB, unsynchronizedOrder })))
          .Verifiable();
      _customer.Orders.SetEventReceiver(_eventReceiverMock.Object);

      BidirectionalRelationSyncService.Synchronize(TestableClientTransaction, RelationEndPointID.Resolve(unsynchronizedOrder, o => o.Customer));

      _eventReceiverMock.Verify();
    }

    [Test]
    public void CommitSubTransaction ()
    {
      _customer.Orders.EnsureDataComplete();

      var orderCollection = _customer.Orders;
      _eventReceiverMock
          .Setup(mock => mock.OnReplaceData())
          .Callback(() => Assert.That(orderCollection.Count, Is.EqualTo(3)))
          .Verifiable();
      _customer.Orders.SetEventReceiver(_eventReceiverMock.Object);

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var newOrder = Order.NewObject();
        newOrder.OrderTicket = OrderTicket.NewObject();
        newOrder.OrderItems.Add(OrderItem.NewObject());
        newOrder.Official = Official.NewObject();

        _customer.Orders.Add(newOrder);
        ClientTransaction.Current.Commit();
      }

      _eventReceiverMock.Verify();
    }

    private Order PrepareUnsynchronizedOrder (ObjectID orderID, ObjectID relatedCustomerID)
    {
      var unsynchronizedOrder = (Order)LifetimeService.GetObjectReference(TestableClientTransaction, orderID);
      var dataContainer = DataContainer.CreateForExisting(
          unsynchronizedOrder.ID,
          null,
          pd => pd.PropertyName.EndsWith("Customer") ? relatedCustomerID : pd.DefaultValue);
      dataContainer.SetDomainObject(unsynchronizedOrder);
      TestableClientTransaction.DataManager.RegisterDataContainer(dataContainer);

      var endPointID = RelationEndPointID.Resolve(unsynchronizedOrder, o => o.Customer);
      var endPoint = (IRealObjectEndPoint)TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID);

      var oppositeID = RelationEndPointID.CreateOpposite(endPoint.Definition, relatedCustomerID);
      var oppositeEndPoint = TestableClientTransaction.DataManager.GetOrCreateVirtualEndPoint(oppositeID);
      oppositeEndPoint.EnsureDataComplete();

      Assert.That(endPoint.IsSynchronized, Is.False);
      return unsynchronizedOrder;
    }
  }
}
