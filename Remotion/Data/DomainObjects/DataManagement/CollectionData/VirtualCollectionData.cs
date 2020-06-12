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
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  public class VirtualCollectionData : IVirtualCollectionData, IFlattenedSerializable
  {
    //TODO: RM-7294: drop IVirtualCollectionData interface from type

    private readonly RelationEndPointID _associatedEndPointID;
    private readonly IDataContainerMapReadOnlyView _dataContainerMap;
    private readonly ValueAccess _valueAccess;
    [CanBeNull]
    private IReadOnlyList<DomainObject> _cachedDomainObjects;

    public VirtualCollectionData (
        RelationEndPointID associatedEndPointID,
        IDataContainerMapReadOnlyView dataContainerMap,
        ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("associatedEndPointID", associatedEndPointID);
      ArgumentUtility.CheckNotNull ("dataContainerMap", dataContainerMap);

      //TODO: RM-7294
      _associatedEndPointID = associatedEndPointID;
      _dataContainerMap = dataContainerMap;
      _valueAccess = valueAccess;
    }

    public IDataContainerMapReadOnlyView DataContainerMap => _dataContainerMap;

    public ValueAccess ValueAccess => _valueAccess;

    public IEnumerator<DomainObject> GetEnumerator () => GetCachedDomainObjects().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

    public int Count => GetCachedDomainObjects().Count;

    public Type RequiredItemType  => _associatedEndPointID.Definition.GetOppositeEndPointDefinition().ClassDefinition.ClassType;

    public bool IsReadOnly { get; } = false;

    RelationEndPointID IVirtualCollectionData.AssociatedEndPointID => _associatedEndPointID;

    bool IVirtualCollectionData.IsDataComplete => true;

    void IVirtualCollectionData.EnsureDataComplete ()
    {
      // NOP
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      //TODO: RM-7294
      return GetCachedDomainObjects().Any (obj => obj.ID == objectID);
    }

    public DomainObject GetObject (int index)
    {
      //TODO: RM-7294

      var cachedDomainObjects = GetCachedDomainObjects();

      if (index < 0)
        throw new ArgumentOutOfRangeException ("index");
      if (index >= cachedDomainObjects.Count)
        throw new ArgumentOutOfRangeException ("index");

      return cachedDomainObjects[index];
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      //TODO: RM-7294
      return GetCachedDomainObjects().FirstOrDefault (obj => obj.ID == objectID);
    }

    public int IndexOf (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      //TODO: RM-7294
      var cachedDomainObjects = GetCachedDomainObjects();
      int index = 0;
      foreach (var domainObject in cachedDomainObjects)
      {
        if (domainObject.ID == objectID)
          return index;

        index++;
      }

      return -1;
    }

    public void Clear ()
    {
      //TODO: RM-7294: API is only needed via ChangeCachingDataDecorator, which in turn is used from the EndPointDeleteCommand.
      //Possibly create dedicated interface when we only cache the data and need a cache-reset otherwise. 
      ResetCachedDomainObjects();
    }

    public void Add (DomainObject domainObject)
    {
      //TODO: RM-7294: API is only needed via ChangeCachingDataDecorator, which in turn is used from the EndPointAddCommand.
      //Possibly create dedicated interface when we only cache the data and need a cache-reset otherwise. 
      ResetCachedDomainObjects();
    }

    public bool Remove (DomainObject domainObject)
    {
      //TODO: RM-7294: API is only needed via ChangeCachingDataDecorator, which in turn is used from the EndPointRemoveCommand.
      //Possibly create dedicated interface when we only cache the data and need a cache-reset otherwise. 
      ResetCachedDomainObjects ();
      return true;
    }

    public bool Remove (ObjectID objectID)
    {
      //TODO: RM-7294: API is only implemented because of the interface. Can probably be dropped since there is no usage
      throw new NotSupportedException();
    }

    public void Sort (Comparison<DomainObject> comparison)
    {
      //TODO: RM-7294: API is only implemented because of the interface. Can probably be dropped since there is no usage
      throw new NotSupportedException();
    }

    [NotNull]
    private IReadOnlyList<DomainObject> GetCachedDomainObjects ()
    {
      if (_cachedDomainObjects == null)
      {
        var foreignKeyRelationEndPointDefinition = (RelationEndPointDefinition) _associatedEndPointID.Definition.GetOppositeEndPointDefinition();
        var foreignKeyPropertyDefinition = foreignKeyRelationEndPointDefinition.PropertyDefinition;
        var requiredClassDefinition = foreignKeyRelationEndPointDefinition.ClassDefinition;
        var requiredItemType = requiredClassDefinition.ClassType;
        var foreignKeyValue = _associatedEndPointID.ObjectID;
        var comparer = GetDomainObjectComparer();

        _cachedDomainObjects = _dataContainerMap
            .Where (dc => !dc.State.IsDiscarded)
            .Where (dc => dc.HasDomainObject)
            .Where (dc => requiredItemType.IsAssignableFrom (dc.DomainObjectType))
#if DEBUG
            // Only in debug builds for performance reasons. The .NET type comparison is sufficient for filtering the types until interface support is added.
            .Where (dc => requiredClassDefinition.IsSameOrBaseClassOf (dc.ClassDefinition))
#endif
            .Where (dc => (ObjectID) dc.GetValueWithoutEvents (foreignKeyPropertyDefinition, _valueAccess) == foreignKeyValue)
            .OrderBy (dc => dc.DomainObject, comparer)
            .Select (dc => dc.DomainObject)
            .ToList();
      }

      return _cachedDomainObjects;
    }

    private IComparer<DomainObject> GetDomainObjectComparer ()
    {
      var idComparer = new DelegateBasedComparer<DomainObject> ((left, right) => left.ID.CompareTo (right.ID));
      var sortExpression = ((VirtualCollectionRelationEndPointDefinition) _associatedEndPointID.Definition).GetSortExpression();
      if (sortExpression != null)
      {
        var propertyComparer = SortedPropertyComparer.CreateCompoundComparer (sortExpression.SortedProperties, _dataContainerMap, _valueAccess);
        return new CompoundComparer<DomainObject> (propertyComparer, idComparer);
      }
      else
      {
        return idComparer;
      }
    }

    private void ResetCachedDomainObjects ()
    {
      _cachedDomainObjects = null;
    }

    #region Serialization

    private VirtualCollectionData (FlattenedDeserializationInfo info)
    {
      _associatedEndPointID = info.GetValueForHandle<RelationEndPointID>();
      _dataContainerMap = info.GetValueForHandle<IDataContainerMapReadOnlyView>();
      _valueAccess = (ValueAccess) info.GetIntValue();
      var hasCachedDomainObjects = info.GetBoolValue();
      if (hasCachedDomainObjects)
      {
        var cachedDomainObjects = new List<DomainObject>();
        info.FillCollection (cachedDomainObjects);
        _cachedDomainObjects = cachedDomainObjects;
      }
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_associatedEndPointID);
      info.AddHandle (_dataContainerMap);
      info.AddIntValue ((int) _valueAccess);
      var hasCachedDomainObjects = _cachedDomainObjects != null;
      info.AddBoolValue (hasCachedDomainObjects);
      if (hasCachedDomainObjects)
        info.AddCollection ((ICollection<DomainObject>) _cachedDomainObjects);
    }

    #endregion
  }
}