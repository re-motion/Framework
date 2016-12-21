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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Implements the <see cref="IDomainObjectCollectionData"/> by forwarding all requests to an implementation of 
  /// <see cref="ICollectionEndPoint"/>.
  /// </summary>
  [Serializable]
  public class EndPointDelegatingCollectionData : IDomainObjectCollectionData
  {
    private readonly RelationEndPointID _endPointID;
    private readonly IVirtualEndPointProvider _virtualEndPointProvider;

    public EndPointDelegatingCollectionData (RelationEndPointID endPointID, IVirtualEndPointProvider virtualEndPointProvider)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("virtualEndPointProvider", virtualEndPointProvider);

      if (endPointID.Definition.Cardinality != CardinalityType.Many)
        throw new ArgumentException ("Associated end-point must be a CollectionEndPoint.", "endPointID");

      _endPointID = endPointID;
      _virtualEndPointProvider = virtualEndPointProvider;
    }

    public IVirtualEndPointProvider VirtualEndPointProvider
    {
      get { return _virtualEndPointProvider; }
    }

    public int Count
    {
      get
      {
        var data = GetAssociatedEndPoint().GetData();
        return data.Count;
      }
    }

    public Type RequiredItemType
    {
      get 
      {
        // Currently, the data backing a CollectionEndPoint does not check the item type.
        // This is hard-coded (rather than delegating to _associatedEndPoint.GetData().RequiredItemType) to avoid lazy loading for
        // item type checks.
        return null; 
      }
    }

    public bool IsReadOnly
    {
      get 
      { 
        // Currently, the data backing a CollectionEndPoint is never read-only.
        // This is hard-coded (rather than delegating to _associatedEndPoint.GetData().IsReadOnly) because that always returns a read-only
        // decorator.
        return false; 
      }
    }

    public RelationEndPointID AssociatedEndPointID
    {
      get { return _endPointID; }
    }

    public ICollectionEndPoint GetAssociatedEndPoint ()
    {
      return (ICollectionEndPoint) _virtualEndPointProvider.GetOrCreateVirtualEndPoint (_endPointID);
    }

    public bool IsDataComplete
    {
      get { return GetAssociatedEndPoint().IsDataComplete; }
    }


    public void EnsureDataComplete ()
    {
      GetAssociatedEndPoint().EnsureDataComplete ();
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var data = GetAssociatedEndPoint().GetData ();
      return data.ContainsObjectID (objectID);
    }

    public DomainObject GetObject (int index)
    {
      var data = GetAssociatedEndPoint().GetData ();
      return data.GetObject (index);
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var data = GetAssociatedEndPoint().GetData ();
      return data.GetObject (objectID);
    }

    public int IndexOf (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var data = GetAssociatedEndPoint().GetData ();
      return data.IndexOf(objectID);
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      var data = GetAssociatedEndPoint().GetData ();
      return data.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public void Clear ()
    {
      DomainObjectCheckUtility.EnsureNotDeleted (GetAssociatedEndPoint().GetDomainObjectReference (), GetAssociatedEndPoint().ClientTransaction);

      var combinedCommand = GetClearCommand ();
      combinedCommand.NotifyAndPerform ();
    }

    public void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      CheckClientTransaction (domainObject, "Cannot insert DomainObject '{0}' into collection of property '{1}' of DomainObject '{2}'.");
      DomainObjectCheckUtility.EnsureNotDeleted (domainObject, GetAssociatedEndPoint().ClientTransaction);
      DomainObjectCheckUtility.EnsureNotDeleted (GetAssociatedEndPoint().GetDomainObjectReference(), GetAssociatedEndPoint().ClientTransaction);

      var insertCommand = GetAssociatedEndPoint().CreateInsertCommand (domainObject, index);
      var bidirectionalModification = insertCommand.ExpandToAllRelatedObjects ();
      bidirectionalModification.NotifyAndPerform ();

      GetAssociatedEndPoint().Touch ();
    }

    public bool Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      CheckClientTransaction (domainObject, "Cannot remove DomainObject '{0}' from collection of property '{1}' of DomainObject '{2}'.");
      DomainObjectCheckUtility.EnsureNotDeleted (domainObject, GetAssociatedEndPoint().ClientTransaction);
      DomainObjectCheckUtility.EnsureNotDeleted (GetAssociatedEndPoint().GetDomainObjectReference (), GetAssociatedEndPoint().ClientTransaction);

      var containsObjectID = ContainsObjectID (domainObject.ID);
      if (containsObjectID)
        CreateAndExecuteRemoveCommand (domainObject);

      GetAssociatedEndPoint().Touch ();
      return containsObjectID;
    }

    public bool Remove (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      DomainObjectCheckUtility.EnsureNotDeleted (GetAssociatedEndPoint().GetDomainObjectReference(), GetAssociatedEndPoint().ClientTransaction);

      var domainObject = GetObject (objectID);
      if (domainObject != null)
      {
        // we can rely on the fact that this object is not deleted, otherwise we wouldn't have got it
        Assertion.IsTrue (domainObject.TransactionContext[GetAssociatedEndPoint().ClientTransaction].State != StateType.Deleted);

        CreateAndExecuteRemoveCommand (domainObject);
      }

      GetAssociatedEndPoint().Touch ();
      return domainObject != null;
    }

    public void Replace (int index, DomainObject value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      CheckClientTransaction (value, "Cannot put DomainObject '{0}' into the collection of property '{1}' of DomainObject '{2}'.");
      DomainObjectCheckUtility.EnsureNotDeleted (value, GetAssociatedEndPoint().ClientTransaction);
      DomainObjectCheckUtility.EnsureNotDeleted (GetAssociatedEndPoint().GetDomainObjectReference (), GetAssociatedEndPoint().ClientTransaction);

      var replaceCommand = GetAssociatedEndPoint().CreateReplaceCommand (index, value);
      var bidirectionalModification = replaceCommand.ExpandToAllRelatedObjects ();
      bidirectionalModification.NotifyAndPerform ();

      GetAssociatedEndPoint().Touch ();
    }

    public void Sort (Comparison<DomainObject> comparison)
    {
      ArgumentUtility.CheckNotNull ("comparison", comparison);

      GetAssociatedEndPoint().SortCurrentData (comparison);
    }

    private void CreateAndExecuteRemoveCommand (DomainObject domainObject)
    {
      var command = GetAssociatedEndPoint().CreateRemoveCommand (domainObject);
      var bidirectionalModification = command.ExpandToAllRelatedObjects ();
      bidirectionalModification.NotifyAndPerform ();
    }

    private CompositeCommand GetClearCommand ()
    {
      var removeCommands = new List<ExpandedCommand>();

      for (int i = Count - 1; i >= 0; --i)
      {
        var removedObject = GetObject (i);

        // we can rely on the fact that this object is not deleted, otherwise we wouldn't have got it
        Assertion.IsTrue (removedObject.TransactionContext[GetAssociatedEndPoint().ClientTransaction].State != StateType.Deleted);
        removeCommands.Add (GetAssociatedEndPoint().CreateRemoveCommand (removedObject).ExpandToAllRelatedObjects ());
      }

      return new CompositeCommand (removeCommands.Cast<IDataManagementCommand> ()).CombineWith (new RelationEndPointTouchCommand (GetAssociatedEndPoint()));
    }

    private void CheckClientTransaction (DomainObject domainObject, string exceptionFormatString)
    {
      Assertion.DebugAssert (domainObject != null);
      
      var endPoint = GetAssociatedEndPoint();

      // This uses IsEnlisted rather than a RootTransaction check because the DomainObject reference is used inside the ClientTransaction, and we
      // explicitly want to allow only objects enlisted in the transaction.
      if (!endPoint.ClientTransaction.IsEnlisted (domainObject))
      {
        var formattedMessage = String.Format (
            exceptionFormatString, 
            domainObject.ID, 
            endPoint.Definition.PropertyName, 
            endPoint.ObjectID);
        throw new ClientTransactionsDifferException (formattedMessage + " The objects do not belong to the same ClientTransaction.");
      }
    }
  }
}
