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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class ObjectIDExtensionsTest : StandardMappingTest
  {
    private ObjectID _orderID1;
    private ObjectID _objectID2;
    private ObjectID _notFoundObjectID;
    private ClientTransaction _clientTransaction;

    public override void SetUp ()
    {
      base.SetUp ();
      _orderID1 = DomainObjectIDs.Order1;
      _objectID2 = DomainObjectIDs.Order2;
      _notFoundObjectID = new ObjectID (typeof (Order), Guid.NewGuid());
      _clientTransaction = ClientTransaction.CreateRootTransaction();
    }

    [Test]
    public void GetObject_LoadsObjectIntoGivenTransaction ()
    {
      var result = _orderID1.GetObject<Order> (_clientTransaction);

      CheckDomainObject (result, _clientTransaction, expectedID: _orderID1, expectedState: StateType.Unchanged);
    }

    [Test]
    public void GetObject_NoClientTransactionGiven_UsesCurrentTransaction ()
    {
      using (_clientTransaction.EnterNonDiscardingScope())
      {
        var result = _orderID1.GetObject<Order>();
        CheckDomainObject (result, _clientTransaction);
      }
    }

    [Test]
    public void GetObject_NoClientTransactionGiven_NoCurrentTransaction_Throws ()
    {
      Assert.That (
          () => _orderID1.GetObject<Order>(),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void GetObject_IncludeDeletedTrue_LoadsDeletedObject ()
    {
      _clientTransaction.ExecuteInScope (() => _orderID1.GetObject<Order>().Delete());

      var result = _orderID1.GetObject<Order> (_clientTransaction, includeDeleted: true);

      Assert.That (result, Is.Not.Null);
      CheckDomainObject (result, _clientTransaction, expectedState: StateType.Deleted);
    }

    [Test]
    public void GetObject_IncludeDeletedFalse_ThrowsOnDeletedObject ()
    {
      _clientTransaction.ExecuteInScope (() => _orderID1.GetObject<Order> ().Delete ());
      Assert.That (() => _orderID1.GetObject<Order> (_clientTransaction, includeDeleted: false), Throws.TypeOf<ObjectDeletedException>());
    }

    [Test]
    public void GetObject_IncludeDeletedUnspecified_ThrowsOnDeletedObject ()
    {
      _clientTransaction.ExecuteInScope (() => _orderID1.GetObject<Order> ().Delete ());
      Assert.That (() => _orderID1.GetObject<Order> (_clientTransaction), Throws.TypeOf<ObjectDeletedException> ());
    }

    [Test]
    public void GetObject_AcceptsBaseType ()
    {
      var result = _orderID1.GetObject<TestDomainBase> (_clientTransaction);

      CheckDomainObject (result, _clientTransaction);
    }

    [Test]
    public void GetObject_ThrowsOnUnrelatedType ()
    {
      Assert.That (
          () => _orderID1.GetObject<OrderItem> (_clientTransaction),
          Throws.TypeOf<ArgumentException> ().With.Message.EqualTo (
              "The ObjectID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is not compatible with type "
              + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem'.\r\nParameter name: id"));
    }

    [Test]
    public void TryGetObject_LoadsObjectIntoGivenTransaction ()
    {
      var result = _orderID1.TryGetObject<Order> (_clientTransaction);
      CheckDomainObject (result, _clientTransaction, expectedID: _orderID1, expectedState: StateType.Unchanged);
    }

    [Test]
    public void TryGetObject_AllowsNotFoundObjects ()
    {
      var result = _notFoundObjectID.TryGetObject<Order> (_clientTransaction);
      Assert.That (result, Is.Null);
    }

    [Test]
    public void TryGetObject_NoClientTransactionGiven_UsesCurrentTransaction ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        var result = _orderID1.GetObject<Order> ();
        CheckDomainObject (result, _clientTransaction);
      }
    }

    [Test]
    public void TryGetObject_NoClientTransactionGiven_NoCurrentTransaction_Throws ()
    {
      Assert.That (
          () => _orderID1.GetObject<Order> (),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void TryGetObject_AcceptsBaseType ()
    {
      var result = _orderID1.TryGetObject<TestDomainBase> (_clientTransaction);

      CheckDomainObject (result, _clientTransaction);
    }

    [Test]
    public void TryGetObject_ThrowsOnUnrelatedType ()
    {
      Assert.That (
          () => _orderID1.TryGetObject<OrderItem> (_clientTransaction),
          Throws.TypeOf<ArgumentException> ().With.Message.EqualTo (
              "The ObjectID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is not compatible with type "
              + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem'.\r\nParameter name: id"));
    }

    [Test]
    public void GetObjectReference_ReturnsReferenceFromGivenTransaction ()
    {
      var result = _orderID1.GetObjectReference<Order> (_clientTransaction);
      CheckDomainObject (result, _clientTransaction, expectedID: _orderID1, expectedState: StateType.NotLoadedYet);
    }

    [Test]
    public void GetObjectReference_NoClientTransactionGiven_UsesCurrentTransaction ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        var result = _orderID1.GetObjectReference<Order> ();
        CheckDomainObject (result, _clientTransaction);
      }
    }

    [Test]
    public void GetObjectReference_NoClientTransactionGiven_NoCurrentTransaction_Throws ()
    {
      Assert.That (
          () => _orderID1.GetObjectReference<Order> (),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void GetObjectReference_AcceptsBaseType ()
    {
      var result = _orderID1.GetObjectReference<TestDomainBase> (_clientTransaction);

      CheckDomainObject (result, _clientTransaction);
    }

    [Test]
    public void GetObjectReference_ThrowsOnUnrelatedType ()
    {
      Assert.That (
          () => _orderID1.GetObjectReference<OrderItem> (_clientTransaction),
          Throws.TypeOf<ArgumentException> ().With.Message.EqualTo (
              "The ObjectID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is not compatible with type "
              + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem'.\r\nParameter name: id"));
    }

    [Test]
    public void GetObjects_LoadsObjectsIntoGivenTransaction ()
    {
      var results = new[] { _orderID1, _objectID2 }.GetObjects<Order> (_clientTransaction);

      Assert.That (results, Has.Length.EqualTo (2));
      CheckDomainObject (results[0], _clientTransaction, expectedID: _orderID1, expectedState: StateType.Unchanged);
      CheckDomainObject (results[1], _clientTransaction, expectedID: _objectID2, expectedState: StateType.Unchanged);
    }

    [Test]
    public void GetObjects_NoClientTransactionGiven_UsesCurrentTransaction ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        var results = new[] { _orderID1 }.GetObjects<Order> ();

        CheckDomainObject (results[0], _clientTransaction);
      }
    }

    [Test]
    public void GetObjects_NoClientTransactionGiven_NoCurrentTransaction_Throws ()
    {
      Assert.That (
          () => new[] { _orderID1 }.GetObjects<Order> (),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void GetObjects_WithNotFound_Throws()
    {
      Assert.That (() => new[] { _notFoundObjectID }.GetObjects<Order> (_clientTransaction), Throws.TypeOf<ObjectsNotFoundException> ());
    }

    [Test]
    public void GetObjects_AcceptsBaseType ()
    {
      var results = new[] { _orderID1 }.GetObjects<TestDomainBase> (_clientTransaction);

      CheckDomainObject (results.Single(), _clientTransaction);
    }

    [Test]
    public void GetObjects_ThrowsOnUnrelatedType ()
    {
      Assert.That (
          () => new[] { _orderID1 }.GetObjects<OrderItem> (_clientTransaction),
          Throws.TypeOf<ArgumentException> ().With.Message.EqualTo (
              "The ObjectID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is not compatible with type "
              + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem'.\r\nParameter name: id"));
    }

    [Test]
    public void TryGetObjects_LoadsObjectsIntoGivenTransaction ()
    {
      var results = new[] { _orderID1, _objectID2 }.TryGetObjects<Order> (_clientTransaction);

      Assert.That (results, Has.Length.EqualTo (2));
      CheckDomainObject (results[0], _clientTransaction, expectedID: _orderID1, expectedState: StateType.Unchanged);
      CheckDomainObject (results[1], _clientTransaction, expectedID: _objectID2, expectedState: StateType.Unchanged);
    }

    [Test]
    public void TryGetObjects_NoClientTransactionGiven_UsesCurrentTransaction ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        var results = new[] { _orderID1 }.TryGetObjects<Order> ();
        CheckDomainObject (results[0], _clientTransaction);
      }
    }

    [Test]
    public void TryGetObjects_NoClientTransactionGiven_NoCurrentTransaction_Throws ()
    {
      Assert.That (
          () => new[] { _orderID1 }.TryGetObjects<Order> (),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void TryGetObjects_WithNotFound_Throws ()
    {
      var results = new[] { _notFoundObjectID }.TryGetObjects<Order> (_clientTransaction);
      Assert.That (results[0], Is.Null);
    }

    [Test]
    public void TryGetObjects_AcceptsBaseType ()
    {
      var results = new[] { _orderID1 }.TryGetObjects<TestDomainBase> (_clientTransaction);

      CheckDomainObject (results.Single (), _clientTransaction);
    }

    [Test]
    public void TryGetObjects_ThrowsOnUnrelatedType ()
    {
      Assert.That (
          () => new[] { _orderID1 }.TryGetObjects<OrderItem> (_clientTransaction),
          Throws.TypeOf<ArgumentException> ().With.Message.EqualTo (
              "The ObjectID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is not compatible with type "
              + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem'.\r\nParameter name: id"));
    }

    private void CheckDomainObject (
        DomainObject result,
        ClientTransaction expectedClientTransaction,
        ObjectID expectedID = null,
        StateType? expectedState = null)
    {
      Assert.That (expectedClientTransaction.IsEnlisted (result), Is.True);
      if (expectedID != null)
        Assert.That (result.ID, Is.EqualTo (expectedID));
      if (expectedState != null)
        Assert.That (expectedClientTransaction.ExecuteInScope (() => result.State), Is.EqualTo (expectedState));
    }
  }
}