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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Represents the state of a <see cref="CollectionEndPoint"/> where not all of its data is available (ie., the end-point has not been (lazily) 
  /// loaded, or it has been unloaded).
  /// </summary>
  public class IncompleteCollectionEndPointLoadState 
      : IncompleteVirtualEndPointLoadStateBase<ICollectionEndPoint, ReadOnlyCollectionDataDecorator, ICollectionEndPointDataManager, ICollectionEndPointLoadState>,
        ICollectionEndPointLoadState
  {
    private readonly ICollectionEndPointDataManagerFactory _dataManagerFactory;

    public IncompleteCollectionEndPointLoadState (
        IEndPointLoader endPointLoader, 
        ICollectionEndPointDataManagerFactory dataManagerFactory)
      : base (endPointLoader)
    {
      ArgumentUtility.CheckNotNull ("dataManagerFactory", dataManagerFactory);
      _dataManagerFactory = dataManagerFactory;
    }

    public ICollectionEndPointDataManagerFactory DataManagerFactory
    {
      get { return _dataManagerFactory; }
    }

    public bool? HasChangedFast ()
    {
      return false;
    }

    public void EnsureDataComplete (ICollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      EndPointLoader.LoadEndPointAndGetNewState (endPoint);
    }

    public new void MarkDataComplete (ICollectionEndPoint collectionEndPoint, IEnumerable<DomainObject> items, Action<ICollectionEndPointDataManager> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("items", items);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      base.MarkDataComplete (collectionEndPoint, items, stateSetter);

      var eventRaiser = collectionEndPoint.GetCollectionEventRaiser();
      eventRaiser.WithinReplaceData();
    }

    public void SortCurrentData (ICollectionEndPoint collectionEndPoint, Comparison<DomainObject> comparison)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("comparison", comparison);

      var completeState = EndPointLoader.LoadEndPointAndGetNewState (collectionEndPoint);
      completeState.SortCurrentData (collectionEndPoint, comparison);
    }

    public IDataManagementCommand CreateSetCollectionCommand (
        ICollectionEndPoint collectionEndPoint, 
        DomainObjectCollection newCollection, 
        ICollectionEndPointCollectionManager collectionEndPointCollectionManager)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("newCollection", newCollection);

      var completeState = EndPointLoader.LoadEndPointAndGetNewState (collectionEndPoint);
      return completeState.CreateSetCollectionCommand (collectionEndPoint, newCollection, collectionEndPointCollectionManager);
    }

    public IDataManagementCommand CreateRemoveCommand (ICollectionEndPoint collectionEndPoint, DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);

      var completeState = EndPointLoader.LoadEndPointAndGetNewState (collectionEndPoint);
      return completeState.CreateRemoveCommand (collectionEndPoint, removedRelatedObject);
    }

    public IDataManagementCommand CreateDeleteCommand (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      
      var completeState = EndPointLoader.LoadEndPointAndGetNewState (collectionEndPoint);
      return completeState.CreateDeleteCommand (collectionEndPoint);
    }

    public IDataManagementCommand CreateInsertCommand (ICollectionEndPoint collectionEndPoint, DomainObject insertedRelatedObject, int index)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("insertedRelatedObject", insertedRelatedObject);

      var completeState = EndPointLoader.LoadEndPointAndGetNewState (collectionEndPoint);
      return completeState.CreateInsertCommand (collectionEndPoint, insertedRelatedObject, index);
    }

    public IDataManagementCommand CreateAddCommand (ICollectionEndPoint collectionEndPoint, DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("addedRelatedObject", addedRelatedObject);
      
      var completeState = EndPointLoader.LoadEndPointAndGetNewState (collectionEndPoint);
      return completeState.CreateAddCommand (collectionEndPoint, addedRelatedObject);
    }

    public IDataManagementCommand CreateReplaceCommand (ICollectionEndPoint collectionEndPoint, int index, DomainObject replacementObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("replacementObject", replacementObject);

      var completeState = EndPointLoader.LoadEndPointAndGetNewState (collectionEndPoint);
      return completeState.CreateReplaceCommand (collectionEndPoint, index, replacementObject);
    }

    protected override ICollectionEndPointDataManager CreateEndPointDataManager (ICollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      return _dataManagerFactory.CreateEndPointDataManager (endPoint.ID);
    }

    #region Serialization

    public IncompleteCollectionEndPointLoadState (FlattenedDeserializationInfo info)
        : base (info)
    {
      _dataManagerFactory = info.GetValueForHandle<ICollectionEndPointDataManagerFactory>();
    }

    protected override void SerializeSubclassData (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddHandle (_dataManagerFactory);
    }

    #endregion
  }
}