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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class ClientTransactionListenerTest : ClientTransactionBaseTest
  {
    private Mock<IClientTransactionListener> _strictListenerMock;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _strictListenerMock = new Mock<IClientTransactionListener>(MockBehavior.Strict);
    }

    [TearDown]
    public override void TearDown ()
    {
      _strictListenerMock.Reset();
      base.TearDown();
    }

    [Test]
    public void TransactionInitialize ()
    {
      ClientTransaction inititalizedTransaction = null;

      _strictListenerMock
          .Setup(mock => mock.TransactionInitialize(It.IsAny<ClientTransaction>()))
          .Callback((ClientTransaction clientTransaction) => inititalizedTransaction = clientTransaction)
          .Verifiable();

      var result = ClientTransactionObjectMother.CreateWithCustomListeners(_strictListenerMock.Object);

      _strictListenerMock.Verify();

      Assert.That(result, Is.SameAs(inititalizedTransaction));
    }

    [Test]
    public void TransactionDiscard ()
    {
      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      _strictListenerMock.Setup(mock => mock.TransactionDiscard(TestableClientTransaction)).Verifiable();

      TestableClientTransaction.Discard();

      _strictListenerMock.Verify();
    }

    [Test]
    public void TransactionDiscard_OnlyFiresIfTransactionIsNotYetDiscarded ()
    {
      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      _strictListenerMock.Setup(mock => mock.TransactionDiscard(TestableClientTransaction)).Verifiable();

      TestableClientTransaction.Discard();
      TestableClientTransaction.Discard();

      _strictListenerMock.Verify();
    }

    [Test]
    public void NewObjectCreating ()
    {
      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      var sequence = new VerifiableSequence();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.NewObjectCreating(TestableClientTransaction, typeof(ClassWithAllDataTypes)))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.DataContainerMapRegistering(TestableClientTransaction, It.IsAny<DataContainer>()))
          .Verifiable();

      ClassWithAllDataTypes.NewObject();

      _strictListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectsLoadingInitializedObjectsLoaded ()
    {
      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      var sequence = new VerifiableSequence();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(TestableClientTransaction, new[] { DomainObjectIDs.ClassWithAllDataTypes1 }))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.DataContainerMapRegistering(TestableClientTransaction, It.IsAny<DataContainer>())).Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(TestableClientTransaction, It.Is<ReadOnlyCollection<DomainObject>>(doc => doc.Count == 1)))
          .Verifiable();

      DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();

      _strictListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ObjectsObjectDeletingObjectsDeleted ()
    {
      ClassWithAllDataTypes cwadt = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      var sequence = new VerifiableSequence();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectDeleting(TestableClientTransaction, cwadt))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.DataContainerStateUpdated(TestableClientTransaction, cwadt.InternalDataContainer, new DataContainerState.Builder().SetDeleted().Value))
          .Verifiable();
      _strictListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleted(TestableClientTransaction, cwadt)).Verifiable();

      cwadt.Delete();

      _strictListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void PropertyValueReadingPropertyValueRead ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      int orderNumber = order.OrderNumber;

      TestableClientTransaction.AddListener(_strictListenerMock.Object);
      var orderNumberPropertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

      var sequence = new VerifiableSequence();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueReading(TestableClientTransaction, order, orderNumberPropertyDefinition, ValueAccess.Current))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueRead(TestableClientTransaction, order, orderNumberPropertyDefinition, orderNumber, ValueAccess.Current))
          .Verifiable();

      Dev.Null = order.OrderNumber;

      _strictListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void PropertyValueChangingPropertyValueChanged ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      int orderNumber = order.OrderNumber;

      TestableClientTransaction.AddListener(_strictListenerMock.Object);
      var orderNumberPropertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");

      var sequence = new VerifiableSequence();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueChanging(TestableClientTransaction, order, orderNumberPropertyDefinition, orderNumber, 43))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.DataContainerStateUpdated(
                  TestableClientTransaction,
                  order.InternalDataContainer,
                  new DataContainerState.Builder().SetChanged().SetPersistentDataChanged().Value))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.PropertyValueChanged(TestableClientTransaction, order, orderNumberPropertyDefinition, orderNumber, 43))
          .Verifiable();

      order.OrderNumber = 43;

      _strictListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void RelationReadingRelationRead ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      Customer customer = order.Customer;
      ObjectList<OrderItem> orderItems = order.OrderItems;
      orderItems.EnsureDataComplete();

      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      IRelationEndPointDefinition customerEndPointDefinition = GetEndPointDefinition(typeof(Order), "Customer");
      IRelationEndPointDefinition orderItemsEndPointDefinition = GetEndPointDefinition(typeof(Order), "OrderItems");

      var sequence = new VerifiableSequence();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(TestableClientTransaction, order, customerEndPointDefinition, ValueAccess.Current))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationRead(TestableClientTransaction, order, customerEndPointDefinition, customer, ValueAccess.Current))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationReading(TestableClientTransaction, order, orderItemsEndPointDefinition, ValueAccess.Current))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationRead(
                  TestableClientTransaction,
                  order,
                  orderItemsEndPointDefinition,
                  It.Is<IReadOnlyCollectionData<DomainObject>>(domainObjects => domainObjects.SequenceEqual(orderItems.Cast<DomainObject>())),
                  ValueAccess.Current))
          .Verifiable();

      Dev.Null = order.Customer;
      Dev.Null = order.OrderItems;

      _strictListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void RelationChangingRelationChanged ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      Customer oldCustomer = order.Customer;
      Customer newCustomer = Customer.NewObject();

      // preload all related objects
      oldCustomer.Orders.EnsureDataComplete();

      var oldCustomerEndPointID = oldCustomer.Orders.AssociatedEndPointID;
      var newCustomerEndPointID = newCustomer.Orders.AssociatedEndPointID;

      IRelationEndPointDefinition customerEndPointDefinition = GetEndPointDefinition(typeof(Order), "Customer");

      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      var sequence = new VerifiableSequence();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanging(TestableClientTransaction, order, customerEndPointDefinition, oldCustomer, newCustomer))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanging(TestableClientTransaction, newCustomer, newCustomerEndPointID.Definition, null, order))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanging(TestableClientTransaction, oldCustomer, oldCustomerEndPointID.Definition, order, null))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.DataContainerStateUpdated(
                  TestableClientTransaction,
                  order.InternalDataContainer,
                  new DataContainerState.Builder().SetChanged().SetPersistentDataChanged().Value))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.VirtualRelationEndPointStateUpdated(TestableClientTransaction, newCustomerEndPointID, null))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.VirtualRelationEndPointStateUpdated(TestableClientTransaction, oldCustomerEndPointID, null))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanged(TestableClientTransaction, oldCustomer, oldCustomerEndPointID.Definition, order, null))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanged(TestableClientTransaction, newCustomer, newCustomerEndPointID.Definition, null, order))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationChanged(TestableClientTransaction, order, customerEndPointDefinition, oldCustomer, newCustomer))
          .Verifiable();

      order.Customer = newCustomer;

      _strictListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void FilterQueryResult ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration("StoredProcedureQuery");
      var orders = (OrderCollection)TestableClientTransaction.QueryManager.GetCollection(query).ToCustomCollection();

      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      var newQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject>();
      _strictListenerMock
          .Setup(mock => mock.FilterQueryResult(TestableClientTransaction, It.Is<QueryResult<DomainObject>>(qr => qr.Count == orders.Count)))
          .Returns(newQueryResult)
          .Verifiable();

      var result = TestableClientTransaction.QueryManager.GetCollection(query);
      Assert.That(result, Is.SameAs(newQueryResult));

      _strictListenerMock.Verify();
    }

    [Test]
    public void FilterCustomQueryResult ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration("CustomQueryReadOnly");

      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      var newQueryResult = new[] { new object(), new object() };
      _strictListenerMock
          .Setup(
              mock => mock.FilterCustomQueryResult(
                  TestableClientTransaction,
                  query,
                  It.Is<IEnumerable<object>>(qr => qr.SetEquals(new[] { "abcdeföäü", "üäöfedcba" }))))
          .Returns(newQueryResult)
          .Verifiable();

      var result = TestableClientTransaction.QueryManager.GetCustom(query, rr => rr.GetRawValue(0));
      Assert.That(result, Is.SameAs(newQueryResult));

      _strictListenerMock.Verify();
    }

    [Test]
    public void TransactionCommittingTransactionCommitted ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      ++order.OrderNumber;

      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      var sequence = new VerifiableSequence();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.TransactionCommitting(
                  TestableClientTransaction,
                  It.Is<ReadOnlyCollection<DomainObject>>(p => p.SetEquals(new[] { order })),
                  It.IsNotNull<CommittingEventRegistrar>()))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.TransactionCommitValidate(
                  TestableClientTransaction,
                  It.Is<ReadOnlyCollection<PersistableData>>(c => c.Select(d => d.DomainObject).SetEquals(new[] { order }))))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.DataContainerStateUpdated(TestableClientTransaction, order.InternalDataContainer, new DataContainerState.Builder().SetUnchanged().Value))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.TransactionCommitted(TestableClientTransaction, It.Is<ReadOnlyCollection<DomainObject>>(p => p.SetEquals(new[] { order }))))
          .Verifiable();

      TestableClientTransaction.Commit();

      _strictListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void TransactionRollingBackTransactionRolledBack ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      ++order.OrderNumber;

      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      var sequence = new VerifiableSequence();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.TransactionRollingBack(TestableClientTransaction, It.Is<ReadOnlyCollection<DomainObject>>(doc => doc.Count == 1)))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.DataContainerStateUpdated(
                  TestableClientTransaction,
                  order.InternalDataContainer,
                  new DataContainerState.Builder().SetUnchanged().Value))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.TransactionRolledBack(TestableClientTransaction, It.Is<ReadOnlyCollection<DomainObject>>(doc => doc.Count == 1)))
          .Verifiable();

      TestableClientTransaction.Rollback();

      _strictListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void RelationEndPointMapRegistering ()
    {
      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      var sequence = new VerifiableSequence();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(TestableClientTransaction, It.IsAny<ReadOnlyCollection<ObjectID>>()))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.DataContainerMapRegistering(TestableClientTransaction, It.IsAny<DataContainer>()))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationEndPointMapRegistering(
                  TestableClientTransaction,
                  It.Is<IRelationEndPoint>(
                      rep => rep.Definition.PropertyName == typeof(Company).FullName + ".IndustrialSector"
                             && rep.ObjectID == DomainObjectIDs.Customer1)))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.RelationEndPointMapRegistering(
                  TestableClientTransaction,
                  It.Is<IRelationEndPoint>(
                      rep => rep.Definition.PropertyName == typeof(IndustrialSector).FullName + ".Companies"
                             && rep.ObjectID == DomainObjectIDs.IndustrialSector1)))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(TestableClientTransaction, It.IsAny<ReadOnlyCollection<DomainObject>>()))
          .Verifiable();

      DomainObjectIDs.Customer1.GetObject<Customer>();

      _strictListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void RelationEndPointMapUnregisteringDataManagerMarkingObjectDiscardedDataContainerMapUnregistering ()
    {
      Order order = Order.NewObject();
      var orderTicketEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID(order.ID, "OrderTicket");
      var orderItemEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID(order.ID, "OrderItems");

      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      var sequence = new VerifiableSequence();
      _strictListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleting(TestableClientTransaction, order)).Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.VirtualRelationEndPointStateUpdated(TestableClientTransaction, orderTicketEndPointID, false))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.VirtualRelationEndPointStateUpdated(TestableClientTransaction, orderItemEndPointID, false))
          .Verifiable();

      // four related objects/object collections in Order
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationEndPointMapUnregistering(TestableClientTransaction, It.Is<RelationEndPointID>(id => id.ObjectID == order.ID)))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationEndPointMapUnregistering(TestableClientTransaction, It.Is<RelationEndPointID>(id => id.ObjectID == order.ID)))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationEndPointMapUnregistering(TestableClientTransaction, It.Is<RelationEndPointID>(id => id.ObjectID == order.ID)))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RelationEndPointMapUnregistering(TestableClientTransaction, It.Is<RelationEndPointID>(id => id.ObjectID == order.ID)))
          .Verifiable();

      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.DataContainerMapUnregistering(TestableClientTransaction, order.InternalDataContainer))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.DataContainerStateUpdated(TestableClientTransaction, order.InternalDataContainer, new DataContainerState.Builder().SetDiscarded().Value))
          .Verifiable();
      _strictListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectMarkedInvalid(TestableClientTransaction, order)).Verifiable();
      _strictListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.ObjectDeleted(TestableClientTransaction, order)).Verifiable();

      order.Delete();

      _strictListenerMock.Verify(
          mock => mock.RelationEndPointMapUnregistering(
              TestableClientTransaction,
              It.Is<RelationEndPointID>(id => id.ObjectID == order.ID)),
          Times.Exactly(4));
      sequence.Verify();
    }

    [Test]
    public void DataContainerMapRegistering ()
    {
      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      var sequence = new VerifiableSequence();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoading(TestableClientTransaction, new[] { DomainObjectIDs.ClassWithAllDataTypes1 }))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.DataContainerMapRegistering(TestableClientTransaction, It.Is<DataContainer>(dc => dc.ID == DomainObjectIDs.ClassWithAllDataTypes1)))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsLoaded(TestableClientTransaction, It.IsAny<ReadOnlyCollection<DomainObject>>()))
          .Verifiable();

      DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();

      _strictListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void SubTransactionCreating_AndSubTransactionCreated ()
    {
      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      ClientTransaction initializedTransaction = null;

      var sequence = new VerifiableSequence();
      _strictListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.SubTransactionCreating(TestableClientTransaction)).Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.SubTransactionInitialize(
                  TestableClientTransaction,
                  It.Is<ClientTransaction>(tx => tx != null && tx != TestableClientTransaction && tx.ParentTransaction == TestableClientTransaction)))
          .Callback((ClientTransaction _, ClientTransaction subTransaction) => initializedTransaction = subTransaction)
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.SubTransactionCreated(TestableClientTransaction, It.Is<ClientTransaction>(tx => tx == initializedTransaction)))
          .Verifiable();

      var result = TestableClientTransaction.CreateSubTransaction();

      _strictListenerMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.SameAs(initializedTransaction));
    }

    [Test]
    public void SubTransactionInitialize_AndTransactionInitialize_AndSubTransactionCreated ()
    {
      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      ClientTransaction initializedTransaction = null;

      var sequence = new VerifiableSequence();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.SubTransactionCreating(TestableClientTransaction))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.SubTransactionInitialize(
                  TestableClientTransaction,
                  It.Is<ClientTransaction>(tx => tx != null && tx != TestableClientTransaction && tx.ParentTransaction == TestableClientTransaction)))
          .Callback(
              (ClientTransaction _, ClientTransaction subTransaction) =>
              {
                initializedTransaction = subTransaction;
                ClientTransactionTestHelper.AddListener(initializedTransaction, _strictListenerMock.Object);
              })
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.TransactionInitialize(It.Is<ClientTransaction>(tx => tx == initializedTransaction)))
          .Verifiable();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.SubTransactionCreated(TestableClientTransaction, It.Is<ClientTransaction>(tx => tx == initializedTransaction)))
          .Verifiable();

      var result = TestableClientTransaction.CreateSubTransaction();

      _strictListenerMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.SameAs(initializedTransaction));
    }

    [Test]
    public void ObjectsUnloadingObjectsUnloaded ()
    {
      var orderTicket1 = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();

      var orderEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID(orderTicket1.ID, "Order");
      var orderTicketEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID(orderTicket1.Order.ID, "OrderTicket");

      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      var sequence = new VerifiableSequence();
      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsUnloading(TestableClientTransaction, new[] { orderTicket1 }))
          .Callback((ClientTransaction _, IReadOnlyList<DomainObject> _) => Assert.That(orderTicket1.State.IsUnchanged, Is.True))
          .Verifiable();

      _strictListenerMock
          .Setup(mock => mock.RelationEndPointMapUnregistering(TestableClientTransaction, orderEndPointID))
          .Verifiable();
      _strictListenerMock
          .Setup(mock => mock.RelationEndPointMapUnregistering(TestableClientTransaction, orderTicketEndPointID))
          .Verifiable();
      _strictListenerMock
          .Setup(mock => mock.RelationEndPointBecomingIncomplete(TestableClientTransaction, orderTicketEndPointID))
          .Verifiable();
      _strictListenerMock
          .Setup(mock => mock.DataContainerMapUnregistering(TestableClientTransaction, orderTicket1.InternalDataContainer))
          .Verifiable();

      _strictListenerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ObjectsUnloaded(TestableClientTransaction, new[] { orderTicket1 }))
          .Callback((ClientTransaction _, IReadOnlyList<DomainObject> _) => Assert.That(orderTicket1.State.IsNotLoadedYet, Is.True))
          .Verifiable();

      UnloadService.UnloadData(TestableClientTransaction, orderTicket1.ID);

      _strictListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void RelationEndPointUnload_ForDomainObjectCollection ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(order1.OrderItems);
      orderItemsEndPoint.EnsureDataComplete();

      Dev.Null = orderItemsEndPoint.HasChanged; // warm up has changed cache

      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      var sequence = new VerifiableSequence();
      _strictListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.RelationEndPointBecomingIncomplete(TestableClientTransaction, orderItemsEndPoint.ID)).Verifiable();

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, orderItemsEndPoint.ID);

      _strictListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void RelationEndPointUnload_ForVirtualCollection ()
    {
      var product1 = DomainObjectIDs.Product1.GetObject<Product>();
      var productReviewsEndPoint = VirtualCollectionDataTestHelper.GetAssociatedEndPoint(product1.Reviews);
      productReviewsEndPoint.EnsureDataComplete();

      Dev.Null = productReviewsEndPoint.HasChanged; // warm up has changed cache

      TestableClientTransaction.AddListener(_strictListenerMock.Object);

      var sequence = new VerifiableSequence();
      _strictListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.RelationEndPointBecomingIncomplete(TestableClientTransaction, productReviewsEndPoint.ID)).Verifiable();
      _strictListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.RelationEndPointMapUnregistering(TestableClientTransaction, productReviewsEndPoint.ID)).Verifiable();

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, productReviewsEndPoint.ID);

      _strictListenerMock.Verify();
      sequence.Verify();
    }
  }
}
