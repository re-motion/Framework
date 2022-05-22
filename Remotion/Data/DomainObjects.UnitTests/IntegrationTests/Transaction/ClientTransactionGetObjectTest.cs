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
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class ClientTransactionGetObjectTest : ClientTransactionBaseTest
  {
    private ClientTransactionEventReceiver _eventReceiver;

    public override void SetUp ()
    {
      base.SetUp();

      _eventReceiver = new ClientTransactionEventReceiver(TestableClientTransaction);
    }

    [Test]
    public void GetObject_Twice ()
    {
      IDomainObject domainObject1 = LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.ClassWithAllDataTypes1, false);
      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(1));

      var domainObjects = _eventReceiver.LoadedDomainObjectLists[0];
      Assert.That(domainObjects.Count, Is.EqualTo(1));
      Assert.That(domainObjects[0], Is.SameAs(domainObject1));
      _eventReceiver.Clear();

      IDomainObject domainObject2 = LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.ClassWithAllDataTypes1, false);
      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(0));

      Assert.That(domainObject2, Is.SameAs(domainObject1));
    }

    [Test]
    public void GetObject_MultipleSimpleObjects ()
    {
      ObjectID id1 = DomainObjectIDs.ClassWithAllDataTypes1;
      ObjectID id2 = DomainObjectIDs.ClassWithAllDataTypes2;

      IDomainObject domainObject1 = LifetimeService.GetObject(TestableClientTransaction, id1, false);
      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(1));

      var domainObjects = _eventReceiver.LoadedDomainObjectLists[0];
      Assert.That(domainObjects.Count, Is.EqualTo(1));
      Assert.That(domainObjects[0], Is.SameAs(domainObject1));
      _eventReceiver.Clear();

      IDomainObject domainObject2 = LifetimeService.GetObject(TestableClientTransaction, id2, false);
      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(1));

      domainObjects = _eventReceiver.LoadedDomainObjectLists[0];
      Assert.That(domainObjects.Count, Is.EqualTo(1));
      Assert.That(domainObjects[0], Is.SameAs(domainObject2));

      Assert.That(ReferenceEquals(domainObject1, domainObject2), Is.False);
    }

    [Test]
    public void GetObject_NotFound_True ()
    {
      var notFoundID = new ObjectID(typeof(Order), Guid.NewGuid());

      Assert.That(
          () => LifetimeService.GetObject(TestableClientTransaction, notFoundID, true),
          Throws.TypeOf<ObjectsNotFoundException>().With.Property("IDs").EqualTo(new[] { notFoundID }));
    }

    [Test]
    public void GetObject_NotFound_False ()
    {
      var notFoundID = new ObjectID(typeof(Order), Guid.NewGuid());

      Assert.That(() => LifetimeService.GetObject(TestableClientTransaction, notFoundID, false), Throws.TypeOf<ObjectsNotFoundException>());
    }

    [Test]
    public void GetObject_Deleted ()
    {
      var domainObject = LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.ClassWithAllDataTypes1, false);
      LifetimeService.DeleteObject(TestableClientTransaction, domainObject);

      Assert.That(
          () => LifetimeService.GetObject(TestableClientTransaction, domainObject.ID, false),
          Throws.TypeOf<ObjectDeletedException>().With.Message.EqualTo(
              "Object 'ClassWithAllDataTypes|3f647d79-0caf-4a53-baa7-a56831f8ce2d|System.Guid' is already deleted."));
      Assert.That(LifetimeService.GetObject(TestableClientTransaction, domainObject.ID, true), Is.SameAs(domainObject));
    }

    [Test]
    public void GetObject_Invalid ()
    {
      var domainObject = ClassWithAllDataTypes.NewObject();
      domainObject.Delete();

      var expectedMessage = string.Format("Object '{0}' is invalid in this transaction.", domainObject.ID);
      Assert.That(
          () => LifetimeService.GetObject(TestableClientTransaction, domainObject.ID, true),
          Throws.TypeOf<ObjectInvalidException>().With.Message.EqualTo(expectedMessage));
      Assert.That(
          () => LifetimeService.GetObject(TestableClientTransaction, domainObject.ID, false),
          Throws.TypeOf<ObjectInvalidException>().With.Message.EqualTo(expectedMessage));
    }

    [Test]
    public void TryGetObject_Twice ()
    {
      IDomainObject domainObject1 = LifetimeService.TryGetObject(TestableClientTransaction, DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(1));

      var domainObjects = _eventReceiver.LoadedDomainObjectLists[0];
      Assert.That(domainObjects.Count, Is.EqualTo(1));
      Assert.That(domainObjects[0], Is.SameAs(domainObject1));
      _eventReceiver.Clear();

      IDomainObject domainObject2 = LifetimeService.TryGetObject(TestableClientTransaction, DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(0));

      Assert.That(domainObject2, Is.SameAs(domainObject1));
    }

    [Test]
    public void TryGetObject_NotFound ()
    {
      var notFoundID = new ObjectID(typeof(Order), Guid.NewGuid());

      var domainObject = LifetimeService.TryGetObject(TestableClientTransaction, notFoundID);
      Assert.That(domainObject, Is.Null);

      domainObject = LifetimeService.TryGetObject(TestableClientTransaction, notFoundID);
      Assert.That(domainObject, Is.Not.Null);
      Assert.That(((DomainObject)domainObject).State.IsInvalid, Is.True);
    }

    [Test]
    public void TryGetObject_Deleted ()
    {
      var domainObject = LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.ClassWithAllDataTypes1, false);
      LifetimeService.DeleteObject(TestableClientTransaction, domainObject);

      Assert.That(LifetimeService.TryGetObject(TestableClientTransaction, domainObject.ID), Is.SameAs(domainObject));
    }

    [Test]
    public void TryGetObject_Invalid ()
    {
      var domainObject = ClassWithAllDataTypes.NewObject();
      domainObject.Delete();
      Assert.That(domainObject.State.IsInvalid, Is.True);

      Assert.That(LifetimeService.TryGetObject(TestableClientTransaction, domainObject.ID), Is.SameAs(domainObject));
    }

    [Test]
    public void GetObjects_UnloadedObjects ()
    {
      DomainObject[] objects = LifetimeService.GetObjects<DomainObject>(
          TestableClientTransaction,
          DomainObjectIDs.Order1,
          DomainObjectIDs.Order3,
          DomainObjectIDs.OrderItem1);

      var expectedObjects = new object[] {
          DomainObjectIDs.Order1.GetObject<Order>(),
          DomainObjectIDs.Order3.GetObject<Order>(),
          DomainObjectIDs.OrderItem1.GetObject<OrderItem>()};
      Assert.That(objects, Is.EqualTo(expectedObjects));
    }

    [Test]
    public void GetObjects_UnloadedObjects_Events ()
    {
      var listenerMock = new Mock<IClientTransactionListener>();
      TestableClientTransaction.AddListener(listenerMock.Object);

      DomainObject[] objects = LifetimeService.GetObjects<DomainObject>(
          TestableClientTransaction,
          DomainObjectIDs.Order1,
          DomainObjectIDs.Order3,
          DomainObjectIDs.OrderItem1);
      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(1));
      Assert.That(_eventReceiver.LoadedDomainObjectLists[0], Is.EqualTo(objects));

      listenerMock.Verify(
          mock => mock.ObjectsLoading(TestableClientTransaction, new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.OrderItem1 }),
          Times.AtLeastOnce());

      listenerMock.Verify(mock => mock.ObjectsLoaded(TestableClientTransaction, objects), Times.AtLeastOnce());
    }

    [Test]
    public void GetObjects_LoadedObjects ()
    {
      var expectedObjects = new object[] {DomainObjectIDs.Order1.GetObject<Order>(), DomainObjectIDs.Order3.GetObject<Order>(),
          DomainObjectIDs.OrderItem1.GetObject<OrderItem>()};
      DomainObject[] objects = LifetimeService.GetObjects<DomainObject>(
          TestableClientTransaction,
          DomainObjectIDs.Order1,
          DomainObjectIDs.Order3,
          DomainObjectIDs.OrderItem1);
      Assert.That(objects, Is.EqualTo(expectedObjects));
    }

    [Test]
    public void GetObjects_LoadedObjects_Events ()
    {
      DomainObjectIDs.Order1.GetObject<Order>();
      DomainObjectIDs.Order3.GetObject<Order>();
      DomainObjectIDs.OrderItem1.GetObject<OrderItem>();

      _eventReceiver.Clear();

      var listenerMock = new Mock<IClientTransactionListener>();
      TestableClientTransaction.AddListener(listenerMock.Object);

      LifetimeService.GetObjects<DomainObject>(TestableClientTransaction, DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.OrderItem1);
      Assert.That(_eventReceiver.LoadedDomainObjectLists, Is.Empty);

      listenerMock.Verify(mock => mock.ObjectsLoading(It.IsAny<ClientTransaction>(), It.IsAny<ReadOnlyCollection<ObjectID>>()), Times.Never());
      listenerMock.Verify(mock => mock.ObjectsLoaded(It.IsAny<ClientTransaction>(), It.IsAny<ReadOnlyCollection<DomainObject>>()), Times.Never());
    }

    [Test]
    public void GetObjects_NewObjects ()
    {
      var expectedObjects = new DomainObject[] { Order.NewObject(), OrderItem.NewObject() };
      DomainObject[] objects = LifetimeService.GetObjects<DomainObject>(TestableClientTransaction, expectedObjects[0].ID, expectedObjects[1].ID);
      Assert.That(objects, Is.EqualTo(expectedObjects));
    }

    [Test]
    public void GetObjects_NewObjects_Events ()
    {
      var expectedObjects = new DomainObject[] { Order.NewObject(), OrderItem.NewObject() };
      _eventReceiver.Clear();

      var listenerMock = new Mock<IClientTransactionListener>();
      TestableClientTransaction.AddListener(listenerMock.Object);

      LifetimeService.GetObjects<DomainObject>(TestableClientTransaction, expectedObjects[0].ID, expectedObjects[1].ID);
      Assert.That(_eventReceiver.LoadedDomainObjectLists, Is.Empty);

      listenerMock.Verify(mock => mock.ObjectsLoading(It.IsAny<ClientTransaction>(), It.IsAny<ReadOnlyCollection<ObjectID>>()), Times.Never());
      listenerMock.Verify(mock => mock.ObjectsLoaded(It.IsAny<ClientTransaction>(), It.IsAny<ReadOnlyCollection<DomainObject>>()), Times.Never());
    }

    [Test]
    public void GetObjects_NotFound ()
    {
      var guid = new Guid("33333333333333333333333333333333");
      Assert.That(
          () => LifetimeService.GetObjects<DomainObject>(TestableClientTransaction, new ObjectID(typeof(Order), guid)),
          Throws.InstanceOf<ObjectsNotFoundException>()
              .With.Message.EqualTo("Object(s) could not be found: 'Order|33333333-3333-3333-3333-333333333333|System.Guid'."));
    }

    [Test]
    public void TryGetObjects_NotFound ()
    {
      Order newObject = Order.NewObject();
      var guid = new Guid("33333333333333333333333333333333");
      Order[] objects = LifetimeService.TryGetObjects<Order>(
          TestableClientTransaction,
          DomainObjectIDs.Order1,
          newObject.ID,
          new ObjectID(typeof(Order), guid),
          DomainObjectIDs.Order3);
      var expectedObjects = new DomainObject[] {
          DomainObjectIDs.Order1.GetObject<Order>(),
          newObject,
          null,
          DomainObjectIDs.Order3.GetObject<Order>() };
      Assert.That(objects, Is.EqualTo(expectedObjects));
    }

    [Test]
    public void GetObjects_InvalidType ()
    {
      Assert.That(
          () => LifetimeService.GetObjects<OrderItem>(TestableClientTransaction, DomainObjectIDs.Order1),
          Throws.InstanceOf<InvalidCastException>());
    }

    [Test]
    public void GetObjects_Deleted ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      order.Delete();

      var result = LifetimeService.GetObjects<Order>(TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That(result[0], Is.SameAs(order));
    }

    [Test]
    public void GetObjects_Discarded ()
    {
      DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>().Delete();
      TestableClientTransaction.Commit();
      Assert.That(
          () => LifetimeService.GetObjects<ClassWithAllDataTypes>(TestableClientTransaction, DomainObjectIDs.ClassWithAllDataTypes1),
          Throws.InstanceOf<ObjectInvalidException>()
              .With.Message.EqualTo(
                  "Object 'ClassWithAllDataTypes|3f647d79-0caf-4a53-baa7-a56831f8ce2d|System.Guid' is invalid in this transaction."));
    }

    [Test]
    public void GetObjectByNewIndependentTransaction ()
    {
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction();
      using (clientTransaction.EnterDiscardingScope())
      {
        Order order = DomainObjectIDs.Order1.GetObject<Order>();

        Assert.That(order.InternalDataContainer.ClientTransaction, Is.SameAs(clientTransaction));
        Assert.That(clientTransaction.IsEnlisted(order), Is.True);
      }
    }

    [Test]
    public void GetDeletedObjectByNewIndependentTransaction ()
    {
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction();
      using (clientTransaction.EnterDiscardingScope())
      {
        Order order = DomainObjectIDs.Order1.GetObject<Order>();

        order.Delete();

        order = DomainObjectIDs.Order1.GetObject<Order>(includeDeleted: true);
        Assert.That(order.State.IsDeleted, Is.True);
        Assert.That(order.InternalDataContainer.ClientTransaction, Is.SameAs(clientTransaction));
        Assert.That(clientTransaction.IsEnlisted(order), Is.True);
      }
    }

    [Test]
    public void ClientTransactionGetObjectIsIndependentOfCurrentTransaction ()
    {
      var clientTransactionMock = new TestableClientTransaction();
      var order = (Order)clientTransactionMock.GetObject(DomainObjectIDs.Order1, false);
      Assert.That(ClientTransactionScope.CurrentTransaction.IsEnlisted(order), Is.False);
      Assert.That(clientTransactionMock.IsEnlisted(order), Is.True);

      using (clientTransactionMock.EnterDiscardingScope())
      {
        Assert.That(clientTransactionMock.IsEnlisted(order.OrderTicket), Is.True);
        Assert.That(clientTransactionMock.IsEnlisted(order.Official), Is.True);
        Assert.That(clientTransactionMock.IsEnlisted(order.OrderItems[0]), Is.True);
      }
    }

    [Test]
    public void GetObjects_WithInvalidObject_Throws ()
    {
      var order = Order.NewObject();
      order.Delete();
      Assert.That(order.State.IsInvalid, Is.True);

      Assert.That(() => LifetimeService.GetObjects<Order>(TestableClientTransaction, order.ID), Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void TryGetObjects_WithInvalidObject_Works ()
    {
      var order = Order.NewObject();
      order.Delete();
      Assert.That(order.State.IsInvalid, Is.True);

      Assert.That(LifetimeService.TryGetObjects<Order>(TestableClientTransaction, order.ID), Is.EqualTo(new[] { order }));
    }

    [Test]
    public void TryGetObjects_WithMoreThan2100IDs ()
    {
      var ids = new List<ObjectID>();
      ids.Add(DomainObjectIDs.Order1);
      ids.Add(DomainObjectIDs.Order3);
      ids.AddRange(Enumerable.Range(0, 4000).Select(i => new ObjectID(DomainObjectIDs.Order1.ClassDefinition, Guid.NewGuid())));

      var result = LifetimeService.TryGetObjects<Order>(TestableClientTransaction, ids.ToArray());
      Assert.That(result.Length, Is.EqualTo(4002));
      Assert.That(result.Distinct().ToArray(), Is.EqualTo(new[] { DomainObjectIDs.Order1.GetObject<Order>(), DomainObjectIDs.Order3.GetObject<Order>(), null }));
    }

  }
}
