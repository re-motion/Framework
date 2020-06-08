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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  public class ChangeCachingVirtualCollectionDataDecorator : IVirtualCollectionData
  {
    private readonly IVirtualCollectionData _virtualCollectionData;

    public ChangeCachingVirtualCollectionDataDecorator (IVirtualCollectionData virtualCollectionData)
    {
      ArgumentUtility.CheckNotNull ("virtualCollectionData", virtualCollectionData);

      _virtualCollectionData = virtualCollectionData;
    }

    public bool IsCacheUpToDate
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasChanged (ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy)
    {
      throw new NotImplementedException();
    }

    public ReadOnlyVirtualCollectionData OriginalData
    {
      get { throw new NotImplementedException(); }
    }

    public void RegisterOriginalItem (DomainObject item)
    {
      throw new NotImplementedException();
    }

    public void UnregisterOriginalItem (ObjectID itemID)
    {
      throw new NotImplementedException();
    }

    public void SortOriginalAndCurrent (Comparison<DomainObject> comparison)
    {
      throw new NotImplementedException();
    }

    public void Commit ()
    {
      throw new NotImplementedException();
    }

    public void Rollback ()
    {
      throw new NotImplementedException();
    }

    public void ReplaceContents (IVirtualCollectionData collectionData)
    {
      throw new NotImplementedException();
    }

    IEnumerator<DomainObject> IEnumerable<DomainObject>.GetEnumerator ()
    {
      return _virtualCollectionData.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return ((IEnumerable) _virtualCollectionData).GetEnumerator();
    }

    int IReadOnlyCollection<DomainObject>.Count => _virtualCollectionData.Count;

    Type IVirtualCollectionData.RequiredItemType => _virtualCollectionData.RequiredItemType;

    bool IVirtualCollectionData.IsReadOnly => _virtualCollectionData.IsReadOnly;

    RelationEndPointID IVirtualCollectionData.AssociatedEndPointID => _virtualCollectionData.AssociatedEndPointID;

    bool IVirtualCollectionData.IsDataComplete => _virtualCollectionData.IsDataComplete;


    void IVirtualCollectionData.EnsureDataComplete ()
    {
      _virtualCollectionData.EnsureDataComplete();
    }

    bool IVirtualCollectionData.ContainsObjectID (ObjectID objectID)
    {
      return _virtualCollectionData.ContainsObjectID (objectID);
    }

    DomainObject IVirtualCollectionData.GetObject (int index)
    {
      return _virtualCollectionData.GetObject (index);
    }

    DomainObject IVirtualCollectionData.GetObject (ObjectID objectID)
    {
      return _virtualCollectionData.GetObject (objectID);
    }

    int IVirtualCollectionData.IndexOf (ObjectID objectID)
    {
      return _virtualCollectionData.IndexOf (objectID);
    }

    void IVirtualCollectionData.Clear ()
    {
      _virtualCollectionData.Clear();
    }

    void IVirtualCollectionData.Add (DomainObject domainObject)
    {
      _virtualCollectionData.Add (domainObject);
    }

    bool IVirtualCollectionData.Remove (DomainObject domainObject)
    {
      return _virtualCollectionData.Remove (domainObject);
    }

    bool IVirtualCollectionData.Remove (ObjectID objectID)
    {
      return _virtualCollectionData.Remove (objectID);
    }

    public void Sort (Comparison<DomainObject> comparison)
    {
      _virtualCollectionData.Sort (comparison);
    }
  }
}