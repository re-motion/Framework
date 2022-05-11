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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Implementation of <see cref="IVirtualCollectionData"/> used for tracking the usages of <see cref="IVirtualCollectionData.Add"/>,
  /// <see cref="IVirtualCollectionData.Remove"/>, and <see cref="IVirtualCollectionData.Clear"/>.
  /// This is used with the <see cref="IDataManagementCommand"/>s of <see cref="IVirtualCollectionEndPoint"/>.
  /// </summary>
  public class ChangeTrackingVirtualCollectionDataDecorator : IVirtualCollectionData
  {
    private readonly IVirtualCollectionData _innerVirtualCollectionData;
    private readonly HashSet<ObjectID> _addedDomainObjects;
    private readonly HashSet<ObjectID> _removedDomainObjects;

    public ChangeTrackingVirtualCollectionDataDecorator (
        IVirtualCollectionData innerVirtualCollectionData,
        HashSet<ObjectID> addedDomainObjects,
        HashSet<ObjectID> removedDomainObjects)
    {
      ArgumentUtility.CheckNotNull("innerVirtualCollectionData", innerVirtualCollectionData);
      ArgumentUtility.CheckNotNull("addedDomainObjects", addedDomainObjects);
      ArgumentUtility.CheckNotNull("removedDomainObjects", removedDomainObjects);

      _innerVirtualCollectionData = innerVirtualCollectionData;
      _addedDomainObjects = addedDomainObjects;
      _removedDomainObjects = removedDomainObjects;
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      return _innerVirtualCollectionData.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return ((IEnumerable)_innerVirtualCollectionData).GetEnumerator();
    }

    public int Count => _innerVirtualCollectionData.Count;

    public Type RequiredItemType => _innerVirtualCollectionData.RequiredItemType;

    public bool IsReadOnly => _innerVirtualCollectionData.IsReadOnly;

    public RelationEndPointID AssociatedEndPointID => _innerVirtualCollectionData.AssociatedEndPointID;

    public bool IsDataComplete => _innerVirtualCollectionData.IsDataComplete;

    public void EnsureDataComplete () => _innerVirtualCollectionData.EnsureDataComplete();

    public bool ContainsObjectID (ObjectID objectID) => _innerVirtualCollectionData.ContainsObjectID(objectID);

    public IDomainObject GetObject (int index) => _innerVirtualCollectionData.GetObject(index);

    public IDomainObject? GetObject (ObjectID objectID) => _innerVirtualCollectionData.GetObject(objectID);

    public int IndexOf (ObjectID objectID) => _innerVirtualCollectionData.IndexOf(objectID);

    public void Clear ()
    {
      DomainObject[] currentDomainObjects;
      if (_innerVirtualCollectionData.IsDataComplete)
      {
        currentDomainObjects = _innerVirtualCollectionData.ToArray();
      }
      else
        currentDomainObjects = Array.Empty<DomainObject>();

      _innerVirtualCollectionData.Clear();

      foreach (var currentDomainObject in currentDomainObjects)
        _removedDomainObjects.Add(currentDomainObject.ID);
      foreach (var addedDomainObject in _addedDomainObjects)
        _removedDomainObjects.Remove(addedDomainObject);
      _addedDomainObjects.Clear();
    }

    public void Add (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      _innerVirtualCollectionData.Add(domainObject);

      if (!_removedDomainObjects.Remove(domainObject.ID))
        _addedDomainObjects.Add(domainObject.ID);
    }

    public bool Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      var result = _innerVirtualCollectionData.Remove(domainObject);

      if (!_addedDomainObjects.Remove(domainObject.ID))
        _removedDomainObjects.Add(domainObject.ID);

      return result;
    }
  }
}
