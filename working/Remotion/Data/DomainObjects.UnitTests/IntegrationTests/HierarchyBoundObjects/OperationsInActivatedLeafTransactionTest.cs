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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.HierarchyBoundObjects
{
  [TestFixture]
  public class OperationsInActivatedLeafTransactionTest : HierarchyBoundObjectsTestBase
  {
    private ClientTransaction _rootTransaction;
    private ClientTransaction _middleTransaction;
    private ClientTransaction _leafTransaction;
    private Order _order1LoadedInMiddleTransaction;
    private Order _objectReferenceFromMiddleTransaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _rootTransaction = ClientTransaction.CreateRootTransaction();
      _middleTransaction = _rootTransaction.CreateSubTransaction ();
      _leafTransaction = _middleTransaction.CreateSubTransaction ();

      _order1LoadedInMiddleTransaction = DomainObjectIDs.Order1.GetObject<Order> (_middleTransaction);
      _objectReferenceFromMiddleTransaction = DomainObjectIDs.Order3.GetObjectReference<Order> (_rootTransaction);
    }

    [Test]
    public void DefaultTransactionContext_IsRootTransaction ()
    {
      Assert.That (_order1LoadedInMiddleTransaction.RootTransaction, Is.SameAs (_rootTransaction));
      Assert.That (_order1LoadedInMiddleTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
    }

    [Test]
    public void DefaultTransactionContext_IsActivatedTransaction ()
    {
      using (_leafTransaction.EnterNonDiscardingScope())
      {
        Assert.That (_order1LoadedInMiddleTransaction.RootTransaction, Is.SameAs (_rootTransaction));
        Assert.That (_order1LoadedInMiddleTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_leafTransaction));
      }
    }

    [Test]
    public void AccessingPropertiesAndState_AffectsActiveLeafTransaction ()
    {
      using (_leafTransaction.EnterNonDiscardingScope())
      {
        Assert.That (_order1LoadedInMiddleTransaction.State, Is.EqualTo (StateType.NotLoadedYet));
        Assert.That (_order1LoadedInMiddleTransaction.OrderNumber, Is.EqualTo (1));
        Assert.That (_order1LoadedInMiddleTransaction.OrderItems, Has.Count.EqualTo (2));

        Assert.That (_leafTransaction.HasChanged(), Is.False);

        _order1LoadedInMiddleTransaction.OrderNumber = 2;
        _order1LoadedInMiddleTransaction.OrderItems.Clear();

        Assert.That (_order1LoadedInMiddleTransaction.State, Is.EqualTo (StateType.Changed));
        Assert.That (_order1LoadedInMiddleTransaction.OrderNumber, Is.EqualTo (2));
        Assert.That (_order1LoadedInMiddleTransaction.OrderItems, Is.Empty);

        Assert.That (_rootTransaction.HasChanged(), Is.False);
        Assert.That (_middleTransaction.HasChanged(), Is.False);
        Assert.That (_leafTransaction.HasChanged(), Is.True);
        Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.Changed));
      }
    }

    [Test]
    public void PropertyIndexer_AffectsActiveLeafTransaction ()
    {
      using (_leafTransaction.EnterNonDiscardingScope())
      {
        Assert.That (_order1LoadedInMiddleTransaction.Properties[typeof (Order), "OrderNumber"].GetValue<int>(), Is.EqualTo (1));
        Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.Unchanged));

        _order1LoadedInMiddleTransaction.Properties[typeof (Order), "OrderNumber"].SetValue (2);

        Assert.That (_order1LoadedInMiddleTransaction.Properties[typeof (Order), "OrderNumber"].GetValue<int>(), Is.EqualTo (2));
        Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.Changed));
      }
    }

    [Test]
    public void LoadedRelatedObjects_AreAssociatedWithSameRootTransaction ()
    {
      using (_leafTransaction.EnterNonDiscardingScope())
      {
        Assert.That (_order1LoadedInMiddleTransaction.OrderTicket.RootTransaction, Is.SameAs (_rootTransaction));
        Assert.That (_order1LoadedInMiddleTransaction.OrderItems[0].RootTransaction, Is.SameAs (_rootTransaction));
      }
    }

    [Test]
    public void SetRelatedObject_SucceedsWithItemFromSameTransactionHierarchy_AndAffectsActiveLeafTransaction ()
    {
      using (_leafTransaction.EnterNonDiscardingScope())
      {
        var orderTicket = DomainObjectIDs.OrderTicket2.GetObject<OrderTicket> (_middleTransaction);

        _order1LoadedInMiddleTransaction.OrderTicket = orderTicket;

        Assert.That (_order1LoadedInMiddleTransaction.OrderTicket, Is.SameAs (orderTicket));
        Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.Changed));
        Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _middleTransaction), Is.EqualTo (StateType.Unchanged));
        Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _rootTransaction), Is.EqualTo (StateType.Unchanged));
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void SetRelatedObject_FailsWithItemFromOtherHierarchy ()
    {
      using (_leafTransaction.EnterNonDiscardingScope())
      {
        var orderTicketFromOtherTransaction = ClientTransaction.CreateRootTransaction ().ExecuteInScope (() => OrderTicket.NewObject ());
        _order1LoadedInMiddleTransaction.OrderTicket = orderTicketFromOtherTransaction;
      }
    }

    [Test]
    public void InsertRelatedObject_SucceedsWithItemFromSameTransactionHierarchy_AndAffectsActiveLeafTransaction ()
    {
      using (_leafTransaction.EnterNonDiscardingScope())
      {
        OrderItem orderItem = DomainObjectIDs.OrderItem3.GetObject<OrderItem> (_middleTransaction);

        _order1LoadedInMiddleTransaction.OrderItems.Add (orderItem);

        Assert.That (_order1LoadedInMiddleTransaction.OrderItems.Last(), Is.SameAs (orderItem));
        Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.Changed));
        Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _middleTransaction), Is.EqualTo (StateType.Unchanged));
        Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _rootTransaction), Is.EqualTo (StateType.Unchanged));
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void InsertRelatedObject_FailsWithItemFromOtherHierarchy ()
    {
      using (_leafTransaction.EnterNonDiscardingScope())
      {
        var orderItemFromOtherHierarchy = ClientTransaction.CreateRootTransaction().ExecuteInScope (() => OrderItem.NewObject());
        _order1LoadedInMiddleTransaction.OrderItems.Add (orderItemFromOtherHierarchy);
      }
    }

    // Note: No Timestamp check here - the value is always the same as in the root tx, so we can't test that it is retrieved from the leaf tx.

    [Test]
    public void EnsureDataAvailable_AffectsActiveLeafTransaction ()
    {
      using (_leafTransaction.EnterNonDiscardingScope())
      {
        Assert.That (GetStateFromTransaction (_objectReferenceFromMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.NotLoadedYet));

        _objectReferenceFromMiddleTransaction.EnsureDataAvailable();

        Assert.That (GetStateFromTransaction (_objectReferenceFromMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.Unchanged));
      }
    }

    [Test]
    public void TryEnsureDataAvailable_AffectsActiveLeafTransaction ()
    {
      using (_leafTransaction.EnterNonDiscardingScope())
      {
        Assert.That (GetStateFromTransaction (_objectReferenceFromMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.NotLoadedYet));

        _objectReferenceFromMiddleTransaction.TryEnsureDataAvailable();

        Assert.That (GetStateFromTransaction (_objectReferenceFromMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.Unchanged));
      }
    }

    [Test]
    public void Delete_AffectsActiveLeafTransaction ()
    {
      using (_leafTransaction.EnterNonDiscardingScope())
      {
        Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.NotLoadedYet));

        _order1LoadedInMiddleTransaction.Delete();

        Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.Deleted));
      }
    }

    [Test]
    public void IsInvalid_AffectsActiveLeafTransaction ()
    {
      using (_leafTransaction.EnterNonDiscardingScope())
      {
        var order = (Order) LifetimeService.NewObject (_leafTransaction, typeof (Order), ParamList.Empty);

        Assert.That (GetStateFromTransaction (order, _leafTransaction), Is.EqualTo (StateType.New));
        Assert.That (order.State, Is.Not.EqualTo (StateType.Invalid));

        order.Delete();

        Assert.That (GetStateFromTransaction (order, _leafTransaction), Is.EqualTo (StateType.Invalid));
        Assert.That (order.State, Is.EqualTo (StateType.Invalid));
      }
    }

    [Test]
    public void RegisterForCommit_AffectsActiveLeafTransaction ()
    {
      using (_leafTransaction.EnterNonDiscardingScope())
      {
        Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.NotLoadedYet));

        _order1LoadedInMiddleTransaction.RegisterForCommit();

        Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.Changed));
      }
    }
  }
}