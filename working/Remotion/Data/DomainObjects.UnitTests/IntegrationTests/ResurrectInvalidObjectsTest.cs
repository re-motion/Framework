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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class ResurrectInvalidObjectsTest : ClientTransactionBaseTest
  {
    [Test]
    public void NotFoundObject ()
    {
      var notFoundID = new ObjectID(typeof (Order), Guid.NewGuid());
      var notFoundObject = LifetimeService.GetObjectReference (TestableClientTransaction, notFoundID);
      notFoundObject.TryEnsureDataAvailable();

      CheckStateIsInvalid (notFoundObject, TestableClientTransaction);

      ResurrectionService.ResurrectInvalidObject (TestableClientTransaction, notFoundID);

      CheckStateIsNotInvalid (notFoundObject, TestableClientTransaction, StateType.NotLoadedYet);
    }

    [Test]
    public void NotFoundObject_TryResurrect ()
    {
      var notFoundID = new ObjectID(typeof (Order), Guid.NewGuid ());
      var notFoundObject = LifetimeService.GetObjectReference (TestableClientTransaction, notFoundID);
      notFoundObject.TryEnsureDataAvailable ();

      CheckStateIsInvalid (notFoundObject, TestableClientTransaction);

      var result = ResurrectionService.TryResurrectInvalidObject (TestableClientTransaction, notFoundID);

      Assert.That (result, Is.True);
      CheckStateIsNotInvalid (notFoundObject, TestableClientTransaction, StateType.NotLoadedYet);
    }

    [Test]
    public void DiscardedNewObject ()
    {
      var newObject = Order.NewObject();
      newObject.Delete();

      CheckStateIsInvalid (newObject, TestableClientTransaction);

      ResurrectionService.ResurrectInvalidObject (TestableClientTransaction, newObject.ID);

      CheckStateIsNotInvalid (newObject, TestableClientTransaction, StateType.NotLoadedYet);
    }

    [Test]
    public void DiscardedNewObject_TryResurrect ()
    {
      var newObject = Order.NewObject ();
      newObject.Delete ();

      CheckStateIsInvalid (newObject, TestableClientTransaction);

      var result = ResurrectionService.TryResurrectInvalidObject (TestableClientTransaction, newObject.ID);

      Assert.That (result, Is.True);
      CheckStateIsNotInvalid (newObject, TestableClientTransaction, StateType.NotLoadedYet);
    }

    [Test]
    public void DiscardedDeletedObject ()
    {
      SetDatabaseModifyable();

      var deletedObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ();
      deletedObject.Delete ();
      TestableClientTransaction.Commit();

      CheckStateIsInvalid (deletedObject, TestableClientTransaction);

      ResurrectionService.ResurrectInvalidObject (TestableClientTransaction, deletedObject.ID);

      CheckStateIsNotInvalid (deletedObject, TestableClientTransaction, StateType.NotLoadedYet);
    }

    [Test]
    public void DiscardedDeletedObject_TryResurrect ()
    {
      SetDatabaseModifyable ();

      var deletedObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ();
      deletedObject.Delete ();
      TestableClientTransaction.Commit ();

      CheckStateIsInvalid (deletedObject, TestableClientTransaction);

      var result = ResurrectionService.TryResurrectInvalidObject (TestableClientTransaction, deletedObject.ID);

      Assert.That (result, Is.True);
      CheckStateIsNotInvalid (deletedObject, TestableClientTransaction, StateType.NotLoadedYet);
    }

    [Test]
    public void NewInDescendant ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope ())
      {
        var newObject = Order.NewObject();

        CheckStateIsInvalid (newObject, TestableClientTransaction);
        CheckStateIsNotInvalid (newObject, subTransaction, StateType.New);

        Assert.That (
            () => ResurrectionService.ResurrectInvalidObject (TestableClientTransaction, newObject.ID),
            Throws.InvalidOperationException.With.Message.EqualTo (
                "Cannot resurrect object '" + newObject.ID + "' because it is not invalid within the whole transaction hierarchy. "
                + "In transaction '" + subTransaction + "', the object has state 'New'."));

        var result = ResurrectionService.TryResurrectInvalidObject (TestableClientTransaction, newObject.ID);
        Assert.That (result, Is.False);
      }
    }

    [Test]
    public void DeletedInParent ()
    {
      var deletedObject = DomainObjectIDs.Order1.GetObject<Order> ();
      deletedObject.Delete();

      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        CheckStateIsInvalid (deletedObject, subTransaction);
        CheckStateIsNotInvalid (deletedObject, TestableClientTransaction, StateType.Deleted);

        Assert.That (
            () => ResurrectionService.ResurrectInvalidObject (TestableClientTransaction, deletedObject.ID),
            Throws.InvalidOperationException.With.Message.EqualTo (
                "Cannot resurrect object '" + deletedObject.ID + "' because it is not invalid within the whole transaction hierarchy. "
                + "In transaction '" + TestableClientTransaction + "', the object has state 'Deleted'."));

        var result = ResurrectionService.TryResurrectInvalidObject (TestableClientTransaction, deletedObject.ID);
        Assert.That (result, Is.False);
      }
    }

    [Test]
    public void NotInvalidAtAll ()
    {
      var notInvalidObject = DomainObjectIDs.Order1.GetObject<Order> ();

      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        CheckStateIsNotInvalid (notInvalidObject, TestableClientTransaction, StateType.Unchanged);
        CheckStateIsNotInvalid (notInvalidObject, subTransaction, StateType.NotLoadedYet);

        Assert.That (
            () => ResurrectionService.ResurrectInvalidObject (TestableClientTransaction, notInvalidObject.ID),
            Throws.InvalidOperationException.With.Message.EqualTo (
                "Cannot resurrect object '" + notInvalidObject.ID + "' because it is not invalid within the whole transaction hierarchy. "
                + "In transaction '" + subTransaction + "', the object has state 'NotLoadedYet'."));

        var result = ResurrectionService.TryResurrectInvalidObject (TestableClientTransaction, notInvalidObject.ID);
        Assert.That (result, Is.False);
      }
    }

    [Test]
    public void ResurrectObject_ViaParentTransaction ()
    {
      var notFoundID = new ObjectID(typeof (Order), Guid.NewGuid ());
      var notFoundObject = LifetimeService.GetObjectReference (TestableClientTransaction, notFoundID);

      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        notFoundObject.TryEnsureDataAvailable();

        CheckStateIsInvalid (notFoundObject, TestableClientTransaction);
        CheckStateIsInvalid (notFoundObject, subTransaction);

        ResurrectionService.ResurrectInvalidObject (TestableClientTransaction, notFoundID);

        CheckStateIsNotInvalid (notFoundObject, TestableClientTransaction, StateType.NotLoadedYet);
        CheckStateIsNotInvalid (notFoundObject, subTransaction, StateType.NotLoadedYet);
      }
    }

    [Test]
    public void ResurrectObject_ViaSubTransaction ()
    {
      var notFoundID = new ObjectID(typeof (Order), Guid.NewGuid ());
      var notFoundObject = LifetimeService.GetObjectReference (TestableClientTransaction, notFoundID);

      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        notFoundObject.TryEnsureDataAvailable ();

        CheckStateIsInvalid (notFoundObject, TestableClientTransaction);
        CheckStateIsInvalid (notFoundObject, subTransaction);

        ResurrectionService.ResurrectInvalidObject (subTransaction, notFoundID);

        CheckStateIsNotInvalid (notFoundObject, TestableClientTransaction, StateType.NotLoadedYet);
        CheckStateIsNotInvalid (notFoundObject, subTransaction, StateType.NotLoadedYet);
      }
    }

    [Test]
    public void TryResurrectObject_ViaParentTransaction ()
    {
      var notFoundID = new ObjectID(typeof (Order), Guid.NewGuid ());
      var notFoundObject = LifetimeService.GetObjectReference (TestableClientTransaction, notFoundID);

      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        notFoundObject.TryEnsureDataAvailable ();

        CheckStateIsInvalid (notFoundObject, TestableClientTransaction);
        CheckStateIsInvalid (notFoundObject, subTransaction);

        var result = ResurrectionService.TryResurrectInvalidObject (TestableClientTransaction, notFoundID);

        Assert.That (result, Is.True);
        CheckStateIsNotInvalid (notFoundObject, TestableClientTransaction, StateType.NotLoadedYet);
        CheckStateIsNotInvalid (notFoundObject, subTransaction, StateType.NotLoadedYet);
      }
    }

    [Test]
    public void TryResurrectObject_ViaSubTransaction ()
    {
      var notFoundID = new ObjectID(typeof (Order), Guid.NewGuid ());
      var notFoundObject = LifetimeService.GetObjectReference (TestableClientTransaction, notFoundID);

      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        notFoundObject.TryEnsureDataAvailable ();

        CheckStateIsInvalid (notFoundObject, TestableClientTransaction);
        CheckStateIsInvalid (notFoundObject, subTransaction);

        var result = ResurrectionService.TryResurrectInvalidObject (subTransaction, notFoundID);

        Assert.That (result, Is.True);
        CheckStateIsNotInvalid (notFoundObject, TestableClientTransaction, StateType.NotLoadedYet);
        CheckStateIsNotInvalid (notFoundObject, subTransaction, StateType.NotLoadedYet);
      }
    }

    private void CheckStateIsNotInvalid (DomainObject domainObject, ClientTransaction clientTransaction, StateType expectedState)
    {
      Assert.That (clientTransaction.IsInvalid (domainObject.ID), Is.False);
      Assert.That (domainObject.TransactionContext[clientTransaction].IsInvalid, Is.False);
      Assert.That (domainObject.TransactionContext[clientTransaction].State, Is.EqualTo (expectedState));
    }

    private void CheckStateIsInvalid (DomainObject domainObject, ClientTransaction clientTransaction)
    {
      Assert.That (clientTransaction.IsInvalid (domainObject.ID), Is.True);
      Assert.That (domainObject.TransactionContext[clientTransaction].IsInvalid, Is.True);
      Assert.That (domainObject.TransactionContext[clientTransaction].State, Is.EqualTo (StateType.Invalid));
    }
  }
}