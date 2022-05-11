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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  /// <summary>
  /// Provides an an encapsulation of the data required when accessing an implementation of <see cref="IObjectList{TDomainObject}"/>, implementing the 
  /// <see cref="IVirtualCollectionData"/> interface. The data is retrieved from from the <see cref="ClientTransaction"/>'s <see cref="T:DataContainerMap"/>
  /// and cached locally.
  /// </summary>
  public class VirtualCollectionData : IVirtualCollectionData, IFlattenedSerializable
  {
    private readonly RelationEndPointID _associatedEndPointID;
    private readonly IDataContainerMapReadOnlyView _dataContainerMap;
    private readonly ValueAccess _valueAccess;
    [CanBeNull]
    private IReadOnlyDictionary<ObjectID, DomainObject>? _cachedDomainObjects;

    public VirtualCollectionData (
        RelationEndPointID associatedEndPointID,
        IDataContainerMapReadOnlyView dataContainerMap,
        ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull("associatedEndPointID", associatedEndPointID);
      ArgumentUtility.CheckNotNull("dataContainerMap", dataContainerMap);

      _associatedEndPointID = associatedEndPointID;
      _dataContainerMap = dataContainerMap;
      _valueAccess = valueAccess;
    }

    public IDataContainerMapReadOnlyView DataContainerMap => _dataContainerMap;

    public ValueAccess ValueAccess => _valueAccess;

    public ReadOnlyVirtualCollectionDataDecorator GetOriginalData ()
    {
      var originalData = new VirtualCollectionData(_associatedEndPointID, _dataContainerMap, ValueAccess.Original);
      return new ReadOnlyVirtualCollectionDataDecorator(originalData);
    }

    [MemberNotNullWhen(true, nameof(_cachedDomainObjects))]
    public bool IsCacheUpToDate
    {
      get { return _cachedDomainObjects != null; }
    }

    public void ResetCachedDomainObjects ()
    {
      _cachedDomainObjects = null;
    }

    public IEnumerator<DomainObject> GetEnumerator () => GetCachedDomainObjectsSorted().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

    public int Count => GetCachedDomainObjects().Count;

    public Type RequiredItemType  => _associatedEndPointID.Definition.GetOppositeEndPointDefinition().TypeDefinition.Type;

    public bool IsReadOnly { get; } = false;

    RelationEndPointID IVirtualCollectionData.AssociatedEndPointID => _associatedEndPointID;

    bool IVirtualCollectionData.IsDataComplete => true;

    void IVirtualCollectionData.EnsureDataComplete ()
    {
      // NOP
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      return GetCachedDomainObjects().ContainsKey(objectID);
    }

    public IDomainObject GetObject (int index)
    {
      if (index < 0)
        throw new ArgumentOutOfRangeException("index");

      if (index >= GetCachedDomainObjects().Count)
        throw new ArgumentOutOfRangeException("index");

      var cachedDomainObjects = GetCachedDomainObjectsSorted();
      int itemIndex = 0;
      foreach (var domainObject in cachedDomainObjects)
      {
        if (itemIndex == index)
          return domainObject;

        itemIndex++;
      }

      throw new ArgumentOutOfRangeException("index");
    }

    public IDomainObject? GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      return GetCachedDomainObjects().GetValueOrDefault(objectID);
    }

    public int IndexOf (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      var cachedDomainObjects = GetCachedDomainObjectsSorted();
      int itemIndex = 0;
      foreach (var domainObject in cachedDomainObjects)
      {
        if (domainObject.ID == objectID)
          return itemIndex;

        itemIndex++;
      }

      return -1;
    }

    void IVirtualCollectionData.Clear ()
    {
      //TODO: RM-7294: API is only needed via ChangeTrackingVirtualCollectionDataDecorator, which in turn is used from the VirtualCollectionEndPointDeleteCommand.
      //Possibly create dedicated interface when we only cache the data and need a cache-reset otherwise. 
      //Or drop interface from VirtualCollectionData.

      ResetCachedDomainObjects();
    }

    void IVirtualCollectionData.Add (DomainObject domainObject)
    {
      //TODO: RM-7294: API is only needed via ChangeTrackingVirtualCollectionDataDecorator, which in turn is used from the VirtualCollectionEndPointAddCommand.
      //Possibly create dedicated interface when we only cache the data and need a cache-reset otherwise. 
      //Or drop interface from VirtualCollectionData.

      ResetCachedDomainObjects();
    }

    bool IVirtualCollectionData.Remove (DomainObject domainObject)
    {
      //TODO: RM-7294: API is only needed via ChangeTrackingVirtualCollectionDataDecorator, which in turn is used from the VirtualCollectionEndPointRemoveCommand.
      //Possibly create dedicated interface when we only cache the data and need a cache-reset otherwise. 
      //Or drop interface from VirtualCollectionData.

      ResetCachedDomainObjects();
      return true;
    }

    [JetBrains.Annotations.NotNull]
    private IReadOnlyDictionary<ObjectID, DomainObject> GetCachedDomainObjects ()
    {
      if (_cachedDomainObjects == null)
      {
        var foreignKeyRelationEndPointDefinition = (RelationEndPointDefinition)_associatedEndPointID.Definition.GetOppositeEndPointDefinition();
        var foreignKeyPropertyDefinition = foreignKeyRelationEndPointDefinition.PropertyDefinition;
        var requiredTypeDefinition = foreignKeyRelationEndPointDefinition.TypeDefinition;
        var requiredItemType = requiredTypeDefinition.Type;
        var foreignKeyValue = _associatedEndPointID.ObjectID;

        _cachedDomainObjects = _dataContainerMap
            .Where(dc => !dc.State.IsDiscarded)
            .Where(dc => dc.HasDomainObject)
            .Where(dc => requiredItemType.IsAssignableFrom(dc.DomainObjectType))
#if DEBUG
            // Only in debug builds for performance reasons. The .NET type comparison is sufficient for filtering the types until interface support is added.
            .Where(dc => requiredTypeDefinition.IsAssignableFrom(dc.ClassDefinition))
#endif
            .Where(dc => (ObjectID?)dc.GetValueWithoutEvents(foreignKeyPropertyDefinition, _valueAccess) == foreignKeyValue)
            .ToDictionary(dc => dc.ID, dc => dc.DomainObject);
      }

      return _cachedDomainObjects;
    }

    [JetBrains.Annotations.NotNull]
    private IEnumerable<DomainObject> GetCachedDomainObjectsSorted ()
    {
      var comparer = GetDomainObjectComparer();
      return GetCachedDomainObjects().Values.OrderBy(obj => obj, comparer);
    }

    private IComparer<DomainObject> GetDomainObjectComparer ()
    {
      var idComparer = new DelegateBasedComparer<DomainObject>((left, right) =>
      {
        Assertion.DebugIsNotNull(left, "left != null");
        Assertion.DebugIsNotNull(right, "right != null");

        return left.ID.CompareTo(right.ID);
      });

      var sortExpression = ((VirtualCollectionRelationEndPointDefinition)_associatedEndPointID.Definition).GetSortExpression();
      if (sortExpression != null)
      {
        var propertyComparer = SortedPropertyComparer.CreateCompoundComparer(sortExpression.SortedProperties, _dataContainerMap, _valueAccess);
        return new CompoundComparer<DomainObject>(propertyComparer, idComparer);
      }
      else
      {
        return idComparer;
      }
    }

    #region Serialization

    private VirtualCollectionData (FlattenedDeserializationInfo info)
    {
      _associatedEndPointID = info.GetValueForHandle<RelationEndPointID>();
      _dataContainerMap = info.GetValueForHandle<IDataContainerMapReadOnlyView>();
      _valueAccess = (ValueAccess)info.GetIntValue();
      var hasCachedDomainObjects = info.GetBoolValue();
      if (hasCachedDomainObjects)
      {
        var cachedDomainObjects = new List<DomainObject>();
        info.FillCollection(cachedDomainObjects);
        _cachedDomainObjects = cachedDomainObjects.ToDictionary(obj => obj.ID);
      }
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle(_associatedEndPointID);
      info.AddHandle(_dataContainerMap);
      info.AddIntValue((int)_valueAccess);
      var hasCachedDomainObjects = _cachedDomainObjects != null;
      info.AddBoolValue(hasCachedDomainObjects);
      if (hasCachedDomainObjects)
        info.AddCollection((ICollection<DomainObject>)_cachedDomainObjects!.Values);
    }

    #endregion
  }
}
