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
    private bool _hasChanges;

    public ChangeCachingVirtualCollectionDataDecorator (VirtualCollectionData virtualCollectionData)
    {
      ArgumentUtility.CheckNotNull ("virtualCollectionData", virtualCollectionData);

      _virtualCollectionData = virtualCollectionData;
      _isCacheUpToDate = true;
      _hasChanges = false;
    }

    public bool IsCacheUpToDate
    {
      get
      {
        // TODO: RM-7294
        return _isCacheUpToDate;
      }
    }

    public bool HasChanged (IVirtualCollectionEndPointChangeDetectionStrategy changeDetectionStrategy)
    {
      // TODO: RM-7294 test, optimize, remove strategy parameter

      if (!_isCacheUpToDate)
      {
        var currentData = _virtualCollectionData;
        var originalData = GetOriginalData();
        if (currentData.Count != originalData.Count)
          _hasChanges = true;
        else
          _hasChanges = currentData.Select (obj => obj.ID).Except (originalData.Select (obj => obj.ID)).Any();

        _isCacheUpToDate = true;
      }

      return _hasChanges;
    }

    private void ResetCache ()
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

    public void RegisterOriginalItem (DomainObject item)
    {
      //TODO: RM-7294: ResetCachedDomainObjects() is equivalent to Add()
      _virtualCollectionData.ResetCachedDomainObjects();
      //_virtualCollectionData.Add (item);
    }

    public void UnregisterOriginalItem (ObjectID itemID)
    {
      // TODO: RM-7294
      // Only used from Synchronize
      ResetCache();
    }

    public void SortOriginalAndCurrent (Comparison<DomainObject> comparison)
    {
      //TODO: RM-7294: API is only implemented because of the interface. Can probably be dropped since there is no usage
      throw new NotSupportedException();
    }

    public void Commit ()
    {
      // TODO: RM-7294
      ResetCache();
    }

    public void Rollback ()
    {
      // TODO: RM-7294
      ResetCache();
      _virtualCollectionData.ResetCachedDomainObjects(); //TODO: RM-7294
    }

    public void ReplaceContents (IVirtualCollectionData collectionData)
    {
      // TODO: RM-7294
      ResetCache();
      _virtualCollectionData.ResetCachedDomainObjects(); //TODO: RM-7294
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
      ResetCache();
      //TODO: RM-7294: ResetCachedDomainObjects() is equivalent to Clear()
      _virtualCollectionData.ResetCachedDomainObjects();
      //_virtualCollectionData.Clear();
    }

    void IVirtualCollectionData.Add (DomainObject domainObject)
    {
      ResetCache();

      //TODO: RM-7294: ResetCachedDomainObjects() is equivalent to Add()
      _virtualCollectionData.ResetCachedDomainObjects();
      //_virtualCollectionData.Add (domainObject);
    }

    bool IVirtualCollectionData.Remove (DomainObject domainObject)
    {
      ResetCache();

      //TODO: RM-7294: ResetCachedDomainObjects() is equivalent to Remove()
      _virtualCollectionData.ResetCachedDomainObjects();
      //return _virtualCollectionData.Remove (domainObject);
      return true;
    }

    bool IVirtualCollectionData.Remove (ObjectID objectID)
    {
      //return _virtualCollectionData.Remove (objectID);
      //TODO: RM-7294: API is only implemented because of the interface. Can probably be dropped since there is no usage
      throw new NotSupportedException();
    }

    public void Sort (Comparison<DomainObject> comparison)
    {
      //_virtualCollectionData.Sort (comparison);
      //TODO: RM-7294: API is only implemented because of the interface. Can probably be dropped since there is no usage
      throw new NotSupportedException();
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