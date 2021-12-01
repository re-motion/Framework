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
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.RhinoMocks.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.FunctionalProgramming;
using Is = NUnit.Framework.Is;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionExtensionTest : ClientTransactionBaseTest
  {
    private Mock<IClientTransactionExtension> _extensionMock;
    private ClientTransaction _subTransaction;
    private ClientTransactionScope _subTransactionScope;

    private Order _order1;
    private DataManager _parentTransactionDataManager;
    private DataManager _subTransactionDataManager;
    private Product _product1;

    public override void OneTimeSetUp ()
    {
      base.OneTimeSetUp();
      SetDatabaseModifyable();
    }

    public override void SetUp ()
    {
      base.SetUp();

      _extensionMock = new Mock<IClientTransactionExtension> (MockBehavior.Strict);

      _subTransaction = TestableClientTransaction.CreateSubTransaction();
      _subTransactionScope = _subTransaction.EnterDiscardingScope();

      _order1 = DomainObjectIDs.Order1.GetObject<Order>();
      _product1 = DomainObjectIDs.Product1.GetObject<Product>();

      _extensionMock.Setup (stub => stub.Key).Returns ("TestExtension");
      TestableClientTransaction.Extensions.Add(_extensionMock.Object);
      _subTransaction.Extensions.Add(_extensionMock.Object);
      _extensionMock.BackToRecord();

      _mockRepository.BackToRecordAll();

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
      var sequence = new MockSequence();
      _extensionMock.Object.NewObjectCreating(_subTransaction, typeof(Order));

      Order.NewObject();

      _extensionMock.Verify();
    }

    [Test]
    public void ObjectLoading ()
    {
      _mockRepository.BackToRecordAll();

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction.ParentTransaction,
            new[] { DomainObjectIDs.Order3 })).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoaded (
            _subTransaction.ParentTransaction,
            It.Is<ReadOnlyCollection<DomainObject>> (list => list.Count == 1))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction,
            new[] { DomainObjectIDs.Order3 })).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoaded (
            _subTransaction,
            It.Is<ReadOnlyCollection<DomainObject>> (list => list.Count == 1))).Verifiable();

      Dev.Null = DomainObjectIDs.Order3.GetObject<Order>();
      Dev.Null = DomainObjectIDs.Order3.GetObject<Order>();

      _extensionMock.Verify();
    }

    private void TestObjectLoadingWithRelatedObject (
        Action accessCode,
        ObjectID expectedMainObjectID,
        bool expectLoadEventsForRelatedObject,
        ObjectID expectedRelatedID)
    {
      _mockRepository.BackToRecordAll();
      var sequence = new MockSequence();
      _extensionMock.Object.ObjectsLoading(
            _subTransaction.ParentTransaction, new[] { expectedMainObjectID });
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction.ParentTransaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null))).Verifiable();
      _extensionMock.Object.ObjectsLoading(_subTransaction, new[] { expectedMainObjectID });
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null))).Verifiable();
      _extensionMock.Expect (_ => _.RelationReading(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)),Arg<DomainObject>.Matches(_ => Mocks_Is.Anything()),Arg<IRelationEndPointDefinition>.Matches(_ => Mocks_Is.Anything()),Arg<ValueAccess>.Matches(_ => Mocks_Is.Anything())));
      if (expectLoadEventsForRelatedObject)
        {
          _extensionMock.Object.ObjectsLoading(
              _subTransaction.ParentTransaction, It.Is<ReadOnlyCollection<ObjectID>> (_ => _.Contains (expectedRelatedID)));
          _extensionMock.Expect (_ => _.ObjectsLoaded(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction.ParentTransaction)),Arg<IReadOnlyList<DomainObject>>.Matches(_ => Mocks_Is.Anything())));

          _extensionMock.Object.ObjectsLoading(_subTransaction, It.Is<ReadOnlyCollection<ObjectID>> (_ => _.Contains (expectedRelatedID)));
          _extensionMock.Expect (_ => _.ObjectsLoaded(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)),Arg<IReadOnlyList<DomainObject>>.Matches(_ => Mocks_Is.Anything())));
        }
      _extensionMock.Expect (_ => _.RelationRead(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)),Arg<DomainObject>.Matches(_ => Mocks_Is.Anything()),Arg<IRelationEndPointDefinition>.Matches(_ => Mocks_Is.Anything()),Arg<DomainObject>.Matches(_ => Mocks_Is.Anything()),Arg<ValueAccess>.Matches(_ => Mocks_Is.Anything())));
      _extensionMock.Expect (_ => _.RelationReading(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)),Arg<DomainObject>.Matches(_ => Mocks_Is.Anything()),Arg<IRelationEndPointDefinition>.Matches(_ => Mocks_Is.Anything()),Arg<ValueAccess>.Matches(_ => Mocks_Is.Anything())));
      _extensionMock.Expect (_ => _.RelationRead(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)),Arg<DomainObject>.Matches(_ => Mocks_Is.Anything()),Arg<IRelationEndPointDefinition>.Matches(_ => Mocks_Is.Anything()),Arg<DomainObject>.Matches(_ => Mocks_Is.Anything()),Arg<ValueAccess>.Matches(_ => Mocks_Is.Anything())));

      accessCode();
      accessCode();

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
      _extensionMock.Object.ObjectsLoading(
            _subTransaction.ParentTransaction, new[] { expectedMainObjectID });
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction.ParentTransaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null))).Verifiable();
      _extensionMock.Object.ObjectsLoading(_subTransaction, new[] { expectedMainObjectID });
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null))).Verifiable();
      _extensionMock.Expect (_ => _.RelationReading(It.Is<ClientTransaction> (_ => object.ReferenceEquals(_, _subTransaction)),Arg<DomainObject>.Matches(_ => Mocks_Is.Anything()),Arg<IRelationEndPointDefinition>.Matches(_ => Mocks_Is.Anything()),Arg<ValueAccess>.Matches(_ => Mocks_Is.Anything())));
      _extensionMock.Expect (_ => _.RelationRead(It.Is<ClientTransaction> (_ => object.ReferenceEquals(_, _subTransaction)),Arg<DomainObject>.Matches(_ => Mocks_Is.Anything()),Arg<IRelationEndPointDefinition>.Matches(_ => Mocks_Is.Anything()),Arg<IReadOnlyCollectionData<DomainObject>>.Matches(_ => Mocks_Is.Anything()),Arg<ValueAccess>.Matches(_ => Mocks_Is.Anything())));
      if (expectLoadEventsForRelatedObjects)
        {
          _extensionMock.Object.ObjectsLoading(
              _subTransaction.ParentTransaction, It.Is<ReadOnlyCollection<ObjectID>> (_ => expectedRelatedIDs.All (_.Contains)));
          _extensionMock.Expect (_ => _.ObjectsLoaded(It.Is<ClientTransaction> (_ => object.ReferenceEquals(_, _subTransaction.ParentTransaction)),Arg<IReadOnlyList<DomainObject>>.Matches(_ => Mocks_Is.Anything())));

          _extensionMock.Object.ObjectsLoading(_subTransaction, It.Is<ReadOnlyCollection<ObjectID>> (_ => expectedRelatedIDs.All (_.Contains)));
          _extensionMock.Expect (_ => _.ObjectsLoaded(It.Is<ClientTransaction> (_ => object.ReferenceEquals(_, _subTransaction)),Arg<IReadOnlyList<DomainObject>>.Matches(_ => Mocks_Is.Anything())));
        }
      _extensionMock.Expect (_ => _.RelationReading(It.Is<ClientTransaction> (_ => object.ReferenceEquals(_, _subTransaction)),Arg<DomainObject>.Matches(_ => Mocks_Is.Anything()),Arg<IRelationEndPointDefinition>.Matches(_ => Mocks_Is.Anything()),Arg<ValueAccess>.Matches(_ => Mocks_Is.Anything())));
      _extensionMock.Expect (_ => _.RelationRead(It.Is<ClientTransaction> (_ => object.ReferenceEquals(_, _subTransaction)),Arg<DomainObject>.Matches(_ => Mocks_Is.Anything()),Arg<IRelationEndPointDefinition>.Matches(_ => Mocks_Is.Anything()),Arg<IReadOnlyCollectionData<DomainObject>>.Matches(_ => Mocks_Is.Anything()),Arg<ValueAccess>.Matches(_ => Mocks_Is.Anything())));

      _mockRepository.ReplayAll();

      accessCode();
      accessCode();

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
          DomainObjectIDs.Order3, true, new[] { DomainObjectIDs.OrderItem3 });
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
          DomainObjectIDs.Employee3, true, DomainObjectIDs.Computer1);
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
          DomainObjectIDs.Customer2, false, null);
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
      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction.ParentTransaction,
            new[] { DomainObjectIDs.ClassWithAllDataTypes1 })).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoaded (
            _subTransaction.ParentTransaction,
            It.Is<ReadOnlyCollection<DomainObject>> (list => list.Count == 1))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction,
            new[] { DomainObjectIDs.ClassWithAllDataTypes1 })).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoaded (
            _subTransaction,
            It.Is<ReadOnlyCollection<DomainObject>> (list => list.Count == 1))).Verifiable();

      DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();

      _extensionMock.Verify();
    }

    [Test]
    public void ObjectsLoadedWithRelations ()
    {
      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction.ParentTransaction,
            new[] { DomainObjectIDs.Order3 })).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoaded (
            _subTransaction.ParentTransaction,
            It.Is<ReadOnlyCollection<DomainObject>> (list => list.Count == 1))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction,
            new[] { DomainObjectIDs.Order3 })).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoaded (
            _subTransaction,
            It.Is<ReadOnlyCollection<DomainObject>> (list => list.Count == 1))).Verifiable();

      DomainObjectIDs.Order3.GetObject<Order>();

      _extensionMock.Verify();
    }

    [Test]
    public void ObjectsLoadedWithEvents ()
    {
      var clientTransactionEventReceiver =
          new Mock<ClientTransactionMockEventReceiver> (MockBehavior.Strict, _subTransaction);

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (
            mock => mock.ObjectsLoading (
                _subTransaction.ParentTransaction,
                new[] { DomainObjectIDs.ClassWithAllDataTypes1 })).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoaded (
            _subTransaction.ParentTransaction,
            It.Is<ReadOnlyCollection<DomainObject>> (list => list.Count == 1))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (
            mock => mock.ObjectsLoading (
                        _subTransaction,
                        new[] { DomainObjectIDs.ClassWithAllDataTypes1 })).Verifiable();
      clientTransactionEventReceiver.InSequence (sequence).Setup (
            mock => mock.Loaded (_subTransaction, It.Is<ClientTransactionEventArgs> (args => args.DomainObjects.Count == 1))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoaded (
            _subTransaction,
            It.Is<ReadOnlyCollection<DomainObject>> (list => list.Count == 1))).Verifiable();

      DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();

      _extensionMock.Verify();
      clientTransactionEventReceiver.Verify();
    }

    [Test]
    public void ObjectDelete ()
    {
      Computer computer = DomainObjectIDs.Computer4.GetObject<Computer>();

      var computerEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, computer);
      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.ObjectDeleting(_subTransaction, computer);

      computerEventReceiver.Object.Deleting(computer, EventArgs.Empty);

      computerEventReceiver.Object.Deleted(computer, EventArgs.Empty);

      _extensionMock.Object.ObjectDeleted(_subTransaction, computer);

      computer.Delete();

      _extensionMock.Verify();
      computerEventReceiver.Verify();
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
        _extensionMock.Object.ObjectDeleting(_subTransaction, _order1);
        order1MockEventReceiver.Object.Deleting(_order1, EventArgs.Empty);

        using (_mockRepository.Unordered())
        {
          customerOrdersMockEventReceiver.Object.Removing(customerOrders, _order1);
          _extensionMock.Object.RelationChanging(_subTransaction, customer, GetEndPointDefinition(typeof(Customer), "Orders"), _order1, null);
          customerMockEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Customer), "Orders"), _order1, null);

          _extensionMock.Object.RelationChanging(_subTransaction, orderTicket, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);
          orderTicketMockEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);

          _extensionMock.Object.RelationChanging(_subTransaction, orderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);
          orderItem1MockEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

          _extensionMock.Object.RelationChanging(_subTransaction, orderItem2, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);
          orderItem2MockEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

          officialOrdersMockEventReceiver.Object.Removing(officialOrders, _order1);
          _extensionMock.Object.RelationChanging(_subTransaction, official, GetEndPointDefinition(typeof(Official), "Orders"), _order1, null);
          officialMockEventReceiver.Object.RelationChanging(GetEndPointDefinition(typeof(Official), "Orders"), _order1, null);
        }

        using (_mockRepository.Unordered())
        {
          customerMockEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Customer), "Orders"), _order1, null);
          _extensionMock.Object.RelationChanged(_subTransaction, customer, GetEndPointDefinition(typeof(Customer), "Orders"), _order1, null);
          customerOrdersMockEventReceiver.Object.Removed(customerOrders, _order1);

          orderTicketMockEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);
          _extensionMock.Object.RelationChanged(_subTransaction, orderTicket, GetEndPointDefinition(typeof(OrderTicket), "Order"), _order1, null);

          orderItem1MockEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);
          _extensionMock.Object.RelationChanged(_subTransaction, orderItem1, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

          orderItem2MockEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);
          _extensionMock.Object.RelationChanged(_subTransaction, orderItem2, GetEndPointDefinition(typeof(OrderItem), "Order"), _order1, null);

          officialOrdersMockEventReceiver.Object.Removed(officialOrders, _order1);
          officialMockEventReceiver.Object.RelationChanged(GetEndPointDefinition(typeof(Official), "Orders"), _order1, null);
          _extensionMock.Object.RelationChanged(_subTransaction, official, GetEndPointDefinition(typeof(Official), "Orders"), _order1, null);
        }

        order1MockEventReceiver.Object.Deleted(_order1, EventArgs.Empty);
        _extensionMock.Object.ObjectDeleted(_subTransaction, _order1);
      }

      _mockRepository.ReplayAll();

      _order1.Delete();

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
      Location location = DomainObjectIDs.Location1.GetObject<Location>();

      Client deletedClient = location.Client;
      deletedClient.Delete();

      Client newClient = Client.NewObject();

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.RelationChanging(_subTransaction, location, GetEndPointDefinition(typeof(Location), "Client"), deletedClient, newClient);

      _extensionMock.Object.RelationChanged(_subTransaction, location, GetEndPointDefinition(typeof(Location), "Client"), deletedClient, newClient);

      location.Client = newClient;

      _extensionMock.Verify();
    }

    [Test]
    public void RelationChangesWithUnidirectionalRelationshipWhenResettingNewLoaded ()
    {
      Location location = DomainObjectIDs.Location1.GetObject<Location>();
      location.Client = Client.NewObject();

      Client deletedClient = location.Client;
      location.Client.Delete();

      Client newClient = Client.NewObject();

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.RelationChanging(_subTransaction, location, GetEndPointDefinition(typeof(Location), "Client"), deletedClient, newClient);

      _extensionMock.Object.RelationChanged(_subTransaction, location, GetEndPointDefinition(typeof(Location), "Client"), deletedClient, newClient);

      location.Client = newClient;

      _extensionMock.Verify();
    }

    [Test]
    public void ObjectDeleteTwice ()
    {
      Computer computer = DomainObjectIDs.Computer4.GetObject<Computer>();

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.ObjectDeleting(_subTransaction, computer);

      _extensionMock.Object.ObjectDeleted(_subTransaction, computer);

      computer.Delete();
      computer.Delete();

      _extensionMock.Verify();
    }

    [Test]
    public void PropertyRead ()
    {
      int orderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord(_extensionMock.Object);

      var propertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

      var sequence = new MockSequence();

      _extensionMock.Object.PropertyValueReading(
            _subTransaction,
            _order1,
            propertyDefinition,
            ValueAccess.Current);

      _extensionMock.Object.PropertyValueRead(
            _subTransaction,
            _order1,
            propertyDefinition,
            orderNumber,
            ValueAccess.Current);

      _extensionMock.Object.PropertyValueReading(
            _subTransaction,
            _order1,
            propertyDefinition,
            ValueAccess.Original);

      _extensionMock.Object.PropertyValueRead(
            _subTransaction,
            _order1,
            propertyDefinition,
            orderNumber,
            ValueAccess.Original);

      Dev.Null = _order1.OrderNumber;
      Dev.Null = _order1.Properties[propertyDefinition.PropertyName].GetOriginalValueWithoutTypeCheck();

      _extensionMock.Verify();
    }

    [Test]
    public void ReadObjectIDProperty ()
    {
      var propertyDefinition = GetPropertyDefinition(typeof(Order), "Customer");
      var customerID = _order1.Properties[propertyDefinition.PropertyName].GetRelatedObjectID();

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.PropertyValueReading(_subTransaction, _order1, propertyDefinition, ValueAccess.Current);

      _extensionMock.Object.PropertyValueRead(_subTransaction, _order1, propertyDefinition, customerID, ValueAccess.Current);

      Dev.Null = _order1.Properties[propertyDefinition.PropertyName].GetRelatedObjectID();

      _extensionMock.Verify();
    }

    [Test]
    public void PropertySetToSameValue ()
    {
      int orderNumber = _order1.OrderNumber;

      _mockRepository.BackToRecord(_extensionMock.Object);

      _order1.OrderNumber = orderNumber;

      _extensionMock.Verify();
    }

    [Test]
    public void ChangeAndReadProperty ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      int newOrderNumber = oldOrderNumber + 1;

      var propertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.PropertyValueChanging(
            _subTransaction,
            _order1,
            propertyDefinition,
            oldOrderNumber,
            newOrderNumber);

      _extensionMock.Object.PropertyValueChanged(
            _subTransaction,
            _order1,
            propertyDefinition,
            oldOrderNumber,
            newOrderNumber);

      _extensionMock.Object.PropertyValueReading(
            _subTransaction,
            _order1,
            propertyDefinition,
            ValueAccess.Current);

      _extensionMock.Object.PropertyValueRead(
            _subTransaction,
            _order1,
            propertyDefinition,
            newOrderNumber,
            ValueAccess.Current);

      _extensionMock.Object.PropertyValueReading(
            _subTransaction,
            _order1,
            propertyDefinition,
            ValueAccess.Original);

      _extensionMock.Object.PropertyValueRead(
            _subTransaction,
            _order1,
            propertyDefinition,
            oldOrderNumber,
            ValueAccess.Original);

      _order1.OrderNumber = newOrderNumber;
      Dev.Null = _order1.OrderNumber;
      Dev.Null = _order1.Properties[typeof(Order), "OrderNumber"].GetOriginalValueWithoutTypeCheck();

      _extensionMock.Verify();
    }

    [Test]
    public void PropertyChange ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord(_extensionMock.Object);

      var propertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

      var sequence = new MockSequence();

      _extensionMock.Object.PropertyValueChanging(
            _subTransaction,
            _order1,
            propertyDefinition,
            oldOrderNumber,
            oldOrderNumber + 1);

      _extensionMock.Object.PropertyValueChanged(
            _subTransaction,
            _order1,
            propertyDefinition,
            oldOrderNumber,
            oldOrderNumber + 1);

      _order1.OrderNumber = oldOrderNumber + 1;

      _extensionMock.Verify();
    }

    [Test]
    public void PropertyChangeWithEvents ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord(_extensionMock.Object);

      var domainObjectMockEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, _order1);
      var propertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

      var sequence = new MockSequence();

      _extensionMock.Object.PropertyValueChanging(
            _subTransaction,
            _order1,
            propertyDefinition,
            oldOrderNumber,
            oldOrderNumber + 1);
      domainObjectMockEventReceiver.InSequence (sequence).Setup (_ => _.PropertyChanging (It.IsAny<object>(), It.IsAny<PropertyChangeEventArgs>())).Verifiable();
      domainObjectMockEventReceiver.InSequence (sequence).Setup (_ => _.PropertyChanged (It.IsAny<object>(), It.IsAny<PropertyChangeEventArgs>())).Verifiable();

      _extensionMock.Object.PropertyValueChanged(
            _subTransaction,
            _order1,
            propertyDefinition,
            oldOrderNumber,
            oldOrderNumber + 1);

      _order1.OrderNumber = oldOrderNumber + 1;

      _extensionMock.Verify();
      domainObjectMockEventReceiver.Verify();
    }

    [Test]
    public void GetRelatedObject ()
    {
      OrderTicket orderTicket = _order1.OrderTicket;

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.RelationReading(_subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), ValueAccess.Current);

      _extensionMock.Object.RelationRead(_subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), orderTicket, ValueAccess.Current);

      Dev.Null = _order1.OrderTicket;

      _extensionMock.Verify();
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      var originalOrderTicket = (OrderTicket)_order1.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.RelationReading(_subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), ValueAccess.Original);

      _extensionMock.Object.RelationRead(_subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), originalOrderTicket, ValueAccess.Original);

      Dev.Null = _order1.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

      _extensionMock.Verify();
    }

    [Test]
    public void GetRelatedObjects ()
    {
      DomainObjectCollection orderItems = _order1.OrderItems;
      orderItems.EnsureDataComplete();

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.RelationReading(
            _subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Current);

      _extensionMock.Expect (_ => _.RelationRead(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)),It.Is<DomainObject> (_ => object.ReferenceEquals (_, _order1)),It.Is<IRelationEndPointDefinition> (_ => object.Equals (_, GetEndPointDefinition(typeof(Order), "OrderItems"))),It.Is<IReadOnlyCollectionData<DomainObject>> (_ => _ != null && object.Equals (_.Count, 2) &&_.Contains (orderItems[0]) &&_.Contains (orderItems[1]) ),It.Is<ValueAccess> (_ => object.Equals (_, ValueAccess.Current))));

      Dev.Null = _order1.OrderItems;

      _extensionMock.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjects_ForDomainObjectCollection ()
    {
      DomainObjectCollection originalOrderItems =
          _order1.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.RelationReading(
            _subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Original);

      _extensionMock.Object.RelationRead(
            _subTransaction,
            _order1,
            GetEndPointDefinition(typeof(Order), "OrderItems"),
            Arg<IReadOnlyCollectionData<DomainObject>>.Matches(_ => _ != null && object.Equals (_.Count, 2) &&originalOrderItems.All (_.Contains) ),
            ValueAccess.Original);

      Dev.Null = _order1.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

      _extensionMock.Verify();
    }

    [Test]

    public void GetOriginalRelatedObjects_ForVirtualCollection ()
    {
      var originalProductReviews =
          _product1.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews");

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();

      _extensionMock.Object.RelationReading(
            _subTransaction, _product1, GetEndPointDefinition(typeof(Product), "Reviews"), ValueAccess.Original);

      _extensionMock.Object.RelationRead(
            _subTransaction,
            _product1,
            GetEndPointDefinition(typeof(Product), "Reviews"),
            It.Is<IReadOnlyCollectionData<DomainObject>> (_ => _ != null && object.Equals (_.Count, 3) &&originalProductReviews.All (_.Contains) ),
            ValueAccess.Original);

      Dev.Null = _product1.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews");

      _extensionMock.Verify();
    }

    [Test]
    public void GetRelatedObjectWithLazyLoad ()
    {
      var sequence = new MockSequence();
      _extensionMock.Object.RelationReading(
            _subTransaction,
            _order1,
            GetEndPointDefinition(typeof(Order), "OrderTicket"),
            ValueAccess.Current);
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
          _subTransaction.ParentTransaction,
          It.Is<ReadOnlyCollection<ObjectID>> (list => list.Count == 1))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction.ParentTransaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 1)))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction,
            It.Is<ReadOnlyCollection<ObjectID>> (list => list.Count == 1))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 1)))).Verifiable();
      _extensionMock.Expect (_ => _.RelationRead(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)),It.Is<DomainObject> (_ => object.ReferenceEquals (_, _order1)),It.Is<IRelationEndPointDefinition> (_ => object.Equals (_, GetEndPointDefinition(typeof(Order), "OrderTicket"))),It.Is<DomainObject> (_ => _ != null),It.Is<ValueAccess> (_ => object.Equals (_, ValueAccess.Current))));

      Dev.Null = _order1.OrderTicket;

      _extensionMock.Verify();
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad ()
    {
      var sequence = new MockSequence();
      _extensionMock.Object.RelationReading(
            _subTransaction,
            _order1,
            GetEndPointDefinition(typeof(Order), "OrderItems"),
            ValueAccess.Current);
      _extensionMock.Expect (_ => _.RelationRead(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)),It.Is<DomainObject> (_ => object.ReferenceEquals (_, _order1)),It.Is<IRelationEndPointDefinition> (_ => object.Equals (_, GetEndPointDefinition(typeof(Order), "OrderItems"))),It.Is<IReadOnlyCollectionData<DomainObject>> (_ => _ != null),It.Is<ValueAccess> (_ => object.Equals (_, ValueAccess.Current))));
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoading (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction.ParentTransaction)), It.Is<IReadOnlyList<ObjectID>> (_ => _ != null && object.Equals (_.Count, 2)))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction.ParentTransaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 2)))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoading (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)), It.Is<IReadOnlyList<ObjectID>> (_ => _ != null && object.Equals (_.Count, 2)))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 2)))).Verifiable();

      _order1.OrderItems.EnsureDataComplete();

      _extensionMock.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      var sequence = new MockSequence();
      _extensionMock.Object.RelationReading(
            _subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderTicket"), ValueAccess.Original);
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction.ParentTransaction,
            It.Is<ReadOnlyCollection<ObjectID>> (list => list.Count == 1))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction.ParentTransaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 1)))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction,
            It.Is<ReadOnlyCollection<ObjectID>> (list => list.Count == 1))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.ObjectsLoaded (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 1)))).Verifiable();
      _extensionMock.Expect (_ => _.RelationRead(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)),It.Is<DomainObject> (_ => object.ReferenceEquals (_, _order1)),It.Is<IRelationEndPointDefinition> (_ => object.Equals (_, GetEndPointDefinition(typeof(Order), "OrderTicket"))),It.Is<DomainObject> (_ => _ != null),It.Is<ValueAccess> (_ => object.Equals (_, ValueAccess.Original))));

      Dev.Null = _order1.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

      _extensionMock.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad_ForDomainObjectCollection ()
    {
      var sequence = new MockSequence();
      _extensionMock.Object.RelationReading(
            _subTransaction, _order1, GetEndPointDefinition(typeof(Order), "OrderItems"), ValueAccess.Original);
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction.ParentTransaction,
            It.Is<ReadOnlyCollection<ObjectID>> (list => list.Count == 2))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoaded (
            _subTransaction.ParentTransaction,
            It.Is<IReadOnlyList<DomainObject>> (list => list.Count == 2))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction,
            It.Is<ReadOnlyCollection<ObjectID>> (list => list.Count == 2))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoaded (
            _subTransaction,
            It.Is<IReadOnlyList<DomainObject>> (list => list.Count == 2))).Verifiable();
      _extensionMock.Object.RelationRead(
            _subTransaction,
            _order1,
            GetEndPointDefinition(typeof(Order), "OrderItems"),
            It.IsNotNull<IReadOnlyCollectionData<DomainObject>>(),
            ValueAccess.Original);

      Dev.Null = _order1.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

      _extensionMock.Verify();
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad_ForVirtualCollection ()
    {
      var sequence = new MockSequence();
      _extensionMock.Object.RelationReading(
            _subTransaction, _product1, GetEndPointDefinition(typeof(Product), "Reviews"), ValueAccess.Original);
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction.ParentTransaction,
            It.Is<ReadOnlyCollection<ObjectID>> (list => list.Count == 3))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoaded (
            _subTransaction.ParentTransaction,
            It.Is<IReadOnlyList<DomainObject>> (list => list.Count == 3))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction,
            It.Is<ReadOnlyCollection<ObjectID>> (list => list.Count == 3))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoaded (
            _subTransaction,
            It.Is<IReadOnlyList<DomainObject>> (list => list.Count == 3))).Verifiable();
      _extensionMock.Object.RelationRead(
            _subTransaction,
            _product1,
            GetEndPointDefinition(typeof(Product), "Reviews"),
            It.IsNotNull<IReadOnlyCollectionData<DomainObject>>(),
            ValueAccess.Original);

      Dev.Null = _product1.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews");

      _extensionMock.Verify();
    }

    [Test]
    public void FilterQueryResult ()
    {
      IQuery query = QueryFactory.CreateQueryFromConfiguration("OrderQuery");
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer1);

      // preload query results to avoid Load notifications later on
      LifetimeService.GetObject(_subTransaction, DomainObjectIDs.Order1, true);
      LifetimeService.GetObject(_subTransaction, DomainObjectIDs.Order2, true);

      _mockRepository.BackToRecord(_extensionMock.Object);

      QueryResult<DomainObject> parentFilteredQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject>(new[] { _order1 });
      QueryResult<DomainObject> subFilteredQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject>();

      _extensionMock
          .Setup(mock => mock.FilterQueryResult(
              _subTransaction.ParentTransaction,
              It.Is<QueryResult<DomainObject>> (qr => qr.Count == 2 && qr.Query == query)))
          .Returns(parentFilteredQueryResult)
          .Verifiable();
      _extensionMock
          .Setup(mock => mock.FilterQueryResult(
            _subTransaction,
            It.Is<QueryResult<DomainObject>> (qr => qr.Count == 1 && qr.Query == query)))
          .Returns(subFilteredQueryResult)
          .Verifiable();

      QueryResult<DomainObject> finalResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(query);
      Assert.That(finalResult, NUnit.Framework.Is.SameAs(subFilteredQueryResult));

      _extensionMock.Verify();
    }

    [Test]
    public void FilterQueryResultWithLoad ()
    {
      IQuery query = QueryFactory.CreateQueryFromConfiguration("OrderQuery");
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer4); // yields Order4, Order5

      QueryResult<DomainObject> parentFilteredQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject>(new[] { _order1 });
      QueryResult<DomainObject> subFilteredQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject>();

      UnloadService.UnloadData(_subTransaction, _order1.ID); // unload _order1 to force Load events
      ClientTransactionTestHelper.SetIsWriteable(TestableClientTransaction, true);
      TestableClientTransaction.EnsureDataAvailable(DomainObjectIDs.Order1); // we only want Load events in the sub-transaction
      ClientTransactionTestHelper.SetIsWriteable(TestableClientTransaction, false);

      _mockRepository.BackToRecordAll();

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction.ParentTransaction,
            It.Is<ReadOnlyCollection<ObjectID>> (list => list.SetEquals (new[] { DomainObjectIDs.Order4, DomainObjectIDs.Order5 })))).Verifiable();

      _extensionMock.Object.ObjectsLoaded(
            _subTransaction.ParentTransaction,
            It.Is<ReadOnlyCollection<DomainObject>> (list => list.Count == 2));
      _extensionMock.InSequence (sequence)
            .Setup(mock => mock.FilterQueryResult(
                _subTransaction.ParentTransaction,
                It.Is<QueryResult<DomainObject>> (qr => qr.Count == 2 && qr.Query == query)))
            .Returns(parentFilteredQueryResult)
            .Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction,
            It.Is<ReadOnlyCollection<ObjectID>> (list => list.SetEquals (new[] { DomainObjectIDs.Order1 })))).Verifiable();

      _extensionMock.Object.ObjectsLoaded(
            _subTransaction,
            It.Is<ReadOnlyCollection<DomainObject>> (list => list.Count == 1));
      _extensionMock.InSequence (sequence)
            .Setup(
            mock =>
            mock.FilterQueryResult(
                _subTransaction, It.Is<QueryResult<DomainObject>> (qr => qr.Count == 1 && qr.Query == query)))
            .Returns(subFilteredQueryResult)
            .Verifiable();

      QueryResult<DomainObject> finalQueryResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(query);
      Assert.That(finalQueryResult, NUnit.Framework.Is.SameAs(subFilteredQueryResult));

      _extensionMock.Verify();
    }

    [Test]
    public void CommitWithChangedPropertyValue ()
    {
      Computer computer = DomainObjectIDs.Computer4.GetObject<Computer>();
      computer.SerialNumber = "newSerialNumber";

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (
            mock =>
            mock.Committing (
                _subTransaction,
                new[] { computer },
                Arg<CommittingEventRegistrar>.Is.TypeOf)).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.CommitValidate (
            _subTransaction,
            It.Is<ReadOnlyCollection<PersistableData>> (c => c.Select (d => d.DomainObject).SetEquals (new[] { computer })))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.Committed (_subTransaction, new[] { computer })).Verifiable();

      _subTransaction.Commit();

      _extensionMock.Verify();
    }

    [Test]
    public void CommitWithChangedRelationValue ()
    {
      Computer computer = DomainObjectIDs.Computer4.GetObject<Computer>();
      Employee employee = DomainObjectIDs.Employee1.GetObject<Employee>();
      computer.Employee = employee;

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (
            mock =>
            mock.Committing (
                _subTransaction,
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new DomainObject[] { computer, employee }),
                Arg<CommittingEventRegistrar>.Is.TypeOf)).Verifiable();
      _extensionMock.InSequence (sequence).Setup (
            mock =>
            mock.CommitValidate (
                _subTransaction,
                It.Is<ReadOnlyCollection<PersistableData>> (c => c.Select (d => d.DomainObject).SetEquals (new DomainObject[] { computer, employee })))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (
            mock =>
            mock.Committed (_subTransaction, Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new DomainObject[] { computer, employee }))).Verifiable();

      _subTransaction.Commit();
      _extensionMock.Verify();
    }

    [Test]
    public void CommitWithChangedRelationValueWithClassIDColumn ()
    {
      Customer oldCustomer = _order1.Customer;
      Customer newCustomer = DomainObjectIDs.Customer2.GetObject<Customer>();
      _order1.Customer = newCustomer;

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (
            mock =>
            mock.Committing (
                _subTransaction,
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new DomainObject[] { _order1, newCustomer, oldCustomer }),
                Arg<CommittingEventRegistrar>.Is.TypeOf)).Verifiable();
      _extensionMock.InSequence (sequence).Setup (
            mock =>
            mock.CommitValidate (
                _subTransaction,
                It.Is<ReadOnlyCollection<PersistableData>> (c => c.Select (d => d.DomainObject).SetEquals (new DomainObject[] { _order1, newCustomer, oldCustomer })))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (
            mock =>
            mock.Committed (
                _subTransaction, Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new DomainObject[] { _order1, newCustomer, oldCustomer }))).Verifiable();

      _subTransactionScope.Commit();

      _extensionMock.Verify();
    }

    [Test]
    public void CommitWithEvents ()
    {
      SetDatabaseModifyable();

      Computer computer = DomainObjectIDs.Computer4.GetObject<Computer>();
      computer.SerialNumber = "newSerialNumber";

      _mockRepository.BackToRecord(_extensionMock.Object);

      var clientTransactionMockEventReceiver =
          new Mock<ClientTransactionMockEventReceiver> (MockBehavior.Strict, _subTransaction);

      var computerEventReveiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, computer);

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (
            mock =>
            mock.Committing (
                _subTransaction,
                new[] { computer },
                Arg<CommittingEventRegistrar>.Is.TypeOf)).Verifiable();
      clientTransactionMockEventReceiver.InSequence (sequence).Setup (mock => mock.Committing (computer)).Verifiable();
      computerEventReveiver.InSequence (sequence).Setup (mock => mock.Committing()).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.CommitValidate (
            _subTransaction,
            It.Is<ReadOnlyCollection<PersistableData>> (c => c.Select (d => d.DomainObject).SetEquals (new[] { computer })))).Verifiable();
      computerEventReveiver.InSequence (sequence).Setup (mock => mock.Committed()).Verifiable();
      clientTransactionMockEventReceiver.InSequence (sequence).Setup (mock => mock.Committed (computer)).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.Committed (_subTransaction, new[] { computer })).Verifiable();

      _subTransaction.Commit();

      _extensionMock.Verify();
      clientTransactionMockEventReceiver.Verify();
      computerEventReveiver.Verify();
    }

    [Test]
    public void Rollback ()
    {
      Computer computer = DomainObjectIDs.Computer4.GetObject<Computer>();
      computer.SerialNumber = "newSerialNumber";

      _mockRepository.BackToRecord(_extensionMock.Object);

      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (_ => _.RollingBack (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _.Contains (computer)))).Verifiable();
      _extensionMock.InSequence (sequence).Setup (_ => _.RolledBack (It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)), It.Is<IReadOnlyList<DomainObject>> (_ => _.Contains (computer)))).Verifiable();

      ClientTransactionScope.CurrentTransaction.Rollback();

      _extensionMock.Verify();
    }

    [Test]
    public void GetObjects ()
    {
      var sequence = new MockSequence();
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction.ParentTransaction,
            new[] { DomainObjectIDs.Order3, DomainObjectIDs.Order4 })).Verifiable();
      _extensionMock.Expect (_ => _.ObjectsLoaded(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction.ParentTransaction)),Arg<IReadOnlyList<DomainObject>>.Matches(_ => Mocks_List.Count(Mocks_Is.Equal(2)))));
      _extensionMock.InSequence (sequence).Setup (mock => mock.ObjectsLoading (
            _subTransaction,
            new[] { DomainObjectIDs.Order3, DomainObjectIDs.Order4 })).Verifiable();
      _extensionMock.Expect (_ => _.ObjectsLoaded(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, _subTransaction)),Arg<IReadOnlyList<DomainObject>>.Matches(_ => Mocks_List.Count(Mocks_Is.Equal(2)))));

      using (_subTransaction.EnterNonDiscardingScope())
      {
        LifetimeService.GetObjects<DomainObject>(_subTransaction, DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4);
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
                        _subTransaction,
                        new[] { _order1 }))
            .Callback((ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects) => Assert.That(_subTransactionDataManager.DataContainers[_order1.ID] != null))
            .Verifiable();
      _extensionMock
            .InSequence (sequence)
            .Setup(mock => mock.ObjectsUnloading(
                        _subTransaction.ParentTransaction,
                        new[] { _order1 }))
            .Callback((ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects) => Assert.That(_parentTransactionDataManager.DataContainers[_order1.ID] != null))
            .Verifiable();
      _extensionMock
            .InSequence (sequence)
            .Setup(mock => mock.ObjectsUnloaded(
                        _subTransaction.ParentTransaction,
                        new[] { _order1 }))
            .Callback((ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects) => Assert.That(_parentTransactionDataManager.DataContainers[_order1.ID] == null))
            .Verifiable();
      _extensionMock
            .InSequence (sequence)
            .Setup(mock => mock.ObjectsUnloaded(
                        _subTransaction,
                        new[] { _order1 }))
            .Callback((ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects) => Assert.That(_subTransactionDataManager.DataContainers[_order1.ID] == null))
            .Verifiable();

      UnloadService.UnloadData(_subTransaction, _order1.ID);

      _extensionMock.Verify();
    }

  }
}
