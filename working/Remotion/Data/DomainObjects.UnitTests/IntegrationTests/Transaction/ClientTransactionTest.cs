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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class ClientTransactionTest : ClientTransactionBaseTest
  {
    private enum ApplicationDataKey
    {
      Key1 = 0
    }

    private ClientTransactionEventReceiver _eventReceiver;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _eventReceiver = new ClientTransactionEventReceiver (TestableClientTransaction);
    }

    public override void TearDown ()
    {
      base.TearDown ();

      _eventReceiver.Unregister ();
    }

    [Test]
    public void ParentTransaction ()
    {
      Assert.That (ClientTransaction.Current.ParentTransaction, Is.Null);
    }

    [Test]
    public void ActiveSubTransaction ()
    {
      Assert.That (ClientTransaction.Current.SubTransaction, Is.Null);
    }

    [Test]
    public void RootTransaction ()
    {
      Assert.That (TestableClientTransaction.RootTransaction, Is.SameAs (TestableClientTransaction));
    }

    [Test]
    public void LeafTransaction ()
    {
      Assert.That (TestableClientTransaction.LeafTransaction, Is.SameAs (TestableClientTransaction));
    }

    [Test]
    public void CommitTwice ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      OrderTicket oldOrderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ();
      OrderTicket newOrderTicket = DomainObjectIDs.OrderTicket2.GetObject<OrderTicket> ();

      oldOrderTicket.Order = newOrderTicket.Order;
      order.OrderTicket = newOrderTicket;

      TestableClientTransaction.Commit ();

      Assert.That (TestableClientTransaction.IsDiscarded, Is.False);

      object orderTimestamp = order.InternalDataContainer.Timestamp;
      object oldOrderTicketTimestamp = oldOrderTicket.InternalDataContainer.Timestamp;
      object newOrderTicketTimestamp = newOrderTicket.InternalDataContainer.Timestamp;

      TestableClientTransaction.Commit ();

      Assert.That (TestableClientTransaction.IsDiscarded, Is.False);

      Assert.That (order.InternalDataContainer.Timestamp, Is.EqualTo (orderTimestamp));
      Assert.That (oldOrderTicket.InternalDataContainer.Timestamp, Is.EqualTo (oldOrderTicketTimestamp));
      Assert.That (newOrderTicket.InternalDataContainer.Timestamp, Is.EqualTo (newOrderTicketTimestamp));
    }

    [Test]
    public void CommitTwiceWithChange ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      OrderTicket oldOrderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ();
      OrderTicket newOrderTicket = DomainObjectIDs.OrderTicket2.GetObject<OrderTicket> ();
      Order oldOrderOfNewOrderTicket = DomainObjectIDs.Order2.GetObject<Order> ();

      oldOrderTicket.Order = newOrderTicket.Order;
      order.OrderTicket = newOrderTicket;

      TestableClientTransaction.Commit ();

      object orderTimestamp = order.InternalDataContainer.Timestamp;
      object oldOrderTicketTimestamp = oldOrderTicket.InternalDataContainer.Timestamp;
      object newOrderTicketTimestamp = newOrderTicket.InternalDataContainer.Timestamp;
      object oldOrderOfNewOrderTicketTimestamp = oldOrderOfNewOrderTicket.InternalDataContainer.Timestamp;

      order.OrderTicket = oldOrderTicket;
      oldOrderOfNewOrderTicket.OrderTicket = newOrderTicket;

      TestableClientTransaction.Commit ();

      Assert.That (order.InternalDataContainer.Timestamp, Is.EqualTo (orderTimestamp));
      Assert.That (oldOrderTicketTimestamp.Equals (oldOrderTicket.InternalDataContainer.Timestamp), Is.False);
      Assert.That (newOrderTicketTimestamp.Equals (newOrderTicket.InternalDataContainer.Timestamp), Is.False);
      Assert.That (oldOrderOfNewOrderTicket.InternalDataContainer.Timestamp, Is.EqualTo (oldOrderOfNewOrderTicketTimestamp));
    }

    [Test]
    public void OppositeDomainObjectsTypeAfterCommit ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer> ();

      customer.Orders.Add (DomainObjectIDs.Order3.GetObject<Order> ());
      TestableClientTransaction.Commit ();

      DomainObjectCollection originalOrders = customer.GetOriginalRelatedObjects ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");
      Assert.That (originalOrders.GetType(), Is.EqualTo (typeof (OrderCollection)));
      Assert.That (originalOrders.IsReadOnly, Is.True);

      Assert.That (originalOrders.RequiredItemType, Is.Null);
    }

    [Test]
    public void RollbackReadOnlyOppositeDomainObjects ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer> ();
      customer.Orders.Add (DomainObjectIDs.Order3.GetObject<Order> ());

      DomainObjectCollectionDataTestHelper.MakeCollectionReadOnly (customer.Orders);
      TestableClientTransaction.Rollback ();

      Assert.That (customer.GetOriginalRelatedObjects ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders").IsReadOnly, Is.True);
      Assert.That (customer.Orders.IsReadOnly, Is.True);
    }

    [Test]
    public void CommitDeletedObject ()
    {
      Computer computer = DomainObjectIDs.Computer1.GetObject<Computer> ();
      computer.Delete ();
      TestableClientTransaction.Commit ();

      Assert.That (() => DomainObjectIDs.Computer1.GetObject<Computer> (), Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void AccessDeletedObjectAfterCommit ()
    {
      Computer computer = DomainObjectIDs.Computer1.GetObject<Computer> ();
      computer.Delete ();
      TestableClientTransaction.Commit ();

      Assert.That (() => Dev.Null = computer.SerialNumber, Throws.TypeOf<ObjectInvalidException> ());
    }

    [Test]
    public void CommitIndependentTransactions ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.CreateRootTransaction();
      ClientTransaction clientTransaction2 = ClientTransaction.CreateRootTransaction();

      Order order1;
      using (clientTransaction1.EnterNonDiscardingScope())
      {
        order1 = DomainObjectIDs.Order1.GetObject<Order> ();
        order1.OrderNumber = 50;
      }

      Order order3;
      using (clientTransaction2.EnterNonDiscardingScope ())
      {
        order3 = DomainObjectIDs.Order3.GetObject<Order> ();
        order3.OrderNumber = 60;
      }

      clientTransaction1.Commit ();
      clientTransaction2.Commit ();

      ClientTransaction clientTransaction3 = ClientTransaction.CreateRootTransaction();
      using (clientTransaction3.EnterNonDiscardingScope ())
      {
        Order changedOrder1 = DomainObjectIDs.Order1.GetObject<Order> ();
        Order changedOrder2 = DomainObjectIDs.Order3.GetObject<Order> ();

        Assert.That (ReferenceEquals (order1, changedOrder1), Is.False);
        Assert.That (ReferenceEquals (order3, changedOrder2), Is.False);

        Assert.That (changedOrder1.OrderNumber, Is.EqualTo (50));
        Assert.That (changedOrder2.OrderNumber, Is.EqualTo (60));
      }
    }

    [Test]
    public void QueryManager ()
    {
      Assert.That (TestableClientTransaction.QueryManager, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void NoAutoInitializationOfCurrent ()
    {
      using (ClientTransactionScope.EnterNullScope())
      {
        Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.False);
        Dev.Null = ClientTransactionScope.CurrentTransaction;
      }
    }

    [Test]
    public void HasCurrentTrue ()
    {
      Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.True);
    }

    [Test]
    public void HasCurrentFalseViaNullTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope())
      {
        Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.False);
      }
    }

    [Test]
    public void HasCurrentFalseViaNullScope ()
    {
      ClientTransactionScope.ResetActiveScope ();
      Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.False);
    }

    [Test]
    public void ClientTransactionCurrentIdenticalToScopeCurrentButNullOnEmptyScope ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.CreateRootTransaction ();
      ClientTransaction clientTransaction2 = ClientTransaction.CreateRootTransaction ();

      Assert.That (ClientTransaction.Current, Is.SameAs (ClientTransactionScope.CurrentTransaction));

      using (clientTransaction1.EnterDiscardingScope ())
      {
        Assert.That (ClientTransaction.Current, Is.SameAs (ClientTransactionScope.CurrentTransaction));
        using (clientTransaction2.EnterDiscardingScope ())
        {
          Assert.That (ClientTransaction.Current, Is.SameAs (ClientTransactionScope.CurrentTransaction));
        }
        Assert.That (ClientTransaction.Current, Is.SameAs (ClientTransactionScope.CurrentTransaction));
      }
      Assert.That (ClientTransaction.Current, Is.SameAs (ClientTransactionScope.CurrentTransaction));

      using (ClientTransactionScope.EnterNullScope ())
      {
        Assert.That (ClientTransaction.Current, Is.Null);
      }
    }

    [Test]
    public void HasChanged ()
    {
      Assert.That (ClientTransactionScope.CurrentTransaction.HasChanged(), Is.False);
      Order order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      order1.OrderNumber = order1.OrderNumber + 1;
      Assert.That (ClientTransactionScope.CurrentTransaction.HasChanged(), Is.True);
    }

    [Test]
    public void ApplicationData ()
    {
      Assert.That (ClientTransactionScope.CurrentTransaction.ApplicationData, Is.Not.Null);
      Assert.IsAssignableFrom (typeof (Dictionary<Enum, object>), ClientTransactionScope.CurrentTransaction.ApplicationData);

      Assert.That (ClientTransactionScope.CurrentTransaction.ApplicationData.ContainsKey (ApplicationDataKey.Key1), Is.False);
      ClientTransactionScope.CurrentTransaction.ApplicationData[ApplicationDataKey.Key1] = "TestData";
      Assert.That (ClientTransactionScope.CurrentTransaction.ApplicationData[ApplicationDataKey.Key1], Is.EqualTo ("TestData"));
      ClientTransactionScope.CurrentTransaction.ApplicationData.Remove (ApplicationDataKey.Key1);
      Assert.That (ClientTransactionScope.CurrentTransaction.ApplicationData.ContainsKey (ApplicationDataKey.Key1), Is.False);
    }

    [Test]
    public void ClientTransactionEventsTriggeredInRightTransaction ()
    {
      var mock = new TestableClientTransaction();
      int events = 0;
// ReSharper disable AccessToModifiedClosure
      mock.Committed += delegate { ++events;
                                   Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (mock));
      };
      mock.Committing += delegate { ++events;
                                    Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (mock));
      };
      mock.Loaded += delegate { ++events;
                                Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (mock));
      };
      mock.RolledBack += delegate { ++events;
                                    Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (mock));
      };
      mock.RollingBack += delegate { ++events;
                                     Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (mock));
      };
// ReSharper restore AccessToModifiedClosure

      Assert.That (events, Is.EqualTo (0));
      mock.GetObject (DomainObjectIDs.Order1, false);
      Assert.That (events, Is.EqualTo (1)); // loaded

      events = 0;
      mock.Commit ();
      Assert.That (events, Is.EqualTo (2)); // committing, committed

      events = 0;
      mock.Rollback ();
      Assert.That (events, Is.EqualTo (2)); // rollingback, rolledback
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The transaction can no longer be used because it has been discarded.")]
    public void DiscardRendersTransactionUnusable ()
    {
      TestableClientTransaction.Discard ();
      Assert.That (TestableClientTransaction.IsDiscarded, Is.True);
      TestableClientTransaction.GetObject (DomainObjectIDs.Order1, false);
    }

    [Test]
    public void DefaultEnterScope ()
    {
      ClientTransactionScope outerScope = ClientTransactionScope.ActiveScope;
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      using (newTransaction.EnterDiscardingScope ())
      {
        Assert.That (ClientTransactionScope.ActiveScope, Is.Not.SameAs (outerScope));
        Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (newTransaction));
        Assert.That (ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.Discard));
      }
    }

    [Test]
    public void EnterScopeWithRollbackBehavior ()
    {
      ClientTransactionScope outerScope = ClientTransactionScope.ActiveScope;
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      using (newTransaction.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Assert.That (ClientTransactionScope.ActiveScope, Is.Not.SameAs (outerScope));
        Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (newTransaction));
        Assert.That (ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.Rollback));
      }

      using (newTransaction.EnterScope (AutoRollbackBehavior.None))
      {
        Assert.That (ClientTransactionScope.ActiveScope, Is.Not.SameAs (outerScope));
        Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (newTransaction));
        Assert.That (ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.None));
      }
    }

    [Test]
    public void EnterNonDiscardingScope ()
    {
      ClientTransactionScope outerScope = ClientTransactionScope.ActiveScope;
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      using (newTransaction.EnterNonDiscardingScope ())
      {
        Assert.That (ClientTransactionScope.ActiveScope, Is.Not.SameAs (outerScope));
        Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (newTransaction));
        Assert.That (ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.None));
      }
    }

    [Test]
    [Obsolete ("TODO 2072 - Remove")]
    public void CopyCollectionEventHandlers ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      DomainObjectCollectionChangeEventHandler addedEventHandler = delegate { };
      DomainObjectCollectionChangeEventHandler addingEventHandler = delegate { };
      DomainObjectCollectionChangeEventHandler removedEventHandler = delegate { };
      DomainObjectCollectionChangeEventHandler removingEventHandler = delegate { };

      order.OrderItems.Added += addedEventHandler;
      order.OrderItems.Adding += addingEventHandler;
      order.OrderItems.Removed += removedEventHandler;
      order.OrderItems.Removing += removingEventHandler;

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.CopyCollectionEventHandlers (order, TestableClientTransaction);

        var orderInThisTransaction = order.GetHandle ().GetObject ();

        Assert.That (HasEventHandler (orderInThisTransaction.OrderItems, "Added", addedEventHandler), Is.True);
        Assert.That (HasEventHandler (orderInThisTransaction.OrderItems, "Adding", addingEventHandler), Is.True);
        Assert.That (HasEventHandler (orderInThisTransaction.OrderItems, "Removed", removedEventHandler), Is.True);
        Assert.That (HasEventHandler (orderInThisTransaction.OrderItems, "Removing", removingEventHandler), Is.True);
      }
    }

    [Test]
    [Obsolete ("TODO 2072 - Remove")]
    public void CopyCollectionEventHandlers_DoesNotLoadRelatedObjectsInOriginalTransaction ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      int loadedObjectsBefore = TestableClientTransaction.DataManager.DataContainers.Count;

      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents (TestableClientTransaction);

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.CopyCollectionEventHandlers (order, TestableClientTransaction);
      }

      int loadedObjectsAfter = TestableClientTransaction.DataManager.DataContainers.Count;
      Assert.That (loadedObjectsAfter, Is.EqualTo (loadedObjectsBefore));
    }

    [Test]
    [Obsolete ("TODO 2072 - Remove")]
    public void CopyCollectionEventHandlers_DoesNotLoadRelatedObjectContentsInDestinationTransaction ()
    {
      var mockRepository = new MockRepository ();
      var listenerMock = mockRepository.StrictMock<IClientTransactionListener> ();

      var innerTransaction = new TestableClientTransaction ();

      listenerMock.Stub (stub => stub.TransactionDiscard (innerTransaction));

      listenerMock.DataContainerMapRegistering (null, null);
      LastCall.IgnoreArguments ().Repeat.Any();
      listenerMock.RelationEndPointMapRegistering (null, null);
      LastCall.IgnoreArguments ().Repeat.Any ();
      listenerMock.VirtualRelationEndPointStateUpdated (null, null, null);
      LastCall.IgnoreArguments ().Repeat.Any ();

      mockRepository.ReplayAll ();

      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      order.OrderItems.Added += delegate { };

      using (innerTransaction.EnterDiscardingScope ())
      {
        // preload order, but not orderItems
        order.GetHandle().GetObject();

        innerTransaction.AddListener (listenerMock);
        int loadedObjectsBefore = innerTransaction.DataManager.DataContainers.Count;
        innerTransaction.CopyCollectionEventHandlers (order, TestableClientTransaction);
        int loadedObjectsAfter = innerTransaction.DataManager.DataContainers.Count;
        Assert.That (loadedObjectsAfter, Is.EqualTo (loadedObjectsBefore));
      }

      mockRepository.VerifyAll ();
    }

    [Test]
    [Obsolete ("TODO 2072 - Remove")]
    public void CopyTransactionEventHandlers ()
    {
      EventHandler<ClientTransactionEventArgs> committedHandler = delegate { };
      EventHandler<ClientTransactionCommittingEventArgs> committingHandler = delegate { };
      EventHandler<ClientTransactionEventArgs> loadedHandler = delegate { };
      EventHandler<ClientTransactionEventArgs> rolledBackHandler = delegate { };
      EventHandler<ClientTransactionEventArgs> rollingBackHandler = delegate { };
      EventHandler<SubTransactionCreatedEventArgs> subTransactionCreatedHandler1 = delegate { };
      EventHandler<SubTransactionCreatedEventArgs> subTransactionCreatedHandler2 = delegate { };

      ClientTransaction.Current.Committed += committedHandler;
      ClientTransaction.Current.Committing += committingHandler;
      ClientTransaction.Current.Loaded += loadedHandler;
      ClientTransaction.Current.RolledBack += rolledBackHandler;
      ClientTransaction.Current.RollingBack += rollingBackHandler;
      ClientTransaction.Current.SubTransactionCreated += subTransactionCreatedHandler1;
      ClientTransaction.Current.SubTransactionCreated += subTransactionCreatedHandler2;

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.CopyTransactionEventHandlers (TestableClientTransaction);
        Assert.That (HasEventHandler (ClientTransaction.Current, "Committed", committedHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "Committing", committingHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "Loaded", loadedHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "RolledBack", rolledBackHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "RollingBack", rollingBackHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "SubTransactionCreated", subTransactionCreatedHandler1), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "SubTransactionCreated", subTransactionCreatedHandler2), Is.True);
      }
    }

    [Test]
    [Obsolete ("TODO 2072 - Remove")]
    public void CopyTransactionEventHandlers_WithNoEventsDoesNotOverwriteOldHandlers ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        EventHandler<ClientTransactionEventArgs> committedHandler = delegate { };
        EventHandler<ClientTransactionCommittingEventArgs> committingHandler = delegate { };
        EventHandler<ClientTransactionEventArgs> loadedHandler = delegate { };
        EventHandler<ClientTransactionEventArgs> rolledBackHandler = delegate { };
        EventHandler<ClientTransactionEventArgs> rollingBackHandler = delegate { };
        EventHandler<SubTransactionCreatedEventArgs> subTransactionCreatedHandler1 = delegate { };
        EventHandler<SubTransactionCreatedEventArgs> subTransactionCreatedHandler2 = delegate { };

        ClientTransaction.Current.Committed += committedHandler;
        ClientTransaction.Current.Committing += committingHandler;
        ClientTransaction.Current.Loaded += loadedHandler;
        ClientTransaction.Current.RolledBack += rolledBackHandler;
        ClientTransaction.Current.RollingBack += rollingBackHandler;
        ClientTransaction.Current.SubTransactionCreated += subTransactionCreatedHandler1;
        ClientTransaction.Current.SubTransactionCreated += subTransactionCreatedHandler2;

        ClientTransaction.Current.CopyTransactionEventHandlers (TestableClientTransaction);

        Assert.That (HasEventHandler (ClientTransaction.Current, "Committed", committedHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "Committing", committingHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "Loaded", loadedHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "RolledBack", rolledBackHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "RollingBack", rollingBackHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "SubTransactionCreated", subTransactionCreatedHandler1), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "SubTransactionCreated", subTransactionCreatedHandler2), Is.True);
      }
    }

    [Test]
    public void LinqToClientTransaction ()
    {
      Order o1 = DomainObjectIDs.Order1.GetObject<Order> ();
      Order o2 = DomainObjectIDs.Order3.GetObject<Order> ();
      Order o3 = DomainObjectIDs.Order4.GetObject<Order> ();

      var loadedOrders = from o in TestableClientTransaction.GetEnlistedDomainObjects().OfType<Order>()
                         select o;
      Assert.That (loadedOrders.ToArray(), Is.EquivalentTo(new[] {o1, o2, o3}));
    }

    [Test]
    public void ToITransaction ()
    {
      ITransaction transaction = TestableClientTransaction.ToITransaction();

      Assert.That (((ClientTransactionWrapper) transaction).WrappedInstance, Is.SameAs (TestableClientTransaction));
    }

    [Test]
    public void ToITransaction_Override ()
    {
      var wrapperStub = MockRepository.GenerateMock<ITransaction> ();
      var transaction = new ClientTransactionWithCustomITransaction (wrapperStub);
      Assert.That (transaction.ToITransaction (), Is.SameAs (wrapperStub));
    }

    private bool HasEventHandler (object instance, string eventName, Delegate handler)
    {
      var eventField = (Delegate) PrivateInvoke.GetNonPublicField (instance, eventName);
      return eventField != null && Array.IndexOf (eventField.GetInvocationList (), handler) != -1;
    }
  }
}
