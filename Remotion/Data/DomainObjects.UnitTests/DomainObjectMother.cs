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
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public static class DomainObjectMother
  {
    public static T CreateObjectInTransaction<T> (ClientTransaction transaction) where T : DomainObject
    {
      return (T)LifetimeService.NewObject(transaction, typeof(T), ParamList.Empty);
    }

    public static T CreateObjectInOtherTransaction<T> () where T : DomainObject
    {
      return CreateObjectInTransaction<T>(ClientTransaction.CreateRootTransaction());
    }

    public static T GetObjectInOtherTransaction<T> (ObjectID objectID) where T : DomainObject
    {
      var transaction = ClientTransaction.CreateRootTransaction();
      return GetObjectInTransaction<T>(transaction, objectID);
    }

    public static T GetObjectInTransaction<T> (ClientTransaction transaction, ObjectID objectID) where T : DomainObject
    {
      return (T)LifetimeService.GetObject(transaction, objectID, true);
    }

    public static T CreateFakeObject<T> () where T: DomainObject
    {
      return CreateObjectInOtherTransaction<T>();
    }

    public static T CreateFakeObject<T> (ObjectID id) where T : DomainObject
    {
      return GetObjectReference<T>(ClientTransaction.CreateRootTransaction(), id);
    }

    public static DomainObject CreateFakeObject (ObjectID id = null)
    {
      return (DomainObject)LifetimeService.GetObjectReference(ClientTransaction.CreateRootTransaction(), id ?? new ObjectID(typeof(Order), Guid.NewGuid()));
    }

    public static T GetObjectReference<T> (ClientTransaction clientTransaction, ObjectID objectID) where T : DomainObject
    {
      return (T)LifetimeService.GetObjectReference(clientTransaction, objectID);
    }

    public static DomainObject GetObjectReference (ClientTransaction clientTransaction, ObjectID objectID)
    {
      return GetObjectReference<DomainObject>(clientTransaction, objectID);
    }

    public static DomainObject GetChangedObject (ClientTransaction transaction, ObjectID objectID)
    {
      var changedInstance = (DomainObject)LifetimeService.GetObject(transaction, objectID, false);
      changedInstance.RegisterForCommit();
      Assert.That(changedInstance.State.IsChanged, Is.True);
      Assert.That(ClientTransactionTestHelper.GetDataManager(transaction).DataContainers[objectID].State.IsChanged, Is.True);
      return changedInstance;
    }

    public static DomainObject GetUnchangedObject (ClientTransaction transaction, ObjectID objectID)
    {
      var unchangedInstance = (DomainObject)LifetimeService.GetObject(transaction, objectID, false);
      Assert.That(unchangedInstance.State.IsUnchanged, Is.True);
      return unchangedInstance;
    }

    public static DomainObject GetInvalidObject (ClientTransaction transaction)
    {
      var invalidInstance = (DomainObject)LifetimeService.NewObject(transaction, typeof(Order), ParamList.Empty);
      LifetimeService.DeleteObject(transaction, invalidInstance);
      Assert.That(invalidInstance.TransactionContext[transaction].State.IsInvalid, Is.True);
      return invalidInstance;
    }

    public static DomainObject GetNotLoadedObject (ClientTransaction transaction, ObjectID objectID)
    {
      var notLoadedInstance = (DomainObject)LifetimeService.GetObjectReference(transaction, objectID);
      Assert.That(notLoadedInstance.TransactionContext[transaction].State.IsNotLoadedYet, Is.True);
      return notLoadedInstance;
    }

    public static DomainObject GetNewObject ()
    {
      var newInstance = ClassWithAllDataTypes.NewObject();
      Assert.That(newInstance.State.IsNew, Is.True);
      return newInstance;
    }

    public static DomainObject GetDeletedObject (ClientTransaction transaction, ObjectID objectID)
    {
      var deletedInstance = (DomainObject)LifetimeService.GetObjectReference(transaction, objectID);
      LifetimeService.DeleteObject(transaction, deletedInstance);
      Assert.That(deletedInstance.TransactionContext[transaction].State.IsDeleted, Is.True);
      return deletedInstance;
    }
  }
}
