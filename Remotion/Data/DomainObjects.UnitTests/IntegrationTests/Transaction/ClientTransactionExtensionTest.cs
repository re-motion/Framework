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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Is = NUnit.Framework.Is;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class ClientTransactionExtensionTest : StandardMappingTest
  {
    private TestableClientTransaction _transaction;

    private Order _order1;
    private Computer _computerWithoutRelatedObjects;
    private Product _product1;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = new TestableClientTransaction();

      _order1 = DomainObjectIDs.Order1.GetObject<Order>(_transaction);
      _computerWithoutRelatedObjects = DomainObjectIDs.Computer5.GetObject<Computer>(_transaction);
      _product1 = DomainObjectIDs.Product1.GetObject<Product>(_transaction);
    }

    public override void TearDown ()
    {
      _transaction.Extensions.Remove("TestExtension");
      base.TearDown();
    }

    [Test]
    public void Extensions ()
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      Assert.That(_transaction.Extensions, Has.Member(extensionMock.Object));
    }

    [Test]
    public void TransactionInitialize ()
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var factoryStub = new Mock<IClientTransactionExtensionFactory>();
      factoryStub
          .Setup(stub => stub.CreateClientTransactionExtensions(It.IsAny<ClientTransaction>()))
          .Returns(new[] { extensionMock.Object });

      var locatorStub = new Mock<IServiceLocator>(MockBehavior.Strict);
      locatorStub
          .Setup(stub => stub.GetInstance<IClientTransactionExtensionFactory>())
          .Returns(factoryStub.Object);
      locatorStub
          .Setup(stub => stub.GetInstance<IStorageSettings>())
          .Returns(Mock.Of<IStorageSettings>());
      locatorStub
          .Setup(stub => stub.GetInstance<IPersistenceExtensionFactory>())
          .Returns(Mock.Of<IPersistenceExtensionFactory>());


      using (new ServiceLocatorScope(locatorStub.Object))
      {
        ClientTransaction inititalizedTransaction = null;

        extensionMock.Setup(stub => stub.Key).Returns("test");
        extensionMock
            .Setup(mock => mock.TransactionInitialize(It.IsAny<ClientTransaction>()))
            .Callback((ClientTransaction clientTransaction) => inititalizedTransaction = clientTransaction)
            .Verifiable();

        var result = ClientTransaction.CreateRootTransaction();

        extensionMock.Verify();

        Assert.That(result, Is.SameAs(inititalizedTransaction));
      }
    }

    [Test]
    public void TransactionDiscard ()
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      extensionMock.Setup(mock => mock.TransactionDiscard(_transaction)).Verifiable();

      _transaction.Discard();

      extensionMock.Verify();
    }

    [Test]
    public void NewObjectCreation ()
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      extensionMock.Setup(mock => mock.NewObjectCreating(_transaction, typeof(Order))).Verifiable();

      _transaction.ExecuteInScope(() => Order.NewObject());

      extensionMock.Verify();
    }

    [Test]
    public void ObjectLoading ()
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_transaction, new[] { DomainObjectIDs.Order3 }))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.ObjectsLoaded(
                  _transaction,
                  It.Is<ReadOnlyCollection<DomainObject>>(loadedObjects => loadedObjects.Count == 1 && loadedObjects[0].ID == DomainObjectIDs.Order3)))
          .Verifiable();

      Dev.Null = DomainObjectIDs.Order3.GetObject<Order>(_transaction);
      Dev.Null = DomainObjectIDs.Order3.GetObject<Order>(_transaction);

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
          DomainObjectIDs.Customer2, false, new ObjectID[] { });
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
          DomainObjectIDs.Client1, false, null);
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
          DomainObjectIDs.Computer4, false, null);
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
          DomainObjectIDs.Employee7, false, null);
    }

    [Test]
    public void ObjectsLoaded ()
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      extensionMock
          .Setup(mock => mock.ObjectsLoading(_transaction, new[] { DomainObjectIDs.ClassWithAllDataTypes1 }))
          .Verifiable();
      extensionMock
          .Setup(mock => mock.ObjectsLoaded(_transaction, It.Is<ReadOnlyCollection<DomainObject>>(collection => collection.Count == 1)))
          .Verifiable();

      DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>(_transaction);

      extensionMock.Verify();
    }

    [Test]
    public void ObjectsLoadedWithRelations ()
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      extensionMock.Setup(mock => mock.ObjectsLoading(_transaction, new[] { DomainObjectIDs.Order3 })).Verifiable();
      extensionMock
          .Setup(mock => mock.ObjectsLoaded(_transaction, It.Is<ReadOnlyCollection<DomainObject>>(collection => collection.Count == 1)))
          .Verifiable();

      DomainObjectIDs.Order3.GetObject<Order>(_transaction);

      extensionMock.Verify();
    }

    [Test]
    public void ObjectsLoadedWithEvents ()
    {
      var clientTransactionEventReceiver =
          ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Strict, _transaction);

      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_transaction, new[] { DomainObjectIDs.ClassWithAllDataTypes1 }))
          .Verifiable();
      clientTransactionEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Loaded(_transaction, It.Is<ClientTransactionEventArgs>(args => args.DomainObjects.Count == 1))).Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_transaction, It.Is<ReadOnlyCollection<DomainObject>>(collection => collection.Count == 1)))
          .Verifiable();

      DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>(_transaction);

      extensionMock.Verify();
      clientTransactionEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectDelete ()
    {
      var eventReceiverMock = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _computerWithoutRelatedObjects);

      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();
      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleting(_transaction, _computerWithoutRelatedObjects)).Verifiable();
      eventReceiverMock.InVerifiableSequence(sequence).Setup(mock => mock.Deleting(_computerWithoutRelatedObjects, EventArgs.Empty)).Verifiable();
      eventReceiverMock.InVerifiableSequence(sequence).Setup(mock => mock.Deleted(_computerWithoutRelatedObjects, EventArgs.Empty)).Verifiable();
      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleted(_transaction, _computerWithoutRelatedObjects)).Verifiable();

      _transaction.ExecuteInScope(() => _computerWithoutRelatedObjects.Delete());

      extensionMock.Verify();
      eventReceiverMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectDeleteWithOldRelatedObjects ()
    {
      OrderItem orderItem1;
      OrderItem orderItem2;
      OrderTicket orderTicket;
      Official official;
      Customer customer;
      OrderCollection customerOrders;
      DomainObjectCollection officialOrders;
      using (_transaction.EnterNonDiscardingScope())
      {
        orderItem1 = _order1.OrderItems[0];
        orderItem2 = _order1.OrderItems[1];
        orderTicket = _order1.OrderTicket;
        official = _order1.Official;
        customer = _order1.Customer;
        customerOrders = customer.Orders;
        customerOrders.EnsureDataComplete();
        officialOrders = official.Orders;
        officialOrders.EnsureDataComplete();
        Dev.Null = orderTicket.Order; // preload
      }

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

      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();

      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleting(_transaction, _order1)).Verifiable();
      order1MockEventReceiver.InVerifiableSequence(sequence).Setup(mock => mock.Deleting(_order1, EventArgs.Empty)).Verifiable();

      customerOrdersMockEventReceiver
          .SetupRemoving(customerOrders, _order1)
          .Verifiable();
      extensionMock
          .Setup(mock => mock.RelationChanging(_transaction, customer, GetEndPointDefinition(typeof(Customer), "Orders"), _order1, null))
          .Verifiable();
      customerMockEventReceiver
          .SetupRelationChanging(customer, GetEndPointDefinition(typeof(Customer), "Orders"), _order1, null)
          .Verifiable();

      extensionMock
          .Setup(mock => mock.RelationChanging(_transaction, orderTicket, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null))
          .Verifiable();
      orderTicketMockEventReceiver
          .SetupRelationChanging(orderTicket, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null)
          .Verifiable();

      extensionMock
          .Setup(mock => mock.RelationChanging(_transaction, orderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null))
          .Verifiable();
      orderItem1MockEventReceiver
          .SetupRelationChanging(orderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null)
          .Verifiable();

      extensionMock
          .Setup(mock => mock.RelationChanging(_transaction, orderItem2, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null))
          .Verifiable();
      orderItem2MockEventReceiver
          .SetupRelationChanging(orderItem2, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null)
          .Verifiable();

      officialOrdersMockEventReceiver
          .SetupRemoving(officialOrders, _order1)
          .Verifiable();
      extensionMock
          .Setup(mock => mock.RelationChanging(_transaction, official, GetEndPointDefinition(typeof(Official), "Orders"), _order1, null))
          .Verifiable();
      officialMockEventReceiver
          .SetupRelationChanging(official, GetEndPointDefinition(typeof(Official), "Orders"), _order1, null)
          .Verifiable();

      customerMockEventReceiver
          .SetupRelationChanged(customer, GetEndPointDefinition(typeof(Customer), "Orders"), _order1, null)
          .Verifiable();
      extensionMock
          .Setup(mock => mock.RelationChanged(_transaction, customer, GetEndPointDefinition(typeof(Customer), "Orders"), _order1, null))
          .Verifiable();
      customerOrdersMockEventReceiver
          .SetupRemoved(customerOrders, _order1)
          .Verifiable();

      orderTicketMockEventReceiver
          .SetupRelationChanged(orderTicket, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null)
          .Verifiable();
      extensionMock
          .Setup(mock => mock.RelationChanged(_transaction, orderTicket, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null))
          .Verifiable();

      orderItem1MockEventReceiver
          .SetupRelationChanged(orderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null)
          .Verifiable();
      extensionMock
          .Setup(mock => mock.RelationChanged(_transaction, orderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null))
          .Verifiable();

      orderItem2MockEventReceiver
          .SetupRelationChanged(orderItem2, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null)
          .Verifiable();
      extensionMock
          .Setup(mock => mock.RelationChanged(_transaction, orderItem2, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null))
          .Verifiable();

      officialOrdersMockEventReceiver
          .SetupRemoved(officialOrders, _order1)
          .Verifiable();
      officialMockEventReceiver
          .SetupRelationChanged(official, GetEndPointDefinition(typeof(Official), "Orders"), _order1, null)
          .Verifiable();
      extensionMock
          .Setup(mock => mock.RelationChanged(_transaction, official, GetEndPointDefinition(typeof(Official), "Orders"), _order1, null))
          .Verifiable();

      order1MockEventReceiver.InVerifiableSequence(sequence).Setup(mock => mock.Deleted(_order1, EventArgs.Empty)).Verifiable();
      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleted(_transaction, _order1)).Verifiable();

      _transaction.ExecuteInScope(() => _order1.Delete());

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
      Location location;
      Client deletedClient;
      Client newClient;

      using (_transaction.EnterNonDiscardingScope())
      {
        location = DomainObjectIDs.Location1.GetObject<Location>();
        deletedClient = location.Client;
        deletedClient.Delete();
        newClient = Client.NewObject();
      }

      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanging(_transaction, location, GetEndPointDefinition(typeof(Location), "Client"), deletedClient, newClient))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanged(_transaction, location, GetEndPointDefinition(typeof(Location), "Client"), deletedClient, newClient))
          .Verifiable();

      _transaction.ExecuteInScope(() => location.Client = newClient);

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void RelationChangesWithUnidirectionalRelationshipWhenResettingNewLoaded ()
    {
      Location location;
      Client deletedClient;
      Client newClient;

      using (_transaction.EnterNonDiscardingScope())
      {
        location = DomainObjectIDs.Location1.GetObject<Location>();
        location.Client = Client.NewObject();

        deletedClient = location.Client;
        location.Client.Delete();

        newClient = Client.NewObject();
      }

      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanging(_transaction, location, GetEndPointDefinition(typeof(Location), "Client"), deletedClient, newClient))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanged(_transaction, location, GetEndPointDefinition(typeof(Location), "Client"), deletedClient, newClient))
          .Verifiable();

      _transaction.ExecuteInScope(() => location.Client = newClient);

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectDeleteTwice ()
    {
      var computer = DomainObjectIDs.Computer4.GetObject<Computer>(_transaction);
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();
      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleting(_transaction, computer)).Verifiable();
      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleted(_transaction, computer)).Verifiable();

      _transaction.ExecuteInScope(computer.Delete);
      _transaction.ExecuteInScope(computer.Delete);

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void PropertyRead ()
    {
      int orderNumber = _transaction.ExecuteInScope(() => _order1.OrderNumber);
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var propertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueReading(_transaction, _order1, propertyDefinition, ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueRead(_transaction, _order1, propertyDefinition, orderNumber, ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueReading(_transaction, _order1, propertyDefinition, ValueAccess.Original))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueRead(_transaction, _order1, propertyDefinition, orderNumber, ValueAccess.Original))
          .Verifiable();

      _transaction.ExecuteInScope(() => Dev.Null = _order1.OrderNumber);
      _transaction.ExecuteInScope(() => Dev.Null = _order1.Properties[propertyDefinition.PropertyName].GetOriginalValueWithoutTypeCheck());

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ReadObjectIDProperty ()
    {
      var customerPropertyDefinition = GetPropertyDefinition(typeof(Order), "Customer");
      var customerID = _order1.Properties[customerPropertyDefinition.PropertyName, _transaction].GetRelatedObjectID();

      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueReading(_transaction, _order1, customerPropertyDefinition, ValueAccess.Current))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueRead(_transaction, _order1, customerPropertyDefinition, customerID, ValueAccess.Current))
          .Verifiable();

      _transaction.ExecuteInScope(() => Dev.Null = _order1.Properties[customerPropertyDefinition.PropertyName, _transaction].GetRelatedObjectID());

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void PropertySetToSameValue ()
    {
      int orderNumber = _transaction.ExecuteInScope(() => _order1.OrderNumber);

      var extensionMock = AddExtensionToClientTransaction(_transaction);
      // Note: No method call on the extension is expected.

      _transaction.ExecuteInScope(() => _order1.OrderNumber = orderNumber);

      extensionMock.Verify();
    }

    [Test]
    public void ChangeAndReadProperty ()
    {
      int oldOrderNumber = _transaction.ExecuteInScope(() => _order1.OrderNumber);
      int newOrderNumber = oldOrderNumber + 1;

      var orderNumberPropertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueChanging(_transaction, _order1, orderNumberPropertyDefinition, oldOrderNumber, newOrderNumber))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueChanged(_transaction, _order1, orderNumberPropertyDefinition, oldOrderNumber, newOrderNumber))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueReading(_transaction, _order1, orderNumberPropertyDefinition, ValueAccess.Current))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueRead(_transaction, _order1, orderNumberPropertyDefinition, newOrderNumber, ValueAccess.Current))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueReading(_transaction, _order1, orderNumberPropertyDefinition, ValueAccess.Original))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueRead(_transaction, _order1, orderNumberPropertyDefinition, oldOrderNumber, ValueAccess.Original))
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        _order1.OrderNumber = newOrderNumber;
        Dev.Null = _order1.OrderNumber;
        Dev.Null = _order1.Properties[typeof(Order), "OrderNumber"].GetOriginalValueWithoutTypeCheck();
      }

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void PropertyChange ()
    {
      int oldOrderNumber = _transaction.ExecuteInScope(() => _order1.OrderNumber);
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var orderNumberPropertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueChanging(_transaction, _order1, orderNumberPropertyDefinition, oldOrderNumber, oldOrderNumber + 1))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueChanged(_transaction, _order1, orderNumberPropertyDefinition, oldOrderNumber, oldOrderNumber + 1))
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        _order1.OrderNumber = oldOrderNumber + 1;
      }

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void PropertyChangeWithEvents ()
    {
      int oldOrderNumber = _transaction.ExecuteInScope(() => _order1.OrderNumber);
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var domainObjectMockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _order1);
      var orderNumberPropertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

      var sequence = new VerifiableSequence();

      // "Changing" notifications

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueChanging(_transaction, _order1, orderNumberPropertyDefinition, oldOrderNumber, oldOrderNumber + 1))
          .Verifiable();
      domainObjectMockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyChanging(It.IsAny<object>(), It.IsAny<PropertyChangeEventArgs>()))
          .Verifiable();
      domainObjectMockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyChanged(It.IsAny<object>(), It.IsAny<PropertyChangeEventArgs>()))
          .Verifiable();

      // "Changed" notifications

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueChanged(_transaction, _order1, orderNumberPropertyDefinition, oldOrderNumber, oldOrderNumber + 1))
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        _order1.OrderNumber = oldOrderNumber + 1;
      }

      extensionMock.Verify();
      domainObjectMockEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void LoadRelatedDataContainerForVirtualEndPoint ()
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      //Note: no reading notification must be performed
      var persistenceManager = new PersistenceManager();
      using (var storageProviderManager = new StorageProviderManager(NullPersistenceExtension.Instance, StorageSettings))
      {
        ClassDefinition orderDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Order));
        IRelationEndPointDefinition orderTicketEndPointDefinition =
            orderDefinition.GetRelationEndPointDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
        persistenceManager.LoadRelatedDataContainer(storageProviderManager, RelationEndPointID.Create(_order1.ID, orderTicketEndPointDefinition));
      }

      extensionMock.Verify();
    }

    [Test]
    public void GetRelatedObject ()
    {
      OrderTicket orderTicket;

      using (_transaction.EnterNonDiscardingScope())
      {
        orderTicket = _order1.OrderTicket;
      }

      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), ValueAccess.Current))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationRead(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), orderTicket, ValueAccess.Current))
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.OrderTicket;
      }

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      OrderTicket originalOrderTicket;
      using (_transaction.EnterNonDiscardingScope())
      {
        originalOrderTicket =
            (OrderTicket)_order1.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      }
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), ValueAccess.Original))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationRead(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), originalOrderTicket, ValueAccess.Original))
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      }

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetRelatedObjects ()
    {
      ObjectList<OrderItem> orderItems;
      using (_transaction.EnterNonDiscardingScope())
      {
        orderItems = _order1.OrderItems;
        orderItems.EnsureDataComplete();
      }
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationRead(
                  _transaction,
                  _order1,
                  GetEndPointDefinition(typeof(Order), "OrderItems"),
                  It.Is<IReadOnlyCollectionData<DomainObject>>(_ => _.SetEquals(orderItems)),
                  ValueAccess.Current))
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.OrderItems;
      }

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjects_WithDomainObjectCollection ()
    {
      ObjectList<OrderItem> originalOrderItems;
      using (_transaction.EnterNonDiscardingScope())
      {
        originalOrderItems = (ObjectList<OrderItem>)_order1.GetOriginalRelatedObjectsAsDomainObjectCollection(
            "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      }
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Original))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationRead(
                  _transaction,
                  _order1,
                  GetEndPointDefinition(typeof(Order), "OrderItems"),
                  It.Is<IReadOnlyCollectionData<DomainObject>>(_ => _.SetEquals(originalOrderItems)),
                  ValueAccess.Original))
          .Verifiable();
      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      }

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjects_WithVirtualCollection ()
    {
      IReadOnlyList<DomainObject> originalProductReviews;
      using (_transaction.EnterNonDiscardingScope())
      {
        originalProductReviews = _product1.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews");
      }
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_transaction, _product1, GetEndPointDefinition(typeof(Product), "Reviews"), ValueAccess.Original))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationRead(
                  _transaction,
                  _product1,
                  GetEndPointDefinition(typeof(Product), "Reviews"),
                  It.Is<IReadOnlyCollectionData<DomainObject>>(_ => _.SetEquals(originalProductReviews)),
                  ValueAccess.Original))
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _product1.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews");
      }

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetRelatedObjectWithLazyLoad ()
    {
      var sequence = new VerifiableSequence();
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RelationReading(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_transaction, It.Is<ReadOnlyCollection<ObjectID>>(list => list.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_transaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationRead(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), It.IsNotNull<DomainObject>(), ValueAccess.Current))
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.OrderTicket;
      }
      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad ()
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.RelationRead(
                  _transaction,
                  _order1,
                  GetEndPointDefinition(typeof(Order), "OrderItems"),
                  It.IsNotNull<IReadOnlyCollectionData<DomainObject>>(),
                  ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_transaction, It.Is<ReadOnlyCollection<ObjectID>>(_ => _.Count == 2)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_transaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Count == 2)))
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        _order1.OrderItems.EnsureDataComplete();
      }

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), ValueAccess.Original))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_transaction, It.Is<ReadOnlyCollection<ObjectID>>(list => list.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_transaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Count == 1)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationRead(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), It.IsNotNull<DomainObject>(), ValueAccess.Original))
          .Verifiable();
      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      }

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad ()
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Original))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_transaction, It.Is<ReadOnlyCollection<ObjectID>>(_ => _.Count == 2)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_transaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Count == 2)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.RelationRead(
                  _transaction,
                  _order1,
                  GetEndPointDefinition(typeof(Order), "OrderItems"),
                  It.IsNotNull<IReadOnlyCollectionData<DomainObject>>(),
                  ValueAccess.Original))
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      }
      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void FilterQueryResult ()
    {
      IQuery query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderQuery"));
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer1);

      using (_transaction.EnterNonDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(query);
      }

      var extensionMock = AddExtensionToClientTransaction(_transaction);

      QueryResult<DomainObject> newQueryResult = TestQueryFactory.CreateTestQueryResult(StorageSettings);
      extensionMock
          .Setup(mock => mock.FilterQueryResult(_transaction, It.Is<QueryResult<DomainObject>>(qr => qr.Count == 2 && qr.Query == query)))
          .Returns(newQueryResult)
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        QueryResult<DomainObject> finalResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(query);
        Assert.That(finalResult, NUnit.Framework.Is.SameAs(newQueryResult));
      }

      extensionMock.Verify();
    }

    [Test]
    public void FilterQueryResultWithLoad ()
    {
      IQuery query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderQuery"));
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer4);

      QueryResult<DomainObject> newQueryResult = TestQueryFactory.CreateTestQueryResult(StorageSettings);

      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_transaction, It.Is<ReadOnlyCollection<ObjectID>>(_ => _.Count == 2)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_transaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Count == 2)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.FilterQueryResult(_transaction, It.Is<QueryResult<DomainObject>>(qr => qr.Count == 2 && qr.Query == query)))
          .Returns(newQueryResult)
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        QueryResult<DomainObject> finalQueryResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(query);
        Assert.That(finalQueryResult, NUnit.Framework.Is.SameAs(newQueryResult));
      }
      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void FilterQueryResultWithFiltering ()
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);

      IQuery query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderQuery"));
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer4);

      var filteringExtension = new Mock<ClientTransactionExtensionWithQueryFiltering>(MockBehavior.Strict);
      _transaction.Extensions.Add(filteringExtension.Object);

      var lastExtension = new Mock<IClientTransactionExtension>(MockBehavior.Strict);
      lastExtension.Setup(stub => stub.Key).Returns("LastExtension");
      _transaction.Extensions.Add(lastExtension.Object);
      lastExtension.Reset();

      QueryResult<DomainObject> newQueryResult1 = TestQueryFactory.CreateTestQueryResult(StorageSettings, query, new[] { _order1 });
      QueryResult<DomainObject> newQueryResult2 = TestQueryFactory.CreateTestQueryResult(StorageSettings, query);

      var sequence = new VerifiableSequence();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_transaction, It.Is<ReadOnlyCollection<ObjectID>>(_ => _.Count == 2)))
          .Verifiable();
      filteringExtension
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_transaction, It.Is<ReadOnlyCollection<ObjectID>>(_ => _.Count == 2)))
          .Verifiable();
      lastExtension
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_transaction, It.Is<ReadOnlyCollection<ObjectID>>(_ => _.Count == 2)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_transaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Count == 2)))
          .Verifiable();
      filteringExtension
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_transaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Count == 2)))
          .Verifiable();
      lastExtension
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_transaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Count == 2)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.FilterQueryResult(_transaction, It.Is<QueryResult<DomainObject>>(qr => qr.Count == 2 && qr.Query == query)))
          .Returns(newQueryResult1)
          .Verifiable();

      filteringExtension
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.FilterQueryResult(_transaction, newQueryResult1))
          .CallBase()
          .Verifiable();

      lastExtension
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.FilterQueryResult(_transaction, It.Is<QueryResult<DomainObject>>(qr => qr.Count == 0 && qr.Query == query)))
          .Returns(newQueryResult2)
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        QueryResult<DomainObject> finalQueryResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(query);
        Assert.That(finalQueryResult, NUnit.Framework.Is.SameAs(newQueryResult2));
      }

      extensionMock.Verify();
      filteringExtension.Verify();
      lastExtension.Verify();
      sequence.Verify();
    }

    [Test]
    public void CommitWithChangedPropertyValue ()
    {
      Computer computer;
      using (_transaction.EnterNonDiscardingScope())
      {
        computer = DomainObjectIDs.Computer4.GetObject<Computer>();
        computer.SerialNumber = "newSerialNumber";
      }

      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Committing(_transaction, new[] { computer }, It.IsNotNull<CommittingEventRegistrar>()))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CommitValidate(_transaction, It.Is<ReadOnlyCollection<PersistableData>>(c => c.Select(d => d.DomainObject).SetEquals(new[] { computer }))))
          .Verifiable();
      extensionMock.Setup(mock => mock.Committed(_transaction, new[] { computer })).Verifiable();

      _transaction.Commit();

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void CommitWithChangedRelationValue ()
    {
      Computer computer;
      Employee employee;

      using (_transaction.EnterNonDiscardingScope())
      {
        computer = DomainObjectIDs.Computer4.GetObject<Computer>();
        employee = DomainObjectIDs.Employee1.GetObject<Employee>();
        computer.Employee = employee;
      }

      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.Committing(
                  _transaction,
                  It.Is<ReadOnlyCollection<DomainObject>>(p => p.SetEquals(new DomainObject[] { computer, employee })),
                  It.IsNotNull<CommittingEventRegistrar>()))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.CommitValidate(
                  _transaction,
                  It.Is<ReadOnlyCollection<PersistableData>>(c => c.Select(d => d.DomainObject).SetEquals(new DomainObject[] { computer, employee }))))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Committed(_transaction, It.Is<ReadOnlyCollection<DomainObject>>(p => p.SetEquals(new DomainObject[] { computer, employee }))))
          .Verifiable();

      _transaction.Commit();

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void CommitWithChangedRelationValueWithClassIDColumn ()
    {
      Customer oldCustomer;
      Customer newCustomer;
      using (_transaction.EnterNonDiscardingScope())
      {
        oldCustomer = _order1.Customer;
        newCustomer = DomainObjectIDs.Customer2.GetObject<Customer>();
        _order1.Customer = newCustomer;
      }

      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.Committing(
                  _transaction,
                  It.Is<ReadOnlyCollection<DomainObject>>(p => p.SetEquals(new DomainObject[] { _order1, newCustomer, oldCustomer })),
                  It.IsNotNull<CommittingEventRegistrar>()))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.CommitValidate(
                  _transaction,
                  It.Is<ReadOnlyCollection<PersistableData>>(c => c.Select(d => d.DomainObject).SetEquals(new DomainObject[] { _order1, newCustomer, oldCustomer }))))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Committed(_transaction, It.Is<ReadOnlyCollection<DomainObject>>(p => p.SetEquals(new DomainObject[] { _order1, newCustomer, oldCustomer }))))
          .Verifiable();

      _transaction.Commit();

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void CommitWithEvents ()
    {
      Computer computer;
      using (_transaction.EnterNonDiscardingScope())
      {
        computer = DomainObjectIDs.Computer4.GetObject<Computer>();
        computer.SerialNumber = "newSerialNumber";
      }

      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var clientTransactionMockEventReceiver =
          ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Strict, _transaction);
      var computerEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, computer);

      var sequence = new VerifiableSequence();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Committing(_transaction, new[] { computer }, It.IsNotNull<CommittingEventRegistrar>()))
          .Verifiable();
      clientTransactionMockEventReceiver.InVerifiableSequence(sequence).SetupCommitting(_transaction ,computer).Verifiable();
      computerEventReceiver.InVerifiableSequence(sequence).Setup(mock => mock.Committing(computer, It.IsAny<DomainObjectCommittingEventArgs>())).Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.CommitValidate(
                  _transaction,
                  It.Is<ReadOnlyCollection<PersistableData>>(c => c.Select(d => d.DomainObject).SetEquals(new DomainObject[] { computer }))))
          .Verifiable();
      computerEventReceiver.InVerifiableSequence(sequence).Setup(mock => mock.Committed(computer, It.IsAny<EventArgs>())).Verifiable();
      clientTransactionMockEventReceiver.InVerifiableSequence(sequence).SetupCommitted(_transaction, computer).Verifiable();
      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.Committed(_transaction, new[] { computer })).Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        _transaction.Commit();
      }

      extensionMock.Verify();
      clientTransactionMockEventReceiver.Verify();
      computerEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void Rollback ()
    {
      Computer computer;
      using (_transaction.EnterNonDiscardingScope())
      {
        computer = DomainObjectIDs.Computer4.GetObject<Computer>();
        computer.SerialNumber = "newSerialNumber";
      }

      var extensionMock = AddExtensionToClientTransaction(_transaction);

      var sequence = new VerifiableSequence();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RollingBack(_transaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Contains(computer))))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RolledBack(_transaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Contains(computer))))
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.Rollback();
      }

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void SubTransactions ()
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);
      ClientTransaction initializedTransaction = null;

      var subExtenstionMock = new Mock<IClientTransactionExtension>(MockBehavior.Strict);

      var sequence = new VerifiableSequence();
      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.SubTransactionCreating(_transaction)).Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.SubTransactionInitialize(_transaction, It.IsAny<ClientTransaction>()))
          .Callback(
              (ClientTransaction _, ClientTransaction subTransaction) =>
              {
                initializedTransaction = subTransaction;
                initializedTransaction.Extensions.Add(subExtenstionMock.Object);
              })
          .Verifiable();
      subExtenstionMock.InVerifiableSequence(sequence).Setup(stub => stub.Key).Returns("inner").Verifiable();
      subExtenstionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.TransactionInitialize(It.Is<ClientTransaction>(tx => tx == initializedTransaction)))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.SubTransactionCreated(_transaction, It.Is<ClientTransaction>(tx => tx == initializedTransaction)))
          .Verifiable();
      subExtenstionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.TransactionDiscard(It.Is<ClientTransaction>(tx => tx == initializedTransaction)))
          .Verifiable();

      var subTransaction = _transaction.CreateSubTransaction();
      subTransaction.Discard();

      extensionMock.Verify();
      subExtenstionMock.Verify();
      sequence.Verify();
      Assert.That(subTransaction, Is.SameAs(initializedTransaction));
    }

    [Test]
    public void GetObjects ()
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);
      var sequence = new VerifiableSequence();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_transaction, new[] { DomainObjectIDs.Order3, DomainObjectIDs.Order4 }))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_transaction, It.Is<IReadOnlyList<DomainObject>>(_ => _.Count == 2)))
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        LifetimeService.GetObjects<DomainObject>(_transaction, DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4);
      }

      extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void UnloadData ()
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);
      var sequence = new VerifiableSequence();
      extensionMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.ObjectsUnloading(_transaction, new[] { _order1 }))
            .Callback((ClientTransaction _, IReadOnlyList<DomainObject> _) => Assert.That(_transaction.DataManager.DataContainers[_order1.ID] != null))
            .Verifiable();
      extensionMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.ObjectsUnloaded(_transaction, new[] { _order1 }))
            .Callback((ClientTransaction _, IReadOnlyList<DomainObject> _) => Assert.That(_transaction.DataManager.DataContainers[_order1.ID] == null))
            .Verifiable();

      UnloadService.UnloadData(_transaction, _order1.ID);

      extensionMock.Verify();
      sequence.Verify();
    }

    private void TestObjectLoadingWithRelatedObjectCollection (
        Action accessCode,
        ObjectID expectedMainObjectID,
        bool expectLoadEventsForRelatedObjects,
        ObjectID[] expectedRelatedIDs)
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);
      var sequence = new VerifiableSequence();

      // loading of main object
      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectsLoading(_transaction, new[] { expectedMainObjectID })).Verifiable();
      extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectsLoaded(_transaction, It.IsNotNull<IReadOnlyList<DomainObject>>())).Verifiable();

      // accessing relation property

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.RelationReading(
                  _transaction,
                  It.IsNotNull<DomainObject>(),
                  It.IsNotNull<IRelationEndPointDefinition>(),
                  ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.RelationRead(
                  _transaction,
                  It.IsNotNull<DomainObject>(),
                  It.IsNotNull<IRelationEndPointDefinition>(),
                  It.IsNotNull<IReadOnlyCollectionData<DomainObject>>(),
                  ValueAccess.Current))
          .Verifiable();

      if (expectLoadEventsForRelatedObjects)
      {
        extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectsLoading(_transaction, expectedRelatedIDs)).Verifiable();
        extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectsLoaded(_transaction, It.IsNotNull<IReadOnlyList<DomainObject>>())).Verifiable();
      }

      // loading of main object a second time

      // accessing relation property a second time

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.RelationReading(
                  _transaction,
                  It.IsNotNull<DomainObject>(),
                  It.IsNotNull<IRelationEndPointDefinition>(),
                  ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.RelationRead(
                  _transaction,
                  It.IsNotNull<DomainObject>(),
                  It.IsNotNull<IRelationEndPointDefinition>(),
                  It.IsNotNull<IReadOnlyCollectionData<DomainObject>>(),
                  ValueAccess.Current))
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        accessCode();
        accessCode();
      }

      extensionMock.Verify();
      sequence.Verify();
    }

    private void TestObjectLoadingWithRelatedObject (
        Action accessCode,
        ObjectID expectedMainObjectID,
        bool expectLoadEventsForRelatedObject,
        ObjectID expectedRelatedID)
    {
      var extensionMock = AddExtensionToClientTransaction(_transaction);
      var sequence = new VerifiableSequence();

      // loading of main object
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(_transaction, new[] { expectedMainObjectID }))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(_transaction, It.IsNotNull<IReadOnlyList<DomainObject>>()))
          .Verifiable();

      // accessing relation property

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.RelationReading(
                  _transaction,
                  It.IsNotNull<DomainObject>(),
                  It.IsNotNull<IRelationEndPointDefinition>(),
                  ValueAccess.Current))
          .Verifiable();

      if (expectLoadEventsForRelatedObject)
      {
        extensionMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.ObjectsLoading(_transaction, new[] { expectedRelatedID }))
            .Verifiable();
        extensionMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.ObjectsLoaded(_transaction, It.IsNotNull<IReadOnlyList<DomainObject>>()))
            .Verifiable();
      }

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.RelationRead(
                  _transaction,
                  It.IsNotNull<DomainObject>(),
                  It.IsNotNull<IRelationEndPointDefinition>(),
                  It.IsAny<DomainObject>(),
                  ValueAccess.Current))
          .Verifiable();

      // loading of main object a second time

      // accessing relation property a second time

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.RelationReading(
                  _transaction,
                  It.IsNotNull<DomainObject>(),
                  It.IsNotNull<IRelationEndPointDefinition>(),
                  ValueAccess.Current))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.RelationRead(
                  _transaction,
                  It.IsNotNull<DomainObject>(),
                  It.IsNotNull<IRelationEndPointDefinition>(),
                  It.IsAny<DomainObject>(),
                  ValueAccess.Current))
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        accessCode();
        accessCode();
      }

      extensionMock.Verify();
      sequence.Verify();
    }

    private static Mock<IClientTransactionExtension> AddExtensionToClientTransaction (TestableClientTransaction transaction)
    {
      var extensionMock = new Mock<IClientTransactionExtension>(MockBehavior.Strict);
      extensionMock.Setup(stub => stub.Key).Returns("TestExtension");
      transaction.Extensions.Add(extensionMock.Object);
      extensionMock.Reset();

      return extensionMock;
    }
  }
}
