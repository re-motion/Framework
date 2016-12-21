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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.NewObject
{
  [TestFixture]
  public class ObjectsThrowingInCtorTest : ClientTransactionBaseTest
  {
    private Exception _exception;
    private InvalidOperationException _deleteException;

    public override void SetUp ()
    {
      base.SetUp ();

      _exception = new Exception ("Test exception.");
      _deleteException = new InvalidOperationException ("Thrown from Deleting!");
    }

    public override void TearDown ()
    {
      TestDomainBase.ClearStaticCtorHandlers();

      base.TearDown ();
    }

    [Test]
    public void ObjectThrowingExceptionInCtor_IsDeleted_AndDisenlisted ()
    {
      DomainObject throwingObject = null;
      RegisterThrowingCtorHandler<OrderItem> (instance => throwingObject = instance);

      Assert.That (() => OrderItem.NewObject (), Throws.Exception.SameAs (_exception));

      Assert.That (TestableClientTransaction.DataManager.DataContainers[throwingObject.ID], Is.Null);
      Assert.That (TestableClientTransaction.IsEnlisted (throwingObject), Is.False);
    }

    [Test]
    public void ThrowingObjectInBidirectionalRelation_IsRemovedFromRelation ()
    {
      var order = Order.NewObject ();
      RegisterThrowingCtorHandler<OrderItem> (instance => instance.Order = order);
      Assert.That (order.OrderItems, Is.Empty);

      Assert.That (() => OrderItem.NewObject (), Throws.Exception.SameAs (_exception));

      Assert.That (order.OrderItems, Is.Empty);
    }

    [Test]
    public void ThrowingObject_AlsoThrowingFromOnDeleting_CausesObjectCleanupException ()
    {
      OrderItem throwingInstance = null;
      RegisterThrowingCtorHandler<OrderItem> (instance =>
      {
        throwingInstance = instance;
        instance.Deleting += (sender, args) => { throw _deleteException; };
      });

      // Unfortunately, we can't use Assert.That (() => ..., Throws) here because the throwingInstance (required for constructing the message
      // check constraint) only exists once the NewObject call has executed. Therefore, use manual try/catch instead.
      try
      {
        OrderItem.NewObject ();
        Assert.Fail ("Expected ObjectCleanupException.");
      }
      catch (ObjectCleanupException ex)
      {
        Assert.That (ex, Is.TypeOf<ObjectCleanupException> ()
              .With.Message.EqualTo (
                  "While cleaning up an object of type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem' that threw an exception of type "
                  + "'System.Exception' from its constructor, another exception of type 'System.InvalidOperationException' was encountered. "
                  + "Cleanup was therefore aborted, and a partially constructed object with ID '" + throwingInstance.ID + "' remains "
                  + "within the ClientTransaction '" + TestableClientTransaction
                  + "'. Rollback the transaction to get rid of the partially constructed instance." + Environment.NewLine
                  + "Message of original exception: Test exception." + Environment.NewLine
                  + "Message of exception occurring during cleanup: Thrown from Deleting!")
              .And.Property ("ObjectID").Matches<ObjectID> (id => throwingInstance != null && id == throwingInstance.ID)
              .And.InnerException.SameAs (_exception)
              .And.Property ("CleanupException").SameAs (_deleteException));
      }

      Assert.That (throwingInstance.State, Is.EqualTo (StateType.New));
      Assert.That (TestableClientTransaction.DataManager.DataContainers[throwingInstance.ID], Is.Not.Null);

      TestableClientTransaction.Rollback();

      Assert.That (throwingInstance.State, Is.EqualTo (StateType.Invalid));
      Assert.That (TestableClientTransaction.DataManager.DataContainers[throwingInstance.ID], Is.Null);
    }

    [Test]
    public void ThrowingObjectInBidirectionalRelation_AlsoThrowingFromOnDeleting_CausesObjectCleanupException_AndRemainsInRelation ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order> ();
      OrderItem throwingInstance = null;
      RegisterThrowingCtorHandler<OrderItem> (instance =>
      {
        throwingInstance = instance;
        instance.Order = order;
        instance.Deleting += (sender, args) => { throw _deleteException; };
      });

      Assert.That (() => OrderItem.NewObject (), Throws.TypeOf<ObjectCleanupException> ());

      Assert.That (throwingInstance.State, Is.EqualTo (StateType.New));
      Assert.That (TestableClientTransaction.DataManager.DataContainers[throwingInstance.ID], Is.Not.Null);
      Assert.That (throwingInstance.Order, Is.SameAs (order));
      Assert.That (order.OrderItems, Has.Member (throwingInstance));

      TestableClientTransaction.Rollback ();

      Assert.That (throwingInstance.State, Is.EqualTo (StateType.Invalid));
      Assert.That (TestableClientTransaction.DataManager.DataContainers[throwingInstance.ID], Is.Null);
      Assert.That (order.OrderItems, Has.No.Member (throwingInstance));
    }

    [Test]
    public void ThrowingObject_UncaughtByOtherObjectCtor_CausesBothToBeCleanedUp ()
    {
      OrderItem throwingOrderItem = null;
      Order triggeringOrder = null;

      RegisterThrowingCtorHandler<OrderItem> (instance => throwingOrderItem = instance);
      RegisterThrowingCtorHandler<Order> (instance => { triggeringOrder = instance; OrderItem.NewObject (); });

      Assert.That (() => Order.NewObject(), Throws.Exception.SameAs (_exception));
      Assert.That (TestableClientTransaction.IsEnlisted (throwingOrderItem), Is.False);
      Assert.That (TestableClientTransaction.IsEnlisted (triggeringOrder), Is.False);
      Assert.That (TestableClientTransaction.DataManager.DataContainers[throwingOrderItem.ID], Is.Null);
      Assert.That (TestableClientTransaction.DataManager.DataContainers[triggeringOrder.ID], Is.Null);
    }

    [Test]
    public void ThrowingObject_CaughtByOtherObjectCtor_CausesOnlyThrowingObjectToBeCleanedUp ()
    {
      OrderItem throwingOrderItem = null;
      Order triggeringOrder = null;

      RegisterThrowingCtorHandler<OrderItem> (instance => throwingOrderItem = instance);
      RegisterCtorHandler<Order> (instance =>
      {
        triggeringOrder = instance;
        Assert.That (() => OrderItem.NewObject (), Throws.Exception.SameAs (_exception));
      });

      Assert.That (() => Order.NewObject (), Throws.Nothing);
      Assert.That (TestableClientTransaction.IsEnlisted (throwingOrderItem), Is.False);
      Assert.That (TestableClientTransaction.IsEnlisted (triggeringOrder), Is.True);
      Assert.That (TestableClientTransaction.DataManager.DataContainers[throwingOrderItem.ID], Is.Null);
      Assert.That (TestableClientTransaction.DataManager.DataContainers[triggeringOrder.ID], Is.Not.Null);
    }

    private void RegisterCtorHandler<T> (Action<T> handler) where T : TestDomainBase
    {
      TestDomainBase.StaticCtorHandler += (sender, args) =>
      {
        if (sender is T)
          handler ((T) sender);
      };
    }

    private void RegisterThrowingCtorHandler<T> (Action<T> additionalHandler = null) where T : TestDomainBase
    {
      RegisterCtorHandler<T> (
          t =>
          {
            if (additionalHandler != null)
              additionalHandler (t);
            throw _exception;
          });
    }
  }
}