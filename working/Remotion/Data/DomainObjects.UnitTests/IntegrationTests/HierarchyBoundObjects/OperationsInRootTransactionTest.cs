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
  public class OperationsInRootTransactionTest : HierarchyBoundObjectsTestBase
  {
    private ClientTransaction _rootTransaction;
    private Order _order1LoadedInRootTransaction;
    private Order _objectReferenceFromRootTransaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _rootTransaction = ClientTransaction.CreateRootTransaction();
      _order1LoadedInRootTransaction = DomainObjectIDs.Order1.GetObject<Order> (_rootTransaction);
      _objectReferenceFromRootTransaction = DomainObjectIDs.Order3.GetObjectReference<Order> (_rootTransaction);
    }

    [Test]
    public void DefaultTransactionContext_IsAssociatedRootTransaction ()
    {
      Assert.That (_order1LoadedInRootTransaction.RootTransaction, Is.SameAs (_rootTransaction));
      Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
    }
    
    [Test]
    public void AccessingPropertiesAndState_AffectsAssociatedRootTransaction ()
    {
      Assert.That (_order1LoadedInRootTransaction.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (1));
      Assert.That (_order1LoadedInRootTransaction.OrderItems, Has.Count.EqualTo (2));
      
      Assert.That (_rootTransaction.HasChanged (), Is.False);

      _order1LoadedInRootTransaction.OrderNumber = 2;
      _order1LoadedInRootTransaction.OrderItems.Clear ();

      Assert.That (_order1LoadedInRootTransaction.State, Is.EqualTo (StateType.Changed));
      Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (2));
      Assert.That (_order1LoadedInRootTransaction.OrderItems, Is.Empty);

      Assert.That (_rootTransaction.HasChanged(), Is.True);
      Assert.That (GetStateFromTransaction (_order1LoadedInRootTransaction, _rootTransaction), Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void PropertyIndexer_AffectsAssociatedRootTransaction ()
    {
      Assert.That (_order1LoadedInRootTransaction.Properties[typeof (Order), "OrderNumber"].GetValue<int>(), Is.EqualTo (1));
      Assert.That (GetStateFromTransaction (_order1LoadedInRootTransaction, _rootTransaction), Is.EqualTo (StateType.Unchanged));

      _order1LoadedInRootTransaction.Properties[typeof (Order), "OrderNumber"].SetValue (2);

      Assert.That (_order1LoadedInRootTransaction.Properties[typeof (Order), "OrderNumber"].GetValue<int> (), Is.EqualTo (2));
      Assert.That (GetStateFromTransaction (_order1LoadedInRootTransaction, _rootTransaction), Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void LoadedRelatedObjects_AreAssociatedWithSameRootTransaction ()
    {
      Assert.That (_order1LoadedInRootTransaction.OrderTicket.RootTransaction, Is.SameAs (_rootTransaction));
      Assert.That (_order1LoadedInRootTransaction.OrderItems[0].RootTransaction, Is.SameAs (_rootTransaction));
    }

    [Test]
    public void SetRelatedObject_SucceedsWithItemFromSameRootTransaction ()
    {
      OrderTicket orderTicket = _rootTransaction.ExecuteInScope (() => OrderTicket.NewObject());
      _order1LoadedInRootTransaction.OrderTicket = orderTicket;
      Assert.That (_order1LoadedInRootTransaction.OrderTicket, Is.SameAs (orderTicket));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void SetRelatedObject_FailsWithItemFromOtherRootTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        _order1LoadedInRootTransaction.OrderTicket = OrderTicket.NewObject ();
      }
    }

    [Test]
    public void InsertRelatedObject_SucceedsWithItemFromSameRootTransaction ()
    {
      OrderItem orderItem = _rootTransaction.ExecuteInScope (() => OrderItem.NewObject());
      _order1LoadedInRootTransaction.OrderItems.Add (orderItem);
      Assert.That (_order1LoadedInRootTransaction.OrderItems.Last(), Is.SameAs (orderItem));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void InsertRelatedObject_FailsWithItemFromOtherRootTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        _order1LoadedInRootTransaction.OrderItems.Add (OrderItem.NewObject ());
      }
    }

    [Test]
    public void Timestamp_AffectsAssociatedRootTransaction ()
    {
      Assert.That (_order1LoadedInRootTransaction.Timestamp, Is.Not.Null);
      Assert.That (
          _order1LoadedInRootTransaction.Timestamp,
          Is.EqualTo (GetDataContainerFromTransaction (_order1LoadedInRootTransaction, _rootTransaction).Timestamp));
    }

    [Test]
    public void EnsureDataAvailable_AffectsAssociatedRootTransaction ()
    {
      Assert.That (GetStateFromTransaction (_objectReferenceFromRootTransaction, _rootTransaction), Is.EqualTo (StateType.NotLoadedYet));

      _objectReferenceFromRootTransaction.EnsureDataAvailable();

      Assert.That (GetStateFromTransaction (_objectReferenceFromRootTransaction, _rootTransaction), Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void TryEnsureDataAvailable_AffectsAssociatedRootTransaction ()
    {
      Assert.That (GetStateFromTransaction (_objectReferenceFromRootTransaction, _rootTransaction), Is.EqualTo (StateType.NotLoadedYet));

      _objectReferenceFromRootTransaction.TryEnsureDataAvailable ();

      Assert.That (GetStateFromTransaction (_objectReferenceFromRootTransaction, _rootTransaction), Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void Delete_AffectsAssociatedRootTransaction ()
    {
      Assert.That (GetStateFromTransaction (_order1LoadedInRootTransaction, _rootTransaction), Is.EqualTo (StateType.Unchanged));

      _order1LoadedInRootTransaction.Delete();

      Assert.That (GetStateFromTransaction (_order1LoadedInRootTransaction, _rootTransaction), Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void IsInvalid_AffectsAssociatedRootTransaction ()
    {
      var order = (Order) LifetimeService.NewObject (_rootTransaction, typeof (Order), ParamList.Empty);
      
      Assert.That (GetStateFromTransaction (order, _rootTransaction), Is.EqualTo (StateType.New));
      Assert.That (order.State, Is.Not.EqualTo (StateType.Invalid));

      order.Delete();

      Assert.That (GetStateFromTransaction (order, _rootTransaction), Is.EqualTo (StateType.Invalid));
      Assert.That (order.State, Is.EqualTo (StateType.Invalid));
    }

    [Test]
    public void RegisterForCommit_AffectsAssociatedRootTransaction ()
    {
      Assert.That (GetStateFromTransaction (_order1LoadedInRootTransaction, _rootTransaction), Is.EqualTo (StateType.Unchanged));

      _order1LoadedInRootTransaction.RegisterForCommit();

      Assert.That (GetStateFromTransaction (_order1LoadedInRootTransaction, _rootTransaction), Is.EqualTo (StateType.Changed));
    }
  }
}