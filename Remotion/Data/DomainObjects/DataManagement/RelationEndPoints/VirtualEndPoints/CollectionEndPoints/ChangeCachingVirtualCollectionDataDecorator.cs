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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  public class ChangeCachingVirtualCollectionDataDecorator : IVirtualCollectionData, IFlattenedSerializable
  {
    private readonly VirtualCollectionData _virtualCollectionData;
    private bool _isCacheUpToDate;

    public ChangeCachingVirtualCollectionDataDecorator (VirtualCollectionData virtualCollectionData)
    {
      ArgumentUtility.CheckNotNull ("virtualCollectionData", virtualCollectionData);

      _virtualCollectionData = virtualCollectionData;
      _isCacheUpToDate = true;
    }

    public bool IsCacheUpToDate
    {
      get
      {
        // TODO: RM-7294
        return _isCacheUpToDate;
      }
    }

    public void ResetCachedHasChangedState ()
    {
      _isCacheUpToDate = false;
    }

    public ReadOnlyVirtualCollectionDataDecorator GetOriginalData ()
    {
      // TODO: RM-7294
      var originalData = new VirtualCollectionData (
          ((IVirtualCollectionData) _virtualCollectionData).AssociatedEndPointID,
          _virtualCollectionData.DataContainerMap,
          ValueAccess.Original);
      return new ReadOnlyVirtualCollectionDataDecorator (originalData);
    }

    public void ResetCachedDomainObjects ()
    {
      _virtualCollectionData.ResetCachedDomainObjects();
      //TODO: RM-7294: originally, this logic never cleared the _isCacheUpToDate-field. Should this be re-worked?
    }

    public void Commit ()
    {
      ResetCachedHasChangedState();
    }

    public void Rollback ()
    {
      ResetCachedHasChangedState();
      _virtualCollectionData.ResetCachedDomainObjects();
    }

    public void ReplaceContents (IVirtualCollectionData collectionData)
    {
      ResetCachedHasChangedState();
      _virtualCollectionData.ResetCachedDomainObjects();
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

    RelationEndPointID IVirtualCollectionData.AssociatedEndPointID => ((IVirtualCollectionData) _virtualCollectionData).AssociatedEndPointID;

    bool IVirtualCollectionData.IsDataComplete => ((IVirtualCollectionData) _virtualCollectionData).IsDataComplete;


    void IVirtualCollectionData.EnsureDataComplete ()
    {
      ((IVirtualCollectionData) _virtualCollectionData).EnsureDataComplete();
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
      ResetCachedHasChangedState();
      _virtualCollectionData.ResetCachedDomainObjects();
    }

    void IVirtualCollectionData.Add (DomainObject domainObject)
    {
      ResetCachedHasChangedState();

      _virtualCollectionData.ResetCachedDomainObjects();
    }

    bool IVirtualCollectionData.Remove (DomainObject domainObject)
    {
      ResetCachedHasChangedState();

      _virtualCollectionData.ResetCachedDomainObjects();
      return true;
    }

    #region Serialization

    private ChangeCachingVirtualCollectionDataDecorator (FlattenedDeserializationInfo info)
    {
      _virtualCollectionData = info.GetValueForHandle<VirtualCollectionData>();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle ((VirtualCollectionData) _virtualCollectionData);
    }

    #endregion
  }
}