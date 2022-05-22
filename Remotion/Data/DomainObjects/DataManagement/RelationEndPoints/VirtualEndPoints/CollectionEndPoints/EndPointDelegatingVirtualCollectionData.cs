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
  /// Implements the <see cref="IVirtualCollectionData"/> by forwarding all requests to an implementation of 
  /// <see cref="IVirtualCollectionEndPoint"/>.
  /// </summary>
  [Serializable]
  [System.Runtime.InteropServices.Guid("37550759-6300-4DDC-88EB-12B0FC29703F")]
  public class EndPointDelegatingVirtualCollectionData : IVirtualCollectionData
  {
    private readonly RelationEndPointID _endPointID;
    private readonly Type _requiredItemType;
    private readonly IVirtualEndPointProvider _virtualEndPointProvider;

    public EndPointDelegatingVirtualCollectionData (
        RelationEndPointID endPointID,
        Type requiredItemType,
        IVirtualEndPointProvider virtualEndPointProvider)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);
      ArgumentUtility.CheckNotNull("requiredItemType", requiredItemType);
      ArgumentUtility.CheckNotNull("virtualEndPointProvider", virtualEndPointProvider);


      if (endPointID.Definition.Cardinality != CardinalityType.Many)
        throw new ArgumentException("Associated end-point must be a CollectionEndPoint.", "endPointID");

      _endPointID = endPointID;
      _requiredItemType = requiredItemType;
      _virtualEndPointProvider = virtualEndPointProvider;
    }

    public IVirtualEndPointProvider VirtualEndPointProvider => _virtualEndPointProvider;

    public int Count
    {
      get
      {
        var data = GetAssociatedEndPoint().GetData();
        return data.Count;
      }
    }

    public Type RequiredItemType => _requiredItemType;

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

    public RelationEndPointID AssociatedEndPointID => _endPointID;

    public IVirtualCollectionEndPoint GetAssociatedEndPoint ()
    {
      return (IVirtualCollectionEndPoint)_virtualEndPointProvider.GetOrCreateVirtualEndPoint(_endPointID);
    }

    public bool IsDataComplete
    {
      get { return GetAssociatedEndPoint().IsDataComplete; }
    }


    public void EnsureDataComplete ()
    {
      GetAssociatedEndPoint().EnsureDataComplete();
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      var data = GetAssociatedEndPoint().GetData();
      return data.ContainsObjectID(objectID);
    }

    public IDomainObject GetObject (int index)
    {
      var data = GetAssociatedEndPoint().GetData();
      return data.GetObject(index);
    }

    public IDomainObject? GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      var data = GetAssociatedEndPoint().GetData();
      return data.GetObject(objectID);
    }

    public int IndexOf (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      var data = GetAssociatedEndPoint().GetData();
      return data.IndexOf(objectID);
    }

    public IEnumerator<IDomainObject> GetEnumerator ()
    {
      var data = GetAssociatedEndPoint().GetData();
      return data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public void Clear ()
    {
      var associatedEndPoint = GetAssociatedEndPoint();
      var associatedDomainObjectReference = associatedEndPoint.GetDomainObjectReference();
      Assertion.DebugAssert(associatedEndPoint.IsNull == false, "GetAssociatedEndPoint().IsNull == false");
      Assertion.DebugIsNotNull(associatedDomainObjectReference, "GetAssociatedEndPoint().GetDomainObjectReference() != null");
      DomainObjectCheckUtility.EnsureNotDeleted(associatedDomainObjectReference, associatedEndPoint.ClientTransaction);

      var combinedCommand = GetClearCommand();
      combinedCommand.NotifyAndPerform();
    }

    public void Add (IDomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      CheckItemType(domainObject, "domainObject");
      if (ContainsObjectID(domainObject.ID))
        throw new ArgumentException(string.Format("The collection already contains an object with ID '{0}'.", domainObject.ID), "domainObject");
      CheckClientTransaction(domainObject, "Cannot add DomainObject '{0}' into collection of property '{1}' of DomainObject '{2}'.");
      var associatedEndPoint = GetAssociatedEndPoint();
      var associatedDomainObjectReference = associatedEndPoint.GetDomainObjectReference();
      Assertion.DebugAssert(associatedEndPoint.IsNull == false, "GetAssociatedEndPoint().IsNull == false");
      Assertion.DebugIsNotNull(associatedDomainObjectReference, "GetAssociatedEndPoint().GetDomainObjectReference() != null");
      DomainObjectCheckUtility.EnsureNotDeleted(domainObject, associatedEndPoint.ClientTransaction);
      DomainObjectCheckUtility.EnsureNotDeleted(associatedDomainObjectReference, associatedEndPoint.ClientTransaction);

      var insertCommand = associatedEndPoint.CreateAddCommand(domainObject);
      var bidirectionalModification = insertCommand.ExpandToAllRelatedObjects();
      bidirectionalModification.NotifyAndPerform();

      associatedEndPoint.Touch();
    }

    public bool Remove (IDomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      var existingObject = GetObject(domainObject.ID);
      if (existingObject != null && existingObject != domainObject)
      {
        var message = string.Format(
            "The object to be removed has the same ID ('{0}') as an object in this collection, but is a different object reference.",
            domainObject.ID);
        throw new ArgumentException(message, "domainObject");
      }
      CheckClientTransaction(domainObject, "Cannot remove DomainObject '{0}' from collection of property '{1}' of DomainObject '{2}'.");
      var associatedEndPoint = GetAssociatedEndPoint();
      var associatedDomainObjectReference = associatedEndPoint.GetDomainObjectReference();
      Assertion.DebugAssert(associatedEndPoint.IsNull == false, "GetAssociatedEndPoint().IsNull == false");
      Assertion.DebugIsNotNull(associatedDomainObjectReference, "GetAssociatedEndPoint().GetDomainObjectReference() != null");
      DomainObjectCheckUtility.EnsureNotDeleted(domainObject, associatedEndPoint.ClientTransaction);
      DomainObjectCheckUtility.EnsureNotDeleted(associatedDomainObjectReference, associatedEndPoint.ClientTransaction);

      var containsObjectID = ContainsObjectID(domainObject.ID);
      if (containsObjectID)
        CreateAndExecuteRemoveCommand(domainObject);

      associatedEndPoint.Touch();
      return containsObjectID;
    }

    private void CreateAndExecuteRemoveCommand (IDomainObject domainObject)
    {
      var command = GetAssociatedEndPoint().CreateRemoveCommand(domainObject);
      var bidirectionalModification = command.ExpandToAllRelatedObjects();
      bidirectionalModification.NotifyAndPerform();
    }

    private CompositeCommand GetClearCommand ()
    {
      var removeCommands = new List<ExpandedCommand>();

      var associatedEndPoint = GetAssociatedEndPoint();

      for (int i = Count - 1; i >= 0; --i)
      {
        var removedObject = GetObject(i);

        // we can rely on the fact that this object is not deleted, otherwise we wouldn't have got it
        Assertion.IsFalse(removedObject.TransactionContext[associatedEndPoint.ClientTransaction].State.IsDeleted);
        removeCommands.Add(associatedEndPoint.CreateRemoveCommand(removedObject).ExpandToAllRelatedObjects());
      }

      return new CompositeCommand(removeCommands).CombineWith(new RelationEndPointTouchCommand(associatedEndPoint));
    }

    private void CheckClientTransaction (IDomainObject domainObject, string exceptionFormatString)
    {
      Assertion.DebugAssert(domainObject != null);

      var endPoint = GetAssociatedEndPoint();

      // This uses IsEnlisted rather than a RootTransaction check because the DomainObject reference is used inside the ClientTransaction, and we
      // explicitly want to allow only objects enlisted in the transaction.
      if (!endPoint.ClientTransaction.IsEnlisted(domainObject))
      {
        var formattedMessage = String.Format(
            exceptionFormatString,
            domainObject.ID,
            endPoint.Definition.PropertyName,
            endPoint.ObjectID);
        throw new ClientTransactionsDifferException(formattedMessage + " The objects do not belong to the same ClientTransaction.");
      }
    }

    private void CheckItemType (IDomainObject domainObject, string argumentName)
    {
      if (!_requiredItemType.IsInstanceOfType(domainObject))
      {
        string message = string.Format(
            "Values of type '{0}' cannot be added to this collection. Values must be of type '{1}' or derived from '{1}'.",
            domainObject.GetPublicDomainObjectType(),
            _requiredItemType);
        throw new ArgumentException(message, argumentName);
      }
    }
  }
}
