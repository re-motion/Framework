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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Implements <see cref="IClientTransactionListener"/> in order to cache the values of <see cref="DomainObject.State"/>.
  /// </summary>
  [Serializable]
  public class DomainObjectStateCache
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly Dictionary<ObjectID, DomainObjectState> _stateCache = new Dictionary<ObjectID, DomainObjectState>();

    [Serializable]
    private class StateUpdateListener : ClientTransactionListenerBase
    {
      private readonly DomainObjectStateCache _cache;

      public StateUpdateListener (DomainObjectStateCache cache)
      {
        ArgumentUtility.CheckNotNull("cache", cache);
        _cache = cache;
      }

      public override void DataContainerStateUpdated (ClientTransaction clientTransaction, DataContainer dataContainer, DataContainerState newDataContainerState)
      {
        _cache.HandleStateUpdate(dataContainer.ID);
      }

      public override void VirtualRelationEndPointStateUpdated (ClientTransaction clientTransaction, RelationEndPointID endPointID, bool? newEndPointChangeState)
      {
        Assertion.DebugIsNotNull(endPointID.ObjectID, "endPointID.ObjectID != null");
        _cache.HandleStateUpdate(endPointID.ObjectID);
      }

      public override void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
      {
        _cache.HandleStateUpdate(container.ID);
      }

      public override void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container)
      {
        _cache.HandleStateUpdate(container.ID);
      }

      public override void ObjectMarkedInvalid (ClientTransaction clientTransaction, IDomainObject domainObject)
      {
        _cache.HandleStateUpdate(domainObject.ID);
      }

      public override void ObjectMarkedNotInvalid (ClientTransaction clientTransaction, IDomainObject domainObject)
      {
        _cache.HandleStateUpdate(domainObject.ID);
      }
    }

    public DomainObjectStateCache (ClientTransaction clientTransaction)
    {
      _clientTransaction = clientTransaction;
      _clientTransaction.AddListener(new StateUpdateListener(this));
    }

    /// <summary>
    /// Gets the <see cref="DomainObjectState"/> for the <see cref="DomainObject"/> with the given <see cref="ObjectID"/> from the cache; recalculating 
    /// it if the cache does not have an up-to-date <see cref="DomainObjectState"/> value.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> whose state to get.</param>
    /// <returns>
    /// The state of the <see cref="DomainObject"/> with the given <see cref="ObjectID"/>. If no such object has been loaded, 
    /// the <see cref="DomainObjectState.IsNotLoadedYet"/> flag is set on the returned <see cref="DomainObjectState"/>.
    /// </returns>
    public DomainObjectState GetState (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      DomainObjectState state;
      if (_stateCache.TryGetValue(objectID, out state))
        return state;

      state = CalculateState(objectID);
      _stateCache.Add(objectID, state);
      return state;
    }

    private void HandleStateUpdate (ObjectID objectID)
    {
      _stateCache.Remove(objectID);
    }

    private DomainObjectState CalculateState (ObjectID objectID)
    {
      var stateBuilder = new DomainObjectState.Builder();

      if (_clientTransaction.IsInvalid(objectID))
        return stateBuilder.SetInvalid().Value;

      var dataContainer = _clientTransaction.DataManager.DataContainers[objectID];
      if (dataContainer == null)
      {
        if (HasRelationChangedForNotLoadedYetDataContainer(objectID))
          stateBuilder = stateBuilder.SetRelationChanged();

        if (IsNewInHierarchyForNotLoadedYetDataContainer(objectID))
          stateBuilder = stateBuilder.SetNewInHierarchy();

        return stateBuilder.SetNotLoadedYet().Value;
      }

      var dataContainerState = dataContainer.State;

      if (dataContainerState.IsDiscarded)
        throw new InvalidOperationException($"DataContainer for object '{objectID}' has been discarded without removing the instance from the DataManager.");

      if (dataContainerState.IsNewInHierarchy)
        stateBuilder = stateBuilder.SetNewInHierarchy();

      var hasRelationChanged = HasRelationChanged(dataContainer);
      if (hasRelationChanged)
        stateBuilder = stateBuilder.SetRelationChanged();

      if (dataContainerState.IsUnchanged)
        return hasRelationChanged ? stateBuilder.SetChanged().Value : stateBuilder.SetUnchanged().Value;

      if (dataContainerState.IsPersistentDataChanged)
        stateBuilder = stateBuilder.SetPersistentDataChanged();

      if (dataContainerState.IsNonPersistentDataChanged)
        stateBuilder = stateBuilder.SetNonPersistentDataChanged();

      if (dataContainerState.IsChanged)
        return stateBuilder.SetChanged().Value;

      if (dataContainerState.IsNew)
        return stateBuilder.SetNew().Value;

      if (dataContainerState.IsDeleted)
        return stateBuilder.SetDeleted().Value;

      throw new InvalidOperationException($"DomainObjectState for '{objectID}' cannot be calculated.");
    }

    private bool HasRelationChanged (DataContainer dataContainer)
    {
      return dataContainer.AssociatedRelationEndPointIDs
          .Select(id => _clientTransaction.DataManager.GetRelationEndPointWithoutLoading(id))
          .Any(endPoint => endPoint != null && endPoint.HasChanged);
    }

    private bool HasRelationChangedForNotLoadedYetDataContainer (ObjectID objectID)
    {
      return RelationEndPointID.GetAllRelationEndPointIDs(objectID)
          .Select(id => _clientTransaction.DataManager.GetRelationEndPointWithoutLoading(id))
          .Any(endPoint => endPoint != null && endPoint.HasChanged);
    }

    private bool IsNewInHierarchyForNotLoadedYetDataContainer (ObjectID objectID)
    {
      var rootTransaction = _clientTransaction.RootTransaction;
      if (rootTransaction == _clientTransaction)
      {
        // Performance optimization for special case: when the _clientTransaction is the rootTransaction, no additional lookup is needed.
        Assertion.DebugAssert(rootTransaction.DataManager.DataContainers[objectID] == null, "rootTransaction.DataManager.DataContainers[objectID] == null");
        return false;
      }

      var dataContainerInRootTransaction = rootTransaction.DataManager.DataContainers[objectID];
      if (dataContainerInRootTransaction != null)
      {
        // Performance optimization for existing objects: when the object is loaded, it always exists in the root transaction
        // and can thus supply the correct DataContainer.State for both existing and new objects.
        return dataContainerInRootTransaction.State.IsNewInHierarchy;
      }

      for (var parentTransaction = _clientTransaction.ParentTransaction;
           parentTransaction != rootTransaction;
           parentTransaction = parentTransaction.ParentTransaction)
      {
        Assertion.DebugIsNotNull(parentTransaction, "parentTransaction != null when iteration is aborted on rootTransaction");

        var dataContainerInParentTransaction = parentTransaction.DataManager.DataContainers[objectID];
        if (dataContainerInParentTransaction != null && dataContainerInParentTransaction.State.IsNewInHierarchy)
          return true;
      }

      return false;
    }
  }

  // Domain object state changes and how they are handled by this cache:
  //
  // TODO 3338: This table must be reevaluated with extended Unload.
  // TODO 3339: This table must be updated when more state combinations are added.
  //
  // Object freshly enlisted => NotLoadedYet: at the beginning, there is no cached state
  //
  // NotLoadedYet currently requires all end-points to be unchanged.
  // NotLoadedYet => NotLoadedYet: [don't care]
  // NotLoadedYet => Unchanged: DataContainerMapRegistering
  // NotLoadedYet => Changed (end-point): VirtualRelationEndPointStateUpdated (not practically possible at the moment)
  // NotLoadedYet => Changed (data): DataContainerMapRegistering (Changed (data) state requires a registered DataContainer)
  // NotLoadedYet => Deleted: DataContainerMapRegistering (Deleted state requires a registered DataContainer)
  // NotLoadedYet => New: DataContainerMapRegistering (New state requires a registered DataContainer)
  // NotLoadedYet => Invalid: ObjectMarkedInvalid
  //
  // Unchanged currently requires DataContainer to be loaded.
  // Unchanged => NotLoadedYet: DataContainerMapUnregistering
  // Unchanged => Unchanged: [don't care]
  // Unchanged => Changed (end-point): VirtualRelationEndPointStateUpdated
  // Unchanged => Changed (data): DataContainerStateUpdated
  // Unchanged => Deleted: DataContainerStateUpdated
  // Unchanged => New: (not possible)
  // Unchanged => Invalid: DataContainerMapUnregistering, ObjectMarkedInvalid
  //
  // Changed (end-point) currently requires DataContainer to be loaded. (Future: TODO 3338)
  // Changed (end-point) => NotLoadedYet: DataContainerMapUnregistering
  // Changed (end-point) => Unchanged: VirtualRelationEndPointStateUpdated
  // Changed (end-point) => Changed (end-point): [don't care]
  // Changed (end-point) => Changed (data): [don't care] (not possible in one step)
  // Changed (end-point) => Deleted: DataContainerStateUpdated
  // Changed (end-point) => New: (not possible)
  // Changed (end-point) => Invalid: DataContainerMapUnregistering, ObjectMarkedInvalid
  //
  // Changed (data) state requires DataContainer to be loaded.
  // Changed (data) => NotLoadedYet: DataContainerMapUnregistering
  // Changed (data) => Unchanged: DataContainerStateUpdated
  // Changed (data) => Changed (end-point): [don't care] (not possible in one step)
  // Changed (data) => Changed (data): [don't care]
  // Changed (data) => Deleted: DataContainerStateUpdated
  // Changed (data) => New: (not possible)
  // Changed (data) => Invalid: DataContainerMapUnregistering, ObjectMarkedInvalid
  //
  // Deleted requires DataContainer to be loaded.
  // Deleted => NotLoadedYet: DataContainerMapUnregistering
  // Deleted => Unchanged: DataContainerStateUpdated
  // Deleted => Changed (end-point): (not possible in one step)
  // Deleted => Changed (data): (not possible in one step)
  // Deleted => Deleted: [don't care]
  // Deleted => New: (not possible)
  // Deleted => Invalid: DataContainerMapUnregistering, ObjectMarkedInvalid
  //
  // New requires DataContainer to be loaded.
  // New => NotLoadedYet: DataContainerMapUnregistering
  // New => Unchanged: DataContainerStateUpdated
  // New => Changed (end-point): (not possible in one step)
  // New => Changed (data): (not possible in one step)
  // New => Deleted: (not possible in one step)
  // New => New: [don't care]
  // New => Invalid: DataContainerMapUnregistering, ObjectMarkedInvalid
  //
  // Invalid => New: ObjectMarkedNotInvalid, DataContainerMapRegistering (when committing a subtransaction where an object was New)
  // Invalid => NotLoadedYet: ObjectMarkedNotInvalid (when resurrecting an object)
  // Invalid => *: (not possible)
}
