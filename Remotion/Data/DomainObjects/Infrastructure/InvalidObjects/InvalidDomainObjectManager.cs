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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.InvalidObjects
{
  /// <summary>
  /// Keeps a collection of <see cref="DomainObject"/> references that were marked as invalid in a given <see cref="ClientTransaction"/>.
  /// </summary>
  [Serializable]
  public class InvalidDomainObjectManager : IInvalidDomainObjectManager
  {
    private readonly IClientTransactionEventSink _transactionEventSink;
    private readonly Dictionary<ObjectID, IDomainObject> _invalidObjects = new Dictionary<ObjectID, IDomainObject>();

    public InvalidDomainObjectManager (IClientTransactionEventSink transactionEventSink)
    {
      ArgumentUtility.CheckNotNull("transactionEventSink", transactionEventSink);
      _transactionEventSink = transactionEventSink;
    }

    public InvalidDomainObjectManager (IClientTransactionEventSink transactionEventSink, IEnumerable<IDomainObject> invalidObjects)
        : this(transactionEventSink)
    {
      ArgumentUtility.CheckNotNull("invalidObjects", invalidObjects);

      foreach (var domainObject in invalidObjects)
      {
        try
        {
          _invalidObjects.Add(domainObject.ID, domainObject);
        }
        catch (ArgumentException ex)
        {
          throw new ArgumentException("The sequence contains multiple different objects with the same ID.", "invalidObjects", ex);
        }
      }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public int InvalidObjectCount
    {
      get { return _invalidObjects.Count; }
    }

    public IEnumerable<ObjectID> InvalidObjectIDs
    {
      get { return _invalidObjects.Keys; }
    }

    public bool IsInvalid (ObjectID id)
    {
      ArgumentUtility.CheckNotNull("id", id);
      return _invalidObjects.ContainsKey(id);
    }

    public IDomainObject GetInvalidObjectReference (ObjectID id)
    {
      ArgumentUtility.CheckNotNull("id", id);

      if (!_invalidObjects.TryGetValue(id, out var invalidDomainObject))
        throw new ArgumentException(String.Format("The object '{0}' has not been marked invalid.", id), "id");
      else
        return invalidDomainObject;
    }

    public bool MarkInvalid (IDomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      if (IsInvalid(domainObject.ID))
      {
        if (GetInvalidObjectReference(domainObject.ID) != domainObject)
        {
          var message = string.Format("Cannot mark the given object invalid, another object with the same ID '{0}' has already been marked.", domainObject.ID);
          throw new InvalidOperationException(message);
        }

        return false;
      }

      _invalidObjects.Add(domainObject.ID, domainObject);
      _transactionEventSink.RaiseObjectMarkedInvalidEvent(domainObject);
      return true;
    }

    public bool MarkNotInvalid (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      if (!_invalidObjects.TryGetValue(objectID, out var domainObject))
        return false;

      _invalidObjects.Remove(objectID);
      _transactionEventSink.RaiseObjectMarkedNotInvalidEvent(domainObject);
      return true;
    }
  }
}
