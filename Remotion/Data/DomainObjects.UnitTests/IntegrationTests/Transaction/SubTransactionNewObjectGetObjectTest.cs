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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionNewObjectGetObjectTest : ClientTransactionBaseTest
  {
    [Test]
    public void SubTransaction_CanBeUsedToCreateAndLoadNewObjects ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (subTransaction));
        Order order = Order.NewObject ();
        Assert.That (subTransaction.IsEnlisted (order), Is.True);
        Assert.That (TestableClientTransaction.IsEnlisted (order), Is.True);

        order.OrderNumber = 4711;
        Assert.That (order.OrderNumber, Is.EqualTo (4711));

        OrderItem item = OrderItem.NewObject ();
        order.OrderItems.Add (item);
        Assert.That (order.OrderItems.Contains (item.ID), Is.True);

        Ceo ceo = DomainObjectIDs.Ceo1.GetObject<Ceo> ();
        Assert.That (ceo, Is.Not.Null);
        Assert.That (subTransaction.IsEnlisted (ceo), Is.True);
        Assert.That (TestableClientTransaction.IsEnlisted (ceo), Is.True);

        Assert.That (DomainObjectIDs.Company1.GetObject<Company> (), Is.SameAs (ceo.Company));
      }
    }

    [Test]
    public void DomainObjects_CreatedInParent_CanBeUsedInSubTransactions ()
    {
      Order order = Order.NewObject ();
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      Assert.That (TestableClientTransaction.IsEnlisted (order), Is.True);
      Assert.That (subTransaction.IsEnlisted (order), Is.True);
    }

    [Test]
    public void DomainObjects_CreatedInParent_NotLoadedYetInSubTransaction ()
    {
      Order order = Order.NewObject ();
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      Assert.That (order.TransactionContext[subTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void DomainObjects_CreatedInSubTransaction_CanBeUsedInParent ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        Order order = Order.NewObject ();
        Assert.That (subTransaction.IsEnlisted (order), Is.True);
        Assert.That (TestableClientTransaction.IsEnlisted (order), Is.True);
      }
    }

    [Test]
    public void DomainObjects_CreatedInSubTransaction_InvalidInParent ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        Order order = Order.NewObject ();
        Assert.That (order.TransactionContext[subTransaction].State, Is.EqualTo (StateType.New));
        Assert.That (order.TransactionContext[TestableClientTransaction].State, Is.EqualTo (StateType.Invalid));
      }
    }

    [Test]
    public void DomainObjects_CreatedInSubTransaction_CommitMakesValidInParent ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        var instance = ClassWithAllDataTypes.NewObject ();
        Assert.That (instance.TransactionContext[subTransaction].State, Is.EqualTo (StateType.New));
        Assert.That (instance.TransactionContext[TestableClientTransaction].State, Is.EqualTo (StateType.Invalid));
        subTransaction.Commit ();
        Assert.That (instance.TransactionContext[TestableClientTransaction].State, Is.EqualTo (StateType.New));
      }
    }

    [Test]
    public void DomainObjects_LoadedInParent_CanBeUsedInSubTransactions ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      Assert.That (TestableClientTransaction.IsEnlisted (order), Is.True);
      Assert.That (subTransaction.IsEnlisted (order), Is.True);
    }

    [Test]
    public void DomainObjects_LoadedInParent_NotLoadedYetInSubTransaction ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      Assert.That (order.TransactionContext[subTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void DomainObjects_LoadedInSubTransaction_CanBeUsedInParent ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        Order order = DomainObjectIDs.Order1.GetObject<Order> ();
        Assert.That (subTransaction.IsEnlisted (order), Is.True);
        Assert.That (TestableClientTransaction.IsEnlisted (order), Is.True);
      }
    }

    [Test]
    public void SubTransaction_CanAccessObject_CreatedInParent ()
    {
      Order order = Order.NewObject ();
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        order.OrderNumber = 5;
        order.OrderTicket = OrderTicket.NewObject ();
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void Parent_CannotAccessObject_CreatedInSubTransaction ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      Order order;
      using (subTransaction.EnterDiscardingScope ())
      {
        order = Order.NewObject ();
      }
      Dev.Null = order.OrderNumber;
    }

    [Test]
    public void SubTransaction_CanAccessObject_LoadedInParent ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        ++order.OrderNumber;
        Dev.Null = order.OrderTicket;
        order.OrderTicket = OrderTicket.NewObject ();
      }
    }

    [Test]
    public void Parent_CanAccessObject_LoadedInSubTransaction ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      Order order;
      using (subTransaction.EnterDiscardingScope ())
      {
        order = DomainObjectIDs.Order1.GetObject<Order> ();
      }
      Assert.That (order.OrderNumber, Is.EqualTo (1));
    }

    [Test]
    public void Parent_CanReloadObject_LoadedInSubTransaction_AndGetTheSameReference ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      Order order;
      using (subTransaction.EnterDiscardingScope ())
      {
        order = DomainObjectIDs.Order1.GetObject<Order> ();
      }
      Assert.That (DomainObjectIDs.Order1.GetObject<Order> (), Is.SameAs (order));
    }

    [Test]
    public void GetObject_DeletedInParentTransaction ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      order1.Delete ();

      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        Assert.That (
            () => order1.ID.GetObject<Order> (),
            Throws.TypeOf<ObjectInvalidException> ().With.Message.EqualTo (
                "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is invalid in this transaction."));
      }
    }

    [Test]
    public void GetObject_DeletedInParentTransaction_IncludeDeletedTrue ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      order1.Delete ();

      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        Assert.That (
            () => order1.ID.GetObject<Order> (includeDeleted: true),
            Throws.TypeOf<ObjectInvalidException> ().With.Message.EqualTo (
                "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is invalid in this transaction."));
      }
    }

    [Test]
    public void GetObjectReference_DeletedInParentTransaction ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      order1.Delete ();

      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        var objectReference = LifetimeService.GetObjectReference (subTransaction, order1.ID);
        Assert.That (objectReference, Is.SameAs (order1));
        Assert.That (objectReference.State, Is.EqualTo (StateType.Invalid));
      }
    }

    [Test]
    public void GetObjects_UnloadedObjects_PropagatedToParent ()
    {
      ClientTransaction parent = ClientTransaction.CreateRootTransaction ();
      ClientTransaction subTransaction = parent.CreateSubTransaction ();

      LifetimeService.GetObject (subTransaction, DomainObjectIDs.ClassWithAllDataTypes1, false); // preload ClassWithAllDataTypes

      var extensionMock = MockRepository.GenerateMock<IClientTransactionExtension> ();
      extensionMock.Stub (stub => stub.Key).Return ("mock");
      parent.Extensions.Add (extensionMock);

      LifetimeService.GetObjects<DomainObject> (
          subTransaction,
          DomainObjectIDs.Order1,
          DomainObjectIDs.ClassWithAllDataTypes1,
        // this has already been loaded
          DomainObjectIDs.Order3,
          DomainObjectIDs.OrderItem1);

      extensionMock.AssertWasCalled (mock => mock.ObjectsLoading (Arg.Is (parent),
          Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.OrderItem1 })));
      extensionMock.AssertWasNotCalled (mock => mock.ObjectsLoading (Arg.Is (parent),
          Arg<ReadOnlyCollection<ObjectID>>.List.ContainsAll (new[] { DomainObjectIDs.ClassWithAllDataTypes1 })));
    }

    [Test]
    public void TryGetObjects_UnloadedObjects_PropagatedToParent ()
    {
      ClientTransaction parent = ClientTransaction.CreateRootTransaction ();
      ClientTransaction subTransaction = parent.CreateSubTransaction ();

      LifetimeService.GetObject (subTransaction, DomainObjectIDs.ClassWithAllDataTypes1, false); // preload ClassWithAllDataTypes

      var extensionMock = MockRepository.GenerateMock<IClientTransactionExtension> ();
      extensionMock.Stub (stub => stub.Key).Return ("mock");
      parent.Extensions.Add (extensionMock);

      LifetimeService.TryGetObjects<DomainObject> (
          subTransaction,
          DomainObjectIDs.Order1,
          DomainObjectIDs.ClassWithAllDataTypes1, // this has already been loaded
          DomainObjectIDs.Order3,
          DomainObjectIDs.OrderItem1);

      extensionMock.AssertWasCalled (mock => mock.ObjectsLoading (Arg.Is (parent),
          Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.OrderItem1 })));
      extensionMock.AssertWasNotCalled (mock => mock.ObjectsLoading (Arg.Is (parent),
          Arg<ReadOnlyCollection<ObjectID>>.List.ContainsAll (new[] { DomainObjectIDs.ClassWithAllDataTypes1 })));
    }

    [Test]
    public void GetObjects_UnloadedObjects_Events ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
        PrivateInvoke.InvokeNonPublicMethod (subTransaction, "AddListener", listenerMock);

        var eventReceiver = new ClientTransactionEventReceiver (subTransaction);
        DomainObject[] objects = LifetimeService.GetObjects<DomainObject> (
            subTransaction,
            DomainObjectIDs.Order1,
            DomainObjectIDs.Order3,
            DomainObjectIDs.OrderItem1);

        Assert.That (eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo (1));
        Assert.That (eventReceiver.LoadedDomainObjectLists[0], Is.EqualTo (objects));

        listenerMock.AssertWasCalled (mock => mock.ObjectsLoading (
            Arg.Is (subTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.OrderItem1 })));

        listenerMock.AssertWasCalled (mock => mock.ObjectsLoaded (
            Arg.Is (subTransaction),
            Arg<ReadOnlyCollection<DomainObject>>.List.Equal (objects)));
      }
    }

    [Test]
    public void GetObjects_LoadedObjects ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        var expectedObjects = new object[]
                              {
                                  DomainObjectIDs.Order1.GetObject<Order> (), DomainObjectIDs.Order3.GetObject<Order> (),
                                  DomainObjectIDs.OrderItem1.GetObject<OrderItem>()
                              };
        DomainObject[] objects = LifetimeService.GetObjects<DomainObject> (
            subTransaction,
            DomainObjectIDs.Order1,
            DomainObjectIDs.Order3,
            DomainObjectIDs.OrderItem1);
        Assert.That (objects, Is.EqualTo (expectedObjects));
      }
    }

    [Test]
    public void GetObjects_LoadedObjects_Events ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        var eventReceiver = new ClientTransactionEventReceiver (subTransaction);
        DomainObjectIDs.Order1.GetObject<Order> ();
        DomainObjectIDs.Order3.GetObject<Order> ();
        DomainObjectIDs.OrderItem1.GetObject<OrderItem>();

        eventReceiver.Clear ();

        var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
        PrivateInvoke.InvokeNonPublicMethod (subTransaction, "AddListener", listenerMock);

        LifetimeService.GetObjects<DomainObject> (subTransaction, DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.OrderItem1);
        Assert.That (eventReceiver.LoadedDomainObjectLists, Is.Empty);

        listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoading (
            Arg<ClientTransaction>.Is.Anything,
            Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
        listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoaded (
            Arg<ClientTransaction>.Is.Anything,
            Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));
      }
    }

    [Test]
    public void GetObjects_NewObjects ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        var expectedObjects = new DomainObject[] { Order.NewObject (), OrderItem.NewObject () };
        DomainObject[] objects = LifetimeService.GetObjects<DomainObject> (subTransaction, expectedObjects[0].ID, expectedObjects[1].ID);
        Assert.That (objects, Is.EqualTo (expectedObjects));
      }
    }

    [Test]
    public void GetObjects_NewObjects_Events ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        var eventReceiver = new ClientTransactionEventReceiver (subTransaction);
        var expectedObjects = new DomainObject[] { Order.NewObject (), OrderItem.NewObject () };
        eventReceiver.Clear ();

        var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
        PrivateInvoke.InvokeNonPublicMethod (subTransaction, "AddListener", listenerMock);

        LifetimeService.GetObjects<DomainObject> (subTransaction, expectedObjects[0].ID, expectedObjects[1].ID);
        Assert.That (eventReceiver.LoadedDomainObjectLists, Is.Empty);

        listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoading (
            Arg<ClientTransaction>.Is.Anything,
            Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
        listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoaded (
            Arg<ClientTransaction>.Is.Anything,
            Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectsNotFoundException), ExpectedMessage = 
        "Object(s) could not be found: 'Order|33333333-3333-3333-3333-333333333333|System.Guid'.")]
    public void GetObjects_NotFound ()
    {
      var guid = new Guid ("33333333333333333333333333333333");
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        LifetimeService.GetObjects<DomainObject> (subTransaction, new ObjectID (typeof (Order), guid));
      }
    }

    [Test]
    public void TryGetObjects_NotFound ()
    {
      var guid = new Guid ("33333333333333333333333333333333");
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        Order newObject = Order.NewObject ();
        Order[] objects = LifetimeService.TryGetObjects<Order> (
            subTransaction,
            DomainObjectIDs.Order1,
            newObject.ID,
            new ObjectID (typeof (Order), guid),
            DomainObjectIDs.Order3);
        var expectedObjects = new DomainObject[]
                              {
                                  DomainObjectIDs.Order1.GetObject<Order> (),
                                  newObject,
                                  null,
                                  DomainObjectIDs.Order3.GetObject<Order> ()
                              };
        Assert.That (objects, Is.EqualTo (expectedObjects));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidCastException))]
    public void GetObjects_InvalidType ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        LifetimeService.GetObjects<OrderItem> (subTransaction, DomainObjectIDs.Order1);
      }
    }

    [Test]
    public void GetObjects_Deleted ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        var order = DomainObjectIDs.Order1.GetObject<Order> ();
        order.Delete ();

        var result = LifetimeService.GetObjects<Order> (subTransaction, DomainObjectIDs.Order1);

        Assert.That (result[0], Is.SameAs (order));
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException),
        ExpectedMessage = "Object 'ClassWithAllDataTypes|3f647d79-0caf-4a53-baa7-a56831f8ce2d|System.Guid' is invalid in this transaction.")]
    public void GetObjects_Invalid_Throws ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ().Delete ();
        subTransaction.Commit ();
        LifetimeService.GetObjects<ClassWithAllDataTypes> (subTransaction, DomainObjectIDs.ClassWithAllDataTypes1);
      }
    }

    [Test]
    public void GetObjects_DeletedInParentTransaction_Throws ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      var order3 = DomainObjectIDs.Order3.GetObject<Order> ();
      var order4 = DomainObjectIDs.Order4.GetObject<Order> ();

      order3.Delete ();
      order4.Delete ();

      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        Assert.That (
            () => LifetimeService.GetObjects<Order> (subTransaction, order1.ID, order3.ID, order4.ID),
            Throws.TypeOf<ObjectInvalidException> ().With.Message.EqualTo (
                "Object 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid' is invalid in this transaction."));
      }
    }

    [Test]
    public void TryGetObjects_DeletedInParentTransaction ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      var order3 = DomainObjectIDs.Order3.GetObject<Order> ();
      var order4 = DomainObjectIDs.Order4.GetObject<Order> ();

      order3.Delete ();
      order4.Delete ();

      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        var result = LifetimeService.TryGetObjects<Order> (subTransaction, order1.ID, order3.ID, order4.ID);
        Assert.That (result, Is.EqualTo(new[] { order1, order3, order4}));
        Assert.That (result[1].State, Is.EqualTo (StateType.Invalid));
        Assert.That (result[2].State, Is.EqualTo (StateType.Invalid));
      }
    }


    
  }
}