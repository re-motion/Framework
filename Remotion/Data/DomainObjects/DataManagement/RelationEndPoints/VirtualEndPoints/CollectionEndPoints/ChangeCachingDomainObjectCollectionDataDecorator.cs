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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Decorates another <see cref="IDomainObjectCollectionData"/> object and manages change state, keeping around a (lazily created) copy of the 
  /// original data until <see cref="Commit"/> or <see cref="Rollback"/> are called. The change state is cached, and the cache is invalidated by 
  /// modifying operations. An change state invalidation also triggers a notification via <see cref="IVirtualEndPointStateUpdateListener"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The <see cref="OriginalData"/> is a <see cref="CopyOnWriteDomainObjectDomainObjectCollectionData"/> and is exposed only through a read-only wrapper. 
  /// As a  result, the  <see cref="ChangeCachingDomainObjectCollectionDataDecorator"/> class is the only class that can change that original data.
  /// </para>
  /// <para>
  /// There are a few operations (e.g., <see cref="SortOriginalAndCurrent"/>) that do not invalidate the change state of this collection (unless the
  /// collection was already changed before) because they affect both the current and original data of this 
  /// <see cref="ChangeCachingDomainObjectCollectionDataDecorator"/>.
  /// </para>
  /// </remarks>
  public class ChangeCachingDomainObjectCollectionDataDecorator : DomainObjectCollectionDataDecoratorBase
  {
    // Used for operations causing _originalData to CopyOnWrite and WrappedData_CollectionChanged to raise _stateUpdateListener events
    private readonly ObservableDomainObjectCollectionDataDecorator _observedWrappedData;
    // Used for operations private to this class that want to change this collection and _originalData in the same way.
    // Using this property circumvents _stateUpdateListener events and _originalData.CopyOnWrite.
    private readonly IDomainObjectCollectionData _unobservedWrappedData;

    private readonly CopyOnWriteDomainObjectDomainObjectCollectionData _originalData;

    private bool _isCacheUpToDate;
    private bool _cachedHasChangedFlag;

    public ChangeCachingDomainObjectCollectionDataDecorator (IDomainObjectCollectionData wrappedData)
      : base(new ObservableDomainObjectCollectionDataDecorator(ArgumentUtility.CheckNotNull("wrappedData", wrappedData)))
    {
      _observedWrappedData = (ObservableDomainObjectCollectionDataDecorator)WrappedData;
      _unobservedWrappedData = wrappedData;

      _originalData = new CopyOnWriteDomainObjectDomainObjectCollectionData(_observedWrappedData);
      _observedWrappedData.CollectionChanged += WrappedData_CollectionChanged();

      _isCacheUpToDate = true;
      _cachedHasChangedFlag = false;
    }

    public ReadOnlyDomainObjectCollectionDataDecorator OriginalData
    {
      get { return new ReadOnlyDomainObjectCollectionDataDecorator(_originalData); }
    }

    public bool IsCacheUpToDate
    {
      get { return _isCacheUpToDate; }
    }

    public bool HasChanged (IDomainObjectCollectionEndPointChangeDetectionStrategy strategy)
    {
      if (!_isCacheUpToDate)
      {
        bool hasChanged = CalculateHasChangedFlag(strategy);
        SetCachedHasChangedFlag(hasChanged);
      }

      return _cachedHasChangedFlag;
    }

    private bool CalculateHasChangedFlag (IDomainObjectCollectionEndPointChangeDetectionStrategy strategy)
    {
      // If the original data still points to this collection, we don't ask the strategy - we know we haven't changed.
      if (!_originalData.IsContentsCopied)
        return false;

      // If the original data has a different number of items, we don't ask the strategy - we know we have changed.
      if (_originalData.Count != Count)
        return true;

      return strategy.HasDataChanged(this, OriginalData);
    }

    public void Commit ()
    {
      _originalData.RevertToCopiedData();
      SetCachedHasChangedFlag(false);
    }

    public void Rollback ()
    {
      this.ReplaceContents(_originalData);
      _originalData.RevertToCopiedData();
      SetCachedHasChangedFlag(false);
    }

    /// <summary>
    /// Registers the given <paramref name="domainObject"/> as an original item of this collection. This means the item is added to the 
    /// <see cref="OriginalData"/> collection, and it is also added to this <see cref="ChangeCachingDomainObjectCollectionDataDecorator"/> collection. If the 
    /// <see cref="OriginalData"/> collection already contains the item, an exception is thrown. If this collection already contains the item, it is
    /// only added to the <see cref="OriginalData"/>. This operation may invalidate the state cache.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> to be registered.</param>
    public void RegisterOriginalItem (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      // Original collection must not contain this item
      if (_originalData.ContainsObjectID(domainObject.ID))
      {
        var message = string.Format("The original collection already contains a domain object with ID '{0}'.", domainObject.ID);
        throw new InvalidOperationException(message);
      }

      // Check if this collection does not contain the item
      if (!_unobservedWrappedData.ContainsObjectID(domainObject.ID))
      {
        // Standard case: Neither collection contains the item; the item is added to both, and the state cache stays valid

        // Add the item to the unobserved inner collection to avoid copy on write: if the contents hasn't been copied, we want to modify both 
        // collections at the same time!
        // This way, if the original collection has not yet been copied, it will automatically contain the  item and the state cache remains valid.
        _unobservedWrappedData.Add(domainObject);

        // If the original collection has already been copied, we must add the item manually. The state cache still remains valid because we always add
        // the item at the end. If the collections were equal before, they remain equal now. If they were different before, they remain different.
        if (_originalData.IsContentsCopied)
          _originalData.Add(domainObject);
      }
      else
      {
        // Special case: The current collection already contains the item

        // We must add the item to the original collection only and raise a potential state change notification
        _originalData.Add(domainObject);
        OnChangeStateUnclear();
      }

      Assertion.IsTrue(ContainsObjectID(domainObject.ID));
      Assertion.IsTrue(_originalData.ContainsObjectID(domainObject.ID));
    }

    /// <summary>
    /// Unregisters the item with the given <paramref name="objectID"/> as an original item of this collection. This means the item is removed from 
    /// the <see cref="OriginalData"/> collection, and it is also removed from this <see cref="ChangeCachingDomainObjectCollectionDataDecorator"/> collection. If 
    /// the <see cref="OriginalData"/> collection does not contain the item, an exception is thrown. If this collection does not contain the item, it 
    /// is only removed from the <see cref="OriginalData"/>. This operation may invalidate the state cache.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> to be unregistered.</param>
    public void UnregisterOriginalItem (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      // Original collection must contain this item
      if (!_originalData.ContainsObjectID(objectID))
      {
        var message = string.Format("The original collection does not contain a domain object with ID '{0}'.", objectID);
        throw new InvalidOperationException(message);
      }

      // Check if this collection contains the item
      if (ContainsObjectID(objectID))
      {
        // Standard case: Both collections contain the item; the item is removed from both.

        // Remove the item from the unobserved inner collection to avoid copy on write: if the contents hasn't been copied, we want to modify both 
        // collections at the same time!
        // This way, if the original collection has not yet been copied, it will automatically not contain the item and the state cache remains valid.
        _unobservedWrappedData.Remove(objectID);

        // If the original collection has already been copied, we must remove the item manually and invalidate the state cache: Collections previously 
        // different because the item was in different places might now be the same.
        if (_originalData.IsContentsCopied)
        {
          _originalData.Remove(objectID);
          OnChangeStateUnclear();
        }
      }
      else
      {
        // Special case: The current collection does not contain the item

        // We must remove the item from the original collection only and raise a potential state change notification
        _originalData.Remove(objectID);
        OnChangeStateUnclear();
      }
    }

    /// <summary>
    /// Sorts the data in this <see cref="ChangeCachingDomainObjectCollectionDataDecorator"/> and the data in the <see cref="OriginalData"/> collection
    /// using the given <paramref name="comparison"/>. This operation causes the change state to be invalidated if the original data is not the same
    /// as the current data.
    /// </summary>
    /// <param name="comparison"></param>
    public void SortOriginalAndCurrent (Comparison<DomainObject> comparison)
    {
      ArgumentUtility.CheckNotNull("comparison", comparison);

      // Sort the unobserved inner collection to avoid copy on write: if the contents hasn't been copied, we want to sort both 
      // collections at the same time!
      _unobservedWrappedData.Sort(comparison);

      // If the original collection has already been copied, we must sort it manually. This might cause the change state cache to be wrong, so it is 
      // invalidated (and a notification raised).
      if (_originalData.IsContentsCopied)
      {
        _originalData.Sort(comparison);
        OnChangeStateUnclear();
      }
    }

    private EventHandler<ObservableDomainObjectCollectionDataDecorator.DataChangeEventArgs> WrappedData_CollectionChanged ()
    {
      return (sender, args) => OnChangeStateUnclear();
    }

    private void OnChangeStateUnclear ()
    {
      _isCacheUpToDate = false;
    }

    private void SetCachedHasChangedFlag (bool hasChanged)
    {
      _cachedHasChangedFlag = hasChanged;
      _isCacheUpToDate = true;
    }
  }
}
