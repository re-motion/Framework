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
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
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
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Interfaces;
using Is = NUnit.Framework.Is;
using Rhino_Is = Rhino.Mocks.Constraints.Is;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class ClientTransactionExtensionTest : StandardMappingTest
  {
    private TestableClientTransaction _transaction;

    private Order _order1;
    private Computer _computerWithoutRelatedObjects;

    private MockRepository _mockRepository;
    private IClientTransactionExtension _extensionMock;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();
      SetDatabaseModifyable();
    }

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = new TestableClientTransaction ();

      _order1 = DomainObjectIDs.Order1.GetObject<Order> (_transaction);
      _computerWithoutRelatedObjects = DomainObjectIDs.Computer5.GetObject<Computer> (_transaction);

      _mockRepository = new MockRepository ();
      _extensionMock = _mockRepository.StrictMock<IClientTransactionExtension> ();

      _extensionMock.Stub (stub => stub.Key).Return ("TestExtension");
      _extensionMock.Replay();
      _transaction.Extensions.Add (_extensionMock);
      _extensionMock.BackToRecord();
    }

    public override void TearDown ()
    {
      _transaction.Extensions.Remove ("TestExtension");
      base.TearDown ();
    }

    [Test]
    public void Extensions ()
    {
      Assert.That (_transaction.Extensions, Has.Member(_extensionMock));
    }

    [Test]
    public void TransactionInitialize ()
    {
      var factoryStub = MockRepository.GenerateStub<IClientTransactionExtensionFactory>();
      factoryStub.Stub (stub => stub.CreateClientTransactionExtensions (Arg<ClientTransaction>.Is.Anything)).Return (new[] { _extensionMock });
      var locatorStub = MockRepository.GenerateStub<IServiceLocator>();
      locatorStub.Stub (stub => stub.GetInstance<IClientTransactionExtensionFactory> ()).Return (factoryStub);

      using (new ServiceLocatorScope (locatorStub))
      {
        ClientTransaction inititalizedTransaction = null;

        _extensionMock.Stub (stub => stub.Key).Return ("test");
        _extensionMock
            .Expect (mock => mock.TransactionInitialize (Arg<ClientTransaction>.Is.Anything))
            .WhenCalled (mi => inititalizedTransaction = (ClientTransaction) mi.Arguments[0]);
        _extensionMock.Replay();

        var result = ClientTransaction.CreateRootTransaction();

        _extensionMock.VerifyAllExpectations();

        Assert.That (result, Is.SameAs (inititalizedTransaction));
      }
    }

    [Test]
    public void TransactionDiscard ()
    {
      _extensionMock.Expect (mock => mock.TransactionDiscard (_transaction));
      _extensionMock.Replay ();

      _transaction.Discard();

      _extensionMock.VerifyAllExpectations ();
    }

    [Test]
    public void NewObjectCreation ()
    {
      _extensionMock.Expect (mock => mock.NewObjectCreating (_transaction, typeof (Order)));

      _mockRepository.ReplayAll();
      
      _transaction.ExecuteInScope (() => Order.NewObject());

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectLoading ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_transaction), 
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order3 })));
        _extensionMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (_transaction), 
            Arg<ReadOnlyCollection<DomainObject>>.Matches (loadedObjects => loadedObjects.Count == 1 && loadedObjects[0].ID == DomainObjectIDs.Order3)));
      }

      _mockRepository.ReplayAll();

      Dev.Null = DomainObjectIDs.Order3.GetObject<Order> (_transaction);
      Dev.Null = DomainObjectIDs.Order3.GetObject<Order> (_transaction);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1Side ()
    {
      TestObjectLoadingWithRelatedObjectCollection (
          delegate
          {
            Order order = DomainObjectIDs.Order3.GetObject<Order> ();
            int orderItemCount = order.OrderItems.Count;
            Assert.That (orderItemCount, Is.EqualTo (1));
          },
          DomainObjectIDs.Order3,
          true,
          new[] { DomainObjectIDs.OrderItem3 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjectsNSide ()
    {
      TestObjectLoadingWithRelatedObject (
          delegate
          {
            OrderItem orderItem = DomainObjectIDs.OrderItem3.GetObject<OrderItem>();
            Order order = orderItem.Order;
            Assert.That (order, Is.Not.Null);
          },
          DomainObjectIDs.OrderItem3,
          false,
          DomainObjectIDs.Order3);
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1To1RealSide ()
    {
      TestObjectLoadingWithRelatedObject (
          delegate
          {
            Computer computer = DomainObjectIDs.Computer1.GetObject<Computer> ();
            Employee employee = computer.Employee;
            Assert.That (employee, Is.Not.Null);
          },
          DomainObjectIDs.Computer1,
          false,
          DomainObjectIDs.Employee3);
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1To1VirtualSide ()
    {
      TestObjectLoadingWithRelatedObject (
          delegate
          {
            Employee employee = DomainObjectIDs.Employee3.GetObject<Employee> ();
            Computer computer = employee.Computer;
            Assert.That (computer, Is.Not.Null);
          },
          DomainObjectIDs.Employee3,
          true,
          DomainObjectIDs.Computer1);
    }

    [Test]
    public void EmptyObjectLoadingWithRelatedObjects1Side ()
    {
      TestObjectLoadingWithRelatedObjectCollection (
          delegate
          {
            Customer customer = DomainObjectIDs.Customer2.GetObject<Customer>();
            int count = customer.Orders.Count;
            Assert.That (count, Is.EqualTo (0));
          },
          DomainObjectIDs.Customer2, false, new ObjectID[] { });
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjectsNSide ()
    {
      TestObjectLoadingWithRelatedObject (
          delegate
          {
            Client client = DomainObjectIDs.Client1.GetObject<Client> ();
            Client parent = client.ParentClient;
            Assert.That (parent, Is.Null);
          },
          DomainObjectIDs.Client1, false, null);
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjects1To1RealSide ()
    {
      TestObjectLoadingWithRelatedObject (
          delegate
          {
            Computer computer = DomainObjectIDs.Computer4.GetObject<Computer> ();
            Employee employee = computer.Employee;
            Assert.That (employee, Is.Null);
          },
          DomainObjectIDs.Computer4, false, null);
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjects1To1VirtualSide ()
    {
      TestObjectLoadingWithRelatedObject (
          delegate
          {
            Employee employee = DomainObjectIDs.Employee7.GetObject<Employee> ();
            Computer computer = employee.Computer;
            Assert.That (computer, Is.Null);
          },
          DomainObjectIDs.Employee7, false, null);
    }

    [Test]
    public void ObjectsLoaded ()
    {
      _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_transaction),
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.ClassWithAllDataTypes1 })));
      _extensionMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (_transaction),
            Arg<ReadOnlyCollection<DomainObject>>.Matches (collection => collection.Count == 1)));

      _mockRepository.ReplayAll();

      DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> (_transaction);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectsLoadedWithRelations ()
    {
      _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_transaction),
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order3 })));
      _extensionMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (_transaction),
            Arg<ReadOnlyCollection<DomainObject>>.Matches (collection => collection.Count == 1)));

      _mockRepository.ReplayAll();

      DomainObjectIDs.Order3.GetObject<Order> (_transaction);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectsLoadedWithEvents ()
    {
      var clientTransactionEventReceiver =
          _mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_transaction);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (
            mock => mock.ObjectsLoading (
                        Arg.Is (_transaction),
                        Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.ClassWithAllDataTypes1 })));

        clientTransactionEventReceiver.Expect (
            mock => mock.Loaded (Arg.Is (_transaction), Arg<ClientTransactionEventArgs>.Matches (args => args.DomainObjects.Count == 1)));
        _extensionMock.Expect (
            mock => mock.ObjectsLoaded (
                Arg.Is (_transaction),
                Arg<ReadOnlyCollection<DomainObject>>.Matches (collection => collection.Count == 1)));
      }

      _mockRepository.ReplayAll();

      DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> (_transaction);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectDelete ()
    {
      var eventReceiverMock = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_computerWithoutRelatedObjects);
      _extensionMock.BackToRecord();

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (mock => mock.ObjectDeleting (_transaction, _computerWithoutRelatedObjects));
        eventReceiverMock.Expect (mock => mock.Deleting (_computerWithoutRelatedObjects, EventArgs.Empty));
        eventReceiverMock.Expect (mock => mock.Deleted (_computerWithoutRelatedObjects, EventArgs.Empty));
        _extensionMock.Expect (mock => mock.ObjectDeleted (_transaction, _computerWithoutRelatedObjects));
      }

      _mockRepository.ReplayAll();

      _transaction.ExecuteInScope (() => _computerWithoutRelatedObjects.Delete ());

      _mockRepository.VerifyAll();
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
        customerOrders.EnsureDataComplete ();
        officialOrders = official.Orders;
        officialOrders.EnsureDataComplete ();
        Dev.Null = orderTicket.Order; // preload
      }

      var order1MockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_order1);
      var orderItem1MockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderItem1);
      var orderItem2MockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderItem2);
      var orderTicketMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderTicket);
      var officialMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (official);
      var customerMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (customer);

      var customerOrdersMockEventReceiver =
          _mockRepository.StrictMock<DomainObjectCollectionMockEventReceiver> (customerOrders);

      var officialOrdersMockEventReceiver =
          _mockRepository.StrictMock<DomainObjectCollectionMockEventReceiver> (officialOrders);

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.ObjectDeleting (_transaction, _order1);
        order1MockEventReceiver.Deleting (_order1, EventArgs.Empty);

        using (_mockRepository.Unordered())
        {
          customerOrdersMockEventReceiver.Removing (customerOrders, _order1);
          _extensionMock.RelationChanging (_transaction, customer, GetEndPointDefinition (typeof (Customer), "Orders"), _order1, null);
          customerMockEventReceiver.RelationChanging (GetEndPointDefinition (typeof (Customer), "Orders"), _order1, null);

          _extensionMock.RelationChanging (_transaction, orderTicket, GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);
          orderTicketMockEventReceiver.RelationChanging (GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);

          _extensionMock.RelationChanging (_transaction, orderItem1, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
          orderItem1MockEventReceiver.RelationChanging (GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);

          _extensionMock.RelationChanging (_transaction, orderItem2, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
          orderItem2MockEventReceiver.RelationChanging (GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);

          officialOrdersMockEventReceiver.Removing (officialOrders, _order1);
          _extensionMock.RelationChanging (_transaction, official, GetEndPointDefinition (typeof (Official), "Orders"), _order1, null);
          officialMockEventReceiver.RelationChanging (GetEndPointDefinition (typeof (Official), "Orders"), _order1, null);
        }

        using (_mockRepository.Unordered ())
        {
          customerMockEventReceiver.RelationChanged (GetEndPointDefinition (typeof (Customer), "Orders"), _order1, null);
          _extensionMock.RelationChanged (_transaction, customer, GetEndPointDefinition (typeof (Customer), "Orders"), _order1, null);
          customerOrdersMockEventReceiver.Removed (customerOrders, _order1);

          orderTicketMockEventReceiver.RelationChanged (GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);
          _extensionMock.RelationChanged (_transaction, orderTicket, GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);

          orderItem1MockEventReceiver.RelationChanged (GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
          _extensionMock.RelationChanged (_transaction, orderItem1, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);

          orderItem2MockEventReceiver.RelationChanged (GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
          _extensionMock.RelationChanged (_transaction, orderItem2, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);

          officialOrdersMockEventReceiver.Removed (officialOrders, _order1);
          officialMockEventReceiver.RelationChanged (GetEndPointDefinition (typeof (Official), "Orders"), _order1, null);
          _extensionMock.RelationChanged (_transaction, official, GetEndPointDefinition (typeof (Official), "Orders"), _order1, null);
        }

        order1MockEventReceiver.Deleted (_order1, EventArgs.Empty);
        _extensionMock.ObjectDeleted (_transaction, _order1);
      }

      _mockRepository.ReplayAll();

      _transaction.ExecuteInScope (() => _order1.Delete());
      
      _mockRepository.VerifyAll();
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

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationChanging (_transaction, location, GetEndPointDefinition (typeof (Location), "Client"), deletedClient, newClient);
        _extensionMock.RelationChanged (_transaction, location, GetEndPointDefinition (typeof (Location), "Client"), deletedClient, newClient);
      }

      _mockRepository.ReplayAll();

      _transaction.ExecuteInScope (() => location.Client = newClient);

      _mockRepository.VerifyAll();
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

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationChanging (_transaction, location, GetEndPointDefinition (typeof (Location), "Client"), deletedClient, newClient);
        _extensionMock.RelationChanged (_transaction, location, GetEndPointDefinition (typeof (Location), "Client"), deletedClient, newClient);
      }

      _mockRepository.ReplayAll();

      _transaction.ExecuteInScope (() => location.Client = newClient);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectDeleteTwice ()
    {
      var computer = DomainObjectIDs.Computer4.GetObject<Computer> (_transaction);
      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.ObjectDeleting (_transaction, computer);
        _extensionMock.ObjectDeleted (_transaction, computer);
      }

      _mockRepository.ReplayAll();

      _transaction.ExecuteInScope (computer.Delete);
      _transaction.ExecuteInScope (computer.Delete);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyRead ()
    {
      int orderNumber = _transaction.ExecuteInScope (() => _order1.OrderNumber);
      _mockRepository.BackToRecord (_extensionMock);

      var propertyDefinition = GetPropertyDefinition (typeof (Order), "OrderNumber");
      using (_mockRepository.Ordered())
      {
        _extensionMock.PropertyValueReading (_transaction, _order1, propertyDefinition, ValueAccess.Current);
        _extensionMock.PropertyValueRead (_transaction, _order1, propertyDefinition, orderNumber, ValueAccess.Current);
        _extensionMock.PropertyValueReading (_transaction, _order1, propertyDefinition, ValueAccess.Original);
        _extensionMock.PropertyValueRead (_transaction, _order1, propertyDefinition, orderNumber, ValueAccess.Original);
      }

      _mockRepository.ReplayAll();

      _transaction.ExecuteInScope (() => Dev.Null = _order1.OrderNumber);
      _transaction.ExecuteInScope (() => Dev.Null = _order1.Properties[propertyDefinition.PropertyName].GetOriginalValueWithoutTypeCheck());

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ReadObjectIDProperty ()
    {
      var customerPropertyDefinition = GetPropertyDefinition (typeof (Order), "Customer");
      var customerID = _order1.Properties[customerPropertyDefinition.PropertyName, _transaction].GetRelatedObjectID();

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.PropertyValueReading (_transaction, _order1, customerPropertyDefinition, ValueAccess.Current);
        _extensionMock.PropertyValueRead (_transaction, _order1, customerPropertyDefinition, customerID, ValueAccess.Current);
      }

      _mockRepository.ReplayAll();

      _transaction.ExecuteInScope (() => Dev.Null = _order1.Properties[customerPropertyDefinition.PropertyName, _transaction].GetRelatedObjectID ());

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertySetToSameValue ()
    {
      int orderNumber = _transaction.ExecuteInScope (() => _order1.OrderNumber);

      _mockRepository.BackToRecord (_extensionMock);
      // Note: No method call on the extension is expected.
      _mockRepository.ReplayAll();

      _transaction.ExecuteInScope (() => _order1.OrderNumber = orderNumber);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ChangeAndReadProperty ()
    {
      int oldOrderNumber = _transaction.ExecuteInScope (() => _order1.OrderNumber);
      int newOrderNumber = oldOrderNumber + 1;

      var orderNumberPropertyDefinition = GetPropertyDefinition (typeof (Order), "OrderNumber");

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.PropertyValueChanging (
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            oldOrderNumber,
            newOrderNumber);
        _extensionMock.PropertyValueChanged (
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            oldOrderNumber,
            newOrderNumber);

        _extensionMock.PropertyValueReading (
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            ValueAccess.Current);
        _extensionMock.PropertyValueRead (
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            newOrderNumber,
            ValueAccess.Current);
        _extensionMock.PropertyValueReading (
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            ValueAccess.Original);
        _extensionMock.PropertyValueRead (
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            oldOrderNumber,
            ValueAccess.Original);
      }

      _mockRepository.ReplayAll();
      using (_transaction.EnterNonDiscardingScope())
      {
        _order1.OrderNumber = newOrderNumber;
        Dev.Null = _order1.OrderNumber;
        Dev.Null = _order1.Properties[typeof (Order), "OrderNumber"].GetOriginalValueWithoutTypeCheck();
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyChange ()
    {
      int oldOrderNumber = _transaction.ExecuteInScope (() => _order1.OrderNumber);
      _mockRepository.BackToRecord (_extensionMock);

      var orderNumberPropertyDefinition = GetPropertyDefinition (typeof (Order), "OrderNumber");

      using (_mockRepository.Ordered())
      {
        _extensionMock.PropertyValueChanging (
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            oldOrderNumber,
            oldOrderNumber + 1);
        _extensionMock.PropertyValueChanged (
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            oldOrderNumber,
            oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        _order1.OrderNumber = oldOrderNumber + 1;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyChangeWithEvents ()
    {
      int oldOrderNumber = _transaction.ExecuteInScope (() => _order1.OrderNumber);
      _mockRepository.BackToRecord (_extensionMock);

      var domainObjectMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_order1);
      var orderNumberPropertyDefinition = GetPropertyDefinition (typeof (Order), "OrderNumber");
      
      using (_mockRepository.Ordered())
      {
        // "Changing" notifications

        _extensionMock.PropertyValueChanging (
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            oldOrderNumber,
            oldOrderNumber + 1);

        domainObjectMockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments();

        // "Changed" notifications

        domainObjectMockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments();

        _extensionMock.PropertyValueChanged (
            _transaction,
            _order1,
            orderNumberPropertyDefinition,
            oldOrderNumber,
            oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        _order1.OrderNumber = oldOrderNumber + 1;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void LoadRelatedDataContainerForVirtualEndPoint ()
    {
      //Note: no reading notification must be performed
      _mockRepository.ReplayAll();

      using (var persistenceManager = new PersistenceManager(NullPersistenceExtension.Instance))
      {
        ClassDefinition orderDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Order));
        IRelationEndPointDefinition orderTicketEndPointDefinition =
            orderDefinition.GetRelationEndPointDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
        persistenceManager.LoadRelatedDataContainer (RelationEndPointID.Create(_order1.ID, orderTicketEndPointDefinition));
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObject ()
    {
      OrderTicket orderTicket;

      using (_transaction.EnterNonDiscardingScope())
      {
        orderTicket = _order1.OrderTicket;
      }

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (_transaction, _order1, GetEndPointDefinition (typeof (Order), "OrderTicket"), ValueAccess.Current);
        _extensionMock.RelationRead (_transaction, _order1, GetEndPointDefinition (typeof (Order), "OrderTicket"), orderTicket, ValueAccess.Current);
      }

      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.OrderTicket;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      OrderTicket originalOrderTicket;
      using (_transaction.EnterNonDiscardingScope())
      {
        originalOrderTicket =
            (OrderTicket) _order1.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      }
      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (_transaction, _order1, GetEndPointDefinition (typeof (Order), "OrderTicket"), ValueAccess.Original);
        _extensionMock.RelationRead (_transaction, _order1, GetEndPointDefinition (typeof (Order), "OrderTicket"), originalOrderTicket, ValueAccess.Original);
      }

      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      }

      _mockRepository.VerifyAll();
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
      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _transaction, _order1, GetEndPointDefinition (typeof(Order), "OrderItems"), ValueAccess.Current);
        _extensionMock.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Rhino_Is.Same (_transaction),
            Rhino_Is.Same (_order1),
            Rhino_Is.Equal (GetEndPointDefinition (typeof (Order), "OrderItems")),
            Property.Value ("Count", 2) & Rhino.Mocks.Constraints.List.IsIn (orderItems[0]) & Rhino.Mocks.Constraints.List.IsIn (orderItems[1]),
            Rhino_Is.Equal (ValueAccess.Current));
      }

      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.OrderItems;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      DomainObjectCollection originalOrderItems;
      using (_transaction.EnterNonDiscardingScope())
      {
        originalOrderItems = _order1.GetOriginalRelatedObjects ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      }
      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _transaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), ValueAccess.Original);
        _extensionMock.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Original);

        LastCall.Constraints (
            Rhino_Is.Same (_transaction),
            Rhino_Is.Same (_order1),
            Rhino_Is.Equal (GetEndPointDefinition (typeof (Order), "OrderItems")),
            Property.Value ("Count", 2) & Rhino.Mocks.Constraints.List.IsIn (originalOrderItems[0]) & Rhino.Mocks.Constraints.List.IsIn (originalOrderItems[1]),
            Rhino_Is.Equal (ValueAccess.Original));
      }

      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.GetOriginalRelatedObjects ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObjectWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _transaction, _order1, GetEndPointDefinition (typeof(Order), "OrderTicket"), ValueAccess.Current);

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_transaction), 
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 1)));
        
        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Rhino_Is.Same (_transaction), Property.Value ("Count", 1));

        _extensionMock.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.Constraints (
            Rhino_Is.Same (_transaction),
            Rhino_Is.Same (_order1),
            Rhino_Is.Equal (GetEndPointDefinition (typeof (Order), "OrderTicket")),
            Rhino_Is.NotNull(),
            Rhino_Is.Equal (ValueAccess.Current));
      }
      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.OrderTicket;
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _transaction, _order1, GetEndPointDefinition (typeof(Order), "OrderItems"), ValueAccess.Current);
        _extensionMock.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Rhino_Is.Same (_transaction),
            Rhino_Is.Same (_order1),
            Rhino_Is.Equal (GetEndPointDefinition (typeof (Order), "OrderItems")),
            Rhino_Is.NotNull (),
            Rhino_Is.Equal (ValueAccess.Current));

        _extensionMock.ObjectsLoading (_transaction, null);
        LastCall.Constraints (Rhino_Is.Same (_transaction), Property.Value ("Count", 2));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Rhino_Is.Same (_transaction), Property.Value ("Count", 2));
      }
      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        _order1.OrderItems.EnsureDataComplete();
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _transaction, _order1, GetEndPointDefinition (typeof (Order), "OrderTicket"), ValueAccess.Original);

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_transaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 1)));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Rhino_Is.Same (_transaction), Property.Value ("Count", 1));
        _extensionMock.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.Constraints (
            Rhino_Is.Same (_transaction),
            Rhino_Is.Same (_order1),
            Rhino_Is.Equal (GetEndPointDefinition (typeof (Order), "OrderTicket")),
            Rhino_Is.NotNull(),
            Rhino_Is.Equal (ValueAccess.Original));
      }
      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _transaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), ValueAccess.Original);

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_transaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 2)));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Rhino_Is.Same (_transaction), Property.Value ("Count", 2));
        _extensionMock.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Rhino_Is.Same (_transaction),
            Rhino_Is.Same (_order1),
            Rhino_Is.Equal (GetEndPointDefinition (typeof (Order), "OrderItems")),
            Rhino_Is.NotNull(),
            Rhino_Is.Equal (ValueAccess.Original));
      }
      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.GetOriginalRelatedObjects ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void FilterQueryResult ()
    {
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1);

      using (_transaction.EnterNonDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      }

      _mockRepository.BackToRecord (_extensionMock);

      QueryResult<DomainObject> newQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject>();
      _extensionMock
          .Expect (
          mock => mock.FilterQueryResult (Arg.Is (_transaction), Arg<QueryResult<DomainObject>>.Matches (qr => qr.Count == 2 && qr.Query == query)))
          .Return (newQueryResult);

      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        QueryResult<DomainObject> finalResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
        Assert.That (finalResult, NUnit.Framework.Is.SameAs (newQueryResult));
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void FilterQueryResultWithLoad ()
    {
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer4);

      QueryResult<DomainObject> newQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject>();

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_transaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 2)));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Rhino_Is.Same (_transaction), Property.Value ("Count", 2));
        _extensionMock
            .Expect (
            mock =>
            mock.FilterQueryResult (Arg.Is (_transaction), Arg<QueryResult<DomainObject>>.Matches (qr => qr.Count == 2 && qr.Query == query)))
            .Return (newQueryResult);
      }

      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        QueryResult<DomainObject> finalQueryResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
        Assert.That (finalQueryResult, NUnit.Framework.Is.SameAs (newQueryResult));
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void FilterQueryResultWithFiltering ()
    {
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer4);

      var filteringExtension = _mockRepository.StrictMock<ClientTransactionExtensionWithQueryFiltering>();
      _transaction.Extensions.Add (filteringExtension);

      var lastExtension = _mockRepository.StrictMock<IClientTransactionExtension>();
      lastExtension.Stub (stub => stub.Key).Return ("LastExtension");
      lastExtension.Replay();
      _transaction.Extensions.Add (lastExtension);
      lastExtension.BackToRecord();

      QueryResult<DomainObject> newQueryResult1 = TestQueryFactory.CreateTestQueryResult<DomainObject> (query, new[] { _order1 });
      QueryResult<DomainObject> newQueryResult2 = TestQueryFactory.CreateTestQueryResult<DomainObject> (query);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_transaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 2)));
        filteringExtension.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_transaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 2)));
        lastExtension.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_transaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 2)));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Rhino_Is.Same (_transaction), Property.Value ("Count", 2));
        filteringExtension.ObjectsLoaded (null, null);
        LastCall.Constraints (Rhino_Is.Same (_transaction), Property.Value ("Count", 2));
        lastExtension.ObjectsLoaded (null, null);
        LastCall.Constraints (Rhino_Is.Same (_transaction), Property.Value ("Count", 2));

        _extensionMock
            .Expect (
            mock =>
            mock.FilterQueryResult (Arg.Is (_transaction), Arg<QueryResult<DomainObject>>.Matches (qr => qr.Count == 2 && qr.Query == query)))
            .Return (newQueryResult1);
        filteringExtension
            .Expect (mock => mock.FilterQueryResult (_transaction, newQueryResult1))
            .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
        lastExtension
            .Expect (
            mock =>
            mock.FilterQueryResult (Arg.Is (_transaction), Arg<QueryResult<DomainObject>>.Matches (qr => qr.Count == 0 && qr.Query == query)))
            .Return (newQueryResult2);
      }

      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        QueryResult<DomainObject> finalQueryResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
        Assert.That (finalQueryResult, NUnit.Framework.Is.SameAs (newQueryResult2));
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithChangedPropertyValue ()
    {
      Computer computer;
      using (_transaction.EnterNonDiscardingScope())
      {
        computer = DomainObjectIDs.Computer4.GetObject<Computer> ();
        computer.SerialNumber = "newSerialNumber";
      }

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (
            mock =>
            mock.Committing (
                Arg.Is (_transaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { computer }),
                Arg<CommittingEventRegistrar>.Is.TypeOf));
        _extensionMock.Expect (mock => mock.CommitValidate (
            Arg.Is (_transaction), 
            Arg<ReadOnlyCollection<PersistableData>>.Matches (c => c.Select (d => d.DomainObject).SetEquals (new[] { computer }))));
        _extensionMock.Expect (mock => mock.Committed (Arg.Is (_transaction), Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { computer })));
      }

      _mockRepository.ReplayAll();

      _transaction.Commit();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithChangedRelationValue ()
    {
      Computer computer;
      Employee employee;

      using (_transaction.EnterNonDiscardingScope())
      {
        computer = DomainObjectIDs.Computer4.GetObject<Computer> ();
        employee = DomainObjectIDs.Employee1.GetObject<Employee> ();
        computer.Employee = employee;
      }

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (
            mock =>
            mock.Committing (
                Arg.Is (_transaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new DomainObject[] { computer, employee }),
                Arg<CommittingEventRegistrar>.Is.TypeOf));
        _extensionMock.Expect (
            mock =>
            mock.CommitValidate (
                Arg.Is (_transaction), 
                Arg<ReadOnlyCollection<PersistableData>>.Matches (c => c.Select (d => d.DomainObject).SetEquals (new DomainObject[] { computer, employee }))));
        _extensionMock.Expect (
            mock =>
            mock.Committed (Arg.Is (_transaction), Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new DomainObject[] { computer, employee })));
      }

      _mockRepository.ReplayAll();

      _transaction.Commit();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithChangedRelationValueWithClassIDColumn ()
    {
      Customer oldCustomer;
      Customer newCustomer;
      using (_transaction.EnterNonDiscardingScope())
      {
        oldCustomer = _order1.Customer;
        newCustomer = DomainObjectIDs.Customer2.GetObject<Customer> ();
        _order1.Customer = newCustomer;
      }
      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (
            mock =>
            mock.Committing (
                Arg.Is (_transaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new DomainObject[] { _order1, newCustomer, oldCustomer }),
                Arg<CommittingEventRegistrar>.Is.TypeOf));
        _extensionMock.Expect (
            mock =>
            mock.CommitValidate (
                Arg.Is (_transaction), 
                Arg<ReadOnlyCollection<PersistableData>>.Matches (c => c.Select (d => d.DomainObject).SetEquals (new DomainObject[] { _order1, newCustomer, oldCustomer }))));
        _extensionMock.Expect (
            mock =>
            mock.Committed (
                Arg.Is (_transaction), Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new DomainObject[] { _order1, newCustomer, oldCustomer })));
      }

      _mockRepository.ReplayAll();

      _transaction.Commit();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithEvents ()
    {
      SetDatabaseModifyable();

      Computer computer;
      using (_transaction.EnterNonDiscardingScope())
      {
        computer = DomainObjectIDs.Computer4.GetObject<Computer> ();
        computer.SerialNumber = "newSerialNumber";
      }
      _mockRepository.BackToRecord (_extensionMock);

      var clientTransactionMockEventReceiver =
          _mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_transaction);
      var computerEventReveiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (computer);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (
            mock =>
            mock.Committing (
                Arg.Is (_transaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { computer }),
                Arg<CommittingEventRegistrar>.Is.TypeOf));
        clientTransactionMockEventReceiver.Expect (mock =>mock.Committing (computer));
        computerEventReveiver.Expect (mock => mock.Committing ());

        _extensionMock.Expect (mock => mock.CommitValidate (
            Arg.Is (_transaction), 
            Arg<ReadOnlyCollection<PersistableData>>.Matches (c => c.Select (d => d.DomainObject).SetEquals (new DomainObject[] { computer }))));

        computerEventReveiver.Expect (mock => mock.Committed ());
        clientTransactionMockEventReceiver.Expect (mock => mock.Committed (computer));
        _extensionMock.Expect (mock => mock.Committed (Arg.Is (_transaction), Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { computer })));
      }

      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        _transaction.Commit();
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Rollback ()
    {
      Computer computer;
      using (_transaction.EnterNonDiscardingScope())
      {
        computer = DomainObjectIDs.Computer4.GetObject<Computer> ();
        computer.SerialNumber = "newSerialNumber";
      }

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RollingBack (null, null);
        LastCall.Constraints (Rhino_Is.Same (_transaction), Rhino.Mocks.Constraints.List.IsIn (computer));

        _extensionMock.RolledBack (null, null);
        LastCall.Constraints (Rhino_Is.Same (_transaction), Rhino.Mocks.Constraints.List.IsIn (computer));
      }

      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.Rollback();
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void SubTransactions ()
    {
      ClientTransaction initializedTransaction = null;

      var subExtenstionMock = _mockRepository.StrictMock<IClientTransactionExtension>();

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (mock => mock.SubTransactionCreating (_transaction));
        _extensionMock
            .Expect (mock => mock.SubTransactionInitialize (Arg.Is (_transaction), Arg<ClientTransaction>.Is.Anything))
            .WhenCalled (
                mi =>
                {
                  initializedTransaction = (ClientTransaction) mi.Arguments[1];
                  initializedTransaction.Extensions.Add (subExtenstionMock);
            });
        subExtenstionMock.Stub (stub => stub.Key).Return ("inner");
        subExtenstionMock.Expect (mock => mock.TransactionInitialize (Arg<ClientTransaction>.Matches (tx => tx == initializedTransaction)));
        _extensionMock.Expect (mock => mock.SubTransactionCreated (
            Arg.Is (_transaction), 
            Arg<ClientTransaction>.Matches (tx => tx == initializedTransaction)));
        subExtenstionMock.Expect (mock => mock.TransactionDiscard (Arg<ClientTransaction>.Matches (tx => tx == initializedTransaction)));
      }

      _mockRepository.ReplayAll();

      var subTransaction = _transaction.CreateSubTransaction ();
      subTransaction.Discard();

      _mockRepository.VerifyAll();
      Assert.That (subTransaction, Is.SameAs (initializedTransaction));
    }

    [Test]
    public void GetObjects ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_transaction),
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order3, DomainObjectIDs.Order4 })));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Rhino_Is.Same (_transaction), Rhino.Mocks.Constraints.List.Count (Rhino_Is.Equal (2)));
      }

      _mockRepository.ReplayAll();

      using (_transaction.EnterNonDiscardingScope())
      {
        LifetimeService.GetObjects<DomainObject> (_transaction, DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4);
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void UnloadData ()
    {
      using (_mockRepository.Ordered ())
      {
        _extensionMock
            .Expect (mock => mock.ObjectsUnloading (
                        Arg.Is (_transaction),
                        Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _order1 })))
            .WhenCalled (mi => Assert.That (_transaction.DataManager.DataContainers[_order1.ID] != null));
        _extensionMock
            .Expect (mock => mock.ObjectsUnloaded (
                        Arg.Is (_transaction),
                        Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _order1 })))
            .WhenCalled (mi => Assert.That (_transaction.DataManager.DataContainers[_order1.ID] == null));
      }

      _mockRepository.ReplayAll ();

      UnloadService.UnloadData (_transaction, _order1.ID);

      _mockRepository.VerifyAll ();
    }

    private void TestObjectLoadingWithRelatedObjectCollection (
        Action accessCode,
        ObjectID expectedMainObjectID,
        bool expectLoadEventsForRelatedObjects,
        ObjectID[] expectedRelatedIDs)
    {
      _mockRepository.BackToRecordAll ();
      using (_mockRepository.Ordered ())
      {
        // loading of main object
        _extensionMock.ObjectsLoading (Arg.Is ((ClientTransaction) _transaction), Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { expectedMainObjectID }));
        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Rhino_Is.Same (_transaction), Rhino_Is.NotNull ());

        // accessing relation property

        _extensionMock.RelationReading (null, null, null, ValueAccess.Current);
        LastCall.IgnoreArguments ();

        _extensionMock.RelationRead (_transaction, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.IgnoreArguments ();

        if (expectLoadEventsForRelatedObjects)
        {
          _extensionMock.ObjectsLoading (Arg.Is ((ClientTransaction) _transaction), Arg<ReadOnlyCollection<ObjectID>>.List.Equal (expectedRelatedIDs));
          _extensionMock.ObjectsLoaded (_transaction, null);
          LastCall.IgnoreArguments();
        }

        // loading of main object a second time

        // accessing relation property a second time

        _extensionMock.RelationReading (_transaction, null, null, ValueAccess.Current);
        LastCall.IgnoreArguments ();

        _extensionMock.RelationRead (_transaction, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.IgnoreArguments ();
      }

      using (_transaction.EnterNonDiscardingScope ())
      {
        _mockRepository.ReplayAll ();

        accessCode ();
        accessCode ();

        _mockRepository.VerifyAll ();
      }
    }

    private void TestObjectLoadingWithRelatedObject (
        Action accessCode,
        ObjectID expectedMainObjectID,
        bool expectLoadEventsForRelatedObject,
        ObjectID expectedRelatedID)
    {
      _mockRepository.BackToRecordAll ();
      using (_mockRepository.Ordered ())
      {
        // loading of main object
        _extensionMock.ObjectsLoading (Arg.Is ((ClientTransaction) _transaction), Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { expectedMainObjectID }));
        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Rhino_Is.Same (_transaction), Rhino_Is.NotNull ());

        // accessing relation property

        _extensionMock.RelationReading (null, null, null, ValueAccess.Current);
        LastCall.IgnoreArguments ();

        if (expectLoadEventsForRelatedObject)
        {
          _extensionMock.ObjectsLoading (Arg.Is ((ClientTransaction) _transaction), Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { expectedRelatedID }));
        }

        if (expectLoadEventsForRelatedObject)
        {
          _extensionMock.ObjectsLoaded (_transaction, null);
          LastCall.IgnoreArguments ();
        }

        _extensionMock.RelationRead (_transaction, null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.IgnoreArguments ();

        // loading of main object a second time

        // accessing relation property a second time

        _extensionMock.RelationReading (_transaction, null, null, ValueAccess.Current);
        LastCall.IgnoreArguments ();

        _extensionMock.RelationRead (_transaction, null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.IgnoreArguments ();
      }

      using (_transaction.EnterNonDiscardingScope ())
      {
        _mockRepository.ReplayAll ();

        accessCode ();
        accessCode ();

        _mockRepository.VerifyAll ();
      }
    }
  }
}
