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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Logging;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// A listener implementation logging all transaction events.
  /// </summary>
  public class LoggingClientTransactionListener : IClientTransactionListener
  {
    private static readonly ILog s_log = LogManager.GetLogger(typeof(LoggingClientTransactionListener));

    public void TransactionInitialize (ClientTransaction clientTransaction)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} TransactionInitialize", clientTransaction.ID);
    }

    public void TransactionDiscard (ClientTransaction clientTransaction)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} TransactionDiscard", clientTransaction.ID);
    }

    public void SubTransactionCreating (ClientTransaction clientTransaction)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} SubTransactionCreating", clientTransaction.ID);
    }

    public void SubTransactionInitialize (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} SubTransactionInitialize: {1}", clientTransaction.ID, subTransaction.ID);
    }

    public void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} SubTransactionCreated: {1}", clientTransaction.ID, subTransaction.ID);
    }

    public void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} NewObjectCreating: {1}", clientTransaction.ID, type.GetFullNameSafe());
    }

    public void ObjectsLoading (ClientTransaction clientTransaction, IReadOnlyList<ObjectID> objectIDs)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} ObjectsLoading: {1}", clientTransaction.ID, GetObjectIDString(objectIDs));
    }

    public void ObjectsLoaded (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} ObjectsLoaded: {1}", clientTransaction.ID, GetDomainObjectsString(domainObjects));
    }

    public void ObjectsNotFound (ClientTransaction clientTransaction, IReadOnlyList<ObjectID> objectIDs)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} ObjectsNotFound: {1}", clientTransaction.ID, GetObjectIDString(objectIDs));
    }

    public void ObjectsUnloaded (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} ObjectsUnloaded: {1}", clientTransaction.ID, GetDomainObjectsString(unloadedDomainObjects));
    }

    public void ObjectsUnloading (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} ObjectsUnloading: {1}", clientTransaction.ID, GetDomainObjectsString(unloadedDomainObjects));
    }

    public void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} ObjectDeleting: {1}", clientTransaction.ID, GetDomainObjectString(domainObject));
    }

    public void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} ObjectDeleted: {1}", clientTransaction.ID, GetDomainObjectString(domainObject));
    }

    public void PropertyValueReading (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
      if (s_log.IsDebugEnabled())
      {
        s_log.DebugFormat(
            "{0} PropertyValueReading: {1} ({2}, {3})",
            clientTransaction.ID,
            propertyDefinition.PropertyName,
            valueAccess,
            domainObject.ID);
      }
    }

    public void PropertyValueRead (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object? value, ValueAccess valueAccess)
    {
      if (s_log.IsDebugEnabled())
      {
        s_log.DebugFormat(
            "{0} PropertyValueRead: {1}=={2} ({3}, {4})",
            clientTransaction.ID,
            propertyDefinition.PropertyName,
            value ?? "<null>",
            valueAccess,
            domainObject.ID);
      }
    }

    public void PropertyValueChanging (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object? oldValue, object? newValue)
    {
      if (s_log.IsDebugEnabled())
      {
        s_log.DebugFormat(
            "{0} PropertyValueChanging: {1} {2}->{3} ({4})",
            clientTransaction.ID,
            propertyDefinition.PropertyName,
            oldValue ?? "<null>",
            newValue ?? "<null>",
            domainObject.ID);
      }
    }

    public void PropertyValueChanged (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object? oldValue, object? newValue)
    {
      if (s_log.IsDebugEnabled())
      {
        s_log.DebugFormat(
            "{0} PropertyValueChanged: {1} {2}->{3} ({4})",
            clientTransaction.ID,
            propertyDefinition.PropertyName,
            oldValue ?? "<null>",
            newValue ?? "<null>",
            domainObject.ID);
      }
    }

    public void RelationReading (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        ValueAccess valueAccess)
    {
      if (s_log.IsDebugEnabled())
      {
        s_log.DebugFormat(
            "{0} RelationReading: {1} ({2}, {3})",
            clientTransaction.ID,
            relationEndPointDefinition.PropertyName,
            valueAccess,
            GetDomainObjectString(domainObject));
      }
    }

    public void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? relatedObject,
        ValueAccess valueAccess)
    {
      if (s_log.IsDebugEnabled())
      {
        s_log.DebugFormat(
            "{0} RelationRead: {1}=={2} ({3}, {4})",
            clientTransaction.ID,
            relationEndPointDefinition.PropertyName,
            GetDomainObjectString(relatedObject),
            valueAccess,
            GetDomainObjectString(domainObject));
      }
    }

    public void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IReadOnlyCollectionData<DomainObject> relatedObjects,
        ValueAccess valueAccess)
    {
      if (s_log.IsDebugEnabled())
      {
        var domainObjectsString = relatedObjects.IsDataComplete ? GetDomainObjectsString(relatedObjects) : "<data not loaded>";
        s_log.DebugFormat(
            "{0} RelationRead: {1} ({2}, {3}): {4}",
            clientTransaction.ID,
            relationEndPointDefinition.PropertyName,
            valueAccess,
            domainObject.ID,
            domainObjectsString);
      }
    }

    public void RelationChanging (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? oldRelatedObject,
        DomainObject? newRelatedObject)
    {
      if (s_log.IsDebugEnabled())
      {
        s_log.DebugFormat(
            "{0} RelationChanging: {1}: {2}->{3} /{4}",
            clientTransaction.ID,
            relationEndPointDefinition.PropertyName,
            GetDomainObjectString(oldRelatedObject),
            GetDomainObjectString(newRelatedObject),
            GetDomainObjectString(domainObject));
      }
    }

    public void RelationChanged (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? oldRelatedObject,
        DomainObject? newRelatedObject)
    {
      if (s_log.IsDebugEnabled())
      {
        s_log.DebugFormat(
            "{0} RelationChanged: {1}: {2}->{3} /{4}",
            clientTransaction.ID,
            relationEndPointDefinition.PropertyName,
            GetDomainObjectString(oldRelatedObject),
            GetDomainObjectString(newRelatedObject),
            GetDomainObjectString(domainObject));
      }
    }

    public QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T: DomainObject
    {
      if (s_log.IsDebugEnabled())
      {
        s_log.DebugFormat(
            "{0} FilterQueryResult: {1} ({2}): {3}",
            clientTransaction.ID,
            queryResult.Query.ID,
            queryResult.Query.Statement,
            GetDomainObjectsString(queryResult.AsEnumerable().Cast<DomainObject>()));
      }
      return queryResult;
    }

    public IEnumerable<T> FilterCustomQueryResult<T> (ClientTransaction clientTransaction, IQuery query, IEnumerable<T> results)
    {
      if (s_log.IsDebugEnabled())
      {
        s_log.DebugFormat(
            "{0} FilterCustomQueryResult: {1} ({2})",
            clientTransaction.ID,
            query.ID,
            query.Statement);
      }
      return results;
    }

    public void TransactionCommitting (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} TransactionCommitting: {1}", clientTransaction.ID, GetDomainObjectsString(domainObjects));
    }

    public void TransactionCommitValidate (ClientTransaction clientTransaction, IReadOnlyList<PersistableData> committedData)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} TransactionCommitValidate: {1}", clientTransaction.ID, GetDomainObjectsString(committedData.Select(pd => pd.DomainObject)));
    }

    public void TransactionCommitted (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} TransactionCommitted: {1}", clientTransaction.ID, GetDomainObjectsString(domainObjects));
    }

    public void TransactionRollingBack (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} TransactionRollingBack: {1}", clientTransaction.ID, GetDomainObjectsString(domainObjects));
    }

    public void TransactionRolledBack (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> domainObjects)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} TransactionRolledBack: {1}", clientTransaction.ID, GetDomainObjectsString(domainObjects));
    }

    public void RelationEndPointMapRegistering (ClientTransaction clientTransaction, IRelationEndPoint endPoint)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} RelationEndPointMapRegistering: {1}", clientTransaction.ID, endPoint.ID);
    }

    public void RelationEndPointMapUnregistering (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} RelationEndPointMapUnregistering: {1}", clientTransaction.ID, endPointID);
    }

    public void RelationEndPointBecomingIncomplete (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} RelationEndPointBecomingIncomplete: {1}", clientTransaction.ID, endPointID);
    }

    public void ObjectMarkedInvalid (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} ObjectMarkedInvalid: {1}", clientTransaction.ID, GetDomainObjectString(domainObject));
    }

    public void ObjectMarkedNotInvalid (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} ObjectMarkedNotInvalid: {1}", clientTransaction.ID, GetDomainObjectString(domainObject));
    }

    public void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} DataContainerMapRegistering: {1}", clientTransaction.ID, container.ID);
    }

    public void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} DataContainerMapUnregistering: {1}", clientTransaction.ID, container.ID);
    }

    public void DataContainerStateUpdated (ClientTransaction clientTransaction, DataContainer container, DataContainerState newDataContainerState)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} DataContainerStateUpdated: {1} {2}", clientTransaction.ID, container.ID, newDataContainerState);
    }

    public void VirtualRelationEndPointStateUpdated (ClientTransaction clientTransaction, RelationEndPointID endPointID, bool? newEndPointChangeState)
    {
      if (s_log.IsDebugEnabled())
        s_log.DebugFormat("{0} VirtualRelationEndPointStateUpdated: {1} {2}", clientTransaction.ID, endPointID, newEndPointChangeState);
    }

    private string GetObjectIDString (IEnumerable<ObjectID> objectIDs)
    {
      return string.Join(", ", ConvertToStringAndCount(objectIDs, 10, GetObjectIDString));
    }

    private string GetObjectIDString (ObjectID? id)
    {
      return id != null ? id.ToString()! : "<null>";
    }

    private string GetDomainObjectsString (IEnumerable<DomainObject> domainObjects)
    {
      return string.Join(", ", ConvertToStringAndCount(domainObjects, 10, GetDomainObjectString));
    }

    private string GetDomainObjectString (DomainObject? domainObject)
    {
      return GetObjectIDString(domainObject.GetSafeID());
    }

    private IEnumerable<string> ConvertToStringAndCount<T> (IEnumerable<T> sequence, int maximumCount, Func<T, string> converter)
    {
      using (var enumerator = sequence.GetEnumerator())
      {
        int i = 0;
        while ( i < maximumCount && enumerator.MoveNext())
        {
          ++i;
          yield return converter(enumerator.Current);
        }
        if (i == maximumCount)
        {
          i = 0;
          while (enumerator.MoveNext())
          {
            ++i;
          }
          yield return "+" + i;
        }
      }
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
