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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Implements the persistence of a subtransaction. Data is loaded via and persisted into the parent transaction.
  /// </summary>
  [Serializable]
  public class SubPersistenceStrategy : IPersistenceStrategy
  {
    private readonly IParentTransactionContext _parentTransactionContext;

    public SubPersistenceStrategy (IParentTransactionContext parentTransactionContext)
    {
      ArgumentUtility.CheckNotNull ("parentTransactionContext", parentTransactionContext);
      _parentTransactionContext = parentTransactionContext;
    }

    public IParentTransactionContext ParentTransactionContext
    {
      get { return _parentTransactionContext; }
    }

    public virtual ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      return _parentTransactionContext.CreateNewObjectID (classDefinition);
    }

    public virtual ILoadedObjectData LoadObjectData (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      // In theory, this might return invalid objects (in practice we won't be called with invalid IDs). 
      // TransferParentObject called by GetLoadedObjectDataForParentObject below will indirectly throw on invalid IDs.
      var parentObject = Tuple.Create (id, _parentTransactionContext.TryGetObject (id));

      return GetLoadedObjectDataForParentObject (parentObject);
    }

    public virtual IEnumerable<ILoadedObjectData> LoadObjectData (IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      var objectIDsAsCollection = objectIDs.ConvertToCollection();

      // In theory, this might return invalid objects (in practice we won't be called with invalid IDs). 
      // TransferParentObject called by GetLoadedObjectDataForParentObject below will throw on invalid IDs.
      var parentObjects = objectIDsAsCollection.Zip (_parentTransactionContext.TryGetObjects (objectIDsAsCollection));

      return parentObjects.Select (GetLoadedObjectDataForParentObject);
    }

    public virtual ILoadedObjectData ResolveObjectRelationData (
        RelationEndPointID relationEndPointID,
        ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      ArgumentUtility.CheckNotNull ("alreadyLoadedObjectDataProvider", alreadyLoadedObjectDataProvider);

      if (!relationEndPointID.Definition.IsVirtual || relationEndPointID.Definition.Cardinality != CardinalityType.One)
        throw new ArgumentException ("ResolveObjectRelationData can only be called for virtual object end points.", "relationEndPointID");

      // parentRelatedObject may be null
      var parentRelatedObject = _parentTransactionContext.ResolveRelatedObject (relationEndPointID);
      return TransferParentObject (parentRelatedObject, alreadyLoadedObjectDataProvider);
    }

    public virtual IEnumerable<ILoadedObjectData> ResolveCollectionRelationData (
        RelationEndPointID relationEndPointID,
        ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      ArgumentUtility.CheckNotNull ("alreadyLoadedObjectDataProvider", alreadyLoadedObjectDataProvider);

      if (relationEndPointID.Definition.Cardinality != CardinalityType.Many)
        throw new ArgumentException ("ResolveCollectionRelationData can only be called for CollectionEndPoints.", "relationEndPointID");
      
      var parentObjects = _parentTransactionContext.ResolveRelatedObjects (relationEndPointID);
      return parentObjects
          .Select (parentObject =>
          {
            Assertion.IsNotNull (parentObject);
            return TransferParentObject (parentObject, alreadyLoadedObjectDataProvider);
          });
    }

    public virtual IEnumerable<ILoadedObjectData> ExecuteCollectionQuery (IQuery query, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull ("query", query);
      ArgumentUtility.CheckNotNull ("alreadyLoadedObjectDataProvider", alreadyLoadedObjectDataProvider);

      var queryResult = _parentTransactionContext.ExecuteCollectionQuery (query);
      Assertion.IsNotNull (queryResult, "Parent transaction never returns a null query result for collection query.");

      var parentObjects = queryResult.AsEnumerable ();

      return parentObjects.Select (parentObject => TransferParentObject (parentObject, alreadyLoadedObjectDataProvider));
    }

    public IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return _parentTransactionContext.ExecuteCustomQuery (query);
    }

    public virtual object ExecuteScalarQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return _parentTransactionContext.ExecuteScalarQuery (query);
    }

    public virtual void PersistData (IEnumerable<PersistableData> data)
    {
      ArgumentUtility.CheckNotNull ("data", data);

      var dataAsCollection = data.ConvertToCollection();

      var dataContainersByState = dataAsCollection.Select (item => item.DataContainer).ToLookup (dc => dc.State);

      // only handle changed end-points; end-points of new and deleted objects will implicitly be handled by PersistDataContainers
      var endPoints = dataAsCollection.SelectMany (item => item.GetAssociatedEndPoints ()).Where (ep => ep.HasChanged);

      // The parent transaction needs to be unlocked for modifications to be allowed
      using (var unlockedParentTransactionContext = _parentTransactionContext.UnlockParentTransaction ())
      {
        // Handle new DataContainers _before_ end-point changes - that way we can be sure that all new end-points have already been registered
        PersistDataContainers (dataContainersByState[StateType.New], unlockedParentTransactionContext);
        PersistDataContainers (dataContainersByState[StateType.Changed], unlockedParentTransactionContext);

        PersistRelationEndPoints (endPoints);

        // Handle deleted DataContainers _after_ end-point changes - that way we can be sure that all end-points have already been decoupled 
        // (i.e., been set to empty)
        PersistDataContainers (dataContainersByState[StateType.Deleted], unlockedParentTransactionContext);
      }
    }

    private ILoadedObjectData GetLoadedObjectDataForParentObject (Tuple<ObjectID, DomainObject> parentObject)
    {
      if (parentObject.Item2 == null)
        return new NotFoundLoadedObjectData (parentObject.Item1);
      else
        return TransferParentObject (parentObject.Item1);
    }

    private ILoadedObjectData TransferParentObject (DomainObject parentObject, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      if (parentObject == null)
        return new NullLoadedObjectData ();

      var existingLoadedObject = alreadyLoadedObjectDataProvider.GetLoadedObject (parentObject.ID);
      if (existingLoadedObject != null)
        return existingLoadedObject;
      else
        return TransferParentObject (parentObject.ID);
    }

    private FreshlyLoadedObjectData TransferParentObject (ObjectID objectID)
    {
      // This will throw if objectID is invalid in the parent transaction, which is just what we want - we can't transfer invalid objects.
      var parentDataContainer = _parentTransactionContext.GetDataContainerWithLazyLoad (objectID, throwOnNotFound: true);
      Assertion.IsNotNull (parentDataContainer);
      var dataContainer = TransferParentContainer (parentDataContainer);
      return new FreshlyLoadedObjectData (dataContainer);
    }

    private DataContainer TransferParentContainer (DataContainer parentDataContainer)
    {
      if (parentDataContainer.State == StateType.Deleted)
      {
        var message = string.Format ("Object '{0}' is already deleted in the parent transaction.", parentDataContainer.ID);
        throw new ObjectDeletedException (message, parentDataContainer.ID);
      }

      var thisDataContainer = DataContainer.CreateForExisting (
          parentDataContainer.ID,
          parentDataContainer.Timestamp,
          pd => parentDataContainer.GetValueWithoutEvents (pd, ValueAccess.Current));

      Assertion.IsTrue (thisDataContainer.State == StateType.Unchanged);
      return thisDataContainer;
    }

    private void PersistDataContainers (IEnumerable<DataContainer> dataContainers, IUnlockedParentTransactionContext unlockedParentTransactionContext)
    {
      foreach (var dataContainer in dataContainers)
      {
        Assertion.IsFalse (
            dataContainer.IsDiscarded,
            "dataContainers cannot contain discarded DataContainers, because its items come"
            + "from DataManager.DataContainerMap, which does not contain discarded containers");
        Assertion.IsTrue (dataContainer.State != StateType.Unchanged, "dataContainers cannot contain an unchanged container");
        Assertion.IsTrue (dataContainer.State != StateType.NotLoadedYet, "dataContainers cannot contain an unloaded container");
        Assertion.IsTrue (
            dataContainer.State == StateType.New || dataContainer.State == StateType.Changed
            || dataContainer.State == StateType.Deleted,
            "Invalid dataContainer.State: " + dataContainer.State);

        switch (dataContainer.State)
        {
          case StateType.New:
            PersistNewDataContainer (dataContainer, unlockedParentTransactionContext);
            break;
          case StateType.Changed:
            PersistChangedDataContainer (dataContainer);
            break;
          case StateType.Deleted:
            PersistDeletedDataContainer (dataContainer, unlockedParentTransactionContext);
            break;
        }
      }
    }

    private void PersistNewDataContainer (DataContainer dataContainer, IUnlockedParentTransactionContext unlockedParentTransactionContext)
    {
      Assertion.IsTrue (_parentTransactionContext.IsInvalid (dataContainer.ID));
      unlockedParentTransactionContext.MarkNotInvalid (dataContainer.ID);

      Assertion.IsNull (
          _parentTransactionContext.GetDataContainerWithoutLoading (dataContainer.ID), 
          "a new data container cannot be known to the parent");
      Assertion.IsFalse (dataContainer.IsDiscarded);

      var parentDataContainer = DataContainer.CreateNew (dataContainer.ID);
      parentDataContainer.SetDomainObject (dataContainer.DomainObject);

      parentDataContainer.SetPropertyDataFromSubTransaction (dataContainer);

      Assertion.IsFalse (dataContainer.HasBeenMarkedChanged);
      Assertion.IsNull (dataContainer.Timestamp);

      unlockedParentTransactionContext.RegisterDataContainer (parentDataContainer);
    }

    private void PersistChangedDataContainer (DataContainer dataContainer)
    {
      DataContainer parentDataContainer = _parentTransactionContext.GetDataContainerWithoutLoading (dataContainer.ID);
      Assertion.IsNotNull (
          parentDataContainer,
          "a changed DataContainer must have been loaded through ParentTransaction, so the "
          + "ParentTransaction must know it");
      Assertion.IsFalse (parentDataContainer.IsDiscarded, "a changed DataContainer cannot be discarded in the ParentTransaction");
      Assertion.IsTrue (parentDataContainer.State != StateType.Deleted, "a changed DataContainer cannot be deleted in the ParentTransaction");
      Assertion.IsTrue (parentDataContainer.DomainObject == dataContainer.DomainObject, "invariant");

      parentDataContainer.SetTimestamp (dataContainer.Timestamp);
      parentDataContainer.SetPropertyDataFromSubTransaction (dataContainer);

      if (dataContainer.HasBeenMarkedChanged && (parentDataContainer.State == StateType.Unchanged || parentDataContainer.State == StateType.Changed))
        parentDataContainer.MarkAsChanged ();
    }

    private void PersistDeletedDataContainer (DataContainer dataContainer, IUnlockedParentTransactionContext unlockedParentTransactionContext)
    {
      var parentDataContainer = _parentTransactionContext.GetDataContainerWithoutLoading (dataContainer.ID);
      Assertion.IsNotNull (
          parentDataContainer,
          "a deleted DataContainer must have been loaded through ParentTransaction, so the ParentTransaction must know it");

      Assertion.IsTrue (
          parentDataContainer.State != StateType.Invalid && parentDataContainer.State != StateType.Deleted,
          "deleted DataContainers cannot be discarded or deleted in the ParentTransaction");
      Assertion.IsTrue (parentDataContainer.DomainObject == dataContainer.DomainObject, "invariant");

      if (parentDataContainer.State == StateType.New)
        unlockedParentTransactionContext.Discard (parentDataContainer);
      else
        parentDataContainer.Delete();
    }

    private void PersistRelationEndPoints (IEnumerable<IRelationEndPoint> endPoints)
    {
      foreach (var endPoint in endPoints)
      {
        var parentEndPoint = _parentTransactionContext.GetRelationEndPointWithoutLoading (endPoint.ID);

        // Because the DataContainers are processed before the RelationEndPoints, the RelationEndPointMaps of both parent and child transaction now
        // contain end points for the same end point IDs. The only scenario in which the ParentTransaction doesn't know an end point known
        // to the child transaction is when the object was of state New in the ParentTransaction and its DataContainer was just discarded.
        // Therefore, we can safely ignore end points unknown to the parent transaction.

        if (parentEndPoint != null)
          parentEndPoint.SetDataFromSubTransaction (endPoint);
      }
    }
  }
}