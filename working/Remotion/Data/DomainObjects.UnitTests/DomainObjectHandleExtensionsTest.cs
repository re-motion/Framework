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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class DomainObjectHandleExtensionsTest : StandardMappingTest
  {
    private IDomainObjectHandle<Order> _orderHandle1;
    private IDomainObjectHandle<Order> _orderHandle2;
    private IDomainObjectHandle<Order> _notFoundOrderHandle;
    private ClientTransaction _clientTransaction;

    public override void SetUp ()
    {
      base.SetUp ();
      _orderHandle1 = DomainObjectIDs.Order1.GetHandle<Order>();
      _orderHandle2 = DomainObjectIDs.Order2.GetHandle<Order> ();
      _notFoundOrderHandle = new ObjectID (typeof (Order), Guid.NewGuid()).GetHandle<Order>();
      _clientTransaction = ClientTransaction.CreateRootTransaction();
    }

    [Test]
    public void GetObject_LoadsObjectIntoGivenTransaction ()
    {
      var result = _orderHandle1.GetObject (_clientTransaction);

      CheckDomainObject (result, _clientTransaction, expectedID: _orderHandle1.ObjectID, expectedState: StateType.Unchanged);
    }

    [Test]
    public void GetObject_NoClientTransactionGiven_UsesCurrentTransaction ()
    {
      using (_clientTransaction.EnterNonDiscardingScope())
      {
        var result = _orderHandle1.GetObject();
        CheckDomainObject (result, _clientTransaction);
      }
    }

    [Test]
    public void GetObject_NoClientTransactionGiven_NoCurrentTransaction_Throws ()
    {
      Assert.That (
          () => _orderHandle1.GetObject(),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void GetObject_IncludeDeletedTrue_LoadsDeletedObject ()
    {
      _clientTransaction.ExecuteInScope (() => _orderHandle1.GetObject().Delete());

      var result = _orderHandle1.GetObject (_clientTransaction, includeDeleted: true);

      Assert.That (result, Is.Not.Null);
      CheckDomainObject (result, _clientTransaction, expectedState: StateType.Deleted);
    }

    [Test]
    public void GetObject_IncludeDeletedFalse_ThrowsOnDeletedObject ()
    {
      _clientTransaction.ExecuteInScope (() => _orderHandle1.GetObject ().Delete ());
      Assert.That (() => _orderHandle1.GetObject (_clientTransaction, includeDeleted: false), Throws.TypeOf<ObjectDeletedException>());
    }

    [Test]
    public void GetObject_IncludeDeletedUnspecified_ThrowsOnDeletedObject ()
    {
      _clientTransaction.ExecuteInScope (() => _orderHandle1.GetObject ().Delete ());
      Assert.That (() => _orderHandle1.GetObject (_clientTransaction), Throws.TypeOf<ObjectDeletedException> ());
    }

    [Test]
    public void TryGetObject_LoadsObjectIntoGivenTransaction ()
    {
      var result = _orderHandle1.TryGetObject (_clientTransaction);
      CheckDomainObject (result, _clientTransaction, expectedID: _orderHandle1.ObjectID, expectedState: StateType.Unchanged);
    }

    [Test]
    public void TryGetObject_AllowsNotFoundObjects ()
    {
      var result = _notFoundOrderHandle.TryGetObject (_clientTransaction);
      Assert.That (result, Is.Null);
    }

    [Test]
    public void TryGetObject_NoClientTransactionGiven_UsesCurrentTransaction ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        var result = _orderHandle1.TryGetObject ();
        CheckDomainObject (result, _clientTransaction);
      }
    }

    [Test]
    public void TryGetObject_NoClientTransactionGiven_NoCurrentTransaction_Throws ()
    {
      Assert.That (
          () => _orderHandle1.TryGetObject (),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void GetObjectReference_ReturnsReferenceFromGivenTransaction ()
    {
      var result = _orderHandle1.GetObjectReference (_clientTransaction);
      CheckDomainObject (result, _clientTransaction, expectedID: _orderHandle1.ObjectID, expectedState: StateType.NotLoadedYet);
    }

    [Test]
    public void GetObjectReference_NoClientTransactionGiven_UsesCurrentTransaction ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        var result = _orderHandle1.GetObjectReference ();
        CheckDomainObject (result, _clientTransaction);
      }
    }

    [Test]
    public void GetObjectReference_NoClientTransactionGiven_NoCurrentTransaction_Throws ()
    {
      Assert.That (
          () => _orderHandle1.GetObjectReference (),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void GetObjects_LoadsObjectsIntoGivenTransaction ()
    {
      var results = new[] { _orderHandle1, _orderHandle2 }.GetObjects (_clientTransaction);

      Assert.That (results, Has.Length.EqualTo (2));
      CheckDomainObject (results[0], _clientTransaction, expectedID: _orderHandle1.ObjectID, expectedState: StateType.Unchanged);
      CheckDomainObject (results[1], _clientTransaction, expectedID: _orderHandle2.ObjectID, expectedState: StateType.Unchanged);
    }

    [Test]
    public void GetObjects_NoClientTransactionGiven_UsesCurrentTransaction ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        var results = new[] { _orderHandle1 }.GetObjects ();

        CheckDomainObject (results[0], _clientTransaction);
      }
    }

    [Test]
    public void GetObjects_NoClientTransactionGiven_NoCurrentTransaction_Throws ()
    {
      Assert.That (
          () => new[] { _orderHandle1 }.GetObjects (),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void GetObjects_WithNotFound_Throws()
    {
      Assert.That (() => new[] { _notFoundOrderHandle }.GetObjects (_clientTransaction), Throws.TypeOf<ObjectsNotFoundException>());
    }

    [Test]
    public void TryGetObjects_LoadsObjectsIntoGivenTransaction ()
    {
      var results = new[] { _orderHandle1, _orderHandle2 }.TryGetObjects (_clientTransaction);

      Assert.That (results, Has.Length.EqualTo (2));
      CheckDomainObject (results[0], _clientTransaction, expectedID: _orderHandle1.ObjectID, expectedState: StateType.Unchanged);
      CheckDomainObject (results[1], _clientTransaction, expectedID: _orderHandle2.ObjectID, expectedState: StateType.Unchanged);
    }

    [Test]
    public void TryGetObjects_NoClientTransactionGiven_UsesCurrentTransaction ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        var results = new[] { _orderHandle1 }.TryGetObjects ();
        CheckDomainObject (results[0], _clientTransaction);
      }
    }

    [Test]
    public void TryGetObjects_NoClientTransactionGiven_NoCurrentTransaction_Throws ()
    {
      Assert.That (
          () => new[] { _orderHandle1 }.TryGetObjects (),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));
    }

    [Test]
    public void TryGetObjects_WithNotFound_Throws ()
    {
      var results = new[] { _notFoundOrderHandle }.TryGetObjects (_clientTransaction);
      Assert.That (results[0], Is.Null);
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