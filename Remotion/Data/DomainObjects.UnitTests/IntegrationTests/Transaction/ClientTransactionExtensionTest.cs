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
using CommonServiceLocator;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.RhinoMocks.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.FunctionalProgramming;
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
    private Mock<IClientTransactionExtension> _extensionMock;

    public override void OneTimeSetUp ()
    {
      base.OneTimeSetUp();
      SetDatabaseModifyable();
    }

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = new TestableClientTransaction();

      _order1 = DomainObjectIDs.Order1.GetObject<Order>(_transaction);
      _computerWithoutRelatedObjects = DomainObjectIDs.Computer5.GetObject<Computer>(_transaction);
      _product1 = DomainObjectIDs.Product1.GetObject<Product>(_transaction);

      _extensionMock = new Mock<IClientTransactionExtension> (MockBehavior.Strict);

      _extensionMock.Setup (stub => stub.Key).Returns ("TestExtension");
      _transaction.Extensions.Add(_extensionMock.Object);
      _extensionMock.BackToRecord();
    }

    public override void TearDown ()
    {
      _transaction.Extensions.Remove("TestExtension");
      base.TearDown();
    }

    [Test]
    public void Extensions ()
    {
      Assert.That(_transaction.Extensions, Has.Member(_extensionMock.Object));
    }

    [Test]
    public void TransactionInitialize ()
    {
      var factoryStub = new Mock<IClientTransactionExtensionFactory>();
      factoryStub.Setup (stub => stub.CreateClientTransactionExtensions (It.IsAny<ClientTransaction>())).Returns (new[] { _extensionMock.Object });
      var locatorStub = new Mock<IServiceLocator>();
      locatorStub.Setup (stub => stub.GetInstance<IClientTransactionExtensionFactory>()).Returns (factoryStub.Object);

      using (new ServiceLocatorScope(locatorStub.Object))
      {
        ClientTransaction inititalizedTransaction = null;

        _extensionMock.Setup (stub => stub.Key).Returns ("test");
        _extensionMock
            .Setup(mock => mock.TransactionInitialize(It.IsAny<ClientTransaction>()))
            .Callback((ClientTransaction clientTransaction) => inititalizedTransaction = clientTransaction)
            .Verifiable();

        var result = ClientTransaction.CreateRootTransaction();

        _extensionMock.Verify();

        Assert.That(result, Is.SameAs(inititalizedTransaction));
      }
    }

    [Test]
    public void TransactionDiscard ()
    {
      _extensionMock.Setup (mock => mock.TransactionDiscard (_transaction)).Verifiable();

      _transaction.Discard();

      _extensionMock.Verify();
    }

    [Test]
    public void NewObjectCreation ()
    {
      _extensionMock.Setup (mock => mock.NewObjectCreating (_transaction, typeof(Order))).Verifiable();

      _transaction.ExecuteInScope(() => Order.NewObject());

      _extensionMock.Verify();
    }

    [Test]
    public void ObjectLoading ()
    {
      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _transaction,
            new[] { DomainObjectIDs.Order3 })).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoaded (
            _transaction,
            It.Is<ReadOnlyCollection<DomainObject>> (loadedObjects => loadedObjects.Count == 1 && loadedObjects[0].ID == DomainObjectIDs.Order3))).Verifiable();

      Dev.Null = DomainObjectIDs.Order3.GetObject<Order>(_transaction);
      Dev.Null = DomainObjectIDs.Order3.GetObject<Order>(_transaction);

      _extensionMock.Verify();
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
      _extensionMock.Setup (mock => mock.ObjectsLoading (
            _transaction,
            new[] { DomainObjectIDs.ClassWithAllDataTypes1 })).Verifiable();
      _extensionMock.Setup (mock => mock.ObjectsLoaded (
            _transaction,
            It.Is<ReadOnlyCollection<DomainObject>> (collection => collection.Count == 1))).Verifiable();

      DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>(_transaction);

      _extensionMock.Verify();
    }

    [Test]
    public void ObjectsLoadedWithRelations ()
    {
      _extensionMock.Setup (mock => mock.ObjectsLoading (
            _transaction,
            new[] { DomainObjectIDs.Order3 })).Verifiable();
      _extensionMock.Setup (mock => mock.ObjectsLoaded (
            _transaction,
            It.Is<ReadOnlyCollection<DomainObject>> (collection => collection.Count == 1))).Verifiable();

      DomainObjectIDs.Order3.GetObject<Order>(_transaction);

      _extensionMock.Verify();
    }

    [Test]
    public void ObjectsLoadedWithEvents ()
    {
      var clientTransactionEventReceiver =
          new Mock<ClientTransactionMockEventReceiver> (MockBehavior.Strict, _transaction);

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (
            mock => mock.ObjectsLoading (
                        _transaction,
                        new[] { DomainObjectIDs.ClassWithAllDataTypes1 })).Verifiable();
      clientTransactionEventReceiver.InSequence (sequence).Setup (
            mock => mock.Loaded (_transaction, It.Is<ClientTransactionEventArgs> (args => args.DomainObjects.Count == 1))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (
            mock => mock.ObjectsLoaded (
                _transaction,
                It.Is<ReadOnlyCollection<DomainObject>> (collection => collection.Count == 1))).Verifiable();

      DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>(_transaction);

      _extensionMock.Verify();
      clientTransactionEventReceiver.Verify();
    }

    [Test]
    public void ObjectDelete ()
    {
      var eventReceiverMock = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, _computerWithoutRelatedObjects);
      _extensionMock.BackToRecord();

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectDeleting (_transaction, _computerWithoutRelatedObjects)).Verifiable();
      eventReceiverMock.InSequence (sequence).Setup (mock => mock.Deleting (_computerWithoutRelatedObjects, EventArgs.Empty)).Verifiable();
      eventReceiverMock.InSequence (sequence).Setup (mock => mock.Deleted (_computerWithoutRelatedObjects, EventArgs.Empty)).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectDeleted (_transaction, _computerWithoutRelatedObjects)).Verifiable();

      _transaction.ExecuteInScope(() => _computerWithoutRelatedObjects.Delete());

      _extensionMock.Verify();
      eventReceiverMock.Verify();
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

      var order1MockEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, _order1);
      var orderItem1MockEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, orderItem1);
      var orderItem2MockEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, orderItem2);
      var orderTicketMockEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, orderTicket);
      var officialMockEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, official);
      var customerMockEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, customer);

      var customerOrdersMockEventReceiver =
          new Mock<DomainObjectCollectionMockEventReceiver> (MockBehavior.Strict, customerOrders);

      var officialOrdersMockEventReceiver =
          new Mock<DomainObjectCollectionMockEventReceiver> (MockBehavior.Strict, officialOrders);

      _mockRepository.BackToRecord(_extensionMock.Object);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Object.ObjectDeleting(_transaction, _order1);
        order1MockEventReceiver.Object.Deleting(_order1, EventArgs.Empty);

        using (_mockRepository.Unordered())
        {
          customerOrdersMockEventReceiver.Object.Removing(customerOrders, _order1);
          _extensionMock.Object.RelationChanging(_transaction, customer, GetEndPointDefinition(typeof(Customer), "Orders"), _order1, null);
          customerMockEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Customer), "Orders"), _order1, null);

          _extensionMock.Object.RelationChanging(_transaction, orderTicket, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);
          orderTicketMockEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);

          _extensionMock.Object.RelationChanging(_transaction, orderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);
          orderItem1MockEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

          _extensionMock.Object.RelationChanging(_transaction, orderItem2, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);
          orderItem2MockEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

          officialOrdersMockEventReceiver.Object.Removing(officialOrders, _order1);
          _extensionMock.Object.RelationChanging(_transaction, official, GetEndPointDefinition(typeof(Official), "Orders"), _order1, null);
          officialMockEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Official), "Orders"), _order1, null);
        }

        using (_mockRepository.Unordered())
        {
          customerMockEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Customer), "Orders"), _order1, null);
          _extensionMock.Object.RelationChanged(_transaction, customer, GetEndPointDefinition(typeof(Customer), "Orders"), _order1, null);
          customerOrdersMockEventReceiver.Object.Removed(customerOrders, _order1);

          orderTicketMockEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);
          _extensionMock.Object.RelationChanged(_transaction, orderTicket, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);

          orderItem1MockEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);
          _extensionMock.Object.RelationChanged(_transaction, orderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

          orderItem2MockEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);
          _extensionMock.Object.RelationChanged(_transaction, orderItem2, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

          officialOrdersMockEventReceiver.Object.Removed(officialOrders, _order1);
          officialMockEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Official), "Orders"), _order1, null);
          _extensionMock.Object.RelationChanged(_transaction, official, GetEndPointDefinition(typeof(Official), "Orders"), _order1, null);
        }

        order1MockEventReceiver.Object.Deleted(_order1, EventArgs.Empty);
        _extensionMock.Object.ObjectDeleted(_transaction, _order1);
      }

      _transaction.ExecuteInScope(() => _order1.Delete());

      _extensionMock.Verify();
      order1MockEventReceiver.Verify();
      orderItem1MockEventReceiver.Verify();
      orderItem2MockEventReceiver.Verify();
      orderTicketMockEventReceiver.Verify();
      officialMockEventReceiver.Verify();
      customerMockEventReceiver.Verify();
      customerOrdersMockEventReceiver.Verify();
      officialOrdersMockEventReceiver.Verify();
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

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.RelationChanging(_transaction, location, GetEndPointDefinition(typeof(Location), "Client"), deletedClient, newClient);

      _extensionMock.Object.RelationChanged(_transaction, location, GetEndPointDefinition(typeof(Location), "Client"), deletedClient, newClient);

      _transaction.ExecuteInScope(() => location.Client = newClient);

      _extensionMock.Verify();
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

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.RelationChanging(_transaction, location, GetEndPointDefinition(typeof(Location), "Client"), deletedClient, newClient);

      _extensionMock.Object.RelationChanged(_transaction, location, GetEndPointDefinition(typeof(Location), "Client"), deletedClient, newClient);

      _transaction.ExecuteInScope(() => location.Client = newClient);

      _extensionMock.Verify();
    }

    [Test]
    public void ObjectDeleteTwice ()
    {
      var computer = DomainObjectIDs.Computer4.GetObject<Computer>(_transaction);
      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.ObjectDeleting(_transaction, computer);

      _extensionMock.Object.ObjectDeleted(_transaction, computer);

      _transaction.ExecuteInScope(computer.Delete);
      _transaction.ExecuteInScope(computer.Delete);

      _extensionMock.Verify();
    }

    [Test]
    public void PropertyRead ()
    {
      int orderNumber = _transaction.ExecuteInScope(() => _order1.OrderNumber);
      _mockRepository.BackToRecord(_extensionMock.Object);

      var propertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");
      var sequence = new MockSequence();
      _extensionMock.Object.PropertyValueReading(_transaction, _order1, propertyDefinition, ValueAccess.Current);
      _extensionMock.Object.PropertyValueRead(_transaction, _order1, propertyDefinition, orderNumber, ValueAccess.Current);
      _extensionMock.Object.PropertyValueReading(_transaction, _order1, propertyDefinition, ValueAccess.Original);
      _extensionMock.Object.PropertyValueRead(_transaction, _order1, propertyDefinition, orderNumber, ValueAccess.Original);

      _transaction.ExecuteInScope(() => Dev.Null = _order1.OrderNumber);
      _transaction.ExecuteInScope(() => Dev.Null = _order1.Properties[propertyDefinition.PropertyName].GetOriginalValueWithoutTypeCheck());

      _extensionMock.Verify();
    }

    [Test]
    public void ReadObjectIDProperty ()
    {
      var customerPropertyDefinition = GetPropertyDefinition(typeof(Order), "Customer");
      var customerID = _order1.Properties[customerPropertyDefinition.PropertyName, _transaction].GetRelatedObjectID();

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.PropertyValueReading(_transaction, _order1, customerPropertyDefinition, ValueAccess.Current);

      _extensionMock.Object.PropertyValueRead(_transaction, _order1, customerPropertyDefinition, customerID, ValueAccess.Current);

      _transaction.ExecuteInScope(() => Dev.Null = _order1.Properties[customerPropertyDefinition.PropertyName, _transaction].GetRelatedObjectID());

      _extensionMock.Verify();
    }

    [Test]
    public void PropertySetToSameValue ()
    {
      int orderNumber = _transaction.ExecuteInScope(() => _order1.OrderNumber);

      _mockRepository.BackToRecord(_extensionMock.Object);

      _transaction.ExecuteInScope(() => _order1.OrderNumber = orderNumber);

      _extensionMock.Verify();
    }

    [Test]
    public void ChangeAndReadProperty ()
    {
      int oldOrderNumber = _transaction.ExecuteInScope(() => _order1.OrderNumber);
      int newOrderNumber = oldOrderNumber + 1;

      var orderNumberPropertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.PropertyValueChanging(
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            oldOrderNumber,
            newOrderNumber);

      _extensionMock.Object.PropertyValueChanged(
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            oldOrderNumber,
            newOrderNumber);

      _extensionMock.Object.PropertyValueReading(
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            ValueAccess.Current);

      _extensionMock.Object.PropertyValueRead(
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            newOrderNumber,
            ValueAccess.Current);

      _extensionMock.Object.PropertyValueReading(
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            ValueAccess.Original);

      _extensionMock.Object.PropertyValueRead(
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            oldOrderNumber,
            ValueAccess.Original);

      using (_transaction.EnterNonDiscardingScope())
      {
        _order1.OrderNumber = newOrderNumber;
        Dev.Null = _order1.OrderNumber;
        Dev.Null = _order1.Properties[typeof(Order), "OrderNumber"].GetOriginalValueWithoutTypeCheck();
      }

      _extensionMock.Verify();
    }

    [Test]
    public void PropertyChange ()
    {
      int oldOrderNumber = _transaction.ExecuteInScope(() => _order1.OrderNumber);
      _mockRepository.BackToRecord(_extensionMock.Object);

      var orderNumberPropertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

      var sequence = new MockSequence();

      _extensionMock.Object.PropertyValueChanging(
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            oldOrderNumber,
            oldOrderNumber + 1);

      _extensionMock.Object.PropertyValueChanged(
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            oldOrderNumber,
            oldOrderNumber + 1);

      using (_transaction.EnterNonDiscardingScope())
      {
        _order1.OrderNumber = oldOrderNumber + 1;
      }

      _extensionMock.Verify();
    }

    [Test]
    public void PropertyChangeWithEvents ()
    {
      int oldOrderNumber = _transaction.ExecuteInScope(() => _order1.OrderNumber);
      _mockRepository.BackToRecord(_extensionMock.Object);

      var domainObjectMockEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, _order1);
      var orderNumberPropertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

      var sequence = new MockSequence();

      _extensionMock.Object.PropertyValueChanging(
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            oldOrderNumber,
            oldOrderNumber + 1);
      domainObjectMockEventReceiver.InSequence (sequence).Setup (_ => _.PropertyChanging (It.IsAny<object>(), It.IsAny<PropertyChangeEventArgs>())).Verifiable();
      domainObjectMockEventReceiver.InSequence (sequence).Setup (_ => _.PropertyChanged (It.IsAny<object>(), It.IsAny<PropertyChangeEventArgs>())).Verifiable();

      _extensionMock.Object.PropertyValueChanged(
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            oldOrderNumber,
            oldOrderNumber + 1);

      using (_transaction.EnterNonDiscardingScope())
      {
        _order1.OrderNumber = oldOrderNumber + 1;
      }

      _extensionMock.Verify();
      domainObjectMockEventReceiver.Verify();
    }

    [Test]
    public void LoadRelatedDataContainerForVirtualEndPoint ()
    {
      using (var persistenceManager = new PersistenceManager(NullPersistenceExtension.Instance))
      {
        ClassDefinition orderDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Order));
        IRelationEndPointDefinition orderTicketEndPointDefinition =
            orderDefinition.GetRelationEndPointDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
        persistenceManager.LoadRelatedDataContainer(RelationEndPointID.Create(_order1.ID, orderTicketEndPointDefinition));
      }

      _extensionMock.Verify();
    }

    [Test]
    public void GetRelatedObject ()
    {
      OrderTicket orderTicket;

      using (_transaction.EnterNonDiscardingScope())
      {
        orderTicket = _order1.OrderTicket;
      }

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.RelationReading(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), ValueAccess.Current);

      _extensionMock.Object.RelationRead(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), orderTicket, ValueAccess.Current);

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.OrderTicket;
      }

      _extensionMock.Verify();
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
      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.RelationReading(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), ValueAccess.Original);

      _extensionMock.Object.RelationRead(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), originalOrderTicket, ValueAccess.Original);

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      }

      _extensionMock.Verify();
    }

    [Test]
    public void GetRelatedObjects ()
    {
      DomainObjectCollection orderItems;
      using (_transaction.EnterNonDiscardingScope())
      {
        orderItems = _order1.OrderItems;
        orderItems.EnsureDataComplete();
      }
      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.RelationReading(
            _transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Current);
      _extensionMock.InSequence (sequence).Setup (_ => _.RelationRead (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<DomainObject> (_ => object.ReferenceEquals (_, _order1)), It.Is<IRelationEndPointDefinition> (_ => object.Equals (_, GetEndPointDefinition (typeof(Order), "OrderItems"))), It.Is<IReadOnlyCollectionData<DomainObject>> (_ => _ != null && object.Equals (_.Count, 2) &&_.Contains (orderItems[0]) &&_.Contains (orderItems[1]) ), It.Is<ValueAccess> (_ => object.Equals (_, ValueAccess.Current)))).Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.OrderItems;
      }

      _extensionMock.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjects_WithDomainObjectCollection ()
    {
      DomainObjectCollection originalOrderItems;
      using (_transaction.EnterNonDiscardingScope())
      {
        originalOrderItems = _order1.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      }
      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.RelationReading(_transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Original);

      _extensionMock.Object.RelationRead(
            _transaction,
            _order1,
            GetEndPointDefinition(typeof(Order), "OrderItems"),
            Arg<IReadOnlyCollectionData<DomainObject>>.Matches(_ => _ != null && object.Equals (_.Count, 2) &&originalOrderItems.All (_.Contains) ),
            ValueAccess.Original);

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      }

      _extensionMock.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjects_WithVirtualCollection ()
    {
      IReadOnlyList<DomainObject> originalProductReviews;
      using (_transaction.EnterNonDiscardingScope())
      {
        originalProductReviews = _product1.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews");
      }
      _mockRepository.BackToRecord(_extensionMock.Object);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Object.RelationReading(_transaction, _product1, GetEndPointDefinition(typeof(Product), "Reviews"), ValueAccess.Original);
        _extensionMock.Object.RelationRead(
            _transaction,
            _product1,
            GetEndPointDefinition(typeof(Product), "Reviews"),
            It.Is<IReadOnlyCollectionData<DomainObject>> (_ => _ != null && object.Equals (_.Count, 3) &&originalProductReviews.All (_.Contains) ),
            ValueAccess.Original);
      }

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _product1.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews");
      }

      _extensionMock.Verify();
    }

    [Test]
    public void GetRelatedObjectWithLazyLoad ()
    {
      var sequence = new MockSequence();
      _extensionMock.Object.RelationReading(
            _transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), ValueAccess.Current);
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _transaction,
            It.Is<ReadOnlyCollection<ObjectID>> (list => list.Count == 1))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 1)))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.RelationRead (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<DomainObject> (_ => object.ReferenceEquals (_, _order1)), It.Is<IRelationEndPointDefinition> (_ => object.Equals (_, GetEndPointDefinition (typeof(Order), "OrderTicket"))), It.Is<DomainObject> (_ => _ != null), It.Is<ValueAccess> (_ => object.Equals (_, ValueAccess.Current)))).Verifiable();
      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.OrderTicket;
      }
      _extensionMock.Verify();
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad ()
    {
      var sequence = new MockSequence();
      _extensionMock.Object.RelationReading(
            _transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Current);
      _extensionMock.InSequence (sequence).Setup (_ => _.RelationRead (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<DomainObject> (_ => object.ReferenceEquals (_, _order1)), It.Is<IRelationEndPointDefinition> (_ => object.Equals (_, GetEndPointDefinition (typeof(Order), "OrderItems"))), It.Is<IReadOnlyCollectionData<DomainObject>> (_ => _ != null), It.Is<ValueAccess> (_ => object.Equals (_, ValueAccess.Current)))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoading (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<IReadOnlyList<ObjectID>> (_ => _ != null && object.Equals (_.Count, 2)))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 2)))).Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        _order1.OrderItems.EnsureDataComplete();
      }

      _extensionMock.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      var sequence = new MockSequence();
      _extensionMock.Object.RelationReading(
            _transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), ValueAccess.Original);
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _transaction,
            It.Is<ReadOnlyCollection<ObjectID>> (list => list.Count == 1))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 1)))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.RelationRead (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<DomainObject> (_ => object.ReferenceEquals (_, _order1)), It.Is<IRelationEndPointDefinition> (_ => object.Equals (_, GetEndPointDefinition (typeof(Order), "OrderTicket"))), It.Is<DomainObject> (_ => _ != null), It.Is<ValueAccess> (_ => object.Equals (_, ValueAccess.Original)))).Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      }

      _extensionMock.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad ()
    {
      var sequence = new MockSequence();
      _extensionMock.Object.RelationReading(
            _transaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Original);
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _transaction,
            It.Is<ReadOnlyCollection<ObjectID>> (list => list.Count == 2))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 2)))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.RelationRead (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<DomainObject> (_ => object.ReferenceEquals (_, _order1)), It.Is<IRelationEndPointDefinition> (_ => object.Equals (_, GetEndPointDefinition (typeof(Order), "OrderItems"))), It.Is<IReadOnlyCollectionData<DomainObject>> (_ => _ != null), It.Is<ValueAccess> (_ => object.Equals (_, ValueAccess.Original)))).Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      }
      _extensionMock.Verify();
    }

    [Test]
    public void FilterQueryResult ()
    {
      IQuery query = QueryFactory.CreateQueryFromConfiguration("OrderQuery");
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer1);

      using (_transaction.EnterNonDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(query);
      }

      _mockRepository.BackToRecord(_extensionMock.Object);

      QueryResult<DomainObject> newQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject>();
      _extensionMock
          .Setup(
          mock => mock.FilterQueryResult(_transaction, It.Is<QueryResult<DomainObject>> (qr => qr.Count == 2 && qr.Query == query)))
          .Returns(newQueryResult)
          .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        QueryResult<DomainObject> finalResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(query);
        Assert.That(finalResult, NUnit.Framework.Is.SameAs(newQueryResult));
      }

      _extensionMock.Verify();
    }

    [Test]
    public void FilterQueryResultWithLoad ()
    {
      IQuery query = QueryFactory.CreateQueryFromConfiguration("OrderQuery");
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer4);

      QueryResult<DomainObject> newQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject>();

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _transaction,
            It.Is<ReadOnlyCollection<ObjectID>> (list => list.Count == 2))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 2)))).Verifiable();
      _extensionMock.InSequence (sequence)
            .Setup(
            mock =>
            mock.FilterQueryResult(_transaction, It.Is<QueryResult<DomainObject>> (qr => qr.Count == 2 && qr.Query == query)))
            .Returns(newQueryResult)
            .Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        QueryResult<DomainObject> finalQueryResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(query);
        Assert.That(finalQueryResult, NUnit.Framework.Is.SameAs(newQueryResult));
      }
      _extensionMock.Verify();
    }

    [Test]
    public void FilterQueryResultWithFiltering ()
    {
      IQuery query = QueryFactory.CreateQueryFromConfiguration("OrderQuery");
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer4);

      var filteringExtension = new Mock<ClientTransactionExtensionWithQueryFiltering> (MockBehavior.Strict);
      _transaction.Extensions.Add(filteringExtension.Object);

      var lastExtension = new Mock<IClientTransactionExtension> (MockBehavior.Strict);
      lastExtension.Setup (stub => stub.Key).Returns ("LastExtension");
      lastExtension.Object.Replay();
      _transaction.Extensions.Add(lastExtension.Object);
      lastExtension.BackToRecord();

      QueryResult<DomainObject> newQueryResult1 = TestQueryFactory.CreateTestQueryResult<DomainObject>(query, new[] { _order1 });
      QueryResult<DomainObject> newQueryResult2 = TestQueryFactory.CreateTestQueryResult<DomainObject>(query);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Setup (mock => mock.ObjectsLoading (
            _transaction,
            It.Is<ReadOnlyCollection<ObjectID>> (list => list.Count == 2))).Verifiable();
        filteringExtension.Setup (mock => mock.ObjectsLoading (
            _transaction,
            It.Is<ReadOnlyCollection<ObjectID>> (list => list.Count == 2))).Verifiable();
        lastExtension.Setup (mock => mock.ObjectsLoading (
            _transaction,
            It.Is<ReadOnlyCollection<ObjectID>> (list => list.Count == 2))).Verifiable();
        _extensionMock.Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 2)))).Verifiable();
        filteringExtension.Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 2)))).Verifiable();
        lastExtension.Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 2)))).Verifiable();

        _extensionMock
            .Setup(
            mock =>
            mock.FilterQueryResult(_transaction, It.Is<QueryResult<DomainObject>> (qr => qr.Count == 2 && qr.Query == query)))
            .Returns(newQueryResult1)
            .Verifiable();
        filteringExtension
            .Setup(mock => mock.FilterQueryResult(_transaction, newQueryResult1))
            .CallOriginalMethod(OriginalCallOptions.CreateExpectation)
            .Verifiable();
        lastExtension
            .Setup(
            mock =>
            mock.FilterQueryResult(_transaction, It.Is<QueryResult<DomainObject>> (qr => qr.Count == 0 && qr.Query == query)))
            .Returns(newQueryResult2)
            .Verifiable();
      }

      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        QueryResult<DomainObject> finalQueryResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(query);
        Assert.That(finalQueryResult, NUnit.Framework.Is.SameAs(newQueryResult2));
      }

      _extensionMock.Verify();
      filteringExtension.Verify();
      lastExtension.Verify();
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

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (
            mock =>
            mock.Committing (
                _transaction,
                new[] { computer },
                Arg<CommittingEventRegistrar>.Is.TypeOf)).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.CommitValidate (
            _transaction,
            It.Is<ReadOnlyCollection<PersistableData>> (c => c.Select (d => d.DomainObject).SetEquals (new[] { computer })))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.Committed (_transaction, new[] { computer })).Verifiable();

      _transaction.Commit();

      _extensionMock.Verify();
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

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (
            mock =>
            mock.Committing (
                _transaction,
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new DomainObject[] { computer, employee }),
                Arg<CommittingEventRegistrar>.Is.TypeOf)).Verifiable();
      _extensionMock.InSequence (sequence).Setup (
            mock =>
            mock.CommitValidate (
                _transaction,
                It.Is<ReadOnlyCollection<PersistableData>> (c => c.Select (d => d.DomainObject).SetEquals (new DomainObject[] { computer, employee })))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (
            mock =>
            mock.Committed (_transaction, Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new DomainObject[] { computer, employee }))).Verifiable();

      _transaction.Commit();

      _extensionMock.Verify();
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
      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (
            mock =>
            mock.Committing (
                _transaction,
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new DomainObject[] { _order1, newCustomer, oldCustomer }),
                Arg<CommittingEventRegistrar>.Is.TypeOf)).Verifiable();
      _extensionMock.InSequence (sequence).Setup (
            mock =>
            mock.CommitValidate (
                _transaction,
                It.Is<ReadOnlyCollection<PersistableData>> (c => c.Select (d => d.DomainObject).SetEquals (new DomainObject[] { _order1, newCustomer, oldCustomer })))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (
            mock =>
            mock.Committed (
                _transaction, Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new DomainObject[] { _order1, newCustomer, oldCustomer }))).Verifiable();

      _transaction.Commit();

      _extensionMock.Verify();
    }

    [Test]
    public void CommitWithEvents ()
    {
      SetDatabaseModifyable();

      Computer computer;
      using (_transaction.EnterNonDiscardingScope())
      {
        computer = DomainObjectIDs.Computer4.GetObject<Computer>();
        computer.SerialNumber = "newSerialNumber";
      }
      _mockRepository.BackToRecord(_extensionMock.Object);

      var clientTransactionMockEventReceiver =
          new Mock<ClientTransactionMockEventReceiver> (MockBehavior.Strict, _transaction);
      var computerEventReveiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, computer);

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (
            mock =>
            mock.Committing (
                _transaction,
                new[] { computer },
                Arg<CommittingEventRegistrar>.Is.TypeOf)).Verifiable();
      clientTransactionMockEventReceiver.InSequence (sequence).Setup (mock =>mock.Committing (computer)).Verifiable();
      computerEventReveiver.InSequence (sequence).Setup (mock => mock.Committing()).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.CommitValidate (
            _transaction,
            It.Is<ReadOnlyCollection<PersistableData>> (c => c.Select (d => d.DomainObject).SetEquals (new DomainObject[] { computer })))).Verifiable();
      computerEventReveiver.InSequence (sequence).Setup (mock => mock.Committed()).Verifiable();
      clientTransactionMockEventReceiver.InSequence (sequence).Setup (mock => mock.Committed (computer)).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.Committed (_transaction, new[] { computer })).Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        _transaction.Commit();
      }

      _extensionMock.Verify();
      clientTransactionMockEventReceiver.Verify();
      computerEventReveiver.Verify();
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

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (_ => _.RollingBack (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _.Contains (computer)))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.RolledBack (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _.Contains (computer)))).Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.Rollback();
      }

      _extensionMock.Verify();
    }

    [Test]
    public void SubTransactions ()
    {
      ClientTransaction initializedTransaction = null;

      var subExtenstionMock = new Mock<IClientTransactionExtension> (MockBehavior.Strict);

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (mock => mock.SubTransactionCreating (_transaction)).Verifiable();
      _extensionMock.InSequence (sequence)
            .Setup(mock => mock.SubTransactionInitialize(_transaction, It.IsAny<ClientTransaction>()))
            .Callback(
                (ClientTransaction parentClientTransaction, ClientTransaction subTransaction) =>
                {
                  initializedTransaction = (ClientTransaction)mi.Arguments[1];
                  initializedTransaction.Extensions.Add(subExtenstionMock.Object);
            })
            .Verifiable();
      subExtenstionMock.InSequence (sequence).Setup (stub => stub.Key).Returns ("inner");
      subExtenstionMock.InSequence (sequence).Setup (mock => mock.TransactionInitialize (It.Is<ClientTransaction> (tx => tx == initializedTransaction))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.SubTransactionCreated (
            _transaction,
            It.Is<ClientTransaction> (tx => tx == initializedTransaction))).Verifiable();
      subExtenstionMock.InSequence (sequence).Setup (mock => mock.TransactionDiscard (It.Is<ClientTransaction> (tx => tx == initializedTransaction))).Verifiable();

      var subTransaction = _transaction.CreateSubTransaction();
      subTransaction.Discard();

      _extensionMock.Verify();
      subExtenstionMock.Verify();
      Assert.That(subTransaction, Is.SameAs(initializedTransaction));
    }

    [Test]
    public void GetObjects ()
    {
      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _transaction,
            new[] { DomainObjectIDs.Order3, DomainObjectIDs.Order4 })).Verifiable();
      _extensionMock.Expect (_ => _.ObjectsLoaded(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)),Arg<IReadOnlyList<DomainObject>>.Matches(_ => Rhino.Mocks.Constraints.List.Count(Rhino_Is.Equal(2)))));

      using (_transaction.EnterNonDiscardingScope())
      {
        LifetimeService.GetObjects<DomainObject>(_transaction, DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4);
      }

      _extensionMock.Verify();
    }

    [Test]
    public void UnloadData ()
    {
      var sequence = new MockSequence();
      _extensionMock
            .InSequence (sequence)
            .Setup(mock => mock.ObjectsUnloading(
                        _transaction,
                        new[] { _order1 }))
            .Callback((ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects) => Assert.That(_transaction.DataManager.DataContainers[_order1.ID] != null))
            .Verifiable();
      _extensionMock
            .InSequence (sequence)
            .Setup(mock => mock.ObjectsUnloaded(
                        _transaction,
                        new[] { _order1 }))
            .Callback((ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects) => Assert.That(_transaction.DataManager.DataContainers[_order1.ID] == null))
            .Verifiable();

      UnloadService.UnloadData(_transaction, _order1.ID);

      _extensionMock.Verify();
    }

    private void TestObjectLoadingWithRelatedObjectCollection (
        Action accessCode,
        ObjectID expectedMainObjectID,
        bool expectLoadEventsForRelatedObjects,
        ObjectID[] expectedRelatedIDs)
    {
      _mockRepository.BackToRecordAll();
      var sequence = new MockSequence();
      _extensionMock.Object.ObjectsLoading((ClientTransaction)_transaction, new[] { expectedMainObjectID });
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.RelationReading (It.IsAny<ClientTransaction>(), It.IsAny<DomainObject>(), It.IsAny<IRelationEndPointDefinition>(), It.IsAny<ValueAccess>())).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.RelationRead (It.IsAny<ClientTransaction>(), It.IsAny<DomainObject>(), It.IsAny<IRelationEndPointDefinition>(), It.IsAny<IReadOnlyCollectionData<DomainObject>>(), It.IsAny<ValueAccess>())).Verifiable();
      if (expectLoadEventsForRelatedObjects)
        {
          _extensionMock.Object.ObjectsLoading((ClientTransaction)_transaction, expectedRelatedIDs);
          _extensionMock.Setup (_ => _.ObjectsLoaded (It.IsAny<ClientTransaction>(), It.IsAny<IReadOnlyList<DomainObject>>())).Verifiable();
        }
      _extensionMock.InSequence (sequence).Setup (_ => _.RelationReading (It.IsAny<ClientTransaction>(), It.IsAny<DomainObject>(), It.IsAny<IRelationEndPointDefinition>(), It.IsAny<ValueAccess>())).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.RelationRead (It.IsAny<ClientTransaction>(), It.IsAny<DomainObject>(), It.IsAny<IRelationEndPointDefinition>(), It.IsAny<IReadOnlyCollectionData<DomainObject>>(), It.IsAny<ValueAccess>())).Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        accessCode();
        accessCode();

        _extensionMock.Verify();
      }
    }

    private void TestObjectLoadingWithRelatedObject (
        Action accessCode,
        ObjectID expectedMainObjectID,
        bool expectLoadEventsForRelatedObject,
        ObjectID expectedRelatedID)
    {
      _mockRepository.BackToRecordAll();
      var sequence = new MockSequence();
      _extensionMock.Object.ObjectsLoading((ClientTransaction)_transaction, new[] { expectedMainObjectID });
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _transaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.RelationReading (It.IsAny<ClientTransaction>(), It.IsAny<DomainObject>(), It.IsAny<IRelationEndPointDefinition>(), It.IsAny<ValueAccess>())).Verifiable();
      if (expectLoadEventsForRelatedObject)
        {
          _extensionMock.Object.ObjectsLoading((ClientTransaction)_transaction, new[] { expectedRelatedID });
        }
      if (expectLoadEventsForRelatedObject)
        {
          _extensionMock.Setup (_ => _.ObjectsLoaded (It.IsAny<ClientTransaction>(), It.IsAny<IReadOnlyList<DomainObject>>())).Verifiable();
        }
      _extensionMock.InSequence (sequence).Setup (_ => _.RelationRead (It.IsAny<ClientTransaction>(), It.IsAny<DomainObject>(), It.IsAny<IRelationEndPointDefinition>(), It.IsAny<DomainObject>(), It.IsAny<ValueAccess>())).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.RelationReading (It.IsAny<ClientTransaction>(), It.IsAny<DomainObject>(), It.IsAny<IRelationEndPointDefinition>(), It.IsAny<ValueAccess>())).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.RelationRead (It.IsAny<ClientTransaction>(), It.IsAny<DomainObject>(), It.IsAny<IRelationEndPointDefinition>(), It.IsAny<DomainObject>(), It.IsAny<ValueAccess>())).Verifiable();

      using (_transaction.EnterNonDiscardingScope())
      {
        accessCode();
        accessCode();

        _extensionMock.Verify();
      }
    }
  }
}
