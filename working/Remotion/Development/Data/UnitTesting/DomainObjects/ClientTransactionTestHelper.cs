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
using System.Reflection;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Development.Data.UnitTesting.DomainObjects
{
  public static class ClientTransactionTestHelper
  {
    public static ITransactionHierarchyManager GetHierarchyManager (ClientTransaction clientTransaction)
    {
      return TransactionHierarchyManagerService.GetTransactionHierarchyManager (clientTransaction);
    }

    public static DataManager GetDataManager (ClientTransaction clientTransaction)
    {
      return (DataManager) DataManagementService.GetDataManager (clientTransaction);
    }

    public static IDataManager GetIDataManager (ClientTransaction clientTransaction)
    {
      return DataManagementService.GetDataManager (clientTransaction);
    }

    public static IObjectLifetimeAgent GetObjectLifetimeAgent (ClientTransaction clientTransaction)
    {
      return (IObjectLifetimeAgent) PrivateInvoke.GetNonPublicField (clientTransaction, "_objectLifetimeAgent");
    }

    public static IEnlistedDomainObjectManager GetEnlistedDomainObjectManager (ClientTransaction clientTransaction)
    {
      return (IEnlistedDomainObjectManager) PrivateInvoke.GetNonPublicField (clientTransaction, "_enlistedDomainObjectManager");
    }

    public static IInvalidDomainObjectManager GetInvalidDomainObjectManager (ClientTransaction clientTransaction)
    {
      return (IInvalidDomainObjectManager) PrivateInvoke.GetNonPublicField (clientTransaction, "_invalidDomainObjectManager");
    }

    public static IPersistenceStrategy GetPersistenceStrategy (ClientTransaction clientTransaction)
    {
      return (IPersistenceStrategy) PrivateInvoke.GetNonPublicField (clientTransaction, "_persistenceStrategy");
    }

    public static IClientTransactionEventBroker GetEventBroker (ClientTransaction clientTransaction)
    {
      return (IClientTransactionEventBroker) PrivateInvoke.GetNonPublicField (clientTransaction, "_eventBroker");
    }

    public static ICommitRollbackAgent GetCommitRollbackAgent (ClientTransaction clientTransaction)
    {
      return (ICommitRollbackAgent) PrivateInvoke.GetNonPublicField (clientTransaction, "_commitRollbackAgent");
    }
    public static DomainObject CallGetObject (ClientTransaction clientTransaction, ObjectID objectID, bool includeDeleted)
    {
      return (DomainObject) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "GetObject", objectID, includeDeleted);
    }

    public static DomainObject CallTryGetObject (ClientTransaction clientTransaction, ObjectID objectID)
    {
      return (DomainObject) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "TryGetObject", objectID);
    }

    public static DomainObject CallGetObjectReference (ClientTransaction clientTransaction, ObjectID objectID)
    {
      return (DomainObject) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "GetObjectReference", objectID);
    }

    public static DomainObject CallGetInvalidObjectReference (ClientTransaction clientTransaction, ObjectID objectID)
    {
      return (DomainObject) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "GetInvalidObjectReference", objectID);
    }

    public static DomainObject CallGetRelatedObject (ClientTransaction clientTransaction, RelationEndPointID relationEndPointID)
    {
      return (DomainObject) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "GetRelatedObject", relationEndPointID);
    }

    public static DomainObject CallGetOriginalRelatedObject (ClientTransaction clientTransaction, RelationEndPointID relationEndPointID)
    {
      return (DomainObject) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "GetOriginalRelatedObject", relationEndPointID);
    }

    public static DomainObjectCollection CallGetRelatedObjects (ClientTransaction clientTransaction, RelationEndPointID relationEndPointID)
    {
      return (DomainObjectCollection) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "GetRelatedObjects", relationEndPointID);
    }

    public static DomainObjectCollection CallGetOriginalRelatedObjects (ClientTransaction clientTransaction, RelationEndPointID relationEndPointID)
    {
      return (DomainObjectCollection) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "GetOriginalRelatedObjects", relationEndPointID);
    }

    public static DomainObject CallNewObject (ClientTransaction clientTransaction, Type domainObjectType, ParamList constructorParameters)
    {
      return (DomainObject) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "NewObject", domainObjectType, constructorParameters);
    }

    public static T[] CallGetObjects<T> (ClientTransaction clientTransaction, params ObjectID[] objectIDs)
    {
      // TODO 5118: Use PrivateInvoke when it gets support for generic.
      var method = typeof (ClientTransaction).GetMethod ("GetObjects", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod (typeof (T));
      return (T[]) method.Invoke (clientTransaction, new object[] { objectIDs });
    }

    public static T[] CallTryGetObjects<T> (ClientTransaction clientTransaction, params ObjectID[] objectIDs)
    {
      // TODO 5118: Use PrivateInvoke when it gets support for generic.
      var method = typeof (ClientTransaction).GetMethod ("TryGetObjects", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod (typeof (T));
      return (T[]) method.Invoke (clientTransaction, new object[] { objectIDs });
    }

    public static void AddListener (ClientTransaction clientTransaction, IClientTransactionListener listener)
    {
      PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "AddListener", listener);
    }

    public static void RemoveListener (ClientTransaction clientTransaction, IClientTransactionListener listener)
    {
      PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "RemoveListener", listener);
    }

    public static void RegisterDataContainer (ClientTransaction clientTransaction, DataContainer dataContainer)
    {
      if (!dataContainer.HasDomainObject)
      {
        var objectReference = LifetimeService.GetObjectReference (clientTransaction, dataContainer.ID);
        dataContainer.SetDomainObject (objectReference);
      }

      var dataManager = GetDataManager (clientTransaction);
      dataManager.RegisterDataContainer (dataContainer);
    }

    public static void SetIsWriteable (ClientTransaction transaction, bool value)
    {
      var hierarchyManager = (TransactionHierarchyManager) GetHierarchyManager (transaction);
      TransactionHierarchyManagerTestHelper.SetIsWriteable (hierarchyManager, value);
    }

    public static void SetSubTransaction (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      var hierarchyManager = (TransactionHierarchyManager) GetHierarchyManager (clientTransaction);
      TransactionHierarchyManagerTestHelper.SetSubtransaction(hierarchyManager, subTransaction);
    }

    public static void ClearAllListeners (ClientTransaction clientTransaction)
    {
      var listenerManager = GetEventBroker (clientTransaction);
      foreach (var listener in listenerManager.Listeners.ToArray ().Reverse ())
        listenerManager.RemoveListener (listener);
    }

    public static IEnumerable<IClientTransactionListener> GetListeners (ClientTransaction clientTransaction)
    {
      var listenerManager = GetEventBroker (clientTransaction);
      return listenerManager.Listeners;
    }

    public static IDisposable MakeInactive (ClientTransaction inactiveTransaction)
    {
      var scope = inactiveTransaction.CreateSubTransaction().EnterNonDiscardingScope();
      Assertion.IsFalse (ReferenceEquals (inactiveTransaction.ActiveTransaction, inactiveTransaction), "The transaction can no longer be active.");
      return scope;
    }
  }
}