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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionExtensionTest : ClientTransactionBaseTest
  {
    private ClientTransaction _subTransaction;
    private ClientTransactionScope _subTransactionScope;

    private Order _order1;
    private DataManager _parentTransactionDataManager;
    private DataManager _subTransactionDataManager;
    private Product _product1;

    public override void SetUp ()
    {
      base.SetUp();


      _subTransaction = TestableClientTransaction.CreateSubTransaction();
      _subTransactionScope = _subTransaction.EnterDiscardingScope();

      _order1 = DomainObjectIDs.Order1.GetObject<Order>();
      _product1 = DomainObjectIDs.Product1.GetObject<Product>();

      _parentTransactionDataManager = ClientTransactionTestHelper.GetDataManager(_subTransaction.ParentTransaction);
      _subTransactionDataManager = ClientTransactionTestHelper.GetDataManager(_subTransaction);
    }

    public override void TearDown ()
    {
      TestableClientTransaction.Extensions.Remove("TestExtension");
      _subTransaction.Extensions.Remove("TestExtension");
      _subTransactionScope.Leave();

      base.TearDown();
    }

    [Test]
    public void NewObjectCreation ()
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);

      var sequence = new VerifiableSequence();

      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.NewObjectCreating(_subTransaction, typeof(Order))).Verifiable();

      Order.NewObject();

      extensionMock.Verify();
    }

    [Test]
    public void ObjectLoading ()
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);

      var sequence = new VerifiableSequence();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction.ParentTransaction, new[] { DomainObjectIDs.Order3 }))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction.ParentTransaction, It.Is<ReadOnlyCollection<DomainObject>>(list => list.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction, new[] { DomainObjectIDs.Order3 }))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction, It.Is<ReadOnlyCollection<DomainObject>>(list => list.Count == 1)))
          .Verifiable();

      Dev.Null = DomainObjectIDs.Order3.GetObject<Order>();
      Dev.Null = DomainObjectIDs.Order3.GetObject<Order>();

      extensionMock.Verify();
      sequence.Verify();
    }

    private void TestObjectLoadingWithRelatedObject (
        Action accessCode,
        ObjectID expectedMainObjectID,
        bool expectLoadEventsForRelatedObject,
        ObjectID expectedRelatedID)
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction.ParentTransaction, new[] { expectedMainObjectID }))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction.ParentTransaction, It.IsNotNull<IReadOnlyList<DomainObject>>()))
          .Verifiable();
      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectsLoading(_subTransaction, new[] { expectedMainObjectID })).Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction, It.IsNotNull<IReadOnlyList<DomainObject>>()))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_subTransaction, It.IsNotNull<DomainObject>(), It.IsNotNull<IRelationEndPointDefinition>(), ValueAccess.Current))
          .Verifiable();
      if (expectLoadEventsForRelatedObject)
      {
        extensionMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.ObjectsLoading(_subTransaction.ParentTransaction, It.Is<ReadOnlyCollection<ObjectID>>(_ => _.Contains(expectedRelatedID))))
            .Verifiable();
        extensionMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.ObjectsLoaded(_subTransaction.ParentTransaction, It.IsNotNull<IReadOnlyList<DomainObject>>()))
            .Verifiable();
        extensionMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.ObjectsLoading(_subTransaction, It.Is<ReadOnlyCollection<ObjectID>>(_ => _.Contains(expectedRelatedID))))
            .Verifiable();
        extensionMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.ObjectsLoaded(_subTransaction, It.IsNotNull<IReadOnlyList<DomainObject>>()))
            .Verifiable();
      }

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationRead(
                  _subTransaction,
                  It.IsNotNull<DomainObject>(),
                  It.IsNotNull<IRelationEndPointDefinition>(),
                  It.IsAny<DomainObject>(),
                  ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationReading(
                  _subTransaction,
                  It.IsNotNull<DomainObject>(),
                  It.IsNotNull<IRelationEndPointDefinition>(),
                  ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationRead(
                  _subTransaction,
                  It.IsNotNull<DomainObject>(),
                  It.IsNotNull<IRelationEndPointDefinition>(),
                  It.IsAny<DomainObject>(),
                  ValueAccess.Current))
          .Verifiable();
      accessCode();
      accessCode();

      extensionMock.Verify();
      sequence.Verify();
    }

    private void TestObjectLoadingWithRelatedObjectCollection (
        Action accessCode,
        ObjectID expectedMainObjectID,
        bool expectLoadEventsForRelatedObjects,
        ObjectID[] expectedRelatedIDs)
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);

      var sequence = new VerifiableSequence();
      // loading of main object
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction.ParentTransaction, new[] { expectedMainObjectID }))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction.ParentTransaction, It.IsNotNull<IReadOnlyList<DomainObject>>()))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction, new[] { expectedMainObjectID })).Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction, It.IsNotNull<IReadOnlyList<DomainObject>>()))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_subTransaction, It.IsNotNull<DomainObject>(), It.IsNotNull<IRelationEndPointDefinition>(), ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationRead(
                  _subTransaction,
                  It.IsNotNull<DomainObject>(),
                  It.IsNotNull<IRelationEndPointDefinition>(),
                  It.IsNotNull<IReadOnlyCollectionData<DomainObject>>(),
                  ValueAccess.Current))
          .Verifiable();

      if (expectLoadEventsForRelatedObjects)
      {
        extensionMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.ObjectsLoading(_subTransaction.ParentTransaction, It.Is<ReadOnlyCollection<ObjectID>>(p => p.SetEquals(expectedRelatedIDs))))
            .Verifiable();
        extensionMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.ObjectsLoaded(_subTransaction.ParentTransaction, It.IsNotNull<IReadOnlyList<DomainObject>>()))
            .Verifiable();

        extensionMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.ObjectsLoading(_subTransaction, It.Is<ReadOnlyCollection<ObjectID>>(p => p.SetEquals(expectedRelatedIDs))))
            .Verifiable();
        extensionMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.ObjectsLoaded(_subTransaction, It.IsNotNull<IReadOnlyList<DomainObject>>()))
            .Verifiable();
      }

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationReading(
                  _subTransaction,
                  It.IsNotNull<DomainObject>(),
                  It.IsNotNull<IRelationEndPointDefinition>(),
                  ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationRead(
                  _subTransaction,
                  It.IsNotNull<DomainObject>(),
                  It.IsNotNull<IRelationEndPointDefinition>(),
                  It.IsNotNull<IReadOnlyCollectionData<DomainObject>>(),
                  ValueAccess.Current))
          .Verifiable();

      accessCode();
      accessCode();

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1Side ()
    {
      TestObjectLoadingWithRelatedObjectCollection(
          delegate
          {
            Order order = DomainObjectIDs.Order3.GetObject<Order>();
            int orderItemCount = order.OrderItems.Count;
            Assert.That(orderItemCount, Is.EqualTo(1));
          },
          DomainObjectIDs.Order3,
          true,
          new[] { DomainObjectIDs.OrderItem3 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjectsNSide ()
    {
      TestObjectLoadingWithRelatedObject(
          delegate
          {
            OrderItem orderItem = DomainObjectIDs.OrderItem3.GetObject<OrderItem>();
            Order order = orderItem.Order;
            Assert.That(order, Is.Not.Null);
          },
          DomainObjectIDs.OrderItem3,
          false,
          DomainObjectIDs.Order3);
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1To1RealSide ()
    {
      TestObjectLoadingWithRelatedObject(
          delegate
          {
            Computer computer = DomainObjectIDs.Computer1.GetObject<Computer>();
            Employee employee = computer.Employee;
            Assert.That(employee, Is.Not.Null);
          },
          DomainObjectIDs.Computer1,
          false,
          DomainObjectIDs.Employee3);
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1To1VirtualSide ()
    {
      TestObjectLoadingWithRelatedObject(
          delegate
          {
            Employee employee = DomainObjectIDs.Employee3.GetObject<Employee>();
            Computer computer = employee.Computer;
            Assert.That(computer, Is.Not.Null);
          },
          DomainObjectIDs.Employee3,
          true,
          DomainObjectIDs.Computer1);
    }

    [Test]
    public void EmptyObjectLoadingWithRelatedObjects1Side ()
    {
      TestObjectLoadingWithRelatedObjectCollection(
          delegate
          {
            Customer customer = DomainObjectIDs.Customer2.GetObject<Customer>();
            int count = customer.Orders.Count;
            Assert.That(count, Is.EqualTo(0));
          },
          DomainObjectIDs.Customer2,
          false,
          null);
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjectsNSide ()
    {
      TestObjectLoadingWithRelatedObject(
          delegate
          {
            Client client = DomainObjectIDs.Client1.GetObject<Client>();
            Client parent = client.ParentClient;
            Assert.That(parent, Is.Null);
          },
          DomainObjectIDs.Client1,
          false,
          null);
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjects1To1RealSide ()
    {
      TestObjectLoadingWithRelatedObject(
          delegate
          {
            Computer computer = DomainObjectIDs.Computer4.GetObject<Computer>();
            Employee employee = computer.Employee;
            Assert.That(employee, Is.Null);
          },
          DomainObjectIDs.Computer4,
          false,
          null);
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjects1To1VirtualSide ()
    {
      TestObjectLoadingWithRelatedObject(
          delegate
          {
            Employee employee = DomainObjectIDs.Employee7.GetObject<Employee>();
            Computer computer = employee.Computer;
            Assert.That(computer, Is.Null);
          },
          DomainObjectIDs.Employee7,
          false,
          null);
    }

    [Test]
    public void ObjectsLoaded ()
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);

      var sequence = new VerifiableSequence();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction.ParentTransaction, new[] { DomainObjectIDs.ClassWithAllDataTypes1 }))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction.ParentTransaction, It.Is<ReadOnlyCollection<DomainObject>>(list => list.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction, new[] { DomainObjectIDs.ClassWithAllDataTypes1 }))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction, It.Is<ReadOnlyCollection<DomainObject>>(list => list.Count == 1)))
          .Verifiable();

      DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectsLoadedWithRelations ()
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);

      var sequence = new VerifiableSequence();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction.ParentTransaction, new[] { DomainObjectIDs.Order3 }))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction.ParentTransaction, It.Is<ReadOnlyCollection<DomainObject>>(list => list.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction, new[] { DomainObjectIDs.Order3 }))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction, It.Is<ReadOnlyCollection<DomainObject>>(list => list.Count == 1)))
          .Verifiable();

      DomainObjectIDs.Order3.GetObject<Order>();

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectsLoadedWithEvents ()
    {
      var clientTransactionEventReceiver =
          ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Strict, _subTransaction);

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction.ParentTransaction, new[] { DomainObjectIDs.ClassWithAllDataTypes1 }))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction.ParentTransaction, It.Is<ReadOnlyCollection<DomainObject>>(list => list.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction, new[] { DomainObjectIDs.ClassWithAllDataTypes1 }))
          .Verifiable();
      clientTransactionEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Loaded(_subTransaction, It.Is<ClientTransactionEventArgs>(args => args.DomainObjects.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction, It.Is<ReadOnlyCollection<DomainObject>>(list => list.Count == 1)))
          .Verifiable();

      DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();

      extensionMock.Verify();
      clientTransactionEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectDelete ()
    {
      Computer computer = DomainObjectIDs.Computer4.GetObject<Computer>();

      var computerEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, computer);

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleting(_subTransaction, computer)).Verifiable();

      computerEventReceiver.InVerifiableSequence(sequence).Setup(mock => mock.Deleting(computer, EventArgs.Empty)).Verifiable();

      computerEventReceiver.InVerifiableSequence(sequence).Setup(mock => mock.Deleted(computer, EventArgs.Empty)).Verifiable();

      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleted(_subTransaction, computer)).Verifiable();

      computer.Delete();

      extensionMock.Verify();
      computerEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectDeleteWithOldRelatedObjects ()
    {
      OrderItem orderItem1 = _order1.OrderItems[0];
      OrderItem orderItem2 = _order1.OrderItems[1];
      OrderTicket orderTicket = _order1.OrderTicket;
      Official official = _order1.Official;
      Customer customer = _order1.Customer;
      OrderCollection customerOrders = customer.Orders;
      customerOrders.EnsureDataComplete();
      ObjectList<Order> officialOrders = official.Orders;
      officialOrders.EnsureDataComplete();
      Dev.Null = orderTicket.Order; // preload

      var order1MockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _order1);
      var orderItem1MockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, orderItem1);
      var orderItem2MockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, orderItem2);
      var orderTicketMockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, orderTicket);
      var officialMockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, official);
      var customerMockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, customer);

      var customerOrdersMockEventReceiver =
          DomainObjectCollectionMockEventReceiver.CreateMock(MockBehavior.Strict, customerOrders);

      var officialOrdersMockEventReceiver =
          DomainObjectCollectionMockEventReceiver.CreateMock(MockBehavior.Strict, officialOrders);

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleting(_subTransaction, _order1)).Verifiable();

      order1MockEventReceiver.InVerifiableSequence(sequence).Setup(mock => mock.Deleting(_order1, EventArgs.Empty)).Verifiable();

      customerOrdersMockEventReceiver
          .SetupRemoving(customerOrders, _order1)
          .Verifiable();
      extensionMock
          .Setup(mock => mock.RelationChanging(_subTransaction, customer, GetEndPointDefinition(typeof(Customer), "Orders"), _order1, null))
          .Verifiable();
      customerMockEventReceiver
          .SetupRelationChanging(customer, GetEndPointDefinition(typeof(Customer), "Orders"), _order1, null)
          .Verifiable();

      extensionMock
          .Setup(mock => mock.RelationChanging(_subTransaction, orderTicket, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null))
          .Verifiable();
      orderTicketMockEventReceiver
          .SetupRelationChanging(orderTicket, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null)
          .Verifiable();

      extensionMock
          .Setup(mock => mock.RelationChanging(_subTransaction, orderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null))
          .Verifiable();
      orderItem1MockEventReceiver
          .SetupRelationChanging(orderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null)
          .Verifiable();

      extensionMock
          .Setup(mock => mock.RelationChanging(_subTransaction, orderItem2, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null))
          .Verifiable();
      orderItem2MockEventReceiver
          .SetupRelationChanging(orderItem2, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null)
          .Verifiable();

      officialOrdersMockEventReceiver
          .SetupRemoving(officialOrders, _order1)
          .Verifiable();
      extensionMock
          .Setup(mock => mock.RelationChanging(_subTransaction, official, GetEndPointDefinition(typeof(Official), "Orders"), _order1, null))
          .Verifiable();
      officialMockEventReceiver
          .SetupRelationChanging(official, GetEndPointDefinition(typeof(Official), "Orders"), _order1, null)
          .Verifiable();

      customerMockEventReceiver
          .SetupRelationChanged(customer, GetEndPointDefinition(typeof(Customer), "Orders"), _order1, null)
          .Verifiable();
      extensionMock
          .Setup(mock => mock.RelationChanged(_subTransaction, customer, GetEndPointDefinition(typeof(Customer), "Orders"), _order1, null))
          .Verifiable();
      customerOrdersMockEventReceiver
          .SetupRemoved(customerOrders, _order1)
          .Verifiable();

      orderTicketMockEventReceiver
          .SetupRelationChanged(orderTicket, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null)
          .Verifiable();
      extensionMock
          .Setup(mock => mock.RelationChanged(_subTransaction, orderTicket, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null))
          .Verifiable();

      orderItem1MockEventReceiver
          .SetupRelationChanged(orderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null)
          .Verifiable();
      extensionMock
          .Setup(mock => mock.RelationChanged(_subTransaction, orderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null))
          .Verifiable();

      orderItem2MockEventReceiver
          .SetupRelationChanged(orderItem2, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null)
          .Verifiable();
      extensionMock
          .Setup(mock => mock.RelationChanged(_subTransaction, orderItem2, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null))
          .Verifiable();

      officialOrdersMockEventReceiver
          .SetupRemoved(officialOrders, _order1)
          .Verifiable();
      officialMockEventReceiver
          .SetupRelationChanged(official, GetEndPointDefinition(typeof(Official), "Orders"), _order1, null)
          .Verifiable();
      extensionMock
          .Setup(mock => mock.RelationChanged(_subTransaction, official, GetEndPointDefinition(typeof(Official), "Orders"), _order1, null))
          .Verifiable();

      order1MockEventReceiver.InVerifiableSequence(sequence).Setup(mock => mock.Deleted(_order1, EventArgs.Empty)).Verifiable();

      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleted(_subTransaction, _order1)).Verifiable();
      _order1.Delete();

      extensionMock.Verify();
      order1MockEventReceiver.Verify();
      orderItem1MockEventReceiver.Verify();
      orderItem2MockEventReceiver.Verify();
      orderTicketMockEventReceiver.Verify();
      officialMockEventReceiver.Verify();
      customerMockEventReceiver.Verify();
      customerOrdersMockEventReceiver.Verify();
      officialOrdersMockEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void RelationChangesWithUnidirectionalRelationshipWhenResettingDeletedLoaded ()
    {
      Location location = DomainObjectIDs.Location1.GetObject<Location>();

      Client deletedClient = location.Client;
      deletedClient.Delete();

      Client newClient = Client.NewObject();

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanging(_subTransaction, location, GetEndPointDefinition(typeof(Location), "Client"), deletedClient, newClient))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanged(_subTransaction, location, GetEndPointDefinition(typeof(Location), "Client"), deletedClient, newClient))
          .Verifiable();
      location.Client = newClient;

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void RelationChangesWithUnidirectionalRelationshipWhenResettingNewLoaded ()
    {
      Location location = DomainObjectIDs.Location1.GetObject<Location>();
      location.Client = Client.NewObject();

      Client deletedClient = location.Client;
      location.Client.Delete();

      Client newClient = Client.NewObject();

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanging(_subTransaction, location, GetEndPointDefinition(typeof(Location), "Client"), deletedClient, newClient))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanged(_subTransaction, location, GetEndPointDefinition(typeof(Location), "Client"), deletedClient, newClient))
          .Verifiable();
      location.Client = newClient;

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectDeleteTwice ()
    {
      Computer computer = DomainObjectIDs.Computer4.GetObject<Computer>();

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleting(_subTransaction, computer)).Verifiable();

      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleted(_subTransaction, computer)).Verifiable();

      computer.Delete();
      computer.Delete();

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void PropertyRead ()
    {
      int orderNumber = _order1.OrderNumber;
      var propertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueReading(_subTransaction, _order1, propertyDefinition, ValueAccess.Current))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueRead(_subTransaction, _order1, propertyDefinition, orderNumber, ValueAccess.Current))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueReading(_subTransaction, _order1, propertyDefinition, ValueAccess.Original))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueRead(_subTransaction, _order1, propertyDefinition, orderNumber, ValueAccess.Original))
          .Verifiable();

      Dev.Null = _order1.OrderNumber;
      Dev.Null = _order1.Properties[propertyDefinition.PropertyName].GetOriginalValueWithoutTypeCheck();

      extensionMock.Verify();
    }

    [Test]
    public void ReadObjectIDProperty ()
    {
      var propertyDefinition = GetPropertyDefinition(typeof(Order), "Customer");
      var customerID = _order1.Properties[propertyDefinition.PropertyName].GetRelatedObjectID();

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueReading(_subTransaction, _order1, propertyDefinition, ValueAccess.Current))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueRead(_subTransaction, _order1, propertyDefinition, customerID, ValueAccess.Current))
          .Verifiable();

      Dev.Null = _order1.Properties[propertyDefinition.PropertyName].GetRelatedObjectID();

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void PropertySetToSameValue ()
    {
      int orderNumber = _order1.OrderNumber;

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);

      _order1.OrderNumber = orderNumber;

      extensionMock.Verify();
    }

    [Test]
    public void ChangeAndReadProperty ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      int newOrderNumber = oldOrderNumber + 1;

      var propertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueChanging(_subTransaction, _order1, propertyDefinition, oldOrderNumber, newOrderNumber))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueChanged(_subTransaction, _order1, propertyDefinition, oldOrderNumber, newOrderNumber))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueReading(_subTransaction, _order1, propertyDefinition, ValueAccess.Current))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueRead(_subTransaction, _order1, propertyDefinition, newOrderNumber, ValueAccess.Current))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueReading(_subTransaction, _order1, propertyDefinition, ValueAccess.Original))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueRead(_subTransaction, _order1, propertyDefinition, oldOrderNumber, ValueAccess.Original))
          .Verifiable();

      _order1.OrderNumber = newOrderNumber;
      Dev.Null = _order1.OrderNumber;
      Dev.Null = _order1.Properties[typeof(Order), "OrderNumber"].GetOriginalValueWithoutTypeCheck();

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void PropertyChange ()
    {
      int oldOrderNumber = _order1.OrderNumber;

      var propertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueChanging(_subTransaction, _order1, propertyDefinition, oldOrderNumber, oldOrderNumber + 1))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueChanged(_subTransaction, _order1, propertyDefinition, oldOrderNumber, oldOrderNumber + 1))
          .Verifiable();

      _order1.OrderNumber = oldOrderNumber + 1;

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void PropertyChangeWithEvents ()
    {
      int oldOrderNumber = _order1.OrderNumber;

      var domainObjectMockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _order1);
      var propertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueChanging(_subTransaction, _order1, propertyDefinition, oldOrderNumber, oldOrderNumber + 1))
          .Verifiable();
      domainObjectMockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyChanging(It.IsAny<object>(), It.IsAny<PropertyChangeEventArgs>()))
          .Verifiable();
      domainObjectMockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyChanged(It.IsAny<object>(), It.IsAny<PropertyChangeEventArgs>()))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueChanged(_subTransaction, _order1, propertyDefinition, oldOrderNumber, oldOrderNumber + 1))
          .Verifiable();
      _order1.OrderNumber = oldOrderNumber + 1;

      extensionMock.Verify();
      domainObjectMockEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetRelatedObject ()
    {
      OrderTicket orderTicket = _order1.OrderTicket;

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), ValueAccess.Current))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationRead(_subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), orderTicket, ValueAccess.Current))
          .Verifiable();
      Dev.Null = _order1.OrderTicket;

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      var originalOrderTicket = (OrderTicket)_order1.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), ValueAccess.Original))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationRead(_subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), originalOrderTicket, ValueAccess.Original))
          .Verifiable();
      Dev.Null = _order1.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetRelatedObjects ()
    {
      ObjectList<OrderItem> orderItems = _order1.OrderItems;
      orderItems.EnsureDataComplete();

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationRead(
                  _subTransaction,
                  _order1,
                  GetEndPointDefinition(typeof(Order), "OrderItems"),
                  It.Is<IReadOnlyCollectionData<DomainObject>>(_ => _.SetEquals(orderItems)),
                  ValueAccess.Current))
          .Verifiable();

      Dev.Null = _order1.OrderItems;

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjects_ForDomainObjectCollection ()
    {
      ObjectList<OrderItem> originalOrderItems =
          (ObjectList<OrderItem>)_order1.GetOriginalRelatedObjectsAsDomainObjectCollection(
              "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Original))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationRead(
                  _subTransaction,
                  _order1,
                  GetEndPointDefinition(typeof(Order), "OrderItems"),
                  It.Is<IReadOnlyCollectionData<DomainObject>>(_ => _.SetEquals(originalOrderItems)),
                  ValueAccess.Original))
          .Verifiable();
      Dev.Null = _order1.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjects_ForVirtualCollection ()
    {
      var originalProductReviews =
          _product1.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews");

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_subTransaction, _product1, GetEndPointDefinition(typeof(Product), "Reviews"), ValueAccess.Original))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationRead(
                  _subTransaction,
                  _product1,
                  GetEndPointDefinition(typeof(Product), "Reviews"),
                  It.Is<IReadOnlyCollectionData<DomainObject>>(_ => _.SetEquals(originalProductReviews)),
                  ValueAccess.Original))
          .Verifiable();
      Dev.Null = _product1.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews");

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetRelatedObjectWithLazyLoad ()
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction.ParentTransaction, It.Is<ReadOnlyCollection<ObjectID>>(list => list.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction.ParentTransaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction, It.Is<ReadOnlyCollection<ObjectID>>(list => list.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction, It.Is<IReadOnlyList<DomainObject>>(list => list.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationRead(
                  _subTransaction,
                  _order1,
                  GetEndPointDefinition(typeof(Order), "OrderTicket"),
                  It.IsNotNull<DomainObject>(),
                  ValueAccess.Current))
          .Verifiable();
      Dev.Null = _order1.OrderTicket;

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad ()
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationRead(
                  _subTransaction,
                  _order1,
                  GetEndPointDefinition(typeof(Order), "OrderItems"),
                  It.IsNotNull<IReadOnlyCollectionData<DomainObject>>(),
                  ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction.ParentTransaction, It.Is<IReadOnlyList<ObjectID>>(_ => _.Count == 2)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction.ParentTransaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Count == 2)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction, It.Is<IReadOnlyList<ObjectID>>(_ => _.Count == 2)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Count == 2)))
          .Verifiable();
      _order1.OrderItems.EnsureDataComplete();

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), ValueAccess.Original))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction.ParentTransaction, It.Is<ReadOnlyCollection<ObjectID>>(_ => _.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction.ParentTransaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction, It.Is<ReadOnlyCollection<ObjectID>>(list => list.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationRead(
                  _subTransaction,
                  _order1,
                  GetEndPointDefinition(typeof(Order), "OrderTicket"),
                  It.IsNotNull<DomainObject>(),
                  ValueAccess.Original))
          .Verifiable();

      Dev.Null = _order1.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad_ForDomainObjectCollection ()
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Original))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction.ParentTransaction, It.Is<ReadOnlyCollection<ObjectID>>(list => list.Count == 2)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction.ParentTransaction, It.Is<IReadOnlyList<DomainObject>>(list => list.Count == 2)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction, It.Is<ReadOnlyCollection<ObjectID>>(list => list.Count == 2)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction, It.Is<IReadOnlyList<DomainObject>>(list => list.Count == 2)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationRead(
                  _subTransaction,
                  _order1,
                  GetEndPointDefinition(typeof(Order), "OrderItems"),
                  It.IsNotNull<IReadOnlyCollectionData<DomainObject>>(),
                  ValueAccess.Original))
          .Verifiable();
      Dev.Null = _order1.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad_ForVirtualCollection ()
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_subTransaction, _product1, GetEndPointDefinition(typeof(Product), "Reviews"), ValueAccess.Original))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction.ParentTransaction, It.Is<ReadOnlyCollection<ObjectID>>(list => list.Count == 3)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction.ParentTransaction, It.Is<IReadOnlyList<DomainObject>>(list => list.Count == 3)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction, It.Is<ReadOnlyCollection<ObjectID>>(list => list.Count == 3)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction, It.Is<IReadOnlyList<DomainObject>>(list => list.Count == 3)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationRead(
                  _subTransaction,
                  _product1,
                  GetEndPointDefinition(typeof(Product), "Reviews"),
                  It.IsNotNull<IReadOnlyCollectionData<DomainObject>>(),
                  ValueAccess.Original))
          .Verifiable();
      Dev.Null = _product1.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews");

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void FilterQueryResult ()
    {
      IQuery query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderQuery"));
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer1);

      // preload query results to avoid Load notifications later on
      LifetimeService.GetObject(_subTransaction, DomainObjectIDs.Order1, true);
      LifetimeService.GetObject(_subTransaction, DomainObjectIDs.Order2, true);

      QueryResult<DomainObject> parentFilteredQueryResult = TestQueryFactory.CreateTestQueryResult(StorageSettings, new[] { _order1 });
      QueryResult<DomainObject> subFilteredQueryResult = TestQueryFactory.CreateTestQueryResult(StorageSettings);

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      extensionMock
          .Setup(mock => mock.FilterQueryResult(_subTransaction.ParentTransaction, It.Is<QueryResult<DomainObject>>(qr => qr.Count == 2 && qr.Query == query)))
          .Returns(parentFilteredQueryResult)
          .Verifiable();
      extensionMock
          .Setup(mock => mock.FilterQueryResult(_subTransaction, It.Is<QueryResult<DomainObject>>(qr => qr.Count == 1 && qr.Query == query)))
          .Returns(subFilteredQueryResult)
          .Verifiable();

      QueryResult<DomainObject> finalResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(query);
      Assert.That(finalResult, Is.SameAs(subFilteredQueryResult));

      extensionMock.Verify();
    }

    [Test]
    public void FilterQueryResultWithLoad ()
    {
      IQuery query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderQuery"));
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer4); // yields Order4, Order5

      QueryResult<DomainObject> parentFilteredQueryResult = TestQueryFactory.CreateTestQueryResult(StorageSettings, new[] { _order1 });
      QueryResult<DomainObject> subFilteredQueryResult = TestQueryFactory.CreateTestQueryResult(StorageSettings);

      UnloadService.UnloadData(_subTransaction, _order1.ID); // unload _order1 to force Load events
      ClientTransactionTestHelper.SetIsWriteable(TestableClientTransaction, true);
      TestableClientTransaction.EnsureDataAvailable(DomainObjectIDs.Order1); // we only want Load events in the sub-transaction
      ClientTransactionTestHelper.SetIsWriteable(TestableClientTransaction, false);

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .Setup(
              mock => mock.ObjectsLoading(
                  _subTransaction.ParentTransaction,
                  It.Is<ReadOnlyCollection<ObjectID>>(list => list.SetEquals(new[] { DomainObjectIDs.Order4, DomainObjectIDs.Order5 }))))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction.ParentTransaction, It.Is<ReadOnlyCollection<DomainObject>>(list => list.Count == 2)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.FilterQueryResult(_subTransaction.ParentTransaction, It.Is<QueryResult<DomainObject>>(qr => qr.Count == 2 && qr.Query == query)))
          .Returns(parentFilteredQueryResult)
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction, It.Is<ReadOnlyCollection<ObjectID>>(list => list.SetEquals(new[] { DomainObjectIDs.Order1 }))))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction, It.Is<ReadOnlyCollection<DomainObject>>(list => list.Count == 1)))
          .Verifiable();
      extensionMock
          .Setup(mock => mock.FilterQueryResult(_subTransaction, It.Is<QueryResult<DomainObject>>(qr => qr.Count == 1 && qr.Query == query)))
          .Returns(subFilteredQueryResult)
          .Verifiable();

      QueryResult<DomainObject> finalQueryResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(query);
      Assert.That(finalQueryResult, Is.SameAs(subFilteredQueryResult));

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void CommitWithChangedPropertyValue ()
    {
      Computer computer = DomainObjectIDs.Computer4.GetObject<Computer>();
      computer.SerialNumber = "newSerialNumber";

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Committing(_subTransaction, new[] { computer }, It.IsNotNull<CommittingEventRegistrar>()))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CommitValidate(_subTransaction, It.Is<ReadOnlyCollection<PersistableData>>(c => c.Select(d => d.DomainObject).SetEquals(new[] { computer }))))
          .Verifiable();
      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.Committed(_subTransaction, new[] { computer })).Verifiable();

      _subTransaction.Commit();

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void CommitWithChangedRelationValue ()
    {
      Computer computer = DomainObjectIDs.Computer4.GetObject<Computer>();
      Employee employee = DomainObjectIDs.Employee1.GetObject<Employee>();
      computer.Employee = employee;

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.Committing(
                  _subTransaction,
                  It.Is<ReadOnlyCollection<DomainObject>>(p => p.SetEquals(new DomainObject[] { computer, employee })),
                  It.IsNotNull<CommittingEventRegistrar>()))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.CommitValidate(
                  _subTransaction,
                  It.Is<ReadOnlyCollection<PersistableData>>(c => c.Select(d => d.DomainObject).SetEquals(new DomainObject[] { computer, employee }))))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Committed(_subTransaction, It.Is<ReadOnlyCollection<DomainObject>>(p => p.SetEquals(new DomainObject[] { computer, employee }))))
          .Verifiable();

      _subTransaction.Commit();
      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void CommitWithChangedRelationValueWithClassIDColumn ()
    {
      Customer oldCustomer = _order1.Customer;
      Customer newCustomer = DomainObjectIDs.Customer2.GetObject<Customer>();
      _order1.Customer = newCustomer;

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.Committing(
                  _subTransaction,
                  It.Is<ReadOnlyCollection<DomainObject>>(p => p.SetEquals(new DomainObject[] { _order1, newCustomer, oldCustomer })),
                  It.IsNotNull<CommittingEventRegistrar>()))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.CommitValidate(
                  _subTransaction,
                  It.Is<ReadOnlyCollection<PersistableData>>(c => c.Select(d => d.DomainObject).SetEquals(new DomainObject[] { _order1, newCustomer, oldCustomer }))))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Committed(_subTransaction, It.Is<ReadOnlyCollection<DomainObject>>(p => p.SetEquals(new DomainObject[] { _order1, newCustomer, oldCustomer }))))
          .Verifiable();

      _subTransactionScope.Commit();

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void CommitWithEvents ()
    {
      Computer computer = DomainObjectIDs.Computer4.GetObject<Computer>();
      computer.SerialNumber = "newSerialNumber";

      var clientTransactionMockEventReceiver = ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Strict, _subTransaction);

      var computerEventReveiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, computer);

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Committing(_subTransaction, new[] { computer }, It.IsNotNull<CommittingEventRegistrar>()))
          .Verifiable();
      clientTransactionMockEventReceiver.InVerifiableSequence(sequence).SetupCommitting(_subTransaction, computer).Verifiable();
      computerEventReveiver.InVerifiableSequence(sequence).Setup(mock => mock.Committing(computer, It.IsAny<DomainObjectCommittingEventArgs>())).Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CommitValidate(_subTransaction, It.Is<ReadOnlyCollection<PersistableData>>(c => c.Select(d => d.DomainObject).SetEquals(new[] { computer }))))
          .Verifiable();
      computerEventReveiver.InVerifiableSequence(sequence).Setup(mock => mock.Committed(computer, It.IsAny<EventArgs>())).Verifiable();
      clientTransactionMockEventReceiver.InVerifiableSequence(sequence).SetupCommitted(_subTransaction, computer).Verifiable();
      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.Committed(_subTransaction, new[] { computer })).Verifiable();

      _subTransaction.Commit();

      extensionMock.Verify();
      clientTransactionMockEventReceiver.Verify();
      computerEventReveiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void Rollback ()
    {
      Computer computer = DomainObjectIDs.Computer4.GetObject<Computer>();
      computer.SerialNumber = "newSerialNumber";

      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RollingBack(_subTransaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Contains(computer))))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RolledBack(_subTransaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Contains(computer))))
          .Verifiable();

      ClientTransactionScope.CurrentTransaction.Rollback();

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetObjects ()
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction.ParentTransaction, new[] { DomainObjectIDs.Order3, DomainObjectIDs.Order4 }))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction.ParentTransaction, It.Is<ReadOnlyCollection<DomainObject>>(list => list.Count == 2)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_subTransaction, new[] { DomainObjectIDs.Order3, DomainObjectIDs.Order4 }))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_subTransaction, It.Is<ReadOnlyCollection<DomainObject>>(list => list.Count == 2)))
          .Verifiable();

      using (_subTransaction.EnterNonDiscardingScope())
      {
        LifetimeService.GetObjects<DomainObject>(_subTransaction, DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4);
      }

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void UnloadData ()
    {
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction, _subTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsUnloading(_subTransaction, new[] { _order1 }))
          .Callback((ClientTransaction _, IReadOnlyList<DomainObject> _) => Assert.That(_subTransactionDataManager.DataContainers[_order1.ID] != null))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsUnloading(_subTransaction.ParentTransaction, new[] { _order1 }))
          .Callback((ClientTransaction _, IReadOnlyList<DomainObject> _) => Assert.That(_parentTransactionDataManager.DataContainers[_order1.ID] != null))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsUnloaded(_subTransaction.ParentTransaction, new[] { _order1 }))
          .Callback((ClientTransaction _, IReadOnlyList<DomainObject> _) => Assert.That(_parentTransactionDataManager.DataContainers[_order1.ID] == null))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsUnloaded(_subTransaction, new[] { _order1 }))
          .Callback((ClientTransaction _, IReadOnlyList<DomainObject> _) => Assert.That(_subTransactionDataManager.DataContainers[_order1.ID] == null))
          .Verifiable();

      UnloadService.UnloadData(_subTransaction, _order1.ID);

      extensionMock.Verify();
      sequence.Verify();
    }

    private Mock<IClientTransactionExtension> AddExtensionToClientTransaction (ClientTransaction rootClientTransaction, ClientTransaction subClientTransaction)
    {
      var extensionMock = new Mock<IClientTransactionExtension>(MockBehavior.Strict);

      extensionMock.Setup(stub => stub.Key).Returns("TestExtension");
      rootClientTransaction.Extensions.Add(extensionMock.Object);
      subClientTransaction.Extensions.Add(extensionMock.Object);
      extensionMock.Reset();

      return extensionMock;
    }
  }
}
